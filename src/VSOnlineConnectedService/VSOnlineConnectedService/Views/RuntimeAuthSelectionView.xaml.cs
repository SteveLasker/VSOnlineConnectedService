using System.Windows;
using System.Windows.Controls;
using VSOnlineConnectedService.ViewModels;

namespace VSOnlineConnectedService.Views
{
    /// <summary>
    /// Interaction logic for RuntimeAuthSelectionView.xaml
    /// </summary>
    public partial class RuntimeAuthSelectionView : UserControl
    {
        public RuntimeAuthSelectionView()
        {
            InitializeComponent();
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;
            if (button.IsChecked ?? true)
            {
                ((RuntimeAuthSelectionPageViewModel)this.DataContext).RuntimeAuthOptions = RuntimeAuthOptions.IntegratedAuth;
            }
            else
            {
                ((RuntimeAuthSelectionPageViewModel)this.DataContext).RuntimeAuthOptions = RuntimeAuthOptions.None;
            }
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;
            if (button.IsChecked ?? true)
            {
                ((RuntimeAuthSelectionPageViewModel)this.DataContext).RuntimeAuthOptions=RuntimeAuthOptions.BasicAuth;
            }
            else
            {
                ((RuntimeAuthSelectionPageViewModel)this.DataContext).RuntimeAuthOptions=RuntimeAuthOptions.None;
            }
        }
    }

}
