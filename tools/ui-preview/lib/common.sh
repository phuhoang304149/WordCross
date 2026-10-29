# shared helpers for preview.sh / restore.sh (sourced)
UNITY=${UNITY_PATH:-/Applications/Unity/Hub/Editor/6000.3.9f1/Unity.app/Contents/MacOS/Unity}
export ANDROID_SERIAL=${ANDROID_SERIAL:-R58MC2CSLQZ}
PKG=com.PHltd.wordcrossy
ACTIVITY=com.unity3d.player.UnityPlayerActivity
PORT=${PREVIEW_PORT:-18080}
APKREV=1   # bump when apk_repack.py/catalog_patch.py change the repacked APK layout

say() { printf '[ui-preview] %s\n' "$*" >&2; }
die() { say "ERROR: $*"; exit 1; }

# adb install with retries (other Unity batch runs kill the shared adb server when they exit)
adb_install() {
  local try=0
  until adb install -r "$1" >/dev/null 2>&1; do
    try=$((try + 1)); [ $try -lt 4 ] || return 1
    sleep 2; adb start-server >/dev/null 2>&1 || true
  done
}

# $1 = a worktree path; sets COMMON (git common dir), MAIN (main checkout), LOGS, SHOTS, STATE, ORIG_APK
resolve_main() {
  COMMON=$(git -C "$1" rev-parse --path-format=absolute --git-common-dir)
  MAIN=$(dirname "$COMMON")
  LOGS=$MAIN/Build/remote-r2-0.3
  SHOTS=$LOGS/qa-shots
  STATE=$COMMON/task-hotupdate
  ORIG_APK=${WC_ORIGINAL_APK:-$LOGS/wordcrossy_0.3_6_202610291042_DEVELOPER_LOG.apk}
  mkdir -p "$STATE" "$SHOTS"
}

state_get() { [ -f "$STATE/device-state.env" ] && sed -n "s/^$1=//p" "$STATE/device-state.env" | tail -1 || true; }
state_set() {
  touch "$STATE/device-state.env"
  grep -v "^$1=" "$STATE/device-state.env" > "$STATE/device-state.env.tmp" || true
  printf '%s=%s\n' "$1" "$2" >> "$STATE/device-state.env.tmp"
  mv "$STATE/device-state.env.tmp" "$STATE/device-state.env"
}

device_update_time() { adb shell dumpsys package "$PKG" | sed -n 's/^ *lastUpdateTime=//p' | head -1 | tr -d '\r'; }

stop_server() {
  [ -f "$STATE/preview-server.pid" ] || return 0
  kill "$(cat "$STATE/preview-server.pid")" 2>/dev/null || true
  rm -f "$STATE/preview-server.pid"
}

install_original() {
  [ -f "$ORIG_APK" ] || die "original APK not found: $ORIG_APK"
  local sum; sum=$(shasum -a 256 "$ORIG_APK" | cut -c1-8)
  [ "$sum" = dcaa9fed ] || say "WARNING: original APK sha256 starts $sum (expected dcaa9fed)"
  # other Unity batch runs on this Mac kill the shared adb server on exit, so retry
  adb_install "$ORIG_APK" || die "cannot install the original APK"
  state_set INSTALLED original
  state_set UPDTIME "$(device_update_time)"
  state_set SCOPE ""
}
