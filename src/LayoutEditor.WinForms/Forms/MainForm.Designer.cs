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
            copyProfileToolStripMenuItem = new ToolStripMenuItem();
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
            dockPanel1 = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            vS2015DarkTheme1 = new WeifenLuo.WinFormsUI.Docking.VS2015DarkTheme();
            mainMenu.SuspendLayout();
            toolbar.SuspendLayout();
            statusBar.SuspendLayout();
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
            openMenuItem.Size = new Size(155, 22);
            openMenuItem.Text = "&Open...";
            openMenuItem.Click += OpenProfile_Click;
            // 
            // saveMenuItem
            // 
            saveMenuItem.Name = "saveMenuItem";
            saveMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            saveMenuItem.Size = new Size(155, 22);
            saveMenuItem.Text = "&Save";
            saveMenuItem.Click += SaveProfile_Click;
            // 
            // saveAsMenuItem
            // 
            saveAsMenuItem.Name = "saveAsMenuItem";
            saveAsMenuItem.Size = new Size(155, 22);
            saveAsMenuItem.Text = "Save &As...";
            saveAsMenuItem.Click += SaveProfileAs_Click;
            // 
            // copyProfileToolStripMenuItem
            // 
            copyProfileToolStripMenuItem.Name = "copyProfileToolStripMenuItem";
            copyProfileToolStripMenuItem.Size = new Size(155, 22);
            copyProfileToolStripMenuItem.Text = "&Copy Profile";
            copyProfileToolStripMenuItem.Click += copyProfileToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(152, 6);
            // 
            // _recentFilesMenuItem
            // 
            _recentFilesMenuItem.Name = "_recentFilesMenuItem";
            _recentFilesMenuItem.Size = new Size(155, 22);
            _recentFilesMenuItem.Text = "Recent &Files";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(152, 6);
            // 
            // exitMenuItem
            // 
            exitMenuItem.Name = "exitMenuItem";
            exitMenuItem.ShortcutKeys = Keys.Alt | Keys.F4;
            exitMenuItem.Size = new Size(155, 22);
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
            // dockPanel1
            // 
            dockPanel1.BackColor = Color.SaddleBrown;
            dockPanel1.Dock = DockStyle.Fill;
            dockPanel1.DockBackColor = Color.FromArgb(45, 45, 48);
            dockPanel1.Location = new Point(0, 49);
            dockPanel1.DockBottomPortion = 150D;
            dockPanel1.DockLeftPortion = 200D;
            dockPanel1.DockRightPortion = 200D;
            dockPanel1.DockTopPortion = 150D;
            dockPanel1.Name = "dockPanel1";
            dockPanel1.Padding = new Padding(6);
            dockPanel1.ShowAutoHideContentOnHover = false;
            dockPanel1.Size = new Size(1029, 559);
            dockPanel1.TabIndex = 4;
            dockPanel1.Theme = vS2015DarkTheme1;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1029, 630);
            Controls.Add(dockPanel1);
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
        private System.Windows.Forms.ToolStripStatusLabel _statusLabel;
        private System.Windows.Forms.ToolStripMenuItem _recentFilesMenuItem;
        private ToolStripMenuItem loadLastFileOnStartupToolStripMenuItem;
        private ToolStripMenuItem copyProfileToolStripMenuItem;
        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel1;
        private WeifenLuo.WinFormsUI.Docking.VS2015DarkTheme vS2015DarkTheme1;
    }
}
