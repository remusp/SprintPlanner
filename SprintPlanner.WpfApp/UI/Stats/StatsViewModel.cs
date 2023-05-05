using SprintPlanner.Core;
using SprintPlanner.Core.Logic;
using SprintPlanner.FrameworkWPF;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace SprintPlanner.WpfApp.UI.Stats
{
    internal class StatsViewModel : ViewModelBase, IStorageManipulator
    {
        public ObservableCollection<StatItem> Stats
        {
            get { return Get(() => Stats); }
            set { Set(() => Stats, value); }
        }
     
        public void Flush()
        {
            // Do nothing
        }

        public void Pull()
        {
            decimal thresold = Business.Data.Capacity.CapacityFactor;
            Stats = new ObservableCollection<StatItem>();
            AddDevStat(thresold);
            AddQaStat(thresold);
            AddMiscStat(thresold);
        }

        private void AddDevStat(decimal thresold)
        {
            var devCapacity = GetRoleCapacity(Role.Dev);
            var planned = GetPlannedWork(i => i.fields.issuetype.id == Settings.Default.ISSUETYPE_DEV);
            var devStat = new StatItem
            {
                StatName = Role.Dev.ToString(),
                PlannedCapacity = planned,
                FullCapacity = devCapacity,
                TresholdCapacity = devCapacity * thresold / 100
            };

            Stats.Add(devStat);
        }

        private void AddQaStat(decimal thresold)
        {
            var qaCapacity = GetRoleCapacity(Role.Qa);
            var planned = GetPlannedWork(i => i.fields.issuetype.id == Settings.Default.ISSUETYPE_QA);
            var qaStat = new StatItem
            {
                StatName = Role.Qa.ToString(),
                PlannedCapacity = planned,
                FullCapacity = qaCapacity,
                TresholdCapacity = qaCapacity * thresold / 100
            };

            Stats.Add(qaStat);
        }

        private void AddMiscStat(decimal thresold)
        {
            var miscCapacity = GetRoleCapacity(Role.Misc);
            var planned = GetPlannedWork(i =>
                i.fields.issuetype.id != Settings.Default.ISSUETYPE_DEV
                && i.fields.issuetype.id != Settings.Default.ISSUETYPE_QA);
            var miscStat = new StatItem
            {
                StatName = Role.Misc.ToString(),
                PlannedCapacity = planned,
                FullCapacity = miscCapacity,
                TresholdCapacity = miscCapacity * thresold / 100
            };

            Stats.Add(miscStat);
        }

        private decimal GetRoleCapacity(Role role)
        {
            var users = Business.Data.Capacity.Users.Where(u => u.Role == role);
            return users.Sum(u => u.Capacity);
        }

        private decimal GetPlannedWork(Func<Issue, bool> issueFilter)
        {
            if (Business.Data.Sprint.Issues == null) 
            {
                return 0;
            }

            return Business.Data.Sprint.Issues
                .Where(i => i.fields.status.id != Settings.Default.STATUS_DONE
                    && issueFilter(i))
                .Sum(i => i.fields.timetracking.remainingEstimateSeconds) / 3600m;
        }
    }
}
