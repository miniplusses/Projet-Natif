using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ReactiveUI;
using Tp2Application.Model;

namespace Tp2Application.ViewModels; 

public class KnowledgeResearchViewModel : PageViewModel {

    private KnowledgeViewModel parent;
    
    public KnowledgeResearchViewModel(KnowledgeViewModel knowledgeViewModel) {
        MenuItemClickCommand = ReactiveCommand.Create<string>(OnMenuItemClick);
        parent = knowledgeViewModel;
        NavigateToDetailsCommand = ReactiveCommand.Create(NavigateToDetails);
    }
    
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

    private ObservableCollection<Knowledge> _listKnowledge = new();
    
    public ObservableCollection<Knowledge> ListKnowledge {
        get => _listKnowledge;
        set => this.RaiseAndSetIfChanged(ref _listKnowledge, value);
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

    private string _selectedCategorie;
    
    public string SelectedCategorie {
        get => _selectedCategorie;
        set => this.RaiseAndSetIfChanged(ref _selectedCategorie, value);
    }
    
    private Knowledge _selectedKnowledge;
    
    public Knowledge SelectedKnowledge {
        get => _selectedKnowledge;
        set => this.RaiseAndSetIfChanged(ref _selectedKnowledge, value);
    }
    
    public ICommand MenuItemClickCommand { get; }
    
    private void OnMenuItemClick(string categorie) {
        SelectedCategorie = categorie;
    }
    
    
    private string _search;
    
    public string Search {
        get => _search;
        set => this.RaiseAndSetIfChanged(ref _search, value);
    }
    
    public void SearchKnowledge() {
        ListKnowledge.Clear();
        if (Search != null && Search.Length > 0) {
            ListKnowledge = Database.SearchKnowledges(ListKnowledge, Search);  
        } else { 
            ListKnowledge = Database.GetKnowledges(ListKnowledge);
        }
    }
    
    
    private string _advancedSearch;
    
    public string AdvancedSearch {
        get => _advancedSearch;
        set => this.RaiseAndSetIfChanged(ref _advancedSearch, value);
    }
    
    public void AdvancedSearchKnowledge() {
        ListKnowledge.Clear();
        ErrorAdvancedSearch = "";
        if (AdvancedSearch != null && AdvancedSearch.Length > 0) {
            if (SelectedCategorie != null && SelectedCategorie != "") {
                if (SelectedCategorie == "Développeur" || SelectedCategorie == "Éditeur" ||
                    SelectedCategorie == "Franchise" || SelectedCategorie == "Parution") {
                    ListKnowledge = Database.AdvanceSearchKnowledges(ListKnowledge, AdvancedSearch, new []{"developpement", RemoveDiacritics(SelectedCategorie).ToLower()});  
                }
                else {
                    ListKnowledge = Database.AdvanceSearchKnowledges(ListKnowledge, AdvancedSearch, new []{RemoveDiacritics(SelectedCategorie).ToLower()});
                }
            }
            else {
                ErrorAdvancedSearch = "Veuillez sélectionner une catégorie";
                ListKnowledge = Database.GetKnowledges(ListKnowledge);
            }
        } else {
            ListKnowledge = Database.GetKnowledges(ListKnowledge);
        }
        
    }

    private string _errorAdvancedSearch;
    
    public string ErrorAdvancedSearch {
        get => _errorAdvancedSearch;
        set => this.RaiseAndSetIfChanged(ref _errorAdvancedSearch, value);
    }
    
    public string RemoveDiacritics(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        text = text.Normalize(NormalizationForm.FormD);
        var chars = text.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
        return new string(chars).Normalize(NormalizationForm.FormC);
    }
    
    public ICommand NavigateToDetailsCommand { get; }

    public void NavigateToDetails() {
        parent.Navigate("DetailsViewModel", SelectedKnowledge);
    }
}