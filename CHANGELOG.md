# Anarchy-BepInEx Change Log
## Patch V1.2.1 
* Fixed vanilla Trees not being overriden by spawning growables. (Note if you add a prop with anarchy conflicting with a standalone prop that wasn't placed with Anarchy, the standalone will be overriden).
* PreventOverride Component will no longer be added in the editor.
* PreventOverride Component will no longer be added while brushing. (Need to test).
* Added some additional checks for JS functions.
* Added a single frame delay after trying to execute the ui.js file for the first time.
* Removed trees and plants from query for PreventCullingSystem to improve performance.
* Fixed cranes not being overriden appropriately.
* Fixed disappearing tooltips.

## Update V1.2.0 2024/01/07
* Anarchy now has an opt-in option to allow placing multiple copies of unique buildings using the normal UI menu with or without Anarchy enabled. The effects of these buildings stack!
* Taxiways added to Targeting markers with bulldozer.

## Update V1.1.0 2023/12/29
* Allows you to place vegetation and props (with DevUI 'Add Object' menu) overlapping or inside the boundaries of other objects and close together.
* Icon to show and EXCLUSIVELY target invisible paths/markers with the bulldozer and remove invisible paths/markers.
* Icon to EXCLUSIVELY target surfaces with the bulldozer and remove with one click. With Anarchy, this also can remove surfaces in buildings.
* Tooltips (Thanks to Algernon)
* Works with Line Tool from ⁠Line Tool for CS2 by Algernon

## Pach V1.0.1 2023/12/18
* If you have Bypass Validation on, Anarchy off, and are using a tool that works with Anarchy, then Bypass Validation will be overridden and forced to be off. 

## Initial Release V1.0.0 2023/12/16
* Anarchy disables error checks for tools in a way that the errors are not shown at all.
* Optional Tool icon
* Keyboard shortcut (Ctrl+A)
* Optional flaming chirper
* Option to automatically enable with bulldozer
* Optional mouse tooltip
* Icons for game manipulation and bypass confirmation for bulldozer.
