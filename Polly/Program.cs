using Polly.Fallback;
using Polly.Wrap;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Polly
{
    class Program
    {
        static void Main(string[] args)
        {
            Wrap_Fallback_WaitAndRetry_CircuitBreaker.OperationWithWrappedPolicyAsync().Wait();
        }
    }
}