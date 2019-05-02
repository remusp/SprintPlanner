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
        private string _key;

        public string Key
        {
            get { return _key; }
            set
            {
                _key = value;
                RaisePropertyChanged();
            }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged();
            }
        }

        private string _parentName;

        public string ParentName
        {
            get { return _parentName; }
            set
            {
                _parentName = value;
                RaisePropertyChanged();
            }
        }

        private decimal _hours;

        public decimal Hours
        {
            get { return _hours; }
            set { _hours = value; }
        }



    }
}
