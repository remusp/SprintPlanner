using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using SprintPlanner.Core;
using SprintPlanner.Core.BusinessModel;
using SprintPlanner.Core.Logic;
using SprintPlanner.FrameworkWPF;
using SprintPlanner.WpfApp.UI.TeamsCrud;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CoreSprint = SprintPlanner.Core.BusinessModel.Sprint;

namespace SprintPlanner.WpfApp.UI.SprintCrud
{
    public class SprintCrudViewModel : ViewModelBase, IStorageManipulator
    {
        private readonly MetroWindow _window;
        private readonly ICommand _goToPlanningCommand;
        private bool _initializing;

        public SprintCrudViewModel(MetroWindow w, ICommand goToPlanningCommand)
        {
            _window = w;
            _goToPlanningCommand = goToPlanningCommand;
            Boards = new ObservableCollection<Tuple<int, string>>();
            SelectedBoards = new ObservableCollection<Tuple<int, string>>();
            SprintPlanItems = new ObservableCollection<SprintPlanItem>();

            CommandFetchBoards = new DelegateCommand(ExecuteFetchBoards);
            CommandFetchSprints = new DelegateCommand(ExecuteFetchSprints);
            CommandSelectedBoardChanged = new DelegateCommand(ExecuteSelectedBoardChanged);
            CommandAddPlan = new DelegateCommand(async () => await ExecuteAddPlanAsync());
            CommandOpenSprintPlan = new DelegateCommand<SprintPlanItem>(ExecuteOpenSprintPlan);

            PropertyChanged += SprintCrudViewModel_PropertyChanged;
        }

        public ObservableCollection<Tuple<int, string>> Boards
        {
            get { return Get(() => Boards); }
            set { Set(() => Boards, value); }
        }

        public ObservableCollection<Tuple<int, string>> SelectedBoards
        {
            get { return Get(() => SelectedBoards); }
            set { Set(() => SelectedBoards, value); }
        }

        public CoreSprint SelectedSprint
        {
            get { return Get(() => SelectedSprint); }
            set { Set(() => SelectedSprint, value); }
        }

        public ObservableCollection<CoreSprint> Sprints
        {
            get { return Get(() => Sprints); }
            set { Set(() => Sprints, value); }
        }

        public ObservableCollection<SelectableServerItem> Servers
        {
            get { return Get(() => Servers); }
            set { Set(() => Servers, value); }
        }

        public SelectableServerItem SelectedServer
        {
            get { return Get(() => SelectedServer); }
            set { Set(() => SelectedServer, value); }
        }

        public ObservableCollection<SprintPlanItem> SprintPlanItems
        {
            get { return Get(() => SprintPlanItems); }
            set { Set(() => SprintPlanItems, value); }
        }

        public ObservableCollection<TeamItem> TeamItems
        {
            get { return Get(() => TeamItems); }
            set { Set(() => TeamItems, value); }
        }

        public TeamItem SelectedTeamItem
        {
            get { return Get(() => SelectedTeamItem); }
            set { Set(() => SelectedTeamItem, value); }
        }

        public string PlanName
        {
            get { return Get(() => PlanName); }
            set { Set(() => PlanName, value); }
        }

        public bool IsBusy
        {
            get { return Get(() => IsBusy); }
            set { Set(() => IsBusy, value); }
        }

        [DependsUpon(nameof(IsBusy))]
        public bool IsNotBusy => !IsBusy;

        public ICommand CommandFetchBoards { get; }

        public ICommand CommandFetchSprints { get; }

        public ICommand CommandSelectedBoardChanged { get; }
        public ICommand CommandAddPlan { get; }
        public ICommand CommandOpenSprintPlan { get; }

        public void PullData()
        {
            _initializing = true;
            LoadServers();
            LoadBoardsAndSprints();
            LoadTeams();
            LoadPlans();
            _initializing = false;
        }

        public void PushData()
        {
            Business.AppData.SprintCrud.Boards = Boards?.ToList();
            Business.AppData.SprintCrud.SelectedBoard = SelectedBoards.FirstOrDefault()?.Item1 ?? 0;
            Business.AppData.SprintCrud.Sprints = Sprints?.ToList();
            Business.AppData.SprintCrud.SelectedSprint = SelectedSprint;
            Business.AppData.SprintCrud.SelectedServer = SelectedServer?.Server;
        }

        private void ExecuteFetchBoards()
        {
            IsBusy = true;
            Boards.Clear();
            Task.Factory.StartNew(() => Business.Jira.GetBoards()).ContinueWith(t =>
            {
                try
                {
                    t.Result.Select(b => new Tuple<int, string>(b.Key, b.Value)).ToList().ForEach(i => Boards.Add(i));
                }
                finally
                {
                    IsBusy = false;
                }

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void ExecuteFetchSprints()
        {
            try
            {
                int board = -1;
                var boardTuple = SelectedBoards.FirstOrDefault();
                if (boardTuple != null)
                {
                    board = boardTuple.Item1;
                }

                if (board >= 0)
                {
                    IsBusy = true;
                    Task.Factory.StartNew(() => Business.Jira.GetOpenSprints(board)).ContinueWith(t =>
                    {
                        Business.AppData.SprintCrud.SelectedBoard = board;
                        if (!t.IsFaulted)
                        {
                            Sprints = new ObservableCollection<CoreSprint>(t.Result);
                            var sprint = Sprints.FirstOrDefault(i => i.Id == Business.AppData.SprintCrud.SelectedSprint?.Id);
                            SelectedSprint = sprint ?? Sprints.FirstOrDefault();
                        }
                        else
                        {
                            _window.ShowMessageAsync("Error fetching open sprints", t.Exception.Flatten().Message + Environment.NewLine + t.Exception.Flatten().StackTrace);
                        }

                        IsBusy = false;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
                else
                {
                    _window.ShowMessageAsync("Error fetching open sprints", "Invalid board");
                }
            }
            catch (Exception ex)
            {
                _window.ShowMessageAsync("Error fetching open sprints", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        private void ExecuteSelectedBoardChanged()
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

                    if (board >= 0 && Business.AppData.SprintCrud.SelectedBoard == board)
                    {
                        return;
                    }

                    if (board >= 0)
                    {
                        IsBusy = true;
                        Task.Factory.StartNew(() => Business.Jira.GetOpenSprints(board)).ContinueWith(t =>
                        {
                            Business.AppData.SprintCrud.SelectedBoard = board;
                            if (!t.IsFaulted)
                            {
                                Sprints = new ObservableCollection<CoreSprint>(t.Result);
                                var sprint = Sprints.FirstOrDefault(i => i.Id == Business.AppData.SprintCrud.SelectedSprint?.Id);
                                SelectedSprint = sprint ?? Sprints.FirstOrDefault();
                            }
                            else
                            {
                                _window.ShowMessageAsync("Error fetching open sprints", t.Exception.Flatten().Message + Environment.NewLine + t.Exception.Flatten().StackTrace);
                            }

                        }, TaskScheduler.FromCurrentSynchronizationContext()).ContinueWith((_) => IsBusy = false);
                    }
                }
                catch (Exception ex)
                {
                    _window.ShowMessageAsync("Error fetching open sprints", ex.Message + Environment.NewLine + ex.StackTrace);
                }
            }

            _initializing = false;
        }

        private async Task ExecuteAddPlanAsync()
        {
            if (SelectedSprint == null)
            {
                await _window.ShowMessageAsync("Cannot create plan!", "Please select a sprint.");
                return;
            }

            if (SelectedTeamItem == null)
            {
                await _window.ShowMessageAsync("Cannot create plan!", "Please select a team.");
                return;
            }

            if (string.IsNullOrWhiteSpace(PlanName))
            {
                await _window.ShowMessageAsync("Cannot create plan!", "Please input a plan name.");
                return;
            }
            
            var plan = new SprintPlan()
            {
                PlanName = PlanName,
                Sprint = SelectedSprint,
                ServerId = SelectedServer.Server.Id,
                TeamAvailability = SelectedTeamItem.GetModel()
            };

            try
            {
                string serialized = JsonConvert.SerializeObject(plan, Formatting.Indented);
                var filePath = Path.Combine(PathsHelper.GetPlansFolder(), $"{PlanName}.spl.json");

                MessageDialogResult overwrite = MessageDialogResult.Negative;
                if (File.Exists(filePath))
                {
                    overwrite = await _window.ShowMessageAsync("Add plan", $"{Path.GetFileName(filePath)} already exists. Overwrite?", MessageDialogStyle.AffirmativeAndNegative);
                    if (overwrite == MessageDialogResult.Negative)
                    {
                        return;
                    }
                }

                File.WriteAllText(filePath, serialized);

                if (overwrite == MessageDialogResult.Negative)
                {
                    SprintPlanItems.Add(new SprintPlanItem { FullPath = filePath, Name = PlanName });
                }
            }
            catch (Exception ex)
            {
                await _window.ShowMessageAsync("Cannot create plan", ex.Message);
            }
        }

        private void ExecuteOpenSprintPlan(SprintPlanItem planItem)
        {
            try
            {
                var plan = Business.Plan = JsonConvert.DeserializeObject<SprintPlan>(File.ReadAllText(planItem.FullPath));
                var server = Business.AppData.ServerModel.Servers.First(s => s.Id == plan.ServerId);

                bool loggedIn = Business.Jira.Login(server.Url, server.UserName, SecureHelper.ToSecure(server.Pass));
                if (!loggedIn)
                {
                    _window.ShowMessageAsync("Login unavailable or expired", "Please login from the servers page");
                    return;
                }

                _goToPlanningCommand.Execute(null);
            }
            catch (Exception ex)
            {
                _window.ShowMessageAsync(ex.Message, ex.StackTrace);
            }
        }

        private void SprintCrudViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                switch (e.PropertyName)
                {
                    case nameof(SelectedServer):
                        if (SelectedServer != null)
                        {
                            bool loggedIn = false;
                            try
                            {
                                loggedIn = Business.Jira.Login(SelectedServer.Server.Url, SelectedServer.Server.UserName, SecureHelper.ToSecure(SelectedServer.Server.Pass));
                            }
                            catch
                            {
                                // Ignore
                            }

                            if (!loggedIn)
                            {
                                Business.Jira.Logout();
                                _window.ShowMessageAsync("Login unavailable or expired", "Please login from the servers page");
                            }
                        }
                        break;

                    case nameof(SelectedSprint):
                        PlanName = $"Plan for {SelectedSprint?.Name}";
                        break;
                }
            }
            catch (Exception ex)
            {
                _window.ShowMessageAsync(ex.Message, ex.StackTrace);
            }
        }

        private void LoadBoardsAndSprints()
        {
            if (Business.AppData?.SprintCrud?.Boards != null && Boards.Count == 0)
            {
                Boards = new ObservableCollection<Tuple<int, string>>(Business.AppData.SprintCrud.Boards);
            }

            if (Business.AppData?.SprintCrud?.Sprints != null)
            {
                Sprints = new ObservableCollection<CoreSprint>(Business.AppData.SprintCrud.Sprints);
            }

            try
            {
                var board = Boards.First(i => i.Item1 == Business.AppData.SprintCrud.SelectedBoard);
                if (!SelectedBoards.Contains(board))
                {
                    SelectedBoards.Add(board);
                }

                SelectedSprint = Sprints.First(i => i.Id == Business.AppData.SprintCrud.SelectedSprint?.Id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message} {ex.StackTrace}");
            }
        }

        private void LoadPlans()
        {
            var path = PathsHelper.GetPlansFolder();
            var plans = Directory.GetFiles(path, "*.spl.json");
            SprintPlanItems = new ObservableCollection<SprintPlanItem>(plans?.Select(p => new SprintPlanItem { Name = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(p)), FullPath = p }));
        }

        private void LoadTeams()
        {
            TeamItems = new ObservableCollection<TeamItem>();

            var path = PathsHelper.GetTeamsFolder();
            var teamFiles = Directory.GetFiles(path, "*.team.json");

            foreach (var tf in teamFiles)
            {
                var team = JsonConvert.DeserializeObject<Team>(File.ReadAllText(tf));
                var teamItem = new TeamItem(team)
                {
                    Name = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(tf)),
                    FullPath = tf,
                };

                TeamItems.Add(teamItem);
            }

            SelectedTeamItem = TeamItems?.FirstOrDefault();
        }

        private void LoadServers()
        {
            if (Business.AppData.ServerModel?.Servers == null)
            {
                return;
            }

            Servers = new ObservableCollection<SelectableServerItem>(Business.AppData.ServerModel.Servers.Select(s => new SelectableServerItem
            {
                DisplayName = $"{s.Name} ({s.Url})",
                Server = s
            }));

            var toSelect = Servers.FirstOrDefault(s => s.Server.Id.Equals(Business.AppData.SprintCrud.SelectedServer?.Id));
            if (toSelect == null)
            {
                return;
            }

            SelectedServer = toSelect;
        }
    }
}
