using System;
using System.IO;

namespace EasySave.Models;

public class SavedJob   // Class representing a saved job, it holds the information about a backup job that can be executed later
{
    private int _id;
    private string _name;
    private string _source;
    private string _destination;
    public bool IsDifferential;


    public string GetName() // Getters for the fields, they are used to access the fields from outside the class, they are not properties because we want to control the setting of the destination and source fields
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

    public bool GetIsDifferential()
    {
        return this.IsDifferential;
    }

    // Properties for the fields, they are used to set the fields from outside the class, they are not used to get the fields because we want to control the setting of the destination and source fields
    public int Id { get => _id; set => _id = value; }
    public string Name { get => _name; set => _name = value; }
    public string Destination { get => _destination; set => SetDestination(value); }
    public string Source  { get => _source; set => _source = value; }


    public SavedJob(int id, string name, string source, string destination) // Constructor that takes all the fields as parameters, it initializes the fields with the given values, it also sets default values for the fields before setting them with the given values to ensure that they are initialized even if the given values are invalid
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

    public SavedJob()   // Default constructor, it initializes the fields with default values
    {
        _id = -1;
        _name = "";
        _source = "./";
        _destination = "./";
    }
    
    public SavedJob(SavedJob savedJob)  // Copy constructor, it initializes the fields with the values of the given SavedJob object
    {
        _id = savedJob._id;
        _name = savedJob._name;
        _source = savedJob._source;
        _destination = savedJob._destination;

    }

    public void SetDestination(string destination)  // Method to set the destination field, it checks if the given destination is a valid path before setting it, if it's not a valid path, it throws an exception
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
    
    public void SetSource(string source)    // Method to set the source field, it checks if the given source is a valid path before setting it, if it's not a valid path, it throws an exception
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