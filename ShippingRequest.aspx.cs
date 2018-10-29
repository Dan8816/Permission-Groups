using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Security.Principal;
using System.Configuration;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.DirectoryServices;

public partial class ShippingRequest : Page
{
    //>===============================================> Dan Engle 10/8/2018 >======================================================================>

    SqlConnection myConnection;//may need to encapsulate this
    public string path = "LDAP://omni.phish/dc=omni,dc=phish"; //<=Remove this later<=/\=>will use this for OAI=>>//"LDAP://oai.local/OU=Users,OU=OAI Company,dc=oai,dc=local";
    public string username = "sa-shipreq-01";
    public string password = "p@$$word999999";  
    public string GetUsersManager(string samaccount)
    {
        System.Diagnostics.Debug.WriteLine("***** Successfully called the GetUsrMgr func and param val is: " + samaccount + "*****");
        string UsersManager = "";
        try
        {
            string ThisUsersName = samaccount;
            string Userfilter = "(&(objectCategory=person)(objectClass=user)(sAMAccountName=" + ThisUsersName + ")(!userAccountControl:1.2.840.113556.1.4.803:=2))";
            string[] UsersProperties = new string[1] { "manager" };
            //init a directory entry
            DirectoryEntry UserEntry = new DirectoryEntry(path, username, password);
            //init a directory searcher
            DirectorySearcher UserSearch = new DirectorySearcher(UserEntry, Userfilter, UsersProperties);
            SearchResultCollection UserResults = UserSearch.FindAll();
            foreach (SearchResult result in UserResults)
            {
                UsersManager = (string)result.Properties["manager"][0];
                System.Diagnostics.Debug.WriteLine("***** Manager Value is: " + UsersManager + "*****");
            }
        }
        catch { }
        return UsersManager;
    }
    public string  GetManagersData(string MgrDN)
    {
        System.Diagnostics.Debug.WriteLine("***** Successfully called the func and param val is: " + MgrDN + "*****");
        string ManagerData = "";
        try
        {
            string UserManager = MgrDN;
            string Managerfilter = "(&(objectCategory=person)(objectClass=user)(DistinguishedName=" + UserManager + ")(!userAccountControl:1.2.840.113556.1.4.803:=2))";
            string[] ManagerProperties = new string[1] { "mail" };
            DirectoryEntry ManagerEntry = new DirectoryEntry(path, username, password);
            DirectorySearcher ManagerSearch = new DirectorySearcher(ManagerEntry, Managerfilter, ManagerProperties);
            SearchResultCollection ManagerResults = ManagerSearch.FindAll();
            foreach (SearchResult result in ManagerResults)
            {
                ManagerData = (string)result.Properties["mail"][0];
                System.Diagnostics.Debug.WriteLine("***** ManagerEmail Value is: " + ManagerData + "*****"); 
            }
        }
        catch { }
        return ManagerData;
    }
//<==================================================< Dan Engle 10/9/2018 <===================================================================<

    #region " Page Load "

    // **********************************************   START Page Load   *********************************************************
    protected void Page_Load(object sender, EventArgs e)
    {
 //>===========================================> Dan Engle 10/8/2018 >=============================================================>
        txtTodaysDate.Text = DateTime.Now.ToShortDateString();
        
        if (!IsPostBack)
        {
            string AccountName = WindowsIdentity.GetCurrent().Name;//this get the current users full logged in name e.g. oai\dengle

            txtLoggedInUser.Text = AccountName;//this sets an existing var used on front-end to populate asp textbox called "txtLoggedInUser"
            //and the var was used from CCALK's code but the assignment was changed

            string UsrMgrDN = GetUsersManager(AccountName.Substring(5));//this assigns var to the result after getting the user's manager's DN
            System.Diagnostics.Debug.WriteLine("***** UsrMgrDN is: " + UsrMgrDN + "*****");//used to see verify the val of the parameter passed in to next func

            string MgrVal = GetManagersData(UsrMgrDN);//this assigned the var to the result after getting the Mgrs data

 //<==============================================< Dan Engle 10/9/2018 <===========================================================<

            //CCALK 6/21/2012 WindowsIdentity would not work in our current setup - using Request.ServerVariables["LOGON_USER"] instead
            //txtLoggedInUser.Text = Request.ServerVariables["LOGON_USER"];//commented out by Dan Engle 10/8/2018

            //CCALK 6/12/2012 Pull departments from DB - populate department drop down list
            GetDepartments();

            //CCALK 6/12/2012 Set initial description text row for all drop down lists
            SetInitialDDLRow();

            //CCALK 6/15/2012 Get the user info from DB - populate Name, phone and email address fields
            GetUserInformation();
        }
    }
    // **********************************************   END Page Load   *********************************************************
  
    #endregion

    #region " Controls "


    // **********************************************   START rblBusinessPrivate_SelectedIndexChanged   *********************************************************
    //CCALK Company is not required for personal shipments
    //CCalk 6/27/2012 T Zimmerman - Remove all requi
    //protected void rblBusinessPrivate_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    if (rblBusinessPrivate.SelectedItem.Text == "Personal")
    //    {
    //        txtToCompany.Enabled = false;
    //    }
    //    else
    //    {
    //        txtToCompany.Enabled = true;
    //    }
    //}
    // **********************************************   START rblBusinessPrivate_SelectedIndexChanged   *********************************************************

    // **********************************************   START ddlCountries_SelectedIndexChanged   *********************************************************
    //CCALK Disable Sates DDL if Contries DDL = anything other than "United States"
    protected void ddlCountries_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlCountries.SelectedItem.Text != "United States")
        {
            ddlStates.Enabled = false;
            ddlStates.SelectedItem.Text = string.Empty;
        }
        else
        {
            ddlStates.Enabled = true;
            ddlStates.SelectedItem.Text = "Alabama";
        }
    }
    // **********************************************   END ddlCountries_SelectedIndexChanged   *********************************************************
    // **********************************************   START Clear Fields   *********************************************************
    //CCALK Clear all fields except logged in user, date, and the From fields (name, phone, email)
    protected void btnClear_Click(object sender, EventArgs e)
    {       
        rblBusinessPrivate.SelectedIndex = 0;
        //lblShippingNumber.Text = string.Empty;
        ddlInitiatingDepartment.SelectedIndex = 0;
        if (ddlSubDepartments.Visible == true) ddlSubDepartments.SelectedIndex = 0;
        ddlSubDepartments.Visible = false;
        txtOtherInitiatingDepartment.Text = string.Empty;
        txtOtherInitiatingDepartment.Visible = false;
        txtToName.Text = string.Empty;
        ddlCountries.SelectedItem.Text = "United States";
        txtToCompany.Text = string.Empty;
        txtToAddressOne.Text = string.Empty;
        txtToAddressTwo.Text = string.Empty;
        txtCity.Text = string.Empty;
        ddlStates.Enabled = true;
        ddlStates.SelectedItem.Text = "Alabama";
        txtPostalCode.Text = string.Empty;
        txtToPhone.Text = string.Empty;
        ddlNumberItemsShipped.SelectedIndex = 0;
        calShipByDate.SelectedDate = DateTime.Today;
        ddlShipByTime.SelectedIndex = 0;
        txtComments.Text = string.Empty;
        txtBillTo3rdParty.Text = string.Empty;
        cbSignatureRequired.Checked = false;
        txtDescriptionOne.Text = string.Empty;
        txtDescriptionTwo.Text = string.Empty;
        txtDescriptionThree.Text = string.Empty;
        txtDescriptionFour.Text = string.Empty;
        txtDescriptionFive.Text = string.Empty;
        txtDescriptionSix.Text = string.Empty;
        txtDescriptionOne.Visible = false;
        txtDescriptionTwo.Visible = false;
        txtDescriptionThree.Visible = false;
        txtDescriptionFour.Visible = false;
        txtDescriptionFive.Visible = false;
        txtDescriptionSix.Visible = false;
        txtQuantityOne.Text = string.Empty;
        txtQuantityTwo.Text = string.Empty;
        txtQuantityThree.Text = string.Empty;
        txtQuantityFour.Text = string.Empty;
        txtQuantityFive.Text = string.Empty;
        txtQuantitySix.Text = string.Empty;
        txtQuantityOne.Visible = false;
        txtQuantityTwo.Visible = false;
        txtQuantityThree.Visible = false;
        txtQuantityFour.Visible = false;
        txtQuantityFive.Visible = false;
        txtQuantitySix.Visible = false;
    }
    // **********************************************   END Clear Fields   *********************************************************
    // **********************************************   START Basic Focus Fields   *********************************************************
    protected void ddlSubDepartments_SelectedIndexChanged(object sender, EventArgs e)
    {
        txtFromName.Focus();
    }
    protected void calShipByDate_SelectionChanged(object sender, EventArgs e)
    {
        ddlShipByTime.Focus();
    }
    // **********************************************   END Basic Focus Fields   *********************************************************
    // **********************************************   START ddlInitiatingDepartment_SelectedIndexChanged   *********************************************************
    //CCALK Logic to handle user selection:
    //  * Administration
    //  * Operations
    //  * MX
    //  * Other
    protected void ddlInitiatingDepartment_SelectedIndexChanged(object sender, EventArgs e)
    {
        //CCALK 6/13/2012 Logic to handle value of "Other" selected in ddlInitiatingDepartment - display txtOtherInitiatingDepartment text box
        if (ddlInitiatingDepartment.SelectedItem.Text == "Other")
        {
            txtOtherInitiatingDepartment.Visible = true;
            txtOtherInitiatingDepartment.Focus();
        }
        else
        {
            txtOtherInitiatingDepartment.Visible = false;
        }
        //END Logic to handle value of "Other" selected in ddlInitiatingDepartment
        //CCALK 6/13/2012 Logic to handle value of "Administration" or "MX" or "Operations" selected in 
        //ddlInitiatingDepartment - display ddlSubDepartments ddl and populate with appropriate values from DB
        if (ddlInitiatingDepartment.SelectedItem.Text == "Administration")
        {
            ddlSubDepartments.Visible = true;
            //CCALK 6/12/2012 Pull departments from DB
            DataTable shippingDept = new DataTable();
            //Set up connection string & SqlConnection instance
            //SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["OAI_ShippingRequest"].ConnectionString);//Commented out by Dan Engle 10/16/2018
            using(myConnection = new SqlConnection (ConfigurationManager.ConnectionStrings["OAI_ShippingRequest"].ConnectionString))//Added by Dan to replace above connection
            {
                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT Department_ID, Department_Name FROM Shipping_Department WHERE Department_Type = 'A'", myConnection);
                    adapter.Fill(shippingDept);
                    ddlSubDepartments.DataSource = shippingDept;
                    ddlSubDepartments.DataTextField = "Department_Name";
                    ddlSubDepartments.DataValueField = "Department_ID";
                    ddlSubDepartments.DataBind();
                }
                catch (Exception ex)
                {
                    lblValidationSummary.Text = "Sub Department drop down list was not populated - " + ex.Message;
                }
            }
            ddlSubDepartments.Items.Insert(0, new ListItem("<Select sub department>", "0"));
            txtOtherInitiatingDepartment.Visible = false;
        }  
        else if (ddlInitiatingDepartment.SelectedItem.Text == "MX")
        {
            ddlSubDepartments.Visible = true;
            //CCALK 6/12/2012 Pull departments from DB
            DataTable shippingDept = new DataTable();
            //Set up connection string & SqlConnection instance
            //SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["OAI_ShippingRequest"].ConnectionString);//Removed by Dan Engle 10/16/2018
            using (myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["OAI_ShippingRequest"].ConnectionString))//Added by Dan to replace above connection
            {
                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT Department_ID, Department_Name FROM Shipping_Department WHERE Department_Type = 'M'", myConnection);
                    adapter.Fill(shippingDept);
                    ddlSubDepartments.DataSource = shippingDept;
                    ddlSubDepartments.DataTextField = "Department_Name";
                    ddlSubDepartments.DataValueField = "Department_ID";
                    ddlSubDepartments.DataBind();
                }
                catch (Exception ex)
                {
                    lblValidationSummary.Text = "Sub Department drop down list was not populated - " + ex.Message;
                }
            }
            ddlSubDepartments.Items.Insert(0, new ListItem("<Select sub department>", "0"));
            txtOtherInitiatingDepartment.Visible = false;
        }
        else if (ddlInitiatingDepartment.SelectedItem.Text == "Operations")
        {
            ddlSubDepartments.Visible = true;
            //CCALK 6/12/2012 Pull departments from DB
            DataTable shippingDept = new DataTable();
            //Set up connection string & SqlConnection instance
            //SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["OAI_ShippingRequest"].ConnectionString);//Removed by Dan Engle 10/16/2018
            using (myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["OAI_ShippingRequest"].ConnectionString))//Added by Dan to replace above connection
            {
                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT Department_ID, Department_Name FROM Shipping_Department WHERE Department_Type = 'O'", myConnection);
                    adapter.Fill(shippingDept);
                    ddlSubDepartments.DataSource = shippingDept;
                    ddlSubDepartments.DataTextField = "Department_Name";
                    ddlSubDepartments.DataValueField = "Department_ID";
                    ddlSubDepartments.DataBind();
                }
                catch (Exception ex)
                {
                    lblValidationSummary.Text = "Sub Department drop down list was not populated - " + ex.Message;
                }
            }
            ddlSubDepartments.Items.Insert(0, new ListItem("<Select sub department>", "0"));
            txtOtherInitiatingDepartment.Visible = false;
        }
        else 
        {
            ddlSubDepartments.Visible = false;
        }
        //END Logic to handle value of "Administration" or "MX" or "Operations" selected in ddlInitiatingDepartment    
    }
 
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        //CCALK Verify required fields have been populated with data - if not a Validation summary StringBuilder is created and sent to the Validation Summary label
        ValidatePage();     
        if (lblValidationSummary.Text.Length > 0) { return; }
        //END       
        SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["OAI_ShippingRequest"].ConnectionString);
        myConnection.Open();
        SqlCommand cmd = new SqlCommand();
        //CCALK 6/6/2012 Insert Shipping Request
        cmd = new SqlCommand("uspInsertShippingRequest", myConnection);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("@Ship_Type_Business_Private", SqlDbType.NVarChar).Value = Server.HtmlDecode(rblBusinessPrivate.SelectedValue);
        cmd.Parameters.Add("@ShipInitatingDate", SqlDbType.Date).Value = Convert.ToDateTime(txtTodaysDate.Text);
        //CCALK 6/13/2012 Determine if selected value is a primary, sub or "other" department and pass appropriate value to DB
        if (ddlInitiatingDepartment.SelectedItem.Text == "Administration" || ddlInitiatingDepartment.SelectedItem.Text == "MX" || ddlInitiatingDepartment.SelectedItem.Text == "Operations") 
            cmd.Parameters.Add("@InitiatingDepartment", SqlDbType.NVarChar).Value = Server.HtmlDecode(ddlSubDepartments.SelectedItem.Text);
        else if (ddlInitiatingDepartment.SelectedItem.Text == "Other")
            cmd.Parameters.Add("@InitiatingDepartment", SqlDbType.NVarChar).Value = Server.HtmlDecode(txtOtherInitiatingDepartment.Text);
        else
        cmd.Parameters.Add("@InitiatingDepartment", SqlDbType.NVarChar).Value = Server.HtmlDecode(ddlInitiatingDepartment.SelectedItem.Text);
        //END Determine if selected value is a primary, sub or "other" department and pass appropriate value to DB
        cmd.Parameters.Add("@From_Name", SqlDbType.NVarChar).Value = Server.HtmlDecode(txtFromName.Text);
        cmd.Parameters.Add("@To_Name", SqlDbType.NVarChar).Value = Server.HtmlDecode(txtToName.Text);
        cmd.Parameters.Add("@To_Company", SqlDbType.NVarChar).Value = Server.HtmlDecode(txtToCompany.Text);
        cmd.Parameters.Add("@To_Address_One", SqlDbType.NVarChar).Value = Server.HtmlDecode(txtToAddressOne.Text);
        cmd.Parameters.Add("@To_Address_Two", SqlDbType.NVarChar).Value = Server.HtmlDecode(txtToAddressTwo.Text);
        cmd.Parameters.Add("@To_Phone", SqlDbType.NVarChar).Value = Server.HtmlDecode(txtToPhone.Text);
        cmd.Parameters.Add("@From_Phone", SqlDbType.NVarChar).Value = Server.HtmlDecode(txtFromPhone.Text);
        cmd.Parameters.Add("@Shipped_By_Date", SqlDbType.Date).Value = Convert.ToDateTime(calShipByDate.SelectedDate);
        cmd.Parameters.Add("@Shipped_By_Time", SqlDbType.Time).Value = ConvertTime(ddlShipByTime.SelectedValue);
        cmd.Parameters.Add("@Signature_Required", SqlDbType.Bit).Value = cbSignatureRequired.Checked;
        cmd.Parameters.Add("@Bill_To_Third_Party", SqlDbType.NVarChar).Value = Server.HtmlDecode(txtBillTo3rdParty.Text);
        cmd.Parameters.Add("@Logged_In_User", SqlDbType.NVarChar).Value = Server.HtmlDecode(txtLoggedInUser.Text);
        cmd.Parameters.Add("@Comments", SqlDbType.NVarChar).Value = Server.HtmlDecode(txtComments.Text);
        cmd.Parameters.Add("@From_Email_Address", SqlDbType.NVarChar).Value = Server.HtmlDecode(txtEmailAddress.Text);
        cmd.Parameters.Add("@To_Country", SqlDbType.NVarChar).Value = Server.HtmlDecode(ddlCountries.SelectedItem.Value);
        cmd.Parameters.Add("@To_City", SqlDbType.NVarChar).Value = Server.HtmlDecode(txtCity.Text);
        cmd.Parameters.Add("@Urgency", SqlDbType.NVarChar).Value = Server.HtmlDecode(ddlUrgency.SelectedItem.Value);//added by Dan 10/29/2018
        //CCALK 6/25/2012 If shipped internationally - State is not required - write empty string
        if (ddlCountries.SelectedItem.Text == "United States")
        {
            cmd.Parameters.Add("@To_State", SqlDbType.NVarChar).Value = Server.HtmlDecode(ddlStates.SelectedItem.Value);
        }
        else
        {
            cmd.Parameters.Add("@To_State", SqlDbType.NVarChar).Value = Server.HtmlDecode("");
        }
        //END 
        cmd.Parameters.Add("@To_Postal_Code", SqlDbType.NVarChar).Value = Server.HtmlDecode(txtPostalCode.Text);
        //CCALK 6/6/2012 Gets Id of inserted record 
        object id;
        id = cmd.ExecuteScalar();
        txtShipID.Text = id.ToString();
        //CCALK 6/6/2012 Insert Items to be shipped
        if (txtDescriptionOne.Text != "")
        {
            cmd = new SqlCommand("uspInsertShippingItems", myConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
            cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = txtDescriptionOne.Text;
            cmd.Parameters.Add("@Quantity", SqlDbType.NVarChar).Value = txtQuantityOne.Text;
            cmd.ExecuteNonQuery();
        }
        if (txtDescriptionTwo.Text != "")
        {
            cmd = new SqlCommand("uspInsertShippingItems", myConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
            cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = txtDescriptionTwo.Text;
            cmd.Parameters.Add("@Quantity", SqlDbType.NVarChar).Value = txtQuantityTwo.Text;
            cmd.ExecuteNonQuery();
        }
        if (txtDescriptionThree.Text != "")
        {
            cmd = new SqlCommand("uspInsertShippingItems", myConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
            cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = txtDescriptionThree.Text;
            cmd.Parameters.Add("@Quantity", SqlDbType.NVarChar).Value = txtQuantityThree.Text;
            cmd.ExecuteNonQuery();
        }
        if (txtDescriptionFour.Text != "")
        {
            cmd = new SqlCommand("uspInsertShippingItems", myConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
            cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = txtDescriptionFour.Text;
            cmd.Parameters.Add("@Quantity", SqlDbType.NVarChar).Value = txtQuantityFour.Text;
            cmd.ExecuteNonQuery();
        }
        if (txtDescriptionFive.Text != "")
        {
            cmd = new SqlCommand("uspInsertShippingItems", myConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
            cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = txtDescriptionFive.Text;
            cmd.Parameters.Add("@Quantity", SqlDbType.NVarChar).Value = txtQuantityFive.Text;
            cmd.ExecuteNonQuery();
        }
        if (txtDescriptionSix.Text != "")
        {
            cmd = new SqlCommand("uspInsertShippingItems", myConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
            cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = txtDescriptionSix.Text;
            cmd.Parameters.Add("@Quantity", SqlDbType.NVarChar).Value = txtQuantitySix.Text;
            cmd.ExecuteNonQuery();
        }
        /*
        *CCALK 6/15/2012 Insert user information
        * 1. Check to see if a row for that user exists - if it does not then create the row
        * 2. If a row does exist for the user set the row to the data in the Name, Phone and Email fields
        */
        //1. Check to see if a row exists for this user
        cmd = new SqlCommand("uspUserCount", myConnection);
        cmd.CommandType = CommandType.StoredProcedure;

        System.Diagnostics.Debug.WriteLine("User is: " + txtLoggedInUser.Text);

        cmd.Parameters.Add("@User_ID", SqlDbType.NVarChar).Value = txtLoggedInUser.Text; 
        object count = cmd.ExecuteScalar();
        if (Convert.ToInt16(count) == 0)
        //If count = 0 then no record exists for this user so create the record
        {
            cmd = new SqlCommand("uspInsertUserInformation", myConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@User_ID", SqlDbType.NVarChar).Value = txtLoggedInUser.Text;
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = txtFromName.Text;
            cmd.Parameters.Add("@Phone", SqlDbType.NVarChar).Value = txtFromPhone.Text;
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = txtEmailAddress.Text;
            cmd.ExecuteNonQuery();
        }
        else
        {
            //count >= 1 the row exists - update existing row
            cmd = new SqlCommand("uspUpdateUserInformation", myConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@User_ID", SqlDbType.NVarChar).Value = txtLoggedInUser.Text;
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = txtFromName.Text;
            cmd.Parameters.Add("@Phone", SqlDbType.NVarChar).Value = txtFromPhone.Text;
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = txtEmailAddress.Text;
            cmd.ExecuteNonQuery();
        }
        //END Insert user information
        myConnection.Close();//Left in place due to this connection being closed=>Dan Engle 10/16/2018
        //CCALK Create and display text file
        //CreateTextFile(id);
        Server.Transfer("Completed.aspx");
    }
    // **********************************************   END Submit Process   *********************************************************
    // **********************************************   START ddlNumberItemsShipped_SelectedIndexChanged   *********************************************************
    //CCALK 6/4/2012 Display rows based on user input - up to 6 rows
    //CCALK 6/18/2012 Remove any text from rows that are not displayed
    protected void ddlNumberItemsShipped_SelectedIndexChanged(object sender, EventArgs e)
    {
        switch (ddlNumberItemsShipped.SelectedIndex)
        {
            case 1:
                txtDescriptionOne.Focus();              
                txtDescriptionOne.Visible = true;
                txtQuantityOne.Visible = true;
                //Hide rows             
                txtDescriptionTwo.Visible = false;
                txtQuantityTwo.Visible = false;              
                txtDescriptionThree.Visible = false;
                txtQuantityThree.Visible = false;             
                txtDescriptionFour.Visible = false;
                txtQuantityFour.Visible = false;               
                txtDescriptionFive.Visible = false;
                txtQuantityFive.Visible = false;            
                txtDescriptionSix.Visible = false;
                txtQuantitySix.Visible = false;
                //Remove any text that might have been entered
                txtDescriptionTwo.Text = string.Empty;
                txtQuantityTwo.Text = string.Empty;              
                txtDescriptionThree.Text = string.Empty;
                txtQuantityThree.Text = string.Empty;               
                txtDescriptionFour.Text = string.Empty;
                txtQuantityFour.Text = string.Empty;              
                txtDescriptionFive.Text = string.Empty;
                txtQuantityFive.Text = string.Empty;              
                txtDescriptionSix.Text = string.Empty;
                txtQuantitySix.Text = string.Empty;
                break;
            case 2:
                txtDescriptionOne.Focus();
                txtDescriptionOne.Visible = true;
                txtQuantityOne.Visible = true;
                txtDescriptionTwo.Visible = true;
                txtQuantityTwo.Visible = true;
                txtDescriptionThree.Visible = false;
                txtQuantityThree.Visible = false;
                txtDescriptionFour.Visible = false;
                txtQuantityFour.Visible = false;
                txtDescriptionFive.Visible = false;
                txtQuantityFive.Visible = false;
                txtDescriptionSix.Visible = false;
                txtQuantitySix.Visible = false;
                //Remove any text that might have been input
                txtDescriptionThree.Text = string.Empty;
                txtQuantityThree.Text = string.Empty;
                txtDescriptionFour.Text = string.Empty;
                txtQuantityFour.Text = string.Empty;
                txtDescriptionFive.Text = string.Empty;
                txtQuantityFive.Text = string.Empty;
                txtDescriptionSix.Text = string.Empty;
                txtQuantitySix.Text = string.Empty;
                break;
            case 3:
                txtDescriptionOne.Focus();
                txtDescriptionOne.Visible = true;
                txtQuantityOne.Visible = true;
                txtDescriptionTwo.Visible = true;
                txtQuantityTwo.Visible = true;
                txtDescriptionThree.Visible = true;
                txtQuantityThree.Visible = true;
                txtDescriptionFour.Visible = false;
                txtQuantityFour.Visible = false;
                txtDescriptionFive.Visible = false;
                txtQuantityFive.Visible = false;
                txtDescriptionSix.Visible = false;
                txtQuantitySix.Visible = false;
                //Remove unecessary text
                txtDescriptionFour.Text = string.Empty;
                txtQuantityFour.Text = string.Empty;
                txtDescriptionFive.Text = string.Empty;
                txtQuantityFive.Text = string.Empty;
                txtDescriptionSix.Text = string.Empty;
                txtQuantitySix.Text = string.Empty;
                break;
            case 4:
                txtDescriptionOne.Focus();
                txtDescriptionOne.Visible = true;
                txtQuantityOne.Visible = true;
                txtDescriptionTwo.Visible = true;
                txtQuantityTwo.Visible = true;
                txtDescriptionThree.Visible = true;
                txtQuantityThree.Visible = true;
                txtDescriptionFour.Visible = true;
                txtQuantityFour.Visible = true;
                txtDescriptionFive.Visible = false;
                txtQuantityFive.Visible = false;
                txtDescriptionSix.Visible = false;
                txtQuantitySix.Visible = false;
                //Remove unecessary text
                txtDescriptionFive.Text = string.Empty;
                txtQuantityFive.Text = string.Empty;
                txtDescriptionSix.Text = string.Empty;
                txtQuantitySix.Text = string.Empty;
                break;
            case 5:
                txtDescriptionOne.Focus();
                txtDescriptionOne.Visible = true;
                txtQuantityOne.Visible = true;
                txtDescriptionTwo.Visible = true;
                txtQuantityTwo.Visible = true;
                txtDescriptionThree.Visible = true;
                txtQuantityThree.Visible = true;
                txtDescriptionFour.Visible = true;
                txtQuantityFour.Visible = true;
                txtDescriptionFive.Visible = true;
                txtQuantityFive.Visible = true;
                txtDescriptionSix.Visible = false;
                txtQuantitySix.Visible = false;
                //Remove unecessary text
                txtDescriptionSix.Text = string.Empty;
                txtQuantitySix.Text = string.Empty;
                break;
            case 6:
                txtDescriptionOne.Focus();
                txtDescriptionOne.Visible = true;
                txtQuantityOne.Visible = true;
                txtDescriptionTwo.Visible = true;
                txtQuantityTwo.Visible = true;
                txtDescriptionThree.Visible = true;
                txtQuantityThree.Visible = true;
                txtDescriptionFour.Visible = true;
                txtQuantityFour.Visible = true;
                txtDescriptionFive.Visible = true;
                txtQuantityFive.Visible = true;
                txtDescriptionSix.Visible = true;
                txtQuantitySix.Visible = true;
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// ddlUrgency_SelectedIndexChanged event handler added by Dan 10/29/2018 to add the selection to the comments for the warehouse
    /// </summary>
    protected void ddlUrgency_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlUrgency.SelectedItem.Text != "<Flexibility of arrival>")
        {
            System.Diagnostics.Debug.WriteLine("ddl txt is: " + ddlUrgency.SelectedItem.Value);
            txtComments.Text = "Shipping urgency is " + ddlUrgency.SelectedItem.Text;
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("if bool did not run, you hit the else");
        }
    }
    // **********************************************   START ddlNumberItemsShipped_SelectedIndexChanged   *********************************************************
    #endregion

    #region " Methods "

    // **********************************************   START Text File Process   *********************************************************
    //CCALK Send all relevant information to text file and display for user to print
    private void CreateTextFile(object shipNumber)
    {

        StringBuilder printString = new StringBuilder("", 25);

        printString.AppendLine("OAI Shipping Request");
        printString.AppendLine("Date: " + txtTodaysDate.Text);
        printString.AppendLine("Ship #: " + shipNumber);
        printString.AppendLine("Ship Type: " + rblBusinessPrivate.SelectedItem.Text);

        //CCALK 6/20/2012 Determine if selected value is a primary, sub or "other" department and pass appropriate value to StreamWriter
        if (ddlInitiatingDepartment.SelectedItem.Text == "Administration" || ddlInitiatingDepartment.SelectedItem.Text == "MX" || ddlInitiatingDepartment.SelectedItem.Text == "Operations")
            printString.AppendLine("Initiating Department: " + ddlSubDepartments.SelectedItem.Text);
        else if (ddlInitiatingDepartment.SelectedItem.Text == "Other")
            printString.AppendLine("Initiating Department: " + txtOtherInitiatingDepartment.Text);
        else
            printString.AppendLine("Initiating Department: " + ddlInitiatingDepartment.SelectedItem.Text);
        //END Determine if selected value is a primary, sub or "other" department and pass appropriate value to StreamWriter

        printString.AppendLine(" ");
        printString.AppendLine("FROM: " + txtFromName.Text);
        printString.AppendLine("Phone: " + txtFromPhone.Text);
        printString.AppendLine("Email: " + txtEmailAddress.Text);

        printString.AppendLine(" ");
        printString.AppendLine("TO: " + txtToName.Text);
        printString.AppendLine("Country: " + ddlCountries.SelectedItem.Text);
        printString.AppendLine("Company: " + txtToCompany.Text);
        printString.AppendLine("Address One: " + txtToAddressOne.Text);
        printString.AppendLine("Address Two: " + txtToAddressTwo.Text);
        printString.AppendLine("City: " + txtCity.Text);
        printString.AppendLine("State: " + ddlStates.SelectedItem.Text);
        printString.AppendLine("Postal Code: " + txtPostalCode.Text);
        printString.AppendLine("Phone: " + txtToPhone.Text);

        printString.AppendLine(" ");
        if (txtDescriptionOne.Text != "") { printString.AppendLine("Package 1: " + txtDescriptionOne.Text + "          Qty: " + txtQuantityOne.Text); }
        if (txtDescriptionTwo.Text != "") { printString.AppendLine("Package 2: " + txtDescriptionTwo.Text + "          Qty: " + txtQuantityTwo.Text); }
        if (txtDescriptionThree.Text != "") { printString.AppendLine("Package 3: " + txtDescriptionThree.Text + "          Qty: " + txtQuantityThree.Text); }
        if (txtDescriptionFour.Text != "") { printString.AppendLine("Package 4: " + txtDescriptionFour.Text + "          Qty: " + txtQuantityFour.Text); }
        if (txtDescriptionFive.Text != "") { printString.AppendLine("Package 5: " + txtDescriptionFive.Text + "          Qty: " + txtQuantityFive.Text); }
        if (txtDescriptionSix.Text != "") { printString.AppendLine("Package 6: " + txtDescriptionSix.Text + "          Qty: " + txtQuantitySix.Text); }

        printString.AppendLine(" ");
        printString.AppendLine("Ship Date: " + calShipByDate.SelectedDate.ToShortDateString());
        printString.AppendLine("Ship Time: " + ddlShipByTime.SelectedItem.Text);

        //added Urgency AppendLine by Dan Engle 10/29/2018
        printString.AppendLine(" ");
        printString.AppendLine("Urgency: " + ddlUrgency.SelectedItem.Text);

        printString.AppendLine(" ");
        printString.AppendLine("Comments: " + txtComments.Text);

        printString.AppendLine(" ");
        printString.AppendLine("Bill to 3rd Party: " + txtBillTo3rdParty.Text);

        printString.AppendLine(" ");
        printString.AppendLine("Signature Required: " + cbSignatureRequired.Checked);

        printString.AppendLine(" ");
        printString.AppendLine(" ");
        printString.AppendLine(" ");
        printString.AppendLine("Manager's Signature____________________________________________________________");
        printString.AppendLine("Note: Package will NOT ship without signature of immediate manager.");

        byte[] buffer;
        using (var memoryStream = new System.IO.MemoryStream())
        {
            buffer = Encoding.Default.GetBytes(printString.ToString()); 
            memoryStream.Write(buffer, 0, buffer.Length);
            Response.Clear();
            //This wil force browser to silently download file
            Response.AddHeader("Content-Disposition", "attachment; filename=ship_request.txt");
            Response.AddHeader("Content-Length", memoryStream.Length.ToString());
            Response.ContentType = "text/plain"; //This is MIME type 
            memoryStream.WriteTo(Response.OutputStream);
        }

            Response.End();

        //CCALK Trying to figure out how force a page refresh
        //try
        //{
        //    Response.End();
        //}
        //catch { }
        //finally
        //{
        //    if (lblValidationSummary.Text.Length == 0)
        //    {
        //        lblValidationSummary.Text = string.Empty;
        //    }
        //}
 
    }
    // **********************************************   END Text File Process   *********************************************************


    // **********************************************   START GetDepartments()   *********************************************************
    //CCALK 6/12/2012 Pull departments from DB and populate Initiating Department DDL
    private void GetDepartments()
    {
        DataTable shippingDept = new DataTable();

        //SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["OAI_ShippingRequest"].ConnectionString);//Removed by Dan Engle 10/16/2018
        using (myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["OAI_ShippingRequest"].ConnectionString))//Added by Dan to replace above connection
        {
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT Department_ID, Department_Name FROM Shipping_Department WHERE Department_Type = 'P'", myConnection);
                adapter.Fill(shippingDept);
                ddlInitiatingDepartment.DataSource = shippingDept;
                ddlInitiatingDepartment.DataTextField = "Department_Name";
                ddlInitiatingDepartment.DataValueField = "Department_ID";
                ddlInitiatingDepartment.DataBind();
            }
            catch (Exception ex)
            {
                lblValidationSummary.Text = "Initiating Department drop down list was not populated - " + ex.Message;
            }
            myConnection.Dispose();
        }
    }
    // **********************************************   END GetDepartments()   *********************************************************



    // **********************************************   START GetUserInformation()   *********************************************************
    //CCALK If user information (name, phone, email) exists for the user populate those fields
    private void GetUserInformation()
    {
        //SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["OAI_ShippingRequest"].ConnectionString);//Removed by Dan Engle 10/16/2018
        using (myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["OAI_ShippingRequest"].ConnectionString))//Added by Dan to replace above connection
        try
        {
            //CCALK 6/15/2012 Get the user info from DB
            myConnection.Open();

            SqlCommand cmd = new SqlCommand();

            //CCALK 6/15/2012 If a record exists for this user - pull from DB and populate name, phone and email fields
            cmd = new SqlCommand("uspSelectUserInformation", myConnection);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@User_ID", SqlDbType.NVarChar).Value = txtLoggedInUser.Text;

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    txtFromName.Text = reader["Name"].ToString();
                    txtFromPhone.Text = reader["Phone"].ToString();
                    txtEmailAddress.Text = reader["Email"].ToString();
                }
            }

            reader.Close();

            myConnection.Close();
        }

        catch (Exception ex)
        {
            lblValidationSummary.Text = ex.Message;
        }
    }
    // **********************************************   END GetUserInformation()   *********************************************************



    // **********************************************   START SetInitialDDLRow()   *********************************************************
    //CCALK Set initial row for all page DDL's 
    private void SetInitialDDLRow()
    {
        ddlInitiatingDepartment.Items.Insert(0, new ListItem("<Select initiating department>", "0"));
        ddlInitiatingDepartment.Focus();

        ddlNumberItemsShipped.Items.Insert(0, new ListItem("<Number of boxes to ship>", "0"));

        ddlShipByTime.Items.Insert(0, new ListItem("<ETA of shipment>", "0"));

        ddlUrgency.Items.Insert(0, new ListItem("<Flexibility of arrival>", "0"));//added by Dan to set default
    }
    // **********************************************   END SetInitialDDLRow()   *********************************************************


    // **********************************************   START ValidateShippingAdresses()   *********************************************************
    //CCALK Validate shipping address fields before saving or updating to DB
    private void ValidateShippingAdresses()
    {
        //CCALK 6/20/2012 Output all data entry errors for the shipping address fields
        StringBuilder OAI_ValidationSummary = new StringBuilder("", 25);

        lblValidationSummaryShipping.Text = string.Empty;

        if (txtToName.Text == string.Empty)
        {
            OAI_ValidationSummary.Append("<br>");
            OAI_ValidationSummary.Append("* To is a required field.");

            lblValidationSummaryShipping.Text = OAI_ValidationSummary.ToString();
        }
        if (ddlCountries.SelectedItem.Text == string.Empty)
        {
            OAI_ValidationSummary.Append("<br>");
            OAI_ValidationSummary.Append("* Country is a required field.");

            lblValidationSummaryShipping.Text = OAI_ValidationSummary.ToString();
        }
        //CCALK 6/25/2012 Company name not required for personal shipments - Thomas Zimmerman
        //CCALK 6/27/2012 It was decided not to do this by Thomas Zimmerman
        //if (rblBusinessPrivate.SelectedItem.Text == "Business")
        //{
        //    if (txtToCompany.Text == string.Empty)
        //    {
        //        OAI_ValidationSummary.Append("<br>");
        //        OAI_ValidationSummary.Append("* Company is a required field.");
        //        lblValidationSummaryShipping.Text = OAI_ValidationSummary.ToString();
        //    }
        //}
        if (txtToAddressOne.Text == string.Empty)
        {
            OAI_ValidationSummary.Append("<br>");
            OAI_ValidationSummary.Append("* Address One is a required field.");

            lblValidationSummaryShipping.Text = OAI_ValidationSummary.ToString();
        }
        if (txtCity.Text == string.Empty)
        {
            OAI_ValidationSummary.Append("<br>");
            OAI_ValidationSummary.Append("* City is a required field.");

            lblValidationSummaryShipping.Text = OAI_ValidationSummary.ToString();
        }
        if (ddlCountries.SelectedItem.Text == "United States") //State is not required if shipping out of country
        {
            if (ddlStates.SelectedItem.Text == string.Empty)
            {
                OAI_ValidationSummary.Append("<br>");
                OAI_ValidationSummary.Append("* State is a required field.");

                lblValidationSummaryShipping.Text = OAI_ValidationSummary.ToString();
            }
        }
        //CCalk 9/17/2012 Zip code is not required if shipping out of country
        if (ddlCountries.SelectedItem.Text == "United States") 
        {
            if (txtPostalCode.Text == string.Empty)
            {
                OAI_ValidationSummary.Append("<br>");
                OAI_ValidationSummary.Append("* Postal Code is a required field.");

                lblValidationSummaryShipping.Text = OAI_ValidationSummary.ToString();
            }
            else if (NumberIsNumeric(txtPostalCode.Text) == false)//Only do a NumberIsNumeric check if shipped stateside
            {
                OAI_ValidationSummary.Append("<br>");
                OAI_ValidationSummary.Append("* Please enter only numbers for the postal code field.");

                lblValidationSummaryShipping.Text = OAI_ValidationSummary.ToString();
            }
        }
        //CCalk 9/17/2012 Because the zip code field may or may not have characters - check length and verify IsNumeric if any characters added
        //CCalk 9/18/2012 International zip codes can be ether alpha-numeric or non existent so remove all checks
        //if (txtPostalCode.Text.Length > 0)
        //{
        //    if (NumberIsNumeric(txtPostalCode.Text) == false)
        //    {
        //        OAI_ValidationSummary.Append("<br>");
        //        OAI_ValidationSummary.Append("* Please enter only numbers for the postal code field.");

        //        lblValidationSummaryShipping.Text = OAI_ValidationSummary.ToString();
        //    }
        //}
        if (txtToPhone.Text == string.Empty)
        {
            OAI_ValidationSummary.Append("<br>");
            OAI_ValidationSummary.Append("* Phone is a required field.");

            lblValidationSummaryShipping.Text = OAI_ValidationSummary.ToString();
        }
        //CCalk - Validate Domestic and International phone numbers for shipping addresses
        //if (ValidPhoneNumber(txtToPhone.Text) == false)
        //{
        //    if (ddlCountries.SelectedItem.Text == "United States")
        //    {
        //        //CCALK Display correct format for domestic numbers
        //        OAI_ValidationSummary.Append("<br>");
        //        OAI_ValidationSummary.Append("* Please enter a valid TO phone number. Domestic Examples: 999-999-9999 OR (999) 999-9999");
        //    }
        //    else
        //    {
        //        //CCALK Display some correct formats for international numbers
        //        OAI_ValidationSummary.Append("<br>");
        //        OAI_ValidationSummary.Append("* Please enter a valid TO phone number. International Examples: (England - +44 20 8xxx xxxx) OR (Ireland +353 23 88xxxxx) OR (Kuwait +965 2200xxxx) ");
        //    }

        //    lblValidationSummaryShipping.Text = OAI_ValidationSummary.ToString();
        //}
    }
    // **********************************************   END ValidateShippingAdresses()   *********************************************************



    // **********************************************   START ValidatePage()   *********************************************************
    //CCALK Validate all fields before saving to DB - any errors sent to StringBuilder and sent to Validation Summary label
    private void ValidatePage()
    {
        //CCALK 6/7/2012 Output all data entry errors
        StringBuilder OAI_ValidationSummary = new StringBuilder("", 25);

        lblValidationSummary.Text = string.Empty;

        if (ddlInitiatingDepartment.SelectedValue == "0")
        {
            OAI_ValidationSummary.Append("* Initiating Department is a required field.");

            lblValidationSummary.Text = OAI_ValidationSummary.ToString();
        }

        if (ddlInitiatingDepartment.SelectedItem.Text == "Other")
        {
            string other = txtOtherInitiatingDepartment.Text;

            if (other.Length == 0)
            {
                OAI_ValidationSummary.Append("<br>");
                OAI_ValidationSummary.Append("* Other Initiating Department is a required field.");

                lblValidationSummary.Text = OAI_ValidationSummary.ToString();
            }
        }

        if (ddlInitiatingDepartment.SelectedItem.Text == "Administration" || ddlInitiatingDepartment.SelectedItem.Text == "MX" || ddlInitiatingDepartment.SelectedItem.Text == "Operations")
        {
            if (ddlSubDepartments.SelectedValue == "0")
            {
                OAI_ValidationSummary.Append("* Please choose a sub department.");

                lblValidationSummary.Text = OAI_ValidationSummary.ToString();
            }
        }

        if (txtFromName.Text == string.Empty)
        {
            OAI_ValidationSummary.Append("<br>");
            OAI_ValidationSummary.Append("* From Name is a required field.");

            lblValidationSummary.Text = OAI_ValidationSummary.ToString();
        }

        if (txtFromPhone.Text == string.Empty)
        {
            OAI_ValidationSummary.Append("<br>");
            OAI_ValidationSummary.Append("* From Phone is a required field.");

            lblValidationSummary.Text = OAI_ValidationSummary.ToString();
        }

        if (txtEmailAddress.Text == string.Empty)
        {
            OAI_ValidationSummary.Append("<br>");
            OAI_ValidationSummary.Append("* From Email Address is a required field.");

            lblValidationSummary.Text = OAI_ValidationSummary.ToString();
        }

        if (txtToName.Text == string.Empty)
        {
            OAI_ValidationSummary.Append("<br>");
            OAI_ValidationSummary.Append("* To Name is a required field.");

            lblValidationSummary.Text = OAI_ValidationSummary.ToString();
        }

        if (ddlCountries.SelectedItem.Text == string.Empty)
        {
            OAI_ValidationSummary.Append("<br>");
            OAI_ValidationSummary.Append("* To Country is a required field.");

            lblValidationSummary.Text = OAI_ValidationSummary.ToString();
        }
        //CCALK 6/27/2012 Make sure either Business or Private is selected
        if (rblBusinessPrivate.SelectedIndex == -1)
        {
            OAI_ValidationSummary.Append("<br>");
            OAI_ValidationSummary.Append("* Please choose either a Business or Personal shipping type.");

            lblValidationSummary.Text = OAI_ValidationSummary.ToString();
        }
        //CCALK 6/25/2012 Company name not required for personal shipments
        //CCALK 6/27/2012 It was decided to remove this and just make the company field not required
        //if (rblBusinessPrivate.SelectedIndex != -1)
        //{
        //    if (rblBusinessPrivate.SelectedItem.Text == "Business")
        //    {
        //        if (txtToCompany.Text == string.Empty)
        //        {
        //            OAI_ValidationSummary.Append("<br>");
        //            OAI_ValidationSummary.Append("* To Company is a required field.");
        //            lblValidationSummary.Text = OAI_ValidationSummary.ToString();
        //        }
        //    }
        //}
        if (txtToAddressOne.Text == string.Empty)
        {
            OAI_ValidationSummary.Append("<br>");
            OAI_ValidationSummary.Append("* To Address One is a required field.");

            lblValidationSummary.Text = OAI_ValidationSummary.ToString();
        }
        //if (txtToAddressTwo.Text == string.Empty)
        //{
        //    OAI_ValidationSummary.Append("<br>");
        //    OAI_ValidationSummary.Append("* To Address Two is a required field.");

        //    lblValidationSummary.Text = OAI_ValidationSummary.ToString();
        //}
        if (txtCity.Text == string.Empty)
        {
            OAI_ValidationSummary.Append("<br>");
            OAI_ValidationSummary.Append("* City is a required field.");

            lblValidationSummary.Text = OAI_ValidationSummary.ToString();
        }

        if (ddlCountries.SelectedItem.Text == "United States") //State is not required if shipping out of country
        {
            if (ddlStates.SelectedItem.Text == string.Empty)
            {
                OAI_ValidationSummary.Append("<br>");
                OAI_ValidationSummary.Append("* State is a required field.");

                lblValidationSummary.Text = OAI_ValidationSummary.ToString();
            }
        }

        //CCalk 9/17/2012 Zip code is not required if shipping out of country
        if (ddlCountries.SelectedItem.Text == "United States")
        {
            if (txtPostalCode.Text == string.Empty)
            {
                OAI_ValidationSummary.Append("<br>");
                OAI_ValidationSummary.Append("* Postal Code is a required field.");

                lblValidationSummary.Text = OAI_ValidationSummary.ToString();
            }
            else if (NumberIsNumeric(txtPostalCode.Text) == false)//Only do a NumberIsNumeric check if shipped stateside
            {
                OAI_ValidationSummary.Append("<br>");
                OAI_ValidationSummary.Append("* Please enter only numbers for the postal code field.");

                lblValidationSummary.Text = OAI_ValidationSummary.ToString();
            }
        }

        ////CCalk 9/17/2012 Because the zip code field may or may not have characters - check length and verify IsNumeric if any characters added
        //CCalk 9/18/2012 International zip codes are alpha-numeric so remove the NumberIsNumeric check
        //if (txtPostalCode.Text.Length > 0)
        //{
        //    if (NumberIsNumeric(txtPostalCode.Text) == false)
        //    {
        //        OAI_ValidationSummary.Append("<br>");
        //        OAI_ValidationSummary.Append("* Please enter only numbers for the postal code field.");

        //        lblValidationSummary.Text = OAI_ValidationSummary.ToString();
        //    }
        //}

        if (txtToPhone.Text == string.Empty)
        {
            OAI_ValidationSummary.Append("<br>");
            OAI_ValidationSummary.Append("* To Phone is a required field.");

            lblValidationSummary.Text = OAI_ValidationSummary.ToString();
        }

        //CCALK 6/7/2012 Validate Items to ship controls

        switch (ddlNumberItemsShipped.SelectedIndex)
        {
            case 0:

                OAI_ValidationSummary.Append("<br>");
                OAI_ValidationSummary.Append("* Please choose the number of items to ship.");

                lblValidationSummary.Text = OAI_ValidationSummary.ToString();

                break;
            case 1:

                if (txtDescriptionOne.Text == string.Empty)
                {
                    OAI_ValidationSummary.Append("<br>");
                    OAI_ValidationSummary.Append("* All descriptions are required for shipped items.");

                    lblValidationSummary.Text = OAI_ValidationSummary.ToString();
                }

                //CCalk 9/18/2012 Need ability to add NA
                //if (NumberIsNumeric(txtQuantityOne.Text) == false)
                //{
                //    OAI_ValidationSummary.Append("<br>");
                //    OAI_ValidationSummary.Append("* Please enter only numbers for the quantity fields.");

                //    lblValidationSummary.Text = OAI_ValidationSummary.ToString();
                //}

                break;
            case 2:

                if (txtDescriptionTwo.Text == string.Empty)
                {
                    OAI_ValidationSummary.Append("<br>");
                    OAI_ValidationSummary.Append("* All descriptions are required for shipped items.");

                    lblValidationSummary.Text = OAI_ValidationSummary.ToString();
                }

                //CCalk 9/18/2012 Need ability to add NA
                //if (NumberIsNumeric(txtQuantityOne.Text) == false || NumberIsNumeric(txtQuantityTwo.Text) == false)
                //{
                //    OAI_ValidationSummary.Append("<br>");
                //    OAI_ValidationSummary.Append("* Please enter only numbers for the quantity fields.");

                //    lblValidationSummary.Text = OAI_ValidationSummary.ToString();
                //}

                break;

            case 3:

                if (txtDescriptionThree.Text == string.Empty)
                {
                    OAI_ValidationSummary.Append("<br>");
                    OAI_ValidationSummary.Append("* All descriptions are required for shipped items.");

                    lblValidationSummary.Text = OAI_ValidationSummary.ToString();
                }

                //CCalk 9/18/2012 Need ability to add NA
                //if (NumberIsNumeric(txtQuantityOne.Text) == false || NumberIsNumeric(txtQuantityTwo.Text) == false || NumberIsNumeric(txtQuantityThree.Text) == false)
                //{
                //    OAI_ValidationSummary.Append("<br>");
                //    OAI_ValidationSummary.Append("* Please enter only numbers for the quantity fields.");

                //    lblValidationSummary.Text = OAI_ValidationSummary.ToString();
                //}

                break;

            case 4:

                if (txtDescriptionFour.Text == string.Empty)
                {
                    OAI_ValidationSummary.Append("<br>");
                    OAI_ValidationSummary.Append("* All descriptions are required for shipped items.");

                    lblValidationSummary.Text = OAI_ValidationSummary.ToString();
                }

                //CCalk 9/18/2012 Need ability to add NA
                //if (NumberIsNumeric(txtQuantityOne.Text) == false || NumberIsNumeric(txtQuantityTwo.Text) == false || NumberIsNumeric(txtQuantityThree.Text) == false || NumberIsNumeric(txtQuantityFour.Text) == false)
                //{
                //    OAI_ValidationSummary.Append("<br>");
                //    OAI_ValidationSummary.Append("* Please enter only numbers for the quantity fields.");

                //    lblValidationSummary.Text = OAI_ValidationSummary.ToString();
                //}

                break;

            case 5:

                if (txtDescriptionFive.Text == string.Empty)
                {
                    OAI_ValidationSummary.Append("<br>");
                    OAI_ValidationSummary.Append("* All descriptions are required for shipped items.");

                    lblValidationSummary.Text = OAI_ValidationSummary.ToString();
                }

                //CCalk 9/18/2012 Need ability to add NA - so removed NumberIsNumeric check
                //if (NumberIsNumeric(txtQuantityOne.Text) == false || NumberIsNumeric(txtQuantityTwo.Text) == false || NumberIsNumeric(txtQuantityThree.Text) == false || NumberIsNumeric(txtQuantityFour.Text) == false || NumberIsNumeric(txtQuantityFive.Text) == false)
                //{
                //    OAI_ValidationSummary.Append("<br>");
                //    OAI_ValidationSummary.Append("* Please enter only numbers for the quantity fields.");

                //    lblValidationSummary.Text = OAI_ValidationSummary.ToString();
                //}

                break;

            case 6:

                if (txtDescriptionSix.Text == string.Empty)
                {
                    OAI_ValidationSummary.Append("<br>");
                    OAI_ValidationSummary.Append("* All descriptions are required for shipped items.");

                    lblValidationSummary.Text = OAI_ValidationSummary.ToString();
                }

                //CCalk 9/18/2012 Need ability to add NA
                //if (NumberIsNumeric(txtQuantityOne.Text) == false || NumberIsNumeric(txtQuantityTwo.Text) == false || NumberIsNumeric(txtQuantityThree.Text) == false || NumberIsNumeric(txtQuantityFour.Text) == false || NumberIsNumeric(txtQuantityFive.Text) == false || NumberIsNumeric(txtQuantitySix.Text) == false)
                //{
                //    OAI_ValidationSummary.Append("<br>");
                //    OAI_ValidationSummary.Append("* Please enter only numbers for the quantity fields.");

                //    lblValidationSummary.Text = OAI_ValidationSummary.ToString();
                //}

                break;

        }

        //END Validate Items to ship controls

        if (calShipByDate.SelectedDate == Convert.ToDateTime("1/1/0001 12:00:00 AM"))
        {
            OAI_ValidationSummary.Append("<br>");
            OAI_ValidationSummary.Append("* Ship By Date is a required field.");

            lblValidationSummary.Text = OAI_ValidationSummary.ToString();
        }


        if (ddlShipByTime.SelectedValue == "0")
        {
            OAI_ValidationSummary.Append("<br>");
            OAI_ValidationSummary.Append("* Time To Ship is a required field.");
            lblValidationSummary.Text = OAI_ValidationSummary.ToString();
        }

        /// <summary>
        /// ddlUrgency validation added by Dan 10/29/2018 to add the selection to the comments for the warehouse
        /// </summary>
        if (ddlUrgency.SelectedValue == "0")
        {
            OAI_ValidationSummary.Append("<br>");
            OAI_ValidationSummary.Append("* Urgency is a required field.");
            lblValidationSummary.Text = OAI_ValidationSummary.ToString();
        }

        //Only validate stateside - some countries do not have postal codes
        if (ddlCountries.SelectedItem.Text == "United States")
        {
            if (NumberIsNumeric(txtPostalCode.Text) == false)
            {
                OAI_ValidationSummary.Append("<br>");
                OAI_ValidationSummary.Append("* Please enter only numbers for the postal code field.");

                lblValidationSummary.Text = OAI_ValidationSummary.ToString();
            }
        }

        //CCAlk - Specifically for the FROM phone number - separate method for TO phone number
        if (ValidDomesticPhoneNumber(txtFromPhone.Text) == false)
        {
            OAI_ValidationSummary.Append("<br>");
            OAI_ValidationSummary.Append("* Please enter a valid FROM phone number.");

            lblValidationSummary.Text = OAI_ValidationSummary.ToString();
        }

        //CCalk Validate Domestic and International phone numbers for final page submit
        //if (ValidPhoneNumber(txtToPhone.Text) == false)
        //{
        //    if (ddlCountries.SelectedItem.Text == "United States")
        //    {
        //        //CCALK Display correct format for domestic numbers
        //        OAI_ValidationSummary.Append("<br>");
        //        OAI_ValidationSummary.Append("* Please enter a valid TO phone number. Domestic Examples: 999-999-9999 OR (999) 999-9999");
        //    }
        //    else
        //    {
        //        //CCALK Display some correct formats for international numbers
        //        OAI_ValidationSummary.Append("<br>");
        //        OAI_ValidationSummary.Append("* Please enter a valid TO phone number. International Examples: (England +44 20 8xxx xxxx) OR (Ireland +353 23 88xxxxx) OR (Kuwait +965 2200xxxx) ");
        //    }

        //    lblValidationSummary.Text = OAI_ValidationSummary.ToString();
        //}       
    }
    // **********************************************   END ValidatePage()   *********************************************************


    //**********************************************   START ConvertTime()   *********************************************************
    //Convert to military time for storage in DB
    private object ConvertTime(string p)
    {
        //Converts standard time in ddlShipTime to military time and stores in this format 14:00:00
        string time = "";

        switch (p)
        {
            case "12:00 AM":

                time = "12:00:00";

                break;

            case "1:00 AM":

                time = "1:00:00";

                break;

            case "2:00 AM":

                time = "2:00:00";

                break;

            case "3:00 AM":

                time = "3:00:00";

                break;

            case "4:00 AM":

                time = "4:00:00";

                break;

            case "5:00 AM":

                time = "5:00:00";

                break;

            case "6:00 AM":

                time = "6:00:00";

                break;

            case "7:00 AM":

                time = "7:00:00";

                break;

            case "8:00 AM":

                time = "8:00:00";

                break;

            case "9:00 AM":

                time = "9:00:00";

                break;

            case "10:00 AM":

                time = "10:00:00";

                break;

            case "11:00 AM":

                time = "11:00:00";

                break;

            case "12:00 PM":

                time = "12:00:00";

                break;

            case "1:00 PM":

                time = "13:00:00";

                break;

            case "2:00 PM":

                time = "14:00:00";

                break;

            case "3:00 PM":

                time = "15:00:00";

                break;

            case "4:00 PM":

                time = "16:00:00";

                break;

            case "5:00 PM":

                time = "17:00:00";

                break;

            case "6:00 PM":

                time = "18:00:00";

                break;

            case "7:00 PM":

                time = "19:00:00";

                break;

            case "8:00 PM":

                time = "20:00:00";

                break;

            case "9:00 PM":

                time = "21:00:00";

                break;

            case "10:00 PM":

                time = "22:00:00";

                break;

            case "11:00 PM":

                time = "23:00:00";

                break;

        }

        return TimeSpan.Parse(time);
    }
    //**********************************************   END ConvertTime()   *********************************************************


    //**********************************************   START NumberIsNumeric()   *********************************************************
    //CCALK Regular Expression to determine if user input is numeric
    bool NumberIsNumeric(string value)
    {
        bool isNumeric = false;

        Regex re = new Regex(@"^\d+$");
        isNumeric = re.Match(value).Success;

        return isNumeric;
    }
    //**********************************************   END NumberIsNumeric()   *********************************************************

    //**********************************************   START ValidDomesticPhoneNumber()   *********************************************************
    //CCALK Regular Expression to determine if user input is a valid domestic phone number - used for the FROM phone
    bool ValidDomesticPhoneNumber(string phoneNumber)
    {
        bool isValidPhoneNumber = false;

        Regex domestic = new Regex(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$");

        isValidPhoneNumber = domestic.IsMatch(phoneNumber);

        return isValidPhoneNumber;
    }
    //**********************************************   END ValidDomesticPhoneNumber()   *********************************************************

    //**********************************************   START ValidPhoneNumber()   *********************************************************
    //CCALK Regular Expression to determine if user input is a valid phone number - either domestic or international
    //bool ValidPhoneNumber(string phoneNumber)
    //{
    //    bool isValidPhoneNumber = false;

    //    if (ddlCountries.SelectedItem.Text == "United States")
    //    {
    //        Regex domestic = new Regex(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$");

    //        isValidPhoneNumber = domestic.IsMatch(phoneNumber);
    //    }
    //    else
    //    {
    //        Regex international = new Regex(@"^\+(?:[0-9] ?){6,14}[0-9]$");

    //        isValidPhoneNumber = international.IsMatch(phoneNumber);
    //    }

    //    return isValidPhoneNumber;
    //}
    //**********************************************   END ValidPhoneNumber()   *********************************************************

    #endregion

    #region " "To" Record Stuff "

    // **********************************************   START Save Shipping Addresses Process   *********************************************************
    //CCALK Saves shipping addresses to DB
    protected void btnSave_Click(object sender, EventArgs e)
    {

        lblValidationSummaryShipping.Text = string.Empty;

        //CCALK Verify required fields have been populated with data
        ValidateShippingAdresses();

        if (lblValidationSummaryShipping.Text.Length > 0) { return; }
        //END

        //Save the current 'to' record to the DB
        //Shipping information is saved to the DB and tied to specific users by login name - ex: "ccalk"

        SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["OAI_ShippingRequest"].ConnectionString);

        myConnection.Open();

        SqlCommand cmd = new SqlCommand();

        cmd = new SqlCommand("uspInsertShippingClientAddress", myConnection);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add("@User_ID", SqlDbType.NVarChar).Value = Server.HtmlDecode(txtLoggedInUser.Text);
        cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = Server.HtmlDecode(txtToName.Text);
        cmd.Parameters.Add("@Country", SqlDbType.NVarChar).Value = Server.HtmlDecode(ddlCountries.SelectedItem.Value);
        cmd.Parameters.Add("@Company", SqlDbType.NVarChar).Value = Server.HtmlDecode(txtToCompany.Text);
        cmd.Parameters.Add("@AddressOne", SqlDbType.NVarChar).Value = Server.HtmlDecode(txtToAddressOne.Text);
        cmd.Parameters.Add("@AddressTwo", SqlDbType.NVarChar).Value = Server.HtmlDecode(txtToAddressTwo.Text);
        cmd.Parameters.Add("@City", SqlDbType.NVarChar).Value = Server.HtmlDecode(txtCity.Text);
        //CCALK 6/25/2012 If shipped internationally - State is not required - write empty string
        if (ddlCountries.SelectedItem.Text == "United States")
        {
            cmd.Parameters.Add("@State", SqlDbType.NVarChar).Value = Server.HtmlDecode(ddlStates.SelectedItem.Value);
        }
        else
        {
            cmd.Parameters.Add("@State", SqlDbType.NVarChar).Value = Server.HtmlDecode("");
        }
        //END
        cmd.Parameters.Add("@Postal_Code", SqlDbType.NVarChar).Value = Server.HtmlDecode(txtPostalCode.Text);
        cmd.Parameters.Add("@Phone", SqlDbType.NVarChar).Value = Server.HtmlDecode(txtToPhone.Text);
        
        cmd.ExecuteScalar();

        myConnection.Close();

        GetShippingAddresses();

    }
    // **********************************************   END Save Shipping Addresses Process   *********************************************************


    // **********************************************   START Delete Shipping Address Process   *********************************************************
    //CCALK Delete selected shipping address from DB
    protected void btnDelete_Click(object sender, EventArgs e)
    {

        lblValidationSummaryShipping.Text = string.Empty;

        SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["OAI_ShippingRequest"].ConnectionString);

        myConnection.Open();

        SqlCommand cmd = new SqlCommand();

        cmd = new SqlCommand("uspDeleteClientAdresses", myConnection);

        cmd.CommandType = CommandType.StoredProcedure;

        try
        {
            cmd.Parameters.Add("@Shipping_Client_Addresses_ID", SqlDbType.Int).Value = Convert.ToInt64(txtID.Text);

            cmd.ExecuteNonQuery();
            myConnection.Close();

            //CCALK 66/19/2012 Clear address rows
            txtToName.Text = string.Empty;
            ddlCountries.SelectedItem.Text = "United States";
            txtToCompany.Text = string.Empty;
            txtToAddressOne.Text = string.Empty;
            txtToAddressTwo.Text = string.Empty;
            txtCity.Text = string.Empty;
            ddlStates.SelectedItem.Text = "Georgia";
            txtPostalCode.Text = string.Empty;
            txtToPhone.Text = string.Empty;

            GetShippingAddresses();

            ClearShippingFields();

            myConnection.Close();

        }
        catch (FormatException ex)
        {
            lblValidationSummaryShipping.Text = "You are trying to delete an address that does not exist in the database.";
        }

    }
    // **********************************************   END Delete Shipping Address Process   *********************************************************


    // **********************************************   START UPDATE Shipping Address Process   *********************************************************
    //CCALK Update selected shipping address in DB
    protected void btnUpdate_Click(object sender, EventArgs e)
    {

        lblValidationSummaryShipping.Text = string.Empty;

        //CCALK Verify required fields have been populated with data
        ValidateShippingAdresses();

        if (lblValidationSummaryShipping.Text.Length > 0) { return; }
        //END

        //CCALK 6/18/2012 Update client address
        SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["OAI_ShippingRequest"].ConnectionString);

        myConnection.Open();//Connection closes 25 lines below

        SqlCommand cmd = new SqlCommand();

        cmd = new SqlCommand("uspUpdateClientAddress", myConnection);

        cmd.CommandType = CommandType.StoredProcedure;


        try
        {

            cmd.Parameters.Add("@Shipping_Client_Addresses_ID", SqlDbType.Int).Value = Convert.ToInt16(txtID.Text);
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = Server.HtmlDecode(txtToName.Text);
            cmd.Parameters.Add("@Country", SqlDbType.NVarChar).Value = Server.HtmlDecode(ddlCountries.SelectedItem.Value);
            cmd.Parameters.Add("@Company", SqlDbType.NVarChar).Value = Server.HtmlDecode(txtToCompany.Text);
            cmd.Parameters.Add("@AddressOne", SqlDbType.NVarChar).Value = Server.HtmlDecode(txtToAddressOne.Text);
            cmd.Parameters.Add("@AddressTwo", SqlDbType.NVarChar).Value = Server.HtmlDecode(txtToAddressTwo.Text);
            cmd.Parameters.Add("@City", SqlDbType.NVarChar).Value = Server.HtmlDecode(txtCity.Text);
            cmd.Parameters.Add("@State", SqlDbType.NVarChar).Value = Server.HtmlDecode(ddlStates.SelectedItem.Value);
            cmd.Parameters.Add("@Postal_Code", SqlDbType.NVarChar).Value = Server.HtmlDecode(txtPostalCode.Text);
            cmd.Parameters.Add("@Phone", SqlDbType.NVarChar).Value = Server.HtmlDecode(txtToPhone.Text);

            cmd.ExecuteNonQuery();

            myConnection.Close();//Verified connection closes from 25 lines above

            GetShippingAddresses();

        }
        catch (FormatException ex)
        {
            lblValidationSummaryShipping.Text = "You are trying to update an address that does not exist in the database. If this is a new address please click the Save button instead.";
        }

    }
    // **********************************************   END UPDATE Shipping Address Process   *********************************************************


    // **********************************************   START Search Process   *********************************************************
    //CCALK Populate and display Grid View with all addresses tied to this specific user
    protected void btnSearch_Click(object sender, EventArgs e)
    {

        lblValidationSummaryShipping.Text = string.Empty;

        //Pulls all records in the database tied to the logged in user
        GetShippingAddresses();
    }

    private void GetShippingAddresses()
    {
        gvClientAddresses.Visible = true;

        SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["OAI_ShippingRequest"].ConnectionString);

        myConnection.Open();

        SqlCommand myCommand = new SqlCommand("uspSelectClientAdresses", myConnection);

        myCommand.CommandType = CommandType.StoredProcedure;

        myCommand.Parameters.Add("@USER_ID", SqlDbType.NVarChar).Value = txtLoggedInUser.Text;

        DataSet myDataSet = new DataSet();

        SqlDataAdapter myAdapter = new SqlDataAdapter(myCommand);
        myAdapter.Fill(myDataSet);

        gvClientAddresses.DataSource = myDataSet;
        gvClientAddresses.DataBind();

        myConnection.Close();//Verified closed SQL connection 
    }
    // **********************************************   END Search Process   *********************************************************

    // **********************************************   START Row Click Process   ****************************************************
    //CCALK Display records based on selected Grid View row
    protected void OnRowCommand(object sender, GridViewCommandEventArgs e)
    {

        int index = Convert.ToInt32(e.CommandArgument);

        GridViewRow gvRow = gvClientAddresses.Rows[index];

        txtID.Text = Server.HtmlDecode(gvRow.Cells[1].Text);
        txtToName.Text = Server.HtmlDecode(gvRow.Cells[2].Text);
        //CCALK 6/25/2012 Countries and States are stored in DB as 2 letter codes - FedEx requirement - display on page as full name using .SelectedValue
        ddlCountries.SelectedValue = Server.HtmlDecode(gvRow.Cells[3].Text);

        //CCALK 7/3/2012 (NOTE: The following is obviously a hack) Replace &#39; with ' in the company name - for some reason the ' is replaced with &#39; which 
        //the browser interprets as a server side script attack???
        //UPDATE 9/20/2012: Server.HtmlDecode fixes this problem!

        //Determine if &#39; is in the string
        //string s1 = gvRow.Cells[4].Text;

        ////string s1 = Server.HtmlDecode(gvRow.Cells[4].Text);

        //int indexOf = s1.IndexOf("&#39;");

        //if (indexOf > 0)
        //{
        //    //&#39; IS in the string so replace with '
        //    s1 = s1.Replace("&#39;", "'");
        //}
       
        //txtToCompany.Text = s1;
        //txtToCompany.Text = Regex.Replace(gvRow.Cells[4].Text, "[^a-zA-Z0-9]+", "", RegexOptions.Compiled);
        //END Replace &#39; with ' in the company name

        txtToCompany.Text = Server.HtmlDecode(gvRow.Cells[4].Text);
        txtToAddressOne.Text = Server.HtmlDecode(gvRow.Cells[5].Text);
        txtToAddressTwo.Text = Server.HtmlDecode(gvRow.Cells[6].Text);
        txtCity.Text = Server.HtmlDecode(gvRow.Cells[7].Text);
        //CCALK International addresses = no State value stored - display empty string
        if (gvRow.Cells[3].Text == "US")
        {
            ddlStates.SelectedValue = Server.HtmlDecode(gvRow.Cells[8].Text);
            ddlStates.Enabled = true;
        }
        else
        {
            ddlStates.SelectedItem.Text = Server.HtmlDecode("");
            ddlStates.Enabled = false;
        }
        
        txtPostalCode.Text = Server.HtmlDecode(gvRow.Cells[9].Text);
        txtToPhone.Text = Server.HtmlDecode(gvRow.Cells[10].Text);

        gvClientAddresses.Visible = false;
    }
    // **********************************************   END Row Click Process   *********************************************************


    protected void btnClearShippingFields_Click(object sender, EventArgs e)
    {
        ClearShippingFields();
    }

    private void ClearShippingFields()
    {

        lblValidationSummaryShipping.Text = string.Empty;

        txtToName.Text = string.Empty;
        ddlCountries.SelectedItem.Text = "United States";
        txtToCompany.Text = string.Empty;
        txtToAddressOne.Text = string.Empty;
        txtToAddressTwo.Text = string.Empty;
        txtCity.Text = string.Empty;
        ddlStates.Enabled = true;
        ddlStates.SelectedItem.Text = "Georgia";
        txtPostalCode.Text = string.Empty;
        txtToPhone.Text = string.Empty;
    }


    #endregion


}