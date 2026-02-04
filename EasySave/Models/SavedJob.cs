namespace EasySave.Models;

public class SavedJob
{
    private int _id;
    private string _name;
    private string _destination;
    private string _source;
    
    public int Id { get => _id; set => _id = value; }
    public string Name { get => _name; set => _name = value; }
    public string Destination { get => _destination; set => SetDestination(value); }
    public string Source  { get => _source; set => _source = value; }


    public SavedJob(int id, string name, string destination, string source)
    {
        _id = -1;
        _name = "";
        _destination = "";
        _source = "";
        
        Id = id;
        Name = name;
        Destination = destination;
        Source = source;
    }

    public void SetDestination(string destination)
    {
        if (Path.IsPathRooted(destination))
        {
            _destination = destination;
        }
        else
        {
            throw new Exception("Invalid destination path"); 
        }
    }
    
    public void SetSource(string source)
    {
        if (Path.IsPathRooted(source))
        {
            _source = source;
        }
        else
        {
            throw new Exception("Invalid destination path"); 
        }
    }
    
}