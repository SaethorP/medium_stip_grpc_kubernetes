#!/usr/bin/env bash
set -euo pipefail

IMAGE_NAME="${IMAGE_NAME:-debit-card-api}"
IMAGE_TAG="${IMAGE_TAG:-local}"

minikube image load "${IMAGE_NAME}:${IMAGE_TAG}"
