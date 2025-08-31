using CommunityToolkit.Maui.Extensions;
using PresetEditor.Localizations;
using PresetEditor.Models;
using PresetEditor.ViewModels;
using zoft.MauiExtensions.Core.Extensions;

namespace PresetEditor.Views;

public partial class GroupNodeEditPopupView : ContentView
{
    private readonly GroupNodeEditPopupViewModel _viewModel;
	
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

    private async void Confirm_OnClicked(object? sender, EventArgs e)
    {
        await Shell.Current.ClosePopupAsync(_viewModel.GroupInput);
    }
}