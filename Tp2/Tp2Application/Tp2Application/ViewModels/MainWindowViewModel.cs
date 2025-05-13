using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using DynamicData;
using ReactiveUI;

namespace Tp2Application.ViewModels;

public class MainWindowViewModel : ViewModelBase {

    private readonly PageViewModel[] Pages;
    
    private PageViewModel _CurrentPage;
    
    public PageViewModel CurrentPage {
        get => _CurrentPage;
        set => this.RaiseAndSetIfChanged(ref _CurrentPage, value);
    }
    
    private Window _window;
    public Window Window {
        get => _window;
        set => this.RaiseAndSetIfChanged(ref _window, value);
    }
    
    public ICommand NavigateNextCommand { get; }
    
    public ICommand NavigatePreviousCommand { get; }
    
    public ICommand NavigateKnowledgeCommand { get; }

    public void NavigateNext() {
        var index = Pages.IndexOf(CurrentPage) + 1;

        CurrentPage = Pages[index];
    }

    public void NavigatePrevious() {
        var index = Pages.IndexOf(CurrentPage) - 1;

        CurrentPage = Pages[index];
    }
    
    public void NavigateKnowledge(string response) {
        CurrentPage = Pages[2];
        if (CurrentPage is KnowledgeViewModel knowledgeViewModel) {
            knowledgeViewModel.ConnectionString = response;
        }
        Window.Height = 720;
        Window.Width = 1280;   
        Window.Position = new PixelPoint(Window.Position.X - 520, Window.Position.Y - 100);
    }
    
    public MainWindowViewModel(Window window) {
        Pages = new PageViewModel[] {new ConnectionViewModel(this), new RegisterViewModel(this), new KnowledgeViewModel()};
        _CurrentPage = Pages[0];
        
        _window = window;
        
        var canNavNext = this.WhenAnyValue(x => x.CurrentPage.CanNavigateNext);
        var canNavPrev = this.WhenAnyValue(x => x.CurrentPage.CanNavigatePrevious);
        NavigateNextCommand = ReactiveCommand.Create(NavigateNext, canNavNext);
        NavigatePreviousCommand = ReactiveCommand.Create(NavigatePrevious, canNavPrev);
        NavigateKnowledgeCommand = ReactiveCommand.Create<string>(response => NavigateKnowledge(response));
    }
}