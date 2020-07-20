using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Configuration;


namespace OnCallDutyPlanner
{
    public partial class Scheduler : System.Web.UI.Page
    {
        class WorkDate
        {
            public int ID { get; set; }
            public string date { get; set; }
            public string dateType { get; set; }
            public string userID { get; set; }
            public int accountDistributionID { get; set; }
        }

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

        class AccountDistributionHistory
        {
            public string userID { get; set; }
            public int accountDistributionID { get; set; }
            public int accountID { get; set; }
            public int totalHours { get; set; }
            public string workPeriod { get; set; }
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

        private int GetTeamIDFromAccountDistribution(int accDistrID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                int teamID = -1;
                var queryString = "SELECT SLATeamID FROM AccountDistribution WHERE ID = @accDistrID";
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

        private string GetTeamName(int teamID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string teamName;
                var queryString = "SELECT TeamName FROM SLATeams WHERE ID = @teamID";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@teamID", teamID);

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

        private int GetSelectedPeriodAccountDistributionIDFromDatesForUser(string userID, string startOfPeriod, string endOfPeriod)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                int accDistrID = -1;
                var queryString = "SELECT DISTINCT AccountDistributionID FROM Dates WHERE UserID = @userID AND WorkDate >= @startOfPeriod AND WorkDate <= @endOfPeriod;";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@userID", userID);
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

        private List<AccountDistribution> GetAllAcccountDistributionsForSelectedPeriod(string startOfPeriod, string endOfPeriod)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                List<AccountDistribution> accDistr = new List<AccountDistribution>();
                var queryString = "SELECT ID, SLATeamID FROM AccountDistribution WHERE WorkPeriod < @endOfPeriod AND (DateFinished IS NULL OR DateFinished > @startOfPeriod);";
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

        private void DeleteAllDatesForUserInMonth(string userID, string startOfPeriod, string endOfPeriod)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "DELETE FROM Dates WHERE WorkDate <= @endOfPeriod AND WorkDate >= @startOfPeriod AND UserID = @userID;";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@userID", userID);
                command.Parameters.AddWithValue("@startOfPeriod", startOfPeriod);
                command.Parameters.AddWithValue("@endOfPeriod", endOfPeriod);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private string GetLastWorkDateForUser(string userID, string startOfLastMonthString, string endOfLastMonthString)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string lastWorkDate = "";
                var queryString = "SELECT MAX(WorkDate) FROM Dates WHERE WorkDate <= @endOfPeriod AND WorkDate >= @startOfPeriod AND UserID = @userID;";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@userID", userID);
                command.Parameters.AddWithValue("@startOfPeriod", startOfLastMonthString);
                command.Parameters.AddWithValue("@endOfPeriod", endOfLastMonthString);

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
                                lastWorkDate = reader.GetDateTime(0).ToString();
                            }
                        }
                    }
                    reader.Close();
                    connection.Close();
                    return lastWorkDate;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        private string GetDateType(string workDate, string userID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string dateType;
                var queryString = "SELECT DateType FROM Dates WHERE WorkDate = @workDate AND UserID = @userID;";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@workDate", workDate);
                command.Parameters.AddWithValue("@userID", userID);

                try
                {
                    connection.Open();
                    dateType = (String)command.ExecuteScalar();
                    connection.Close();
                    if (dateType == null)
                    {
                        dateType = "";
                    }
                    return dateType;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        private void InsertDate(string userID, string date, string dateType, int accDistrID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "INSERT INTO Dates(WorkDate, DateType, UserID, AccountDistributionID) VALUES(@date, @dateType, @userID, @accDistrID)";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@date", date);
                command.Parameters.AddWithValue("@dateType", dateType);
                command.Parameters.AddWithValue("@userID", userID);
                command.Parameters.AddWithValue("@accDistrID", accDistrID);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private void DeleteDate(string date, string userID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "DELETE FROM Dates WHERE WorkDate = @date AND UserID = @userID";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@date", date);
                command.Parameters.AddWithValue("@userID", userID);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    connection.Close();
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

        private void DeleteAllHistoryForUserInMonth(string userID, string startOfPeriod)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "DELETE FROM AccountDistributionHistory WHERE TimePeriod = @startOfPeriod AND UserID = @userID;";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@userID", userID);
                command.Parameters.AddWithValue("@startOfPeriod", startOfPeriod);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private void InsertAccountDistributionHistory(AccountDistributionHistory accDistHist)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "INSERT INTO AccountDistributionHistory(UserID, AccountID, TotalHours, AccountDistributionID, TimePeriod) VALUES(@UserID, @AccountID, @TotalHours, @AccountDistributionID, @TimePeriod)";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@UserID", accDistHist.userID);
                command.Parameters.AddWithValue("@AccountID", accDistHist.accountID);
                command.Parameters.AddWithValue("@TotalHours", accDistHist.totalHours);
                command.Parameters.AddWithValue("@AccountDistributionID", accDistHist.accountDistributionID);
                command.Parameters.AddWithValue("@TimePeriod", accDistHist.workPeriod);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        protected void ScheduleGridView_RowDataBoundWorker(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells[0].Text == User.Identity.Name && DateTime.Now.Month.ToString() == DropDownMonth.SelectedValue && DateTime.Now.Year.ToString() == DropDownYear.SelectedValue)
                {
                    for (int i = 1; i < e.Row.Cells.Count; i++)
                    {
                        if (e.Row.Cells[i].Text == "&nbsp;")
                        {
                            DataControlFieldCell cell = (DataControlFieldCell)e.Row.Cells[i];
                            string dayString = cell.ContainingField.HeaderText;
                            int found = dayString.IndexOf(" ");
                            dayString = dayString.Substring(0, found);
                            int day = Int32.Parse(dayString);
                            string date = DropDownYear.SelectedValue + "-" + DropDownMonth.SelectedValue + "-" + day.ToString();

                            string dateTypeDB = GetDateType(date, User.Identity.GetUserId());
                            DropDownList dateType = new DropDownList();
                            dateType.ID = "DDLDateType-" + e.Row.RowIndex.ToString() + "-" + i.ToString() + "(" + date + ")";
                            dateType.ClientIDMode = ClientIDMode.Static;
                            dateType.SelectedIndexChanged += new EventHandler(dateType_IndexChanged);
                            dateType.AutoPostBack = true;
                            dateType.Items.Add("");
                            dateType.Items.Add("w");
                            dateType.Items.Add("h");
                            dateType.SelectedValue = dateTypeDB;
                            if(dateTypeDB == "w"|| dateTypeDB == "h")
                            {
                                dateType.BackColor = Color.LightGray;
                            }

                            e.Row.Cells[i].Controls.Add(dateType);
                        }
                    }
                }
                else
                {
                    var userStore = new UserStore<IdentityUser>();
                    var manager = new UserManager<IdentityUser>(userStore);

                    var user = manager.FindByName(e.Row.Cells[0].Text);

                    for (int i = 1; i < e.Row.Cells.Count; i++)
                    {
                        if (e.Row.Cells[i].Text == "&nbsp;")
                        {
                            DataControlFieldCell cell = (DataControlFieldCell)e.Row.Cells[i];
                            string dayString = cell.ContainingField.HeaderText;
                            int found = dayString.IndexOf(" ");
                            dayString = dayString.Substring(0, found);
                            int day = Int32.Parse(dayString);
                            string date = DropDownYear.SelectedValue + "-" + DropDownMonth.SelectedValue + "-" + day.ToString();

                            string dateTypeDB = GetDateType(date, user.Id);
                            Label dateType = new Label();
                            dateType.ID = "LabelDate-" + e.Row.RowIndex.ToString() + "-" + i.ToString() + "(" + date + ")";
                            dateType.ClientIDMode = ClientIDMode.Static;
                            dateType.Text = dateTypeDB;

                            e.Row.Cells[i].Controls.Add(dateType);
                        }
                    }
                }
            }

            GridViewRow gvRow = e.Row;
            if (gvRow.RowType == DataControlRowType.Header)
            {
                gvRow.Cells.Remove(gvRow.Cells[0]);
                GridViewRow gvHeader = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);
                TableCell headerCell0 = new TableCell()
                {
                    Text = "Team members",
                    HorizontalAlign = HorizontalAlign.Center,
                    RowSpan = 2
                };
                TableCell headerCell1 = new TableCell()
                {
                    Text = DropDownMonth.SelectedValue + "/" + DropDownYear.SelectedValue,
                    HorizontalAlign = HorizontalAlign.Center,
                    ColumnSpan = e.Row.Cells.Count
                };
                gvHeader.Cells.Add(headerCell0);
                gvHeader.Cells.Add(headerCell1);
                e.Row.Parent.Parent.Controls[0].Controls.AddAt(0, gvHeader);
            }
        }

        protected void ScheduleGridView_RowDataBoundAdmin(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var userStore = new UserStore<IdentityUser>();
                var manager = new UserManager<IdentityUser>(userStore);

                var user = manager.FindByName(e.Row.Cells[0].Text);

                for (int i = 1; i < e.Row.Cells.Count; i++)
                {
                    if (e.Row.Cells[i].Text == "&nbsp;")
                    {
                        DataControlFieldCell cell = (DataControlFieldCell)e.Row.Cells[i];
                        string dayString = cell.ContainingField.HeaderText;
                        int found = dayString.IndexOf(" ");
                        dayString = dayString.Substring(0, found);
                        int day = Int32.Parse(dayString);
                        string date = DropDownYear.SelectedValue + "-" + DropDownMonth.SelectedValue + "-" + day.ToString();

                        string dateTypeDB = GetDateType(date, user.Id);
                        DropDownList dateType = new DropDownList();
                        dateType.ID = "DDLDateType-" + e.Row.RowIndex.ToString() + "-" + i.ToString() + "(" + date + ")";
                        dateType.ClientIDMode = ClientIDMode.Static;
                        dateType.SelectedIndexChanged += new EventHandler(dateType_IndexChanged);
                        dateType.AutoPostBack = true;
                        dateType.Items.Add("");
                        dateType.Items.Add("w");
                        dateType.Items.Add("h");
                        dateType.SelectedValue = dateTypeDB;
                        if (dateTypeDB == "w")
                        {
                            dateType.BackColor = Color.LightGreen;
                        }
                        else if(dateTypeDB == "h")
                        {
                            dateType.BackColor = Color.GreenYellow;
                        }

                        e.Row.Cells[i].Controls.Add(dateType);
                    }
                }
            }

            else if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells.Remove(e.Row.Cells[0]);
                GridViewRow gvHeader = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);
                TableCell headerCell0 = new TableCell()
                {
                    Text = "Team members",
                    HorizontalAlign = HorizontalAlign.Center,
                    RowSpan = 2
                };
                TableCell headerCell1 = new TableCell()
                {
                    Text = DropDownMonth.SelectedValue + "/" + DropDownYear.SelectedValue,
                    HorizontalAlign = HorizontalAlign.Center,
                    ColumnSpan = e.Row.Cells.Count
                };
                gvHeader.Cells.Add(headerCell0);
                gvHeader.Cells.Add(headerCell1);
                e.Row.Parent.Parent.Controls[0].Controls.AddAt(0, gvHeader);
            }

            else if(e.Row.RowType == DataControlRowType.Footer)
            {
                Button autoInsertBtn = new Button();
                autoInsertBtn.ID = "AutoGenerateScheduleBtn" + "-" + e.Row.NamingContainer.ID;
                autoInsertBtn.ClientIDMode = ClientIDMode.Static;
                autoInsertBtn.CommandName = "AutoGenerateSchedule";
                autoInsertBtn.CommandArgument = e.Row.NamingContainer.ID;
                autoInsertBtn.Text = "Auto Generate Schedule";

                Button createHistoryBtn = new Button();
                createHistoryBtn.ID = "CreateHistoryBtn" + "-" + e.Row.NamingContainer.ID;
                createHistoryBtn.ClientIDMode = ClientIDMode.Static;
                createHistoryBtn.CommandName = "CreateHistory";
                createHistoryBtn.CommandArgument = e.Row.NamingContainer.ID;
                createHistoryBtn.Text = "Write into history";

                int ddlYear = int.Parse(DropDownYear.SelectedValue);
                int ddlMonth = int.Parse(DropDownMonth.SelectedValue);

                if (ddlMonth == DateTime.Now.Month && ddlYear == DateTime.Now.Year)
                {
                    int colCount = e.Row.Cells.Count;
                    for (int i = 0; i < colCount - 2; i++)
                    {
                        e.Row.Cells.RemoveAt(1);
                    }
                    e.Row.Cells[0].ColumnSpan = colCount / 2;
                    e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Left;
                    e.Row.Cells[0].Controls.Add(autoInsertBtn);

                    e.Row.Cells[1].ColumnSpan = colCount / 2;
                    e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Right;
                    e.Row.Cells[1].Controls.Add(createHistoryBtn);
                }
                else if((DateTime.Now.Month == 12 && ddlMonth == 1 && ddlYear == (DateTime.Now.Year + 1)) || (ddlMonth == (DateTime.Now.Month + 1) && ddlYear == DateTime.Now.Year))
                {
                    int colCount = e.Row.Cells.Count;
                    for (int i = 0; i < colCount - 1; i++)
                    {
                        e.Row.Cells.RemoveAt(1);
                    }
                    e.Row.Cells[0].ColumnSpan = colCount;
                    e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Left;
                    e.Row.Cells[0].Controls.Add(autoInsertBtn);
                }
                else if((ddlMonth < DateTime.Now.Month && ddlYear == DateTime.Now.Year) || ddlYear < DateTime.Now.Year)
                {
                    int colCount = e.Row.Cells.Count;
                    for (int i = 0; i < colCount - 1; i++)
                    {
                        e.Row.Cells.RemoveAt(1);
                    }
                    e.Row.Cells[0].ColumnSpan = colCount;
                    e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Right;
                    e.Row.Cells[0].Controls.Add(createHistoryBtn);
                }
            }
        }

        protected void ScheduleGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "AutoGenerateSchedule")
            {
                var userStore = new UserStore<IdentityUser>();
                var manager = new UserManager<IdentityUser>(userStore);
                int ddlYear = int.Parse(DropDownYear.SelectedValue);
                int ddlMonth = int.Parse(DropDownMonth.SelectedValue);
                DateTime startOfSelectedPeriod = new DateTime(ddlYear, ddlMonth, 1);
                DateTime endOfSelectedPeriod = new DateTime(ddlYear, ddlMonth, DateTime.DaysInMonth(ddlYear, ddlMonth));
                string startOfSelectedPeriodString = startOfSelectedPeriod.ToString("yyyy-MM-dd");
                string endOfSelectedPeriodString = endOfSelectedPeriod.ToString("yyyy-MM-dd");
                DateTime startOfLastMonth = new DateTime();
                DateTime endOfLastMonth = new DateTime();
                string startOfLastMonthString;
                string endOfLastMonthString;
                if (ddlMonth == 1)
                {
                    startOfLastMonth = new DateTime((ddlYear-1), 12, 1);
                    endOfLastMonth = new DateTime((ddlYear-1), 12, DateTime.DaysInMonth((ddlYear - 1), 12));
                    startOfLastMonthString = startOfLastMonth.ToString("yyyy-MM-dd");
                    endOfLastMonthString = endOfLastMonth.ToString("yyyy-MM-dd");
                }
                else
                {
                    startOfLastMonth = new DateTime(ddlYear, (ddlMonth - 1), 1);
                    endOfLastMonth = new DateTime(ddlYear, (ddlMonth - 1), DateTime.DaysInMonth(ddlYear, (ddlMonth - 1)));
                    startOfLastMonthString = startOfLastMonth.ToString("yyyy-MM-dd");
                    endOfLastMonthString = endOfLastMonth.ToString("yyyy-MM-dd");
                }
                GridView gv = (GridView)GridViewsPanel.FindControl(e.CommandArgument.ToString());
                List<WorkDate> teamMembersWorkDates = new List<WorkDate>();

                foreach (GridViewRow row in gv.Rows)
                {
                    if(row.RowType == DataControlRowType.DataRow)
                    {
                        var user = manager.FindByName(row.Cells[0].Text);
                        teamMembersWorkDates.Add(new WorkDate { userID = user.Id});
                    }
                }

                int teamID = GetCurrentTeamIDFromUser(teamMembersWorkDates[0].userID);
                int accDistrID = GetSelectedPeriodAccountDistributionID(teamID, startOfSelectedPeriodString, endOfSelectedPeriodString);

                int lastWorked = 0;
                DateTime tempDate = startOfSelectedPeriod;
                for (int i = 0; i < teamMembersWorkDates.Count; i++)
                {
                    DeleteAllDatesForUserInMonth(teamMembersWorkDates[i].userID, startOfSelectedPeriodString, endOfSelectedPeriodString);
                    teamMembersWorkDates[i].date = GetLastWorkDateForUser(teamMembersWorkDates[i].userID, startOfLastMonthString, endOfLastMonthString);

                    if(teamMembersWorkDates[i].date != "")
                    {
                        DateTime lastMonthLastWorkDate = DateTime.Parse(teamMembersWorkDates[i].date);
                        //if (lastMonthLastWorkDate >= endOfLastMonth.AddDays(-6) && lastMonthLastWorkDate <= endOfLastMonth)
                        if (lastMonthLastWorkDate == endOfLastMonth)
                        {
                            DayOfWeek lastMonthLastWorkDay = lastMonthLastWorkDate.DayOfWeek;
                            if (lastMonthLastWorkDay == DayOfWeek.Sunday)
                            {
                                continue;
                            }
                            else
                            {
                                DayOfWeek day = tempDate.DayOfWeek;
                                while (day != DayOfWeek.Monday)
                                {
                                    string insertDate = tempDate.ToString("yyyy-MM-dd");
                                    if (day == DayOfWeek.Saturday || day == DayOfWeek.Sunday)
                                    {
                                        string dateType = "h";
                                        InsertDate(teamMembersWorkDates[i].userID, insertDate, dateType, accDistrID);
                                    }
                                    else
                                    {
                                        string dateType = "w";
                                        InsertDate(teamMembersWorkDates[i].userID, insertDate, dateType, accDistrID);
                                    }
                                    tempDate = tempDate.AddDays(1);
                                    day = tempDate.DayOfWeek;
                                }
                                lastWorked = i;
                            }
                        }
                    }
                }

                int tempLastWorked = lastWorked;
                if (tempLastWorked == 0)
                    tempLastWorked = 1;
                else
                    tempLastWorked = 0;

                while (tempDate.Month == ddlMonth)
                {
                    for (int i = tempLastWorked; i < teamMembersWorkDates.Count; i++)
                    {
                        for (int j = 1; j <= 7; j++)
                        {
                            if (tempDate.Month == ddlMonth)
                            {
                                string insertDate = tempDate.ToString("yyyy-MM-dd");
                                DayOfWeek day = tempDate.DayOfWeek;

                                if (day == DayOfWeek.Sunday)
                                {
                                    string dateType = "h";
                                    InsertDate(teamMembersWorkDates[i].userID, insertDate, dateType, accDistrID);
                                    tempDate = tempDate.AddDays(1);
                                    break;
                                }
                                else if (day == DayOfWeek.Saturday)
                                {
                                    string dateType = "h";
                                    InsertDate(teamMembersWorkDates[i].userID, insertDate, dateType, accDistrID);
                                }
                                else
                                {
                                    string dateType = "w";
                                    InsertDate(teamMembersWorkDates[i].userID, insertDate, dateType, accDistrID);
                                }
                                tempDate = tempDate.AddDays(1);
                            }
                            else
                                break;
                        }
                        if (tempDate.Month != ddlMonth)
                            break;
                    }
                    tempLastWorked = 0;
                }
                GridViewsPanel.Controls.Clear();
                FillGridView();
            }

            else if(e.CommandName == "CreateHistory")
            {
                var userStore = new UserStore<IdentityUser>();
                var manager = new UserManager<IdentityUser>(userStore);
                int ddlYear = int.Parse(DropDownYear.SelectedValue);
                int ddlMonth = int.Parse(DropDownMonth.SelectedValue);
                DateTime startOfSelectedPeriod = new DateTime(ddlYear, ddlMonth, 1);
                DateTime endOfSelectedPeriod = new DateTime(ddlYear, ddlMonth, DateTime.DaysInMonth(ddlYear, ddlMonth));
                string startOfSelectedPeriodString = startOfSelectedPeriod.ToString("yyyy-MM-dd");
                string endOfSelectedPeriodString = endOfSelectedPeriod.ToString("yyyy-MM-dd");

                GridView gv = (GridView)GridViewsPanel.FindControl(e.CommandArgument.ToString());
                List<string> teamMembersID = new List<string>();

                foreach (GridViewRow row in gv.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        var user = manager.FindByName(row.Cells[0].Text);
                        teamMembersID.Add(user.Id);
                    }
                }

                int accDistrID = GetSelectedPeriodAccountDistributionIDFromDatesForUser(teamMembersID[0], startOfSelectedPeriodString, endOfSelectedPeriodString);
                if (accDistrID != -1 && accDistrID != -2)
                {
                    List<AccountDistributionAccounts> accountsInDistribution = GetAllAccountIDFromAccountDistributionAccounts(accDistrID);
                    List<AccountDistributionHistory> teamMembersHistory = new List<AccountDistributionHistory>();
                    
                    for (int i = 0; i < teamMembersID.Count; i++)
                    {
                        DeleteAllHistoryForUserInMonth(teamMembersID[i], startOfSelectedPeriodString);
                        List<string> allWorkDaysWorker = GetAllWorkDayTypesInMonthForWorker(teamMembersID[i], startOfSelectedPeriodString, endOfSelectedPeriodString);
                        int totalWorkHoursWorker = 0;

                        for (int j = 0; j < allWorkDaysWorker.Count; j++)
                        {
                            if(allWorkDaysWorker[j] == "w")
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

                            teamMembersHistory.Add(new AccountDistributionHistory { userID = teamMembersID[i], workPeriod = startOfSelectedPeriodString, accountID = accID, totalHours = hoursWorkedOnAcc, accountDistributionID = accDistrID });
                        }
                    }

                    foreach(AccountDistributionHistory accDistHist in teamMembersHistory)
                    {
                        InsertAccountDistributionHistory(accDistHist);
                    }
                }
            }
        }

        private void FillGridView()
        {
            GridViewPanelWarningLabel.Visible = false;
            var userStore = new UserStore<IdentityUser>();
            var manager = new UserManager<IdentityUser>(userStore);
            int ddlYear = int.Parse(DropDownYear.SelectedValue);
            int ddlMonth = int.Parse(DropDownMonth.SelectedValue);
            DateTime startOfSelectedPeriod = new DateTime(ddlYear, ddlMonth, 1);
            DateTime endOfSelectedPeriod = new DateTime(ddlYear, ddlMonth, DateTime.DaysInMonth(ddlYear, ddlMonth));
            string startOfSelectedPeriodString = startOfSelectedPeriod.ToString("yyyy-MM-dd");
            string endOfSelectedPeriodString = endOfSelectedPeriod.ToString("yyyy-MM-dd");

            if (manager.GetRoles(User.Identity.GetUserId()).FirstOrDefault() == "Worker")
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("TeamMembers");

                var user = manager.FindById(User.Identity.GetUserId());
                int accDistrID = -2;
                int teamID = -1;
                string teamName = "";

                if ((ddlMonth == DateTime.Now.Month && ddlYear == DateTime.Now.Year) || (DateTime.Now.Month == 12 && ddlMonth == 1 && ddlYear == (DateTime.Now.Year+1)) || (ddlMonth == (DateTime.Now.Month+1) && ddlYear == DateTime.Now.Year))
                {
                    teamID = GetCurrentTeamIDFromUser(user.Id);
                    teamName = GetTeamName(teamID);
                    accDistrID = GetSelectedPeriodAccountDistributionID(teamID, startOfSelectedPeriodString, endOfSelectedPeriodString);

                    List<string> teamMembers = GetCurrentTeamMembers(teamID);
                    if (teamMembers.Count != 0)
                    {
                        foreach (string teamMember in teamMembers)
                        {
                            DataRow dr = dt.NewRow();
                            dr["TeamMembers"] = teamMember;
                            dt.Rows.Add(dr);
                        }
                    }
                    else
                    {
                        accDistrID = -1;
                    }
                }
                else if ((ddlYear == DateTime.Now.Year && ddlMonth < DateTime.Now.Month) || ddlYear < DateTime.Now.Year)
                {
                    accDistrID = GetSelectedPeriodAccountDistributionIDFromDatesForUser(user.Id, startOfSelectedPeriodString, endOfSelectedPeriodString);
                    if(accDistrID > 0)
                    {
                        teamID = GetTeamIDFromAccountDistribution(accDistrID);
                        teamName = GetTeamName(teamID);

                        List<string> teamMembersID = GetAllTeamMembersIDFromDates(accDistrID, startOfSelectedPeriodString, endOfSelectedPeriodString);
                        if (teamMembersID.Count != 0)
                        {
                            foreach (string teamMemberID in teamMembersID)
                            {
                                var tempUser = manager.FindById(teamMemberID);
                                DataRow dr = dt.NewRow();
                                dr["TeamMembers"] = tempUser.UserName;
                                dt.Rows.Add(dr);
                            }
                        }
                        else
                        {
                            accDistrID = -1;
                        }
                    }
                }
                else
                {
                    accDistrID = -3;
                }


                if (accDistrID == -2)
                {
                    GridViewPanelWarningLabel.Text = "Something has gone wrong!";
                    GridViewPanelWarningLabel.ForeColor = Color.Red;
                    GridViewPanelWarningLabel.Visible = true;
                }
                else if (accDistrID == -1)
                {
                    GridViewPanelWarningLabel.Text = "No available Account Distributions for this period!";
                    GridViewPanelWarningLabel.ForeColor = Color.Red;
                    GridViewPanelWarningLabel.Visible = true;
                }
                else if (accDistrID == -3)
                {
                    GridViewPanelWarningLabel.Text = "You cant look that far into the future!";
                    GridViewPanelWarningLabel.ForeColor = Color.Red;
                    GridViewPanelWarningLabel.Visible = true;
                }
                else
                {
                    DateTime tempDate = startOfSelectedPeriod;
                    while (tempDate.Month == ddlMonth)
                    {
                        DataColumn dc = new DataColumn();
                        dc.ColumnName = tempDate.Day.ToString() + " " + tempDate.ToString("ddd", CultureInfo.CreateSpecificCulture("en-US"));
                        dt.Columns.Add(dc.ColumnName);
                        tempDate = tempDate.AddDays(1);
                    }

                    GridView gv = new GridView();
                    gv.ID = "gv-" + user.UserName + "(" + accDistrID.ToString() + ")";
                    gv.ClientIDMode = ClientIDMode.Static;
                    gv.DataSource = dt;
                    gv.RowDataBound += new GridViewRowEventHandler(ScheduleGridView_RowDataBoundWorker);
                    GridViewsPanel.Controls.Add(gv);
                    gv.DataBind();
                    GridViewsPanel.Controls.Add(new LiteralControl("<br/>"));

                    GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);
                    TableHeaderCell MYHeader = new TableHeaderCell();
                    MYHeader.Text = teamName;
                    MYHeader.ColumnSpan = dt.Columns.Count;
                    MYHeader.BackColor = Color.LightSkyBlue;
                    row.Controls.Add(MYHeader);
                    gv.FooterStyle.BackColor = Color.LightSteelBlue;
                    gv.HeaderRow.Parent.Controls.AddAt(0, row);
                }
            }
            else
            {
                List<AccountDistribution> accountDistributions = GetAllAcccountDistributionsForSelectedPeriod(startOfSelectedPeriodString, endOfSelectedPeriodString);
                int noDatesCheck = 0;
                if(accountDistributions.Count != 0)
                {
                    foreach(AccountDistribution accDistr in accountDistributions)
                    {
                        DataTable dt = new DataTable();
                        dt.Columns.Add("TeamMembers");

                        string teamName = GetTeamName(accDistr.teamID);

                        if ((ddlMonth == DateTime.Now.Month && ddlYear == DateTime.Now.Year) || (DateTime.Now.Month == 12 && ddlMonth == 1 && ddlYear == (DateTime.Now.Year + 1)) || (ddlMonth == (DateTime.Now.Month + 1) && ddlYear == DateTime.Now.Year))
                        {
                            List<string> teamMembers = GetCurrentTeamMembers(accDistr.teamID);
                            if (teamMembers.Count != 0)
                            {
                                foreach (string teamMember in teamMembers)
                                {
                                    DataRow dr = dt.NewRow();
                                    dr["TeamMembers"] = teamMember;
                                    dt.Rows.Add(dr);
                                }
                            }
                            else
                            {
                                noDatesCheck++;
                                continue;
                            }
                        }
                        else if((ddlYear == DateTime.Now.Year && ddlMonth < DateTime.Now.Month) || ddlYear < DateTime.Now.Year)
                        {
                            List<string> teamMembersID = GetAllTeamMembersIDFromDates(accDistr.teamID, startOfSelectedPeriodString, endOfSelectedPeriodString);
                            if (teamMembersID.Count != 0)
                            {
                                foreach (string teamMemberID in teamMembersID)
                                {
                                    var tempUser = manager.FindById(teamMemberID);
                                    DataRow dr = dt.NewRow();
                                    dr["TeamMembers"] = tempUser.UserName;
                                    dt.Rows.Add(dr);
                                }
                            }
                            else
                            {
                                noDatesCheck++;
                                continue;
                            }
                        }
                        else
                        {
                            GridViewPanelWarningLabel.Text = "You cant look that far into the future!";
                            GridViewPanelWarningLabel.ForeColor = Color.Red;
                            GridViewPanelWarningLabel.Visible = true;
                            break;
                        }

                        DateTime tempDate = startOfSelectedPeriod;
                        while (tempDate.Month == ddlMonth)
                        {
                            DataColumn dc = new DataColumn();
                            dc.ColumnName = tempDate.Day.ToString() + " " + tempDate.ToString("ddd", CultureInfo.CreateSpecificCulture("en-US"));
                            dt.Columns.Add(dc.ColumnName);
                            tempDate = tempDate.AddDays(1);
                        }

                        GridView gv = new GridView();
                        gv.ID = "gv-" + teamName + "(" + accDistr.id.ToString() + ")";
                        gv.ClientIDMode = ClientIDMode.Static;
                        gv.DataSource = dt;
                        gv.RowDataBound += new GridViewRowEventHandler(ScheduleGridView_RowDataBoundAdmin);
                        gv.RowCommand += new GridViewCommandEventHandler(ScheduleGridView_RowCommand);
                        GridViewsPanel.Controls.Add(gv);
                        if ((ddlMonth == DateTime.Now.Month && ddlYear == DateTime.Now.Year) || (DateTime.Now.Month == 12 && ddlMonth == 1 && ddlYear == (DateTime.Now.Year + 1)) || (ddlMonth == (DateTime.Now.Month + 1) && ddlYear == DateTime.Now.Year) || (ddlMonth == (DateTime.Now.Month - 1) && ddlYear == DateTime.Now.Year) || (DateTime.Now.Month == 1 && ddlMonth == 12 && ddlYear == (DateTime.Now.Year - 1)))
                        {
                            gv.ShowFooter = true;
                        }
                        else
                        {
                            gv.ShowFooter = false;
                        }
                        gv.DataBind();
                        GridViewsPanel.Controls.Add(new LiteralControl("<br/>"));

                        GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);
                        TableHeaderCell MYHeader = new TableHeaderCell();
                        MYHeader.Text = teamName;
                        MYHeader.ColumnSpan = dt.Columns.Count;
                        MYHeader.BackColor = Color.LightSkyBlue;
                        row.Controls.Add(MYHeader);
                        gv.FooterStyle.BackColor = Color.LightSteelBlue;
                        gv.HeaderRow.Parent.Controls.AddAt(0, row);
                    }
                }
                else if(accountDistributions.Count == 0)
                {
                    GridViewPanelWarningLabel.Text = "No available Account Distributions for this period!";
                    GridViewPanelWarningLabel.ForeColor = Color.Red;
                    GridViewPanelWarningLabel.Visible = true;
                }
                else
                {
                    GridViewPanelWarningLabel.Text = "Something has gone wrong!";
                    GridViewPanelWarningLabel.ForeColor = Color.Red;
                    GridViewPanelWarningLabel.Visible = true;
                }

                if(noDatesCheck == accountDistributions.Count)
                {
                    GridViewPanelWarningLabel.Text = "Noone worked in this period!";
                    GridViewPanelWarningLabel.ForeColor = Color.Red;
                    GridViewPanelWarningLabel.Visible = true;
                }
            }
        }

        public void dateType_IndexChanged(Object sender, EventArgs e)
        {
            var userStore = new UserStore<IdentityUser>();
            var manager = new UserManager<IdentityUser>(userStore);
            DropDownList ddl = (DropDownList)sender;
            GridViewRow row = (GridViewRow)ddl.NamingContainer;

            string SaccDistrID = row.NamingContainer.ID.ToString();
            int found = SaccDistrID.IndexOf("(");
            SaccDistrID = SaccDistrID.Substring(found + 1).Replace(")", "");
            int accDistrID = Int32.Parse(SaccDistrID);

            var user = manager.FindByName(row.Cells[0].Text);
            string date = ddl.ID;
            found = date.IndexOf("(");
            date = date.Substring(found + 1).Replace(")", "");
            string dateType = ddl.SelectedValue;
            if (dateType == "")
            {
                DeleteDate(date, user.Id);
            }
            else
            {
                DeleteDate(date, user.Id);
                InsertDate(user.Id, date, dateType, accDistrID);
            }

            GridViewsPanel.Controls.Clear();
            FillGridView();
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
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        protected void SelectBtnClick(object sender, EventArgs e)
        {
            GridViewsPanel.Controls.Clear();
            FillGridView();
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
            }

            FillGridView();
        }

        protected void SignOut(object sender, EventArgs e)
        {
            var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.SignOut();
            Response.Redirect("~/Default.aspx");
        }
    }
}
