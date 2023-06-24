using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using SprintPlanner.Core.BusinessModel;
using SprintPlanner.Core.Logic;
using SprintPlanner.FrameworkWPF;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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

            FetchBoardsCommand = new DelegateCommand(ExecuteFetchBoards);
            FetchSprintsCommand = new DelegateCommand(ExecuteFetchSprints);
            SelectedBoardChangedCommand = new DelegateCommand(ExecuteSelectedBoardChanged);
            AddPlanCommand = new DelegateCommand(async () => await ExecuteAddPlanAsync());
            OpenSprintPlanCommand = new DelegateCommand<SprintPlanItem>(ExecuteOpenSprintPlan);

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

        public ObservableCollection<SprintPlanItem> SprintPlanItems
        {
            get { return Get(() => SprintPlanItems); }
            set { Set(() => SprintPlanItems, value); }
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

        public ICommand FetchBoardsCommand { get; }

        public ICommand FetchSprintsCommand { get; }

        public ICommand SelectedBoardChangedCommand { get; }
        public ICommand AddPlanCommand { get; }
        public ICommand OpenSprintPlanCommand { get; }

        public void Pull()
        {
            _initializing = true;
            if (Business.Data?.Sprint?.Boards != null && Boards.Count == 0)
            {
                Boards = new ObservableCollection<Tuple<int, string>>(Business.Data.Sprint.Boards);
            }

            if (Business.Data?.Sprint?.Sprints != null)
            {
                Sprints = new ObservableCollection<CoreSprint>(Business.Data.Sprint.Sprints);
            }

            try
            {
                var board = Boards.First(i => i.Item1 == Business.Data.Sprint.SelectedBoard);
                if (!SelectedBoards.Contains(board))
                {
                    SelectedBoards.Add(board);
                }

                SelectedSprint = Sprints.First(i => i.Id == Business.Data.Sprint.SelectedSprint?.Id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message} {ex.StackTrace}");
            }

            LoadPlans();

            _initializing = false;
        }

        public void Flush()
        {
            Business.Data.Sprint.Boards = Boards?.ToList();
            Business.Data.Sprint.SelectedBoard = SelectedBoards.FirstOrDefault()?.Item1 ?? 0;
            Business.Data.Sprint.Sprints = Sprints?.ToList();
            Business.Data.Sprint.SelectedSprint = SelectedSprint;
        }

        private void ExecuteFetchBoards()
        {
            IsBusy = true;
            Boards.Clear();
            Task.Factory.StartNew(() => Business.Jira.GetBoards()).ContinueWith(t =>
            {
                t.Result.Select(b => new Tuple<int, string>(b.Key, b.Value)).ToList().ForEach(i => Boards.Add(i));
                IsBusy = false;
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
                        Business.Data.Sprint.SelectedBoard = board;
                        if (!t.IsFaulted)
                        {
                            Sprints = new ObservableCollection<CoreSprint>(t.Result);
                            var sprint = Sprints.FirstOrDefault(i => i.Id == Business.Data.Sprint.SelectedSprint?.Id);
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
                                Sprints = new ObservableCollection<CoreSprint>(t.Result);
                                var sprint = Sprints.FirstOrDefault(i => i.Id == Business.Data.Sprint.SelectedSprint?.Id);
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
                return;
            }

            var plan = new SprintPlan()
            {
                Sprint = SelectedSprint
            };

            string serialized = JsonConvert.SerializeObject(plan, Formatting.Indented);
            var filePath = Path.Combine(GetPlansFolder(), $"{PlanName}.spl.json");

            if (File.Exists(filePath))
            {
                var result = await _window.ShowMessageAsync("Add plan", $"{Path.GetFileName(filePath)} already exists. Overwrite?", MessageDialogStyle.AffirmativeAndNegative);
                if (result == MessageDialogResult.Negative)
                {
                    return;
                }
            }

            File.WriteAllText(filePath, serialized);

            SprintPlanItems.Add(new SprintPlanItem { FullPath = filePath, Name = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(filePath)) });
        }

        private void ExecuteOpenSprintPlan(SprintPlanItem plan) 
        {
            Business.Plan = JsonConvert.DeserializeObject<SprintPlan>(File.ReadAllText(plan.FullPath));
            _goToPlanningCommand.Execute(null);
        }

        private string GetPlansFolder()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var appName = ((AssemblyTitleAttribute)assembly.GetCustomAttribute(typeof(AssemblyTitleAttribute))).Title;

            string plansFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), appName, "SprintPlans");
            Directory.CreateDirectory(plansFolder);
            return plansFolder;
        }

        private void SprintCrudViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(SelectedSprint)))
            {
                PlanName = $"Plan for {SelectedSprint?.Name}";
            }
        }

        private void LoadPlans()
        {
            var path = GetPlansFolder();
            var plans = Directory.GetFiles(path, "*.spl.json");
            SprintPlanItems = new ObservableCollection<SprintPlanItem>(plans?.Select(p => new SprintPlanItem { Name = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(p)), FullPath = p }));
        }
    }
}
