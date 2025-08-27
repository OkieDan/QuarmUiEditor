using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using LayoutEditor.WinForms.Controls;
using LayoutEditor.Common.Windows;
using LayoutEditor.Common;
using System;
using System.Drawing;

namespace LayoutEditor.WinForms.Forms
{
    public class ViewportWindow : DockContent
    {

        // Event to forward the selection changed event
        public event EventHandler<UiWindowBase?>? SelectionChanged;

        public ViewportWindow()
        {
            Text = "Viewport";
            Viewport = new UiViewport
            {
                Dock = DockStyle.Fill
            };
            
            InitializeComponents();
        }
        
        private void InitializeComponents()
        {
            // Forward the selection changed event
            Viewport.SelectionChanged += (s, w) => SelectionChanged?.Invoke(s, w);
            
            Controls.Add(Viewport);
        }
        
        // Expose only the properties that are accessed from MainForm
        public CharacterUiProfile? Profile
        {
            get => Viewport.Profile;
            set => Viewport.Profile = value;
        }
        
        public string? LoadedFilePath
        {
            get => Viewport.LoadedFilePath;
            set => Viewport.LoadedFilePath = value;
        }
        
        public Size TargetResolution
        {
            get => Viewport.TargetResolution;
            set => Viewport.TargetResolution = value;
        }
        
        public bool MaintainAspectRatio
        {
            get => Viewport.MaintainAspectRatio;
            set => Viewport.MaintainAspectRatio = value;
        }

        // Direct access to the viewport if needed
        public UiViewport Viewport { get; private set; }
    }
}