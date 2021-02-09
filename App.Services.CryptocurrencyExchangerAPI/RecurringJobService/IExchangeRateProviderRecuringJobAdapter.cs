namespace App.Services.CryptocurrencyExchangerAPI.RecurringJobService
{
    public interface IExchangeRateProviderRecuringJobAdapter
    {
        void RunJob(string ProviderName);
    }
}