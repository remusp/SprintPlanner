using SprintPlanner.Core.BusinessModel;
using SprintPlanner.FrameworkWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SprintPlanner.WpfApp.UI.Capacity
{
    public class TeamItem : WrappingViewModel<Team>
    {
        public TeamItem(Team team) : base(team)
        {
            TeamAvailability = new TeamAvailabilityViewModel(team);        
        }

        public string Name
        {
            get { return Get(() => Name); }
            set { Set(() => Name, value); }
        }

        public string FullPath
        {
            get { return Get(() => FullPath); }
            set { Set(() => FullPath, value); }
        }

        public TeamAvailabilityViewModel TeamAvailability
        {
            get { return Get(() => TeamAvailability); }
            set { Set(() => TeamAvailability, value); }
        }

        public override Team GetModel()
        {
            _model.Users = new List<Core.Logic.UserDetailsModel>(TeamAvailability.Users.Select(u => u.GetModel()));
            return base.GetModel();
        }
    }
}
