# Builds the API Docker image, tagged with the current git commit.
# PowerShell equivalent of build-image.sh. Override with $env:IMAGE_TAG / $env:IMAGE_NAME.

$ErrorActionPreference = "Stop"
$RepoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)

$ImageName = if ($env:IMAGE_NAME) { $env:IMAGE_NAME } else { "debit-card-api" }
if (-not $env:IMAGE_TAG) { $env:IMAGE_TAG = (git -C $RepoRoot rev-parse --short HEAD) }

docker build -t "${ImageName}:$env:IMAGE_TAG" $RepoRoot
Write-Host "Built ${ImageName}:$env:IMAGE_TAG"
