# Loads the API image into a minikube cluster. PowerShell equivalent of load-image-minikube.sh.

$ErrorActionPreference = "Stop"
$RepoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)

$ImageName = if ($env:IMAGE_NAME) { $env:IMAGE_NAME } else { "debit-card-api" }
if (-not $env:IMAGE_TAG) { $env:IMAGE_TAG = (git -C $RepoRoot rev-parse --short HEAD) }

minikube image load "${ImageName}:$env:IMAGE_TAG"
