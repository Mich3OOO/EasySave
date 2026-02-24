using System;
using System.IO;

namespace EasySave.Models;

/// <summary>
/// Class representing a saved job, it holds the information about a backup 
/// job that can be executed later
/// </summary>
public class SavedJob   
{
    private int _id;
    private string _name;
    private string _source;
    private string _destination;
    


    public string GetName()
    {
        return this._name;
    }
    public string GetDestination()
    {
        return this._destination;
    }
    public string GetSource()
    {
        return this._source;
    }


    /// <summary>
    /// Properties for the fields, they are used to set the fields from outside 
    /// the class, they are not used to get the fields because we want to control 
    /// the setting of the destination and source fields
    /// </summary>
    public int Id { get => _id; set => _id = value; }
    public string Name { get => _name; set => _name = value; }
    public string Destination { get => _destination; set => SetDestination(value); }
    public string Source { get => _source; set => _source = value; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="source"></param>
    /// <param name="destination"></param>
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

    /// <summary>
    /// Default constructor initializing with default values
    /// </summary>
    public SavedJob()
    {
        _id = -1;
        _name = "";
        _source = "./";
        _destination = "./";
    }

    /// <summary>
    /// Copy constructor, it initializes the fields with the values of the 
    /// given SavedJob object
    /// </summary>
    /// <param name="savedJob"></param>
    public SavedJob(SavedJob savedJob)  
    {
        _id = savedJob._id;
        _name = savedJob._name;
        _source = savedJob._source;
        _destination = savedJob._destination;

    }

    /// <summary>
    /// Method to set the destination field, it checks if the given destination 
    /// is a valid path before setting it, if it's not a valid path, it throws an exception
    /// </summary>
    /// <param name="destination"></param>
    /// <exception cref="Exception"></exception>
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

    /// <summary>
    /// Method to set the source field, it checks if the given source is a 
    /// valid path before setting it, if it's not a valid path, it throws an exception
    /// </summary>
    /// <param name="source"></param>
    /// <exception cref="Exception"></exception>
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

    public override string ToString()   // Override the ToString method to return a string representation of the SavedJob object, it is used to display the SavedJob object in the UI
    {
        return $"{_id} -  {_name} | {_destination} -> {_source}";
    }

}