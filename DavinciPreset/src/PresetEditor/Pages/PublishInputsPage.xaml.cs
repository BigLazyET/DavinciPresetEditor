using PresetEditor.Localizations;
using PresetEditor.Models;
using zoft.MauiExtensions.Controls;

namespace PresetEditor.Pages;

public partial class PublishInputsPage : ContentPage
{
    private readonly PublishInputsPageModel _pageModel;

    private string Remind => LocalizationResourceManager.Instance["Reminder"].ToString();
    private string Confirm => LocalizationResourceManager.Instance["Confirm"].ToString();
    private string Cancel => LocalizationResourceManager.Instance["Cancel"].ToString();
    
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
            await App.Current.MainPage!.DisplayAlert(Remind, LocalizationResourceManager.Instance["GroupNodeRemind"].ToString(),Confirm);
            GroupCb.IsChecked = false;
            return;
        }
        
        var groupSourceNames = groupInputs.Select(x => x.GroupSourceName);  // Label0
        if (!groupSourceNames.Any())
        {
            GroupCb.IsChecked = false;
            return;
        }
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
    
    private void BaseInput_OnSuggestionChosen(object? sender, AutoCompleteEntrySuggestionChosenEventArgs e)
    {
        if (e.SelectedItem is not string selectedItem) return;
        var selectedInputs = _pageModel.SelectedInstanceInputs.OfType<InstanceInput>();
        var selectedInputNames = selectedInputs.Select(s => s.InputName);
        if (selectedInputNames.Contains(selectedItem)) return;
        _pageModel.BaseInputName = selectedItem;
    }

    private async void OnSearchBtnClicked(object? sender, EventArgs e)
    {
        if (_pageModel.InstanceInputs.Count == 0)
        {
            await App.Current.MainPage.DisplayAlert(Remind, LocalizationResourceManager.Instance["ParasEmptyRemind"].ToString(),Confirm);
            return;
        }
        
        if (SearchPicker.SelectedIndex < 0)
        {
            await App.Current.MainPage!.DisplayAlert(Remind, LocalizationResourceManager.Instance["SearchTypeRemind"].ToString(),Confirm);
            return;
        }

        if (string.IsNullOrWhiteSpace(SearchContent.Text))
        {
            await App.Current.MainPage!.DisplayAlert(Remind, LocalizationResourceManager.Instance["SearchContentRemind"].ToString(),Confirm);
            return;
        }

        var searchIndex = SearchPicker.SelectedIndex;
        var searchContent = SearchContent.Text.ToLower();

        var filtered = searchIndex switch
        {
            0 => _pageModel.InstanceInputs.Where(i => i.InputName.Equals(searchContent, StringComparison.CurrentCultureIgnoreCase)),
            1 => _pageModel.InstanceInputs.Where(i =>
                i.PropertyList.Any(p => p.Key == "Name" && string.Equals(p.Value, searchContent, StringComparison.CurrentCultureIgnoreCase))),
            2 => _pageModel.InstanceInputs.Where(i =>
                i.PropertyList.Any(p => p.Key == "Source" && string.Equals(p.Value, searchContent, StringComparison.CurrentCultureIgnoreCase))),
            3 => _pageModel.InstanceInputs.Where(i =>
                i.PropertyList.Any(p => p.Key == "SourceOp" && string.Equals(p.Value,searchContent,StringComparison.CurrentCultureIgnoreCase))),
            _ => []
        };

        foreach (var item in _pageModel.InstanceInputs)
        {
            item.MarkColor = Colors.Transparent;
        }
        foreach (var item in filtered)
        {
            item.MarkColor = Colors.Green;
        }
    }

    private void OnCleanBtnClicked(object? sender, EventArgs e)
    {
        foreach (var item in _pageModel.InstanceInputs)
        {
            item.MarkColor = Colors.Transparent;
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        
        InputsCv.Unfocus();
        SearchPicker.Unfocus();
    }

    private async void OnPageSetBtnClicked(object? sender, EventArgs e)
    {
        if (_pageModel.SelectedInstanceInputs.Count == 0)
        {
            await App.Current.MainPage.DisplayAlert(Remind, LocalizationResourceManager.Instance["SelectItemsRemind"].ToString(),Confirm);
            return;
        }

        if (string.IsNullOrWhiteSpace(PageValue.Text))
        {
            await App.Current.MainPage.DisplayAlert(Remind, LocalizationResourceManager.Instance["PageNameEmptyRemind"].ToString(),Confirm);
            return;
        }
        
        var result = await App.Current.MainPage.DisplayAlert(Remind, LocalizationResourceManager.Instance["PageNameRemind"].ToString(),Confirm, Cancel);
        if (!result) return;

        var instanceInputs = _pageModel.SelectedInstanceInputs.OfType<InstanceInput>();
        foreach (var instanceInput in instanceInputs)
        {
            var inputItem = instanceInput.PropertyList.FirstOrDefault(p => p.Key == "Page");
            if (inputItem == null)
                instanceInput.PropertyList.Add(new InputItem { Key = "Page", Value = PageValue.Text });
            else
                inputItem.Value = PageValue.Text;
            instanceInput.PropertyListChanged();
        }
    }
}