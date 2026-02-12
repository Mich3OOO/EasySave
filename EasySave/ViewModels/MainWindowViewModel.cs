using System.Diagnostics;

namespace EasySave.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to EasySave!";

    public void CreateJob()
    {
        // Plus tard : On ouvrira une fenêtre ou un formulaire pour demander Source/Cible
        Debug.WriteLine("Action : L'utilisateur veut CRÉER un job.");
    }

    public void RunJob()
    {
        // Plus tard : On regardera quel job est sélectionné dans la liste et on le lancera
        Debug.WriteLine("Action : L'utilisateur veut LANCER un job.");
    }

    public void EditJob()
    {
        Debug.WriteLine("Action : L'utilisateur veut MODIFIER un job.");
    }

    public void DeleteJob()
    {
        Debug.WriteLine("Action : L'utilisateur veut SUPPRIMER un job.");
    }
}