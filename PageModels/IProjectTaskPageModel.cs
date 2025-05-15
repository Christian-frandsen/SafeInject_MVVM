using CommunityToolkit.Mvvm.Input;
using SafeInject_MVVM.Models;

namespace SafeInject_MVVM.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}