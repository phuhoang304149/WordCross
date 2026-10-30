# Store listing Google Play — Word Crossy

Chỉ là nội dung chữ (copy). Chưa upload/submit; hình do Codex làm riêng. Tên app trên Play giữ **Word Crossy** (không đổi theo locale).

## Mapping locale

| Thư mục | Locale Play | Ghi chú |
|---|---|---|
| `en-US/` | en-US | **Ngôn ngữ mặc định** (USER chốt default en) |
| `vi/` | vi | |
| `fr-FR/` | fr-FR | UI game chỉ có `fr`; map sang fr-FR của Store |
| `es-ES/` | es-ES | UI game chỉ có `es`; map sang es-ES của Store |
| `pt-BR/` | pt-BR | UI game có `pt`; Store dùng pt-BR |

Không thêm locale khác. Mỗi thư mục: `short-description.txt` (≤80 ký tự), `full-description.txt` (≤4000), `headlines.txt`.

Số ký tự (kiểm 1 lần): short en 79, vi 73, fr 80, es 76, pt 77; full 1307–1606 mỗi ngôn ngữ.

## headlines.txt

6 dòng: `Tagline` (feature graphic) rồi 5 caption theo thứ tự Home, SelectWorld, Gameplay, Daily, Win. Dịch cùng một ý ở 5 ngôn ngữ.

## Đã quảng bá (có bằng chứng)

- Vuốt nối chữ trên vòng chữ, từ điền vào ô chữ; xáo chữ.
- Gợi ý mở 1 chữ, tốn ruby, không bắt buộc (key `game.hint`, `game.hintFree`, `game.coach2`).
- Từ thưởng (extra words) nhận ruby, mỗi từ tính một lần mỗi màn (`extra.*`).
- Ruby nhận khi thắng màn lần đầu (`win.first`, `win.replay`).
- Daily Challenge: T041 done, QA thiết bị PASS Home → DailyDialog → Main daily; chuỗi ngày và mốc thưởng (`win.streak`, `win.milestone`). Win Daily chưa QA thiết bị; copy chỉ nói thưởng và chuỗi như trong code/l10n.
- Chọn thế giới / chọn màn, vuốt đổi thế giới (`world.swipe`).
- Mô tả IAP/ads có điều kiện: gói ruby + gói tắt ads là tuỳ chọn mua; video thưởng do người chơi chọn.

## Chưa quảng bá

- VS Bot, VS AI: toast "sắp ra mắt" (`home.toast.bot/ai`).
- Chơi offline, số màn cụ thể (vd 1000), AI, minigame, phát âm, ảnh minh hoạ: chưa chứng minh.
- Ngôn ngữ bộ câu đố: UI đa ngôn ngữ (vi/en/fr/es/pt) không đồng nghĩa bộ câu đố cùng ngôn ngữ; copy không khẳng định điều này.
- Số lượng ruby, giá, tên gói IAP: không ghi (có thể đổi).
- Giá/ads: không mô tả như phần thưởng miễn phí không giới hạn.

## Nguồn xác minh

- `Assets/WordCross/Config/GameData/Localization/LocalizationData.asset` (key game.*, extra.*, win.*, home.*, world.swipe).
- `Assets/_Scripts/Controller/HomeController.cs` (nút Daily, DailyDialog).
- `02-reskin-maintain/index.html`: T041 (Daily), T040 (l10n), T120/T130 (IAP/noads).
- Không có `AGENTS.md` trong repo tại thời điểm làm; áp dụng ràng buộc từ yêu cầu task.
