using UnityEngine;

// Toàn bộ level của một ngôn ngữ chơi (vi/fr/es/pt) trong MỘT asset: ClrBootstrap nạp trước mọi asset trong index, nên không tách từng level.
public class LocaleLevelPack : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public int world;
        public int subWorld;
        public int level;
        public string word;
        public string answers;
        public string validWords;
    }

    public Entry[] entries;
}
