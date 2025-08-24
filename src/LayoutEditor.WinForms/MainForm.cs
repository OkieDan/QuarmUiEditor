using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using LayoutEditor.Common;
using LayoutEditor.Common.Windows; // Added missing using directive
using LayoutEditor.WinForms.Controls;

namespace LayoutEditor.WinForms
{
    public partial class MainForm : Form
    {
        private CharacterUiProfile? _profile; // Made nullable
        private UiViewport? _viewport; // Made nullable
        private ToolStripStatusLabel? _statusLabel; // Made nullable
        private PropertyGrid? _propertyGrid; // Made nullable
        private readonly Dictionary<string, Size> _commonResolutions = new()
        {
            { "HD (1280x720)", new Size(1280, 720) },
            { "Full HD (1920x1080)", new Size(1920, 1080) },
            { "WQHD (2560x1440)", new Size(2560, 1440) },
            { "4K UHD (3840x2160)", new Size(3840, 2160) },
            { "1600x900", new Size(1600, 900) },
            { "1366x768", new Size(1366, 768) },
            { "3440x1440 (Ultrawide)", new Size(3440, 1440) },
            { "2560x1080 (Ultrawide)", new Size(2560, 1080) }
        };

        public MainForm()
        {
            InitializeComponent();
            ConfigureUI();
        }

        private void ConfigureUI()
        {
            // Set form properties
            Text = "Quarm Character UI Profile Editor";
            MinimumSize = new Size(800, 600);
            
            // Menu
            var mainMenu = new MenuStrip
            {
                Dock = DockStyle.Top
            };
            
            var fileMenu = new ToolStripMenuItem("&File");
            fileMenu.DropDownItems.Add(new ToolStripMenuItem("&Open...", null, OpenProfile_Click, Keys.Control | Keys.O));
            fileMenu.DropDownItems.Add(new ToolStripMenuItem("&Save", null, SaveProfile_Click, Keys.Control | Keys.S));
            fileMenu.DropDownItems.Add(new ToolStripMenuItem("Save &As...", null, SaveProfileAs_Click));
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add(new ToolStripMenuItem("E&xit", null, (s, e) => Close(), Keys.Alt | Keys.F4));
            
            var viewMenu = new ToolStripMenuItem("&View");
            viewMenu.DropDownItems.Add(new ToolStripMenuItem("&Maintain Aspect Ratio", null, MaintainAspectRatio_Click) { Checked = true, CheckOnClick = true });
            
            var helpMenu = new ToolStripMenuItem("&Help");
            helpMenu.DropDownItems.Add(new ToolStripMenuItem("&About", null, (s, e) => MessageBox.Show(
                "Quarm Character UI Profile Editor\n\nA tool for editing EQ UI layouts", 
                "About", MessageBoxButtons.OK, MessageBoxIcon.Information)));
            
            mainMenu.Items.Add(fileMenu);
            mainMenu.Items.Add(viewMenu);
            mainMenu.Items.Add(helpMenu);
            
            Controls.Add(mainMenu);
            MainMenuStrip = mainMenu;
            
            // Toolbar
            var toolbar = new ToolStrip
            {
                Dock = DockStyle.Top,
                GripStyle = ToolStripGripStyle.Hidden,
                RenderMode = ToolStripRenderMode.System
            };
            
            // Resolution label
            toolbar.Items.Add(new ToolStripLabel("Resolution:"));
            
            // Resolution dropdown
            var resolutionDropdown = new ToolStripComboBox("ResolutionSelector")
            {
                Width = 180,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            
            foreach (var resolution in _commonResolutions)
            {
                resolutionDropdown.Items.Add(resolution.Key);
            }
            
            // Select WQHD by default
            resolutionDropdown.SelectedIndex = 2; 
            resolutionDropdown.SelectedIndexChanged += ResolutionDropdown_SelectedIndexChanged;
            toolbar.Items.Add(resolutionDropdown);
            
            // Spacing
            toolbar.Items.Add(new ToolStripSeparator());
            
            // Window Properties section
            toolbar.Items.Add(new ToolStripLabel("Window Properties:"));
            
            // Add toolbar to form
            Controls.Add(toolbar);
            
            // Status bar
            var statusBar = new StatusStrip();
            _statusLabel = new ToolStripStatusLabel
            {
                Text = "Ready"
            };
            statusBar.Items.Add(_statusLabel);
            Controls.Add(statusBar);
            
            // Splitter container for viewport and property panel - CHANGED TO VERTICAL
            var splitter = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical, // Changed from Horizontal to Vertical
                SplitterDistance = ClientSize.Width - 300 // Give properties panel ~300px width
            };
            
            // Create and add the viewport
            _viewport = new UiViewport
            {
                Dock = DockStyle.Fill,
                MaintainAspectRatio = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            
            splitter.Panel1.Controls.Add(_viewport);
            
            // Property grid for selected window
            var propertyPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5)
            };
            
            _propertyGrid = new PropertyGrid
            {
                Dock = DockStyle.Fill,
                PropertySort = PropertySort.Categorized,
                ToolbarVisible = false,
                HelpVisible = true
            };
            
            propertyPanel.Controls.Add(_propertyGrid);
            splitter.Panel2.Controls.Add(propertyPanel);
            
            // Make splitter distance responsive to form size
            splitter.SplitterMoved += (s, e) => {
                // Ensure viewport gets updated when splitter moves
                _viewport?.Invalidate();
            };
            
            Controls.Add(splitter);
            
            // Set initial resolution
            if (resolutionDropdown.SelectedItem is string selectedResolution && 
                _commonResolutions.TryGetValue(selectedResolution, out var size))
            {
                _viewport.TargetResolution = size;
            }
            
            // Add event handler for window selection change
            _viewport.SelectionChanged += Viewport_SelectionChanged;
            
            // Make sure viewport is visible after resizing
            this.Resize += (s, e) => {
                _viewport?.Invalidate();
            };
        }

        // Fixed signature to match EventHandler<UiWindowBase?>
        private void Viewport_SelectionChanged(object? sender, UiWindowBase? window)
        {
            if (_propertyGrid != null)
            {
                _propertyGrid.SelectedObject = window;
            }

            if (_statusLabel != null)
            {
                _statusLabel.Text = window != null 
                    ? $"Selected: {window.Name}" 
                    : "No window selected";
            }
        }

        // Fixed signature for nullability
        private void OpenProfile_Click(object? sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Filter = "INI Files|*.ini;*.proj.ini|All Files|*.*",
                Title = "Open EverQuest UI Profile"
            };
            
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    _profile = CharacterUiProfile.LoadFromFile(openFileDialog.FileName);
                    
                    if (_viewport != null)
                    {
                        _viewport.Profile = _profile;
                    }
                    
                    Text = $"Quarm Character UI Profile Editor - {Path.GetFileName(openFileDialog.FileName)}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading profile: {ex.Message}", "Error", 
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        // Fixed signature for nullability
        private void SaveProfile_Click(object? sender, EventArgs e)
        {
            if (_profile == null)
            {
                MessageBox.Show("No profile is loaded.", "Error", 
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            // For now, we'll just use Save As functionality
            SaveProfileAs_Click(sender, e);
        }

        // Fixed signature for nullability
        private void SaveProfileAs_Click(object? sender, EventArgs e)
        {
            if (_profile == null)
            {
                MessageBox.Show("No profile is loaded.", "Error", 
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            using var saveFileDialog = new SaveFileDialog
            {
                Filter = "INI Files|*.ini|Project INI Files|*.proj.ini|All Files|*.*",
                Title = "Save EverQuest UI Profile",
                OverwritePrompt = true
            };
            
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    _profile.SaveToFile(saveFileDialog.FileName);
                    Text = $"Quarm Character UI Profile Editor - {Path.GetFileName(saveFileDialog.FileName)}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving profile: {ex.Message}", "Error", 
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        // Fixed signature for nullability
        private void ResolutionDropdown_SelectedIndexChanged(object? sender, EventArgs e)
        {
            var dropdown = sender as ToolStripComboBox;
            if (dropdown?.SelectedItem is string selectedResolution && 
                _commonResolutions.TryGetValue(selectedResolution, out var size) &&
                _viewport != null)
            {
                _viewport.TargetResolution = size;
            }
        }

        // Fixed signature for nullability
        private void MaintainAspectRatio_Click(object? sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem && _viewport != null)
            {
                _viewport.MaintainAspectRatio = menuItem.Checked;
            }
        }
    }
}
