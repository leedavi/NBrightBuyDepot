# NBrightBuyDepot
Multiple Depots for NBrightBuy 

Description
-----------

This plugin will allow NBS to send emails to different Depots when an order is placed on the store.


Installation
------------

Install as a normal DNN module.

After installation a UI for Depots will appear in the BO of NBS.

Useage
------

Create the different depots in the UI created on the BO of NBS.

Assign a depot to each client that uses a depot.  This is done in the UI of the Admin>Sales>Client or by importing the data.  (See "Bespoke client fields", "Import")

Import
------

If you place a file called "depotimport.csv" in the "<Portal Root>\Themes\config\Default" folder it will be imported as "Client"/"Depot" link.

The format of the file is Client Email,Depot ref.

E.g.

dcl@nevoweb.com,01
anais@anaismassini.com,02
blake.st@gmail.com,03
am.berthod@free.fr,02


The file will be deleted after importation.

Bespoke Client Fields
---------------------

A bespoke field can be created in the Client UI, to allow depots to be manually assigned to clients.

The template required is included in the NBrightBuyDepot modules and can be found in the Module Folder.  

It is called "clientfields.cshtml".

This file should be placed in the "<Portal Root>\Themes\config\Default" folder.
