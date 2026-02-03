![C#](https://img.shields.io/badge/C%23-11-239120)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)
![WPF](https://img.shields.io/badge/WPF-MVVM-blue)
![Visual Studio](https://img.shields.io/badge/Visual_Studio-2026-purple)

# EasySave - Sauvegarde ProSoft

## Contexte du Projet

Projet du cursus A3 FISA INFO Bloc Génie Logiciel

Cest une solution de sauvegarde destinée à être commercialisée. Le projet suit un cycle de développement itératif simulant une évolution logicielle réelle, passant d'une application Console à une interface graphique complète avec gestion du parallélisme.

## Fonctionnalités Principales

L'application permet de gérer des travaux de sauvegarde (complets ou différentiels) avec une gestion précise des logs et des états en temps réel.

### Capacités Générales

* Création de travaux de sauvegarde : Source, Cible, Type (Complet/Différentiel).
* Multilingue : Interface disponible en Français et Anglais.
* Journalisation (Logging) : Historique des actions dans `EasyLog.dll` (JSON/XML).
* État en temps réel (State) : Suivi de la progression des sauvegardes (JSON).
* Ligne de commande : Exécution via arguments (ex: `EasySave.exe 1-3`).

### Roadmap des Versions (Cycle de Vie)

Le développement est découpé en 3 livrables majeurs :

Le tableau suivant détaille l'évolution des fonctionnalités à travers les différentes itérations du projet :

| Fonction | Version 1.0 | Version 1.1 | Version 2.0 |
| :--- | :--- | :--- | :--- |
| **Interface Graphique** | Console | Console | Graphique (WPF) |
| **Multi-langues** | Anglais et Français | Anglais et Français | Anglais et Français |
| **Travaux de sauvegarde** | Limité à 5 | Limité à 5 | Illimité |
| **Fichier Log journalier** | Oui | Oui | Oui (Infos suppl. sur temps de cryptage) |
| **Utilisation d'une DLL pour le log** | Oui | Oui | Oui |
| **Fichier Etat** | Oui | Oui | Oui |
| **Fonctionnement Sauvegarde** | Mono ou séquentielle | Mono ou séquentielle | Mono ou séquentielle |
| **Arrêt si logiciel métier détecté** | Non | Non | Oui |
| **Ligne de commande** | Oui | Oui | Identique version 1.0 |
| **Utilisation de « CryptoSoft »** | Non | Non | Oui |


## Stack Technique

* Langage : C#
* Framework : .NET 8.0
* Environnement : Visual Studio 2022
* Interface Graphique (V2+) : WPF (Architecture MVVM)
* Modélisation : Darw.io
* Composants externes :
* `EasyLog.dll` : Librairie de gestion des logs.
* `CryptoSoft` : Logiciel de cryptage tiers (Mono-instance en V3).
