using GalaSoft.MvvmLight;

namespace SprintPlanner.WpfApp
{
    public class UserDetails : ViewModelBase
    {
        private string _uid;

        public string Uid
        {
            get { return _uid; }
            set
            {
                _uid = value;
                RaisePropertyChanged();
            }
        }

        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                RaisePropertyChanged();
            }
        }

        private int _hoursPerDay;

        public int HoursPerDay
        {
            get { return _hoursPerDay; }
            set
            {
                _hoursPerDay = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Capacity));
            }
        }

        private int _daysOff;

        public int DaysOff
        {
            get { return _daysOff; }
            set
            {
                _daysOff = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Capacity));
            }
        }

        private int _daysInSprint;

        public int DaysInSprint
        {
            get { return _daysInSprint; }
            set
            {
                _daysInSprint = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Capacity));
            }
        }

        public int Capacity
        {
            get { return (DaysInSprint - DaysOff) * HoursPerDay; }

        }
    }
}
