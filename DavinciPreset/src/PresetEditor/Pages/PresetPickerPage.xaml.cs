namespace PresetEditor.Pages;

public partial class PresetPickerPage : ContentPage
{
	public PresetPickerPage(PresetPickerPageModel pageModel)
	{
		InitializeComponent();
		BindingContext = pageModel;
	}
}