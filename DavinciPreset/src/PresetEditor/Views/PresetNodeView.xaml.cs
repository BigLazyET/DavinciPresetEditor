using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PresetEditor.Views;

public partial class PresetNodeView : ContentView
{
    public PresetNodeView()
    {
        InitializeComponent();
    }

    private void OnGroupCheckBoxCheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        var pageModel = this.BindingContext as PresetPickerPageModel;
        if (pageModel == null) return;
        pageModel.IsMarkGroup = e.Value;

        var groupNames = pageModel.GroupInputs.Select(x => x.GroupName);
        var groupNameArray = groupNames as string[] ?? groupNames.ToArray();
        if (groupNameArray.Length == 0) return;
        foreach (var instanceInput in pageModel.InstanceInputs)
        {
            var source = instanceInput.PropertyList.FirstOrDefault(p => p.Key == "Source")?.Value;
            if (string.IsNullOrWhiteSpace(source)) continue;
            var name = instanceInput.PropertyList.FirstOrDefault(p => p.Key == "Name")?.Value;
            if (string.IsNullOrWhiteSpace(name)) continue;

            if (source != pageModel.GroupSource || !groupNameArray.Contains(name)) continue;
            
            if (pageModel.IsMarkGroup)
                instanceInput.MarkColor = Colors.Red;
            else
                instanceInput.MarkColor = Colors.Transparent;
        }
    }

    private void OnTabCheckBoxheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        var pageModel = this.BindingContext as PresetPickerPageModel;
        if (pageModel == null) return;
        pageModel.IsMarkTab = e.Value;
        
        var groupNames = pageModel.GroupInputs.Select(x => x.GroupName);
        var groupNameArray = groupNames as string[] ?? groupNames.ToArray();
        if (groupNameArray.Length == 0) return;
        foreach (var instanceInput in pageModel.InstanceInputs)
        {
            var page = instanceInput.PropertyList.FirstOrDefault(p => p.Key == "Page")?.Value;
            if (string.IsNullOrWhiteSpace(page)) continue;
            
            if (pageModel.IsMarkTab)
                instanceInput.MarkColor = Colors.Blue;
            else
                instanceInput.MarkColor = Colors.Transparent;
        }
    }
}