# Sinh economy-loc.json: bản dịch fr/es/pt cho design/GD/economy-year-data/loc-keys.json (vi/en lấy từ file gốc khi importer chạy),
# khoá UI mới econ.* (5 ngôn ngữ) và chuỗi game đổi ruby -> coin. Ghi chú: bản dịch do Claude, cần USER duyệt.
import json, os
T = {}
def put(k, vi, en, fr, es, pt):
    T[k] = {"vi": vi, "en": en, "fr": fr, "es": es, "pt": pt}
def tr(k, fr, es, pt):
    T[k] = {"fr": fr, "es": es, "pt": pt}

# ---- fr/es/pt cho loc-keys.json ----
tr("currency.coin", "Pièces", "Monedas", "Moedas")
for n, (f, e, p) in {"dq_win_3": ("Gagner 3 niveaux", "Gana 3 niveles", "Vença 3 níveis"),
    "dq_words_20": ("Trouver 20 mots", "Encuentra 20 palabras", "Encontre 20 palavras"),
    "dq_daily": ("Finir le Quotidien", "Completa el Diario", "Conclua o Diário"),
    "dq_bot_1": ("Jouer 1 match Bot", "Juega 1 partida Bot", "Jogue 1 partida Bot"),
    "dq_extra_1": ("Trouver 1 mot bonus", "Encuentra 1 palabra extra", "Encontre 1 palavra extra"),
    "wq_win_20": ("Gagner 20 niveaux", "Gana 20 niveles", "Vença 20 níveis"),
    "wq_words_150": ("Trouver 150 mots", "Encuentra 150 palabras", "Encontre 150 palavras"),
    "wq_daily_5": ("Finir le Quotidien 5 fois", "Completa el Diario 5 veces", "Conclua o Diário 5 vezes"),
    "wq_bot_win_3": ("Battre le Bot 3 fois", "Vence al Bot 3 veces", "Vença o Bot 3 vezes"),
    "wq_extra_5": ("Trouver 5 mots bonus", "Encuentra 5 palabras extra", "Encontre 5 palavras extras"),
    "wq_login_5": ("Se connecter 5 jours", "Entra 5 días", "Entre em 5 dias"),
    "mq_win_100": ("Gagner 100 niveaux", "Gana 100 niveles", "Vença 100 níveis"),
    "mq_daily_20": ("Finir le Quotidien 20 fois", "Completa el Diario 20 veces", "Conclua o Diário 20 vezes"),
    "mq_login_20": ("Se connecter 20 jours", "Entra 20 días", "Entre em 20 dias"),
    "mq_words_600": ("Trouver 600 mots", "Encuentra 600 palabras", "Encontre 600 palavras"),
    "eq_win": ("Gagner des niveaux pendant l'événement", "Gana niveles durante el evento", "Vença níveis durante o evento"),
    "eq_words": ("Trouver des mots pendant l'événement", "Encuentra palabras durante el evento", "Encontre palavras durante o evento"),
    "eq_daily": ("Quotidien pendant l'événement", "Diario durante el evento", "Diário durante o evento")}.items():
    tr("quest." + n, f, e, p)
for n, (f, e, p) in {"newyear": ("Nouvel An", "Año Nuevo", "Ano Novo"), "spring": ("Fête du printemps", "Fiesta de primavera", "Festa da primavera"),
    "hearts": ("Jour des cœurs", "Día de los corazones", "Dia dos corações"), "wordweek": ("Semaine des mots", "Semana de las palabras", "Semana das palavras"),
    "earth": ("Jour de la Terre", "Día de la Tierra", "Dia da Terra"), "summer": ("Été éclatant", "Verano radiante", "Verão radiante"),
    "midyear": ("Mi-année", "Mitad de año", "Meio do ano"), "school": ("Rentrée", "Vuelta al cole", "Volta às aulas"),
    "harvest": ("Moisson", "Cosecha", "Colheita"), "spooky": ("Nuit déguisée", "Noche de disfraces", "Noite das fantasias"),
    "thanks": ("Semaine merci", "Semana de gracias", "Semana de gratidão"), "winter": ("Hiver magique", "Invierno mágico", "Inverno mágico")}.items():
    tr("event." + n, f, e, p)
for m in range(1, 13):
    tr("month.%02d" % m, "Mois %d" % m, "Mes %d" % m, "Mês %d" % m)
tr("login.base", "Récompense de connexion", "Premio de conexión", "Prêmio de login")
tr("login.week", "Récompense du 7e jour", "Premio del día 7", "Prêmio do 7º dia")
tr("login.month", "Récompense de fin de mois", "Premio de fin de mes", "Prêmio de fim de mês")
tr("login.cycle", "Récompense de fin de cycle", "Premio de fin de ciclo", "Prêmio de fim de ciclo")
for d in (3, 7, 14, 30, 60, 100, 180, 365):
    tr("streak.%d" % d, "Série de %d jours" % d, "Racha de %d días" % d, "Sequência de %d dias" % d)
for n in (10, 25, 50, 100, 200, 300, 400, 534):
    tr("milestone.lm_%d" % n, "Finir %d niveaux" % n, "Completa %d niveles" % n, "Conclua %d níveis" % n)
tr("sink.hint_cell", "Indice : 1 case", "Pista: 1 casilla", "Dica: 1 casa")
tr("sink.hint_word", "Indice : mot entier", "Pista: palabra entera", "Dica: palavra inteira")
tr("sink.streak_repair", "Sauver la série", "Salvar racha", "Salvar sequência")
for i, n in enumerate((500, 1100, 2500, 4500, 11000), 1):
    tr("shop.pk_%d" % i, "Pack de %d pièces" % n, "Paquete de %d monedas" % n, "Pacote de %d moedas" % n)
tr("shop.pk_noads", "Sans pub", "Sin anuncios", "Sem anúncios")
tr("ad.loading", "Chargement de la pub…", "Cargando anuncio…", "Carregando anúncio…")
tr("ad.ready", "Prêt", "Listo", "Pronto")
tr("ad.cooldown", "En recharge", "En espera", "Em espera")
tr("ad.cap", "Limite du jour atteinte", "Límite diario alcanzado", "Limite diário atingido")
tr("ad.nofill", "Pas de pub, réessayez plus tard", "Sin anuncio, inténtalo luego", "Sem anúncio, tente mais tarde")
tr("ad.offline", "Vous êtes hors ligne", "Sin conexión", "Você está offline")
tr("ad.cancel", "Fermée tôt, pas de récompense", "Cerrado antes, sin premio", "Fechado cedo, sem prêmio")
tr("ad.error", "Erreur de pub, aucune pièce perdue", "Error de anuncio, sin perder monedas", "Erro no anúncio, nenhuma moeda perdida")
tr("ad.success", "Récompense obtenue", "Premio recibido", "Prêmio recebido")

# ---- khoá UI mới (5 ngôn ngữ) ----
put("econ.title", "Nhiệm vụ & Thưởng", "Quests & Rewards", "Quêtes et récompenses", "Misiones y premios", "Missões e prêmios")
put("econ.tab.today", "Hôm nay", "Today", "Aujourd'hui", "Hoy", "Hoje")
put("econ.tab.week", "Tuần", "Week", "Semaine", "Semana", "Semana")
put("econ.tab.month", "Tháng", "Month", "Mois", "Mes", "Mês")
put("econ.tab.event", "Sự kiện", "Event", "Événement", "Evento", "Evento")
put("econ.tab.year", "Năm", "Year", "Année", "Año", "Ano")
put("econ.claim", "NHẬN", "CLAIM", "PRENDRE", "COBRAR", "PEGAR")
put("econ.claimed", "ĐÃ NHẬN", "CLAIMED", "REÇU", "HECHO", "FEITO")
put("econ.login", "Thưởng đăng nhập hôm nay", "Today's login reward", "Connexion du jour", "Premio de hoy", "Prêmio de hoje")
put("econ.streak", "Chuỗi {n} ngày · Kỷ lục {b}", "{n}-day streak · Best {b}", "Série {n} j · Record {b}", "Racha {n} d · Récord {b}", "Sequência {n} d · Recorde {b}")
put("econ.repair", "Cứu chuỗi (lỡ 1 ngày)", "Repair streak (1 day missed)", "Sauver la série (1 jour manqué)", "Salvar racha (1 día perdido)", "Salvar sequência (1 dia perdido)")
put("econ.streakMilestones", "Mốc chuỗi", "Streak milestones", "Paliers de série", "Hitos de racha", "Marcos de sequência")
put("econ.levelMilestones", "Mốc level", "Level milestones", "Paliers de niveaux", "Hitos de niveles", "Marcos de níveis")
put("econ.dailyQuests", "Nhiệm vụ ngày · còn {t}", "Daily quests · {t} left", "Quêtes du jour · reste {t}", "Misiones del día · quedan {t}", "Missões do dia · faltam {t}")
put("econ.periodDays", "{p} · ngày {a}–{b}", "{p} · days {a}–{b}", "{p} · jours {a}–{b}", "{p} · días {a}–{b}", "{p} · dias {a}–{b}")
put("econ.weekN", "Tuần {n}", "Week {n}", "Semaine {n}", "Semana {n}", "Semana {n}")
put("econ.noWeek", "Ngày cầu nối: không có nhiệm vụ tuần", "Bridge day: no weekly quests", "Jour de transition : pas de quêtes hebdo", "Día puente: sin misiones semanales", "Dia de ligação: sem missões semanais")
put("econ.noEvent", "Không có sự kiện hôm nay", "No event today", "Pas d'événement aujourd'hui", "Hoy no hay evento", "Nenhum evento hoje")
put("econ.nextEvent", "Sự kiện kế: {e} (ngày {a})", "Next event: {e} (day {a})", "Prochain : {e} (jour {a})", "Próximo: {e} (día {a})", "Próximo: {e} (dia {a})")
put("econ.allClear", "Thưởng hoàn thành hết", "Clear-all bonus", "Bonus tout terminé", "Bono completar todo", "Bônus concluir tudo")
put("econ.free", "Nhiệm vụ không cần mua hay xem quảng cáo", "Quests never require purchases or ads", "Les quêtes n'exigent ni achat ni pub", "Las misiones no exigen compras ni anuncios", "As missões não exigem compras nem anúncios")
put("econ.soon", "Sắp có", "Coming soon", "Bientôt", "Pronto", "Em breve")
put("econ.progress", "{a}/{b} · +{n} coin", "{a}/{b} · +{n} coins", "{a}/{b} · +{n} pièces", "{a}/{b} · +{n} monedas", "{a}/{b} · +{n} moedas")
put("econ.reward", "+{n} coin", "+{n} coins", "+{n} pièces", "+{n} monedas", "+{n} moedas")
put("econ.cycle", "Chu kỳ {c} · Ngày {d}/365", "Cycle {c} · Day {d}/365", "Cycle {c} · Jour {d}/365", "Ciclo {c} · Día {d}/365", "Ciclo {c} · Dia {d}/365")
put("econ.dayWeek", "Ngày {d} · Tuần {w}", "Day {d} · Week {w}", "Jour {d} · Semaine {w}", "Día {d} · Semana {w}", "Dia {d} · Semana {w}")
put("econ.bridge", "Ngày {d} · Ngày cầu nối", "Day {d} · Bridge day", "Jour {d} · Transition", "Día {d} · Día puente", "Dia {d} · Dia de ligação")
put("econ.legend", "Viền xanh: hôm nay · Hồng: sự kiện · Vàng: cầu nối", "Teal border: today · Pink: event · Yellow: bridge", "Bord turquoise : aujourd'hui · Rose : événement · Jaune : transition", "Borde turquesa: hoy · Rosa: evento · Amarillo: puente", "Borda turquesa: hoje · Rosa: evento · Amarelo: ligação")
put("econ.quests", "Nhiệm vụ: {q}", "Quests: {q}", "Quêtes : {q}", "Misiones: {q}", "Missões: {q}")
put("econ.ad.left", "Còn {n} lượt hôm nay", "{n} left today", "Encore {n} aujourd'hui", "Quedan {n} hoy", "Restam {n} hoje")
put("econ.extraToast", "Từ thưởng: +{n} coin", "Bonus word: +{n} coins", "Mot bonus : +{n} pièces", "Palabra extra: +{n} monedas", "Palavra extra: +{n} moedas")
put("rv.s.cap", "Hết lượt hôm nay", "Daily limit reached", "Limite du jour atteinte", "Límite diario alcanzado", "Limite diário atingido")
put("rv.d.cap", "Quay lại ngày mai để xem tiếp. Chơi vẫn bình thường.", "Come back tomorrow for more videos. Play continues normally.", "Revenez demain pour d'autres vidéos. Le jeu continue normalement.", "Vuelve mañana para ver más vídeos. El juego sigue con normalidad.", "Volte amanhã para mais vídeos. O jogo continua normalmente.")

# ---- chuỗi game ruby -> coin ----
C = ("coin", "coins", "pièces", "monedas", "moedas")
put("common.rubies", *C)
put("common.balance", "Số coin", "Coin balance", "Solde de pièces", "Saldo de monedas", "Saldo de moedas")
put("common.addRubies", "Thêm coin", "Get coins", "Obtenir des pièces", "Conseguir monedas", "Ganhar moedas")
put("game.coach2", "Tìm đủ các từ để thắng. Không cần coin.", "Find every word to win. No coins needed.", "Trouvez tous les mots pour gagner. Aucune pièce requise.", "Encuentra todas las palabras para ganar. No hacen falta monedas.", "Encontre todas as palavras para vencer. Não precisa de moedas.")
put("how.3", "Xáo chữ miễn phí. Gợi ý tốn coin nhưng không bắt buộc.", "Shuffle is free. Hints cost coins but are never required.", "Mélanger est gratuit. Les indices coûtent des pièces mais ne sont jamais obligatoires.", "Mezclar es gratis. Las pistas cuestan monedas, pero nunca son obligatorias.", "Embaralhar é grátis. As dicas custam moedas, mas nunca são obrigatórias.")
put("win.first", "Lần đầu thắng: +{n} coin", "First clear: +{n} coins", "Première victoire : +{n} pièces", "Primera victoria: +{n} monedas", "Primeira vitória: +{n} moedas")
put("win.replay", "Chơi lại để luyện tập: không thêm coin", "Replay is practice: no extra coins", "Rejouer sert à s'entraîner : pas de pièces en plus", "Repetir es práctica: sin monedas extra", "Jogar de novo é treino: sem moedas extras")
put("win.x2", "XEM VIDEO: +{n} coin nữa", "WATCH VIDEO: +{n} more coins", "VIDÉO : +{n} pièces de plus", "VER VÍDEO: +{n} monedas más", "ASSISTIR VÍDEO: +{n} moedas a mais")
put("win.dailyReward", "Thưởng hôm nay: +{n} coin", "Today's reward: +{n} coins", "Récompense du jour : +{n} pièces", "Recompensa de hoy: +{n} monedas", "Recompensa de hoje: +{n} moedas")
put("win.milestone", "Mốc chuỗi {d} ngày: +{n} coin", "{d}-day streak bonus: +{n} coins", "Bonus pour {d} jours de suite : +{n} pièces", "Bono por {d} días seguidos: +{n} monedas", "Bônus por {d} dias seguidos: +{n} moedas")
put("extra.desc", "Tìm thêm từ mới để nhận coin", "Find more and more new words to collect coins", "Trouvez de nouveaux mots pour gagner des pièces", "Encuentra más palabras nuevas para conseguir monedas", "Encontre mais palavras novas para ganhar moedas")
put("extra.claim", "NHẬN +{n} COIN", "CLAIM +{n} COINS", "RÉCUPÉRER +{n} PIÈCES", "RECOGER +{n} MONEDAS", "RESGATAR +{n} MOEDAS")
put("extra.claimed", "Đã nhận +{n} coin", "Claimed +{n} coins", "+{n} pièces récupérées", "Recogidas +{n} monedas", "+{n} moedas resgatadas")
put("shop.free", "COIN MIỄN PHÍ", "FREE COINS", "PIÈCES GRATUITES", "MONEDAS GRATIS", "MOEDAS GRÁTIS")
put("shop.ready", "Xem video ngắn: +{n} coin", "Short video: +{n} coins", "Courte vidéo : +{n} pièces", "Vídeo corto: +{n} monedas", "Vídeo curto: +{n} moedas")
put("shop.pack", "{n} coin", "{n} coins", "{n} pièces", "{n} monedas", "{n} moedas")
put("rv.d.nofill", "Không trừ coin, không có thưởng. Thử lại sau.", "No coins charged, no reward. Try later.", "Aucune pièce débitée, aucune récompense. Réessayez plus tard.", "No se cobran monedas ni hay recompensa. Inténtalo más tarde.", "Nenhuma moeda cobrada e nenhuma recompensa. Tente mais tarde.")
put("rv.d.offline", "Coin từ video cần mạng. Chơi vẫn bình thường.", "Video coins need internet. Play continues normally.", "Les pièces vidéo nécessitent Internet. Le jeu continue normalement.", "Las monedas del vídeo necesitan internet. El juego sigue con normalidad.", "As moedas do vídeo precisam de internet. O jogo continua normalmente.")
put("rv.d.success", "Đã cộng {n} coin.", "{n} coins added.", "{n} pièces ajoutées.", "Se añadieron {n} monedas.", "{n} moedas adicionadas.")
for k in ("rv.d.cancel", "rv.d.error"):
    put(k, "Không có thưởng và không bị trừ coin.", "No reward and no coins charged.", "Aucune récompense et aucune pièce débitée.", "Sin recompensa y sin cobro de monedas.", "Sem recompensa e sem cobrança de moedas.")
put("rv.d.playingReal", "Xem đến cuối để nhận {n} coin.", "Watch to the end to get {n} coins.", "Regardez jusqu'à la fin pour obtenir {n} pièces.", "Mira hasta el final para obtener {n} monedas.", "Assista até o fim para ganhar {n} moedas.")
put("low.title", "CẦN {n} COIN", "NEED {n} COINS", "IL FAUT {n} PIÈCES", "HACEN FALTA {n} MONEDAS", "PRECISA DE {n} MOEDAS")
put("low.desc", "Bạn có {a} coin. Gợi ý là tuỳ chọn, bạn vẫn chơi tiếp được miễn phí.", "You have {a} coins. Hints are optional; you can keep playing for free.", "Vous avez {a} pièces. Les indices sont facultatifs, vous pouvez continuer à jouer gratuitement.", "Tienes {a} monedas. Las pistas son opcionales; puedes seguir jugando gratis.", "Você tem {a} moedas. As dicas são opcionais; você pode continuar jogando de graça.")
put("low.watch", "XEM VIDEO +{n} COIN (tuỳ chọn)", "WATCH VIDEO +{n} COINS (optional)", "VIDÉO +{n} PIÈCES (facultatif)", "VER VÍDEO +{n} MONEDAS (opcional)", "VÍDEO +{n} MOEDAS (opcional)")
put("daily.reward", "Thưởng: +{n} coin, nhận một lần", "Reward: +{n} coins, claimed once", "Récompense : +{n} pièces, une seule fois", "Recompensa: +{n} monedas, una sola vez", "Recompensa: +{n} moedas, resgatada uma vez")
put("daily.backfill", "Bù ngày bị lỡ: +{n} coin, giữ chuỗi", "Make-up day: +{n} coins, keeps your streak", "Jour de rattrapage : +{n} pièces, série conservée", "Día de recuperación: +{n} monedas, mantiene tu racha", "Dia de reposição: +{n} moedas, mantém sua sequência")
put("daily.nextMilestone", "Mốc kế: {d} ngày → +{n} coin", "Next bonus: {d} days → +{n} coins", "Prochain bonus : {d} jours → +{n} pièces", "Próximo bonus: {d} días → +{n} monedas", "Próximo bônus: {d} dias → +{n} moedas")
put("bot.d.easy", "Chậm, hay bỏ sót từ. Thắng +{n} coin", "Slow, often misses words. Win +{n} coins", "Lent, rate souvent des mots. Victoire +{n} pièces", "Lento, suele dejarse palabras. Victoria +{n} monedas", "Lento, costuma perder palavras. Vitória +{n} moedas")
put("bot.d.normal", "Tốc độ vừa phải. Thắng +{n} coin", "Steady pace. Win +{n} coins", "Rythme régulier. Victoire +{n} pièces", "Ritmo constante. Victoria +{n} monedas", "Ritmo constante. Vitória +{n} moedas")
put("bot.d.hard", "Nhanh, ít bỏ sót. Thắng +{n} coin", "Fast, rarely misses. Win +{n} coins", "Rapide, rate rarement. Victoire +{n} pièces", "Rápido, rara vez falla. Victoria +{n} monedas", "Rápido, raramente perde palavras. Vitória +{n} moedas")
put("bot.cap", "Thưởng bot hôm nay: {a}/{b} coin", "Bot rewards today: {a}/{b} coins", "Récompenses du bot aujourd'hui : {a}/{b} pièces", "Recompensas del bot hoy: {a}/{b} monedas", "Recompensas do bot hoje: {a}/{b} moedas")
put("bot.reward", "Thưởng: +{n} coin", "Reward: +{n} coins", "Récompense : +{n} pièces", "Recompensa: +{n} monedas", "Recompensa: +{n} moedas")
put("noti.toast", "Bạn nhận được {n} coin miễn phí", "You got {n} free coins", "Vous avez reçu {n} pièces gratuites", "Has conseguido {n} monedas gratis", "Você ganhou {n} moedas grátis")
put("noti.rubyTitle", "Quà coin", "Coin gift", "Cadeau de pièces", "Regalo de monedas", "Presente de moedas")
put("noti.ruby", "Bạn nhận được {n} coin miễn phí. Vào chơi và thử thách bản thân nhé", "You got {n} free coins. Play and challenge yourself!", "Vous avez reçu {n} pièces gratuites. Jouez et relevez le défi !", "Has conseguido {n} monedas gratis. ¡Juega y ponte a prueba!", "Você ganhou {n} moedas grátis. Jogue e desafie-se!")

json.dump(T, open(os.path.join(os.path.dirname(os.path.abspath(__file__)), "economy-loc.json"), "w", encoding="utf-8"), ensure_ascii=False, indent=1, sort_keys=True)
print(len(T))
