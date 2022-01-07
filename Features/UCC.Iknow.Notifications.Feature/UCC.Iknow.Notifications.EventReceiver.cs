using System;
using System.Runtime.InteropServices;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace UCC.Iknow.Notifications
{
    [Guid("c789abab-a635-4bb3-b66e-2e2b6f601bdb")]
    public class UCCIknowNotificationsEventReceiver : SPFeatureReceiver
    {
        #region Private Members
        private const string JobName = "IKNOW Notifications";
        #endregion
        #region Private Methods
        private bool CreateJob(SPWebApplication webApp)
        {
            bool jobCreated = false;
            try
            {
                NotificationTimerJob job = new NotificationTimerJob(JobName, webApp);

                // Every Sunday job will running start from 1:00 AM to 2:00 AM
                SPWeeklySchedule schedule = new SPWeeklySchedule();
                schedule.BeginDayOfWeek = DayOfWeek.Sunday;
                schedule.BeginHour = 1;
                schedule.BeginMinute = 0;
                schedule.BeginSecond = 0;
                schedule.EndSecond = 5;
                schedule.EndMinute = 0;
                schedule.EndHour = 2;
                schedule.EndDayOfWeek = DayOfWeek.Sunday;

                job.Schedule = schedule;
                job.Update();
            }
            catch (Exception)
            {
                return jobCreated;
            }
            return jobCreated;
        }
        private bool DeleteExistingJob(string jobName, SPWebApplication site)
        {
            bool jobDeleted = false;
            try
            {
                foreach (SPJobDefinition job in site.JobDefinitions)
                {
                    if (job.Name == jobName)
                    {
                        job.Delete();
                        jobDeleted = true;
                    }
                }
            }
            catch (Exception)
            {
                return jobDeleted;
            }
            return jobDeleted;
        }
        #endregion
        #region Overrides
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    SPWebApplication parentwebApp = (SPWebApplication)properties.Feature.Parent;
                    SPSite site = properties.Feature.Parent as SPSite;
                    DeleteExistingJob(JobName, parentwebApp);
                    CreateJob(parentwebApp);
                });
            }
            catch (Exception ex)
            {
                SPLogger.LogError(ex.ToString());
            }
        }
        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            try
            {
                SPWebApplication parentwebApp = (SPWebApplication)properties.Feature.Parent;
                DeleteExistingJob(JobName, parentwebApp);
            }
            catch (Exception ex)
            {
                SPLogger.LogError(ex.ToString());
            }
        }
        #endregion
    }
}
