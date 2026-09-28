#!/bin/bash
set -euo pipefail

HARDSHIP_VERSION="${1:-0.0.10}"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
DIST_DIR="$SCRIPT_DIR/dist"
PACKAGE_DIR="$DIST_DIR/tmp"
PACKAGE_PATH="$DIST_DIR/Hardship.zip"

cd "$SCRIPT_DIR"

echo "# Build Hardship $HARDSHIP_VERSION"
dotnet build "hardship/Hardship.csproj" \
  -c Release \
  -p:VersionPrefix="$HARDSHIP_VERSION"

cleanup() {
  rm -rf "$PACKAGE_DIR"
}
trap cleanup EXIT

echo "# Package $PACKAGE_PATH"
mkdir -p "$PACKAGE_DIR"
rm -f "$PACKAGE_PATH"
cp "hardship/bin/Release/net462/Hardship.dll" "$PACKAGE_DIR/"
cp hardship/thunderstore/{CHANGELOG.md,icon.png,manifest.json,README.md} "$PACKAGE_DIR/"
sed -i "s/\$HARDSHIP_VERSION/$HARDSHIP_VERSION/g" "$PACKAGE_DIR/manifest.json"
(cd "$PACKAGE_DIR" && zip -qr "$PACKAGE_PATH" .)

echo "Created $PACKAGE_PATH"