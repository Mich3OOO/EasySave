namespace EasySave.Models;

public class SavedJob
{
    private int _id;
    private string _name;
    private string _destination;
    private string _source;
    
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