using GalaSoft.MvvmLight;

namespace SprintPlanner.WpfApp
{
    public class UserDetails : ViewModelBase
    {
        private string uid;

        public string Uid
        {
            get { return uid; }
            set
            {
                uid = value;
                RaisePropertyChanged();
            }
        }

        private string userName;

        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
                RaisePropertyChanged();
            }
        }

        private int hoursPerDay;

        public int HoursPerDay
        {
            get { return hoursPerDay; }
            set
            {
                hoursPerDay = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Capacity));
            }
        }

        private int daysOff;

        public int DaysOff
        {
            get { return daysOff; }
            set
            {
                daysOff = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Capacity));
            }
        }

        private int daysInSprint;

        public int DaysInSprint
        {
            get { return daysInSprint; }
            set
            {
                daysInSprint = value;
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
