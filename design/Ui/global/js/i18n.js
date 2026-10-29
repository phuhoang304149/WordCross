/* UI localization vi/en + state demo. Ngôn ngữ UI (vi/en) TÁCH khỏi ngôn ngữ chơi (gameLang).
   Giả định: user chưa chỉ định locale nên mặc định vi, hỗ trợ en. Mỗi dòng: key -> [vi, en]. {n} là tham số.
   Lưu: localStorage nếu có, nếu không dùng window.name. Iframe preview (?embed=1) không đọc/ghi lưu trữ. */
window.WC_ROWS = {
  'brand.sub': ['DEMO · tên hiển thị chỉnh theo bản chính', 'DEMO · display name to be confirmed'],
  'tag.sample': ['MẪU', 'SAMPLE'],
  'tag.demo': ['DEMO', 'DEMO'],
  'tag.sampleLevel': ['Level mẫu (dữ liệu thật rút gọn)', 'Sample level (real data, trimmed)'],
  'common.back': ['Quay lại', 'Back'], 'common.close': ['Đóng', 'Close'], 'common.ok': ['OK', 'OK'],
  'common.continue': ['TIẾP TỤC', 'CONTINUE'], 'common.gotIt': ['ĐÃ HIỂU', 'GOT IT'],
  'common.later': ['ĐỂ SAU', 'LATER'], 'common.skip': ['BỎ QUA, CHƠI TIẾP', 'SKIP, KEEP PLAYING'],
  'common.rubies': ['ruby', 'rubies'], 'common.balance': ['Số ruby', 'Ruby balance'], 'common.addRubies': ['Thêm ruby', 'Get rubies'],
  'common.soon': ['SẮP CÓ', 'SOON'], 'common.level': ['Level {n}', 'Level {n}'], 'common.retry': ['THỬ LẠI', 'TRY AGAIN'],
  'home.play': ['CHƠI', 'PLAY'], 'home.daily': ['HÀNG NGÀY', 'DAILY'], 'home.bot': ['ĐẤU BOT', 'VS BOT'],
  'home.ai': ['ĐẤU AI', 'VS AI'], 'home.shop': ['CỬA HÀNG', 'SHOP'], 'home.rate': ['ĐÁNH GIÁ', 'RATE'],
  'home.lang': ['Ngôn ngữ', 'Language'], 'home.resume': ['Tiếp tục: Subworld {s} - {l}', 'Resume: Subworld {s} - {l}'],
  'world.title': ['Chọn World', 'Select World'], 'world.name': ['World {n}', 'World {n}'], 'world.swipe': ['vuốt để đổi', 'swipe to change'],
  'world.sub': ['Subworld {n}', 'Subworld {n}'], 'world.locked': ['Chưa mở khoá', 'Locked'],
  'lvl.title': ['Subworld {n}', 'Subworld {n}'], 'lvl.locked': ['Level {n}, chưa mở', 'Level {n}, locked'],
  'lvl.done': ['Level {n}, đã xong', 'Level {n}, completed'], 'lvl.cur': ['Level {n}, level hiện tại', 'Level {n}, current'],
  'game.shuffle': ['Xáo chữ', 'Shuffle'], 'game.hint': ['Gợi ý', 'Hint'], 'game.extra': ['Từ thưởng', 'Extra'],
  'game.ask': ['Hỏi bạn', 'Ask friend'], 'game.pause': ['Tạm dừng', 'Pause'], 'game.banner': ['QUẢNG CÁO BANNER (chỗ giữ)', 'BANNER AD (placeholder)'],
  'game.nice': ['Hay! {w}', 'Nice! {w}'], 'game.already': ['Đã tìm rồi', 'Already found'], 'game.notIn': ['Không có trong danh sách', 'Not in list'],
  'game.extraFound': ['Từ thưởng: {w}', 'Extra word: {w}'], 'game.extraAgain': ['Từ thưởng đã có', 'Extra already found'],
  'game.ask.msg': ['Mở chia sẻ ảnh chụp (native share, ngoài prototype)', 'Opens screenshot share (native, outside prototype)'],
  'game.coach1': ['Vuốt qua các chữ để ghép từ.', 'Swipe across the letters to build a word.'],
  'game.coach2': ['Tìm đủ các từ để thắng. Không cần ruby.', 'Find every word to win. No rubies needed.'],
  'game.coachDaily': ['Mục tiêu hôm nay: tìm đủ {n} từ.', "Today's goal: find all {n} words."],
  'game.dailyName': ['Thử thách {d}', 'Daily {d}'], 'game.words': ['{a}/{b} từ', '{a}/{b} words'],
  'game.hintFree': ['Gợi ý mở 1 chữ. Tuỳ chọn.', 'A hint reveals 1 letter. Optional.'],
  'pause.title': ['TẠM DỪNG', 'PAUSE'], 'pause.music': ['Nhạc', 'Music'], 'pause.sound': ['Âm thanh', 'Sound'],
  'pause.continue': ['TIẾP TỤC', 'CONTINUE'], 'pause.how': ['CÁCH CHƠI', 'HOW TO PLAY'], 'pause.menu': ['VỀ MENU', 'TO MENU'],
  'pause.quit': ['THOÁT', 'QUIT'], 'pause.lang': ['NGÔN NGỮ', 'LANGUAGE'],
  'how.title': ['CÁCH CHƠI ?', 'HOW TO PLAY ?'], 'how.1': ['Vuốt để ghép thành từ hợp lệ.', 'Swipe to build a valid word.'],
  'how.2': ['Dùng "Xáo chữ" hoặc "Gợi ý" khi bí. Bạn cũng có thể chụp màn hình hỏi bạn bè.', 'Use "Shuffle" or "Hint" if you are stuck. You can also take a screenshot and ask friends.'],
  'how.3': ['Xáo chữ miễn phí. Gợi ý tốn ruby nhưng không bắt buộc.', 'Shuffle is free. Hints cost rubies but are never required.'],
  'win.title': ['HOÀN THÀNH LEVEL!', 'LEVEL COMPLETE!'], 'win.next': ['TIẾP', 'NEXT'], 'win.menu': ['MENU', 'MENU'],
  'win.first': ['Lần đầu thắng: +{n} ruby', 'First clear: +{n} rubies'], 'win.replay': ['Chơi lại để luyện tập: không thêm ruby', 'Replay is practice: no extra rubies'],
  'win.x2': ['XEM VIDEO: +{n} ruby nữa', 'WATCH VIDEO: +{n} more rubies'], 'win.x2done': ['Đã nhận thưởng thêm', 'Bonus collected'],
  'win.goal': ['Tiếp theo: Level {l} · còn {n} level để mở Subworld kế', 'Next: Level {l} · {n} more levels to unlock the next Subworld'],
  'win.goalLast': ['Subworld hoàn thành! Tiếp theo: Subworld {s}', 'Subworld complete! Next: Subworld {s}'],
  'win.extraReady': ['Từ thưởng {a}/{b}: sẵn sàng nhận', 'Extra words {a}/{b}: ready to claim'],
  'win.extraNot': ['Từ thưởng {a}/{b}', 'Extra words {a}/{b}'],
  'win.dailyTitle': ['XONG THỬ THÁCH!', 'DAILY COMPLETE!'], 'win.dailyReward': ['Thưởng hôm nay: +{n} ruby', "Today's reward: +{n} rubies"],
  'win.streak': ['Chuỗi {n} ngày', '{n}-day streak'], 'win.milestone': ['Mốc chuỗi {d} ngày: +{n} ruby', '{d}-day streak bonus: +{n} rubies'],
  'win.dailyNext': ['Mai có thử thách mới. Giữ chuỗi nhé!', 'A new puzzle tomorrow. Keep your streak!'],
  'win.toDaily': ['VỀ HÀNG NGÀY', 'BACK TO DAILY'],
  'extra.title': ['TỪ THƯỞNG', 'EXTRA WORDS'], 'extra.desc': ['Tìm thêm từ mới để nhận ruby', 'Find more and more new words to collect rubies'],
  'extra.claim': ['NHẬN +{n} RUBY', 'CLAIM +{n} RUBIES'], 'extra.claimed': ['Đã nhận +{n} ruby', 'Claimed +{n} rubies'],
  'extra.note': ['Từ thưởng chỉ tính một lần cho mỗi level.', 'Each extra word counts once per level.'],
  'shop.title': ['CỬA HÀNG', 'SHOP'], 'shop.free': ['RUBY MIỄN PHÍ', 'FREE RUBIES'], 'shop.watch': ['XEM', 'WATCH'],
  'shop.ready': ['Xem video ngắn: +{n} ruby', 'Short video: +{n} rubies'], 'shop.cool': ['Video sẵn sàng sau {t}', 'Ad available in {t}'],
  'shop.pack': ['{n} ruby', '{n} rubies'], 'shop.note': ['Mua bất kỳ gói nào sẽ tắt quảng cáo (không tắt video tuỳ chọn) · giá demo', 'Buy any item removes ads (optional videos stay) · demo prices'],
  'shop.noCheckout': ['DEMO: chưa có thanh toán thật, chưa xác minh IAP.', 'DEMO: no real checkout, IAP not verified.'],
  'shop.best': ['LỜI NHẤT', 'BEST VALUE'], 'shop.adsOff': ['Quảng cáo đã tắt (mô phỏng)', 'Ads removed (simulated)'],
  'rv.title': ['XEM VIDEO', 'WATCH A VIDEO'], 'rv.value': ['Xem hết video để nhận', 'Watch the whole video to get'],
  'rv.watch': ['XEM VIDEO', 'WATCH VIDEO'], 'rv.no': ['KHÔNG, CẢM ƠN', 'NO THANKS'],
  'rv.s.loading': ['Đang tải video…', 'Loading video…'], 'rv.d.loading': ['Bạn vẫn chơi bình thường trong lúc chờ.', 'You can keep playing while it loads.'],
  'rv.s.cooldown': ['Video tiếp theo sau {t}', 'Next video in {t}'], 'rv.d.cooldown': ['Chưa mất gì. Quay lại sau nhé.', 'Nothing lost. Come back soon.'],
  'rv.s.nofill': ['Chưa có video lúc này', 'No video available right now'], 'rv.d.nofill': ['Không trừ ruby, không có thưởng. Thử lại sau.', 'No rubies charged, no reward. Try later.'],
  'rv.s.offline': ['Bạn đang offline', "You're offline"], 'rv.d.offline': ['Ruby từ video cần mạng. Chơi vẫn bình thường.', 'Video rubies need internet. Play continues normally.'],
  'rv.s.playing': ['QUẢNG CÁO MÔ PHỎNG', 'SIMULATED AD'], 'rv.d.playing': ['Không phải quảng cáo thật. Còn {n}s', 'Not a real ad. {n}s left'],
  'rv.closeAd': ['ĐÓNG QUẢNG CÁO', 'CLOSE AD'],
  'rv.s.success': ['NHẬN THƯỞNG!', 'REWARD EARNED!'], 'rv.d.success': ['Đã cộng {n} ruby.', '{n} rubies added.'],
  'rv.s.cancel': ['Đã đóng sớm', 'Closed early'], 'rv.d.cancel': ['Không có thưởng và không bị trừ ruby.', 'No reward and no rubies charged.'],
  'rv.s.error': ['Có lỗi khi phát video', 'Video failed to play'], 'rv.d.error': ['Không có thưởng và không bị trừ ruby.', 'No reward and no rubies charged.'],
  'rv.demo': ['DEMO · chọn trạng thái quảng cáo để xem', 'DEMO · pick an ad state to preview'],
  'rv.st.loading': ['đang tải', 'loading'], 'rv.st.ready': ['sẵn sàng', 'ready'], 'rv.st.cooldown': ['chờ', 'cooldown'],
  'rv.st.nofill': ['hết quảng cáo', 'no-fill'], 'rv.st.offline': ['offline', 'offline'], 'rv.out': ['Kết quả khi xem:', 'Outcome when watched:'],
  'rv.out.success': ['xong', 'complete'], 'rv.out.cancel': ['đóng sớm', 'cancel'], 'rv.out.error': ['lỗi', 'error'],
  'low.title': ['CẦN {n} RUBY', 'NEED {n} RUBIES'], 'low.desc': ['Bạn có {a} ruby. Gợi ý là tuỳ chọn, bạn vẫn chơi tiếp được miễn phí.', 'You have {a} rubies. Hints are optional; you can keep playing for free.'],
  'low.watch': ['XEM VIDEO +{n} RUBY (tuỳ chọn)', 'WATCH VIDEO +{n} RUBIES (optional)'], 'low.shop': ['CỬA HÀNG', 'SHOP'],
  'rate.title': ['BẠN THÍCH GAME?', 'ENJOYING THE GAME?'], 'rate.desc': ['Nếu thích game, hãy đánh giá 5 sao trên cửa hàng.', 'If you love the game, please rate us 5 stars in the store.'],
  'rate.yes': ['ĐƯỢC', 'YES'], 'rate.later': ['ĐỂ SAU', 'LATER'],
  'lang.title': ['Ngôn ngữ', 'Language'], 'lang.ui': ['Ngôn ngữ giao diện', 'Interface language'], 'lang.uiDesc': ['Đổi chữ trên nút, popup, thông báo. Không đổi từ trong level.', 'Changes button, popup and message text. Does not change level words.'],
  'lang.game': ['Ngôn ngữ chơi', 'Game language'], 'lang.gameDesc': ['Bộ level và từ điển. Mỗi ngôn ngữ có tiến độ riêng.', 'Level set and dictionary. Each language keeps its own progress.'],
  'lang.vi': ['Tiếng Việt', 'Vietnamese'], 'lang.en': ['Tiếng Anh', 'English'], 'lang.fr': ['Tiếng Pháp', 'French'], 'lang.es': ['Tiếng Tây Ban Nha', 'Spanish'], 'lang.pt': ['Tiếng Bồ Đào Nha', 'Portuguese'],
  'lang.ui.vi': ['Tiếng Việt', 'Tiếng Việt'], 'lang.ui.en': ['English', 'English'],
  'lang.playable': ['Chơi được · 534 level', 'Playable · 534 levels'], 'lang.soon': ['Sắp có · đã có từ điển, chưa có level', 'Coming soon · word list only, no levels yet'],
  'lang.soonVi': ['Đề xuất · chưa có từ điển lẫn level', 'Proposed · no word list or levels yet'], 'lang.noteSoon': ['Ngôn ngữ này chưa có bộ level hợp lệ nên chưa chơi được.', 'This language has no valid level set yet, so it is not playable.'],
  'lang.saved': ['Đã lưu', 'Saved'],
  'daily.title': ['Thử thách hàng ngày', 'Daily Challenge'], 'daily.today': ['Hôm nay · {d}', 'Today · {d}'], 'daily.goal': ['Mục tiêu: tìm đủ {n} từ trong 1 bàn chung cho mọi người chơi.', 'Goal: find all {n} words in one puzzle shared by every player.'],
  'daily.play': ['CHƠI HÔM NAY', 'PLAY TODAY'], 'daily.replay': ['CHƠI LẠI (không thưởng)', 'REPLAY (no reward)'], 'daily.playDay': ['CHƠI NGÀY {d}', 'PLAY {d}'],
  'daily.reward': ['Thưởng: +{n} ruby, nhận một lần', 'Reward: +{n} rubies, claimed once'], 'daily.rewardDone': ['Đã nhận thưởng ngày này', 'Reward already claimed'],
  'daily.backfill': ['Bù ngày bị lỡ: +{n} ruby, giữ chuỗi', 'Make-up day: +{n} ruby, keeps your streak'], 'daily.noReward': ['Ngày này không còn thưởng', 'No reward for this day'],
  'daily.streak': ['Chuỗi {n} ngày', '{n}-day streak'], 'daily.best': ['Kỷ lục {n}', 'Best {n}'], 'daily.streakRisk': ['Chuỗi của bạn đang chờ! Chơi hôm nay hoặc bù hôm qua để giữ.', 'Your streak is waiting! Play today or make up yesterday to keep it.'],
  'daily.streakLost': ['Chuỗi đã nghỉ. Kỷ lục {n} vẫn được giữ, bắt đầu lại hôm nay.', 'Streak paused. Your best {n} is kept; start again today.'],
  'daily.nextMilestone': ['Mốc kế: {d} ngày → +{n} ruby', 'Next bonus: {d} days → +{n} rubies'],
  'daily.legend': ['Xanh: đã xong · Cam: bù được · Mờ: chưa đến ngày', 'Teal: done · Orange: make-up · Dimmed: not yet'],
  'daily.prev': ['Tháng trước', 'Previous month'], 'daily.next': ['Tháng sau', 'Next month'],
  'daily.month': ['Tháng {m} · {y}', '{m} {y}'], 'daily.dayAria': ['Ngày {d}, {s}', 'Day {d}, {s}'],
  'daily.s.done': ['đã xong', 'completed'], 'daily.s.miss': ['bỏ lỡ, bù được', 'missed, can make up'], 'daily.s.old': ['bỏ lỡ, hết hạn bù', 'missed, make-up expired'],
  'daily.s.today': ['hôm nay', 'today'], 'daily.s.future': ['chưa đến', 'not available yet'],
  'daily.tz': ['Ngày đổi lúc 00:00 giờ trên máy. Chơi được khi offline.', 'Day changes at 00:00 device time. Playable offline.'],
  'daily.demoClock': ['Đồng hồ demo: {d}', 'Demo clock: {d}'], 'daily.advance': ['DEMO: sang ngày kế', 'DEMO: next day'], 'daily.resetDemo': ['DEMO: đặt lại dữ liệu', 'DEMO: reset data'],
  'daily.sample': ['Dữ liệu lịch và chuỗi là mẫu để xem luồng.', 'Calendar and streak data are samples to preview the flow.'],
  'bot.title': ['Đấu với Bot', 'Play with Bot'], 'bot.intro': ['Cùng một bàn, cùng ngôn ngữ. Ai tìm được nhiều điểm hơn trong thời gian giới hạn sẽ thắng.', 'Same puzzle, same language. Whoever scores more in the time limit wins.'],
  'bot.diff': ['Chọn độ khó', 'Choose difficulty'], 'bot.easy': ['Dễ', 'Easy'], 'bot.normal': ['Vừa', 'Normal'], 'bot.hard': ['Khó', 'Hard'],
  'bot.name.easy': ['Bot Pip', 'Bot Pip'], 'bot.name.normal': ['Bot Zip', 'Bot Zip'], 'bot.name.hard': ['Bot Zap', 'Bot Zap'],
  'bot.d.easy': ['Chậm, hay bỏ sót từ. Thắng +{n} ruby', 'Slow, often misses words. Win +{n} rubies'],
  'bot.d.normal': ['Tốc độ vừa phải. Thắng +{n} ruby', 'Steady pace. Win +{n} rubies'], 'bot.d.hard': ['Nhanh, ít bỏ sót. Thắng +{n} ruby', 'Fast, rarely misses. Win +{n} rubies'],
  'bot.fair': ['Bot là chương trình tính sẵn, không phải người thật. Không xem trước đáp án của bạn, không dùng gợi ý.', 'The bot is a scripted program, not a real person. It never sees your input and uses no hints.'],
  'bot.rules': ['Không gợi ý trong trận. Xáo chữ miễn phí. Từ thưởng không tính điểm.', 'No hints in a match. Shuffle is free. Extra words do not score.'],
  'bot.start': ['BẮT ĐẦU', 'START'], 'bot.cap': ['Thưởng bot hôm nay: {a}/{b} ruby', "Bot rewards today: {a}/{b} rubies"],
  'bot.you': ['Bạn', 'You'], 'bot.time': ['Còn {n}s', '{n}s left'], 'bot.pts': ['{n} điểm', '{n} pts'], 'bot.badge': ['BOT', 'BOT'],
  'bot.win': ['BẠN THẮNG!', 'YOU WIN!'], 'bot.lose': ['BOT THẮNG', 'BOT WINS'], 'bot.draw': ['HÒA', 'DRAW'],
  'bot.reward': ['Thưởng: +{n} ruby', 'Reward: +{n} rubies'], 'bot.noReward': ['Không thưởng thêm (trận chơi lại hoặc đã đủ giới hạn ngày)', 'No reward (replay or daily cap reached)'],
  'bot.replay': ['CHƠI LẠI CÙNG BÀN', 'REPLAY SAME PUZZLE'], 'bot.new': ['TRẬN MỚI', 'NEW MATCH'], 'bot.seed': ['Mã trận {s}', 'Match seed {s}'],
  'bot.found': ['Từ đã tìm: bạn {a} · bot {b}', 'Words found: you {a} · bot {b}'], 'bot.leave': ['THOÁT TRẬN', 'LEAVE MATCH'],
  'bot.sampleNote': ['Bàn lấy từ level mẫu thật của World/Subworld tương ứng.', 'Puzzle comes from a real sample level of the matching World/Subworld.'],
  'ai.title': ['ĐẤU AI', 'PLAY WITH AI'], 'ai.soon': ['SẮP RA MẮT', 'COMING SOON'], 'ai.desc': ['Chưa có ngày phát hành. Hiện chưa có ghép trận, thanh toán hay quảng cáo cho tính năng này.', 'No release date yet. There is no matchmaking, payment or ads for this feature.'],
  'ai.note': ['Hãy thử đấu Bot hoặc Thử thách hàng ngày trong lúc chờ.', 'Try Play with Bot or the Daily Challenge in the meantime.'], 'ai.toBot': ['ĐẤU BOT', 'PLAY WITH BOT'],
  'scr.home.name': ['Home', 'Home'], 'scr.home.note': ['Nền, logo, Play, Daily, Bot, AI (sắp có), Shop, Language', 'Backdrop, logo, Play, Daily, Bot, AI (soon), Shop, Language'],
  'scr.select-world.name': ['Chọn World', 'Select World'], 'scr.select-world.note': ['Vuốt theo World 1–6, mỗi World 5 subworld, tiến độ n/tổng thật', 'Swipe World 1–6, 5 subworlds each, real n/total counts'],
  'scr.select-level.name': ['Chọn Level', 'Select Level'], 'scr.select-level.note': ['Lưới level: Subworld 1 có 12 level, còn lại 18', 'Level grid: Subworld 1 has 12 levels, others 18'],
  'scr.game.name': ['Gameplay', 'Gameplay'], 'scr.game.note': ['Mỗi đáp án một dòng, vòng chữ vuốt, Hint 1 chữ, Shuffle, Extra', 'One line per answer, swipe wheel, 1-letter Hint, Shuffle, Extra'],
  'scr.daily.name': ['Daily Challenge', 'Daily Challenge'], 'scr.daily.note': ['Lịch, mục tiêu, chuỗi, bù ngày, thưởng một lần', 'Calendar, goal, streak, make-up day, once-only reward'],
  'scr.bot-setup.name': ['Bot: chọn độ khó', 'Bot: difficulty'], 'scr.bot-setup.note': ['3 bot, luật công bằng, giới hạn thưởng ngày', '3 bots, fair rules, daily reward cap'],
  'scr.bot-match.name': ['Bot: trận đấu', 'Bot: match'], 'scr.bot-match.note': ['Cùng bàn, bot mô phỏng tính sẵn theo mã trận', 'Same puzzle, scripted bot driven by a match seed'],
  'scr.language.name': ['Ngôn ngữ', 'Language'], 'scr.language.note': ['Ngôn ngữ UI vi/en tách khỏi ngôn ngữ chơi', 'UI vi/en separate from game language'],
  'scr.pause.name': ['Pause', 'Pause'], 'scr.pause.note': ['Continue · How to play · Language · To Menu · Quit', 'Continue · How to play · Language · To Menu · Quit'],
  'scr.how-to-play.name': ['How to play', 'How to play'], 'scr.how-to-play.note': ['Hướng dẫn vuốt, Shuffle/Hint', 'Swipe tutorial, Shuffle/Hint'],
  'scr.win.name': ['Win', 'Win'], 'scr.win.note': ['Thưởng lần đầu, x2 tuỳ chọn, mục tiêu kế', 'First-clear reward, optional x2, next goal'],
  'scr.extra-word.name': ['Extra words', 'Extra words'], 'scr.extra-word.note': ['Tiến độ từ thưởng, nhận ruby', 'Extra-word progress, ruby claim'],
  'scr.shop.name': ['Shop', 'Shop'], 'scr.shop.note': ['5 gói ruby (giá demo), video miễn phí', '5 ruby packs (demo prices), free video'],
  'scr.rewarded-video.name': ['Rewarded video', 'Rewarded video'], 'scr.rewarded-video.note': ['9 trạng thái quảng cáo mô phỏng, thưởng sau khi xem xong', '9 simulated ad states, reward after completion'],
  'scr.low-ruby.name': ['Thiếu ruby', 'Not enough rubies'], 'scr.low-ruby.note': ['Lối chơi miễn phí, video tuỳ chọn, bỏ qua được', 'Free path, optional video, skippable'],
  'scr.rate.name': ['Rate', 'Rate'], 'scr.rate.note': ['YES / LATER', 'YES / LATER'],
  'scr.bot-result.name': ['Bot: kết quả', 'Bot: result'], 'scr.bot-result.note': ['Thắng/thua/hòa, thưởng, chơi lại cùng mã trận', 'Win/lose/draw, reward, replay same seed'],
  'scr.ai-soon.name': ['AI: sắp có', 'AI: coming soon'], 'scr.ai-soon.note': ['Nút Coming soon, không ghép trận/thanh toán/quảng cáo', 'Coming-soon only; no matchmaking, payment or ads'],
  'cat.title': ['WordCross UI', 'WordCross UI'], 'cat.intro': ['Phong cách phẳng, sáng, teal / coral / yellow. Mỗi thẻ mở màn hình thật; xem preview không ảnh hưởng dữ liệu demo.', 'Flat, bright teal / coral / yellow. Each card opens a real screen; previews never touch demo data.'],
  'cat.screens': ['Màn hình', 'Screens'], 'cat.popups': ['Popup', 'Popups'], 'cat.tokens': ['Token', 'Tokens'], 'cat.portal': ['← Portal', '← Portal'],
  'cat.flow': ['Home → World → Level → Gameplay · Daily · Bot · Language', 'Home → World → Level → Gameplay · Daily · Bot · Language'],
  'cat.popSub': ['Hiển thị trên nền màn hình tương ứng', 'Shown over the matching screen'],
  'cat.proto': ['Prototype: gameplay dùng level thật (Subworld 1 đủ 12 level + vài level mẫu), Hint mở 1 chữ tốn 2 ruby, Daily/Bot/ads mô phỏng; dữ liệu lưu cục bộ trong trình duyệt. Phần nào là mẫu có nhãn DEMO/MẪU.', 'Prototype: gameplay uses real levels (Subworld 1 all 12 + a few samples), a Hint reveals 1 letter for 2 rubies, Daily/Bot/ads are simulated; data is stored locally in the browser. Sample parts carry a DEMO/SAMPLE tag.'],
  'cat.resetData': ['Đặt lại dữ liệu demo', 'Reset demo data'], 'cat.gd': ['GD', 'GD'], 'cat.tokenSub': ['global/css/theme.css (:root)', 'global/css/theme.css (:root)'],
  'cat.open': ['Mở', 'Open'],
  'portal.title': ['WordCross Design', 'WordCross Design'], 'portal.ui': ['UI Catalogue', 'UI Catalogue'], 'portal.uiSub': ['Màn hình/popup mở offline, gameplay + Daily + Bot chơi thử', 'Offline screens/popups; playable gameplay, Daily and Bot'],
  'portal.gd': ['GD', 'GD'], 'portal.gdSub': ['Quan sát từ source, level, ngôn ngữ, Daily, Bot, ruby/ads, retention', 'Source observations, levels, languages, Daily, Bot, ruby/ads, retention']
};

(function () {
  var q = new URLSearchParams(location.search);
  var embed = !!q.get('embed');
  var KEY = 'wc_design_state_v2';
  var hasLS = typeof window.localStorage !== 'undefined';
  var DEMO_TODAY = '2026-10-29'; /* ngày demo cố định để dữ liệu mẫu nhất quán */

  var defaults = function () {
    return {
      v: 2, uiLang: 'vi', gameLang: 'en', rubies: 10, /* DEFAULT_CURRENCY = 10 (CurrencyController.cs); Header.prefab đặt 12 chỉ là placeholder */
      level: {w: 0, s: 0, l: 0}, mode: 'level', cleared: {}, progress: {}, claims: {},
      extra: {progress: 1, target: 5, total: 1}, winCtx: null,
      daily: {today: DEMO_TODAY, solved: {'2026-10-22': 1, '2026-10-23': 1, '2026-10-24': 1, '2026-10-25': 1, '2026-10-26': 1, '2026-10-27': 1}, claimed: {}, played: {}, best: 6, milestones: {}},
      bot: {diff: 'normal', seed: 1, day: DEMO_TODAY, earned: 0, rewarded: {}, last: null},
      ads: {forced: 'ready', outcome: 'success', lastRewardAt: -1, ctx: {placement: 'shop', amount: 2}, removed: false},
      music: true, sound: true
    };
  };
  var load = function () {
    if (embed) return defaults();
    var raw = hasLS ? window.localStorage.getItem(KEY) : (String(window.name).indexOf('WC:') === 0 ? String(window.name).slice(3) : null);
    if (typeof raw === 'string' && raw.charAt(0) === '{') {
      var s = JSON.parse(raw);
      if (s && s.v === 2) return s;
    }
    return defaults();
  };
  var state = load();
  var lang = q.get('lang') === 'en' || q.get('lang') === 'vi' ? q.get('lang') : state.uiLang;

  var WC = window.WC = window.WC || {};
  WC.embed = embed; WC.state = state; WC.demoToday = DEMO_TODAY; WC.defaults = defaults;
  WC.lang = lang;
  WC.save = function () {
    if (embed) return;
    var raw = JSON.stringify(state);
    if (hasLS) window.localStorage.setItem(KEY, raw); else window.name = 'WC:' + raw;
  };
  WC.reset = function () { var d = defaults(); Object.keys(state).forEach(function (k) { delete state[k]; }); Object.keys(d).forEach(function (k) { state[k] = d[k]; }); WC.save(); };
  WC.setLang = function (l) { state.uiLang = l; WC.save(); };
  var esc = function (s) { return String(s).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;'); };
  WC.esc = esc;
  /* t(): trả chuỗi đã thoát HTML; thiếu key trả chính key (completeness được kiểm bằng smoke). */
  WC.t = function (key, p) {
    var row = window.WC_ROWS[key];
    var s = row ? row[lang === 'en' ? 1 : 0] : key;
    if (p) Object.keys(p).forEach(function (k) { s = s.split('{' + k + '}').join(p[k]); });
    return esc(s);
  };
  document.documentElement.lang = lang;
})();
