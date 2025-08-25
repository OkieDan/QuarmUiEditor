using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Configuration;
using System.Linq;
using LayoutEditor.Common;
using LayoutEditor.Common.Windows;
using LayoutEditor.WinForms.Controls;
using LayoutEditor.WinForms;

namespace LayoutEditor.WinForms
{
    public partial class MainForm : Form
    {
        // Keep only business logic fields in MainForm.cs, not UI controls
        private CharacterUiProfile? _profile;
        private readonly List<string> _recentFiles = new();
        private readonly int _maxRecentFiles = 10;
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

            // Setup everything after the InitializeComponent call
            SetupEventHandlers();
            PopulateResolutions();
            LoadRecentFiles();
            UpdateRecentFilesMenu();
            propertyPanel.Controls.Add(_propertyGrid);
            contentPanel.Controls.Add(MainSplitter);

            // Add Load event handler to handle layout after form is fully initialized
            this.Load += MainForm_Load;
        }

        private void SetupEventHandlers()
        {
        }

        private void PopulateResolutions()
        {
            foreach (var resolution in _commonResolutions)
            {
                resolutionDropdown.Items.Add(resolution.Key);
            }

            // Select WQHD by default
            resolutionDropdown.SelectedIndex = 2;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        #region Recent Files Management

        private void LoadRecentFiles()
        {
            _recentFiles.Clear();

            try
            {
                var settings = Properties.Settings.Default;
                var recentFilesCollection = settings.RecentFiles;

                // If we have saved recent files, add them to our list
                if (recentFilesCollection != null)
                {
                    foreach (string filePath in recentFilesCollection)
                    {
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            _recentFiles.Add(filePath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // If loading fails, log the error but continue with an empty list
                System.Diagnostics.Debug.WriteLine($"Failed to load recent files: {ex.Message}");
            }
        }

        private void SaveRecentFiles()
        {
            try
            {
                var settings = Properties.Settings.Default;

                // Create a new StringCollection to hold our recent files
                var recentFilesCollection = new System.Collections.Specialized.StringCollection();

                // Add all recent files to the collection
                foreach (var filePath in _recentFiles)
                {
                    recentFilesCollection.Add(filePath);
                }

                // Save to settings
                settings.RecentFiles = recentFilesCollection;
                settings.Save();
            }
            catch (Exception ex)
            {
                // If saving fails, log the error but continue
                System.Diagnostics.Debug.WriteLine($"Failed to save recent files: {ex.Message}");
            }
        }

        private void AddToRecentFiles(string filePath)
        {
            // Remove file if it already exists in the list
            _recentFiles.Remove(filePath);

            // Add file at the beginning of the list
            _recentFiles.Insert(0, filePath);

            // Keep only the most recent files
            while (_recentFiles.Count > _maxRecentFiles)
            {
                _recentFiles.RemoveAt(_recentFiles.Count - 1);
            }

            // Update menu and save
            UpdateRecentFilesMenu();
            SaveRecentFiles();
        }

        private void UpdateRecentFilesMenu()
        {
            if (_recentFilesMenuItem == null)
                return;

            _recentFilesMenuItem.DropDownItems.Clear();

            if (_recentFiles.Count == 0)
            {
                var noRecentItem = new ToolStripMenuItem("No recent files")
                {
                    Enabled = false
                };
                _recentFilesMenuItem.DropDownItems.Add(noRecentItem);
                return;
            }

            // Add each file
            for (int i = 0; i < _recentFiles.Count; i++)
            {
                string filePath = _recentFiles[i];
                string displayText = $"{i + 1}. {Path.GetFileName(filePath)}";

                var menuItem = new ToolStripMenuItem(displayText)
                {
                    ToolTipText = filePath,
                    Tag = filePath
                };
                menuItem.Click += RecentFile_Click;
                _recentFilesMenuItem.DropDownItems.Add(menuItem);
            }

            _recentFilesMenuItem.DropDownItems.Add(new ToolStripSeparator());
            _recentFilesMenuItem.DropDownItems.Add(new ToolStripMenuItem("Clear Recent Files", null, ClearRecentFiles_Click));
        }

        private void RecentFile_Click(object? sender, EventArgs e)
        {
            if (sender is not ToolStripMenuItem menuItem || menuItem.Tag is not string filePath)
                return;

            if (File.Exists(filePath))
            {
                OpenProfileFile(filePath);
            }
            else
            {
                _recentFiles.Remove(filePath);
                UpdateRecentFilesMenu();
                SaveRecentFiles();

                MessageBox.Show(
                    $"The file '{Path.GetFileName(filePath)}' no longer exists and has been removed from the recent files list.",
                    "File Not Found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void ClearRecentFiles_Click(object? sender, EventArgs e)
        {
            _recentFiles.Clear();
            UpdateRecentFilesMenu();
            SaveRecentFiles();
        }

        #endregion

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

        private void OpenProfile_Click(object? sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Filter = "INI Files|*.ini;*.proj.ini|All Files|*.*",
                Title = "Open EverQuest UI Profile"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                OpenProfileFile(openFileDialog.FileName);
            }
        }

        private void OpenProfileFile(string filePath)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                _profile = CharacterUiProfile.LoadFromFile(filePath);

                if (UiViewport != null)
                {
                    UiViewport.Profile = _profile;
                }

                Text = $"Quarm Character UI Profile Editor - {Path.GetFileName(filePath)}";

                // Add to recent files list
                AddToRecentFiles(filePath);
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

                    // Add to recent files list
                    AddToRecentFiles(saveFileDialog.FileName);
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

        private void ResolutionDropdown_SelectedIndexChanged(object? sender, EventArgs e)
        {
            var dropdown = sender as ToolStripComboBox;
            if (dropdown?.SelectedItem is string selectedResolution &&
                _commonResolutions.TryGetValue(selectedResolution, out var size) &&
                UiViewport != null)
            {
                UiViewport.TargetResolution = size;
            }
        }

        private void MaintainAspectRatio_Click(object? sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem && UiViewport != null)
            {
                UiViewport.MaintainAspectRatio = menuItem.Checked;
            }
        }

        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Quarm Character UI Profile Editor\n\nA tool for editing EQ UI layouts",
                            "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
