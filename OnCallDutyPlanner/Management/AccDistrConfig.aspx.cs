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

namespace OnCallDutyPlanner.Management
{
    public partial class AccDistrConfig : System.Web.UI.Page
    {
        class AccountDistribution
        {
            public int id { get; set; }
            public int teamID { get; set; }
        }

        class Account
        {
            public int ID { get; set; }
            public string accountNumber { get; set; }
            public string projectName { get; set; }
            public int percent { get; set; }
        }

        class AccountDistributionAccounts
        {
            public int accountID { get; set; }
            public int percentage { get; set; }
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
                            accDistr.Add(new AccountDistribution { id = reader.GetInt32(0), teamID = reader.GetInt32(1) });
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

        private List<AccountDistributionAccounts> GetAllAccountIDFromAccountDistributionAccounts(int accDistID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                List<AccountDistributionAccounts> accounts = new List<AccountDistributionAccounts>();
                var queryString = "SELECT AccountID, Percentage FROM AccountDistributionAccounts WHERE AccountDistributionID = @accDistID";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@accDistID", accDistID);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            accounts.Add(new AccountDistributionAccounts { accountID = reader.GetInt32(0), percentage = reader.GetInt32(1) });
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

        private Account GetAccount(int accountID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                Account account = new Account();
                var queryString = "SELECT ID, AccountNumber, ProjectName FROM Accounts WHERE ID = @accountID";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@accountID", accountID);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            account.ID = reader.GetInt32(0);
                            account.accountNumber = reader.GetString(1);
                            account.projectName = reader.GetString(2);
                        }
                    }
                    reader.Close();
                    connection.Close();
                    return account;

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

        private void ListAccountDistributionConfigurations()
        {
            var userStore = new UserStore<IdentityUser>();
            var manager = new UserManager<IdentityUser>(userStore);
            string workPeriod = "2020-01-01";
            List<AccountDistribution> accDistr = GetAllAccountDistributionInPeriod(workPeriod);

            if (accDistr.Count != 0)
            {
                foreach (AccountDistribution accountDistribution in accDistr)
                {
                    DataTable dt = new DataTable("AccountDistributionConfiguration");
                    DataRow dr;

                    dt.Columns.Add("Percentage");
                    dt.Columns.Add("AccountNumber");
                    dt.Columns.Add("ProjectName");

                    string teamName = GetTeamName(accountDistribution.teamID);
                    List<AccountDistributionAccounts> accounts = GetAllAccountIDFromAccountDistributionAccounts(accountDistribution.id);
                    List<Account> accountsInDistribution = new List<Account>();

                    foreach(AccountDistributionAccounts account in accounts)
                    {
                        Account acc = GetAccount(account.accountID);
                        acc.percent = account.percentage;
                        accountsInDistribution.Add(acc);
                    }

                    foreach(Account acc in accountsInDistribution)
                    {
                        dr = dt.NewRow();
                        dr["Percentage"] = acc.percent.ToString();
                        dr["AccountNumber"] = acc.accountNumber;
                        dr["ProjectName"] = acc.projectName;
                        dt.Rows.Add(dr);
                    }

                    GridView gv = new GridView();
                    gv.ID = "gv-" + teamName + "(" + workPeriod + ")";
                    gv.ClientIDMode = ClientIDMode.Static;
                    gv.DataSource = dt;
                    GridViewsPanel.Controls.Add(gv);
                    gv.DataBind();
                    GridViewsPanel.Controls.Add(new LiteralControl("<br/>"));

                    gv.HeaderRow.Cells[0].Text = "Percentage";
                    gv.HeaderRow.Cells[1].Text = "Account number";
                    gv.HeaderRow.Cells[2].Text = "Project name";

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

        protected void Page_Load(object sender, EventArgs e)
        {
            Welcome.Text = string.Format("Hello, {0}!", User.Identity.GetUserName());
            ListAccountDistributionConfigurations();
        }

        protected void SignOut(object sender, EventArgs e)
        {
            var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.SignOut();
            Response.Redirect("~/Default.aspx");
        }
    }
}