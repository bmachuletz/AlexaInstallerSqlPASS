# AlexaInstallerSqlPASS
ALEXA installiert einen SQL Server


1. Alexa... starte "Pass System Installer"
2. Installiere mir einen SQL-Server (Abfrage Servername, IP-Adresse, Instanzname)
3. Amazon startet die Funktion "Function.cs" aus dem Projekt "SqlPassInstallerFunc" 
4. Alexa "INTEND" wird ausgewertet und an das Projekt "SqlPassInstallWebApi" weitergegeben (InitController).
5. Der "InitController" lädt die entsprechenden SLOTS aus dem INTEND und übergibt diese an die Powershell-Skripte.
6. Fertig!!

7. Mit der installierten DEMO-Umgebung kann man dann Sachen machen :)
