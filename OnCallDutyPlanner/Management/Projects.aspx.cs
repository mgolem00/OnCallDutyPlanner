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
        private int AddRecipeProject(int recID, int projectID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "INSERT INTO RecipeProject(fk_RecipeID, fk_ProjectID) VALUES (@recID, @projectID);";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@recID", recID);
                command.Parameters.AddWithValue("@projectID", projectID);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    return 1;
                }
                catch (Exception ex)
                {
                    CreateProjectLiteral.Text = string.Format(ex.Message);
                    return -1;
                }
            }
        }

        private int GetTeamID(string teamName)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                int teamID;
                var queryString = "SELECT SLATeamID FROM SLATeams WHERE Name = @teamName";
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
                    ErrorEditLiteral.Text = string.Format(ex.Message);
                    return -1;
                }

            }
        }

        private int? GetCurrentTeamRecipeID(int teamID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                int? recipeID = null;
                var queryString = "SELECT RecipeID FROM Recipe WHERE fk_SLATeamID = @teamID AND Active = 'True'";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@teamID", teamID);

                try
                {
                    connection.Open();
                    //recipeID = (Int32)command.ExecuteScalar();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            recipeID = reader.GetInt32(0);
                        }
                    }
                    connection.Close();
                    return recipeID;
                }
                catch (Exception ex)
                {
                    CreateProjectLiteral.Text = string.Format(ex.Message);
                    return -1;
                }
            }
        }

        private List<int> GetOtherProjectsID(int recipeID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                List<int> otherProjects = new List<int>();
                var queryString = "SELECT fk_ProjectID FROM RecipeProject WHERE fk_RecipeID = @recipeID;";
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
                            otherProjects.Add(reader.GetInt32(0));
                        }
                    }
                    connection.Close();
                    return otherProjects;
                }
                catch (Exception ex)
                {
                    CreateProjectLiteral.Text = string.Format(ex.Message);
                    return null;
                }
            }
        }

        private void SetOldRecipeNotActive(int recipeID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string dateFinished = DateTime.Now.ToString("yyyy-MM-dd");
                var queryString = "UPDATE Recipe SET Active = 'False', DateFinished = @dateFinished WHERE RecipeID = @recipeID;";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@recipeID", recipeID);
                command.Parameters.AddWithValue("@dateFinished", dateFinished);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    //ErrorEditLiteral.Text = string.Format(ex.Message);
                }

            }
        }

        private int GetNumberOfOldProjects(int teamID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                int numOfOldProjects=0;
                var queryString = "SELECT COUNT(RecipeID) FROM Recipe WHERE fk_SLATeamID = @teamID;";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@teamID", teamID);

                try
                {
                    connection.Open();
                    numOfOldProjects = (Int32)command.ExecuteScalar();
                    connection.Close();
                    return numOfOldProjects;
                }
                catch (Exception ex)
                {
                    CreateProjectLiteral.Text = string.Format(ex.Message);
                    return -1;
                }
            }
        }

        private int CreateNewRecipe(int teamID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                int numOfProjects = GetNumberOfOldProjects(teamID) + 1;
                string dateCreated = DateTime.Now.ToString("yyyy-MM-dd");
                string recipeName;
                if(teamID < 1000)
                {
                    if(teamID < 100)
                    {
                        if (teamID < 10)
                        {
                            recipeName = "000" + teamID.ToString() + "-";
                        }
                        else
                        {
                            recipeName = "00" + teamID.ToString() + "-";
                        }
                    }
                    else
                    {
                        recipeName = "0" + teamID.ToString() + "-";
                    }
                }
                else
                {
                    recipeName = teamID.ToString() + "-";
                }
                recipeName += numOfProjects.ToString();

                var queryString = "INSERT INTO Recipe(Name, fk_SLATeamID, Active, DateCreated) VALUES (@recipeName, @teamID, 'True', @dateCreated);";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@recipeName", recipeName);
                command.Parameters.AddWithValue("@teamID", teamID);
                command.Parameters.AddWithValue("@dateCreated", dateCreated);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    CreateProjectLiteral.Text = string.Format(ex.Message);
                }
            }

            int? newRecipeID = GetCurrentTeamRecipeID(teamID);
            return (Int32)newRecipeID;
        }

        private int CreateProject(string projectName, string projectUID, string projectDesc)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string dateCreated = DateTime.Now.ToString("yyyy-MM-dd");
                var queryString = "INSERT INTO Projects(Name, UID, Description, DateCreated) VALUES (@projectName, @projectUID, @projectDesc, @dateCreated);";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@projectName", projectName);
                command.Parameters.AddWithValue("@projectUID", projectUID);
                command.Parameters.AddWithValue("@projectDesc", projectDesc);
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
                    CreateProjectLiteral.Text = string.Format(ex.Message);
                    return -1;
                }
            }
        }

        protected void CreateProject_Click(object sender, EventArgs e)
        {
            string projectName = ProjectName.Text;
            string projectUID = ProjectUID.Text;
            string projectDesc = ProjectDesc.Text;
            bool isNameTaken = false;
            bool isUIDTaken = false;

            for (int i = 0; i < ProjectsGridView.Rows.Count; i++)
            {
                Label teamN = ProjectsGridView.Rows[i].FindControl("lbl_ProjectName") as Label;
                Label projUID = ProjectsGridView.Rows[i].FindControl("lbl_ProjectUID") as Label;
                if (projectName == teamN.Text && projectUID == projUID.Text)
                {
                    isNameTaken = true;
                    isUIDTaken = true;
                    break;
                }
                else if(projectName == teamN.Text && projectUID != projUID.Text)
                {
                    isNameTaken = true;
                    isUIDTaken = false;
                    break;
                }
                else if (projectName != teamN.Text && projectUID == projUID.Text)
                {
                    isNameTaken = false;
                    isUIDTaken = true;
                    break;
                }
                else
                {
                    isNameTaken = false;
                    isUIDTaken = false;
                    break;
                }
            }

            if (isNameTaken == false && isUIDTaken == false)
            {
                int? result = CreateProject(projectName, projectUID, projectDesc);

                foreach (ListItem li in TeamsListBox.Items)
                {
                    if (li.Selected == true)
                    {
                        int teamID = GetTeamID(li.Value);
                        int projectID = GetProjectID(projectName);
                        int? oldRecipeID = GetCurrentTeamRecipeID(teamID);

                        if(oldRecipeID != null)
                        {
                            int oldRecID = (Int32)oldRecipeID;
                            SetOldRecipeNotActive(oldRecID);
                            List<int> otherProjectsID = GetOtherProjectsID(oldRecID);
                            if (otherProjectsID.Count != 0)
                            {
                                int recipeID = CreateNewRecipe(teamID);
                                foreach (int projId in otherProjectsID)
                                {
                                    result = AddRecipeProject(recipeID, projId);
                                }
                                result = AddRecipeProject(recipeID, projectID);
                            }
                        }
                        else
                        {
                            int recipeID = CreateNewRecipe(teamID);
                            result = AddRecipeProject(recipeID, projectID);
                        }
                    }
                }

                if (result == 1)
                {
                    CreateProjectLiteral.Text = string.Format("{0} was created successfully!", projectName);
                }
                else if (result == -1)
                {
                    CreateProjectLiteral.Text = string.Format("Something has gone horribly wrong! COULD NOT CREATE TEAM!");
                }
                else
                {
                    CreateProjectLiteral.Text = string.Format("Dont even know what went wrong!");
                }
                ProjectName.Text = null;
                ProjectUID.Text = null;
                ProjectDesc.Text = null;
                TeamsListBox.Items.Clear();
                OpenCreateProjectButton.Visible = true;
                CreateProjectPanel.Visible = false;
                ListProjects();
            }
            else
            {
                CreateProjectLiteral.Text = string.Format("Project name or UID already taken! Choose another one!");
                ProjectName.Text = null;
                ProjectUID.Text = null;
            }
        }

        private List<string> GetAllProjects()
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "SELECT Name FROM Projects";
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
                    ErrorEditLiteral.Text = string.Format(ex.Message);
                    return null;
                }
            }
        }

        private int GetProjectID(string projectName)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                int projectID;
                var queryString = "SELECT ProjectID FROM Projects WHERE Name = @projectName";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@projectName", projectName);

                try
                {
                    connection.Open();
                    projectID = (Int32)command.ExecuteScalar();
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

        private List<string> GetProjectUIDDescDates(int projectID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "SELECT UID, Description, DateCreated, DateFinished FROM Projects WHERE ProjectID = @projectID";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@projectID", projectID);
                List<string> projectsUIDDesc = new List<string>();

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            projectsUIDDesc.Add(reader.GetString(0));

                            if (reader[1] == DBNull.Value)
                            {
                                projectsUIDDesc.Add("---");
                            }
                            else
                            {
                                projectsUIDDesc.Add(reader.GetString(1));
                            }

                            projectsUIDDesc.Add(reader.GetDateTime(2).ToString("dd-MM-yyyy"));

                            if(reader[3] == DBNull.Value)
                            {
                                projectsUIDDesc.Add("---");
                            }
                            else
                            {
                                projectsUIDDesc.Add(reader.GetDateTime(3).ToString("dd-MM-yyyy"));
                            }
                        }
                    }
                    reader.Close();
                    connection.Close();
                    return projectsUIDDesc;

                }
                catch (Exception ex)
                {
                    ErrorEditLiteral.Text = string.Format(ex.Message);
                    return null;
                }
            }
        }

        private List<string> GetAllPastTeams(int projectID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "SELECT DISTINCT Name FROM SLATeams WHERE SLATeamID IN (SELECT fk_SLATeamID FROM Recipe WHERE RecipeID IN (SELECT fk_RecipeID FROM RecipeProject WHERE fk_ProjectID = @projectID) AND Active = 'False');";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@projectID", projectID);
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
                    //ErrorEditLiteral.Text = string.Format(ex.Message);
                    return null;
                }
            }
        }

        private List<string> GetAllTeamsOnProject(int projectID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "SELECT Name FROM SLATeams WHERE SLATeamID IN (SELECT fk_SLATeamID FROM Recipe WHERE RecipeID IN (SELECT fk_RecipeID FROM RecipeProject WHERE fk_ProjectID = @projectID) AND Active = 'True');";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@projectID", projectID);
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
                    //ErrorEditLiteral.Text = string.Format(ex.Message);
                    return null;
                }
            }
        }

        protected void ListProjects()
        {
            var userStore = new UserStore<IdentityUser>();
            var manager = new UserManager<IdentityUser>(userStore);
            DataTable dt = new DataTable("Projects");
            List<string> projects = GetAllProjects();

            dt.Columns.Add("lbl_ProjectName");
            dt.Columns.Add("lbl_ProjectUID");
            dt.Columns.Add("lbl_ProjectDesc");
            dt.Columns.Add("lbl_Teams", typeof(List<string>));
            dt.Columns.Add("lbl_PastTeams", typeof(List<string>));
            dt.Columns.Add("lbl_DateCreated");
            dt.Columns.Add("lbl_DateFinished");
            
            if (projects.Count != 0)
            {
                foreach (string project in projects)
                {
                    DataRow dr = null;
                    dr = dt.NewRow();
                    dr["lbl_ProjectName"] = project;
                    int projectID = GetProjectID(project);
                    List<string> projectUIDDescDates = GetProjectUIDDescDates(projectID);
                    dr["lbl_ProjectUID"] = projectUIDDescDates[0];
                    dr["lbl_ProjectDesc"] = projectUIDDescDates[1];
                    dr["lbl_DateCreated"] = projectUIDDescDates[2];
                    dr["lbl_DateFinished"] = projectUIDDescDates[3];

                    List<string> teams = GetAllTeamsOnProject(projectID);
                    if (teams.Count != 0)
                    {
                        dr["lbl_Teams"] = teams;
                    }
                    else
                    {
                        teams.Add("---");
                        dr["lbl_Teams"] = teams;
                    }

                    List<string> pastTeams = GetAllPastTeams(projectID);
                    if (pastTeams.Count != 0)
                    {
                        dr["lbl_PastTeams"] = pastTeams;
                    }
                    else
                    {
                        pastTeams.Add("---");
                        dr["lbl_PastTeams"] = pastTeams;
                    }

                    dt.Rows.Add(dr);
                }

                ProjectsGridView.DataSource = dt;
                ProjectsGridView.DataBind();
            }
        }

        protected void ListAllTeams(ListBox listBox)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "SELECT Name FROM SLATeams";
                SqlCommand command = new SqlCommand(queryString, connection);
                List<string> projectsUIDDesc = new List<string>();

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            listBox.Items.Add(reader.GetString(0));
                        }
                    }
                    reader.Close();
                    connection.Close();

                }
                catch (Exception ex)
                {
                    CreateProjectLiteral.Text = string.Format(ex.Message);
                }
            }
        }

        protected void OpenCreateProject_Click(object sender, EventArgs e)
        {
            OpenCreateProjectButton.Visible = false;
            CreateProjectPanel.Visible = true;
            ListAllTeams(TeamsListBox);
        }

        protected void CancelCreateProject_Click(object sender, EventArgs e)
        {
            ProjectName.Text = null;
            ProjectUID.Text = null;
            ProjectDesc.Text = null;
            TeamsListBox.Items.Clear();
            OpenCreateProjectButton.Visible = true;
            CreateProjectPanel.Visible = false;
        }

        private void EndProject(int projectID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string dateFinished = DateTime.Now.ToString("yyyy-MM-dd");
                var queryString = "UPDATE Projects SET DateFinished = @dateFinished WHERE ProjectID = @projectID;";
                SqlCommand command = new SqlCommand(queryString, connection);

                command.Parameters.AddWithValue("@dateFinished", dateFinished);
                command.Parameters.AddWithValue("@projectID", projectID);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    EditWarningLiteral.Text = string.Format(ex.Message);
                }
            }
        }

        protected void ProjectsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label dateFinishedLabel = (Label)e.Row.Cells[6].Controls[1];
                if (dateFinishedLabel.Text != "---")
                {
                    e.Row.Cells[7].Visible = false;
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
                Label projectUID = ProjectsGridView.Rows[rowIndex].FindControl("lbl_ProjectUID") as Label;
                Label projectDesc = ProjectsGridView.Rows[rowIndex].FindControl("lbl_ProjectDesc") as Label;
                NewProjectNameTextBox.Text = projectName.Text;
                NewProjectUIDTextBox.Text = projectUID.Text;
                NewProjectDescTextBox.Text = projectDesc.Text;

                int projectID = GetProjectID(projectName.Text);
                List<string> teamsOnProject = GetAllTeamsOnProject(projectID);
                ListAllTeams(EditAvailableTeamsListBox);
                foreach (string team in teamsOnProject)
                {
                    EditCurrentTeamsListBox.Items.Add(team);
                    EditAvailableTeamsListBox.Items.Remove(team);
                }

                HiddenEditProjectName.Value = projectName.Text;
                HiddenEditProjectUID.Value = projectUID.Text;
                HiddenEditProjectDesc.Value = projectDesc.Text;
                HiddenEditRowIndex.Value = rowIndex.ToString();
            }

            if (e.CommandName == "EndProject")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                ProjectsGridView.Rows[rowIndex].BackColor = Color.HotPink;
                ProjectsGridView.Columns[7].Visible = false;
                ProjectsGridView.Columns[8].Visible = true;
                for (int i = 0; i < ProjectsGridView.Rows.Count; i++)
                {
                    if (i == rowIndex)
                    {
                        ProjectsGridView.Rows[rowIndex].Cells[8].Visible = true;
                    }
                    else
                        ProjectsGridView.Rows[i].Cells[8].Visible = false;
                }
            }

            if (e.CommandName == "YesEnd")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                Label projectName = ProjectsGridView.Rows[rowIndex].FindControl("lbl_ProjectName") as Label;
                int projectID = GetProjectID(projectName.Text);
                List<string> teams = GetAllTeamsOnProject(projectID);

                EndProject(projectID);
                foreach(string team in teams)
                {
                    int teamID = GetTeamID(team);
                    int? currentRecipeID = GetCurrentTeamRecipeID(teamID);
                    if(currentRecipeID != null)
                    {
                        int recipeID = (Int32)currentRecipeID;
                        SetOldRecipeNotActive(recipeID);
                        List<int> otherProjects = GetOtherProjectsID(recipeID);
                        for(int i=0; i < otherProjects.Count;i++)
                        {
                            if (otherProjects[i] == projectID)
                            {
                                otherProjects.RemoveAt(i);
                            }
                        }
                        
                        if(otherProjects.Count != 0)
                        {
                            int newRecipeID = CreateNewRecipe(teamID);
                            foreach (int newProjID in otherProjects)
                            {
                                int result = AddRecipeProject(newRecipeID, newProjID);
                            }
                        }
                    }
                }
                EditLiteral.Text = string.Format("Project {0} successfully ended!", projectName.Text);
                ProjectsGridView.Columns[7].Visible = true;
                ProjectsGridView.Columns[8].Visible = false;
                ListProjects();
            }

            if (e.CommandName == "NoEnd")
            {
                ProjectsGridView.Columns[7].Visible = true;
                ProjectsGridView.Columns[8].Visible = false;
            }
        }

        private void ChangeProjectName(int projectID, string projectName)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "UPDATE Projects SET Name = @projectName WHERE ProjectID = @projectID;";
                SqlCommand command = new SqlCommand(queryString, connection);

                command.Parameters.AddWithValue("@projectName", projectName);
                command.Parameters.AddWithValue("@projectID", projectID);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    EditWarningLiteral.Text = string.Format(ex.Message);
                }

            }
        }

        private void ChangeProjectUID(int projectID, string projectUID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "UPDATE Projects SET UID = @projectUID WHERE ProjectID = @projectID;";
                SqlCommand command = new SqlCommand(queryString, connection);

                command.Parameters.AddWithValue("@projectUID", projectUID);
                command.Parameters.AddWithValue("@projectID", projectID);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    EditWarningLiteral.Text = string.Format(ex.Message);
                }

            }
        }

        private void ChangeProjectDesc(int projectID, string projectDesc)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "UPDATE Projects SET Description = @projectDesc WHERE ProjectID = @projectID;";
                SqlCommand command = new SqlCommand(queryString, connection);

                command.Parameters.AddWithValue("@projectDesc", projectDesc);
                command.Parameters.AddWithValue("@projectID", projectID);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    EditWarningLiteral.Text = string.Format(ex.Message);
                }

            }
        }

        private void EditProject()
        {
            bool isEmpty = false;
            bool isTaken = false;
            bool[] updatesMade = new bool[4];
            bool teamsRemoved = false, teamsAdded = false;
            string projectName = NewProjectNameTextBox.Text.ToString();
            string projectUID = NewProjectUIDTextBox.Text.ToString();
            string projectDesc = NewProjectDescTextBox.Text.ToString();
            int projectID = GetProjectID(HiddenEditProjectName.Value);

            isEmpty = string.IsNullOrWhiteSpace(NewProjectNameTextBox.Text);
            if (isEmpty == false && projectName != HiddenEditProjectName.Value)
            {
                for (int i = 0; i < ProjectsGridView.Rows.Count; i++)
                {
                    Label projectN = ProjectsGridView.Rows[i].FindControl("lbl_ProjectName") as Label;
                    if (projectName == projectN.Text)
                    {
                        isTaken = true;
                        break;
                    }
                    else
                        isTaken = false;
                }

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

            isEmpty = string.IsNullOrWhiteSpace(NewProjectUIDTextBox.Text);
            if (isEmpty == false && projectUID != HiddenEditProjectUID.Value)
            {
                for (int i = 0; i < ProjectsGridView.Rows.Count; i++)
                {
                    Label projectU = ProjectsGridView.Rows[i].FindControl("lbl_ProjectUID") as Label;
                    if (projectUID == projectU.Text)
                    {
                        isTaken = true;
                        break;
                    }
                    else
                        isTaken = false;
                }

                if (isTaken == false)
                {
                    ChangeProjectUID(projectID, projectUID);
                    updatesMade[1] = true;
                }

                else
                {
                    EditWarningLiteral.Text = string.Format("UID already taken! Choose another one!");
                }
            }

            isEmpty = string.IsNullOrWhiteSpace(NewProjectDescTextBox.Text);
            if (isEmpty == false && projectDesc != HiddenEditProjectDesc.Value)
            {
                Label projectD = ProjectsGridView.Rows[Int32.Parse(HiddenEditRowIndex.Value)].FindControl("lbl_ProjectDesc") as Label;
                
                ChangeProjectDesc(projectID, projectDesc);
                updatesMade[2] = true;
            }

            foreach (ListItem li in EditCurrentTeamsListBox.Items)
            {
                if (li.Selected == true)
                {
                    int teamID = GetTeamID(li.Value);
                    int? currentRecipeID = GetCurrentTeamRecipeID(teamID);
                    if (currentRecipeID != null)
                    {
                        int recipeID = (Int32)currentRecipeID;
                        SetOldRecipeNotActive(recipeID);
                        List<int> otherProjects = GetOtherProjectsID(recipeID);
                        for (int i = 0; i < otherProjects.Count; i++)
                        {
                            if (otherProjects[i] == projectID)
                            {
                                otherProjects.RemoveAt(i);
                            }
                        }
                        
                        if(otherProjects.Count != 0)
                        {
                            int newRecipeID = CreateNewRecipe(teamID);
                            foreach (int newProjID in otherProjects)
                            {
                                int result = AddRecipeProject(newRecipeID, newProjID);
                            }
                        }
                        teamsRemoved = true;
                    }
                }
            }

            foreach (ListItem li in EditAvailableTeamsListBox.Items)
            {
                if (li.Selected == true)
                {
                    int teamID = GetTeamID(li.Value);
                    int? oldRecipeID = GetCurrentTeamRecipeID(teamID);
                    int recipeID = CreateNewRecipe(teamID);
                    int result;

                    if (oldRecipeID != null)
                    {
                        int oldRecID = (Int32)oldRecipeID;
                        SetOldRecipeNotActive(oldRecID);
                        List<int> otherProjectsID = GetOtherProjectsID(oldRecID);
                        if (otherProjectsID.Count != 0)
                        {
                            foreach (int projId in otherProjectsID)
                            {
                                result = AddRecipeProject(recipeID, projId);
                            }
                        }
                    }
                    result = AddRecipeProject(recipeID, projectID);
                    teamsAdded = true;
                }
            }

            if (teamsRemoved == true || teamsAdded == true)
            {
                updatesMade[3] = true;
            }
            else
            {
                updatesMade[3] = false;
            }

            if (updatesMade[0] == true && updatesMade[1] == true && updatesMade[2] == true && updatesMade[3] == true)
            {
                EditLiteral.Text = string.Format("{0}'s (formerly {1}) name, UID, Description and teams updated!", projectName, HiddenEditProjectName.Value);
            }
            else if (updatesMade[0] == true && updatesMade[1] == true && updatesMade[2] == true && updatesMade[3] == false)
            {
                EditLiteral.Text = string.Format("{0}'s (formerly {1}) name, UID and description updated!", projectName, HiddenEditProjectName.Value);
            }
            else if (updatesMade[0] == true && updatesMade[1] == true && updatesMade[2] == false && updatesMade[3] == true)
            {
                EditLiteral.Text = string.Format("{0}'s (formerly {1}) name, UID and teams updated!", projectName, HiddenEditProjectName.Value);
            }
            else if (updatesMade[0] == true && updatesMade[1] == true && updatesMade[2] == false && updatesMade[3] == false)
            {
                EditLiteral.Text = string.Format("{0}'s (formerly {1}) name and UID updated!", projectName, HiddenEditProjectName.Value);
            }
            else if (updatesMade[0] == true && updatesMade[1] == false && updatesMade[2] == true && updatesMade[3] == true)
            {
                EditLiteral.Text = string.Format("{0}'s (formerly {1}) name, description and teams updated!", projectName, HiddenEditProjectName.Value);
            }
            else if (updatesMade[0] == true && updatesMade[1] == false && updatesMade[2] == true && updatesMade[3] == false)
            {
                EditLiteral.Text = string.Format("{0}'s (formerly {1}) name and description updated!", projectName, HiddenEditProjectName.Value);
            }
            else if (updatesMade[0] == true && updatesMade[1] == false && updatesMade[2] == false && updatesMade[3] == true)
            {
                EditLiteral.Text = string.Format("{0}'s (formerly {1}) name and teams updated!", projectName, HiddenEditProjectName.Value);
            }
            else if (updatesMade[0] == true && updatesMade[1] == false && updatesMade[2] == false && updatesMade[3] == false)
            {
                EditLiteral.Text = string.Format("{0}'s (formerly {1}) name updated!", projectName, HiddenEditProjectName.Value);
            }
            else if (updatesMade[0] == false && updatesMade[1] == true && updatesMade[2] == true && updatesMade[3] == true)
            {
                EditLiteral.Text = string.Format("{0}'s UID, description and teams updated!", HiddenEditProjectName.Value);
            }
            else if (updatesMade[0] == false && updatesMade[1] == true && updatesMade[2] == true && updatesMade[3] == false)
            {
                EditLiteral.Text = string.Format("{0}'s UID and description updated!", HiddenEditProjectName.Value);
            }
            else if (updatesMade[0] == false && updatesMade[1] == true && updatesMade[2] == false && updatesMade[3] == true)
            {
                EditLiteral.Text = string.Format("{0}'s UID and teams updated!", HiddenEditProjectName.Value);
            }
            else if (updatesMade[0] == false && updatesMade[1] == true && updatesMade[2] == false && updatesMade[3] == false)
            {
                EditLiteral.Text = string.Format("{0}'s UID updated!", HiddenEditProjectName.Value);
            }
            else if (updatesMade[0] == false && updatesMade[1] == false && updatesMade[2] == true && updatesMade[3] == true)
            {
                EditLiteral.Text = string.Format("{0}'s description and teams updated!", HiddenEditProjectName.Value);
            }
            else if (updatesMade[0] == false && updatesMade[1] == false && updatesMade[2] == true && updatesMade[3] == false)
            {
                EditLiteral.Text = string.Format("{0}'s description updated!", HiddenEditProjectName.Value);
            }
            else if (updatesMade[0] == false && updatesMade[1] == false && updatesMade[2] == false && updatesMade[3] == true)
            {
                EditLiteral.Text = string.Format("{0}'s teams updated!", HiddenEditProjectName.Value);
            }
        }

        protected void SaveEditProject_Click(object sender, EventArgs e)
        {
            EditProject();
            NewProjectNameTextBox.Text = null;
            NewProjectUIDTextBox.Text = null;
            NewProjectDescTextBox.Text = null;
            EditCurrentTeamsListBox.Items.Clear();
            EditAvailableTeamsListBox.Items.Clear();
            EditProjectPanel.Visible = false;
            HiddenEditProjectName.Value = null;
            HiddenEditProjectUID.Value = null;
            HiddenEditProjectDesc.Value = null;
            HiddenEditRowIndex.Value = null;
            ListProjects();
        }

        protected void ApplyEditProject_Click(object sender, EventArgs e)
        {
            EditProject();
            HiddenEditProjectName.Value = NewProjectNameTextBox.Text;
            HiddenEditProjectUID.Value = NewProjectUIDTextBox.Text;
            HiddenEditProjectDesc.Value = NewProjectDescTextBox.Text;
            ListProjects();
            int rowIndex = Int32.Parse(HiddenEditRowIndex.Value);
            ProjectsGridView.Rows[rowIndex].BackColor = Color.HotPink;
            EditCurrentTeamsListBox.Items.Clear();
            EditAvailableTeamsListBox.Items.Clear();

            int projectID = GetProjectID(HiddenEditProjectName.Value);
            List<string> teamsOnProject = GetAllTeamsOnProject(projectID);
            ListAllTeams(EditAvailableTeamsListBox);
            foreach (string team in teamsOnProject)
            {
                EditCurrentTeamsListBox.Items.Add(team);
                EditAvailableTeamsListBox.Items.Remove(team);
            }
        }

        protected void CancelEditProject_Click(object sender, EventArgs e)
        {
            NewProjectNameTextBox.Text = null;
            NewProjectUIDTextBox.Text = null;
            NewProjectDescTextBox.Text = null;
            EditCurrentTeamsListBox.Items.Clear();
            EditAvailableTeamsListBox.Items.Clear();
            EditProjectPanel.Visible = false;
            HiddenEditProjectName.Value = null;
            HiddenEditProjectUID.Value = null;
            HiddenEditProjectDesc.Value = null;
            HiddenEditRowIndex.Value = null;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Welcome.Text = string.Format("Hello, {0}!", User.Identity.GetUserName());
            ListProjects();
        }

        protected void SignOut(object sender, EventArgs e)
        {
            var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.SignOut();
            Response.Redirect("~/Default.aspx");
        }
    }
}