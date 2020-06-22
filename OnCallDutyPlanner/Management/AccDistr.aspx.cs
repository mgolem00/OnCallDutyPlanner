using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OnCallDutyPlanner
{
    public partial class AccDistr : System.Web.UI.Page
    {
        class AccountDistribution
        {
            public int id { get; set; }
            public int teamID { get; set; }
        }

        class AccountDistributionHistory
        {
            //pretvori accId i totalHours u Liste pa u GetAccountDistributionHistory napravi provjeru ako je isti userID da samo upise u listu?
            public int accountID { get; set; }
            public string userID { get; set; }
            public int totalHours { get; set; }
        }

        class Account
        {
            public int ID { get; set; }
            public string accountNumber { get; set; }
            public string projectName { get; set; }
        }
        
        private List<AccountDistributionHistory> GetAccountDistributionHistory(int accDistrID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                List<AccountDistributionHistory> accDistrHistory = new List<AccountDistributionHistory>();
                var queryString = "SELECT UserID, AccountID, TotalHours FROM AccountDistributionHistory WHERE AccountDistributionID = @accDistrID";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@accDistrID", accDistrID);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            accDistrHistory.Add(new AccountDistributionHistory() { userID = reader.GetString(0), accountID = reader.GetInt32(1), totalHours = reader.GetInt32(2) });
                        }
                    }
                    reader.Close();
                    connection.Close();
                    return accDistrHistory;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        private List<AccountDistribution> GetAllAccountDistributionInPeriod(string workPeriod)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                List<AccountDistribution> accDistr = new List<AccountDistribution>();
                var queryString = "SELECT ID, SLATeamID FROM AccountDistribution WHERE WorkPeriod = @workPeriod";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@workPeriod", workPeriod);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            accDistr.Add(new AccountDistribution {id=reader.GetInt32(0), teamID=reader.GetInt32(1)});
                        }
                    }
                    reader.Close();
                    connection.Close();
                    return accDistr;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        private List<Account> GetAllAccounts()
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                List<Account> accounts = new List<Account>();
                var queryString = "SELECT ID, AccountNumber, ProjectName FROM Accounts";
                SqlCommand command = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            accounts.Add(new Account { ID = reader.GetInt32(0), accountNumber = reader.GetString(1), projectName = reader.GetString(2) });
                        }
                    }
                    reader.Close();
                    connection.Close();
                    return accounts;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        private string GetTeamName(int teamID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string teamName;
                var queryString = "SELECT TeamName FROM SLATeams WHERE ID = @userTeamID";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@userTeamID", teamID);

                try
                {
                    connection.Open();
                    teamName = (String)command.ExecuteScalar();
                    connection.Close();
                    return teamName;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        private void ListAccountDistributions()
        {
            var userStore = new UserStore<IdentityUser>();
            var manager = new UserManager<IdentityUser>(userStore);
            string workPeriod = "2020-01-01";
            List<AccountDistribution> accDistr = GetAllAccountDistributionInPeriod(workPeriod);
            
            if(accDistr.Count != 0)
            {
                foreach (AccountDistribution accountDistribution in accDistr)
                {
                    DataTable dt = new DataTable("AccountDistributionHistory");
                    DataRow dr;

                    dt.Columns.Add("TeamMembers");
                    string teamName = GetTeamName(accountDistribution.teamID);
                    List<AccountDistributionHistory> accDistHistory = GetAccountDistributionHistory(accountDistribution.id);
                    List<Account> allAccounts = GetAllAccounts();

                    string userNameCheck = "";
                    foreach (AccountDistributionHistory accDistHist in accDistHistory)
                    {
                        var user = manager.FindById(accDistHist.userID);
                        if (userNameCheck != user.UserName)
                        {
                            dr = dt.NewRow();
                            dr["TeamMembers"] = user.UserName;
                            dt.Rows.Add(dr);
                            userNameCheck = user.UserName;
                        }
                    }

                    foreach (Account account in allAccounts)
                    {
                        dt.Columns.Add(account.accountNumber);
                    }

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        foreach(AccountDistributionHistory accDistHist in accDistHistory)
                        {
                            var user = manager.FindById(accDistHist.userID);
                            if(user.UserName == dt.Rows[i][0].ToString())
                            {
                                foreach (Account account in allAccounts)
                                {
                                    if (accDistHist.accountID == account.ID)
                                    {
                                        dt.Rows[i][account.accountNumber] = accDistHist.totalHours.ToString();
                                    }
                                }
                            }
                        }
                    }

                    //count total hours per row
                    dt.Columns.Add("Total");
                    for(int i = 0; i < dt.Rows.Count; i++)
                    {
                        int total = 0;
                        for(int j = 1; j < dt.Columns.Count; j++)
                        {
                            if(!string.IsNullOrWhiteSpace(dt.Rows[i][j].ToString()))
                            {
                                string temp;
                                temp = dt.Rows[i][j].ToString();
                                total += Int32.Parse(temp);
                            }
                        }
                        dt.Rows[i]["Total"] = total.ToString();
                    }

                    //count total hours per column
                    dr = dt.NewRow();
                    dr["TeamMembers"] = "Total:";
                    dt.Rows.Add(dr);
                    for (int i = 1; i < dt.Columns.Count-1; i++)
                    {
                        int total = 0;
                        for (int j = 0; j < dt.Rows.Count-1; j++)
                        {
                            if (!string.IsNullOrWhiteSpace(dt.Rows[j][i].ToString()))
                            {
                                string temp;
                                temp = dt.Rows[j][i].ToString();
                                total += Int32.Parse(temp);
                            }
                        }
                        if(total != 0)
                        {
                            dt.Rows[dt.Rows.Count - 1][i] = total.ToString();
                        }
                    }

                    //sum of total hours of rows and columns
                    int totals = 0;
                    for(int i = 0; i < dt.Rows.Count-1; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(dt.Rows[i]["Total"].ToString()))
                        {
                            string temp;
                            temp = dt.Rows[i]["Total"].ToString();
                            totals += Int32.Parse(temp);
                        }
                    }
                    dt.Rows[dt.Rows.Count - 1][dt.Columns.Count - 1] = totals.ToString();

                    GridView gv = new GridView();
                    gv.ID = "gv-" + teamName + "(" + workPeriod + ")";
                    gv.ClientIDMode = ClientIDMode.Static;
                    gv.RowDataBound += new GridViewRowEventHandler(AccountDistributionHistoryGridView_RowDataBound);
                    gv.DataSource = dt;
                    GridViewsPanel.Controls.Add(gv);
                    gv.DataBind();
                    GridViewsPanel.Controls.Add(new LiteralControl("<br/>"));

                    gv.HeaderRow.Cells[0].Text = "Team members";
                    int gvCell = 1;
                    foreach (Account account in allAccounts)
                    {
                        gv.HeaderRow.Cells[gvCell].Text = account.accountNumber + "<br/>" + account.projectName;
                        gv.HeaderRow.Cells[gvCell].BackColor = Color.Gold;
                        gvCell++;
                    }

                    GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);
                    TableHeaderCell MYHeader = new TableHeaderCell();
                    MYHeader.Text = teamName;
                    MYHeader.ColumnSpan = dt.Columns.Count;
                    MYHeader.BackColor = Color.Aquamarine;
                    row.Controls.Add(MYHeader);
                    gv.HeaderRow.Parent.Controls.AddAt(0, row);
                }
            }
        }

        protected void AccountDistributionHistoryGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                for (int i = 1; i < e.Row.Cells.Count - 1; i++)
                {
                    if (e.Row.Cells[i].Text != "&nbsp;" && e.Row.Cells[0].Text != "Total:")
                    {
                        e.Row.Cells[i].BackColor = Color.YellowGreen;
                    }
                    else if(e.Row.Cells[0].Text == "Total:")
                    {
                        e.Row.BackColor = Color.LightSteelBlue;
                    }
                }
                e.Row.Cells[e.Row.Cells.Count-1].BackColor = Color.LightSteelBlue;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Welcome.Text = string.Format("Hello, {0}!", User.Identity.GetUserName());
            ListAccountDistributions();
        }

        protected void SignOut(object sender, EventArgs e)
        {
            var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.SignOut();
            Response.Redirect("~/Default.aspx");
        }
    }
}