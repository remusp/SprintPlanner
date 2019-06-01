using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using SprintPlanner.WpfApp.Properties;
using SprintPlanner.WpfApp.UI.Capacity;
using SprintPlanner.WpfApp.UI.Login;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;

namespace SprintPlanner.WpfApp.UI.MainPlanner
{
    public class MainPlannerWindowViewModel : ViewModelBase
    {
        private bool _initializing;

        public MainPlannerWindowViewModel(Window w)
        {
            _window = w;
            _selectedBoards = new ObservableCollection<Tuple<int, string>>();
            UserLoads = new ObservableCollection<UserLoadViewModel>();

        }

        private Window _window;

        private ICommand _openCapacityWindowCommand;

        public ICommand OpenCapacityWindowCommand
        {
            get
            {
                if (_openCapacityWindowCommand == null)
                {
                    _openCapacityWindowCommand = new RelayCommand(OpenCapacityWindowCommandExecute);
                }

                return _openCapacityWindowCommand;
            }
        }



        private ObservableCollection<Tuple<int, string>> _boards;

        public ObservableCollection<Tuple<int, string>> Boards
        {
            get { return _boards; }
            set
            {
                _boards = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<Tuple<int, string>> _sprints;

        public ObservableCollection<Tuple<int, string>> Sprints
        {
            get { return _sprints; }
            set
            {
                _sprints = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<UserLoadViewModel> _userLoads;

        public ObservableCollection<UserLoadViewModel> UserLoads
        {
            get { return _userLoads; }
            set
            {
                _userLoads = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<Tuple<int, string>> _selectedBoards;

        public ObservableCollection<Tuple<int, string>> SelectedBoards
        {
            get { return _selectedBoards; }
            set
            {
                _selectedBoards = value;
                RaisePropertyChanged();
            }
        }

        private Tuple<int, string> _selectedSprint;

        public Tuple<int, string> SelectedSprint
        {
            get { return _selectedSprint; }
            set
            {
                _selectedSprint = value;
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

        private ICommand _reloadCommand;

        public ICommand ReloadCommand
        {
            get
            {
                if (_reloadCommand == null)
                {
                    _reloadCommand = new RelayCommand(ReloadComandExecute);
                }

                return _reloadCommand;
            }
        }

        //SyncLoadCommand

        private ICommand _syncLoadCommand;

        public ICommand SyncLoadCommand
        {
            get
            {
                if (_syncLoadCommand == null)
                {
                    _syncLoadCommand = new RelayCommand(SyncLoadComandExecute);
                }

                return _syncLoadCommand;
            }
        }

        private ICommand _selectedBoardChangedCommand;

        public ICommand SelectedBoardChangedCommand
        {
            get
            {
                if (_selectedBoardChangedCommand == null)
                {
                    _selectedBoardChangedCommand = new RelayCommand(SelectedBoardChangedComandExecute);
                }

                return _selectedBoardChangedCommand;
            }
        }

        private void SelectedBoardChangedComandExecute()
        {
            if (!_initializing && !SelectedBoards.Equals(default(Tuple<int, string>)))
            {
                try
                {
                    Sprints = new ObservableCollection<Tuple<int, string>>(Business.Jira.GetOpenSprints(SelectedBoards.First().Item1));
                    SelectedSprint = Sprints.FirstOrDefault();
                }
                catch (Exception ex)
                {
                }
            }
        }

        private void SyncLoadComandExecute()
        {
            try
            {
                // TODO: duplicate code: capacity load
                string fileName = "CapacityData.json";
                if (File.Exists(fileName))
                {
                    var cm = JsonConvert.DeserializeObject<CapacityModel>(File.ReadAllText(fileName));
                    var loads = Business.Jira.GetIssuesPerAssignee(SelectedBoards.First().Item1, SelectedSprint.Item1);
                    var capacities = (from u in cm.Users
                                      select new UserLoadViewModel
                                      {
                                          Name = u.UserName,
                                          Uid = u.Uid,
                                          Capacity = u.Capacity,
                                          ScaledCapacity = u.ScaledCapacity,
                                          Status = UserStatus.Normal
                                      }).ToList();


                    foreach (UserLoadViewModel u in capacities)
                    {
                        u.PictureData = Business.Jira.GetPicture(u.Uid);
                        var load = loads.FirstOrDefault(l => l.Key.Equals(u.Uid));
                        if (load != null)
                        {
                            u.Load = load.Sum(i => i.fields.timetracking.remainingEstimateSeconds) / 3600m;
                            u.Status = GetStatusAccordingToLoad(u);
                            u.Issues = new ObservableCollection<IssueViewModel>(load.Select(i => new IssueViewModel
                            {
                                TaskId = i.key,
                                TaskLink = "https://www.google.com",
                                StoryLink = "https://www.youtube.com/",
                                StoryId = i.fields.issuetype.subtask ? i.fields.parent.key : string.Empty,
                                ParentName = i.fields.issuetype.subtask ? i.fields.parent.fields.summary : string.Empty,
                                Name = i.fields.summary,
                                Hours = i.fields.timetracking.remainingEstimateSeconds / 3600m
                            }));

                        }
                    }

                    var team = cm.Users.Select(u => u.Uid).ToList();
                    foreach (var load in loads.Where(l => !team.Contains(l.Key)))
                    {
                        capacities.Add(new UserLoadViewModel
                        {
                            Name = Business.Jira.GetUserDisplayName(load.Key),
                            Status = UserStatus.External,
                            Uid = load.Key,
                            Capacity = 0,
                            PictureData = Business.Jira.GetPicture(load.Key),
                            Load = load.Sum(i => i.fields.timetracking.remainingEstimateSeconds) / 3600m,
                            Issues = new ObservableCollection<IssueViewModel>(load.Select(i => new IssueViewModel
                            {
                                TaskId = i.key,
                                StoryId = i.fields.issuetype.subtask ? i.fields.parent.key : string.Empty,
                                TaskLink = "https://www.google.com",
                                StoryLink = "https://www.youtube.com/",
                                ParentName = i.fields.issuetype.subtask ? i.fields.parent.fields.summary : string.Empty,
                                Name = i.fields.summary,
                                Hours = i.fields.timetracking.remainingEstimateSeconds / 3600m
                            }))
                        });
                    }

                    var unassignedIssues = Business.Jira.GetUnassignedIssues(SelectedBoards.First().Item1, SelectedSprint.Item1);
                    if (unassignedIssues.Any())
                    {
                        capacities.Add(new UserLoadViewModel
                        {
                            Name = "Unassigned",
                            Status = UserStatus.External,
                            Capacity = 0,
                            PictureData = File.ReadAllBytes(@"Data\Unassigned.png"),
                            Load = unassignedIssues.Sum(i => i.fields.timetracking.remainingEstimateSeconds) / 3600m,
                            Issues = new ObservableCollection<IssueViewModel>(unassignedIssues.Select(i => new IssueViewModel
                            {
                                TaskId = i.key,
                                StoryId = i.fields.issuetype.subtask ? i.fields.parent.key : string.Empty,
                                TaskLink = "https://www.google.com",
                                StoryLink = "https://www.youtube.com/",
                                ParentName = i.fields.issuetype.subtask ? i.fields.parent.fields.summary : string.Empty,
                                Name = i.fields.summary,
                                Hours = i.fields.timetracking.remainingEstimateSeconds / 3600m
                            }))
                        });
                    }

                    UserLoads = new ObservableCollection<UserLoadViewModel>(capacities);
                }
            }
            catch (Exception ex)
            {
                ((MetroWindow)_window).ShowMessageAsync("Error getting team tasks", ex.Message + Environment.NewLine + ex.StackTrace);
            }

        }

        private UserStatus GetStatusAccordingToLoad(UserLoadViewModel u)
        {
            if (u.Load >= u.Capacity * 0.875m)
            {
                return UserStatus.Danger;
            }

            if ((u.Load >= u.ScaledCapacity * 0.875m) || (u.Load < u.ScaledCapacity * 0.625m))
            {
                return UserStatus.Warning;
            }

            return UserStatus.Normal;
        }

        private void ReloadComandExecute()
        {
            Boards = new ObservableCollection<Tuple<int, string>>(Business.Jira.GetBoards().Select(b => new Tuple<int, string>(b.Key, b.Value)).ToList());
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
                _initializing = true;
                var sm = JsonConvert.DeserializeObject<SprintModel>(File.ReadAllText(fileName));
                Boards = sm.Boards;
                Sprints = sm.Sprints;

                SelectedBoards.Add(Boards.First(i => i.Item1 == sm.SelectedBoard));
                RaisePropertyChanged(nameof(SelectedBoards));
                SelectedSprint = Sprints.First(i => i.Item1 == sm.SelectedSprint);
                _initializing = false;
            }
        }


    }
}
