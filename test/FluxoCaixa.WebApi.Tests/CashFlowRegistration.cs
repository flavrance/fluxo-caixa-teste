namespace FluxoCaixa.WebApi.Tests
{
    using Microsoft.AspNetCore.Hosting;
    using Autofac.Extensions.DependencyInjection;
    using Microsoft.Extensions.Configuration;
    using FluxoCaixa.WebApi;
    using Microsoft.AspNetCore.TestHost;
    using Xunit;
    using System.Threading.Tasks;
    using System.Net.Http;
    using Newtonsoft.Json;
    using System.Text;
    using Newtonsoft.Json.Linq;
    using System;

    public class CashFlowRegistration
    {
        private readonly TestServer server;
        private readonly HttpClient client;

        public CashFlowRegistration()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    IWebHostEnvironment env = builderContext.HostingEnvironment;
                    
                    config.AddJsonFile("autofac.json")
                    .AddEnvironmentVariables();
                })
                .ConfigureServices(services => services.AddAutofac());

            server = new TestServer(webHostBuilder);
            client = server.CreateClient();
        }

        [Fact]
        public async Task Register_Deposit_Withdraw_Close()
        {   
            /*
            await GetCashFlow(customerId_accountId.Item2);
            await Debit(customerId_accountId.Item2, 100);            
            await Credit(customerId_accountId.Item2, 500);
            await Credit(customerId_accountId.Item2, 400);            
            await Debit(customerId_accountId.Item2, 400);
            await Debit(customerId_accountId.Item2, 500);            
            */
            
        }        

        private async Task GetCashFlow(string cashFlowId)
        {
            string result = await client.GetStringAsync("/api/CashFlows/" + cashFlowId);
        }
       

        private async Task Credit(string cashFlow, double amount)
        {
            var json = new
            {
                cashFlowId = cashFlow,
                amount = amount,
            };

            string data = JsonConvert.SerializeObject(json);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

            var response = await client.PatchAsync("api/CashFlows/Credit", content);
            string result = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
        }

        private async Task Debit(string cashFlow, double amount)
        {
            var json = new
            {
                cashFlowId = cashFlow,
                amount = amount,
            };

            string data = JsonConvert.SerializeObject(json);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

            var response = await client.PatchAsync("api/CashFlows/Debit", content);
            string result = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
        }        
    }

    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri, HttpContent content)
        {
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = content
            };

            var response = await client.SendAsync(request);
            return response;
        }
    }
}
