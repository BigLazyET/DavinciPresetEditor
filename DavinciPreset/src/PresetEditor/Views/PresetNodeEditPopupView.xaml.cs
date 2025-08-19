using CommunityToolkit.Maui.Extensions;
using PresetEditor.Localizations;
using PresetEditor.Models;
using PresetEditor.ViewModels;
using zoft.MauiExtensions.Core.Extensions;

namespace PresetEditor.Views;

public partial class PresetNodeEditPopupView : ContentView
{
	private readonly PresetNodeEditPopupViewModel _viewModel;
	
	private string Remind => LocalizationResourceManager.Instance["Reminder"].ToString();
	private string ConfirmD => LocalizationResourceManager.Instance["Confirm"].ToString();
	private string Cancel => LocalizationResourceManager.Instance["Cancel"].ToString();
	
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

	private async void Delete_OnClicked(object? sender, EventArgs e)
	{
		if (_viewModel.InstanceInput.PropertyList.Count == 0) return;
		var res=  await App.Current.MainPage.DisplayAlert(Remind, LocalizationResourceManager.Instance["DeleteLIneContent"].ToString(),ConfirmD, Cancel);
		if (!res) return;
		_viewModel.InstanceInput.PropertyList.RemoveLast();
	}

	private async void Confirm_OnClicked(object? sender, EventArgs e)
	{
		await Shell.Current.ClosePopupAsync(_viewModel.InstanceInput);
	}
}