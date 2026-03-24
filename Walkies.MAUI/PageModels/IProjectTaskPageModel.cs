using CommunityToolkit.Mvvm.Input;
using Walkies.MAUI.Models;

namespace Walkies.MAUI.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}