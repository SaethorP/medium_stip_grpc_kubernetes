# Repoints the router Service to the given variant without restarting pods.
# PowerShell equivalent of switch.sh.  Usage: .\scripts\switch.ps1 stip

param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("regular", "stip")]
    [string]$Variant
)

$ErrorActionPreference = "Stop"
$RepoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)

helm upgrade debit-card-api (Join-Path $RepoRoot "charts\debit-card-api") `
    --namespace debit-card-api `
    --reuse-values `
    --set activeVariant=$Variant

Write-Host "Active variant is now: $Variant"
