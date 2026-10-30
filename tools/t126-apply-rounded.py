#!/usr/bin/env python3
"""T126: vá các khối Image dùng ui_round/ui_circle trong scene/prefab YAML theo báo cáo của RoundedSpriteSwap (-swapDry).
Dùng: t126-apply-rounded.py <báo cáo> <thư mục dự án>. Chỉ sửa m_Sprite, m_Type, m_PixelsPerUnitMultiplier, m_PreserveAspect."""
import re, sys, os

ROUND = '7c3e9a1b5d2f4a8e96b0c4d7e1f3a582'


def guid_of(meta):
    return re.search(r'^guid: (\w+)', open(meta).read(), re.M).group(1)


def main():
    report, proj = sys.argv[1], sys.argv[2]
    circle_old = guid_of(os.path.join(proj, 'Assets/WordCross/UI/Home/ui_circle.png.meta'))
    new = {n: guid_of(os.path.join(proj, 'Assets/WordCross/UI/Rounded/%s.png.meta' % n))
           for n in ['round-r10', 'round-r12', 'round-r14', 'round-rm', 'pill-44', 'round-rl', 'pill-52', 'circle']}
    rows = {}
    for line in open(report):
        p = line.rstrip('\n').split('|')
        rows[(p[0], p[1])] = p
        if p[0].endswith('.unity'):
            rows[(p[0], p[1].split('/', 1)[1])] = p  # scene: GameObject gốc (Canvas) không có Transform trong chuỗi cha
    files = {}
    for root, _, names in os.walk(os.path.join(proj, 'Assets')):
        if 'catgamingmodule' in root:
            continue
        for n in names:
            if n.endswith(('.unity', '.prefab')):
                files[n] = os.path.join(root, n)
    done = 0
    for name in sorted({k[0] for k in rows}):
        text = open(files[name]).read()
        docs = re.split(r'(?m)^(?=--- !u!)', text)
        objs = {}
        for i, d in enumerate(docs):
            m = re.match(r'--- !u!(\d+) &(-?\d+)', d)
            if m:
                objs[m.group(2)] = (i, m.group(1))
        go_name, tr_go, parent = {}, {}, {}
        for fid, (i, c) in objs.items():
            d = docs[i]
            if c == '1':
                m = re.search(r'm_Name: (.*)', d)
                if m:
                    go_name[fid] = m.group(1)
            if c in ('4', '224'):
                g = re.search(r'm_GameObject: \{fileID: (-?\d+)', d)
                f = re.search(r'm_Father: \{fileID: (-?\d+)', d)
                if g and f:
                    tr_go[fid] = g.group(1)
                    parent[g.group(1)] = f.group(1)

        def path(go):
            out = []
            while go in go_name and len(out) < 20:
                out.append(go_name[go])
                go = tr_go.get(parent.get(go))
            return '/'.join(reversed(out))

        for fid, (i, c) in objs.items():
            if c != '114':
                continue
            d = docs[i]
            sm = re.search(r'm_Sprite: \{fileID: (\d+), guid: (\w+), type: 3\}', d)
            if not sm or sm.group(2) not in (ROUND, circle_old):
                continue
            go = re.search(r'm_GameObject: \{fileID: (-?\d+)', d).group(1)
            row = rows.get((name, path(go))) or next((r for (f, k), r in rows.items() if f == name and k.endswith('/' + path(go))), None)
            if row is None:
                continue
            if sm.group(2) == circle_old:
                d = d.replace(sm.group(0), 'm_Sprite: {fileID: 21300000, guid: %s, type: 3}' % new['circle'])
                if row[3] == 'Simple' and abs(float(row[4].split('x')[0]) - float(row[4].split('x')[1])) < 0.5:
                    d = re.sub(r'm_PreserveAspect: \d', 'm_PreserveAspect: 1', d)
            else:
                d = d.replace(sm.group(0), 'm_Sprite: {fileID: 21300000, guid: %s, type: 3}' % new[row[4]])
                d = re.sub(r'm_Type: \d', 'm_Type: 1', d)
                d = re.sub(r'm_PixelsPerUnitMultiplier: [\d.eE+-]+', 'm_PixelsPerUnitMultiplier: %s' % row[5], d)
            docs[i] = d
            done += 1
        open(files[name], 'w').write(''.join(docs))
    print('đã vá', done, 'Image')


main()
