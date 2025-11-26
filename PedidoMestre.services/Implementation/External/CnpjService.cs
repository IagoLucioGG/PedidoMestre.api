using System.Text.Json;
using Microsoft.Extensions.Logging;
using PedidoMestre.Models.Common;
using PedidoMestre.Services.Infrastructure;
using PedidoMestre.Services.Interfaces;
using Polly;
using Polly.Retry;

namespace PedidoMestre.Services.Implementation.External
{
    /// <summary>
    /// Service isolado para validação de CNPJ usando APIs externas
    /// </summary>
    public class CnpjService : ICnpjService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CnpjService> _logger;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public CnpjService(HttpClient httpClient, ILogger<CnpjService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _retryPolicy = HttpRetryPolicy.CreateRetryPolicy(maxRetries: 3, logger);
        }

        public async Task<ResponseModel<CnpjInfo>> ValidarCnpjAsync(string cnpj)
        {
            try
            {
                // Limpar CNPJ (remover caracteres especiais)
                var cnpjLimpo = LimparCnpj(cnpj);

                if (!ValidarFormatoCnpj(cnpjLimpo))
                {
                    return new ResponseModel<CnpjInfo>("CNPJ inválido. Formato incorreto.");
                }

                // Tentar múltiplas APIs gratuitas (fallback)
                // 1. Tentar BrasilAPI primeiro (mais confiável)
                var resultado = await ConsultarBrasilApiAsync(cnpjLimpo);
                
                if (!resultado.Status)
                {
                    // 2. Fallback para OpenCNPJ
                    resultado = await ConsultarOpenCnpjAsync(cnpjLimpo);
                }

                if (!resultado.Status)
                {
                    // 3. Último fallback: CNPJá
                    resultado = await ConsultarCnpjaAsync(cnpjLimpo);
                }

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar CNPJ: {Cnpj}", cnpj);
                return new ResponseModel<CnpjInfo>($"Erro ao validar CNPJ: {ex.Message}");
            }
        }

        private async Task<ResponseModel<CnpjInfo>> ConsultarOpenCnpjAsync(string cnpj)
        {
            try
            {
                var url = $"https://opencnpj.org/api/v1/cnpj/{cnpj}";
                
                _logger.LogInformation("Consultando CNPJ na API OpenCNPJ: {Cnpj}", cnpj);

                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync(url)
                );

                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseModel<CnpjInfo>($"Erro ao consultar CNPJ: {response.StatusCode}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var dados = JsonSerializer.Deserialize<JsonElement>(json);

                if (dados.ValueKind == JsonValueKind.Undefined || dados.ValueKind == JsonValueKind.Null)
                {
                    return new ResponseModel<CnpjInfo>("CNPJ não encontrado ou inválido.");
                }

                // OpenCNPJ retorna um array com um objeto
                JsonElement empresa;
                if (dados.ValueKind == JsonValueKind.Array && dados.GetArrayLength() > 0)
                {
                    empresa = dados[0];
                }
                else
                {
                    empresa = dados;
                }

                var cnpjInfo = new CnpjInfo
                {
                    Cnpj = cnpj,
                    RazaoSocial = empresa.TryGetProperty("razao_social", out var razaoSocial) 
                        ? razaoSocial.GetString() ?? string.Empty 
                        : string.Empty,
                    NomeFantasia = empresa.TryGetProperty("nome_fantasia", out var nomeFantasia) 
                        ? nomeFantasia.GetString() ?? string.Empty 
                        : string.Empty,
                    Situacao = empresa.TryGetProperty("situacao_cadastral", out var situacao) 
                        ? situacao.GetString() ?? "Desconhecida" 
                        : "Desconhecida",
                    Logradouro = empresa.TryGetProperty("logradouro", out var logradouro) 
                        ? logradouro.GetString() 
                        : null,
                    Numero = empresa.TryGetProperty("numero", out var numero) 
                        ? numero.GetString() 
                        : null,
                    Complemento = empresa.TryGetProperty("complemento", out var complemento) 
                        ? complemento.GetString() 
                        : null,
                    Bairro = empresa.TryGetProperty("bairro", out var bairro) 
                        ? bairro.GetString() 
                        : null,
                    Cidade = empresa.TryGetProperty("municipio", out var municipio) 
                        ? municipio.GetString() 
                        : null,
                    Uf = empresa.TryGetProperty("uf", out var uf) 
                        ? uf.GetString() 
                        : null,
                    Cep = empresa.TryGetProperty("cep", out var cep) 
                        ? cep.GetString() 
                        : null,
                    Valido = empresa.TryGetProperty("situacao_cadastral", out var sit) && 
                             (sit.GetString()?.ToLower() == "ativa" || sit.GetString()?.ToLower() == "2")
                };

                return new ResponseModel<CnpjInfo>(cnpjInfo, "CNPJ validado com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Erro ao consultar CNPJ na API OpenCNPJ: {Cnpj}", cnpj);
                return new ResponseModel<CnpjInfo>($"Erro ao consultar CNPJ: {ex.Message}");
            }
        }

        private async Task<ResponseModel<CnpjInfo>> ConsultarCnpjaAsync(string cnpj)
        {
            try
            {
                var url = $"https://www.cnpja.com/api/open/{cnpj}";
                
                _logger.LogInformation("Consultando CNPJ na API CNPJá: {Cnpj}", cnpj);

                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync(url)
                );

                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseModel<CnpjInfo>($"Erro ao consultar CNPJ: {response.StatusCode}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var dados = JsonSerializer.Deserialize<JsonElement>(json);

                if (dados.ValueKind == JsonValueKind.Undefined || dados.ValueKind == JsonValueKind.Null)
                {
                    return new ResponseModel<CnpjInfo>("CNPJ não encontrado ou inválido.");
                }

                var cnpjInfo = new CnpjInfo
                {
                    Cnpj = cnpj,
                    RazaoSocial = dados.TryGetProperty("razao_social", out var razaoSocial) 
                        ? razaoSocial.GetString() ?? string.Empty 
                        : string.Empty,
                    NomeFantasia = dados.TryGetProperty("nome_fantasia", out var nomeFantasia) 
                        ? nomeFantasia.GetString() ?? string.Empty 
                        : string.Empty,
                    Situacao = dados.TryGetProperty("situacao", out var situacao) 
                        ? situacao.GetString() ?? "Desconhecida" 
                        : "Desconhecida",
                    Logradouro = dados.TryGetProperty("logradouro", out var logradouro) 
                        ? logradouro.GetString() 
                        : null,
                    Numero = dados.TryGetProperty("numero", out var numero) 
                        ? numero.GetString() 
                        : null,
                    Complemento = dados.TryGetProperty("complemento", out var complemento) 
                        ? complemento.GetString() 
                        : null,
                    Bairro = dados.TryGetProperty("bairro", out var bairro) 
                        ? bairro.GetString() 
                        : null,
                    Cidade = dados.TryGetProperty("municipio", out var municipio) 
                        ? municipio.GetString() 
                        : null,
                    Uf = dados.TryGetProperty("uf", out var uf) 
                        ? uf.GetString() 
                        : null,
                    Cep = dados.TryGetProperty("cep", out var cep) 
                        ? cep.GetString() 
                        : null,
                    Valido = dados.TryGetProperty("situacao", out var sit) && 
                             sit.GetString()?.ToLower() == "ativa"
                };

                return new ResponseModel<CnpjInfo>(cnpjInfo, "CNPJ validado com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Erro ao consultar CNPJ na API CNPJá: {Cnpj}", cnpj);
                return new ResponseModel<CnpjInfo>($"Erro ao consultar CNPJ: {ex.Message}");
            }
        }

        private async Task<ResponseModel<CnpjInfo>> ConsultarBrasilApiAsync(string cnpj)
        {
            try
            {
                var url = $"https://brasilapi.com.br/api/cnpj/v1/{cnpj}";
                
                _logger.LogInformation("Consultando CNPJ na API BrasilAPI: {Cnpj}", cnpj);

                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync(url)
                );

                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseModel<CnpjInfo>($"Erro ao consultar CNPJ: {response.StatusCode}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var dados = JsonSerializer.Deserialize<JsonElement>(json);

                if (dados.ValueKind == JsonValueKind.Undefined || dados.ValueKind == JsonValueKind.Null)
                {
                    return new ResponseModel<CnpjInfo>("CNPJ não encontrado ou inválido.");
                }

                var cnpjInfo = new CnpjInfo
                {
                    Cnpj = cnpj,
                    RazaoSocial = dados.TryGetProperty("razao_social", out var razaoSocial) 
                        ? razaoSocial.GetString() ?? string.Empty 
                        : string.Empty,
                    NomeFantasia = dados.TryGetProperty("nome_fantasia", out var nomeFantasia) 
                        ? nomeFantasia.GetString() ?? string.Empty 
                        : string.Empty,
                    Situacao = dados.TryGetProperty("descricao_situacao_cadastral", out var situacao) 
                        ? situacao.GetString() ?? "Desconhecida" 
                        : "Desconhecida",
                    Logradouro = dados.TryGetProperty("logradouro", out var logradouro) 
                        ? logradouro.GetString() 
                        : null,
                    Numero = dados.TryGetProperty("numero", out var numero) 
                        ? numero.GetString() 
                        : null,
                    Complemento = dados.TryGetProperty("complemento", out var complemento) 
                        ? complemento.GetString() 
                        : null,
                    Bairro = dados.TryGetProperty("bairro", out var bairro) 
                        ? bairro.GetString() 
                        : null,
                    Cidade = dados.TryGetProperty("municipio", out var municipio) 
                        ? municipio.GetString() 
                        : null,
                    Uf = dados.TryGetProperty("uf", out var uf) 
                        ? uf.GetString() 
                        : null,
                    Cep = dados.TryGetProperty("cep", out var cep) 
                        ? cep.GetString() 
                        : null,
                    Valido = dados.TryGetProperty("descricao_situacao_cadastral", out var sit) && 
                             sit.GetString()?.ToLower().Contains("ativa") == true
                };

                return new ResponseModel<CnpjInfo>(cnpjInfo, "CNPJ validado com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Erro ao consultar CNPJ na API BrasilAPI: {Cnpj}", cnpj);
                return new ResponseModel<CnpjInfo>($"Erro ao consultar CNPJ: {ex.Message}");
            }
        }

        private string LimparCnpj(string cnpj)
        {
            return new string(cnpj.Where(char.IsDigit).ToArray());
        }

        private bool ValidarFormatoCnpj(string cnpj)
        {
            return cnpj.Length == 14;
        }
    }
}

