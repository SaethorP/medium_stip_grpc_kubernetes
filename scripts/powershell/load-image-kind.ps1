# Loads the API image into a kind cluster. PowerShell equivalent of load-image-kind.sh.

$ErrorActionPreference = "Stop"
$RepoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)

$ImageName = if ($env:IMAGE_NAME) { $env:IMAGE_NAME } else { "debit-card-api" }
if (-not $env:IMAGE_TAG) { $env:IMAGE_TAG = (git -C $RepoRoot rev-parse --short HEAD) }

kind load docker-image "${ImageName}:$env:IMAGE_TAG"
