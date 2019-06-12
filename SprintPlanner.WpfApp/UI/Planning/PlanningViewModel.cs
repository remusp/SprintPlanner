using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SprintPlanner.Core;
using SprintPlanner.Core.Logic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace SprintPlanner.WpfApp.UI.Planning
{
    public class PlanningViewModel : ViewModelBase, IStorageManipulator
    {
        private bool _initializing;

        private MetroWindow _window;

        public PlanningViewModel(MetroWindow w)
        {
            _selectedBoards = new ObservableCollection<Tuple<int, string>>();
            UserLoads = new ObservableCollection<UserLoadViewModel>();
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
            try
            {
                var capacities = new List<UserLoadViewModel>();
                var team = new List<string>();
                var loads = Business.Jira.GetIssuesPerAssignee(SelectedBoards.First().Item1, SelectedSprint.Item1);


                capacities = (from u in Business.Data.Capacity.Users
                              select new UserLoadViewModel
                              {
                                  Name = u.UserName,
                                  Uid = u.Uid,
                                  Capacity = (u.DaysInSprint - u.DaysOff) * u.HoursPerDay, // TODO: Duplicate capacity formula
                                  CapacityFactor = u.CapacityFactor,
                                  Status = UserStatus.Normal
                              }).ToList();

                //var storyPoints = loads.SelectMany(l => l)
                //    .Where(i => i.fields.issuetype.id == "7" && i.fields.customfield_10013 != null)
                //    .GroupBy(i => i.key)
                //    .Select(g => int.Parse(g.First().fields.customfield_16035.ToString())).Sum();

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
                            TaskLink = $"https://jira.sdl.com/browse/{i.key}",
                            StoryLink = $"https://jira.sdl.com/browse/{(i.fields.issuetype.subtask ? i.fields.parent.key : string.Empty)}",
                            StoryId = i.fields.issuetype.subtask ? i.fields.parent.key : string.Empty,
                            ParentName = i.fields.issuetype.subtask ? i.fields.parent.fields.summary : string.Empty,
                            Name = i.fields.summary,
                            Hours = i.fields.timetracking.remainingEstimateSeconds / 3600m
                        }));

                    }
                }

                team = Business.Data.Capacity.Users.Select(u => u.Uid).ToList();


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
                            TaskLink = $"https://jira.sdl.com/browse/{i.key}",
                            StoryLink = $"https://jira.sdl.com/browse/{(i.fields.issuetype.subtask ? i.fields.parent.key : string.Empty)}",
                            ParentName = i.fields.issuetype.subtask ? i.fields.parent.fields.summary : string.Empty,
                            Name = i.fields.summary,
                            Hours = i.fields.timetracking.remainingEstimateSeconds / 3600m
                        }))
                    });
                }

                IEnumerable<Issue> unassignedIssues = Business.Jira.GetUnassignedIssues(SelectedBoards.First().Item1, SelectedSprint.Item1);
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
                            TaskLink = $"https://jira.sdl.com/browse/{i.key}",
                            StoryLink = $"https://jira.sdl.com/browse/{(i.fields.issuetype.subtask ? i.fields.parent.key : string.Empty)}",
                            ParentName = i.fields.issuetype.subtask ? i.fields.parent.fields.summary : string.Empty,
                            Name = i.fields.summary,
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

        }

        private void ReloadComandExecute()
        {
            Boards = new ObservableCollection<Tuple<int, string>>(Business.Jira.GetBoards().Select(b => new Tuple<int, string>(b.Key, b.Value)).ToList());
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

        }
    }
}
