public class GameState
{
    public static int currentWorld, currentSubWorld, currentLevel;
    public static string currentSubWorldName = "Subworld name";
    public static int unlockedWorld = -1, unlockedSubWord = -1, unlockedLevel = -1;

    // Ngữ cảnh Daily (chỉ trong bộ nhớ): dailyDate != null nghĩa là Main đang chơi bàn Daily của ngày đó, không phải level tiến độ chính.
    public static string dailyDate;
    public static bool openDailyOnHome;
    public static bool isDaily { get { return dailyDate != null; } }
}