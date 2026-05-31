#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/.." && pwd)"

IMAGE_NAME="${IMAGE_NAME:-debit-card-api}"
# Default to the current git commit so each rebuild gets a unique tag and
# Kubernetes always picks up the change. Falls back to "local" outside a repo.
IMAGE_TAG="${IMAGE_TAG:-$(git -C "${REPO_ROOT}" rev-parse --short HEAD 2>/dev/null || echo local)}"

docker build -t "${IMAGE_NAME}:${IMAGE_TAG}" "${REPO_ROOT}"
echo "Built ${IMAGE_NAME}:${IMAGE_TAG}"
