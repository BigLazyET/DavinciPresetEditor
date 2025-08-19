using CommunityToolkit.Maui.Extensions;
using PresetEditor.Localizations;
using PresetEditor.Models;
using PresetEditor.ViewModels;
using zoft.MauiExtensions.Core.Extensions;

namespace PresetEditor.Views;

public partial class GroupNodeEditPopupView : ContentView
{
    private readonly GroupNodeEditPopupViewModel _viewModel;
    
    private string Remind => LocalizationResourceManager.Instance["Reminder"].ToString();
    private string ConfirmD => LocalizationResourceManager.Instance["Confirm"].ToString();
    private string Cancel => LocalizationResourceManager.Instance["Cancel"].ToString();
	
    public GroupNodeEditPopupView(GroupNodeEditPopupViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    private void Add_OnClicked(object? sender, EventArgs e)
    {
        _viewModel.GroupInput.PropertyList.Add(new InputItem());
    }

    private async void Delete_OnClicked(object? sender, EventArgs e)
    {
        if (_viewModel.GroupInput.PropertyList.Count == 0) return;
        var res=  await App.Current.MainPage.DisplayAlert(Remind, LocalizationResourceManager.Instance["DeleteLIneContent"].ToString(),ConfirmD, Cancel);
        if (!res) return;
        _viewModel.GroupInput.PropertyList.RemoveLast();
    }

    private async void Confirm_OnClicked(object? sender, EventArgs e)
    {
        await Shell.Current.ClosePopupAsync(_viewModel.GroupInput);
    }
}