using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using SprintPlanner.Core;
using SprintPlanner.Core.Logic;
using SprintPlanner.Core.Reporting;
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

        private readonly MetroWindow _window;

        private ReportGenerator _reportGenerator;

        public PlanningViewModel(MetroWindow w)
        {
            _selectedBoards = new ObservableCollection<Tuple<int, string>>();
            UserLoads = new ObservableCollection<UserLoadViewModel>();
            Boards = new ObservableCollection<Tuple<int, string>>();
            _window = w;
            _reportGenerator = new ReportGenerator();
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
                return _reloadCommand ?? (_reloadCommand = new RelayCommand(ReloadComandExecute));
            }
        }

        #region SyncLoad command

        private ICommand _syncLoadCommand;

        public ICommand SyncLoadCommand
        {
            get
            {
                return _syncLoadCommand ?? (_syncLoadCommand = new RelayCommand(SyncLoadComandExecute));
            }
        }

        #endregion SyncLoad command

        #region Export command

        private ICommand _exportCommand;

        public ICommand ExportCommand
        {
            get
            {
                return _exportCommand ?? (_exportCommand = new RelayCommand(ExportComandExecute));
            }
        }

        #endregion Export command

        private ICommand _selectedBoardChangedCommand;

        public ICommand SelectedBoardChangedCommand
        {
            get
            {
                return _selectedBoardChangedCommand ?? (_selectedBoardChangedCommand = new RelayCommand(SelectedBoardChangedComandExecute));
            }
        }



        private void SelectedBoardChangedComandExecute()
        {
            if (!_initializing && !SelectedBoards.Equals(default(Tuple<int, string>)))
            {
                try
                {
                    int board = -1;
                    var boardTuple = SelectedBoards.FirstOrDefault();
                    if (boardTuple != null)
                    {
                        board = boardTuple.Item1;
                    }

                    if (board >= 0 && Business.Data.Sprint.SelectedBoard == board)
                    {
                        return;
                    }

                    if (board >= 0) 
                    {
                        IsBusy = true;
                        Task.Factory.StartNew(() => Business.Jira.GetOpenSprints(board)).ContinueWith(t =>
                        {
                            Business.Data.Sprint.SelectedBoard = board;
                            if (!t.IsFaulted)
                            {
                                Sprints = new ObservableCollection<Tuple<int, string>>(t.Result);
                                var sprint = Sprints.FirstOrDefault(i => i.Item1 == Business.Data.Sprint.SelectedSprint);
                                SelectedSprint = sprint ?? Sprints.FirstOrDefault();
                            }
                            else
                            {
                                _window.ShowMessageAsync("Error fetching open sprints", t.Exception.Flatten().Message + Environment.NewLine + t.Exception.Flatten().StackTrace);
                            }

                            IsBusy = false;
                        }, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                }
                catch (Exception ex)
                {
                    _window.ShowMessageAsync("Error fetching open sprints", ex.Message + Environment.NewLine + ex.StackTrace);
                }
            }

            _initializing = false;
        }

        private void ExportComandExecute()
        {
            if (!_reportGenerator.HasData())
            {
                _window.ShowMessageAsync("Please load sprint data first!", string.Empty);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel (*.xls)|*.xls|Excel (*.xlsx)|*.xlsx";
            sfd.FileName = SelectedSprint.Item2;

            var dialogResult = sfd.ShowDialog();

            if (dialogResult != true)
            {
                return;
            }

            _reportGenerator.GenerateReport(sfd.FileName);
        }



        private void SyncLoadComandExecute()
        {
            IsBusy = true;
            Task.Factory.StartNew(() =>
            {
                Stopwatch performanceTimer = new Stopwatch();
                performanceTimer.Start();

                var capacities = new List<UserLoadViewModel>();
                var team = new List<string>();

                Stopwatch query1 = new Stopwatch();
                query1.Start();
                var mandatoryFields = new List<string>
                    {
                        "id", "key", "timetracking", "status", "assignee",
                        "issuetype", "subtasks", "parent","summary",Settings.Default.StoryPointsField
                    };

                var customFields = new List<string> { Settings.Default.StoryPointsField };

                var extendedIssues = Business.Jira.GetAllIssuesInSprint(SelectedSprint.Item1, mandatoryFields, customFields);
                var allIssues = extendedIssues.Item1;
                query1.Stop();
                Debug.WriteLine($"Query 1: {query1.Elapsed}");

                var openIssues = allIssues.Where(i => i.fields.status.id != STATUS_DONE);


                var openAssignedIssues = openIssues.Where(i => i.fields.assignee != null);
                var openIssueKeys = openIssues.Select(i => i.key);
                var customDataForOpenIssues = extendedIssues.Item2.Where(kvp => openIssueKeys.Contains(kvp.Key));

                var flatCustomDataForOpenIssues = customDataForOpenIssues.SelectMany(kvp => kvp.Value);
                double storyPointsRaw = flatCustomDataForOpenIssues.Where(d => d != null && d is JProperty).Select(d1 => d1 as JProperty).Where(d2 => d2.Name == Settings.Default.StoryPointsField).Sum(d3 => ((JValue)d3.Value).Value != null ? d3.Value.Value<double>() : 0);

                StoryPoints = (int)Math.Round(storyPointsRaw);

                var loads = openAssignedIssues.Where(l => l.fields.issuetype.subtask || l.fields.subtasks.Count == 0).GroupBy(i => i.fields.assignee.name);
                string link = new Uri(Settings.Default.Server).Append("browse/").AbsoluteUri;

                if (Business.Data.Capacity.Users != null)
                {
                    foreach (var user in Business.Data.Capacity.Users)
                    {
                        var userIssues = loads.FirstOrDefault(l => l.Key.Equals(user.Uid));
                        AddCapacity(capacities, userIssues, user, link, Business.Jira.GetPicture(user.Uid));
                    }
                }

                if (Business.Data.Capacity.Users != null)
                {
                    team = Business.Data.Capacity.Users.Select(u => u.Uid).ToList();
                }

                foreach (var load in loads.Where(l => !team.Contains(l.Key)))
                {
                    var user = new UserDetailsModel
                    {
                        UserName = Business.Jira.GetUserDisplayName(load.Key),
                        Uid = load.Key
                    };

                    AddCapacity(capacities, load, user, link, Business.Jira.GetPicture(load.Key), UserStatus.External);
                }

                IEnumerable<Issue> unassignedIssues = openIssues.Where(i => (i.fields.assignee == null) && (i.fields.issuetype.subtask || i.fields.subtasks.Count == 0));
                if (unassignedIssues.Any())
                {
                    var user = new UserDetailsModel
                    {
                        UserName = "Unassigned"
                    };

                    AddCapacity(capacities, unassignedIssues, user, link, File.ReadAllBytes(@"Data\Unassigned.png"), UserStatus.External);
                }

                var oldUserLoads = new ObservableCollection<UserLoadViewModel>(UserLoads);
                UserLoads = new ObservableCollection<UserLoadViewModel>(capacities);

                foreach (var user in oldUserLoads)
                {
                    if (user.IsExpanded == false)
                    {
                        var recreated = UserLoads.FirstOrDefault(u => u.Uid == user.Uid);
                        if (recreated != null)
                        {
                            recreated.IsExpanded = user.IsExpanded;
                        }
                    }
                }

                _reportGenerator.SetReportData(openIssues, UserLoads.Select(ul => ul.GetModel()), customDataForOpenIssues);
                _reportGenerator.StoryPointsField = Settings.Default.StoryPointsField;

                performanceTimer.Stop();

                Debug.WriteLine($"Sync load duration: {performanceTimer.Elapsed}");
            }).ContinueWith(t =>
            {
                IsBusy = false;

                if (t.Exception != null)
                {
                    var message = string.Join("; ", t.Exception.InnerExceptions);
                    var stackTraces = string.Join($"---{Environment.NewLine}", t.Exception.InnerExceptions.Select(ie => ie.StackTrace));
                    var flatException = t.Exception.Flatten();
                    _window.ShowMessageAsync("Error getting team tasks", $"{message}{Environment.NewLine}{stackTraces}");
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void AddCapacity(List<UserLoadViewModel> capacities, IEnumerable<Issue> issues, UserDetailsModel user, string link, byte[] picture, UserStatus? explicitStatus = null)
        {
            decimal load = issues != null ? issues.Sum(i => i.fields.timetracking.remainingEstimateSeconds) / 3600m : 0;
            var computedStatus = GetStatusAccordingToLoad(user, load);

            List<IssueModel> issuesModel;
            if (issues != null)
            {
                issuesModel = issues.Select(i => new IssueModel
                {
                    TaskId = i.fields.issuetype.subtask ? i.key : string.Empty,
                    StoryId = i.fields.issuetype.subtask ? i.fields.parent.key : i.key,
                    TaskLink = link + $"{(i.fields.issuetype.subtask ? i.key : string.Empty)}",
                    StoryLink = link + $"{(i.fields.issuetype.subtask ? i.fields.parent.key : i.key)}",
                    ParentName = i.fields.issuetype.subtask ? i.fields.parent.fields.summary : i.fields.summary,
                    Name = i.fields.issuetype.subtask ? i.fields.summary : string.Empty,
                    Hours = i.fields.timetracking.remainingEstimateSeconds / 3600m
                }).ToList();
            }
            else
            {
                issuesModel = new List<IssueModel>();
            }

            capacities.Add(new UserLoadViewModel(new UserLoadModel(user, issuesModel)
            {
                Load = load,
                PictureData = picture,
            })
            {
                Status = explicitStatus ?? computedStatus,
            });
        }

        private void ReloadComandExecute()
        {
            IsBusy = true;
            Boards.Clear();
            Task.Factory.StartNew(() => Business.Jira.GetBoards()).ContinueWith(t =>
            {
                t.Result.Select(b => new Tuple<int, string>(b.Key, b.Value)).ToList().ForEach(i => Boards.Add(i));
                IsBusy = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private UserStatus GetStatusAccordingToLoad(UserDetailsModel u, decimal load)
        {
            if (load >= u.Capacity)
            {
                return UserStatus.Danger;
            }

            if ((load >= u.ScaledCapacity) || (load < u.ScaledCapacity * 0.625m))
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
            if (Business.Data?.Sprint?.Boards != null && Boards.Count == 0)
            {
                Boards = new ObservableCollection<Tuple<int, string>>(Business.Data.Sprint.Boards);
            }

            if (Business.Data?.Sprint?.Sprints != null)
            {
                Sprints = new ObservableCollection<Tuple<int, string>>(Business.Data.Sprint.Sprints);
            }

            try
            {
                var board = Boards.First(i => i.Item1 == Business.Data.Sprint.SelectedBoard);
                if (!SelectedBoards.Contains(board))
                {
                    SelectedBoards.Add(board);
                }

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
