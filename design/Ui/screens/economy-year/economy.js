/* Demo mô phỏng coin/shop/ads/nhiệm vụ/chu kỳ 365. Không thanh toán thật, không gọi SDK quảng cáo, không mạng. Số liệu từ GD/economy-year-data (ĐỀ XUẤT tuning). */
(function () {
  var TX = {
    vi: { title: 'Coin & Năm 365', sim: 'MÔ PHỎNG — không thanh toán thật, không quảng cáo thật', shop: 'Shop', ads: 'Quảng cáo', rew: 'Thưởng', quest: 'Nhiệm vụ', year: 'Năm',
      buy: 'Mua (mô phỏng)', spend: 'Tiêu coin', get: 'Nhận', got: 'Đã nhận', claim: 'Nhận', prog: '+ tiến độ', notEnough: 'Thiếu coin', needCoin: 'Cần %n coin. Xem video là tuỳ chọn.', watch: 'Xem video +%n (mô phỏng)', skip: 'Bỏ qua, chơi tiếp', toShop: 'Mở Shop',
      placement: 'Vị trí', outcome: 'Kết quả giả lập', play: 'Xem quảng cáo (mô phỏng)', winLevel: 'Thắng level mới (mô phỏng)', time: 'Thời gian giả lập', plus30: '+30 giây', nextDay: 'Sang ngày kế (00:00 UTC)', day: 'Ngày', cycle: 'Chu kỳ', login: 'Đăng nhập hôm nay', streak: 'Chuỗi', repair: 'Cứu chuỗi', best: 'Kỷ lục',
      noads: 'Đã tắt quảng cáo xen kẽ', inter: 'Quảng cáo xen kẽ (mô phỏng)', interNo: 'Không hiện xen kẽ', daily: 'Ngày', weekly: 'Tuần', monthly: 'Tháng', event: 'Sự kiện', allClear: 'Thưởng hoàn thành hết', noQuest: 'Không có sự kiện hôm nay', free: 'Nhiệm vụ không cần mua hay xem quảng cáo',
      month: 'Tháng', setToday: 'Đặt làm hôm nay', week: 'Tuần', bridge: 'Ngày cầu nối', today: 'Hôm nay', dl: 'Tải JSON/schema', milestones: 'Mốc chuỗi', baseline: 'Free-player: casual %c, chơi nhiều %e coin/ngày (không ads)', closed: 'Đóng', balance: 'Số dư', capLeft: 'Còn %n lượt', cdLeft: 'Chờ %n giây' },
    en: { title: 'Coins & Year 365', sim: 'SIMULATED — no real payment, no real ads', shop: 'Shop', ads: 'Ads', rew: 'Rewards', quest: 'Quests', year: 'Year',
      buy: 'Buy (simulated)', spend: 'Spend coins', get: 'Claim', got: 'Claimed', claim: 'Claim', prog: '+ progress', notEnough: 'Not enough coins', needCoin: 'Need %n coins. Watching a video is optional.', watch: 'Watch video +%n (simulated)', skip: 'Skip, keep playing', toShop: 'Open Shop',
      placement: 'Placement', outcome: 'Simulated outcome', play: 'Watch ad (simulated)', winLevel: 'Win a new level (simulated)', time: 'Simulated time', plus30: '+30 seconds', nextDay: 'Next day (00:00 UTC)', day: 'Day', cycle: 'Cycle', login: 'Today login', streak: 'Streak', repair: 'Streak repair', best: 'Best',
      noads: 'Interstitials removed', inter: 'Interstitial (simulated)', interNo: 'No interstitial shown', daily: 'Daily', weekly: 'Weekly', monthly: 'Monthly', event: 'Event', allClear: 'Clear-all bonus', noQuest: 'No event today', free: 'Quests never require purchase or ads',
      month: 'Month', setToday: 'Set as today', week: 'Week', bridge: 'Bridge day', today: 'Today', dl: 'Download JSON/schema', milestones: 'Streak milestones', baseline: 'Free player: casual %c, engaged %e coins/day (no ads)', closed: 'Close', balance: 'Balance', capLeft: '%n left', cdLeft: 'Wait %n s' }
  };
  var FILES = ['economy-config.json', 'quest-templates.json', 'calendar-365.json', 'loc-keys.json', 'economy-year.schema.json'];
  var OUTCOMES = ['success', 'cancelled', 'no_fill', 'offline', 'error'];

  class EcoDemo {
    constructor(root) {
      this.root = root; this.D = window.WC_ECON; this.cfg = this.D.config; this.cal = this.D.calendar;
      var wc = window.WC; this.lang = wc && wc.lang === 'vi' ? 'vi' : 'en'; this.T = TX[this.lang];
      this.Q = {}; this.D.quests.templates.forEach(function (q) { this.Q[q.id] = q; }, this);
      this.LR = {}; this.cal.loginRewards.forEach(function (x) { this.LR[x.id] = x; }, this);
      this.ads = {}; this.cfg.ads.rewarded.forEach(function (a) { this.ads[a.id] = a; }, this);
      this.s = { coin: this.cfg.currency.startBalance, day: 1, cycle: 1, t: 0, tab: 'shop', scope: 'daily', month: 1, sel: 1, claimed: {}, prog: {}, used: {}, cdUntil: {}, level: 0, streak: 0, best: 0, lastLogin: -9, noAds: false, log: [], interToday: 0, lastInter: -9999, lastRew: -9999, placement: 'ad_shop_free', outcome: 'success', popup: null, rewardsDay: {} };
      this.render();
    }
    tr(k, p) { var s = this.T[k] || k; Object.keys(p || {}).forEach(function (x) { s = s.replace('%' + x, p[x]); }); return s; }
    lk(k) { var e = this.D.loc.keys[k]; return e ? (e[this.lang] || e.en) : k; }
    abs() { return (this.s.cycle - 1) * 365 + this.s.day; }
    log(m) { this.s.log.unshift('[C' + this.s.cycle + ' d' + this.s.day + ' t' + this.s.t + 's] ' + m); this.s.log = this.s.log.slice(0, 12); }
    add(n, why) { this.s.coin += n; this.log('+' + n + ' coin · ' + why); }
    key(scope, period, ref) { return 'claim:C' + this.s.cycle + ':' + scope + ':' + period + ':' + ref; }
    claimOnce(k, coin, why) {
      if (this.s.claimed[k]) { this.log('duplicate claim ignored · ' + k); return false; }
      this.s.claimed[k] = true; this.add(coin, why); return true;
    }
    dayRow() { return this.cal.days[this.s.day - 1]; }
    spend(id) {
      var c = this.cfg.sinks.filter(function (x) { return x.id === id; })[0];
      if (this.s.coin < c.coin) { this.s.popup = { need: c.coin }; return; }
      this.s.coin -= c.coin; this.log('-' + c.coin + ' coin · ' + id);
      if (id === 'sk_streak_repair') { this.s.streak += 1; this.s.lastLogin = this.abs() - 1; }
    }
    period(scope) {
      var d = this.dayRow();
      if (scope === 'daily') return 'd' + String(d.day).padStart(3, '0');
      if (scope === 'weekly') return d.week ? 'w' + String(d.week).padStart(2, '0') : 'bridge';
      if (scope === 'monthly') return 'm' + String(d.month).padStart(2, '0');
      return d.eventRef || 'none';
    }
    questRefs(scope) {
      var d = this.dayRow();
      if (scope === 'daily') return d.dailyQuestRefs;
      if (scope === 'weekly') return d.week ? this.cal.weeks[d.week - 1].questRefs : [];
      if (scope === 'monthly') return this.cal.months[d.month - 1].questRefs;
      return d.eventRef ? this.cal.events.filter(function (e) { return e.id === d.eventRef; })[0].questRefs : [];
    }
    allClear(scope) {
      var d = this.dayRow();
      if (scope === 'daily') return { id: 'allclear', coin: this.D.quests.dailyAllClearCoin };
      if (scope === 'weekly' && d.week) return { id: 'allclear', coin: this.cal.weeks[d.week - 1].allClearCoin };
      if (scope === 'monthly') return { id: 'allclear', coin: this.cal.months[d.month - 1].allClearCoin };
      if (scope === 'event' && d.eventRef) return { id: 'allclear', coin: this.cal.events.filter(function (e) { return e.id === d.eventRef; })[0].completionCoin };
      return null;
    }
    bump(metric, n) {
      ['daily', 'weekly', 'monthly', 'event'].forEach(function (sc) {
        this.questRefs(sc).forEach(function (r) {
          if (this.Q[r].metric === metric) { var k = this.key(sc, this.period(sc), r); this.s.prog[k] = Math.min(this.Q[r].target, (this.s.prog[k] || 0) + n); }
        }, this);
      }, this);
    }
    adStatus(id) {
      var a = this.ads[id], g = a.sharedCapGroup, used = this.s.used[g + ':' + this.abs()] || 0;
      var cap = this.cfg.ads.rewarded.filter(function (x) { return x.sharedCapGroup === g; })[0].dailyCap;
      if (id === 'ad_daily_x2' && (this.s.used['x2d:' + this.abs()] || 0) >= 1) return { st: 'cap_reached' };
      if (used >= cap) return { st: 'cap_reached' };
      var cd = (this.s.cdUntil[id] || 0) - this.s.t;
      if (cd > 0) return { st: 'cooldown', cd: cd };
      return { st: 'ready', left: cap - used };
    }
    playAd(id) {
      var a = this.ads[id], r = this.adStatus(id), o = this.s.outcome, st = r.st === 'ready' ? o : r.st;
      if (st === 'success') {
        this.s.used[a.sharedCapGroup + ':' + this.abs()] = (this.s.used[a.sharedCapGroup + ':' + this.abs()] || 0) + 1;
        if (id === 'ad_daily_x2') this.s.used['x2d:' + this.abs()] = 1;
        this.s.cdUntil[id] = this.s.t + a.cooldownSec; this.s.lastRew = this.s.t; this.add(a.coin, id);
      } else { this.log('0 coin · ' + id + ' · ' + this.lk(this.cfg.ads.states.filter(function (x) { return x.id === st; })[0].locKey)); }
    }
    winLevel() {
      this.s.level += 1; var first = this.s.level === 1 ? this.cfg.earn.firstEverWinBonusCoin : 0;
      this.add(this.cfg.earn.firstWinCoin + first, 'win level ' + this.s.level); this.bump('levels_won', 1); this.bump('words_found', 6);
      var i = this.cfg.ads.interstitial, s = this.s;
      var why = s.noAds ? 'noads' : s.level <= i.graceLevels ? 'grace' : s.t - s.lastInter < i.minGapSec ? 'gap' : s.t - s.lastRew < i.blockedAfterRewardedSec ? 'after-rewarded' : s.interToday >= i.dailyCap ? 'cap' : '';
      if (why) { this.log(this.T.interNo + ' (' + why + ')'); return; }
      s.interToday += 1; s.lastInter = s.t; this.s.popup = { inter: true };
    }
    claimLogin() {
      var d = this.dayRow(), ab = this.abs();
      if (!this.claimOnce(this.key('login', this.period('daily'), d.loginRef), this.LR[d.loginRef].coin, d.loginRef)) return;
      this.s.streak = this.s.lastLogin === ab - 1 ? this.s.streak + 1 : 1; this.s.lastLogin = ab; this.s.best = Math.max(this.s.best, this.s.streak);
      this.bump('login_days', 1);
    }
    nextDay() {
      this.s.day += 1; this.s.t += 86400;
      if (this.s.day > 365) { this.s.day = 1; this.s.cycle += 1; this.log('Cycle C' + this.s.cycle + ' started'); }
      this.s.interToday = 0; this.s.sel = this.s.day; this.s.month = this.dayRow().month;
    }
    repairable() { return this.s.streak > 0 && this.s.lastLogin === this.abs() - 2; }
    coinPill() { return '<div class="coin-pill"><span class="coin-dot"></span><b>' + this.s.coin + '</b></div>'; }
    card(t, sub, btn) { return '<div class="eco-card"><div class="grow"><b>' + t + '</b><small>' + sub + '</small></div>' + btn + '</div>'; }
    btn(label, act, arg, cls, off) { return '<button class="btn ' + (cls || '') + (off ? ' off' : '') + '" data-act="' + act + '" data-arg="' + (arg || '') + '">' + label + '</button>'; }
    tabShop() {
      var h = '<div class="eco-h">' + this.T.shop + '</div>';
      this.cfg.shop.packs.forEach(function (p) {
        h += this.card(this.lk(p.locKey), '<code>' + p.productId + '</code> · ~$' + p.proposedUsd + (p.removesAds ? ' · no ads' : ''), this.btn(this.T.buy, 'buy', p.id, 'yellow'));
      }, this);
      h += '<div class="eco-h">' + this.T.spend + '</div>';
      this.cfg.sinks.forEach(function (x) {
        var off = x.id === 'sk_streak_repair' && !this.repairable();
        h += this.card(this.lk(x.locKey), x.coin + ' coin', this.btn(x.coin, 'spend', x.id, 'coral', off));
      }, this);
      return h + '<div class="eco-h">' + this.T.free + '</div>';
    }
    tabAds() {
      var s = this.s, h = '<div class="eco-row"><span>' + this.T.time + ': <b>' + s.t + 's</b></span>' + this.btn(this.T.plus30, 'tick', '', 'ghost') + '</div>';
      h += '<div class="eco-row"><label>' + this.T.outcome + '</label><select class="eco-sel" data-sel="outcome">' + OUTCOMES.map(function (o) { return '<option' + (o === s.outcome ? ' selected' : '') + '>' + o + '</option>'; }).join('') + '</select></div>';
      this.cfg.ads.rewarded.forEach(function (a) {
        var r = this.adStatus(a.id), sub = r.st === 'ready' ? this.tr('capLeft', { n: r.left }) : r.st === 'cooldown' ? this.tr('cdLeft', { n: r.cd }) : this.lk('ad.cap');
        h += this.card(a.placement + ' +' + a.coin, a.id + ' · ' + sub, this.btn(this.T.play, 'ad', a.id, '', r.st !== 'ready'));
      }, this);
      h += this.card(this.T.inter, 'after_win · grace ' + this.cfg.ads.interstitial.graceLevels + ' · ' + this.cfg.ads.interstitial.minGapSec + 's · ' + this.cfg.ads.interstitial.dailyCap + '/d' + (s.noAds ? ' · ' + this.T.noads : ''), this.btn(this.T.winLevel, 'win', '', 'yellow'));
      return h;
    }
    tabRew() {
      var s = this.s, d = this.dayRow(), done = s.claimed[this.key('login', this.period('daily'), d.loginRef)];
      var h = this.card(this.T.login + ' · ' + d.loginRef, '+' + this.LR[d.loginRef].coin + ' · ' + this.T.streak + ' ' + s.streak + ' · ' + this.T.best + ' ' + s.best, this.btn(done ? this.T.got : this.T.get, 'login', '', '', done));
      if (this.repairable()) h += this.card(this.T.repair, this.cfg.sinks[2].coin + ' coin', this.btn(this.T.repair, 'spend', 'sk_streak_repair', 'coral'));
      h += '<div class="eco-h">' + this.T.milestones + '</div>';
      this.cfg.streak.milestones.forEach(function (m) {
        var k = this.key('streak', 'cycle', m.id), ok = s.streak >= m.streakDays, got = s.claimed[k];
        h += this.card(this.lk(m.locKey), '+' + m.coin, this.btn(got ? this.T.got : this.T.claim, 'ms', 'streak|' + m.id + '|' + m.coin, '', got || !ok));
      }, this);
      this.cfg.levelMilestones.forEach(function (m) {
        var k = 'claim:life:level:' + m.id, ok = s.level >= m.levelsCleared, got = s.claimed[k];
        h += this.card(this.lk(m.locKey), '+' + m.coin + ' · ' + s.level + '/' + m.levelsCleared, this.btn(got ? this.T.got : this.T.claim, 'ms', 'level|' + m.id + '|' + m.coin, '', got || !ok));
      }, this);
      return h + this.btn(this.T.winLevel, 'win', '', 'yellow block');
    }
    tabQuest() {
      var s = this.s, h = '<div class="eco-row">' + ['daily', 'weekly', 'monthly', 'event'].map(function (sc) { return '<button class="chip2' + (sc === s.scope ? ' on' : '') + '" data-act="scope" data-arg="' + sc + '">' + this.T[sc] + '</button>'; }, this).join('') + '</div>';
      var refs = this.questRefs(s.scope), per = this.period(s.scope), all = true;
      if (!refs.length) h += '<div class="eco-card">' + this.T.noQuest + '</div>';
      refs.forEach(function (r) {
        var q = this.Q[r], k = this.key(s.scope, per, r), p = s.prog[k] || 0, ok = p >= q.target, got = s.claimed[k]; if (!got) all = false;
        h += '<div class="eco-card"><div class="grow"><b>' + this.lk(q.locKey) + '</b><small>' + p + '/' + q.target + ' · +' + q.coin + ' coin</small><div class="eco-bar"><i style="width:' + Math.round(100 * p / q.target) + '%"></i></div></div>' + (ok ? this.btn(got ? this.T.got : this.T.claim, 'qclaim', r, '', got) : this.btn(this.T.prog, 'qbump', r, 'ghost')) + '</div>';
      }, this);
      var ac = this.allClear(s.scope);
      if (ac && refs.length) { var ak = this.key(s.scope, per, 'allclear'); h += this.card(this.T.allClear, '+' + ac.coin, this.btn(s.claimed[ak] ? this.T.got : this.T.claim, 'qall', '', 'yellow', s.claimed[ak] || !all)); }
      return h + '<div class="eco-h">' + this.T.free + '</div>';
    }
    tabYear() {
      var s = this.s, c = this.cal, m = c.months[s.month - 1], h = '<div class="eco-row"><label>' + this.T.cycle + ' C' + s.cycle + '</label><select class="eco-sel" data-sel="month">' + c.months.map(function (x) { return '<option value="' + x.month + '"' + (x.month === s.month ? ' selected' : '') + '>' + this.lk(x.locKey) + '</option>'; }, this).join('') + '</select><span>' + this.T.today + ': ' + this.T.day + ' ' + s.day + '</span></div><div class="eco-cal">';
      for (var d = m.startDay; d <= m.endDay; d++) {
        var r = c.days[d - 1];
        h += '<button class="eco-day' + (r.eventRef ? ' ev' : '') + (r.bridge ? ' br' : '') + (d === s.day ? ' today' : '') + (d === s.sel ? ' sel' : '') + '" data-act="sel" data-arg="' + d + '">' + d + '</button>';
      }
      var r = c.days[s.sel - 1], ev = r.eventRef ? c.events.filter(function (e) { return e.id === r.eventRef; })[0] : null;
      h += '</div>' + this.card(this.T.day + ' ' + r.day + (r.bridge ? ' · ' + this.T.bridge : ' · ' + this.T.week + ' ' + r.week), this.lk(this.LR[r.loginRef].locKey) + ' +' + this.LR[r.loginRef].coin + '<br>' + r.dailyQuestRefs.map(function (q) { return this.lk(this.Q[q].locKey); }, this).join(', ') + (ev ? '<br>' + this.lk(ev.locKey) + ' (' + ev.startDay + '–' + ev.endDay + ')' : ''), this.btn(this.T.setToday, 'today', '', 'ghost'));
      h += this.card(this.T.baseline.replace('%c', this.cfg.baseline.casualDay.net).replace('%e', this.cfg.baseline.engagedDay.net), 'C' + s.cycle + ' · 52 ' + this.T.week + ' + 1 ' + this.T.bridge, '');
      return h + '<div class="eco-h">' + this.T.dl + '</div><div class="eco-row">' + FILES.map(function (f) { return '<a class="chip2" href="../../../GD/economy-year-data/' + f + '" download>' + f + '</a>'; }).join('') + '</div>';
    }
    popup() {
      var p = this.s.popup; if (!p) return '';
      if (p.inter) return '<div class="eco-pop"><div class="dlg"><h3>' + this.T.inter + '</h3><div>' + this.T.sim + '</div>' + this.btn(this.T.closed, 'close', '', 'block') + '</div></div>';
      var a = this.ads.ad_low_coin;
      return '<div class="eco-pop"><div class="dlg"><h3>' + this.T.notEnough + '</h3><div>' + this.tr('needCoin', { n: p.need }) + '</div>' + this.btn(this.tr('watch', { n: a.coin }), 'lowad', '', 'yellow block', this.adStatus('ad_low_coin').st !== 'ready') + this.btn(this.T.toShop, 'tabshop', '', 'block') + this.btn(this.T.skip, 'close', '', 'ghost block') + '</div></div>';
    }
    render() {
      var s = this.s, tabs = ['shop', 'ads', 'rew', 'quest', 'year'];
      var body = { shop: this.tabShop, ads: this.tabAds, rew: this.tabRew, quest: this.tabQuest, year: this.tabYear }[s.tab].call(this);
      this.root.innerHTML = '<div class="demo-tag">DEMO</div><div class="hdr"><a class="icon-btn" href="../../index.html" aria-label="back">←</a><div class="title">' + this.T.title + '</div>' + this.coinPill() + '</div>' +
        '<div class="eco-sim">' + this.T.sim + '</div><div class="eco-row" style="padding:0 14px"><span>' + this.T.cycle + ' <b>C' + s.cycle + '</b> · ' + this.T.day + ' <b>' + s.day + '/365</b></span>' + this.btn(this.T.nextDay, 'next', '', 'ghost') + '</div>' +
        '<div class="eco-tabs">' + tabs.map(function (t) { return '<button class="chip2' + (t === s.tab ? ' on' : '') + '" data-act="tab" data-arg="' + t + '">' + this.T[t] + '</button>'; }, this).join('') + '</div>' +
        '<div class="eco-body">' + body + '<div class="eco-log">' + (s.log.join('<br>') || '—') + '</div></div>' + this.popup();
      this.bind();
    }
    bind() {
      var self = this;
      [].forEach.call(this.root.querySelectorAll('[data-act]'), function (e) { e.addEventListener('click', function () { self.act(e.dataset.act, e.dataset.arg); }); });
      [].forEach.call(this.root.querySelectorAll('[data-sel]'), function (e) { e.addEventListener('change', function () { self.s[e.dataset.sel] = e.dataset.sel === 'month' ? +e.value : e.value; self.render(); }); });
    }
    act(a, arg) {
      var s = this.s, p;
      if (a === 'tab') s.tab = arg; else if (a === 'scope') s.scope = arg; else if (a === 'sel') s.sel = +arg;
      else if (a === 'today') { s.day = s.sel; s.month = this.dayRow().month; }
      else if (a === 'next') this.nextDay(); else if (a === 'tick') s.t += 30; else if (a === 'close') s.popup = null; else if (a === 'tabshop') { s.popup = null; s.tab = 'shop'; }
      else if (a === 'buy') { p = this.cfg.shop.packs.filter(function (x) { return x.id === arg; })[0]; this.add(p.coin, 'SIMULATED purchase ' + p.productId); if (p.removesAds) s.noAds = true; }
      else if (a === 'spend') this.spend(arg); else if (a === 'ad') this.playAd(arg); else if (a === 'lowad') { s.popup = null; this.playAd('ad_low_coin'); }
      else if (a === 'win') this.winLevel(); else if (a === 'login') this.claimLogin();
      else if (a === 'ms') { p = arg.split('|'); this.claimOnce(p[0] === 'level' ? 'claim:life:level:' + p[1] : this.key('streak', 'cycle', p[1]), +p[2], p[1]); }
      else if (a === 'qbump') this.bump(this.Q[arg].metric, Math.max(1, Math.ceil(this.Q[arg].target / 3)));
      else if (a === 'qclaim') this.claimOnce(this.key(s.scope, this.period(s.scope), arg), this.Q[arg].coin, arg);
      else if (a === 'qall') this.claimOnce(this.key(s.scope, this.period(s.scope), 'allclear'), this.allClear(s.scope).coin, 'allclear ' + s.scope);
      this.render();
    }
  }
  new EcoDemo(document.getElementById('eco'));
})();
