import Foundation
import CoreGraphics
import CoreText
import ImageIO
import UniformTypeIdentifiers

let root = URL(fileURLWithPath: FileManager.default.currentDirectoryPath).appendingPathComponent("design/codex")
let fontURL = URL(fileURLWithPath: FileManager.default.currentDirectoryPath).appendingPathComponent("Assets/WordCross/Fonts/Baloo2/static/Baloo2-ExtraBold.ttf")
let provider = CGDataProvider(url: fontURL as CFURL)!
let cgFont = CGFont(provider)!
let font = CTFontCreateWithGraphicsFont(cgFont, 100, nil, nil)
let teal = "#12b5a6", deep = "#0b8c80", yellow = "#ffc93c", brown = "#5a4100"

func color(_ hex: String) -> CGColor {
    let n = UInt32(hex.dropFirst(), radix: 16)!
    return CGColor(colorSpace:CGColorSpace(name:CGColorSpace.sRGB)!, components:[CGFloat((n >> 16) & 255)/255, CGFloat((n >> 8) & 255)/255, CGFloat(n & 255)/255, 1])!
}
func svgPath(_ path: CGPath) -> String {
    var data = ""
    func pt(_ p: CGPoint) -> String { String(format: "%.3f %.3f", Double(p.x), Double(p.y)) }
    path.applyWithBlock { raw in
        let e = raw.pointee
        switch e.type {
        case .moveToPoint: data += "M" + pt(e.points[0])
        case .addLineToPoint: data += "L" + pt(e.points[0])
        case .addQuadCurveToPoint: data += "Q" + pt(e.points[0]) + " " + pt(e.points[1])
        case .addCurveToPoint: data += "C" + pt(e.points[0]) + " " + pt(e.points[1]) + " " + pt(e.points[2])
        case .closeSubpath: data += "Z"
        @unknown default: break
        }
    }
    return data
}
struct Shape { let path: CGPath; let fill: String }
func rect(_ x: CGFloat, _ y: CGFloat, _ w: CGFloat, _ h: CGFloat, _ r: CGFloat, _ fill: String) -> Shape {
    Shape(path: CGPath(roundedRect: CGRect(x:x,y:y,width:w,height:h), cornerWidth:r, cornerHeight:r, transform:nil), fill:fill)
}
func lettering(_ text: String, _ box: CGRect, _ fill: String) -> Shape {
    let utf = Array(text.utf16)
    var glyphs = [CGGlyph](repeating:0,count:utf.count)
    CTFontGetGlyphsForCharacters(font, utf, &glyphs, utf.count)
    let combined = CGMutablePath()
    var cursor: CGFloat = 0
    for glyph in glyphs {
        if let p = CTFontCreatePathForGlyph(font,glyph,nil) {
            let t = CGAffineTransform(translationX:cursor,y:0)
            combined.addPath(p,transform:t)
        }
        var g = glyph
        var advance = CGSize.zero
        CTFontGetAdvancesForGlyphs(font,.horizontal,&g,&advance,1)
        cursor += advance.width
    }
    let b = combined.boundingBoxOfPath
    let s = min(box.width/b.width,box.height/b.height)
    var t = CGAffineTransform(a:s,b:0,c:0,d:-s,tx:box.midX-b.midX*s,ty:box.midY+b.midY*s)
    return Shape(path:combined.copy(using:&t)!,fill:fill)
}
func saveSVG(_ relative: String, _ width: Int, _ height: Int, _ shapes: [Shape], _ title: String) {
    let paths = shapes.map { "<path fill=\"\($0.fill)\" d=\"\(svgPath($0.path))\"/>" }.joined(separator:"\n")
    let data = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"\(width)\" height=\"\(height)\" viewBox=\"0 0 \(width) \(height)\" role=\"img\" aria-labelledby=\"title\"><title id=\"title\">\(title)</title>\n<!-- Baloo 2 ExtraBold, converted to paths. -->\n\(paths)\n</svg>\n"
    let file = fopen(root.appendingPathComponent(relative).path,"wb")!
    let bytes = Array(data.utf8)
    let count = bytes.withUnsafeBytes { fwrite($0.baseAddress,1,$0.count,file) }
    precondition(count == bytes.count)
    fclose(file)
}
func render(_ relative: String, _ width: Int, _ height: Int, _ viewWidth: CGFloat, _ viewHeight: CGFloat, _ shapes:[Shape]) {
    let context = CGContext(data:nil,width:width,height:height,bitsPerComponent:8,bytesPerRow:width*4,space:CGColorSpace(name:CGColorSpace.sRGB)!,bitmapInfo:CGImageAlphaInfo.premultipliedLast.rawValue)!
    context.translateBy(x:0,y:CGFloat(height))
    context.scaleBy(x:CGFloat(width)/viewWidth,y:-CGFloat(height)/viewHeight)
    context.setAllowsAntialiasing(true)
    context.setShouldAntialias(true)
    for shape in shapes { context.setFillColor(color(shape.fill)); context.addPath(shape.path); context.fillPath() }
    let image = context.makeImage()!
    let destination = CGImageDestinationCreateWithURL(root.appendingPathComponent(relative) as CFURL, UTType.png.identifier as CFString,1,nil)!
    CGImageDestinationAddImage(destination,image,nil)
    precondition(CGImageDestinationFinalize(destination))
}

// Full icon artwork stays inside a circle of radius 326 (< 33% of 1024).
// Foreground layer uses the same geometry: 432 / 1024, centered without rescaling later.
var mark:[Shape] = []
mark.append(rect(285,307,300,300,72,deep))
mark.append(rect(285,285,300,300,72,"#ffffff"))
mark.append(lettering("W",CGRect(x:319,y:338,width:232,height:126),deep))
mark.append(rect(476,494,264,264,64,"#e0a413"))
mark.append(rect(476,474,264,264,64,yellow))
mark.append(lettering("C",CGRect(x:536,y:528,width:144,height:154),brown))
let icon = [rect(0,0,1024,1024,0,teal)] + mark
saveSVG("icon/icon.svg",1024,1024,icon,"WordCross app icon — white W tile and yellow C tile")
render("icon/icon-1024.png",1024,1024,1024,1024,icon)
render("icon/icon-fg-432.png",432,432,1024,1024,mark)
render("icon/icon-bg-432.png",432,432,432,432,[rect(0,0,432,432,0,teal)])
render("icon/icon-48.png",48,48,1024,1024,icon)
saveSVG("icon/icon-foreground.svg",1024,1024,mark,"WordCross adaptive foreground")

var splash:[Shape] = []
let letters = Array("WORD").map(String.init)
for i in 0..<4 {
    let x = CGFloat(80+i*220)
    let fill = ["#ffffff",yellow,"#ff6b5e","#ffffff"][i]
    let ink = [deep,brown,"#ffffff",deep][i]
    splash.append(rect(x,307,204,204,52,deep))
    splash.append(rect(x,289,204,204,52,fill))
    splash.append(lettering(letters[i],CGRect(x:x+28,y:341,width:148,height:103),ink))
}
splash.append(lettering("CROSS",CGRect(x:172,y:554,width:680,height:154),"#ffffff"))
saveSVG("splash/splash-logo.svg",1024,1024,splash,"WordCross splash — WORD tiles and CROSS")
render("splash/splash-logo-1024.png",1024,1024,1024,1024,splash)
// Center the visual bounds, rather than the transparent square, in the portrait preview.
let visual = splash.reduce(CGRect.null) { $0.union($1.path.boundingBoxOfPath) }
let scale: CGFloat = 0.95
var move = CGAffineTransform(a:scale,b:0,c:0,d:scale,tx:610-visual.midX*scale,ty:1356-visual.midY*scale)
let centered = splash.map { Shape(path:$0.path.copy(using:&move)!,fill:$0.fill) }
render("splash/splash-preview-1220x2712.png",1220,2712,1220,2712,[rect(0,0,1220,2712,0,teal)]+centered)

let specs:[(String,Int,Int,Int)] = [("round-r10",10,96,96),("round-r12",12,112,112),("round-r14",14,128,128),("round-rm",16,144,144),("round-rl",24,208,208),("pill-52",26,224,208),("pill-44",22,192,176),("circle",26,208,208)]
for (name,r,w,h) in specs {
    let radius = CGFloat(r*4)
    render("sprites/\(name).png",w,h,CGFloat(w),CGFloat(h),[rect(0,0,CGFloat(w),CGFloat(h),radius,"#ffffff")])
}
print("Đã xuất icon, splash và 8 sprite PNG bằng CoreGraphics/CoreText; font Baloo 2 ExtraBold đã thành path.")

func readPNG(_ relative: String) -> CGImage {
    let source = CGImageSourceCreateWithURL(root.appendingPathComponent(relative) as CFURL,nil)!
    return CGImageSourceCreateImageAtIndex(source,0,nil)!
}
func drawImage(_ context: CGContext, _ image: CGImage, _ target: CGRect) {
    context.saveGState()
    context.translateBy(x:target.minX,y:target.maxY)
    context.scaleBy(x:1,y:-1)
    context.draw(image,in:CGRect(x:0,y:0,width:target.width,height:target.height))
    context.restoreGState()
}
func nineSlice(_ context: CGContext, _ image: CGImage, _ border: Int, _ target: CGRect) {
    let xs = [0,border,image.width-border,image.width]
    let ys = [0,border,image.height-border,image.height]
    let dx = [target.minX,target.minX+CGFloat(border),target.maxX-CGFloat(border),target.maxX]
    let dy = [target.minY,target.minY+CGFloat(border),target.maxY-CGFloat(border),target.maxY]
    for row in 0..<3 {
        for col in 0..<3 {
            let sw = xs[col+1]-xs[col], sh = ys[row+1]-ys[row]
            if sw > 0 && sh > 0 {
                let patch = image.cropping(to:CGRect(x:xs[col],y:ys[row],width:sw,height:sh))!
                drawImage(context,patch,CGRect(x:dx[col],y:dy[row],width:dx[col+1]-dx[col],height:dy[row+1]-dy[row]))
            }
        }
    }
}
// Comparison reference reads the same CSS token; CoreGraphics rasterizes the
// circular border-radius geometry. This is explicitly NOT a browser screenshot.
let theme = String(data:FileManager.default.contents(atPath:"design/Ui/global/css/theme.css")!,encoding:.utf8)!
precondition(theme.contains("--r-m:16px"))
let reference = [rect(0,0,340,208,64,"#ffffff")]
render("sprites/.reference-temp.png",340,208,340,208,reference)
let ref = readPNG("sprites/.reference-temp.png")
let sprite = readPNG("sprites/round-rm.png")
let canvas = CGContext(data:nil,width:1600,height:1120,bitsPerComponent:8,bytesPerRow:1600*4,space:CGColorSpace(name:CGColorSpace.sRGB)!,bitmapInfo:CGImageAlphaInfo.premultipliedLast.rawValue)!
canvas.translateBy(x:0,y:1120)
canvas.scaleBy(x:1,y:-1)
func paint(_ shape: Shape) { canvas.setFillColor(color(shape.fill)); canvas.addPath(shape.path); canvas.fillPath() }
func label(_ text: String, _ x: CGFloat, _ y: CGFloat, _ w: CGFloat, _ h: CGFloat, _ fill: String = "#1f2d3d") {
    paint(lettering(text,CGRect(x:x,y:y,width:w,height:h),fill))
}
paint(rect(0,0,1600,1120,0,"#f4fbfa"))
label("CSS RADIUS / SPRITE 9-SLICE",60,40,1480,62,deep)
label("4 PX PER CSS PX  /  NO BLUR",60,132,1480,30)
label("CSS GEOMETRY",200,222,500,40,deep)
label("PNG 9-SLICE",900,222,500,40,deep)
for x:CGFloat in [150,850] { paint(rect(x,300,600,280,24,"#d8f5f1")) }
drawImage(canvas,ref,CGRect(x:280,y:336,width:340,height:208))
nineSlice(canvas,sprite,64,CGRect(x:980,y:336,width:340,height:208))
label("R16 CSS = 64 PX",60,610,1480,30)
// Crop identical upper-left corners; enlarge 4x with nearest filtering.
let slicedContext = CGContext(data:nil,width:340,height:208,bitsPerComponent:8,bytesPerRow:340*4,space:CGColorSpace(name:CGColorSpace.sRGB)!,bitmapInfo:CGImageAlphaInfo.premultipliedLast.rawValue)!
slicedContext.translateBy(x:0,y:208); slicedContext.scaleBy(x:1,y:-1)
nineSlice(slicedContext,sprite,64,CGRect(x:0,y:0,width:340,height:208))
let sliced = slicedContext.makeImage()!
canvas.interpolationQuality = .none
for (i,img) in [ref,sliced].enumerated() {
    let x = CGFloat(200+i*700)
    paint(rect(x,690,320,272,0,teal))
    let crop = img.cropping(to:CGRect(x:0,y:0,width:72,height:64))!
    drawImage(canvas,crop,CGRect(x:x+16,y:706,width:288,height:256))
}
label("CORNER CROP / 4X NEAREST",60,988,1480,30)
label("COREGRAPHICS REFERENCE / NOT A BROWSER RENDER",60,1055,1480,24)
let dest = CGImageDestinationCreateWithURL(root.appendingPathComponent("sprites/compare.png") as CFURL,UTType.png.identifier as CFString,1,nil)!
CGImageDestinationAddImage(dest,canvas.makeImage()!,nil)
precondition(CGImageDestinationFinalize(dest))
let temp = root.appendingPathComponent("sprites/.reference-temp.png")
_ = unlink(temp.path)
print("compare.png: CSS-token geometry vs actual PNG nine-slice, corner crops 4x; browser render unavailable.")
