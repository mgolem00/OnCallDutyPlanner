# OnCallDutyPlanner

Fresh installation guide:

After downloading the project you should copy the DatabaseBackup/DB_Table_CreateScripts/AllTablesAtOnce.txt T-SQL script and use it on your SQL Server to create all the tables in the database (NOTE: you should already have a database on your SQL Server and change the first line in the script to match your database name).
The OCDPDB.bak file is a SQL Express Server test database with test-dummy data so you should really use it unless you want to test stuff.

Change the connection string in OnCallDutyPlanner/OnCallDutyPlanner/Web.config file to connect to your database

If you are creating a completely new database you should put the following code in the default.aspx.cs Page_Load function:
```
var userStore = new UserStore<IdentityUser>();
var manager = new UserManager<IdentityUser>(userStore);
var user = new IdentityUser() { UserName = "admin" };
manager.Create(user, "Password");
manager.AddToRole(user.Id, "Admin");
```
Ofcourse you should change the UserName and Password as you want. That shall create the first user, that is an admin.
Start the application for the first time and after getting to the login page try to login as the user that was just created. If you can. just shut down the application, remove the code you just added and start the application again.

Thats it! I hope it works!
