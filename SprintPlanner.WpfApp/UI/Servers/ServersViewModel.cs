using MahApps.Metro.Controls;
using SprintPlanner.Core.BusinessModel;
using SprintPlanner.Core.Logic;
using SprintPlanner.FrameworkWPF;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SprintPlanner.WpfApp.UI.Servers
{
    public class ServersViewModel : ViewModelBase, IStorageManipulator
    {
        private readonly MetroWindow _window;

        public ServersViewModel(MetroWindow w)
        {
            _window = w;

            Servers = new ObservableCollection<ServerItemViewModel>();
            CommandAddNewServer = new DelegateCommand(ExecuteAddNewServer);
        }

        public ObservableCollection<ServerItemViewModel> Servers
        {
            get { return Get(() => Servers); }
            set { Set(() => Servers, value); }
        }

        public string NewServerName
        {
            get { return Get(() => NewServerName); }
            set { Set(() => NewServerName, value); }
        }

        public string NewServerUrl
        {
            get { return Get(() => NewServerUrl); }
            set { Set(() => NewServerUrl, value); }
        }

        public string NewServerStoryPointsField
        {
            get { return Get(() => NewServerStoryPointsField); }
            set { Set(() => NewServerStoryPointsField, value); }
        }

        public ICommand CommandAddNewServer { get; }

        public void PushData()
        {
            Business.AppData.ServerModel.Servers = Servers.Select(s => s.GetModel()).ToList();
        }

        public void PullData()
        {
            if (Business.AppData.ServerModel?.Servers == null)
            {
                return;
            }

            Servers = new ObservableCollection<ServerItemViewModel>(Business.AppData.ServerModel.Servers.Select(s => new ServerItemViewModel(s, _window)));
        }

        private void ExecuteAddNewServer()
        {
            Servers.Add(new ServerItemViewModel(new Server
            {
                Id = Guid.NewGuid(),
                Name = NewServerName,
                Url = NewServerUrl,
                StoryPointsField = NewServerStoryPointsField,
                StoreCredentials = true
            }, _window));
        }
    }
}
