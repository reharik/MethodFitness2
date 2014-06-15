<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Activity.aspx.cs" Inherits="MethodFitness.Web.Areas.Reporting.ReportViewer.DailyPayments1" %>
<%@ Register TagPrefix="rsweb" Namespace="Microsoft.Reporting.WebForms" Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
        <form id="form2" runat="server">
    <div>
    
        <rsweb:ReportViewer ID="ReportViewer1" 
            runat="server" 
            Font-Names="Verdana" 
            Font-Size="8pt" 
            WaitMessageFont-Names="Verdana" 
            WaitMessageFont-Size="14pt"
            Width="875"
            Height="875"
            SizeToReportContent="True">
            <LocalReport ReportPath="Areas\Reporting\RDLC\Activity.rdlc">
                <DataSources>
                    <rsweb:ReportDataSource DataSourceId="SqlDataSource1" Name="Activity" />
                </DataSources>
            </LocalReport>
        </rsweb:ReportViewer>
      <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
        ConnectionString='<%$ appSettings:MethodFitness.sql_server_connection_string %>'
        SelectCommand="Activity" SelectCommandType="StoredProcedure"> 
        <SelectParameters>
            <asp:QueryStringParameter Name="StartDate" DefaultValue="dbnull" QueryStringField="StartDate" Type="DateTime" />
            <asp:QueryStringParameter Name="EndDate" DefaultValue="dbnull" QueryStringField="EndDate" Type="DateTime" />
            <asp:QueryStringParameter Name="TrainerId" QueryStringField="TrainerId" Type="Int32" />
            <asp:QueryStringParameter Name="ClientId" QueryStringField="ClientId" Type="Int32" />
            <asp:QueryStringParameter Name="LocationId" QueryStringField="locationId" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    
    </div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
    </form>
</body>
</html>
