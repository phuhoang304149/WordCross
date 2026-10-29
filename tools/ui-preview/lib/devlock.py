#!/usr/bin/env python3
"""Single-device lock shared by every worktree (the lab has one phone).

State: <dir>/device.lock.json {owner, pid, keep, since, beat}. Mutations are serialized with flock on <dir>/device.lock.guard.
- normal holder: valid while its pid is alive
- --keep holder: pid may exit; valid until `beat` is older than KEEP_TTL (renewed by every preview.sh run)
usage: devlock.py acquire DIR OWNER WAIT_MIN KEEP PID | release DIR OWNER [force] | status DIR
"""
import fcntl, json, os, sys, time

KEEP_TTL = 30 * 60


def alive(pid):
    try:
        os.kill(pid, 0)
        return True
    except OSError:
        return False


def valid(state):
    if not state:
        return False
    if state.get("keep"):
        return time.time() - state.get("beat", 0) < KEEP_TTL
    return alive(state.get("pid", 0))


def describe(state):
    since = time.strftime("%H:%M:%S", time.localtime(state["since"]))
    return "%s (pid %s, từ %s%s)" % (state["owner"], state["pid"], since, ", --keep" if state.get("keep") else "")


class Lock:
    def __init__(self, folder):
        self.path = os.path.join(folder, "device.lock.json")
        self.guard = open(os.path.join(folder, "device.lock.guard"), "a+")

    def __enter__(self):
        fcntl.flock(self.guard, fcntl.LOCK_EX)
        return self

    def __exit__(self, *exc):
        fcntl.flock(self.guard, fcntl.LOCK_UN)

    def read(self):
        if not os.path.exists(self.path):
            return None
        with open(self.path) as handle:
            return json.load(handle)

    def write(self, state):
        if state is None:
            if os.path.exists(self.path):
                os.remove(self.path)
            return
        with open(self.path, "w") as handle:
            json.dump(state, handle)


def main():
    command, folder = sys.argv[1], sys.argv[2]
    os.makedirs(folder, exist_ok=True)
    lock = Lock(folder)
    if command == "acquire":
        owner, wait, keep, pid = sys.argv[3], float(sys.argv[4]) * 60, sys.argv[5] == "1", int(sys.argv[6])
        deadline, told = time.time() + wait, 0
        while True:
            with lock:
                state = lock.read()
                if not valid(state) or state["owner"] == owner:
                    since = state["since"] if state and state["owner"] == owner else time.time()
                    lock.write({"owner": owner, "pid": pid, "keep": keep, "since": since, "beat": time.time()})
                    return 0
                holder = describe(state)
            if time.time() > deadline:
                print("device busy, giving up. Holder: " + holder, file=sys.stderr)
                return 3
            if time.time() - told > 30:
                print("device busy, waiting... holder: " + holder, file=sys.stderr)
                told = time.time()
            time.sleep(3)
    if command == "release":
        owner = sys.argv[3]
        with lock:
            state = lock.read()
            if state and (state["owner"] == owner or (len(sys.argv) > 4 and sys.argv[4] == "force")):
                lock.write(None)
        return 0
    if command == "status":
        with lock:
            state = lock.read()
        print(("held by " + describe(state)) if valid(state) else "free")
        return 0
    return 2


if __name__ == "__main__":
    sys.exit(main())
