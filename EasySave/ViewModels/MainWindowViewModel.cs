using System.Collections.ObjectModel;
using System.Diagnostics;
using EasySave.Models; 

namespace EasySave.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<SavedJob> Jobs { get; set; }

    public MainWindowViewModel()
    {
        Jobs = new ObservableCollection<SavedJob>();
        LoadJobsFromConfig();
    }

    private void LoadJobsFromConfig()
    {
        // ... Ton code de chargement existant ...
        // (Je remets un exemple bidon si tu n'as pas encore le vrai backend connecté)
        // Jobs.Add(new SavedJob { Id = 1, Name = "Test", Source = "A", Destination = "B" });
    }

    // --- COMMANDES MISES À JOUR ---

    // Note le paramètre "SavedJob job" !
    public void CreateJob()
    {
        //Debug.WriteLine("Création d'un nouveau job...");
        // Ici on ouvrira une fenêtre vide
    }

    public void RunJob(SavedJob job)
    {
        //Debug.WriteLine($"Lancement du job : {job.Name} (ID: {job.Id})");
        // Ici tu appelleras ta logique de backup V1
    }

    public void EditJob(SavedJob job)
    {
        //Debug.WriteLine($"Modification du job : {job.Name}");
    }

    public void DeleteJob(SavedJob job)
    {
        //Debug.WriteLine($"Suppression du job : {job.Name}");

        // On le retire de la liste visuelle immédiatement
        Jobs.Remove(job);

        // TODO: Appeler Config.DeleteJob(job.Name) pour le supprimer du json
    }
}