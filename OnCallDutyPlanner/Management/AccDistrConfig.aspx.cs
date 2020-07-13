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
                var queryString = "SELECT ID, SLATeamID FROM AccountDistribution WHERE WorkPeriod <= @workPeriod AND (DateFinished IS NULL OR DateFinished >= @workPeriod);";
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

        private int GetAccountDistributionIDByTeam(int teamID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                int accDistrID = 0;
                var queryString = "SELECT ID FROM AccountDistribution WHERE SLATeamID = @teamID AND DateFinished IS NULL";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@teamID", teamID);

                try
                {
                    connection.Open();
                    accDistrID = (Int32)command.ExecuteScalar();
                    connection.Close();
                    return accDistrID;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return -1;
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
            string workPeriod = DropDownYear.Text + "-" + DropDownMonth.Text + "-01";
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

        private List<int> GetYears()
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                List<int> year = new List<int>();
                List<int> yearDistinct = new List<int>();
                var queryString = "SELECT DISTINCT WorkDate FROM Dates ORDER BY WorkDate ASC";
                SqlCommand command = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            year.Add(reader.GetDateTime(0).Year);
                        }
                    }
                    reader.Close();
                    connection.Close();
                    yearDistinct = year.Distinct().ToList();
                    return yearDistinct;
                }
                catch (Exception ex)
                {
                    //ErrorEditLiteral.Text = string.Format(ex.Message);
                    return null;
                }
            }
        }

        private void FillDDLYear()
        {
            List<int> yearsInDB = GetYears();
            if (yearsInDB.Count != 0)
            {
                foreach (int year in yearsInDB)
                {
                    DropDownYear.Items.Add(year.ToString());
                }

                if (DateTime.Now.Month == 12)
                {
                    int nextYear = DateTime.Now.Year + 1;
                    DropDownYear.Items.Add(nextYear.ToString());
                }
            }
            else
            {
                DropDownYear.Items.Add(DateTime.Now.Year.ToString());
            }
        }

        protected void SelectBtnClick(object sender, EventArgs e)
        {
            GridViewsPanel.Controls.Clear();
            ListAccountDistributionConfigurations();
        }

        private List<string> GetAllTeams()
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "SELECT TeamName FROM SLATeams WHERE IsDeleted = 0";
                SqlCommand command = new SqlCommand(queryString, connection);
                List<string> teams = new List<string>();

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            teams.Add(reader.GetString(0));
                        }
                    }
                    reader.Close();
                    connection.Close();
                    return teams;

                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        protected void OpenCreateAccDistrConfigButton_Click(object sender, EventArgs e)
        {
            if (ChooseTeamDDL.SelectedValue != "---")
            {
                string workPeriod = DateTime.Now.Year.ToString() + "-" + (DateTime.Now.Month + 1).ToString() + "-1";
                AccountDistributionStartDateLabel.Text = "This Account Distribution Configuration will apply on " + workPeriod;
                AccountDistributionStartDateLabel.Visible = true;

                int teamID = GetTeamID(ChooseTeamDDL.SelectedValue);
                int accDistrID = GetAccountDistributionIDByTeam(teamID);
                List<AccountDistributionAccounts> accountDistributionAccounts = GetAllAccountIDFromAccountDistributionAccounts(accDistrID);
                List<Account> accounts = GetAllAccounts();

                DataTable dt = new DataTable();
                dt.Columns.Add("Percent");
                dt.Columns.Add("Account");

                if (accountDistributionAccounts.Count != 0)
                {
                    foreach (AccountDistributionAccounts accDistrAcc in accountDistributionAccounts)
                    {
                        int index = accounts.IndexOf(accounts.Where(obj => obj.ID == accDistrAcc.accountID).FirstOrDefault());
                        accounts[index].percent = accDistrAcc.percentage;
                        DataRow dr = dt.NewRow();
                        dr["Percent"] = accounts[index].percent.ToString();
                        dr["Account"] = accounts[index].accountNumber;
                        dt.Rows.Add(dr);
                    }
                }
                else
                {
                    DataRow dr = dt.NewRow();
                    dr["Percent"] = "0";
                    dr["Account"] = "---";
                    dt.Rows.Add(dr);
                }

                ViewState["panelContents"] = dt;
                AccountsListGridView.DataSource = dt;
                AccountsListGridView.DataBind();
                
                ModalPopupExtender1.Show();
            }
            else
                lbl_warningChooseTeamDDL.Visible = true;
        }

        protected void AccountsListGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if(e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRow dr = ((DataTable)ViewState["panelContents"]).Rows[e.Row.RowIndex];

                TextBox txtBox = (TextBox)e.Row.FindControl("percentTxtBox");
                txtBox.Text = dr[0].ToString();

                List<Account> accounts = GetAllAccounts();
                DropDownList ddl = (DropDownList)e.Row.FindControl("ddlAccount");
                foreach (Account acc in accounts)
                {
                    ddl.Items.Add(acc.accountNumber);
                }
                if (dr["Account"].ToString() != "---")
                {
                    ddl.SelectedValue = dr["Account"].ToString();
                }

                LinkButton btn = (LinkButton)e.Row.FindControl("RemoveRowBtn");
            }
        }

        protected void percentTxtBox_TextChanged(object sender, EventArgs e)
        {
            TextBox txtBox = (TextBox)sender;
            GridViewRow row = (GridViewRow)txtBox.Parent.Parent;
            int rowIndex = row.RowIndex;

            DataTable dt = (DataTable)ViewState["panelContents"];
            dt.Rows[rowIndex][0] = txtBox.Text;
            ViewState["panelContents"] = dt;
        }

        protected void ddlAccount_IndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            GridViewRow row = (GridViewRow)ddl.Parent.Parent;
            int rowIndex = row.RowIndex;

            DataTable dt = (DataTable)ViewState["panelContents"];
            dt.Rows[rowIndex][1] = ddl.SelectedValue;
            ViewState["panelContents"] = dt;
        }

        protected void AccountsListGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if(e.CommandName == "RemoveRow")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                DataTable dt = (DataTable)ViewState["panelContents"];

                DataRow dr = dt.Rows[rowIndex];
                dt.Rows.Remove(dr);
                dt.AcceptChanges();

                if(dt.Rows.Count == 0)
                {
                    DataRow drNew = dt.NewRow();
                    dr["Percent"] = "0";
                    dr["Account"] = "---";
                    dt.Rows.Add(drNew);
                }

                ViewState["panelContents"] = dt;
                AccountsListGridView.DataSource = dt;
                AccountsListGridView.DataBind();
                AddAccDistrUpdatePanel.Update();
                ModalPopupExtender1.Show();
            }
        }

        protected void AddAccountButton_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)ViewState["panelContents"];
            DataRow dr = dt.NewRow();

            dr["Percent"] = "0";
            dr["Account"] = "---";

            dt.Rows.Add(dr);

            ViewState["panelContents"] = dt;
            AccountsListGridView.DataSource = dt;
            AccountsListGridView.DataBind();
            AddAccDistrUpdatePanel.Update();
            ModalPopupExtender1.Show();
        }

        private int GetAccountID(string accountNumber)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                int accID = 0;
                var queryString = "SELECT ID FROM Accounts WHERE AccountNumber = @accountNumber";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@accountNumber", accountNumber);

                try
                {
                    connection.Open();
                    accID = (Int32)command.ExecuteScalar();
                    connection.Close();
                    return accID;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return -1;
                }
            }
        }

        private void CreateAccountDistributionAccounts(int percent, int accountDistributonID, int accountID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "INSERT INTO AccountDistributionAccounts(Percentage, AccountDistributionID, AccountID) VALUES (@percent, @accountDistributonID, @accountID);";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@percent", percent);
                command.Parameters.AddWithValue("@accountDistributonID", accountDistributonID);
                command.Parameters.AddWithValue("@accountID", accountID);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void CreateAccountDistribution(int teamID, string workPeriod)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "INSERT INTO AccountDistribution(SLATeamID, WorkPeriod) VALUES (@teamID, @workPeriod);";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@teamID", teamID);
                command.Parameters.AddWithValue("@workPeriod", workPeriod);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void FinishOldAccountDistribution(int teamID, string dateFinished)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "UPDATE AccountDistribution SET DateFinished = @dateFinished WHERE SLATeamID = @teamID AND DateFinished IS NULL;";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@teamID", teamID);
                command.Parameters.AddWithValue("@dateFinished", dateFinished);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {

                }

            }
        }

        protected void CreateAccDistrConfigButton_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)ViewState["panelContents"];
            if(dt.Rows.Count != 0)
            {
                int percentCheck = 0;

                foreach (DataRow dr in dt.Rows)
                {
                    percentCheck += Int32.Parse(dr["Percent"].ToString());
                }

                if (percentCheck == 100)
                {
                    string teamName = ChooseTeamDDL.SelectedValue;
                    int teamID = GetTeamID(teamName);
                    string dateFinished = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month).ToString();
                    string workPeriod = DateTime.Now.Year.ToString() + "-" + (DateTime.Now.Month + 1).ToString() + "-1";
                    FinishOldAccountDistribution(teamID, dateFinished);
                    CreateAccountDistribution(teamID, workPeriod);

                    foreach (DataRow dr in dt.Rows)
                    {
                        int percent = Int32.Parse(dr["Percent"].ToString());
                        string accountNumber = dr["Account"].ToString();
                        int accountID = GetAccountID(accountNumber);
                        int accDistrID = GetAccountDistributionIDByTeam(teamID);
                        CreateAccountDistributionAccounts(percent, accDistrID, accountID);
                        lbl_SuccessCreate.Visible = true;
                    }

                    ModalPopupExtender1.Hide();
                }
                else
                    SumWarningLabel.Visible = true;
            }
            else
                SumWarningLabel.Visible = true;
        }

        private int GetTeamID(string teamName)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                int teamID;
                var queryString = "SELECT ID FROM SLATeams WHERE TeamName = @teamName";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@teamName", teamName);

                try
                {
                    connection.Open();
                    teamID = (Int32)command.ExecuteScalar();
                    connection.Close();
                    return teamID;

                }
                catch (Exception ex)
                {
                    return -1;
                }

            }
        }

        private List<Account> GetAllAccounts()
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                List<Account> accounts = new List<Account>();
                var queryString = "SELECT ID, AccountNumber, ProjectName FROM Accounts WHERE DateFinished IS NULL";
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                Welcome.Text = string.Format("Hello, {0}!", User.Identity.GetUserName());

                if (DropDownYear.Items.Count == 0)
                {
                    FillDDLYear();
                    DropDownYear.Items.FindByText(DateTime.Now.Year.ToString()).Selected = true;
                }

                if (DropDownMonth.Items.Count == 0)
                {
                    DropDownMonth.Items.Add(new ListItem("1"));
                    DropDownMonth.Items.Add(new ListItem("2"));
                    DropDownMonth.Items.Add(new ListItem("3"));
                    DropDownMonth.Items.Add(new ListItem("4"));
                    DropDownMonth.Items.Add(new ListItem("5"));
                    DropDownMonth.Items.Add(new ListItem("6"));
                    DropDownMonth.Items.Add(new ListItem("7"));
                    DropDownMonth.Items.Add(new ListItem("8"));
                    DropDownMonth.Items.Add(new ListItem("9"));
                    DropDownMonth.Items.Add(new ListItem("10"));
                    DropDownMonth.Items.Add(new ListItem("11"));
                    DropDownMonth.Items.Add(new ListItem("12"));
                    DropDownMonth.Items.FindByText(DateTime.Now.Month.ToString()).Selected = true;
                }

                if (ChooseTeamDDL.Items.Count == 0)
                {
                    List<string> teams = GetAllTeams();
                    ChooseTeamDDL.Items.Add("---");
                    if (teams.Count != 0)
                    {
                        foreach (string teamName in teams)
                        {
                            ChooseTeamDDL.Items.Add(teamName);
                        }
                    }
                    ChooseTeamDDL.SelectedValue = "---";
                }

                ListAccountDistributionConfigurations();
            }
            else
            {
                bool t1 = ScriptManager.GetCurrent(Page).IsInAsyncPostBack;
                string t2 = ScriptManager.GetCurrent(Page).AsyncPostBackSourceElementID;
            }
        }

        protected void SignOut(object sender, EventArgs e)
        {
            var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.SignOut();
            Response.Redirect("~/Default.aspx");
        }
    }
}