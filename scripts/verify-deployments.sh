#!/usr/bin/env bash
set -euo pipefail

kubectl get pods -n debit-card-api-regular
kubectl get pods -n debit-card-api-stip
kubectl get deployment -n debit-card-api-regular
kubectl get deployment -n debit-card-api-stip
