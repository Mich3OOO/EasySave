using EasySave.Interfaces;

namespace EasySave.Models;

/// <summary>
/// Gère les états des sauvegardes et écoute les événements de progression
/// </summary>
public class StateManager: IEventListener
{
    /// <summary>
    /// Liste des états de toutes les sauvegardes en cours
    /// </summary>
    private List<StateInfo> _states;
    
    /// <summary>
    /// Chemin du fichier où les états sont sauvegardés
    /// </summary>
    private readonly string _statePath;
    
    /// <summary>
    /// Met à jour l'état d'une sauvegarde suite à un événement
    /// </summary>
    /// <param name="data">Données de progression de la sauvegarde</param>
    public void Update(BackupInfo data)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Sauvegarde les états actuels dans un fichier
    /// </summary>
    private void Save()
    {
        throw new NotImplementedException();
    }
}