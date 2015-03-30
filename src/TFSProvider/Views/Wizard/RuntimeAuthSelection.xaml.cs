using System.Windows;
using System.Windows.Controls;
using TFSConnectedService;

namespace TFSProvider
{
    /// <summary>
    /// Interaction logic for SelectRuntimeAuth.xaml
    /// </summary>
    public partial class RuntimeAuthSelection : UserControl
    {
        public RuntimeAuthSelection(RuntimeAuthSelectionPage dataContext)
        {
            InitializeComponent();

            this.DataContext = dataContext;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;
            if (button.IsChecked ?? true)
            {
                ((RuntimeAuthSelectionPage)this.DataContext).GetAuthSelection(RuntimeAuthOptions.IntegratedAuth);
            }
            else
            {
                ((RuntimeAuthSelectionPage)this.DataContext).GetAuthSelection(RuntimeAuthOptions.None);
            }
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;
            if (button.IsChecked ?? true)
            {
                ((RuntimeAuthSelectionPage)this.DataContext).GetAuthSelection(RuntimeAuthOptions.BasicAuth);
            }
            else
            {
                ((RuntimeAuthSelectionPage)this.DataContext).GetAuthSelection(RuntimeAuthOptions.None);
            }
        }
    }
}
