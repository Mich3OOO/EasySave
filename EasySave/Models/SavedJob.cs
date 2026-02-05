namespace EasySave.Models;

public class SavedJob
{
    private int _id;
    private string _name;
    private string _destination;
    private string _source;

    public SavedJob(int id, string name, string source, string destination)
    {
        this._id = id;
        this._name = name;
        this._destination = destination;
        this._source = source;
    }

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
}