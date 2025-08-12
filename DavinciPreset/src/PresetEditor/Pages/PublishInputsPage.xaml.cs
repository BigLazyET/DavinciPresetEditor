using zoft.MauiExtensions.Controls;

namespace PresetEditor.Pages;

public partial class PublishInputsPage : ContentPage
{
    private readonly PublishInputsPageModel _pageModel;
    
    public PublishInputsPage(PublishInputsPageModel pageModel)
    {
        InitializeComponent();
        
        BindingContext = _pageModel = pageModel;
    }
    
    private async void OnGroupCheckBoxCheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        var groupSourcePageModel = App.Current.ServiceProvider.GetRequiredService<GroupSourcesPageModel>();
        var groupSourceOp = groupSourcePageModel.GroupSourceOp;
        var groupInputs = groupSourcePageModel.GroupInputs;
        
        if (e.Value && string.IsNullOrWhiteSpace(groupSourceOp))
        {
            await App.Current.MainPage!.DisplayAlert("确认", "⚠️请先填写分组源页中的分组节点名，之后才能进行匹配","确认");
            return;
        }
        
        var groupSourceNames = groupInputs.Select(x => x.GroupSourceName);  // Label0
        if (!groupSourceNames.Any()) return;
        foreach (var instanceInput in _pageModel.InstanceInputs)
        {
            var source = instanceInput.PropertyList.FirstOrDefault(p => p.Key == "Source")?.Value;  // Label0
            if (source == null) continue;
            var sourceOp = instanceInput.PropertyList.FirstOrDefault(p => p.Key == "SourceOp")?.Value;
            if (sourceOp == null) continue;

            if (sourceOp != groupSourceOp|| !groupSourceNames.Contains(source)) continue;
            
            instanceInput.MarkColor = e.Value ? Colors.Red : Colors.Transparent;
        }
    }

    private void OnTabCheckBoxheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        foreach (var instanceInput in _pageModel.InstanceInputs)
        {
            var page = instanceInput.PropertyList.FirstOrDefault(p => p.Key == "Page")?.Value;
            if (page == null) continue;
            
            instanceInput.MarkColor = e.Value ? Colors.Blue : Colors.Transparent;
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

    private void OnMoveBtnClicked(object? sender, EventArgs e)
    {
        MultiplyMove.IsVisible = !MultiplyMove.IsVisible;
        MoveInputsGrid.IsVisible = !MoveInputsGrid.IsVisible;
    }
}