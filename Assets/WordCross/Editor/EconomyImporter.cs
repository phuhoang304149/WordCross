using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SimpleJSON;
using UnityEditor;
using UnityEngine;
using Util.Editor;
using WordCross.Gameplay;

namespace WordCross.Editor
{
    // Nạp design/GD/economy-year-data/{economy-config,quest-templates,calendar-365,loc-keys}.json vào EconomyData.asset (Addressables, OTA AA)
    // và gộp chuỗi vào LocalizationData (vi/en từ loc-keys.json; fr/es/pt + khoá UI econ.* + chuỗi đổi ruby→coin từ tools/econ/economy-loc.json).
    // Hàm static chỉ là điểm vào cho Unity (-executeMethod), phần việc nằm ở hàm instance.
    public class EconomyImporter
    {
        private const string DataFolder = "design/GD/economy-year-data";
        private const string LocExtraPath = "tools/econ/economy-loc.json";
        private const string AssetPath = "Assets/WordCross/Config/GameData/Economy/EconomyData.asset";
        private const string LocalizationPath = "Assets/WordCross/Config/GameData/Localization/LocalizationData.asset";
        private const string ConfigPath = "Assets/WordCross/Config/Editor/AddressableConfig.asset";
        // Quyết định T134: neo 2026-10-26 (2027-01-01 của GD nằm ở tương lai → dayNumber âm). Ghi đè calendar.cycle.anchorUtcDate.
        private const string AnchorOverride = "2026-10-26";
        private const int RubyToCoin = 10;

        public static void ImportAll()
        {
            new EconomyImporter().Run();
        }

        private string root;

        private JSONNode Read(string relative)
        {
            return JSON.Parse(File.ReadAllText(Path.Combine(root, relative), Encoding.UTF8));
        }

        private string Str(JSONNode node)
        {
            string value = node == null ? "" : node.Value;
            return value == "null" ? "" : value;
        }

        private string[] Strs(JSONNode node)
        {
            var list = new List<string>();
            for (int i = 0; i < node.Count; i++) list.Add(Str(node[i]));
            return list.ToArray();
        }

        private void Run()
        {
            root = Directory.GetParent(Application.dataPath).FullName;
            var config = Read(DataFolder + "/economy-config.json");
            var questFile = Read(DataFolder + "/quest-templates.json");
            var calendar = Read(DataFolder + "/calendar-365.json");
            var loc = Read(DataFolder + "/loc-keys.json");

            var data = AssetDatabase.LoadAssetAtPath<EconomyData>(AssetPath);
            bool isNew = data == null;
            if (isNew) data = ScriptableObject.CreateInstance<EconomyData>();

            data.startBalance = config["currency"]["startBalance"].AsInt;
            data.rubyToCoin = RubyToCoin;
            var earn = config["earn"];
            data.firstWinCoin = earn["firstWinCoin"].AsInt;
            data.firstEverWinBonusCoin = earn["firstEverWinBonusCoin"].AsInt;
            data.replayWinCoin = earn["replayWinCoin"].AsInt;
            data.dailyChallengeCoin = earn["dailyChallengeCoin"].AsInt;
            data.botEasyCoin = earn["botWinCoinByDifficulty"]["easy"].AsInt;
            data.botMediumCoin = earn["botWinCoinByDifficulty"]["medium"].AsInt;
            data.botHardCoin = earn["botWinCoinByDifficulty"]["hard"].AsInt;
            data.botDrawCoin = earn["botDrawCoin"].AsInt;
            data.botDailyCap = earn["botDailyCap"].AsInt;
            data.extraWordCoin = earn["extraWordCoin"].AsInt;
            data.extraWordCapPerLevel = earn["extraWordCapPerLevel"].AsInt;

            var sinks = new List<EconomyData.Sink>();
            foreach (JSONNode n in config["sinks"].Childs)
                sinks.Add(new EconomyData.Sink { id = Str(n["id"]), coin = n["coin"].AsInt, locKey = Str(n["locKey"]), maxMissedDays = n["maxMissedDays"].AsInt });
            data.sinks = sinks.ToArray();

            var packs = new List<EconomyData.Pack>();
            foreach (JSONNode n in config["shop"]["packs"].Childs)
                packs.Add(new EconomyData.Pack { id = Str(n["id"]), productId = Str(n["productId"]), coin = n["coin"].AsInt, removesAds = n["removesAds"].AsBool, locKey = Str(n["locKey"]) });
            data.packs = packs.ToArray();

            var ads = config["ads"];
            var rewarded = new List<EconomyData.Rewarded>();
            foreach (JSONNode n in ads["rewarded"].Childs)
                rewarded.Add(new EconomyData.Rewarded { id = Str(n["id"]), placement = Str(n["placement"]), coin = n["coin"].AsInt, cooldownSec = n["cooldownSec"].AsInt, dailyCap = n["dailyCap"].AsInt, sharedCapGroup = Str(n["sharedCapGroup"]) });
            data.rewarded = rewarded.ToArray();
            var inter = ads["interstitial"];
            data.interstitial = new EconomyData.Interstitial
            {
                graceLevels = inter["graceLevels"].AsInt,
                minGapSec = inter["minGapSec"].AsInt,
                dailyCap = inter["dailyCap"].AsInt,
                blockedAfterRewardedSec = inter["blockedAfterRewardedSec"].AsInt,
                disabledBy = Strs(inter["disabledBy"])
            };
            data.maxRewardedCoinPerDay = ads["maxRewardedCoinPerDay"].AsInt;

            data.streakMilestones = Milestones(config["streak"]["milestones"], "streakDays");
            data.levelMilestones = Milestones(config["levelMilestones"], "levelsCleared");

            var quests = new List<EconomyData.Quest>();
            foreach (JSONNode n in questFile["templates"].Childs)
                quests.Add(new EconomyData.Quest { id = Str(n["id"]), scope = Str(n["scope"]), metric = Str(n["metric"]), target = n["target"].AsInt, coin = n["coin"].AsInt, locKey = Str(n["locKey"]) });
            data.quests = quests.ToArray();
            data.dailyAllClearCoin = questFile["dailyAllClearCoin"].AsInt;

            data.anchorUtcDate = AnchorOverride;
            data.lengthDays = calendar["cycle"]["lengthDays"].AsInt;
            var logins = new List<EconomyData.LoginReward>();
            foreach (JSONNode n in calendar["loginRewards"].Childs)
                logins.Add(new EconomyData.LoginReward { id = Str(n["id"]), coin = n["coin"].AsInt, locKey = Str(n["locKey"]) });
            data.loginRewards = logins.ToArray();
            var days = new List<EconomyData.Day>();
            foreach (JSONNode n in calendar["days"].Childs)
                days.Add(new EconomyData.Day { day = n["day"].AsInt, month = n["month"].AsInt, week = n["week"].AsInt, dow = n["dow"].AsInt, loginRef = Str(n["loginRef"]), dailyQuestRefs = Strs(n["dailyQuestRefs"]), eventRef = Str(n["eventRef"]), bridge = n["bridge"].AsBool });
            data.days = days.ToArray();
            data.weeks = Periods(calendar["weeks"], "week", "allClearCoin");
            data.months = Periods(calendar["months"], "month", "allClearCoin");
            data.events = Periods(calendar["events"], "id", "completionCoin");

            if (isNew)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(root, AssetPath)));
                AssetDatabase.CreateAsset(data, AssetPath);
            }
            else EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();

            int registered = Register();
            int locChanged = MergeLocalization(loc);
            int errors = Verify(data, config);
            Debug.Log("[EconomyImporter] days=" + data.days.Length + " quests=" + data.quests.Length + " packs=" + data.packs.Length + " anchor=" + data.anchorUtcDate + " newConfigEntries=" + registered + " locEntries=" + locChanged + " errors=" + errors);
            if (errors > 0) EditorApplication.Exit(1);
        }

        private EconomyData.Milestone[] Milestones(JSONNode list, string daysField)
        {
            var result = new List<EconomyData.Milestone>();
            foreach (JSONNode n in list.Childs)
                result.Add(new EconomyData.Milestone { id = Str(n["id"]), days = n[daysField].AsInt, coin = n["coin"].AsInt, locKey = Str(n["locKey"]) });
            return result.ToArray();
        }

        private EconomyData.Period[] Periods(JSONNode list, string idField, string coinField)
        {
            var result = new List<EconomyData.Period>();
            foreach (JSONNode n in list.Childs)
                result.Add(new EconomyData.Period { id = Str(n[idField]), startDay = n["startDay"].AsInt, endDay = n["endDay"].AsInt, questRefs = Strs(n["questRefs"]), allClearCoin = n[coinField].AsInt, locKey = Str(n["locKey"]) });
            return result.ToArray();
        }

        // Như LocaleLevelImporter: address = đường dẫn đầy đủ, ClrBootstrap nạp trước khi vào game (key "Economy/EconomyData").
        private int Register()
        {
            var config = AssetDatabase.LoadAssetAtPath<AddressableConfig>(ConfigPath);
            foreach (var item in config.scriptableObjects)
                if (item != null && item.path == AssetPath) return 0;
            config.scriptableObjects.Add(new AddressableConfig.ScriptableObjectItem
            {
                path = AssetPath,
                asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetPath),
                address = AssetPath,
                label = ""
            });
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            return 1;
        }

        private int MergeLocalization(JSONNode loc)
        {
            var table = AssetDatabase.LoadAssetAtPath<LocalizationData>(LocalizationPath);
            var extra = JSON.Parse(File.ReadAllText(Path.Combine(root, LocExtraPath), Encoding.UTF8));
            var values = new Dictionary<string, string[]>();
            foreach (KeyValuePair<string, JSONNode> pair in (JSONClass)loc["keys"]) Fill(values, pair.Key, pair.Value, table.languages);
            foreach (KeyValuePair<string, JSONNode> pair in (JSONClass)extra) Fill(values, pair.Key, pair.Value, table.languages);

            var entries = new List<LocalizationData.Entry>(table.entries);
            int changed = 0;
            foreach (var pair in values)
            {
                var entry = entries.Find(e => e.key == pair.Key);
                if (entry == null)
                {
                    entry = new LocalizationData.Entry { key = pair.Key, values = new string[table.languages.Length] };
                    entries.Add(entry);
                }
                for (int i = 0; i < table.languages.Length; i++)
                {
                    if (string.IsNullOrEmpty(pair.Value[i])) continue;
                    if (entry.values[i] == pair.Value[i]) continue;
                    entry.values[i] = pair.Value[i];
                    changed++;
                }
            }
            foreach (var entry in entries)
                for (int i = 0; i < entry.values.Length; i++)
                    if (string.IsNullOrEmpty(entry.values[i]) && values.ContainsKey(entry.key)) Debug.LogError("[EconomyImporter] missing " + table.languages[i] + " for " + entry.key);
            table.entries = entries.ToArray();
            EditorUtility.SetDirty(table);
            AssetDatabase.SaveAssets();
            return changed;
        }

        private void Fill(Dictionary<string, string[]> values, string key, JSONNode node, string[] languages)
        {
            string[] row;
            if (!values.TryGetValue(key, out row)) values[key] = row = new string[languages.Length];
            for (int i = 0; i < languages.Length; i++)
            {
                string value = Str(node[languages[i]]);
                if (value.Length > 0) row[i] = value;
            }
        }

        // So tổng/chu kỳ với economy-config.json#totals (như validate.py) và kiểm tham chiếu.
        private int Verify(EconomyData data, JSONNode config)
        {
            int errors = 0;
            var quest = new Dictionary<string, EconomyData.Quest>();
            foreach (var q in data.quests) quest[q.id] = q;
            var login = new Dictionary<string, int>();
            foreach (var l in data.loginRewards) login[l.id] = l.coin;
            int loginSum = 0, dailySum = 0, weeklySum = 0, monthlySum = 0, eventSum = 0, streakSum = 0, levelSum = 0;
            foreach (var day in data.days)
            {
                loginSum += login[day.loginRef];
                foreach (var id in day.dailyQuestRefs) dailySum += quest[id].coin;
                dailySum += data.dailyAllClearCoin;
            }
            foreach (var w in data.weeks) { foreach (var id in w.questRefs) weeklySum += quest[id].coin; weeklySum += w.allClearCoin; }
            foreach (var m in data.months) { foreach (var id in m.questRefs) monthlySum += quest[id].coin; monthlySum += m.allClearCoin; }
            foreach (var e in data.events) { foreach (var id in e.questRefs) eventSum += quest[id].coin; eventSum += e.allClearCoin; }
            foreach (var s in data.streakMilestones) streakSum += s.coin;
            foreach (var s in data.levelMilestones) levelSum += s.coin;
            var totals = config["totals"];
            errors += Expect("loginPerCycle", loginSum, totals["loginPerCycle"].AsInt);
            errors += Expect("dailyQuestPerCycle", dailySum, totals["dailyQuestPerCycle"].AsInt);
            errors += Expect("weeklyQuestPerCycle", weeklySum, totals["weeklyQuestPerCycle"].AsInt);
            errors += Expect("monthlyQuestPerCycle", monthlySum, totals["monthlyQuestPerCycle"].AsInt);
            errors += Expect("eventPerCycle", eventSum, totals["eventPerCycle"].AsInt);
            errors += Expect("streakMilestonesPerCycle", streakSum, totals["streakMilestonesPerCycle"].AsInt);
            errors += Expect("levelMilestonesLifetime", levelSum, totals["levelMilestonesLifetime"].AsInt);
            errors += Expect("days", data.days.Length, 365) + Expect("weeks", data.weeks.Length, 52) + Expect("months", data.months.Length, 12) + Expect("events", data.events.Length, 12);
            return errors;
        }

        private int Expect(string name, int actual, int expected)
        {
            if (actual == expected)
            {
                Debug.Log("[EconomyImporter] total " + name + "=" + actual + " OK");
                return 0;
            }
            Debug.LogError("[EconomyImporter] total " + name + "=" + actual + " expected " + expected);
            return 1;
        }
    }
}
