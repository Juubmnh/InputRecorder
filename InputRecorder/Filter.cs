using System.Text.RegularExpressions;

namespace InputRecorder;

internal class Filter
{
    private readonly TextBox _txtFilter;
    private readonly ListBox _lstFilter;

    public Filter(Button? btnFilter, TextBox txtFilter, ListBox lstFilter, string defaultPattern)
    {
        btnFilter?.Click += btnFilter_Click;

        txtFilter.KeyDown += txtFilter_KeyDown;
        _txtFilter = txtFilter;

        lstFilter.MouseDoubleClick += lstFilter_MouseDoubleClick;
        lstFilter.Items.Add(new Regex(defaultPattern));
        _lstFilter = lstFilter;
    }

    private void btnFilter_Click(object? sender, EventArgs e)
    {
        if (_txtFilter.Text.IsWhiteSpace())
        {
            return;
        }

        Regex regex;
        try
        {
            regex = new Regex(_txtFilter.Text);
        }
        catch (Exception)
        {
            MessageBox.Show("Invalid regex");
            return;
        }

        bool exist = false;
        foreach (var item in _lstFilter.Items)
        {
            if ((item as Regex)!.ToString().Equals(regex.ToString()))
            {
                exist = true;
                break;
            }
        }

        if (!exist)
        {
            _lstFilter.Items.Add(regex);
        }
    }

    private void txtFilter_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            btnFilter_Click(sender, e);
        }
    }

    private void lstFilter_MouseDoubleClick(object? sender, MouseEventArgs e)
    {
        if (_lstFilter.SelectedIndex != ListBox.NoMatches)
        {
            _lstFilter.Items.RemoveAt(_lstFilter.SelectedIndex);
        }
    }

    public bool IsUnfiltered(List<string> pressed)
    {
        foreach (var obj in _lstFilter.Items)
        {
            bool ok = false;
            foreach (var item in pressed)
            {
                if ((obj as Regex)!.IsMatch(item))
                {
                    ok = true;
                    break;
                }
            }

            if (!ok)
            {
                return false;
            }
        }

        return true;
    }
}
