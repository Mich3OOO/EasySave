namespace EasySave.Models;

public class SavedJob
{
    private int _id;
    private string _name;
    private string _source;
    private string _destination;


    public string getName()
    {
        return this._name;
    }
    public string getDestination()
    {
        return this._destination;
    }
    public string getSource()
    {
        return this._source;
    }
    
    public int Id { get => _id; set => _id = value; }
    public string Name { get => _name; set => _name = value; }
    public string Destination { get => _destination; set => SetDestination(value); }
    public string Source  { get => _source; set => _source = value; }


    public SavedJob(int id, string name, string source, string destination)
    {
        _id = -1;
        _name = "";
        _source = "./";
        _destination = "./";

        Id = id;
        Name = name;
        Source = source;
        Destination = destination;
    }

    public SavedJob()
    {
        _id = -1;
        _name = "";
        _source = "./";
        _destination = "./";
    }
    
    public SavedJob(SavedJob savedJob)
    {
        _id = savedJob._id;
        _name = savedJob._name;
        _source = savedJob._source;
        _destination = savedJob._destination;

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

    public override string ToString()
    {
        return $"{_id} -  {_name} | {_destination} -> {_source}";
    }
    
}