using System;
using System.IO;
using System.Reflection;

namespace SprintPlanner.Core
{
    public static class PathsHelper
    {
        public static string GetAppDataFolder()
        {
            string appName = GetAppName();
            var dataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appName);
            Directory.CreateDirectory(dataFolder);
            return dataFolder;
        }

        public static string GetPlansFolder()
        {
            string appName = GetAppName();
            string plansFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), appName, "SprintPlans");
            Directory.CreateDirectory(plansFolder);
            return plansFolder;
        }

        public static string GetTeamsFolder()
        {
            string appName = GetAppName();
            string teamsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), appName, "Teams");
            Directory.CreateDirectory(teamsFolder);
            return teamsFolder;
        }

        private static string GetAppName()
        {
            var assembly = Assembly.GetEntryAssembly();
            var appName = ((AssemblyTitleAttribute)assembly.GetCustomAttribute(typeof(AssemblyTitleAttribute))).Title;
            return appName;
        }
    }
}
