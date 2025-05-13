using System;
using ReactiveUI;
using Tp2Application.Model;

namespace Tp2Application.ViewModels; 

public class DetailsViewModel : PageViewModel {
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
    
    private Knowledge _selectedKnowledge;
    
    public Knowledge SelectedKnowledge {
        get => _selectedKnowledge;
        set => this.RaiseAndSetIfChanged(ref _selectedKnowledge, value);
    }
}