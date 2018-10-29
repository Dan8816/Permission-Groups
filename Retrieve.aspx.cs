using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        dbquery(System.DateTime.Now.AddDays(-120), System.DateTime.Now);
    }
    public void dbquery(DateTime fromDate, DateTime toDate)
    {
        string connectionSTR = "Server=AAG-Omni;Database=OAI_ShippingRequest;Integrated Security=SSPI";
        //
        //using (ShippingRequestDataContext db = new ShippingRequestDataContext(new SqlConnection(connectionSTR)))
        //{
        //}
        //

        SqlConnection allrequests = new SqlConnection(connectionSTR);
        ShippingRequestDataContext db = new ShippingRequestDataContext(allrequests);

        var query = from x in db.Shipping_Requests
                    //where x.Ship_Type_Business_Private == "P"
                    where x.ShipInitatingDate.Value > fromDate
                    where x.ShipInitatingDate.Value <= toDate
                     select new
                     {
                         id = x.ID,
                         Sender = x.From_Name,
                         Purpose = x.Ship_Type_Business_Private,
                         Cost = x.Original_Cost
                     };

        Literal1.Text = "<table id='main-table' class='display table table-hover text-center'>";
            Literal1.Text += "<thead>";
                Literal1.Text += "<tr>";
                    Literal1.Text += "<th>ID</th>";
                    Literal1.Text += "<th>Sender</th>";
                    Literal1.Text += "<th>Purpose</th>";
                    Literal1.Text += "<th>Cost</th>";
                Literal1.Text += "</tr>";
            Literal1.Text += "</thead>";
            Literal1.Text += "<tbody>";

        foreach (var rec in query)
        {
            Literal1.Text += "<tr>";
                Literal1.Text += "<td><a href='UpdateInfo.aspx?field1=" + rec.id + "'>" + rec.id + "</a></td>";
                Literal1.Text += "<td>" + rec.Sender + "</td>";
                Literal1.Text += "<td>" + rec.Purpose + "</td>";
                Literal1.Text += "<td>" + rec.Cost.ToString("C") + "</td>";
            Literal1.Text += "</tr>";
        }
            Literal1.Text += "</tbody>";
        Literal1.Text += "</table>";

        foreach (var q in query)
        {
            System.Diagnostics.Debug.WriteLine(">>>>> " + q.Sender + " <<<<<");
        }
        //GridView1.DataSource = querry;
        //GridView1.DataBind();

        db.Dispose();
        
    }
}