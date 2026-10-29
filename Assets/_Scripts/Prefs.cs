using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Prefs {

    public static readonly WordCross.Gameplay.ProgressKeys progressKeys = new WordCross.Gameplay.ProgressKeys();

    public static int unlockedWorld
    {
        get
        {
            if (GameState.unlockedWorld == -1)
            {
                int value = CPlayerPrefs.GetInt(progressKeys.Of("unlocked_world"));
                GameState.unlockedWorld = value;
            }
            return GameState.unlockedWorld;
        }
        set { CPlayerPrefs.SetInt(progressKeys.Of("unlocked_world"), value); GameState.unlockedWorld = value; }
    }

    public static int unlockedSubWorld
    {
        get
        {
            if (GameState.unlockedSubWord == -1)
            {
                int value = CPlayerPrefs.GetInt(progressKeys.Of("unlocked_sub_world"));
                GameState.unlockedSubWord = value;
            }
            return GameState.unlockedSubWord;
        }
        set { CPlayerPrefs.SetInt(progressKeys.Of("unlocked_sub_world"), value); GameState.unlockedSubWord = value; }
    }

    public static int unlockedLevel
    {
        get
        {
            if (GameState.unlockedLevel == -1)
            {
                int value = CPlayerPrefs.GetInt(progressKeys.Of("unlocked_level"));
                GameState.unlockedLevel = value;
            }
            return GameState.unlockedLevel;
        }
        set { CPlayerPrefs.SetInt(progressKeys.Of("unlocked_level"), value); GameState.unlockedLevel = value; }
    }

    public static List<int> GetPanWordIndexes(int world, int subWorld, int level)
    {
        string data = PlayerPrefs.GetString(progressKeys.Of("pan_word_indexes_v2_") + world + "_" + subWorld + "_" + level);
        return CUtils.BuildListFromString<int>(data);
    }

    public static void SetPanWordIndexes(int world, int subWorld, int level, int[] indexes)
    {
        string data = CUtils.BuildStringFromCollection(indexes);
        PlayerPrefs.SetString(progressKeys.Of("pan_word_indexes_v2_") + world + "_" + subWorld + "_" + level, data);
    }

    public static bool IsLastLevel()
    {
        // Bàn Daily không phải level tiến độ chính: không lưu/đọc level_progress, không tính là thắng lần đầu.
        if (GameState.isDaily) return false;
        return  GameState.currentWorld == unlockedWorld &&
                GameState.currentSubWorld == unlockedSubWorld && 
                GameState.currentLevel == unlockedLevel;
    }

    public static void SetExtraWords(int world, int subWorld, int level, string[] extraWords)
    {
        CryptoPlayerPrefsX.SetStringArray(progressKeys.Of("extra_words_") + world + "_" + subWorld + "_" + level, extraWords);
    }

    public static string[] GetExtraWords(int world, int subWorld, int level)
    {
        return CryptoPlayerPrefsX.GetStringArray(progressKeys.Of("extra_words_") + world + "_" + subWorld + "_" + level);
    }

    public static int extraProgress
    {
        get { return CPlayerPrefs.GetInt(progressKeys.Of("extra_progress"), 0); }
        set { CPlayerPrefs.SetInt(progressKeys.Of("extra_progress"), value); }
    }

    public static int extraTarget
    {
        get { return CPlayerPrefs.GetInt(progressKeys.Of("extra_target"), 5); }
        set { CPlayerPrefs.SetInt(progressKeys.Of("extra_target"), value); }
    }

    public static int totalExtraAdded
    {
        get { return CPlayerPrefs.GetInt(progressKeys.Of("total_extra_added"), 0); }
        set { CPlayerPrefs.SetInt(progressKeys.Of("total_extra_added"), value); }
    }

    public static string[] levelProgress
    {
        get { return CryptoPlayerPrefsX.GetStringArray(progressKeys.Of("level_progress")); }
        set { CryptoPlayerPrefsX.SetStringArray(progressKeys.Of("level_progress"), value); }
    }

    public static bool isNoti1Enabled
    {
        get { return PlayerPrefs.GetInt("is_noti_1_enabled") == 1; }
        set { PlayerPrefs.SetInt("is_noti_1_enabled", value ? 1 : 0); }
    }

    public static bool isNoti2Enabled
    {
        get { return PlayerPrefs.GetInt("is_noti_2_enabled") == 1; }
        set { PlayerPrefs.SetInt("is_noti_2_enabled", value ? 1 : 0); }
    }

    public static int noti3Ruby
    {
        get { return PlayerPrefs.GetInt("noti_3_ruby"); }
        set { PlayerPrefs.SetInt("noti_3_ruby", value); }
    }

    public static int noti4Ruby
    {
        get { return PlayerPrefs.GetInt("noti_4_ruby"); }
        set { PlayerPrefs.SetInt("noti_4_ruby", value); }
    }

    public static int noti5Ruby
    {
        get { return PlayerPrefs.GetInt("noti_5_ruby"); }
        set { PlayerPrefs.SetInt("noti_5_ruby", value); }
    }

    public static int noti6Ruby
    {
        get { return PlayerPrefs.GetInt("noti_6_ruby"); }
        set { PlayerPrefs.SetInt("noti_6_ruby", value); }
    }

    public static int noti7Ruby
    {
        get { return PlayerPrefs.GetInt("noti_7_ruby"); }
        set { PlayerPrefs.SetInt("noti_7_ruby", value); }
    }

    public static void AddToNumHint(int world, int subWorld, int level)
    {
        int numHint = GetNumHint(world, subWorld, level);
        PlayerPrefs.SetInt(progressKeys.Of("numhint_used_") + world + "_" + subWorld + "_" + level, numHint + 1);
    }

    public static int GetNumHint(int world, int subWorld, int level)
    {
        return PlayerPrefs.GetInt(progressKeys.Of("numhint_used_") + world + "_" + subWorld + "_" + level);
    }
}
