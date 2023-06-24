using SprintPlanner.FrameworkWPF;

namespace SprintPlanner.WpfApp.UI.SprintCrud
{
    public class SprintPlanItem : ViewModelBase
    {
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
    }
}
