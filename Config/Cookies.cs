using Newtonsoft.Json;
using CnD_Sound.Config;
using CounterStrikeSharp.API;
using System.Collections.Concurrent;

namespace CnD_Sound;

public class Cookies
{
    private static readonly SemaphoreSlim _fileLock = new SemaphoreSlim(1, 1);
    private static readonly SemaphoreSlim _loadLock = new SemaphoreSlim(1, 1);
    private static readonly ConcurrentDictionary<ulong, Globals_Static.PersonData> _cachedData = new ConcurrentDictionary<ulong, Globals_Static.PersonData>();
    private static bool _isDataLoaded = false;
    private static bool _isSaving = false;
    private static readonly ConcurrentQueue<SaveRequest> _saveQueue = new ConcurrentQueue<SaveRequest>();
    private static readonly object _saveLock = new object();
    private static DateTime _lastFileWriteTime = DateTime.MinValue;

    private class SaveRequest
    {
        public ulong PlayerSteamID { get; set; }
        public Globals_Static.PersonData? Data { get; set; }
    }

    public static async Task SaveToJsonFile(ulong PlayerSteamID, int Toggle_Messages, int Toggle_Sounds, DateTime DateAndTime)
    {
        var newData = new Globals_Static.PersonData
        {
            PlayerSteamID = PlayerSteamID,
            Toggle_Messages = Toggle_Messages,
            Toggle_Sounds = Toggle_Sounds,
            DateAndTime = DateAndTime
        };

        _cachedData.AddOrUpdate(PlayerSteamID, newData, (key, oldValue) => newData);

        var saveRequest = new SaveRequest { PlayerSteamID = PlayerSteamID, Data = newData };
        _saveQueue.Enqueue(saveRequest);

        await ProcessSaveQueue();
    }

    private static async Task ProcessSaveQueue()
    {
        lock (_saveLock)
        {
            if (_isSaving) return;
            _isSaving = true;
        }

        await Task.Run(async () =>
        {
            await _fileLock.WaitAsync();
            try
            {
                var cookiesPath = GetCookiesFilePath();
                await EnsureDataLoadedAsync(cookiesPath);

                var changedPlayers = new HashSet<ulong>();
                while (_saveQueue.TryDequeue(out var saveRequest))
                {
                    if (saveRequest.Data != null)
                    {
                        _cachedData.AddOrUpdate(saveRequest.PlayerSteamID, saveRequest.Data, (key, oldValue) => saveRequest.Data!);
                        changedPlayers.Add(saveRequest.PlayerSteamID);
                    }
                }

                if (changedPlayers.Count > 0)
                {
                    await SaveAllDataAsync(cookiesPath);
                    Helper.DebugMessage($"Saved {changedPlayers.Count} player cookies. Total in cache: {_cachedData.Count}");
                }
            }
            catch (Exception ex)
            {
                Helper.DebugMessage($"ProcessSaveQueue error: {ex.Message}", true);
            }
            finally
            {
                _fileLock.Release();
                lock (_saveLock)
                {
                    _isSaving = false;
                }

                if (!_saveQueue.IsEmpty)
                {
                    _ = Task.Run(async () => await ProcessSaveQueue());
                }
            }
        });
    }

    private static async Task EnsureDataLoadedAsync(string cookiesPath)
    {
        bool needsReload = ShouldReloadDataAsync(cookiesPath);
        
        if (_isDataLoaded && !needsReload) return;

        await _loadLock.WaitAsync();
        try
        {
            needsReload = ShouldReloadDataAsync(cookiesPath);
            if (_isDataLoaded && !needsReload) return;

            try
            {
                if (File.Exists(cookiesPath))
                {
                    var json = await File.ReadAllTextAsync(cookiesPath);
                    var existingData = JsonConvert.DeserializeObject<List<Globals_Static.PersonData>>(json) ?? new List<Globals_Static.PersonData>();
                    
                    _cachedData.Clear();
                    foreach (var person in existingData)
                    {
                        _cachedData.TryAdd(person.PlayerSteamID, person);
                    }
                    
                    _lastFileWriteTime = File.GetLastWriteTime(cookiesPath);
                    Helper.DebugMessage($"Loaded {_cachedData.Count} player cookies from file");
                }
                else
                {
                    _cachedData.Clear();
                    _lastFileWriteTime = DateTime.MinValue;
                    Helper.DebugMessage("No existing cookies file found, starting with empty cache");
                }
            }
            catch (Exception ex)
            {
                Helper.DebugMessage($"Error loading cookies data: {ex.Message}", true);
                _cachedData.Clear();
                _lastFileWriteTime = DateTime.MinValue;
            }
            
            _isDataLoaded = true;
        }
        finally
        {
            _loadLock.Release();
        }
    }

    private static bool ShouldReloadDataAsync(string cookiesPath)
    {
        try
        {
            if (!File.Exists(cookiesPath))
            {
                return _isDataLoaded;
            }

            var currentWriteTime = File.GetLastWriteTime(cookiesPath);
            
            if (_lastFileWriteTime == DateTime.MinValue || currentWriteTime > _lastFileWriteTime)
            {
                Helper.DebugMessage($"File changed detected. Reloading data. Last: {_lastFileWriteTime}, Current: {currentWriteTime}");
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"Error checking file modification time: {ex.Message}", true);
            return false;
        }
    }

    private static async Task SaveAllDataAsync(string cookiesPath)
    {
        var tempPath = cookiesPath + ".tmp";
        try
        {
            var directory = Path.GetDirectoryName(cookiesPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var allPlayersData = _cachedData.Values.ToList();
            var jsonData = JsonConvert.SerializeObject(allPlayersData, Formatting.Indented);
            
            await File.WriteAllTextAsync(tempPath, jsonData);
            
            File.Move(tempPath, cookiesPath, true);
            
            _lastFileWriteTime = File.GetLastWriteTime(cookiesPath);
            
            Helper.DebugMessage($"Successfully saved {allPlayersData.Count} player cookies to file");
        }
        catch (Exception ex)
        {
            try
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
            }
            catch
            {
            }
            
            Helper.DebugMessage($"SaveAllDataAsync error: {ex.Message}", true);
            throw;
        }
    }

    public static async Task<Globals_Static.PersonData> RetrievePersonDataById(ulong targetId)
    {
        await _fileLock.WaitAsync();
        try
        {
            var cookiesPath = GetCookiesFilePath();
            await EnsureDataLoadedAsync(cookiesPath);

            if (_cachedData.TryGetValue(targetId, out var personData))
            {
                return personData;
            }
            else
            {
                Helper.DebugMessage($"No data found for SteamID: {targetId}");
                return new Globals_Static.PersonData();
            }
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"Error retrieving data for {targetId}: {ex.Message}", true);
            return new Globals_Static.PersonData();
        }
        finally
        {
            _fileLock.Release();
        }
    }

    public static async Task RemoveOldEntries()
    {
        if (Configs.Instance.Cookies_AutoRemovePlayerOlderThanXDays < 1) 
            return;

        await _fileLock.WaitAsync();
        try
        {
            var cookiesPath = GetCookiesFilePath();
            await EnsureDataLoadedAsync(cookiesPath);

            int daysToKeep = Configs.Instance.Cookies_AutoRemovePlayerOlderThanXDays;
            var removalCutoff = DateTime.Now.AddDays(-daysToKeep);
            
            var oldEntries = _cachedData.Where(kv => kv.Value.DateAndTime < removalCutoff)
                                      .Select(kv => kv.Key).ToList();
            
            int removedCount = 0;
            foreach (var oldKey in oldEntries)
            {
                if (_cachedData.TryRemove(oldKey, out _))
                {
                    removedCount++;
                }
            }

            if (removedCount > 0)
            {
                await SaveAllDataAsync(cookiesPath);
                Helper.DebugMessage($"Removed {removedCount} player entries older than {daysToKeep} days. Cache size: {_cachedData.Count}");
            }
            else
            {
                Helper.DebugMessage($"No old entries found to remove (cutoff: {removalCutoff})");
            }
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"RemoveOldEntries error: {ex.Message}", true);
        }
        finally
        {
            _fileLock.Release();
        }
    }

    private static string GetCookiesFilePath()
    {
        return Path.Combine(MainPlugin.Instance.ModuleDirectory, "cookies", "cookies.json");
    }
}