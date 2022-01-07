<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VisualNotificationsUserControl.ascx.cs" Inherits="UCC.Iknow.Notifications.TEMP.VisualNotifications.VisualNotificationsUserControl" %>

<asp:Literal runat="server" ID="subscriberName"></asp:Literal>
<br />
<ul>
    <li>Books   
        <ul>
            <asp:Literal runat="server" ID="litBooks"></asp:Literal>
        </ul>
    </li>
    <li>eBooks
         <ul>
             <asp:Literal runat="server" ID="litEBooks"></asp:Literal>
         </ul>
    </li>
    <li>Magazines
        <ul>
            <asp:Literal runat="server" ID="litMagazines"></asp:Literal>
        </ul>
    </li>
    <li>Presentations
        <ul>
            <asp:Literal runat="server" ID="litPresentations"></asp:Literal>
        </ul>
    </li>
    <li>Researches
        <ul>
            <asp:Literal runat="server" ID="litResearches"></asp:Literal>
        </ul>
    </li>
    <li>Videos
        <ul>
            <asp:Literal runat="server" ID="litVideos"></asp:Literal>
        </ul>
    </li>
    <li>News
        <ul>
            <asp:Literal runat="server" ID="litNews"></asp:Literal>
        </ul>
    </li>
    <li>Insights
        <ul>
            <asp:Literal runat="server" ID="litInsights"></asp:Literal>
        </ul>
    </li>
    <li>Events
        <ul>
            <asp:Literal runat="server" ID="litEvents"></asp:Literal>
        </ul>
    </li>
</ul>
