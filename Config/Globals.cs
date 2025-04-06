using CounterStrikeSharp.API.Core;
using Newtonsoft.Json.Linq;

namespace CnD_Sound;

public static class Globals_Static
{
    public class PersonData
    {
        public ulong PlayerSteamID { get; set; }
        public int Toggle_Messages { get; set; }
        public int Toggle_Sounds { get; set; }
        public DateTime DateAndTime { get; set; }
    }
}

public class Globals
{
    public string ServerPublicIpAdress = "";
    public string ServerPort = "";
    public class PlayerDataClass
    {
        public CCSPlayerController Player { get; set; }
        public ulong SteamId { get; set; }
        public bool Remove_Icon { get; set; }
        public int Toggle_Messages { get; set; }
        public int Toggle_Sounds { get; set; }
        public PlayerDataClass(CCSPlayerController player, ulong steamId, bool remove_Icon, int toggle_Messages, int toggle_Sounds)
        {
            Player = player;
            SteamId = steamId;
            Remove_Icon = remove_Icon;
            Toggle_Messages = toggle_Messages;
            Toggle_Sounds = toggle_Sounds;
        }
    }
    public Dictionary<CCSPlayerController, PlayerDataClass> Player_Data = new Dictionary<CCSPlayerController, PlayerDataClass>();
    public Dictionary<CCSPlayerController, bool> OnLoop = new Dictionary<CCSPlayerController, bool>();
    public JObject? JsonData_Disconnect { get; set; }
    public JObject? JsonData_Settings { get; set; }

    public Dictionary<string, int> sequentialIndices = new Dictionary<string, int>();
    public Dictionary<string, Queue<string>> randomQueues = new Dictionary<string, Queue<string>>();

}