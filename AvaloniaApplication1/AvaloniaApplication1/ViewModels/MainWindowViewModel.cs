using System.Reactive;
using System.Windows.Input;
using ReactiveUI;
using System.Reactive.Linq;

namespace AvaloniaApplication1.ViewModels;

public class MainWindowViewModel : ViewModelBase {
    public string Greeting { get; set; } = "";

    public Interaction<DialogViewModel, Unit> ShowDialog { get; }

    public MainWindowViewModel() {
        ShowDialog = new Interaction<DialogViewModel, Unit>();
        Save = ReactiveCommand.CreateFromTask(async () => {
                var vm = new DialogViewModel();
                await ShowDialog.Handle(vm);
        });
    }

    public ICommand Save { get; }
}