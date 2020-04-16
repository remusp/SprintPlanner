using SprintPlanner.Core.Logic;
using System.Collections.Generic;

namespace SprintPlanner.WpfApp.UI.Planning
{
    public class IssueViewModel : WrappingViewModel<IssueModel>
    {
        public IssueViewModel(IssueModel model) : base(model)
        {
        }

        public List<Assignation> Assignables
        {
            get { return Get(() => Assignables); }
            set { Set(() => Assignables, value); }
        }

        public decimal Hours
        {
            get { return _model.Hours; }
            set { SetBackingField(() => _model.Hours = value); }
        }

        public string Id
        {
            get => $"{StoryId}+{TaskId}";
        }

        public string Name
        {
            get { return _model.Name; }
            set { SetBackingField(() => _model.Name = value); }
        }

        public string ParentName
        {
            get { return _model.ParentName; }
            set { SetBackingField(() => _model.ParentName = value); }
        }

        public string StoryId
        {
            get { return _model.StoryId; }
            set { SetBackingField(() => _model.StoryId = value); }
        }

        public string StoryLink
        {
            get { return _model.StoryLink; }
            set { SetBackingField(() => _model.StoryLink = value); }
        }

        public string TaskId
        {
            get { return _model.TaskId; }
            set { SetBackingField(() => _model.TaskId = value); }
        }

        public string TaskLink
        {
            get { return _model.TaskLink; }
            set { SetBackingField(() => _model.TaskLink = value); }
        }
    }
}