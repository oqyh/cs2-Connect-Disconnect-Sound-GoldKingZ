## .:[ Join Our Discord For Support ]:.

<a href="https://discord.com/invite/U7AuQhu"><img src="https://discord.com/api/guilds/651838917687115806/widget.png?style=banner2"></a>

# [CS2] Connect-Disconnect-Sound-GoldKingZ (1.1.6)

Connect , Disconnect , Country , City , Message , Sound , Logs , Discord

![cnd](https://github.com/user-attachments/assets/3f21c82b-2aad-44e9-9963-e65db51d7478)

![cnd_owner](https://github.com/user-attachments/assets/411b9588-b735-43f3-b7a9-6bf44c7af408)

![cnd_discord](https://github.com/user-attachments/assets/b0630ae4-77ee-4b6b-9071-c458c46c3aff)


---

## 📦 Dependencies
[![Metamod:Source](https://img.shields.io/badge/Metamod:Source-2d2d2d?logo=sourceengine)](https://www.sourcemm.net)

[![CounterStrikeSharp](https://img.shields.io/badge/CounterStrikeSharp-83358F)](https://github.com/roflmuffin/CounterStrikeSharp)

[![MultiAddonManager](https://img.shields.io/badge/MultiAddonManager-181717?logo=github&logoColor=white)](https://github.com/Source2ZE/MultiAddonManager) [Optional: If You Want Custom Sounds]



[![MySQL](https://img.shields.io/badge/MySQL-4479A1?logo=mysql&logoColor=white)](https://dev.mysql.com/doc/connector-net/en/) [Included in zip]

[![JSON](https://img.shields.io/badge/JSON-000000?logo=json)](https://www.newtonsoft.com/json) [Included in zip]

[![GeoLite2-City.mmdb](https://img.shields.io/badge/GeoLite2--City.mmdb-181717?logo=github&logoColor=white)](https://github.com/P3TERX/GeoLite.mmdb) [Included in zip]

[![MaxMind.Db](https://img.shields.io/badge/MaxMind.Db-2A4365?logo=database&logoColor=white)](https://www.nuget.org/packages/MaxMind.Db) [Included in zip]

[![MaxMind.GeoIP2](https://img.shields.io/badge/MaxMind.GeoIP2-2A4365?logo=database&logoColor=white)](https://www.nuget.org/packages/MaxMind.GeoIP2) [Included in zip]

---

## 📥 Installation

### Plugin Installation
1. Download the latest `Connect-Disconnect-Sound-GoldKingZ.x.x.x.zip` release
2. Extract contents to your `csgo` directory
3. Configure settings in `Connect-Disconnect-Sound-GoldKingZ/config/config.json`
4. Restart your server

---

# ⚙️ Configuration

> [!IMPORTANT]
> **Main Configuration**  
> `../Connect-Disconnect-Sound-GoldKingZ/config/config.json`  
> **Chat & Sound Configuration**  
> `../Connect-Disconnect-Sound-GoldKingZ/config/connect_disconnect_config.json`

## 🛠️ `config/config.json`


<details open>
<summary><b>Main Config</b> (Click to expand 🔽)</summary>

| Property                       | Description                                                        | Values                                                                   | Required                    |
| ------------------------------ | ------------------------------------------------------------------ | ------------------------------------------------------------------------ | --------------------------- |
| `AutoSetPlayerLanguage`        | Auto set player language based on player country                   | `true` = Yes, `false` = No                                               | -                           |
| `Reload_CnD_CommandsInGame`    | Commands to reload Connect/Disconnect plugin                       | `Console_Commands: css_reloadconnect, css_reloadcnd`<br>`Chat_Commands:` | -                           |
| `Reload_CnD_Flags`             | Restricted flags for reload command                                | `SteamIDs: ...`<br>`Flags: ...`<br>`Groups: ...`<br>`""` = Everyone      | `Reload_CnD_CommandsInGame` |
| `Reload_CnD_Hide`              | Hide chat after reload command                                     | `0` = No<br>`1` = Only after success<br>`2` = Always hide                | `Reload_CnD_CommandsInGame` |
| `EarlyConnection`              | Enable early connection of players                                 | `true` = Yes<br>`false` = No                                             | -                           |
| `IgnoreTheseDisconnectReasons` | Ignore specific disconnect reasons (see `disconnect_reasons.json`) | Array of codes, e.g., `[1,54,55]`                                        | -                           |
| `PickSoundsByOrder`            | Play connect/disconnect sounds in order instead of randomly        | `true` = Ordered<br>`false` = Random                                     | -                           |
| `RemoveDefaultDisconnect`      | Remove default disconnect message/icon                             | `0` = No<br>`1` = Remove messages<br>`2` = Remove messages + icon        | -                           |
| `DateFormat`                   | Date format used in messages                                       | e.g., `MM-dd-yyyy`, `dd MM yyyy`                                         | -                           |
| `TimeFormat`                   | Time format used in messages                                       | e.g., `HH:mm`, `HH:mm:ss`, `hh:mm a`                                     | -                           |

</details>

<details>
<summary><b>Connect/Disconnect Sounds Config</b> (Click to expand 🔽)</summary>

| Property                   | Description                                    | Values                                                                                 | Required                   |
| -------------------------- | ---------------------------------------------- | -------------------------------------------------------------------------------------- | -------------------------- |
| `CnDSounds`                | Enable connect/disconnect sounds               | `0` = No<br>`1` = Yes<br>`2` = Yes, togglable enabled<br>`3` = Yes, togglable disabled | -                          |
| `CnDSounds_CommandsInGame` | Commands to toggle sounds (if `CnDSounds=2/3`) | `Console_Commands: css_sound, css_sounds`<br>`Chat_Commands:`                          | `CnDSounds=2 or 3`         |
| `CnDSounds_Flags`          | Restricted flags for toggle command            | `SteamIDs: ...`<br>`Flags: ...`<br>`Groups: ...`<br>`""` = Everyone                    | `CnDSounds_CommandsInGame` |
| `CnDSounds_Hide`           | Hide chat after toggle                         | `0` = No<br>`1` = Only after success<br>`2` = Always hide                              | `CnDSounds_CommandsInGame` |

</details>

<details>
<summary><b>Connect/Disconnect Messages Config</b> (Click to expand 🔽)</summary>

| Property                     | Description                                        | Values                                                                                 | Required                     |
| ---------------------------- | -------------------------------------------------- | -------------------------------------------------------------------------------------- | ---------------------------- |
| `CnDMessages`                | Enable connect/disconnect messages                 | `0` = No<br>`1` = Yes<br>`2` = Yes, togglable enabled<br>`3` = Yes, togglable disabled | -                            |
| `CnDMessages_CommandsInGame` | Commands to toggle messages (if `CnDMessages=2/3`) | `Console_Commands: css_message, css_messages`<br>`Chat_Commands:`                      | `CnDMessages=2 or 3`         |
| `CnDMessages_Flags`          | Restricted flags for toggle command                | `SteamIDs: ...`<br>`Flags: ...`<br>`Groups: ...`<br>`""` = Everyone                    | `CnDMessages_CommandsInGame` |
| `CnDMessages_Hide`           | Hide chat after toggle                             | `0` = No<br>`1` = Only after success<br>`2` = Always hide                              | `CnDMessages_CommandsInGame` |

</details>

<details>
<summary><b>Locally Config</b> (Click to expand 🔽)</summary>

| Property                                     | Description                                    | Values                                                                                                        | Required                  |
| -------------------------------------------- | ---------------------------------------------- | ------------------------------------------------------------------------------------------------------------- | ------------------------- |
| `Log_Locally_Enable`                         | Enable local logging                           | `true` = Yes<br>`false` = No                                                                                  | -                         |
| `Log_Locally_Connect_Format`                 | Connect message format                         | Template placeholders: `{DATE}`, `{TIME}`, `{PLAYERNAME}`, `{STEAMID64}`, `{IP}`, `{CITY}`, etc.              | `Log_Locally_Enable=true` |
| `Log_Locally_Disconnect_Format`              | Disconnect message format                      | Template placeholders: `{DATE}`, `{TIME}`, `{PLAYERNAME}`, `{STEAMID64}`, `{IP}`, `{DISCONNECT_REASON}`, etc. | `Log_Locally_Enable=true` |
| `Log_Locally_DateFormat`                     | Date format for logs                           | e.g., `MM-dd-yyyy`, `dd MM yyyy`                                                                              | `Log_Locally_Enable=true` |
| `Log_Locally_TimeFormat`                     | Time format for logs                           | e.g., `HH:mm:ss`, `hh:mm a`                                                                                   | `Log_Locally_Enable=true` |
| `Log_Locally_AutoDeleteLogsMoreThanXdaysOld` | Auto-delete old logs                           | Number of days (`0` = disable)                                                                                | `Log_Locally_Enable=true` |
| `Cookies_Enable`                             | Save player cookies locally                    | `0` = No<br>`1` = On disconnect<br>`2` = On map change                                                        | -                         |
| `Cookies_AutoRemovePlayerOlderThanXDays`     | Auto-delete inactive cookies older than X days | Number of days (`0` = disable)                                                                                | `Cookies_Enable=1 or 2`   |

</details>

<details>
<summary><b>Discord Config</b> (Click to expand 🔽)</summary>

| Property                         | Description                               | Values                                                                                                                     | Required              |
| -------------------------------- | ----------------------------------------- | -------------------------------------------------------------------------------------------------------------------------- | --------------------- |
| `Discord_Connect_WebHook`        | Connect webhook URL                       | URL string, `""` = disable                                                                                                 | -                     |
| `Discord_Connect_Style`          | Connect message style                     | `0` = Disable<br>`1` = Text only<br>`2` = Text+Link<br>`3` = +Profile Pic<br>`4` = +Separate Date/Time<br>`5` = +Server IP | If webhook active     |
| `Discord_Connect_SideColor`      | Message side color                        | Hex color code, e.g., `0cff00`                                                                                             | Style 2/3/4/5         |
| `Discord_Connect_Format`         | Connect message template                  | Placeholders: `{PLAYERNAME}`, `{SHORTCOUNTRY}`, `{CITY}`, etc.                                                             | Webhook active        |
| `Discord_Disconnect_WebHook`     | Disconnect webhook URL                    | URL string, `""` = disable                                                                                                 | -                     |
| `Discord_Disconnect_Style`       | Disconnect message style                  | Same as connect style                                                                                                      | Webhook active        |
| `Discord_Disconnect_SideColor`   | Message side color                        | Hex color code, e.g., `ff0000`                                                                                             | Style 2/3/4/5         |
| `Discord_Disconnect_Format`      | Disconnect message template               | Placeholders: `{PLAYERNAME}`, `{SHORTCOUNTRY}`, `{CITY}`, `{DISCONNECT_REASON}`, etc.                                      | Webhook active        |
| `Discord_DateFormat`             | Date format for Discord messages          | e.g., `MM-dd-yyyy`, `dd/MM/yy`                                                                                             | If any webhook active |
| `Discord_TimeFormat`             | Time format for Discord messages          | e.g., `HH:mm:ss`                                                                                                           | If any webhook active |
| `Discord_FooterImage`            | Footer image URL for messages             | URL string                                                                                                                 | Style 3/4/5           |
| `Discord_UsersWithNoAvatarImage` | Default avatar for players without avatar | URL string                                                                                                                 | Style 5               |

</details>

<details>
<summary><b>MySQL Config</b> (Click to expand 🔽)</summary>

| Property                               | Description                                    | Values                                                                     | Required         |
| -------------------------------------- | ---------------------------------------------- | -------------------------------------------------------------------------- | ---------------- |
| `MySql_Enable`                         | Enable MySQL                                   | `0` = No<br>`1` = Save on disconnect<br>`2` = Save on map change           | -                |
| `MySql_ConnectionTimeout`              | Connection timeout in seconds                  | Number, e.g., `30`                                                         | `MySql_Enable≠0` |
| `MySql_RetryAttempts`                  | Retry attempts when connection fails           | Number, e.g., `3`                                                          | `MySql_Enable≠0` |
| `MySql_RetryDelay`                     | Delay between retries (seconds)                | Number, e.g., `2`                                                          | `MySql_Enable≠0` |
| `MySql_Servers`           | MySQL server configs                           | Array of servers with `Server`, `Port`, `Database`, `Username`, `Password` | `MySql_Enable≠0` |
| `MySql_AutoRemovePlayerOlderThanXDays` | Auto-delete inactive players older than X days | Number of days (`0` = disable)                                             | `MySql_Enable≠0` |

</details>

<details>
<summary><b>Utilities Config</b> (Click to expand 🔽)</summary>

| Property                | Description                                    | Values                       | Required |
| ----------------------- | ---------------------------------------------- | ---------------------------- | -------- |
| `AutoUpdateSignatures`  | Auto-update signatures in `../Connect-Disconnect-Sound-GoldKingZ/gamedata/`          | `true` = Yes<br>`false` = No | -        |
| `AutoUpdateGeoLocation` | Auto-update geolocation data in `../shared/GoldKingZ/GeoLocation/` | `true` = Yes<br>`false` = No | -        |
| `AutoUpdateDisconnectReasons` | Auto-update `../Connect-Disconnect-Sound-GoldKingZ/config/disconnect_reasons.json`    | `true` = Yes<br>`false` = No | -        |
| `EnableDebug`           | Enable debug in server console                 | `true` = Yes<br>`false` = No | -        |

</details>




## 🛠️ `config/connect_disconnect_config.json`

<details open>
<summary><b>Connect Disconnect Config</b> (Click to expand 🔽)</summary>

### **Configuration**

| Key                             | Description                          | Values/Examples                                                           |
| ------------------------------- | ------------------------------------ | ------------------------------------------------------------------------- |
| `CONNECT_MESSAGE_CHAT`          | Connect message template             | `"{Green}{PLAYERNAME} joined"`                                            |
| `CONNECT_SOUND_VOLUME`          | Connect sound volume (1-100%)        | `"75%"`                                                                   |
| `CONNECT_SOUND`                 | Connect sound file paths             | `["ui/item_acquired.vsnd"]`                                               |
| `DISCONNECT_MESSAGE_CHAT`       | Disconnect message template          | `"{Red}{PLAYERNAME} left"`                                                |
| `DISCONNECT_SOUND_VOLUME`       | Disconnect sound volume (1-100%)     | `"60%"`                                                                   |
| `DISCONNECT_SOUND`              | Disconnect sound file paths          | `["ui/item_drop.vsnd"]`                                                   |

---

### **Placeholders**

| Placeholder      | Description                                      | Example Output                                                     |
| ---------------- | ------------------------------------------------ | ------------------------------------------------------------------ |
| `{NEXTLINE}`     | Creates line break in messages                   | `"Line1{NEXTLINE}Line2"`                                           |
| `{DATE}`         | Current date (from main config format)           | `12-31-2023`                                                       |
| `{TIME}`         | Current time (from main config format)           | `23:59:59`                                                         |
| `{PLAYERNAME}`   | Player's display name                            | `ProPlayer123`                                                     |
| `{STEAMID}`      | SteamID                                          | `STEAM_0:1:122910632`                                              |
| `{STEAMID3}`     | SteamID3                                         | `U:1:245821265`                                                    |
| `{STEAMID32}`    | SteamID32                                        | `245821265`                                                        |
| `{STEAMID64}`    | SteamID64                                        | `76561198206086993`                                                |
| `{IP}`           | Player's IP address                              | `123.45.67.89`                                                     |
| `{CONTINENT}`    | Player's continent                               | `Europe, Asia, Africa, North/South America, Australia, Antarctica` |
| `{LONGCOUNTRY}`  | Full country name                                | `United Arab Emirates`                                             |
| `{SHORTCOUNTRY}` | Country code                                     | `AE`                                                               |
| `{CITY}`         | City name                                        | `Abu Dhabi`                                                        |
| `{DISCONNECT_REASON}`       | Disconnect reason from `disconnect_reasons.json` | `Lost Connection`                                                  |

---

### **Colors**

| Color Tag       | Example                        |
| --------------- | ------------------------------ |
| `{Default}`     | `{Default}Normal Text`         |
| `{White}`       | `{White}Notification`          |
| `{Darkred}`     | `{Darkred}Banned!`             |
| `{Green}`       | `{Green}Connected`             |
| `{LightYellow}` | `{LightYellow}Hint Text`       |
| `{LightBlue}`   | `{LightBlue}Server Note`       |
| `{Olive}`       | `{Olive}Team Chat`             |
| `{Lime}`        | `{Lime}Success Message`        |
| `{Red}`         | `{Red}Error!`                  |
| `{LightPurple}` | `{LightPurple}Special Message` |
| `{Purple}`      | `{Purple}Admin Command`        |
| `{Grey}`        | `{Grey}12:00:00`               |
| `{Yellow}`      | `{Yellow}Warning Message`      |
| `{Gold}`        | `{Gold}[VIP] Player`           |
| `{Silver}`      | `{Silver}Regular Member`       |
| `{Blue}`        | `{Blue}Information`            |
| `{DarkBlue}`    | `{DarkBlue}Moderator Tag`      |
| `{BlueGrey}`    | `{BlueGrey}System Alert`       |
| `{Magenta}`     | `{Magenta}Event Notification`  |
| `{LightRed}`    | `{LightRed}Urgent Alert`       |
| `{Orange}`      | `{Orange}Alert/Warning`        |

</details>

---


## 📜 Changelog

<details>
<summary><b>📋 View Version History</b> (Click to expand 🔽)</summary>
  
### [1.1.6]
- Moved GeoLocation To Shared
- Fix Globals On Reload Plugin Command
- Fix on CnDSounds 1 Not Play Sound
- Fix on CnDMessages 1 Not Show Message
- In connect_disconnect_config.json Changed From {REASON} To {DISCONNECT_REASON}

### [1.1.5]
- Fix Some Bugs And Clean Up
- Fix CustomHooks
- Fix config.json
- Fix RemoveDefaultDisconnect 2 Killfeed Hidden Sometimes
- Fix On config.json Flags 
- Fix On connect_disconnect_config.json Flags 
- Fix Exploit Colors Names In connect_disconnect_config.json
- Changed IgnoreTheseDisconnectReasons From string To int
- Changed PickRandomSounds To PickSoundsByOrder
- Added AutoSetPlayerLanguage
- Added Reload_CnD_CommandsInGame
- Added Reload_CnD_Flags
- Added Reload_CnD_Hide
- Added CnDSounds
- Added CnDSounds_CommandsInGame
- Added CnDSounds_Flags
- Added CnDSounds_Hide
- Added CnDMessages
- Added CnDMessages_CommandsInGame
- Added CnDMessages_Flags
- Added CnDMessages_Hide
- Added AutoUpdateDisconnectReasons
- Added MySql Multiple
  
### [1.1.4]
- Fix Some Bugs
- Fix Toggles
- Fix MySql_AutoRemovePlayerOlderThanXDays = 0
- Cookies_AutoRemovePlayerOlderThanXDays = 0
- Removed DisableLoopConnections
- Added Console css_ ! Commands To Toggle_Messages_CommandsInGame
- Added Console css_ ! Commands To Toggle_Sounds_CommandsInGame
- Added IgnoreTheseDisconnectReasons
- Added Toggle_Messages_Hide
- Added Toggle_Sounds_Hide

### [1.1.3]
- Removed CompatibilityWithCS2Fixes On By Default
- Rework On Players Permissions
- Fix `connect_disconnect_config.json` When Player Have Flag Not Getting His Disconnect (Message/Sounds/Vol)
- Fix Default Of `CONNECT_SOUND_VOLUME`/`DISCONNECT_SOUND_VOLUME` Value If Not Found Now Its `100%` If Not Found

### [1.1.2]
- Added CompatibilityWithCS2Fixes

### [1.1.1]
- Fix Error 'CounterStrikeSharp.API.Core.CCSPlayerController' was not present in the dictionary

### [1.1.0]
#### **Core Improvements**  
- Reworked plugin architecture for better stability
- Fixed connection-related lag spikes
- Added comprehensive config descriptions

#### **Configuration Updates**  
- Added `EarlyConnection` (early player initialization)  
- Added `DisableServerHibernate` (hibernation control)  
- Added `PickRandomSounds` (random sound selection)  
- Added `RemoveDefaultDisconnect` (message+icon removal)  
- Added permission systems for sound/message toggles  
- Added cookie-based player data storage  
- Added MySQL support with auto-cleanup  
- Added automatic signature/geo updates

#### **New Features**  
- Added continent tracking (`{CONTINENT}` placeholder)  
- Added separate Discord disconnect webhook/style  
- Added sound volume controls  
- Added custom disconnect reasons file  
- Added multi-sound support with random selection

#### **Technical Improvements**  
- Added proper sound precaching system  
- Added debug mode toggle  
- Added chat message customization  
- Fixed killfeed icon removal

### [1.0.9]
#### **General Changes**  
- Added `DisableLoopConnections` anti-spam feature  
- Added `RemoveDefaultDisconnect` message control  
- Fixed various minor issues  

#### **Localization**  
- Added console messages for connect/disconnect events  
- Added command permission warnings  
- Added validation messages for SteamID/IP  
- Added geographic unknown state messages  

#### **Cookie System**  
- Added cookie auto-cleanup via `RemovePlayerCookieOlderThanXDays`  

### [1.0.8]
#### **General Changes**  
- Fixed in-game sound toggle commands  
- Improved cookie storage path handling  

#### **Message Formatting**  
- Added connect/disconnect message templates to localization  
- Added `{REASON}` placeholder for disconnect messages  

### [1.0.7]
#### **Logging Improvements**  
- Fixed log auto-deletion system  
- Added separate connect/disconnect message formats  
- Added sound toggle command system  

### [1.0.6]
#### **General Maintenance**  
- Fixed various stability issues  

### [1.0.5]
#### **Discord Integration**  
- Added webhook send modes (1/2/3)  
- Added customizable message side colors  
- Fixed Discord webhook lag issues  

#### **Log Management**  
- Implemented log auto-deletion system  

### [1.0.4]
#### **Console Integration**  
- Added server console logging  
- Improved sound file management  

#### **Format Fixes**  
- Fixed timestamp formatting sequence  
- Improved Steam profile linking in Discord  

### [1.0.3]
#### **Discord Enhancements**  
- Added separate connect/disconnect formats  
- Expanded placeholder support ({TIME}, {DATE}, SteamIDs)  

#### **Stability**  
- Fixed file write permission issues  
- Improved async task handling  

### [1.0.2]
#### **Geolocation**  
- Added detailed country/city tracking  
- Implemented webhook logging system  

### [1.0.1]
#### **Security**  
- Added IP address logging  
- Improved empty field handling  

### [1.0.0]
- Initial plugin release

</details>

---
