namespace LayoutEditor.WinForms.Forms;

partial class CopyProfileForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
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
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        lbCharacters = new ListBox();
        label1 = new Label();
        btnOk = new Button();
        btnCancel = new Button();
        btnSelectAll = new Button();
        btnSelectNone = new Button();
        SuspendLayout();
        // 
        // lbCharacters
        // 
        lbCharacters.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lbCharacters.FormattingEnabled = true;
        lbCharacters.IntegralHeight = false;
        lbCharacters.ItemHeight = 21;
        lbCharacters.Location = new Point(12, 72);
        lbCharacters.Name = "lbCharacters";
        lbCharacters.SelectionMode = SelectionMode.MultiSimple;
        lbCharacters.Size = new Size(203, 301);
        lbCharacters.TabIndex = 0;
        // 
        // label1
        // 
        label1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        label1.Location = new Point(12, 9);
        label1.Name = "label1";
        label1.Size = new Size(284, 60);
        label1.TabIndex = 1;
        label1.Text = "Select the characters to copy UI profile settings to below.";
        // 
        // btnOk
        // 
        btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnOk.Location = new Point(221, 379);
        btnOk.Name = "btnOk";
        btnOk.Size = new Size(75, 34);
        btnOk.TabIndex = 2;
        btnOk.Text = "&Ok";
        btnOk.UseVisualStyleBackColor = true;
        btnOk.Click += btnOk_Click;
        // 
        // btnCancel
        // 
        btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnCancel.Location = new Point(140, 379);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(75, 34);
        btnCancel.TabIndex = 2;
        btnCancel.Text = "&Cancel";
        btnCancel.UseVisualStyleBackColor = true;
        btnCancel.Click += btnCancel_Click;
        // 
        // btnSelectAll
        // 
        btnSelectAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnSelectAll.Location = new Point(221, 72);
        btnSelectAll.Name = "btnSelectAll";
        btnSelectAll.Size = new Size(75, 34);
        btnSelectAll.TabIndex = 3;
        btnSelectAll.Text = "&All";
        btnSelectAll.UseVisualStyleBackColor = true;
        btnSelectAll.Click += btnSelectAll_Click;
        // 
        // btnSelectNone
        // 
        btnSelectNone.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnSelectNone.Location = new Point(221, 112);
        btnSelectNone.Name = "btnSelectNone";
        btnSelectNone.Size = new Size(75, 34);
        btnSelectNone.TabIndex = 3;
        btnSelectNone.Text = "&None";
        btnSelectNone.UseVisualStyleBackColor = true;
        btnSelectNone.Click += btnSelectNone_Click;
        // 
        // CopyProfileForm
        // 
        AcceptButton = btnOk;
        AutoScaleDimensions = new SizeF(9F, 21F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(308, 425);
        Controls.Add(btnSelectNone);
        Controls.Add(btnSelectAll);
        Controls.Add(btnCancel);
        Controls.Add(btnOk);
        Controls.Add(label1);
        Controls.Add(lbCharacters);
        Font = new Font("Segoe UI", 12F);
        Margin = new Padding(4);
        Name = "CopyProfileForm";
        Text = "Copy Profile";
        ResumeLayout(false);
    }

    #endregion

    private ListBox lbCharacters;
    private Label label1;
    private Button btnOk;
    private Button btnCancel;
    private Button btnSelectAll;
    private Button btnSelectNone;
}