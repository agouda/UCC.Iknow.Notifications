using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using System;
using System.IO;
using System.Text;

namespace UCC.Iknow.Notifications
{
    public class NotificationTimerJob : SPJobDefinition
    {
        public NotificationTimerJob()
            : base()
        {
        }

        public NotificationTimerJob(string jobName, SPWebApplication webApplication)
            : base(jobName, webApplication, null, SPJobLockType.Job)
        {
            this.Title = "IKNOW Notifications";
        }

        public override void Execute(Guid targetInstanceId)
        {
            try
            {
                SPWebApplication webApp = this.Parent as SPWebApplication;
                if (webApp.Properties["IKNOWSiteURL"] == null || webApp.Properties["OutboundSMTP"] == null)
                {
                    SPLogger.LogError("Error - IKNOW Notifications (IKNOWSiteURL or OutboundSMTP property not found.)");
                }
                else
                {
                    // get site collection url
                    string siteURL = webApp.Properties["IKNOWSiteURL"].ToString();
                    string smtpHost = webApp.Properties["OutboundSMTP"].ToString();
                    using (SPSite site = webApp.Sites[siteURL])
                    {
                        using (SPWeb web = site.RootWeb)
                        {
                            SPList subscriptionsList = web.Lists.TryGetList("Subscriptions");
                            SPList eBookslist = web.Lists.TryGetList("eBooks");
                            SPList bookslist = web.Lists.TryGetList("Books");
                            SPList magazineslist = web.Lists.TryGetList("Magazines");
                            SPList presentationslist = web.Lists.TryGetList("Presentations");
                            SPList researcheslist = web.Lists.TryGetList("Researches");
                            SPList videosList = web.Lists.TryGetList("Videos List");
                            SPList newsList = web.Lists.TryGetList("News List");
                            SPList insightsList = web.Lists.TryGetList("Insights List");
                            SPList eventsList = web.Lists.TryGetList("Events List");

                            if (subscriptionsList != null)
                            {
                                var subscribers = subscriptionsList.GetItems();
                                foreach (SPListItem subscriber in subscribers)
                                {
                                    // get the current visitor                                            
                                    SPListItem visitorItem = subscriptionsList.GetItemById(int.Parse(subscriber["ID"].ToString()));
                                    SPFieldUserValue userValue = new SPFieldUserValue(web, visitorItem[SPBuiltInFieldId.Author].ToString());
                                    SPUser user = userValue.User;
                                    string email = user.Email;
                                    if (visitorItem != null)
                                    {
                                        try
                                        {
                                            string html = File.ReadAllText(SPUtility.GetVersionedGenericSetupPath("/TEMPLATE/LAYOUTS/UCC.Iknow.Notifications/Templates/Notification.html", 15));
                                            // send notification to author
                                            if (!string.IsNullOrEmpty(html))
                                            {
                                                html = html.Replace("<%VisitorName%>", visitorItem["Title"].ToString());
                                                StringBuilder htmlContent = new StringBuilder();
                                                htmlContent.Append("<ul>");
                                                #region eBooks                                    
                                                // get eBooks subscribed categories
                                                StringBuilder eBooksSelectedCategories = new StringBuilder();
                                                SPFieldLookupValueCollection eBooksValues = new SPFieldLookupValueCollection(visitorItem["eBooksCategories"].ToString());
                                                foreach (SPFieldLookupValue value in eBooksValues)
                                                {
                                                    eBooksSelectedCategories.Append(string.Format("<Value Type='LookupMulti'>{0}</Value>", value.LookupValue));
                                                }

                                                // get list of ebook added last week 
                                                if (eBookslist != null)
                                                {
                                                    SPQuery eBooksQuery = new SPQuery();
                                                    eBooksQuery.Query = string.Format("<Where><And><Geq><FieldRef Name='eBookDate' /><Value Type='DateTime'><Today OffsetDays='-7' /></Value></Geq><In><FieldRef Name='Category' LookupId='True' /><Values>{0}</Values></In></And></Where>", eBooksSelectedCategories);
                                                    eBooksQuery.ViewFields = "<FieldRef Name='ID' /><FieldRef Name='Title' /><FieldRef Name='eBookDate' /><FieldRef Name='Category' />";
                                                    eBooksQuery.ViewFieldsOnly = true;
                                                    var eBooks = eBookslist.GetItems(eBooksQuery);
                                                    if (eBooks.Count > 0)
                                                    {
                                                        htmlContent.Append("<li>eBooks<ul>");
                                                        foreach (SPListItem eBook in eBooks)
                                                        {
                                                            htmlContent.Append(String.Format("<li><a href=\"{0}/pages/ebookdetails.aspx?itemId={1}\" title=\"{2}\">{2}</a></li>", web.Site.Url, eBook["ID"].ToString(), eBook["Title"].ToString()));
                                                        }
                                                        htmlContent.Append("</ul></li>");
                                                    }
                                                }
                                                #endregion
                                                #region Books Summary
                                                // get books subscribed categories
                                                StringBuilder booksSelectedCategories = new StringBuilder();
                                                SPFieldLookupValueCollection booksValues = new SPFieldLookupValueCollection(visitorItem["BooksCategories"].ToString());
                                                foreach (SPFieldLookupValue value in booksValues)
                                                {
                                                    booksSelectedCategories.Append(string.Format("<Value Type='LookupMulti'>{0}</Value>", value.LookupValue));
                                                }

                                                // get list of book summary added last week 
                                                if (bookslist != null)
                                                {
                                                    SPQuery booksQuery = new SPQuery();
                                                    booksQuery.Query = string.Format("<Where><And><Geq><FieldRef Name='BookDate' /><Value Type='DateTime'><Today OffsetDays='-7' /></Value></Geq><In><FieldRef Name='Category' LookupId='True' /><Values>{0}</Values></In></And></Where>", booksSelectedCategories);
                                                    booksQuery.ViewFields = "<FieldRef Name='ID' /><FieldRef Name='Title' /><FieldRef Name='BookDate' /><FieldRef Name='Category' />";
                                                    booksQuery.ViewFieldsOnly = true;
                                                    var books = bookslist.GetItems(booksQuery);
                                                    if (books.Count > 0)
                                                    {
                                                        htmlContent.Append("<li>Books Summaries<ul>");
                                                        foreach (SPListItem book in books)
                                                        {
                                                            htmlContent.Append(String.Format("<li><a href=\"{0}/pages/booksummary.aspx?itemId={1}\" title=\"{2}\">{2}</a></li>", web.Site.Url, book["ID"].ToString(), book["Title"].ToString()));
                                                        }
                                                        htmlContent.Append("</ul></li>");
                                                    }
                                                }
                                                #endregion
                                                #region Magazines
                                                // get magazines subscribed categories
                                                StringBuilder magazinesSelectedCategories = new StringBuilder();
                                                SPFieldLookupValueCollection magazinesValues = new SPFieldLookupValueCollection(visitorItem["MagazinesCategories"].ToString());
                                                foreach (SPFieldLookupValue value in magazinesValues)
                                                {
                                                    magazinesSelectedCategories.Append(string.Format("<Value Type='LookupMulti'>{0}</Value>", value.LookupValue));
                                                }

                                                // get list of magazines added last week 
                                                if (magazineslist != null)
                                                {
                                                    SPQuery magazinesQuery = new SPQuery();
                                                    magazinesQuery.Query = string.Format("<Where><And><Geq><FieldRef Name='MagazineDate' /><Value Type='DateTime'><Today OffsetDays='-7' /></Value></Geq><In><FieldRef Name='Category' LookupId='True' /><Values>{0}</Values></In></And></Where>", magazinesSelectedCategories);
                                                    magazinesQuery.ViewFields = "<FieldRef Name='ID' /><FieldRef Name='Title' /><FieldRef Name='MagazineDate' /><FieldRef Name='Category' />";
                                                    magazinesQuery.ViewFieldsOnly = true;
                                                    var magazines = magazineslist.GetItems(magazinesQuery);
                                                    if (magazines.Count > 0)
                                                    {
                                                        htmlContent.Append("<li>Magazines<ul>");
                                                        foreach (SPListItem magazine in magazines)
                                                        {
                                                            htmlContent.Append(String.Format("<li><a href=\"{0}/pages/magazinedetails.aspx?itemId={1}\" title=\"{2}\">{2}</a></li>", web.Site.Url, magazine["ID"].ToString(), magazine["Title"].ToString()));
                                                        }
                                                        htmlContent.Append("</ul></li>");
                                                    }
                                                }
                                                #endregion
                                                #region Presentations
                                                // get presentations subscribed categories
                                                StringBuilder presentationsSelectedCategories = new StringBuilder();
                                                SPFieldLookupValueCollection presentationsValues = new SPFieldLookupValueCollection(visitorItem["PresentationsCategories"].ToString());
                                                foreach (SPFieldLookupValue value in presentationsValues)
                                                {
                                                    presentationsSelectedCategories.Append(string.Format("<Value Type='LookupMulti'>{0}</Value>", value.LookupValue));
                                                }

                                                // get list of presentations added last week 
                                                if (presentationslist != null)
                                                {
                                                    SPQuery presentationsQuery = new SPQuery();
                                                    presentationsQuery.Query = string.Format("<Where><And><Geq><FieldRef Name='PresentationDate' /><Value Type='DateTime'><Today OffsetDays='-7' /></Value></Geq><In><FieldRef Name='Category' LookupId='True' /><Values>{0}</Values></In></And></Where>", presentationsSelectedCategories);
                                                    presentationsQuery.ViewFields = "<FieldRef Name='ID' /><FieldRef Name='Title' /><FieldRef Name='PresentationDate' /><FieldRef Name='Category' />";
                                                    presentationsQuery.ViewFieldsOnly = true;
                                                    var presentations = presentationslist.GetItems(presentationsQuery);
                                                    if (presentations.Count > 0)
                                                    {
                                                        htmlContent.Append("<li>Presentations<ul>");
                                                        foreach (SPListItem presentation in presentations)
                                                        {
                                                            htmlContent.Append(String.Format("<li><a href=\"{0}/pages/presentationdetails.aspx?itemId={1}\" title=\"{2}\">{2}</a></li>", web.Site.Url, presentation["ID"].ToString(), presentation["Title"].ToString()));
                                                        }
                                                        htmlContent.Append("</ul></li>");
                                                    }
                                                }
                                                #endregion
                                                #region Researches
                                                // get researches subscribed categories
                                                StringBuilder researchesSelectedCategories = new StringBuilder();
                                                SPFieldLookupValueCollection researchesValues = new SPFieldLookupValueCollection(visitorItem["ResearchesCategories"].ToString());
                                                foreach (SPFieldLookupValue value in researchesValues)
                                                {
                                                    researchesSelectedCategories.Append(string.Format("<Value Type='LookupMulti'>{0}</Value>", value.LookupValue));
                                                }

                                                // get list of researches added last week 
                                                if (researcheslist != null)
                                                {
                                                    SPQuery researchesQuery = new SPQuery();
                                                    researchesQuery.Query = string.Format("<Where><And><Geq><FieldRef Name='ResearchDate' /><Value Type='DateTime'><Today OffsetDays='-7' /></Value></Geq><In><FieldRef Name='Category' LookupId='True' /><Values>{0}</Values></In></And></Where>", researchesSelectedCategories);
                                                    researchesQuery.ViewFields = "<FieldRef Name='ID' /><FieldRef Name='Title' /><FieldRef Name='ResearchDate' /><FieldRef Name='Category' />";
                                                    researchesQuery.ViewFieldsOnly = true;
                                                    var researches = researcheslist.GetItems(researchesQuery);
                                                    if (researches.Count > 0)
                                                    {
                                                        htmlContent.Append("<li>Researches<ul>");
                                                        foreach (SPListItem research in researches)
                                                        {
                                                            htmlContent.Append(String.Format("<li><a href=\"{0}/pages/researchdetails.aspx?itemId={1}\" title=\"{2}\">{2}</a></li>", web.Site.Url, research["ID"].ToString(), research["Title"].ToString()));
                                                        }
                                                        htmlContent.Append("</ul></li>");
                                                    }
                                                }
                                                #endregion
                                                #region Videos
                                                // get videos subscribed categories
                                                StringBuilder videosSelectedCategories = new StringBuilder();
                                                SPFieldLookupValueCollection videosValues = new SPFieldLookupValueCollection(visitorItem["VideosCategories"].ToString());
                                                foreach (SPFieldLookupValue value in videosValues)
                                                {
                                                    videosSelectedCategories.Append(string.Format("<Value Type='LookupMulti'>{0}</Value>", value.LookupValue));
                                                }

                                                // get list of videos added last week 
                                                if (videosList != null)
                                                {
                                                    SPQuery videosQuery = new SPQuery();
                                                    videosQuery.Query = string.Format("<Where><And><Geq><FieldRef Name='VideoDate' /><Value Type='DateTime'><Today OffsetDays='-7' /></Value></Geq><In><FieldRef Name='Category' LookupId='True' /><Values>{0}</Values></In></And></Where>", videosSelectedCategories);
                                                    videosQuery.ViewFields = "<FieldRef Name='ID' /><FieldRef Name='Title' /><FieldRef Name='VideoDate' /><FieldRef Name='Category' /><FieldRef Name='EncodedAbsUrl' />";
                                                    videosQuery.ViewFieldsOnly = true;
                                                    var videos = videosList.GetItems(videosQuery);
                                                    if (videos.Count > 0)
                                                    {
                                                        htmlContent.Append("<li>Videos<ul>");
                                                        foreach (SPListItem video in videos)
                                                        {
                                                            htmlContent.Append(String.Format("<li><a href=\"{0}\" title=\"{1}\">{1}</a></li>", video["EncodedAbsUrl"].ToString(), video["Title"].ToString()));
                                                        }
                                                        htmlContent.Append("</ul></li>");
                                                    }
                                                }
                                                #endregion
                                                #region News
                                                // get news subscribed categories
                                                StringBuilder newsSelectedCategories = new StringBuilder();
                                                SPFieldLookupValueCollection newsValues = new SPFieldLookupValueCollection(visitorItem["NewsCategories"].ToString());
                                                foreach (SPFieldLookupValue value in newsValues)
                                                {
                                                    newsSelectedCategories.Append(string.Format("<Value Type='LookupMulti'>{0}</Value>", value.LookupValue));
                                                }

                                                // get list of news added last week 
                                                if (newsList != null)
                                                {
                                                    SPQuery newsQuery = new SPQuery();
                                                    newsQuery.Query = string.Format("<Where><And><Geq><FieldRef Name='ArticleStartDate' /><Value Type='DateTime'><Today OffsetDays='-7' /></Value></Geq><In><FieldRef Name='news_category' LookupId='True' /><Values>{0}</Values></In></And></Where>", newsSelectedCategories);
                                                    newsQuery.ViewFields = "<FieldRef Name='ID' /><FieldRef Name='Title' /><FieldRef Name='ArticleStartDate' /><FieldRef Name='news_category' />";
                                                    newsQuery.ViewFieldsOnly = true;
                                                    var news = newsList.GetItems(newsQuery);
                                                    if (news.Count > 0)
                                                    {
                                                        htmlContent.Append("<li>News<ul>");
                                                        foreach (SPListItem newsItem in news)
                                                        {
                                                            htmlContent.Append(String.Format("<li><a href=\"{0}/pages/newsdetails.aspx?itemId={1}\" title=\"{2}\">{2}</a></li>", web.Site.Url, newsItem["ID"].ToString(), newsItem["Title"].ToString()));
                                                        }
                                                        htmlContent.Append("</ul></li>");
                                                    }
                                                }
                                                #endregion
                                                #region Insights
                                                // get insights subscribed categories
                                                StringBuilder insightsSelectedCategories = new StringBuilder();
                                                SPFieldLookupValueCollection insightsValues = new SPFieldLookupValueCollection(visitorItem["InsightsCategories"].ToString());
                                                foreach (SPFieldLookupValue value in insightsValues)
                                                {
                                                    insightsSelectedCategories.Append(string.Format("<Value Type='LookupMulti'>{0}</Value>", value.LookupValue));
                                                }

                                                // get list of insights added last week 
                                                if (insightsList != null)
                                                {
                                                    SPQuery insightsQuery = new SPQuery();
                                                    insightsQuery.Query = string.Format("<Where><And><Geq><FieldRef Name='ArticleStartDate' /><Value Type='DateTime'><Today OffsetDays='-7' /></Value></Geq><In><FieldRef Name='Category' LookupId='True' /><Values>{0}</Values></In></And></Where>", insightsSelectedCategories);
                                                    insightsQuery.ViewFields = "<FieldRef Name='ID' /><FieldRef Name='Title' /><FieldRef Name='ArticleStartDate' /><FieldRef Name='Category' />";
                                                    insightsQuery.ViewFieldsOnly = true;
                                                    var insights = insightsList.GetItems(insightsQuery);
                                                    if (insights.Count > 0)
                                                    {
                                                        htmlContent.Append("<li>Insights<ul>");
                                                        foreach (SPListItem insight in insights)
                                                        {
                                                            htmlContent.Append(String.Format("<li><a href=\"{0}/pages/insightdetails.aspx?itemId={1}\" title=\"{2}\">{2}</a></li>", web.Site.Url, insight["ID"].ToString(), insight["Title"].ToString()));
                                                        }
                                                        htmlContent.Append("</ul></li>");
                                                    }
                                                }
                                                #endregion
                                                #region Events
                                                // get events subscribed categories
                                                StringBuilder eventsSelectedCategories = new StringBuilder();
                                                SPFieldLookupValueCollection eventsValues = new SPFieldLookupValueCollection(visitorItem["EventsCategories"].ToString());
                                                foreach (SPFieldLookupValue value in eventsValues)
                                                {
                                                    eventsSelectedCategories.Append(string.Format("<Value Type='LookupMulti'>{0}</Value>", value.LookupValue));
                                                }

                                                // get list of events added last week 
                                                if (eventsList != null)
                                                {
                                                    SPQuery eventsQuery = new SPQuery();
                                                    eventsQuery.Query = string.Format("<Where><And><Geq><FieldRef Name='EventDate' /><Value Type='DateTime'><Today OffsetDays='-7' /></Value></Geq><In><FieldRef Name='event_category' LookupId='True' /><Values>{0}</Values></In></And></Where>", eventsSelectedCategories);
                                                    eventsQuery.ViewFields = "<FieldRef Name='ID' /><FieldRef Name='Title' /><FieldRef Name='EventDate' /><FieldRef Name='event_category' />";
                                                    eventsQuery.ViewFieldsOnly = true;
                                                    var events = eventsList.GetItems(eventsQuery);
                                                    if (events.Count > 0)
                                                    {
                                                        htmlContent.Append("<li>Events<ul>");
                                                        foreach (SPListItem eventItem in events)
                                                        {
                                                            htmlContent.Append(String.Format("<li>{0}</li>", eventItem["Title"].ToString()));
                                                        }
                                                        htmlContent.Append("</ul></li>");
                                                    }
                                                }
                                                #endregion
                                                htmlContent.Append("</ul>");
                                                html = html.Replace("<%HTMLContent%>", htmlContent.ToString());
                                                System.Net.Mail.AlternateView htmlView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(html, null, "text/html");
                                                // send email to visitor
                                                Utilities.SendEmail(smtpHost, "Iknow <no-reply@stc.com.sa>", email, "Iknow", htmlView);
                                                SPLogger.LogError(string.Format("Info - IKNOW Notifications - Email notification send to {0} <{1}>", visitorItem["Title"].ToString(), email));
                                            }
                                            else
                                            {
                                                SPLogger.LogError("Error - IKNOW Notifications (Email template not found.)");
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            SPLogger.LogError(ex.ToString());
                                            continue;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                SPLogger.LogError("Error - IKNOW Notifications (Subscriptions list not found.)");
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { SPLogger.LogError(ex.ToString()); }
        }

    }
}