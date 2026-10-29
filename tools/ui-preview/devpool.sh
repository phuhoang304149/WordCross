#!/usr/bin/env bash
# Device pool shared by all worktrees. Registry: ~/Documents/android.rtf (human readable), state in ~/Documents/android-devices.state
# usage: devpool.sh refresh            add connected devices (status finish), mark vanished ones offline
#        devpool.sh claim <owner>      print the serial of a free (finish) device and mark it working; exit 1 if none free
#        devpool.sh release <serial>   mark finish
#        devpool.sh list
set -euo pipefail
DIR=$HOME/Documents; STATE=$DIR/android-devices.state; RTF=$DIR/android.rtf; GUARD=$DIR/.android-devices.lock
touch "$STATE"
until mkdir "$GUARD" 2>/dev/null; do
  [ -n "$(find "$GUARD" -mmin +2 2>/dev/null)" ] && rmdir "$GUARD" 2>/dev/null || true
  sleep 0.2
done
trap 'rmdir "$GUARD"' EXIT
render() {
  { printf '{\\rtf1\\ansi\\deff0{\\fonttbl{\\f0 Menlo;}}\\f0\\fs22\n'
    printf 'Android devices (updated %s)\\line\n' "$(date '+%F %T')"
    printf 'serial | model | status | owner\\line\n'
    while IFS='|' read -r s m st o; do printf '%s | %s | %s | %s\\line\n' "$s" "$m" "$st" "$o"; done < "$STATE"
    printf '}\n'; } > "$RTF"
}
set_status() { # serial status owner
  grep -v "^$1|" "$STATE" > "$STATE.tmp" || true
  m=$(grep "^$1|" "$STATE" | cut -d'|' -f2 || true)
  printf '%s|%s|%s|%s\n' "$1" "${m:-?}" "$2" "$3" >> "$STATE.tmp"; mv "$STATE.tmp" "$STATE"
}
case "${1:-list}" in
  refresh)
    live=$(adb devices -l | awk 'NR>1 && $2=="device"{print $1"|"$0}')
    while IFS='|' read -r s rest; do
      [ -n "$s" ] || continue
      if grep -q "^$s|" "$STATE"; then
        grep -q "^$s|[^|]*|offline|" "$STATE" && set_status "$s" finish ""
        continue
      fi
      m=$(printf '%s' "$rest" | sed -n 's/.*model:\([^ ]*\).*/\1/p')
      printf '%s|%s|finish|\n' "$s" "$m" >> "$STATE"
    done <<< "$live"
    while IFS='|' read -r s m st o; do
      printf '%s\n' "$live" | grep -q "^$s|" || [ "$st" = offline ] || set_status "$s" offline ""
    done < "$STATE"
    render ;;
  claim)
    s=$(awk -F'|' '$3=="finish"{print $1; exit}' "$STATE")
    [ -n "$s" ] || exit 1
    set_status "$s" working "${2:-unknown}"; render; echo "$s" ;;
  release) set_status "$2" finish ""; render ;;
  list) cat "$STATE" ;;
esac
