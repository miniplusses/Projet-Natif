using System;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using ReactiveUI;
using Tp2Application.Model;
using Tp2Application.ViewModels;

namespace Tp2Application.Views; 

public partial class ManageKnowledgeView : UserControl {
    
    public ManageKnowledgeView() {
        InitializeComponent();
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e) {
        (DataContext as ManageKnowledgeViewModel).OnGlobalClickCommand.Execute(null);
    }
}