using SprintPlanner.FrameworkWPF;
using System.Windows.Input;

namespace SprintPlanner.WpfApp.UI.TeamsCrud
{
    public class SearchUserItem : ViewModelBase
    {
        public string Name
        {
            get { return Get(() => Name); }
            set { Set(() => Name, value); }
        }

        public string Email
        {
            get { return Get(() => Email); }
            set { Set(() => Email, value); }
        }

        public string UserId
        {
            get { return Get(() => UserId); }
            set { Set(() => UserId, value); }
        }

        public ICommand AddCommand { get; set; }
    }
}
