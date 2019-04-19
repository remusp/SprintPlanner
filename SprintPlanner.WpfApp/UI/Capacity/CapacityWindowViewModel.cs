using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SprintPlanner.WpfCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                JiraHelper jh = new JiraHelper();
                jh.Login("ropop", "");
                return jh.GetAllAssigneesInSprint(1137, 6182);
            }).ContinueWith((t) =>
            {
                foreach (var item in t.Result)
                {
                    Users.Add(new UserDetails
                    {
                        UserName = item
                    });
                }

                IsBusy = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());

        }
    }
}
