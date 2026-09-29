#!/bin/bash
set -euo pipefail

script_dir="$(cd "$(dirname "$0")" && pwd)"
package_dir="$(cd "$script_dir/.." && pwd)"
bundle="$package_dir/Runtime/Plugins/MacOS/SheepIsleMacBridge.bundle"
executable="$bundle/Contents/MacOS/SheepIsleMacBridge"

mkdir -p "$bundle/Contents/MacOS"
cp "$script_dir/Info.plist" "$bundle/Contents/Info.plist"

xcrun --sdk macosx clang++ \
  -fobjc-arc \
  -std=c++17 \
  -arch arm64 \
  -mmacosx-version-min=12.0 \
  -framework AppKit \
  -bundle \
  -fvisibility=hidden \
  "$script_dir/SheepIsleMacBridge.mm" \
  -o "$executable"

codesign --force --sign - --timestamp=none "$bundle"
echo "Built $bundle"
