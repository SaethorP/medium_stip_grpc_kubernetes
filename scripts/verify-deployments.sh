#!/usr/bin/env bash
set -euo pipefail

# Both variants run in the single debit-card-api namespace; the VARIANT column
# shows which is regular and which is stip.
kubectl get pods -n debit-card-api -L debit-card-api/variant
kubectl get deployment -n debit-card-api

# Which variant the router Service currently sends traffic to.
echo -n "active variant: "
kubectl get svc debit-card-api -n debit-card-api -o jsonpath='{.spec.selector.debit-card-api/variant}'
echo
