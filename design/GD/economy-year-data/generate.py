#!/usr/bin/env python3
"""Sinh dữ liệu kinh tế coin/chu kỳ 365 ngày. Số liệu là ĐỀ XUẤT tuning (proposal), chưa user duyệt, giá IAP chưa live.
Chạy: python3 generate.py  -> ghi 4 file JSON + bundle.js cạnh script."""
import json
from pathlib import Path

OUT = Path(__file__).resolve().parent
MONTH_DAYS = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31]
SCHEMA_VERSION = "1.0.0"


class Builder:
    def __init__(self):
        self.month_start = []
        d = 1
        for n in MONTH_DAYS:
            self.month_start.append(d)
            d += n

    def month_of(self, day):
        m = 1
        for i, s in enumerate(self.month_start):
            if day >= s:
                m = i + 1
        return m

    def quests(self):
        def q(id_, scope, metric, target, coin, loc):
            return {"id": id_, "scope": scope, "metric": metric, "target": target, "coin": coin, "locKey": loc, "requiresPurchase": False, "requiresAd": False}
        daily = [
            q("dq_win_3", "daily", "levels_won", 3, 15, "quest.dq_win_3"),
            q("dq_words_20", "daily", "words_found", 20, 15, "quest.dq_words_20"),
            q("dq_daily", "daily", "daily_challenge_done", 1, 10, "quest.dq_daily"),
            q("dq_bot_1", "daily", "bot_match_played", 1, 10, "quest.dq_bot_1"),
            q("dq_extra_1", "daily", "extra_word_found", 1, 10, "quest.dq_extra_1"),
        ]
        weekly = [
            q("wq_win_20", "weekly", "levels_won", 20, 40, "quest.wq_win_20"),
            q("wq_words_150", "weekly", "words_found", 150, 40, "quest.wq_words_150"),
            q("wq_daily_5", "weekly", "daily_challenge_done", 5, 40, "quest.wq_daily_5"),
            q("wq_bot_win_3", "weekly", "bot_match_won", 3, 30, "quest.wq_bot_win_3"),
            q("wq_extra_5", "weekly", "extra_word_found", 5, 30, "quest.wq_extra_5"),
            q("wq_login_5", "weekly", "login_days", 5, 30, "quest.wq_login_5"),
        ]
        monthly = [
            q("mq_win_100", "monthly", "levels_won", 100, 150, "quest.mq_win_100"),
            q("mq_daily_20", "monthly", "daily_challenge_done", 20, 150, "quest.mq_daily_20"),
            q("mq_login_20", "monthly", "login_days", 20, 100, "quest.mq_login_20"),
            q("mq_words_600", "monthly", "words_found", 600, 100, "quest.mq_words_600"),
        ]
        return daily, weekly, monthly

    def events(self):
        spec = [
            ("ev_newyear", 1, 7, 8), ("ev_spring", 32, 40, 10), ("ev_hearts", 45, 46, 4),
            ("ev_wordweek", 91, 97, 8), ("ev_earth", 112, 113, 4), ("ev_summer", 152, 166, 12),
            ("ev_midyear", 182, 184, 4), ("ev_school", 244, 250, 8), ("ev_harvest", 274, 280, 8),
            ("ev_spooky", 300, 304, 6), ("ev_thanks", 325, 331, 8), ("ev_winter", 355, 365, 12),
        ]
        events, quests = [], []
        for id_, s, e, win in spec:
            qs = [("win", "levels_won", win, 20), ("words", "words_found", win * 6, 20), ("daily", "daily_challenge_done", 1 if e - s < 6 else 3, 30)]
            refs = []
            for k, metric, target, coin in qs:
                qid = "eq_%s_%s" % (id_[3:], k)
                refs.append(qid)
                quests.append({"id": qid, "scope": "event", "metric": metric, "target": target, "coin": coin, "locKey": "quest.eq_" + k, "eventId": id_, "requiresPurchase": False, "requiresAd": False})
            events.append({"id": id_, "startDay": s, "endDay": e, "locKey": "event." + id_[3:], "questRefs": refs, "completionCoin": 50, "optional": True})
        return events, quests

    def build(self):
        daily, weekly, monthly = self.quests()
        events, eq = self.events()
        logins = [
            {"id": "lr_base", "coin": 10, "locKey": "login.base"},
            {"id": "lr_week", "coin": 25, "locKey": "login.week"},
            {"id": "lr_month", "coin": 60, "locKey": "login.month"},
            {"id": "lr_cycle", "coin": 100, "locKey": "login.cycle"},
        ]
        month_last = [self.month_start[i] + MONTH_DAYS[i] - 1 for i in range(12)]
        days = []
        for d in range(1, 366):
            week = None if d == 365 else (d - 1) // 7 + 1
            if d == 365:
                login = "lr_cycle"
            elif d in month_last:
                login = "lr_month"
            elif d % 7 == 0:
                login = "lr_week"
            else:
                login = "lr_base"
            dq = [daily[(d - 1 + i) % 5]["id"] for i in range(3)]
            ev = next((e["id"] for e in events if e["startDay"] <= d <= e["endDay"]), None)
            days.append({"day": d, "month": self.month_of(d), "week": week, "dow": (d - 1) % 7 + 1, "loginRef": login, "dailyQuestRefs": dq, "eventRef": ev, "bridge": d == 365})
        weeks = []
        for w in range(1, 53):
            weeks.append({"week": w, "startDay": (w - 1) * 7 + 1, "endDay": w * 7, "questRefs": [weekly[(w - 1 + i) % 6]["id"] for i in range(3)], "allClearCoin": 30})
        months = []
        for m in range(1, 13):
            months.append({"month": m, "startDay": self.month_start[m - 1], "endDay": month_last[m - 1], "lengthDays": MONTH_DAYS[m - 1],
                           "questRefs": [monthly[(m - 1 + i) % 4]["id"] for i in range(3)], "allClearCoin": 100, "locKey": "month.%02d" % m})
        calendar = {
            "schemaVersion": SCHEMA_VERSION, "kind": "calendar",
            "cycle": {"lengthDays": 365, "anchorUtcDate": "2026-01-01", "idFormat": "C{n}", "dayIndexBase": 1, "weeksInCycle": 52, "bridgeDay": 365, "designMonthDays": MONTH_DAYS},
            "loginRewards": logins, "days": days, "weeks": weeks, "months": months, "events": events,
        }
        quests = {"schemaVersion": SCHEMA_VERSION, "kind": "quests", "templates": daily + weekly + monthly + eq,
                  "dailyAllClearCoin": 10, "dailyPickRule": "dailyQuestRefs = 3 mẫu liên tiếp trong vòng 5 mẫu daily, bắt đầu tại (day-1)%5"}
        return calendar, quests

    def config(self, calendar, quests):
        t = {q["id"]: q for q in quests["templates"]}
        lr = {x["id"]: x["coin"] for x in calendar["loginRewards"]}
        login_total = sum(lr[d["loginRef"]] for d in calendar["days"])
        dq_total = sum(sum(t[r]["coin"] for r in d["dailyQuestRefs"]) + quests["dailyAllClearCoin"] for d in calendar["days"])
        wq_total = sum(sum(t[r]["coin"] for r in w["questRefs"]) + w["allClearCoin"] for w in calendar["weeks"])
        mq_total = sum(sum(t[r]["coin"] for r in m["questRefs"]) + m["allClearCoin"] for m in calendar["months"])
        ev_total = sum(sum(t[r]["coin"] for r in e["questRefs"]) + e["completionCoin"] for e in calendar["events"])
        streak = [{"id": "st_%d" % n, "streakDays": n, "coin": c, "locKey": "streak.%d" % n} for n, c in [(3, 20), (7, 40), (14, 70), (30, 150), (60, 250), (100, 400), (180, 700), (365, 1500)]]
        level = [{"id": "lm_%d" % n, "levelsCleared": n, "coin": c, "locKey": "milestone.lm_%d" % n} for n, c in [(10, 20), (25, 30), (50, 50), (100, 100), (200, 150), (300, 200), (400, 250), (534, 400)]]
        packs = [
            {"id": "pk_1", "productId": "word.item1", "coin": 500, "priceTier": 1, "proposedUsd": 0.99, "removesAds": False},
            {"id": "pk_2", "productId": "word.item2", "coin": 1100, "priceTier": 2, "proposedUsd": 1.99, "removesAds": False},
            {"id": "pk_3", "productId": "word.item3", "coin": 2500, "priceTier": 5, "proposedUsd": 4.99, "removesAds": False},
            {"id": "pk_4", "productId": "word.item4", "coin": 4500, "priceTier": 9, "proposedUsd": 8.99, "removesAds": True},
            {"id": "pk_5", "productId": "word.item5", "coin": 11000, "priceTier": 20, "proposedUsd": 19.99, "removesAds": True},
            {"id": "pk_noads", "productId": "word.noads", "coin": 0, "priceTier": 5, "proposedUsd": 4.99, "removesAds": True},
        ]
        for p in packs:
            p["locKey"] = "shop." + p["id"]
        rewarded = [
            {"id": "ad_shop_free", "type": "rewarded", "placement": "shop", "coin": 20, "cooldownSec": 120, "dailyCap": 5, "sharedCapGroup": "rw_coin", "optional": True},
            {"id": "ad_low_coin", "type": "rewarded", "placement": "low_coin_popup", "coin": 20, "cooldownSec": 120, "dailyCap": 5, "sharedCapGroup": "rw_coin", "optional": True},
            {"id": "ad_win_x2", "type": "rewarded", "placement": "win_dialog", "coin": 10, "cooldownSec": 120, "dailyCap": 5, "sharedCapGroup": "rw_x2", "optional": True, "note": "cộng thêm bằng phần thưởng thắng lần đầu (x2)"},
            {"id": "ad_daily_x2", "type": "rewarded", "placement": "daily_challenge", "coin": 30, "cooldownSec": 120, "dailyCap": 1, "sharedCapGroup": "rw_x2", "optional": True, "note": "cộng thêm bằng thưởng Daily (x2)"},
        ]
        inter = {"id": "ad_inter_win", "type": "interstitial", "placement": "after_win", "graceLevels": 5, "minGapSec": 90, "dailyCap": 6, "blockedAfterRewardedSec": 30,
                 "disabledBy": ["word.noads", "word.item4", "word.item5"], "forbiddenPlacements": ["pause", "shop", "how_to_play", "extra_word", "mid_swipe", "over_rewarded"]}
                # trung bình ngày từ dữ liệu: login và daily quest hằng ngày (cả chu kỳ)
        avg_login = round(login_total / 365, 1)
        avg_dq = round(dq_total / 365, 1)
        cfg = {
            "schemaVersion": SCHEMA_VERSION, "kind": "economy",
            "status": "PROPOSAL_TUNING_NOT_APPROVED",
            "currency": {"id": "coin", "displayLocKey": "currency.coin", "startBalance": 100, "secondCurrency": None,
                         "designNote": "Coin là thiết kế mới theo yêu cầu user; GD/runtime hiện tại vẫn là ruby (CurrencyController, 10 ruby khởi đầu). Không migration/runtime ở task này."},
            "earn": {"firstWinCoin": 10, "firstEverWinBonusCoin": 20, "replayWinCoin": 0, "dailyChallengeCoin": 30, "botWinCoinByDifficulty": {"easy": 5, "medium": 10, "hard": 15}, "botDrawCoin": 5, "botDailyCap": 60, "extraWordCoin": 5, "extraWordCapPerLevel": 1},
            "sinks": [
                {"id": "sk_hint_cell", "coin": 20, "locKey": "sink.hint_cell", "optional": True},
                {"id": "sk_hint_word", "coin": 60, "locKey": "sink.hint_word", "optional": True},
                {"id": "sk_streak_repair", "coin": 50, "locKey": "sink.streak_repair", "optional": True, "maxMissedDays": 1},
            ],
            "freeActions": ["play", "win", "shuffle", "extra_word", "daily_challenge", "bot_match", "claim_quest"],
            "shop": {"packs": packs, "noAdsProductId": "word.noads", "paymentSimulatedInDesign": True, "priceNote": "proposedUsd/priceTier là đề xuất, chưa phải giá live Google Play/App Store"},
            "ads": {"rewarded": rewarded, "interstitial": inter,
                    "states": [
                        {"id": "loading", "coinDelta": 0, "locKey": "ad.loading"}, {"id": "ready", "coinDelta": 0, "locKey": "ad.ready"},
                        {"id": "cooldown", "coinDelta": 0, "locKey": "ad.cooldown"}, {"id": "cap_reached", "coinDelta": 0, "locKey": "ad.cap"},
                        {"id": "no_fill", "coinDelta": 0, "locKey": "ad.nofill"}, {"id": "offline", "coinDelta": 0, "locKey": "ad.offline"},
                        {"id": "cancelled", "coinDelta": 0, "locKey": "ad.cancel"}, {"id": "error", "coinDelta": 0, "locKey": "ad.error"},
                        {"id": "success", "coinDelta": "placement.coin", "locKey": "ad.success"}],
                    "rules": ["Thưởng chỉ cộng ở callback hoàn thành, idempotent theo sessionId.", "no_fill/offline/error/cancelled/cap/cooldown: không thưởng, không trừ coin, không chặn chơi.", "Nhiệm vụ không bao giờ requiresAd/requiresPurchase.", "Mua bất kỳ gói removesAds tắt interstitial/banner; rewarded vẫn tuỳ chọn."],
                    "maxRewardedCoinPerDay": 5 * 20 + 5 * 10 + 30},
            "login": {"source": "calendar.loginRewards", "claimOncePerUtcDay": True},
            "streak": {"milestones": streak, "countingRule": "ngày UTC liên tiếp có claim login", "keepBestOnBreak": True, "milestoneClaimScope": "per_cycle"},
            "levelMilestones": level,
            "totals": {"loginPerCycle": login_total, "dailyQuestPerCycle": dq_total, "weeklyQuestPerCycle": wq_total, "monthlyQuestPerCycle": mq_total, "eventPerCycle": ev_total,
                       "streakMilestonesPerCycle": sum(s["coin"] for s in streak), "levelMilestonesLifetime": sum(l["coin"] for l in level)},
            "baseline": {"avgLoginPerDay": avg_login, "avgDailyQuestPerDayAllClear": avg_dq,
                         "casualDay": {"wins": 3, "winCoin": 30, "login": 14, "quest": 25, "earn": 69, "hintsUsed": 3, "spend": 60, "net": 9},
                         "engagedDay": {"wins": 5, "winCoin": 50, "dailyChallenge": 30, "login": 14, "quest": 46, "earn": 140, "hintsUsed": 4, "spend": 80, "net": 60},
                         "adsExcluded": True,
                         "note": "Free player (không IAP, không ads) hoà vốn ở mức casual và dư ~60/ngày khi chơi nhiều; ads chỉ cộng thêm, tối đa maxRewardedCoinPerDay."},
            "rules": {
                "timeBasis": "UTC", "resetUtcHour": 0, "dayNumber": "floor((utcMidnight(now)-anchorUtcDate)/86400000)",
                "dayIndex": "dayNumber % 365 + 1", "cycleId": "'C' + (floor(dayNumber/365)+1)",
                "leapDay": "Chu kỳ đếm theo số ngày từ anchor, không theo lịch; 29/02 chỉ là một ngày thường, chu kỳ trong năm nhuận kết thúc lệch 1 ngày lịch. Tháng thiết kế là nhãn 28-31 ngày của chu kỳ, không phải tháng lịch.",
                "bridgeDay": "Ngày 365 thuộc tháng 12, không thuộc tuần nào (52 tuần = 364 ngày); thưởng login lr_cycle. Ngày kế tiếp là ngày 1 chu kỳ C{n+1}.",
                "claimKey": "claim:{cycleId}:{scope}:{periodId}:{refId}", "claimIdempotent": True,
                "clockGuard": "Lưu maxSeenUtcDay; không lùi. Nhảy tiến nhiều ngày: không bù phần thưởng quá khứ, kỳ nhiệm vụ cũ hết hạn, streak chỉ sửa được khi lỡ đúng 1 ngày bằng sk_streak_repair.",
                "offline": "Tiến độ và claim ghi ledger cục bộ; ads/IAP cần mạng; đồng bộ server (nếu có sau này) dựa trên claimKey, bản ghi trùng key bị bỏ.",
                "expiry": "Nhiệm vụ daily hết hạn 00:00 UTC kế tiếp; weekly hết hạn khi sang tuần; monthly khi sang tháng thiết kế; event hết hạn sau endDay. Chưa claim thì mất.",
                "cycleRollover": "Sang C{n+1}: reset tiến độ nhiệm vụ/sự kiện/milestone per_cycle; giữ coin, streak, best streak, levelMilestones, mua hàng.",
            },
        }
        return cfg

    def loc(self, cfg, calendar, quests):
        L = {}
        def add(k, vi, en):
            L[k] = {"vi": vi, "en": en, "fr": None, "es": None, "pt": None}
        add("currency.coin", "Coin", "Coins")
        names = {"dq_win_3": ("Thắng 3 level", "Win 3 levels"), "dq_words_20": ("Tìm 20 từ", "Find 20 words"), "dq_daily": ("Hoàn thành Daily", "Finish the Daily"),
                 "dq_bot_1": ("Chơi 1 trận Bot", "Play 1 Bot match"), "dq_extra_1": ("Tìm 1 từ thưởng", "Find 1 bonus word"),
                 "wq_win_20": ("Thắng 20 level", "Win 20 levels"), "wq_words_150": ("Tìm 150 từ", "Find 150 words"), "wq_daily_5": ("Hoàn thành Daily 5 lần", "Finish the Daily 5 times"),
                 "wq_bot_win_3": ("Thắng Bot 3 lần", "Beat the Bot 3 times"), "wq_extra_5": ("Tìm 5 từ thưởng", "Find 5 bonus words"), "wq_login_5": ("Vào game 5 ngày", "Log in on 5 days"),
                 "mq_win_100": ("Thắng 100 level", "Win 100 levels"), "mq_daily_20": ("Hoàn thành Daily 20 lần", "Finish the Daily 20 times"), "mq_login_20": ("Vào game 20 ngày", "Log in on 20 days"), "mq_words_600": ("Tìm 600 từ", "Find 600 words"),
                 "eq_win": ("Thắng level trong sự kiện", "Win levels during the event"), "eq_words": ("Tìm từ trong sự kiện", "Find words during the event"), "eq_daily": ("Daily trong sự kiện", "Daily during the event")}
        for k, (vi, en) in names.items():
            add("quest." + k, vi, en)
        evn = {"newyear": ("Năm mới", "New Year"), "spring": ("Lễ hội mùa xuân", "Spring Festival"), "hearts": ("Ngày trái tim", "Hearts Day"), "wordweek": ("Tuần lễ chữ", "Word Week"), "earth": ("Ngày Trái Đất", "Earth Day"), "summer": ("Hè rực rỡ", "Summer Splash"),
               "midyear": ("Giữa năm", "Midyear"), "school": ("Tựu trường", "Back to School"), "harvest": ("Mùa gặt", "Harvest"), "spooky": ("Đêm hoá trang", "Spooky Night"), "thanks": ("Tri ân", "Thanks Week"), "winter": ("Mùa đông diệu kỳ", "Winter Wonder")}
        for k, (vi, en) in evn.items():
            add("event." + k, vi, en)
        mon = ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"]
        for i, vi in enumerate(mon):
            add("month.%02d" % (i + 1), vi, "Month %d" % (i + 1))
        add("login.base", "Thưởng đăng nhập", "Login reward"); add("login.week", "Thưởng cuối tuần ngày 7", "Day-7 login reward")
        add("login.month", "Thưởng cuối tháng", "Month-end login reward"); add("login.cycle", "Thưởng ngày cuối chu kỳ", "Cycle-end login reward")
        for s in cfg["streak"]["milestones"]:
            add(s["locKey"], "Chuỗi %d ngày" % s["streakDays"], "%d-day streak" % s["streakDays"])
        for m in cfg["levelMilestones"]:
            add(m["locKey"], "Hoàn thành %d level" % m["levelsCleared"], "Clear %d levels" % m["levelsCleared"])
        sinks = {"sink.hint_cell": ("Gợi ý 1 ô", "Hint: 1 tile"), "sink.hint_word": ("Gợi ý cả từ", "Hint: whole word"), "sink.streak_repair": ("Cứu chuỗi", "Streak repair")}
        for k, (vi, en) in sinks.items():
            add(k, vi, en)
        for p in cfg["shop"]["packs"]:
            add(p["locKey"], "Gói %s coin" % p["coin"] if p["coin"] else "Tắt quảng cáo", "%s coins pack" % p["coin"] if p["coin"] else "Remove ads")
        ad = {"ad.loading": ("Đang tải quảng cáo…", "Loading ad…"), "ad.ready": ("Sẵn sàng", "Ready"), "ad.cooldown": ("Chờ hồi chiêu", "On cooldown"),
              "ad.cap": ("Hết lượt hôm nay", "Daily limit reached"), "ad.nofill": ("Chưa có quảng cáo, thử lại sau", "No ad available, try later"),
              "ad.offline": ("Không có mạng", "You are offline"), "ad.cancel": ("Đóng sớm, không có thưởng", "Closed early, no reward"),
              "ad.error": ("Lỗi quảng cáo, không mất coin", "Ad error, no coins lost"), "ad.success": ("Nhận thưởng", "Reward granted")}
        for k, (vi, en) in ad.items():
            add(k, vi, en)
        return {"schemaVersion": SCHEMA_VERSION, "kind": "loc", "languages": ["vi", "en", "fr", "es", "pt"], "pendingLanguages": ["fr", "es", "pt"], "keys": L}


def main():
    b = Builder()
    cal, qs = b.build()
    cfg = b.config(cal, qs)
    loc = b.loc(cfg, cal, qs)
    files = {"economy-config.json": cfg, "quest-templates.json": qs, "calendar-365.json": cal, "loc-keys.json": loc}
    for name, obj in files.items():
        (OUT / name).write_text(json.dumps(obj, ensure_ascii=False, indent=1) + "\n", encoding="utf-8")
    bundle = {"schemaVersion": SCHEMA_VERSION, "config": cfg, "quests": qs, "calendar": cal, "loc": loc}
    (OUT / "bundle.js").write_text("/* Sinh bởi generate.py từ 4 file JSON; không sửa tay. */\nwindow.WC_ECON = " + json.dumps(bundle, ensure_ascii=False, separators=(",", ":")) + ";\n", encoding="utf-8")
    print("totals", json.dumps(cfg["totals"]), "avg login/day", cfg["baseline"]["avgLoginPerDay"], "avg dq/day", cfg["baseline"]["avgDailyQuestPerDayAllClear"])


main()
