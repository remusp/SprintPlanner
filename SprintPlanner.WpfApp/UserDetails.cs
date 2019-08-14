using System;
using GalaSoft.MvvmLight;
using SprintPlanner.Core.Logic;

namespace SprintPlanner.WpfApp
{
    public class UserDetails : ViewModelBase
    {
        private readonly UserDetailsModel _model;

        public UserDetails(UserDetailsModel model)
        {
            _model = model;
        }

        public string Uid
        {
            get { return _model.Uid; }
            set
            {
                _model.Uid = value;
                RaisePropertyChanged();
            }
        }

        public string UserName
        {
            get { return _model.UserName; }
            set
            {
                _model.UserName = value;
                RaisePropertyChanged();
            }
        }

        public decimal HoursPerDay
        {
            get { return _model.HoursPerDay; }
            set
            {
                _model.HoursPerDay = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Capacity));
                RaisePropertyChanged(nameof(ScaledCapacity));
            }
        }

        public int DaysOff
        {
            get { return _model.DaysOff; }
            set
            {
                _model.DaysOff = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Capacity));
                RaisePropertyChanged(nameof(ScaledCapacity));
            }
        }


        public int DaysInSprint
        {
            get { return _model.DaysInSprint; }
            set
            {
                _model.DaysInSprint = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Capacity));
                RaisePropertyChanged(nameof(ScaledCapacity));
            }
        }

        public decimal CapacityFactor
        {
            get { return _model.CapacityFactor; }
            set
            {
                _model.CapacityFactor = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(ScaledCapacity));
            }
        }

        public decimal Capacity
        {
            get
            {
                return _model.Capacity;
            }
        }

        public decimal ScaledCapacity
        {
            get { return _model.ScaledCapacity; }
        }

        public UserDetailsModel GetModel()
        {
            return _model;
        }
    }
}
