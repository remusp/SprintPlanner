using SprintPlanner.Core.BusinessModel;
using System.Collections.ObjectModel;
using System.Linq;

namespace SprintPlanner.WpfApp.UI.TeamsCrud
{
    public class TeamAvailabilityViewModel : WrappingViewModel<Team>
    {
        public TeamAvailabilityViewModel(Team team) : base(team)
        {
            Users = team.Users != null
               ? new ObservableCollection<UserDetailsItem>(team.Users?.Select(u => new UserDetailsItem(u)))
               : new ObservableCollection<UserDetailsItem>();
        }

        public ObservableCollection<UserDetailsItem> Users
        {
            get { return Get(() => Users); }
            set { Set(() => Users, value); }
        }
    }
}
