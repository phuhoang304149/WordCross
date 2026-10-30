// Native brand artwork and screenshot layouts. Does not change game images or call network APIs.
import Foundation
import CoreGraphics
import CoreText
import ImageIO
import UniformTypeIdentifiers
let repo = URL(fileURLWithPath: CommandLine.arguments.count > 1 ? CommandLine.arguments[1] : FileManager.default.currentDirectoryPath)
let out = URL(fileURLWithPath: FileManager.default.currentDirectoryPath).appendingPathComponent("design/store-listing")
let fontURL = repo.appendingPathComponent("Assets/WordCross/Fonts/Baloo2/static/Baloo2-ExtraBold.ttf")
let cgfont = CGFont(CGDataProvider(url: fontURL as CFURL)!)!
let space = CGColorSpace(name: CGColorSpace.sRGB)!
func ink(_ hex: String) -> CGColor {
 let n = UInt32(hex, radix:16)!
 return CGColor(colorSpace:space,components:[CGFloat((n>>16)&255)/255,CGFloat((n>>8)&255)/255,CGFloat(n&255)/255,1])!
}
func canvas(_ w: Int,_ h: Int) -> CGContext {
 let c = CGContext(data:nil,width:w,height:h,bitsPerComponent:8,bytesPerRow:w*4,space:space,bitmapInfo:CGImageAlphaInfo.noneSkipLast.rawValue)!
 c.translateBy(x:0,y:CGFloat(h)); c.scaleBy(x:1,y:-1)
 c.setAllowsAntialiasing(true)
 return c
}
func box(_ c: CGContext,_ r: CGRect,_ color: String,_ radius: CGFloat = 0) {
 c.setFillColor(ink(color)); c.addPath(CGPath(roundedRect:r,cornerWidth:radius,cornerHeight:radius,transform:nil)); c.fillPath()
}
func circle(_ c: CGContext,_ r: CGRect,_ color: String) {c.setFillColor(ink(color));c.fillEllipse(in:r)}
func text(_ c: CGContext,_ s: String,_ rect: CGRect,_ size: CGFloat,_ color: String,_ centered: Bool = false) {
 let lines = s.components(separatedBy:"\n")
 var useSize = size
 for line in lines {
  let f = CTFontCreateWithGraphicsFont(cgfont,useSize,nil,nil)
  let l = CTLineCreateWithAttributedString(NSAttributedString(string:line,attributes:[NSAttributedString.Key(kCTFontAttributeName as String):f]))
  let width = CGFloat(CTLineGetTypographicBounds(l,nil,nil,nil))
  if width > rect.width {useSize *= rect.width / width}
 }
 let f = CTFontCreateWithGraphicsFont(cgfont,useSize,nil,nil)
 for (i,line) in lines.enumerated() {
  let l = CTLineCreateWithAttributedString(NSAttributedString(string:line,attributes:[NSAttributedString.Key(kCTFontAttributeName as String):f,NSAttributedString.Key(kCTForegroundColorAttributeName as String):ink(color)]))
  let width = CGFloat(CTLineGetTypographicBounds(l,nil,nil,nil))
  c.saveGState();c.translateBy(x:rect.minX+(centered ? (rect.width-width)/2 : 0),y:rect.minY+useSize+CGFloat(i)*useSize*1.06);c.scaleBy(x:1,y:-1);c.textPosition = .zero;CTLineDraw(l,c);c.restoreGState()
 }
}
func load(_ path: String) -> CGImage {
 let src = CGImageSourceCreateWithURL(repo.appendingPathComponent(path) as CFURL,nil)!
 return CGImageSourceCreateImageAtIndex(src,0,nil)!
}
func image(_ c: CGContext,_ img: CGImage,_ r: CGRect) {
 c.saveGState();c.translateBy(x:r.minX,y:r.maxY);c.scaleBy(x:1,y:-1);c.interpolationQuality = .high;c.draw(img,in:CGRect(x:0,y:0,width:r.width,height:r.height));c.restoreGState()
}
func save(_ c: CGContext,_ relative: String) {
 let url=out.appendingPathComponent(relative)
 let dest = CGImageDestinationCreateWithURL(url as CFURL,UTType.png.identifier as CFString,1,nil)!
 CGImageDestinationAddImage(dest,c.makeImage()!,nil);precondition(CGImageDestinationFinalize(dest))
}
struct Locale {
 let folder: String; let lang: String; let tagline: String; let captions: [String]
}
let locales = [
 Locale(folder:"en-US",lang:"en",tagline:"One swipe.\nSo many words.",captions:["Play at your own pace","Explore new worlds","Swipe. Connect. Solve.","A daily word challenge","Celebrate every puzzle"]),
 Locale(folder:"vi",lang:"vi",tagline:"Nối từng chữ.\nKhám phá từng từ.",captions:["Chơi theo nhịp của bạn","Khám phá các thế giới","Vuốt chữ. Ghép từ.","Thử thách từ mỗi ngày","Niềm vui mỗi lần giải đố"]),
 Locale(folder:"fr-FR",lang:"fr",tagline:"Reliez les lettres.\nTrouvez les mots.",captions:["Jouez à votre rythme","Explorez des mondes","Reliez les lettres","Un défi de mots par jour","Savourez chaque réussite"]),
 Locale(folder:"es-ES",lang:"es",tagline:"Une las letras.\nDescubre palabras.",captions:["Juega a tu ritmo","Explora nuevos mundos","Une letras. Forma palabras.","Un reto de palabras al día","Celebra cada solución"]),
 Locale(folder:"pt-BR",lang:"pt",tagline:"Conecte letras.\nDescubra palavras.",captions:["Jogue no seu ritmo","Explore novos mundos","Ligue letras. Forme palavras.","Um desafio de palavras por dia","Comemore cada conquista"])
]
let screens = ["home","worlds","gameplay","daily","win"]
let qa = "Build/remote-r2-0.3/qa-shots/"
let qaURL = repo.appendingPathComponent(qa)
let qaFiles: [String] = ["preview-l10n-after-daily-en-20261030-014705.png", "preview-l10n-after-daily-vi-20261030-014340.png", "preview-l10n-after-daily-fr-20261030-015028.png", "preview-l10n-after-daily-es-20261030-015351.png", "preview-l10n-after-daily-pt-20261030-015717.png", "preview-l10n-after-win-en-20261030-014730.png", "preview-l10n-after-win-vi-20261030-014405.png", "preview-l10n-after-win-fr-20261030-015053.png", "preview-l10n-after-win-es-20261030-015417.png", "preview-l10n-after-win-pt-20261030-015742.png"]
func source(_ screen: String,_ lang: String) -> String {
 if screen == "home" {return qa+"t129-final-home-\(lang).png"}
 if screen == "worlds" {return qa+"l10n-final-14-selectworld-\(lang).png"}
 if screen == "gameplay" {return qa+"l10n-final-17-main-\(lang).png"}
 let prefix = "preview-l10n-after-\(screen)-\(lang)-"
 let found = qaFiles.filter{$0.hasPrefix(prefix) && $0.hasSuffix(".png")}.sorted().last!
 return qa+found
}
var manifest = "# Image assets — 2026-10-30\n\nDefault: en-US. UI locales: en, vi, fr, es, pt; Store mappings fr-FR/es-ES/pt-BR.\n\nFeature graphics: 1024×500, opaque RGB PNG; native artwork derived from existing brand tiles, palette and Baloo 2. App icon: 512×512 from current 1024×1024 artwork, shared across locales. No AI-generated gameplay.\n\n**Screenshot layouts are DRAFTS, not ready to upload.** 1080×1920; each contains a full, unmodified runtime QA capture fitted proportionally, plus localized promotional caption. Source images include status bar/floating gamepad and some older UGUI/debug-preview states. They do not certify the latest TMP/release build. Replace capture panels with clean current-runtime screenshots in matching UI locale before upload. Never claim translated puzzle dictionaries from translated UI. Keep this source list; original QA captures remain in their existing location. No new device session/build/upload was run.\n\n| Locale | Screen | Source |\n|---|---|---|\n"
for loc in locales {
 let folder=out.appendingPathComponent(loc.folder)
 precondition(FileManager.default.fileExists(atPath:folder.path),"Create locale output directories first")
 let c=canvas(1024,500)
 box(c,CGRect(x:0,y:0,width:1024,height:500),"12b5a6")
 circle(c,CGRect(x:642,y:-154,width:570,height:570),"20c3b4")
 circle(c,CGRect(x:-130,y:370,width:340,height:340),"ffc93c")
 text(c,"Word Crossy",CGRect(x:64,y:48,width:570,height:88),76,"ffffff")
 text(c,loc.tagline,CGRect(x:66,y:176,width:530,height:150),44,"ffffff")
 box(c,CGRect(x:68,y:362,width:84,height:7),"ffc93c",3)
 // Existing W/C brand motif, not a fabricated gameplay screen.
 box(c,CGRect(x:674,y:92,width:180,height:180),"0b8c80",40)
 box(c,CGRect(x:674,y:80,width:180,height:180),"ffffff",40)
 text(c,"W",CGRect(x:694,y:101,width:140,height:120),114,"0b8c80",true)
 box(c,CGRect(x:790,y:232,width:157,height:157),"e0a413",36)
 box(c,CGRect(x:790,y:220,width:157,height:157),"ffc93c",36)
 text(c,"C",CGRect(x:807,y:237,width:123,height:113),104,"5a4100",true)
 save(c,"\(loc.folder)/feature-graphic.png")
 for (i,screen) in screens.enumerated() {
  let s=source(screen,loc.lang), raw=load(s)
  let c=canvas(1080,1920)
  box(c,CGRect(x:0,y:0,width:1080,height:1920),"12b5a6")
  circle(c,CGRect(x:760,y:-160,width:500,height:500),"20c3b4")
  circle(c,CGRect(x:-240,y:1620,width:620,height:620),"ffc93c")
  text(c,"WORD CROSSY",CGRect(x:64,y:35,width:900,height:40),28,"ffffff")
  text(c,loc.captions[i],CGRect(x:64,y:105,width:952,height:100),64,"ffffff")
  let h:CGFloat=1574, w=h*CGFloat(raw.width)/CGFloat(raw.height), x=(1080-w)/2
  box(c,CGRect(x:x-7,y:268,width:w+14,height:h+14),"0b8c80",10)
  image(c,raw,CGRect(x:x,y:275,width:w,height:h))
  save(c,"\(loc.folder)/screenshot-draft-0\(i+1)-\(screen).png")
  manifest += "| \(loc.folder) | \(screen) | `\(s)` |\n"
 }
}
let icon=canvas(512,512);image(icon,load("design/codex/icon/icon-1024.png"),CGRect(x:0,y:0,width:512,height:512));save(icon,"app-icon-512.png")
let data=Array(manifest.utf8);let f=fopen(out.appendingPathComponent("IMAGE-ASSETS.md").path,"wb")!;data.withUnsafeBytes{precondition(fwrite($0.baseAddress,1,$0.count,f)==$0.count)};fclose(f)
let sheet=canvas(1600,1400);box(sheet,CGRect(x:0,y:0,width:1600,height:1400),"f2f8f8")
for (row,loc) in locales.enumerated() {
 text(sheet,loc.folder,CGRect(x:20,y:CGFloat(row*280+10),width:250,height:40),28,"0b8c80")
 let rel="design/store-listing/\(loc.folder)/feature-graphic.png"
 let src=CGImageSourceCreateWithURL(out.deletingLastPathComponent().deletingLastPathComponent().appendingPathComponent(rel) as CFURL,nil)!
 let img=CGImageSourceCreateImageAtIndex(src,0,nil)!
 image(sheet,img,CGRect(x:20,y:CGFloat(row*280+54),width:430,height:210))
 for i in 0..<5 {
  let path=out.appendingPathComponent("\(loc.folder)/screenshot-draft-0\(i+1)-\(screens[i]).png")
  let im=CGImageSourceCreateImageAtIndex(CGImageSourceCreateWithURL(path as CFURL,nil)!,0,nil)!
  image(sheet,im,CGRect(x:CGFloat(490+i*218),y:CGFloat(row*280+8),width:145,height:258))
 }
}
save(sheet,"preview-all-locales.png")
print("Generated 5 feature graphics, 25 screenshot drafts, shared icon and preview.")
