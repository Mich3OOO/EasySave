# 💾 EasySave - Solution de Sauvegarde ProSoft

## 📋 Contexte du Projet

Ce projet s'inscrit dans le cursus PGE A3 FISA INFO - Génie Logiciel 2526 pour le module de Programmation Système.

Développé pour l'éditeur de logiciels ProSoft, EasySave est une solution de sauvegarde destinée à être commercialisée (200 €HT + maintenance). Le projet suit un cycle de développement itératif simulant une évolution logicielle réelle, passant d'une application Console à une interface graphique complète avec gestion du parallélisme.

## 🚀 Fonctionnalités Principales

L'application permet de gérer des travaux de sauvegarde (complets ou différentiels) avec une gestion précise des logs et des états en temps réel.

### Capacités Générales

* Création de travaux de sauvegarde : Source, Cible, Type (Complet/Différentiel).
* Multilingue : Interface disponible en Français et Anglais.
* Journalisation (Logging) : Historique des actions dans `EasyLog.dll` (JSON/XML).
* État en temps réel (State) : Suivi de la progression des sauvegardes (JSON).
* Ligne de commande : Exécution via arguments (ex: `EasySave.exe 1-3`).

### 📅 Roadmap des Versions (Cycle de Vie)

Le développement est découpé en 3 livrables majeurs :

| Fonctionnalité | Version 1.0 | Version 1.1 | Version 2.0 | Version 3.0 |
| --- | --- | --- | --- | --- |
| Interface | Console | Console | GUI (WPF/MVVM) | GUI (WPF/MVVM) |
| Nb. Travaux | 5 max | 5 max | Illimité | Illimité |
| Format Logs | JSON | JSON / XML | JSON / XML | JSON / XML (Centralisé Docker) |
| Exécution | Séquentielle | Séquentielle | Séquentielle | Parallèle |
| Cryptage | Non | Non | Oui (CryptoSoft) | Oui (Mono-instance) |
| Logiciel Métier | Non | Non | Interdiction lancement | Pause automatique |
| Priorité Fichiers | Non | Non | Non | Oui |

## 🛠️ Stack Technique

* Langage : C#
* Framework : .NET 8.0
* Environnement : Visual Studio 2022
* Interface Graphique (V2+) : WPF (Architecture MVVM)
* Modélisation : ArgoUML
* Composants externes :
* `EasyLog.dll` : Librairie de gestion des logs.
* `CryptoSoft` : Logiciel de cryptage tiers (Mono-instance en V3).
