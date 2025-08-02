using PresetEditor.ViewModels;

namespace PresetEditor.Views;

public partial class InstanceInputView : ContentView
{
	public InstanceInputView(InstanceInputViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}