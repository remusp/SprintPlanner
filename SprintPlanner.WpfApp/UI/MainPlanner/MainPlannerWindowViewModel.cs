using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using SprintPlanner.WpfApp.Properties;
using SprintPlanner.WpfApp.UI.Capacity;
using SprintPlanner.WpfApp.UI.Login;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SprintPlanner.WpfApp.UI.MainPlanner
{
    public class MainPlannerWindowViewModel : ViewModelBase
    {
        public MainPlannerWindowViewModel(Window w)
        {
            _window = w;
            selectedBoards = new ObservableCollection<Tuple<int, string>>();
            UserLoads = new ObservableCollection<UserLoadViewModel>();
        }

        private Window _window;

        private ICommand openCapacityWindowCommand;

        public ICommand OpenCapacityWindowCommand
        {
            get
            {
                if (openCapacityWindowCommand == null)
                {
                    openCapacityWindowCommand = new RelayCommand(OpenCapacityWindowCommandExecute);
                }

                return openCapacityWindowCommand;
            }
        }



        private ObservableCollection<Tuple<int, string>> boards;

        public ObservableCollection<Tuple<int, string>> Boards
        {
            get { return boards; }
            set
            {
                boards = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<Tuple<int, string>> sprints;

        public ObservableCollection<Tuple<int, string>> Sprints
        {
            get { return sprints; }
            set
            {
                sprints = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<UserLoadViewModel> userLoads;

        public ObservableCollection<UserLoadViewModel> UserLoads
        {
            get { return userLoads; }
            set
            {
                userLoads = value;
                RaisePropertyChanged();
            }
        }


        private ObservableCollection<Tuple<int, string>> selectedBoards;

        public ObservableCollection<Tuple<int, string>> SelectedBoards
        {
            get { return selectedBoards; }
            set
            {
                selectedBoards = value;
                RaisePropertyChanged();
            }
        }

        private Tuple<int, string> selectedSprint;

        public Tuple<int, string> SelectedSprint
        {
            get { return selectedSprint; }
            set
            {
                selectedSprint = value;
                RaisePropertyChanged();
            }
        }

        public void EnsureLoggedIn()
        {
            if (string.IsNullOrWhiteSpace(Settings.Default.User) || string.IsNullOrWhiteSpace(Settings.Default.Pass))
            {
                new LoginWindow() { Owner = _window }.Show();
            }
            else
            {
                Business.Jira.Login(Settings.Default.User, Settings.Default.Pass);
            }
        }

        private void OpenCapacityWindowCommandExecute()
        {
            new CapacityWindow(SelectedBoards.First().Item1, SelectedSprint.Item1).Show();
        }

        private ICommand reloadCommand;

        public ICommand ReloadCommand
        {
            get
            {
                if (reloadCommand == null)
                {
                    reloadCommand = new RelayCommand(ReloadComandExecute);
                }

                return reloadCommand;
            }
        }

        //SyncLoadCommand

        private ICommand syncLoadCommand;

        public ICommand SyncLoadCommand
        {
            get
            {
                if (syncLoadCommand == null)
                {
                    syncLoadCommand = new RelayCommand(SyncLoadComandExecute);
                }

                return syncLoadCommand;
            }
        }

        private ICommand selectedBoardChangedCommand;

        public ICommand SelectedBoardChangedCommand
        {
            get
            {
                if (selectedBoardChangedCommand == null)
                {
                    selectedBoardChangedCommand = new RelayCommand(SelectedBoardChangedComandExecute);
                }

                return selectedBoardChangedCommand;
            }
        }

        private void SelectedBoardChangedComandExecute()
        {
            if (!initializing && !SelectedBoards.Equals(default(Tuple<int, string>)))
            {
                try
                {
                    Sprints = new ObservableCollection<Tuple<int, string>>(Business.Jira.GetOpenSprints(SelectedBoards.First().Item1));
                    SelectedSprint = Sprints.FirstOrDefault();
                }
                catch (Exception)
                {
                }
            }
        }

        private void SyncLoadComandExecute()
        {
            // TODO: duplicate code: capacity load
            string fileName = "CapacityData.json";
            if (File.Exists(fileName))
            {
                var cm = JsonConvert.DeserializeObject<CapacityModel>(File.ReadAllText(fileName));
                var loads = Business.Jira.GetIssuesPerAssignee(SelectedBoards.First().Item1, SelectedSprint.Item1);
                var capacities = from u in cm.Users
                                 select new UserLoadViewModel
                                 {
                                     Name = u.UserName,
                                     Capacity = u.Capacity,
                                     Load = loads.FirstOrDefault(l => l.Key.Equals(u.Uid)).Sum(i => i.fields.timetracking.remainingEstimateSeconds) / 3600m,
                                     Issues = new ObservableCollection<IssueViewModel>(loads.FirstOrDefault(l => l.Key.Equals(u.Uid)).Select(i => new IssueViewModel
                                     {
                                         Key = i.key,
                                         ParentName = i.fields.issuetype.subtask ? i.fields.parent.fields.summary : string.Empty,
                                         Name = i.fields.summary
                                     }))
                                 };
                UserLoads = new ObservableCollection<UserLoadViewModel>(capacities);
            }
        }

        private void ReloadComandExecute()
        {
            Boards = new ObservableCollection<Tuple<int, string>>(Business.Jira.GetBoards().Select(b => new Tuple<int, string>(b.Key, b.Value)));
        }

        public void Persist()
        {
            if (Boards != null && Boards.Count > 0)
            {
                var data = new SprintModel
                {
                    Boards = Boards,
                    SelectedBoard = SelectedBoards.First().Item1,
                    Sprints = Sprints,
                    SelectedSprint = SelectedSprint.Item1
                };

                string serialized = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText("SprintData.json", serialized);
            }
        }

        public void Load()
        {
            string fileName = "SprintData.json";
            if (File.Exists(fileName))
            {
                initializing = true;
                var sm = JsonConvert.DeserializeObject<SprintModel>(File.ReadAllText(fileName));
                Boards = sm.Boards;
                Sprints = sm.Sprints;

                SelectedBoards.Add(Boards.First(i => i.Item1 == sm.SelectedBoard));
                RaisePropertyChanged(nameof(SelectedBoards));
                SelectedSprint = Sprints.First(i => i.Item1 == sm.SelectedSprint);
                initializing = false;
            }
        }

        bool initializing;
    }
}
