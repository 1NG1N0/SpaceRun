#!/bin/sh
echo -ne '\033c\033]0;Space Run\a'
base_path="$(dirname "$(realpath "$0")")"
"$base_path/SpaceRunV0.001.x86_64" "$@"
