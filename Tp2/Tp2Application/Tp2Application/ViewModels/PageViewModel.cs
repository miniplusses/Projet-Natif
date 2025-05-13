namespace Tp2Application.ViewModels; 

public abstract class PageViewModel : ViewModelBase {
    public abstract bool CanNavigateNext { get; protected set; }
    
    public abstract bool CanNavigatePrevious { get; protected set; }
}