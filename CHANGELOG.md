# Anarchy-BepInEx Change Log
## Patch V1.3.1
* Fixed drawing power poles with anarchy enabled causing terrain deformation below them.
* Fixed NetCompositionData for newly built networks while using Anarchy.
* Fixed CTD when trying to relocate owner building while placing building upgrades/extensions/subbuildings.

## Update V1.3.0
* All bulldozing aspects of the mod have been forked over to a new mod called Better Bulldozer.
* While using the net tool Anarchy will now let you violate the clearance of other networks. You should however avoid zoning under low bridges, but there is a new setting to help if you insist.
* Fixed accidentally adding the PreventOverride component to vehicles, cims, households, events, and buildings. This caused issues for parked vehicles getting stuck in a loop and not able to leave the stall. This version should remove all of the improperly added components and not add new ones to those types of entities.
* Fixed plugin data
* Updated Logo
 
## Patch V1.2.2
* Added trees and plants back into query for PreventCullingSystem.

## Patch V1.2.1 
* Fixed vanilla Trees not being overriden by spawning growables. (Note if you add a prop with anarchy conflicting with a standalone prop that wasn't placed with Anarchy, the standalone will be overriden).
* Added some additional checks for JS functions.
* Added a single frame delay after trying to execute the ui.js file for the first time.
* Removed trees and plants from query for PreventCullingSystem to improve performance.
* Fixed cranes not being overriden appropriately.
* Fixed disappearing tooltips.
* Fixed issue where sometimes Anarchy button needed to be toggled twice to change.
* While using the editor, Anarchy will no longer prevent overriding of anything.
* Please note: In the next update all bulldozing aspects will be moved to a different new mod called Better Bulldozer.

## Update V1.2.0 2024/01/07
* Anarchy now has an opt-in option to allow placing multiple copies of unique buildings using the normal UI menu with or without Anarchy enabled. The effects of these buildings stack!
* Taxiways added to Targeting markers with bulldozer.

## Update V1.1.0 2023/12/29
* Allows you to place vegetation and props (with DevUI 'Add Object' menu) overlapping or inside the boundaries of other objects and close together.
* Icon to show and EXCLUSIVELY target invisible paths/markers with the bulldozer and remove invisible paths/markers.
* Icon to EXCLUSIVELY target surfaces with the bulldozer and remove with one click. With Anarchy, this also can remove surfaces in buildings.
* Tooltips (Thanks to Algernon)
* Works with Line Tool from ⁠Line Tool Lite by Algernon

## Patch V1.0.1 2023/12/18
* If you have Bypass Validation on, Anarchy off, and are using a tool that works with Anarchy, then Bypass Validation will be overridden and forced to be off. 

## Initial Release V1.0.0 2023/12/16
* Anarchy disables error checks for tools in a way that the errors are not shown at all.
* Optional Tool icon
* Keyboard shortcut (Ctrl+A)
* Optional flaming chirper
* Option to automatically enable with bulldozer
* Optional mouse tooltip
* Icons for game manipulation and bypass confirmation for bulldozer.
