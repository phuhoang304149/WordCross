/* Registry of every screen/popup. src = source file(s) the design is matched to; tên/ghi chú lấy từ i18n key scr.<id>.name/note.
   origin: 'observed' = có trong Unity gốc; 'proposed' = đề xuất mới trong thiết kế. */
window.WC_SCREENS = [
  {id:'home', kind:'screen', origin:'observed', src:'_Scenes/Home.unity · HomeController.cs (Daily/Bot/AI/Language là đề xuất)'},
  {id:'select-world', kind:'screen', origin:'observed', src:'_Scenes/SelectWorld.unity · WorldItem.prefab · WorldController.cs'},
  {id:'select-level', kind:'screen', origin:'observed', src:'_Scenes/SelectLevel.unity · LevelButton.prefab · SelectLevelController.cs · Utils.GetNumLevels'},
  {id:'game', kind:'screen', origin:'observed', src:'_Scenes/Main.unity · MainController.cs · WordRegion.cs · Pan.cs · LineWord.cs'},
  {id:'daily', kind:'screen', origin:'proposed', src:'Tham khảo: Wordle DailyChallengeService/View, TileConnect-Onet daily.js'},
  {id:'bot-setup', kind:'screen', origin:'proposed', src:'Tham khảo: BlastBattle bot-gameplay.html, bot-engine.js'},
  {id:'bot-match', kind:'screen', origin:'proposed', src:'Tham khảo: BlastBattle bot-ui.js · Wordle WordleBot.cs'},
  {id:'language', kind:'screen', origin:'proposed', src:'Level assets (World_*/SubWorld_*) chỉ tiếng Anh; 4 thư mục từ điển chưa được code tham chiếu'},
  {id:'pause', kind:'popup', base:'game', origin:'observed', src:'_Prefabs/Dialogs/PauseDialog.prefab'},
  {id:'how-to-play', kind:'popup', base:'game', origin:'observed', src:'_Prefabs/Dialogs/HowToPlayDialog.prefab'},
  {id:'win', kind:'popup', base:'game', origin:'observed', src:'_Prefabs/Dialogs/WinDialog.prefab · WinDialog.cs (thưởng/x2/mục tiêu là đề xuất)'},
  {id:'extra-word', kind:'popup', base:'game', origin:'observed', src:'_Prefabs/Dialogs/ExtraWordDialog.prefab · ExtraWordDialog.cs'},
  {id:'low-ruby', kind:'popup', base:'game', origin:'proposed', src:'Gốc: HintClick mở Shop trực tiếp (WordRegion.cs)'},
  {id:'shop', kind:'popup', base:'home', origin:'observed', src:'_Prefabs/Dialogs/ShopDialog.prefab · ShopItem.prefab · ShopDialog.cs'},
  {id:'rewarded-video', kind:'popup', base:'home', origin:'observed', src:'_Prefabs/Dialogs/RewardedVideoDialog.prefab · RewardedButton.cs (các trạng thái là đề xuất)'},
  {id:'rate', kind:'popup', base:'home', origin:'observed', src:'_Prefabs/Dialogs/RateDialog.prefab'},
  {id:'bot-result', kind:'popup', base:'bot-match', origin:'proposed', src:'Tham khảo: BlastBattle GameOver (Chơi lại / Về Home)'},
  {id:'ai-soon', kind:'popup', base:'home', origin:'proposed', src:'Chỉ Coming soon, không có nguồn gameplay'}
];
