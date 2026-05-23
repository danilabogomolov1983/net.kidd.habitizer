#!/usr/bin/env sh
set -eu

if [ $# -lt 1 ] || [ -z "${1}" ]; then
  echo "Missing required parameter: VERSION"
  echo "Usage: $0 <VERSION>"
  exit 1
fi

VERSION="${1}"
SCRIPT_DIR="$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)"
REPO_DIR="$(CDPATH= cd -- "${SCRIPT_DIR}/.." && pwd)"

SYNCDAEMON_IMAGE_TAG="wst-tools-posibridge-syncdaemon:${VERSION}"

docker build \
  -t "${SYNCDAEMON_IMAGE_TAG}" \
  -f "${SCRIPT_DIR}/apps/Dockerfile.syncdaemon" \
  "${REPO_DIR}"
