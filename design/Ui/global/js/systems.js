/* Luật prototype: level, ruby, Daily, Bot, ads mô phỏng. Không gọi mạng/ads thật. Số liệu "ECON" là PROPOSED tuning trừ khi ghi (observed). */
(function () {
  var WC = window.WC, S = WC.state;

  WC.ECON = {
    start: 10,            /* observed: CurrencyController.DEFAULT_CURRENCY */
    hint: 2,              /* observed: Const.HINT_COST */
    extraPer: 5,          /* observed: ExtraWordDialog claim = target / 5 * 2 */
    firstClear: 2,        /* proposed: gốc không thưởng khi thắng */
    firstClearTutorial: 3,/* proposed: level 1 */
    video: 2,             /* observed: GameConfig.rewardedVideoAmount */
    videoCooldown: 120,   /* observed: GameConfig.rewardedVideoPeriod */
    interstitialGap: 90,  /* observed: GameConfig.adPeriod */
    dailyBase: 3,         /* proposed */
    backfill: 1,          /* proposed */
    milestones: {3: 2, 7: 5, 14: 8}, /* proposed */
    botWin: {easy: 1, normal: 2, hard: 3}, botDraw: 1, botCap: 6, /* proposed */
    botLimit: 90
  };

  /* ---------- level ---------- */
  var SUB_TOTAL = [[12, 18, 18, 18, 18], [18, 18, 18, 18, 18], [18, 18, 18, 18, 18], [18, 18, 18, 18, 18], [18, 18, 18, 18, 18], [18, 18, 18, 18, 18]]; /* Utils.GetNumLevels */
  WC.subTotal = function (w, s) { return SUB_TOTAL[w][s]; };
  WC.key = function (lv) { return lv.w + '-' + lv.s + '-' + lv.l; };
  WC.getLevel = function (lv) {
    var k = WC.key(lv), d = window.WC_LEVELS[k];
    if (d) return {key: k, data: d, sample: false};
    var rep = window.WC_REP[lv.w];
    return {key: k, data: window.WC_LEVELS[rep], sample: true, repKey: rep};
  };
  WC.progressOf = function (key) {
    if (!S.progress[key]) S.progress[key] = {hints: {}, found: [], extras: []};
    return S.progress[key];
  };
  WC.isCleared = function (lv) { return !!S.cleared[WC.key(lv)]; };
  /* Mở khoá tuyến tính như Prefs.unlocked*: level đầu tiên chưa xong là level hiện tại. */
  WC.current = function () {
    for (var w = 0; w < 6; w++) for (var s = 0; s < 5; s++) for (var l = 0; l < SUB_TOTAL[w][s]; l++)
      if (!S.cleared[w + '-' + s + '-' + l]) return {w: w, s: s, l: l};
    return {w: 5, s: 4, l: 17};
  };
  WC.subCleared = function (w, s) { var n = 0; for (var l = 0; l < SUB_TOTAL[w][s]; l++) if (S.cleared[w + '-' + s + '-' + l]) n++; return n; };
  /* Cột như WordRegion.Load: <=4 từ 1 cột, <=10 từ 2 cột, còn lại 3 cột; tên cột theo thứ tự cột trước. */
  WC.columns = function (n) { return n <= 4 ? 1 : n <= 10 ? 2 : 3; };
  WC.normalize = function (w) { return w.normalize('NFD').replace(/[̀-ͯ]/g, '').toLowerCase(); };
  WC.canBuild = function (word, letters) {
    var bag = {}; letters.split('').forEach(function (c) { bag[c] = (bag[c] || 0) + 1; });
    return word.split('').every(function (c) { if (!bag[c]) return false; bag[c]--; return true; });
  };
  /* Phân loại từ: answer / extra / reject, giống WordRegion.CheckAnswer (answers trước, rồi validWords). */
  WC.classify = function (data, word) {
    var w = WC.normalize(word);
    if (data.a.indexOf(w) >= 0) return 'answer';
    if (data.v.indexOf(w) >= 0) return 'extra';
    return 'reject';
  };

  /* ---------- ruby ---------- */
  WC.credit = function (n, id) {
    if (id) { if (S.claims[id]) return false; S.claims[id] = 1; }
    S.rubies += n; WC.save(); return true;
  };
  WC.debit = function (n) {
    if (S.rubies < n) return false;
    S.rubies -= n; WC.save(); return true;
  };
  WC.firstClearReward = function (lv) { return lv.w === 0 && lv.s === 0 && lv.l === 0 ? WC.ECON.firstClearTutorial : WC.ECON.firstClear; };
  WC.extraClaimAmount = function () { return S.extra.target / WC.ECON.extraPer * 2; };
  WC.addExtra = function () {
    S.extra.progress++; S.extra.total++; WC.save();
  };
  WC.claimExtra = function () {
    if (S.extra.progress < S.extra.target) return 0;
    var n = WC.extraClaimAmount();
    S.extra.progress -= S.extra.target;
    if (S.extra.target === 5 && S.extra.total > 10) S.extra.target = 10;
    S.rubies += n; WC.save(); return n;
  };
  /* Thắng level: trả thưởng lần đầu đúng một lần (idempotent theo cleared). */
  WC.completeLevel = function (lv) {
    var k = WC.key(lv), first = !S.cleared[k], reward = 0;
    if (first) { S.cleared[k] = 1; reward = WC.firstClearReward(lv); S.rubies += reward; }
    S.winCtx = {kind: 'level', key: k, first: first, reward: reward};
    WC.save(); return S.winCtx;
  };
  /* Bonus x2 xem video chỉ nhân phần thưởng lần đầu, một lần cho mỗi level, không cộng dồn với mốc chuỗi/extra. */
  WC.winBonus = function (ctx) {
    if (!ctx || !ctx.reward) return 0;
    return ctx.reward;
  };

  /* ---------- ngày/Daily ---------- */
  var pad = function (n) { return (n < 10 ? '0' : '') + n; };
  WC.fmtDate = function (d) { return d.getUTCFullYear() + '-' + pad(d.getUTCMonth() + 1) + '-' + pad(d.getUTCDate()); };
  WC.parseDate = function (s) { var p = s.split('-'); return new Date(Date.UTC(+p[0], +p[1] - 1, +p[2])); };
  WC.addDays = function (s, n) { var d = WC.parseDate(s); d.setUTCDate(d.getUTCDate() + n); return WC.fmtDate(d); };
  WC.daysBetween = function (a, b) { return Math.round((WC.parseDate(b) - WC.parseDate(a)) / 86400000); };
  var hash = function (str) { var h = 2166136261; for (var i = 0; i < str.length; i++) { h ^= str.charCodeAt(i); h = Math.imul(h, 16777619); } return h >>> 0; };
  WC.hash = hash;
  /* Bàn Daily: hash(ngôn ngữ chơi | ngày) chọn trong pool level của ngôn ngữ đó. Pool prototype = 6 level mẫu thật (không phải level 1–12 của hướng dẫn). */
  var POOL = ['0-2-3', '1-0-0', '2-2-5', '3-1-2', '4-3-9', '5-4-17'];
  WC.dailyKey = function (date, gameLang) { return POOL[hash(gameLang + '|' + date) % POOL.length]; };
  WC.dailyNo = function (date) { return WC.daysBetween('2026-01-01', date) + 1; };
  WC.dailyStatus = function (date) {
    var t = S.daily.today;
    if (date > t) return 'future';
    if (S.daily.solved[date]) return 'done';
    if (date === t) return 'today';
    if (date === WC.addDays(t, -1)) return 'miss';
    return 'old';
  };
  WC.dailyReward = function (date) {
    var st = WC.dailyStatus(date);
    if (S.daily.claimed[date]) return 0;
    if (st === 'today') return WC.ECON.dailyBase;
    if (st === 'miss') return WC.ECON.backfill;
    return 0;
  };
  /* Chuỗi = số ngày liên tiếp đã xong tính lùi từ hôm nay (hoặc từ hôm qua nếu hôm nay chưa chơi). */
  WC.streakFrom = function (end) { var n = 0, d = end; while (S.daily.solved[d]) { n++; d = WC.addDays(d, -1); } return n; };
  /* Điểm cuối của chuỗi: hôm nay nếu đã xong, không thì hôm qua; nếu lỡ hôm qua nhưng còn chuỗi trước đó thì vẫn hiển thị (đang chờ bù). */
  WC.streakEnd = function () {
    var t = S.daily.today, y = WC.addDays(t, -1);
    if (S.daily.solved[t]) return t;
    if (S.daily.solved[y]) return y;
    if (S.daily.solved[WC.addDays(t, -2)]) return WC.addDays(t, -2);
    return y;
  };
  WC.streak = function () { return WC.streakFrom(WC.streakEnd()); };
  WC.streakState = function () {
    var t = S.daily.today, y = WC.addDays(t, -1);
    if (S.daily.solved[t]) return 'safe';
    if (S.daily.solved[y]) return 'risk';
    if (!S.daily.solved[y] && S.daily.solved[WC.addDays(t, -2)]) return 'risk'; /* hôm qua lỡ, còn bù được */
    return 'lost';
  };
  WC.nextMilestone = function (n) {
    var ks = Object.keys(WC.ECON.milestones).map(Number).sort(function (a, b) { return a - b; });
    for (var i = 0; i < ks.length; i++) if (ks[i] > n) return {d: ks[i], n: WC.ECON.milestones[ks[i]]};
    return null;
  };
  /* Hoàn thành Daily cho `date`: thưởng cơ bản một lần + mốc chuỗi một lần cho mỗi chuỗi. */
  WC.completeDaily = function (date) {
    var D = S.daily, base = 0, mile = 0, mileDays = 0;
    var first = !D.solved[date];
    D.solved[date] = 1;
    if (first && !D.claimed[date]) {
      base = WC.dailyRewardBefore(date);
      D.claimed[date] = 1; S.rubies += base;
    }
    var run = WC.streak(), end = WC.streakEnd(), start = WC.addDays(end, -(run - 1));
    Object.keys(WC.ECON.milestones).forEach(function (m) {
      var id = 'ms:' + start + ':' + m;
      if (run >= +m && !D.milestones[id]) { D.milestones[id] = 1; mile += WC.ECON.milestones[m]; mileDays = +m; S.rubies += WC.ECON.milestones[m]; }
    });
    if (run > D.best) D.best = run;
    S.winCtx = {kind: 'daily', date: date, first: first, reward: base, milestone: mile, milestoneDays: mileDays, streak: run};
    WC.save(); return S.winCtx;
  };
  WC.dailyRewardBefore = function (date) {
    var t = S.daily.today;
    if (date === t) return WC.ECON.dailyBase;
    if (date === WC.addDays(t, -1)) return WC.ECON.backfill;
    return 0;
  };
  WC.advanceDay = function () { S.daily.today = WC.addDays(S.daily.today, 1); WC.save(); };

  /* ---------- Bot (deterministic, không LLM, không giả người thật) ---------- */
  var mulberry = function (a) { return function () { a |= 0; a = a + 0x6D2B79F5 | 0; var t = Math.imul(a ^ a >>> 15, 1 | a); t = t + Math.imul(t ^ t >>> 7, 61 | t) ^ t; return ((t ^ t >>> 14) >>> 0) / 4294967296; }; };
  WC.rng = mulberry;
  var DIFF = {easy: {sec: 12, jit: .5, miss: .35, level: '0-2-3'}, normal: {sec: 8, jit: .5, miss: .12, level: '1-0-0'}, hard: {sec: 5.5, jit: .4, miss: 0, level: '2-2-5'}};
  WC.botDiff = DIFF;
  /* Kế hoạch của bot chỉ phụ thuộc (levelKey, difficulty, seed): không đọc input của người chơi. Cùng seed → cùng timeline → replay giống hệt. */
  WC.botPlan = function (levelKey, diff, seed) {
    var d = DIFF[diff], data = window.WC_LEVELS[levelKey], r = mulberry(hash(levelKey + '|' + diff + '|' + seed));
    var order = data.a.map(function (w) { return {w: w, k: w.length + r() * 2.5}; }).sort(function (a, b) { return a.k - b.k; });
    var t = 0, plan = [];
    order.forEach(function (o) {
      t += d.sec * (1 - d.jit / 2 + d.jit * r()) * (0.7 + 0.15 * o.w.length);
      if (r() >= d.miss) plan.push({w: o.w, t: Math.round(t * 10) / 10});
    });
    return plan;
  };
  WC.botScoreAt = function (plan, t) { return plan.filter(function (p) { return p.t <= t; }).reduce(function (s, p) { return s + p.w.length; }, 0); };
  WC.botFoundAt = function (plan, t) { return plan.filter(function (p) { return p.t <= t; }); };
  /* Kết quả: bên tìm đủ tất cả từ trước thắng; hết giờ thì so điểm (chữ), bằng nhau là hòa. */
  WC.botOutcome = function (levelKey, plan, playerFound, playerTimes, endT) {
    var data = window.WC_LEVELS[levelKey], total = data.a.length, L = WC.ECON.botLimit;
    var pPts = playerFound.reduce(function (s, w) { return s + w.length; }, 0);
    var bFound = WC.botFoundAt(plan, endT), bPts = bFound.reduce(function (s, p) { return s + p.w.length; }, 0);
    var res = 'draw';
    if (playerFound.length === total && (bFound.length < total || playerTimes[playerTimes.length - 1] <= plan[plan.length - 1].t)) res = 'win';
    else if (bFound.length === total && playerFound.length < total) res = 'lose';
    else if (endT >= L) res = pPts > bPts ? 'win' : pPts < bPts ? 'lose' : 'draw';
    return {result: res, player: pPts, bot: bPts, botWords: bFound.length};
  };
  WC.botReward = function (res, diff, seedKey) {
    var B = S.bot, E = WC.ECON;
    if (B.day !== S.daily.today) { B.day = S.daily.today; B.earned = 0; B.rewarded = {}; }
    if (B.rewarded[seedKey] || res === 'lose') return 0;
    var want = res === 'win' ? E.botWin[diff] : E.botDraw;
    var n = Math.min(want, E.botCap - B.earned);
    if (n <= 0) return 0;
    B.rewarded[seedKey] = 1; B.earned += n; S.rubies += n; WC.save(); return n;
  };

  /* ---------- Ads mô phỏng ---------- */
  WC.nowSec = function () { return Math.floor(Date.now() / 1000); };
  WC.cooldownLeft = function () {
    if (S.ads.lastRewardAt < 0) return 0;
    return Math.max(0, WC.ECON.videoCooldown - (WC.nowSec() - S.ads.lastRewardAt));
  };
  /* Trạng thái hiển thị: demo chọn tay; riêng 'ready' sẽ thành cooldown nếu vừa nhận (observed: rewardedVideoPeriod). */
  WC.adState = function () {
    var f = S.ads.forced;
    if (f === 'ready' && WC.cooldownLeft() > 0) return 'cooldown';
    return f;
  };
  /* Thưởng CHỈ cấp ở callback hoàn thành; mỗi phiên có id riêng nên bấm lặp không cộng hai lần. */
  WC.adComplete = function (sessionId, amount) {
    if (!WC.credit(amount, 'ad:' + sessionId)) return false;
    S.ads.lastRewardAt = WC.nowSec(); WC.save(); return true;
  };
  WC.fmtTime = function (s) { return pad(Math.floor(s / 60)) + ':' + pad(s % 60); };
  /* Interstitial (PROPOSED): chỉ ở khoảng nghỉ sau khi thắng. */
  WC.interstitialAllowed = function (c) {
    if (c.adsRemoved) return false;
    if (c.where !== 'afterWin') return false;
    if (c.clearedCount < 6) return false;      /* grace: 5 level đầu không có interstitial */
    if (c.secondsSinceLast < WC.ECON.interstitialGap) return false;
    if (c.shownToday >= 6) return false;
    if (c.afterRewarded) return false;
    return true;
  };
})();
