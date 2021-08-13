using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSytem
{
    public static void SaveGame(GameObject player, GameManager gameManager, EnemyManager enemyManager, TerrainManager terrainManager)
    {
        DeleteGame();
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Path.Combine(Application.persistentDataPath, "savedGame.pocketmonster");
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            PlayerData data = new PlayerData(player, gameManager, enemyManager, terrainManager);
            Debug.Log("saved game");
            formatter.Serialize(stream, data);
            stream.Close();
        }
    }

    public static PlayerData LoadGame()
    {
        string path = Path.Combine(Application.persistentDataPath, "savedGame.pocketmonster");
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(path, FileMode.Open)) {

                PlayerData data = formatter.Deserialize(stream) as PlayerData;
                stream.Close();
                return data;
            }
        }
        else
        {
            Debug.LogError("No file found");
            return null;
        }
    }

    public static void DeleteGame()
    {
        string path = Path.Combine(Application.persistentDataPath, "savedGame.pocketmonster");
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("deleted game");
        }
        else
        {
            Debug.Log("No file found");
        }
    }

    public static bool CheckIfFileExist()
    {
        string path = Path.Combine(Application.persistentDataPath, "savedGame.pocketmonster");
        if (File.Exists(path))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
