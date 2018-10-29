<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UpdateInfo.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Update a Request</title>
    <link href="Content/bootstrap.css" rel="stylesheet" />
    <link rel="stylesheet" type="text/css" href="/DataTables/datatables.css" />
    <script src="packages/jQuery.3.3.1/Content/Scripts/jquery-3.3.1.js"></script>
    <script src="bootstrap-4.0.0/dist/js/bootstrap.js"></script>
    <script src="Scripts/lib/simple-mask-money.js"></script>
</head>
<body>
    <div class="container">
    <nav class="navbar navbar-dark bg-dark" style="padding: 0px;">
        <div class="container-fluid">
            <div class="navbar-header">
                <a class="navbar-brand" href="https://www.oai.aero/"><img src="images/omni_logo.png" alt="" style="padding: 0px; margin: 0px;"></a>
                <button class="btn btn-sm btn-primary" type="button"><a style="color:azure;" href="ShippingRequest.aspx">Make new Ship Request</a></button>
            </div>
        </div>
    </nav>
    <form class="form" id="form1" runat="server">
    <div class="header text-center">
        <h4 class="display-5">You selected ship-request ID# : <asp:Label ID="Label_ShipID" runat="server" Text=""></asp:Label> and maybe you can update information</h4>
    </div>
        <div class="row">
            <div class="col-4">
                <div class="list-group" id="list-tab" role="tablist">
                    <a class="list-group-item list-group-item-action active" id="list-home-list" data-toggle="list" href="#list-home" role="tab" aria-controls="home">Business Info</a>
                    <a class="list-group-item list-group-item-action" id="list-profile-list" data-toggle="list" href="#list-profile" role="tab" aria-controls="profile">Address Info</a>
                    <a class="list-group-item list-group-item-action" id="list-messages-list" data-toggle="list" href="#list-messages" role="tab" aria-controls="messages">Tracking and Cost Info</a>
                    <a class="list-group-item list-group-item-action" id="list-settings-list" data-toggle="list" href="#list-settings" role="tab" aria-controls="settings">Restricted Update</a>
                </div>
            </div>
            <div class="col-8">
                <div class="tab-content" id="nav-tabContent">
                    <div class="tab-pane fade show active" id="list-home" role="tabpanel" aria-labelledby="list-home-list">
                        <h5>This is info like purpose, logged user at who made the request, the date of request</h5>
                        <ul class="list-group">
                            <li class="list-group-item">Purpose: <asp:Label ID="Label_Purpose" runat="server" Text=""></asp:Label></li>
                            <li class="list-group-item">Sender: <asp:Label ID="Label_User" runat="server" Text=""></asp:Label></li>
                            <li class="list-group-item">Request Date: <asp:Label ID="Label_ReqDate" runat="server" Text=""></asp:Label></li>
                        </ul>
                    </div>
                    <div class="tab-pane fade" id="list-profile" role="tabpanel" aria-labelledby="list-profile-list">
                        <h5>This is the destination address information, the origin will always be OAI, Tulsa</h5>
                        <ul class="list-group">
                            <li class="list-group-item">Address: <asp:Label ID="Label_ToAdd" runat="server" Text=""></asp:Label></li>
                            <li class="list-group-item">City: <asp:Label ID="Label_ToCity" runat="server" Text=""></asp:Label></li>
                            <li class="list-group-item">Country: <asp:Label ID="Label_ToCountry" runat="server" Text=""></asp:Label></li>
                        </ul>
                    </div>
                    <div class="tab-pane fade" id="list-messages" role="tabpanel" aria-labelledby="list-messages-list">
                        <h5>This is the reference for tracking number and cost</h5>
                        <ul class="list-group">
                            <li class="list-group-item">Tracking: <asp:Label ID="Label_Tracking" runat="server" Text=""></asp:Label></li>
                            <li class="list-group-item">Cost: <asp:Label ID="Label_Cost" runat="server" Text=""></asp:Label></li>
                        </ul>
                    </div>
                    <div class="tab-pane fade" id="list-settings" role="tabpanel" aria-labelledby="list-settings-list">
                        <table class="table table-hover">
                            <tr>
                                <td><h5>Tracking Number:</h5></td>
                            </tr>
                            <tr>
                                <td><asp:TextBox class="form-control" ID="Input_TrackingNum" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td><h5>Cost in USD:</h5></td>
                            </tr>
                            <tr>
                                <td><asp:TextBox class="form-control" ID="Input_Cost" runat="server" inputmode="numeric" value="0.00" type="text"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td><asp:Button class="btn btn-sm btn-outline-secondary" ID="btnModify" runat="server" onclick="btnModify_Click" Text="Update"  /></td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </form>
    </div>
</body>
<script>
    let input = SimpleMaskMoney.setMask('#Input_Cost', {
        prefix: '',
        suffix: '',
        fixed: true,
        fractionDigits: 2,
        decimalSeparator: '.',
        thousandsSeparator: ',',
        emptyOrInvalid: () => {
            return this.SimpleMaskMoney.args.fixed
                ? `0${this.SimpleMaskMoney.args.decimalSeparator}00`
                : `_${this.SimpleMaskMoney.args.decimalSeparator}__`;
        }
    });
</script>
</html>
