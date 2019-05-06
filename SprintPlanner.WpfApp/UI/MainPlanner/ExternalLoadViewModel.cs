using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SprintPlanner.WpfApp.UI.MainPlanner
{
    public class ExternalLoadViewModel : ViewModelBase
    {
        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                RaisePropertyChanged();
            }
        }

        private decimal _hours;

        public decimal Hours
        {
            get { return _hours; }
            set
            {
                _hours = value;
                RaisePropertyChanged();
            }
        }

        private string _issueKey;

        public string IssueKey
        {
            get { return _issueKey; }
            set { _issueKey = value; }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _parentKey;

        public string ParentKey
        {
            get { return _parentKey; }
            set { _parentKey = value; }
        }

        private string _parentName;

        public string ParentName
        {
            get { return _parentName; }
            set { _parentName = value; }
        }




    }
}
