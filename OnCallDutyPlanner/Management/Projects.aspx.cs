using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Drawing;

namespace OnCallDutyPlanner
{
    public partial class Projects : System.Web.UI.Page
    {
        class Account
        {
            public int ID { get; set; }
            public string accountNumber { get; set; }
            public string projectName { get; set; }
            public string dateStarted { get; set; }
            public string dateFinished { get; set; }
        }

        private List<Account> GetAllProjects()
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "SELECT ID, ProjectName, AccountNumber, DateCreated, DateFinished FROM Accounts";
                SqlCommand command = new SqlCommand(queryString, connection);
                List<Account> projects = new List<Account>();

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if(!reader.IsDBNull(4))
                            {
                                DateTime startedDB = reader.GetDateTime(3);
                                DateTime endedDB = reader.GetDateTime(4);
                                string SstartedDB = startedDB.ToString("yyyy-MM-dd");
                                string SendedDB = endedDB.ToString("yyyy-MM-dd");
                                projects.Add(new Account { ID = reader.GetInt32(0), projectName = reader.GetString(1), accountNumber = reader.GetString(2), dateStarted = SstartedDB, dateFinished = SendedDB });
                            }
                            else
                            {
                                DateTime startedDB = reader.GetDateTime(3);
                                string SstartedDB = startedDB.ToString("yyyy-MM-dd");
                                projects.Add(new Account { ID = reader.GetInt32(0), projectName = reader.GetString(1), accountNumber = reader.GetString(2), dateStarted = SstartedDB, dateFinished = "---" });
                            }
                        }
                    }
                    reader.Close();
                    connection.Close();
                    return projects;

                }
                catch (Exception ex)
                {
                    ErrorEditLiteral.Text = string.Format(ex.Message);
                    return null;
                }
            }
        }

        private List<string> GetAllTeamsOnProject(int projectID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string endOfCurrentMonth = DateTime.Now.ToString("yyyy-MM") + "-" + DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month).ToString();
                var queryString = "SELECT TeamName FROM SLATeams WHERE SLATeams.ID IN (SELECT SLATeamID FROM AccountDistribution WHERE AccountDistribution.ID IN (SELECT AccountDistributionID FROM AccountDistributionAccounts WHERE AccountID = @projectID) AND (DateFinished IS NULL OR DateFinished = @endOfCurrentMonth));";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@projectID", projectID);
                command.Parameters.AddWithValue("@endOfCurrentMonth", endOfCurrentMonth);
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
                    ErrorEditLiteral.Text = string.Format(ex.Message);
                    return null;
                }
            }
        }

        protected void ListProjects()
        {
            var userStore = new UserStore<IdentityUser>();
            var manager = new UserManager<IdentityUser>(userStore);
            DataTable dt = new DataTable("Projects");
            List<Account> projects = GetAllProjects();

            dt.Columns.Add("lbl_ProjectName");
            dt.Columns.Add("lbl_AccountNumber");
            dt.Columns.Add("lbl_Teams", typeof(List<string>));
            dt.Columns.Add("lbl_DateCreated");
            dt.Columns.Add("lbl_DateFinished");

            if (projects.Count != 0)
            {
                foreach (Account project in projects)
                {
                    DataRow dr = null;
                    dr = dt.NewRow();
                    dr["lbl_ProjectName"] = project.projectName;
                    dr["lbl_AccountNumber"] = project.accountNumber;
                    dr["lbl_DateCreated"] = project.dateStarted;
                    dr["lbl_DateFinished"] = project.dateFinished;

                    List<string> teams = GetAllTeamsOnProject(project.ID);
                    if (teams.Count != 0)
                    {
                        dr["lbl_Teams"] = teams;
                    }
                    else
                    {
                        teams.Add("---");
                        dr["lbl_Teams"] = teams;
                    }

                    dt.Rows.Add(dr);
                }

                ProjectsGridView.DataSource = dt;
                ProjectsGridView.DataBind();
            }
        }

        private int GetProjectID(string projectName)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                int projectID = -1;
                var queryString = "SELECT ID FROM Accounts WHERE ProjectName = @projectName";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@projectName", projectName);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            projectID = reader.GetInt32(0);
                        }
                    }
                    reader.Close();
                    connection.Close();
                    return projectID;
                }
                catch (Exception ex)
                {
                    ErrorEditLiteral.Text = string.Format(ex.Message);
                    return -1;
                }
            }
        }

        private void ChangeProjectName(int projectID, string projectName)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "UPDATE Accounts SET ProjectName = @projectName WHERE ID = @projectID;";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@projectName", projectName);
                command.Parameters.AddWithValue("@projectID", projectID);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    EditWarningLiteral.Text = string.Format(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private void ChangeAccountNumber(int projectID, string accountNumber)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "UPDATE Accounts SET AccountNumber = @accountNumber WHERE ID = @projectID;";
                SqlCommand command = new SqlCommand(queryString, connection);

                command.Parameters.AddWithValue("@accountNumber", accountNumber);
                command.Parameters.AddWithValue("@projectID", projectID);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    EditWarningLiteral.Text = string.Format(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private bool ProjectDetailsTakenCheck(string projectName, string accountNumber)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                bool isTaken = true;
                var queryString = "SELECT ID FROM Accounts WHERE ProjectName = @projectName OR AccountNumber = @accountNumber;";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@projectName", projectName);
                command.Parameters.AddWithValue("@accountNumber", accountNumber);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows == false)
                    {
                        isTaken = false;
                    }
                    reader.Close();
                    connection.Close();
                    return isTaken;

                }
                catch (Exception ex)
                {
                    connection.Close();
                    Console.WriteLine(ex.Message);
                    return true;
                }
            }
        }

        private void EditProject()
        {
            bool isEmpty = false;
            bool isTaken = false;
            bool[] updatesMade = new bool[2];
            string projectName = NewProjectNameTextBox.Text.ToString();
            string accountNumber = NewAccountNumberTextBox.Text.ToString();
            int projectID = GetProjectID(HiddenEditProjectName.Value);

            isEmpty = string.IsNullOrWhiteSpace(NewProjectNameTextBox.Text);
            if (isEmpty == false && projectName != HiddenEditProjectName.Value)
            {
                isTaken = ProjectDetailsTakenCheck(projectName, "---");

                if (isTaken == false)
                {
                    ChangeProjectName(projectID, projectName);
                    updatesMade[0] = true;
                }
                else
                {
                    EditWarningLiteral.Text = string.Format("Project name already taken! Choose another one!");
                }
            }

            isEmpty = string.IsNullOrWhiteSpace(NewAccountNumberTextBox.Text);
            if (isEmpty == false && accountNumber != HiddenEditAccountNumber.Value)
            {
                isTaken = ProjectDetailsTakenCheck("---", accountNumber);

                if (isTaken == false)
                {
                    ChangeAccountNumber(projectID, accountNumber);
                    updatesMade[1] = true;
                }
                else
                {
                    EditWarningLiteral.Text = string.Format("Account Number already taken! Choose another one!");
                }
            }

            if (updatesMade[0] == true && updatesMade[1] == true)
            {
                EditLiteral.Text = string.Format("{0}'s (formerly {1}) name and account number updated!", projectName, HiddenEditProjectName.Value);
            }
            else if (updatesMade[0] == true && updatesMade[1] == false)
            {
                EditLiteral.Text = string.Format("{0}'s (formerly {1}) name updated!", projectName, HiddenEditProjectName.Value);
            }
            else if (updatesMade[0] == false && updatesMade[1] == true)
            {
                EditLiteral.Text = string.Format("{0}'s account number updated!", HiddenEditProjectName.Value);
            }
        }

        private void EndProject(int projectID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string dateFinished = DateTime.Now.ToString("yyyy-MM") + "-" + DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month).ToString();
                var queryString = "UPDATE Accounts SET DateFinished = @dateFinished WHERE ID = @projectID;";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@dateFinished", dateFinished);
                command.Parameters.AddWithValue("@projectID", projectID);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    EditWarningLiteral.Text = string.Format(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        protected void ProjectsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditProject")
            {
                EditProjectPanel.Visible = true;
                ScriptManager.RegisterStartupScript(this, GetType(), "scrollToEdit", "document.getElementById('EditTeamPanel').scrollIntoView({ behavior: 'smooth', block: 'center' });", true);
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                ProjectsGridView.Rows[rowIndex].BackColor = Color.HotPink;
                Label projectName = ProjectsGridView.Rows[rowIndex].FindControl("lbl_ProjectName") as Label;
                Label accountNumber = ProjectsGridView.Rows[rowIndex].FindControl("lbl_AccountNumber") as Label;
                NewProjectNameTextBox.Text = projectName.Text;
                NewAccountNumberTextBox.Text = accountNumber.Text;

                HiddenEditProjectName.Value = projectName.Text;
                HiddenEditAccountNumber.Value = accountNumber.Text;
                HiddenEditRowIndex.Value = rowIndex.ToString();
            }

            if (e.CommandName == "EndProject")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                ProjectsGridView.Rows[rowIndex].BackColor = Color.HotPink;
                ProjectsGridView.Columns[5].Visible = false;
                ProjectsGridView.Columns[6].Visible = true;
                for (int i = 0; i < ProjectsGridView.Rows.Count; i++)
                {
                    if (i == rowIndex)
                    {
                        ProjectsGridView.Rows[rowIndex].Cells[6].Visible = true;
                    }
                    else
                        ProjectsGridView.Rows[i].Cells[6].Visible = false;
                }
            }

            if (e.CommandName == "YesEnd")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                Label projectName = ProjectsGridView.Rows[rowIndex].FindControl("lbl_ProjectName") as Label;
                int projectID = GetProjectID(projectName.Text);

                EndProject(projectID);
                EditLiteral.Text = string.Format("Project {0} successfully ended!", projectName.Text);
                ProjectsGridView.Columns[5].Visible = true;
                ProjectsGridView.Columns[6].Visible = false;
                ListProjects();
            }

            if (e.CommandName == "NoEnd")
            {
                ProjectsGridView.Columns[5].Visible = true;
                ProjectsGridView.Columns[6].Visible = false;
            }
        }

        protected void ProjectsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label dateFinishedLabel = (Label)e.Row.Cells[4].Controls[1];
                if (dateFinishedLabel.Text != "---")
                {
                    e.Row.Cells[5].Visible = false;
                }
            }
        }

        protected void SaveEditProject_Click(object sender, EventArgs e)
        {
            EditProject();
            NewProjectNameTextBox.Text = null;
            NewAccountNumberTextBox.Text = null;
            EditProjectPanel.Visible = false;
            HiddenEditProjectName.Value = null;
            HiddenEditAccountNumber.Value = null;
            HiddenEditRowIndex.Value = null;
            ListProjects();
        }

        protected void ApplyEditProject_Click(object sender, EventArgs e)
        {
            EditProject();
            HiddenEditProjectName.Value = NewProjectNameTextBox.Text;
            HiddenEditAccountNumber.Value = NewAccountNumberTextBox.Text;
            ListProjects();
            int rowIndex = Int32.Parse(HiddenEditRowIndex.Value);
            ProjectsGridView.Rows[rowIndex].BackColor = Color.HotPink;
        }

        protected void CancelEditProject_Click(object sender, EventArgs e)
        {
            NewProjectNameTextBox.Text = null;
            NewAccountNumberTextBox.Text = null;
            EditProjectPanel.Visible = false;
            HiddenEditProjectName.Value = null;
            HiddenEditAccountNumber.Value = null;
            HiddenEditRowIndex.Value = null;
            ListProjects();
        }

        private int CreateProject(string projectName, string accountNumber)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string dateCreated = DateTime.Now.ToString("yyyy-MM") + "-01";
                var queryString = "INSERT INTO Accounts(ProjectName, AccountNumber, DateCreated) VALUES (@projectName, @accountNumber, @dateCreated);";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@projectName", projectName);
                command.Parameters.AddWithValue("@accountNumber", accountNumber);
                command.Parameters.AddWithValue("@dateCreated", dateCreated);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    return 1;
                }
                catch (Exception ex)
                {
                    connection.Close();
                    CreateProjectLiteral.Text = string.Format(ex.Message);
                    return -1;
                }
            }
        }

        protected void OpenCreateProject_Click(object sender, EventArgs e)
        {
            OpenCreateProjectButton.Visible = false;
            CreateProjectPanel.Visible = true;
            ListProjects();
        }

        protected void CancelCreateProject_Click(object sender, EventArgs e)
        {
            ProjectName.Text = null;
            AccountNumber.Text = null;
            OpenCreateProjectButton.Visible = true;
            CreateProjectPanel.Visible = false;
            ListProjects();
        }

        protected void CreateProject_Click(object sender, EventArgs e)
        {
            string projectName = ProjectName.Text;
            string accountNumber = AccountNumber.Text;
            bool isTaken = false;

            isTaken = ProjectDetailsTakenCheck(projectName, accountNumber);

            if (isTaken == false)
            {
                int result = CreateProject(projectName, accountNumber);

                if (result == 1)
                {
                    CreateProjectLiteral.Text = string.Format("{0} was created successfully!", projectName);
                }
                else if (result == -1)
                {
                    CreateProjectLiteral.Text = string.Format("Something has gone horribly wrong! COULD NOT CREATE PROJECT!");
                }
                else
                {
                    CreateProjectLiteral.Text = string.Format("Dont even know what went wrong!");
                }
                ProjectName.Text = null;
                AccountNumber.Text = null;
                OpenCreateProjectButton.Visible = true;
                CreateProjectPanel.Visible = false;
                ListProjects();
            }
            else
            {
                CreateProjectLiteral.Text = string.Format("Project name or Account Number already taken! Choose another one!");
                ProjectName.Text = null;
                AccountNumber.Text = null;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                Welcome.Text = string.Format("Hello, {0}!", User.Identity.GetUserName());
                ListProjects();
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