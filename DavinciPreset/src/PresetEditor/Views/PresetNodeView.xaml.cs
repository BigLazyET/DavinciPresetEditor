using zoft.MauiExtensions.Controls;

namespace PresetEditor.Views;

public partial class PresetNodeView : ContentView
{
    private PresetPickerPageModel _pageModel;
    
    public PresetNodeView()
    {
        InitializeComponent();
        
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        _pageModel = BindingContext as PresetPickerPageModel;
    }

    private void OnGroupCheckBoxCheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        _pageModel.IsMarkGroup = e.Value;

        var groupNames = _pageModel.GroupInputs.Select(x => x.GroupName);
        var groupNameArray = groupNames as string[] ?? groupNames.ToArray();
        if (groupNameArray.Length == 0) return;
        foreach (var instanceInput in _pageModel.InstanceInputs)
        {
            var source = instanceInput.PropertyList.FirstOrDefault(p => p.Key == "Source")?.Value;
            if (string.IsNullOrWhiteSpace(source)) continue;
            var name = instanceInput.PropertyList.FirstOrDefault(p => p.Key == "Name")?.Value;
            if (string.IsNullOrWhiteSpace(name)) continue;

            if (source != _pageModel.GroupSource || !groupNameArray.Contains(name)) continue;
            
            if (_pageModel.IsMarkGroup)
                instanceInput.MarkColor = Colors.Red;
            else
                instanceInput.MarkColor = Colors.Transparent;
        }
    }

    private void OnTabCheckBoxheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        _pageModel.IsMarkTab = e.Value;
        
        var groupNames = _pageModel.GroupInputs.Select(x => x.GroupName);
        var groupNameArray = groupNames as string[] ?? groupNames.ToArray();
        if (groupNameArray.Length == 0) return;
        foreach (var instanceInput in _pageModel.InstanceInputs)
        {
            var page = instanceInput.PropertyList.FirstOrDefault(p => p.Key == "Page")?.Value;
            if (string.IsNullOrWhiteSpace(page)) continue;
            
            if (_pageModel.IsMarkTab)
                instanceInput.MarkColor = Colors.Blue;
            else
                instanceInput.MarkColor = Colors.Transparent;
        }
    }

    private void AutoCompleteEntry_OnSuggestionChosen(object? sender, AutoCompleteEntrySuggestionChosenEventArgs e)
    {
        if (e.SelectedItem is not string selectedItem) return;
        if (_pageModel.MoveInputNames.Contains(selectedItem)) return;
        if (selectedItem == _pageModel.BaseInputName) return;
        _pageModel.MoveInputNames.Add(selectedItem);
    }

    private void BaseInput_OnSuggestionChosen(object? sender, AutoCompleteEntrySuggestionChosenEventArgs e)
    {
        if (e.SelectedItem is not string selectedItem) return;
        if (_pageModel.MoveInputNames.Contains(selectedItem)) return;
        _pageModel.BaseInputName = selectedItem;
    }
}