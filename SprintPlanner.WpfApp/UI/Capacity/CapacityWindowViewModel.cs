using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using SprintPlanner.CoreFramework;
using SprintPlanner.WpfApp.Properties;
using System;
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

                string data = JsonConvert.SerializeObject(cm);
                File.WriteAllText("CapacityData.json", data);
            }
        }

        private int daysInSprint;

        public int DaysInSprint
        {
            get { return daysInSprint; }
            set
            {
                daysInSprint = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<UserDetails> users;

        public ObservableCollection<UserDetails> Users
        {
            get { return users; }
            set
            {
                users = value;
                RaisePropertyChanged();
            }
        }

        private ICommand refreshCommand;

        public ICommand RefreshCommand
        {
            get
            {
                if (refreshCommand == null)
                {
                    refreshCommand = new RelayCommand(RefreshCommandExecute);
                }

                return refreshCommand;
            }
        }

        private bool isBusy;

        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                RaisePropertyChanged();
            }
        }


        private string busyReason;

        public string BusyReason
        {
            get { return busyReason; }
            set
            {
                busyReason = value;
                RaisePropertyChanged();
            }
        }


        private void RefreshCommandExecute()
        {
            IsBusy = true;
            BusyReason = "Fetching users...";
            Users.Clear();
            Task<List<string>>.Factory.StartNew(() =>
            {
                var boardId = 1137;//147;
                var sprintId = 6182;//349;
                return Business.Jira.GetAllAssigneesInSprint(boardId, sprintId);
            }).ContinueWith((t) =>
            {
                try
                {
                    foreach (var item in t.Result)
                    {
                        Users.Add(new UserDetails
                        {
                            UserName = item
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
