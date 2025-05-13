using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia.Input;
using ReactiveUI;
using Tp2Application.Model;

namespace Tp2Application.ViewModels; 

public class ManageKnowledgeViewModel : PageViewModel {
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
            Database.ConnectionString = value;
            ListKnowledge.Clear();
            ListKnowledge = Database.GetKnowledges(ListKnowledge);
        } 
    }
    
    private ObservableCollection<Knowledge> _listKnowledge = new();
    
    public ObservableCollection<Knowledge> ListKnowledge {
        get => _listKnowledge;
        set => this.RaiseAndSetIfChanged(ref _listKnowledge, value);
    }
    
    private Knowledge _selectedKnowledge;
    
    public Knowledge SelectedKnowledge {
        get => _selectedKnowledge;
        set => this.RaiseAndSetIfChanged(ref _selectedKnowledge, value);
    }

    private Knowledge _selectedItem;
    
    public Knowledge SelectedItem {
        get => _selectedItem;
        set {
           this.RaiseAndSetIfChanged(ref _selectedItem, value);
           if (value != null) {
             SelectedKnowledge = new Knowledge() {
                 id = value.id,
                 title = value.title,
                 description = value.description,
                 data = value.data
             };
             Confirmation = "";
             this.RaisePropertyChanged(nameof(CompleteEntry));
           }
        } 
    }
    
    private bool _isEditing;
    
    public bool IsEditing {
        get => _isEditing;
        set => this.RaiseAndSetIfChanged(ref _isEditing, value);
    }
    public void Edit() {
        if (!IsEditing) {
            IsEditing = true;
            IsRemoving = false;
            Confirmation = "Appuyer de nouveau pour confirmer la modification.\nPour annuler, cliquer à côté du boutton.";
        } else { 
            Confirmation = "";
            if (SelectedKnowledge != null) {
               if (SelectedKnowledge.title != null && SelectedKnowledge.description != null && SelectedKnowledge.data != null &&
                   SelectedKnowledge.title != "" && SelectedKnowledge.description != "") {
                   if (SelectedKnowledge.data.developpement.parution != null && SelectedKnowledge.data.developpement.parution.Length != 10)
                   {
                       Confirmation = "Veuillez remplir les champs correctement.";
                   }
                   else {
                       ListKnowledge.Clear();
                       Database.UpdateKnowledge(_selectedKnowledge);
                       ListKnowledge = Database.GetKnowledges(ListKnowledge);  
                   }
               } else {
                   Confirmation = "Veuillez remplir les champs correctement.";
                } 
            }else {
                Confirmation = "Rien a modifier.";
            }
            IsEditing = false;
        }
        
    }
    
    private bool _isRemoving;
    
    public bool IsRemoving {
        get => _isRemoving;
        set => this.RaiseAndSetIfChanged(ref _isRemoving, value);
    }

    public void Delete() {
        if (!IsRemoving) {
            IsRemoving = true;
            IsEditing = false;
            Confirmation = "Appuyer de nouveau pour confirmer la suppression.\nPour annuler, cliquer à côté du boutton.";
        } else {
            Confirmation = "";
            if (SelectedKnowledge != null) {
                ListKnowledge.Clear();
                Database.DeleteKnowledge(_selectedKnowledge);
                ListKnowledge = Database.GetKnowledges(ListKnowledge);
                SelectedKnowledge = null;
                this.RaisePropertyChanged(nameof(CompleteEntry));
            } else {
                Confirmation = "Rien a supprimer";
            }
            IsRemoving = false;
        }
    }
    
    private string _confirmation;
    public string Confirmation {
        get => _confirmation;
        set => this.RaiseAndSetIfChanged(ref _confirmation, value);
    }
    
    public string CompleteEntry {
        get => SelectedKnowledge.data.CompletTranslate;
    }

    public ICommand OnGlobalClickCommand { get; }
    
    public void OnGlobalClick()
    {
        IsEditing = false;
        IsRemoving = false;
        Confirmation = "";
    }
    
    public void Refresh() {
        ListKnowledge.Clear();
        ListKnowledge = Database.GetKnowledges(ListKnowledge);
    }

    public void Completed() {
        if (SelectedKnowledge != null) { 
            SelectedKnowledge.data.complet = !SelectedKnowledge.data.complet;
            this.RaisePropertyChanged(nameof(CompleteEntry)); 
        }
    }
    
    public ManageKnowledgeViewModel() {
        OnGlobalClickCommand = ReactiveCommand.Create(OnGlobalClick);
    }
    
    
}