// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using Avalonia.Controls;
using Avalonia.Input;

namespace ICSharpCode.TreeView;

public class EditTextBox : TextBox
{
    private bool _committing;

    static EditTextBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(EditTextBox),
            new FrameworkPropertyMetadata(typeof(EditTextBox)));
    }

    public EditTextBox() => Loaded += delegate { Init(); };

    public SharpTreeViewItem Item { get; set; }

    public SharpTreeNode Node => Item.Node;

    void Init()
    {
        if (Node != null)
            Text = Node.LoadEditText();

        Focus();
        SelectAll();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            Commit();
        else if (e.Key == Key.Escape && Node != null)
            Node.IsEditing = false;
    }

    protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
        if (Node is { IsEditing: true }) Commit();
    }

    void Commit()
    {
        if (!_committing)
        {
            _committing = true;

            if (Node != null)
            {
                Node.IsEditing = false;

                if (!Node.SaveEditText(Text)) Item.Focus();

                Node.RaisePropertyChanged("Text");
            }

            _committing = false;
        }
    }
}