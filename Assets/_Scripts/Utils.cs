using UnityEngine;
using System.Collections.Generic;

namespace Superpow
{
    public class Utils
    {
        // Số world có nội dung (World_0..World_5); world >= NUM_WORLDS nghĩa là đã hoàn thành hết.
        public const int NUM_WORLDS = 6;

        public static int GetNumLevels(int world, int subWorld)
        {
            if (world < 0 || world >= NUM_WORLDS || subWorld < 0 || subWorld >= Const.NUM_SUBWORLD) return 0;

            // Indicate the number of levels in specific sub-worlds.
            int[,] numLevels =
            {
                { 12, 18, 18, 18, 18 }, // For world 0
                { 18, 18, 18, 18, 18 }, // For world 1
                { 18, 18, 18, 18, 18 }, // For world 2
                { 18, 18, 18, 18, 18 }, // For world 3
                { 18, 18, 18, 18, 18 }, // For world 4
                { 18, 18, 18, 18, 18 }, // For world 5
                { 18, 18, 18, 18, 18 }, // Not used yet
                { 18, 18, 18, 18, 18 }, // Not used yet
                { 18, 18, 18, 18, 18 }, // Not used yet
                { 18, 18, 18, 18, 18 }  // Not used yet
            };

            return numLevels[world, subWorld];
        }

        public static int GetLeaderboardScore()
        {
            int levelInSub = Prefs.unlockedWorld == 0 && Prefs.unlockedSubWorld == 0 ? 12 : 18;
            int score = (Prefs.unlockedWorld * 5 + Prefs.unlockedSubWorld) * levelInSub + Prefs.unlockedLevel;

            if (levelInSub == 18) score -= 6;
            return score;
        }

        // Locale khác ngôn ngữ chơi mặc định: pack theo locale và GameLevel dựng runtime, cache để mỗi (locale, world, sub, level) luôn cùng một instance.
        private static readonly Dictionary<string, LocaleLevelPack> packs = new Dictionary<string, LocaleLevelPack>();
        private static readonly Dictionary<string, GameLevel> packLevels = new Dictionary<string, GameLevel>();

        public static GameLevel Load(int world, int subWorld, int level)
        {
            if (level < 0 || level >= GetNumLevels(world, subWorld)) return null;
            var loc = Script.Utils.Locator<WordCross.Gameplay.LocalizationController>.Instance;
            if (loc == null || loc.GameLanguage == loc.DefaultGameLanguage)
                return CatGaming.Clr.ClrContent.Instance.Load<GameLevel>("Levels/World_" + world + "/SubWorld_" + subWorld + "/Level_" + level);

            var lang = loc.GameLanguage;
            var key = lang + "/" + world + "/" + subWorld + "/" + level;
            GameLevel gameLevel;
            if (packLevels.TryGetValue(key, out gameLevel)) return gameLevel;

            LocaleLevelPack pack;
            if (!packs.TryGetValue(lang, out pack))
            {
                pack = CatGaming.Clr.ClrContent.Instance.Load<LocaleLevelPack>(loc.LevelFolder + "/Pack");
                packs[lang] = pack;
            }
            if (pack == null) return null;

            foreach (var entry in pack.entries)
            {
                if (entry.world != world || entry.subWorld != subWorld || entry.level != level) continue;
                gameLevel = ScriptableObject.CreateInstance<GameLevel>();
                gameLevel.word = entry.word;
                gameLevel.answers = entry.answers;
                gameLevel.validWords = entry.validWords;
                packLevels[key] = gameLevel;
                return gameLevel;
            }
            return null;
        }
    }
}