using UnityEditor.SpeedTree.Importer;
using UnityEngine;

public static class SaveSystem
{
    private const string Key = "GAME_STATE";

    public static void Save(GameState state)
    {
        if (GameController.IsGameOver) return;

        var json = JsonUtility.ToJson(state.ToSaveData());
        Debug.Log("Saving game state: " + json);
        PlayerPrefs.SetString(Key, json);
        PlayerPrefs.Save();
    }

    public static bool TryLoad(GameState state)
    {
        if (!PlayerPrefs.HasKey(Key)) return false;
        var json = PlayerPrefs.GetString(Key);
        var data = JsonUtility.FromJson<GameStateSaveData>(json);
        if (data == null) return false;
        state.LoadFromSaveData(data);
        Debug.Log("Loaded game state: " + json);
        return true;
    }

    public static void Clear()
    {
        PlayerPrefs.DeleteKey(Key);
        PlayerPrefs.Save();
    }
}