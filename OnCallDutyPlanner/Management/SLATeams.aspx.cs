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
    public partial class SLATeams : System.Web.UI.Page
    {
        private void InsertUserToTeam(int teamID, string userID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "UPDATE AspNetUsers SET SLATeamID = @teamID WHERE Id = @userID;";
                SqlCommand command = new SqlCommand(queryString, connection);

                command.Parameters.AddWithValue("@userID", userID);
                command.Parameters.AddWithValue("@teamID", teamID);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    ErrorEditLiteral.Text = string.Format(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private void RemoveUserFromTeam(string userID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "UPDATE AspNetUsers SET SLATeamID = NULL WHERE Id = @userID;";
                SqlCommand command = new SqlCommand(queryString, connection);

                command.Parameters.AddWithValue("@userID", userID);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    ErrorEditLiteral.Text = string.Format(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private int DeleteTeam(int teamID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "UPDATE SLATeams SET IsDeleted = 1 WHERE ID = @teamID;";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@teamID", teamID);

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
                    ErrorEditLiteral.Text = string.Format(ex.Message);
                    return -1;
                }
            }
        }

        private int CreateTeam(string teamName)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "INSERT INTO SLATeams(TeamName) VALUES (@name);";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@name", teamName);

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
                    CreateTeamLiteral.Text = string.Format(ex.Message);
                    return -1;
                }

            }
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
                    connection.Close();
                    ErrorEditLiteral.Text = string.Format(ex.Message);
                    return -1;
                }
            }
        }

        private int GetUserTeamID(string userID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                int userTeamID = - 1;
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
                            if(!reader.IsDBNull(0))
                                userTeamID = reader.GetInt32(0);
                        }
                    }
                    reader.Close();
                    connection.Close();
                    return userTeamID;
                }
                catch (Exception ex)
                {
                    connection.Close();
                    Console.WriteLine(ex.Message);
                    return -2;
                }
            }
        }

        private bool IsUserDeleted(string userID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                bool userIsDeleted;
                var queryString = "SELECT IsDeleted FROM AspNetUsers WHERE Id = @userID";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@userID", userID);

                try
                {
                    connection.Open();
                    userIsDeleted = (bool)command.ExecuteScalar();
                    connection.Close();
                    return userIsDeleted;

                }
                catch (Exception ex)
                {
                    connection.Close();
                    Console.WriteLine(ex.Message);
                    return true;
                }
            }
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
                    ErrorEditLiteral.Text = string.Format(ex.Message);
                    return null;
                }
            }
        }

        private List<string> GetAllTeamMembers(int teamID)
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
                    ErrorEditLiteral.Text = string.Format(ex.Message);
                    return null;
                }
            }
        }

        private void ListEditMembers(int rowIndex)
        {
            Repeater membersRepeater = TeamsGridView.Rows[rowIndex].FindControl("MembersRepeater") as Repeater;
            foreach (RepeaterItem rI in membersRepeater.Items)
            {
                Label members = rI.FindControl("lbl_Members") as Label;
                if (members.Text != "---")
                {
                    EditCurrentMembersListBox.Items.Add(members.Text);
                }
            }
        }

        protected void ListUsers(ListBox listBox)
        {
            var userStore = new UserStore<IdentityUser>();
            var manager = new UserManager<IdentityUser>(userStore);

            foreach (var user in manager.Users.ToList().OrderBy(usr => usr.UserName.ToString()))
            {
                int userTeamID = GetUserTeamID(user.Id.ToString());
                if (userTeamID == -1 && userTeamID != -2 && IsUserDeleted(user.Id) == false)
                {
                    listBox.Items.Add(user.UserName.ToString());
                }
            }
        }

        protected void ListTeams()
        {
            var userStore = new UserStore<IdentityUser>();
            var manager = new UserManager<IdentityUser>(userStore);
            DataTable dt = new DataTable("Teams");
            List<string> teams = GetAllTeams();

            dt.Columns.Add("lbl_TeamName");
            dt.Columns.Add("lbl_Members", typeof(List<string>));
            if (teams.Count != 0)
            {
                foreach (string team in teams)
                {
                    DataRow dr = null;
                    dr = dt.NewRow();
                    dr["lbl_TeamName"] = team;
                    int teamID = GetTeamID(team);
                    List<string> members = GetAllTeamMembers(teamID);
                    if (members.Count != 0)
                    {
                        dr["lbl_Members"] = members;
                    }
                    else
                    {
                        members.Add("---");
                        dr["lbl_Members"] = members;
                    }
                    dt.Rows.Add(dr);
                }
                TeamsGridView.DataSource = dt;
                TeamsGridView.DataBind();
            }
        }

        protected void ChangeTeamName(int teamID, string newName)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "UPDATE SLATeams SET TeamName = @newName WHERE ID = @teamID;";
                SqlCommand command = new SqlCommand(queryString, connection);

                command.Parameters.AddWithValue("@newName", newName);
                command.Parameters.AddWithValue("@teamID", teamID);

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
                finally 
                { 
                    connection.Close(); 
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                Welcome.Text = string.Format("Hello, {0}!", User.Identity.GetUserName());
                ListTeams();
            }
        }

        protected void SignOut(object sender, EventArgs e)
        {
            var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.SignOut();
            Response.Redirect("~/Default.aspx");
        }

        protected void OpenCreateTeam_Click(object sender, EventArgs e)
        {
            OpenCreateTeamButton.Visible = false;
            CreateTeamPanel.Visible = true;
            ListUsers(UserListBox);
            ListTeams();
        }

        protected void CancelCreateTeam_Click(object sender, EventArgs e)
        {
            TeamName.Text = null;
            OpenCreateTeamButton.Visible = true;
            CreateTeamPanel.Visible = false;
            ListTeams();
        }

        protected void CreateTeam_Click(object sender, EventArgs e)
        {
            var userStore = new UserStore<IdentityUser>();
            var manager = new UserManager<IdentityUser>(userStore);
            string teamName = TeamName.Text.ToString();
            bool isNameTaken = false;
            

            for (int i=0; i < TeamsGridView.Rows.Count; i++)
            {
                Label teamN = TeamsGridView.Rows[i].FindControl("lbl_TeamName") as Label;
                if (teamName == teamN.Text)
                {
                    isNameTaken = true;
                    break;
                }
                else 
                    isNameTaken = false;
            }

            if (isNameTaken == false)
            {
                int result = CreateTeam(teamName);

                if (result == 1)
                {
                    foreach (ListItem li in UserListBox.Items)
                    {
                        if (li.Selected == true)
                        {
                            var user = manager.FindByName(li.Text);
                            string userID = user.Id.ToString();
                            int teamID = GetTeamID(teamName);
                            InsertUserToTeam(teamID, userID);
                        }
                    }
                    CreateTeamLiteral.Text = string.Format("{0} was created successfully!", teamName);
                }
                else if (result == -1)
                {
                    CreateTeamLiteral.Text = string.Format("Something has gone horribly wrong! COULD NOT CREATE TEAM!");
                }
                else
                {
                    CreateTeamLiteral.Text = string.Format("Dont even know what went wrong!");
                }
            }
            else
            {
                CreateTeamLiteral.Text = string.Format("Team name already taken! Choose another one!");
            }
            TeamName.Text = null;
            UserListBox.Items.Clear();
            OpenCreateTeamButton.Visible = true;
            CreateTeamPanel.Visible = false;
            ListTeams();
        }

        protected void TeamsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditTeam")
            {
                EditTeamPanel.Visible = true;
                ScriptManager.RegisterStartupScript(this, GetType(), "scrollToEdit", "document.getElementById('EditTeamPanel').scrollIntoView({ behavior: 'smooth', block: 'center' });", true);
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                TeamsGridView.Rows[rowIndex].BackColor = Color.HotPink;
                Label teamLabel = TeamsGridView.Rows[rowIndex].FindControl("lbl_TeamName") as Label;
                NewTeamNameTextBox.Text = teamLabel.Text;
                ListEditMembers(rowIndex);
                ListUsers(EditNonMembersListBox);
                HiddenEditTeamName.Value = teamLabel.Text;
                HiddenEditRowIndex.Value = rowIndex.ToString();
            }

            if (e.CommandName == "DeleteTeam")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                TeamsGridView.Rows[rowIndex].BackColor = Color.HotPink;
                TeamsGridView.Columns[2].Visible = false;
                TeamsGridView.Columns[3].Visible = true;
                for (int i = 0; i < TeamsGridView.Rows.Count; i++)
                {
                    if (i == rowIndex)
                    {
                        TeamsGridView.Rows[rowIndex].Cells[3].Visible = true;
                    }
                    else
                        TeamsGridView.Rows[i].Cells[3].Visible = false;
                }
            }

            if (e.CommandName == "YesDelete")
            {
                var userStore = new UserStore<IdentityUser>();
                var manager = new UserManager<IdentityUser>(userStore);
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                Label teamName = TeamsGridView.Rows[rowIndex].FindControl("lbl_TeamName") as Label;
                int teamID = GetTeamID(teamName.Text);
                List<string> members = GetAllTeamMembers(teamID);
                if (members.Count != 0)
                {
                    foreach (string userName in members)
                    {
                        var user = manager.FindByName(userName);
                        RemoveUserFromTeam(user.Id);
                    }
                }
                DeleteTeam(teamID);
                EditLiteral.Text = string.Format("{0} was deleted!", teamName.Text);
                TeamsGridView.Columns[2].Visible = true;
                TeamsGridView.Columns[3].Visible = false;
                ListTeams();
            }

            if (e.CommandName == "NoDelete")
            {
                TeamsGridView.Columns[2].Visible = true;
                TeamsGridView.Columns[3].Visible = false;
            }
        }

        protected void EditTeam()
        {
            var userStore = new UserStore<IdentityUser>();
            var manager = new UserManager<IdentityUser>(userStore);
            bool isEmpty = false;
            bool isNameTaken = false;
            bool[] updatesMade = new bool[2];
            bool membersRemoved=false, membersAdded=false;
            string teamName = NewTeamNameTextBox.Text.ToString();

            isEmpty = string.IsNullOrWhiteSpace(NewTeamNameTextBox.Text);
            if(isEmpty == false && teamName != HiddenEditTeamName.Value)
            {
                for (int i = 0; i < TeamsGridView.Rows.Count; i++)
                {
                    Label teamN = TeamsGridView.Rows[i].FindControl("lbl_TeamName") as Label;
                    if (teamName == teamN.Text)
                    {
                        isNameTaken = true;
                        break;
                    }
                    else
                        isNameTaken = false;
                }

                if (isNameTaken == false)
                {
                    int teamID = GetTeamID(HiddenEditTeamName.Value);
                    ChangeTeamName(teamID, teamName);
                    updatesMade[0] = true;
                }
                
                else
                {
                    EditWarningLiteral.Text = string.Format("Team name already taken! Choose another one!");
                }
            }
            else 
            {
                updatesMade[0] = false;
            }

            foreach (ListItem li in EditCurrentMembersListBox.Items)
            {
                if (li.Selected == true)
                {
                    var user = manager.FindByName(li.Text);
                    RemoveUserFromTeam(user.Id);
                    membersRemoved = true;
                }
            }
            foreach (ListItem li in EditNonMembersListBox.Items)
            {
                if (li.Selected == true)
                {
                    var user = manager.FindByName(li.Text);
                    int teamID = GetTeamID(HiddenEditTeamName.Value);
                    InsertUserToTeam(teamID, user.Id);
                    membersAdded = true;
                }
            }

            if(membersRemoved == true || membersAdded == true)
            {
                updatesMade[1] = true;
            }
            else
            {
                updatesMade[1] = false;
            }

            if (updatesMade[0] == true && updatesMade[1] == true)
            {
                EditLiteral.Text = string.Format("{0}'s (formerly {1}) name and members!", teamName, HiddenEditTeamName.Value);
            }
            else if (updatesMade[0] == true && updatesMade[1] == false)
            {
                EditLiteral.Text = string.Format("{0}'s (formerly {1}) name updated!", teamName, HiddenEditTeamName.Value);
            }
            else if (updatesMade[0] == false && updatesMade[1] == true)
            {
                EditLiteral.Text = string.Format("{0}'s users updated!", HiddenEditTeamName.Value);
            }
        }

        protected void SaveEditTeam_Click(object sender, EventArgs e)
        {
            EditTeam();
            NewTeamNameTextBox.Text = null;
            EditCurrentMembersListBox.Items.Clear();
            EditNonMembersListBox.Items.Clear();
            EditTeamPanel.Visible = false;
            HiddenEditTeamName.Value = null;
            HiddenEditRowIndex.Value = null;
            ListTeams();
        }

        protected void ApplyEditTeam_Click(object sender, EventArgs e)
        {
            EditTeam();
            HiddenEditTeamName.Value = NewTeamNameTextBox.Text;
            ListTeams();
            int rowIndex = Int32.Parse(HiddenEditRowIndex.Value);
            TeamsGridView.Rows[rowIndex].BackColor = Color.HotPink;
            EditCurrentMembersListBox.Items.Clear();
            EditNonMembersListBox.Items.Clear();
            ListEditMembers(rowIndex);
            ListUsers(EditNonMembersListBox);
        }

        protected void CancelEditTeam_Click(object sender, EventArgs e)
        {
            NewTeamNameTextBox.Text = null;
            EditCurrentMembersListBox.Items.Clear();
            EditNonMembersListBox.Items.Clear();
            EditTeamPanel.Visible = false;
            HiddenEditTeamName.Value = null;
            HiddenEditRowIndex.Value = null;
            ListTeams();
        }
    }
}