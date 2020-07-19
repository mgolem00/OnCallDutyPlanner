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
    public partial class Users : System.Web.UI.Page
    {
        private int? GetUserTeamID(string userID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                int? userTeamID;
                var queryString = "SELECT SLATeamID FROM AspNetUsers WHERE Id = @userID";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@userID", userID);

                try
                {
                    connection.Open();
                    userTeamID = (Int32)command.ExecuteScalar();
                    connection.Close();
                    return userTeamID;

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
                string TeamName;
                var queryString = "SELECT TeamName FROM SLATeams WHERE ID = @userTeamID";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@userTeamID", teamID);

                try
                {
                    connection.Open();
                    TeamName = (String)command.ExecuteScalar();
                    connection.Close();
                    return TeamName;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        private List<string> GetActiveUsersID()
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "SELECT Id FROM AspNetUsers WHERE IsDeleted = 0";
                SqlCommand command = new SqlCommand(queryString, connection);
                List<string> usersID = new List<string>();

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            usersID.Add(reader.GetString(0));
                        }
                    }
                    reader.Close();
                    connection.Close();
                    return usersID;

                }
                catch (Exception ex)
                {
                    ErrorEditLiteral.Text = string.Format(ex.Message);
                    return null;
                }
            }
        }

        protected void ListUsers()
        {
            var userStore = new UserStore<IdentityUser>();
            var manager = new UserManager<IdentityUser>(userStore);

            DataTable dt = new DataTable("Users");

            dt.Columns.Add("lbl_Username");
            dt.Columns.Add("lbl_Role");
            dt.Columns.Add("lbl_SLATeam");

            List<string> users = GetActiveUsersID();

            foreach (string userID in users)
            {
                var user = manager.FindById(userID);
                int? userTeamID = GetUserTeamID(user.Id.ToString());
                string userRole = manager.GetRoles(user.Id).FirstOrDefault().ToString();
                DataRow dr = null;
                dr = dt.NewRow();
                dr["lbl_Username"] = user.UserName.ToString();
                dr["lbl_Role"] = userRole;
                if (userTeamID == null)
                {
                    dr["lbl_SLATeam"] = "---";
                }
                else
                {
                    int userTeamID2 = (int)userTeamID;
                    dr["lbl_SLATeam"] = GetTeamName(userTeamID2);
                }
                dt.Rows.Add(dr);
            }

            UsersGridView.DataSource = dt;
            UsersGridView.DataBind();
            if(User.IsInRole("Admin"))
            {
                UsersGridView.Columns[3].Visible = true;
            }
            else
                UsersGridView.Columns[3].Visible = false;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Welcome.Text = string.Format("Hello, {0}!", User.Identity.GetUserName());
            ListUsers();
        }

        protected void OpenCreateUser_Click(object sender, EventArgs e)
        {
            OpenCreateUserButton.Visible = false;
            CreateUserPanel.Visible = true;
        }

        protected void CancelCreateUser_Click(object sender, EventArgs e)
        {
            UserName.Text = null;
            Password.Text = null;
            OpenCreateUserButton.Visible = true;
            CreateUserPanel.Visible = false;
        }

        protected void CreateUser_Click(object sender, EventArgs e)
        {
            // Default UserStore constructor uses the default connection string named: DefaultConnection
            var userStore = new UserStore<IdentityUser>();
            var manager = new UserManager<IdentityUser>(userStore);

            var user = new IdentityUser() { UserName = UserName.Text };
            IdentityResult result = manager.Create(user, Password.Text);
            if (result.Succeeded)
            {
                if (!manager.IsInRole(manager.FindByName(UserName.Text).Id, RoleDropDown.SelectedValue))
                {
                    result = manager.AddToRole(manager.FindByName(UserName.Text).Id, RoleDropDown.SelectedValue);
                    string getUserRole = manager.GetRoles(user.Id).FirstOrDefault().ToString();
                    CreateUserLiteral.Text = string.Format("{0} {1} was created successfully!", getUserRole, user.UserName.ToString());
                }
            }
            else
            {
                CreateUserLiteral.Text = result.Errors.FirstOrDefault();
            }
            UserName.Text = null;
            Password.Text = null;
            OpenCreateUserButton.Visible = true;
            CreateUserPanel.Visible = false;
            ListUsers();
        }

        protected void SignOut(object sender, EventArgs e)
        {
            var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.SignOut();
            Response.Redirect("~/Default.aspx");
        }

        private int DeleteUser(string userID)
        {
            using (SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var queryString = "UPDATE AspNetUsers SET IsDeleted = 1 WHERE Id = @userID;";
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@userID", userID);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    return 1;
                }
                catch (Exception ex)
                {
                    ErrorEditLiteral.Text = string.Format(ex.Message);
                    return -1;
                }

            }
        }

        protected void UsersGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditUser")
            {
                EditUserPanel.Visible = true;
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                Label userName = UsersGridView.Rows[rowIndex].FindControl("lbl_Username") as Label;
                Label userRole = UsersGridView.Rows[rowIndex].FindControl("lbl_Role") as Label;
                NewUserNameTextBox.Text = userName.Text;
                EditRoleDropDown.SelectedValue = userRole.Text;
                UsersGridView.Rows[rowIndex].BackColor = Color.HotPink;
                ScriptManager.RegisterStartupScript(this, GetType(), "scrollToEdit", "document.getElementById('EditUserPanel').scrollIntoView({ behavior: 'smooth', block: 'center' });", true);
                HiddenEditUsername.Value = userName.Text;
                HiddenEditRole.Value = userRole.Text;
                HiddenEditRowIndex.Value = rowIndex.ToString();
            }

            if (e.CommandName == "DeleteUser")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                UsersGridView.Rows[rowIndex].BackColor = Color.HotPink;
                UsersGridView.Columns[3].Visible = false;
                UsersGridView.Columns[4].Visible = true;
                for (int i= 0; i < UsersGridView.Rows.Count; i++)
                {
                    if(i == rowIndex)
                    {
                        UsersGridView.Rows[rowIndex].Cells[4].Visible = true;
                    }
                    else
                        UsersGridView.Rows[i].Cells[4].Visible = false;
                }
            }

            if (e.CommandName == "YesDelete")
            {
                var userStore = new UserStore<IdentityUser>();
                var manager = new UserManager<IdentityUser>(userStore);
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                Label userName = UsersGridView.Rows[rowIndex].FindControl("lbl_Username") as Label;
                var user = manager.FindByName(userName.Text);
                var result = DeleteUser(user.Id);
                if (result == 1)
                {
                    EditLiteral.Text = string.Format("{0} was deleted!", userName.Text);
                    ListUsers();
                }
                else
                {
                    EditLiteral.Text = string.Format("Something went wrong! Deletion failed!");
                }
                UsersGridView.Columns[3].Visible = true;
                UsersGridView.Columns[4].Visible = false;
            }

            if (e.CommandName == "NoDelete")
            {
                UsersGridView.Columns[3].Visible = true;
                UsersGridView.Columns[4].Visible = false;
            }
        }

        protected void EditUser()
        {
            var userStore = new UserStore<IdentityUser>();
            var manager = new UserManager<IdentityUser>(userStore);
            bool[] updatesMade = new bool[3];
            var user = manager.FindByName(HiddenEditUsername.Value);
            string userID = user.Id;
            bool isEmpty = false;

            isEmpty = string.IsNullOrWhiteSpace(NewUserNameTextBox.Text);
            if (user.UserName != NewUserNameTextBox.Text && isEmpty == false)
            {
                string newUsrName = NewUserNameTextBox.Text;
                user.UserName = newUsrName;
                var result = manager.Update(user);
                if (result.Succeeded)
                {
                    updatesMade[0] = true;
                }
                else
                {
                    ErrorEditLiteral.Text = string.Format("Username change failed!");
                    updatesMade[0] = false;
                }
            }
            else updatesMade[0] = false;

            isEmpty = string.IsNullOrWhiteSpace(NewPasswordTextBox.Text);
            if (isEmpty == false)
            {
                if (manager.HasPassword(userID))
                {
                    var result1 = manager.RemovePassword(userID);
                    if(result1.Succeeded)
                    {
                        string newPwd = NewPasswordTextBox.Text;
                        string passwordHash = manager.PasswordHasher.HashPassword(newPwd);
                        user.PasswordHash = passwordHash;
                        var result2 = manager.Update(user);

                        if (result2.Succeeded)
                        {
                            updatesMade[1] = true;
                        }
                        else
                        {
                            ErrorEditLiteral.Text = string.Format("(2)Password change failed!");
                            updatesMade[1] = false;
                        }
                    }
                }
                else
                {
                    ErrorEditLiteral.Text = string.Format("(1)Password change failed!");
                    updatesMade[1] = false;
                }
                
                /*var result1 = manager.RemovePassword(user.Id);
                if (result1.Succeeded)
                {
                    string newPwd = NewPasswordTextBox.Text;
                    var result2 = manager.AddPassword(user.Id, newPwd);
                    if (result2.Succeeded)
                    {
                        updatesMade[1] = true;
                    }
                    else
                    {
                        ErrorEditLiteral.Text = string.Format("(2)Password change failed!");
                        updatesMade[1] = false;
                    }
                }
                else
                {
                    ErrorEditLiteral.Text = string.Format("(1)Password change failed!");
                    updatesMade[2] = false;
                }*/
            }
            else updatesMade[1] = false;

            if (HiddenEditRole.Value != EditRoleDropDown.SelectedValue)
            {
                string[] allUserRoles = manager.GetRoles(userID).ToArray();
                var result1 = manager.RemoveFromRoles(userID, allUserRoles);
                if (result1.Succeeded)
                {
                    var result2 = manager.AddToRole(userID, EditRoleDropDown.SelectedValue);
                    if (result2.Succeeded)
                    {
                        updatesMade[2] = true;
                    }
                    else
                    {
                        ErrorEditLiteral.Text = string.Format("(2)Role change failed!");
                        updatesMade[2] = false;
                    }
                }
                else
                {
                    ErrorEditLiteral.Text = string.Format("(1)Role change failed!");
                    updatesMade[2] = false;
                }
            }
            else updatesMade[2] = false;

            if (updatesMade[0] == true && updatesMade[1] == true && updatesMade[2] == true)
            {
                EditLiteral.Text = string.Format("{0}'s (formerly {1}) username, password and role updated!", user.UserName, HiddenEditUsername.Value);
            }
            else if (updatesMade[0] == true && updatesMade[1] == false && updatesMade[2] == false)
            {
                EditLiteral.Text = string.Format("{0}'s (formerly {1}) username updated!", user.UserName, HiddenEditUsername.Value);
            }
            else if (updatesMade[0] == false && updatesMade[1] == true && updatesMade[2] == false)
            {
                EditLiteral.Text = string.Format("{0}'s password updated!", user.UserName);
            }
            else if (updatesMade[0] == false && updatesMade[1] == false && updatesMade[2] == true)
            {
                EditLiteral.Text = string.Format("{0}'s role updated!", user.UserName);
            }
            else if (updatesMade[0] == true && updatesMade[1] == true && updatesMade[2] == false)
            {
                EditLiteral.Text = string.Format("{0}'s (formerly {1}) username and password updated!", user.UserName, HiddenEditUsername.Value);
            }
            else if (updatesMade[0] == true && updatesMade[1] == false && updatesMade[2] == true)
            {
                EditLiteral.Text = string.Format("{0}'s (formerly {1}) username and role updated!", user.UserName, HiddenEditUsername.Value);
            }
            else if (updatesMade[0] == false && updatesMade[1] == true && updatesMade[2] == true)
            {
                EditLiteral.Text = string.Format("{0}'s password and role updated!", user.UserName);
            }
        }

        protected void SaveEditUser_Click(object sender, EventArgs e)
        {
            EditUser();
            NewUserNameTextBox.Text = null;
            NewPasswordTextBox.Text = null;
            EditUserPanel.Visible = false;
            HiddenEditUsername.Value = null;
            HiddenEditRole.Value = null;
            HiddenEditRowIndex.Value = null;
            ListUsers();
        }

        protected void ApplyEditUser_Click(object sender, EventArgs e)
        {
            EditUser();
            ListUsers();
            HiddenEditUsername.Value = NewUserNameTextBox.Text;
            HiddenEditRole.Value = EditRoleDropDown.SelectedValue;
            int rowIndex = Int32.Parse(HiddenEditRowIndex.Value);
            UsersGridView.Rows[rowIndex].BackColor = Color.HotPink;
        }

        protected void CancelEditUser_Click(object sender, EventArgs e)
        {
            NewUserNameTextBox.Text = null;
            NewPasswordTextBox.Text = null;
            EditUserPanel.Visible = false;
            HiddenEditUsername.Value = null;
            HiddenEditRole.Value = null;
            HiddenEditRowIndex.Value = null;
        }
    }
}