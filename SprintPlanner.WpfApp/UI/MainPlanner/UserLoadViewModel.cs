using GalaSoft.MvvmLight;

namespace SprintPlanner.WpfApp.UI.MainPlanner
{
    public class UserLoadViewModel : ViewModelBase
    {
        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                RaisePropertyChanged();
            }
        }


        private int capacity;

        public int Capacity
        {
            get { return capacity; }
            set
            {
                capacity = value;
                RaisePropertyChanged();
            }
        }


        private decimal load;

        public decimal Load
        {
            get { return load; }
            set
            {
                load = value;
                RaisePropertyChanged();
            }
        }

    }
}
