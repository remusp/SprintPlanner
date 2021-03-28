using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SprintPlanner.Core;
using SprintPlanner.Core.Logic;
using SprintPlanner.FrameworkWPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SprintPlanner.WpfApp.UI.Capacity
{
    public class CapacityViewModel : ViewModelBase, IStorageManipulator
    {
        private readonly MetroWindow _window;
        private int _sprintId;

        public CapacityViewModel(MetroWindow w)
        {
            AddAllInCurrentSprintCommand = new DelegateCommand(AddAllInCurrentSprintCommandExecute);
            AddFromComboSprintCommand = new DelegateCommand(AddFromComboSprintCommandExecute);
            Users = new ObservableCollection<UserDetails>();
            CapacityFactor = 1;
            _window = w;
            PropertyChanged += CapacityWindowViewModel_PropertyChanged;
        }

        public string BusyReason
        {
            get { return Get(() => BusyReason); }
            set { Set(() => BusyReason, value); }
        }

        public decimal CapacityFactor
        {
            get { return Get(() => CapacityFactor); }
            set { Set(() => CapacityFactor, value); }
        }

        public int DaysInSprint
        {
            get { return Get(() => DaysInSprint); }
            set { Set(() => DaysInSprint, value); }
        }

        public bool IsBusy
        {
            get { return Get(() => IsBusy); }
            set { Set(() => IsBusy, value); }
        }

        public List<Tuple<int, string>> Sprints
        {
            get { return Get(() => Sprints); }
            set { Set(() => Sprints, value); }
        }

        public ObservableCollection<UserDetails> Users
        {
            get { return Get(() => Users); }
            set { Set(() => Users, value); }
        }

        public Tuple<int, string> SelectedSprint
        {
            get { return Get(() => SelectedSprint); }
            set { Set(() => SelectedSprint, value); }
        }

        public ICommand AddAllInCurrentSprintCommand { get; private set; }

        public ICommand AddFromComboSprintCommand { get; private set; }

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

            if (Business.Data?.Sprint?.Sprints != null)
            {
                Sprints = new List<Tuple<int, string>>(Business.Data.Sprint.Sprints);
            }
        }

        private void CapacityWindowViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
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

        private void AddAllInCurrentSprintCommandExecute()
        {
            AddAssigneesFromSprint(_sprintId);
        }

        private void AddFromComboSprintCommandExecute()
        {
            AddAssigneesFromSprint(SelectedSprint.Item1);
        }

        private void AddAssigneesFromSprint(int sprintId)
        {
            IsBusy = true;
            BusyReason = "Fetching users...";
            Task<List<Assignee>>.Factory.StartNew(() => Business.Jira.GetAllAssigneesInSprint(sprintId)).ContinueWith((t) =>
            {
                try
                {
                    if (!t.IsFaulted)
                    {
                        var existingUsers = Users.Select(u => u.Uid).ToList();
                        foreach (var assignee in t.Result)
                        {
                            if (!existingUsers.Contains(assignee.name))
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
    }
}