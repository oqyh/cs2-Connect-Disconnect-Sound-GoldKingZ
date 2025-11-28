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
        public int Toggle_Messages { get; set; }
        public int Toggle_Sounds { get; set; }
        public DateTime EventPlayerChat { get; set; }
        public PlayerDataClass(CCSPlayerController Playerr, ulong SteamIdd, int Toggle_Messagess, int Toggle_Soundss, DateTime EventPlayerChatt)
        {
            Player = Playerr;
            SteamId = SteamIdd;
            Toggle_Messages = Toggle_Messagess;
            Toggle_Sounds = Toggle_Soundss;
            EventPlayerChat = EventPlayerChatt;
        }
    }
    public Dictionary<int, PlayerDataClass> Player_Data = new Dictionary<int, PlayerDataClass>();
    public JObject? JsonData_connect_disconnect_config { get; set; }
    public JObject? JsonData_disconnect_reasons { get; set; }
    public Dictionary<int, HashSet<string>> Player_Disconnect_Reasons = new Dictionary<int, HashSet<string>>();
    public Dictionary<string, HashSet<string>> GlobalSoundTracker = new();

    public void Clear()
    {

        JsonData_connect_disconnect_config = null;
        JsonData_disconnect_reasons = null;

        Player_Disconnect_Reasons?.Clear();
        GlobalSoundTracker?.Clear();
    }

}