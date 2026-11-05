using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace FraudRuleEngine.API.Observability;

// Sends logs, traces and metrics to an OTLP endpoint (Seq locally; any OTLP backend in other environments).
// The endpoint is set with Observability:OtlpEndpoint, so the code does not change between environments.
public static class ObservabilityExtensions
{
    public static WebApplicationBuilder AddFraudEngineObservability(this WebApplicationBuilder builder)
    {
        var section = builder.Configuration.GetSection("Observability");
        var serviceName = section["ServiceName"] ?? "FraudRuleEngine.API";
        var endpointSetting = section["OtlpEndpoint"]
            ?? throw new InvalidOperationException("Observability:OtlpEndpoint is not configured.");
        // Configured as the base OTLP URL (e.g. http://localhost:5341/ingest/otlp). The exporter uses an
        // endpoint set in code as-is, so each signal's path (/v1/traces, /v1/metrics, /v1/logs) is added here.
        var baseEndpoint = endpointSetting.TrimEnd('/');
        var serviceVersion = typeof(ObservabilityExtensions).Assembly.GetName().Version?.ToString();

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName: serviceName, serviceVersion: serviceVersion))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(options => ConfigureOtlp(options, baseEndpoint, "v1/traces")))
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddOtlpExporter(options => ConfigureOtlp(options, baseEndpoint, "v1/metrics")));

        // Logs from ILogger go out through OpenTelemetry as well as to the console
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeScopes = true;
            logging.IncludeFormattedMessage = true;
            logging.AddOtlpExporter(options => ConfigureOtlp(options, baseEndpoint, "v1/logs"));
        });

        return builder;
    }

    private static void ConfigureOtlp(OtlpExporterOptions options, string baseEndpoint, string signalPath)
    {
        options.Endpoint = new Uri($"{baseEndpoint}/{signalPath}");
        // Seq and most HTTP-based collectors take OTLP over HTTP/protobuf (port 4317 gRPC is not used)
        options.Protocol = OtlpExportProtocol.HttpProtobuf;
    }
}
