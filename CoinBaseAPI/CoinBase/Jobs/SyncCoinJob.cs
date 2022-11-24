using CoinBase.Services;

namespace CoinBase.Jobs
{
    [Serializable]
    public class SyncCoinJob
    {
        public async Task ExecuteJob()
        {
            var service = new CoinService();
            await service.SyncToApi();
        }
    }
}
