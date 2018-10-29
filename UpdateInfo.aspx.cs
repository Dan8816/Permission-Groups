using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.DirectoryServices;
using System.Security.Principal;
using System.DirectoryServices.AccountManagement;

public partial class _Default : Page
{
    public string connectionSTR = "Server=AAG-Omni;Database=OAI_ShippingRequest;Integrated Security=SSPI";//MSSQL db string

    public string permissionGroup = "app-ShippingRequestManagement";//perm group name

    SqlConnection myConnection;

    public string path = "LDAP://omni.phish/dc=omni,dc=phish"; //<=Remove this later<=/\=>will use this for OAI=>>//"LDAP://oai.local/OU=Users,OU=OAI Company,dc=oai,dc=local";

    public string User = WindowsIdentity.GetCurrent().Name.Substring(5);//if dev env use substr 5, or OAI use substr 4

    public bool CheckIsInGroup(string GroupName, string UserName)
    {
        bool InGroup = false;
        PrincipalContext pc = new PrincipalContext(ContextType.Domain, "omni.phish", userName: "sa-shipreq-01", password: "p@$$word999999");
        GroupPrincipal gp = GroupPrincipal.FindByIdentity(pc, IdentityType.Name, GroupName);
        foreach (Principal p in gp.GetMembers(true))
        {
            if (p.SamAccountName == UserName)
            {
                InGroup = true;
                System.Diagnostics.Debug.WriteLine("User is a member of permission group and the value is " + p.SamAccountName);
            }
        }
        gp.Dispose();
        pc.Dispose();
        return InGroup;
    }

    public class LINQresult
    {
        public int Id { get; set; }
        public string Purpose { get; set; }
        public string Sender { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Tracking { get; set; }
        public decimal Cost { get; set; }
        public string ReqDate { get; set; }
        public string ArvDate { get; set; }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            var PermGroupCheck = CheckIsInGroup(permissionGroup, User);

            if (PermGroupCheck != true)
            {
                Input_TrackingNum.Enabled = false;
                Input_Cost.Enabled = false;
                btnModify.Visible = false;
            }
            var x = ThisReQquery();//need to add a bool to handle a null error, presently user sees a sql error
            Input_TrackingNum.Text = x[0].Tracking;
            Input_Cost.Text = x[0].Cost.ToString();
            if (x[0].Purpose == "B")
            {
                Label_Purpose.Text = "Business";
            }
            if (x[0].Purpose == "P")
            {
                Label_Purpose.Text = "Personal";
            }
            Label_User.Text = x[0].Sender;
            Label_ReqDate.Text = x[0].ReqDate;
            Label_ToAdd.Text = x[0].Address;
            Label_ToCity.Text = x[0].City;
            Label_ToCountry.Text = x[0].Country;
            Label_Tracking.Text = x[0].Tracking;
            Label_Cost.Text = x[0].Cost.ToString();
        }
    }
    public List<LINQresult> ThisReQquery()
    {
        SqlConnection oneShipReq = new SqlConnection(connectionSTR);
        ShippingRequestDataContext db = new ShippingRequestDataContext(oneShipReq);
        var SummaryQuery = (from x in db.Shipping_Requests
                    where x.ID == Convert.ToInt32(Request.QueryString["field1"])
                    select new LINQresult
                    {
                        Id = x.ID,
                        Sender = x.From_Name,
                        Address = x.To_Address_One,
                        City = x.To_City,
                        Country = x.To_Country,
                        Tracking = x.Tracking_Num,
                        Purpose = x.Ship_Type_Business_Private,
                        Cost = x.Original_Cost,
                        ReqDate = x.ShipInitatingDate.ToString(),
                        ArvDate = x.Shipped_By_Date.ToString()
                        
                    }).ToList();
        foreach (var record in SummaryQuery)
        {
            System.Diagnostics.Debug.WriteLine("Ship Req#: " + record.Id + " was sent by: " + record.Sender + " to " + record.Address + ", " + record.City + " in " + record.Country);
        }
        db.Dispose();
        Label_ShipID.Text = SummaryQuery[0].Id.ToString();
        return SummaryQuery;
    }

    protected void btnModify_Click(object sender, EventArgs e)//event handler to update tracking and cost info for this ship req on the page
    {
        Label_Tracking.Text = Input_TrackingNum.Text;
        Label_Cost.Text = Input_Cost.Text;
        System.Diagnostics.Debug.WriteLine("Tracking value is: " + Input_TrackingNum.Text + " and Cost value is " + Input_Cost.Text);
        SqlConnection updateShipReq = new SqlConnection(connectionSTR);
        ShippingRequestDataContext db = new ShippingRequestDataContext(updateShipReq);
        int? thisShipReq = Convert.ToInt32(Request.QueryString["field1"]);
        var UpdateFinder = db.Shipping_Requests.Where(s => s.ID.Equals(thisShipReq)).Select(s => s).ToList();
        foreach (var found in UpdateFinder)
        {
            found.Tracking_Num = Input_TrackingNum.Text;
                //var foo = decimal.Parse(string1);//Bryan's decimal to str conversion snippet
            var DecimalCost = decimal.Parse(Input_Cost.Text);
            found.Original_Cost = DecimalCost;
            db.SubmitChanges();
        }
        db.Dispose();
    }
}