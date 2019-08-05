using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SprintPlanner.Core.Logic;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SprintPlanner.WpfApp.UI.Capacity
{
    public class CapacityViewModel : ViewModelBase, IStorageManipulator
    {
        private int _sprintId;
        private int _boardId;

        public CapacityViewModel()
        {
            Users = new ObservableCollection<UserDetails>();
            CapacityFactor = 1;
            PropertyChanged += CapacityWindowViewModel_PropertyChanged;
        }



        private void CapacityWindowViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(DaysInSprint):
                    foreach (var u in Users)
                    {
                        u.DaysInSprint = DaysInSprint;
                    }

                    break;

                case nameof(CapacityFactor):
                    foreach (var u in Users)
                    {
                        u.CapacityFactor = CapacityFactor;
                    }

                    break;
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
            }
        }

        private ObservableCollection<UserDetails> _users;

        public ObservableCollection<UserDetails> Users
        {
            get { return _users; }
            set
            {
                _users = value;
                RaisePropertyChanged();
            }
        }

        private ICommand _refreshCommand;

        public ICommand RefreshCommand
        {
            get
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new RelayCommand(RefreshCommandExecute);
                }

                return _refreshCommand;
            }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged();
            }
        }


        private string _busyReason;

        public string BusyReason
        {
            get { return _busyReason; }
            set
            {
                _busyReason = value;
                RaisePropertyChanged();
            }
        }



        private void RefreshCommandExecute()
        {
            IsBusy = true;
            BusyReason = "Fetching users...";
            Users.Clear();
            Task<List<string>>.Factory.StartNew(() =>
            {
                return Business.Jira.GetAllAssigneesInSprint(_sprintId);
            }).ContinueWith((t) =>
            {
                try
                {
                    foreach (var item in t.Result)
                    {
                        string name = Business.Jira.GetUserDisplayName(item);
                        Users.Add(new UserDetails
                        {
                            Uid = item,
                            UserName = name,
                            DaysInSprint = DaysInSprint,
                            CapacityFactor = CapacityFactor
                        });
                    }
                }
                finally
                {
                    IsBusy = false;
                }

            }, TaskScheduler.FromCurrentSynchronizationContext());

        }

        public void Flush()
        {
            Business.Data.Capacity.DaysInSprint = DaysInSprint;
            Business.Data.Capacity.CapacityFactor = CapacityFactor;
            Business.Data.Capacity.Users = Users.Select(u => new UserDetailsModel
            {
                Uid = u.Uid,
                UserName = u.UserName,
                HoursPerDay = u.HoursPerDay,
                DaysOff = u.DaysOff,
                DaysInSprint = u.DaysInSprint,
                CapacityFactor = u.CapacityFactor
            }).ToList();
        }

        public void Pull()
        {
            DaysInSprint = Business.Data.Capacity.DaysInSprint;
            if (Business.Data?.Capacity?.Users != null)
            {
                Users = new ObservableCollection<UserDetails>(Business.Data.Capacity.Users?.Select(u => new UserDetails
                {
                    Uid = u.Uid,
                    UserName = u.UserName,
                    HoursPerDay = u.HoursPerDay,
                    DaysOff = u.DaysOff,
                    DaysInSprint = u.DaysInSprint,
                    CapacityFactor = u.CapacityFactor
                }));
            }

            CapacityFactor = Business.Data.Capacity.CapacityFactor;

            _boardId = Business.Data.Sprint.SelectedBoard;
            _sprintId = Business.Data.Sprint.SelectedSprint;
        }
    }
}
