using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PresetEditor.Pages;

public partial class ProjectTheme : ContentPage
{
    public ProjectTheme()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        VersionLbl.Text = $"{AppInfo.VersionString}.{AppInfo.BuildString}";
    }
}