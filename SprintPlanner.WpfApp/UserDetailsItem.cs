using SprintPlanner.Core.Logic;

namespace SprintPlanner.WpfApp
{
    public class UserDetailsItem : WrappingViewModel<UserDetailsModel>
    {
        public UserDetailsItem(UserDetailsModel model) : base(model)
        {
        }

        [DependsUpon(nameof(HoursPerDay))]
        [DependsUpon(nameof(DaysOff))]
        [DependsUpon(nameof(DaysInSprint))]
        public decimal Capacity { get { return _model.Capacity; } }

        public decimal CapacityFactor
        {
            get { return _model.CapacityFactor; }
            set { SetBackingField(() => _model.CapacityFactor = value); }
        }

        public int DaysInSprint
        {
            get { return _model.DaysInSprint; }
            set { SetBackingField(() => _model.DaysInSprint = value); }
        }

        public int DaysOff
        {
            get { return _model.DaysOff; }
            set { SetBackingField(() => _model.DaysOff = value); }
        }

        public decimal HoursPerDay
        {
            get { return _model.HoursPerDay; }
            set { SetBackingField(() => _model.HoursPerDay = value); }
        }

        [DependsUpon(nameof(HoursPerDay))]
        [DependsUpon(nameof(DaysOff))]
        [DependsUpon(nameof(DaysInSprint))]
        [DependsUpon(nameof(CapacityFactor))]
        public decimal ScaledCapacity { get { return _model.ScaledCapacity; } }

        public string Uid
        {
            get { return _model.Uid; }
            set { SetBackingField(() => _model.Uid = value); }
        }

        public string UserName
        {
            get { return _model.UserName; }
            set { SetBackingField(() => _model.UserName = value); }
        }

        public Role Role
        {
            get { return _model.Role; }
            set { SetBackingField(() => _model.Role = value); }
        }
    }
}