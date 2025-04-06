## .:[ Join Our Discord For Support ]:.

<a href="https://discord.com/invite/U7AuQhu"><img src="https://discord.com/api/guilds/651838917687115806/widget.png?style=banner2"></a>

# [CS2] Connect-Disconnect-Sound-GoldKingZ (1.1.1)

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

## 🛠️ `config.json`

<details open>
<summary><b>Main Config</b> (Click to expand 🔽)</summary>

| Property | Description | Values | Required |  
|----------|-------------|--------|----------|
| `EarlyConnection` | Enable Early Connection | `true`-Yes<br>`false`-No | - |
| `DisableLoopConnections` | Disable Looping Connections | `true`-Yes<br>`false`-No | - |
| `DisableServerHibernate` | Disable Server Hibernation | `true`-Yes (Recommended)<br>`false`-No | - |
| `PickRandomSounds` | Random Sound Selection | `true`-Random<br>`false`-Sequential | - |
| `RemoveDefaultDisconnect` | Remove Disconnect Messages | `0`-No<br>`1`-Remove messages<br>`2`-Remove messages+icon | - |
| `Toggle_Sounds_CommandsInGame` | Sound Toggle Commands | Example: `!sound,!sounds`<br>`""`-Disable | - |
| `Toggle_Sounds_Flags` | Sound Toggle Flags | Example: `@css/vvip,#css/vvip`<br>`""`-Everyone | `Toggle_Sounds_CommandsInGame` ≠ `""` |
| `Toggle_Messages_CommandsInGame` | Message Toggle Commands | Example: `!message`<br>`""`-Disable | - |
| `Toggle_Messages_Flags` | Message Toggle Flags | Example: `@css/vvip,#css/vvip`<br>`""`-Everyone | `Toggle_Messages_CommandsInGame` ≠ `""` |
| `Default_Sounds` | Default Sound State | `true`-On<br>`false`-Off | - |
| `Default_Messages` | Default Message State | `true`-On<br>`false`-Off | - |
| `DateFormat` | Date Format | Examples: `MM-dd-yyyy` | - |
| `TimeFormat` | Time Format | Examples: `HH:mm:ss` | - |

</details>

<details>
<summary><b>Locally Config</b> (Click to expand 🔽)</summary>

| Property | Description | Values | Required |  
|----------|-------------|--------|----------|
| `Log_Locally_Enable` | Enable Local Logging | `true`-Yes<br>`false`-No | - |
| `Log_Locally_DateFormat` | Log Date Format | Examples: `MM-dd-yyyy` | `Log_Locally_Enable=true` |
| `Log_Locally_TimeFormat` | Log Time Format | Examples: `HH:mm:ss` | `Log_Locally_Enable=true` |
| `Log_Locally_Connect_Format` | Connect Message Format | Template with placeholders<br>`""`-Disable | `Log_Locally_Enable=true` |
| `Log_Locally_Disconnect_Format` | Disconnect Message Format | Template with placeholders<br>`""`-Disable | `Log_Locally_Enable=true` |
| `Log_Locally_AutoDeleteLogsMoreThanXdaysOld` | Auto-Delete Logs | Days to keep<br>`0`-Disable | `Log_Locally_Enable=true` |
| `Cookies_Enable` | Enable Player Cookies | `true`-Yes<br>`false`-No | - |
| `Cookies_AutoRemovePlayerOlderThanXDays` | Auto-Delete Inactive Cookies | Days to keep<br>`0`-Disable | `Cookies_Enable=true` |

</details>

<details>
<summary><b>Discord Config</b> (Click to expand 🔽)</summary>

| Property | Description | Values | Required |  
|----------|-------------|--------|----------|
| `Discord_Connect_WebHook` | Connect Webhook URL | Example URL<br>`""`-Disable | - |
| `Discord_Connect_Style` | Connect Message Style | `0`-Disable<br>`1`-Text only<br>`2`-Text+Link<br>`3`-+Profile Pic<br>`4`-+Separate DT<br>`5`-+Server IP | `Discord_Connect_WebHook` ≠ `""` |
| `Discord_Connect_SideColor` | Connect Message Color | Hex code (e.g. `0cff00`) | `Discord_Connect_Style=2/3/4/5` |
| `Discord_Connect_Format` | Connect Message Format | Template with placeholders | `Discord_Connect_WebHook` ≠ `""` |
| `Discord_Disconnect_WebHook` | Disconnect Webhook URL | Example URL<br>`""`-Disable | - |
| `Discord_Disconnect_Style` | Disconnect Message Style | `0`-Disable<br>`1`-Text only<br>`2`-Text+Link<br>`3`-+Profile Pic<br>`4`-+Separate DT<br>`5`-+Server IP | `Discord_Disconnect_WebHook` ≠ `""` |
| `Discord_Disconnect_SideColor` | Disconnect Message Color | Hex code (e.g. `ff0000`) | `Discord_Disconnect_Style=2/3/4/5` |
| `Discord_Disconnect_Format` | Disconnect Message Format | Template with placeholders | `Discord_Disconnect_WebHook` ≠ `""` |
| `Discord_DateFormat` | Date Format | Examples: `MM-dd-yyyy` | Webhook active |
| `Discord_TimeFormat` | Time Format | Examples: `HH:mm:ss` | Webhook active |
| `Discord_FooterImage` | Footer Image URL | Example image URL | Style=3/4/5 |
| `Discord_UsersWithNoAvatarImage` | Default Avatar URL | Example image URL | Style=5 |

</details>

<details>
<summary><b>MySQL Config</b> (Click to expand 🔽)</summary>

| Property | Description | Values | Required |  
|----------|-------------|--------|----------|
| `MySql_Enable` | Enable MySQL | `true`-Yes<br>`false`-No | - |
| `MySql_Host` | Database Host | Example: `123.45.67.89` | `MySql_Enable=true` |
| `MySql_Database` | Database Name | Example: `test` | `MySql_Enable=true` |
| `MySql_Username` | Database User | Example: `root` | `MySql_Enable=true` |
| `MySql_Password` | Database Password | Example: `Password123` | `MySql_Enable=true` |
| `MySql_Port` | Database Port | Default: `3306` | `MySql_Enable=true` |
| `MySql_AutoRemovePlayerOlderThanXDays` | Auto-Delete Old Data | Days to keep<br>`0`-Disable | `MySql_Enable=true` |

</details>

<details>
<summary><b>Utilities Config</b> (Click to expand 🔽)</summary>

| Property | Description | Values | Required |  
|----------|-------------|--------|----------|
| `AutoUpdateSignatures` | Auto-Update Signatures | `true`-Yes<br>`false`-No | - |
| `AutoUpdateGeoLocation` | Auto-Update GeoLocation | `true`-Yes<br>`false`-No | - |
| `EnableDebug` | Debug Mode | `true`-Enable<br>`false`-Disable | - |

</details>


## 🛠️ `connect_disconnect_config.json`

<details open>
<summary><b>Connect Disconnect Config</b> (Click to expand 🔽)</summary>

| Item | Description | Example |
|------|-------------|---------|
| **Colors** | | |
| `{Yellow}` | Yellow text color | `{Yellow}Warning` |
| `{Gold}` | Gold text color | `{Gold}[VIP]` |
| `{Silver}` | Silver text color | `{Silver}Member` |
| `{Blue}` | Blue text color | `{Blue}Info` |
| `{DarkBlue}` | Dark blue text color | `{DarkBlue}Moderator` |
| `{BlueGrey}` | Blue-grey text color | `{BlueGrey}System` |
| `{Magenta}` | Magenta text color | `{Magenta}Event` |
| `{LightRed}` | Light red text color | `{LightRed}Alert` |
| `{LightBlue}` | Light blue text color | `{LightBlue}Note` |
| `{Olive}` | Olive green text color | `{Olive}Team` |
| `{Lime}` | Lime green text color | `{Lime}Success` |
| `{Red}` | Red text color | `{Red}Error` |
| `{Purple}` | Purple text color | `{Purple}Admin` |
| `{Grey}` | Grey text color | `{Grey}Timestamp` |
| `{Default}` | Default text color | `{Default}Message` |
| `{White}` | White text color | `{White}Notification` |
| `{Darkred}` | Dark red text color | `{Darkred}Banned` |
| `{Green}` | Green text color | `{Green}Connected` |
| `{LightYellow}` | Light yellow text color | `{LightYellow}Hint` |
| **Other** | | |
| `"ANY"` | Applies to all non-specified players | `"ANY": { ... }` |
| `@css/admins` | Targets players with admin flags | `@css/vip` |
| `#css/admins` | Targets player groups | `#css/regulars` |
| `!STEAM_0:...` | Targets specific players by SteamID | `!76561198206086993` |
| `CONNECT_MESSAGE_CHAT` | Connect message template | `"{Green}{PLAYERNAME} joined"` |
| `CONNECT_SOUND_VOLUME` | Connect sound volume (1-100%) | `"75%"` |
| `CONNECT_SOUND` | Connect sound file paths | `["ui/item_acquired.vsnd"]` |
| `DISCONNECT_MESSAGE_CHAT` | Disconnect message template | `"{Red}{PLAYERNAME} left"` |
| `DISCONNECT_SOUND_VOLUME` | Disconnect sound volume (1-100%) | `"60%"` |
| `DISCONNECT_SOUND` | Disconnect sound file paths | `["ui/item_drop.vsnd"]` |
| **Placeholders** | | |
| `{NEXTLINE}` | Creates line break in messages | `"Line1{NEXTLINE}Line2"` |
| `{DATE}` | Current date (from main config format) | `12-31-2023` |
| `{TIME}` | Current time (from main config format) | `23:59:59` |
| `{PLAYERNAME}` | Player's display name | `ProPlayer123` |
| `{STEAMID}` | SteamID  | `STEAM_0:1:122910632` |
| `{STEAMID3}` | SteamID3 | `[U:1:245821265]` |
| `{STEAMID32}` | SteamID32 | `245821265` |
| `{STEAMID64}` | SteamID64 | `76561198206086993` |
| `{IP}` | Player's IP address | `123.45.67.89` |
| `{CONTINENT}` | Player's continent | `Asia` |
| `{LONGCOUNTRY}` | Full country name | `United Arab Emirates` |
| `{SHORTCOUNTRY}` | Country code | `AE` |
| `{CITY}` | City name | `Abu Dhabi` |
| `{REASON}` | Disconnect reason from `disconnect_reasons.json` | - |

</details>

---


## 📜 Changelog

<details>
<summary><b>📋 View Version History</b> (Click to expand 🔽)</summary>

### [1.1.0]
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
