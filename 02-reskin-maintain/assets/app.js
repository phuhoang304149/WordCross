(function () {
  'use strict';

  var SCHEMA = 'publishapp-workflow';
  var VERSION = 1;
  var STATUS = { waiting: 'Chờ', doing: 'Đang làm', blocked: 'Blocker', done: 'Xong', hold: 'HOLD/Hoãn', na: 'Không áp dụng' };
  var TYPES = {
    'code': 'Code', 'compile': 'Compile/build', 'cli-smoke': 'CLI smoke', 'runtime': 'Runtime thiết bị',
    'ui': 'UI so design', 'manual': 'Manual (người)', 'content': 'Nội dung thật', 'ads-iap-ota': 'Ads/IAP/OTA'
  };
  var CLAIM_TYPES = ['runtime', 'ui', 'content', 'ads-iap-ota'];
  var TASK_KEYS = ['id', 'title', 'type', 'status', 'screen', 'goal', 'scope', 'owner', 'deps', 'criteria', 'evidence', 'note'];
  var MAX_TEXT = 20000;

  function isObj(v) { return v !== null && typeof v === 'object' && !Array.isArray(v); }

  function validateTasks(list) {
    if (!Array.isArray(list)) { return 'tasks phải là mảng'; }
    if (list.length > 500) { return 'quá 500 task'; }
    var seen = {};
    for (var i = 0; i < list.length; i++) {
      var t = list[i];
      if (!isObj(t)) { return 'task #' + (i + 1) + ' không phải object'; }
      for (var k = 0; k < TASK_KEYS.length; k++) {
        var key = TASK_KEYS[k];
        if (typeof t[key] !== 'string' || t[key].length > MAX_TEXT) { return 'task #' + (i + 1) + ': thiếu/sai kiểu "' + key + '"'; }
      }
      if (!t.id.trim()) { return 'task #' + (i + 1) + ': id rỗng'; }
      if (seen[t.id]) { return 'task trùng id ' + t.id; }
      seen[t.id] = true;
      if (!TYPES[t.type]) { return 'task ' + t.id + ': type không hợp lệ'; }
      if (!STATUS[t.status]) { return 'task ' + t.id + ': status không hợp lệ'; }
    }
    return '';
  }

  function validateSnapshot(obj, workflow, meta) {
    if (!isObj(obj)) { return 'Gốc JSON phải là object'; }
    if (obj.schema !== SCHEMA) { return 'schema không đúng (cần ' + SCHEMA + ')'; }
    if (obj.version !== VERSION) { return 'version ' + obj.version + ' chưa hỗ trợ (cần ' + VERSION + ')'; }
    if (obj.workflow !== workflow) { return 'JSON thuộc tuyến "' + obj.workflow + '", trang này là "' + workflow + '"'; }
    if (!isObj(obj.fields)) { return 'thiếu fields'; }
    var ids = Object.keys(obj.fields);
    for (var i = 0; i < ids.length; i++) {
      var m = meta[ids[i]];
      var v = obj.fields[ids[i]];
      if (!m) { continue; }
      if (m.kind === 'check' && typeof v !== 'boolean') { return 'field ' + ids[i] + ' phải boolean'; }
      if (m.kind !== 'check' && (typeof v !== 'string' || v.length > MAX_TEXT)) { return 'field ' + ids[i] + ' phải là chuỗi'; }
      if (m.kind === 'select' && m.options.indexOf(v) < 0) { return 'field ' + ids[i] + ': giá trị "' + v + '" không có trong lựa chọn'; }
    }
    return validateTasks(obj.tasks);
  }

  function buildSnapshot(workflow, mode, values, tasks, now) {
    return { schema: SCHEMA, version: VERSION, workflow: workflow, mode: mode, exportedAt: now, fields: values, tasks: tasks };
  }

  function summarize(phases, tasks) {
    var p = {}, t = {}, k;
    for (k in STATUS) { p[k] = 0; t[k] = 0; }
    var blockers = [];
    phases.forEach(function (ph) {
      p[ph.status] += 1;
      if (ph.status === 'blocked') { blockers.push({ where: ph.title, note: ph.note }); }
    });
    var types = {};
    Object.keys(TYPES).forEach(function (ty) { types[ty] = { total: 0, done: 0, ok: 0, blocked: 0, na: 0, naNoNote: 0, applicable: 0 }; });
    var doneNoEvidence = 0, naNoNote = 0;
    tasks.forEach(function (tk) {
      t[tk.status] += 1;
      types[tk.type].total += 1;
      if (tk.status === 'na') {
        types[tk.type].na += 1;
        if (!tk.note.trim()) { types[tk.type].naNoNote += 1; naNoNote += 1; }
      }
      if (tk.status === 'done') {
        types[tk.type].done += 1;
        if (tk.evidence.trim()) { types[tk.type].ok += 1; } else { doneNoEvidence += 1; }
      }
      if (tk.status === 'blocked') { types[tk.type].blocked += 1; blockers.push({ where: tk.id + ' ' + tk.title, note: tk.note }); }
    });
    var gate = [];
    if (tasks.length === 0) { gate.push('Chưa rã task thật — chưa được code/claim.'); }
    Object.keys(TYPES).forEach(function (ty) {
      var s = types[ty];
      s.applicable = s.total - (s.na - s.naNoNote);
      if (s.total === 0) {
        if (CLAIM_TYPES.indexOf(ty) >= 0) { gate.push(TYPES[ty] + ': chưa có task → chưa xác minh (nếu ngoài scope: tạo task/record Không áp dụng kèm lý do)'); }
      } else if (s.ok < s.applicable) { gate.push(TYPES[ty] + ': ' + (s.applicable - s.ok) + '/' + s.applicable + ' áp dụng chưa xong kèm evidence'); }
    });
    if (naNoNote > 0) { gate.push(naNoNote + ' task Không áp dụng thiếu note lý do → chưa chốt, không tính đạt'); }
    if (doneNoEvidence > 0) { gate.push(doneNoEvidence + ' task Xong nhưng thiếu evidence'); }
    var counted = phases.length - p.na;
    var pct = counted > 0 ? Math.round(p.done * 100 / counted) : 0;
    return { phases: p, tasks: t, types: types, blockers: blockers, gate: gate, pct: pct, doneNoEvidence: doneNoEvidence };
  }

  var STAGES = {
    '': 'chưa ghi stage', 'draft': 'draft', 'ready': 'ready-for-review', 'submitted': 'submitted (chờ review)', 'in-review': 'in review',
    'approved-wait': 'approved-waiting-user: CHỜ USER publish, chưa done/live', 'live': 'live (USER đã publish)'
  };

  // Pure: agent chỉ được gửi review; không có đầu ra nào cấp quyền publish/rollout public.
  function submitGate(f, qaOk) {
    var w = [];
    if (f.release === 'prepare') { w.push('Quyền: PREPARE-ONLY → chuẩn bị xong, chưa gửi review.'); }
    else if (f.release === 'hold') { w.push('Quyền: HOLD → không gửi review/publish; làm phần độc lập.'); }
    else if (f.release !== 'allow') { w.push('Quyền gửi review chưa chốt → chỉ chuẩn bị, chưa gửi.'); }
    if (f.pub === 'first') { w.push('First publication (kể cả app cũ chưa từng public): không có Managed publishing → có thể auto-live → chuẩn bị xong và bàn giao, không gửi review production.'); }
    else if (f.pub !== 'published') { w.push('Chưa xác minh trạng thái first/published trên Play → coi như first publication: bàn giao, không gửi.'); }
    else if (f.managed === 'off') { w.push('Managed publishing OFF → auto-live sau approve → agent không submit production.'); }
    else if (f.managed !== 'on') { w.push('Managed publishing chưa biết/chưa chốt → note + bàn giao, không submit production.'); }
    else if (!String(f.evidence || '').trim()) { w.push('Managed ON chưa có evidence kiểm Console thật → chưa đủ để giữ manual release; bàn giao.'); }
    if (!qaOk) { w.push('QA gate cần thiết chưa đạt (xem Cổng claim) → chưa gửi review.'); }
    return { canSubmit: w.length === 0, warnings: w };
  }

  function releaseHandoff(f, qaOk) {
    var g = submitGate(f, qaOk);
    var out = g.warnings.slice();
    var stage = f.stage || '';
    var late = ['submitted', 'in-review', 'approved-wait', 'live'].indexOf(stage) >= 0;
    if (late && !g.canSubmit) { out.push('Stage đã ghi là "' + STAGES[stage] + '" nhưng điều kiện gửi review không thỏa → kiểm lại record thật.'); }
    if (g.canSubmit) { out.unshift('Đủ điều kiện GỬI REVIEW (không gồm publish). Sau approve: USER tự nhấn Publish.'); }
    if (stage === 'approved-wait') { out.push('Đang chờ USER publish — không đánh dấu release xong.'); }
    if (stage === 'submitted' || stage === 'in-review') { out.push('Stage ' + STAGES[stage] + ' ≠ live; chờ review rồi USER publish.'); }
    if (!f.push) { out.push('Quyền commit/push riêng chưa chốt (không suy từ quyền review).'); }
    out.push('Stage hiện tại: ' + (STAGES[stage] || STAGES['']) + '. Manual release luôn là việc của USER.');
    return { canSubmit: g.canSubmit, items: out };
  }

  var logic = { submitGate: submitGate, releaseHandoff: releaseHandoff, STAGES: STAGES, validateTasks: validateTasks, validateSnapshot: validateSnapshot, buildSnapshot: buildSnapshot, summarize: summarize, STATUS: STATUS, TYPES: TYPES };
  if (typeof document === 'undefined') { globalThis.PublishFlowLogic = logic; return; }

  var body = document.body;
  var workflow = body.dataset.workflow;
  var taskList = document.getElementById('task-list');
  var box = document.getElementById('json-box');
  var msgEl = document.getElementById('save-msg');
  var dirtyEl = document.getElementById('dirty');
  var dirty = false;

  function $(id) { return document.getElementById(id); }
  function el(tag, cls, text) {
    var n = document.createElement(tag);
    if (cls) { n.className = cls; }
    if (text !== undefined) { n.textContent = text; }
    return n;
  }
  function allFields() { return Array.prototype.slice.call(document.querySelectorAll('[data-field]')); }
  function fieldKind(f) { return f.type === 'checkbox' ? 'check' : (f.tagName === 'SELECT' ? 'select' : 'text'); }

  function fieldMeta() {
    var meta = {};
    allFields().forEach(function (f) {
      meta[f.id] = { kind: fieldKind(f), options: f.tagName === 'SELECT' ? Array.prototype.map.call(f.options, function (o) { return o.value; }) : [] };
    });
    return meta;
  }

  function say(text, cls) {
    msgEl.textContent = text;
    msgEl.className = 'msg ' + (cls || '');
  }
  function setDirty(on) {
    dirty = on;
    dirtyEl.classList.toggle('on', on);
    scheduleSave();
  }

  function readTasks() {
    return Array.prototype.map.call(taskList.querySelectorAll('.task'), function (card) {
      var t = {};
      TASK_KEYS.forEach(function (k) { t[k] = card.querySelector('[data-k="' + k + '"]').value; });
      return t;
    });
  }

  function labeled(label, node, wide) {
    var w = el('label', 'f' + (wide ? ' wide' : ''), label);
    w.appendChild(node);
    return w;
  }
  function textField(key, value, multi) {
    var n = el(multi ? 'textarea' : 'input');
    if (!multi) { n.type = 'text'; }
    n.dataset.k = key;
    n.value = value;
    return n;
  }
  function selectField(key, map, value) {
    var s = el('select');
    s.dataset.k = key;
    Object.keys(map).forEach(function (v) {
      var o = el('option', '', map[v]);
      o.value = v;
      s.appendChild(o);
    });
    s.value = value;
    return s;
  }

  function renderTask(t) {
    var card = el('article', 'task');
    var head = el('div', 'task-head');
    var idIn = textField('id', t.id);
    idIn.style.maxWidth = '110px';
    idIn.style.flex = 'none';
    head.appendChild(idIn);
    head.appendChild(textField('title', t.title));
    head.appendChild(selectField('type', TYPES, t.type));
    head.appendChild(selectField('status', STATUS, t.status));
    var del = el('button', 'danger', 'Xoá');
    del.type = 'button';
    del.addEventListener('click', function () {
      if (window.confirm('Xoá task ' + idIn.value + '?')) { card.remove(); setDirty(true); refresh(); }
    });
    head.appendChild(del);
    card.appendChild(head);
    var g = el('div', 'grid');
    g.appendChild(labeled('Màn hình / chức năng', textField('screen', t.screen)));
    g.appendChild(labeled('Người phụ trách', textField('owner', t.owner)));
    g.appendChild(labeled('Phụ thuộc (id task / dịch vụ)', textField('deps', t.deps)));
    g.appendChild(labeled('Mục tiêu', textField('goal', t.goal, true), true));
    g.appendChild(labeled('Phạm vi (làm / không làm)', textField('scope', t.scope, true), true));
    g.appendChild(labeled('Tiêu chí đạt', textField('criteria', t.criteria, true), true));
    g.appendChild(labeled('Evidence (lệnh, path, version/hash, kết quả)', textField('evidence', t.evidence, true), true));
    g.appendChild(labeled('Ghi chú / blocker + đầu vào cần', textField('note', t.note, true), true));
    card.appendChild(g);
    taskList.appendChild(card);
  }

  function renderTasks(list) {
    while (taskList.firstChild) { taskList.removeChild(taskList.firstChild); }
    list.forEach(renderTask);
  }

  function nextId() {
    var max = 0;
    readTasks().forEach(function (t) {
      var m = /^T(\d+)$/.exec(t.id);
      if (m) { max = Math.max(max, parseInt(m[1], 10)); }
    });
    return 'T' + String(max + 1).padStart(3, '0');
  }

  function phaseList() {
    return Array.prototype.map.call(document.querySelectorAll('section.phase'), function (s) {
      return {
        key: s.id, el: s, status: s.querySelector('[data-role="status"]').value,
        title: s.querySelector('h2').textContent.trim(), note: s.querySelector('[data-role="note"]').value
      };
    });
  }

  function fillList(ul, items, empty) {
    while (ul.firstChild) { ul.removeChild(ul.firstChild); }
    if (!items.length) { ul.appendChild(el('li', '', empty)); }
    items.forEach(function (text) { ul.appendChild(el('li', '', text)); });
  }

  function refresh() {
    var phases = phaseList();
    var tasks = readTasks();
    phases.forEach(function (p) {
      p.el.dataset.status = p.status;
      var link = document.querySelector('.rail a[href="#' + p.key + '"]');
      if (link) { link.dataset.status = p.status; }
    });
    Array.prototype.forEach.call(taskList.querySelectorAll('.task'), function (card, i) {
      card.dataset.status = tasks[i].status;
      card.classList.toggle('warn', tasks[i].status === 'done' && !tasks[i].evidence.trim());
    });
    $('empty-tasks').hidden = tasks.length > 0;
    var bad = tasks.some(function (t) { return !TYPES[t.type] || !STATUS[t.status]; });
    var s = summarize(phases, bad ? [] : tasks);
    $('st-done').textContent = s.phases.done;
    $('st-doing').textContent = s.phases.doing;
    $('st-blocked').textContent = s.phases.blocked;
    $('st-hold').textContent = s.phases.hold;
    $('st-tasks').textContent = s.tasks.done + '/' + tasks.length;
    $('st-pct').textContent = s.pct + '%';
    $('bar').style.width = s.pct + '%';
    fillList($('blockers'), s.blockers.map(function (b) { return b.where + ' — ' + (b.note.trim() || 'CHƯA GHI lý do/đầu vào cần'); }), 'Không có blocker.');
    fillList($('gate'), s.gate, 'Đạt trong PHẠM VI task/NA đã chốt — không tự nhận full game/publish; vẫn đối chiếu design, QA/manual và release permission riêng trước khi claim.');
    var rg = $('release-gate');
    if (rg) {
      var gv = function (id) { var n = $(id); return n ? n.value : ''; };
      fillList(rg, releaseHandoff({ release: gv('in-release'), push: gv('in-push'), pub: gv('in-pub-state'), managed: gv('in-managed'), evidence: gv('in-managed-evidence'), stage: gv('release-stage') }, tasks.length > 0 && s.gate.length === 0).items, '');
    }
    var tb = $('matrix-body');
    while (tb.firstChild) { tb.removeChild(tb.firstChild); }
    Object.keys(TYPES).forEach(function (ty) {
      var r = el('tr');
      var x = s.types[ty];
      [TYPES[ty], x.total, x.ok + '/' + x.applicable + ' (na ' + x.na + ')', x.blocked, x.total === 0 ? 'chưa xác minh' : (x.naNoNote > 0 ? 'na thiếu lý do' : (x.applicable === 0 ? 'Không áp dụng' : (x.ok === x.applicable ? 'đạt' : 'chưa đạt')))].forEach(function (c) { r.appendChild(el('td', '', String(c))); });
      tb.appendChild(r);
    });
    var platform = $('in-platform');
    if (platform) {
      Array.prototype.forEach.call(document.querySelectorAll('[data-when-platform]'), function (n) {
        var need = n.dataset.whenPlatform;
        n.hidden = !(platform.value === 'both' || platform.value === need);
      });
    }
    var mode = body.dataset.mode;
    var name = $('in-name');
    $('mode-banner').textContent = mode === 'template'
      ? 'TEMPLATE CHUNG — không ghi tiến độ/ID dự án. Copy NGUYÊN folder vào root project để tạo BẢN RIÊNG.'
      : 'BẢN RIÊNG — dự án: ' + ((name && name.value.trim()) || '(chưa nhập tên)') + '. Cập nhật ngay sau mỗi quyết định/kết quả.';
    $('mode-banner').dataset.mode = mode;
  }

  function download(text, name) {
    var blob = new Blob([text], { type: 'application/json' });
    var a = document.createElement('a');
    a.href = URL.createObjectURL(blob);
    a.download = name;
    document.body.appendChild(a);
    a.click();
    a.remove();
    setTimeout(function () { URL.revokeObjectURL(a.href); }, 2000);
  }

  function exportJson() {
    var values = Object.assign({}, orphanFields, readFields());
    var snap = buildSnapshot(workflow, body.dataset.mode, values, readTasks(), new Date().toISOString());
    var text = JSON.stringify(snap, null, 2);
    box.value = text;
    var name = ((($('in-name') || {}).value || 'template').trim().replace(/[^\w.-]+/g, '-') || 'template');
    download(text, workflow + '-' + name + '-snapshot.json');
    setDirty(false);
    say('Đã xuất JSON (' + Object.keys(values).length + ' field, ' + snap.tasks.length + ' task). Giữ file mới nhất; lần đọc sau đưa lại cho agent.', 'ok');
  }

  function applySnapshot(obj) {
    allFields().forEach(function (f) {
      if (!Object.prototype.hasOwnProperty.call(obj.fields, f.id)) { return; }
      if (fieldKind(f) === 'check') { f.checked = obj.fields[f.id]; } else { f.value = obj.fields[f.id]; }
    });
    renderTasks(obj.tasks);
    refresh();
  }

  function importText(text, label) {
    new Response(text).json().then(function (obj) {
      var err = validateSnapshot(obj, workflow, fieldMeta());
      if (err) { say('TỪ CHỐI JSON (' + label + '): ' + err + '. Chưa thay đổi gì.', 'err'); return; }
      applySnapshot(obj);
      setDirty(false);
      say('Đã nạp JSON (' + label + ') xuất lúc ' + String(obj.exportedAt) + '. Agent phải đồng bộ lại HTML nguồn theo JSON này.', 'ok');
    }, function () {
      say('TỪ CHỐI JSON (' + label + '): sai cú pháp. Chưa thay đổi gì.', 'err');
    });
  }

  // ---- Tự lưu localStorage (chỉ bản riêng) + merge 3 chiều với HTML nguồn ----
  // base = giá trị HTML nguồn đã biết ở lần lưu trước; local = bản user; source = HTML hiện tại.
  var AUTO_SCHEMA = 'publishapp-autosave';
  var AUTO_KEY = 'publishapp-tracker:v1:' + workflow + ':' + decodeURIComponent(location.pathname);
  var store = null;
  var autoReady = false;
  var autoTimer = 0;
  var autoBase = { fields: {}, tasks: {} };
  var orphanFields = {};
  var conflicts = [];
  var touchedEarly = {};
  var autoEl = $('autosave-msg');

  function autoSay(text, cls) {
    if (!autoEl) { return; }
    autoEl.textContent = text;
    autoEl.className = 'msg ' + (cls || '');
  }
  function has(o, k) { return Object.prototype.hasOwnProperty.call(o, k); }
  function copyTask(t) {
    var c = {};
    TASK_KEYS.forEach(function (k) { c[k] = typeof t[k] === 'string' ? t[k] : ''; });
    return c;
  }
  function sameTask(a, b) { return TASK_KEYS.every(function (k) { return a[k] === b[k]; }); }
  function readFields() {
    var values = {};
    allFields().forEach(function (f) { values[f.id] = fieldKind(f) === 'check' ? f.checked : f.value; });
    return values;
  }
  function sourceFields() {
    var values = {};
    allFields().forEach(function (f) {
      var kind = fieldKind(f);
      if (kind === 'check') { values[f.id] = f.defaultChecked; return; }
      if (kind === 'select') {
        var opt = Array.prototype.filter.call(f.options, function (o) { return o.defaultSelected; })[0] || f.options[0];
        values[f.id] = opt ? opt.value : '';
        return;
      }
      values[f.id] = f.defaultValue;
    });
    return values;
  }
  function fieldOk(f, v) {
    var kind = fieldKind(f);
    if (kind === 'check') { return typeof v === 'boolean'; }
    if (typeof v !== 'string') { return false; }
    return kind !== 'select' || Array.prototype.some.call(f.options, function (o) { return o.value === v; });
  }
  function writeField(f, v) {
    if (fieldKind(f) === 'check') { f.checked = v; } else { f.value = v; }
  }
  function cardById(id) {
    return Array.prototype.filter.call(taskList.querySelectorAll('.task'), function (c) { return c.querySelector('[data-k="id"]').value === id; })[0];
  }

  // Một trường: user chưa sửa (l === b) → lấy file; file chưa đổi (s === b) → giữ user; cả hai đổi khác nhau → xung đột, giữ user.
  function pick(l, s, b) {
    if (l === s || l === b) { return { v: s, b: s, c: false }; }
    if (s === b) { return { v: l, b: b, c: false }; }
    return { v: l, b: b, c: true };
  }

  function mergeState(src, rec) {
    var out = { fields: {}, tasks: [], base: { fields: {}, tasks: {} }, conflicts: [], orphans: {}, added: 0, kept: 0 };
    Object.keys(src.fields).forEach(function (id) {
      if (!has(rec.local.fields, id)) { out.fields[id] = src.fields[id]; out.base.fields[id] = src.fields[id]; return; }
      var r = pick(rec.local.fields[id], src.fields[id], rec.base.fields[id]);
      out.fields[id] = r.v;
      out.base.fields[id] = r.b;
      if (r.v !== src.fields[id]) { out.kept += 1; }
      if (r.c) { out.conflicts.push({ kind: 'field', id: id, mine: r.v, file: src.fields[id] }); }
    });
    Object.keys(rec.orphans).concat(Object.keys(rec.local.fields)).forEach(function (id) {
      if (!has(src.fields, id)) { out.orphans[id] = has(rec.local.fields, id) ? rec.local.fields[id] : rec.orphans[id]; }
    });
    var local = {}, srcIds = {}, used = {};
    rec.local.tasks.forEach(function (t) { if (!has(local, t.id)) { local[t.id] = t; } });
    src.tasks.forEach(function (st) {
      var id = st.id, lt = local[id], bt = has(rec.base.tasks, id) ? rec.base.tasks[id] : null;
      srcIds[id] = true;
      if (lt) {
        used[id] = true;
        var t = {}, nb = {};
        TASK_KEYS.forEach(function (k) {
          var r = pick(lt[k], st[k], bt ? bt[k] : undefined);
          t[k] = r.v;
          if (r.b !== undefined) { nb[k] = r.b; }
          if (r.v !== st[k]) { out.kept += 1; }
          if (r.c) { out.conflicts.push({ kind: 'task', id: id, key: k, mine: r.v, file: st[k] }); }
        });
        out.tasks.push(t);
        out.base.tasks[id] = nb;
      } else if (!bt) {
        out.tasks.push(copyTask(st));
        out.base.tasks[id] = copyTask(st);
        out.added += 1;
      } else {
        // User đã xoá task này; file đổi task đó → báo, không tự hồi sinh cũng không bỏ qua im lặng.
        out.base.tasks[id] = bt;
        if (!sameTask(st, bt)) { out.conflicts.push({ kind: 'deleted-mine', id: id, file: copyTask(st) }); }
      }
    });
    rec.local.tasks.forEach(function (lt, i) {
      if (srcIds[lt.id] && used[lt.id] && local[lt.id] === lt) { return; }
      var bt = has(rec.base.tasks, lt.id) && !srcIds[lt.id] ? rec.base.tasks[lt.id] : null;
      if (bt && sameTask(lt, bt)) { return; }
      if (bt) { out.base.tasks[lt.id] = bt; out.conflicts.push({ kind: 'deleted-file', id: lt.id }); }
      out.kept += 1;
      var at = 0;
      for (var j = i - 1; j >= 0; j--) {
        var pos = out.tasks.indexOf(rec.local.tasks[j]);
        if (pos < 0) { pos = out.tasks.map(function (t) { return t.id; }).indexOf(rec.local.tasks[j].id); }
        if (pos >= 0) { at = pos + 1; break; }
      }
      out.tasks.splice(at, 0, lt);
    });
    return out;
  }

  function recordOk(r) {
    return isObj(r) && r.schema === AUTO_SCHEMA && r.version === 1 && r.workflow === workflow && isObj(r.base) && isObj(r.base.fields) &&
      isObj(r.base.tasks) && isObj(r.local) && isObj(r.local.fields) && Array.isArray(r.local.tasks) && r.local.tasks.every(isObj);
  }

  function writeKey(key, text) {
    return new Promise(function (res) { store.setItem(key, text); res(); });
  }

  function saveNow() {
    if (!autoReady) { return; }
    clearTimeout(autoTimer);
    autoTimer = 0;
    var rec = { schema: AUTO_SCHEMA, version: 1, workflow: workflow, path: AUTO_KEY, savedAt: new Date().toISOString(),
      base: autoBase, local: { fields: readFields(), tasks: readTasks() }, orphans: orphanFields };
    writeKey(AUTO_KEY, JSON.stringify(rec)).then(function () {
      autoSay('Đã tự lưu trong trình duyệt lúc ' + new Date().toLocaleTimeString() + (conflicts.length ? ' — còn ' + conflicts.length + ' xung đột chưa chọn.' : '.'), conflicts.length ? 'err' : 'ok');
    }, function (e) {
      autoSay('KHÔNG tự lưu được (' + e.name + ') — bấm 💾 Lưu JSON ngay để khỏi mất dữ liệu.', 'err');
    });
  }
  function scheduleSave() {
    if (!autoReady) { return; }
    clearTimeout(autoTimer);
    autoTimer = setTimeout(saveNow, 400);
    autoSay('Tự lưu: đang chờ ghi…');
  }
  function flushSave() { if (autoTimer) { saveNow(); } }

  function short(v) {
    var t = typeof v === 'string' ? v : String(v);
    return '«' + (t.length > 160 ? t.slice(0, 160) + '…' : t) + '»';
  }
  function conflictText(c) {
    if (c.kind === 'field') {
      var f = $(c.id);
      var lab = f && f.closest('label') ? f.closest('label').textContent.trim().slice(0, 60) : c.id;
      return 'Field ' + c.id + ' (' + lab + '): bạn ' + short(c.mine) + ' ≠ file ' + short(c.file);
    }
    if (c.kind === 'task') { return 'Task ' + c.id + ' · ' + c.key + ': bạn ' + short(c.mine) + ' ≠ file ' + short(c.file); }
    if (c.kind === 'deleted-mine') { return 'Task ' + c.id + ': bạn đã xoá, nhưng file vừa cập nhật task này (' + STATUS[c.file.status] + ' · ' + short(c.file.title) + ').'; }
    return 'Task ' + c.id + ': file đã bỏ task này, nhưng bạn có sửa — đang giữ bản của bạn.';
  }
  function resolve(c, useFile) {
    var bt;
    if (c.kind === 'field') {
      autoBase.fields[c.id] = c.file;
      if (useFile && $(c.id)) { writeField($(c.id), c.file); }
    } else if (c.kind === 'task') {
      bt = autoBase.tasks[c.id] || (autoBase.tasks[c.id] = {});
      bt[c.key] = c.file;
      var card = cardById(c.id);
      if (useFile && card) { card.querySelector('[data-k="' + c.key + '"]').value = c.file; }
    } else if (c.kind === 'deleted-mine') {
      autoBase.tasks[c.id] = copyTask(c.file);
      if (useFile && !cardById(c.id)) { renderTask(c.file); }
    } else {
      delete autoBase.tasks[c.id];
      if (useFile && cardById(c.id)) { cardById(c.id).remove(); }
    }
    conflicts.splice(conflicts.indexOf(c), 1);
    renderConflicts();
    refresh();
    saveNow();
  }
  function renderConflicts() {
    var ul = $('conflict-list');
    if (!ul) { return; }
    while (ul.firstChild) { ul.removeChild(ul.firstChild); }
    if (!conflicts.length) { ul.appendChild(el('li', '', 'Không có xung đột.')); return; }
    conflicts.forEach(function (c) {
      var li = el('li', 'conflict', conflictText(c) + ' ');
      var mine = el('button', '', c.kind === 'deleted-mine' ? 'Vẫn xoá' : 'Giữ bản của tôi');
      var file = el('button', '', c.kind === 'deleted-mine' ? 'Khôi phục bản file' : (c.kind === 'deleted-file' ? 'Xoá theo file' : 'Dùng bản file'));
      mine.type = 'button';
      file.type = 'button';
      mine.addEventListener('click', function () { resolve(c, false); });
      file.addEventListener('click', function () { resolve(c, true); });
      li.appendChild(mine);
      li.appendChild(document.createTextNode(' '));
      li.appendChild(file);
      ul.appendChild(li);
    });
  }

  function startFresh(src) {
    renderTasks(src.tasks);
    autoBase = { fields: src.fields, tasks: {} };
    src.tasks.forEach(function (t) { autoBase.tasks[t.id] = copyTask(t); });
  }
  function applyMerged(src, rec) {
    var m = mergeState(src, rec);
    allFields().forEach(function (f) {
      if (touchedEarly[f.id] || !has(m.fields, f.id)) { return; }
      if (fieldOk(f, m.fields[f.id])) { writeField(f, m.fields[f.id]); return; }
      m.orphans[f.id + '#bad-saved-value'] = m.fields[f.id];
      m.base.fields[f.id] = rec.base.fields[f.id];
    });
    renderTasks(m.tasks);
    autoBase = m.base;
    orphanFields = m.orphans;
    conflicts = m.conflicts;
    return m;
  }

  function startAutosave(src) {
    if (body.dataset.mode === 'template') {
      renderTasks(src.tasks);
      autoSay('Template chung: KHÔNG tự lưu (tránh lẫn dữ liệu dự án).');
      return;
    }
    new Promise(function (res) { res(window.localStorage); }).then(function (s) {
      store = s;
      var raw = store.getItem(AUTO_KEY);
      if (raw === null) { startFresh(src); return 'Chưa có bản tự lưu cho file này — bắt đầu theo HTML nguồn.'; }
      return new Response(raw).json().then(function (rec) { return rec; }, function () { return null; }).then(function (rec) {
        if (!recordOk(rec)) {
          startFresh(src);
          return writeKey(AUTO_KEY + ':corrupt-' + Date.now(), raw).then(function () { return 'Bản tự lưu cũ hỏng — đã cất riêng (:corrupt-…), dùng HTML nguồn.'; });
        }
        return writeKey(AUTO_KEY + ':backup', raw).then(function () {
          var m = applyMerged(src, rec);
          return 'Đã khôi phục bản tự lưu (' + String(rec.savedAt) + '): giữ ' + m.kept + ' thay đổi của bạn, thêm ' + m.added + ' task mới từ file' +
            (m.conflicts.length ? ', ' + m.conflicts.length + ' XUNG ĐỘT cần chọn (khu Tự lưu).' : '.');
        });
      });
    }).then(function (text) {
      autoReady = true;
      renderConflicts();
      refresh();
      saveNow();
      say(text, conflicts.length ? 'err' : 'ok');
      var info = $('autosave-info');
      if (info) { info.textContent = 'Khoá lưu: ' + AUTO_KEY; }
    }, function (e) {
      if (!taskList.querySelector('.task')) { renderTasks(src.tasks); }
      refresh();
      autoSay('Tự lưu TẮT: trình duyệt chặn/không ghi được localStorage (' + (e && e.name) + '). Dùng 💾 Lưu JSON.', 'err');
    });
  }

  function downloadAutosave() {
    if (!store) { say('Chưa có localStorage để tải.', 'err'); return; }
    flushSave();
    var cur = store.getItem(AUTO_KEY), bak = store.getItem(AUTO_KEY + ':backup');
    download('{"key":' + JSON.stringify(AUTO_KEY) + ',\n"current":' + JSON.stringify(cur) + ',\n"backup":' + JSON.stringify(bak) + '}\n', workflow + '-autosave-backup.json');
    say('Đã tải bản tự lưu + backup (dạng chuỗi JSON thô). Dùng 💾 Lưu JSON cho snapshot nạp lại được.', 'ok');
  }

  function init() {
    $('btn-export').addEventListener('click', exportJson);
    $('btn-copy').addEventListener('click', function () {
      exportJson();
      box.select();
      document.execCommand('copy');
      say('Đã xuất và copy JSON vào clipboard.', 'ok');
    });
    $('btn-load').addEventListener('click', function () { $('file-in').click(); });
    $('file-in').addEventListener('change', function (e) {
      var file = e.target.files[0];
      if (!file) { return; }
      file.text().then(function (t) { importText(t, file.name); });
      e.target.value = '';
    });
    $('btn-paste').addEventListener('click', function () { importText(box.value, 'ô dán'); });
    $('btn-add-task').addEventListener('click', function () {
      renderTask({ id: nextId(), title: '', type: 'code', status: 'waiting', screen: '', goal: '', scope: '', owner: '', deps: '', criteria: '', evidence: '', note: '' });
      setDirty(true);
      refresh();
    });
    document.addEventListener('input', function (e) {
      if (e.target.id === 'json-box' || e.target.id === 'file-in') { return; }
      if (!autoReady && e.target.id) { touchedEarly[e.target.id] = true; }
      setDirty(true);
      refresh();
    });
    document.addEventListener('change', function (e) {
      if (e.target.id === 'file-in') { return; }
      setDirty(true);
      refresh();
    });
    window.addEventListener('beforeunload', function (e) {
      flushSave();
      if (dirty && !autoReady) { e.preventDefault(); e.returnValue = ''; }
    });
    window.addEventListener('pagehide', flushSave);
    document.addEventListener('visibilitychange', function () { if (document.visibilityState === 'hidden') { flushSave(); } });
    window.addEventListener('storage', function (e) {
      if (e.key === AUTO_KEY) { autoSay('CẢNH BÁO: tab khác vừa ghi bản tự lưu của file này. Chỉ sửa ở MỘT tab; tab nào lưu sau sẽ thắng.', 'err'); }
    });
    if ($('btn-autosave-dl')) { $('btn-autosave-dl').addEventListener('click', downloadAutosave); }
    var src = $('tasks-source');
    new Response(src.textContent).json().then(function (obj) {
      var list = isObj(obj) ? obj.tasks : null;
      var err = validateTasks(list);
      if (err) { say('tasks-source trong HTML lỗi: ' + err, 'err'); autoSay('Tự lưu TẮT vì tasks-source lỗi — bản lưu trình duyệt giữ nguyên.', 'err'); return; }
      startAutosave({ fields: sourceFields(), tasks: list });
    }, function () { say('tasks-source trong HTML sai cú pháp JSON — agent cần sửa.', 'err'); autoSay('Tự lưu TẮT vì tasks-source lỗi — bản lưu trình duyệt giữ nguyên.', 'err'); });
    refresh();
  }

  init();
})();
