using System.Windows;

namespace Gile.AutoCAD.Inspector
{
    public partial class InspectorDialog : Window
    {
        InspectorViewModel viewModel;

        public InspectorDialog(InspectorViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            DataContext = viewModel;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) => 
            viewModel.SetProperties(e.NewValue);
    }
}
