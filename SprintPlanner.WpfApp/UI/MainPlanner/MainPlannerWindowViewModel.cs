using MahApps.Metro.Controls;
using Newtonsoft.Json;
using SprintPlanner.Core;
using SprintPlanner.Core.Logic;
using SprintPlanner.FrameworkWPF;
using SprintPlanner.WpfApp.UI.About;
using SprintPlanner.WpfApp.UI.Capacity;
using SprintPlanner.WpfApp.UI.Planning;
using SprintPlanner.WpfApp.UI.Servers;
using SprintPlanner.WpfApp.UI.SprintCrud;
using SprintPlanner.WpfApp.UI.Stats;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Input;

namespace SprintPlanner.WpfApp.UI.MainPlanner
{
    public class MainPlannerWindowViewModel : ViewModelBase
    {
        private const string sprintFileName = "Sprint.app.json";

        private readonly AboutViewModel _aboutViewModel;
        private readonly CapacityViewModel _capacityViewModel;
        private readonly StatsViewModel _statsViewModel;
        private readonly PlanningViewModel _planningViewModel;
        private readonly SprintCrudViewModel _sprintCrudViewModel;
        private readonly ServersViewModel _serversViewModel;

        private readonly MetroWindow _window;

        public MainPlannerWindowViewModel(MetroWindow w)
        {
            _window = w;

            CapacityViewCommand = new DelegateCommand(() => SetView(_capacityViewModel));
            StatsViewCommand = new DelegateCommand(() => SetView(_statsViewModel));
            AboutViewCommand = new DelegateCommand(() => SetView(_aboutViewModel));
            ServersViewCommand = new DelegateCommand(() => SetView(_serversViewModel));
            PlanningViewCommand = new DelegateCommand(() => SetView(_planningViewModel));
            SprintCrudViewCommand = new DelegateCommand(() => SetView(_sprintCrudViewModel));

            _planningViewModel = new PlanningViewModel(w);
            _capacityViewModel = new CapacityViewModel(w);
            _statsViewModel = new StatsViewModel();
            _sprintCrudViewModel = new SprintCrudViewModel(w, PlanningViewCommand);

            _serversViewModel = new ServersViewModel(w);
            var assembly = Assembly.GetExecutingAssembly();
            _aboutViewModel = new AboutViewModel(w)
            {
                ProductName = ((AssemblyTitleAttribute)assembly.GetCustomAttribute(typeof(AssemblyTitleAttribute))).Title,
                ProductVersion = assembly.GetName().Version.ToString()
            };
        }

        public ViewModelBase MainViewModel
        {
            get { return Get(() => MainViewModel); }
            set { Set(() => MainViewModel, value); }
        }

        public ICommand AboutViewCommand { get; }
        public ICommand CapacityViewCommand { get; }
        public ICommand StatsViewCommand { get; }
        public ICommand PlanningViewCommand { get; }
        public ICommand SprintCrudViewCommand { get; }
        public ICommand ServersViewCommand { get; }

        public void Load()
        {
            string dataFolder = PathsHelper.GetAppDataFolder();

            var sprintFilePath = Path.Combine(dataFolder, sprintFileName);

            try
            {
                if (File.Exists(sprintFilePath))
                {
                    Business.AppData = JsonConvert.DeserializeObject<DataStorageModel>(File.ReadAllText(sprintFilePath));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            Business.AppData ??= new DataStorageModel();

            if (Business.AppData.SprintCrud.SelectedServer != null && Business.AppData.SprintCrud.SelectedServer.IsLoggedIn)
            {
                SetView(_sprintCrudViewModel);
            }
            else 
            {
                SetView(_serversViewModel);
            }
        }

        public void Persist()
        {
            string serialized = JsonConvert.SerializeObject(Business.AppData, Formatting.Indented);

            var sprintFilePath = Path.Combine(PathsHelper.GetAppDataFolder(), sprintFileName);
            File.WriteAllText(sprintFilePath, serialized);
        }

        private void SetView(ViewModelBase vm)
        {
            if (MainViewModel is IStorageManipulator m)
            {
                m.PushData();
            }

            (vm as IStorageManipulator)?.PullData();
            MainViewModel = vm;
        }
    }
}