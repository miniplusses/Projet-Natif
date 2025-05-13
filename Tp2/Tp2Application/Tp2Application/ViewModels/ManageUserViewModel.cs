using System;
using System.Collections.Generic;
using System.Net.Http;
using ReactiveUI;
using Tp2Application.Model;

namespace Tp2Application.ViewModels; 

public class ManageUserViewModel : PageViewModel {
    public override bool CanNavigateNext {
        get => true;
        protected set => throw new NotSupportedException();
    }
    
    public override bool CanNavigatePrevious {
        get => true;
        protected set => throw new NotSupportedException();
    }
    
    private void UpdateCanNavigateNext() {
        CanNavigateNext = true;
    }
    
    private string _connectionString;
    
    public string ConnectionString {
        get => _connectionString;
        set {
            this.RaiseAndSetIfChanged(ref _connectionString, value);
            getRegularUser();
            getInactifUser();
        }
    }
    
    private List<User> _listRegularUser;
    
    public List<User> ListRegularUser {
        get => _listRegularUser;
        set => this.RaiseAndSetIfChanged(ref _listRegularUser, value);
    }
    
    private User _selectedRegular;
    
    public User SelectedRegular {
        get => _selectedRegular;
        set => this.RaiseAndSetIfChanged(ref _selectedRegular, value);
    }
    
    private List<User> _listInactifUser;
    
    public List<User> ListInactifUser {
        get => _listInactifUser;
        set => this.RaiseAndSetIfChanged(ref _listInactifUser, value);
    }
    
    private User _selectedInactif;
    
    public User SelectedInactif {
        get => _selectedInactif;
        set => this.RaiseAndSetIfChanged(ref _selectedInactif, value);
    }
    
    public async void getRegularUser() {
        var info = ConnectionString.Split("=");
        var username = info[3].Split(";")[0];
        var response = await ApiRequest.SendRequest<List<User>>("regulier/", $"{{ \"username\": \"{username}\", \"password\": \"\" }}", "POST");
        ListRegularUser = response;
    }
    
    public async void getInactifUser() {
        var info = ConnectionString.Split("=");
        var username = info[3].Split(";")[0];
        var response = await ApiRequest.SendRequest<List<User>>("inactif/", $"{{ \"username\": \"{username}\", \"password\": \"\" }}", "POST");
        ListInactifUser = response;
    }

    public async void Promote() {
        var info = ConnectionString.Split("=");
        var username = info[3].Split(";")[0];
        var response = await ApiRequest.SendRequest<HttpResponseMessage>($"{{{SelectedRegular.Id}}}", $"{{ \"username\": \"{username}\", \"password\": \"\" }}", "PUT");
        if (response.IsSuccessStatusCode) {
            getRegularUser();
            getInactifUser();
        }
    }
    
    public async void Delete() {
        var info = ConnectionString.Split("=");
        var username = info[3].Split(";")[0];
        var response = await ApiRequest.SendRequest<HttpResponseMessage>("", $"{{ \"username\": \"{username}\", \"password\": \"\" }}", "DELETE");
        if (response.IsSuccessStatusCode) {
            getRegularUser();
            getInactifUser();
        }
    }
}