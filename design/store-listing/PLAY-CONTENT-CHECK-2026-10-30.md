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
| Email liên hệ | chuikho@gmail.com (hiện trên Console) | USER đã chốt **support@catgaming.asia** — **CHƯA lưu được**, xem 2.1 |
| Điện thoại | +84947153576 | Giữ nguyên, chưa coi là đã xác nhận |
| Website | trống (hiện trên Console) | USER đã chốt **https://catgaming.asia** — **CHƯA lưu được**, xem 2.1 |
| External marketing | bật | Giữ nguyên |

### 2.1 Email/website do USER chốt — kết quả lưu (2026-10-30)

- Đã xác minh record trước khi thao tác: tab ở `.../developers/8227324553133862774/app/4976044463777164379/...`, tiêu đề "Word Crossy".
- Mở Store settings → "Store listing contact details" → Edit; điền email `support@catgaming.asia`, website `https://catgaming.asia`, giữ phone `+84947153576`. Privacy URL không đụng tới.
- **Không lưu được dưới dạng DRAFT.** Thanh dưới của form chỉ có "Discard" và "Save and publish" kèm dòng "Changes made will be published to Google Play immediately"; không có nút "Save" hiển thị (nút Save trong DOM có kích thước 0×0). Vì "Save and publish" xuất bản ngay nên **không bấm**; đã bấm Discard (xác nhận Discard) và đọc lại: email vẫn `chuikho@gmail.com`, website trống, phone không đổi. **Cả 2 trường chưa được lưu.**
- Việc còn lại: cần USER cho phép xuất bản thay đổi liên hệ này ("Save and publish"), hoặc cách khác do USER chọn. Không khẳng định đã gửi/xuất bản gì.

### 2.2 Có gửi review không cần AAB mới không? (chỉ đọc, Publishing overview)

Nguồn: Console → Publishing overview (`.../publishing`), đọc 2026-10-30.
- Hiển thị: "Managed publishing on", "Last published on January 4, 2020", "Changes not yet submitted for review".
- Nút **"Submit 1 change for review" đang disabled** (thuộc tính `disabled=true`). Mục thay đổi liệt kê: App content → Health apps ("Complete Health apps declaration"). Phần "What you've told us" liệt kê Sign in details, Advertising ID, Government apps, Financial features (chưa xuất bản, sẽ được xét khi review).
- Banner "Running quick checks for commonly found issues" (còn "Up to 7 minutes remaining" lúc đọc) và "View 1 issue". Mở ra: **"1 issue found — We found some common issues that are preventing your app from being sent for review. You must address these issues before you can send your changes for review. Incomplete data safety declaration (Policy)"** với nút "Complete declaration".
- Kết luận cho app này: Console **không** nêu yêu cầu AAB mới như một blocker của việc gửi; blocker hiển thị duy nhất là **Data safety chưa khai**. Nghĩa là thay đổi chỉ-metadata/policy về nguyên tắc có thể gửi không cần AAB (Console không đòi bundle trong danh sách lỗi), nhưng **hiện chưa gửi được** vì Data safety. Chưa rõ nút có mở khoá sau khi khai Data safety hay không, và quick checks chưa kết thúc lúc đọc, nên có thể xuất hiện thêm lỗi.
- Phân biệt: khả năng review metadata-only nói chung là tính năng chung của Play Console; riêng app này còn các yếu tố **chưa kiểm chứng**: trạng thái "App removed" (02/09/2021, lý do gốc chưa đọc) có thể buộc appeal/resubmit; production track Inactive; cảnh báo artifact cũ. Không có thông báo nào trên Publishing overview nói các yếu tố này chặn nút. Chưa bấm Submit/Publish; không upload AAB.
- Hai điểm "Save and publish" (Store settings) và "Submit for review" (Publishing overview) là hai cơ chế khác nhau; Store settings hiện chỉ cho xuất bản ngay.
- Vẫn pending: nhóm tuổi (Target audience), IARC, Privacy policy URL/nội dung, Data safety.

## 3. App content (Console: còn 1 mục cần xử lý — Data safety)

| Mục | Trạng thái | Căn cứ |
|---|---|---|
| Sign in details (App access) | **Đã lưu: "No" — không hạn chế truy cập** (tải lại: radio "No" được chọn, cảnh báo biến mất) | Không có mã đăng nhập/UI đăng nhập trong game (`Assets/WordCross`, `Assets/WordChef`, `Assets/_Scripts` không tham chiếu AuthenticationService/Google/Apple sign-in; chỉ `catgamingmodule/DebugButton/PlayManager.cs` và module Social). IAP chỉ là mua tuỳ chọn, không khoá nội dung chính. |
| Advertising ID | **Đã lưu: "Yes", mục đích "Advertising or marketing"** (tải lại vẫn đúng) | APK release `.../release/wordcrossy_1.0.0_202001035_RELEASE.apk` khai `com.google.android.gms.permission.AD_ID`, `ACCESS_ADSERVICES_AD_ID`; AAB có `play-services-ads` và `play-services-ads-identifier`. Chỉ tích mục đích có chứng cứ trực tiếp (quảng cáo); không tích Analytics/Fraud prevention vì chưa có chứng cứ. |
| Government apps | **Đã lưu: "No"** (tải lại: No) | Định nghĩa Console: "developed by or on behalf of a government". Chủ tài khoản là cá nhân (Personal account), game giải đố chữ. |
| Financial features | **Đã lưu: "My app doesn't provide any financial features"**, bước Documentation không yêu cầu thêm; mục biến khỏi danh sách cần xử lý | Danh sách của Console (ngân hàng, vay, ví, giao dịch, tiền mã hoá, bảo hiểm…) không có mục nào khớp. IAP qua Play Billing và xu/vật phẩm ảo trong game không thuộc các loại này. |
| Health apps | **Đã lưu: "My app does not have any health features"**, bước Regional requirements không yêu cầu | Danh sách tính năng sức khoẻ của Console không khớp; nội dung listing 5 locale không có từ khoá sức khoẻ/brain training/stress (đã grep). |
| Ads | Đã khai "Có quảng cáo" (2019) | Khớp: Google Mobile Ads 11.6.0. Không đổi. |
| Privacy policy | **DRAFT đã đổi sang URL mới (30/10/2026)**: `https://data.catgaming.asia/WordCross/privacypolicy/privacy-policy.html`; tải lại thấy giá trị đã lưu | Xem mục 3.2 và 3.3. |
| Content rating | **IARC mới 30/10/2026 11:35, email support@catgaming.asia** (đã Save; xem 3.5) | Bản 2019 bị Google coi là sai (thư 02/09/2021). Rating thuật toán: Google Play/IARC 12+, ESRB Teen, PEGI 12, USK all ages |
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

### 3.3 Privacy policy mới (USER chốt 2026-10-30)

- Nguồn: `legal/privacypolicy/index.html` (English, HTML tĩnh, không cookie/tracking). Mẫu tham chiếu: R2 `catgaming/wordle/privacypolicy/` (chỉ `privacy-policy.html`, `delete-data.html`; URL thư mục trả 200 `application/octet-stream`, không route index) → dùng đường dẫn file tường minh.
- R2 (bucket `catgaming`, profile CLI sẵn có): `WordCross/privacypolicy/privacy-policy.html` và `WordCross/privacypolicy/index.html` (cùng nội dung). Khoá phân biệt hoa/thường, khác prefix `wordcross/` của OTA; không đụng CLR/AA/OTA/Wordle.
- URL public đã kiểm: `https://data.catgaming.asia/WordCross/privacypolicy/privacy-policy.html` → 200, `text/html; charset=utf-8`, sha256 khớp file local: `167569a7f7a7d18bea7e079c812df9341cd3b7b5d5a9bdfdda2e4c2aa88a8f8c`. Lần upload đầu lệch hash vì Cloudflare tự đổi email thành script bảo vệ email; đã dùng `&#64;`/`&#46;` như policy Wordle.
- Play Console: Privacy policy URL đã Save (DRAFT), reload đọc lại đúng URL. Chưa gửi review/AAB/rollout.
- Nguồn chứng cứ: Google Mobile Ads data disclosure (developers.google.com/admob/android/privacy/play-data-disclosure): SDK tự thu thập/chia sẻ IP, tương tác sản phẩm, chẩn đoán, ID thiết bị/tài khoản cho quảng cáo, phân tích, chống gian lận. Luồng IAP: `CATGAMING_NAKAMA` **không** có trong define của `ProjectSettings` WordCross (chỉ có trong config game khác), nên `PurchaseService` đi nhánh `PendingPurchase` cục bộ → `Purchaser` cộng thưởng và `ConfirmPurchase`; không có validate receipt qua server, `MainModule.InitServer` không chạy Nakama. Vậy không có backend nhận ID thiết bị/dữ liệu mua hàng.
- Điểm Data safety mới xác minh (chưa khai, task này không đổi Data safety): (1) điểm "backend Nakama" ở 3.1 đã có câu trả lời: không dùng; (2) AdMob: IP, ID thiết bị (Advertising ID), tương tác quảng cáo, chẩn đoán – chia sẻ với Google. Mua hàng do Google Play xử lý.
- Còn thiếu thật sự: cấu hình thông điệp consent/GDPR trên AdMob Console (chưa mở nên policy chỉ nói "tuỳ cấu hình"); game không có mục Privacy options trong UI (chỉ có hàm trong `AdmobService`); chưa chạy runtime để xác nhận `CFirebase.LogEvent` là stub và `unity-ads` không khởi tạo; không nêu thời hạn lưu giữ dữ liệu của Google; tên hiển thị trong policy là "Word Crossy" theo hồ sơ cũ.

### 3.4 Data safety + URL policy mới + Contact (USER chốt 2026-10-30, lượt sau)
- Policy: đổi 2 chữ "coins" → "rubies" trong `legal/privacypolicy/index.html`; upload R2 prefix chữ thường `wordcross/privacypolicy/{privacy-policy,index}.html` (`text/html; charset=utf-8`). Public `https://data.catgaming.asia/wordcross/privacypolicy/privacy-policy.html` và `index.html` đều 200, sha256 `59789d36525a11fb153350fddeb848ba7808201370274b95fb84c85556a53094` khớp file local. Play Privacy policy URL đã Save sang URL chữ thường, reload đọc lại đúng. Hai object chữ hoa `WordCross/privacypolicy/*` chưa xoá trong lượt này (xem summary).
- Contact: Store settings → Save and publish (chỉ contact, USER cho phép): email `support@catgaming.asia`, website `https://catgaming.asia`, phone `+84947153576` giữ nguyên; reload đọc lại đúng.
- Data safety đã Save ("Change saved"): có thu thập/chia sẻ; mã hoá khi truyền = Yes (Google nêu dữ liệu Mobile Ads SDK mã hoá TLS); không tạo tài khoản; không đăng nhập tài khoản ngoài; không có cách yêu cầu xoá dữ liệu (No, không tick dễ dãi). 4 loại, mỗi loại Collected + Shared, không ephemeral, bắt buộc, mục đích Analytics + Advertising or marketing + Fraud prevention: Approximate location (IP), App interactions, Diagnostics, Device or other IDs. Nguồn: Google Mobile Ads data disclosure; `CATGAMING_NAKAMA` không bật; Firebase SDK đã gỡ (`FirebaseControl.cs` rỗng) nên không khai Analytics riêng. Không khai Purchase history (Google Play xử lý thanh toán, game không gửi receipt).
- Chưa xác minh: Ad ID thực tế là tuỳ chọn theo Google nhưng khai "bắt buộc" cho an toàn; unity-ads chưa trace init; nhóm tuổi 13+ giữ nguyên, IARC chưa làm.

### 3.5 Content rating, Device IDs, Policy status (2026-10-30, lượt sau)

**Phạm vi:** record `developers/8227324553133862774/app/4976044463777164379`, tab 2077603181 / window 2077603180 (đã xác minh bằng URL). Chrome chỉ qua osascript, không screenshot/OAuth. Không bấm Submit/Publish/Rollout/Remove changes/appeal.

**Policy status App Removed (lý do gốc đọc được).** Policy status → issue "App removed" → "View email" mở trang Console `policy-emails/developers/…?id=…` (có nội dung, không phải mail client). Thư gửi chuikho@gmail.com: "Your app's content rating is incorrect" (Application Ratings + App screenshot en_US, 2 ảnh đính kèm, không tải). Trạng thái Removed từ 02/09/2021; yêu cầu: sửa rồi submit bản cập nhật, làm lại rating questionnaire. Không khẳng định phải appeal: Console còn nút "Submit an appeal" nhưng chưa dùng. Cùng trang còn 3 issue về target API 35/36 và Billing 8 (cũ, thuộc artifact 2019-2020; xem mục 5).

**1. Content rating (IARC).** Rating 2019 là nguyên nhân gỡ app nên không còn hợp lệ; đã làm questionnaire mới (Game), email `support@catgaming.asia`, bước cuối của Console là "Save" (không có Submit riêng), Save xong overview hiện "Submitted October 30, 2026, 11:35 AM", certificate ID "-" (chưa cấp lúc đọc).
- Trả lời: Violence No, Fear No, Sexuality No, Gambling No. **Language Yes**: minor profanities + moderate swearing, cả hai *Rarely* (word list có "damn" làm đáp án, "porn" đáp án, "whore" là từ hợp lệ trong `Levels/*.asset`; không có discriminatory/sexual expletives). **Controlled Substance Yes**: Alcohol + Tobacco, mức Reference, Rarely (từ "beer, wine, alcohol, cigar"; từ "drug" mơ hồ nên không tick drugs). Crude Humor No. **Digital Purchases Yes**: chỉ Purchases of digital goods (rubies/noads); loot box No, trade No, cash/NFT không tick. Misc: không chat/UGC, không chia sẻ vị trí, không Nazi/Korea/terror/crime đều No.
- Khác bản cũ: thêm Language, Alcohol/Tobacco reference (word list), Digital Purchases (IAP). Mức Language/Alcohol là diễn giải theo word list; nếu USER muốn bỏ "whore"/"damn" khỏi list thì khai lại sẽ nhẹ hơn.
- Rating thuật toán, không ép 13+: IARC Generic/Google Play 12+, ESRB Teen, PEGI 12, USK all ages, ACB Parental Guidance, ClassInd 14+, GRAC 15+, DGSC PG15, Gmedia 16.
- **Target audience giữ nguyên, không lưu lại:** 13-15, 16-17, 18 and over được chọn; 5 and under / 6-8 / 9-12 không chọn → không Families dưới 13. 13+ khác với IARC (12+): không mâu thuẫn.

**2. Device or other IDs: giữ "bắt buộc".** Nguồn: Google Mobile Ads data disclosure (cập nhật 28/10/2026): nhóm "Device and Account identifiers" gồm Android ad ID **và app set ID** (+ ID tài khoản nếu có), thu/chia sẻ tự động cho advertising/analytics/fraud. Chỉ **ad ID** được nêu là optional (có thể chặn qua manifest, user reset); app set ID không có cơ chế tắt nào được nêu. APK release 1.0.0 khai `AD_ID` + `ACCESS_ADSERVICES_AD_ID`, AAB có `play-services-appset` và `-ads-identifier` nên không gỡ ad ID. Vậy nhóm là bắt buộc vì còn app set ID, không phải vì cảm giác an toàn; Data safety không đổi, không Save lại.
- unity-ads: AAB có `com.unity3d.ads:unity-ads:4.16.5` và `unityads-adapter` do `Editor/ISUnityAdsAdapterDependencies.xml` kéo về. Code init nằm trong asmdef `CatGaming.Module.LevelPlay` có `defineConstraints: CATGAMING_LEVELPLAY`; define này **không có** ở Android trong `ProjectSettings` (chỉ `CATGAMING_ADMOB`), `manifest.json` không có package LevelPlay → không có đường init trong code game. Thư viện có mặt nhưng không có bằng chứng được khởi tạo; không thay đổi khai báo.
- `CFirebase.LogEvent`: chỉ ở `GPGSController.cs` bên trong `#if USE_GPGS`, `USE_GPGS` không có trong define Android, và không có lớp `CFirebase` nào trong repo → không biên dịch vào bản build. Nakama không bật (đã xác minh trước, không hỏi lại).
- Policy `legal/privacypolicy/index.html` (R2 chữ thường) không mâu thuẫn với kết luận trên; không sửa/đăng lại.

**Review state sau các Save (chỉ đọc, 11:3x).** Publishing overview: "Changes in review" gồm Privacy policy URL, Data safety, Health apps. Content Rating (questionnaire mới) nằm ở "Changes not yet submitted for review – Submit 1 change for review", quick checks "Up to 14 minutes remaining", Console ghi sẽ tự gửi khi checks xong. Policy status: "Update in review". Worker không bấm Submit; chưa thấy kết quả sau quick checks.

**Remaining:** (a) xem Content Rating chuyển sang in review hay phát sinh lỗi sau quick checks; (b) certificate ID chưa hiện; (c) USER quyết giữ/bỏ từ nhạy cảm trong word list nếu muốn rating thấp hơn; (d) screenshot sạch (Codex); (e) appeal chỉ cân nhắc nếu review từ chối.


## Cập nhật cuối (Claude, Browser tích hợp, 2026-10-30)

- **Đã PUBLISH** (Publishing overview: "Last published on October 30, 2026"): Privacy policy URL, Data safety, Health apps. Managed publishing vẫn ON. App vẫn "Removed by Google"; chưa phát hành bản app/reskin nào.
- **Chưa gửi**: Content Rating (questionnaire mới) nằm ở "Changes not yet submitted for review". Giữ lại chưa Submit vì điểm IARC textual sexual references (porn/whore) chưa được xác minh theo định nghĩa.
- **Screenshot (draft listing, đọc bằng DOM)**: en-US 5, fr-FR 5, pt-BR 5, vi 5, es-ES 4 (thiếu 1). Chưa xác nhận thứ tự/nội dung từng ảnh và chưa bấm Save/Publish listing.
- **Chặn upload**: Browser tích hợp không có tool chọn file; fetch/img từ localhost bị CSP chặn; osascript không có quyền assistive access. Cần USER cấp quyền Accessibility cho osascript hoặc upload tay ảnh còn thiếu của es-ES.
