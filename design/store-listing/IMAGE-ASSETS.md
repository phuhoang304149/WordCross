# Hình Google Play — Word Crossy — 2026-10-30

Ngôn ngữ mặc định: en-US. Đủ en-US, vi, fr-FR, es-ES, pt-BR.

## Nguồn và trạng thái

Theo yêu cầu USER ngày 30/10: **dùng design làm screenshot, không chờ thiết bị; AAB chỉ build khi USER yêu cầu sau**.

25 ảnh `screenshot-01-home.png` đến `screenshot-05-win.png` được render trực tiếp từ HTML tại `design/Ui/screens/` bằng WebKit qua Swift CLI, rồi ghép caption bản địa hóa. Đây là ảnh thiết kế, **không phải ảnh chụp runtime hay bằng chứng QA bản APK/AAB**. Không dùng Chrome screenshot, MCP hoặc thiết bị.

| Thứ tự | File | Design nguồn |
|---|---|---|
| 01 | screenshot-01-home.png | design/Ui/screens/home/index.html |
| 02 | screenshot-02-worlds.png | design/Ui/screens/select-world/index.html |
| 03 | screenshot-03-gameplay.png | design/Ui/screens/game/index.html |
| 04 | screenshot-04-daily.png | design/Ui/screens/daily/index.html |
| 05 | screenshot-05-win.png | design/Ui/screens/win/index.html |

Mỗi hàng có đủ 5 ngôn ngữ UI. Từ trong bàn chơi vẫn tiếng Anh; không quảng bá bộ câu đố đa ngôn ngữ.

## Export

- PNG RGB không alpha, 1080×1920 (9:16), mỗi file dưới 8 MB.
- Panel design 390×844 CSS px; font Baloo 2 từ project, render lên ảnh bằng WebKit, nền/caption bằng CoreGraphics/CoreText.
- Chỉ trong phiên export: ẩn nhãn DEMO và nút điều khiển prototype, dùng trạng thái đã tắt ads để không có banner placeholder; xếp các dòng hero Daily đúng khối. Không sửa HTML/CSS design gốc.
- Dữ liệu minh họa có chủ đích: 18 ruby; World 1 đã hoàn thành; bàn PARTY từ level thật `1-0-0` (đã tìm PART/TRAY và đang nối PARTY); Daily ngày 30/10/2026 với chuỗi 6 ngày; Win +2 ruby. Không dùng dữ liệu người chơi/thiết bị.
- Không quảng bá Bot/AI như tính năng đã hoạt động; Home giữ các nút đúng design nhưng caption không giới thiệu các chế độ này.
- 25 ảnh QA nháp cũ đã bỏ để không upload nhầm. Các capture QA nguồn ở Build giữ nguyên.

5 `feature-graphic.png` (1024×500), `app-icon-512.png` (512×512) giữ nguyên bộ brand đã chuẩn bị. `preview-all-locales.png` là ảnh tổng hợp để duyệt, không upload làm screenshot.

## Tái xuất bằng CLI macOS

Chạy HTTP server ở root project: `python3 -m http.server 18670 --bind 127.0.0.1`, rồi `swift design/store-listing/render-design.swift` cũng từ root. Script dùng WKWebView không hiển thị trên màn hình, không thay đổi tab Chrome. Dừng server sau khi xong.
