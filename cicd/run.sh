#!/usr/bin/env sh
set -eu

SCRIPT_DIR="$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)"
if [ $# -lt 1 ] || [ -z "${1}" ]; then
  echo "Missing required parameter: VERSION"
  echo "Usage: $0 <VERSION>"
  exit 1
fi

VERSION="${1}"
SYNCDAEMON_IMAGE_TAG="wst-tools-posibridge-syncdaemon:${VERSION}"
SYNCDAEMON_TAR_FILE="${SCRIPT_DIR}/wst-tools-posibridge-syncdaemon-${VERSION}.tar"
COMPOSE_FILE="${SCRIPT_DIR}/docker-compose.yml"
ENV_FILE="${SCRIPT_DIR}/.env"

if [ ! -f "${SYNCDAEMON_TAR_FILE}" ]; then
  echo "Tar file not found: ${SYNCDAEMON_TAR_FILE}"
  exit 1
fi

if [ ! -f "${COMPOSE_FILE}" ]; then
  echo "Compose file not found: ${COMPOSE_FILE}"
  exit 1
fi

echo "Loading image from ${SYNCDAEMON_TAR_FILE}..."
docker load -i "${SYNCDAEMON_TAR_FILE}"

echo "Recreating docker compose stack from ${COMPOSE_FILE}..."
if [ -f "${ENV_FILE}" ]; then
  SYNCDAEMON_IMAGE_TAG="${SYNCDAEMON_IMAGE_TAG}" IMAGE_VERSION="${VERSION}" docker compose -f "${COMPOSE_FILE}" --env-file "${ENV_FILE}" down
  SYNCDAEMON_IMAGE_TAG="${SYNCDAEMON_IMAGE_TAG}" IMAGE_VERSION="${VERSION}" docker compose -f "${COMPOSE_FILE}" --env-file "${ENV_FILE}" up -d
else
  SYNCDAEMON_IMAGE_TAG="${SYNCDAEMON_IMAGE_TAG}" IMAGE_VERSION="${VERSION}" docker compose -f "${COMPOSE_FILE}" down
  SYNCDAEMON_IMAGE_TAG="${SYNCDAEMON_IMAGE_TAG}" IMAGE_VERSION="${VERSION}" docker compose -f "${COMPOSE_FILE}" up -d
fi
