using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using SprintPlanner.Core;
using SprintPlanner.Core.BusinessModel;
using SprintPlanner.Core.Logic;
using SprintPlanner.FrameworkWPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CoreSprint = SprintPlanner.Core.BusinessModel.Sprint;

namespace SprintPlanner.WpfApp.UI.TeamsCrud
{
    public class TeamsCrudViewModel : ViewModelBase, IStorageManipulator
    {
        private readonly MetroWindow _window;
        private ICommand _addUserCommand;

        public TeamsCrudViewModel(MetroWindow w)
        {
            Boards = new ObservableCollection<Tuple<int, string>>();
            SelectedBoards = new ObservableCollection<Tuple<int, string>>();
            TeamItems = new ObservableCollection<TeamItem>(); 

            CommandFetchBoards = new DelegateCommand(ExecuteFetchBoards);
            CommandAddTeam = new DelegateCommand(async () => await ExecuteAddTeamAsync());
            CommandAddFromComboSprint = new DelegateCommand(ExecuteAddFromComboSprintCommand);
            CommandSearch = new DelegateCommand(ExecuteSearchCommand);
            _addUserCommand = new DelegateCommand<SearchUserItem>(ExecuteAddUser);
            
            CapacityFactor = 1;
            _window = w;
            PropertyChanged += CapacityWindowViewModel_PropertyChanged;
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

        public string BusyReason
        {
            get { return Get(() => BusyReason); }
            set { Set(() => BusyReason, value); }
        }

        public List<CoreSprint> Sprints
        {
            get { return Get(() => Sprints); }
            set { Set(() => Sprints, value); }
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

        public string NewTeamName
        {
            get { return Get(() => NewTeamName); }
            set { Set(() => NewTeamName, value); }
        }

        public ICommand CommandFetchBoards { get; }

        public ICommand CommandAddTeam { get; }

        public ICommand CommandAddAllInCurrentSprint { get; private set; }

        public ICommand CommandAddFromComboSprint { get; private set; }

        public ICommand CommandSearch { get; private set; }

        public void PushData()
        {
            Business.AppData.Capacity.DaysInSprint = DaysInSprint;
            Business.AppData.Capacity.CapacityFactor = CapacityFactor;
            SaveTeams();
        }

        public void PullData()
        {
            LoadBoards();
            LoadTeams();

            DaysInSprint = Business.AppData.Capacity.DaysInSprint;
           

            CapacityFactor = Business.AppData.Capacity.CapacityFactor;

            if (Business.AppData?.SprintCrud?.Sprints != null)
            {
                Sprints = new List<CoreSprint>(Business.AppData.SprintCrud.Sprints);
                SelectedSprint = Sprints.First();
            }
        }

        private void CapacityWindowViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //switch (e.PropertyName)
            //{
            //    case nameof(DaysInSprint):
            //        foreach (var u in Users)
            //        {
            //            u.DaysInSprint = DaysInSprint;
            //        }

            //        break;

            //    case nameof(CapacityFactor):
            //        foreach (var u in Users)
            //        {
            //            u.CapacityFactor = CapacityFactor;
            //        }

            //        break;
            //}
        }

        private async Task ExecuteAddTeamAsync()
        {
            if (SelectedSprint == null)
            {
                await _window.ShowMessageAsync("Cannot create plan!", "No sprint selected");
                return;
            }

            if (string.IsNullOrWhiteSpace(NewTeamName))
            {
                await _window.ShowMessageAsync("Cannot create team!", "Please input a team name");
                return;
            }

            var team = new Team()
            {
                ServerId = Business.AppData.SprintCrud.SelectedServer.Id
            };

            try
            {
                string serialized = JsonConvert.SerializeObject(team, Formatting.Indented);
                var filePath = Path.Combine(PathsHelper.GetTeamsFolder(), $"{NewTeamName}.team.json");

                MessageDialogResult overwrite = MessageDialogResult.Negative;
                if (File.Exists(filePath))
                {
                    overwrite = await _window.ShowMessageAsync("Add team", $"{Path.GetFileName(filePath)} already exists. Overwrite?", MessageDialogStyle.AffirmativeAndNegative);
                    if (overwrite == MessageDialogResult.Negative)
                    {
                        return;
                    }
                }

                File.WriteAllText(filePath, serialized);

                if (overwrite == MessageDialogResult.Negative)
                {
                    TeamItems.Add(new TeamItem(team) 
                    { 
                        FullPath = filePath, 
                        Name = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(filePath)),
                    });
                }
            }
            catch (Exception ex)
            {
                await _window.ShowMessageAsync("Cannot create team", ex.Message);
            }
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

        private void ExecuteAddFromComboSprintCommand()
        {
            if (SelectedSprint == null)
            {
                _window.ShowMessageAsync("Sprint not available!", "Make sure that credentials are set for the current server or that sprints are available in the plans list screen.");
                return;
            }

            AddAssigneesFromSprint(SelectedSprint.Id);
        }

        private void ExecuteSearchCommand()
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
                    UserId = fu.emailAddress,
                    AddCommand = _addUserCommand
                }));
            }
            catch (Exception ex)
            {
                _window.ShowMessageAsync(ex.Message, ex.StackTrace);
            }
        }

        private void ExecuteAddUser(SearchUserItem user)
        {
            var existingUsers = SelectedTeamItem.TeamAvailability.Users.Select(u => u.Uid).ToList();
            if (!existingUsers.Contains(user.UserId))
            {
                SelectedTeamItem.TeamAvailability.Users.Add(new UserDetailsItem(new UserDetailsModel
                {
                    Uid = user.UserId,
                    UserName = user.Name,
                    DaysInSprint = DaysInSprint,
                    CapacityFactor = CapacityFactor
                }));
            }
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
                        var existingUsers = SelectedTeamItem.TeamAvailability.Users.Select(u => u.Uid).ToList();
                        foreach (var assignee in t.Result)
                        {
                            if (!existingUsers.Contains(assignee.displayName))
                            {
                                SelectedTeamItem.TeamAvailability.Users.Add(new UserDetailsItem(new UserDetailsModel
                                {
                                    Uid = assignee.accountId,
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
                        _window.ShowMessageAsync("Error fetching users", $"{message}{Environment.NewLine}{stackTraces}");
                    }
                }
                finally
                {
                    IsBusy = false;
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void LoadBoards()
        {
            if (Business.AppData?.SprintCrud?.Boards != null && Boards.Count == 0)
            {
                Boards = new ObservableCollection<Tuple<int, string>>(Business.AppData.SprintCrud.Boards);
            }

            try
            {
                var board = Boards.First(i => i.Item1 == Business.AppData.SprintCrud.SelectedBoard);
                if (!SelectedBoards.Contains(board))
                {
                    SelectedBoards.Add(board);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message} {ex.StackTrace}");
            }
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
        }

        private void SaveTeams()
        {
            foreach (var teamItem in TeamItems)
            {
                var team = teamItem.GetModel();
                string serializedTeam = JsonConvert.SerializeObject(team, Formatting.Indented);
                var filePath = Path.Combine(teamItem.FullPath);
                File.WriteAllText(filePath, serializedTeam);
            }
        }
    }
}