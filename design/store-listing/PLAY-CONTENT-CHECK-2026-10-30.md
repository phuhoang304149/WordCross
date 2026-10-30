# Kiểm tra hồ sơ Google Play — Word Crossy (com.PHltd.wordcrossy) — 2026-10-30

Phạm vi: chuẩn bị như app mới trên record hiện hữu. **Chưa gửi review, chưa submit AAB, không rollout/appeal/email.** Mọi thay đổi bên dưới là **DRAFT** (Console hiện "Draft changes / Changes ready to send for review"; Managed publishing đang bật). Không có mục nào ở trạng thái SUBMITTED.

## 1. Store listing (DRAFT đã lưu, đã đọc lại sau khi tải lại trang)

| Locale | Tên | Mô tả ngắn | Mô tả đầy đủ | Icon | Feature graphic | Phone screenshot |
|---|---|---|---|---|---|---|
| en-US (mặc định) | 25 ký tự ✓ | 79 ✓ | 1363 ✓ | app-icon-512.png (mới) ✓ | feature-graphic.png mới ✓ | **THIẾU — chỉ có 8 ảnh cũ** |
| vi | 20 ✓ | 73 ✓ | 1316 ✓ | dùng chung ✓ | feature-vi.png ✓ | thiếu (như trên) |
| fr-FR | 25 ✓ | 80 ✓ | 1620 ✓ | dùng chung ✓ | feature-fr-FR.png ✓ | thiếu |
| es-ES | 29 ✓ | 76 ✓ | 1564 ✓ | dùng chung ✓ | feature-es-ES.png ✓ | thiếu |
| pt-BR | 29 ✓ | 77 ✓ | 1465 ✓ | dùng chung ✓ | feature-pt-BR.png ✓ | thiếu |

- Số ký tự khớp các file `design/store-listing/<locale>/`. Nội dung lấy từ file nguồn, không sửa chữ.
- Icon: listing chỉ còn 1/1 icon mới (đã gỡ tham chiếu icon cũ; asset cũ vẫn nằm trong thư viện, không xoá).
- Feature graphic en-US: đã thay ảnh cũ bằng ảnh mới. Ảnh cũ vẫn trong thư viện.
- **Phone screenshot: cả 5 locale đang hiển thị 8 ảnh 1080×1920 tải lên 14/06/2025 (bản cũ, không phản ánh bản hiện tại).** Không upload 25 `screenshot-draft-*` (overlay, nháp). Cần ảnh sạch từ Codex trước khi coi listing là sẵn sàng. Tablet/Desktop/XR: không bắt buộc, để trống.
- Lỗi đã gặp và đã sửa: script điền chữ lệch ô (tên vào ô mô tả ngắn…) làm Console báo "Some languages have errors"; đã điền lại đúng ô cho vi/es-ES/pt-BR, tải lại thấy hết lỗi.

## 2. Store settings

| Mục | Hiện tại | Trạng thái |
|---|---|---|
| Loại / danh mục | Game / Puzzle | Có |
| Tags | trống (gợi ý: Puzzle, Word) | **Thiếu** — cần USER chọn |
| Email liên hệ | chuikho@gmail.com | Có nhưng là email cũ (2019) — cần USER xác nhận còn dùng |
| Điện thoại | +84947153576 | Có — USER xác nhận |
| Website | trống | Thiếu (tuỳ chọn) |
| External marketing | bật | Giữ nguyên |

## 3. App content

Console: 6 mục cần xử lý, 4 mục đã khai (từ 2019).

| Mục | Trạng thái Console | Ghi chú |
|---|---|---|
| Privacy policy | Đã khai 25/05/2019 | URL là **Google Docs** (`docs.google.com/document/d/1SSBZ5nQ…/edit?usp=sharing`), curl trả 200. Console yêu cầu trang web chuẩn, không phải tài liệu chỉnh sửa được; chưa xác minh nội dung có phủ AdMob, IAP, thông báo. **Cần URL chính thức từ USER.** |
| Ads | Đã khai "Có quảng cáo" (2019) | Khớp project: Google Mobile Ads (AdMob) 11.6.0 |
| Content rating | IARC hoàn tất 27/05/2019, email chuikho@gmail.com | Bản cũ; app đã đổi nhiều (IAP, daily). Cần làm lại questionnaire — USER trả lời/xác nhận, tôi không tự khai |
| Target audience & content | Đã khai 25/05/2019 | Console chặn mở lại vì **Sign in details chưa khai** |
| Data safety | **Quá hạn** (hạn 23/08/2022), chưa khai | Chưa điền. SDK thực tế: AdMob, Unity IAP 5.4.4, Unity Mobile Notifications 2.4.3, module Unity Analytics 1.0.0; Firebase đã gỡ. Cần USER xác nhận Unity Analytics có bật gửi dữ liệu không và cấu hình AdMob (personalized ads) trước khi khai |
| Sign in details (App access) | Chưa khai | Dự kiến "không cần đăng nhập" — project không có đăng nhập; cần USER xác nhận rồi khai |
| Advertising ID | Chưa khai | Project dùng AdMob nên khả năng cao là "Có"; chưa lưu, chờ USER xác nhận |
| Financial features | **Quá hạn**, chưa khai | Chưa lưu (khai báo pháp lý) — USER xác nhận "không có tính năng tài chính" |
| Health apps | **Quá hạn**, chưa khai | Chưa lưu — USER xác nhận "không" |
| Government apps | Chưa khai | Chưa lưu — USER xác nhận "không" |
| Account deletion | Không áp dụng (không có tài khoản) | Không thấy mục trên Console |

Tôi không điền các khai báo nhạy cảm theo suy đoán.

## 4. Quốc gia, giá, xuất bản

- Production track: **Inactive**, release mới nhất 2.0, 177 quốc gia/vùng, 2 installs. Không đổi.
- Pricing: không đọc/không đổi.
- Publishing overview: Managed publishing **bật**; lần cuối xuất bản 04/01/2020; có "Changes not yet submitted for review". Không bấm gửi.
- Policy: app **Removed by Google** (02/09/2021), Dashboard báo 4 policy issues.

## 5. Blocker (để sau, không sửa code/build trong lượt này)

- App bị gỡ 02/09/2021: cần xử lý với Google (appeal/liên hệ) — USER quyết, chưa làm.
- Console báo lỗi API 36+ và Billing 8+ cho bản AAB: cần build lại AAB sau. Build gần nhất `Build/remote-r2-1.0.0/release/wordcrossy_1.0.0_202001035_RELEASE.aab` (30/10 10:20), **chưa upload**.

## 6. Môi trường để Codex chuẩn bị ảnh (chỉ đọc)

- `adb devices`: `emulator-5554` (sdk_gphone64_arm64) đang có com.PHltd.wordcrossy **versionName 1.0.0, versionCode 202001035, targetSdk 36**; trong devpool đang `working|tmpui` (thuộc task khác — **không dùng**). `ZY22LHV7PJ` (Motorola edge 60 fusion): offline. Không có lock guard `.android-devices.lock` lúc kiểm.
- APK release mới: `Build/remote-r2-1.0.0/release/wordcrossy_1.0.0_202001035_RELEASE.apk` (30/10 10:19). Có thêm bản DEVELOPER_LOG 29/10.
- Screenshot runtime sạch: **chưa có** gói sạch của bản hiện tại. `Build/remote-r2-0.3/qa-shots/` có `home-final-1.0.0.png` và các `l10n-final-*` (gồm status bar/gamepad, một số là UI cũ như `IMAGE-ASSETS.md` đã ghi). Chụp lại sẽ cần máy trống và claim devpool.

## 7. Câu hỏi cần USER

1. Email liên hệ công khai: giữ chuikho@gmail.com hay đổi? Website có không?
2. URL privacy policy chính thức dạng trang web (hiện là Google Docs)?
3. Tags (đề xuất: Puzzle, Word)?
4. Content rating: ai trả lời questionnaire IARC mới và email nhận chứng chỉ?
5. Target audience (nhóm tuổi): USER chọn — tôi không mặc định.
6. Xác nhận cho Data safety: Unity Analytics có thu thập dữ liệu không; AdMob có quảng cáo cá nhân hoá không; có thu thập thêm gì ngoài Advertising ID/purchase?
7. Xác nhận khai: không cần đăng nhập, không financial, không health, không government, dùng Advertising ID.
8. Hướng xử lý app bị gỡ (appeal) và thời điểm build AAB mới.
