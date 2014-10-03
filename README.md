Zicores Trading Post Notifier R16
=============================

Current release **R16**: http://notifier.zicore.de/dl/r16/ZicoresTradingPostNotifier.zip

For the changelog and the latest releases visit the trading post notifiers website: [notifier.zicore.de](http://notifier.zicore.de).

Test Release
=========
All I can say is, I expect bugs in this release.

Search Service
=========
The old search used to make requests to the trading post servers of arena net or gw2spidy.com, with this new search service, I can search on a local DB, which is a json file currently loaded into ram on startup. This means a short wait time until the search is usable at start. That also means, the search is way more efficient and can provide items faster.
This also includes more ways to search in future releases.

Official Guild Wars 2 API
=========
Since the old inofficial api got replaced by another interface, I decided to implement the official API's at first.
As far as I know, the new API supports accounts and characters at some point, so I can probably implement transactions with the official API soon. **Until then transactions and anything related to personalized data, is disabled.**

Blend SDK
=========
You need to install the Blend WPF SDK for .Net 4.0: http://www.microsoft.com/en-us/download/details.aspx?id=10801 otherwise you can't build or debug this application.
(System.Windows.Interactivity.dll)

Features
========
* Notifications
* Multiple rules for every item
* Item search
* Recipe view
* Watchlist
* Item-name filter
* Prices
* Volumes
* Margin
* Gem/Gold exchange calculator
* Gem price notifications
* Timeout notifications

Currently not supported
========
* Current transactions (buying/selling/bought/sold)
* Transaction notifications
* Trading Post Dataprovider

Dataprovider (Obsolete)
============
* GW2Spidy.com as Dataprovider

This requests data from gw2spidy developed by Drakie. 
While using this dataprovider, there is no way to track what you're doing.
However the amount of requests is limited, there is no access to transactions and some features only work with the official trading post dataprovider.

* The official Trading Post as Dataprovider

The official Trading Post runs as a website in the Guild Wars 2 client. By knowing that, it's quite easy to replicate the requests and get the data.
To request data, a session key is required, which is requested by the Guild Wars 2 client.
This application scans the memory of Guild Wars 2 to obtain this session key and caches it.
It will use the gathered session key, as long as it's valid. So in best case the memory of GW2 is read only once a few days.
This dataprovider unlocks all features of this application.

* Custom dataprovider

Since my project is open source now, everyone could easily implement their own dataprovider, given the api remains similar.

View Technology
===============
I had a lot troubles with WinForms, to fit things, how i would like to see them.
So i decided to step up and switched to WPF MVVM, and you see what cames out.
I tried to stick to MVVM as far as possible, but it's not possible in every situation.
For example: Saving the column order and column width is a pain. 
You find the ColumnHelper in every code-behind file (if there is a list) to take care of this problem.
So atleast the code remains readable there.

Another Platforms
=================
The data api is mostly view independent. So it's possible to adapt it to mono and develop a gui for mac/linux.
But i'm not the one who will do this.

GW2DB
=====
The notifier takes use of gw2db's recipe and item data for the recipe view.
Recipes are going to be replaced as soon as the new API supports them.

GW2Spidy
========
All items are linking to gw2spidy.com to view the corresponding chart. Thanks to drakie for your help.
