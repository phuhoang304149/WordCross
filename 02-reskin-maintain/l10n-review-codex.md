# Rà dịch WordCross — đề xuất Codex

Đã rà đủ **25 key × 5 ngôn ngữ = 125 ô** từ `l10n-review.md`, theo ngữ cảnh sử dụng đã mô tả. Bảng dưới chỉ liệt kê ô nên sửa; 107 ô còn lại giữ nguyên. Đây là đánh giá biên tập ngôn ngữ của Codex, không phải kết quả duyệt của người bản ngữ độc lập. Tiếng Bồ Đào Nha giữ biến thể **pt-BR** theo “você”, “sua”, “bônus” của nguồn.

Tên tháng và thứ viết hoa trong tiêu đề/đầu cột được chấp nhận như quy ước trình bày UI; không đổi hàng loạt chỉ vì cách viết trong câu thường dùng chữ thường. Các danh sách tháng/tháng ngắn/thứ giữ nguyên đủ **12/12/7 mục**, đúng thứ tự và dấu `|`. Ngày ngắn tiếng Việt `{d}/{m}` và tiếng Anh `{m} {d}` đang đúng thứ tự.

Độ dài được tính theo số ký tự Unicode thực tế (kể cả khoảng trắng và placeholder), không dùng số ước tính trong ghi chú nguồn. Mọi đề xuất ≤120% độ dài ô gốc, giữ nguyên các placeholder `{n}`, `{c}`, `{d}`. Chưa kiểm tra tràn chữ trong Unity.

| Key | Ngôn ngữ | Hiện tại | Đề xuất | Lý do |
|---|---|---|---|---|
| `shop.success` | en | Your purchase is successful | Purchase successful! | Toast xác nhận kết quả đã xảy ra; câu gọn, tự nhiên hơn “is successful”. (27 → 20 ký tự.) |
| `shop.success` | fr | Votre achat a réussi | Achat effectué ! | Thông báo xác nhận giao dịch tự nhiên hơn cách nói “achat a réussi”. (20 → 16 ký tự.) |
| `noti.letterTitle` | vi | Mở khoá chữ cái | Đã mở khoá chữ | Tiêu đề thông báo kết quả đã mở chữ, đồng nhất với nội dung; tránh hiểu thành lệnh yêu cầu mở khoá. (15 → 14 ký tự.) |
| `noti.letterTitle` | en | Unlock letter | Letter unlocked | Tiêu đề báo chữ đã được mở, thay cho mệnh lệnh “Unlock letter” còn thiếu mạo từ. (13 → 15 ký tự.) |
| `noti.letter` | en | We unlocked a letter {c} for you. Open the game and continue :) | We unlocked the letter {c} for you. Open the game and keep playing :) | Dùng “the letter {c}” khi xác định chữ; “keep playing” làm rõ lời mời chơi tiếp. (63 → 69 ký tự.) |
| `noti.letter` | fr | Nous avons débloqué la lettre {c} pour vous. Ouvrez le jeu et continuez :) | Nous avons débloqué la lettre {c} pour vous. Ouvrez le jeu et reprenez la partie :) | “Reprenez la partie” mời tiếp tục ván đang chơi rõ hơn “continuez” đứng một mình. (74 → 83 ký tự.) |
| `noti.letter` | es | Hemos desbloqueado la letra {c} para ti. Abre el juego y sigue :) | Hemos desbloqueado la letra {c} para ti. Abre el juego y sigue jugando :) | Bổ sung “jugando” để lời mời tiếp tục chơi trọn nghĩa. (65 → 73 ký tự.) |
| `noti.letter` | pt | Desbloqueamos a letra {c} para você. Abra o jogo e continue :) | Desbloqueamos a letra {c} para você. Abra o jogo e continue jogando :) | “Continue jogando” rõ và tự nhiên hơn động từ “continue” đứng riêng; giữ giọng pt-BR theo bản gốc. (62 → 70 ký tự.) |
| `noti.ruby` | en | You got {n} free rubies. Let's play and challenge yourself | You got {n} free rubies. Play and challenge yourself! | Tránh chuyển chủ thể từ “Let’s” (chúng ta) sang “yourself”; dùng hai lời mời cùng hướng tới người chơi. (58 → 53 ký tự.) |
| `shop.adsNote` | en | Buy any item removes ads (optional videos stay) | Any purchase removes ads (optional videos stay) | Sửa cấu trúc sai “Buy any item removes”; giữ ý mọi giao dịch đều tắt quảng cáo, video tuỳ chọn vẫn có. (47 → 47 ký tự.) |
| `win.dailyNext` | fr | Un nouveau défi demain. Gardez votre série ! | Un nouveau défi demain. Continuez votre série ! | “Continuez votre série” diễn đạt tiếp tục chuỗi thành tích tự nhiên hơn “Gardez votre série”. (44 → 47 ký tự.) |
| `win.toDaily` | vi | VỀ HÀNG NGÀY | VỀ LỊCH NGÀY | “Hàng ngày” là trạng ngữ; gọi rõ lịch thử thách mà nút quay về. (12 → 12 ký tự.) |
| `win.toDaily` | fr | RETOUR AU QUOTIDIEN | VOIR LE CALENDRIER | “Retour au quotidien” dễ hiểu là quay về đời sống thường ngày; gọi đúng màn lịch Daily. (19 → 18 ký tự.) |
| `win.toDaily` | es | VOLVER AL DIARIO | VER CALENDARIO | “Diario” đứng riêng dễ hiểu là nhật ký hoặc báo; gọi đúng màn lịch và giữ nhãn ngắn. (16 → 14 ký tự.) |
| `win.toDaily` | pt | VOLTAR AO DIÁRIO | VER CALENDÁRIO | “Diário” đứng riêng dễ hiểu là nhật ký; gọi đúng lịch thử thách và giữ nhãn ngắn. (16 → 14 ký tự.) |
| `win.milestone` | fr | Bonus de série de {d} jours : +{n} rubis | Bonus pour {d} jours de suite : +{n} rubis | “Jours de suite” diễn tả rõ số ngày liên tiếp; tránh cụm “bonus de série de” nặng nề. (40 → 42 ký tự.) |
| `win.milestone` | es | Bonus por racha de {d} días: +{n} rubíes | Bono por {d} días seguidos: +{n} rubíes | Dùng danh từ Tây Ban Nha “bono”; “días seguidos” làm rõ điều kiện liên tiếp, câu ngắn hơn. (40 → 39 ký tự.) |
| `win.milestone` | pt | Bônus de sequência de {d} dias: +{n} rubis | Bônus por {d} dias seguidos: +{n} rubis | Dùng cấu trúc pt-BR tự nhiên, nói rõ thưởng cho các ngày liên tiếp. (42 → 39 ký tự.) |

## Key mới cho chuỗi một ngày

Chỉ thêm đề xuất `win.streak.one`, dùng khi **n = 1**. Cột hiện tại trích đúng `win.streak` từ `Assets/WordCross/Config/GameData/Localization/LocalizationData.asset` (chỉ đọc). Giữ `{n}` để cùng cơ chế thay placeholder. Vi/en không đổi hình thức; fr/es/pt chuyển danh từ ngày sang số ít. Không thay chuỗi số nhiều `win.streak`.

| Key | Ngôn ngữ | Hiện tại | Đề xuất | Lý do |
|---|---|---|---|---|
| `win.streak.one` | vi | Chuỗi {n} ngày (từ `win.streak`) | Chuỗi {n} ngày | Tiếng Việt không biến đổi “ngày” theo số lượng. |
| `win.streak.one` | en | {n}-day streak (từ `win.streak`) | {n}-day streak | “day” trong tính từ ghép “{n}-day” luôn ở số ít, kể cả n = 1. |
| `win.streak.one` | fr | Série de {n} jours (từ `win.streak`) | Série de {n} jour | n = 1 dùng “jour”, không dùng “jours”. |
| `win.streak.one` | es | Racha de {n} días (từ `win.streak`) | Racha de {n} día | n = 1 dùng “día”, không dùng “días”. |
| `win.streak.one` | pt | Sequência de {n} dias (từ `win.streak`) | Sequência de {n} dia | n = 1 dùng “dia”, không dùng “dias”. |

## Tổng số ô

| Ngôn ngữ | Sửa ô hiện có | Thêm win.streak.one | Tổng đề xuất |
|---|---:|---:|---:|
| vi | 2 | 1 | 3 |
| en | 5 | 1 | 6 |
| fr | 5 | 1 | 6 |
| es | 3 | 1 | 4 |
| pt | 3 | 1 | 4 |
| **Tổng** | **18** | **5** | **23** |

Tổng số ô sửa theo ngôn ngữ: **vi 2, en 5, fr 5, es 3, pt 3**. Thêm riêng 5 ô cho key mới; không cộng chúng vào 18 ô sửa hiện có.
