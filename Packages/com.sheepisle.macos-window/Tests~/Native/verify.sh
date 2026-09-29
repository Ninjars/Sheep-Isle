#!/bin/bash
set -euo pipefail

script_dir="$(cd "$(dirname "$0")" && pwd)"
package_dir="$(cd "$script_dir/../.." && pwd)"
build_dir="$script_dir/.build"
bundle="$package_dir/Runtime/Plugins/MacOS/SheepIsleMacBridge.bundle"
binary="$bundle/Contents/MacOS/SheepIsleMacBridge"

mkdir -p "$build_dir"

xcrun --sdk macosx clang++ \
  -fobjc-arc \
  -std=c++17 \
  -arch arm64 \
  -mmacosx-version-min=12.0 \
  -framework AppKit \
  "$script_dir/SheepIsleMacBridgeHost.mm" \
  -o "$build_dir/SheepIsleMacBridgeHost"

if [[ ! -d "$bundle" ]]; then
  echo "Missing bridge bundle: $bundle" >&2
  exit 20
fi

"$build_dir/SheepIsleMacBridgeHost" "$binary"

file "$binary"
nm -gj "$binary" | sort
codesign --verify --deep --strict --verbose=2 "$bundle"
