// CLI renderer of the existing HTML design using WebKit, not Chrome or a device.
import AppKit
import WebKit
import CoreText
import ImageIO
import UniformTypeIdentifiers
let root = URL(fileURLWithPath: FileManager.default.currentDirectoryPath)
let target = root.appendingPathComponent("design/store-listing")
let base = "http://127.0.0.1:18670"
let space = CGColorSpace(name: CGColorSpace.sRGB)!
let font = CGFont(CGDataProvider(url:root.appendingPathComponent("Assets/WordCross/Fonts/Baloo2/static/Baloo2-ExtraBold.ttf") as CFURL)!)!
struct Locale {
 let folder:String; let lang:String; let captions:[String]
}
let locales = [
 Locale(folder:"en-US",lang:"en",captions:["Play at your own pace","Explore new worlds","Swipe. Connect. Solve.","A daily word challenge","Celebrate every puzzle"]),
 Locale(folder:"vi",lang:"vi",captions:["Chơi theo nhịp của bạn","Khám phá các thế giới","Vuốt chữ. Ghép từ.","Thử thách từ mỗi ngày","Niềm vui mỗi lần giải đố"]),
 Locale(folder:"fr-FR",lang:"fr",captions:["Jouez à votre rythme","Explorez des mondes","Reliez les lettres","Un défi de mots par jour","Savourez chaque réussite"]),
 Locale(folder:"es-ES",lang:"es",captions:["Juega a tu ritmo","Explora nuevos mundos","Une letras. Forma palabras.","Un reto de palabras al día","Celebra cada solución"]),
 Locale(folder:"pt-BR",lang:"pt",captions:["Jogue no seu ritmo","Explore novos mundos","Ligue letras. Forme palavras.","Um desafio de palavras por dia","Comemore cada conquista"])
]
let screens=["home","select-world","game","daily","win"]
let filenames=["home","worlds","gameplay","daily","win"]
func ink(_ hex:String)->CGColor {
 let n=UInt32(hex,radix:16)!
 return CGColor(colorSpace:space,components:[CGFloat((n>>16)&255)/255,CGFloat((n>>8)&255)/255,CGFloat(n&255)/255,1])!
}
func canvas(_ w:Int,_ h:Int)->CGContext {
 let c=CGContext(data:nil,width:w,height:h,bitsPerComponent:8,bytesPerRow:w*4,space:space,bitmapInfo:CGImageAlphaInfo.noneSkipLast.rawValue)!
 c.translateBy(x:0,y:CGFloat(h));c.scaleBy(x:1,y:-1);return c
}
func box(_ c:CGContext,_ r:CGRect,_ color:String,_ radius:CGFloat=0) {
 c.setFillColor(ink(color));c.addPath(CGPath(roundedRect:r,cornerWidth:radius,cornerHeight:radius,transform:nil));c.fillPath()
}
func ellipse(_ c:CGContext,_ r:CGRect,_ color:String){c.setFillColor(ink(color));c.fillEllipse(in:r)}
func text(_ c:CGContext,_ s:String,_ r:CGRect,_ size:CGFloat,_ color:String){
 let f=CTFontCreateWithGraphicsFont(font,size,nil,nil)
 let attrs:[NSAttributedString.Key:Any]=[NSAttributedString.Key(kCTFontAttributeName as String):f]
 let l=CTLineCreateWithAttributedString(NSAttributedString(string:s,attributes:attrs))
 let width=CGFloat(CTLineGetTypographicBounds(l,nil,nil,nil))
 let use=min(size,size*r.width/max(width,1))
 let ff=CTFontCreateWithGraphicsFont(font,use,nil,nil)
 let line=CTLineCreateWithAttributedString(NSAttributedString(string:s,attributes:[NSAttributedString.Key(kCTFontAttributeName as String):ff,NSAttributedString.Key(kCTForegroundColorAttributeName as String):ink(color)]))
 c.saveGState();c.translateBy(x:r.minX,y:r.minY+use);c.scaleBy(x:1,y:-1);c.textPosition = .zero;CTLineDraw(line,c);c.restoreGState()
}
func draw(_ c:CGContext,_ img:CGImage,_ r:CGRect){c.saveGState();c.translateBy(x:r.minX,y:r.maxY);c.scaleBy(x:1,y:-1);c.interpolationQuality = .high;c.draw(img,in:CGRect(x:0,y:0,width:r.width,height:r.height));c.restoreGState()}
func save(_ c:CGContext,_ url:URL){
 let dest=CGImageDestinationCreateWithURL(url as CFURL,UTType.png.identifier as CFString,1,nil)!
 CGImageDestinationAddImage(dest,c.makeImage()!,nil);precondition(CGImageDestinationFinalize(dest))
}
func load(_ url:URL)->CGImage{CGImageSourceCreateImageAtIndex(CGImageSourceCreateWithURL(url as CFURL,nil)!,0,nil)!}
func compose(_ image:CGImage,_ row:Int,_ col:Int){
 let c=canvas(1080,1920)
 box(c,CGRect(x:0,y:0,width:1080,height:1920),"12b5a6")
 ellipse(c,CGRect(x:760,y:-160,width:500,height:500),"20c3b4")
 ellipse(c,CGRect(x:-240,y:1620,width:620,height:620),"ffc93c")
 text(c,"WORD CROSSY",CGRect(x:64,y:35,width:900,height:40),28,"ffffff")
 text(c,locales[row].captions[col],CGRect(x:64,y:105,width:952,height:100),64,"ffffff")
 let h:CGFloat=1574,w=h*390/844,x=(1080-w)/2
 box(c,CGRect(x:x-7,y:268,width:w+14,height:h+14),"0b8c80",10)
 draw(c,image,CGRect(x:x,y:275,width:w,height:h))
 save(c,target.appendingPathComponent("\(locales[row].folder)/screenshot-0\(col+1)-\(filenames[col]).png"))
}
func finish(){
 let c=canvas(1600,1400);box(c,CGRect(x:0,y:0,width:1600,height:1400),"f2f8f8")
 for (row,loc) in locales.enumerated(){
  text(c,loc.folder,CGRect(x:20,y:CGFloat(row*280+10),width:250,height:40),28,"0b8c80")
  draw(c,load(target.appendingPathComponent("\(loc.folder)/feature-graphic.png")),CGRect(x:20,y:CGFloat(row*280+54),width:430,height:210))
  for i in 0..<5{draw(c,load(target.appendingPathComponent("\(loc.folder)/screenshot-0\(i+1)-\(filenames[i]).png")),CGRect(x:CGFloat(490+i*218),y:CGFloat(row*280+8),width:145,height:258))}
 }
 save(c,target.appendingPathComponent("preview-all-locales.png"))
 print("Rendered 25 design-based store images, 1080x1920 RGB, and preview.")
 fflush(stdout);exit(0)
}
class Renderer:NSObject,WKNavigationDelegate,WKScriptMessageHandler {
 var index=0
 var web:WKWebView!
 var window:NSWindow!
 func start(){
  let config=WKWebViewConfiguration()
  config.websiteDataStore = .nonPersistent()
  config.userContentController.add(self,name:"ready")
  config.preferences.javaScriptCanOpenWindowsAutomatically=false
  web=WKWebView(frame:NSRect(x:0,y:0,width:390,height:844),configuration:config)
  web.navigationDelegate=self
  window=NSWindow(contentRect:NSRect(x:-10000,y:0,width:390,height:844),styleMask:.borderless,backing:.buffered,defer:false)
  window.contentView=web;window.orderFrontRegardless()
  next()
 }
 func next(){
  if index>=25{finish();return}
  let row=index/5,col=index%5,loc=locales[row],screen=screens[col]
  let file=root.appendingPathComponent("design/Ui/screens/\(screen)/index.html")
  var html=String(data:FileManager.default.contents(atPath:file.path)!,encoding:.utf8)!
  let css="""
  @font-face{font-family:'StoreBaloo';src:url('\(base)/Assets/WordCross/Fonts/Baloo2/static/Baloo2-ExtraBold.ttf') format('truetype');font-weight:700 900}
  @font-face{font-family:'StoreBaloo';src:url('\(base)/Assets/WordCross/Fonts/Baloo2/static/Baloo2-SemiBold.ttf') format('truetype');font-weight:400 600}
  :root{--font:'StoreBaloo',system-ui,sans-serif}body{font-family:var(--font)!important}.tile{font-family:var(--font)!important}.demo-tag,.tagx,.demo-row{display:none!important}*{animation:none!important;transition:none!important}.phone{border-radius:0!important}.daily-hero>div{display:flex;flex-direction:column;gap:5px}body[data-page=daily] .scroll-body>.note:last-child{display:none!important}
  """
  html=html.replacingOccurrences(of:"</head>",with:"<style>\(css)</style></head>")
  let setup="""
  var S=WC.state;S.gameLang='en';S.rubies=18;S.ads.removed=true;S.level={w:1,s:0,l:0};S.sel={w:1,s:0};S.cleared={};for(var s=0;s<5;s++){for(var l=0;l<WC.subTotal(0,s);l++)S.cleared['0-'+s+'-'+l]=1}S.progress['1-0-0']={hints:{},found:\(col==4 ? "WC_LEVELS['1-0-0'].a.slice()" : "['part','tray']"),extras:[],order:'party'.split('')};S.daily.today='2026-10-30';S.daily.solved={};for(var d=24;d<=29;d++)S.daily.solved['2026-10-'+d]=1;S.daily.best=6;S.winCtx={kind:'level',key:'1-0-0',first:true,reward:2};
  """
  html=html.replacingOccurrences(of:"<script src=\"../../global/js/app.js\">",with:"<script>\(setup)</script><script src=\"../../global/js/app.js\">")
  let end="""
  <script>
  if('\(screen)'==='game'){
   var pts=[];document.querySelectorAll('.ltr').forEach(function(e){e.classList.add('sel');pts.push(parseFloat(e.style.left)+','+parseFloat(e.style.top))});document.querySelector('[data-line]').setAttribute('points',pts.join(' '));document.querySelector('[data-preview]').innerHTML='PARTY'.split('').map(function(x){return '<b>'+x+'</b>'}).join('');
  }
  document.fonts.ready.then(function(){window.webkit.messageHandlers.ready.postMessage({lang:document.documentElement.lang,screen:document.body.dataset.page,phone:document.querySelector('.phone').getBoundingClientRect().toJSON(),text:document.querySelector('.phone').innerText.slice(0,140)})});
  </script>
  """
  html=html.replacingOccurrences(of:"</body>",with:end+"</body>")
  let url=URL(string:"\(base)/design/Ui/screens/\(screen)/index.html?embed=1&lang=\(loc.lang)")!
  web.loadHTMLString(html,baseURL:url)
 }
 func userContentController(_ userContentController:WKUserContentController,didReceive message:WKScriptMessage){
  let row=index/5,col=index%5
  guard let result=message.body as? [String:Any],let lang=result["lang"] as? String,lang==locales[row].lang else{print("Locale did not load: \(message.body)");exit(1)}
  print("\(locales[row].folder)/\(screens[col]): \(result["text"] ?? "")");fflush(stdout)
  let config=WKSnapshotConfiguration();config.rect=CGRect(x:0,y:0,width:390,height:844);config.snapshotWidth=1080;config.afterScreenUpdates=false
  web.takeSnapshot(with:config){img,error in
   guard error==nil,let img=img,let cg=img.cgImage(forProposedRect:nil,context:nil,hints:nil) else{print("Snapshot failed: \(String(describing:error))");exit(1)}
   compose(cg,row,col);self.index+=1;self.next()
  }
 }
 func webView(_ webView:WKWebView,didFail navigation:WKNavigation!,withError error:Error){print("Navigation failed: \(error.localizedDescription)");exit(1)}
 func webView(_ webView:WKWebView,didFailProvisionalNavigation navigation:WKNavigation!,withError error:Error){print("Load failed: \(error.localizedDescription)");exit(1)}
}
let app=NSApplication.shared;app.setActivationPolicy(.prohibited)
let renderer=Renderer();renderer.start();app.run()
