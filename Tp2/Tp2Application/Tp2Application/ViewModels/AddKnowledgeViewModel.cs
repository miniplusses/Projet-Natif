using System;
using System.Runtime.InteropServices.JavaScript;
using ReactiveUI;
using Tp2Application.Model;

namespace Tp2Application.ViewModels; 

public class AddKnowledgeViewModel : PageViewModel {
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
        set => this.RaiseAndSetIfChanged(ref _connectionString, value);
    }
    
    private Knowledge _newKnowledge = new();
    
    public Knowledge NewKnowledge {
        get => _newKnowledge;
        set => this.RaiseAndSetIfChanged(ref _newKnowledge, value);
    }
    
    private string _error;
    
    public string Error {
        get => _error;
        set => this.RaiseAndSetIfChanged(ref _error, value);
    }
    
    private string _success;
    
    public string Success {
        get => _success;
        set => this.RaiseAndSetIfChanged(ref _success, value);
    }
    
    public string CompleteEntry {
        get => NewKnowledge.data.CompletTranslate;
    }
    
    public void Completed() {
        NewKnowledge.data.complet = !NewKnowledge.data.complet;
        this.RaisePropertyChanged(nameof(CompleteEntry));
    }

    public void Add() {
        Error = "";
        Success = "";
        if (NewKnowledge.title != null && NewKnowledge.description != null && NewKnowledge.data != null &&
            NewKnowledge.title != "" && NewKnowledge.description != "") {
            if (NewKnowledge.data.developpement.parution != null && NewKnowledge.data.developpement.parution.Length != 10) {
                Error = "Veuillez remplir les champs correctement.";
            }
            else {
                Database.AddKnowledge(NewKnowledge);
                Success = "La connaissance a été ajoutée avec succès.";
                NewKnowledge = new Knowledge();  
            }
        } else {
            Error = "Veuillez remplir les champs obligatoires ou correctement.";
        }
    }
}