# EasySave - ProSoft Backup

![C#](https://img.shields.io/badge/C%23-11-239120)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)
![WPF](https://img.shields.io/badge/WPF-MVVM-blue)
![Visual Studio](https://img.shields.io/badge/Visual_Studio-2026-purple)

## Project's Context

A3 curriculum project for FISAI in the Software Engineering block

EasySave is is a backup solution aiming for commercialization. The project followed an iterative development cycle simulating real software evolution, transitioning from a Console application to a full graphical interface with parallelism management.

## Main Functionnality

The app allows de manager backup jobs (complete or differential) with log management and real-time state tracking.

### General Features

* Multi-instance: Support for multiple backup jobs running simultaneously.
* Creation of save jobs : Source, Target, Type (Complete/Differential).
* Multilingual: Interface available in English and French.
* Logging : Daily log files with detailed information with `EasyLog.dll` (JSON/XML).
* State Tracking : Real-time tracking of backup progress (JSON).

### Versions' Roadmap (Life Cycle)

The development is divided into 3 major releases:

The following table details the evolution of features across the different iterations of the project :

| Function | Version 1.0 | Version 1.1 | Version 2.0 |
| :--- | :--- | :--- | :--- |
| **Grahpical Interface** | Console | Console | Graphical (Avalonia) |
| **Multilingual** | English and French | English and French | English and French |
| **Save Jobs** | Limited to 5 | Limited to 5 | Unlimited |
| **Daily Log File** | Yes | Yes | Yes (Informations about encryption time) |
| **use of a dll for logging** | Yes | Yes | Yes |
| **State File** | Yes | Yes | Yes |
| **Bakcup Operation** | Mono ou séquential | Mono ou séquential | Mono ou séquential |
| **Stop if business software is detected** | Non | Non | Yes |
| **Command Line** | Yes | Yes | Identical to version 1.0 |
| **Use of « CryptoSoft »** | No | No | Yes |


## Technical Stack

* Language : C#
* Framework : .NET 8.0
* Environment : Visual Studio 2026
* Graphical Interface (V2+) : Avalonia (MVVM architecture)
* Modelisation : Draw.io
* External Components :
  * `EasyLog.dll` : Logs managment library.
  * `CryptoSoft` : Third party encrypting software (7zip).

## Centralized Logging with EasyLog API

You can choose to use the EasyLog API to add centralized logging on an external server. You will need a container with the API (deployment described bellow) and to configure the API's URL in the app settings.

To deploy the EasyLog API in a Docker container, you need to execute those commands :

```
cd /EasyLogAPI
docker compose up
```

Then set the API URL in the app settings to point to your server (e.g. `http://localhost:8080/api/logs`).

## How to use EasyLog

EasyLog is a C# logging library designed to provide logging to EasySave.

### Features

- **Singleton** pattern: a single shared instance across the application via `Logger.GetInstance()`.
- **Thread-safe**: uses a Mutex to safely handle concurrent writes from parallel backup jobs.
- **Daily log files**: one file per day, named `yyyy-MM-dd_log.<format>`, stored in the `./logs` directory (/app/logs in a container).
- **Format support**: `json`, `xml`, or plain `txt` — determined by the `format` parameter passed on each call.

### Usage

In order to write logs with EasyLog, you need to add a reference to the `EasyLog.dll` in your project and then use the `Logger` class as follows:
```
Logger.GetInstance().Log(logText, formatExtension);
```
- `logText` is a string containing the log message. It needs to be already transformed, EasySave Log() method write it as is.
- `formatExtension` is the extension of the log file. Currently, JSON and XML are supported, any other format will turn into a txt.