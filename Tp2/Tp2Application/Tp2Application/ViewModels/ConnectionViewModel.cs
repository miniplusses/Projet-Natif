using System;
using System.Windows.Input;
using ReactiveUI;
using Tp2Application.Model;

namespace Tp2Application.ViewModels; 

public class ConnectionViewModel : PageViewModel {
    
    private MainWindowViewModel _mainWindowViewModel;
    
    public MainWindowViewModel MainViewModel {
        get => _mainWindowViewModel;
        set => this.RaiseAndSetIfChanged(ref _mainWindowViewModel, value);
    }

    private bool _InAction;
    
    public bool InAction {
        get => _InAction;
        set => this.RaiseAndSetIfChanged(ref _InAction, value);
    }
    
    private bool _activated;
    
    public bool Activated {
        get => _activated;
        set => this.RaiseAndSetIfChanged(ref _activated, value);
    }

    private string _username;
    
    public string Username {
        get => _username;
        set => this.RaiseAndSetIfChanged(ref _username, value);
    }
    
    private string _password;
    
    public string Password {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }
    
    private string _error;
    
    public string Error {
        get => _error;
        set => this.RaiseAndSetIfChanged(ref _error, value);
    }
    
    public async void Connection() {
        /*Error = "";
        if (Username != "" && Password != "" && Username != null && Password != null && !InAction) {
            InAction = true;
            var response = await ApiRequest.SendRequest<string>("", $"{{ \"username\": \"{Username}\", \"password\": \"{Password}\" }}", "PUT");
            if (response != null && response != "") {
                Activated = false;
                MainViewModel.NavigateKnowledgeCommand.Execute(response);
            } else {
                Error = "Veuillez utiliser des identifiants valide.";
            }
            InAction = false;
        } else if (InAction) {
            Error = "Veuillez attendre la fin de la requête.";
        } else {
            Error = "Veuillez remplir les champs correctement.";
        }*/
        MainViewModel.NavigateKnowledgeCommand.Execute("");
    }

    public void Register() {
        MainViewModel.NavigateNextCommand.Execute(null);
    }
    
    public override bool CanNavigateNext {
        get => true;
        protected set => throw new NotSupportedException();
    }
    
    public override bool CanNavigatePrevious {
        get => false;
        protected set => throw new NotSupportedException();
    }
    
    public ConnectionViewModel(MainWindowViewModel mainWindowViewModel) {
        _mainWindowViewModel = mainWindowViewModel;
        Activated = true;
    }
}