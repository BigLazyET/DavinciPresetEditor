using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PresetEditor.Views;

public partial class GroupNodeView : ContentView
{
    private PresetPickerPageModel? _pageModel;
    
    public GroupNodeView()
    {
        InitializeComponent();
        
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        if (_pageModel == null)
            _pageModel = BindingContext as PresetPickerPageModel;
    }
}