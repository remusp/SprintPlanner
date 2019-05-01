using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SprintPlanner.WpfApp.UI.Capacity
{
    public class CapacityWindowViewModel : ViewModelBase
    {
        public CapacityWindowViewModel()
        {
            Users = new ObservableCollection<UserDetails>();

            PropertyChanged += CapacityWindowViewModel_PropertyChanged;
        }

        public void Load()
        {
            // TODO: duplicate code: capacity load
            string fileName = "CapacityData.json";
            if (File.Exists(fileName))
            {
                var cm = JsonConvert.DeserializeObject<CapacityModel>(File.ReadAllText(fileName));
                DaysInSprint = cm.DaysInSprint;
                Users = cm.Users;
            }
        }

        private void CapacityWindowViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(DaysInSprint):
                    foreach (var u in Users)
                    {
                        u.DaysInSprint = DaysInSprint;
                    }

                    break;
            }
        }

        public void Persist()
        {
            if (Users.Count > 0)
            {
                var cm = new CapacityModel()
                {
                    DaysInSprint = DaysInSprint,
                    Users = Users
                };

                string data = JsonConvert.SerializeObject(cm, Formatting.Indented);
                File.WriteAllText("CapacityData.json", data);
            }
        }

        private int _daysInSprint;

        public int DaysInSprint
        {
            get { return _daysInSprint; }
            set
            {
                _daysInSprint = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<UserDetails> _users;

        public ObservableCollection<UserDetails> Users
        {
            get { return _users; }
            set
            {
                _users = value;
                RaisePropertyChanged();
            }
        }

        private ICommand _refreshCommand;

        public ICommand RefreshCommand
        {
            get
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new RelayCommand(RefreshCommandExecute);
                }

                return _refreshCommand;
            }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged();
            }
        }


        private string _busyReason;

        public string BusyReason
        {
            get { return _busyReason; }
            set
            {
                _busyReason = value;
                RaisePropertyChanged();
            }
        }

        public int SprintId { get;  set; }
        public int BoardId { get;  set; }

        private void RefreshCommandExecute()
        {
            IsBusy = true;
            BusyReason = "Fetching users...";
            Users.Clear();
            Task<List<string>>.Factory.StartNew(() =>
            {
                return Business.Jira.GetAllAssigneesInSprint(BoardId, SprintId);
            }).ContinueWith((t) =>
            {
                try
                {
                    foreach (var item in t.Result)
                    {
                        string name = Business.Jira.GetUserDisplayName(item);
                        Users.Add(new UserDetails
                        {
                            Uid = item,
                            UserName = name
                        });
                    }
                }
                finally
                {
                    IsBusy = false;
                }

            }, TaskScheduler.FromCurrentSynchronizationContext());

        }
    }
}
