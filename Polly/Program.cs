using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Polly
{
    class Program
    {
        private static Policy policy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(5));

        public async Task OperationWithBasicRetryAsync()
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

        public async Task TransientOperationAsync()
        {
            HttpClient client = new HttpClient() { BaseAddress = new Uri("https://reqres.inn/") };
            var response = policy.Execute(() => client.GetAsync(("api/users")).Result);
        }
    }
}
