# Anarchy

Anarchy disables error checks for tools in a way that the errors are not shown at all.

This mod allows you to place vegetation and props (with DevUI 'Add Object' menu) in buildings, nets or areas.

This mod allows you to place vegetation such as trees and bushes close together.

For consistency within the community, please do not use the term Anarchy to mean something else for CSL2.

## Dependencies
[Unified Icon Library](https://thunderstore.io/c/cities-skylines-ii/p/algernon/Unified_Icon_Library/)

[BepInExPack](https://thunderstore.io/c/cities-skylines-ii/p/BepInEx/BepInExPack/)

## Detailed Descrption
The mod also has: 
* Optional Tool icon
* Keyboard shortcut (Ctrl+A)
* Optional flaming chirper
* Option to automatically enable with bulldozer
* Optional mouse tooltip
* Icons for game manipulation and bypass verification for bulldozer.
* Use Bulldoze tool to remove invisible paths/markers. See below.

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

## Props
Placing standalone props is an unsupported feature of the game. You need DevUI to access the 'Add Object' menu via the home button to place standalong props.

There is an option to "Permenently Prevent Override". This applies to trees and props previously placed with Anarchy. 
If you later interact with these objects with this option on, and Anarchy off, then the object will persist.
If you later interact with these objects with this option off, and Anarchy off, then the object will be overriden per vanilla.
When drawing buildings, nets, or areas the trees and props that are not prevented from overriding, will behave per vanilla.

Props added to buildings or nets may sometimes be culled by the game, and disappear until reloading or something interacts with or near them.
The mod has an option to routinely refresh props that were culled so they don't disappear. This affects perforamnce but you can adjust the frequency.
You can also manually trigger a prop refresh using a button in the options menu.

## Invisible Paths and Markers
Drawing invisible Paths and markers is an unsupported feature of the game. You need DevUI to access the 'Add Object' menu via the home button to draw invisible paths and markers.

With the Bulldoze tool, the DevUI option to "Show Markers", and Anarchy enabled, you can remove Invisible paths and markers, **BUT SAVE BEFORE HAND!**

Removing them in the wrong order may result in a crash to desktop.

## Disclaimer
This mod does NOT allow you to do everything including:
* Use the net tool to draw a bridge through a sea-lane clearance
* If the vanilla net tool would remove an existing network, it will still do that.
* Even if the mod disables the error check, the UI may still prevent you from doing something.
* Does not give additional control for prop placement.

**Please save frequently, in multiple files, and learn to use responsibly.**

## Support
I will respond on the code modding channels on **Cities: Skylines modding Discord**

## Credits 
* yenyang - Mod Author
* Chameleon TBN - Testing, Feedback, Icons, & Logo
* Sully - Testing and Promotional material.
* Algernon - Help with UI, Cooperative Development & Code Sharing
* Bad Peanut - Image Credit for Flaming Chirper
* T.D.W., Klyte45, krzychu124, & Quboid - Cooperative Development & Code Sharing
