using System;
using System.Runtime.InteropServices.JavaScript;
using System.Windows.Input;
using ReactiveUI;
using Tp2Application.Model;

namespace Tp2Application.ViewModels; 

public class RegisterViewModel : PageViewModel {
    
    private MainWindowViewModel _mainWindowViewModel;
    
    public MainWindowViewModel MainViewModel {
        get => _mainWindowViewModel;
        set => this.RaiseAndSetIfChanged(ref _mainWindowViewModel, value);
    }
    
    
    public override bool CanNavigateNext {
        get => false;
        protected set => throw new NotSupportedException();
    }
    
    public override bool CanNavigatePrevious {
        get => true;
        protected set => throw new NotSupportedException();
    }
    
    public RegisterViewModel(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
        Activated = true;
    }
    
    private bool _inAction;
    
    public bool InAction {
        get => _inAction;
        set => this.RaiseAndSetIfChanged(ref _inAction, value);
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
    
    private string _confirmedPassword;
    
    public string ConfirmedPassword {
        get => _confirmedPassword;
        set => this.RaiseAndSetIfChanged(ref _confirmedPassword, value);
    }
    
    private string _error;
    
    public string Error {
        get => _error;
        set => this.RaiseAndSetIfChanged(ref _error, value);
    }
    
    public void Connection() {
       MainViewModel.NavigatePreviousCommand.Execute(null);
    }

    public async void Register() {
        Error = "";
        if (Username != "" && Password != "" && ConfirmedPassword != "" && Password == ConfirmedPassword && Username != null && Password != null && ConfirmedPassword != null && !InAction) {
            InAction = true;
            Activated = false;
            var response = await ApiRequest.SendRequest<bool>("", $"{{ \"username\": \"{Username}\", \"password\": \"{Password}\" }}", "POST");
            if (response) {
                MainViewModel.NavigatePreviousCommand.Execute(null);
            } else {
                Error = "Le nom d'utilisateur est déjà pris.";
            }
            InAction = false;
            Activated = true;
        } else if (InAction) {
            Error = "Veuillez attendre la fin de la requête.";
        } else {
            Error = "Veuillez remplir les champs correctement.";
        } 
    }
    
}