
using System.Windows;

namespace Gile.AutoCAD.Inspector
{
    public partial class InspectorDialog : Window
    {
        InspectorViewModel viewModel;

        #region Constructors
        public InspectorDialog(InspectorViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            DataContext = viewModel;
        }
        #endregion

        #region Event handlers
        private void OK_Click(object sender, RoutedEventArgs e) => DialogResult = true;

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) => 
            viewModel.SetProperties(e.NewValue);
        #endregion
    }
}
