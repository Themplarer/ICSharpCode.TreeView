/*
	Copyright (c) 2015 Ki

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ICSharpCode.TreeView;

internal struct ObjectChangedEventArgs
{
    public SharpTreeNode OldNode { get; }

    public SharpTreeNode NewNode { get; }

    public ObjectChangedEventArgs(SharpTreeNode oldNode, SharpTreeNode newNode)
    {
        OldNode = oldNode;
        NewNode = newNode;
    }
}

internal class SharpTreeNodeProxy : CustomTypeDescriptor
{
    static SharpTreeNodeProxy()
    {
        DescMap = new Dictionary<string, IPropDesc>();
        AddPropertyDesc("Foreground", node => node.Foreground);
        AddPropertyDesc("IsExpanded", node => node.IsExpanded, (node, value) => node.IsExpanded = value);
        AddPropertyDesc("IsChecked", node => node.IsChecked, (node, value) => node.IsChecked = value);
        AddPropertyDesc("ToolTip", node => node.ToolTip);
        AddPropertyDesc("Icon", node => node.Icon);
        AddPropertyDesc("Text", node => node.Text);
        AddPropertyDesc("IsEditing", node => node.IsEditing, (node, value) => node.IsEditing = value);
        AddPropertyDesc("ShowIcon", node => node.ShowIcon);
        AddPropertyDesc("ShowExpander", node => node.ShowExpander);
        AddPropertyDesc("ExpandedIcon", node => node.ExpandedIcon);
        AddPropertyDesc("IsCheckable", node => node.IsCheckable);
        AddPropertyDesc("IsCut", node => node.IsCut);
        Descs = new PropertyDescriptorCollection(DescMap.Values.Cast<PropertyDescriptor>().ToArray());
    }

    private static readonly PropertyDescriptorCollection Descs;
    private static readonly Dictionary<string, IPropDesc> DescMap;

    static void AddPropertyDesc<T>(string name, Func<SharpTreeNode, T> getter, Action<SharpTreeNode, T> setter = null)
    {
        var desc = new PropDesc<T>(name, getter, setter);
        DescMap.Add(name, desc);
    }

    public SharpTreeNodeProxy(SharpTreeNode obj) => UpdateObject(obj);

    public void UpdateObject(SharpTreeNode obj)
    {
        if (Object == obj)
            return;

        if (Object != null)
            Object.PropertyChanged -= OnPropertyChanged;

        var oldNode = Object;
        Object = obj;

        if (obj == null)
            IsNull = true;
        else
        {
            IsNull = false;
            obj.PropertyChanged += OnPropertyChanged;

            foreach (var desc in DescMap) desc.Value.OnValueChanged(this);
        }

        ObjectChanged?.Invoke(this, new ObjectChangedEventArgs(oldNode, obj));
    }

    void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (DescMap.TryGetValue(e.PropertyName, out var desc))
            desc.OnValueChanged(this);
    }

    public event EventHandler<ObjectChangedEventArgs> ObjectChanged;

    public bool IsNull { get; private set; }

    public SharpTreeNode Object { get; private set; }

    public override PropertyDescriptorCollection GetProperties() => Descs;

    public override PropertyDescriptorCollection GetProperties(Attribute[] attributes) => GetProperties();

    private interface IPropDesc
    {
        void OnValueChanged(object component);
    }

    private class PropDesc<T> : PropertyDescriptor, IPropDesc
    {
        private readonly Func<SharpTreeNode, T> _getter;
        private readonly Action<SharpTreeNode, T> _setter;

        public PropDesc(string name, Func<SharpTreeNode, T> getter, Action<SharpTreeNode, T> setter)
            : base(name, null)
        {
            _getter = getter;
            _setter = setter;
        }

        public override object GetValue(object component) => _getter(((SharpTreeNodeProxy)component).Object);

        public override bool IsReadOnly => _setter == null;

        public override Type PropertyType => typeof(T);

        public override void SetValue(object component, object value) => _setter(((SharpTreeNodeProxy)component).Object, (T)value);

        public void OnValueChanged(object component) => OnValueChanged(component, new PropertyChangedEventArgs(Name));

        public override bool CanResetValue(object component) => false;

        public override bool ShouldSerializeValue(object component) => false;

        public override void ResetValue(object component) => throw new NotSupportedException();

        public override Type ComponentType => null;
    }
}