using System;
using System.Net.Http;

namespace DependencyInjectionWorkshop.Models
{
    public interface IFailedCounter
    {
        void ResetFailedCount(string accountId);
        void AddFailedCount(string accountId);
        int GetFailedCount(string accountId);
        bool GetAccountIsLocked(string accountId);
    }

    public class FailedCounter : IFailedCounter
    {
        public void ResetFailedCount(string accountId)
        {
            var resetResponse = new HttpClient() { BaseAddress = new Uri("http://joey.com/") }.PostAsJsonAsync("api/failedCounter/Reset", accountId).Result;
            resetResponse.EnsureSuccessStatusCode();
        }

        public void AddFailedCount(string accountId)
        {
            var addFailedCountResponse = new HttpClient() { BaseAddress = new Uri("http://joey.com/") }.PostAsJsonAsync("api/failedCounter/Add", accountId).Result;
            addFailedCountResponse.EnsureSuccessStatusCode();
        }

        public int GetFailedCount(string accountId)
        {
            var httpClient = new HttpClient() { BaseAddress = new Uri("http://joey.com/") };
            httpClient.PostAsJsonAsync("api/failedCounter/GetFailedCount", accountId).Result.EnsureSuccessStatusCode();
            var failedCountResponse = httpClient.PostAsJsonAsync("api/failedCounter/GetFailedCount", accountId).Result;
            var failedCount = failedCountResponse.Content.ReadAsAsync<int>().Result;
            return failedCount;
        }

        public bool GetAccountIsLocked(string accountId)
        {
            var isLockedResponse = new HttpClient() { BaseAddress = new Uri("http://joey.com/") }.PostAsJsonAsync("api/failedCounter/IsLocked", accountId).Result;
            isLockedResponse.EnsureSuccessStatusCode();
            return isLockedResponse.Content.ReadAsAsync<bool>().Result;
        }
    }
}