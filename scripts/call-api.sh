#!/usr/bin/env bash
set -euo pipefail

# Calls the ingress DNS host (debit-card-api.local) over gRPC and logs the response,
# including which variant (regular/stip) served the call.
#
# Defaults dial localhost:443 (the Docker Desktop load balancer), so it works out of the
# box without a hosts-file entry. Once debit-card-api.local is in your hosts file you can
# run:  RESOLVE_IP="" ./scripts/call-api.sh   to exercise real DNS resolution.

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/.." && pwd)"

export API_HOST="${API_HOST:-debit-card-api.local}"
export API_PORT="${API_PORT:-443}"

dotnet run --project "${REPO_ROOT}/tools/grpc-client" --verbosity quiet