using System.Windows;

namespace Gile.AutoCAD.R19.Inspector
{
    /// <summary>
    /// Code behind.
    /// </summary>
    public partial class InspectorDialog : Window
    {
        readonly InspectorViewModel viewModel;

        /// <summary>
        /// Creates a new instance of InspectorDialog.
        /// </summary>
        /// <param name="viewModel">Instance of InspectorViewModel.</param>
        public InspectorDialog(InspectorViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            DataContext = viewModel;
            Closing += viewModel.OnWindowClosing;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) =>
            viewModel.SetProperties(e.NewValue);
    }
}
