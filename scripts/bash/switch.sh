#!/usr/bin/env bash
set -euo pipefail

VARIANT="${1:?usage: switch.sh <regular|stip>}"

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/../.." && pwd)"

# Repoints the router Service selector to ${VARIANT} without touching the running pods.
# Both variants stay up; only the traffic target changes.
helm upgrade debit-card-api "${REPO_ROOT}/charts/debit-card-api" \
  --namespace debit-card-api \
  --reuse-values \
  --set activeVariant="${VARIANT}"

echo "Active variant is now: ${VARIANT}"