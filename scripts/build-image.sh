#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/.." && pwd)"

IMAGE_NAME="${IMAGE_NAME:-debit-card-api}"
IMAGE_TAG="${IMAGE_TAG:-local}"

docker build -t "${IMAGE_NAME}:${IMAGE_TAG}" "${REPO_ROOT}"
