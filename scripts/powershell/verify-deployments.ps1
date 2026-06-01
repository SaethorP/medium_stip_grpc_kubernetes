# Lists both variants' pods/deployments and the active variant.
# PowerShell equivalent of verify-deployments.sh.

$ErrorActionPreference = "Stop"

kubectl get pods -n debit-card-api -L debit-card-api/variant
kubectl get deployment -n debit-card-api

Write-Host -NoNewline "active variant: "
kubectl get svc debit-card-api -n debit-card-api -o jsonpath='{.spec.selector.debit-card-api/variant}'
Write-Host ""
