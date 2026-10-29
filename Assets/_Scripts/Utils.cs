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

        public static GameLevel Load(int world, int subWorld, int level)
        {
            if (level < 0 || level >= GetNumLevels(world, subWorld)) return null;
            var loc = Script.Utils.Locator<WordCross.Gameplay.LocalizationController>.Instance;
            var folder = loc == null ? "Levels" : loc.LevelFolder;
            return CatGaming.Clr.ClrContent.Instance.Load<GameLevel>(folder + "/World_" + world + "/SubWorld_" + subWorld + "/Level_" + level);
        }
    }
}