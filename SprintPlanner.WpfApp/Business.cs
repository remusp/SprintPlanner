using SprintPlanner.CoreFramework;
using SprintPlanner.WpfApp.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SprintPlanner.WpfApp
{
    public static class Business
    {
        static Business()
        {
            Jira = new JiraHelper() { Url = Settings.Default.Server};
        }

        public static JiraHelper Jira { get; set; }
    }
}
