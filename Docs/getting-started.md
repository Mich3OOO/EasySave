# Getting Started

## Prerequisites

- .NET 8.0 
- Visual Studio 2022
- Git

## Installation

1. Clone the repository:
   ```
   git clone https://github.com/Mich3OOO/EasySave.git
   ```
2. Open the projet in your Visual Studio 2022.

## Running the Application

Set `EasySave` as the startup project in your IDE.

## Building the Documentation

The documentation is generated using DocFX.

1. Install DocFX globally:
   ```
   dotnet tool install -g docfx
   ```
2. Build and serve the docs locally:
   ```
   docfx docfx.json --serve
   ```
3. Open `http://localhost:8080` in your browser.