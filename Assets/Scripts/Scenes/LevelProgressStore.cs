using UnityEngine;

public static class LevelProgressStore
{
    private const string KeyLastCompletedName = "LastCompletedLevelName";

    public static void SetLastCompletedName(string sceneName)
    {
        PlayerPrefs.SetString(KeyLastCompletedName, sceneName);
        PlayerPrefs.Save();
        Debug.Log($"[LevelProgressStore] Guardado último nivel completado: {sceneName}");
    }

    public static string GetLastCompletedName()
    {
        return PlayerPrefs.GetString(KeyLastCompletedName, string.Empty);
    }

    public static void Clear()
    {
        PlayerPrefs.DeleteKey(KeyLastCompletedName);
    }
}
