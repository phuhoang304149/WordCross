#!/usr/bin/env python3
"""Repack the original WordCross APK into a preview APK.

- assets/clr: embed the freshly built hot-update DLLs + hotupdate.hash (the player falls back to them when the CLR URL is unusable)
- ClrConfiguration URL baked in sharedassets: replaced by an unreachable https URL of the SAME length so the player never
  downloads the production CLR and uses the embedded one
- assets/aa/settings.json: remote catalog URL -> http://127.0.0.1:<port>
Then zipalign + apksigner (debug key, same signer as the original APK).
Usage: apk_repack.py --src ORIG.apk --out OUT.apk --upload DIR --port 18080 [--print-scope]
"""
import argparse, glob, hashlib, json, os, re, subprocess, sys, zipfile

CLR_ASSEMBLIES = ("CatGaming.Module", "WordCross.Gameplay.HotUpdate")


def build_tool(name):
    root = os.path.expanduser(os.environ.get("ANDROID_SDK_ROOT", "~/Library/Android/sdk"))
    found = sorted(glob.glob(os.path.join(root, "build-tools", "*", name)))
    if not found:
        sys.exit("missing Android build-tool: " + name)
    return found[-1]


def patch_equal_length(data, old, new):
    # Serialized Unity strings are length-prefixed: a same-length replacement keeps every offset valid.
    new = (new + "_" * len(old))[:len(old)].encode()
    old = old.encode()
    if len(new) != len(old):
        sys.exit("internal error: length mismatch")
    return data.replace(old, new), data.count(old)


def patch_catalog_text(settings_text, port):
    match = re.search(r"https://([^/\"]+)(/[^\"]*?)/catalog_[^\"/]*\.(?:hash|bin|json)", settings_text)
    if not match:
        sys.exit("remote catalog URL not found in settings.json")
    host, path = match.group(1), match.group(2)
    text = settings_text.replace("https://" + host + path, "http://127.0.0.1:%d%s" % (port, path))
    return text, host, path


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--src", required=True)
    ap.add_argument("--out", required=True)
    ap.add_argument("--upload", required=True)
    ap.add_argument("--port", type=int, default=18080)
    ap.add_argument("--unsigned-only", action="store_true")
    args = ap.parse_args()

    replacements = {}
    for name in CLR_ASSEMBLIES:
        with open(os.path.join(args.upload, name + ".dll.bytes"), "rb") as handle:
            replacements["assets/clr/%s.dll.bytes" % name] = handle.read()
    with open(os.path.join(args.upload, "hotupdate.hash"), "rb") as handle:
        replacements["assets/clr/hotupdate.hash"] = handle.read()

    tmp = args.out + ".unaligned"
    with zipfile.ZipFile(args.src) as zin, zipfile.ZipFile(tmp, "w") as zout:
        for info in zin.infolist():
            if info.filename.startswith("META-INF/"):
                continue
            data = zin.read(info.filename)
            if info.filename in replacements:
                data = replacements[info.filename]
            elif info.filename == "assets/aa/settings.json":
                text, host, path = patch_catalog_text(data.decode("utf-8"), args.port)
                data = text.encode("utf-8")
            elif info.filename == "assets/clr/config.json":
                data = re.sub(rb"https://[^\"]+", b"https://127.0.0.1:9/preview-embedded", data)
            elif re.match(r"assets/bin/Data/sharedassets\d+\.assets\.split\d+$", info.filename):
                cfg = json.loads(zin.read("assets/clr/config.json"))
                old = cfg["hotUpdateBaseUrl"]
                new = "https://127.0.0.1:9/preview-embedded/0.3"
                data, count = patch_equal_length(data, old, new)
                if count:
                    print("patched ClrConfiguration URL x%d in %s" % (count, info.filename))
            zout.writestr(info, data, compress_type=info.compress_type)
    out_aligned = args.out + ".aligned"
    subprocess.check_call([build_tool("zipalign"), "-f", "-p", "4", tmp, out_aligned])
    os.remove(tmp)
    subprocess.check_call([build_tool("apksigner"), "sign", "--ks", os.path.expanduser("~/.android/debug.keystore"),
                           "--ks-pass", "pass:android", "--key-pass", "pass:android", "--out", args.out, out_aligned])
    os.remove(out_aligned)
    if os.path.exists(args.out + ".idsig"):
        os.remove(args.out + ".idsig")
    print("preview apk:", args.out)


if __name__ == "__main__":
    main()
