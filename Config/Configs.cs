using System.Reflection;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Encodings.Web;

namespace CnD_Sound.Config
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]public class ForceStringAttribute : Attribute{public string FallbackValue { get; }public ForceStringAttribute(string fallbackValue){FallbackValue = fallbackValue;}}
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)] public class StringAttribute : Attribute{public string[] Keys { get; }public StringAttribute(params string[] keys) => Keys = keys;}
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)] public class CommentAttribute : Attribute{public string Text;public CommentAttribute(string t) => Text = t;}
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]public class BreakLineAttribute : Attribute{public string Text;public BreakLineAttribute(string t) => Text = t;}
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]public class InfoAttribute : Attribute{public string Key { get; }public InfoAttribute(string key) => Key = key;}
    [AttributeUsage(AttributeTargets.Property)] public class RangeAttribute : Attribute { public double Min, Max, Default; public string? Message; public RangeAttribute(double min, double max, double def, string? msg = null) { Min = min; Max = max; Default = def; Message = msg; } }

    public class Reload_CnD
    {
        [Comment("Commands To Reload Plugin")]
        [Comment("Note: Console_Commands Can Be Execute Via Both Console And Chat By (!)")]
        [Comment("Making Both Console_Commands And Chat_Commands Empty = Disable")]
        [String("Console_Commands", "Chat_Commands")]
        public string Reload_CnD_CommandsInGame { get; set; } = "Console_Commands: css_reloadconnect,css_reloadcnd | Chat_Commands: ";

        [Comment("If [Reload_CnD_CommandsInGame] Pass, Is There Any Specified Restricted Flags, Groups, SteamIDs")]
        [Comment("Example:")]
        [Comment("\"SteamIDs: 76561198206086993,STEAM_0:1:507335558 | Flags: @css/root,@css/admin | Groups: #css/root,#css/admin\"")]
        [Comment("\"SteamIDs:  | Flags:  | Groups: \" = To Allow Everyone")]
        [String("SteamIDs", "Flags", "Groups")]
        public string Reload_CnD_Flags { get; set; } = "SteamIDs: 76561198206086993,STEAM_0:1:507335558 | Flags: @css/root,@css/admin | Groups: #css/root,#css/admin";

        [Comment("If [Reload_CnD_Flags] Pass, Hide Chat After Execute Reload_CnD_CommandsInGame?:")]
        [Comment("0 = No")]
        [Comment("1 = Yes, But Only After Toggle Successfully")]
        [Comment("2 = Yes, Hide All The Time")]
        [Range(0, 2, 0,
        "Reload_CnD_Hide: is invalid, setting to default value (0) Please Choose From 0 To 2.\n" +
        "0 = No\n" +
        "1 = Yes, But Only After Toggle Successfully\n" +
        "2 = Yes, Hide All The Time")]
        public int Reload_CnD_Hide { get; set; } = 0;
    }

    public class CnD_Sounds
    {
        [Comment("Enable Connect Disconnect Sounds In connect_disconnect_config.json?")]
        [Comment("0 = No")]
        [Comment("1 = Yes")]
        [Comment("2 = Yes, But Make It Togglable And Enabled By Default To New Players")]
        [Comment("3 = Yes, But Make It Togglable And Disabled By Default To New Players")]
        [Range(0, 3, 1,
        "CnDSounds: is invalid, setting to default value (1) Please Choose From 0 To 3.\n" +
        "0 = No\n" +
        "1 = Yes\n" +
        "2 = Yes, But Make It Togglable And Enabled By Default To New Players\n" +
        "3 = Yes, But Make It Togglable And Disabled By Default To New Players")]
        public int CnDSounds { get; set; } = 1;

        [Comment("If [CnDSounds = 2 or 3], Commands To Toggle")]
        [Comment("Note: Console_Commands Can Be Execute Via Both Console And Chat By (!)")]
        [Comment("Making Both Console_Commands And Chat_Commands Empty = Disable")]
        [String("Console_Commands", "Chat_Commands")]
        public string CnDSounds_CommandsInGame { get; set; } = "Console_Commands: css_sound,css_sounds | Chat_Commands: ";

        [Comment("If [CnDSounds_CommandsInGame] Pass, Is There Any Specified Restricted Flags, Groups, SteamIDs")]
        [Comment("Example:")]
        [Comment("\"SteamIDs: 76561198206086993,STEAM_0:1:507335558 | Flags: @css/root,@css/admin | Groups: #css/root,#css/admin\"")]
        [Comment("\"SteamIDs:  | Flags:  | Groups: \" = To Allow Everyone")]
        [String("SteamIDs", "Flags", "Groups")]
        public string CnDSounds_Flags { get; set; } = "SteamIDs: | Flags: | Groups:";

        [Comment("If [CnDSounds_Flags] Pass, Hide Chat After Execute CnDSounds_CommandsInGame?:")]
        [Comment("0 = No")]
        [Comment("1 = Yes, But Only After Toggle Successfully")]
        [Comment("2 = Yes, Hide All The Time")]
        [Range(0, 2, 0,
        "CnDSounds_Hide: is invalid, setting to default value (0) Please Choose From 0 To 2.\n" +
        "0 = No\n" +
        "1 = Yes, But Only After Toggle Successfully\n" +
        "2 = Yes, Hide All The Time")]
        public int CnDSounds_Hide { get; set; } = 0;
    }
    
    public class CnD_Messages
    {
        [Comment("Enable Connect Disconnect Messages In connect_disconnect_config.json?")]
        [Comment("0 = No")]
        [Comment("1 = Yes")]
        [Comment("2 = Yes, But Make It Togglable And Enabled By Default To New Players")]
        [Comment("3 = Yes, But Make It Togglable And Disabled By Default To New Players")]
        [Range(0, 3, 1,
        "CnDMessages: is invalid, setting to default value (1) Please Choose From 0 To 3.\n" +
        "0 = No\n" +
        "1 = Yes\n" +
        "2 = Yes, But Make It Togglable And Enabled By Default To New Players\n" +
        "3 = Yes, But Make It Togglable And Disabled By Default To New Players")]
        public int CnDMessages { get; set; } = 1;

        [Comment("If [CnDMessages = 2 or 3], Commands To Toggle")]
        [Comment("Note: Console_Commands Can Be Execute Via Both Console And Chat By (!)")]
        [Comment("Making Both Console_Commands And Chat_Commands Empty = Disable")]
        [String("Console_Commands", "Chat_Commands")]
        public string CnDMessages_CommandsInGame { get; set; } = "Console_Commands: css_message,css_messages | Chat_Commands: ";

        [Comment("If [CnDMessages_CommandsInGame] Pass, Is There Any Specified Restricted Flags, Groups, SteamIDs")]
        [Comment("Example:")]
        [Comment("\"SteamIDs: 76561198206086993,STEAM_0:1:507335558 | Flags: @css/root,@css/admin | Groups: #css/root,#css/admin\"")]
        [Comment("\"SteamIDs:  | Flags:  | Groups: \" = To Allow Everyone")]
        [String("SteamIDs", "Flags", "Groups")]
        public string CnDMessages_Flags { get; set; } = "SteamIDs: | Flags: | Groups:";

        [Comment("If [CnDMessages_Flags] Pass, Hide Chat After Execute CnDMessages_CommandsInGame?:")]
        [Comment("0 = No")]
        [Comment("1 = Yes, But Only After Toggle Successfully")]
        [Comment("2 = Yes, Hide All The Time")]
        [Range(0, 2, 0,
        "CnDMessages_Hide: is invalid, setting to default value (0) Please Choose From 0 To 2.\n" +
        "0 = No\n" +
        "1 = Yes, But Only After Toggle Successfully\n" +
        "2 = Yes, Hide All The Time")]
        public int CnDMessages_Hide { get; set; } = 0;
    }

    

    public class MySqlServer
    {
        [Comment("MySQL Server address (hostname or IP)")]
        public string Server { get; set; } = "localhost";

        [Comment("MySQL Server port")]
        public int Port { get; set; } = 3306;

        [Comment("MySQL Database name")]
        public string Database { get; set; } = "MySql_Database";

        [Comment("MySQL Username")]
        public string Username { get; set; } = "MySql_Username";

        [Comment("MySQL Password")]
        public string Password { get; set; } = "MySql_Password";
    }
    public class MySqlConfig
    {
        [Comment("MySQL Servers You Can Add As Many As You like")]
        public List<MySqlServer> MySql_Servers { get; set; } = new List<MySqlServer>
        {
            new MySqlServer
            {
                Server = "localhost",
                Port = 3306,
                Database = "Database",
                Username = "Username",
                Password = "Password"
            },
            new MySqlServer
            {
                Server = "localhost2",
                Port = 3306,
                Database = "Database2",
                Username = "Username2",
                Password = "Password2"
            }
        };
    }

    public class Config
    {
        [BreakLine("----------------------------[ ↓ Plugin Info ↓ ]----------------------------{nextline}")]
        [Info("Version")]
        [Info("Github")]
        public object __InfoSection { get; set; } = null!;

        [BreakLine("----------------------------[ ↓ Main Config ↓ ]----------------------------{nextline}")]

        [Comment("Auto Set Player Language Depend Player Country?")]
        [Comment("true = Yes (Use Lang Depend Player Country Json If Found In Lang Folder, If Not Found Use Default Server core.json ServerLanguage json)")]
        [Comment("false = No (Use Default Server core.json ServerLanguage json)")]
        public bool AutoSetPlayerLanguage { get; set; } = false;

        [Comment("Reload Connect Disconnect Sound Plugin")]
        public Reload_CnD Reload_CnD { get; set; } = new();

        [Comment("Enable Early Connection Of The Players?")]
        [Comment("true = Yes")]
        [Comment("false = No (Wait When Player Fully Connected)")]
        public bool EarlyConnection { get; set; } = true;

        [Comment("Ignore These Disconnect Reasons Check In disconnect_reasons.json")]
        [Comment("Empty = Disable")]
        public List<int> IgnoreTheseDisconnectReasons { get; set; } = new List<int>
        {
            1,
            54,
            55
        };

        [Comment("Pick Sounds By Order?")]
        [Comment("true = Yes (From Top To Bottom)")]
        [Comment("false = No (Randomly)")]
        public bool PickSoundsByOrder { get; set; } = true;

        [Comment("Remove Default Disconnect Message?:")]
        [Comment("0 = No")]
        [Comment("1 = Yes Completely")]
        [Comment("2 = Yes Completely Also Remove Disconnect Icon In Killfeed")]
        [Range(0, 2, 1,
        "RemoveDefaultDisconnect: is invalid, setting to default value (1) Please Choose From 0 To 2.\n" +
        "0 = No\n" +
        "1 = Yes Completely\n" +
        "2 = Yes Completely Also Remove Disconnect Icon In Killfeed")]
        public int RemoveDefaultDisconnect { get; set; } = 1;
        
        [Comment("How Do You Like Date Format Message")]
        [Comment("Examples:")]
        [Comment("dd MM yyyy = 25 12 2023")]
        [Comment("MM/dd/yy = 12/25/23")]
        [Comment("MM-dd-yyyy = 12-25-2025")]
        public string DateFormat { get; set; } = "MM-dd-yyyy";

        [Comment("How Do You Like Time Format Message")]
        [Comment("Examples:")]
        [Comment("HH:mm = 14:30")]
        [Comment("hh:mm a = 02:30 PM")]
        [Comment("HH:mm:ss = 14:30:45")]
        public string TimeFormat { get; set; } = "HH:mm:ss";

        [Comment("Connect Disconnect Sounds")]
        public CnD_Sounds CnD_Sounds { get; set; } = new();

        [Comment("Connect Disconnect Messages")]
        public CnD_Messages CnD_Messages { get; set; } = new();

        [BreakLine("----------------------------[ ↓ Locally Config ↓ ]----------------------------{nextline}")]

        [Comment("Log Connect/Disconnect Locally (In ../Connect-Disconnect-Sound-GoldKingZ/logs/)?")]
        [Comment("true = Yes")]
        [Comment("false = No")]
        public bool Log_Locally_Enable { get; set; } = true;

        [Comment("Required [Log_Locally_Enable = true]")]
        [Comment("How Do You Like Connect Message Format")]
        [Comment("{DATE} = Log_Locally_DateFormat")]
        [Comment("{TIME} = Log_Locally_TimeFormat")]
        [Comment("{PLAYERNAME} = Player Name")]
        [Comment("{STEAMID} = STEAM_0:1:122910632")]
        [Comment("{STEAMID3} = U:1:245821265")]
        [Comment("{STEAMID32} = 245821265")]
        [Comment("{STEAMID64} = 76561198206086993")]
        [Comment("{IP} = 123.45.67.89")]
        [Comment("{CONTINENT} = Asia")]
        [Comment("{LONGCOUNTRY} = United Arab Emirates")]
        [Comment("{SHORTCOUNTRY} = AE")]
        [Comment("{CITY} = Abu Dhabi")]
        [Comment("Empty = Disable")]
        public string Log_Locally_Connect_Format { get; set; } = "[{DATE} - {TIME}] [{STEAMID64} - {PLAYERNAME}] Connected From [{CONTINENT} - {LONGCOUNTRY} - {CITY}] [{IP}]";

        [Comment("Required [Log_Locally_Enable = true]")]
        [Comment("How Do You Like Disconnect Message Format")]
        [Comment("{DATE} = Log_Locally_DateFormat")]
        [Comment("{TIME} = Log_Locally_TimeFormat")]
        [Comment("{PLAYERNAME} = Player Name")]
        [Comment("{STEAMID} = STEAM_0:1:122910632")]
        [Comment("{STEAMID3} = U:1:245821265")]
        [Comment("{STEAMID32} = 245821265")]
        [Comment("{STEAMID64} = 76561198206086993")]
        [Comment("{IP} = 123.45.67.89")]
        [Comment("{CONTINENT} = Asia")]
        [Comment("{LONGCOUNTRY} = United Arab Emirates")]
        [Comment("{SHORTCOUNTRY} = AE")]
        [Comment("{CITY} = Abu Dhabi")]
        [Comment("{DISCONNECT_REASON} = Disconnect Reason")]
        [Comment("Empty = Disable")]
        public string Log_Locally_Disconnect_Format { get; set; } = "[{DATE} - {TIME}] [{STEAMID64} - {PLAYERNAME}] Disconnected From [{CONTINENT} - {LONGCOUNTRY} - {CITY}] [{IP} - {DISCONNECT_REASON}]";

        [Comment("Required [Log_Locally_Enable = true]")]
        [Comment("How Do You Like Date Format")]
        [Comment("Examples:")]
        [Comment("dd MM yyyy = 25 12 2023")]
        [Comment("MM/dd/yy = 12/25/23")]
        [Comment("MM-dd-yyyy = 12-25-2025")]
        public string Log_Locally_DateFormat { get; set; } = "MM-dd-yyyy";

        [Comment("Required [Log_Locally_Enable = true]")]
        [Comment("How Do You Like Time Format")]
        [Comment("Examples:")]
        [Comment("HH:mm = 14:30")]
        [Comment("hh:mm a = 02:30 PM")]
        [Comment("HH:mm:ss = 14:30:45")]
        public string Log_Locally_TimeFormat { get; set; } = "HH:mm:ss";

        [Comment("Required [Log_Locally_Enable = true]")]
        [Comment("Auto Delete File Logs That Pass Than X Old Days")]
        [Comment("0 = Disable This Feature")]
        public int Log_Locally_AutoDeleteLogsMoreThanXdaysOld { get; set; } = 7;

        [Comment("Save Players Data By Cookies Locally (In ../Connect-Disconnect-Sound-GoldKingZ/cookies/)?")]
        [Comment("0 = No")]
        [Comment("1 = Yes, But Save Data On Players Disconnect (Warning Performance)")]
        [Comment("2 = Yes, But Save Data On Map Change (Recommended)")]
        [Range(0, 2, 2,
        "Cookies_Enable: is invalid, setting to default value (2) Please Choose From 0 To 2.\n" +
        "0 = No\n" +
        "1 = Yes, But Save Data On Players Disconnect (Warning Performance)\n" +
        "2 = Yes, But Save Data On Map Change (Recommended)")]
        public int Cookies_Enable { get; set; } = 2;

        [Comment("If [Cookies_Enable = 1 or 2], Auto Delete Inactive Players More Than X (Days) Old")]
        [Comment("0 = Dont Auto Delete")]
        public int Cookies_AutoRemovePlayerOlderThanXDays { get; set; } = 7;

        [BreakLine("----------------------------[ ↓ Discord Config ↓ ]----------------------------{nextline}")]

        [Comment("Discord Players Connect WebHook")]
        [Comment("Example: https://discord.com/api/webhooks/XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX")]
        [Comment("\"\" = Disable")]
        public string Discord_Connect_WebHook { get; set; } = "";

        [Comment("Required [Discord_Connect_WebHook]")]
        [Comment("How Do You Like Message Look Like For Players Connect")]
        [Comment("1 = Text Only (Result Image: https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode1.png?raw=true)")]
        [Comment("2 = Text With + Name + Hyperlink To Steam Profile (Result Image: https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode2.png?raw=true)")]
        [Comment("3 = Text With + Name + Hyperlink To Steam Profile + Profile Picture (Result Image: https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode3.png?raw=true)")]
        [Comment("4 = Text With + Name + Hyperlink To Steam Profile + Profile Picture + Separate Date And Time From Message (Result Image: https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode4.png?raw=true)")]
        [Comment("5 = Text With + Name + Hyperlink To Steam Profile + Profile Picture + Separate Date And Time From Message + Server Ip In Footer (Result Image: https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode5.png?raw=true)")]
        [Comment("0 = Disable")]
        [Range(1, 5, 3,
        "Discord_Connect_Style: is invalid, setting to default value (3) Please Choose From 1 To 5.\n" +
        "1 = Text Only (Result Image: https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode1.png?raw=true)\n" +
        "2 = Text With + Name + Hyperlink To Steam Profile (Result Image: https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode2.png?raw=true)\n" +
        "3 = Text With + Name + Hyperlink To Steam Profile + Profile Picture (Result Image: https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode3.png?raw=true)\n" +
        "4 = Text With + Name + Hyperlink To Steam Profile + Profile Picture + Separate Date And Time From Message (Result Image: https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode4.png?raw=true)\n" +
        "5 = Text With + Name + Hyperlink To Steam Profile + Profile Picture + Separate Date And Time From Message + Server Ip In Footer (Result Image: https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode5.png?raw=true)"
        )]
        public int Discord_Connect_Style { get; set; } = 3;

        [Comment("Required [Discord_Connect_Style 2/3/4/5]")]
        [Comment("How Would You Side Color Message To Be Use This Site (https://htmlcolorcodes.com/color-picker) For Color Pick")]
        public string Discord_Connect_SideColor { get; set; } = "0cff00";

        [Comment("Required [Discord_Connect_WebHook]")]
        [Comment("How Do You Like Connect Message Format")]
        [Comment("{DATE} = Discord_DateFormat")]
        [Comment("{TIME} = Discord_TimeFormat")]
        [Comment("{PLAYERNAME} = Player Name")]
        [Comment("{STEAMID} = STEAM_0:1:122910632")]
        [Comment("{STEAMID3} = U:1:245821265")]
        [Comment("{STEAMID32} = 245821265")]
        [Comment("{STEAMID64} = 76561198206086993")]
        [Comment("{IP} = 123.45.67.89")]
        [Comment("{CONTINENT} = Asia")]
        [Comment("{LONGCOUNTRY} = United Arab Emirates")]
        [Comment("{SHORTCOUNTRY} = AE")]
        [Comment("{CITY} = Abu Dhabi")]
        [Comment("Empty = Disable")]
        public string Discord_Connect_Format { get; set; } = "{PLAYERNAME} Connected From [{SHORTCOUNTRY} - {CITY}]";

        [Comment("Discord Players Disconnect WebHook")]
        [Comment("Example: https://discord.com/api/webhooks/XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX")]
        [Comment("Empty = Disable")]
        public string Discord_Disconnect_WebHook { get; set; } = "";

        [Comment("Required [Discord_Disconnect_WebHook]")]
        [Comment("How Do You Like Message Look Like For Players Connect")]
        [Comment("1 = Text Only (Result Image: https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode1.png?raw=true)")]
        [Comment("2 = Text With + Name + Hyperlink To Steam Profile (Result Image: https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode2.png?raw=true)")]
        [Comment("3 = Text With + Name + Hyperlink To Steam Profile + Profile Picture (Result Image: https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode3.png?raw=true)")]
        [Comment("4 = Text With + Name + Hyperlink To Steam Profile + Profile Picture + Separate Date And Time From Message (Result Image: https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode4.png?raw=true)")]
        [Comment("5 = Text With + Name + Hyperlink To Steam Profile + Profile Picture + Separate Date And Time From Message + Server Ip In Footer (Result Image: https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode5.png?raw=true)")]
        [Comment("0 = Disable")]
        [Range(1, 5, 3,
        "Discord_Disconnect_Style: is invalid, setting to default value (3) Please Choose From 1 To 5.\n" +
        "1 = Text Only (Result Image: https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode1.png?raw=true)\n" +
        "2 = Text With + Name + Hyperlink To Steam Profile (Result Image: https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode2.png?raw=true)\n" +
        "3 = Text With + Name + Hyperlink To Steam Profile + Profile Picture (Result Image: https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode3.png?raw=true)\n" +
        "4 = Text With + Name + Hyperlink To Steam Profile + Profile Picture + Separate Date And Time From Message (Result Image: https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode4.png?raw=true)\n" +
        "5 = Text With + Name + Hyperlink To Steam Profile + Profile Picture + Separate Date And Time From Message + Server Ip In Footer (Result Image: https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode5.png?raw=true)"
        )]
        public int Discord_Disconnect_Style { get; set; } = 3;

        [Comment("Required [Discord_Disconnect_Style 2/3/4/5]")]
        [Comment("How Would You Side Color Message To Be Use This Site (https://htmlcolorcodes.com/color-picker) For Color Pick")]
        public string Discord_Disconnect_SideColor { get; set; } = "ff0000";

        [Comment("Required [Discord_Disconnect_WebHook]")]
        [Comment("How Do You Like Disconnect Message Format")]
        [Comment("{DATE} = Discord_DateFormat")]
        [Comment("{TIME} = Discord_TimeFormat")]
        [Comment("{PLAYERNAME} = Player Name")]
        [Comment("{STEAMID} = STEAM_0:1:122910632")]
        [Comment("{STEAMID3} = U:1:245821265")]
        [Comment("{STEAMID32} = 245821265")]
        [Comment("{STEAMID64} = 76561198206086993")]
        [Comment("{IP} = 123.45.67.89")]
        [Comment("{CONTINENT} = Asia")]
        [Comment("{LONGCOUNTRY} = United Arab Emirates")]
        [Comment("{SHORTCOUNTRY} = AE")]
        [Comment("{CITY} = Abu Dhabi")]
        [Comment("{DISCONNECT_REASON} = Disconnect Reason")]
        [Comment("Empty = Disable")]
        public string Discord_Disconnect_Format { get; set; } = "{PLAYERNAME} Disconnected From [{SHORTCOUNTRY} - {CITY}]";

        [Comment("Required [Discord_Connect_WebHook Or Discord_Disconnect_WebHook]")]
        [Comment("How Do You Like Date Format")]
        [Comment("Examples:")]
        [Comment("dd MM yyyy = 25 12 2023")]
        [Comment("MM/dd/yy = 12/25/23")]
        [Comment("MM-dd-yyyy = 12-25-2025")]
        public string Discord_DateFormat { get; set; } = "MM-dd-yyyy";

        [Comment("Required [Discord_Connect_WebHook Or Discord_Disconnect_WebHook]")]
        [Comment("How Do You Like Time Format")]
        [Comment("Examples:")]
        [Comment("HH:mm = 14:30")]
        [Comment("hh:mm a = 02:30 PM")]
        [Comment("HH:mm:ss = 14:30:45")]
        public string Discord_TimeFormat { get; set; } = "HH:mm:ss";

        [Comment("Required [Discord_Disconnect_Style Or Discord_Connect_SideColor 3/4/5]")]
        [Comment("Footer Image")]
        public string Discord_FooterImage { get; set; } = "https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/serverip.png?raw=true";

        [Comment("Required [Discord_Disconnect_Style Or Discord_Connect_SideColor 5]")]
        [Comment("If Player Doest Have Avatar What Should We Replace It")]
        public string Discord_UsersWithNoAvatarImage { get; set; } = "https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/avatar.jpg?raw=true";

        [BreakLine("----------------------------[ ↓ MySql Config ↓ ]----------------------------{nextline}")]
        
        [Comment("Save Players Data Into MySql?")]
        [Comment("0 = No")]
        [Comment("1 = Yes, But Save Data On Players Disconnect (Warning Performance)")]
        [Comment("2 = Yes, But Save Data On Map Change (Recommended)")]
        [Range(0, 2, 0,
        "MySql_Enable: is invalid, setting to default value (0) Please Choose From 0 To 2.\n" +
        "0 = No\n" +
        "1 = Yes, But Save Data On Players Disconnect (Warning Performance)\n" +
        "2 = Yes, But Save Data On Map Change (Recommended)")]
        public int MySql_Enable { get; set; } = 0;

        [Comment("Connection Timeout In Seconds")]
        [Range(5, 60, 30, "Connection timeout must be between 5 and 60 seconds")]
        public int MySql_ConnectionTimeout { get; set; } = 30;

        [Comment("Retry Attempts When Connection Fails")]
        [Range(1, 5, 3, "Retry attempts must be between 1 and 5")]
        public int MySql_RetryAttempts { get; set; } = 3;

        [Comment("Delay Between Retries In Seconds")]
        [Range(1, 10, 2, "Retry delay must be between 1 and 10 seconds")]
        public int MySql_RetryDelay { get; set; } = 2;

        [Comment("MySql Config")]
        public MySqlConfig MySql_Config { get; set; } = new MySqlConfig();

        [Comment("Auto Delete Inactive Players More Than X (Days) Old")]
        [Comment("0 = Dont Auto Delete")]
        public int MySql_AutoRemovePlayerOlderThanXDays { get; set; } = 7;

        [BreakLine("----------------------------[ ↓ Utilities  ↓ ]----------------------------{nextline}")]

        [Comment("Auto Update Signatures (In ../Connect-Disconnect-Sound-GoldKingZ/gamedata/)?")]
        [Comment("true = Yes")]
        [Comment("false = No")]
        public bool AutoUpdateSignatures { get; set; } = true;

        [Comment("Auto Update GeoLocation (In ../Connect-Disconnect-Sound-GoldKingZ/GeoLocation/)?")]
        [Comment("true = Yes")]
        [Comment("false = No")]
        public bool AutoUpdateGeoLocation { get; set; } = false;

        [Comment("Auto Update disconnect_reasons.json (In ../Connect-Disconnect-Sound-GoldKingZ/config/disconnect_reasons.json)?")]
        [Comment("true = Yes")]
        [Comment("false = No")]
        public bool AutoUpdateDisconnectReasons { get; set; } = false;
        
        [Comment("Enable Debug Plugin In Server Console (Helps You To Debug Issues You Facing)?")]
        [Comment("true = Yes")]
        [Comment("false = No")]
        public bool EnableDebug { get; set; } = false;
    }

    public static class Configs
    {
        public static string Version = $"Version : {MainPlugin.Instance.ModuleVersion ?? "Unknown"}";
        public static string Github = "https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ";
        public static Config Instance { get; private set; } = new Config();
        static string? filePath;
        static bool IsSimple(Type t) => t.IsPrimitive || t.IsEnum || t == typeof(string) || t == typeof(decimal) || t == typeof(DateTime) || t == typeof(uint);
        static bool IsList(Type t) => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>);
        
        private static readonly JsonSerializerOptions SimpleValueJsonOptions = new JsonSerializerOptions{WriteIndented = false,Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping};

        public static void Load(string moduleDirectory)
        {
            string configDirectory = Path.Combine(moduleDirectory ?? ".", "config");
            if (!Directory.Exists(configDirectory))
            {
                Directory.CreateDirectory(configDirectory);
            }

            filePath = Path.Combine(configDirectory, "config.json");
            Helper.CreateResource(Path.Combine(configDirectory, "ServerPrecacheResources.txt"));

            if (!File.Exists(filePath)) { Save(); return; }

            try
            {
                var json = File.ReadAllText(filePath);

                json = RemoveTrailingCommas(json);
                
                var lines = json.Split('\n').Where(l => !l.TrimStart().StartsWith("//")).ToArray();
                json = string.Join("\n", lines);

                JsonNode? root = null;
                try
                {
                    root = JsonNode.Parse(json);
                }
                catch
                {
                    Instance = new Config();
                    EnsureNestedDefaults(Instance);
                    ValidateStringRecursive(Instance);
                    ValidateRangesRecursive(Instance);
                    ValidateForceStringRecursive(Instance);
                    Save();
                    return;
                }

                if (root is JsonObject rootObj)
                {
                    CleanJsonObjectStrict(rootObj, Instance.GetType());
                }

                string normalizedJson = root?.ToJsonString(new JsonSerializerOptions { WriteIndented = false }) ?? "{}";
                Instance = JsonSerializer.Deserialize<Config>(normalizedJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new Config();
            }
            catch
            {
                Instance = new Config();
            }

            EnsureNestedDefaults(Instance);
            ValidateStringRecursive(Instance);
            ValidateRangesRecursive(Instance);
            ValidateForceStringRecursive(Instance);
            Save();
        }

        public static void Save()
        {
            try
            {
                var path = filePath ?? Path.Combine(".", "config", "config.json");
                string? directory = Path.GetDirectoryName(path);

                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                var props = typeof(Config).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0);

                var rendered = new List<string>();
                foreach (var p in props) 
                {
                    try
                    {
                        rendered.Add(RenderProperty(p, Instance, 2));
                    }
                    catch
                    {
                    }
                }

                string JoinJsonProperties(List<string> propsList)
                {
                    var filtered = propsList.Where(r => !string.IsNullOrWhiteSpace(r)).ToList();
                    var result = new List<string>();

                    bool BlockContainsProperty(string block)
                    {
                        return block
                            .Split('\n')
                            .Any(line =>
                            {
                                var t = line.TrimStart();
                                return t.StartsWith("\"") && t.Contains("\":");
                            });
                    }

                    for (int i = 0; i < filtered.Count; i++)
                    {
                        var current = filtered[i];
                        bool isCurrentPropertyBlock = BlockContainsProperty(current);

                        int nextPropIndex = -1;
                        for (int j = i + 1; j < filtered.Count; j++)
                        {
                            if (BlockContainsProperty(filtered[j]))
                            {
                                nextPropIndex = j;
                                break;
                            }
                        }

                        bool hasNextProp = nextPropIndex != -1;
                        if (isCurrentPropertyBlock && hasNextProp)
                        {
                            if (!current.TrimEnd().EndsWith(","))
                            {
                                current += ",";
                            }
                        }

                        result.Add(current);
                    }

                    return string.Join("\n\n", result);
                }

                var body = JoinJsonProperties(rendered);
                var final = "{\n" + body + "\n}\n";
                File.WriteAllText(path, final);
            }
            catch
            {
            }
        }

        static void EnsureNestedDefaults(object? obj)
        {
            if (obj == null) return;
            
            var props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0);

            foreach (var p in props)
            {
                if (p.GetCustomAttributes<InfoAttribute>().Any()) continue;

                if (IsList(p.PropertyType)) continue;

                if (p.PropertyType == typeof(string) || p.PropertyType.IsValueType) continue;
                
                try
                {
                    var val = p.GetValue(obj);
                    if (val == null)
                    {
                        try 
                        { 
                            var inst = Activator.CreateInstance(p.PropertyType); 
                            if (inst != null) p.SetValue(obj, inst); 
                        }
                        catch { }
                    }
                    EnsureNestedDefaults(p.GetValue(obj));
                }
                catch (TargetParameterCountException)
                {
                    continue;
                }
                catch
                {
                }
            }
        }

        public static void ValidateStringRecursive(object? obj)
        {
            if (obj == null) return;
            
            var props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0);

            foreach (var prop in props)
            {
                if (prop.GetCustomAttributes<InfoAttribute>().Any() && obj is Config) continue;

                if (prop.PropertyType == typeof(string))
                {
                    if (prop.GetCustomAttribute<StringAttribute>() is StringAttribute attr)
                    {
                        try
                        {
                            var current = prop.GetValue(obj) as string;
                            prop.SetValue(obj, string.Join(" | ", attr.Keys.Select(key => 
                                $"{key}: {GetStringValue(current, key)}")));
                        }
                        catch
                        {
                        }
                    }
                }
                else if (!IsSimple(prop.PropertyType) && !IsList(prop.PropertyType))
                {
                    try
                    {
                        ValidateStringRecursive(prop.GetValue(obj));
                    }
                    catch (TargetParameterCountException)
                    {
                        continue;
                    }
                }
            }
        }

        public static string GetStringValue(string? input, string key)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";
            
            return input.Split('|')
                .Select(segment => segment.Trim())
                .FirstOrDefault(segment => segment.StartsWith(key + ":", StringComparison.OrdinalIgnoreCase))?
                .Substring(key.Length + 1)
                .Trim() ?? "";
        }

        static void ValidateRangesRecursive(object? obj)
        {
            if (obj == null) return;
            
            var props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0);

            foreach (var p in props)
            {
                if (p.GetCustomAttributes<InfoAttribute>().Any()) continue;

                try
                {
                    var range = p.GetCustomAttribute<RangeAttribute>();
                    var val = p.GetValue(obj);
                    if (range != null && val != null)
                    {
                        if (double.TryParse(Convert.ToString(val), out double d))
                        {
                            if (d < range.Min || d > range.Max)
                            {
                                if (!string.IsNullOrEmpty(range.Message))
                                {
                                    var messageLines = range.Message.Replace("\\n", "\n").Split('\n');
                                    foreach (var line in messageLines)
                                    {
                                        if (!string.IsNullOrWhiteSpace(line))
                                        {
                                            Helper.DebugMessage(line.Trim(), true);
                                        }
                                    }
                                }
                                p.SetValue(obj, Convert.ChangeType(range.Default, p.PropertyType));
                            }
                        }
                    }
                    if (!IsSimple(p.PropertyType) && !IsList(p.PropertyType)) 
                    {
                        ValidateRangesRecursive(p.GetValue(obj));
                    }
                }
                catch (TargetParameterCountException)
                {
                    continue;
                }
                catch
                {
                }
            }
        }

        static void ValidateForceStringRecursive(object? obj)
        {
            if (obj == null) return;
            
            var props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0);

            foreach (var prop in props)
            {
                if (prop.GetCustomAttributes<InfoAttribute>().Any()) continue;

                try
                {
                    if (prop.PropertyType == typeof(string))
                    {
                        if (prop.GetCustomAttribute<ForceStringAttribute>() is ForceStringAttribute forceAttr)
                        {
                            var current = prop.GetValue(obj) as string;
                            if (string.IsNullOrWhiteSpace(current))
                            {
                                prop.SetValue(obj, forceAttr.FallbackValue);
                            }
                        }
                    }
                    else if (!IsSimple(prop.PropertyType) && !IsList(prop.PropertyType))
                    {
                        ValidateForceStringRecursive(prop.GetValue(obj));
                    }
                }
                catch (TargetParameterCountException)
                {
                    continue;
                }
                catch
                {
                }
            }
        }

        static IEnumerable<string> RenderCommentLines(string? text, string pad)
        {
            if (string.IsNullOrWhiteSpace(text)) yield break;
            var lines = text.Replace("\r", "").Split('\n');
            foreach (var raw in lines)
            {
                var t = raw.TrimEnd();
                if (string.IsNullOrWhiteSpace(t))
                {
                    yield return pad + "//";
                }
                else if (t == "{nextline}")
                {
                    yield return "";
                }
                else
                {
                    yield return pad + "// " + t;
                }
            }
        }
        private static string RemoveTrailingCommas(string json)
        {
            json = System.Text.RegularExpressions.Regex.Replace(json, @",(\s*[]])", "$1");
            json = System.Text.RegularExpressions.Regex.Replace(json, @",(\s*[}])", "$1");
            return json;
        }

        static string RenderProperty(PropertyInfo p, object? instance, int indent)
        {
            var pad = new string(' ', indent);
            var parts = new List<string>();

            try
            {
                var br = p.GetCustomAttribute<BreakLineAttribute>();
                if (br != null)
                {
                    var txt = br.Text ?? "";

                    bool emptyLineBefore = txt.StartsWith("{nextline}");
                    bool emptyLineAfter = txt.EndsWith("{nextline}");

                    if (emptyLineBefore) txt = txt.Substring("{nextline}".Length);
                    if (emptyLineAfter) txt = txt.Substring(0, txt.Length - "{nextline}".Length);

                    txt = txt.Trim();

                    if (emptyLineBefore)
                        parts.Add(string.Empty);

                    foreach (var line in RenderCommentLines(txt, pad))
                        parts.Add(line);

                    if (emptyLineAfter)
                        parts.Add(string.Empty);
                }

                var infos = p.GetCustomAttributes<InfoAttribute>();
                foreach (var info in infos)
                {
                    string text = info.Key switch
                    {
                        "Version" => Version,
                        "Github" => Github,
                        _ => info.Key
                    };
                    foreach (var line in RenderCommentLines(text, pad))
                        parts.Add(line);
                }

                var comments = p.GetCustomAttributes<CommentAttribute>();
                foreach (var comment in comments)
                {
                    foreach (var line in RenderCommentLines(comment.Text, pad))
                        parts.Add(line);
                }

                if (p.GetCustomAttributes<InfoAttribute>().Any() && (p.PropertyType == typeof(object) || p.PropertyType == typeof(void)))
                    return string.Join("\n", parts);

                var val = p.GetValue(instance);
                if (IsSimple(p.PropertyType))
                {
                    var jsonVal = JsonSerializer.Serialize(val, SimpleValueJsonOptions);
                    parts.Add(pad + $"\"{p.Name}\": {jsonVal}");
                }
                else if (IsList(p.PropertyType))
                {
                    parts.Add(pad + $"\"{p.Name}\":");
                    parts.Add(pad + "[");

                    if (val is System.Collections.IList list && list.Count > 0)
                    {
                        var listItems = new List<string>();
                        var elementType = p.PropertyType.GetGenericArguments()[0];
                        
                        if (IsSimple(elementType))
                        {
                            foreach (var item in list)
                            {
                                if (item != null)
                                {
                                    var jsonVal = JsonSerializer.Serialize(item, SimpleValueJsonOptions);
                                    listItems.Add(pad + "  " + jsonVal);
                                }
                            }
                            parts.Add(string.Join(",\n", listItems));
                        }
                        else
                        {
                            foreach (var item in list)
                            {
                                if (item != null)
                                {
                                    var itemProps = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                        .Where(ip => ip.CanRead && ip.GetIndexParameters().Length == 0);
                                    var itemLines = new List<string>();
                                    foreach (var ip in itemProps) 
                                    {
                                        try
                                        {
                                            itemLines.Add(RenderProperty(ip, item, indent + 4));
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    var filtered = itemLines.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                                    var itemJson = string.Join(",\n", filtered);
                                    listItems.Add(pad + "  {\n" + itemJson + "\n" + pad + "  }");
                                }
                            }
                            parts.Add(string.Join(",\n", listItems));
                        }
                    }

                    parts.Add(pad + "]");
                }
                else
                {
                    if (val == null)
                    {
                        parts.Add(pad + $"\"{p.Name}\": null");
                    }
                    else
                    {
                        parts.Add(pad + $"\"{p.Name}\":");
                        parts.Add(pad + "{");

                        var innerProps = p.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(ip => ip.CanRead && ip.GetIndexParameters().Length == 0);
                        
                        var innerLines = new List<string>();
                        foreach (var ip in innerProps) 
                        {
                            try
                            {
                                innerLines.Add(RenderProperty(ip, val, indent + 2));
                            }
                            catch
                            {
                            }
                        }
                        
                        var filtered = innerLines.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                        var innerJoined = string.Join(",\n\n", filtered);
                        if (!string.IsNullOrEmpty(innerJoined)) parts.Add(innerJoined);

                        parts.Add(pad + "}");
                    }
                }
            }
            catch
            {
                return "";
            }

            return string.Join("\n", parts);
        }

        static void CleanJsonObjectStrict(JsonObject jsonObj, Type targetType)
        {
            if (jsonObj == null || targetType == null) return;

            var props = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .Where(p => !p.GetCustomAttributes<InfoAttribute>().Any() && p.GetIndexParameters().Length == 0)
                                .ToList();
            var map = props.ToDictionary(p => p.Name.ToLowerInvariant(), p => p);

            foreach (var key in jsonObj.Select(kv => kv.Key).ToList())
            {
                var keyLower = key.ToLowerInvariant();
                if (!map.TryGetValue(keyLower, out var prop))
                {
                    jsonObj.Remove(key);
                    continue;
                }

                var expectedType = prop.PropertyType;
                var underlying = Nullable.GetUnderlyingType(expectedType) ?? expectedType;
                var node = jsonObj[key];
                if (node == null) continue;

                if (IsSimple(underlying) || underlying.IsEnum)
                {
                    if (node is JsonValue jv)
                    {
                        if (!JsonValueIsExactType(jv, underlying))
                        {
                            jsonObj.Remove(key);
                        }
                    }
                    else
                    {
                        jsonObj.Remove(key);
                    }
                }
                else if (IsList(underlying))
                {
                    if (!(node is JsonArray))
                    {
                        jsonObj.Remove(key);
                    }
                    else
                    {
                        var array = node.AsArray();
                        var itemType = underlying.GetGenericArguments()[0];
                        
                        for (int i = array.Count - 1; i >= 0; i--)
                        {
                            var item = array[i];
                            if (IsSimple(itemType))
                            {
                                if (!(item is JsonValue jvItem) || !JsonValueIsExactType(jvItem, itemType))
                                {
                                    if (itemType == typeof(string) && item is JsonValue jv)
                                    {
                                        try
                                        {
                                            if (jv.TryGetValue<int>(out int intVal))
                                            {
                                                array[i] = JsonValue.Create(intVal.ToString());
                                                continue;
                                            }
                                            if (jv.TryGetValue<double>(out double doubleVal))
                                            {
                                                array[i] = JsonValue.Create(doubleVal.ToString());
                                                continue;
                                            }
                                            if (jv.TryGetValue<long>(out long longVal))
                                            {
                                                array[i] = JsonValue.Create(longVal.ToString());
                                                continue;
                                            }
                                        }
                                        catch
                                        {
                                            array.RemoveAt(i);
                                        }
                                    }
                                    else
                                    {
                                        array.RemoveAt(i);
                                    }
                                }
                            }
                            else if (item is JsonObject itemObj)
                            {
                                CleanJsonObjectStrict(itemObj, itemType);
                            }
                            else if (!(item is JsonObject))
                            {
                                array.RemoveAt(i);
                            }
                        }
                    }
                }
                else
                {
                    if (node is JsonObject childObj)
                    {
                        CleanJsonObjectStrict(childObj, underlying);
                    }
                    else
                    {
                        jsonObj.Remove(key);
                    }
                }
            }
        }

        static bool JsonValueIsExactType(JsonValue jv, Type clrType)
        {
            try
            {
                var t = clrType;
                if (t == typeof(bool))
                {
                    try { jv.GetValue<bool>(); return true; } catch { return false; }
                }

                if (t == typeof(int) || t == typeof(long) || t == typeof(short) || t == typeof(byte) ||
                    t == typeof(uint) || t == typeof(ulong) || t == typeof(ushort) || t == typeof(sbyte))
                {
                    try
                    {
                        jv.GetValue<long>();
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }

                if (t == typeof(double) || t == typeof(float) || t == typeof(decimal))
                {
                    try { jv.GetValue<double>(); return true; } catch { return false; }
                }

                if (t == typeof(string))
                {
                    try { jv.GetValue<string>(); return true; } catch { return false; }
                }

                if (t == typeof(DateTime))
                {
                    try
                    {
                        var s = jv.GetValue<string>();
                        return DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out _);
                    }
                    catch { return false; }
                }

                if (t.IsEnum)
                {
                    try
                    {
                        var s = jv.GetValue<string>();
                        if (!string.IsNullOrEmpty(s))
                        {
                            var names = Enum.GetNames(t);
                            if (names.Any(n => string.Equals(n, s, StringComparison.OrdinalIgnoreCase)))
                                return true;
                        }
                    }
                    catch { }

                    try
                    {
                        var v = jv.GetValue<long>();
                        return true;
                    }
                    catch { }

                    return false;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}