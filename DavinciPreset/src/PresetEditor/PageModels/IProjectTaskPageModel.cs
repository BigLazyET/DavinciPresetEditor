using CommunityToolkit.Mvvm.Input;
using PresetEditor.Models;

namespace PresetEditor.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}