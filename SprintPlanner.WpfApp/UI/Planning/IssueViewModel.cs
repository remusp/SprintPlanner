using SprintPlanner.Core.Logic;
using System.Collections.Generic;

namespace SprintPlanner.WpfApp.UI.Planning
{
    public class IssueViewModel : WrappingViewModel<IssueModel>
    {
        public IssueViewModel(IssueModel model) : base(model)
        {
        }

        public decimal Hours
        {
            get { return _model.Hours; }
            set
            {
                _model.Hours = value;
                RaisePropertyChanged();
            }
        }

        public string Name
        {
            get { return _model.Name; }
            set
            {
                _model.Name = value;
                RaisePropertyChanged();
            }
        }

        public string ParentName
        {
            get { return _model.ParentName; }
            set
            {
                _model.ParentName = value;
                RaisePropertyChanged();
            }
        }

        public string StoryId
        {
            get { return _model.StoryId; }
            set
            {
                _model.StoryId = value;
                RaisePropertyChanged();
            }
        }

        public string StoryLink
        {
            get { return _model.StoryLink; }
            set
            {
                _model.StoryLink = value;
                RaisePropertyChanged();
            }
        }

        public string TaskId
        {
            get { return _model.TaskId; }
            set
            {
                _model.TaskId = value;
                RaisePropertyChanged();
            }
        }

        public string TaskLink
        {
            get { return _model.TaskLink; }
            set
            {
                _model.TaskLink = value;
                RaisePropertyChanged();
            }
        }

        private List<Assignation> _assignables;

        public List<Assignation> Assignables
        {
            get => _assignables;
            set => Set(ref _assignables, value);
        }

        public string Id
        {
            get => $"{StoryId}+{TaskId}";
        }

    }
}