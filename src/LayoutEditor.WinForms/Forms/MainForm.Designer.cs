using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using LayoutEditor.WinForms.Controls;

namespace LayoutEditor.WinForms
{
    partial class MainForm : Form
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            mainMenu = new MenuStrip();
            fileMenu = new ToolStripMenuItem();
            openMenuItem = new ToolStripMenuItem();
            saveMenuItem = new ToolStripMenuItem();
            saveAsMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            _recentFilesMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            exitMenuItem = new ToolStripMenuItem();
            optionsMenu = new ToolStripMenuItem();
            maintainAspectMenuItem = new ToolStripMenuItem();
            loadLastFileOnStartupToolStripMenuItem = new ToolStripMenuItem();
            helpMenu = new ToolStripMenuItem();
            aboutMenuItem = new ToolStripMenuItem();
            toolbar = new ToolStrip();
            resolutionLabel = new ToolStripLabel();
            resolutionDropdown = new ToolStripComboBox();
            toolStripSeparator3 = new ToolStripSeparator();
            windowPropertiesLabel = new ToolStripLabel();
            statusBar = new StatusStrip();
            _statusLabel = new ToolStripStatusLabel();
            contentPanel = new Panel();
            MainSplitter = new SplitContainer();
            UiViewport = new UiViewport();
            propertyPanel = new Panel();
            _propertyGrid = new PropertyGrid();
            copyProfileToolStripMenuItem = new ToolStripMenuItem();
            mainMenu.SuspendLayout();
            toolbar.SuspendLayout();
            statusBar.SuspendLayout();
            contentPanel.SuspendLayout();
            ((ISupportInitialize)MainSplitter).BeginInit();
            MainSplitter.Panel1.SuspendLayout();
            MainSplitter.Panel2.SuspendLayout();
            MainSplitter.SuspendLayout();
            propertyPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainMenu
            // 
            mainMenu.Items.AddRange(new ToolStripItem[] { fileMenu, optionsMenu, helpMenu });
            mainMenu.Location = new Point(0, 0);
            mainMenu.Name = "mainMenu";
            mainMenu.Size = new Size(1029, 24);
            mainMenu.TabIndex = 0;
            mainMenu.Text = "menuStrip1";
            // 
            // fileMenu
            // 
            fileMenu.DropDownItems.AddRange(new ToolStripItem[] { openMenuItem, saveMenuItem, saveAsMenuItem, copyProfileToolStripMenuItem, toolStripSeparator1, _recentFilesMenuItem, toolStripSeparator2, exitMenuItem });
            fileMenu.Name = "fileMenu";
            fileMenu.Size = new Size(37, 20);
            fileMenu.Text = "&File";
            // 
            // openMenuItem
            // 
            openMenuItem.Name = "openMenuItem";
            openMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            openMenuItem.Size = new Size(180, 22);
            openMenuItem.Text = "&Open...";
            openMenuItem.Click += OpenProfile_Click;
            // 
            // saveMenuItem
            // 
            saveMenuItem.Name = "saveMenuItem";
            saveMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            saveMenuItem.Size = new Size(180, 22);
            saveMenuItem.Text = "&Save";
            saveMenuItem.Click += SaveProfile_Click;
            // 
            // saveAsMenuItem
            // 
            saveAsMenuItem.Name = "saveAsMenuItem";
            saveAsMenuItem.Size = new Size(180, 22);
            saveAsMenuItem.Text = "Save &As...";
            saveAsMenuItem.Click += SaveProfileAs_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(177, 6);
            // 
            // _recentFilesMenuItem
            // 
            _recentFilesMenuItem.Name = "_recentFilesMenuItem";
            _recentFilesMenuItem.Size = new Size(180, 22);
            _recentFilesMenuItem.Text = "Recent &Files";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(177, 6);
            // 
            // exitMenuItem
            // 
            exitMenuItem.Name = "exitMenuItem";
            exitMenuItem.ShortcutKeys = Keys.Alt | Keys.F4;
            exitMenuItem.Size = new Size(180, 22);
            exitMenuItem.Text = "E&xit";
            exitMenuItem.Click += exitMenuItem_Click;
            // 
            // optionsMenu
            // 
            optionsMenu.DropDownItems.AddRange(new ToolStripItem[] { maintainAspectMenuItem, loadLastFileOnStartupToolStripMenuItem });
            optionsMenu.Name = "optionsMenu";
            optionsMenu.Size = new Size(61, 20);
            optionsMenu.Text = "&Options";
            // 
            // maintainAspectMenuItem
            // 
            maintainAspectMenuItem.Checked = true;
            maintainAspectMenuItem.CheckOnClick = true;
            maintainAspectMenuItem.CheckState = CheckState.Checked;
            maintainAspectMenuItem.Name = "maintainAspectMenuItem";
            maintainAspectMenuItem.Size = new Size(203, 22);
            maintainAspectMenuItem.Text = "&Maintain Aspect Ratio";
            maintainAspectMenuItem.Click += MaintainAspectRatio_Click;
            // 
            // loadLastFileOnStartupToolStripMenuItem
            // 
            loadLastFileOnStartupToolStripMenuItem.Checked = true;
            loadLastFileOnStartupToolStripMenuItem.CheckOnClick = true;
            loadLastFileOnStartupToolStripMenuItem.CheckState = CheckState.Checked;
            loadLastFileOnStartupToolStripMenuItem.Name = "loadLastFileOnStartupToolStripMenuItem";
            loadLastFileOnStartupToolStripMenuItem.Size = new Size(203, 22);
            loadLastFileOnStartupToolStripMenuItem.Text = "&Load Last File on Startup";
            loadLastFileOnStartupToolStripMenuItem.Click += loadLastFileOnStartupToolStripMenuItem_Click;
            // 
            // helpMenu
            // 
            helpMenu.DropDownItems.AddRange(new ToolStripItem[] { aboutMenuItem });
            helpMenu.Name = "helpMenu";
            helpMenu.Size = new Size(44, 20);
            helpMenu.Text = "&Help";
            // 
            // aboutMenuItem
            // 
            aboutMenuItem.Name = "aboutMenuItem";
            aboutMenuItem.Size = new Size(107, 22);
            aboutMenuItem.Text = "&About";
            aboutMenuItem.Click += aboutMenuItem_Click;
            // 
            // toolbar
            // 
            toolbar.Items.AddRange(new ToolStripItem[] { resolutionLabel, resolutionDropdown, toolStripSeparator3, windowPropertiesLabel });
            toolbar.Location = new Point(0, 24);
            toolbar.Name = "toolbar";
            toolbar.Size = new Size(1029, 25);
            toolbar.TabIndex = 1;
            toolbar.Text = "toolStrip1";
            // 
            // resolutionLabel
            // 
            resolutionLabel.Name = "resolutionLabel";
            resolutionLabel.Size = new Size(66, 22);
            resolutionLabel.Text = "Resolution:";
            // 
            // resolutionDropdown
            // 
            resolutionDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            resolutionDropdown.Name = "resolutionDropdown";
            resolutionDropdown.Size = new Size(180, 25);
            resolutionDropdown.SelectedIndexChanged += ResolutionDropdown_SelectedIndexChanged;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 25);
            // 
            // windowPropertiesLabel
            // 
            windowPropertiesLabel.Name = "windowPropertiesLabel";
            windowPropertiesLabel.Size = new Size(110, 22);
            windowPropertiesLabel.Text = "Window Properties:";
            // 
            // statusBar
            // 
            statusBar.Items.AddRange(new ToolStripItem[] { _statusLabel });
            statusBar.Location = new Point(0, 608);
            statusBar.Name = "statusBar";
            statusBar.Size = new Size(1029, 22);
            statusBar.TabIndex = 2;
            statusBar.Text = "statusStrip1";
            // 
            // _statusLabel
            // 
            _statusLabel.Name = "_statusLabel";
            _statusLabel.Size = new Size(39, 17);
            _statusLabel.Text = "Ready";
            // 
            // contentPanel
            // 
            contentPanel.Controls.Add(MainSplitter);
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.Location = new Point(0, 49);
            contentPanel.Name = "contentPanel";
            contentPanel.Size = new Size(1029, 559);
            contentPanel.TabIndex = 3;
            // 
            // MainSplitter
            // 
            MainSplitter.Dock = DockStyle.Fill;
            MainSplitter.FixedPanel = FixedPanel.Panel2;
            MainSplitter.Location = new Point(0, 0);
            MainSplitter.Name = "MainSplitter";
            // 
            // MainSplitter.Panel1
            // 
            MainSplitter.Panel1.Controls.Add(UiViewport);
            // 
            // MainSplitter.Panel2
            // 
            MainSplitter.Panel2.Controls.Add(propertyPanel);
            MainSplitter.Size = new Size(1029, 559);
            MainSplitter.SplitterDistance = 767;
            MainSplitter.TabIndex = 0;
            // 
            // UiViewport
            // 
            UiViewport.BackColor = Color.DarkGray;
            UiViewport.BorderStyle = BorderStyle.FixedSingle;
            UiViewport.Dock = DockStyle.Fill;
            UiViewport.Location = new Point(0, 0);
            UiViewport.MaintainAspectRatio = true;
            UiViewport.Margin = new Padding(0);
            UiViewport.Name = "UiViewport";
            UiViewport.Profile = null;
            UiViewport.Size = new Size(767, 559);
            UiViewport.TabIndex = 0;
            UiViewport.TargetResolution = new Size(2560, 1440);
            UiViewport.SelectionChanged += Viewport_SelectionChanged;
            // 
            // propertyPanel
            // 
            propertyPanel.Controls.Add(_propertyGrid);
            propertyPanel.Dock = DockStyle.Fill;
            propertyPanel.Location = new Point(0, 0);
            propertyPanel.Name = "propertyPanel";
            propertyPanel.Padding = new Padding(5);
            propertyPanel.Size = new Size(258, 559);
            propertyPanel.TabIndex = 0;
            // 
            // _propertyGrid
            // 
            _propertyGrid.Dock = DockStyle.Fill;
            _propertyGrid.Location = new Point(5, 5);
            _propertyGrid.Name = "_propertyGrid";
            _propertyGrid.PropertySort = PropertySort.Categorized;
            _propertyGrid.Size = new Size(248, 549);
            _propertyGrid.TabIndex = 0;
            _propertyGrid.ToolbarVisible = false;
            // 
            // copyProfileToolStripMenuItem
            // 
            copyProfileToolStripMenuItem.Name = "copyProfileToolStripMenuItem";
            copyProfileToolStripMenuItem.Size = new Size(180, 22);
            copyProfileToolStripMenuItem.Text = "&Copy Profile";
            copyProfileToolStripMenuItem.Click += copyProfileToolStripMenuItem_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1029, 630);
            Controls.Add(contentPanel);
            Controls.Add(statusBar);
            Controls.Add(toolbar);
            Controls.Add(mainMenu);
            Font = new Font("Segoe UI", 12F);
            MainMenuStrip = mainMenu;
            Margin = new Padding(4);
            MinimumSize = new Size(800, 600);
            Name = "MainForm";
            Text = "Quarm Character UI Profile Editor";
            mainMenu.ResumeLayout(false);
            mainMenu.PerformLayout();
            toolbar.ResumeLayout(false);
            toolbar.PerformLayout();
            statusBar.ResumeLayout(false);
            statusBar.PerformLayout();
            contentPanel.ResumeLayout(false);
            MainSplitter.Panel1.ResumeLayout(false);
            MainSplitter.Panel2.ResumeLayout(false);
            ((ISupportInitialize)MainSplitter).EndInit();
            MainSplitter.ResumeLayout(false);
            propertyPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

            // Form event handlers
            //_splitter.SplitterMoved += (s, e) => UiViewport?.Invalidate();


        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileMenu;
        private System.Windows.Forms.ToolStripMenuItem openMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsMenu;
        private System.Windows.Forms.ToolStripMenuItem maintainAspectMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpMenu;
        private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
        private System.Windows.Forms.ToolStrip toolbar;
        private System.Windows.Forms.ToolStripLabel resolutionLabel;
        private System.Windows.Forms.ToolStripComboBox resolutionDropdown;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripLabel windowPropertiesLabel;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.Panel propertyPanel;
        
        // Control declarations that should be kept in the Designer
        private System.Windows.Forms.SplitContainer MainSplitter;
        private System.Windows.Forms.PropertyGrid _propertyGrid;
        private System.Windows.Forms.ToolStripStatusLabel _statusLabel;
        private LayoutEditor.WinForms.Controls.UiViewport UiViewport;
        private System.Windows.Forms.ToolStripMenuItem _recentFilesMenuItem;
        private ToolStripMenuItem loadLastFileOnStartupToolStripMenuItem;
        private ToolStripMenuItem copyProfileToolStripMenuItem;
    }
}
