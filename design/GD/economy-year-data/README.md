# economy-year-data — ruby (khóa kỹ thuật coin), shop, ads, nhiệm vụ, chu kỳ 365 ngày

Dữ liệu **trung lập** (JSON, UTF-8), schemaVersion `1.0.0`. Mọi số là **đề xuất tuning chưa user duyệt**; giá `proposedUsd` không phải giá live. **Quyết định USER: GIỮ RUBY.** Không đổi tên tiền tệ runtime, không migration, không chuyển đổi số dư; các giá trị trong dữ liệu này áp dụng trực tiếp cho ruby. Khóa kỹ thuật `coin`, `*Coin`, `currency.id = "coin"` và ID/path giữ nguyên để tương thích consumer/runtime — mọi "coin" trong khóa/trường nghĩa là **lượng ruby**, không phải một currency coin khác. Hiển thị/localization dùng "ruby" (`loc currency.coin` = Ruby/Rubies).

**Anchor:** `anchorUtcDate = 2026-01-01` (C1 = 01/01/2026–31/12/2026; C2 bắt đầu 01/01/2027). 2026-10-30 là ngày 303 của C1.

**Chính sách triển khai:** không `git push`; được upload OTA R2 sau khi smoke test pass khi triển khai runtime. Task sửa design này không build/upload.

**Chưa có importer Unity cho economy.** `Assets/WordCross/Editor/LocaleLevelImporter.cs` chỉ nhập `design/GD/data/<mã>.json` thành `LocaleLevelPack` (534 level) — không đọc các file này. Cần viết importer/runtime riêng.

| File | kind | Nội dung |
|---|---|---|
| `economy-year.schema.json` | schema | Tập con JSON Schema 2020-12, `$defs` theo kind, map `files` |
| `economy-config.json` | economy | currency, earn, sinks, shop.packs, ads, streak, levelMilestones, totals, baseline, rules |
| `quest-templates.json` | quests | mẫu nhiệm vụ daily/weekly/monthly/event |
| `calendar-365.json` | calendar | cycle, loginRewards, `days[365]`, `weeks[52]`, `months[12]`, `events[12]` |
| `loc-keys.json` | loc | key → vi/en (fr/es/pt = null, chờ dịch) |
| `bundle.js` | (sinh) | `window.WC_ECON` cho UI mở offline qua file:// |
| `generate.py` | | nguồn sinh 4 JSON + bundle (sửa số ở đây rồi chạy lại) |
| `validate.py` | | validator tối thiểu |

## Quy ước
- **ID**: `^[a-z][a-z0-9_.]*$`. Tiền tố: `dq_/wq_/mq_/eq_` quest, `lr_` login, `st_` chuỗi, `lm_` mốc level, `ev_` sự kiện, `pk_` gói, `sk_` sink, `ad_` quảng cáo.
- **Đơn vị**: `coin` = integer coin; `*Sec` = giây; `day` 1..365 (cycle day); `proposedUsd` = USD đề xuất.
- **Ref**: `days[].loginRef → loginRewards.id`; `dailyQuestRefs/questRefs → quest-templates.templates.id`; `eventRef → events.id`; mọi `locKey → loc-keys.keys`; `productId` giữ `word.item1..5`, `word.noads`.
- Quest luôn `requiresAd=false`, `requiresPurchase=false`.

## Thứ tự import đề xuất
1. `loc-keys.json` → bảng localization (key, vi, en…).
2. `quest-templates.json` → bảng QuestTemplate.
3. `economy-config.json` → currency, sink, shop pack (map `productId` với Shop hiện có), ads placement, streak/level milestone, `rules`.
4. `calendar-365.json` → loginRewards, events, rồi months/weeks, cuối cùng 365 `days` (cần quest/event/login đã có).
5. Chạy `python3 validate.py` trước và sau import; so tổng `totals` với dữ liệu đã nạp.

## Mapping cần bổ sung (chưa tồn tại trong Unity)
`CurrencyController` giữ ruby (không đổi tên; trường `coin` = lượng ruby), `Const.HINT_COST` → `sinks.sk_hint_cell`, Shop SO/prefab → `shop.packs`, `AdmobController` placements → `ads`, kho ledger claim (`claimKey`), bộ lập lịch UTC (`rules`), localization 5 ngôn ngữ.

## Quy tắc thời gian & claim (chi tiết ở `economy-config.json#rules`)
`dayNumber = floor((utcMidnight(now) − anchorUtcDate)/86400000)`; `dayIndex = dayNumber % 365 + 1`; `cycleId = "C" + (floor(dayNumber/365)+1)`. Ngày 365 là ngày cầu nối (không thuộc tuần), sau đó là ngày 1 của `C{n+1}`. Năm nhuận: không có ngày riêng cho 29/02. Claim idempotent theo `claim:{cycleId}:{scope}:{periodId}:{refId}`; `maxSeenUtcDay` không lùi; offline dùng ledger cục bộ.

## Kiểm
`python3 generate.py && python3 validate.py` (schema, tham chiếu, đủ 365 ngày/52 tuần/12 tháng, tổng thưởng, baseline, `bundle.js` khớp JSON, số `data-econ` trong GD).
