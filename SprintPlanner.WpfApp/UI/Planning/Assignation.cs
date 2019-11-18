using System;
using System.Windows.Input;

namespace SprintPlanner.WpfApp.UI.Planning
{
    public class Assignation : ICloneable
    {
        public string UidTarget { get; set; }

        public string UidSource { get; set; }

        public string Name { get; set; }

        public ICommand AssignCommand { get; set; }

        public string IssueKey { get; set; }

        public object Clone()
        {
            return new Assignation
            {
                UidTarget = this.UidTarget,
                UidSource = this.UidSource,
                Name = this.Name,
                AssignCommand = this.AssignCommand,
                IssueKey = this.IssueKey
            };
        }
    }
}
