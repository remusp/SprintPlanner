using GalaSoft.MvvmLight;
using SprintPlanner.Core.Logic;
using System.Collections.ObjectModel;

namespace SprintPlanner.WpfApp.UI.Planning
{
    public class UserLoadViewModel : ViewModelBase
    {
        private readonly UserDetailsModel _userModel;

        public UserLoadViewModel(UserDetailsModel user)
        {
            _userModel = user;
        }

        public string Uid
        {
            get { return _userModel.Uid; }
            set
            {
                _userModel.Uid = value;
                RaisePropertyChanged();
            }
        }

        public string Name
        {
            get { return _userModel.UserName; }
            set
            {
                _userModel.UserName = value;
                RaisePropertyChanged();
            }
        }

        public decimal Capacity
        {
            get { return _userModel.Capacity; }
        }

        public decimal ScaledCapacity
        {
            get { return _userModel.ScaledCapacity; }
        }

        public decimal CapacityFactor
        {
            get { return _userModel.CapacityFactor; }
            set
            {
                _userModel.CapacityFactor = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(ScaledCapacity));
            }
        }

        private decimal _load;

        public decimal Load
        {
            get { return _load; }
            set
            {
                _load = value;
                RaisePropertyChanged(nameof(_load));
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

        #region PictureData Property

        private byte[] _pictureData;
        public byte[] PictureData
        {
            get
            {
                return _pictureData;
            }

            set
            {
                _pictureData = value;
                RaisePropertyChanged();
            }
        }

        #endregion PictureData Property

        private UserStatus _status;

        public UserStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                RaisePropertyChanged();
            }
        }


    }
}
