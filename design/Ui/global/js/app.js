(function(){
  var screens = window.WC_SCREENS;
  var page = document.body.dataset.page;
  var root = document.getElementById('app');
  var rubies = 12; /* demo: Header.prefab đặt 12 */
  var HINT_COST = 2; /* Const.HINT_COST */

  var svg = function(p){ return '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.4" stroke-linecap="round" stroke-linejoin="round">'+p+'</svg>'; };
  var ic = {
    back: svg('<path d="m15 5-7 7 7 7"/>'), pause: svg('<path d="M9 5v14M15 5v14"/>'),
    shuffle: svg('<path d="M3 7h4l10 10h4M3 17h4l3-3M14 10l3-3h4M18 4l3 3-3 3M18 14l3 3-3 3"/>'),
    hint: svg('<path d="M9 18h6M10 21h4M12 3a6 6 0 0 0-4 9.5c.6.6 1 1.4 1 2.5h6c0-1.1.4-1.9 1-2.5A6 6 0 0 0 12 3Z"/>'),
    share: svg('<circle cx="6" cy="12" r="2.5"/><circle cx="18" cy="6" r="2.5"/><circle cx="18" cy="18" r="2.5"/><path d="m8.2 11 7.6-4M8.2 13l7.6 4"/>'),
    book: svg('<path d="M4 5a2 2 0 0 1 2-2h13v16H6a2 2 0 0 0-2 2V5Z"/><path d="M8 7h7"/>'),
    close: svg('<path d="M6 6l12 12M18 6 6 18"/>')
  };
  var find = function(id){ return screens.filter(function(s){ return s.id === id; })[0]; };
  var go = function(id){ return page ? '../'+id+'/index.html' : 'screens/'+id+'/index.html'; };
  var rubyPill = function(){ return '<div class="ruby-pill"><span class="ruby"></span><span data-bal>'+rubies+'</span><a class="plus" href="'+go('shop')+'">+</a></div>'; };
  var bar = function(title, backTo){
    return '<div class="hdr"><a class="icon-btn" href="'+go(backTo)+'">'+ic.back+'</a><div class="title">'+title+'</div>'+rubyPill()+'</div>';
  };

  /* ---------- screens ---------- */
  var views = {};
  views.home = function(){
    return '<div class="demo-tag">DEMO · tên hiển thị chỉnh theo bản chính</div><div class="home"><div class="logo"><div class="tiles"><div class="tile">W</div><div class="tile c">O</div><div class="tile k">R</div><div class="tile">D</div></div><h1>WORD<br>CROSS</h1></div>'+
      '<div class="actions"><a class="btn coral xl" href="'+go('select-world')+'">PLAY</a><div class="row"><a class="btn yellow" href="'+go('shop')+'">SHOP</a><a class="btn ghost" href="'+go('rate')+'">RATE</a></div></div></div>';
  };
  views['select-world'] = function(){
    var names = ['Subworld 1','Subworld 2','Subworld 3','Subworld 4','Subworld 5']; /* DEMO: tên subworld lấy từ WorldItem.subWorldName */
    var counts = [[18,18],[7,18],[0,18],[0,18],[0,18]];
    var pages = '', dots = '';
    for (var w = 0; w < 6; w++) {
      var rows = '';
      for (var s = 0; s < 5; s++) {
        var c = w === 0 ? (s === 0 ? [18,18] : s === 1 ? [7,18] : [0,18]) : [0,18];
        var locked = w > 0 || s > 1;
        var link = locked ? '#' : go('select-level');
        rows += '<a class="sub'+(locked?' locked':'')+'" href="'+link+'"><div class="num">'+(s+1)+'</div><div class="meta"><b>'+names[s]+'</b><div class="bar"><i style="width:'+Math.round(c[0]/c[1]*100)+'%"></i></div></div><span class="cnt">'+c[0]+'/'+c[1]+'</span></a>';
      }
      pages += '<div class="world-page"><div class="world-head"><h2>World '+(w+1)+'</h2><span>vuốt để đổi</span></div>'+rows+'</div>';
      dots += '<i'+(w===0?' class="on"':'')+'></i>';
    }
    return bar('Chọn World','home')+'<div class="world-wrap"><div class="world-scroll" data-worlds>'+pages+'</div><div class="dots" data-dots>'+dots+'</div></div>';
  };
  views['select-level'] = function(){
    var cells = '';
    for (var i = 1; i <= 18; i++) {
      var cls = i < 7 ? 'done' : i === 7 ? 'cur' : 'lock';
      cells += '<a class="lvl '+cls+'" href="'+(i<=7?go('game'):'#')+'">'+i+'</a>';
    }
    return bar('Subworld 2','select-world')+'<div class="lvl-grid">'+cells+'</div>';
  };

  /* demo level: chữ C A R T; từ trong lưới + 1 từ thưởng */
  var LEVEL = {letters:['C','A','R','T'], words:[
    {w:'CART', cells:[[1,0],[1,1],[1,2],[1,3]]},
    {w:'CAT',  cells:[[1,0],[2,0],[3,0]]},
    {w:'TAR',  cells:[[1,3],[2,3],[3,3]]}], extra:['ACT','RAT','ART']};
  views.game = function(){
    var grid = '';
    var map = {};
    LEVEL.words.forEach(function(wd){ wd.cells.forEach(function(p,i){ map[p[0]+','+p[1]] = wd.w[i]; }); });
    for (var r = 0; r < 5; r++) for (var c = 0; c < 4; c++) {
      var k = r+','+c;
      grid += map[k] ? '<div class="cell on" data-k="'+k+'" data-l="'+map[k]+'"></div>' : '<div class="cell"></div>';
    }
    var ltr = '';
    LEVEL.letters.forEach(function(l,i){
      var a = i / LEVEL.letters.length * Math.PI * 2 - Math.PI/2;
      ltr += '<div class="ltr" data-i="'+i+'" style="left:'+(130+Math.cos(a)*84)+'px;top:'+(130+Math.sin(a)*84)+'px">'+l+'</div>';
    });
    return '<div class="game-top"><button class="icon-btn" data-open="pause">'+ic.pause+'</button><div class="lvl-name">Subworld 2 - 8<small>Level 8</small></div>'+rubyPill()+'</div>'+
      '<div class="board"><div class="grid" style="grid-template-columns:repeat(4,44px)">'+grid+'</div><div class="preview" data-preview></div><div class="msg" data-msg></div></div>'+
      '<div class="tools"><div class="tool"><button class="icon-btn" data-hint>'+ic.hint+'</button><span class="cost"><span class="ruby"></span>'+HINT_COST+'</span></div>'+
      '<div class="tool"><button class="icon-btn" data-shuffle>'+ic.shuffle+'</button>Shuffle</div>'+
      '<div class="tool"><button class="icon-btn extra-btn" data-open="extra-word">'+ic.book+'<em data-extra>1</em></button>Extra</div>'+
      '<div class="tool"><button class="icon-btn" data-ask>'+ic.share+'</button>Ask friend</div></div>'+
      '<div class="wheel" data-wheel><svg viewBox="0 0 260 260"><polyline data-line fill="none" stroke="#ffc93c" stroke-width="10" stroke-linecap="round" stroke-linejoin="round" points=""/></svg>'+ltr+'</div>'+
      '<div class="banner">BANNER AD (placeholder)</div>';
  };

  /* ---------- popups ---------- */
  var dlg = function(inner, closeTo){ return '<div class="scrim" data-scrim><div class="dlg">'+(closeTo?'<a class="x" data-close href="'+closeTo+'">'+ic.close+'</a>':'')+inner+'</div></div>'; };
  var popups = {};
  popups.pause = function(){
    return dlg('<h3>PAUSE</h3><div class="toggle on" data-toggle><span>Music</span><i></i></div><div class="toggle on" data-toggle><span>Sound</span><i></i></div>'+
      '<a class="btn block" href="#" data-close>CONTINUE</a><a class="btn yellow block" href="'+go('how-to-play')+'">HOW TO PLAY</a>'+
      '<div class="pair"><a class="btn ghost" href="'+go('select-world')+'">TO MENU</a><a class="btn coral" href="'+go('home')+'">QUIT</a></div>', '#');
  };
  popups['how-to-play'] = function(){
    return dlg('<h3>HOW TO PLAY ?</h3><div class="howto"><div class="step"><div class="mini-wheel"><b>C</b><b>A</b><b>T</b></div><span>Swipe to build a valid word.</span></div>'+
      '<div class="step"><i>2</i><span>Use "Shuffle" or "Hint" if you are stuck. You can also take screenshot and ask friends.</span></div></div><a class="btn block" href="#" data-close>GOT IT</a>', '#');
  };
  popups.win = function(){
    return dlg('<div class="badge">★</div><h3>LEVEL COMPLETE!</h3><div class="stars"><span>★</span><span>★</span><span>★</span></div><a class="btn coral xl block" href="'+go('game')+'">NEXT</a><a class="btn ghost block" href="'+go('select-level')+'">MENU</a>');
  };
  popups['extra-word'] = function(){
    return dlg('<h3>EXTRA WORDS</h3><div class="book"><div class="prog-lbl"><span>Find more and more new words to collect rubies</span></div><div class="prog"><i style="width:10%"></i></div><div class="prog-lbl"><span></span><b>1/10</b></div></div><a class="btn off block" href="#">CLAIM</a>', '#');
  };
  popups.shop = function(){
    var items = [[50,'0.99'],[110,'1.99'],[250,'3.99'],[450,'5.99'],[1100,'11.99']]; /* số ruby/giá từ ShopDialog.prefab; ghép cặp theo thứ tự là giả định */
    var list = items.map(function(it,i){ return '<div class="shop-item'+(i===4?' best':'')+'"><div class="amt"><span class="ruby lg"></span>'+it[0]+'</div><a class="btn coral" href="#">'+it[1]+' $</a></div>'; }).join('');
    return dlg('<h3>SHOP</h3><div class="ruby-pill bal"><span class="ruby"></span>'+rubies+'</div><a class="free" href="'+go('rewarded-video')+'"><span class="ruby lg"></span><span class="t">FREE RUBIES<small>Ad available in 01:59</small></span><span class="btn yellow" style="min-height:40px;padding:0 14px;font-size:14px">WATCH</span></a><div class="shop-list">'+list+'</div><div class="note">Buy any items will remove all ads · giá demo</div>', go('home'));
  };
  popups['rewarded-video'] = function(){
    return dlg('<h3>CONGRATULATION!</h3><div class="big-ruby"><span class="ruby lg"></span>2</div><p>Congratulation! You got 2 free rubies.</p><a class="btn coral block" href="'+go('shop')+'">CLAIM</a>');
  };
  popups.rate = function(){
    return dlg('<div class="badge">★</div><h3>ENJOYING THE GAME?</h3><p>If you love the game, Please rate us 5 stars in Google Play store</p><div class="pair"><a class="btn ghost" href="'+go('home')+'">LATER</a><a class="btn coral" href="'+go('home')+'">YES</a></div>');
  };

  /* ---------- render ---------- */
  var s = find(page);
  var html = '';
  if (s) {
    if (s.kind === 'screen') html = views[s.id]();
    else html = views[s.base]() + popups[s.id]();
  }
  if (new URLSearchParams(location.search).get('embed')) document.body.classList.add('embed');
  root.innerHTML = '<div class="phone">'+html+'</div>';

  /* ---------- prototype interactions ---------- */
  var phone = root.firstChild;
  var bals = function(){ [].forEach.call(phone.querySelectorAll('[data-bal]'), function(e){ e.textContent = rubies; }); };
  [].forEach.call(phone.querySelectorAll('[data-toggle]'), function(t){ t.addEventListener('click', function(){ t.classList.toggle('on'); }); });
  [].forEach.call(phone.querySelectorAll('[data-close]'), function(c){ c.addEventListener('click', function(e){ var sc = phone.querySelector('[data-scrim]'); if (sc && c.getAttribute('href') === '#') { e.preventDefault(); if (s.kind === 'popup' && s.base === 'game') { location.href = go('game'); } else { sc.classList.add('hide'); } } }); });

  var worlds = phone.querySelector('[data-worlds]');
  if (worlds) worlds.addEventListener('scroll', function(){
    var i = Math.round(worlds.scrollLeft / worlds.clientWidth);
    [].forEach.call(phone.querySelectorAll('[data-dots] i'), function(d,k){ d.classList.toggle('on', k === i); });
  });

  var wheel = phone.querySelector('[data-wheel]');
  if (wheel && s.id === 'game') {
    var found = {}, extraFound = {}, picked = [], dragging = false;
    var preview = phone.querySelector('[data-preview]'), msg = phone.querySelector('[data-msg]');
    var line = phone.querySelector('[data-line]'), extraBadge = phone.querySelector('[data-extra]');
    var letters = [].slice.call(wheel.querySelectorAll('.ltr'));
    var say = function(t, ok){ msg.textContent = t; msg.className = 'msg' + (ok ? ' ok' : ''); };
    var draw = function(){
      preview.innerHTML = picked.map(function(i){ return '<b>'+LEVEL.letters[i]+'</b>'; }).join('');
      letters.forEach(function(l,i){ l.classList.toggle('sel', picked.indexOf(i) >= 0); });
      line.setAttribute('points', picked.map(function(i){ var l = letters[i]; return parseFloat(l.style.left)+','+parseFloat(l.style.top); }).join(' '));
    };
    var reveal = function(wd){
      wd.cells.forEach(function(p,i){ var c = phone.querySelector('[data-k="'+p[0]+','+p[1]+'"]'); c.textContent = wd.w[i]; c.classList.add('open','pop'); });
    };
    var finish = function(){
      var w = picked.map(function(i){ return LEVEL.letters[i]; }).join('');
      var wd = LEVEL.words.filter(function(x){ return x.w === w; })[0];
      if (wd && !found[w]) { found[w] = 1; reveal(wd); say('Nice! '+w, true);
        if (Object.keys(found).length === LEVEL.words.length) setTimeout(function(){ location.href = go('win'); }, 900);
      } else if (wd) say('Already found', false);
      else if (LEVEL.extra.indexOf(w) >= 0 && !extraFound[w]) { extraFound[w] = 1; extraBadge.textContent = 1 + Object.keys(extraFound).length; say('Extra word: '+w, true); }
      else if (w.length > 1) say('Not in list', false);
      picked = []; draw();
    };
    var hit = function(e){
      var el = document.elementFromPoint(e.clientX, e.clientY);
      var i = el && el.dataset ? el.dataset.i : undefined;
      if (i === undefined) return;
      i = +i;
      var at = picked.indexOf(i);
      if (at < 0) { picked.push(i); draw(); }
      else if (at === picked.length - 2) { picked.pop(); draw(); }
    };
    wheel.addEventListener('pointerdown', function(e){ dragging = true; say('', false); hit(e); });
    window.addEventListener('pointermove', function(e){ if (dragging) hit(e); });
    window.addEventListener('pointerup', function(){ if (dragging) { dragging = false; finish(); } });
    phone.querySelector('[data-shuffle]').addEventListener('click', function(){
      var xs = LEVEL.letters.map(function(l){ return l; });
      xs.sort(function(){ return Math.random() - .5; });
      letters.forEach(function(l,i){ l.textContent = xs[i]; });
      LEVEL.letters = xs;
    });
    phone.querySelector('[data-hint]').addEventListener('click', function(){
      if (rubies < HINT_COST) { location.href = go('shop'); return; }
      var cells = [].slice.call(phone.querySelectorAll('.cell.on:not(.open)'));
      if (!cells.length) return;
      var c = cells[0]; c.textContent = c.dataset.l; c.classList.add('hint','open');
      rubies -= HINT_COST; bals();
    });
    phone.querySelector('[data-ask]').addEventListener('click', function(){ say('Mở chia sẻ ảnh chụp (native share, ngoài prototype)', true); });
    [].forEach.call(phone.querySelectorAll('[data-open]'), function(b){ b.addEventListener('click', function(){ location.href = go(b.dataset.open); }); });
  }
})();
