#!/usr/bin/env python3
"""Tiny static server for the preview Addressables export.
Maps /wordcross/data/<ver>/android/<file> (the URL baked in the catalog) onto <root>/<ver>/android/<file>.
Usage: serve.py <root> <port> <log>"""
import functools, http.server, os, sys

PREFIX = "/wordcross/data"


class Handler(http.server.SimpleHTTPRequestHandler):
    def translate_path(self, path):
        if path.startswith(PREFIX):
            path = path[len(PREFIX):]
        return super().translate_path(path)

    def end_headers(self):
        self.send_header("Cache-Control", "no-store")
        super().end_headers()

    def log_message(self, fmt, *args):
        with open(LOG, "a") as handle:
            handle.write("%s %s\n" % (self.log_date_time_string(), fmt % args))


if __name__ == "__main__":
    root, port, LOG = sys.argv[1], int(sys.argv[2]), sys.argv[3]
    handler = functools.partial(Handler, directory=root)
    http.server.ThreadingHTTPServer(("127.0.0.1", port), handler).serve_forever()
