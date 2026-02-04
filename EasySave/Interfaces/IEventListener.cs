using System.Data;
using EasySave.Models;

namespace EasySave.Interfaces;

public interface IEventListener
{
    public void Update(BackupInfo data);
}