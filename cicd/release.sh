#!/usr/bin/env sh
set -eu

if [ $# -lt 1 ] || [ -z "${1}" ]; then
  echo "Missing required parameter: TARGET_ENV"
  echo "Usage: $0 <TARGET_ENV> <VERSION>"
  exit 1
fi

if [ $# -lt 2 ] || [ -z "${2}" ]; then
  echo "Missing required parameter: VERSION"
  echo "Usage: $0 <TARGET_ENV> <VERSION>"
  exit 1
fi

TARGET_ENV="${1}"
VERSION="${2}"
SCRIPT_DIR="$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)"

SYNCDAEMON_IMAGE_TAG="wst-tools-posibridge-syncdaemon:${VERSION}"
RELEASE_DIR="${SCRIPT_DIR}/release/${TARGET_ENV}"
SYNCDAEMON_TAR_FILE="${RELEASE_DIR}/wst-tools-posibridge-syncdaemon-${VERSION}.tar"

mkdir -p "${RELEASE_DIR}"

docker save -o "${SYNCDAEMON_TAR_FILE}" "${SYNCDAEMON_IMAGE_TAG}"

cp "${SCRIPT_DIR}/apps/docker-compose.yml" "${RELEASE_DIR}/docker-compose.yml"
cp "${SCRIPT_DIR}/run.sh" "${RELEASE_DIR}/run.sh"
chmod +x "${RELEASE_DIR}/run.sh"

if [ -f "${SCRIPT_DIR}/apps/.env.${TARGET_ENV}" ]; then
  cp "${SCRIPT_DIR}/apps/.env.${TARGET_ENV}" "${RELEASE_DIR}/.env"
fi
