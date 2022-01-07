using Microsoft.SharePoint.Administration;
using System.Collections.Generic;

namespace UCC.Iknow.Notifications
{
    public class SPLogger : SPDiagnosticsServiceBase
    {
        public static string STCDiagnosticAreaName = "STC";
        private static SPLogger _Current;

        public static SPLogger Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = new SPLogger();
                }

                return _Current;
            }
        }

        private SPLogger()
            : base("STC Logging Service", SPFarm.Local)
        {

        }

        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
        {
            List<SPDiagnosticsArea> areas = new List<SPDiagnosticsArea>
            {
                new SPDiagnosticsArea(STCDiagnosticAreaName, new List<SPDiagnosticsCategory>
                {
                    new SPDiagnosticsCategory("IKNOW", TraceSeverity.Unexpected, EventSeverity.Error)
                }),                
            };

            return areas;
        }
        
        public static void LogError(string errorMessage)
        {
            SPDiagnosticsCategory category = SPLogger.Current.Areas[STCDiagnosticAreaName].Categories["IKNOW"];
            Current.WriteTrace(0, category, TraceSeverity.Unexpected, errorMessage);
        }
    }
}
