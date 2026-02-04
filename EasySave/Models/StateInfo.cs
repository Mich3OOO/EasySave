namespace EasySave.Models;

/// <summary>
/// Informations sur l'état actuel d'une sauvegarde
/// </summary>
public class StateInfo
{
    /// <summary>
    /// Nom du job de sauvegarde
    /// </summary>
    public string JobName;
    
    /// <summary>
    /// Date et heure de la dernière sauvegarde complète
    /// </summary>
    public DateTime LastCompleteSave;
    
    /// <summary>
    /// État actuel de la sauvegarde (Active, Inactive, End)
    /// </summary>
    public StateLevel Status;
    
    /// <summary>
    /// Taille totale de la sauvegarde en octets
    /// </summary>
    public int TotalBackupSize;
    
    /// <summary>
    /// Nombre de fichiers restant à sauvegarder
    /// </summary>
    public int RemainingFiles;
    
    /// <summary>
    /// Taille restante à sauvegarder en octets
    /// </summary>
    public int RemainingSize;
    
    /// <summary>
    /// Chemin du fichier actuellement en cours de copie
    /// </summary>
    public string CurrentFileSource;
    
    /// <summary>
    /// Destination du fichier actuellement en cours de copie
    /// </summary>
    public string CurrentFileDestination;
}
