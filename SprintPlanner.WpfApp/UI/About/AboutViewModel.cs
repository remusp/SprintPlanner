using SprintPlanner.FrameworkWPF;

namespace SprintPlanner.WpfApp.UI.About
{
    public class AboutViewModel : ViewModelBase
    {
        public string ProductName
        {
            get { return Get(() => ProductName); }
            set { Set(() => ProductName, value); }
        }

        public string ProductVersion
        {
            get { return Get(() => ProductVersion); }
            set { Set(() => ProductVersion, value); }
        }
    }
}