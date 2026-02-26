# Introduction

Welcome to the **EasySave** developers' documentation! 

EasySave is a professional backup solution developed by Charlie WIART, Marine MAZOU, Michael SEPULVEDA GONZALEZ, Nolan JOURDAN and Victor MARIE.

The project followed an iterative development cycle, from a Console application to a Graphical User Interface (Avalonia) with parallelism for backups.

This documentation is generated using **DocFX** and is intended for developers.

## Project Overview

EasySave allows users to create manage backup jobs (complete or differential) with logging and real-time state tracking.

### Key Features
- **Backup Jobs Management**: Define Source, Target, and Type (Complete/Differential).
- **Multilingual Support**: App available in English and French.
- **Logging**: Action history managed by `EasyLog.dll` (JSON/XML/TXT formats).
- **Real-time State**: Tracking of backup progress (JSON format).
- **Business Software Detection**: Pauses or prevent backups if specified business software is running.
- **Encryption**: Integration with `CryptoSoft` (7zip) to secure backups.

## Technical Stack

- **Language**: C#
- **Framework**: .NET 8.0
- **GUI**: Avalonia using the MVVM architecture pattern.
- **External Components**:
  - `EasyLog.dll`: Custom library for log management.
  - `CryptoSoft`: Third-party encryption software (7zip).
