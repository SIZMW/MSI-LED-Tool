# MSI-LED-Tool

## Description

This project is forked from [Vipeax/MSI-LED-Tool](https://github.com/Vipeax/MSI-LED-Tool).

This is a tiny project that allows modifying the LEDs of MSI Gaming X cards without all the bloatware that the MSI tooling itself includes. Other MSI cards are very likely to work as well, but right now the code performs a check specifically for the device code of Pascal (GTX 1060/GTX 1070/GTX 1080), Maxwell (GTX 980Ti/GTX 960) and Polaris cards as well as the MSI brand subvendor code.

### Supported Animations

| Animation | Number | Description |
|-----------|--------|-------------|
| Off | 1 | All lights are off |
| No Animation | 2 | All lights are lit a solid color|
| Breathing | 3 | All lights slowly turn on and off over time |
| Flashing | 4 | All lights flash once recurrently |
| Double Flashing | 5 | All lights flash twice recurrently |
| Temperature Based | 6 | Reads the graphics card data and scales between low temperatures (green) and high temperatures (red) based on the min and max of the settings file. Default range is 45C - 85C, meaning anything from 45C and lower is 100% green and anything from 85C and upper is 100% red. It will then colorize from green to yellow to orange to red as your GPU temperature changes. |
| Breathing RGB Cycle | 7 | Same as _Breathing_, except the colors cycle over the basic rainbow colors (ROYGBIV + White) |

If you desire a different effect or any custom mode and you aren't a developer, just [create an issue](https://github.com/SIZMW/MSI-LED-Tool/issues/new) here or [create a new issue on the source project](https://github.com/Vipeax/MSI-LED-Tool/issues/new) describing what you want and time permitting I might try to add it.

## Installation

1. Download the latest release from the _Releases_ section or build the project yourself.
2. Place the `MSI LED Tool.exe`, `Settings.json`, and the `Lib` folder in whichever location is desired.
3. Edit the `regedit.reg` file to point to this desired folder .
4. Edit the `Settings.json` file with your desired color using the _RGB_ values, and set the animation type based on the table above.
  * If needed, you can disable device ID checks by setting the `OverwriteSecurityChecks` setting to `true` when selecting the _Temperature Based_ mode.
5. Run the `MSI LED Tool.exe` and if you like what you see, run `regedit.reg` to register the tool to startup upon Windows boot, after which it will automatically apply your configured settings.

## Releases

For bundled releases, see the _Releases_ area.

## Alternative Projects

* https://github.com/Vipeax/MSI-LED-Tool
* https://github.com/MinDBreaK/MSI-LED-Control
