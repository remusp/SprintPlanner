using SprintPlanner.FrameworkWPF;
using System.Collections.ObjectModel;

namespace SprintPlanner.WpfApp.UI.Servers
{
    public class ServersViewModel : ViewModelBase
    {
        public ObservableCollection<ServerItemViewModel> Servers
        {
            get { return Get(() => Servers); }
            set { Set(() => Servers, value); }
        }
    }
}
