#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/.." && pwd)"
IMAGE_TAG="${IMAGE_TAG:-local}"

helm upgrade --install debit-card-api-regular "${REPO_ROOT}/charts/debit-card-api" \
  --namespace debit-card-api-regular \
  --create-namespace \
  --set image.tag="${IMAGE_TAG}" \
  -f "${REPO_ROOT}/charts/debit-card-api/values-regular.yaml"
