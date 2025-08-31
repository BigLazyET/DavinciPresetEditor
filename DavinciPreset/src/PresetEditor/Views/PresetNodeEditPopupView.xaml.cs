using CommunityToolkit.Maui.Extensions;
using PresetEditor.Localizations;
using PresetEditor.Models;
using PresetEditor.ViewModels;
using zoft.MauiExtensions.Core.Extensions;

namespace PresetEditor.Views;

public partial class PresetNodeEditPopupView : ContentView
{
	private readonly PresetNodeEditPopupViewModel _viewModel;
	
	public PresetNodeEditPopupView(PresetNodeEditPopupViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
		_viewModel = viewModel;
	}

	private void Add_OnClicked(object? sender, EventArgs e)
	{
		_viewModel.InstanceInput.PropertyList.Add(new InputItem());
	}

	private async void Confirm_OnClicked(object? sender, EventArgs e)
	{
		await Shell.Current.ClosePopupAsync(_viewModel.InstanceInput);
	}
}