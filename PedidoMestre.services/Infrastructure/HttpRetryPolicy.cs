using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System.Net;

namespace PedidoMestre.Services.Infrastructure
{
    /// <summary>
    /// Classe base para políticas de retry em chamadas HTTP
    /// </summary>
    public static class HttpRetryPolicy
    {
        /// <summary>
        /// Cria uma política de retry com backoff exponencial
        /// </summary>
        /// <param name="maxRetries">Número máximo de tentativas (padrão: 3)</param>
        /// <param name="logger">Logger para registrar tentativas</param>
        /// <returns>Política de retry configurada</returns>
        public static AsyncRetryPolicy<HttpResponseMessage> CreateRetryPolicy(
            int maxRetries = 3,
            ILogger? logger = null)
        {
            return Policy
                .HandleResult<HttpResponseMessage>(r => 
                    !r.IsSuccessStatusCode && 
                    (r.StatusCode == HttpStatusCode.RequestTimeout ||
                     r.StatusCode == HttpStatusCode.InternalServerError ||
                     r.StatusCode == HttpStatusCode.BadGateway ||
                     r.StatusCode == HttpStatusCode.ServiceUnavailable ||
                     r.StatusCode == HttpStatusCode.GatewayTimeout))
                .Or<HttpRequestException>()
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync(
                    retryCount: maxRetries,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        var statusInfo = outcome.Result?.StatusCode.ToString() 
                            ?? outcome.Exception?.GetType().Name 
                            ?? "Unknown";
                        
                        logger?.LogWarning(
                            "Tentativa {RetryCount}/{MaxRetries} após {Delay} segundos. Status: {StatusCode}",
                            retryCount,
                            maxRetries,
                            timespan.TotalSeconds,
                            statusInfo
                        );
                    }
                );
        }

        /// <summary>
        /// Cria uma política de retry genérica para qualquer exceção
        /// </summary>
        public static AsyncRetryPolicy CreateGenericRetryPolicy(
            int maxRetries = 3,
            ILogger? logger = null)
        {
            return Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: maxRetries,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timespan, retryCount, context) =>
                    {
                        logger?.LogWarning(
                            "Tentativa {RetryCount}/{MaxRetries} após {Delay} segundos. Erro: {Error}",
                            retryCount,
                            maxRetries,
                            timespan.TotalSeconds,
                            exception.Message
                        );
                    }
                );
        }
    }
}

