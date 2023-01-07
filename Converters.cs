// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;

namespace ICSharpCode.TreeView;

public class CollapsedWhenFalse : MarkupExtension, IValueConverter
{
    public static CollapsedWhenFalse Instance = new();

    public override object ProvideValue(IServiceProvider serviceProvider) => Instance;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
        // return (bool)value
        //     ? Visibility.Visible
        //     : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}