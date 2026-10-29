#!/usr/bin/env bash
# One command to look at a screen of a worktree on the real phone.
#   tools/ui-preview/preview.sh [--keep] [--name N] [--wait MIN] [--settle SEC] [--rebuild] [--no-build] <worktree> <screen>
# screen: home | selectworld | selectlevel[:w-s] | main:w-s-l | pause | win[:first|replay] | extra[:ready|notready] | howto |
#         lowruby | shop | rate | language | rewarded:<state>   (optional ;ruby=N and ;level=w-s-l suffixes) — see README.md
set -euo pipefail
HERE=$(cd "$(dirname "$0")" && pwd)
. "$HERE/lib/common.sh"

KEEP=0; WAIT=20; SETTLE=3; REBUILD=0; NOBUILD=0; NAME=""; ARGS=()
while [ $# -gt 0 ]; do
  case "$1" in
    --keep) KEEP=1 ;; --rebuild) REBUILD=1 ;; --no-build) NOBUILD=1 ;;
    --wait) WAIT=$2; shift ;; --settle) SETTLE=$2; shift ;; --name) NAME=$2; shift ;;
    -h|--help) sed -n '2,6p' "$0"; exit 0 ;;
    *) ARGS+=("$1") ;;
  esac; shift
done
[ ${#ARGS[@]} -eq 2 ] || die "usage: preview.sh [--keep] <worktree> <screen>"
WT=$(cd "${ARGS[0]}" && pwd); SPEC=${ARGS[1]}
[ -d "$WT/Assets" ] || die "not a Unity project: $WT"
resolve_main "$WT"
WTNAME=$(basename "$WT")
EXP=$WT/Export; PV=$EXP/preview; mkdir -p "$PV"
VER=$(sed -n 's/^  bundleVersion: //p' "$WT/ProjectSettings/ProjectSettings.asset" | head -1)   # CLR/AA export folders follow the game version
[ -n "$VER" ] || die "cannot read bundleVersion from $WT/ProjectSettings/ProjectSettings.asset"
HEADNAME=${SPEC%%[:;]*}; HEADNAME=$(printf '%s' "$HEADNAME" | tr 'A-Z' 'a-z')
case "$HEADNAME" in
  home|shop|rate|language|rewarded|reset|daily) SCENE=Home ;;
  selectworld) SCENE=SelectWorld ;; selectlevel) SCENE=SelectLevel ;;
  main|pause|win|extra|howto|lowruby|dailymain|dailywin) SCENE=Main ;;
  *) die "unknown screen '$SPEC'" ;;
esac
[ -n "$NAME" ] || NAME=$(printf '%s' "$SPEC" | tr ':;=' '-__' | tr -c 'A-Za-z0-9_.-' '_')
STAMP=$(date +%Y%m%d-%H%M%S)
T0=$SECONDS; T_CLR=0; T_AA=0; T_APK=0; GUARD=0; LOCKED=0

# ---- worktree guard: Unity rewrites many tracked files during a batch run; put everything back afterwards
snapshot_wt() {
  rm -rf "${PV:?}/snap"; mkdir -p "$PV/snap"
  git -C "$WT" diff --name-only HEAD -z > "$PV/dirty-before"
  while IFS= read -r -d '' f; do
    if [ -e "$WT/$f" ]; then mkdir -p "$PV/snap/$(dirname "$f")"; cp -p "$WT/$f" "$PV/snap/$f"; fi
  done < "$PV/dirty-before"
  if [ -d "$WT/Assets/StreamingAssets/clr" ]; then echo 1 > "$PV/had-streaming-clr"; else rm -f "$PV/had-streaming-clr"; fi
  GUARD=1
}
restore_wt() {
  [ "$GUARD" = 1 ] || return 0
  GUARD=0
  git -C "$WT" diff --name-only HEAD -z > "$PV/dirty-after"
  tr '\0' '\n' < "$PV/dirty-before" > "$PV/before.txt"
  while IFS= read -r -d '' f; do
    if grep -qxF -- "$f" "$PV/before.txt"; then
      if [ -e "$PV/snap/$f" ]; then cp -p "$PV/snap/$f" "$WT/$f"; else rm -f "${WT:?}/$f"; fi
    else
      git -C "$WT" checkout HEAD -- "$f"
    fi
  done < "$PV/dirty-after"
  if [ ! -f "$PV/had-streaming-clr" ]; then rm -rf "${WT:?}/Assets/StreamingAssets/clr" "${WT:?}/Assets/StreamingAssets/clr.meta"; fi
}
on_exit() {
  local rc=$?
  restore_wt
  if [ "$KEEP" = 0 ] && [ "$LOCKED" = 1 ]; then
    say "releasing: restoring the original APK and the lock"
    "$HERE/restore.sh" "$WT" >&2 || true
  fi
  exit $rc
}
trap on_exit EXIT

# ---- fingerprints: code (cs/asmdef/asmref/ProjectSettings) decides the CLR build, everything else decides Addressables
fingerprint() {
  local idx="$PV/fp.index"
  cp "$(git -C "$WT" rev-parse --path-format=absolute --git-path index)" "$idx"
  GIT_INDEX_FILE=$idx git -C "$WT" add -A -- Assets Packages ProjectSettings ':!Assets/StreamingAssets/clr' ':!Assets/StreamingAssets/clr.meta' >/dev/null 2>&1
  GIT_INDEX_FILE=$idx git -C "$WT" ls-files -s -- Assets Packages ProjectSettings > "$PV/fp.list"
  rm -f "$idx"
  FP_CODE=$(grep -E '\.(cs|asmdef|asmref)$|	ProjectSettings/' "$PV/fp.list" | shasum | cut -c1-16)
  FP_ASSET=$(grep -vE '\.(cs|asmdef|asmref)$|	ProjectSettings/' "$PV/fp.list" | shasum | cut -c1-16)
}

unity_batch() { # $1 method  $2 log  $3.. extra args
  local method=$1 log=$2; shift 2
  "$UNITY" -batchmode -nographics -quit -projectPath "$WT" -buildTarget Android -executeMethod "$method" "$@" -logFile "$log" || true
  if grep -q "another Unity instance is running" "$log"; then die "Unity Editor has this worktree open — close it first ($log)"; fi
  if grep -qE "error CS[0-9]+" "$log"; then grep -E "error CS[0-9]+" "$log" | sort -u | head -15 >&2; die "compile errors ($log)"; fi
}

build_content() {
  [ "$NOBUILD" = 1 ] && return 0
  fingerprint
  local last_code last_asset
  last_code=$(cat "$PV/fp-code" 2>/dev/null || true); last_asset=$(cat "$PV/fp-asset" 2>/dev/null || true)
  local need_clr=0 need_aa=0
  [ "$FP_CODE" != "$last_code" ] && need_clr=1
  [ "$FP_ASSET" != "$last_asset" ] && need_aa=1
  if [ "$REBUILD" = 1 ]; then need_clr=1; need_aa=1; fi
  [ -f "$PV/Upload/hotupdate.hash" ] || need_clr=1
  [ -f "$EXP/ServerData/$VER/android/catalog_$VER.bin" ] || need_aa=1
  if [ $need_clr = 0 ] && [ $need_aa = 0 ]; then say "content unchanged since the last build — skipping Unity"; return 0; fi
  snapshot_wt
  # GAME_DEBUG switches DebugOpener on in the hot-update DLL (restored afterwards)
  python3 -I - "$WT/ProjectSettings/ProjectSettings.asset" <<'PY'
import re, sys
p = sys.argv[1]; s = open(p).read()
m = re.search(r"(scriptingDefineSymbols:\n(?:    .*\n)*?    Android: )(.*)\n", s)
if not m: sys.exit("scriptingDefineSymbols for Android not found")
if "GAME_DEBUG" not in m.group(2).split(";"):
    s = s[:m.start(2)] + m.group(2) + ";GAME_DEBUG" + s[m.end(2):]
    open(p, "w").write(s)
PY
  if [ $need_clr = 1 ]; then
    local rt="$EXP/CLR/$VER/android/Runtime"
    if [ ! -f "$rt/config.json" ]; then # incremental CLR needs the AOT metadata of the installed APK: seed it from the original APK
      say "seeding CLR runtime metadata from the original APK"
      mkdir -p "$rt"; unzip -o -q "$ORIG_APK" 'assets/clr/*' -d "$PV/orig"
      cp "$PV"/orig/assets/clr/*.dll.bytes "$PV/orig/assets/clr/config.json" "$PV/orig/assets/clr/build-target.txt" "$rt/"
      rm -f "$rt/WordCross.Gameplay.HotUpdate.dll.bytes" "$rt/CatGaming.Module.dll.bytes"; rm -rf "${PV:?}/orig"
    fi
    say "building CLR (HybridCLR incremental)…"; local t=$SECONDS
    rm -rf "${EXP:?}/CLR/$VER/android/Upload"
    unity_batch Util.Editor.BuildDatabase.BuildAndroid "$LOGS/preview-$WTNAME-clr.log" -batchIncrementalClr
    T_CLR=$((SECONDS - t))
    [ -f "$EXP/CLR/$VER/android/Upload/hotupdate.hash" ] || { grep -E "\[CLR\]|Error" "$LOGS/preview-$WTNAME-clr.log" | head >&2; die "CLR export failed ($LOGS/preview-$WTNAME-clr.log)"; }
    rm -rf "${PV:?}/Upload"; cp -R "$EXP/CLR/$VER/android/Upload" "$PV/Upload"
  fi
  if [ $need_aa = 1 ]; then
    say "building Addressables (content update)…"; local t=$SECONDS
    unity_batch Util.Editor.BuildDatabase.BuildOrUpdateAddressable "$LOGS/preview-$WTNAME-aa.log"
    T_AA=$((SECONDS - t))
    grep -q "Addressables content update success\|Addressables build success" "$LOGS/preview-$WTNAME-aa.log" || { grep -E "Addressables|failed" "$LOGS/preview-$WTNAME-aa.log" | head >&2; die "Addressables build failed ($LOGS/preview-$WTNAME-aa.log)"; }
    local host
    host=$(python3 -I - "$ORIG_APK" <<'PY'
import re, sys, zipfile
t = zipfile.ZipFile(sys.argv[1]).read("assets/aa/settings.json").decode()
print(re.search(r"https://([^/\"]+)/", t).group(1))
PY
)
    python3 -I "$HERE/lib/catalog_patch.py" "$EXP/ServerData/$VER/android" "$host" "$PORT" >&2
  fi
  restore_wt
  fingerprint
  echo "$FP_CODE" > "$PV/fp-code"; echo "$FP_ASSET" > "$PV/fp-asset"
}

# ---- 1. build (no device needed, so other people can use the phone meanwhile)
build_content
[ -f "$PV/Upload/hotupdate.hash" ] || die "no CLR build yet (run without --no-build)"
EMBED_HASH=$(cat "$PV/Upload/hotupdate.hash")

# ---- 2. device lock
say "waiting for the device lock (max ${WAIT} min)…"
python3 -I "$HERE/lib/devlock.py" acquire "$STATE" "$WT" "$WAIT" "$KEEP" "$$" >&2 || die "could not get the device lock"
LOCKED=1
adb start-server >/dev/null 2>&1 || true
adb get-state >/dev/null 2>&1 || die "phone $ANDROID_SERIAL not connected"

# ---- 3. server + reverse
stop_server
nohup python3 "$HERE/lib/serve.py" "$EXP/ServerData" "$PORT" "$PV/server.log" >/dev/null 2>&1 &
echo $! > "$STATE/preview-server.pid"
adb reverse "tcp:$PORT" "tcp:$PORT" >/dev/null

# ---- 4. APK: install only when needed, otherwise push the new CLR payload into the verified cache
install_preview() {
  local t=$SECONDS
  say "repacking + installing the preview APK…"
  python3 -I "$HERE/lib/apk_repack.py" --src "$ORIG_APK" --out "$PV/preview.apk" --upload "$PV/Upload" --port "$PORT" >&2
  adb_install "$PV/preview.apk" || die "preview APK install failed"
  state_set INSTALLED "preview-$APKREV"; state_set DEVICE_CLR "$EMBED_HASH"; state_set UPDTIME "$(device_update_time)"; state_set SCOPE ""
  T_APK=$((SECONDS - t))
}
push_clr() {
  local scope=$1 dir
  dir=/sdcard/Android/data/$PKG/files/clr/$scope/$EMBED_HASH
  adb shell "mkdir -p $dir" && adb push "$PV/Upload/." "$dir/" >/dev/null
  printf '%s' "$EMBED_HASH" > "$PV/last-verified.hash"
  adb push "$PV/last-verified.hash" "/sdcard/Android/data/$PKG/files/clr/$scope/last-verified.hash" >/dev/null
  state_set DEVICE_CLR "$EMBED_HASH"
}
FAST=0; PUSHED=0
if [ "$(state_get INSTALLED)" = "preview-$APKREV" ] && [ "$(state_get UPDTIME)" = "$(device_update_time)" ] && [ -n "$(state_get SCOPE)" ]; then FAST=1; fi
if [ $FAST = 1 ]; then
  if [ "$(state_get DEVICE_CLR)" != "$EMBED_HASH" ]; then say "pushing the new CLR payload to the device cache"; push_clr "$(state_get SCOPE)"; PUSHED=1; fi
else
  install_preview
fi

# ---- 5. launch straight into the screen
launch() {
  adb shell am force-stop "$PKG"
  adb logcat -c
  adb shell "am start -W -n $PKG/$ACTIVITY --es open '$SPEC'" >/dev/null
  local n=0
  until adb logcat -d -s Unity:V | grep -q "\[DebugOpener\] opened"; do
    sleep 1; n=$((n + 1))
    if [ $n -ge 90 ]; then adb logcat -d -s Unity:V | grep -E " E Unity|DebugOpener|CLR" | tail -20 >&2; die "the app did not open '$SPEC' in 90 s"; fi
  done
  adb logcat -d -s Unity:V > "$PV/launch.log"
  SCOPE_NOW=$(sed -n 's#.*\[CLR\] Cache path: .*/clr/\([0-9a-f]\{64\}\).*#\1#p' "$PV/launch.log" | tail -1)
  if [ -n "$SCOPE_NOW" ]; then state_set SCOPE "$SCOPE_NOW"; fi
  CLR_SRC=$(grep -o 'Fallback source: [A-Za-z]* [A-Za-z]*' "$PV/launch.log" | tail -1 || true)
}
launch
if [ $PUSHED = 1 ] && [ "$CLR_SRC" != "Fallback source: verified cache" ]; then
  say "device ignored the pushed CLR cache ($CLR_SRC) — reinstalling the preview APK"
  install_preview; launch
fi

# ---- 6. wait for the scene, settle animations, screenshot
n=0
until adb logcat -d -s Unity:V | grep -q "\[Scene\] loaded $SCENE"; do
  sleep 1; n=$((n + 1)); [ $n -lt 40 ] || break
done
sleep "$SETTLE"
SHOT=$SHOTS/preview-$NAME-$STAMP.png
adb exec-out screencap -p > "$SHOT"
adb logcat -d -s Unity:V > "$LOGS/preview-$NAME-$STAMP.log"
ERRS=$(grep -c " E Unity" "$LOGS/preview-$NAME-$STAMP.log" || true)
python3 -I "$HERE/lib/devlock.py" acquire "$STATE" "$WT" 1 "$KEEP" "$$" >&2 || true   # heartbeat
say "screen '$SPEC' on $WTNAME — build clr ${T_CLR}s, aa ${T_AA}s, apk ${T_APK}s, total $((SECONDS - T0))s; CLR ${EMBED_HASH:0:8}… via ${CLR_SRC:-n/a}; Unity error lines: $ERRS"
if [ "$KEEP" = 1 ]; then say "--keep: device lock + preview APK + server stay up. Finish with: tools/ui-preview/restore.sh $WT"; fi
echo "$SHOT"
