// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Avalonia;

namespace ICSharpCode.TreeView;

static class ExtensionMethods
{
    public static T FindAncestor<T>(this AvaloniaObject d) where T : class => AncestorsAndSelf(d).OfType<T>().FirstOrDefault();

    public static IEnumerable<AvaloniaObject> AncestorsAndSelf(this AvaloniaObject d)
    {
        while (d != null)
        {
            yield return d;

            d = VisualTreeHelper.GetParent(d);
        }
    }

    public static void AddOnce(this IList list, object item)
    {
        if (!list.Contains(item))
            list.Add(item);
    }
}