using MySqlConnector;
using CnD_Sound.Config;

namespace CnD_Sound;

public class MySqlDataManager
{
    private static string ConnectionString => new MySqlConnectionStringBuilder
    {
        Server = Configs.GetConfigData().MySql_Host,
        Port = Configs.GetConfigData().MySql_Port,
        Database = Configs.GetConfigData().MySql_Database,
        UserID = Configs.GetConfigData().MySql_Username,
        Password = Configs.GetConfigData().MySql_Password,
        Pooling = true,
        MinimumPoolSize = 0,
        MaximumPoolSize = 100
    }.ConnectionString;

    public static async Task CreateTableIfNotExistsAsync()
    {
        try
        {
            await using var connection = new MySqlConnection(ConnectionString);
            await connection.OpenAsync();

            bool tableExists;
            await using (var checkCmd = new MySqlCommand("SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = DATABASE() AND table_name = 'CnD_PersonData'", connection))
            {
                tableExists = Convert.ToInt32(await checkCmd.ExecuteScalarAsync()) > 0;
            }

            const string query = @"
                CREATE TABLE IF NOT EXISTS CnD_PersonData (
                    PlayerSteamID BIGINT UNSIGNED PRIMARY KEY,
                    Toggle_Messages INT NOT NULL DEFAULT 0,
                    Toggle_Sounds INT NOT NULL DEFAULT 0,
                    DateAndTime DATETIME NOT NULL
                );";

            await using (var command = new MySqlCommand(query, connection))
            {
                await command.ExecuteNonQueryAsync();
            }

            if (tableExists)
            {
                Helper.DebugMessage("Database table already exists - verified structure");
            }
            else
            {
                Helper.DebugMessage("Database table created successfully");
            }
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"DB Init Error: {ex.Message}");
        }
    }

    public static async Task SaveToMySqlAsync(Globals_Static.PersonData data)
    {
        const string insertOrUpdateQuery = @"
            INSERT INTO CnD_PersonData 
                (PlayerSteamID, Toggle_Messages, Toggle_Sounds, DateAndTime)
            VALUES 
                (@PlayerSteamID, @Toggle_Messages, @Toggle_Sounds, @DateAndTime)
            ON DUPLICATE KEY UPDATE 
                Toggle_Messages = VALUES(Toggle_Messages),
                Toggle_Sounds = VALUES(Toggle_Sounds),
                DateAndTime = VALUES(DateAndTime)";
        
        try
        {
            using var connection = new MySqlConnection(ConnectionString);
            await connection.OpenAsync();
            await using var command = new MySqlCommand(insertOrUpdateQuery, connection);
            AddPersonDataParameters(command, data);
            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"Saving Values In MySql Error: {ex.Message}");
        }
    }

    public static async Task<Globals_Static.PersonData> RetrievePersonDataByIdAsync(ulong steamId)
    {
        const string retrieveQuery = "SELECT * FROM CnD_PersonData WHERE PlayerSteamID = @PlayerSteamID";
        try
        {
            await using var connection = new MySqlConnection(ConnectionString);
            await connection.OpenAsync();
            
            await using var command = new MySqlCommand(retrieveQuery, connection);
            command.Parameters.Add("@PlayerSteamID", MySqlDbType.UInt64).Value = steamId;

            await using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                return new Globals_Static.PersonData
                {
                    PlayerSteamID = reader.GetUInt64("PlayerSteamID"),
                    Toggle_Messages = reader.GetInt32("Toggle_Messages"),
                    Toggle_Sounds = reader.GetInt32("Toggle_Sounds"),
                    DateAndTime = reader.GetDateTime("DateAndTime")
                };
            }
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"Retrieve Values In MySql Error: {ex.Message}");
        }
        return new Globals_Static.PersonData();
    }

    public static async Task DeleteOldPlayersAsync()
    {
        try
        {
            int days = Configs.GetConfigData().MySql_AutoRemovePlayerOlderThanXDays;
            const string cleanupQuery = "DELETE FROM CnD_PersonData WHERE DateAndTime < NOW() - INTERVAL @Days DAY";
            
            await using var connection = new MySqlConnection(ConnectionString);
            await connection.OpenAsync();
            
            await using var cleanupCommand = new MySqlCommand(cleanupQuery, connection);
            cleanupCommand.Parameters.Add("@Days", MySqlDbType.Int32).Value = days;
            await cleanupCommand.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"Delete Old Players In MySql Error: {ex.Message}");
        }
    }

    private static void AddPersonDataParameters(MySqlCommand command, Globals_Static.PersonData data)
    {
        command.Parameters.Add("@PlayerSteamID", MySqlDbType.UInt64).Value = data.PlayerSteamID;
        command.Parameters.Add("@Toggle_Messages", MySqlDbType.Int32).Value = data.Toggle_Messages;
        command.Parameters.Add("@Toggle_Sounds", MySqlDbType.Int32).Value = data.Toggle_Sounds;
        command.Parameters.Add("@DateAndTime", MySqlDbType.DateTime).Value = data.DateAndTime;
    }
}