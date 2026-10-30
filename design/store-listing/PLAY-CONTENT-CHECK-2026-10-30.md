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
| Tags | **Puzzle, Word** — đã chọn trong Console (cả hai nằm trong "Suggested tags", đúng game chữ) và Save, tải lại vẫn thấy | DRAFT đã lưu |
| Email liên hệ | chuikho@gmail.com | Có (email 2019); USER đang được hỏi riêng |
| Điện thoại | +84947153576 | Có; USER đang được hỏi riêng |
| Website | trống | Chưa có; USER đang được hỏi riêng |
| External marketing | bật | Giữ nguyên |

## 3. App content (Console: còn 1 mục cần xử lý — Data safety)

| Mục | Trạng thái | Căn cứ |
|---|---|---|
| Sign in details (App access) | **Đã lưu: "No" — không hạn chế truy cập** (tải lại: radio "No" được chọn, cảnh báo biến mất) | Không có mã đăng nhập/UI đăng nhập trong game (`Assets/WordCross`, `Assets/WordChef`, `Assets/_Scripts` không tham chiếu AuthenticationService/Google/Apple sign-in; chỉ `catgamingmodule/DebugButton/PlayManager.cs` và module Social). IAP chỉ là mua tuỳ chọn, không khoá nội dung chính. |
| Advertising ID | **Đã lưu: "Yes", mục đích "Advertising or marketing"** (tải lại vẫn đúng) | APK release `.../release/wordcrossy_1.0.0_202001035_RELEASE.apk` khai `com.google.android.gms.permission.AD_ID`, `ACCESS_ADSERVICES_AD_ID`; AAB có `play-services-ads` và `play-services-ads-identifier`. Chỉ tích mục đích có chứng cứ trực tiếp (quảng cáo); không tích Analytics/Fraud prevention vì chưa có chứng cứ. |
| Government apps | **Đã lưu: "No"** (tải lại: No) | Định nghĩa Console: "developed by or on behalf of a government". Chủ tài khoản là cá nhân (Personal account), game giải đố chữ. |
| Financial features | **Đã lưu: "My app doesn't provide any financial features"**, bước Documentation không yêu cầu thêm; mục biến khỏi danh sách cần xử lý | Danh sách của Console (ngân hàng, vay, ví, giao dịch, tiền mã hoá, bảo hiểm…) không có mục nào khớp. IAP qua Play Billing và xu/vật phẩm ảo trong game không thuộc các loại này. |
| Health apps | **Đã lưu: "My app does not have any health features"**, bước Regional requirements không yêu cầu | Danh sách tính năng sức khoẻ của Console không khớp; nội dung listing 5 locale không có từ khoá sức khoẻ/brain training/stress (đã grep). |
| Ads | Đã khai "Có quảng cáo" (2019) | Khớp: Google Mobile Ads 11.6.0. Không đổi. |
| Privacy policy | Đã khai 25/05/2019 | Xem mục 3.2. |
| Content rating | IARC 27/05/2019 | Bản cũ. Không khai lại theo suy đoán; USER đang được hỏi riêng. |
| Target audience & content | **Chưa đổi.** Giá trị cũ đọc được: **13-15, 16-17, 18 and over** (không chọn 5 and under / 6-8 / 9-12) | Chỉ đọc; thoát khỏi trang mà không lưu. Nhóm tuổi phục vụ do USER quyết (đang hỏi riêng). |
| Data safety | **Chưa khai** (quá hạn từ 23/08/2022) | Xem mục 3.1: còn điểm chưa xác minh. |
| Account deletion | Không thấy mục trên Console | Không có tài khoản người dùng trong game. |

Ghi chú: "Need attention" trên Console giảm từ 6 xuống 1 (Data safety). Các lần Save chỉ vào DRAFT; không gửi review.

### 3.1 Data safety — bảng nguồn

Có SDK/module không đồng nghĩa đã thu dữ liệu. Chỉ kiểm chọn lọc, không audit toàn source, không chạy runtime.

| Hạng mục | Chứng cứ đã đọc | Kết luận | Chưa biết |
|---|---|---|---|
| AdMob (Google Mobile Ads 11.6.0) | AAB `dependencies.pb` có `play-services-ads`, `-ads-identifier`, `-appset`; APK có AD_ID, ACCESS_ADSERVICES_*; defines `prod` có `CATGAMING_ADMOB`; `GoogleMobileAdsSettings.asset` có AppId, không tắt khởi tạo | SDK được build vào và hoạt động theo mặc định → có thu/chia sẻ Advertising ID, ID thiết bị khác, dữ liệu chẩn đoán/tương tác quảng cáo với Google | Danh mục chính xác Google nêu trong tài liệu "Google Mobile Ads data disclosure" chưa đối chiếu lúc này |
| UMP / cá nhân hoá quảng cáo | `catgamingmodule/Monetization/AdmobService.cs`: `ConsentInformation.Update` + `ConsentForm.LoadAndShowConsentFormIfRequired`; `UMP_AVAILABLE` có trong define `prod`. Code không đặt `NonPersonalized`, `TagForChildDirected`, `TagForUnderAge`, `MaxAdContentRating` | Có luồng consent (hiện theo cấu hình message của AdMob). Không có cờ ép quảng cáo không cá nhân hoá | Cấu hình GDPR/consent message trên AdMob Console (chưa mở) |
| Unity Analytics | Chỉ có module built-in `com.unity.modules.unityanalytics 1.0.0` trong `manifest.json`; `cloudServicesEnabled` = 0; không thấy lệnh khởi tạo `UnityServices`/`AnalyticsService` trong code game (chỉ khớp 1 file trong `Monetization/LevelPlay`, không có dòng Analytics thực) | **Không có chứng cứ thu thập** từ Unity Analytics | Chưa chứng minh tuyệt đối; AAB còn `com.unity3d.ads:unity-ads` trong dependencies (không rõ có khởi tạo hay không) |
| Firebase / `CFirebase.LogEvent` | `WordChef/.../GPGSController.cs` còn gọi `CFirebase.LogEvent`; không tìm thấy định nghĩa lớp `CFirebase` trong `Assets` ngoài Plugins; manifest không còn Firebase | Có thể là stub/không hoạt động | Chưa xác minh runtime |
| Mua hàng (Unity IAP 5.4.4, Play Billing 9.0.0) | `PurchaseService.cs` gọi RPC `Validate*Purchase`/`Restore*` qua `NakamaRpc` (gửi dữ liệu mua lên server backend) | Dữ liệu mua hàng (purchase history) có khả năng gửi tới server của chủ app | Địa chỉ/backend thực, có kèm ID thiết bị/ID người dùng hay không |
| Tài khoản / telemetry | `Nakama*Authentication` chỉ thấy trong `DebugButton/PlayManager.cs` và `catgamingmodule/Social`; game không có UI đăng nhập | Không có tài khoản do người dùng tạo | Release có tự đăng nhập thiết bị (device ID) lên Nakama hay không |
| Thông báo | `POST_NOTIFICATIONS`, Unity Mobile Notifications 2.4.3 (thông báo cục bộ); không thấy push token | Không có chứng cứ thu dữ liệu | — |

**Kết luận: chưa lưu Data safety.** Phần có chứng cứ (AdMob: Advertising ID/Device IDs/diagnostics chia sẻ với Google) đã rõ; còn hai điểm cần câu trả lời thật trước khi khai: (1) backend Nakama có nhận ID thiết bị/dữ liệu mua hàng; (2) cấu hình consent AdMob. Mở Console khai dở sẽ không có lợi, nên để nguyên.

### 3.2 Privacy policy (Google Docs)

- URL: `https://docs.google.com/document/d/1SSBZ5nQ2JFsDu5nN9fDbQFkww_hmSi_jYmbcTLD8qjM/edit?usp=sharing`. Đọc được qua Chrome (`/mobilebasic`, 4630 ký tự). Dạng `/edit` **không tự chứng minh** ai cũng sửa được; quyền "edit" thực của người xem chưa kiểm tra và không kết luận invalid vì lý do này. Hạn chế: không thể xác minh tính công khai cho người chưa đăng nhập, vì tab Chrome đang dùng phiên đã đăng nhập (curl trước đó trả 200).
- Nội dung đã đọc: tên app "Word crossy", đơn vị "phuhoang304149", email liên hệ chuikho@gmail.com; nhắc "Free app"; không dưới 13 tuổi.
- Lý do thực tế cần sửa (không phải vì định dạng URL): liệt kê **Firebase Analytics, Facebook, Fabric Crashlytics, Matomo, Clicky, Flurry** — không có trong project hiện tại; **không** nhắc Unity IAP/mua hàng và dữ liệu gửi server, UMP/consent, Advertising ID cụ thể, thông báo; tên đơn vị không khớp tên nhà phát triển trên Console (CatGaming). Chưa tạo trang web hay đăng chính sách mới: thiếu email/website chính thức và kết luận Data safety.

## 4. Quốc gia, giá, xuất bản

- **Pricing (đã đọc, chỉ đọc): ứng dụng Free; Console ghi "Your app is available for free on Google Play, and can't be changed to paid".** Không đổi giá/quốc gia.
- Production track: **Inactive**, release mới nhất 2.0, 177 quốc gia/vùng, 2 installs. Không đổi.
- Publishing overview: Managed publishing **bật**; lần cuối xuất bản 04/01/2020; có "Changes not yet sent for review". Không bấm gửi.
- Policy: app **Removed by Google** (02/09/2021), Dashboard báo 4 policy issues; trang pricing hiển thị banner "App removed". Chi tiết trên Console chỉ chung chung, **chưa có lý do gốc**.

## 5. Blocker / điểm cần xác định

- **App Removed:** cần xác định lý do từ thông báo Google (email/Policy status) trước khi quyết resubmit hay appeal. Không khẳng định appeal là bắt buộc.
- **Tương thích API 36/Billing 8+ (không kết luận cần rebuild):** Console cảnh báo trên artifact AAB cũ đã upload (2019-2020). Artifact hiện tại đọc từ file, **không build/cài**:
  - APK `Build/remote-r2-1.0.0/release/wordcrossy_1.0.0_202001035_RELEASE.apk` (aapt): versionName 1.0.0, versionCode 202001035, **compileSdk 36, targetSdk 36**, minSdk 25.
  - Manifest có `com.google.android.play.billingclient.version` = **9.0.0** (≥ 8); AAB (30/10 10:20) có Play Billing client resources và AGP 8.10.0, Unity 6000.3.9f1.
  - Kết luận: bản hiện tại **có vẻ đã đáp ứng** mức API 36 và Billing ≥ 8; cảnh báo Console nhiều khả năng thuộc bản cũ. **Chưa xác minh** bằng pre-launch/upload thật (chưa upload AAB) nên không khẳng định cần hay không cần rebuild.

## 6. Môi trường để Codex chuẩn bị ảnh (chỉ đọc)

- `adb devices`: `emulator-5554` (sdk_gphone64_arm64) đang có com.PHltd.wordcrossy **versionName 1.0.0, versionCode 202001035, targetSdk 36**; trong devpool đang `working|tmpui` (thuộc task khác — **không dùng**). `ZY22LHV7PJ` (Motorola edge 60 fusion): offline. Không có lock guard `.android-devices.lock` lúc kiểm.
- APK release mới: `Build/remote-r2-1.0.0/release/wordcrossy_1.0.0_202001035_RELEASE.apk` (30/10 10:19). Có thêm bản DEVELOPER_LOG 29/10.
- Screenshot runtime sạch: **chưa có** gói sạch của bản hiện tại. `Build/remote-r2-0.3/qa-shots/` có `home-final-1.0.0.png` và các `l10n-final-*` (gồm status bar/gamepad, một số là UI cũ như `IMAGE-ASSETS.md` đã ghi). Chụp lại sẽ cần máy trống và claim devpool.


## 7. Cần USER quyết (đã rút gọn)

USER đang được hỏi riêng, chưa có trả lời: email/số điện thoại/website privacy chính thức; nhóm tuổi phục vụ và có trẻ dưới 13 hay không; (kéo theo) IARC.

Còn lại thật sự cần quyết/cung cấp:
1. Backend Nakama có nhận ID thiết bị/dữ liệu mua hàng không, và cấu hình consent AdMob — để khai Data safety.
2. Hướng xử lý app Removed sau khi đọc lý do gốc trong thông báo Google.
3. Screenshot sạch (Codex/chủ ảnh) trước khi coi listing sẵn sàng.
