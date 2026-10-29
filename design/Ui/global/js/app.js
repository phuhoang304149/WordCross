(function () {
  var WC = window.WC, S = WC.state, t = WC.t, E = WC.ECON;
  var screens = window.WC_SCREENS;
  var page = document.body.dataset.page;
  var root = document.getElementById('app');

  var svg = function (p) { return '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.4" stroke-linecap="round" stroke-linejoin="round">' + p + '</svg>'; };
  var ic = {
    back: svg('<path d="m15 5-7 7 7 7"/>'), pause: svg('<path d="M9 5v14M15 5v14"/>'),
    shuffle: svg('<path d="M3 7h4l10 10h4M3 17h4l3-3M14 10l3-3h4M18 4l3 3-3 3M18 14l3 3-3 3"/>'),
    hint: svg('<path d="M9 18h6M10 21h4M12 3a6 6 0 0 0-4 9.5c.6.6 1 1.4 1 2.5h6c0-1.1.4-1.9 1-2.5A6 6 0 0 0 12 3Z"/>'),
    share: svg('<circle cx="6" cy="12" r="2.5"/><circle cx="18" cy="6" r="2.5"/><circle cx="18" cy="18" r="2.5"/><path d="m8.2 11 7.6-4M8.2 13l7.6 4"/>'),
    book: svg('<path d="M4 5a2 2 0 0 1 2-2h13v16H6a2 2 0 0 0-2 2V5Z"/><path d="M8 7h7"/>'),
    close: svg('<path d="M6 6l12 12M18 6 6 18"/>'), globe: svg('<circle cx="12" cy="12" r="9"/><path d="M3 12h18M12 3c3 3 3 15 0 18M12 3c-3 3-3 15 0 18"/>')
  };
  var go = function (id) { return page ? '../' + id + '/index.html' : 'screens/' + id + '/index.html'; };
  var nav = function (id) { location.href = go(id) + (WC.embed ? '?embed=1&lang=' + WC.lang : ''); };
  var find = function (id) { return screens.filter(function (s) { return s.id === id; })[0]; };
  var ruby = function (big) { return '<span class="ruby' + (big ? ' lg' : '') + '"></span>'; };
  var rubyPill = function () { return '<a class="ruby-pill" aria-label="' + t('common.balance') + '" href="' + go('shop') + '">' + ruby() + '<span data-bal>' + S.rubies + '</span><span class="plus" aria-label="' + t('common.addRubies') + '">+</span></a>'; };
  var bar = function (title, backTo) {
    return '<div class="hdr"><a class="icon-btn" aria-label="' + t('common.back') + '" href="' + go(backTo) + '">' + ic.back + '</a><div class="title">' + title + '</div>' + rubyPill() + '</div>';
  };
  var tag = function (k) { return '<span class="tagx">' + t(k) + '</span>'; };
  var fmtShort = WC.shortDate;

  /* level hiện tại của màn game: level thường hoặc Daily */
  var ctxLevel = function () {
    if (S.mode === 'daily' && S.dailyDate) {
      var k = WC.dailyKey(S.dailyDate, S.gameLang);
      return {key: 'daily:' + S.dailyDate, data: window.WC_LEVELS[k], sample: false, daily: S.dailyDate};
    }
    return WC.getLevel(S.level);
  };

  /* ---------- bàn chơi: mỗi đáp án một dòng, cột theo WordRegion.Load ---------- */
  var boardHTML = function (data, prog) {
    var n = data.a.length, cols = WC.columns(n), rows = Math.ceil(n / cols), maxCells = 0;
    for (var c = 0; c < cols; c++) { var m = 0; for (var r = 0; r < rows; r++) { var w = data.a[c * rows + r]; if (w && w.length > m) m = w.length; } maxCells += m; }
    var size = Math.max(14, Math.min(40, Math.floor((350 - 14 * (cols - 1)) / (maxCells * 1.1))));
    var out = '';
    data.a.forEach(function (w) {
      var shown = prog.found.indexOf(w) >= 0 ? w.length : (prog.hints[w] || 0);
      var cells = '';
      for (var i = 0; i < w.length; i++) cells += '<div class="cell on' + (i < shown ? ' open' + (prog.found.indexOf(w) < 0 ? ' hint' : '') : '') + '" style="width:' + size + 'px;height:' + size + 'px;font-size:' + Math.round(size * .55) + 'px">' + (i < shown ? w[i].toUpperCase() : '') + '</div>';
      out += '<div class="line">' + cells + '</div>';
    });
    return '<div class="lines" style="grid-template-rows:repeat(' + rows + ',auto)">' + out + '</div>';
  };
  var wheelHTML = function (letters) {
    var ltr = '', r = letters.length > 6 ? 90 : 84;
    letters.forEach(function (l, i) {
      var a = i / letters.length * Math.PI * 2 - Math.PI / 2;
      ltr += '<div class="ltr" data-i="' + i + '" style="left:' + (130 + Math.cos(a) * r) + 'px;top:' + (130 + Math.sin(a) * r) + 'px">' + l.toUpperCase() + '</div>';
    });
    return '<div class="wheel" data-wheel><svg viewBox="0 0 260 260"><polyline data-line fill="none" stroke="#ffc93c" stroke-width="10" stroke-linecap="round" stroke-linejoin="round" points=""/></svg>' + ltr + '</div>';
  };
  var order = function (word, salt) {
    return word.split('').map(function (c, i) { return {c: c, k: WC.hash(salt + c + i)}; }).sort(function (a, b) { return a.k - b.k; }).map(function (o) { return o.c; });
  };

  /* ---------- views ---------- */
  var views = {};
  views.home = function () {
    var cur = WC.current(), dailyOpen = !S.daily.solved[S.daily.today];
    return '<div class="demo-tag">' + t('brand.sub') + '</div><div class="home"><div class="logo"><div class="tiles"><div class="tile">W</div><div class="tile c">O</div><div class="tile k">R</div><div class="tile">D</div></div><h1>WORD<br>CROSS</h1></div>' +
      '<div class="actions"><a class="btn coral xl" href="' + go('select-world') + '">' + t('home.play') + '</a>' +
      '<div class="resume">' + t('home.resume', {s: cur.s + 1, l: cur.l + 1}) + '</div>' +
      '<div class="row"><a class="btn yellow" href="' + go('daily') + '">' + t('home.daily') + (dailyOpen ? '<i class="dot"></i>' : '') + '</a><a class="btn" href="' + go('bot-setup') + '">' + t('home.bot') + '</a></div>' +
      '<div class="row"><a class="btn ghost" href="' + go('ai-soon') + '">' + t('home.ai') + ' <em class="soon">' + t('common.soon') + '</em></a><a class="btn ghost" href="' + go('shop') + '">' + t('home.shop') + '</a></div>' +
      '<div class="row"><a class="btn ghost sm" href="' + go('language') + '">' + ic.globe + t('home.lang') + '</a><a class="btn ghost sm" href="' + go('rate') + '">' + t('home.rate') + '</a></div></div></div>';
  };
  views['select-world'] = function () {
    var cur = WC.current(), pages = '', dots = '';
    for (var w = 0; w < 6; w++) {
      var rows = '';
      for (var s = 0; s < 5; s++) {
        var total = WC.subTotal(w, s), done = WC.subCleared(w, s), open = w * 5 + s <= cur.w * 5 + cur.s;
        rows += '<a class="sub' + (open ? '' : ' locked') + '" ' + (open ? 'data-sel="' + w + ',' + s + '" href="#"' : 'aria-disabled="true" aria-label="' + t('world.locked') + '"') + '><div class="num">' + (s + 1) + '</div><div class="meta"><b>' + t('world.sub', {n: s + 1}) + '</b><div class="bar"><i style="width:' + Math.round(done / total * 100) + '%"></i></div></div><span class="cnt">' + done + '/' + total + '</span></a>';
      }
      pages += '<div class="world-page"><div class="world-head"><h2>' + t('world.name', {n: w + 1}) + '</h2><span>' + t('world.swipe') + '</span></div>' + rows + '</div>';
      dots += '<i' + (w === 0 ? ' class="on"' : '') + '></i>';
    }
    return bar(t('world.title'), 'home') + '<div class="world-wrap"><div class="world-scroll" data-worlds>' + pages + '</div><div class="dots">' + dots + '</div></div>';
  };
  views['select-level'] = function () {
    var sel = S.sel || {w: WC.current().w, s: WC.current().s}, total = WC.subTotal(sel.w, sel.s), cur = WC.current(), cells = '';
    var curIdx = sel.w === cur.w && sel.s === cur.s ? cur.l : (WC.subCleared(sel.w, sel.s) >= total ? -1 : 0);
    for (var i = 0; i < total; i++) {
      var done = !!S.cleared[sel.w + '-' + sel.s + '-' + i], isCur = i === curIdx, cls = done ? 'done' : isCur ? 'cur' : 'lock';
      cells += '<a class="lvl ' + cls + '" aria-label="' + t(done ? 'lvl.done' : isCur ? 'lvl.cur' : 'lvl.locked', {n: i + 1}) + '" ' + (cls !== 'lock' ? 'data-lvl="' + sel.w + ',' + sel.s + ',' + i + '" href="#"' : '') + '>' + (i + 1) + '</a>';
    }
    var sample = WC.getLevel({w: sel.w, s: sel.s, l: total - 1}).sample;
    return bar(t('lvl.title', {n: sel.s + 1}), 'select-world') + '<div class="lvl-grid">' + cells + '</div>' + (sample ? '<div class="note pad">' + tag('tag.sample') + ' ' + t('tag.sampleLevel') + '</div>' : '');
  };

  views.game = function () {
    var L = ctxLevel(), prog = WC.progressOf(L.key), data = L.data;
    var first = !L.daily && S.level.w === 0 && S.level.s === 0 && S.level.l === 0 && !S.cleared['0-0-0'];
    var name = L.daily ? t('game.dailyName', {d: fmtShort(L.daily)}) : t('world.sub', {n: S.level.s + 1}) + ' - ' + (S.level.l + 1);
    var sub = L.daily ? '#' + WC.dailyNo(L.daily) : t('common.level', {n: S.level.l + 1});
    var coach = first ? '<div class="coach">' + t('game.coach1') + '<br>' + t('game.coach2') + '</div>' : (L.daily ? '<div class="coach">' + t('game.coachDaily', {n: data.a.length}) + '</div>' : '');
    prog.order = prog.order || order(data.w, L.key);
    return '<div class="game-top"><button class="icon-btn" aria-label="' + t('game.pause') + '" data-open="pause">' + ic.pause + '</button><div class="lvl-name">' + name + '<small>' + sub + (L.sample ? ' · ' + t('tag.sample') : '') + '</small></div>' + rubyPill() + '</div>' +
      '<div class="board"><div data-board>' + boardHTML(data, prog) + '</div>' + coach + '<div class="preview" data-preview></div><div class="msg" data-msg></div></div>' +
      '<div class="tools"><div class="tool"><button class="icon-btn" aria-label="' + t('game.hint') + '" title="' + t('game.hintFree') + '" data-hint>' + ic.hint + '</button><span class="cost">' + ruby() + E.hint + '</span></div>' +
      '<div class="tool"><button class="icon-btn" aria-label="' + t('game.shuffle') + '" data-shuffle>' + ic.shuffle + '</button>' + t('game.shuffle') + '</div>' +
      '<div class="tool"><button class="icon-btn extra-btn" aria-label="' + t('game.extra') + '" data-open="extra-word">' + ic.book + '<em data-extra>' + S.extra.progress + '</em></button>' + t('game.extra') + '</div>' +
      '<div class="tool"><button class="icon-btn" aria-label="' + t('game.ask') + '" data-ask>' + ic.share + '</button>' + t('game.ask') + '</div></div>' +
      wheelHTML(prog.order) + (S.ads.removed ? '' : '<div class="banner">' + t('game.banner') + '</div>');
  };

  views.daily = function () {
    var D = S.daily, sel = S.dsel && S.dsel <= D.today ? S.dsel : D.today, vm = S.dmonth || D.today.slice(0, 7);
    var y = +vm.slice(0, 4), m = +vm.slice(5, 7), first = new Date(Date.UTC(y, m - 1, 1)), count = new Date(Date.UTC(y, m, 0)).getUTCDate(), off = (first.getUTCDay() + 6) % 7, cells = '';
    for (var i = 0; i < off; i++) cells += '<span></span>';
    for (var d = 1; d <= count; d++) {
      var ds = vm + '-' + (d < 10 ? '0' : '') + d, st = WC.dailyStatus(ds);
      cells += '<button class="day ' + st + (ds === sel ? ' selected' : '') + '" data-day="' + ds + '"' + (st === 'future' ? ' disabled' : '') + ' aria-label="' + t('daily.dayAria', {d: d, s: t('daily.s.' + st)}) + '">' + d + '</button>';
    }
    var st = WC.dailyStatus(sel), key = WC.dailyKey(sel, S.gameLang), words = window.WC_LEVELS[key].a.length, rw = WC.dailyRewardBefore(sel);
    var rewardLine = st === 'done' ? t('daily.rewardDone') : rw ? (st === 'miss' ? t('daily.backfill', {n: rw}) : t('daily.reward', {n: rw})) : t('daily.noReward');
    var label = st === 'today' ? t('daily.play') : st === 'done' ? t('daily.replay') : t('daily.playDay', {d: fmtShort(sel)});
    var str = WC.streak(), ss = WC.streakState(), nm = WC.nextMilestone(str);
    var msg = ss === 'risk' ? t('daily.streakRisk') : ss === 'lost' ? t('daily.streakLost', {n: D.best}) : '';
    var pm = new Date(Date.UTC(y, m - 2, 1)), nx = new Date(Date.UTC(y, m, 1)), ptxt = WC.fmtDate(pm).slice(0, 7), ntxt = WC.fmtDate(nx).slice(0, 7);
    return bar(t('daily.title'), 'home') + '<div class="scroll-body">' +
      '<div class="daily-hero"><div><small>' + t('daily.today', {d: fmtShort(D.today)}) + ' · ' + tag('tag.demo') + '</small><b>#' + WC.dailyNo(sel) + ' · ' + fmtShort(sel) + '</b><span>' + t('daily.goal', {n: words}) + '</span></div></div>' +
      '<div class="streak ' + ss + '"><b>' + t('daily.streak', {n: str}) + '</b><span>' + t('daily.best', {n: D.best}) + '</span>' + (nm ? '<small>' + t('daily.nextMilestone', {d: nm.d, n: nm.n}) + '</small>' : '') + (msg ? '<p>' + msg + '</p>' : '') + '</div>' +
      '<div class="month-nav"><button class="icon-btn" aria-label="' + t('daily.prev') + '" data-month="' + ptxt + '">' + ic.back + '</button><b>' + t('daily.month', {m: WC.monthName(m), y: y}) + '</b><button class="icon-btn flip" aria-label="' + t('daily.next') + '" data-month="' + ntxt + '"' + (ntxt > D.today.slice(0, 7) ? ' disabled' : '') + '>' + ic.back + '</button></div>' +
      '<div class="cal">' + WC.weekdays().map(function (x) { return '<i>' + x + '</i>'; }).join('') + cells + '</div>' +
      '<div class="note">' + t('daily.legend') + '</div><div class="reward-line">' + ruby() + rewardLine + '</div>' +
      '<a class="btn coral block" data-play-daily="' + sel + '" href="#">' + label + '</a><div class="note">' + t('daily.tz') + '</div>' +
      '<div class="demo-row"><span>' + tag('tag.demo') + ' ' + t('daily.demoClock', {d: D.today}) + '</span><button class="btn ghost sm" data-advance>' + t('daily.advance') + '</button><button class="btn ghost sm" data-resetdemo>' + t('daily.resetDemo') + '</button></div><div class="note">' + t('daily.sample') + '</div></div>';
  };

  var botBoard = function (inner) { return inner; };
  views['bot-setup'] = function () {
    var cards = ['easy', 'normal', 'hard'].map(function (d) {
      return '<button class="bot-card' + (S.bot.diff === d ? ' on' : '') + '" data-diff="' + d + '"><span class="bot-ico">' + t('bot.badge') + '</span><span class="bt"><b>' + t('bot.name.' + d) + ' · ' + t('bot.' + d) + '</b><small>' + t('bot.d.' + d, {n: E.botWin[d]}) + '</small></span></button>';
    }).join('');
    return bar(t('bot.title'), 'home') + '<div class="scroll-body"><p class="lead">' + t('bot.intro') + '</p><h4>' + t('bot.diff') + '</h4><div class="bot-cards">' + cards + '</div>' +
      '<div class="note ok">' + t('bot.fair') + '</div><div class="note">' + t('bot.rules') + '</div><div class="note">' + t('bot.sampleNote') + '</div>' +
      '<div class="reward-line">' + ruby() + t('bot.cap', {a: S.bot.day === S.daily.today ? S.bot.earned : 0, b: E.botCap}) + '</div><a class="btn coral xl block" data-botstart href="#">' + t('bot.start') + '</a></div>';
  };
  views['bot-match'] = function () {
    var d = S.bot.diff, key = WC.botDiff[d].level, data = window.WC_LEVELS[key], prog = {found: [], hints: {}}, ord = order(data.w, key + S.bot.seed);
    return '<div class="game-top"><a class="icon-btn" aria-label="' + t('bot.leave') + '" href="' + go('home') + '">' + ic.back + '</a><div class="lvl-name">' + t('bot.title') + '<small>' + t('bot.seed', {s: S.bot.seed}) + '</small></div><div class="timer" data-timer>' + t('bot.time', {n: E.botLimit}) + '</div></div>' +
      '<div class="vs"><div class="side you"><b>' + t('bot.you') + '</b><span data-ypts>' + t('bot.pts', {n: 0}) + '</span></div><div class="side bot"><b>' + t('bot.name.' + d) + ' <em>' + t('bot.badge') + '</em></b><span data-bpts>' + t('bot.pts', {n: 0}) + '</span></div></div>' +
      '<div class="board"><div data-board>' + boardHTML(data, prog) + '</div><div class="preview" data-preview></div><div class="msg" data-msg></div><div class="note" data-found>' + t('bot.found', {a: 0, b: 0}) + '</div></div>' +
      '<div class="tools solo"><div class="tool"><button class="icon-btn" aria-label="' + t('game.shuffle') + '" data-shuffle>' + ic.shuffle + '</button>' + t('game.shuffle') + '</div></div>' + wheelHTML(ord);
  };
  views.language = function () {
    var ui = WC.locales.map(function (l) { return '<button class="opt' + (WC.lang === l ? ' on' : '') + '" data-ui="' + l + '"><b>' + t('lang.ui.' + l) + '</b></button>'; }).join('');
    var rows = [['en', 'lang.playable', true], ['fr', 'lang.playable'], ['es', 'lang.playable'], ['pt', 'lang.playable'], ['vi', 'lang.playable']].map(function (r) {
      return '<div class="opt game' + (r[2] ? ' on' : '') + '"><b>' + t('lang.' + r[0]) + '</b><small>' + t(r[1]) + '</small></div>';
    }).join('');
    return bar(t('lang.title'), 'home') + '<div class="scroll-body"><h4>' + t('lang.ui') + '</h4><p class="lead">' + t('lang.uiDesc') + '</p><div class="opts two wrap">' + ui + '</div><div class="note ok" data-saved></div>' +
      '<h4>' + t('lang.game') + '</h4><p class="lead">' + t('lang.gameDesc') + '</p><div class="opts">' + rows + '</div></div>';
  };

  /* ---------- popups ---------- */
  var dlg = function (inner, closeTo) { return '<div class="scrim" data-scrim><div class="dlg">' + (closeTo ? '<a class="x" aria-label="' + t('common.close') + '" data-close href="' + closeTo + '">' + ic.close + '</a>' : '') + inner + '</div></div>'; };
  var popups = {};
  popups.pause = function () {
    return dlg('<h3>' + t('pause.title') + '</h3><div class="toggle' + (S.music ? ' on' : '') + '" data-toggle="music"><span>' + t('pause.music') + '</span><i></i></div><div class="toggle' + (S.sound ? ' on' : '') + '" data-toggle="sound"><span>' + t('pause.sound') + '</span><i></i></div>' +
      '<a class="btn block" href="' + go('game') + '">' + t('pause.continue') + '</a><a class="btn yellow block" href="' + go('how-to-play') + '">' + t('pause.how') + '</a><a class="btn ghost block" href="' + go('language') + '">' + t('pause.lang') + '</a>' +
      '<div class="pair"><a class="btn ghost" href="' + go('select-world') + '">' + t('pause.menu') + '</a><a class="btn coral" href="' + go('home') + '">' + t('pause.quit') + '</a></div>', go('game'));
  };
  popups['how-to-play'] = function () {
    return dlg('<h3>' + t('how.title') + '</h3><div class="howto"><div class="step"><div class="mini-wheel"><b>T</b><b>H</b><b>E</b></div><span>' + t('how.1') + '</span></div><div class="step"><i>2</i><span>' + t('how.2') + '</span></div><div class="step"><i>3</i><span>' + t('how.3') + '</span></div></div><a class="btn block" href="' + go('game') + '">' + t('common.gotIt') + '</a>', go('game'));
  };
  popups.win = function () {
    var c = S.winCtx || {kind: 'level', first: true, reward: E.firstClear, key: 'sample'};
    if (c.kind === 'daily') {
      var x2id = 'x2d:' + c.date, got = S.claims['ad:' + x2id];
      return dlg('<div class="badge">★</div><h3>' + t('win.dailyTitle') + '</h3><div class="big-ruby">' + ruby(1) + '+' + c.reward + '</div><p>' + t('win.dailyReward', {n: c.reward}) + '</p><div class="win-lines"><b>' + t('win.streak', {n: c.streak}) + '</b>' + (c.milestone ? '<span>' + t('win.milestone', {d: c.milestoneDays, n: c.milestone}) + '</span>' : '') + '<span>' + t('win.dailyNext') + '</span></div>' +
        (c.reward ? (got ? '<div class="note ok">' + t('win.x2done') + '</div>' : '<a class="btn yellow block" data-x2="' + x2id + ',' + c.reward + '" href="#">' + t('win.x2', {n: c.reward}) + '</a>') : '') + '<a class="btn coral block" href="' + go('daily') + '">' + t('win.toDaily') + '</a>');
    }
    var lv = S.level, total = WC.subTotal(lv.w, lv.s), left = total - lv.l - 1, last = left === 0, xid = 'x2:' + c.key, gotx = S.claims['ad:' + xid];
    var goal = last ? t('win.goalLast', {s: lv.s + 2 > 5 ? 1 : lv.s + 2}) : t('win.goal', {l: lv.l + 2, n: left});
    var extra = S.extra.progress >= S.extra.target ? t('win.extraReady', {a: S.extra.progress, b: S.extra.target}) : t('win.extraNot', {a: S.extra.progress, b: S.extra.target});
    return dlg('<div class="badge">★</div><h3>' + t('win.title') + '</h3><div class="stars"><span>★</span><span>★</span><span>★</span></div>' +
      (c.first ? '<div class="big-ruby sm">' + ruby(1) + '+' + c.reward + '</div><p>' + t('win.first', {n: c.reward}) + '</p>' : '<p>' + t('win.replay') + '</p>') +
      (c.first && c.reward ? (gotx ? '<div class="note ok">' + t('win.x2done') + '</div>' : '<a class="btn yellow block" data-x2="' + xid + ',' + c.reward + '" href="#">' + t('win.x2', {n: c.reward}) + '</a>') : '') +
      '<div class="win-lines"><b>' + goal + '</b><span>' + extra + '</span></div><a class="btn coral xl block" data-next href="#">' + t('win.next') + '</a><a class="btn ghost block" href="' + go(last ? 'select-world' : 'select-level') + '">' + t('win.menu') + '</a>');
  };
  popups['extra-word'] = function () {
    var ex = S.extra, ready = ex.progress >= ex.target, n = WC.extraClaimAmount();
    return dlg('<h3>' + t('extra.title') + '</h3><div class="book"><div class="prog-lbl"><span>' + t('extra.desc') + '</span></div><div class="prog"><i style="width:' + Math.min(100, Math.round(ex.progress / ex.target * 100)) + '%"></i></div><div class="prog-lbl"><span>' + t('extra.note') + '</span><b>' + ex.progress + '/' + ex.target + '</b></div></div>' +
      '<a class="btn block' + (ready ? '' : ' off') + '" data-claim-extra href="#">' + t('extra.claim', {n: n}) + '</a>', go('game'));
  };
  popups['low-ruby'] = function () {
    return dlg('<h3>' + t('low.title', {n: E.hint}) + '</h3><p>' + t('low.desc', {a: S.rubies}) + '</p><a class="btn yellow block" data-ad="hint,' + E.video + ',game" href="#">' + t('low.watch', {n: E.video}) + '</a><a class="btn block" href="' + go('shop') + '">' + t('low.shop') + '</a><a class="btn ghost block" href="' + go('game') + '">' + t('common.skip') + '</a>', go('game'));
  };
  var PACKS = [[50, '0.99'], [110, '1.99'], [250, '3.99'], [450, '5.99'], [1100, '11.99']]; /* số ruby/giá từ ShopDialog.prefab; ghép cặp là giả định */
  popups.shop = function () {
    var st = WC.adState(), left = WC.cooldownLeft();
    var sub = st === 'cooldown' ? t('shop.cool', {t: WC.fmtTime(left || E.videoCooldown)}) : t('shop.ready', {n: E.video});
    var list = PACKS.map(function (it, i) { return '<div class="shop-item' + (i === 4 ? ' best' : '') + '"><div class="amt">' + ruby(1) + t('shop.pack', {n: it[0]}) + '</div><a class="btn coral" data-buy href="#">' + it[1] + ' $</a></div>'; }).join('');
    return dlg('<h3>' + t('shop.title') + '</h3><div class="ruby-pill bal">' + ruby() + '<span data-bal>' + S.rubies + '</span></div><a class="free" data-ad="shop,' + E.video + ',shop" href="#">' + ruby(1) + '<span class="t">' + t('shop.free') + '<small>' + sub + '</small></span><span class="btn yellow sm">' + t('shop.watch') + '</span></a><div class="shop-list">' + list + '</div><div class="note" data-msg>' + t('shop.note') + '</div>' + (S.ads.removed ? '<div class="note ok">' + t('shop.adsOff') + '</div>' : ''), go('home'));
  };
  popups['rewarded-video'] = function () { return '<div data-rv></div>'; };
  popups.rate = function () {
    return dlg('<div class="badge">★</div><h3>' + t('rate.title') + '</h3><p>' + t('rate.desc') + '</p><div class="pair"><a class="btn ghost" href="' + go('home') + '">' + t('rate.later') + '</a><a class="btn coral" href="' + go('home') + '">' + t('rate.yes') + '</a></div>');
  };
  popups['ai-soon'] = function () {
    return dlg('<div class="badge">AI</div><h3>' + t('ai.title') + '</h3><div class="soon-pill">' + t('ai.soon') + '</div><p>' + t('ai.desc') + '</p><p>' + t('ai.note') + '</p><a class="btn block off" aria-disabled="true" href="#">' + t('ai.soon') + '</a><a class="btn block" href="' + go('bot-setup') + '">' + t('ai.toBot') + '</a>', go('home'));
  };
  popups['bot-result'] = function () {
    var L = S.bot.last || {result: 'win', player: 12, bot: 8, reward: E.botWin.normal, diff: 'normal', seed: 1, sample: true};
    return dlg('<div class="badge">' + (L.result === 'win' ? '★' : L.result === 'draw' ? '=' : 'B') + '</div><h3>' + t('bot.' + L.result) + '</h3><div class="vs res"><div class="side you"><b>' + t('bot.you') + '</b><span>' + t('bot.pts', {n: L.player}) + '</span></div><div class="side bot"><b>' + t('bot.name.' + L.diff) + ' <em>' + t('bot.badge') + '</em></b><span>' + t('bot.pts', {n: L.bot}) + '</span></div></div>' +
      '<p>' + (L.reward ? t('bot.reward', {n: L.reward}) : t('bot.noReward')) + '</p><div class="note">' + t('bot.seed', {s: L.seed}) + '</div><a class="btn coral block" data-botreplay href="#">' + t('bot.replay') + '</a><div class="pair"><a class="btn" data-botnew href="#">' + t('bot.new') + '</a><a class="btn ghost" href="' + go('home') + '">' + t('pause.menu') + '</a></div>');
  };

  /* ---------- render ---------- */
  var s = find(page), html = '';
  if (s) {
    var base = s.base;
    if (s.id === 'rewarded-video' && !WC.embed) base = {shop: 'home', game: 'game', win: 'game', daily: 'daily'}[S.ads.ctx.back] || 'home';
    html = s.kind === 'screen' ? views[s.id]() : views[base]() + popups[s.id]();
  }
  if (WC.embed) document.body.classList.add('embed');
  root.innerHTML = '<div class="phone">' + html + '</div>';
  var phone = root.firstChild;
  var $ = function (sel) { return phone.querySelector(sel); };
  var $$ = function (sel) { return [].slice.call(phone.querySelectorAll(sel)); };
  var on = function (sel, ev, fn) { $$(sel).forEach(function (e) { e.addEventListener(ev, function (x) { x.preventDefault(); fn(e, x); }); }); };
  var bals = function () { $$('[data-bal]').forEach(function (e) { e.textContent = S.rubies; }); };

  $$('[data-toggle]').forEach(function (tg) { tg.addEventListener('click', function () { var k = tg.dataset.toggle; S[k] = !S[k]; WC.save(); tg.classList.toggle('on', S[k]); }); });
  $$('[data-close]').forEach(function (c) { c.addEventListener('click', function () { if (WC.embed) { var sc = $('[data-scrim]'); if (sc) sc.classList.add('hide'); } }); });
  on('[data-open]', 'click', function (b) { nav(b.dataset.open); });

  var worlds = $('[data-worlds]');
  if (worlds) worlds.addEventListener('scroll', function () { var i = Math.round(worlds.scrollLeft / worlds.clientWidth); $$('.dots i').forEach(function (d, k) { d.classList.toggle('on', k === i); }); });
  on('[data-sel]', 'click', function (a) { var p = a.dataset.sel.split(','); S.sel = {w: +p[0], s: +p[1]}; WC.save(); nav('select-level'); });
  on('[data-lvl]', 'click', function (a) {
    var p = a.dataset.lvl.split(',').map(Number); S.level = {w: p[0], s: p[1], l: p[2]}; S.mode = 'level';
    if (S.cleared[WC.key(S.level)]) { var pk = WC.getLevel(S.level).key, old = S.progress[pk]; S.progress[pk] = {hints: {}, found: [], extras: old ? old.extras : []}; } /* chơi lại: bàn mới, giữ sổ extra đã tính */
    WC.save(); nav('game');
  });

  /* ---- shop / ads ---- */
  on('[data-buy]', 'click', function () { var m = $('[data-msg]'); if (m) m.textContent = t('shop.noCheckout'); });
  on('[data-ad]', 'click', function (a) {
    var p = a.dataset.ad.split(','); S.ads.ctx = {placement: p[0], amount: +p[1], back: p[2]}; WC.save(); nav('rewarded-video');
  });
  on('[data-x2]', 'click', function (a) {
    var p = a.dataset.x2.split(','); if (S.claims['ad:' + p[0]]) { location.reload(); return; }
     S.ads.ctx = {placement: 'x2', amount: +p[1], back: S.winCtx && S.winCtx.kind === 'daily' ? 'win' : 'win', id: p[0]}; WC.save(); nav('rewarded-video');
  });
  var rv = $('[data-rv]');
  if (rv) {
    var ctx = S.ads.ctx, shown = WC.adState(), sid = null, left = 3;
    var chips = ['loading', 'ready', 'cooldown', 'nofill', 'offline'].map(function (k) { return '<button class="chip2' + (S.ads.forced === k ? ' on' : '') + '" data-force="' + k + '">' + t('rv.st.' + k) + '</button>'; }).join('');
    var outs = ['success', 'cancel', 'error'].map(function (k) { return '<button class="chip2' + (S.ads.outcome === k ? ' on' : '') + '" data-out="' + k + '">' + t('rv.out.' + k) + '</button>'; }).join('');
    var back = function () { nav(ctx.back === 'win' ? 'win' : ctx.back); };
    var draw = function () {
      var st = shown, body, btns;
      var key = {nofill: 'nofill'}[st] || st;
      var title = t('rv.s.' + key, {t: WC.fmtTime(WC.cooldownLeft() || E.videoCooldown)}), desc = t('rv.d.' + key, {n: st === 'playing' ? left : ctx.amount});
      if (st === 'ready') { title = t('rv.title'); desc = t('rv.value'); }
      btns = '';
      if (st === 'ready') btns = '<a class="btn coral block" data-watch href="#">' + t('rv.watch') + '</a><a class="btn ghost block" data-back href="#">' + t('rv.no') + '</a>';
      else if (st === 'playing') btns = '<a class="btn ghost block" data-cancelad href="#">' + t('rv.closeAd') + '</a>';
      else if (st === 'cancel' || st === 'error') btns = '<a class="btn coral block" data-watch href="#">' + t('common.retry') + '</a><a class="btn ghost block" data-back href="#">' + t('common.ok') + '</a>';
      else btns = '<a class="btn block" data-back href="#">' + (st === 'success' ? t('common.continue') : t('common.ok')) + '</a>';
      var amt = st === 'ready' || st === 'success' ? '<div class="big-ruby">' + ruby(1) + '+' + ctx.amount + '</div>' : st === 'loading' ? '<div class="spin"></div>' : st === 'playing' ? '<div class="adbox">' + t('rv.s.playing') + '</div>' : '';
      rv.innerHTML = dlg('<h3>' + title + '</h3>' + amt + '<p>' + desc + '</p>' + btns + '<div class="demo-ads"><small>' + t('rv.demo') + '</small><div class="chips2">' + chips + '</div><small>' + t('rv.out') + '</small><div class="chips2">' + outs + '</div></div>', '#');
      on('[data-back],[data-close]', 'click', back);
      on('[data-force]', 'click', function (b) { S.ads.forced = b.dataset.force; WC.save(); shown = WC.adState(); draw(); });
      on('[data-out]', 'click', function (b) { S.ads.outcome = b.dataset.out; WC.save(); draw(); });
      on('[data-watch]', 'click', function () { if (WC.adState() !== 'ready' && shown === 'ready') { shown = WC.adState(); draw(); return; } sid = sid || (ctx.id || 'shop:' + Date.now()); shown = 'playing'; left = 3; draw(); tick(); });
      on('[data-cancelad]', 'click', function () { left = -1; shown = 'cancel'; draw(); });
    };
    /* phát quảng cáo MÔ PHỎNG 3s; thưởng chỉ khi chạy hết (callback hoàn thành), đóng sớm/lỗi thì không thưởng */
    var tick = function () {
      setTimeout(function () {
        if (shown !== 'playing') return;
        left--;
        if (left > 0) { draw(); tick(); return; }
        if (S.ads.outcome === 'success') shown = WC.adComplete(sid, ctx.amount) ? 'success' : 'error';
        else shown = S.ads.outcome;
        draw(); bals();
      }, 1000);
    };
    draw();
  }

  /* ---- extra / win ---- */
  on('[data-claim-extra]', 'click', function () { if (WC.claimExtra()) location.reload(); });
  on('[data-next]', 'click', function () {
    var lv = S.level, total = WC.subTotal(lv.w, lv.s);
    if (lv.l + 1 < total) { S.level = {w: lv.w, s: lv.s, l: lv.l + 1}; S.mode = 'level'; WC.save(); nav('game'); }
    else { var ns = lv.s + 1; if (ns < 5) S.level = {w: lv.w, s: ns, l: 0}; else if (lv.w < 5) S.level = {w: lv.w + 1, s: 0, l: 0}; S.sel = {w: S.level.w, s: S.level.s}; WC.save(); nav('select-world'); }
  });

  /* ---- language ---- */
  on('[data-ui]', 'click', function (b) { WC.setLang(b.dataset.ui); location.reload(); });

  /* ---- Daily ---- */
  on('[data-day]', 'click', function (b) { S.dsel = b.dataset.day; WC.save(); location.reload(); });
  on('[data-month]', 'click', function (b) { S.dmonth = b.dataset.month; WC.save(); location.reload(); });
  on('[data-play-daily]', 'click', function (b) {
    var d = b.dataset.playDaily; S.mode = 'daily'; S.dailyDate = d;
    if (S.daily.solved[d]) { var olde = S.progress['daily:' + d]; S.progress['daily:' + d] = {hints: {}, found: [], extras: olde ? olde.extras : []}; } /* giữ sổ extra đã tính */
    WC.save(); nav('game');
  });
  on('[data-advance]', 'click', function () { WC.advanceDay(); S.dsel = null; S.dmonth = null; WC.save(); location.reload(); });
  on('[data-resetdemo]', 'click', function () { var l = S.uiLang; WC.reset(); S.uiLang = l; WC.save(); location.reload(); });

  /* ---- Bot ---- */
  on('[data-diff]', 'click', function (b) { S.bot.diff = b.dataset.diff; WC.save(); location.reload(); });
  on('[data-botstart]', 'click', function () { S.bot.seed++; WC.save(); nav('bot-match'); });
  on('[data-botreplay]', 'click', function () { nav('bot-match'); });
  on('[data-botnew]', 'click', function () { S.bot.seed++; WC.save(); nav('bot-match'); });

  /* ---------- wheel + chơi (dùng chung Gameplay và Bot-match) ---------- */
  var wheel = $('[data-wheel]');
  if (wheel && (s.id === 'game' || s.id === 'bot-match')) {
    var isBot = s.id === 'bot-match', L = isBot ? null : ctxLevel(), botKey = isBot ? WC.botDiff[S.bot.diff].level : null;
    var data = isBot ? window.WC_LEVELS[botKey] : L.data;
    var prog = isBot ? {hints: {}, found: [], extras: []} : WC.progressOf(L.key);
    var letters = isBot ? order(data.w, botKey + S.bot.seed) : prog.order;
    var picked = [], dragging = false, done = false, times = [], simT = 0;
    var preview = $('[data-preview]'), msg = $('[data-msg]'), line = $('[data-line]'), ltrEls = $$('.ltr'), board = $('[data-board]');
    var say = function (txt, ok) { msg.textContent = txt; msg.className = 'msg' + (ok ? ' ok' : ''); };
    var raw = function (k, p) { return t(k, p).replace(/&amp;/g, '&').replace(/&lt;/g, '<').replace(/&gt;/g, '>').replace(/&quot;/g, '"'); };
    var redraw = function () { board.innerHTML = boardHTML(data, prog); };
    var draw = function () {
      preview.innerHTML = picked.map(function (i) { return '<b>' + letters[i].toUpperCase() + '</b>'; }).join('');
      ltrEls.forEach(function (l, i) { l.classList.toggle('sel', picked.indexOf(i) >= 0); });
      line.setAttribute('points', picked.map(function (i) { var l = ltrEls[i]; return parseFloat(l.style.left) + ',' + parseFloat(l.style.top); }).join(' '));
    };
    var win = function () {
      if (done) return; done = true;
      if (isBot) return;
      var ctxw = L.daily ? WC.completeDaily(L.daily) : WC.completeLevel(S.level);
      bals();
      setTimeout(function () { nav('win'); }, 900);
    };
    var botEnd = function (endT) {
      if (done) return; done = true;
      var plan = WC.botPlan(botKey, S.bot.diff, S.bot.seed), out = WC.botOutcome(botKey, plan, prog.found, times, endT);
      var seedKey = S.bot.diff + '|' + S.bot.seed, reward = WC.botReward(out.result, S.bot.diff, seedKey);
      S.bot.last = {result: out.result, player: out.player, bot: out.bot, reward: reward, diff: S.bot.diff, seed: S.bot.seed}; WC.save();
      setTimeout(function () { nav('bot-result'); }, 700);
    };
    var finish = function () {
      var w = WC.normalize(picked.map(function (i) { return letters[i]; }).join('')), kind = w.length > 1 ? WC.classify(data, w) : 'short';
      if (kind === 'answer' && prog.found.indexOf(w) < 0) {
        prog.found.push(w); times.push(simT); if (!isBot) WC.save(); redraw(); say(raw('game.nice', {w: w.toUpperCase()}), true);
        if (isBot) { $('[data-ypts]').textContent = raw('bot.pts', {n: prog.found.reduce(function (a, x) { return a + x.length; }, 0)}); $('[data-found]').textContent = raw('bot.found', {a: prog.found.length, b: WC.botFoundAt(WC.botPlan(botKey, S.bot.diff, S.bot.seed), simT).length}); if (prog.found.length === data.a.length) botEnd(simT); }
        else if (prog.found.length === data.a.length) win();
      } else if (kind === 'answer') say(raw('game.already'), false);
      else if (kind === 'extra') {
        if (isBot) say(raw('game.extraFound', {w: w.toUpperCase()}), true);
        else if (prog.extras.indexOf(w) < 0) { prog.extras.push(w); WC.addExtra(); $('[data-extra]').textContent = S.extra.progress; say(raw('game.extraFound', {w: w.toUpperCase()}), true); }
        else say(raw('game.extraAgain'), false);
      } else if (kind === 'reject') say(raw('game.notIn'), false);
      picked = []; draw();
    };
    var hit = function (e) {
      var el = document.elementFromPoint(e.clientX, e.clientY), i = el && el.dataset ? el.dataset.i : undefined;
      if (i === undefined) return; i = +i;
      var at = picked.indexOf(i);
      if (at < 0) { picked.push(i); draw(); } else if (at === picked.length - 2) { picked.pop(); draw(); }
    };
    wheel.addEventListener('pointerdown', function (e) { dragging = true; say('', false); hit(e); });
    window.addEventListener('pointermove', function (e) { if (dragging) hit(e); });
    window.addEventListener('pointerup', function () { if (dragging) { dragging = false; finish(); } });
    var shuffles = 0;
    $('[data-shuffle]').addEventListener('click', function () {
      shuffles++; letters = order(letters.join(''), 'sh' + shuffles + Date.now()); prog.order = isBot ? undefined : letters;
      ltrEls.forEach(function (l, i) { l.textContent = letters[i].toUpperCase(); }); picked = []; draw();
    });
    if (!isBot) {
      $('[data-hint]').addEventListener('click', function () {
        var target = data.a.filter(function (w) { return prog.found.indexOf(w) < 0; })[0];
        if (!target) return;
        if (S.rubies < E.hint) { nav('low-ruby'); return; }
        WC.debit(E.hint);
        prog.hints[target] = (prog.hints[target] || 0) + 1; /* như LineWord.ShowHint: mở 1 ô */
        if (prog.hints[target] >= target.length) { prog.found.push(target); delete prog.hints[target]; }
        WC.save(); redraw(); bals();
        if (prog.found.length === data.a.length) win();
      });
      $('[data-ask]').addEventListener('click', function () { say(raw('game.ask.msg'), true); });
    } else {
      var plan = WC.botPlan(botKey, S.bot.diff, S.bot.seed);
      /* đồng hồ mô phỏng: 1 tick 250ms = 0.25s thời gian trận (90s mô phỏng = 90s thực) (không phụ thuộc đồng hồ máy) → cùng seed cho cùng timeline */
      var tickBot = function () {
        setTimeout(function () {
          if (done) return;
          simT += 0.25;
          var bp = WC.botScoreAt(plan, simT), bf = WC.botFoundAt(plan, simT).length;
          $('[data-bpts]').textContent = raw('bot.pts', {n: bp});
          $('[data-timer]').textContent = raw('bot.time', {n: Math.max(0, Math.ceil(E.botLimit - simT))});
          $('[data-found]').textContent = raw('bot.found', {a: prog.found.length, b: bf});
          if (bf === data.a.length || simT >= E.botLimit) { botEnd(simT); return; }
          tickBot();
        }, 250);
      };
      if (!WC.embed) tickBot();
    }
  }
})();
