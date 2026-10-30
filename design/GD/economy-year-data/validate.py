#!/usr/bin/env python3
"""Validator tối thiểu (không phụ thuộc thư viện). Chạy: python3 validate.py  (exit 1 nếu lỗi)."""
import json
import re
import sys
from pathlib import Path

HERE = Path(__file__).resolve().parent
GD = HERE.parent / "economy-year.html"
UI = HERE.parent.parent / "Ui" / "screens" / "economy-year"


class Validator:
    def __init__(self):
        self.errors = []
        self.schema = self.load("economy-year.schema.json")
        self.cfg = self.load("economy-config.json")
        self.qs = self.load("quest-templates.json")
        self.cal = self.load("calendar-365.json")
        self.loc = self.load("loc-keys.json")

    def load(self, name):
        return json.loads((HERE / name).read_text(encoding="utf-8"))

    def err(self, msg):
        self.errors.append(msg)

    def check(self, cond, msg):
        if not cond:
            self.err(msg)

    def resolve(self, ref):
        node = self.schema
        for part in ref[2:].split("/"):
            node = node[part]
        return node

    def typeok(self, v, t):
        m = {"string": str, "object": dict, "array": list, "boolean": bool}
        if t == "integer":
            return isinstance(v, int) and not isinstance(v, bool)
        return isinstance(v, m[t])

    def vschema(self, v, sc, path):
        if "$ref" in sc:
            sc = self.resolve(sc["$ref"])
        if "enum" in sc and v not in sc["enum"]:
            self.err("%s: ngoài enum %r" % (path, v))
        if "type" in sc and not self.typeok(v, sc["type"]):
            self.err("%s: sai type %s" % (path, sc["type"]))
            return
        if "minimum" in sc and isinstance(v, int) and v < sc["minimum"]:
            self.err("%s: < %s" % (path, sc["minimum"]))
        if "pattern" in sc and isinstance(v, str) and not re.search(sc["pattern"], v):
            self.err("%s: sai pattern %s" % (path, v))
        if isinstance(v, dict):
            for r in sc.get("required", []):
                self.check(r in v, "%s: thiếu %s" % (path, r))
            for k, s in sc.get("properties", {}).items():
                if k in v:
                    self.vschema(v[k], s, path + "." + k)
        if isinstance(v, list) and "items" in sc:
            for i, x in enumerate(v):
                self.vschema(x, sc["items"], "%s[%d]" % (path, i))

    def run_schema(self):
        for name, ref in self.schema["files"].items():
            self.vschema(self.load(name), self.resolve(ref), name)

    def run_refs(self):
        tq = {q["id"]: q for q in self.qs["templates"]}
        self.check(len(tq) == len(self.qs["templates"]), "quest id trùng")
        lr = {x["id"]: x["coin"] for x in self.cal["loginRewards"]}
        evs = {e["id"]: e for e in self.cal["events"]}
        days = self.cal["days"]
        self.check([d["day"] for d in days] == list(range(1, 366)), "days phải đủ 1..365 liên tiếp")
        self.check(self.cal["cycle"]["lengthDays"] == 365 and self.cal["cycle"]["bridgeDay"] == 365, "cycle length/bridge")
        self.check(self.cal["cycle"]["designMonthDays"] == [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31] and sum(self.cal["cycle"]["designMonthDays"]) == 365, "designMonthDays")
        for d in days:
            n = d["day"]
            self.check(d["loginRef"] in lr, "day %d loginRef" % n)
            self.check(len(d["dailyQuestRefs"]) == 3 and all(tq.get(r, {}).get("scope") == "daily" for r in d["dailyQuestRefs"]), "day %d dailyQuestRefs" % n)
            self.check(d["eventRef"] is None or d["eventRef"] in evs, "day %d eventRef" % n)
            if d["eventRef"]:
                e = evs[d["eventRef"]]
                self.check(e["startDay"] <= n <= e["endDay"], "day %d ngoài event" % n)
            self.check((d["week"] is None) == (n == 365) and d["bridge"] == (n == 365), "day %d week/bridge" % n)
            if n < 365:
                self.check(d["week"] == (n - 1) // 7 + 1, "day %d week" % n)
        w = self.cal["weeks"]
        self.check([x["week"] for x in w] == list(range(1, 53)), "đủ 52 tuần")
        for x in w:
            self.check(x["endDay"] - x["startDay"] == 6 and all(tq.get(r, {}).get("scope") == "weekly" for r in x["questRefs"]), "tuần %d" % x["week"])
        m = self.cal["months"]
        self.check(len(m) == 12 and m[0]["startDay"] == 1 and m[-1]["endDay"] == 365, "12 tháng phủ 1..365")
        for i, x in enumerate(m):
            self.check(x["endDay"] - x["startDay"] + 1 == x["lengthDays"], "tháng %d length" % x["month"])
            self.check(i == 0 or x["startDay"] == m[i - 1]["endDay"] + 1, "tháng %d liền kề" % x["month"])
            self.check(all(tq.get(r, {}).get("scope") == "monthly" for r in x["questRefs"]), "tháng %d quest" % x["month"])
            for n in range(x["startDay"], x["endDay"] + 1):
                self.check(days[n - 1]["month"] == x["month"], "day %d month" % n)
        for e in self.cal["events"]:
            self.check(1 <= e["startDay"] <= e["endDay"] <= 365 and all(tq.get(r, {}).get("eventId") == e["id"] for r in e["questRefs"]), "event %s" % e["id"])
        for q in self.qs["templates"]:
            self.check(not q["requiresAd"] and not q["requiresPurchase"], "quest %s ép ads/mua" % q["id"])
        keys = self.loc["keys"]
        used = [q["locKey"] for q in self.qs["templates"]] + [e["locKey"] for e in self.cal["events"]] + [x["locKey"] for x in m] + [x["locKey"] for x in self.cal["loginRewards"]]
        used += [s["locKey"] for s in self.cfg["streak"]["milestones"]] + [s["locKey"] for s in self.cfg["levelMilestones"]] + [s["locKey"] for s in self.cfg["sinks"]]
        used += [p["locKey"] for p in self.cfg["shop"]["packs"]] + [s["locKey"] for s in self.cfg["ads"]["states"]] + [self.cfg["currency"]["displayLocKey"]]
        for k in used:
            self.check(k in keys and keys[k]["vi"] and keys[k]["en"], "thiếu loc " + k)
        for k, v in keys.items():
            for lang in self.loc["languages"]:
                self.check(bool(v.get(lang)), "loc %s thiếu %s" % (k, lang))

    def run_totals(self):
        t = {q["id"]: q["coin"] for q in self.qs["templates"]}
        lr = {x["id"]: x["coin"] for x in self.cal["loginRewards"]}
        days = self.cal["days"]
        tot = self.cfg["totals"]
        login = sum(lr[d["loginRef"]] for d in days)
        dq = sum(sum(t[r] for r in d["dailyQuestRefs"]) + self.qs["dailyAllClearCoin"] for d in days)
        wq = sum(sum(t[r] for r in x["questRefs"]) + x["allClearCoin"] for x in self.cal["weeks"])
        mq = sum(sum(t[r] for r in x["questRefs"]) + x["allClearCoin"] for x in self.cal["months"])
        ev = sum(sum(t[r] for r in e["questRefs"]) + e["completionCoin"] for e in self.cal["events"])
        for name, got in [("loginPerCycle", login), ("dailyQuestPerCycle", dq), ("weeklyQuestPerCycle", wq), ("monthlyQuestPerCycle", mq), ("eventPerCycle", ev)]:
            self.check(tot[name] == got, "totals.%s %s != %s" % (name, tot[name], got))
        self.check(tot["streakMilestonesPerCycle"] == sum(s["coin"] for s in self.cfg["streak"]["milestones"]), "totals streak")
        self.check(tot["levelMilestonesLifetime"] == sum(s["coin"] for s in self.cfg["levelMilestones"]), "totals level")
        b = self.cfg["baseline"]
        self.check(round(login / 365, 1) == b["avgLoginPerDay"] and b["engagedDay"]["login"] == round(login / 365) and round(dq / 365, 1) == b["avgDailyQuestPerDayAllClear"], "baseline vs data")
        for k in ["casualDay", "engagedDay"]:
            x = b[k]
            earn = x["winCoin"] + x.get("dailyChallenge", 0) + x["login"] + x["quest"]
            spend = x["hintsUsed"] * next(s["coin"] for s in self.cfg["sinks"] if s["id"] == "sk_hint_cell")
            self.check(earn == x["earn"] and spend == x["spend"] and earn - spend == x["net"], "baseline %s số học" % k)
            self.check(x["winCoin"] == x["wins"] * self.cfg["earn"]["firstWinCoin"], "baseline %s winCoin" % k)
        self.check(b["casualDay"]["net"] >= 0 and b["adsExcluded"], "free player phải không âm khi bỏ ads")
        ads = self.cfg["ads"]
        self.check(ads["maxRewardedCoinPerDay"] == sum(c["coin"] * c["dailyCap"] for c in ads["rewarded"] if c["sharedCapGroup"] == "rw_coin" and c["id"] == "ad_shop_free") + 5 * 10 + 30, "maxRewardedCoinPerDay")
        self.check(self.cfg["currency"]["secondCurrency"] is None and not any(p["coin"] < 0 for p in self.cfg["shop"]["packs"]), "một currency")
        for p in self.cfg["shop"]["packs"]:
            self.check(p["productId"].startswith("word."), "productId " + p["id"])

    def run_bundle_and_views(self):
        js = (HERE / "bundle.js").read_text(encoding="utf-8")
        body = js.split("window.WC_ECON = ", 1)[1].rstrip().rstrip(";")
        b = json.loads(body)
        self.check(b["config"] == self.cfg and b["quests"] == self.qs and b["calendar"] == self.cal and b["loc"] == self.loc, "bundle.js lệch JSON (chạy generate.py)")
        gd = GD.read_text(encoding="utf-8") if GD.exists() else ""
        self.check(bool(gd), "thiếu design/GD/economy-year.html")
        for m in re.finditer(r'data-econ="([^"]+)">([^<]*)<', gd):
            node = {"config": self.cfg, "calendar": self.cal, "quests": self.qs}
            cur = node
            for part in m.group(1).split("."):
                cur = cur[int(part)] if isinstance(cur, list) else cur[part]
            self.check(str(cur) == m.group(2).strip().replace(",", ""), "GD %s: %s != %s" % (m.group(1), m.group(2), cur))
        self.check(len(re.findall(r'data-econ="', gd)) >= 20, "GD thiếu data-econ")
        for f in ["index.html", "economy.js"]:
            self.check((UI / f).exists(), "thiếu UI " + f)

    def run(self):
        self.run_schema()
        self.run_refs()
        self.run_totals()
        self.run_bundle_and_views()
        for e in self.errors:
            print("FAIL", e)
        print("OK" if not self.errors else "%d lỗi" % len(self.errors))
        return 1 if self.errors else 0


sys.exit(Validator().run())
