# medium_stip_grpc_kubernetes

This is a repo for the sample code and README for a medium post I am making.

It shows a gRPC API that has two interchangeable implementations — a **regular**
one and a **STIP** one — and how to run both in Kubernetes behind a single
address so you can **switch live traffic between them** with one command.

## Projects

- `DebitCardApi` is the gRPC API. It exposes `DebitCardService.CreatePayment`.
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

`STIP_Enabled=true` selects the STIP business and data layers. `false` or unset
selects the regular layers.

## Test

```powershell
dotnet test DebitCardApi.sln
```

## Deploy To Local Kubernetes

### How it works (read this first)

You deploy **one** Helm release into **one** namespace (`debit-card-api`). That
release runs **both** implementations at the same time:

```
                         ┌─────────────────────────────┐
  https://               │  Ingress  debit-card-api.local │
  debit-card-api.local ──▶│  (nginx, terminates TLS)      │
                         └───────────────┬─────────────┘
                                         │
                              ┌──────────▼──────────┐
                              │  Service  debit-card-api │   ← the "router"
                              │  picks pods by variant   │
                              └──────────┬──────────┘
                          activeVariant=regular │ (flip to stip)
                          ┌────────────────────┴───────────────────┐
                          ▼                                         ▼
              ┌───────────────────────┐               ┌───────────────────────┐
              │ debit-card-api-regular │               │  debit-card-api-stip   │
              │   STIP_Enabled=false   │               │   STIP_Enabled=true    │
              └───────────────────────┘               └───────────────────────┘
              (always running)                         (always running)
```

Terms, if you are new to Kubernetes:

- **Pod** — a running copy of the API container.
- **Deployment** — keeps a set of identical pods running. We have two:
  `debit-card-api-regular` and `debit-card-api-stip`.
- **Service** — a single stable in-cluster address. Ours is the *router*: it
  forwards to whichever variant is currently selected (`activeVariant`).
- **Ingress** — exposes the Service outside the cluster on a hostname
  (`debit-card-api.local`) with TLS.
- **Helm release** — one installed copy of the chart. Changing it (deploy,
  switch, image update) is always `helm upgrade`.

Because **both** variants stay running, switching only changes which one new
calls reach — in-flight calls finish on the old variant instead of being cut off.

### Prerequisites

- Docker
- A local Kubernetes cluster (Docker Desktop's Kubernetes, or `kind`/`minikube`)
- `kubectl` and `helm`
- An **nginx ingress controller** installed in the cluster
- A way for your host to reach the ingress on `localhost`. Docker Desktop's
  built-in load balancer publishes ports `80`/`443` to `localhost`
  automatically, so `https://localhost:443` reaches nginx with no port-forward.

> **Image tags.** All of the scripts below default `IMAGE_TAG` to the current
> git commit (`git rev-parse --short HEAD`). This gives every rebuild a unique
> tag, so Kubernetes always pulls in your latest code instead of reusing a cached
> image. As long as you run build, load, and deploy at the same commit, they all
> agree on the tag automatically. Override it any time with `IMAGE_TAG=...`.

> **Pick the scripts for your OS.** The same steps are provided in two flavors:
> - **macOS / Linux** — bash scripts in `scripts/bash/` (e.g. `./scripts/bash/deploy.sh`)
> - **Windows** — PowerShell scripts in `scripts/powershell/` (e.g. `.\scripts\powershell\deploy.ps1`)
>
> They do the same thing; use whichever matches your machine. The examples below
> show both.

### 1. Build the Docker image

Bash:

```bash
./scripts/bash/build-image.sh          # builds debit-card-api:<git short sha>
```

PowerShell:

```powershell
.\scripts\powershell\build-image.ps1
```

### 2. Make the image available to your cluster

Docker Desktop Kubernetes can usually use the local image directly.

For kind:

```bash
./scripts/bash/load-image-kind.sh        # PowerShell: .\scripts\powershell\load-image-kind.ps1
```

For minikube:

```bash
./scripts/bash/load-image-minikube.sh    # PowerShell: .\scripts\powershell\load-image-minikube.ps1
```

### 3. Deploy both variants

This creates the `debit-card-api` namespace, both deployments, the router
Service, the ingress, and a self-signed TLS secret. `activeVariant` defaults to
`regular`.

Bash:

```bash
./scripts/bash/deploy.sh
```

PowerShell:

```powershell
.\scripts\powershell\deploy.ps1
```

To start on the STIP variant instead, set `ACTIVE_VARIANT=stip` (bash) or
`$env:ACTIVE_VARIANT = "stip"` (PowerShell) before running deploy.

### 4. Point your host at the ingress

Add this line to your hosts file so `debit-card-api.local` resolves to your
machine (the Docker Desktop load balancer forwards `localhost:443` to nginx):

```text
127.0.0.1  debit-card-api.local
```

- Windows: `C:\Windows\System32\drivers\etc\hosts` (edit as Administrator)
- macOS/Linux: `/etc/hosts` (use `sudo`)

### 5. Call the API

Point a gRPC client at `https://debit-card-api.local`. The certificate is
self-signed, so disable certificate validation in your client (test usage only).
Calling `CreatePayment` returns a plain payment id from the regular variant, or
one prefixed with `STIP-` from the STIP variant — that is how you can tell which
variant served the call.

A ready-made test client is included. It calls the API in a loop and logs each
response (and which variant served it) until you press Ctrl+C:

```bash
./scripts/bash/call-api.sh           # PowerShell: .\scripts\powershell\call-api.ps1
```

It dials `localhost:443` by default, so it works even before you add the hosts
entry from step 4. Set `INTERVAL_MS` to change the delay between calls. Leave it
running in one terminal and switch variants (step 6) in another to watch traffic
move live.

### 6. Switch live traffic between variants

Send traffic to STIP, then back to regular:

Bash:

```bash
./scripts/bash/switch.sh stip
./scripts/bash/switch.sh regular
```

PowerShell:

```powershell
.\scripts\powershell\switch.ps1 stip
.\scripts\powershell\switch.ps1 regular
```

The switch runs `helm upgrade --reuse-values --set activeVariant=<variant>`,
which repoints the router Service. No pods are restarted. Give nginx a couple of
seconds to pick up the change; calls already in progress finish on the previous
variant.

Check which variant is active at any time:

```powershell
kubectl get svc debit-card-api -n debit-card-api -o jsonpath='{.spec.selector.debit-card-api/variant}'
```

### 7. Update to a new image

When you change code, build a new tag, load it, and redeploy with that tag.
`--reuse-values` keeps your current `activeVariant`.

Bash:

```bash
export IMAGE_TAG="$(git rev-parse --short HEAD)"
./scripts/bash/build-image.sh
./scripts/bash/load-image-kind.sh          # or load-image-minikube.sh
helm upgrade debit-card-api ./charts/debit-card-api \
  --namespace debit-card-api \
  --reuse-values \
  --set image.tag="${IMAGE_TAG}"
```

PowerShell:

```powershell
$env:IMAGE_TAG = (git rev-parse --short HEAD)
.\scripts\powershell\build-image.ps1
.\scripts\powershell\load-image-kind.ps1          # or load-image-minikube.ps1
helm upgrade debit-card-api .\charts\debit-card-api `
  --namespace debit-card-api `
  --reuse-values `
  --set image.tag=$env:IMAGE_TAG
```

### 8. Verify the deployment

Bash:

```bash
./scripts/bash/verify-deployments.sh
```

PowerShell:

```powershell
.\scripts\powershell\verify-deployments.ps1
```

This lists both deployments and pods (with a `VARIANT` column) and prints the
currently active variant. Expected pod status:

```text
READY   STATUS
1/1     Running
```

## Helm Configuration

Key values in `charts/debit-card-api/values.yaml`:

```yaml
# Which variant the router Service sends traffic to: regular | stip
activeVariant: regular

# Both variants always run; activeVariant decides which one is wired up.
variants:
  regular:
    stipEnabled: false
  stip:
    stipEnabled: true

ingress:
  enabled: true
  className: nginx
  host: debit-card-api.local
```

Each variant becomes a Deployment whose pods get `STIP_Enabled` from its
`stipEnabled` value, so the same image runs as regular or STIP depending on the
variant.
