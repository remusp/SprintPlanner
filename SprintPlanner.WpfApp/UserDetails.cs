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

        private decimal _hoursPerDay;

        public decimal HoursPerDay
        {
            get { return _hoursPerDay; }
            set
            {
                _hoursPerDay = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Capacity));
                RaisePropertyChanged(nameof(ScaledCapacity));
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
                RaisePropertyChanged(nameof(ScaledCapacity));
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
                RaisePropertyChanged(nameof(ScaledCapacity));
            }
        }

        private decimal _capacityFactor;

        public decimal CapacityFactor
        {
            get { return _capacityFactor; }
            set
            {
                _capacityFactor = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(ScaledCapacity));
            }
        }

        public decimal Capacity
        {
            get { return (DaysInSprint - DaysOff) * HoursPerDay; }

        }

        public decimal ScaledCapacity
        {
            get { return Capacity * CapacityFactor; }
        }
    }
}
