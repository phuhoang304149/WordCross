#!/usr/bin/env python3
"""Kiểm vi/fr/es/pt.json. Chạy: python3 -I validate.py <repo_root>. Thoát 1 nếu có lỗi."""
import gzip, hashlib, json, os, sys, unicodedata
from collections import Counter

ROOT = sys.argv[1]
DATA = os.path.join(ROOT, "design/GD/data")
SRC = {"fr": "Assets/10000 French Words/10000 common french words.txt",
       "es": "Assets/10000 Spanish Words/10000 common spanish words.txt",
       "pt": "Assets/5000 Portuguese Words/5000 common portuguese words.txt"}
errs = []
man = json.load(open(os.path.join(DATA, "manifest.json"), encoding="utf-8"))
for loc in ["vi", "fr", "es", "pt"]:
    p = os.path.join(DATA, loc + ".json")
    if hashlib.sha256(open(p, "rb").read()).hexdigest() != man["locales"][loc]["sha256"]:
        errs.append(f"{loc}: checksum")
    d = json.load(open(p, encoding="utf-8"))
    if loc == "vi":
        raw = gzip.open(os.path.join(DATA, "source/Viet74K.txt.gz")).read()
        if hashlib.sha256(raw).hexdigest() != man["sources"]["vi"]["rawSha256"]:
            errs.append("vi: source sha")
        corpus = {unicodedata.normalize("NFC", l.strip()) for l in raw.decode("utf-8").split("\n")}
    else:
        corpus = {unicodedata.normalize("NFC", l.strip()) for l in open(os.path.join(ROOT, SRC[loc]), "rb").read().decode("latin-1").split("\n")}
    L = d["levels"]
    if d["schemaVersion"] != 1 or d["locale"] != loc or d["normalization"]["unicode"] != "NFC":
        errs.append(f"{loc}: header")
    if len(L) != 534:
        errs.append(f"{loc}: count {len(L)}")
    exp = [(w, s, l) for w in range(1, 7) for s in range(1, 6) for l in range(1, 13 if (w, s) == (1, 1) else 19)]
    if [(x["world"], x["subworld"], x["level"]) for x in L] != exp:
        errs.append(f"{loc}: topology")
    ids = [x["id"] for x in L]
    words = [x["word"] for x in L]
    if len(set(ids)) != 534 or len(set(words)) != 534:
        errs.append(f"{loc}: duplicate id/word")
    if len({tuple(x["answers"]) for x in L}) != 534:
        errs.append(f"{loc}: duplicate answer sets")
    for x in L:
        w, a, v, b = x["word"], x["answers"], x["validWords"], x["bonusWords"]
        tag = f"{loc}:{x['id']}"
        if x["id"] != f"{loc}-{x['world']}-{x['subworld']}-{x['level']}":
            errs.append(tag + " id")
        if not 3 <= len(w) <= 7 or w not in a:
            errs.append(tag + " seed")
        if not 2 <= len(a) <= 15 or len(set(a)) != len(a):
            errs.append(tag + " answers size/dup")
        if [len(t) for t in a] != sorted(len(t) for t in a):
            errs.append(tag + " answer order")
        if v != a + b or len(set(v)) != len(v) or len(b) > 9:
            errs.append(tag + " validWords/bonus")
        sc = Counter(w)
        for t in v:
            if t not in corpus:
                errs.append(f"{tag} not in corpus {t}")
            if unicodedata.normalize("NFC", t) != t or t != t.lower() or not t.isalpha() or len(t) < 2:
                errs.append(f"{tag} unicode/policy {t}")
            if Counter(t) - sc:
                errs.append(f"{tag} multiset {t}")
    diff = [len(x["word"]) for x in L]
    if diff != sorted(diff):
        errs.append(f"{loc}: seed length not non-decreasing")
    print(loc, len(L), "lengths", dict(sorted(Counter(diff).items())), "answers min/max", min(len(x["answers"]) for x in L), max(len(x["answers"]) for x in L),
          "noBonus", sum(1 for x in L if not x["bonusWords"]))
print("FAIL" if errs else "PASS")
for e in errs[:30]:
    print(e)
sys.exit(1 if errs else 0)
