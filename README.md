# Anarchy

Anarchy disables error checks for tools in a way that the errors are not shown at all.

This mod allows you to place vegetation and props (with DevUI 'Add Object' menu) overlapping or inside the boundaries of other objects and close together.

For consistency within the community, please do not use the term Anarchy to mean something else for CSL2.

Sully has prepared an Amazing! demo video about detailing with Anarchy available on [Youtube](https://www.youtube.com/watch?v=dJiCmFIxPK0).

## Dependencies
[Unified Icon Library](https://thunderstore.io/c/cities-skylines-ii/p/algernon/Unified_Icon_Library/)

[BepInExPack](https://thunderstore.io/c/cities-skylines-ii/p/BepInEx/BepInExPack/)

## Change Log
[Available on Github](https://github.com/yenyang/Anarchy-BepInEx/blob/master/CHANGELOG.md)

## Detailed Descrption
The mod also has: 
* Optional Tool icon
* Keyboard shortcut (Ctrl+A)
* Optional flaming chirper
* Option to automatically enable with bulldozer
* Optional mouse tooltip
* Opt-In Option to allow multiple copies of unique buildings using toolbar menu. Effects of multiple buildings stack!

These features will be moved to a new mod called Better Bulldozer soon:
* Icons for game manipulation and bypass confirmation for bulldozer.
* Icon to show and EXCLUSIVELY target invisible paths/markers with the bulldozer and remove invisible paths/markers. See below.
* Icon to EXCLUSIVELY target surfaces with the bulldozer and remove with one click. With Anarchy, this also can remove surfaces in buildings.

Currently it applies to these tools:
* Object Tool
* Net Tool
* Area Tool (Can exceed limits for specialized industry areas)
* Bulldoze Tool (Option to default Anarchy to ON when activated)
* Terrain Tool (Cross the line within playable area.)
* Upgrade Tool
* Line Tool from [Line Tool Lite](https://thunderstore.io/c/cities-skylines-ii/p/algernon/Line_Tool_Lite/) by Algernon

You can activate anarchy with the keyboard shortcut "Ctrl+A" or with the optional tool icon that only appears while using the above tools.

You can tell anarchy is active using optional Flaming Chirper, the tool icon, or a tooltip.

The following errors will not occur while Anarchy is enabled:
* Overlap Existing
* Invalid Shape
* Long Distance
* Tight Curve
* Already Upgraded
* In Water
* No Water
* Exceeds City Limits (This provides Cross the line Functionality)
* Not On Shoreline
* Already Exists
* Short Distance
* Low Elevation
* Small Area
* Steep Slope
* Not On Border
* No Groundwater
* On Fire
* Exceeds Lot Limits (Editor Only)

If you find an error that you think should be added or if you find a tool that this should also be included, please let me know. 

## Props and Trees
Placing standalone props is an unsupported feature of the game. You need DevUI to access the 'Add Object' menu via the home button to place standalone props.

With Anarchy enabled, you can place props and trees overlapping or inside the boundaries of buildings, nets, areas, other trees, other props, etc. Props and trees placed with Anarchy enabled cannot be overriden later (even if later Anarchy is disabled), but can be removed with bulldozer or brush.

Props overlapping with buildings or nets may sometimes be culled by the game, and disappear until reloading or something interacts with or near them.
The mod has an option to routinely refresh props that were culled so they don't disappear. This affects performance but you can adjust the frequency.
You can also manually trigger a prop refresh using a button in the options menu.

Pro tip: Use the brush mode to remove trees and standalone props. If you unselect the brush snapping option for "Remove only matching type", and right click you can remove them within a radius and it only targets standalone props and trees.

## Invisible Paths and Markers
Drawing invisible paths and markers is an unsupported feature of the game. You need DevUI to access the 'Add Object' menu via the home button to draw invisible paths and markers.

With the Bulldoze tool you can select an icon to show and EXCLUSIVELY target these invisible paths and markers. With this selected and Anarchy enabled you can remove them, **BUT SAVE BEFORE HAND!**

One configuration I built/demolished resulted in a crash to desktop because I left a segment that was too short. Deleting them in a different order worked. 

You cannot demolish invisible paths and markers within buildings.

The mod will automatically toggle the DevUI setting to "Show Markers" when drawing invisible paths.

## Disclaimer
This mod does NOT allow you to do everything including:
* Use the net tool to draw a bridge through a sea-lane clearance
* If the vanilla net tool would remove an existing network, it will still do that.
* Even if the mod disables the error check, the UI may still prevent you from doing something.
* Does not give additional control for prop placement.
* Not much testing is done on the effects of this mod on maps created using the unfinished editor.

**Please save frequently, in multiple files, and learn to use responsibly.**

## Support
I will respond on the code modding channels on **Cities: Skylines Modding Discord**

## Credits 
* yenyang - Mod Author
* Chameleon TBN - Testing, Feedback, Icons, & Logo
* Sully - Testing, Feedback, and Promotional Material.
* Algernon - Help with UI, Cooperative Development & Code Sharing
* Bad Peanut - Image Credit for Flaming Chirper
* T.D.W., Klyte45, krzychu124, & Quboid - Cooperative Development & Code Sharing
* Dante - Testing, Feedback