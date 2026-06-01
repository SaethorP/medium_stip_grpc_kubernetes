# Calls the ingress DNS host (debit-card-api.local) over gRPC and logs the response,
# including which variant (regular/stip) served the call. PowerShell equivalent of call-api.sh.
#
# Defaults dial localhost:443 (the Docker Desktop load balancer), so it works out of the
# box without a hosts-file entry. Once debit-card-api.local is in your hosts file you can run:
#   $env:RESOLVE_IP = ""; .\scripts\call-api.ps1   to exercise real DNS resolution.

$ErrorActionPreference = "Stop"
$RepoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)

if (-not $env:API_HOST) { $env:API_HOST = "debit-card-api.local" }
if (-not $env:API_PORT) { $env:API_PORT = "443" }

dotnet run --project (Join-Path $RepoRoot "tools\grpc-client") --verbosity quiet