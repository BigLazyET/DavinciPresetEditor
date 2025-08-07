using CommunityToolkit.Maui.Extensions;
using PresetEditor.Models;
using PresetEditor.ViewModels;
using zoft.MauiExtensions.Core.Extensions;

namespace PresetEditor.Views;

public partial class InstanceInputView : ContentView
{
	private readonly InstanceInputViewModel _viewModel;
	
	public InstanceInputView(InstanceInputViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
		_viewModel = viewModel;
	}

	private async void Close_OnClicked(object? sender, EventArgs e)
	{
		await Shell.Current.ClosePopupAsync();
	}

	private void Add_OnClicked(object? sender, EventArgs e)
	{
		_viewModel.InstanceInput.PropertyList.Add(new InputItem());
	}

	private async void Delete_OnClicked(object? sender, EventArgs e)
	{
		var res=  await App.Current.MainPage.DisplayAlert("警告", "确认要删除么?","确认", "取消");
		if (!res) return;
		_viewModel.InstanceInput.PropertyList.RemoveLast();
	}
}