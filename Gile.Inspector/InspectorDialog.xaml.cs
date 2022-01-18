
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
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = (InspectableItem)e.NewValue;
            if (!item.ObjectId.IsNull)
                viewModel.SetObjectIdProperties(item.ObjectId);
            else if (item.DynamicProperty != null)
                viewModel.SetProperties(item.DynamicProperty);
            else if (item.ResultBuffer != null)
                viewModel.SetResultBufferProperties(item.ResultBuffer);
            else if (item.PolylineVertex != null)
                viewModel.SetProperties(item.PolylineVertex);
            else if (item.Points != null)
                viewModel.SetPoint3dCollectionProperties(item.Points);
            else if (item.Doubles != null)
                viewModel.SetDoubleCollectionProperties(item.Doubles);
        }
        #endregion
    }
}
