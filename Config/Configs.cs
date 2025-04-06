using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Text;

namespace CnD_Sound.Config
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RangeAttribute : Attribute
    {
        public int Min { get; }
        public int Max { get; }
        public int Default { get; }
        public string Message { get; }

        public RangeAttribute(int min, int max, int defaultValue, string message)
        {
            Min = min;
            Max = max;
            Default = defaultValue;
            Message = message;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CommentAttribute : Attribute
    {
        public string Comment { get; }

        public CommentAttribute(string comment)
        {
            Comment = comment;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class BreakLineAttribute : Attribute
    {
        public string BreakLine { get; }

        public BreakLineAttribute(string breakLine)
        {
            BreakLine = breakLine;
        }
    }
    public static class Configs
    {
        private static readonly string ConfigDirectoryName = "config";
        private static readonly string ConfigFileName = "config.json";
        private static readonly string PrecacheResources = "ServerPrecacheResources.txt";
        private static string? _configFilePath;
        private static string? _PrecacheResources;
        private static ConfigData? _configData;

        private static readonly JsonSerializerOptions SerializationOptions = new()
        {
            Converters =
            {
                new JsonStringEnumConverter()
            },
            WriteIndented = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
        };

        public static bool IsLoaded()
        {
            return _configData is not null;
        }

        public static ConfigData GetConfigData()
        {
            if (_configData is null)
            {
                throw new Exception("Config not yet loaded.");
            }
            
            return _configData;
        }

        public static ConfigData Load(string modulePath)
        {
            var configFileDirectory = Path.Combine(modulePath, ConfigDirectoryName);
            if(!Directory.Exists(configFileDirectory))
            {
                Directory.CreateDirectory(configFileDirectory);
            }
            _PrecacheResources = Path.Combine(configFileDirectory, PrecacheResources);
            Helper.CreateResource(_PrecacheResources);

            _configFilePath = Path.Combine(configFileDirectory, ConfigFileName);
            if (File.Exists(_configFilePath))
            {
                _configData = JsonSerializer.Deserialize<ConfigData>(File.ReadAllText(_configFilePath), SerializationOptions);
                _configData!.Validate();
            }
            else
            {
                _configData = new ConfigData();
                _configData.Validate();
            }

            if (_configData is null)
            {
                throw new Exception("Failed to load configs.");
            }

            SaveConfigData(_configData);
            
            return _configData;
        }

        private static void SaveConfigData(ConfigData configData)
        {
            if (_configFilePath is null)
                throw new Exception("Config not yet loaded.");

            string json = JsonSerializer.Serialize(configData, SerializationOptions);
            json = Regex.Unescape(json);

            var lines = json.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var newLines = new List<string>();

            foreach (var line in lines)
            {
                var match = Regex.Match(line, @"^\s*""(\w+)""\s*:.*");
                bool isPropertyLine = false;
                PropertyInfo? propInfo = null;

                if (match.Success)
                {
                    string propName = match.Groups[1].Value;
                    propInfo = typeof(ConfigData).GetProperty(propName);

                    var breakLineAttr = propInfo?.GetCustomAttribute<BreakLineAttribute>();
                    if (breakLineAttr != null)
                    {
                        string breakLine = breakLineAttr.BreakLine;

                        if (breakLine.Contains("{space}"))
                        {
                            breakLine = breakLine.Replace("{space}", "").Trim();

                            if (breakLineAttr.BreakLine.StartsWith("{space}"))
                            {
                                newLines.Add("");
                            }

                            newLines.Add("// " + breakLine);
                            newLines.Add("");
                        }
                        else
                        {
                            newLines.Add("// " + breakLine);
                        }
                    }

                    var commentAttr = propInfo?.GetCustomAttribute<CommentAttribute>();
                    if (commentAttr != null)
                    {
                        var commentLines = commentAttr.Comment.Split('\n');
                        foreach (var commentLine in commentLines)
                        {
                            newLines.Add("// " + commentLine.Trim());
                        }
                    }

                    isPropertyLine = true;
                }

                newLines.Add(line);

                if (isPropertyLine && propInfo?.GetCustomAttribute<CommentAttribute>() != null)
                {
                    newLines.Add("");
                }
            }

            var adjustedLines = new List<string>();
            foreach (var line in newLines)
            {
                adjustedLines.Add(line);
                if (Regex.IsMatch(line, @"^\s*\],?\s*$"))
                {
                    adjustedLines.Add("");
                }
            }

            File.WriteAllText(_configFilePath, string.Join(Environment.NewLine, adjustedLines), Encoding.UTF8);
        }

        public class ConfigData
        {
            private string? _Version;
            private string? _Link;
            [BreakLine("----------------------------[ ↓ Plugin Info ↓ ]----------------------------{space}")]
            public string Version
            {
                get => _Version!;
                set
                {
                    _Version = value;
                    if (_Version != MainPlugin.Instance.ModuleVersion)
                    {
                        Version = MainPlugin.Instance.ModuleVersion;
                    }
                }
            }

            public string Link
            {
                get => _Link!;
                set
                {
                    _Link = value;
                    if (_Link != "https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ")
                    {
                        Link = "https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ";
                    }
                }
            }

            [BreakLine("{space}----------------------------[ ↓ Main Config ↓ ]----------------------------{space}")]
            [Comment("Enable Early Connection Of The Players?\ntrue = Yes\nfalse = No (Wait When Player Fully Connected)")]
            public bool EarlyConnection { get; set; }

            [Comment("Disable Looping Connections To Anti Spam Chat?\ntrue = Yes\nfalse = No")]
            public bool DisableLoopConnections { get; set; }

            [Comment("Make sv_hibernate_when_empty false?\nWhy? It is Important To Disable sv_hibernate_when_empty Otherwise Will Bug Log Last Player Disconnect From The Server\ntrue = Yes (Recommended)\nfalse = No")]
            public bool DisableServerHibernate { get; set; }

            [Comment("Pick Random Sounds?\ntrue = Yes \nfalse = No (Pick Sounds From Top To Bottom)")]
            public bool PickRandomSounds { get; set; }

            [Comment("Remove Default Disconnect Message?:\n0 = No\n1 = Yes Completely\n2 = Yes Completely Also Remove Disconnect Icon In Killfeed")]
            [Range(0, 2, 0, "[CnD] RemoveDefaultDisconnect: is invalid, setting to default value (2) Please Choose From 0 To 2.\n[CnD] 0 = No\n[CnD] 1 = Yes Completely\n[CnD] 2 = Yes Completely Also Remove Disconnect Icon In Killfeed")]
            public int RemoveDefaultDisconnect { get; set; }

            [Comment("Commands Toggle On/Off To Sounds\n\"\" = Disable")]
            public string Toggle_Sounds_CommandsInGame { get; set; }

            [Comment("Required [Toggle_Sounds_Flags]\nToggle On/Off To Sounds\nExample:\n\"!76561198206086993,@css/include,#css/include,include\"\n\"\" = To Allow Everyone")]
            public string Toggle_Sounds_Flags { get; set; }


            [Comment("Commands Toggle On/Off To Messages Connect/Disconnect\n\"\" = Disable")]
            public string Toggle_Messages_CommandsInGame { get; set; }

            [Comment("Required [Toggle_Messages_CommandsInGame]\nToggle On/Off To Messages Connect/Disconnect\nExample:\n\"!76561198206086993,@css/include,#css/include,include\"\n\"\" = To Allow Everyone")]
            public string Toggle_Messages_Flags { get; set; }

            [Comment("Default Value Of Sounds To New Players?\ntrue = On\nfalse = Off")]
            public bool Default_Sounds { get; set; }

            [Comment("Default Value Of Messages To New Players?\ntrue = On\nfalse = Off")]
            public bool Default_Messages { get; set; }
            [Comment("How Do You Like Date Format Message\nExamples:\ndd MM yyyy = 25 12 2023\nMM/dd/yy = 12/25/23\nMM-dd-yyyy = 12-25-2025")]
            public string DateFormat { get; set; }

            [Comment("How Do You Like Time Format Message\nExamples:\nHH:mm = 14:30\nhh:mm a = 02:30 PM\nHH:mm:ss = 14:30:45")]
            public string TimeFormat { get; set; }

            [BreakLine("{space}----------------------------[ ↓ Locally Config ↓ ]----------------------------{space}")]         
            [Comment("Log Connect/Disconnect Locally (In ../Connect-Disconnect-Sound-GoldKingZ/logs/)?\ntrue = Yes\nfalse = No")]
            public bool Log_Locally_Enable { get; set; }

            [Comment("Required [Log_Locally_Enable = true]\nHow Do You Like Date Format\nExamples:\ndd MM yyyy = 25 12 2023\nMM/dd/yy = 12/25/23\nMM-dd-yyyy = 12-25-2025")]
            public string Log_Locally_DateFormat { get; set; }

            [Comment("Required [Log_Locally_Enable = true]\nHow Do You Like Time Format\nExamples:\nHH:mm = 14:30\nhh:mm a = 02:30 PM\nHH:mm:ss = 14:30:45")]
            public string Log_Locally_TimeFormat { get; set; }

            [Comment("Required [Log_Locally_Enable = true]\nHow Do You Like Connect Message Format\n{DATE} = Log_Locally_DateFormat\n{TIME} = Log_Locally_TimeFormat\n{PLAYERNAME} = Player Name\n{STEAMID} = STEAM_0:1:122910632\n{STEAMID3} = U:1:245821265\n{STEAMID32} = 245821265\n{STEAMID64} = 76561198206086993\n{IP} = 123.45.67.89\n{CONTINENT} = Asia\n{LONGCOUNTRY} = United Arab Emirates\n{SHORTCOUNTRY} = AE\n{CITY} = Abu Dhabi\"\n\"\" = Disable")]
            public string Log_Locally_Connect_Format { get; set; }

            [Comment("Required [Log_Locally_Enable = true]\nHow Do You Like Disconnect Message Format\n{DATE} = Log_Locally_DateFormat\n{TIME} = Log_Locally_TimeFormat\n{PLAYERNAME} = Player Name\n{STEAMID} = STEAM_0:1:122910632\n{STEAMID3} = U:1:245821265\n{STEAMID32} = 245821265\n{STEAMID64} = 76561198206086993\n{IP} = 123.45.67.89\n{CONTINENT} = Asia\n{LONGCOUNTRY} = United Arab Emirates\n{SHORTCOUNTRY} = AE\n{CITY} = Abu Dhabi\n{DISCONNECT_REASON} = Disconnect Reason\"\n\"\" = Disable")]
            public string Log_Locally_Disconnect_Format { get; set; }

            [Comment("Required [Log_Locally_Enable = true]\nAuto Delete File Logs That Pass Than X Old Days\n0 = Disable This Feature")]
            public int Log_Locally_AutoDeleteLogsMoreThanXdaysOld { get; set; }

            [Comment("Save Players Data By Cookies Locally (In ../Connect-Disconnect-Sound-GoldKingZ/cookies/)?\ntrue = Yes\nfalse = No")]
            public bool Cookies_Enable { get; set; }
            
            [Comment("Required [Cookies_Enable = true]\nAuto Delete Inactive Players More Than X (Days) Old\n0 = Dont Auto Delete")]
            public int Cookies_AutoRemovePlayerOlderThanXDays { get; set; }

            [BreakLine("{space}----------------------------[ ↓ Discord Config ↓ ]----------------------------{space}")]
            [Comment("Discord Players Connect WebHook\nExample: https://discord.com/api/webhooks/XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX\n\"\" = Disable")]
            public string Discord_Connect_WebHook { get; set; }

            [Comment("Required [Discord_Connect_WebHook]\nHow Do You Like Message Look Like For Players Connect\n1 = Text Only (Result Image : https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode1.png?raw=true)\n2 = Text With + Name + Hyperlink To Steam Profile (Result Image : https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode2.png?raw=true)\n3 = Text With + Name + Hyperlink To Steam Profile + Profile Picture (Result Image : https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode3.png?raw=true)\n4 = Text With + Name + Hyperlink To Steam Profile + Profile Picture + Saparate Date And Time From Message (Result Image : https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode4.png?raw=true)\n5 = Text With + Name + Hyperlink To Steam Profile + Profile Picture + Saparate Date And Time From Message + Server Ip In Footer (Result Image : https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode5.png?raw=true)\n0 = Disable")]
            [Range(1, 5, 3, "[CnD] Discord_Connect_Style: is invalid, setting to default value (3) Please Choose From 1 To 5.\n[CnD] 1 = Text Only (Result Image : https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode1.png?raw=true)\n[CnD] 2 = Text With + Name + Hyperlink To Steam Profile (Result Image : https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode2.png?raw=true)\n[CnD] 3 = Text With + Name + Hyperlink To Steam Profile + Profile Picture (Result Image : https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode3.png?raw=true)\n[CnD] 4 = Text With + Name + Hyperlink To Steam Profile + Profile Picture + Saparate Date And Time From Message (Result Image : https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode4.png?raw=true)\n[CnD] 5 = Text With + Name + Hyperlink To Steam Profile + Profile Picture + Saparate Date And Time From Message + Server Ip In Footer (Result Image : https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode5.png?raw=true)")]
            public int Discord_Connect_Style { get; set; }

            [Comment("Required [Discord_Connect_Style 2/3/4/5]\nHow Would You Side Color Message To Be Use This Site (https://htmlcolorcodes.com/color-picker) For Color Pick")]
            public string Discord_Connect_SideColor { get; set; }

            [Comment("Required [Discord_Connect_WebHook]\nHow Do You Like Connect Message Format\n{DATE} = Discord_DateFormat\n{TIME} = Discord_TimeFormat\n{PLAYERNAME} = Player Name\n{STEAMID} = STEAM_0:1:122910632\n{STEAMID3} = U:1:245821265\n{STEAMID32} = 245821265\n{STEAMID64} = 76561198206086993\n{IP} = 123.45.67.89\n{CONTINENT} = Asia\n{LONGCOUNTRY} = United Arab Emirates\n{SHORTCOUNTRY} = AE\n{CITY} = Abu Dhabi\"\n\"\" = Disable")]
            public string Discord_Connect_Format { get; set; }

            [Comment("Discord Players Disconnect WebHook\nExample: https://discord.com/api/webhooks/XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX\n\"\" = Disable")]
            public string Discord_Disconnect_WebHook { get; set; }

            [Comment("Required [Discord_Disconnect_WebHook]\nHow Do You Like Message Look Like For Players Connect\n1 = Text Only (Result Image : https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode1.png?raw=true)\n2 = Text With + Name + Hyperlink To Steam Profile (Result Image : https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode2.png?raw=true)\n3 = Text With + Name + Hyperlink To Steam Profile + Profile Picture (Result Image : https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode3.png?raw=true)\n4 = Text With + Name + Hyperlink To Steam Profile + Profile Picture + Saparate Date And Time From Message (Result Image : https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode4.png?raw=true)\n5 = Text With + Name + Hyperlink To Steam Profile + Profile Picture + Saparate Date And Time From Message + Server Ip In Footer (Result Image : https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode5.png?raw=true)\n0 = Disable")]
            [Range(1, 5, 3, "[CnD] Discord_Disconnect_Style: is invalid, setting to default value (3) Please Choose From 1 To 5.\n[CnD] 1 = Text Only (Result Image : https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode1.png?raw=true)\n[CnD] 2 = Text With + Name + Hyperlink To Steam Profile (Result Image : https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode2.png?raw=true)\n[CnD] 3 = Text With + Name + Hyperlink To Steam Profile + Profile Picture (Result Image : https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode3.png?raw=true)\n[CnD] 4 = Text With + Name + Hyperlink To Steam Profile + Profile Picture + Saparate Date And Time From Message (Result Image : https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode4.png?raw=true)\n[CnD] 5 = Text With + Name + Hyperlink To Steam Profile + Profile Picture + Saparate Date And Time From Message + Server Ip In Footer (Result Image : https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/Mode5.png?raw=true)")]
            public int Discord_Disconnect_Style { get; set; }

            [Comment("Required [Discord_Disconnect_Style 2/3/4/5]\nHow Would You Side Color Message To Be Use This Site (https://htmlcolorcodes.com/color-picker) For Color Pick")]
            public string Discord_Disconnect_SideColor { get; set; }

            [Comment("Required [Discord_Connect_WebHook Or Discord_Disconnect_WebHook]\nHow Do You Like Date Format\nExamples:\ndd MM yyyy = 25 12 2023\nMM/dd/yy = 12/25/23\nMM-dd-yyyy = 12-25-2025")]
            public string Discord_DateFormat { get; set; }

            [Comment("Required [Discord_Connect_WebHook Or Discord_Disconnect_WebHook]\nHow Do You Like Time Format\nExamples:\nHH:mm = 14:30\nhh:mm a = 02:30 PM\nHH:mm:ss = 14:30:45")]
            public string Discord_TimeFormat { get; set; }

            [Comment("Required [Discord_Disconnect_WebHook]\nHow Do You Like Disconnect Message Format\n{DATE} = Discord_DateFormat\n{TIME} = Discord_TimeFormat\n{PLAYERNAME} = Player Name\n{STEAMID} = STEAM_0:1:122910632\n{STEAMID3} = U:1:245821265\n{STEAMID32} = 245821265\n{STEAMID64} = 76561198206086993\n{IP} = 123.45.67.89\n{CONTINENT} = Asia\n{LONGCOUNTRY} = United Arab Emirates\n{SHORTCOUNTRY} = AE\n{CITY} = Abu Dhabi\n{DISCONNECT_REASON} = Disconnect Reason\"\n\"\" = Disable")]
            public string Discord_Disconnect_Format { get; set; }

            [Comment("Required [Discord_Disconnect_Style Or Discord_Connect_SideColor 3/4/5]\nFooter Image")]
            public string Discord_FooterImage { get; set; }

            [Comment("Required [Discord_Disconnect_Style Or Discord_Connect_SideColor 5]\nIf Player Doest Have Avatar What Should We Replace It")]
            public string Discord_UsersWithNoAvatarImage { get; set; }


            [BreakLine("{space}----------------------------[ ↓ MySql Config ↓ ]----------------------------{space}")]
            [Comment("Save Players Data Into MySql?\ntrue = Yes\nfalse = No")]
            public bool MySql_Enable { get; set; }

            [Comment("MySql Host")]
            public string MySql_Host { get; set; }

            [Comment("MySql Database")]
            public string MySql_Database { get; set; }

            [Comment("MySql Username")]
            public string MySql_Username { get; set; }

            [Comment("MySql Password")]
            public string MySql_Password { get; set; }

            [Comment("MySql Port")]
            public uint MySql_Port { get; set; }

            [Comment("Required [MySql_Enable = true]\nAuto Delete Inactive Players More Than X (Days) Old\n0 = Dont Auto Delete")]
            public int MySql_AutoRemovePlayerOlderThanXDays { get; set; }


            [BreakLine("{space}----------------------------[ ↓ Utilities  ↓ ]----------------------------{space}")]

            [Comment("Auto Update Signatures (In ../Connect-Disconnect-Sound-GoldKingZ/gamedata/)?\ntrue = Yes\nfalse = No")]
            public bool AutoUpdateSignatures { get; set; }

            [Comment("Auto Update GeoLocation (In ../Connect-Disconnect-Sound-GoldKingZ/GeoLocation/)?\ntrue = Yes\nfalse = No")]
            public bool AutoUpdateGeoLocation { get; set; }

            [Comment("Enable Debug Plugin In Server Console (Helps You To Debug Issues You Facing)?\ntrue = Yes\nfalse = No")]
            public bool EnableDebug { get; set; }

            public ConfigData()
            {
                Version = MainPlugin.Instance.ModuleVersion;
                Link = "https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ";

                EarlyConnection = true;
                DisableLoopConnections = true;
                DisableServerHibernate = true;
                PickRandomSounds = true;
                RemoveDefaultDisconnect = 2;
                Toggle_Sounds_CommandsInGame  = "!sound,!sounds";
                Toggle_Sounds_Flags  = "@css/vvip,#css/vvip";
                Toggle_Messages_CommandsInGame  = "!message,!message";
                Toggle_Messages_Flags  = "@css/vvip,#css/vvip";
                Default_Sounds = true;
                Default_Messages = true;
                DateFormat = "MM-dd-yyyy";
                TimeFormat = "HH:mm:ss";

                Log_Locally_Enable = true;
                Log_Locally_DateFormat = "MM-dd-yyyy";
                Log_Locally_TimeFormat = "HH:mm:ss";
                Log_Locally_Connect_Format = "[{DATE} - {TIME}] [{STEAMID64} - {PLAYERNAME}] Connected From [{CONTINENT} - {LONGCOUNTRY} - {CITY}] [{IP}]";
                Log_Locally_Disconnect_Format = "[{DATE} - {TIME}] [{STEAMID64} - {PLAYERNAME}] Disconnected From [{CONTINENT} - {LONGCOUNTRY} - {CITY}] [{IP} - {DISCONNECT_REASON}]";
                Log_Locally_AutoDeleteLogsMoreThanXdaysOld = 7;
                Cookies_Enable = true;
                Cookies_AutoRemovePlayerOlderThanXDays = 7;

                Discord_Connect_WebHook = "";
                Discord_Connect_Style = 4;
                Discord_Connect_SideColor = "0cff00";
                Discord_Connect_Format = "{PLAYERNAME} Connected From [{SHORTCOUNTRY} - {CITY}]";
                Discord_Disconnect_WebHook = "";
                Discord_Disconnect_Style = 4;
                Discord_Disconnect_SideColor = "ff0000";
                Discord_Disconnect_Format = "{PLAYERNAME} Disconnected From [{SHORTCOUNTRY} - {CITY}]";
                Discord_DateFormat = "MM-dd-yyyy";
                Discord_TimeFormat = "HH:mm:ss";
                Discord_FooterImage = "https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/serverip.png?raw=true";
                Discord_UsersWithNoAvatarImage = "https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ/blob/main/Resources/avatar.jpg?raw=true";

                MySql_Enable = false;
                MySql_Host = "MySql_Host";
                MySql_Database = "MySql_Database";
                MySql_Username = "MySql_Username";
                MySql_Password = "MySql_Password";
                MySql_Port = 3306;
                MySql_AutoRemovePlayerOlderThanXDays = 7;

                AutoUpdateSignatures = true;
                AutoUpdateGeoLocation = false;
                EnableDebug = false;
            }
            public void Validate()
            {
                foreach (var prop in GetType().GetProperties())
                {
                    var rangeAttr = prop.GetCustomAttribute<RangeAttribute>();
                    if (rangeAttr != null && prop.PropertyType == typeof(int))
                    {
                        int value = (int)prop.GetValue(this)!;
                        if (value < rangeAttr.Min || value > rangeAttr.Max)
                        {
                            prop.SetValue(this, rangeAttr.Default);
                            Helper.DebugMessage(rangeAttr.Message,false);
                        }
                    }
                }
            }
        }
    }
}
