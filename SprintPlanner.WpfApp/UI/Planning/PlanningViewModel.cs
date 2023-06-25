using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using SprintPlanner.Core;
using SprintPlanner.Core.Logic;
using SprintPlanner.Core.Reporting;
using SprintPlanner.FrameworkWPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SprintPlanner.WpfApp.UI.Planning
{
    public class PlanningViewModel : ViewModelBase
    {
        private readonly ReportGenerator _reportGenerator;
        private readonly MetroWindow _window;

        public PlanningViewModel(MetroWindow w)
        {
            SyncLoadCommand = new DelegateCommand(SyncLoadComandExecute);
            ExportCommand = new DelegateCommand(ExportComandExecute);
            AssignCommand = new DelegateCommand<Assignation>(AssignCommandExecute);
            
            UserLoads = new ObservableCollection<UserLoadViewModel>();
            
            _window = w;
            _reportGenerator = new ReportGenerator();
        }

        public ICommand AssignCommand { get; }

        public ICommand ExportCommand { get; }

        public bool IsBusy
        {
            get { return Get(() => IsBusy); }
            set { Set(() => IsBusy, value); }
        }

        [DependsUpon(nameof(IsBusy))]
        public bool IsNotBusy => !IsBusy;

        public int StoryPoints
        {
            get { return Get(() => StoryPoints); }
            set { Set(() => StoryPoints, value); }
        }

        public ICommand SyncLoadCommand { get; }

        public ObservableCollection<UserLoadViewModel> UserLoads
        {
            get { return Get(() => UserLoads); }
            set { Set(() => UserLoads, value); }
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

        private void AssignCommandExecute(Assignation assignation)
        {
            IsBusy = true;
            Task.Factory.StartNew(() =>
            {
                Business.Jira.AssignIssue(assignation.IssueKey, assignation.UidTarget);
            }).ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {
                    IssueViewModel issue = null;
                    var sourceLoad = UserLoads.FirstOrDefault(ul => ul.Uid == assignation.UidSource);
                    if (sourceLoad != null)
                    {
                        issue = sourceLoad.Issues.FirstOrDefault(i => i.Id == assignation.Id);
                        if (issue != null)
                        {
                            sourceLoad.Issues.Remove(issue);
                            sourceLoad.Load = sourceLoad.Issues.Sum(i => i.Hours);
                            if (sourceLoad.Status != UserStatus.External)
                            {
                                sourceLoad.Status = GetStatusAccordingToLoad(sourceLoad.GetModel().UserDetails, sourceLoad.Load);
                            }

                            var targetLoad = UserLoads.FirstOrDefault(ul => ul.Uid == assignation.UidTarget);
                            if (targetLoad != null)
                            {
                                targetLoad.Issues.Add(issue);
                                targetLoad.Load = targetLoad.Issues.Sum(i => i.Hours);
                                if (targetLoad.Status != UserStatus.External)
                                {
                                    targetLoad.Status = GetStatusAccordingToLoad(targetLoad.GetModel().UserDetails, targetLoad.Load);
                                }

                                issue.Assignables.ForEach(a => a.UidSource = assignation.UidTarget);
                            }
                        }
                    }
                }
                else
                {
                    if (t.Exception != null)
                    {
                        // TODO: refactor duplicate code #ExceptionHandling
                        var message = string.Join("; ", t.Exception.InnerExceptions);
                        var stackTraces = string.Join($"---{Environment.NewLine}", t.Exception.InnerExceptions.Select(ie => ie.StackTrace));
                        _window.ShowMessageAsync("Error assigning task", $"{message}{Environment.NewLine}{stackTraces}");
                    }
                }

                IsBusy = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());
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
            sfd.FileName = Business.Plan.Sprint.Name;

            var dialogResult = sfd.ShowDialog();

            if (dialogResult != true)
            {
                return;
            }

            _reportGenerator.GenerateReport(sfd.FileName);
        }

        private List<Assignation> ExtractBasicUsers(ObservableCollection<UserLoadViewModel> userLoads)
        {
            return userLoads.Select(u => new Assignation
            {
                UidTarget = u.Uid,
                Name = u.Name,
                AssignCommand = AssignCommand
            }).ToList();
        }

        private UserStatus GetStatusAccordingToLoad(UserDetailsModel u, decimal load)
        {
            if (load >= u.Capacity)
            {
                return UserStatus.Danger;
            }

            decimal warningLowerLimit = u.ScaledCapacity - (u.Capacity * 0.2m);
            if (warningLowerLimit < 0)
            {
                warningLowerLimit = 0;
            }

            if ((load > u.ScaledCapacity) || (load < warningLowerLimit))
            {
                return UserStatus.Warning;
            }

            return UserStatus.Normal;
        }
        
        private void SyncLoadComandExecute()
        {
            if (Business.Plan == null) 
            {
                _window.ShowMessageAsync("No plan open!", "Open a plan from the plans list.");
                return;
            }

            IsBusy = true;
            Task.Factory.StartNew(() =>
            {
                Stopwatch performanceTimer = new Stopwatch();
                performanceTimer.Start();

                var capacities = new List<UserLoadViewModel>();
                var team = new List<string>();

                string storyPointsField = Business.Plan.Server.StoryPointsField;

                Stopwatch query1 = new Stopwatch();
                query1.Start();
                var mandatoryFields = new List<string>
                    {
                        "id", "key", "timetracking", "status", "assignee",
                        "issuetype", "subtasks", "parent","summary",storyPointsField
                    };

                var customFields = new List<string> { storyPointsField };

                var extendedIssues = Business.Jira.GetAllIssuesInSprint(Business.Plan.Sprint.Id, mandatoryFields, customFields);
                Business.AppData.SprintCrud.Issues = extendedIssues.Item1;
                var allIssues = extendedIssues.Item1;
                query1.Stop();
                Debug.WriteLine($"Query 1: {query1.Elapsed}");

                var openIssues = allIssues.Where(i => i.fields.status.id != Settings.Default.STATUS_DONE);

                var openAssignedIssues = openIssues.Where(i => i.fields.assignee != null);
                var openIssueKeys = openIssues.Select(i => i.key);
                var customDataForOpenIssues = extendedIssues.Item2.Where(kvp => openIssueKeys.Contains(kvp.Key));

                var flatCustomDataForOpenIssues = customDataForOpenIssues.SelectMany(kvp => kvp.Value);
                double storyPointsRaw = flatCustomDataForOpenIssues.Where(d => d != null && d is JProperty).Select(d1 => d1 as JProperty).Where(d2 => d2.Name == storyPointsField).Sum(d3 => ((JValue)d3.Value).Value != null ? d3.Value.Value<double>() : 0);

                StoryPoints = (int)Math.Round(storyPointsRaw);

                var loads = openAssignedIssues.Where(l => l.fields.issuetype.subtask || l.fields.subtasks.Count == 0).GroupBy(i => i.fields.assignee.name);
                string link = new Uri(Business.Plan.Server.Url).Append("browse/").AbsoluteUri;

                if (Business.AppData.Capacity.Users != null)
                {
                    foreach (var user in Business.AppData.Capacity.Users)
                    {
                        var userIssues = loads.FirstOrDefault(l => l.Key.Equals(user.Uid));
                        byte[] pictureData = null;

                        try
                        {
                            pictureData = Business.Jira.GetPicture(user.Uid);
                        }
                        catch (WebException wex)
                        {
                            Debug.WriteLine($"Unable to fetch picture for user {user.Uid}");
                            Debug.WriteLine(wex);
                        }

                        AddCapacity(capacities, userIssues, user, link, pictureData);
                    }
                }

                if (Business.AppData.Capacity.Users != null)
                {
                    team = Business.AppData.Capacity.Users.Select(u => u.Uid).ToList();
                }

                foreach (var load in loads.Where(l => !team.Contains(l.Key)))
                {
                    var user = new UserDetailsModel
                    {
                        UserName = load.First().fields.assignee.displayName,
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

                var assignables = ExtractBasicUsers(UserLoads);
                foreach (var ul in UserLoads)
                {
                    foreach (var issue in ul.Issues)
                    {
                        issue.Assignables = assignables.ConvertAll(a => (Assignation)a.Clone());
                        foreach (var a in issue.Assignables)
                        {
                            a.Id = issue.Id;
                            a.IssueKey = !string.IsNullOrWhiteSpace(issue.TaskId) ? issue.TaskId : issue.StoryId;
                            a.UidSource = ul.Uid;
                        }
                    }
                }

                foreach (var user in oldUserLoads)
                {
                    if (!user.IsExpanded)
                    {
                        var recreated = UserLoads.FirstOrDefault(u => u.Uid == user.Uid);
                        if (recreated != null)
                        {
                            recreated.IsExpanded = user.IsExpanded;
                        }
                    }
                }

                _reportGenerator.SetReportData(openIssues, UserLoads.Select(ul => ul.GetModel()), customDataForOpenIssues);
                _reportGenerator.StoryPointsField = storyPointsField;

                performanceTimer.Stop();

                Debug.WriteLine($"Sync load duration: {performanceTimer.Elapsed}");
            }).ContinueWith(t =>
            {
                IsBusy = false;

                if (t.Exception != null)
                {
                    // TODO: refactor duplicate code #ExceptionHandling
                    var message = string.Join("; ", t.Exception.InnerExceptions);
                    var stackTraces = string.Join($"---{Environment.NewLine}", t.Exception.InnerExceptions.Select(ie => ie.StackTrace));
                    var flatException = t.Exception.Flatten();
                    _window.ShowMessageAsync("Error getting team tasks", $"{message}{Environment.NewLine}{stackTraces}");
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}