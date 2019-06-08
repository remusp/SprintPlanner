using GalaSoft.MvvmLight;

namespace SprintPlanner.WpfApp.UI.About
{
    public class AboutViewModel : ViewModelBase
    {
        private string _productName;

        public string ProductName
        {
            get { return _productName; }
            set
            {
                _productName = value;
                RaisePropertyChanged();
            }
        }

        private string _productVersion;

        public string ProductVersion
        {
            get { return _productVersion; }
            set
            {
                _productVersion = value;
                RaisePropertyChanged();
            }
        }


    }
}
