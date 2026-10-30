# ui-preview — xem UI trên máy thật bằng một lệnh

Dev UI tự kiểm trên thiết bị (R58MC2CSLQZ) rồi sửa lặp, QA độc lập chỉ là cổng cuối.

```bash
# từ bất kỳ đâu; <worktree> là thư mục Unity project của bạn
tools/ui-preview/preview.sh [--keep] <worktree> <screen>
tools/ui-preview/restore.sh [<worktree>] [--force]     # cài lại APK gốc + nhả khoá
```

In ra đường dẫn ảnh `Build/remote-r2-0.3/qa-shots/preview-<screen>-<thời gian>.png` (ở main checkout) và log logcat cùng tên `.log`.
Đọc ảnh, so với `design/`, sửa, chạy lại. Dùng `--keep` khi lặp nhiều lần (giữ khoá + APK preview + server, lần sau nhanh hơn).

## Screen

| screen | mở |
|---|---|
| `home`, `selectworld`, `selectlevel[:w-s]`, `main:w-s-l` | màn tương ứng (chỉ số bắt đầu từ 0; `main:0-0-0` = level 1) |
| `pause`, `howto`, `extra[:ready\|notready]`, `win[:first\|replay]` | dialog trong Main, level đang mở khoá (đổi bằng `;level=w-s-l`) |
| `shop`, `rate`, `language`, `quests[:tab]` | dialog trong Home (`quests:0..4` = Hôm nay/Tuần/Tháng/Sự kiện/Năm) |
| `rewarded:<ready\|loading\|cooldown\|nofill\|offline\|playing\|success\|cancel\|error>` | popup rewarded ở Home, giữ nguyên trạng thái |
| `daily[:fresh\|risk\|safe\|lost]` | dialog Daily ở Home với chuỗi giả (lưu thật; trả lại bằng `reset`) |
| `dailymain[:state]`, `dailywin[:state]` | Main với bàn Daily hôm nay / Win biến thể Daily (dữ liệu Daily và ruby trả lại sau 1 giây) |
| `reset` | trả số ruby, dữ liệu Daily và level đang mở khoá (`;unlock=`) về giá trị trước khi ghi đè |

Hậu tố `;lang=fr` đổi ngôn ngữ giao diện (lưu thật; trả lại bằng `;lang=vi`). Hậu tố `;unlock=w-s-l` đặt level đang mở khoá (ví dụ `selectworld;unlock=1-2-5`; lưu thật, trả lại bằng `reset`). Hậu tố `;ad=reward|close|fail|nofill` dùng quảng cáo rewarded giả (xem xong / đóng sớm / lỗi / không có; chỉ trong bản GAME_DEBUG); đổi chế độ khi đang chạy bằng `adb push` file chứa chữ đó lên `/sdcard/Android/data/<pkg>/files/debug_ad.txt`; logcat có `[DebugAd]`, `[Win] x2 click`, `[DebugOpener] ruby=N`, `[Scene] load`. Hậu tố `;extra=N` đặt tiến độ từ thưởng (lưu thật, trả lại bằng `;extra=0`). Hậu tố `;ruby=N` đặt số ruby (ví dụ `shop;ruby=99999`; nhớ chạy `reset` sau đó). `win` vào level đang mở khoá nên là "thắng lần đầu" (+ruby);
`win:replay` là level đã qua. Win/Extra ghi tiến độ khi mở nên DebugOpener trả lại ruby/tiến độ sau 1 giây; số ruby hiển thị trong ảnh có thể khác số đã lưu.

Tùy chọn: `--settle SEC` (chờ hoạt ảnh trước khi chụp, mặc định 3), `--wait MIN` (chờ khoá, mặc định 20), `--name N` (tên ảnh), `--rebuild` (ép build lại), `--no-build`.

## Quy trình bên trong

1. **Build** trong worktree (Unity 6000.3.9f1 batch, không cần máy): CLR incremental (`BuildDatabase.BuildAndroid -batchIncrementalClr`) khi `.cs/.asmdef/.asmref/ProjectSettings` đổi;
   `BuildOrUpdateAddressable` khi asset khác đổi (so fingerprint cây file, không đổi thì bỏ qua Unity). Tạm thời thêm `GAME_DEBUG` vào define Android để biên dịch `DebugOpener`;
   sau build mọi file tracked mà Unity sửa (meta, InputManager, state bin, ProjectSettings…) được trả về như trước khi chạy. Kết quả ở `<worktree>/Export/` (đã gitignore).
   Lần đầu seed `Export/CLR/0.3/android/Runtime` từ APK gốc (metadata AOT) nên không cần `Generate All`.
2. **Khoá máy** `.git/task-hotupdate/device.lock.json` (python `fcntl`; chờ tối đa `--wait` phút, in ai đang giữ). Khoá thường nhả khi script xong; khoá `--keep` hết hạn sau 30 phút không dùng.
3. **Phục vụ**: `lib/serve.py` http://127.0.0.1:18080 + `adb reverse`. Catalog Addressables được vá để tải bundle từ đó (không đụng R2, không upload).
4. **APK preview** = APK gốc đã repack (`lib/apk_repack.py`): URL catalog → localhost, URL CLR bị vá thành URL https không tới được (cùng độ dài) nên player rơi về DLL nhúng sẵn; zipalign + ký debug key (cùng chữ ký APK gốc).
   Chỉ cài khi chưa có. Khi chỉ đổi CLR, DLL mới được `adb push` vào cache đã xác minh của app (`Android/data/<pkg>/files/clr/<scope>/`), không cài lại.
5. **Mở màn**: `am start --es open "<screen>"` → `DebugOpener` (file `Assets/WordCross/HotUpdate/Debug/DebugOpener.cs`, chỉ biên dịch khi `GAME_DEBUG || DEVELOPMENT_BUILD`) mở thẳng màn/dialog với dữ liệu giả. Cũng đọc `<persistentDataPath>/debug_open.txt` nếu không có extra.
6. `adb exec-out screencap -p` → ảnh. Sau đó (không `--keep`) tự cài lại APK gốc và nhả khoá.

## Thời gian đo được (SM-A515F, Library đã ấm)

| tình huống | thời gian |
|---|---|
| lần đầu (build CLR + AA, repack + cài APK, chụp) | ≈ 115 s |
| lặp, đổi code (CLR build + push cache) | ≈ 48 s |
| lặp, không đổi gì / chỉ đổi màn | ≈ 18–20 s |
| lặp, đổi asset (AA) | ≈ 35–60 s |

Worktree mới cần Library: `cp -c -R <worktree-khác>/Library <worktree>/Library` (APFS clone) tránh import hàng chục phút.

## Hạn chế

- Chỉ 1 máy: người khác chờ khoá; QA/dev khác dùng máy phải gọi `restore.sh` hoặc đợi khoá hết hạn. `--keep` nhớ `restore.sh` khi xong.
- Unity batch của luồng khác trên máy có thể tắt adb server giữa chừng; script thử lại khi cài APK gốc, nếu vẫn lỗi chạy `adb start-server` rồi `restore.sh`.
- Phải đóng Unity Editor đang mở worktree đó (batch không chạy được song song trên cùng project).
- Nội dung preview đi theo APK gốc dcaa9fed (versionCode 6); đổi code AOT/plugin/manifest không thấy được — cần APK mới.
- `adb install -r` giữ dữ liệu, nhưng DebugOpener đổi ruby/tiến độ tạm thời (đã trả lại) — dữ liệu người chơi trên máy vẫn là của máy test.
- Quảng cáo/IAP không được bấm; popup rewarded chỉ để xem trạng thái UI.
- Ảnh có thể chứa overlay của hệ điều hành (bong bóng chat của máy) — không thuộc game.
- Bản release: không có `GAME_DEBUG` nên DLL không chứa `DebugOpener` (đã kiểm bằng `strings`: 0 kết quả; bản preview có 7).
