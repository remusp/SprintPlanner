using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace SprintPlanner.WpfApp.UI.MainPlanner
{
    public class UserLoadViewModel : ViewModelBase
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged();
            }
        }


        private int _capacity;

        public int Capacity
        {
            get { return _capacity; }
            set
            {
                _capacity = value;
                RaisePropertyChanged();
            }
        }


        private decimal _load;

        public decimal Load
        {
            get { return _load; }
            set
            {
                _load = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<IssueViewModel> _issues;

        public ObservableCollection<IssueViewModel> Issues
        {
            get { return _issues; }
            set
            {
                _issues = value;
                RaisePropertyChanged();
            }
        }

    }
}
