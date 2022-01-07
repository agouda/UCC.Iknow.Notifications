using Microsoft.SharePoint;
using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace UCC.Iknow.Notifications.TEMP.VisualNotifications
{
    public partial class VisualNotificationsUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                using (SPSite spsite = new SPSite(SPContext.Current.Site.Url))
                {
                    using (SPWeb web = spsite.OpenWeb())
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
                                    subscriberName.Text = visitorItem["Title"].ToString();

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

                                        StringBuilder eBooksString = new StringBuilder();
                                        foreach (SPListItem eBook in eBooks)
                                        {
                                            eBooksString.Append(String.Format("<li><a href=\"{0}/pages/ebookdetails.aspx?itemId={1}\" title=\"{2}\">{2}</a></li>", web.Site.Url, eBook["ID"].ToString(), eBook["Title"].ToString()));
                                        }

                                        litEBooks.Text = eBooksString.ToString();
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

                                        StringBuilder booksString = new StringBuilder();
                                        foreach (SPListItem book in books)
                                        {
                                            booksString.Append(String.Format("<li><a href=\"{0}/pages/booksummary.aspx?itemId={1}\" title=\"{2}\">{2}</a></li>", web.Site.Url, book["ID"].ToString(), book["Title"].ToString()));
                                        }

                                        litBooks.Text = booksString.ToString();
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

                                        StringBuilder magazinesString = new StringBuilder();
                                        foreach (SPListItem magazine in magazines)
                                        {
                                            magazinesString.Append(String.Format("<li><a href=\"{0}/pages/magazinedetails.aspx?itemId={1}\" title=\"{2}\">{2}</a></li>", web.Site.Url, magazine["ID"].ToString(), magazine["Title"].ToString()));
                                        }

                                        litMagazines.Text = magazinesString.ToString();
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

                                        StringBuilder presentationsString = new StringBuilder();
                                        foreach (SPListItem presentation in presentations)
                                        {
                                            presentationsString.Append(String.Format("<li><a href=\"{0}/pages/presentationdetails.aspx?itemId={1}\" title=\"{2}\">{2}</a></li>", web.Site.Url, presentation["ID"].ToString(), presentation["Title"].ToString()));
                                        }

                                        litPresentations.Text = presentationsString.ToString();
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

                                        StringBuilder researchesString = new StringBuilder();
                                        foreach (SPListItem research in researches)
                                        {
                                            researchesString.Append(String.Format("<li><a href=\"{0}/pages/researchdetails.aspx?itemId={1}\" title=\"{2}\">{2}</a></li>", web.Site.Url, research["ID"].ToString(), research["Title"].ToString()));
                                        }

                                        litResearches.Text = researchesString.ToString();
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

                                        StringBuilder videosString = new StringBuilder();
                                        foreach (SPListItem video in videos)
                                        {
                                            videosString.Append(String.Format("<li><a href=\"{0}\" title=\"{1}\">{1}</a></li>", video["EncodedAbsUrl"].ToString(), video["Title"].ToString()));
                                        }

                                        litVideos.Text = videosString.ToString();
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

                                        StringBuilder newsString = new StringBuilder();
                                        foreach (SPListItem newsItem in news)
                                        {
                                            newsString.Append(String.Format("<li><a href=\"{0}/pages/newsdetails.aspx?itemId={1}\" title=\"{2}\">{2}</a></li>", web.Site.Url, newsItem["ID"].ToString(), newsItem["Title"].ToString()));
                                        }

                                        litNews.Text = newsString.ToString();
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

                                        StringBuilder insightsString = new StringBuilder();
                                        foreach (SPListItem insight in insights)
                                        {
                                            insightsString.Append(String.Format("<li><a href=\"{0}/pages/insightdetails.aspx?itemId={1}\" title=\"{2}\">{2}</a></li>", web.Site.Url, insight["ID"].ToString(), insight["Title"].ToString()));
                                        }

                                        litInsights.Text = insightsString.ToString();
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

                                        StringBuilder eventsString = new StringBuilder();
                                        foreach (SPListItem eventItem in events)
                                        {
                                            eventsString.Append(String.Format("<li>{0}</li>", eventItem["Title"].ToString()));
                                        }

                                        litEvents.Text = eventsString.ToString();
                                    }
                                    #endregion
                                }

                                break;
                            }
                        }
                        else
                        {
                            SPLogger.LogError("Error - IKNOW Notifications (Subscriptions list not found.)");
                        }
                    }
                }
            }
            catch (Exception ex) { SPLogger.LogError(ex.ToString()); }
        }
    }
}
