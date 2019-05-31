using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace SprintPlanner.WpfApp.UI.MainPlanner
{
    public class UserLoadViewModel : ViewModelBase
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


        private decimal _capacity;

        public decimal Capacity
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

    }
}
