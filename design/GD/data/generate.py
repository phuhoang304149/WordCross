#!/usr/bin/env python3
"""Sinh vi/fr/es/pt.json + manifest.json. Chạy: python3 -I generate.py <repo_root>"""
import gzip, hashlib, json, os, re, sys, unicodedata
from collections import Counter

ROOT = sys.argv[1]
DATA = os.path.join(ROOT, "design/GD/data")
MAXANS, MAXBONUS = 15, 9
SIZES = [12] + [18] * 29
QUOTA = {3: 12, 4: 36, 5: 126, 6: 180, 7: 180}  # phân bố Anh, chỉ là mục tiêu; thiếu thì dồn sang độ dài kề
VI_ALPHA = set("aăâbcdđeêghiklmnoôơpqrstuưvxy")
VI_TONES = {"̀", "́", "̃", "̉", "̣"}


def sha(path):
    return hashlib.sha256(open(path, "rb").read()).hexdigest()


def nfc(s):
    return unicodedata.normalize("NFC", s.strip())


def read_lines(path, enc, gz=False):
    raw = gzip.open(path).read() if gz else open(path, "rb").read()
    return raw.decode(enc).split("\n")


VI_SYL = re.compile(r"^(ngh|ng|nh|ph|th|tr|ch|gh|gi|kh|qu|[bcdđghklmnprstvx])?[aăâeêioôơuưy]+(ch|ng|nh|[cmnpt])?$")
VOWELS = set("aeiouyàáâãäåæèéêëìíîïòóôõöùúûüýÿœ")


def vi_ok(w):
    base = "".join(c for c in unicodedata.normalize("NFD", w) if c not in VI_TONES)
    base = unicodedata.normalize("NFC", base)
    return all(c in VI_ALPHA for c in base) and VI_SYL.match(base) is not None


def load(loc):
    a = os.path.join(ROOT, "Assets")
    if loc == "vi":
        files = [os.path.join(DATA, "source/Viet74K.txt.gz")]
        lines = read_lines(files[0], "utf-8", True)
    else:
        d, n = {"fr": ("10000 French Words", "10000 common french words.txt"),
                "es": ("10000 Spanish Words", "10000 common spanish words.txt"),
                "pt": ("5000 Portuguese Words", "5000 common portuguese words.txt")}[loc]
        files = [os.path.join(a, d, n)]
        lines = read_lines(files[0], "latin-1")
    words, seen, rej = [], set(), Counter()
    for ln in lines:
        w = nfc(ln)
        if not w or w != ln.strip():
            rej["empty"] += 1
        elif w != w.lower():
            rej["uppercase/proper"] += 1
        elif not w.isalpha():
            rej["space/hyphen/punct/digit"] += 1
        elif not 2 <= len(w) <= 7:
            rej["length"] += 1
        elif loc == "vi" and not vi_ok(w):
            rej["not-vietnamese-syllable"] += 1
        elif loc != "vi" and not any(c in VOWELS for c in w):
            rej["no-vowel"] += 1
        elif w in seen:
            rej["duplicate"] += 1
        else:
            seen.add(w)
            words.append(w)
    return files, lines, words, rej


def hkey(w):
    return hashlib.sha1(w.encode()).hexdigest()


def subwords(seed, words):
    sc = Counter(seed)
    return [w for w in words if len(w) <= len(seed) and not (Counter(w) - sc)]


def build(loc):
    files, lines, words, rej = load(loc)
    rank = {w: i for i, w in enumerate(words)}
    # vi không có tần suất: thứ tự ổn định theo hash; fr/es/pt: thứ tự tần suất của file
    order = (lambda w: hkey(w)) if loc == "vi" else (lambda w: rank[w])
    cands = {n: [] for n in QUOTA}
    seen_anagram = set()
    for w in sorted(words, key=order):
        if 3 <= len(w) <= 7 and "".join(sorted(w)) not in seen_anagram:
            seen_anagram.add("".join(sorted(w)))
            subs = subwords(w, words)
            if len(subs) >= 2:
                cands[len(w)].append((w, subs))
    for n in cands:
        cands[n].sort(key=lambda t: order(t[0]))
    avail = {n: len(v) for n, v in cands.items()}
    quota = dict(QUOTA)
    # dồn thiếu hụt: lên độ dài lớn hơn trước, rồi nhỏ hơn
    for n in sorted(QUOTA):
        if avail[n] < quota[n]:
            deficit = quota[n] - avail[n]
            quota[n] = avail[n]
            for m in sorted(QUOTA, key=lambda m: (m < n, abs(m - n))):
                if m != n and deficit and avail[m] > quota[m]:
                    t = min(deficit, avail[m] - quota[m])
                    quota[m] += t
                    deficit -= t
    chosen = []
    for n in sorted(QUOTA):
        chosen += cands[n][:quota[n]]
    short = 534 - len(chosen)
    items = []
    for w, subs in chosen:
        subs = sorted(subs, key=lambda x: (x != w, order(x)))  # seed trước, rồi ưu tiên phổ biến
        ans = subs[:MAXANS - 1] if w not in subs[:MAXANS] else subs[:MAXANS]
        if w not in ans:
            ans = [w] + ans[:MAXANS - 1]
        rest = [x for x in subs if x not in ans][:MAXBONUS]
        ans = sorted(ans, key=lambda x: (len(x), order(x)))
        dup = len(w) - len(set(w))
        diff = len(w) * 100 + dup * 8 + len(ans) * 2
        items.append((diff, hkey(w), w, ans, rest))
    items.sort()
    levels, i = [], 0
    for wi in range(6):
        for si in range(5):
            for li in range(SIZES[wi * 5 + si]):
                if i >= len(items):
                    break
                _, _, w, ans, rest = items[i]
                i += 1
                levels.append({
                    "id": f"{loc}-{wi+1}-{si+1}-{li+1}", "world": wi + 1, "subworld": si + 1, "level": li + 1,
                    "word": w, "answers": ans, "bonusWords": sorted(rest, key=lambda x: (len(x), x)),
                    "validWords": ans + sorted(rest, key=lambda x: (len(x), x))})
    doc = {"schemaVersion": 1, "locale": loc,
           "normalization": {"unicode": "NFC", "case": "lowercase", "accents": "exact (không gộp dấu; mỗi code point NFC là 1 ô chữ, ñ/ç/đ/ă/â/ê/ô/ơ/ư là chữ riêng, thanh điệu vi nằm trong ô)",
                             "letters": "chỉ ký tự chữ (str.isalpha), có nguyên âm (vi: đúng cấu trúc âm tiết onset+vần+coda); mỗi tập chữ cái chỉ làm seed 1 lần (không level anagram trùng); loại khoảng trắng, dấu nối, dấu nháy, số, chữ hoa/tên riêng viết hoa",
                             "wordLength": "2-7 ký tự; word (vòng chữ) 3-7",
                             "multiset": "mỗi answer/validWord ghép được từ multiset ký tự của word"},
           "fields": {"word": "seed = vòng chữ, luôn có trong answers", "answers": "tối đa 15, tăng dần độ dài, bắt buộc tìm",
                      "bonusWords": "từ thưởng = validWords trừ answers, tối đa 9", "validWords": "answers + bonusWords (giống schema gốc: validWords chứa answers)"},
           "levels": levels}
    stats = {"corpusLines": len(lines), "usableWords": len(words), "rejected": dict(rej), "seedCandidatesByLen": avail,
             "quotaUsedByLen": quota, "levelsShortfall": short, "files": [{"path": os.path.relpath(f, ROOT), "sha256": sha(f)} for f in files]}
    return doc, stats


def main():
    man = {"schemaVersion": 1, "target": {"levelsPerLocale": 534, "topology": "6 world x 5 subworld; W1 sub1 = 12, còn lại 18"},
           "policy": {
               "answers": "mọi từ corpus cùng locale ghép được từ word (>=2 đáp án/level); nếu >15 giữ seed + từ phổ biến (fr/es/pt theo thứ hạng file; vi theo hash ổn định vì không có tần suất); phần còn lại thành bonus (<=9)",
               "difficulty": "tăng dần theo độ dài word, chữ lặp, số đáp án; quota độ dài mặc định theo phân bố Anh, thiếu từ thì dồn sang độ dài kề, không nhân bản level",
               "vi": "CHỈ tiếng đơn/âm tiết (Viet74K lọc mục không khoảng trắng/dấu nối). Là policy bộ data để tích hợp vòng chữ hiện tại, KHÔNG phải hỗ trợ từ ghép nhiều tiếng. Dấu giữ chính xác.",
               "quality": "Chưa native-review. Corpus fr/es/pt là danh sách từ phổ biến đã hạ chữ thường (có thể lẫn tên riêng/viết tắt/động từ chia); vi là wordlist rộng gồm cả từ hiếm/phương ngữ."},
           "sources": {
               "vi": {"url": "https://raw.githubusercontent.com/duyet/vietnamese-wordlist/master/Viet74K.txt", "author": "Hồ Ngọc Đức wordlist qua duyet/vietnamese-wordlist", "license": "GPL-2.0 (LICENSE repo nguồn); output phái sinh, cần xem xét khi phát hành", "rawSha256": "b83d49308336f12ca46d06287d8db6e2cdc7a725d9a9dac56930051775fafb90", "stored": "design/GD/data/source/Viet74K.txt.gz"},
               "fr/es/pt": "Assets/10000 French Words, Assets/10000 Spanish Words, Assets/5000 Portuguese Words (ISO-8859-1, license chưa ghi trong repo)"},
           "locales": {}, "outputs": {}}
    for loc in ["vi", "fr", "es", "pt"]:
        doc, st = build(loc)
        p = os.path.join(DATA, loc + ".json")
        with open(p, "w", encoding="utf-8") as f:
            json.dump(doc, f, ensure_ascii=False, indent=1)
            f.write("\n")
        st["levels"] = len(doc["levels"])
        st["sha256"] = sha(p)
        man["locales"][loc] = st
        print(loc, st["levels"], st["seedCandidatesByLen"], st["quotaUsedByLen"], st["rejected"])
    with open(os.path.join(DATA, "manifest.json"), "w", encoding="utf-8") as f:
        json.dump(man, f, ensure_ascii=False, indent=1)
        f.write("\n")


main()
