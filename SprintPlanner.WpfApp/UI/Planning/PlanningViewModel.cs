using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SprintPlanner.Core;
using SprintPlanner.Core.Logic;
using SprintPlanner.WpfApp.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SprintPlanner.WpfApp.UI.Planning
{
    public class PlanningViewModel : ViewModelBase, IStorageManipulator
    {
        private const string STATUS_DONE = "6";
        private bool _initializing;

        private MetroWindow _window;

        public PlanningViewModel(MetroWindow w)
        {
            _selectedBoards = new ObservableCollection<Tuple<int, string>>();
            UserLoads = new ObservableCollection<UserLoadViewModel>();
            Boards = new ObservableCollection<Tuple<int, string>>();
            _window = w;
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

        private int _storyPoints;

        public int StoryPoints
        {
            get { return _storyPoints; }
            set
            {
                _storyPoints = value;
                RaisePropertyChanged();
            }
        }

        private int _committedStoryPoints;

        public int CommittedStoryPoints
        {
            get { return _committedStoryPoints; }
            set
            {
                _committedStoryPoints = value;
                RaisePropertyChanged();
            }
        }

        #region IsBusy Property

        private bool _isBusy;
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }

            set
            {
                _isBusy = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsNotBusy));
            }
        }

        #endregion IsBusy Property

        public bool IsNotBusy => !_isBusy;

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
                    _window.ShowMessageAsync("Error fetching open sprints", ex.Message + Environment.NewLine + ex.StackTrace);
                }
            }

            _initializing = false;
        }

        private void SyncLoadComandExecute()
        {
            IsBusy = true;
            Task.Factory.StartNew(() =>
            {
                Stopwatch performanceTimer = new Stopwatch();
                performanceTimer.Start();
                try
                {
                    var capacities = new List<UserLoadViewModel>();
                    var team = new List<string>();

                    Stopwatch query1 = new Stopwatch();
                    query1.Start();
                    var allIssues = Business.Jira.GetAllIssuesInSprint(SelectedSprint.Item1);
                    query1.Stop();
                    Debug.WriteLine($"Query 1: {query1.Elapsed}");

                    var openIssues = allIssues.Where(i => i.fields.status.id != STATUS_DONE);
                    var openAssignedIssues = openIssues.Where(i => i.fields.assignee != null);

                    double storyPointsRaw = allIssues.Where(i => i.fields.customfield_10013 != null).Select(j => j.fields.customfield_10013).Sum().Value;
                    StoryPoints = (int)Math.Round(storyPointsRaw);

                    var loads = openAssignedIssues.Where(l => l.fields.issuetype.subtask || l.fields.subtasks.Count == 0).GroupBy(i => i.fields.assignee.name);

                    if (Business.Data.Capacity.Users != null)
                    {
                        capacities = (from u in Business.Data.Capacity.Users
                                      select new UserLoadViewModel
                                      {
                                          Name = u.UserName,
                                          Uid = u.Uid,
                                          Capacity = (u.DaysInSprint - u.DaysOff) * u.HoursPerDay, // TODO: Duplicate capacity formula
                                          CapacityFactor = u.CapacityFactor,
                                          Status = UserStatus.Normal
                                      }).ToList();
                    }

                    //string issueLinkTemplate = $"https://jira.sdl.com/browse/";
                    //new Uri(Settings.Default.Server).Append("browse")

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
                                TaskId = i.fields.issuetype.subtask ? i.key : string.Empty,
                                StoryId = i.fields.issuetype.subtask ? i.fields.parent.key : i.key,
                                TaskLink = $"https://jira.sdl.com/browse/{(i.fields.issuetype.subtask ? i.key : string.Empty)}",
                                StoryLink = $"https://jira.sdl.com/browse/{(i.fields.issuetype.subtask ? i.fields.parent.key : i.key)}",
                                ParentName = i.fields.issuetype.subtask ? i.fields.parent.fields.summary : i.fields.summary,
                                Name = i.fields.issuetype.subtask ? i.fields.summary : string.Empty,
                                Hours = i.fields.timetracking.remainingEstimateSeconds / 3600m
                            }));

                        }
                    }

                    if (Business.Data.Capacity.Users != null)
                    {
                        team = Business.Data.Capacity.Users.Select(u => u.Uid).ToList();
                    }

                    foreach (var load in loads.Where(l => !team.Contains(l.Key)))
                    {
                        var v = new UserLoadViewModel();
                        v.Name = Business.Jira.GetUserDisplayName(load.Key);
                        v.Status = UserStatus.External;
                        v.Uid = load.Key;
                        v.Capacity = 0;
                        v.PictureData = Business.Jira.GetPicture(load.Key);
                        v.Load = load.Sum(i => i.fields.timetracking.remainingEstimateSeconds) / 3600m;
                        v.Issues = new ObservableCollection<IssueViewModel>(load.Select(i => new IssueViewModel
                        {
                            TaskId = i.fields.issuetype.subtask ? i.key : string.Empty,
                            StoryId = i.fields.issuetype.subtask ? i.fields.parent.key : i.key,
                            TaskLink = $"https://jira.sdl.com/browse/{(i.fields.issuetype.subtask ? i.key : string.Empty)}",
                            StoryLink = $"https://jira.sdl.com/browse/{(i.fields.issuetype.subtask ? i.fields.parent.key : i.key)}",
                            ParentName = i.fields.issuetype.subtask ? i.fields.parent.fields.summary : i.fields.summary,
                            Name = i.fields.issuetype.subtask ? i.fields.summary : string.Empty,
                            Hours = i.fields.timetracking.remainingEstimateSeconds / 3600m
                        }));
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
                                TaskId = i.fields.issuetype.subtask ? i.key : string.Empty,
                                StoryId = i.fields.issuetype.subtask ? i.fields.parent.key : i.key,
                                TaskLink = $"https://jira.sdl.com/browse/{(i.fields.issuetype.subtask ? i.key : string.Empty)}",
                                StoryLink = $"https://jira.sdl.com/browse/{(i.fields.issuetype.subtask ? i.fields.parent.key : i.key)}",
                                ParentName = i.fields.issuetype.subtask ? i.fields.parent.fields.summary : i.fields.summary,
                                Name = i.fields.issuetype.subtask ? i.fields.summary : string.Empty,
                                Hours = i.fields.timetracking.remainingEstimateSeconds / 3600m
                            }))
                        });
                    }

                    IEnumerable<Issue> unassignedIssues = openIssues.Where(i => (i.fields.assignee == null) && (i.fields.issuetype.subtask || i.fields.subtasks.Count == 0));
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
                                TaskId = i.fields.issuetype.subtask ? i.key : string.Empty,
                                StoryId = i.fields.issuetype.subtask ? i.fields.parent.key : i.key,
                                TaskLink = $"https://jira.sdl.com/browse/{(i.fields.issuetype.subtask ? i.key : string.Empty)}",
                                StoryLink = $"https://jira.sdl.com/browse/{(i.fields.issuetype.subtask ? i.fields.parent.key : i.key)}",
                                ParentName = i.fields.issuetype.subtask ? i.fields.parent.fields.summary : i.fields.summary,
                                Name = i.fields.issuetype.subtask ? i.fields.summary : string.Empty,
                                Hours = i.fields.timetracking.remainingEstimateSeconds / 3600m
                            }))
                        });
                    }

                    UserLoads = new ObservableCollection<UserLoadViewModel>(capacities);

                }
                catch (Exception ex)
                {
                    _window.ShowMessageAsync("Error getting team tasks", ex.Message + Environment.NewLine + ex.StackTrace);
                }

                performanceTimer.Stop();

                Debug.WriteLine($"Sync load duration: {performanceTimer.Elapsed}");
            }).ContinueWith(t =>
            {
                IsBusy = false;
            });
        }

        private void ReloadComandExecute()
        {
            Boards.Clear();
            Business.Jira.GetBoards().Select(b => new Tuple<int, string>(b.Key, b.Value)).ToList().ForEach(i =>
            {
                Boards.Add(i);
            });

        }

        private UserStatus GetStatusAccordingToLoad(UserLoadViewModel u)
        {
            if (u.Load >= u.Capacity)
            {
                return UserStatus.Danger;
            }

            if ((u.Load >= u.ScaledCapacity) || (u.Load < u.ScaledCapacity * 0.625m))
            {
                return UserStatus.Warning;
            }

            return UserStatus.Normal;
        }

        public void Flush()
        {
            Business.Data.Sprint.SelectedBoard = SelectedBoards.FirstOrDefault()?.Item1 ?? 0;
            Business.Data.Sprint.SelectedSprint = SelectedSprint?.Item1 ?? 0;
            Business.Data.Sprint.Boards = Boards?.ToList();
            Business.Data.Sprint.Sprints = Sprints?.ToList();
        }

        public void Pull()
        {
            _initializing = true;
            if (Business.Data?.Sprint?.Boards != null)
            {
                Boards = new ObservableCollection<Tuple<int, string>>(Business.Data.Sprint.Boards);
            }

            if (Business.Data?.Sprint?.Sprints != null)
            {
                Sprints = new ObservableCollection<Tuple<int, string>>(Business.Data.Sprint.Sprints);
            }

            try
            {
                SelectedBoards.Add(Boards.First(i => i.Item1 == Business.Data.Sprint.SelectedBoard));
                RaisePropertyChanged(nameof(SelectedBoards));
                SelectedSprint = Sprints.First(i => i.Item1 == Business.Data.Sprint.SelectedSprint);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message} {ex.StackTrace}");
            }

            _initializing = false;

        }
    }
}
