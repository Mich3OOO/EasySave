using System.Data;
using EasySave.Models;

namespace EasySave.Interfaces;

public interface IEventListener // Interface for event listeners that respond to backup events
{
    public void Update(BackupInfo data);
}