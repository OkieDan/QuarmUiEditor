using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LayoutEditor.WinForms.Forms;
public partial class CopyProfileForm : Form
{
    public List<string> Characters { get; }

    public CopyProfileForm(List<String> characters)
    {
        InitializeComponent();
        Characters = characters;
        
        // Populate the listbox with character names
        lbCharacters.Items.AddRange(characters.ToArray());
    }

    private void btnSelectAll_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < lbCharacters.Items.Count; i++)
        {
            lbCharacters.SetSelected(i, true);
        }
    }

    private void btnSelectNone_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < lbCharacters.Items.Count; i++)
        {
            lbCharacters.SetSelected(i, false);
        }
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
        Characters.Clear();
        foreach (var item in lbCharacters.SelectedItems)
        {
            Characters.Add(item.ToString());
        }
        DialogResult = DialogResult.OK;
        Close();
    }
}