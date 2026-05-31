using UnityEngine;

public static class LastLevelStore
{
    private const string Key = "LastLevelIndex";

    public static int Get()
    {
        return PlayerPrefs.GetInt(Key, -1); 
    }

    public static void Set(int buildIndex)
    {
        PlayerPrefs.SetInt(Key, buildIndex);
        PlayerPrefs.Save(); 
        Debug.Log($"[LastLevelStore] Guardado buildIndex={buildIndex}");
    }

    public static void Clear()
    {
        PlayerPrefs.DeleteKey(Key);
    }
}
