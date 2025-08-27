using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace LayoutEditor.WinForms.Forms
{
    public class PropertyWindow : DockContent
    {
        private PropertyGrid _propertyGrid;

        public PropertyWindow()
        {
            Text = "Properties";
            _propertyGrid = new PropertyGrid
            {
                Dock = DockStyle.Fill,
                PropertySort = PropertySort.Categorized,
                ToolbarVisible = true
            };
            
            InitializeComponents();
        }
        
        private void InitializeComponents()
        {
            Controls.Add(_propertyGrid);
        }
        
        // Only need to expose the functionality that MainForm uses
        public void SetSelectedObject(object? obj)
        {
            _propertyGrid.SelectedObject = obj;
        }
        public object SelectedObject
        {
            get => _propertyGrid.SelectedObject;
            set { _propertyGrid.SelectedObject = value; }
        }
    }
}