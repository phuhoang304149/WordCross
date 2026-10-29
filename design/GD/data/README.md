# Dữ liệu level đa ngôn ngữ (vi / fr / es / pt)

Mỗi file `<locale>.json` có 534 level (6 World × 5 Subworld; W1·Sub1 = 12, các Subworld còn lại = 18), cùng topology với level Anh gốc. **Không** thay dữ liệu Anh, `design/GD/index.html` hay runtime Unity.

## Schema (schemaVersion 1)
`{schemaVersion, locale, normalization, fields, levels[]}`; mỗi level: `id` (`<locale>-<world>-<sub>-<level>`, ổn định, 1-based), `world`, `subworld`, `level`, `word` (vòng chữ/seed), `answers`, `bonusWords`, `validWords`.
Ánh xạ sang `GameLevel`: `word` = `word`; `answers` = nối `answers` bằng `|` (tăng dần độ dài, luôn chứa `word`); `validWords` = nối `validWords` bằng `|` (= answers + bonusWords, giống schema gốc). Từ thưởng = `validWords` trừ `answers`.

## Policy
- Unicode NFC, chữ thường, **giữ dấu chính xác** (không gộp dấu; vòng chữ gồm đúng các code point NFC, ñ ç đ ă â ê ô ơ ư là chữ riêng, thanh điệu vi nằm trong ô). Vì vậy không có đáp án nào trùng khoá chuẩn hoá trong cùng level.
- Loại: khoảng trắng, dấu nối, dấu nháy, số, mục viết hoa (tên riêng), từ không có nguyên âm, từ ngoài 2–7 ký tự (seed 3–7).
- Mọi answer/validWord nằm trong corpus đúng locale và ghép được bằng multiset chữ của `word`. Mỗi tập chữ chỉ làm seed một lần (không có level anagram trùng); không nhân bản level.
- `answers`: 2–15 từ. Nếu corpus có >15 từ ghép được: giữ seed + từ phổ biến nhất (fr/es/pt theo thứ hạng trong file; vi không có tần suất nên theo hash ổn định); phần còn lại thành `bonusWords` (tối đa 9). Nhiều level không có từ thưởng (xem `validate.py` in số liệu).
- Độ khó tăng dần theo độ dài seed → chữ lặp → số đáp án. Quota độ dài mặc định theo Anh (3×12, 4×36, 5×126, 6×180, 7×180); thiếu thì dồn sang độ dài kề (vi: âm tiết đơn rất ít từ 6–7 chữ nên chỉ có 3×12, 4×36, 5×384, 6×101, 7×1 — số thật trong `manifest.json`).
- **vi**: chỉ tiếng đơn/âm tiết (Viet74K lọc mục không khoảng trắng/dấu nối, kiểm cấu trúc âm tiết). Đây là policy bộ data để tích hợp vòng chữ hiện tại, **chưa phải** hỗ trợ từ ghép nhiều tiếng.

## Chưa xác minh
Chưa native-review, chưa playtest. fr/es/pt là danh sách từ phổ biến trong `Assets/` (có thể lẫn viết tắt, tên riêng viết thường, từ chia, từ nước ngoài như "aa"/"ee"); vi là wordlist rộng nên có từ hiếm/phương ngữ/thán từ. Nguồn vi theo GPL-2.0: cần cân nhắc khi phát hành data phái sinh.

## Tái lập
`python3 -I generate.py <repo_root>` (sinh json + manifest; vài phút) rồi `python3 -I validate.py <repo_root>` (kiểm count, topology, membership, multiset, NFC, unique, thứ tự, schema, checksum). Nguồn vi: `source/Viet74K.txt.gz` (nén từ https://raw.githubusercontent.com/duyet/vietnamese-wordlist/master/Viet74K.txt, Hồ Ngọc Đức, GPL-2.0, SHA256 raw trong `manifest.json`).
