<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ShippingRequest.aspx.cs" Inherits="ShippingRequest" ResponseEncoding="utf-8" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>OAI Shipping Request</title>
    <!-- <meta http-equiv="Content-Type" content="text/html; charset=UTF-8"/>-->
    <!--<link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />original css link-->
    <!--This page was refactored to a modernized layout that may display for mobile devices by Dan Engle 10/5/2018 but 
        the tags with "asp" attribute were left-over from exisitng code-->
    <link href="Content/bootstrap.css" rel="stylesheet" />
</head>
<body>
    <div class="container">
    <nav class="navbar navbar-dark bg-dark" style="padding: 0px;">
        <div class="container-fluid">
            <div class="navbar-header">
                <a class="navbar-brand" href="https://www.oai.aero/"><img src="images/omni_logo.png" alt="" style="padding: 0px; margin: 0px;"></a>
                <button class="btn btn-sm btn-primary" type="button"><a style="color:azure;" href="Retrieve.aspx">Retrieve request info</a></button>
                <!--<button class="btn btn-sm btn-outline-secondary" type="button"><a href="UpdateInfo.aspx">Modify request info</a></button>-->
            </div>
        </div>
    </nav>
    <div class="header text-center">
        <h4 class="display-5">Shipping Request Form</h4>
    </div>
    <div class="text-center">
        <asp:Label ID="lblValidationSummary" runat="server" Text="" ForeColor="red"></asp:Label><br />
    </div>
    <form class="form" id="form1" runat="server">
    <table class="table table-hover text-center">
        <thead>
            <tr>
                <th>Logged in as:</th>
                <th>
                    <asp:TextBox class="form-control" ID="txtLoggedInUser" runat="server" Enabled="False"></asp:TextBox>
                </th>
            </tr>
            <tr>
                <th>Date:</th>
                <th><asp:TextBox class="form-control" ID="txtTodaysDate" runat="server" AutoPostBack="True" Enabled="False"></asp:TextBox></th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <th>Purpose:</th>
                <th>
                    <asp:RadioButtonList ID="rblBusinessPrivate" runat="server" RepeatDirection="Horizontal" AutoPostBack="True">
                        <asp:ListItem Value="B"> Business</asp:ListItem>
                        <asp:ListItem Value="P"> Personal</asp:ListItem>
                    </asp:RadioButtonList>
                </th>
            </tr>
        </tbody>
    </table>            
    <asp:TextBox ID="txtShipID" runat="server" Visible="false"></asp:TextBox>
    <asp:panel id="Panel2" runat="server" groupingtext="Initiating Department">
        <asp:DropDownList class="form-control" ID="ddlInitiatingDepartment" runat="server" AutoPostBack="True" onselectedindexchanged="ddlInitiatingDepartment_SelectedIndexChanged"></asp:DropDownList>
        <asp:TextBox ID="txtOtherInitiatingDepartment" runat="server" Visible="False"></asp:TextBox>
        <asp:DropDownList class="form-control" ID="ddlSubDepartments" runat="server" AutoPostBack="True" Visible="False" onselectedindexchanged="ddlSubDepartments_SelectedIndexChanged"></asp:DropDownList>
    </asp:panel>
    <br><br>
    <asp:panel id="Panel3" runat="server" groupingtext="Shipping Information"> 
    <table class="table table-hover text-center">
	    <tr>
		<td><asp:Label ID="Label13" runat="server" Text="FROM" ></asp:Label></td>
        <td><asp:TextBox class="form-control" ID="txtFromName" runat="server"></asp:TextBox></td>
        </tr>       
       <tr>
        <td><asp:Label ID="Label3" runat="server" Text="Phone"></asp:Label></td>
        <td><asp:TextBox class="form-control" ID="txtFromPhone" runat="server"></asp:TextBox></td>
        </tr>
         <tr>
        <td><asp:Label ID="Label10" runat="server" Text="Email Address"></asp:Label></td>
        <td><asp:TextBox class="form-control" ID="txtEmailAddress" runat="server"></asp:TextBox></td>
        </tr>
    </table>
    <br>
    <asp:GridView ID="gvClientAddresses" runat="server" OnRowCommand="OnRowCommand" AutoGenerateColumns="False" CellPadding="4" ForeColor="#333333" GridLines="None">
        <AlternatingRowStyle BackColor="White"/>
        <Columns> 
            <asp:ButtonField CommandName="ButtonField" ButtonType="Button"/> 
            <asp:boundfield datafield="Shipping_Client_Addresses_ID" headertext="ID" /> 
            <asp:boundfield datafield="Name" headertext="Name"/> 
            <asp:boundfield datafield="Country" headertext="Country"/> 
            <asp:boundfield datafield="Company" headertext="Company"/> 
            <asp:boundfield datafield="AddressOne" headertext="Address One"/>
            <asp:boundfield datafield="AddressTwo" headertext="Address Two"/>
            <asp:boundfield datafield="City" headertext="City"/> 
            <asp:boundfield datafield="State" headertext="State"/> 
            <asp:boundfield datafield="Postal_Code" headertext="Postal Code"/> 
            <asp:boundfield datafield="Phone" headertext="Phone"/>
        </Columns>    
            <EditRowStyle BackColor="#2461BF"/>
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White"/>
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White"/>
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center"/>
            <RowStyle BackColor="#EFF3FB"/>
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333"/>
            <SortedAscendingCellStyle BackColor="#F5F7FB"/>
            <SortedAscendingHeaderStyle BackColor="#6D95E1"/>
            <SortedDescendingCellStyle BackColor="#E9EBEF"/>
            <SortedDescendingHeaderStyle BackColor="#4870BE"/>
    </asp:GridView>
    <br><br>
    <table>
        <tr>
            <td>
                <asp:Label ID="lblValidationSummaryShipping" runat="server" Text="" ForeColor="red"></asp:Label>
            </td>
        </tr>
    </table>
    <br />
    <div class="wrapper text-center">
        <div class="btn-group" role="group">
            <asp:Button class="btn btn-primary" ID="btnSearch" runat="server" Text="Search" onclick="btnSearch_Click" ToolTip="Search for previously entered shipping addresses."/>      
            <asp:Button class="btn btn-success" ID="btnSave" runat="server" Text="Save" onclick="btnSave_Click" ToolTip="Save new shipping adresses to the database."/>
            <asp:Button class="btn btn-secondary" ID="btnUpdate" runat="server" Text="Update" onclick="btnUpdate_Click" ToolTip="Update current shipping address."/>
            <asp:Button class="btn btn-danger" ID="btnDelete" runat="server" Text="Delete" onclick="btnDelete_Click" ToolTip="Delete current shipping address."/>
            <asp:Button class="btn btn-warning" ID="btnClearShippingFields" runat="server" Text="Clear" ToolTip="Clear shipping address fields." onclick="btnClearShippingFields_Click"/>
        </div>
    </div>
    <br><br>
    <table class="table table-hover text-center">
        <tr>
		    <td><asp:Label ID="Label16" runat="server" Text="TO"></asp:Label></td>
            <td><asp:TextBox class="form-control" ID="txtToName" runat="server"></asp:TextBox></td>
        </tr>
         <tr>
		    <td><asp:Label ID="Label15" runat="server" Text="Country"></asp:Label></td>
            <td><asp:DropDownList class="form-control" ID="ddlCountries" runat="server" AutoPostBack="True" onselectedindexchanged="ddlCountries_SelectedIndexChanged">
                     <asp:ListItem Value="AF">Afghanistan</asp:ListItem>
                     <asp:ListItem Value="AL">Albania</asp:ListItem>
                     <asp:ListItem Value="DZ">Algeria</asp:ListItem>
                     <asp:ListItem Value="AS">American Samoa</asp:ListItem>
                     <asp:ListItem Value="AD">Andorra</asp:ListItem>
                     <asp:ListItem Value="AO">Angola</asp:ListItem>
                     <asp:ListItem Value="AI">Anguilla</asp:ListItem>
                     <asp:ListItem Value="AQ">Antarctica</asp:ListItem>
                     <asp:ListItem Value="AG">Antigua And Barbuda</asp:ListItem>
                     <asp:ListItem Value="AR">Argentina</asp:ListItem>
                     <asp:ListItem Value="AM">Armenia</asp:ListItem>
                     <asp:ListItem Value="AW">Aruba</asp:ListItem>
                     <asp:ListItem Value="AU">Australia</asp:ListItem>
                     <asp:ListItem Value="AT">Austria</asp:ListItem>
                     <asp:ListItem Value="AZ">Azerbaijan</asp:ListItem>
                     <asp:ListItem Value="BS">Bahamas</asp:ListItem>
                     <asp:ListItem Value="BH">Bahrain</asp:ListItem>
                     <asp:ListItem Value="BD">Bangladesh</asp:ListItem>
                     <asp:ListItem Value="BB">Barbados</asp:ListItem>
                     <asp:ListItem Value="BY">Belarus</asp:ListItem>
                     <asp:ListItem Value="BE">Belgium</asp:ListItem>
                     <asp:ListItem Value="BZ">Belize</asp:ListItem>
                     <asp:ListItem Value="BJ">Benin</asp:ListItem>
                     <asp:ListItem Value="BM">Bermuda</asp:ListItem>
                     <asp:ListItem Value="BT">Bhutan</asp:ListItem>
                     <asp:ListItem Value="BO">Bolivia</asp:ListItem>
                     <asp:ListItem Value="BA">Bosnia And Herzegovina</asp:ListItem>
                     <asp:ListItem Value="BW">Botswana</asp:ListItem>
                     <asp:ListItem Value="BV">Bouvet Island</asp:ListItem>
                     <asp:ListItem Value="BR">Brazil</asp:ListItem>
                     <asp:ListItem Value="IO">British Indian Ocean Territory</asp:ListItem>
                     <asp:ListItem Value="BN">Brunei Darussalam</asp:ListItem>
                     <asp:ListItem Value="BG">Bulgaria</asp:ListItem>
                     <asp:ListItem Value="BF">Burkina Faso</asp:ListItem>
                     <asp:ListItem Value="BI">Burundi</asp:ListItem>
                     <asp:ListItem Value="KH">Cambodia</asp:ListItem>
                     <asp:ListItem Value="CM">Cameroon</asp:ListItem>
                     <asp:ListItem Value="CA">Canada</asp:ListItem>
                     <asp:ListItem Value="CV">Cape Verde</asp:ListItem>
                     <asp:ListItem Value="KY">Cayman Islands</asp:ListItem>
                     <asp:ListItem Value="CF">Central African Republic</asp:ListItem>
                     <asp:ListItem Value="TD">Chad</asp:ListItem>
                     <asp:ListItem Value="CL">Chile</asp:ListItem>
                     <asp:ListItem Value="CN">China</asp:ListItem>
                     <asp:ListItem Value="CX">Christmas Island</asp:ListItem>
                     <asp:ListItem Value="CC">Cocos (Keeling) Islands</asp:ListItem>
                     <asp:ListItem Value="CO">Colombia</asp:ListItem>
                     <asp:ListItem Value="KM">Comoros</asp:ListItem>
                     <asp:ListItem Value="CG">Congo</asp:ListItem>
                     <asp:ListItem Value="CK">Cook Islands</asp:ListItem>
                     <asp:ListItem Value="CR">Costa Rica</asp:ListItem>
                     <asp:ListItem Value="CI">Cote D'Ivoire</asp:ListItem>
                     <asp:ListItem Value="HR">Croatia (Local Name: Hrvatska)</asp:ListItem>
                     <asp:ListItem Value="CU">Cuba</asp:ListItem>
                     <asp:ListItem Value="CY">Cyprus</asp:ListItem>
                     <asp:ListItem Value="CZ">Czech Republic</asp:ListItem>
                     <asp:ListItem Value="DK">Denmark</asp:ListItem>
                     <asp:ListItem Value="DJ">Djibouti</asp:ListItem>
                     <asp:ListItem Value="DM">Dominica</asp:ListItem>
                     <asp:ListItem Value="DO">Dominican Republic</asp:ListItem>
                     <asp:ListItem Value="TP">East Timor</asp:ListItem>
                     <asp:ListItem Value="EC">Ecuador</asp:ListItem>
                     <asp:ListItem Value="EG">Egypt</asp:ListItem>
                     <asp:ListItem Value="SV">El Salvador</asp:ListItem>
                     <asp:ListItem Value="GQ">Equatorial Guinea</asp:ListItem>
                     <asp:ListItem Value="ER">Eritrea</asp:ListItem>
                     <asp:ListItem Value="EE">Estonia</asp:ListItem>
                     <asp:ListItem Value="ET">Ethiopia</asp:ListItem>
                     <asp:ListItem Value="FK">Falkland Islands (Malvinas)</asp:ListItem>
                     <asp:ListItem Value="FO">Faroe Islands</asp:ListItem>
                     <asp:ListItem Value="FJ">Fiji</asp:ListItem>
                     <asp:ListItem Value="FI">Finland</asp:ListItem>
                     <asp:ListItem Value="FR">France</asp:ListItem>
                     <asp:ListItem Value="GF">French Guiana</asp:ListItem>
                     <asp:ListItem Value="PF">French Polynesia</asp:ListItem>
                     <asp:ListItem Value="TF">French Southern Territories</asp:ListItem>
                     <asp:ListItem Value="GA">Gabon</asp:ListItem>
                     <asp:ListItem Value="GM">Gambia</asp:ListItem>
                     <asp:ListItem Value="GE">Georgia</asp:ListItem>
                     <asp:ListItem Value="DE">Germany</asp:ListItem>
                     <asp:ListItem Value="GH">Ghana</asp:ListItem>
                     <asp:ListItem Value="GI">Gibraltar</asp:ListItem>
                     <asp:ListItem Value="GR">Greece</asp:ListItem>
                     <asp:ListItem Value="GL">Greenland</asp:ListItem>
                     <asp:ListItem Value="GD">Grenada</asp:ListItem>
                     <asp:ListItem Value="GP">Guadeloupe</asp:ListItem>
                     <asp:ListItem Value="GU">Guam</asp:ListItem>
                     <asp:ListItem Value="GT">Guatemala</asp:ListItem>
                     <asp:ListItem Value="GN">Guinea</asp:ListItem>
                     <asp:ListItem Value="GW">Guinea-Bissau</asp:ListItem>
                     <asp:ListItem Value="GY">Guyana</asp:ListItem>
                     <asp:ListItem Value="HT">Haiti</asp:ListItem>
                     <asp:ListItem Value="HM">Heard And Mc Donald Islands</asp:ListItem>
                     <asp:ListItem Value="VA">Holy See (Vatican City State)</asp:ListItem>
                     <asp:ListItem Value="HN">Honduras</asp:ListItem>
                     <asp:ListItem Value="HK">Hong Kong</asp:ListItem>
                     <asp:ListItem Value="HU">Hungary</asp:ListItem>
                     <asp:ListItem Value="IS">Iceland</asp:ListItem>
                     <asp:ListItem Value="IN">India</asp:ListItem>
                     <asp:ListItem Value="ID">Indonesia</asp:ListItem>
                     <asp:ListItem Value="IR">Iran (Islamic Republic Of)</asp:ListItem>
                     <asp:ListItem Value="IQ">Iraq</asp:ListItem>
                     <asp:ListItem Value="IE">Ireland</asp:ListItem>
                     <asp:ListItem Value="IL">Israel</asp:ListItem>
                     <asp:ListItem Value="IT">Italy</asp:ListItem>
                     <asp:ListItem Value="JM">Jamaica</asp:ListItem>
                     <asp:ListItem Value="JP">Japan</asp:ListItem>
                     <asp:ListItem Value="JO">Jordan</asp:ListItem>
                     <asp:ListItem Value="KZ">Kazakhstan</asp:ListItem>
                     <asp:ListItem Value="KE">Kenya</asp:ListItem>
                     <asp:ListItem Value="KI">Kiribati</asp:ListItem>
                     <asp:ListItem Value="KP">Korea, Dem People'S Republic</asp:ListItem>
                     <asp:ListItem Value="KR">Korea, Republic Of</asp:ListItem>
                     <asp:ListItem Value="KW">Kuwait</asp:ListItem>
                     <asp:ListItem Value="KG">Kyrgyzstan</asp:ListItem>
                     <asp:ListItem Value="LA">Lao People'S Dem Republic</asp:ListItem>
                     <asp:ListItem Value="LV">Latvia</asp:ListItem>
                     <asp:ListItem Value="LB">Lebanon</asp:ListItem>
                     <asp:ListItem Value="LS">Lesotho</asp:ListItem>
                     <asp:ListItem Value="LR">Liberia</asp:ListItem>
                     <asp:ListItem Value="LY">Libyan Arab Jamahiriya</asp:ListItem>
                     <asp:ListItem Value="LI">Liechtenstein</asp:ListItem>
                     <asp:ListItem Value="LT">Lithuania</asp:ListItem>
                     <asp:ListItem Value="LU">Luxembourg</asp:ListItem>
                     <asp:ListItem Value="MO">Macau</asp:ListItem>
                     <asp:ListItem Value="MK">Macedonia</asp:ListItem>
                     <asp:ListItem Value="MG">Madagascar</asp:ListItem>
                     <asp:ListItem Value="MW">Malawi</asp:ListItem>
                     <asp:ListItem Value="MY">Malaysia</asp:ListItem>
                     <asp:ListItem Value="MV">Maldives</asp:ListItem>
                     <asp:ListItem Value="ML">Mali</asp:ListItem>
                     <asp:ListItem Value="MT">Malta</asp:ListItem>
                     <asp:ListItem Value="MH">Marshall Islands</asp:ListItem>
                     <asp:ListItem Value="MQ">Martinique</asp:ListItem>
                     <asp:ListItem Value="MR">Mauritania</asp:ListItem>
                     <asp:ListItem Value="MU">Mauritius</asp:ListItem>
                     <asp:ListItem Value="YT">Mayotte</asp:ListItem>
                     <asp:ListItem Value="MX">Mexico</asp:ListItem>
                     <asp:ListItem Value="FM">Micronesia, Federated States</asp:ListItem>
                     <asp:ListItem Value="MD">Moldova, Republic Of</asp:ListItem>
                     <asp:ListItem Value="MC">Monaco</asp:ListItem>
                     <asp:ListItem Value="MN">Mongolia</asp:ListItem>
                     <asp:ListItem Value="MS">Montserrat</asp:ListItem>
                     <asp:ListItem Value="MA">Morocco</asp:ListItem>
                     <asp:ListItem Value="MZ">Mozambique</asp:ListItem>
                     <asp:ListItem Value="MM">Myanmar</asp:ListItem>
                     <asp:ListItem Value="NA">Namibia</asp:ListItem>
                     <asp:ListItem Value="NR">Nauru</asp:ListItem>
                     <asp:ListItem Value="NP">Nepal</asp:ListItem>
                     <asp:ListItem Value="NL">Netherlands</asp:ListItem>
                     <asp:ListItem Value="AN">Netherlands Ant Illes</asp:ListItem>
                     <asp:ListItem Value="NC">New Caledonia</asp:ListItem>
                     <asp:ListItem Value="NZ">New Zealand</asp:ListItem>
                     <asp:ListItem Value="NI">Nicaragua</asp:ListItem>
                     <asp:ListItem Value="NE">Niger</asp:ListItem>
                     <asp:ListItem Value="NG">Nigeria</asp:ListItem>
                     <asp:ListItem Value="NU">Niue</asp:ListItem>
                     <asp:ListItem Value="NF">Norfolk Island</asp:ListItem>
                     <asp:ListItem Value="MP">Northern Mariana Islands</asp:ListItem>
                     <asp:ListItem Value="NO">Norway</asp:ListItem>
                     <asp:ListItem Value="OM">Oman</asp:ListItem>
                     <asp:ListItem Value="PK">Pakistan</asp:ListItem>
                     <asp:ListItem Value="PW">Palau</asp:ListItem>
                     <asp:ListItem Value="PA">Panama</asp:ListItem>
                     <asp:ListItem Value="PG">Papua New Guinea</asp:ListItem>
                     <asp:ListItem Value="PY">Paraguay</asp:ListItem>
                     <asp:ListItem Value="PE">Peru</asp:ListItem>
                     <asp:ListItem Value="PH">Philippines</asp:ListItem>
                     <asp:ListItem Value="PN">Pitcairn</asp:ListItem>
                     <asp:ListItem Value="PL">Poland</asp:ListItem>
                     <asp:ListItem Value="PT">Portugal</asp:ListItem>
                     <asp:ListItem Value="PR">Puerto Rico</asp:ListItem>
                     <asp:ListItem Value="QA">Qatar</asp:ListItem>
                     <asp:ListItem Value="RE">Reunion</asp:ListItem>
                     <asp:ListItem Value="RO">Romania</asp:ListItem>
                     <asp:ListItem Value="RU">Russian Federation</asp:ListItem>
                     <asp:ListItem Value="RW">Rwanda</asp:ListItem>
                     <asp:ListItem Value="KN">Saint K Itts And Nevis</asp:ListItem>
                     <asp:ListItem Value="LC">Saint Lucia</asp:ListItem>
                     <asp:ListItem Value="VC">Saint Vincent, The Grenadines</asp:ListItem>
                     <asp:ListItem Value="WS">Samoa</asp:ListItem>
                     <asp:ListItem Value="SM">San Marino</asp:ListItem>
                     <asp:ListItem Value="ST">Sao Tome And Principe</asp:ListItem>
                     <asp:ListItem Value="SA">Saudi Arabia</asp:ListItem>
                     <asp:ListItem Value="SN">Senegal</asp:ListItem>
                     <asp:ListItem Value="SC">Seychelles</asp:ListItem>
                     <asp:ListItem Value="SL">Sierra Leone</asp:ListItem>
                     <asp:ListItem Value="SG">Singapore</asp:ListItem>
                     <asp:ListItem Value="SK">Slovakia (Slovak Republic)</asp:ListItem>
                     <asp:ListItem Value="SI">Slovenia</asp:ListItem>
                     <asp:ListItem Value="SB">Solomon Islands</asp:ListItem>
                     <asp:ListItem Value="SO">Somalia</asp:ListItem>
                     <asp:ListItem Value="ZA">South Africa</asp:ListItem>
                     <asp:ListItem Value="GS">South Georgia , S Sandwich Is.</asp:ListItem>
                     <asp:ListItem Value="ES">Spain</asp:ListItem>
                     <asp:ListItem Value="LK">Sri Lanka</asp:ListItem>
                     <asp:ListItem Value="SH">St. Helena</asp:ListItem>
                     <asp:ListItem Value="PM">St. Pierre And Miquelon</asp:ListItem>
                     <asp:ListItem Value="SD">Sudan</asp:ListItem>
                     <asp:ListItem Value="SR">Suriname</asp:ListItem>
                     <asp:ListItem Value="SJ">Svalbard, Jan Mayen Islands</asp:ListItem>
                     <asp:ListItem Value="SZ">Sw Aziland</asp:ListItem>
                     <asp:ListItem Value="SE">Sweden</asp:ListItem>
                     <asp:ListItem Value="CH">Switzerland</asp:ListItem>
                     <asp:ListItem Value="SY">Syrian Arab Republic</asp:ListItem>
                     <asp:ListItem Value="TW">Taiwan</asp:ListItem>
                     <asp:ListItem Value="TJ">Tajikistan</asp:ListItem>
                     <asp:ListItem Value="TZ">Tanzania, United Republic Of</asp:ListItem>
                     <asp:ListItem Value="TH">Thailand</asp:ListItem>
                     <asp:ListItem Value="TG">Togo</asp:ListItem>
                     <asp:ListItem Value="TK">Tokelau</asp:ListItem>
                     <asp:ListItem Value="TO">Tonga</asp:ListItem>
                     <asp:ListItem Value="TT">Trinidad And Tobago</asp:ListItem>
                     <asp:ListItem Value="TN">Tunisia</asp:ListItem>
                     <asp:ListItem Value="TR">Turkey</asp:ListItem>
                     <asp:ListItem Value="TM">Turkmenistan</asp:ListItem>
                     <asp:ListItem Value="TC">Turks And Caicos Islands</asp:ListItem>
                     <asp:ListItem Value="TV">Tuvalu</asp:ListItem>
                     <asp:ListItem Value="UG">Uganda</asp:ListItem>
                     <asp:ListItem Value="UA">Ukraine</asp:ListItem>
                     <asp:ListItem Value="AE">United Arab Emirates</asp:ListItem>
                     <asp:ListItem Value="GB">United Kingdom</asp:ListItem>
                     <asp:ListItem Value="US" Selected="True">United States</asp:ListItem>
                     <asp:ListItem Value="UM">United States Minor Is.</asp:ListItem>
                     <asp:ListItem Value="UY">Uruguay</asp:ListItem>
                     <asp:ListItem Value="UZ">Uzbekistan</asp:ListItem>
                     <asp:ListItem Value="VU">Vanuatu</asp:ListItem>
                     <asp:ListItem Value="VE">Venezuela</asp:ListItem>
                     <asp:ListItem Value="VN">Viet Nam</asp:ListItem>
                     <asp:ListItem Value="VG">Virgin Islands (British)</asp:ListItem>
                     <asp:ListItem Value="VI">Virgin Islands (U.S.)</asp:ListItem>
                     <asp:ListItem Value="WF">Wallis And Futuna Islands</asp:ListItem>
                     <asp:ListItem Value="EH">Western Sahara</asp:ListItem>
                     <asp:ListItem Value="YE">Yemen</asp:ListItem>
                     <asp:ListItem Value="YU">Yugoslavia</asp:ListItem>
                     <asp:ListItem Value="ZR">Zaire</asp:ListItem>
                     <asp:ListItem Value="ZM">Zambia</asp:ListItem>
                     <asp:ListItem Value="ZW">Zimbabwe</asp:ListItem>
                </asp:DropDownList>                   
            </td>   
        </tr>
        <tr>
            <td><asp:Label ID="Label6" runat="server" Text="Company"></asp:Label></td>
            <td><asp:TextBox class="form-control" ID="txtToCompany" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td> <asp:Label ID="Label7" runat="server" Text="Address One"></asp:Label></td>
            <td><asp:TextBox class="form-control" ID="txtToAddressOne" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td><asp:Label ID="Label11" runat="server" Text="Address Two"></asp:Label></td>
            <td><asp:TextBox class="form-control" ID="txtToAddressTwo" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td><asp:Label ID="Label2" runat="server" Text="City"></asp:Label></td>
            <td><asp:TextBox class="form-control" ID="txtCity" runat="server"></asp:TextBox></td>
        </tr>
        <tr>      
            <td><asp:Label ID="Label8" runat="server" Text="State"></asp:Label></td>
            <td>
                <asp:DropDownList class="form-control" ID="ddlStates" runat="server">
	                <asp:ListItem Value="AL">Alabama</asp:ListItem>
	                <asp:ListItem Value="AK">Alaska</asp:ListItem>
	                <asp:ListItem Value="AZ">Arizona</asp:ListItem>
	                <asp:ListItem Value="AR">Arkansas</asp:ListItem>
	                <asp:ListItem Value="CA">California</asp:ListItem>
	                <asp:ListItem Value="CO">Colorado</asp:ListItem>
	                <asp:ListItem Value="CT">Connecticut</asp:ListItem>
	                <asp:ListItem Value="DC">District of Columbia</asp:ListItem>
	                <asp:ListItem Value="DE">Delaware</asp:ListItem>
	                <asp:ListItem Value="FL">Florida</asp:ListItem>
	                <asp:ListItem Value="GA">Georgia</asp:ListItem>
	                <asp:ListItem Value="HI">Hawaii</asp:ListItem>
	                <asp:ListItem Value="ID">Idaho</asp:ListItem>
	                <asp:ListItem Value="IL">Illinois</asp:ListItem>
	                <asp:ListItem Value="IN">Indiana</asp:ListItem>
	                <asp:ListItem Value="IA">Iowa</asp:ListItem>
	                <asp:ListItem Value="KS">Kansas</asp:ListItem>
	                <asp:ListItem Value="KY">Kentucky</asp:ListItem>
	                <asp:ListItem Value="LA">Louisiana</asp:ListItem>
	                <asp:ListItem Value="ME">Maine</asp:ListItem>
	                <asp:ListItem Value="MD">Maryland</asp:ListItem>
	                <asp:ListItem Value="MA">Massachusetts</asp:ListItem>
	                <asp:ListItem Value="MI">Michigan</asp:ListItem>
	                <asp:ListItem Value="MN">Minnesota</asp:ListItem>
	                <asp:ListItem Value="MS">Mississippi</asp:ListItem>
	                <asp:ListItem Value="MO">Missouri</asp:ListItem>
	                <asp:ListItem Value="MT">Montana</asp:ListItem>
	                <asp:ListItem Value="NE">Nebraska</asp:ListItem>
	                <asp:ListItem Value="NV">Nevada</asp:ListItem>
	                <asp:ListItem Value="NH">New Hampshire</asp:ListItem>
	                <asp:ListItem Value="NJ">New Jersey</asp:ListItem>
	                <asp:ListItem Value="NM">New Mexico</asp:ListItem>
	                <asp:ListItem Value="NY">New York</asp:ListItem>
	                <asp:ListItem Value="NC">North Carolina</asp:ListItem>
	                <asp:ListItem Value="ND">North Dakota</asp:ListItem>
	                <asp:ListItem Value="OH">Ohio</asp:ListItem>
	                <asp:ListItem Value="OK">Oklahoma</asp:ListItem>
	                <asp:ListItem Value="OR">Oregon</asp:ListItem>
	                <asp:ListItem Value="PA">Pennsylvania</asp:ListItem>
	                <asp:ListItem Value="RI">Rhode Island</asp:ListItem>
	                <asp:ListItem Value="SC">South Carolina</asp:ListItem>
	                <asp:ListItem Value="SD">South Dakota</asp:ListItem>
	                <asp:ListItem Value="TN">Tennessee</asp:ListItem>
	                <asp:ListItem Value="TX">Texas</asp:ListItem>
	                <asp:ListItem Value="UT">Utah</asp:ListItem>
	                <asp:ListItem Value="VT">Vermont</asp:ListItem>
	                <asp:ListItem Value="VA">Virginia</asp:ListItem>
	                <asp:ListItem Value="WA">Washington</asp:ListItem>
	                <asp:ListItem Value="WV">West Virginia</asp:ListItem>
	                <asp:ListItem Value="WI">Wisconsin</asp:ListItem>
	                <asp:ListItem Value="WY">Wyoming</asp:ListItem>
                </asp:DropDownList>     
            </td>
        </tr>
        <tr>
            <td><asp:Label ID="Label9" runat="server" Text="Postal Code"></asp:Label></td>
            <td><asp:TextBox class="form-control" ID="txtPostalCode" runat="server"></asp:TextBox></td>
        </tr>
	    <tr>
            <td><asp:Label ID="Label5" runat="server" Text="Phone (+ Ext.)"></asp:Label></td>
            <td><asp:TextBox class="form-control" ID="txtToPhone" runat="server"></asp:TextBox></td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td><asp:TextBox ID="txtID" runat="server" Visible="false"></asp:TextBox></td>
        </tr>
    </table>		
        </asp:panel>
    <asp:panel id="Panel5" runat="server" groupingtext="Boxes To Ship">
        <asp:Label ID="Label14" runat="server" Text="Note: If more than one box shipping to one address do you prefer separate AWB #’s or one AWB for all boxes?"></asp:Label>
        <asp:DropDownList class="form-control" ID="ddlNumberItemsShipped" runat="server" AutoPostBack="True" onselectedindexchanged="ddlNumberItemsShipped_SelectedIndexChanged">
            <asp:ListItem>1</asp:ListItem>
            <asp:ListItem>2</asp:ListItem>
            <asp:ListItem>3</asp:ListItem>
            <asp:ListItem>4</asp:ListItem>
            <asp:ListItem>5</asp:ListItem>
            <asp:ListItem>6</asp:ListItem>
        </asp:DropDownList>         
        <br><hr>
        <asp:Label ID="lblDescription" runat="server" Text="Items Being Shipped and "></asp:Label> 
        <asp:Label ID="lblQuantity" runat="server" Text="Quantity for each!"></asp:Label>
        <br><hr>
        <asp:TextBox class="form-control" ID="txtDescriptionOne" runat="server" Visible="False" placeholder="Item one desc"></asp:TextBox>
        <asp:TextBox class="form-control" ID="txtQuantityOne" runat="server" Visible="False" placeholder="Item one qty"></asp:TextBox>
        
        <asp:TextBox class="form-control" ID="txtDescriptionTwo" runat="server" Visible="False" placeholder="Item two desc"></asp:TextBox>
        <asp:TextBox class="form-control" ID="txtQuantityTwo" runat="server" Visible="False" placeholder="Item two qty"></asp:TextBox>  
        
        <asp:TextBox class="form-control" ID="txtDescriptionThree" runat="server" Visible="False" placeholder="Item three desc"></asp:TextBox>
        <asp:TextBox class="form-control" ID="txtQuantityThree" runat="server" Visible="False" placeholder="Item three qty"></asp:TextBox>
        
        <asp:TextBox class="form-control" ID="txtDescriptionFour" runat="server" Visible="False" placeholder="Item four desc" ></asp:TextBox>
        <asp:TextBox class="form-control" ID="txtQuantityFour" runat="server" Visible="False" placeholder="Item four qty"></asp:TextBox>
        
        <asp:TextBox class="form-control" ID="txtDescriptionFive" runat="server" Visible="False" placeholder="Item five desc"></asp:TextBox>
        <asp:TextBox class="form-control" ID="txtQuantityFive" runat="server" Visible="False" placeholder="Item five qty"></asp:TextBox>
        
        <asp:TextBox class="form-control" ID="txtDescriptionSix" runat="server" Visible="False" placeholder="Item six desc"></asp:TextBox>
        <asp:TextBox class="form-control" ID="txtQuantitySix" runat="server" Visible="False" placeholder="Item six qty"></asp:TextBox>
        </asp:panel>
        <asp:panel id="Panel10" runat="server" groupingtext="Delivered By">
            <table>
                <thead>
                    <tr>
                        <th>
                            <asp:Label ID="Label1" runat="server" Text="Date"></asp:Label>
                        </th>
                    </tr>
                </thead>
                    <tr>
                        <td>
                            <asp:Calendar ID="calShipByDate" runat="server" onselectionchanged="calShipByDate_SelectionChanged"></asp:Calendar>
                        </td>
                    </tr>
            </table>
            <br>
            <table class="table table-hover text-center">
                <thead>
                    <tr>
                        <th>
                            <asp:Label ID="Label4" runat="server" Text="Time"></asp:Label> 
                        </th>
                    </tr>
                </thead>
                    <tr>
                        <td>
                            <asp:DropDownList class="form-control" ID="ddlShipByTime" runat="server">
                                <asp:ListItem>12:00 AM</asp:ListItem>
                                <asp:ListItem>1:00 AM</asp:ListItem>
                                <asp:ListItem>2:00 AM</asp:ListItem>
                                <asp:ListItem>3:00 AM</asp:ListItem>
                                <asp:ListItem>4:00 AM</asp:ListItem>
                                <asp:ListItem>5:00 AM</asp:ListItem>
                                <asp:ListItem>6:00 AM</asp:ListItem>
                                <asp:ListItem>7:00 AM</asp:ListItem>
                                <asp:ListItem>8:00 AM</asp:ListItem>
                                <asp:ListItem>9:00 AM</asp:ListItem>
                                <asp:ListItem>10:00 AM</asp:ListItem>
                                <asp:ListItem>11:00 AM</asp:ListItem>
                                <asp:ListItem>12:00 PM</asp:ListItem>
                                <asp:ListItem>1:00 PM</asp:ListItem>
                                <asp:ListItem>2:00 PM</asp:ListItem>
                                <asp:ListItem>3:00 PM</asp:ListItem>
                                <asp:ListItem>4:00 PM</asp:ListItem>
                                <asp:ListItem>5:00 PM</asp:ListItem>
                                <asp:ListItem>6:00 PM</asp:ListItem>
                                <asp:ListItem>7:00 PM</asp:ListItem>
                                <asp:ListItem>8:00 PM</asp:ListItem>
                                <asp:ListItem>9:00 PM</asp:ListItem>
                                <asp:ListItem>10:00 PM</asp:ListItem>
                                <asp:ListItem>11:00 PM</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>     
            </table>
            <table class="table table-hover text-center">
                <thead>
                    <tr>
                        <th>
                            <asp:Label ID="Label_Urgency" runat="server" Text="Urgency"></asp:Label>
                            <h5>If urgency is flexible, cost of shipping may be considered before ship by date.</h5>
                        </th>
                    </tr>
                </thead>
                    <tr>
                        <td>
                            <asp:DropDownList class="form-control" ID="ddlUrgency" runat="server" OnSelectedIndexChanged="ddlUrgency_SelectedIndexChanged" AutoPostBack="true">
                                <asp:ListItem>Lowest cost</asp:ListItem>
                                <asp:ListItem>Flexible</asp:ListItem>
                                <asp:ListItem>Non-Flexible</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
            </table>
        </asp:panel>
         <asp:panel id="Panel1" runat="server" groupingtext="Comments">     
             <asp:TextBox class="form-control" ID="txtComments" runat="server" TextMode="MultiLine"></asp:TextBox>
        </asp:panel>
         <asp:panel id="Panel7" runat="server" groupingtext="Bill to 3rd Party Account No.">
        <asp:TextBox class="form-control" ID="txtBillTo3rdParty" runat="server"></asp:TextBox>
        </asp:panel>                   
         <asp:panel id="Panel14" runat="server" groupingtext="Signature Required">        
            <asp:CheckBox ID="cbSignatureRequired" runat="server" Text="Residential Recipient's Signature Required (Additional Charge)"/>
        </asp:panel>
    <div class="wrapper text-center">
        <div class="btn-group" role="group">
            <asp:Button class="btn btn-success" ID="btnSubmit" runat="server" onclick="btnSubmit_Click" Text="Submit" /> 
            <asp:Button class="btn btn-warning" ID="btnClear" runat="server" Text="Clear" onclick="btnClear_Click" />
        </div>
    </div>
    <br />    
    <!--</div>this is for the main div-->
    <div class="clear">
    </div>
    <!--</div>this is for the page div-->
    <div class="footer">    
    </div>
    </form>
    </div>
</body>
</html>
