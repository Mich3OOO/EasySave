using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using EasySave.Models.Exepctions;
using Microsoft.VisualBasic.CompilerServices;

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
    private bool _isSelected = false;


    public bool IsSelected
    {
        get => _isSelected;
        set { _isSelected = value; }
    }

    /// <summary>
    /// Properties for the fields, they are used to set the fields from outside 
    /// the class, they are not used to get the fields because we want to control 
    /// the setting of the destination and source fields
    /// </summary>
    public int Id
    {
        get
        {
            if (_id < 0) throw new InvalidOperationException("class property not set properly expected an id >= 0");
            return _id;
            
        }
        set
        {
            if (value < 0) throw new ValidationException("class property not set properly expected an id >= 0");
            _id = value;
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            if (string.Empty == value) throw new UserException("error_bad_jobName");
            _name = value;
        }
    }

    public string Destination
    {
        get => _destination;
        set
        {
            if (!Path.Exists(value)) throw new UserException("invalid_destination");
            _destination = value;
        }
    }

    public string Source
    {
        get => _source;
        set
        {
            if (!Path.Exists(value)) throw new UserException("invalid_source");
            _source = value;
        }
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public SavedJob(int id, string name, string source, string destination)
    {
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
    public SavedJob(SavedJob savedJob)
    {
        _id = savedJob._id;
        _name = savedJob._name;
        _source = savedJob._source;
        _destination = savedJob._destination;
    }


    /// <summary>
    /// Override the ToString method to return a string representation of the 
    /// SavedJob object, it is used to display the SavedJob object in the UI
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{_id} -  {_name} | {_destination} -> {_source}";
    }
}