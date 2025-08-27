using LayoutEditor.Common;
using LayoutEditor.Common.Windows;
using LayoutEditor.WinForms;
using LayoutEditor.WinForms.Controls;
using LayoutEditor.WinForms.Forms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.ThemeVS2015;
using WeifenLuo.WinFormsUI.Docking;

namespace LayoutEditor.WinForms
{
    public partial class MainForm : Form
    {
        // Keep only business logic fields in MainForm.cs, not UI controls
        private CharacterUiProfile? _profile;
        private bool _loatLastFileOnStartup;
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

        private PropertyWindow? _propertyWindow = new PropertyWindow();
        private ViewportWindow? _viewportWindow = new ViewportWindow();

        public MainForm()
        {
            InitializeComponent();

            InitializeDockPanel();
            PopulateResolutions();
            LoadSettings();
            UpdateRecentFilesMenu();
            AutoLoadLastFile();
            // Add Load event handler to handle layout after form is fully initialized
            this.Load += MainForm_Load;
        }

        private void InitializeDockPanel()
        {
            _propertyWindow?.Show(dockPanel1, DockState.DockRight);
            _viewportWindow?.Show(dockPanel1, DockState.Document);
            _viewportWindow!.SelectionChanged += Viewport_SelectionChanged;
        }

        private void AutoLoadLastFile()
        {
            if (!loadLastFileOnStartupToolStripMenuItem.Checked)
                return;
            if (_recentFiles.Count > 0 && File.Exists(_recentFiles[0]))
                OpenProfileFile(_recentFiles[0]);
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

        private void LoadSettings()
        {
            _recentFiles.Clear();

            try
            {
                var settings = Properties.Settings.Default;
                loadLastFileOnStartupToolStripMenuItem.Checked = settings.LoadLastFileOnStartup;
                // If we have saved recent files, add them to our list
                if (settings.RecentFiles != null)
                {
                    foreach (string filePath in settings.RecentFiles)
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

        private void SaveSettings()
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
                settings.LoadLastFileOnStartup = loadLastFileOnStartupToolStripMenuItem.Checked;
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
            SaveSettings();
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
                SaveSettings();

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
            SaveSettings();
        }

        #endregion

        private void Viewport_SelectionChanged(object? sender, UiWindowBase? window)
        {
            if (_propertyWindow != null)
            {
                _propertyWindow.SelectedObject = window;
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
                Filter = "UI INI Files|UI_*.proj.ini|All Files|*.*",
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
                WarnIfNotQuarmIni(filePath);
                Cursor = Cursors.WaitCursor;
                _profile = CharacterUiProfile.LoadFromFile(filePath);

                if (_viewportWindow != null)
                {
                    _viewportWindow.Profile = _profile;
                    _viewportWindow.LoadedFilePath = filePath;
                }

                Text = $"Quarm Character UI Profile Editor - {Path.GetFileName(filePath)}";

                // Add to recent files list
                AddToRecentFiles(filePath);
            }
            catch (OperationCanceledException)
            {
                // User canceled loading non-standard file, do nothing
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

        private void WarnIfNotQuarmIni(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            if (!fileName.StartsWith("UI_") || !fileName.EndsWith("_pq.proj.ini"))
            {
                var result = MessageBox.Show(
                    "The selected file was not created for EQ client version used by Project Quarm or is not a UI settings file.\n\nSaving any changes to this file may corrupt the file! \n\nProceed with loading file?",
                    "Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                {
                    throw new OperationCanceledException("User canceled loading non-standard file.");
                }
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

            //SaveProfileAs_Click(sender, e);
            SaveLoadedProfile();
        }

        private void SaveLoadedProfile()
        {
            try
            {
                var filePath = _viewportWindow?.LoadedFilePath;
                if (string.IsNullOrEmpty(filePath))
                {
                    SaveProfileAs_Click(this, new EventArgs());
                }
                else
                {
                    Cursor = Cursors.WaitCursor;
                    _profile?.SaveToFile(filePath);
                    Text = $"Quarm Character UI Profile Editor - {Path.GetFileName(filePath)}";
                    // Add to recent files list
                    AddToRecentFiles(filePath);
                }
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
                _viewportWindow != null)
            {
                _viewportWindow.TargetResolution = size;
            }
        }

        private void MaintainAspectRatio_Click(object? sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem && _viewportWindow != null)
            {
                _viewportWindow.MaintainAspectRatio = menuItem.Checked;
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

        private void loadLastFileOnStartupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _loatLastFileOnStartup = !_loatLastFileOnStartup;
        }

        private void copyProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                var thisCharacter = GetLoadedProfileCharacterName();
                var characters = GetAllCharactersFromLoadedFilePath().Where(c => !c.Equals(thisCharacter)).ToList();
                var frm = new Forms.CopyProfileForm(characters);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    var selectedCharacters = frm.Characters;
                    if (selectedCharacters.Count == 0)
                    {
                        MessageBox.Show("No characters selected.", "Info",
                                       MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (_profile == null)
                    {
                        MessageBox.Show("No profile is loaded.", "Error",
                                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    try
                    {
                        Cursor = Cursors.WaitCursor;
                        foreach (var character in selectedCharacters)
                        {
                            var filePath = _viewportWindow?.LoadedFilePath;
                            if (string.IsNullOrEmpty(filePath))
                                continue;
                            var directory = Path.GetDirectoryName(filePath);
                            if (directory == null || !Directory.Exists(directory))
                                continue;
                            var newFilePath = Path.Combine(directory, $"UI_{character}_pq.proj.ini");
                            _profile.SaveToFile(newFilePath);
                        }
                        MessageBox.Show($"Profile copied to {selectedCharacters.Count} characters.", "Success",
                                       MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error copying profile: {ex.Message}", "Error",
                                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetLoadedProfileCharacterName()
        {
            try
            {
                return Path.GetFileName(_viewportWindow?.LoadedFilePath)?.Replace("UI_", "").Replace("_pq.proj.ini", "");
            }
            catch
            {
                throw new InvalidOperationException("No profile is loaded.");
            }
        }
        private List<string> GetAllCharactersFromLoadedFilePath()
        {
            var filePath = _viewportWindow?.LoadedFilePath;
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("No profile is loaded.", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<string>();
            }
            var directory = Path.GetDirectoryName(filePath);
            if (directory == null || !Directory.Exists(directory))
            {
                MessageBox.Show("Profile directory does not exist.", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<string>();
            }
            var files = Directory.GetFiles(directory, "UI_*_pq.proj.ini");
            return files.Select(f => Path.GetFileName(f)?.Replace("UI_", "").Replace("_pq.proj.ini", ""))
                                  .Where(name => !string.IsNullOrEmpty(name)
                                    && !name.Contains("'s Corpse", StringComparison.OrdinalIgnoreCase)
                                  )
                                  .ToList()!;
        }
    }
}
