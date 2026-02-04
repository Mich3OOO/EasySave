namespace EasySave.Models;

/// <summary>
/// États possibles d'une sauvegarde
/// </summary>
public enum StateLevel
{
    /// <summary>
    /// Sauvegarde en cours d'exécution
    /// </summary>
    Active,
    
    /// <summary>
    /// Sauvegarde en pause ou en attente
    /// </summary>
    Inactive,
    
    /// <summary>
    /// Sauvegarde terminée
    /// </summary>
    End,
}