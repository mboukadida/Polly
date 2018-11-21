using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Polly
{
    public static class WaitAndRetry
    {
        private static Policy policy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(5));

        public static async Task OperationWithBasicRetryAsync()
        {
            try
            {
                await policy.ExecuteAsync(() => TransientOperationAsync());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task TransientOperationAsync()
        {
            HttpClient client = new HttpClient() { BaseAddress = new Uri("https://reqres.inn/") };
            var response = await client.GetAsync("api/users");
        }
    }
}
