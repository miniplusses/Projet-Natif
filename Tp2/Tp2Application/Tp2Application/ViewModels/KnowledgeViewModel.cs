using System;
using System.Collections.Generic;
using Avalonia.Controls;
using DynamicData;
using ReactiveUI;
using Tp2Application.Model;

namespace Tp2Application.ViewModels; 

public class KnowledgeViewModel : PageViewModel {
    
    public KnowledgeViewModel() {
        _regularButtons = new List<string> { "Connaissance" }; 
        _adminButtons = new List<string> { "Gestion utilisateur", "Gestion connaissance", "Ajout connaissance" };
        Pages = new PageViewModel[] {new KnowledgeResearchViewModel(this), new ManageUserViewModel(), new ManageKnowledgeViewModel(), new AddKnowledgeViewModel(), new DetailsViewModel()};
        _currentPage = Pages[0];
        _regularSelectedButton = _regularButtons[0];
    }

    public async void getIsAdmin(string username) {
        IsAdmin = await ApiRequest.SendRequest<bool>("isAdmin/", $"{{ \"username\": \"{username}\", \"password\": \"\" }}", "POST");
    }
    
    private readonly PageViewModel[] Pages;
    
    private PageViewModel _currentPage;
    
    public PageViewModel CurrentPage {
        get => _currentPage;
        set {
            this.RaiseAndSetIfChanged(ref _currentPage, value);
            
            if (CurrentPage is KnowledgeResearchViewModel knowledgeResearchViewModel) {
                knowledgeResearchViewModel.ConnectionString = ConnectionString;
            } else if (CurrentPage is ManageKnowledgeViewModel adminViewModel) {
                adminViewModel.ConnectionString = ConnectionString;
            }
        } 
    }
    
    public void Navigate(string pageName, Knowledge knowledge = null) {
        var index = Array.FindIndex(Pages, page => page.GetType().Name == pageName);
        CurrentPage = Pages[index];
        if (CurrentPage is ManageKnowledgeViewModel manageKnowledgeViewModel) {
            manageKnowledgeViewModel.Refresh();
        } else if (CurrentPage is DetailsViewModel detailsViewModel) {
            detailsViewModel.SelectedKnowledge = knowledge;
            RegularSelectedButton = null;
            AdminSelectedButton = null;
        } else if (CurrentPage is ManageUserViewModel manageUserViewModel) {
            manageUserViewModel.ConnectionString = ConnectionString;
        }
    }
    
    public override bool CanNavigateNext {
        get => true;
        protected set => throw new NotSupportedException();
    }
    
    public override bool CanNavigatePrevious {
        get => true;
        protected set => throw new NotSupportedException();
    }
    
    private void UpdateCanNavigateNext(){}
    
    private List<string> _regularButtons;
    
    public List<string> RegularButtons {
        get => _regularButtons;
        set => this.RaiseAndSetIfChanged(ref _regularButtons, value);
    }
    
    private List<string> _adminButtons;
    
    public List<string> AdminButtons {
        get => _adminButtons;
        set => this.RaiseAndSetIfChanged(ref _adminButtons, value);
    }
    
    private string _regularSelectedButton;
    
    public string RegularSelectedButton {
        get => _regularSelectedButton;
        set {
            this.RaiseAndSetIfChanged(ref _regularSelectedButton, value);
            switch (_regularSelectedButton) {
                case "Connaissance":
                    Navigate("KnowledgeResearchViewModel");
                    AdminSelectedButton = null;
                    break;
            }
        }
    }
    
    private string _adminSelectedButton;
    
    public string AdminSelectedButton {
        get => _adminSelectedButton;
        set {
            this.RaiseAndSetIfChanged(ref _adminSelectedButton, value);
            switch (_adminSelectedButton) {
                case "Gestion utilisateur":
                    Navigate("ManageUserViewModel");
                    RegularSelectedButton = null;
                    break;
                case "Gestion connaissance":
                    Navigate("ManageKnowledgeViewModel");
                    RegularSelectedButton = null;
                    break;
                case "Ajout connaissance":
                    Navigate("AddKnowledgeViewModel");
                    RegularSelectedButton = null;
                    break;
            }
        }
    }
    
    private string _connectionString;
    
    public string ConnectionString {
        get => _connectionString;
        set {
           this.RaiseAndSetIfChanged(ref _connectionString, value); 
           getIsAdmin(ConnectionString.Split("=")[3].Split(";")[0]);
           if (CurrentPage is KnowledgeResearchViewModel knowledgeResearchViewModel) {
               knowledgeResearchViewModel.ConnectionString = value;
           } else if (CurrentPage is ManageKnowledgeViewModel adminViewModel) {
               adminViewModel.ConnectionString = value;
           }
        }
    }
    
    private bool _isAdmin;
    
    public bool IsAdmin {
        get => _isAdmin;
        set => this.RaiseAndSetIfChanged(ref _isAdmin, value);
    }
}