README – Inventory GUI

Formål

Et simpelt lager- og ordresystem lavet som GUI i C#.
Programmet viser hvordan ordrer behandles, flyttes fra kø til færdige, og hvordan total omsætning opdateres automatisk.

Funktioner

To DataGrids: “Kø (Queued)” og “Behandlede (Processed)”

Knap til Processér næste ordre

Viser total omsætning løbende

Lager med både UnitItem (stk.) og BulkItem (kg)

Kundeoplysninger og ordrelinjer

Automatisk opdatering af GUI med ObservableCollection

Bruger DataGrid fra Avalonia til visning

Kørsel

Åbn projektmappen og kør:

dotnet run


Programmet åbner et vindue med to tabeller.
Klik på “Processér næste ordre” for at flytte en ordre fra kø til behandlet og se omsætningen stige.

Filer

App.axaml / App.axaml.cs

MainWindow.axaml / MainWindow.axaml.cs

MainWindowViewModel.cs

Domain.cs (models for varer, ordrer, lager m.m.)

Screencast

En kort video (10 sek.) viser hvordan en ordre behandles i GUI’en, og total omsætning opdateres.

Bemærkning

Fundamentet til programmet og denne ReadMe er lavet med hjælp fra AI, derefter tilpasset til opgaven.
