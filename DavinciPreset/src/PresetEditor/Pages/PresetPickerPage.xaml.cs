using PresetEditor.Models;

namespace PresetEditor.Pages;

public partial class PresetPickerPage : ContentPage
{
	private readonly PresetPickerPageModel _pageModel;

	public PresetPickerPage(PresetPickerPageModel pageModel)
	{
		InitializeComponent();
		BindingContext = pageModel;
		_pageModel = pageModel;
	}
}