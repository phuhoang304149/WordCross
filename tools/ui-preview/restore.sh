#!/usr/bin/env bash
# Put the phone back to the original APK, stop the preview server and release the device lock.
# usage: restore.sh [<worktree>] [--force]   (--force: also release a lock held by someone else)
set -euo pipefail
HERE=$(cd "$(dirname "$0")" && pwd)
. "$HERE/lib/common.sh"
WT=""; FORCE=0
for arg in "$@"; do case "$arg" in --force) FORCE=1 ;; *) WT=$(cd "$arg" && pwd) ;; esac; done
resolve_main "${WT:-$HERE}"
OWNER=${WT:-none}
status=$(python3 -I "$HERE/lib/devlock.py" status "$STATE")
if [ "$status" != free ] && [ "$FORCE" = 0 ] && [[ "$status" != *"$OWNER"* ]]; then
  die "device lock is $status — pass your worktree path (owner) or --force"
fi
install_original
stop_server
adb reverse --remove "tcp:$PORT" >/dev/null 2>&1 || true
adb shell am force-stop "$PKG" || true
python3 -I "$HERE/lib/devlock.py" release "$STATE" "$OWNER" force
say "original APK restored (sha256 $(shasum -a 256 "$ORIG_APK" | cut -c1-8)…), server stopped, lock released"
