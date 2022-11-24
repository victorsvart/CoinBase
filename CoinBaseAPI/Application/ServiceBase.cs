
using EntityFramework;

namespace CoinBase.ServicesBase
{
    public class ServiceBase
    {
        public CoinContext db;
        public ServiceBase()
        {
            db = new CoinContext();
        }
    }
}
