using Script.Utils;

namespace WordCross.Gameplay
{
    // Đặt tên khoá tiến độ theo ngôn ngữ chơi: progress.<gameLang>.<khoá>.
    // Ngôn ngữ chơi mặc định (en) giữ nguyên tên khoá cũ nên tiến độ của người đang chơi không cần migrate.
    public class ProgressKeys
    {
        public string Of(string key)
        {
            var loc = Locator<LocalizationController>.Instance;
            if (loc == null || loc.GameLanguage == loc.DefaultGameLanguage) return key;
            return "progress." + loc.GameLanguage + "." + key;
        }
    }
}
