from pathlib import Path
import re
from collections import Counter
source = Path('02-reskin-maintain/l10n-review.md').read_text()
langs = ['vi','en','fr','es','pt']
rows = {}
for line in source.splitlines():
    if line.startswith('| `'):
        cells = [s.strip().replace(r'\|','|') for s in re.split(r'(?<!\\)\|',line)[1:-1]]
        rows[cells[0].strip('`')] = dict(zip(langs,cells[1:6]))
assert len(rows) == 25
changes = [
('shop.success','en','Purchase successful!','Toast xác nhận kết quả đã xảy ra; câu gọn, tự nhiên hơn “is successful”.'),
('shop.success','fr','Achat effectué !','Thông báo xác nhận giao dịch tự nhiên hơn cách nói “achat a réussi”.'),
('noti.letterTitle','vi','Đã mở khoá chữ','Tiêu đề thông báo kết quả đã mở chữ, đồng nhất với nội dung; tránh hiểu thành lệnh yêu cầu mở khoá.'),
('noti.letterTitle','en','Letter unlocked','Tiêu đề báo chữ đã được mở, thay cho mệnh lệnh “Unlock letter” còn thiếu mạo từ.'),
('noti.letter','en','We unlocked the letter {c} for you. Open the game and keep playing :)','Dùng “the letter {c}” khi xác định chữ; “keep playing” làm rõ lời mời chơi tiếp.'),
('noti.letter','fr','Nous avons débloqué la lettre {c} pour vous. Ouvrez le jeu et reprenez la partie :)','“Reprenez la partie” mời tiếp tục ván đang chơi rõ hơn “continuez” đứng một mình.'),
('noti.letter','es','Hemos desbloqueado la letra {c} para ti. Abre el juego y sigue jugando :)','Bổ sung “jugando” để lời mời tiếp tục chơi trọn nghĩa.'),
('noti.letter','pt','Desbloqueamos a letra {c} para você. Abra o jogo e continue jogando :)','“Continue jogando” rõ và tự nhiên hơn động từ “continue” đứng riêng; giữ giọng pt-BR theo bản gốc.'),
('noti.ruby','en','You got {n} free rubies. Play and challenge yourself!','Tránh chuyển chủ thể từ “Let’s” (chúng ta) sang “yourself”; dùng hai lời mời cùng hướng tới người chơi.'),
('shop.adsNote','en','Any purchase removes ads (optional videos stay)','Sửa cấu trúc sai “Buy any item removes”; giữ ý mọi giao dịch đều tắt quảng cáo, video tuỳ chọn vẫn có.'),
('win.dailyNext','fr','Un nouveau défi demain. Continuez votre série !','“Continuez votre série” diễn đạt tiếp tục chuỗi thành tích tự nhiên hơn “Gardez votre série”.'),
('win.toDaily','vi','VỀ LỊCH NGÀY','“Hàng ngày” là trạng ngữ; gọi rõ lịch thử thách mà nút quay về.'),
('win.toDaily','fr','VOIR LE CALENDRIER','“Retour au quotidien” dễ hiểu là quay về đời sống thường ngày; gọi đúng màn lịch Daily.'),
('win.toDaily','es','VER CALENDARIO','“Diario” đứng riêng dễ hiểu là nhật ký hoặc báo; gọi đúng màn lịch và giữ nhãn ngắn.'),
('win.toDaily','pt','VER CALENDÁRIO','“Diário” đứng riêng dễ hiểu là nhật ký; gọi đúng lịch thử thách và giữ nhãn ngắn.'),
('win.milestone','fr','Bonus pour {d} jours de suite : +{n} rubis','“Jours de suite” diễn tả rõ số ngày liên tiếp; tránh cụm “bonus de série de” nặng nề.'),
('win.milestone','es','Bono por {d} días seguidos: +{n} rubíes','Dùng danh từ Tây Ban Nha “bono”; “días seguidos” làm rõ điều kiện liên tiếp, câu ngắn hơn.'),
('win.milestone','pt','Bônus por {d} dias seguidos: +{n} rubis','Dùng cấu trúc pt-BR tự nhiên, nói rõ thưởng cho các ngày liên tiếp.'),
]
new = {'vi':'Chuỗi {n} ngày','en':'{n}-day streak','fr':'Série de {n} jour','es':'Racha de {n} día','pt':'Sequência de {n} dia'}
for key,lang,value,reason in changes:
    old = rows[key][lang]
    assert len(value) <= len(old)*1.2, (key,lang,len(old),len(value))
    assert sorted(re.findall(r'\{[^}]+\}',value)) == sorted(re.findall(r'\{[^}]+\}',old)), (key,lang)
    assert value.count('|') == old.count('|')
for key,count in [('daily.months',12),('daily.monthsShort',12),('daily.weekdays',7)]:
    assert all(len(rows[key][lang].split('|')) == count for lang in langs)
# Read win.streak values directly from the asset, preserving the exact strings.
asset = Path('Assets/WordCross/Config/GameData/Localization/LocalizationData.asset').read_text()
block = re.search(r'  - key: "win.streak"\n    values:\n((?:    - .*\n){5})',asset).group(1)
originals = re.findall(r'^    - "(.*)"$',block,re.M)
assert originals == [rows['win.streak'][lang] for lang in langs]
for lang,old in zip(langs,originals):
    assert len(new[lang]) <= len(old)*1.2
    assert re.findall(r'\{[^}]+\}',new[lang]) == re.findall(r'\{[^}]+\}',old)

out = ['# Rà dịch WordCross — đề xuất Codex','',
'Đã rà đủ **25 key × 5 ngôn ngữ = 125 ô** từ `l10n-review.md`, theo ngữ cảnh sử dụng đã mô tả. Bảng dưới chỉ liệt kê ô nên sửa; 107 ô còn lại giữ nguyên. Đây là đánh giá biên tập ngôn ngữ của Codex, không phải kết quả duyệt của người bản ngữ độc lập. Tiếng Bồ Đào Nha giữ biến thể **pt-BR** theo “você”, “sua”, “bônus” của nguồn.','',
'Tên tháng và thứ viết hoa trong tiêu đề/đầu cột được chấp nhận như quy ước trình bày UI; không đổi hàng loạt chỉ vì cách viết trong câu thường dùng chữ thường. Các danh sách tháng/tháng ngắn/thứ giữ nguyên đủ **12/12/7 mục**, đúng thứ tự và dấu `|`. Ngày ngắn tiếng Việt `{d}/{m}` và tiếng Anh `{m} {d}` đang đúng thứ tự.','',
'Độ dài được tính theo số ký tự Unicode thực tế (kể cả khoảng trắng và placeholder), không dùng số ước tính trong ghi chú nguồn. Mọi đề xuất ≤120% độ dài ô gốc, giữ nguyên các placeholder `{n}`, `{c}`, `{d}`. Chưa kiểm tra tràn chữ trong Unity.','',
'| Key | Ngôn ngữ | Hiện tại | Đề xuất | Lý do |','|---|---|---|---|---|']
for key,lang,value,reason in changes:
    old = rows[key][lang]
    out.append(f'| `{key}` | {lang} | {old} | {value} | {reason} ({len(old)} → {len(value)} ký tự.) |')
out += ['', '## Key mới cho chuỗi một ngày', '',
'Chỉ thêm đề xuất `win.streak.one`, dùng khi **n = 1**. Cột hiện tại trích đúng `win.streak` từ `Assets/WordCross/Config/GameData/Localization/LocalizationData.asset` (chỉ đọc). Giữ `{n}` để cùng cơ chế thay placeholder. Vi/en không đổi hình thức; fr/es/pt chuyển danh từ ngày sang số ít. Không thay chuỗi số nhiều `win.streak`.', '',
'| Key | Ngôn ngữ | Hiện tại | Đề xuất | Lý do |','|---|---|---|---|---|']
reasons = {'vi':'Tiếng Việt không biến đổi “ngày” theo số lượng.', 'en':'“day” trong tính từ ghép “{n}-day” luôn ở số ít, kể cả n = 1.', 'fr':'n = 1 dùng “jour”, không dùng “jours”.', 'es':'n = 1 dùng “día”, không dùng “días”.', 'pt':'n = 1 dùng “dia”, không dùng “dias”.'}
for lang,old in zip(langs,originals):
    out.append(f'| `win.streak.one` | {lang} | {old} (từ `win.streak`) | {new[lang]} | {reasons[lang]} |')
counts = Counter(c[1] for c in changes)
out += ['', '## Tổng số ô', '', '| Ngôn ngữ | Sửa ô hiện có | Thêm win.streak.one | Tổng đề xuất |','|---|---:|---:|---:|']
for lang in langs:
    out.append(f'| {lang} | {counts[lang]} | 1 | {counts[lang]+1} |')
out += [f'| **Tổng** | **{len(changes)}** | **5** | **{len(changes)+5}** |','',f'Tổng số ô sửa theo ngôn ngữ: **vi {counts["vi"]}, en {counts["en"]}, fr {counts["fr"]}, es {counts["es"]}, pt {counts["pt"]}**. Thêm riêng 5 ô cho key mới; không cộng chúng vào 18 ô sửa hiện có.','']
Path('02-reskin-maintain/l10n-review-codex.md').write_text('\n'.join(out))
print(f'Đã rà 125 ô; {len(changes)} ô sửa + 5 ô mới; {dict(counts)}; placeholder, giới hạn 20% và 12/12/7 mục: đạt.')
