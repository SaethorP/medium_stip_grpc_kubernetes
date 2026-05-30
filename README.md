# medium_stip_grpc_kubernetes
This is a repo for the sample code and README for a medium post I am making. 

## Projects

- `DebitCardApi` is the gRPC API.
- `DebitCardApi.Tests` contains xUnit tests for the API.
- `charts/debit-card-api` contains the Helm chart.

## Run Locally

The API listens on HTTPS port `50051` with HTTP/2.

Run the regular implementation:

```powershell
$env:STIP_Enabled = "false"
dotnet run --project DebitCardApi\DebitCardApi.csproj
```

Run the STIP implementation:

```powershell
$env:STIP_Enabled = "true"
dotnet run --project DebitCardApi\DebitCardApi.csproj
```

`STIP_Enabled=true` selects the STIP business and data layers. `false` or unset selects the regular layers.

## Test

```powershell
dotnet test DebitCardApi.sln
```

## Deploy To Local Kubernetes

These commands build the API image locally and install two Helm releases:

- regular API in the `debit-card-api-regular` namespace with `STIP_Enabled=false`
- STIP API in the `debit-card-api-stip` namespace with `STIP_Enabled=true`

### 1. Build the Docker image

PowerShell:

```powershell
docker build -t debit-card-api:local .
```

Bash:

```bash
./scripts/build-image.sh
```

### 2. Make the image available to your cluster

Docker Desktop Kubernetes can usually use the local image directly.

For kind:

PowerShell:

```powershell
kind load docker-image debit-card-api:local
```

Bash:

```bash
./scripts/load-image-kind.sh
```

For minikube:

PowerShell:

```powershell
minikube image load debit-card-api:local
```

Bash:

```bash
./scripts/load-image-minikube.sh
```

### 3. Install the regular API

PowerShell:

```powershell
helm upgrade --install debit-card-api-regular .\charts\debit-card-api `
  --namespace debit-card-api-regular `
  --create-namespace `
  -f .\charts\debit-card-api\values-regular.yaml
```

Bash:

```bash
./scripts/deploy-regular.sh
```

### 4. Install the STIP API

PowerShell:

```powershell
helm upgrade --install debit-card-api-stip .\charts\debit-card-api `
  --namespace debit-card-api-stip `
  --create-namespace `
  -f .\charts\debit-card-api\values-stip.yaml
```

Bash:

```bash
./scripts/deploy-stip.sh
```

To install both releases with one bash command:

```bash
./scripts/deploy-all.sh
```

### 5. Verify both pods are running

PowerShell:

```powershell
kubectl get pods -n debit-card-api-regular
kubectl get pods -n debit-card-api-stip
```

Bash:

```bash
./scripts/verify-deployments.sh
```

Expected status:

```text
READY   STATUS
1/1     Running
```

### 6. Verify the deployments

```powershell
kubectl get deployment -n debit-card-api-regular
kubectl get deployment -n debit-card-api-stip
```

Expected readiness:

```text
READY
1/1
```

## Helm Configuration

The STIP values file sets:

```yaml
stip:
  enabled: true
```

That renders the pod environment variable:

```yaml
STIP_Enabled: "true"
```

The regular release sets `STIP_Enabled` to `"false"`.
