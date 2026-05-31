#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/.." && pwd)"
# Same default as build-image.sh: the current git commit (fallback "local").
IMAGE_TAG="${IMAGE_TAG:-$(git -C "${REPO_ROOT}" rev-parse --short HEAD 2>/dev/null || echo local)}"
ACTIVE_VARIANT="${ACTIVE_VARIANT:-regular}"

# Deploys both variants (regular + stip) into one namespace and wires the router
# Service to ACTIVE_VARIANT. Use scripts/switch.sh to flip the active variant later.
helm upgrade --install debit-card-api "${REPO_ROOT}/charts/debit-card-api" \
  --namespace debit-card-api \
  --create-namespace \
  --set image.tag="${IMAGE_TAG}" \
  --set activeVariant="${ACTIVE_VARIANT}"