using SprintPlanner.FrameworkWPF;

namespace SprintPlanner.WpfApp
{
    public class WrappingViewModel<T> : ViewModelBase
    {
        protected readonly T _model;

        public WrappingViewModel(T model)
        {
            _model = model;
        }

        public virtual T GetModel()
        {
            return _model;
        }
    }
}
