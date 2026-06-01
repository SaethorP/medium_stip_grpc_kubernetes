using System.Net;
using System.Net.Sockets;
using DebitCardApi;
using Grpc.Net.Client;

// Calls DebitCardService.CreatePayment on the ingress DNS host and logs the response.
//
// Environment variables:
//   API_HOST    gRPC :authority / TLS SNI host        (default debit-card-api.local)
//   API_PORT    port                                   (default 443)
//   RESOLVE_IP  dial this IP instead of resolving DNS  (default 127.0.0.1; set to "" to use real DNS)
//
// By default it dials 127.0.0.1 while still presenting the DNS host as the authority/SNI,
// so it works against the Docker Desktop load balancer without a hosts-file entry. Once
// "debit-card-api.local" is in your hosts file you can set RESOLVE_IP="" to use real DNS.

var host = Environment.GetEnvironmentVariable("API_HOST") is { Length: > 0 } h ? h : "debit-card-api.local";
var port = int.TryParse(Environment.GetEnvironmentVariable("API_PORT"), out var p) ? p : 443;
var resolveIp = Environment.GetEnvironmentVariable("RESOLVE_IP");
resolveIp = resolveIp is null ? "127.0.0.1" : resolveIp; // null (unset) => default; "" => use DNS

var handler = new SocketsHttpHandler
{
    SslOptions = new System.Net.Security.SslClientAuthenticationOptions
    {
        // The ingress uses a self-signed cert; accept it for this test client.
        RemoteCertificateValidationCallback = (_, _, _, _) => true,
    },
    // Fresh connection each run so a router switch is observed immediately.
    PooledConnectionLifetime = TimeSpan.Zero,
};

if (!string.IsNullOrEmpty(resolveIp))
{
    handler.ConnectCallback = async (_, ct) =>
    {
        var socket = new Socket(SocketType.Stream, ProtocolType.Tcp) { NoDelay = true };
        await socket.ConnectAsync(IPAddress.Parse(resolveIp), port, ct);
        return new NetworkStream(socket, ownsSocket: true);
    };
}

// Default https port is implicit so the :authority / Host header is the bare host,
// which is what the nginx ingress server_name matches on.
var address = port == 443 ? $"https://{host}" : $"https://{host}:{port}";
var dialDescription = string.IsNullOrEmpty(resolveIp) ? "via DNS" : $"dialing {resolveIp}:{port}";

Console.WriteLine($"--> {address}  DebitCardService/CreatePayment  ({dialDescription})");

using var channel = GrpcChannel.ForAddress(address, new GrpcChannelOptions { HttpHandler = handler });
var client = new DebitCardService.DebitCardServiceClient(channel);

try
{
    var reply = await client.CreatePaymentAsync(new CreatePaymentRequest
    {
        CardNumber = "4111111111111111",
        Amount = 25.50,
        Merchant = "Test Merchant",
    });

    var variant = reply.PaymentId.StartsWith("STIP-", StringComparison.Ordinal) ? "stip" : "regular";
    Console.WriteLine("<-- OK");
    Console.WriteLine($"    paymentId : {reply.PaymentId}");
    Console.WriteLine($"    message   : {reply.Message}");
    Console.WriteLine($"    servedBy  : {variant} variant");
}
catch (Grpc.Core.RpcException ex)
{
    Console.WriteLine($"<-- FAILED  {ex.StatusCode}: {ex.Status.Detail}");
    Environment.Exit(1);
}