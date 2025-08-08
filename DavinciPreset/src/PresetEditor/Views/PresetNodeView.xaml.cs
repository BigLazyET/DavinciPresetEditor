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

        var groupSourceNames = _pageModel.GroupInputs.Select(x => x.GroupSouceName);  // Label0
        if (!groupSourceNames.Any()) return;
        foreach (var instanceInput in _pageModel.InstanceInputs)
        {
            var source = instanceInput.PropertyList.FirstOrDefault(p => p.Key == "Source")?.Value;  // Label0
            if (source == null) continue;
            var sourceOp = instanceInput.PropertyList.FirstOrDefault(p => p.Key == "SourceOp")?.Value;
            if (sourceOp == null) continue;

            if (sourceOp.ToString() != _pageModel.GroupSourceOp || !groupSourceNames.Contains(source.ToString())) continue;
            
            if (_pageModel.IsMarkGroup)
                instanceInput.MarkColor = Colors.Red;
            else
                instanceInput.MarkColor = Colors.Transparent;
        }
    }

    private void OnTabCheckBoxheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        _pageModel.IsMarkTab = e.Value;
        
        foreach (var instanceInput in _pageModel.InstanceInputs)
        {
            var page = instanceInput.PropertyList.FirstOrDefault(p => p.Key == "Page")?.Value;
            if (page == null) continue;
            
            instanceInput.MarkColor = _pageModel.IsMarkTab ? Colors.Blue : Colors.Transparent;
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