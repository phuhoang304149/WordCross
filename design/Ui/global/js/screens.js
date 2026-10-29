/* Registry of every screen/popup. src = source file(s) the design is matched to. */
window.WC_SCREENS = [
  {id:'home', kind:'screen', name:'Home', note:'Nền, logo WORD CROSS, Play, Shop', src:'_Scenes/Home.unity · HomeController.cs'},
  {id:'select-world', kind:'screen', name:'Chọn World', note:'Vuốt snap theo World 0–5, mỗi World 5 subworld, n/total', src:'_Scenes/SelectWorld.unity · WorldItem.prefab · WorldController.cs'},
  {id:'select-level', kind:'screen', name:'Chọn Level', note:'Lưới LevelButton cuộn dọc, tới level đã mở khoá', src:'_Scenes/SelectLevel.unity · LevelButton.prefab · SelectLevelController.cs'},
  {id:'game', kind:'screen', name:'Gameplay', note:'Lưới ô từ, vòng chữ vuốt, Hint/Shuffle/Ask friend, Extra words', src:'_Scenes/Main.unity · MainController.cs · WordRegion.cs · Pan.cs'},
  {id:'pause', kind:'popup', base:'game', name:'Pause', note:'Continue · To Menu · Quit · How to play · nhạc/âm', src:'_Prefabs/Dialogs/PauseDialog.prefab'},
  {id:'how-to-play', kind:'popup', base:'game', name:'How to play', note:'Hướng dẫn vuốt tạo từ, dùng Shuffle/Hint', src:'_Prefabs/Dialogs/HowToPlayDialog.prefab'},
  {id:'win', kind:'popup', base:'game', name:'Win', note:'Hoàn thành level: Next · Menu', src:'_Prefabs/Dialogs/WinDialog.prefab · WinDialog.cs'},
  {id:'extra-word', kind:'popup', base:'game', name:'Extra words', note:'Tiến độ 1/10 từ thưởng, Claim ruby', src:'_Prefabs/Dialogs/ExtraWordDialog.prefab'},
  {id:'shop', kind:'popup', base:'home', name:'Shop', note:'5 gói ruby, xem video miễn phí, bỏ quảng cáo khi mua', src:'_Prefabs/Dialogs/ShopDialog.prefab · ShopItem.prefab'},
  {id:'rewarded-video', kind:'popup', base:'home', name:'Rewarded video', note:'Nhận ruby miễn phí sau khi xem quảng cáo', src:'_Prefabs/Dialogs/RewardedVideoDialog.prefab'},
  {id:'rate', kind:'popup', base:'home', name:'Rate', note:'YES / LATER đánh giá 5 sao', src:'_Prefabs/Dialogs/RateDialog.prefab'}
];
