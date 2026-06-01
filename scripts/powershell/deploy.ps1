# Deploys both variants into the debit-card-api namespace and wires the router
# Service to ACTIVE_VARIANT. PowerShell equivalent of deploy.sh.
#
# Defaults IMAGE_TAG to the current git commit (set $env:IMAGE_TAG to override) and
# ACTIVE_VARIANT to "regular" (set $env:ACTIVE_VARIANT to start on stip).

$ErrorActionPreference = "Stop"
$RepoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)

if (-not $env:IMAGE_TAG) { $env:IMAGE_TAG = (git -C $RepoRoot rev-parse --short HEAD) }
$ActiveVariant = if ($env:ACTIVE_VARIANT) { $env:ACTIVE_VARIANT } else { "regular" }

helm upgrade --install debit-card-api (Join-Path $RepoRoot "charts\debit-card-api") `
    --namespace debit-card-api `
    --create-namespace `
    --set image.tag=$env:IMAGE_TAG `
    --set activeVariant=$ActiveVariant
