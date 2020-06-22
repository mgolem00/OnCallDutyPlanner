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
        private int[] CountWorkdaysPerWorker(int workerCount, int year, int month, int startDay)
        {
            int[] teamMemberWorkDays = new int[workerCount];
            int days = DateTime.DaysInMonth(year, month);
            int temp = days - startDay + 1;
            for (int i = temp; i > 0; i--)
            {
                teamMemberWorkDays[(temp - i) % workerCount]++;
            }

            return teamMemberWorkDays;
        }

        private bool IsRecipeActive(int recipeID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                bool isActve = false;
                var queryString = "SELECT Active FROM Recipe WHERE RecipeID = @recipeID";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@recipeID", recipeID);

                try
                {
                    connection.Open();
                    string result = (string)command.ExecuteScalar();
                    if(result == "True")
                    {
                        isActve = true;
                    }
                    connection.Close();
                    return isActve;
                }
                catch (Exception ex)
                {
                    //ErrorEditLiteral.Text = string.Format(ex.Message);
                    return false;
                }

            }
        }

        private List<int> GetRecipeIDForUserByDates(string userID, int year, int month)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                DateTime date = DateTime.Parse(year.ToString() + "." + month.ToString() + ".1");
                string startDate = date.ToString("yyyy-MM-dd");
                int nextMonth = month + 1;
                date = DateTime.Parse(year.ToString() + "." + nextMonth.ToString() + ".1");
                string endDate = date.ToString("yyyy-MM-dd");
                List<int> recipesID = new List<int>();

                var queryString = "SELECT DISTINCT fk_RecipeID FROM Dates WHERE fk_UserID = @userID AND Date >= @startDate AND  Date < @endDate";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@userID", userID);
                command.Parameters.AddWithValue("@startDate", startDate);
                command.Parameters.AddWithValue("@endDate", endDate);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            recipesID.Add(reader.GetInt32(0));
                        }
                    }
                    reader.Close();
                    connection.Close();
                    return recipesID;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        private List<string> GetRecipeStartEndDate(int recipeID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                List<string> recipeStartEnd = new List<string>();
                var queryString = "SELECT DateCreated, DateFinished FROM Recipe WHERE RecipeID = @recipeID;";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@recipeID", recipeID);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            recipeStartEnd.Add(reader.GetDateTime(0).ToString());
                            if(reader[1] == DBNull.Value)
                            {
                                recipeStartEnd.Add("");
                            }
                            else
                            {
                                recipeStartEnd.Add(reader.GetDateTime(1).ToString());
                            }
                        }
                    }
                    reader.Close();
                    connection.Close();
                    return recipeStartEnd;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        private List<string> GetActiveRecipeIDAndDateCreatedForTeam(int teamID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                List<string> recipeIDDateCreated = new List<string>();
                var queryString = "SELECT RecipeID, DateCreated FROM Recipe WHERE fk_SLATeamID = @teamID AND Active='True';";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@teamID", teamID);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            recipeIDDateCreated.Add(reader.GetInt32(0).ToString());
                            recipeIDDateCreated.Add(reader.GetDateTime(1).ToString());
                        }
                    }
                    reader.Close();
                    connection.Close();
                    return recipeIDDateCreated;
                }
                catch (Exception ex)
                {
                    //ErrorEditLiteral.Text = string.Format(ex.Message);
                    return null;
                }

            }
        }

        private string GetDateType(string date, string userID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string dateType;
                var queryString = "SELECT DateType FROM Dates WHERE Date = @date AND fk_UserID = @userID;";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@date", date);
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

        private void InsertDateAutomatic(string date, string userID, int recipeID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                DateTime dateD = DateTime.Parse(date);
                DayOfWeek day = dateD.DayOfWeek;
                string dateType;
                if ((day == DayOfWeek.Saturday) || (day == DayOfWeek.Sunday))
                {
                    dateType = "h";
                }
                else
                {
                    dateType = "w";
                }
                var queryString = "INSERT INTO Dates(Date, DateType, fk_UserID, fk_RecipeID) VALUES(@date, @dateType, @userID, @recipeID)";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@date", date);
                command.Parameters.AddWithValue("@dateType", dateType);
                command.Parameters.AddWithValue("@userID", userID);
                command.Parameters.AddWithValue("@recipeID", recipeID);

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

        private void InsertDate(string date, string dateType, string userID, int recipeID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "INSERT INTO Dates(Date, DateType, fk_UserID, fk_RecipeID) VALUES(@date, @dateType, @userID, @recipeID)";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@date", date);
                command.Parameters.AddWithValue("@dateType", dateType);
                command.Parameters.AddWithValue("@userID", userID);
                command.Parameters.AddWithValue("@recipeID", recipeID);

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
                var queryString = "DELETE FROM Dates WHERE Date = @date AND fk_UserID = @userID";
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
                    //ErrorEditLiteral.Text = string.Format(ex.Message);
                }
                finally
                {
                    connection.Close();
                }

            }
        }

        private List<string> GetAllTeamMembersIDFromRecipe(int recipeID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "SELECT DISTINCT fk_UserID FROM Dates WHERE fk_RecipeID = @recipeID";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@recipeID", recipeID);
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
                    //ErrorEditLiteral.Text = string.Format(ex.Message);
                    return null;
                }
            }
        }

        private List<string> GetAllTeamMembersFromTeams(int teamID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "SELECT UserName FROM AspNetUsers WHERE fk_SLATeamID = @teamID";
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
                    //ErrorEditLiteral.Text = string.Format(ex.Message);
                    return null;
                }
            }
        }

        private string GetTeamName(int teamID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string teamName;
                var queryString = "SELECT Name FROM SLATeams WHERE SLATeamID = @teamID";
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

        private int GetTeamIDFromRecipe(int recipeID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                int teamID;
                var queryString = "SELECT fk_SLATeamID FROM Recipe WHERE RecipeID = @recipeID";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@recipeID", recipeID);

                try
                {
                    connection.Open();
                    teamID = (Int32)command.ExecuteScalar();
                    connection.Close();
                    return teamID;

                }
                catch (Exception ex)
                {
                    //ErrorEditLiteral.Text = string.Format(ex.Message);
                    return -1;
                }

            }
        }

        private int? GetTeamIDFromUser(string userID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                int? teamID;
                var queryString = "SELECT fk_SLATeamID FROM AspNetUsers WHERE Id = @userID";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@userID", userID);

                try
                {
                    connection.Open();
                    teamID = (Int32)command.ExecuteScalar();
                    connection.Close();
                    return teamID;

                }
                catch (Exception ex)
                {
                    //ErrorEditLiteral.Text = string.Format(ex.Message);
                    return -1;
                }

            }
        }

        private List<int> GetAllRecipesID(int year, int month)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                DateTime date = DateTime.Parse(year.ToString() + "." + month.ToString() + ".1");
                string startDate = date.ToString("yyyy-MM-dd");
                int nextMonth = month + 1;
                date = DateTime.Parse(year.ToString() + "." + nextMonth.ToString() + ".1");
                string endDate = date.ToString("yyyy-MM-dd");
                List<int> recipesID = new List<int>();

                var queryString = "SELECT RecipeID FROM Recipe WHERE DateCreated < @endDate AND (DateFinished >= @startDate OR DateFinished IS NULL);";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@startDate", startDate);
                command.Parameters.AddWithValue("@endDate", endDate);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            recipesID.Add(reader.GetInt32(0));
                        }
                    }
                    reader.Close();
                    connection.Close();
                    return recipesID;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        private List<int> GetAllActiveRecipesID()
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                List<int> recipesID = new List<int>();

                var queryString = "SELECT RecipeID FROM Recipe WHERE Active = 'True';";
                SqlCommand command = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            recipesID.Add(reader.GetInt32(0));
                        }
                    }
                    reader.Close();
                    connection.Close();
                    return recipesID;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        private void FillGridView()
        {
            var userStore = new UserStore<IdentityUser>();
            var manager = new UserManager<IdentityUser>(userStore);
            int DDYear = int.Parse(DropDownYear.SelectedValue);
            int DDMonth = int.Parse(DropDownMonth.SelectedValue);
            DateTime date; //= new DateTime(DDYear, DDMonth, 1);
            int i = 1;

            DataRow dr = null;
            DataColumn dc = null;
            string str = string.Empty;

            if (manager.GetRoles(User.Identity.GetUserId()).FirstOrDefault() == "Worker")
            {
                var user = manager.FindById(User.Identity.GetUserId());
                if (DDMonth == DateTime.Now.Month && DateTime.Now.Year == DDYear)
                {
                    List<int> recipeIDList = GetRecipeIDForUserByDates(user.Id, DDYear, DDMonth);

                    if (recipeIDList.Count != 0)
                    {
                        foreach (int recipeID in recipeIDList)
                        {
                            DataTable dt = new DataTable("gridview");
                            dt.Columns.Add("TeamMembers");
                            int teamID = GetTeamIDFromRecipe(recipeID);
                            List<string> usersID = GetAllTeamMembersIDFromRecipe(recipeID);
                            List<string> recipeStartEnd = GetRecipeStartEndDate(recipeID);
                            List<string> teamMembers = new List<string>();
                            foreach (string userID in usersID)
                            {
                                teamMembers.Add(manager.FindById(userID).UserName);
                            }
                            string team = GetTeamName(teamID);

                            foreach (string teamMember in teamMembers)
                            {
                                dr = dt.NewRow();
                                dr["TeamMembers"] = teamMember;
                                dt.Rows.Add(dr);
                            }

                            DateTime recipeStart = DateTime.Parse(recipeStartEnd[0]);
                            DateTime recipeEnd;
                            if (DDMonth != recipeStart.Month || DDYear != recipeStart.Year)
                            {
                                i = 1;
                            }
                            else
                            {
                                i = recipeStart.Day;
                            }
                            date = new DateTime(DDYear, DDMonth, i);
                            if (recipeStartEnd[1] != "")
                            {
                                recipeEnd = DateTime.Parse(recipeStartEnd[1]);
                                recipeEnd = recipeEnd.AddDays(-1);
                                while (date.Day <= recipeEnd.Day)
                                {
                                    str = i.ToString() + " " + date.ToString("ddd", CultureInfo.CreateSpecificCulture("en-US"));
                                    dc = new DataColumn();
                                    dc.ColumnName = str;
                                    dt.Columns.Add(dc.ColumnName);
                                    i++;
                                    date = date.AddDays(1);
                                }
                            }
                            else
                            {
                                while (date.Month == DDMonth)
                                {
                                    str = i.ToString() + " " + date.ToString("ddd", CultureInfo.CreateSpecificCulture("en-US"));
                                    dc = new DataColumn();
                                    dc.ColumnName = str;
                                    dt.Columns.Add(dc);
                                    i++;
                                    date = date.AddDays(1);
                                }
                            }

                            GridView gv = new GridView();
                            gv.ID = "gv-" + user.UserName + "(" + recipeID.ToString() + ")";
                            gv.ClientIDMode = ClientIDMode.Static;
                            gv.DataSource = dt;
                            gv.RowDataBound += new GridViewRowEventHandler(ScheduleGridView_RowDataBoundWorker);
                            GridViewsPanel.Controls.Add(gv);
                            gv.DataBind();
                            GridViewsPanel.Controls.Add(new LiteralControl("<br/>"));

                            GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);
                            TableHeaderCell MYHeader = new TableHeaderCell();
                            MYHeader.Text = team;
                            MYHeader.ColumnSpan = dt.Columns.Count;
                            MYHeader.BackColor = Color.Aquamarine;
                            row.Controls.Add(MYHeader);
                            gv.HeaderRow.Parent.Controls.AddAt(0, row);
                        }
                    }
                    else
                    {
                        DataTable dt = new DataTable("gridview");
                        dt.Columns.Add("TeamMembers");
                        int? NteamID = GetTeamIDFromUser(user.Id);
                        if(NteamID != null)
                        {
                            int teamID = (Int32)NteamID;
                            string team = GetTeamName(teamID);
                            List<string> recipeIDDateCreated = GetActiveRecipeIDAndDateCreatedForTeam(teamID);
                            if(recipeIDDateCreated.Count != 0)
                            {
                                int recipeID = Int32.Parse(recipeIDDateCreated[0]);
                                List<string> teamMembers = GetAllTeamMembersFromTeams(teamID);

                                foreach (string teamMember in teamMembers)
                                {
                                    dr = dt.NewRow();
                                    dr["TeamMembers"] = teamMember;
                                    dt.Rows.Add(dr);
                                }

                                DateTime recipeStart = DateTime.Parse(recipeIDDateCreated[1]);
                                i = recipeStart.Day;
                                date = new DateTime(DDYear, DDMonth, i);
                                while (date.Month == DDMonth)
                                {
                                    str = i.ToString() + " " + date.ToString("ddd", CultureInfo.CreateSpecificCulture("en-US"));
                                    dc = new DataColumn();
                                    dc.ColumnName = str;
                                    dt.Columns.Add(dc);
                                    i++;
                                    date = date.AddDays(1);
                                }

                                int day = recipeStart.Day;
                                int[] teamMemberWorkDays = CountWorkdaysPerWorker(teamMembers.Count, DDYear, DDMonth, day);
                                while (!Array.TrueForAll(teamMemberWorkDays, temp => temp == 0))
                                {
                                    for (int z = 0; z < teamMemberWorkDays.Length; z++)
                                    {
                                        string teamMember = teamMembers[z];
                                        if (teamMemberWorkDays[z] != 0 && teamMemberWorkDays[z] >= 7)
                                        {
                                            for (int y = 0; y < 7; y++)
                                            {
                                                string dateInsert = DDYear.ToString() + "-" + DDMonth.ToString() + "-" + day.ToString();
                                                InsertDateAutomatic(dateInsert, manager.FindByName(teamMember).Id, recipeID);
                                                day++;
                                                teamMemberWorkDays[z]--;
                                            }
                                        }
                                        else
                                        {
                                            while (teamMemberWorkDays[z] != 0)
                                            {
                                                string dateInsert = DDYear.ToString() + "-" + DDMonth.ToString() + "-" + day.ToString();
                                                InsertDateAutomatic(dateInsert, manager.FindByName(teamMember).Id, recipeID);
                                                day++;
                                                teamMemberWorkDays[z]--;
                                            }
                                        }
                                    }
                                }

                                GridView gv = new GridView();
                                gv.ID = "gv-" + user.UserName + "(" + recipeID.ToString() + ")";
                                gv.ClientIDMode = ClientIDMode.Static;
                                gv.DataSource = dt;
                                gv.RowDataBound += new GridViewRowEventHandler(ScheduleGridView_RowDataBoundWorker);
                                GridViewsPanel.Controls.Add(gv);
                                gv.DataBind();
                                GridViewsPanel.Controls.Add(new LiteralControl("<br/>"));

                                GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);
                                TableHeaderCell MYHeader = new TableHeaderCell();
                                MYHeader.Text = team;
                                MYHeader.ColumnSpan = dt.Columns.Count;
                                MYHeader.BackColor = Color.Aquamarine;
                                row.Controls.Add(MYHeader);
                                gv.HeaderRow.Parent.Controls.AddAt(0, row);
                            }
                        }
                    }
                }

                else
                {
                    List<int> recipeIDList = GetRecipeIDForUserByDates(user.Id, DDYear, DDMonth);

                    if (recipeIDList.Count != 0)
                    {
                        foreach (int recipeID in recipeIDList)
                        {
                            DataTable dt = new DataTable("gridview");
                            dt.Columns.Add("TeamMembers");
                            int teamID = GetTeamIDFromRecipe(recipeID);
                            List<string> usersID = GetAllTeamMembersIDFromRecipe(recipeID);
                            List<string> recipeStartEnd = GetRecipeStartEndDate(recipeID);
                            List<string> teamMembers = new List<string>();
                            foreach (string userID in usersID)
                            {
                                teamMembers.Add(manager.FindById(userID).UserName);
                            }
                            string team = GetTeamName(teamID);

                            foreach (string teamMember in teamMembers)
                            {
                                dr = dt.NewRow();
                                dr["TeamMembers"] = teamMember;
                                dt.Rows.Add(dr);
                            }

                            DateTime recipeStart = DateTime.Parse(recipeStartEnd[0]);
                            DateTime recipeEnd;
                            i = recipeStart.Day;
                            date = new DateTime(DDYear, DDMonth, i);
                            if (recipeStartEnd[1] != "")
                            {
                                recipeEnd = DateTime.Parse(recipeStartEnd[1]);
                                if(date.Month == recipeEnd.Month)
                                {
                                    while (date.Day <= recipeEnd.Day)
                                    {
                                        str = i.ToString() + " " + date.ToString("ddd", CultureInfo.CreateSpecificCulture("en-US"));
                                        dc = new DataColumn();
                                        dc.ColumnName = str;
                                        dt.Columns.Add(dc);
                                        i++;
                                        date = date.AddDays(1);
                                    }
                                }
                                else
                                {
                                    while (date.Month == DDMonth)
                                    {
                                        str = i.ToString() + " " + date.ToString("ddd", CultureInfo.CreateSpecificCulture("en-US"));
                                        dc = new DataColumn();
                                        dc.ColumnName = str;
                                        dt.Columns.Add(dc);
                                        i++;
                                        date = date.AddDays(1);
                                    }
                                }
                            }
                            else
                            {
                                while (date.Month == DDMonth)
                                {
                                    str = i.ToString() + " " + date.ToString("ddd", CultureInfo.CreateSpecificCulture("en-US"));
                                    dc = new DataColumn();
                                    dc.ColumnName = str;
                                    dt.Columns.Add(dc);
                                    i++;
                                    date = date.AddDays(1);
                                }
                            }

                            GridView gv = new GridView();
                            gv.ID = "gv-" + user.UserName + "(" + recipeID.ToString() + ")";
                            gv.ClientIDMode = ClientIDMode.Static;
                            gv.DataSource = dt;
                            gv.RowDataBound += new GridViewRowEventHandler(ScheduleGridView_RowDataBoundWorker);
                            GridViewsPanel.Controls.Add(gv);
                            gv.DataBind();
                            GridViewsPanel.Controls.Add(new LiteralControl("<br/>"));

                            GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);
                            TableHeaderCell MYHeader = new TableHeaderCell();
                            MYHeader.Text = team;
                            MYHeader.ColumnSpan = dt.Columns.Count;
                            MYHeader.BackColor = Color.Aquamarine;
                            row.Controls.Add(MYHeader);
                            gv.HeaderRow.Parent.Controls.AddAt(0, row);
                        }
                    }
                }
            }
            else
            {
                DateTime dateCheck = new DateTime(DDYear, DDMonth, 1);
                if ((dateCheck.Year == DateTime.Now.Year && dateCheck.Month <= DateTime.Now.Month) || dateCheck.Year < DateTime.Now.Year)
                {
                    List<int> allRecipesForAMonth = GetAllRecipesID(DDYear, DDMonth);
                    if (allRecipesForAMonth.Count != 0)
                    {
                        foreach (int recipeID in allRecipesForAMonth)
                        {
                            DataTable dt = new DataTable("gridview");
                            dt.Columns.Add("TeamMembers");
                            List<string> recipeStartEnd = GetRecipeStartEndDate(recipeID);
                            List<string> usersID = GetAllTeamMembersIDFromRecipe(recipeID);
                            int teamID = GetTeamIDFromRecipe(recipeID);
                            string team = GetTeamName(teamID);

                            List<string> teamMembers = new List<string>();
                            foreach (string userID in usersID)
                            {
                                teamMembers.Add(manager.FindById(userID).UserName);
                            }

                            foreach (string teamMember in teamMembers)
                            {
                                dr = dt.NewRow();
                                dr["TeamMembers"] = teamMember;
                                dt.Rows.Add(dr);
                            }

                            //generate calendar days
                            DateTime recipeStart = DateTime.Parse(recipeStartEnd[0]);
                            DateTime recipeEnd;
                            if (DDMonth != recipeStart.Month || DDYear != recipeStart.Year)
                            {
                                i = 1;
                            }
                            else
                            {
                                i = recipeStart.Day;
                            }
                            date = new DateTime(DDYear, DDMonth, i);

                            if (recipeStartEnd[1] != "")
                            {
                                recipeEnd = DateTime.Parse(recipeStartEnd[1]);
                                recipeEnd = recipeEnd.AddDays(-1);
                                if (date.Month == recipeEnd.Month)
                                {
                                    while (date.Day <= recipeEnd.Day)
                                    {
                                        str = i.ToString() + " " + date.ToString("ddd", CultureInfo.CreateSpecificCulture("en-US"));
                                        dc = new DataColumn();
                                        dc.ColumnName = str;
                                        dt.Columns.Add(dc);
                                        i++;
                                        date = date.AddDays(1);
                                    }
                                }
                                else
                                {
                                    while (date.Month == DDMonth)
                                    {
                                        str = i.ToString() + " " + date.ToString("ddd", CultureInfo.CreateSpecificCulture("en-US"));
                                        dc = new DataColumn();
                                        dc.ColumnName = str;
                                        dt.Columns.Add(dc);
                                        i++;
                                        date = date.AddDays(1);
                                    }
                                }
                            }
                            else
                            {
                                while (date.Month == DDMonth)
                                {
                                    str = i.ToString() + " " + date.ToString("ddd", CultureInfo.CreateSpecificCulture("en-US"));
                                    dc = new DataColumn();
                                    dc.ColumnName = str;
                                    dt.Columns.Add(dc);
                                    i++;
                                    date = date.AddDays(1);
                                }
                            }

                            GridView gv = new GridView();
                            gv.ID = "gv-" + team + "(" + recipeID.ToString() + ")";
                            gv.DataSource = dt;
                            gv.RowDataBound += new GridViewRowEventHandler(ScheduleGridView_RowDataBoundAdmin);
                            GridViewsPanel.Controls.Add(gv);
                            gv.DataBind();
                            GridViewsPanel.Controls.Add(new LiteralControl("<br/>"));

                            GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);
                            TableHeaderCell MYHeader = new TableHeaderCell();
                            MYHeader.Text = team;
                            MYHeader.ColumnSpan = dt.Columns.Count;
                            MYHeader.BackColor = Color.Aquamarine;
                            row.Controls.Add(MYHeader);
                            gv.HeaderRow.Parent.Controls.AddAt(0, row);
                        }
                    }
                }
                else
                {
                    List<int> allActiveRecipesID = GetAllActiveRecipesID();
                    if(allActiveRecipesID.Count != 0)
                    {
                        foreach(int recipeID in allActiveRecipesID)
                        {
                            DataTable dt = new DataTable("gridview");
                            dt.Columns.Add("TeamMembers");
                            int teamID = GetTeamIDFromRecipe(recipeID);
                            string team = GetTeamName(teamID);
                            List<string> teamMembers = GetAllTeamMembersFromTeams(teamID);

                            foreach (string teamMember in teamMembers)
                            {
                                dr = dt.NewRow();
                                dr["TeamMembers"] = teamMember;
                                dt.Rows.Add(dr);
                            }

                            date = new DateTime(DDYear, DDMonth, 1);
                            i = 1;
                            while (date.Month == DDMonth)
                            {
                                str = i.ToString() + " " + date.ToString("ddd", CultureInfo.CreateSpecificCulture("en-US"));
                                dc = new DataColumn();
                                dc.ColumnName = str;
                                dt.Columns.Add(dc);
                                i++;
                                date = date.AddDays(1);
                            }

                            GridView gv = new GridView();
                            gv.ID = "gv-" + team + "(" + recipeID.ToString() + ")";
                            gv.DataSource = dt;
                            gv.RowDataBound += new GridViewRowEventHandler(ScheduleGridView_RowDataBoundAdmin);
                            GridViewsPanel.Controls.Add(gv);
                            gv.DataBind();
                            GridViewsPanel.Controls.Add(new LiteralControl("<br/>"));

                            GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);
                            TableHeaderCell MYHeader = new TableHeaderCell();
                            MYHeader.Text = team;
                            MYHeader.ColumnSpan = dt.Columns.Count;
                            MYHeader.BackColor = Color.Aquamarine;
                            row.Controls.Add(MYHeader);
                            gv.HeaderRow.Parent.Controls.AddAt(0, row);
                        }
                    }
                }
            }
        }

        private List<int> GetYears()
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                List<int> year = new List<int>();
                List<int> yearDistinct = new List<int>();
                var queryString = "SELECT DISTINCT Date FROM Dates ORDER BY Date ASC";
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                Welcome.Text = string.Format("Hello, {0}!", User.Identity.GetUserName());

                if (User.IsInRole("Worker"))
                {
                    ProjectsLink.Visible = false;
                    AccDistrLink.Visible = false;
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
                FillGridView();
            }
        }

        protected void ScheduleGridView_RowDataBoundWorker(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string SrecipeID = e.Row.NamingContainer.ID;
                int found = SrecipeID.IndexOf("(");
                SrecipeID = SrecipeID.Substring(found + 1).Replace(")", "");
                int recipeID = Int32.Parse(SrecipeID);
                bool isRecipeActive = IsRecipeActive(recipeID);

                if (e.Row.Cells[0].Text == User.Identity.Name && DateTime.Now.Month.ToString() == DropDownMonth.SelectedValue && DateTime.Now.Year.ToString() == DropDownYear.SelectedValue && isRecipeActive == true)
                {
                    for (int i = 1; i < e.Row.Cells.Count; i++)
                    {
                        if (e.Row.Cells[i].Text == "&nbsp;")
                        {
                            DataControlFieldCell cell = (DataControlFieldCell)e.Row.Cells[i];
                            string dayString = cell.ContainingField.HeaderText;
                            found = dayString.IndexOf(" ");
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
                            found = dayString.IndexOf(" ");
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
                        dateType.SelectedIndexChanged += new EventHandler(dateType_IndexChanged);
                        dateType.AutoPostBack = true;
                        dateType.Items.Add("");
                        dateType.Items.Add("w");
                        dateType.Items.Add("h");
                        dateType.SelectedValue = dateTypeDB;

                        e.Row.Cells[i].Controls.Add(dateType);
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

        public void dateType_IndexChanged(Object sender, EventArgs e)
        {
            var userStore = new UserStore<IdentityUser>();
            var manager = new UserManager<IdentityUser>(userStore);
            DropDownList ddl = (DropDownList)sender;
            GridViewRow row = (GridViewRow)ddl.NamingContainer;

            string SrecipeID = row.NamingContainer.ToString();
            int found = SrecipeID.IndexOf("(");
            SrecipeID = SrecipeID.Substring(found + 1).Replace(")", "");
            int recipeID = Int32.Parse(SrecipeID);

            var user = manager.FindByName(row.Cells[0].Text);
            string date = ddl.ID;
            found = date.IndexOf("(");
            date = date.Substring(found+1).Replace(")", "");
            string dateType = ddl.SelectedValue;
            if(dateType == "")
            {
                DeleteDate(date, user.Id);
            }
            else
            {
                DeleteDate(date, user.Id);
                InsertDate(date, dateType, user.Id, recipeID);
            }
        }

        protected void SelectBtnClick(object sender, EventArgs e)
        {
            GridViewsPanel.Controls.Clear();
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
