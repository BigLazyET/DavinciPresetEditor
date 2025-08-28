using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PresetEditor.Pages;

public partial class GroupSourcesPage : ContentPage
{
    private readonly GroupSourcesPageModel _pageModel;
    
    public GroupSourcesPage(GroupSourcesPageModel pageModel)
    {
        InitializeComponent();
        
        BindingContext = _pageModel = pageModel;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        
        GsOpEntry.Unfocus();
        GsOpTypeEntry.Unfocus();
    }
}