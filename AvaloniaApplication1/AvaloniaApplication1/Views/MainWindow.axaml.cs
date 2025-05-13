using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using AvaloniaApplication1.ViewModels;
using ReactiveUI;

namespace AvaloniaApplication1.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel> {
    public MainWindow() {
        InitializeComponent();
        this.WhenActivated(d => d(ViewModel!.ShowDialog.RegisterHandler(DoShowDialogAsync)));
    }


    private async Task DoShowDialogAsync(InteractionContext<DialogViewModel, Unit> interactionContext) {
        var dialog = new Dialogue();
        dialog.DataContext = interactionContext.Input;

        var result = await dialog.ShowDialog<Unit>(this);
        interactionContext.SetOutput(result);
    }
}