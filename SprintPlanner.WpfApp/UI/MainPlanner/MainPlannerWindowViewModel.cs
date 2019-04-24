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
            UserLoads = new ObservableCollection<UserLoadViewModel>();
            PropertyChanged += MainPlannerWindowViewModel_PropertyChanged;
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



        private ObservableCollection<KeyValuePair<int, string>> boards;

        public ObservableCollection<KeyValuePair<int, string>> Boards
        {
            get { return boards; }
            set
            {
                boards = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<KeyValuePair<int, string>> sprints;

        public ObservableCollection<KeyValuePair<int, string>> Sprints
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


        private KeyValuePair<int, string> selectedBoard;

        public KeyValuePair<int, string> SelectedBoard
        {
            get { return selectedBoard; }
            set
            {
                selectedBoard = value;
                RaisePropertyChanged();
            }
        }

        private KeyValuePair<int, string> selectedSprint;

        public KeyValuePair<int, string> SelectedSprint
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
            new CapacityWindow().Show();
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

        private void SyncLoadComandExecute()
        {
            // TODO: duplicate code: capacity load
            string fileName = "CapacityData.json";
            if (File.Exists(fileName))
            {
                var cm = JsonConvert.DeserializeObject<CapacityModel>(File.ReadAllText(fileName));
                var loads = Business.Jira.GetIssuesPerAssignee(SelectedBoard.Key, SelectedSprint.Key);
                var capacities = from u in cm.Users
                                 select new UserLoadViewModel
                                 {
                                     Name = u.UserName,
                                     Capacity = u.Capacity,
                                     Load = loads.FirstOrDefault(l => l.Key.Equals(u.UserName)).Sum(i => i.fields.timetracking.remainingEstimateSeconds) / 3600m,
                                     Issues = new ObservableCollection<string>(loads.FirstOrDefault(l => l.Key.Equals(u.UserName)).Select(i => i.key))
                                 };
                UserLoads = new ObservableCollection<UserLoadViewModel>(capacities);
            }
        }

        private void ReloadComandExecute()
        {
            Boards = new ObservableCollection<KeyValuePair<int, string>>(Business.Jira.GetBoards().ToList());
            SelectedBoard = Boards.FirstOrDefault();
        }

        private void MainPlannerWindowViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SelectedBoard):
                    if (!initializing && !SelectedBoard.Equals(default(KeyValuePair<int, string>)))
                    {
                        Sprints = new ObservableCollection<KeyValuePair<int, string>>(Business.Jira.GetOpenSprints(SelectedBoard.Key).ToList());
                        SelectedSprint = Sprints.FirstOrDefault();
                    }

                    break;
            }
        }

        public void Persist()
        {
            if (Boards != null && Boards.Count > 0)
            {
                var data = new SprintModel
                {
                    Boards = Boards,
                    SelectedBoard = SelectedBoard.Key,
                    Sprints = Sprints,
                    SelectedSprint = SelectedSprint.Key
                };

                string serialized = JsonConvert.SerializeObject(data);
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
                SelectedBoard = Boards.First(i => i.Key == sm.SelectedBoard);
                SelectedSprint = Sprints.First(i => i.Key == sm.SelectedSprint);
                initializing = false;
            }
        }

        bool initializing;
    }
}
