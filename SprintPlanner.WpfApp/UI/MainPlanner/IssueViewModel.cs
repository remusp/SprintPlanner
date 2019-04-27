using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SprintPlanner.WpfApp.UI.MainPlanner
{
    public class IssueViewModel : ViewModelBase
    {
        private string key;

        public string Key
        {
            get { return key; }
            set
            {
                key = value;
                RaisePropertyChanged();
            }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                RaisePropertyChanged();
            }
        }

        private string parentName;

        public string ParentName
        {
            get { return parentName; }
            set
            {
                parentName = value;
                RaisePropertyChanged();
            }
        }


    }
}
