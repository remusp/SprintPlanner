using System.Windows.Controls;
using System.Windows.Input;

namespace SprintPlanner.WpfApp.UI.SprintCrud
{
    /// <summary>
    /// Interaction logic for SprintCrudView.xaml
    /// </summary>
    public partial class SprintCrudView : UserControl
    {
        public SprintCrudView()
        {
            InitializeComponent();
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is not ListBoxItem lbi)
            {
                return;
            }

            if (lbi.DataContext is not SprintPlanItem spi) 
            {
                return;
            }

            if (DataContext is not SprintCrudViewModel vm) 
            {
                return;
            }

            vm.CommandOpenSprintPlan.Execute(spi);
        }
    }
}
