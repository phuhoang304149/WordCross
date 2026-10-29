#!/usr/bin/env python3
"""Make the Addressables remote catalog (binary) load bundles from http://127.0.0.1:<port> instead of the production host.

The binary catalog stores a bundle URL as a chain of length-prefixed parts ("https:/" <- host <- path ...). Offsets stay valid
when a part keeps its position: we only shrink the scheme part (https:/ -> http:/) and the host part (-> 127.0.0.1:<port>), padding
the rest. The .hash file is rewritten (it only has to differ from the cached one).
Usage: catalog_patch.py <android export dir> <host-to-replace> <port>
"""
import glob, hashlib, os, struct, sys


def main():
    folder, host, port = sys.argv[1], sys.argv[2], int(sys.argv[3])
    new_host = "127.0.0.1:%d" % port
    for catalog in glob.glob(os.path.join(folder, "catalog_*.bin")):
        data = bytearray(open(catalog, "rb").read())
        scheme_old = struct.pack("<I", 7) + b"https:/"
        scheme_new = struct.pack("<I", 6) + b"http:/" + b"/"
        host_old = struct.pack("<I", len(host)) + host.encode()
        host_new = struct.pack("<I", len(new_host)) + new_host.encode() + b"\0" * (len(host) - len(new_host))
        if len(new_host) > len(host) or data.count(scheme_old) != 1 or data.count(host_old) != 1:
            sys.exit("unexpected catalog layout (scheme x%d, host x%d)" % (data.count(scheme_old), data.count(host_old)))
        data = data.replace(scheme_old, scheme_new).replace(host_old, host_new)
        open(catalog, "wb").write(data)
        hash_path = catalog[:-4] + ".hash"
        open(hash_path, "w").write(hashlib.md5(bytes(data)).hexdigest())
        print("patched", os.path.basename(catalog), "->", new_host)


if __name__ == "__main__":
    main()
