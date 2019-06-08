using GalaSoft.MvvmLight;

namespace SprintPlanner.WpfApp.UI.Planning
{
    public class IssueViewModel : ViewModelBase
    {
        private string _storyId;

        public string StoryId
        {
            get { return _storyId; }
            set
            {
                _storyId = value;
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
            set
            {
                _hours = value;
                RaisePropertyChanged();
            }
        }

        private string _taskId;

        public string TaskId
        {
            get { return _taskId; }
            set
            {
                _taskId = value;
                RaisePropertyChanged();
            }
        }

        private string _taskLink;

        public string TaskLink
        {
            get { return _taskLink; }
            set
            {
                _taskLink = value;
                RaisePropertyChanged();
            }
        }


        private string _storyLink;

        public string StoryLink
        {
            get { return _storyLink; }
            set
            {
                _storyLink = value;
                RaisePropertyChanged();
            }
        }

    }
}
