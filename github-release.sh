#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
DIST_DIR="$SCRIPT_DIR/dist"
PACKAGE_PATH="$DIST_DIR/Hardship.zip"

if ! command -v gh >/dev/null 2>&1; then
    echo "Error: GitHub CLI (gh) is required." >&2
    exit 1
fi

if [[ ! -f "$PACKAGE_PATH" ]]; then
    echo "Error: package not found: $PACKAGE_PATH" >&2
    echo "Build it first with ./release.sh VERSION." >&2
    exit 1
fi

if ! command -v unzip >/dev/null 2>&1; then
    echo "Error: unzip is required to read the package manifest." >&2
    exit 1
fi

MANIFEST="$(unzip -p "$PACKAGE_PATH" manifest.json 2>/dev/null || true)"
VERSION="$(printf '%s\n' "$MANIFEST" | sed -n 's/.*"version_number"[[:space:]]*:[[:space:]]*"\([^"]*\)".*/\1/p')"

if [[ -z "$VERSION" || "$VERSION" == '$HARDSHIP_VERSION' ]]; then
    echo "Error: could not read a concrete version from manifest.json in $PACKAGE_PATH." >&2
    exit 1
fi

if [[ ! "$VERSION" =~ ^[0-9]+\.[0-9]+\.[0-9]+([.-][0-9A-Za-z.-]+)?$ ]]; then
    echo "Error: invalid package version '$VERSION'." >&2
    exit 1
fi

REPO="$(git -c "safe.directory=$SCRIPT_DIR" -C "$SCRIPT_DIR" remote get-url origin 2>/dev/null || true)"
if [[ -z "$REPO" ]]; then
    echo "Error: could not determine the GitHub repository from origin." >&2
    exit 1
fi

case "$REPO" in
    https://github.com/*/*.git) REPO="${REPO#https://github.com/}"; REPO="${REPO%.git}" ;;
    git@github.com:*) REPO="${REPO#git@github.com:}"; REPO="${REPO%.git}" ;;
    *) echo "Error: unsupported GitHub remote URL: $REPO" >&2; exit 1 ;;
esac

TAG="v$VERSION"
echo "Creating GitHub release $TAG for $REPO using $(basename "$PACKAGE_PATH")"
gh release create "$TAG" "$PACKAGE_PATH" \
    --repo "$REPO" \
    --title "Hardship $VERSION" \
    --generate-notes
