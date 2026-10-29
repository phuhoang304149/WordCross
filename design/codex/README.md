# WordCross — bộ asset Codex

## Icon ứng dụng

Chọn **ô W trắng, chữ teal-deep `#0b8c80`**, chồng **ô C vàng `#ffc93c`, chữ nâu `#5a4100`**. W/C đại diện Word/Cross, chỉ hai chữ nên giữ nét rõ hơn bốn chữ nhỏ khi thu xuống 48px. Các góc bo và bóng đặc phía dưới kế thừa `.tile` của Home; không có gradient, bóng mờ hoặc chi tiết trang trí nhỏ. Nền teal **`#12b5a6`** full-bleed, không bo góc launcher, alpha toàn ảnh bằng 255.

- `icon/icon.svg`: nguồn vector 1024×1024, toàn bộ chữ Baloo 2 ExtraBold đã chuyển thành path.
- `icon/icon-1024.png`: icon vuông đặc 1024×1024.
- `icon/icon-fg-432.png`: foreground RGBA trong suốt 432×432. Giữ cùng tỷ lệ với icon gốc; toàn bộ phần có alpha nằm trong vùng giữa 66%, đồng thời trong hình tròn an toàn đường kính 66% (kiểm theo pixel).
- `icon/icon-bg-432.png`: nền đặc teal 432×432.
- `icon/icon-foreground.svg`: nguồn vector riêng cho adaptive foreground.
- `icon/icon-48.png`: bản thu nhỏ 48×48 để xem khả năng nhận diện; không phải asset launcher bổ sung.

Bản icon full-bleed và adaptive có cùng bố cục để ghép foreground/background cho kết quả đồng nhất. Launcher tự mask góc; không thêm viền nền, badge hoặc chữ tên game dài ở icon.

## Logo màn mở ứng dụng

- `splash/splash-logo.svg` và `splash/splash-logo-1024.png`: bốn ô **W trắng / O vàng / R coral / D trắng**, bóng teal-deep phía dưới, chữ **CROSS trắng** ở dưới. PNG trong suốt; nền dự kiến teal `#12b5a6`.
- `splash/splash-preview-1220x2712.png`: preview nền teal đặc, tâm bounds phần logo hữu hình trùng tâm màn; logo không chứa nền teal trong file trong suốt.

Font nguồn: `Assets/WordCross/Fonts/Baloo2/static/Baloo2-ExtraBold.ttf`, chỉ đọc. SVG không còn thẻ text nên không cần cài/nhúng font khi tích hợp. Home thực tế có ưu tiên Bagel Fat One trong tile; bộ này dùng Baloo 2 ExtraBold theo yêu cầu để đồng bộ các asset và bảo đảm có font nguồn tại chỗ. Màu tham chiếu từ theme.css; cấu trúc WORD/CROSS từ Home. `Assets/_Sprites/logogame.png` chỉ là tham chiếu cũ, không sửa.

## Sprite và bản dịch

Thông số border/PPU: [`sprites/README.md`](sprites/README.md). So sánh: [`sprites/compare.png`](sprites/compare.png); giới hạn bằng chứng render CSS ghi rõ trong README sprite.

Bản rà ngôn ngữ: [`../../02-reskin-maintain/l10n-review-codex.md`](../../02-reskin-maintain/l10n-review-codex.md): 18 ô đề xuất sửa + 5 ô cho key `win.streak.one`. Script `review-l10n.py` kiểm placeholder, chiều dài và số mục lịch, không sửa LocalizationData.

## Tái tạo và phạm vi

`generate-assets.swift` dùng CoreGraphics/CoreText/ImageIO có sẵn trên macOS để xuất SVG/PNG; `review-l10n.py` chỉ dùng Python standard library. Không cần thư viện ngoài, không dùng MCP, không build Unity, không commit. File tạo chỉ trong `design/codex/` và bản rà dịch được chỉ định. Các thay đổi có sẵn trong repository giữ nguyên.

Usage chat điều phối: **không có số liệu** input/output/cache hoặc chi phí thực tế. Không gọi Claude CLI/agent con; không phát sinh lượt tính phí qua các CLI đó. Đây không phải số liệu hóa đơn nhà cung cấp.
