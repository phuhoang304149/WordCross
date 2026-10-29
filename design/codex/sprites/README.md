# Sprite bo góc WordCross

Nguồn bán kính: `design/Ui/global/css/theme.css`: preview 10px, tile 12px, icon-btn 14px, `--r-m:16px`, `--r-l:24px`; nút thường cao 52px, ruby-pill/icon-btn cao 44px. Tất cả PNG là **RGBA trắng**, nền trong suốt, không viền, không bóng và không blur; tô màu bằng `Image.color`. CoreGraphics xuất trực tiếp ở độ phân giải đích, antialias ở biên, không upscale từ sprite 128px.

**Mật độ nguồn: 4 px/css**, cao hơn mức màn test ≈3,128 px/css khoảng 28%. Chọn 4 để mọi radius/border là số nguyên. Đây là mật độ raster, không phải PPU Unity.

| Tên PNG | Kích thước W×H px | Bán kính css | Bán kính nguồn px | Border L,R,T,B px | Sprite PPU | Image PPU multiplier |
|---|---:|---:|---:|---|---:|---:|
| round-r10.png | 96×96 | 10 | 40 | 40,40,40,40 | 100 | 2,3649 |
| round-r12.png | 112×112 | 12 | 48 | 48,48,48,48 | 100 | 2,3649 |
| round-r14.png | 128×128 | 14 | 56 | 56,56,56,56 | 100 | 2,3649 |
| round-rm.png | 144×144 | 16 | 64 | 64,64,64,64 | 100 | 2,3649 |
| round-rl.png | 208×208 | 24 | 96 | 96,96,96,96 | 100 | 2,3649 |
| pill-52.png | 224×208 | 26 | 104 | 104,104,104,104 | 100 | 2,3649 |
| pill-44.png | 192×176 | 22 | 88 | 88,88,88,88 | 100 | 2,3649 |
| circle.png | 208×208 | 26 ở đường kính 52 css | 104 | 104,104,104,104* | 100 | 2,3649* |

## Cài đặt Unity đề xuất

- Import: Texture Type `Sprite (2D and UI)`, Sprite Mode `Single`, Mesh Type `Full Rect`, pivot giữa; Alpha Is Transparency bật, Compression `None`, Filter `Bilinear`, Wrap `Clamp`, Generate Mip Maps tắt. Giữ kích thước gốc; không để atlas/platform override giảm độ phân giải.
- Canvas `referencePixelsPerUnit = 100`, Sprite PPU = 100; Image Type `Sliced`, Fill Center bật và `pixelsPerUnitMultiplier = 4 / 1,6914 = 2,3649048` cho các rounded rectangle/pill.
- Công thức bán kính canvas: `borderPx / ((SpritePPU / CanvasReferencePPU) × ImageMultiplier)`. Ví dụ r-m: `64 / 2,3649048 = 27,0624` đơn vị canvas = `16 × 1,6914`. Nếu CanvasReferencePPU khác 100, tính lại theo công thức.
- Cấu hình tương đương: Sprite PPU = **236,49048**, CanvasReferencePPU = 100, ImageMultiplier = 1. Chỉ chọn một trong hai cách; không nhân hai lần.
- Nhập kích thước RectTransform theo `css × 1,6914`; không dùng `SetNativeSize` để suy ra kích thước CSS từ ImageMultiplier. Giữ width/height ≥ hai lần bán kính canvas, tránh Unity tự co border khi ảnh quá nhỏ.
- Pill 52: giữ cao **87,9528** canvas, rộng ≥87,9528. Pill 44: cao **74,4216** canvas, rộng ≥74,4216. Chỉ kéo chiều ngang. Tổng border trên+dưới bằng chiều cao nguồn: hàng giữa có UV cao bằng 0, lấy đường giữa trắng để fill; hai bán nguyệt vẫn giữ hình học. Không tăng chiều cao rồi kỳ vọng vẫn là pill; đổi sprite/scale theo chiều cao tương ứng.
- **Circle***: dùng `Simple` + Preserve Aspect và RectTransform vuông; mặc định 87,9528×87,9528 canvas cho đường kính 52 css. ImageMultiplier không tác dụng với `Simple`. Border bảng chỉ dùng nếu bắt buộc `Sliced` tại đúng kích thước 52 css; kéo Sliced lớn hơn sẽ thành rounded rectangle, không còn tròn. Để đổi đường kính, scale đều bằng `Simple`.
- Với màn 1220px ngang, radius hiển thị gần `css × 3,128` pixel. Canvas scale thực tế quyết định pixel cuối; giữ scale đồng đều và tọa độ hợp lý để tránh blur do transform lẻ.

## So sánh

[`compare.png`](compare.png) đối chiếu **hình học CSS border-radius 16px** (đọc token từ theme.css, raster bằng CoreGraphics) với **PNG round-rm kéo 9-slice** tới cùng 340×208px, cùng radius 64px. Hàng dưới crop cùng góc và phóng nearest 4× để thấy dải antialias sát biên. Không dùng lọc blur để làm mượt cạnh. Kiểm pixel trên hai vùng 340×208 cho kết quả **0/70.720 pixel khác nhau** trong renderer này. Đây là bằng chứng PNG giữ hình học góc khi kéo 9-slice; khác biệt raster vẫn có thể xảy ra ở engine khác.

**Giới hạn:** Chrome headless thoát mã 134; WebKit không khởi tạo được dịch vụ do sandbox và hết thời gian chờ. Vì vậy `compare.png` **không phải ảnh CSS do trình duyệt render** và chưa đáp ứng phần bằng chứng trình duyệt của yêu cầu. `compare.html` giữ đối chiếu CSS thật với `border-image` để mở thủ công khi môi trường cho phép; không nhận đây là QA Unity hoặc so với ui_round cũ. Các PNG đã kiểm kích thước/alpha; chưa import hoặc thử kéo trên Unity.

Tái tạo bằng CLI từ gốc repository: `swift design/codex/generate-assets.swift`. Không sửa hoặc import vào `Assets/`.
