## DNightmare's SeedFinder

One could call it a combination of StarMapTools, ResourceSpreadsheetGen and my own little twists added to it.

The goal will be to have a handy little pre-game tool to hunt for specific seeds ("starter" "speedrun" "endgame" "make-all" etc) and export given seeds in a formatted json structure.

With galaxy & mod informations, resources (configurable in levels where 0 = no information, 1 = explorer mode -> only O-Types or if there are enough unipolars, 2 = all), etc so it's shareable and even possible to feed some web-version of the starmap with it for out of game lookup / searching.

For now, consider this pre-alpha and it doesn't do that much apart from logmessages etc but it will hopefully grow with each commit.
Input, additions, pull-requests, etc.. VERY welcome.

## Changelog
### 0.2.0
- Added more settings to control export of data
- Plugin now creates json-files with rudimentary galaxydata

### 0.1.0
- Just to get some version out there for help & feedback.
- It can scan and stop once conditions are met & outputs some info into the ```LogOutput.log``` file of BepInEx