using Polly.CircuitBreaker;
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
            Wrap_WaitAndRetry_CircuitBreaker.OperationWithWrappedPolicyAsync().Wait();
        }
    }
}
