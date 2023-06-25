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
using CoreSprint = SprintPlanner.Core.BusinessModel.Sprint;

namespace SprintPlanner.WpfApp.UI.Capacity
{
    public class CapacityViewModel : ViewModelBase, IStorageManipulator
    {
        private readonly MetroWindow _window;
        private int _sprintId;
        private ICommand _addUserCommand;

        public CapacityViewModel(MetroWindow w)
        {
            AddAllInCurrentSprintCommand = new DelegateCommand(AddAllInCurrentSprintCommandExecute);
            AddFromComboSprintCommand = new DelegateCommand(AddFromComboSprintCommandExecute);
            SearchCommand = new DelegateCommand(SearchCommandExecute);
            _addUserCommand = new DelegateCommand<SearchUserItem>(AddUserExecute);
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

        public List<CoreSprint> Sprints
        {
            get { return Get(() => Sprints); }
            set { Set(() => Sprints, value); }
        }

        public ObservableCollection<UserDetails> Users
        {
            get { return Get(() => Users); }
            set { Set(() => Users, value); }
        }

        public CoreSprint SelectedSprint
        {
            get { return Get(() => SelectedSprint); }
            set { Set(() => SelectedSprint, value); }
        }

        public string SearchText
        {
            get { return Get(() => SearchText); }
            set { Set(() => SearchText, value); }
        }

        public ObservableCollection<SearchUserItem> FoundUsers
        {
            get { return Get(() => FoundUsers); }
            set { Set(() => FoundUsers, value); }
        }

        public ICommand AddAllInCurrentSprintCommand { get; private set; }

        public ICommand AddFromComboSprintCommand { get; private set; }

        public ICommand SearchCommand { get; private set; }

        public void PushData()
        {
            Business.AppData.Capacity.DaysInSprint = DaysInSprint;
            Business.AppData.Capacity.CapacityFactor = CapacityFactor;
            Business.AppData.Capacity.Users = Users.Select(u => u.GetModel()).ToList();
        }

        public void PullData()
        {
            DaysInSprint = Business.AppData.Capacity.DaysInSprint;
            if (Business.AppData?.Capacity?.Users != null)
            {
                Users = new ObservableCollection<UserDetails>(Business.AppData.Capacity.Users?.Select(u => new UserDetails(u)));
            }

            CapacityFactor = Business.AppData.Capacity.CapacityFactor;

            _sprintId = Business.Plan?.Sprint?.Id ?? 0;

            if (Business.AppData?.SprintCrud?.Sprints != null)
            {
                Sprints = new List<CoreSprint>(Business.AppData.SprintCrud.Sprints);
                SelectedSprint = Sprints.First();
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
            if (SelectedSprint == null)
            {
                _window.ShowMessageAsync("Sprint not available!", "Make sure that credentials are set for the current server or that sprints are available in the plans list screen.");
                return;
            }

            AddAssigneesFromSprint(SelectedSprint.Id);
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

        private void SearchCommandExecute()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchText) || SearchText.Length < 3)
                {
                    return;
                }

                var foundUsers = Business.Jira.SearchUsers(SearchText);
                if (foundUsers == null || !foundUsers.Any())
                {
                    return;
                }

                FoundUsers = new ObservableCollection<SearchUserItem>(foundUsers.Select(fu => new SearchUserItem
                {
                    Name = fu.displayName,
                    Email = fu.emailAddress,
                    UserId = fu.name,
                    AddCommand = _addUserCommand
                }));
            }
            catch (Exception ex)
            {
                _window.ShowMessageAsync(ex.Message, ex.StackTrace);
            }
        }

        private void AddUserExecute(SearchUserItem user)
        {
            var existingUsers = Users.Select(u => u.Uid).ToList();
            if (!existingUsers.Contains(user.UserId))
            {
                Users.Add(new UserDetails(new UserDetailsModel
                {
                    Uid = user.UserId,
                    UserName = user.Name,
                    DaysInSprint = DaysInSprint,
                    CapacityFactor = CapacityFactor
                }));
            }
        }
    }
}