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
            public string userID { get; set; }
            public int accountDistributionID { get; set; }
            public int accountID { get; set; }
            public int totalHours { get; set; }
            public string workPeriod { get; set; }
        }

        class Account
        {
            public int ID { get; set; }
            public string accountNumber { get; set; }
            public string projectName { get; set; }
        }

        class AccountDistributionAccounts
        {
            public int accountID { get; set; }
            public int percentage { get; set; }
        }

        private List<AccountDistributionHistory> GetAccountDistributionHistory(int accDistrID, string timePeriod)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                List<AccountDistributionHistory> accDistrHistory = new List<AccountDistributionHistory>();
                var queryString = "SELECT UserID, AccountID, TotalHours FROM AccountDistributionHistory WHERE AccountDistributionID = @accDistrID AND TimePeriod = @timePeriod";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@accDistrID", accDistrID);
                command.Parameters.AddWithValue("@timePeriod", timePeriod);

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

        private List<Account> GetAllAccounts(string startOfPeriod, string endOfPeriod)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                List<Account> accounts = new List<Account>();
                var queryString = "SELECT ID, AccountNumber, ProjectName FROM Accounts WHERE DateCreated < @endOfPeriod AND (DateFinished IS NULL OR DateFinished > @startOfPeriod);";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@startOfPeriod", startOfPeriod);
                command.Parameters.AddWithValue("@endOfPeriod", endOfPeriod);

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

        private List<string> GetAllTeamMembersIDFromDates(int accDistrID, string startOfPeriod, string endOfPeriod)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "SELECT DISTINCT UserID FROM Dates WHERE AccountDistributionID = @accDistrID AND WorkDate <= @endOfPeriod AND WorkDate >= @startOfPeriod";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@accDistrID", accDistrID);
                command.Parameters.AddWithValue("@startOfPeriod", startOfPeriod);
                command.Parameters.AddWithValue("@endOfPeriod", endOfPeriod);
                List<string> teamMembersID = new List<string>();

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            teamMembersID.Add(reader.GetString(0));
                        }
                    }
                    reader.Close();
                    connection.Close();
                    return teamMembersID;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        private List<string> GetAllWorkDayTypesInMonthForWorker(string userID, string startOfPeriod, string endOfPeriod)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "SELECT DateType FROM Dates WHERE WorkDate <= @endOfPeriod AND WorkDate >= @startOfPeriod AND UserID = @userID;";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@userID", userID);
                command.Parameters.AddWithValue("@startOfPeriod", startOfPeriod);
                command.Parameters.AddWithValue("@endOfPeriod", endOfPeriod);
                List<string> workDayTypes = new List<string>();

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            workDayTypes.Add(reader.GetString(0));
                        }
                    }
                    reader.Close();
                    connection.Close();
                    return workDayTypes;

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

        private int GetCurrentTeamIDFromUser(string userID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                int teamID = -1;
                var queryString = "SELECT SLATeamID FROM AspNetUsers WHERE Id = @userID";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@userID", userID);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            teamID = reader.GetInt32(0);
                        }
                    }
                    reader.Close();
                    connection.Close();
                    return teamID;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return -2;
                }
            }
        }

        private int GetSelectedPeriodAccountDistributionID(int teamID, string startOfPeriod, string endOfPeriod)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                int accDistrID = -1;
                var queryString = "SELECT ID FROM AccountDistribution WHERE SLATeamID = @teamID AND WorkPeriod < @endOfPeriod AND (DateFinished IS NULL OR DateFinished > @startOfPeriod);";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@teamID", teamID);
                command.Parameters.AddWithValue("@startOfPeriod", startOfPeriod);
                command.Parameters.AddWithValue("@endOfPeriod", endOfPeriod);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            accDistrID = reader.GetInt32(0);
                        }
                    }
                    reader.Close();
                    connection.Close();
                    return accDistrID;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return -2;
                }
            }
        }

        private List<string> GetCurrentTeamMembers(int teamID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "SELECT UserName FROM AspNetUsers WHERE SLATeamID = @teamID";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@teamID", teamID);
                List<string> teamMembers = new List<string>();

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            teamMembers.Add(reader.GetString(0));
                        }
                    }
                    reader.Close();
                    connection.Close();
                    return teamMembers;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        private bool CheckIfUserIsInAccountDistributionFromHistory(string userID, int accDistrID, string timePeriod)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                bool isUserInAccDistr = false;
                var queryString = "SELECT UserID FROM AccountDistributionHistory WHERE UserID = @userID AND AccountDistributionID = @accDistrID AND TimePeriod = @timePeriod;";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@userID", userID);
                command.Parameters.AddWithValue("@accDistrID", accDistrID);
                command.Parameters.AddWithValue("@timePeriod", timePeriod);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if(!reader.IsDBNull(0))
                            {
                                isUserInAccDistr = true;
                                break;
                            }
                        }
                    }
                    reader.Close();
                    connection.Close();
                    return isUserInAccDistr;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }

        private void ListAccountDistributions()
        {
            var userStore = new UserStore<IdentityUser>();
            var manager = new UserManager<IdentityUser>(userStore);
            int ddlYear = Int32.Parse(DropDownYear.SelectedValue);
            int ddlMonth = Int32.Parse(DropDownMonth.SelectedValue);
            DateTime startOfSelectedPeriod = new DateTime(ddlYear, ddlMonth, 1);
            DateTime endOfSelectedPeriod = new DateTime(ddlYear, ddlMonth, DateTime.DaysInMonth(ddlYear, ddlMonth));
            string startOfSelectedPeriodString = startOfSelectedPeriod.ToString("yyyy-MM-dd");
            string endOfSelectedPeriodString = endOfSelectedPeriod.ToString("yyyy-MM-dd");
            List<Account> allAccounts = GetAllAccounts(startOfSelectedPeriodString, endOfSelectedPeriodString);

            if (manager.GetRoles(User.Identity.GetUserId()).FirstOrDefault() == "Worker")
            {
                var user = manager.FindById(User.Identity.GetUserId());
                string teamName = "";
                DateTime dateCheck = new DateTime(ddlYear, ddlMonth, 1);
                bool hasData = false;
                DataTable dt = new DataTable("AccountDistributionHistory");
                DataRow dr;

                dt.Columns.Add("TeamMembers");

                if ((dateCheck.Year == DateTime.Now.Year && dateCheck.Month < DateTime.Now.Month) || dateCheck.Year < DateTime.Now.Year)
                {
                    AccountDistribution accDistr = new AccountDistribution();
                    List<AccountDistribution> accountDistributions = GetAllAccountDistributionInPeriod(startOfSelectedPeriodString);
                    foreach(AccountDistribution accountDistribution in accountDistributions)
                    {
                        bool checkIfUserInAccDistr = CheckIfUserIsInAccountDistributionFromHistory(user.Id, accountDistribution.id, startOfSelectedPeriodString);

                        if (checkIfUserInAccDistr == true)
                        {
                            accDistr = accountDistribution;
                            break;
                        }
                    }

                    List<AccountDistributionHistory> accDistHistory = GetAccountDistributionHistory(accDistr.id, startOfSelectedPeriodString);
                    teamName = GetTeamName(accDistr.teamID);

                    if (accDistHistory.Count != 0)
                    {
                        string userNameCheck = "";
                        foreach (AccountDistributionHistory accDistHist in accDistHistory)
                        {
                            var user2 = manager.FindById(accDistHist.userID);
                            if (userNameCheck != user2.UserName)
                            {
                                dr = dt.NewRow();
                                dr["TeamMembers"] = user2.UserName;
                                dt.Rows.Add(dr);
                                userNameCheck = user2.UserName;
                            }
                        }

                        foreach (Account account in allAccounts)
                        {
                            dt.Columns.Add(account.accountNumber);
                        }

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            foreach (AccountDistributionHistory accDistHist in accDistHistory)
                            {
                                var user3 = manager.FindById(accDistHist.userID);
                                if (user3.UserName == dt.Rows[i][0].ToString())
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

                        hasData = true;
                    }
                }

                else if((ddlMonth == DateTime.Now.Month && ddlYear == DateTime.Now.Year) || (DateTime.Now.Month == 12 && ddlMonth == 1 && ddlYear == (DateTime.Now.Year + 1)) || (ddlMonth == (DateTime.Now.Month + 1) && ddlYear == DateTime.Now.Year))
                {
                    int teamID = GetCurrentTeamIDFromUser(user.Id);
                    teamName = GetTeamName(teamID);
                    int accDistrID = GetSelectedPeriodAccountDistributionID(teamID, startOfSelectedPeriodString, endOfSelectedPeriodString);
                    List<AccountDistributionAccounts> accountsInDistribution = GetAllAccountIDFromAccountDistributionAccounts(accDistrID);
                    List<AccountDistributionHistory> teamMembersHistory = new List<AccountDistributionHistory>();
                    List<string> teamMembers = GetCurrentTeamMembers(teamID);

                    if (accountsInDistribution.Count != 0 && teamMembers.Count != 0)
                    {
                        foreach (string teamMember in teamMembers)
                        {
                            dr = dt.NewRow();
                            dr["TeamMembers"] = teamMember;
                            dt.Rows.Add(dr);
                        }

                        foreach (Account account in allAccounts)
                        {
                            dt.Columns.Add(account.accountNumber);
                        }


                        for (int i = 0; i < teamMembers.Count; i++)
                        {
                            var user2 = manager.FindByName(teamMembers[i]);
                            List<string> allWorkDaysWorker = GetAllWorkDayTypesInMonthForWorker(user2.Id, startOfSelectedPeriodString, endOfSelectedPeriodString);
                            int totalWorkHoursWorker = 0;

                            for (int j = 0; j < allWorkDaysWorker.Count; j++)
                            {
                                if (allWorkDaysWorker[j] == "w")
                                {
                                    totalWorkHoursWorker += 16;
                                }
                                else if (allWorkDaysWorker[j] == "h")
                                {
                                    totalWorkHoursWorker += 24;
                                }
                            }

                            for (int z = 0; z < accountsInDistribution.Count; z++)
                            {
                                int accID = accountsInDistribution[z].accountID;
                                int accPercent = accountsInDistribution[z].percentage;
                                double tempDouble = (double)totalWorkHoursWorker * ((double)accPercent / (double)100);
                                int hoursWorkedOnAcc = (int)Math.Round(tempDouble, MidpointRounding.AwayFromZero);

                                teamMembersHistory.Add(new AccountDistributionHistory { userID = user2.Id, workPeriod = startOfSelectedPeriodString, accountID = accID, totalHours = hoursWorkedOnAcc, accountDistributionID = accDistrID });
                            }
                        }

                        if (teamMembersHistory.Count != 0)
                        {
                            for (int x = 0; x < dt.Rows.Count; x++)
                            {
                                foreach (AccountDistributionHistory accDistHist in teamMembersHistory)
                                {
                                    var user3 = manager.FindById(accDistHist.userID);
                                    if (user3.UserName == dt.Rows[x][0].ToString())
                                    {
                                        foreach (Account account in allAccounts)
                                        {
                                            if (accDistHist.accountID == account.ID)
                                            {
                                                dt.Rows[x][account.accountNumber] = accDistHist.totalHours.ToString();
                                            }
                                        }
                                    }
                                }
                            }

                            hasData = true;
                        }
                    }
                }

                if (hasData == true)
                {
                    //count total hours per row
                    dt.Columns.Add("Total");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int total = 0;
                        for (int j = 1; j < dt.Columns.Count; j++)
                        {
                            if (!string.IsNullOrWhiteSpace(dt.Rows[i][j].ToString()))
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
                    for (int i = 1; i < dt.Columns.Count - 1; i++)
                    {
                        int total = 0;
                        for (int j = 0; j < dt.Rows.Count - 1; j++)
                        {
                            if (!string.IsNullOrWhiteSpace(dt.Rows[j][i].ToString()))
                            {
                                string temp;
                                temp = dt.Rows[j][i].ToString();
                                total += Int32.Parse(temp);
                            }
                        }
                        if (total != 0)
                        {
                            dt.Rows[dt.Rows.Count - 1][i] = total.ToString();
                        }
                    }

                    //sum of total hours of rows and columns
                    int totals = 0;
                    for (int i = 0; i < dt.Rows.Count - 1; i++)
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
                    gv.ID = "gv-" + teamName + "(" + startOfSelectedPeriodString + ")";
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
                    MYHeader.BackColor = Color.LightSkyBlue;
                    row.Controls.Add(MYHeader);
                    gv.HeaderRow.Parent.Controls.AddAt(0, row);
                }
            }

            else
            {
                List<AccountDistribution> accDistr = GetAllAccountDistributionInPeriod(startOfSelectedPeriodString);
                if (accDistr.Count != 0)
                {
                    foreach (AccountDistribution accountDistribution in accDistr)
                    {
                        DateTime dateCheck = new DateTime(ddlYear, ddlMonth, 1);
                        string teamName = GetTeamName(accountDistribution.teamID);
                        bool hasData = false;
                        DataTable dt = new DataTable("AccountDistributionHistory");
                        DataRow dr;

                        dt.Columns.Add("TeamMembers");

                        if ((dateCheck.Year == DateTime.Now.Year && dateCheck.Month < DateTime.Now.Month) || dateCheck.Year < DateTime.Now.Year)
                        {
                            List<AccountDistributionHistory> accDistHistory = GetAccountDistributionHistory(accountDistribution.id, startOfSelectedPeriodString);

                            if (accDistHistory.Count == 0)
                            {
                                break;
                            }

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
                                foreach (AccountDistributionHistory accDistHist in accDistHistory)
                                {
                                    var user = manager.FindById(accDistHist.userID);
                                    if (user.UserName == dt.Rows[i][0].ToString())
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

                            hasData = true;
                        }

                        else if ((ddlMonth == DateTime.Now.Month && ddlYear == DateTime.Now.Year) || (DateTime.Now.Month == 12 && ddlMonth == 1 && ddlYear == (DateTime.Now.Year + 1)) || (ddlMonth == (DateTime.Now.Month + 1) && ddlYear == DateTime.Now.Year))
                        {
                            List<AccountDistributionAccounts> accountsInDistribution = GetAllAccountIDFromAccountDistributionAccounts(accountDistribution.id);
                            List<AccountDistributionHistory> teamMembersHistory = new List<AccountDistributionHistory>();
                            List<string> teamMembersID = GetAllTeamMembersIDFromDates(accountDistribution.id, startOfSelectedPeriodString, endOfSelectedPeriodString);

                            if (accountsInDistribution.Count == 0 || teamMembersID.Count == 0)
                            {
                                break;
                            }

                            foreach (string teamMemberID in teamMembersID)
                            {
                                var user = manager.FindById(teamMemberID);
                                dr = dt.NewRow();
                                dr["TeamMembers"] = user.UserName;
                                dt.Rows.Add(dr);
                            }

                            foreach (Account account in allAccounts)
                            {
                                dt.Columns.Add(account.accountNumber);
                            }


                            for (int i = 0; i < teamMembersID.Count; i++)
                            {
                                List<string> allWorkDaysWorker = GetAllWorkDayTypesInMonthForWorker(teamMembersID[i], startOfSelectedPeriodString, endOfSelectedPeriodString);
                                int totalWorkHoursWorker = 0;

                                for (int j = 0; j < allWorkDaysWorker.Count; j++)
                                {
                                    if (allWorkDaysWorker[j] == "w")
                                    {
                                        totalWorkHoursWorker += 16;
                                    }
                                    else if (allWorkDaysWorker[j] == "h")
                                    {
                                        totalWorkHoursWorker += 24;
                                    }
                                }

                                for (int z = 0; z < accountsInDistribution.Count; z++)
                                {
                                    int accID = accountsInDistribution[z].accountID;
                                    int accPercent = accountsInDistribution[z].percentage;
                                    double tempDouble = (double)totalWorkHoursWorker * ((double)accPercent / (double)100);
                                    int hoursWorkedOnAcc = (int)Math.Round(tempDouble, MidpointRounding.AwayFromZero);

                                    teamMembersHistory.Add(new AccountDistributionHistory { userID = teamMembersID[i], workPeriod = startOfSelectedPeriodString, accountID = accID, totalHours = hoursWorkedOnAcc, accountDistributionID = accountDistribution.id });
                                }
                            }

                            if (teamMembersHistory.Count == 0)
                            {
                                break;
                            }

                            for (int x = 0; x < dt.Rows.Count; x++)
                            {
                                foreach (AccountDistributionHistory accDistHist in teamMembersHistory)
                                {
                                    var user = manager.FindById(accDistHist.userID);
                                    if (user.UserName == dt.Rows[x][0].ToString())
                                    {
                                        foreach (Account account in allAccounts)
                                        {
                                            if (accDistHist.accountID == account.ID)
                                            {
                                                dt.Rows[x][account.accountNumber] = accDistHist.totalHours.ToString();
                                            }
                                        }
                                    }
                                }
                            }

                            hasData = true;
                        }

                        if (hasData == true)
                        {
                            //count total hours per row
                            dt.Columns.Add("Total");
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                int total = 0;
                                for (int j = 1; j < dt.Columns.Count; j++)
                                {
                                    if (!string.IsNullOrWhiteSpace(dt.Rows[i][j].ToString()))
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
                            for (int i = 1; i < dt.Columns.Count - 1; i++)
                            {
                                int total = 0;
                                for (int j = 0; j < dt.Rows.Count - 1; j++)
                                {
                                    if (!string.IsNullOrWhiteSpace(dt.Rows[j][i].ToString()))
                                    {
                                        string temp;
                                        temp = dt.Rows[j][i].ToString();
                                        total += Int32.Parse(temp);
                                    }
                                }
                                if (total != 0)
                                {
                                    dt.Rows[dt.Rows.Count - 1][i] = total.ToString();
                                }
                            }

                            //sum of total hours of rows and columns
                            int totals = 0;
                            for (int i = 0; i < dt.Rows.Count - 1; i++)
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
                            gv.ID = "gv-" + teamName + "(" + startOfSelectedPeriodString + ")";
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
                            MYHeader.BackColor = Color.LightSkyBlue;
                            row.Controls.Add(MYHeader);
                            gv.HeaderRow.Parent.Controls.AddAt(0, row);
                        }
                    }
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
            ListAccountDistributions();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                Welcome.Text = string.Format("Hello, {0}!", User.Identity.GetUserName());

                if (User.IsInRole("Worker"))
                {
                    ProjectsLink.Visible = false;
                    AccDistrConfigLink.Visible = false;
                    SLATeamsLink.Visible = false;
                    UsersLink.Visible = false;
                }

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

                ListAccountDistributions();
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