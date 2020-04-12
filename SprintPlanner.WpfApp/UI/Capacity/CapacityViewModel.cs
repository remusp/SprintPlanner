using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SprintPlanner.Core;
using SprintPlanner.Core.Logic;
using System;
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
        private readonly MetroWindow _window;

        public CapacityViewModel(MetroWindow w)
        {
            Users = new ObservableCollection<UserDetails>();
            CapacityFactor = 1;
            _window = w;
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
            Task<List<Assignee>>.Factory.StartNew(() => Business.Jira.GetAllAssigneesInSprint(_sprintId)).ContinueWith((t) =>
            {
                try
                {
                    if (!t.IsFaulted)
                    {
                        foreach (var assignee in t.Result)
                        {
                            Users.Add(new UserDetails(new UserDetailsModel
                            {
                                Uid = assignee.name,
                                UserName = assignee.displayName,
                                DaysInSprint = DaysInSprint,
                                CapacityFactor = CapacityFactor
                            }));
                        }
                    }
                    else
                    {
                        var message = string.Join("; ", t.Exception.InnerExceptions);
                        var stackTraces = string.Join($"---{Environment.NewLine}", t.Exception.InnerExceptions.Select(ie => ie.StackTrace));
                        var flatException = t.Exception.Flatten();
                        _window.ShowMessageAsync("Error fetching users", $"{message}{Environment.NewLine}{stackTraces}");
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
            Business.Data.Capacity.Users = Users.Select(u => u.GetModel()).ToList();
        }

        public void Pull()
        {
            DaysInSprint = Business.Data.Capacity.DaysInSprint;
            if (Business.Data?.Capacity?.Users != null)
            {
                Users = new ObservableCollection<UserDetails>(Business.Data.Capacity.Users?.Select(u => new UserDetails(u)));
            }

            CapacityFactor = Business.Data.Capacity.CapacityFactor;

            _sprintId = Business.Data.Sprint.SelectedSprint;
        }
    }
}
