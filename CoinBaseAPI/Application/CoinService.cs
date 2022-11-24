using CoinBase.Core;
using CoinBase.ServicesBase;
using EntityFramework;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace CoinBase.Services
{
    public class CoinService : ServiceBase
    {
        //static HttpClient client = new HttpClient();
        public bool Insert(List<Coin> coins)
        {
            try
            {
                db.AddRange(coins);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool InsertSingular(Coin coin)
        {
            try
            {
                db.Add(coin);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }



        public bool UpdateCoins(Coin coin, int ID, string newName, string newCode, string newSymbol, int ConverterID, string ConverterNewName, double newValue)
        {
            try
            {


                var _coin = db.Coins.Include(x => x.Converters).FirstOrDefault(x => x.ID == ID);
                _coin.Name = newName;
                _coin.Code = newCode;
                _coin.Symbol = newSymbol;
                _coin.ID = ID;

                var _converter = db.CoinConverters.FirstOrDefault(x => x.ID == ConverterID);
                _converter.ConverterName = ConverterNewName;
                _converter.Value = newValue;


                db.Attach(_coin);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public Coin GetCoin(int ID)
        {

            var _coin = db.Coins.Include(x => x.Converters).FirstOrDefault(x => x.ID == ID);
            if (_coin == null)
                throw new Exception("This coin does not exist in the database.");
            return _coin;

        }

        public List<Coin> CoinsList()
        {
            return db.Coins.Include(x => x.Converters).OrderBy(x => x.ID).ToList();
        }


        private void ValidateCoin(int ID, int IDto)
        {
            var coinCount = db.Coins.Count();
            var convertersCount = db.CoinConverters.Count();
            if (coinCount == 0)
            {
                throw new Exception("No coins in the database.");
            }
            if (ID == IDto)
            {
                throw new Exception("Can't convert a coin to itself.");
            }
           

        }

        public Coin UpdateDatabase(Coin input)
        {

            try
            {
                var _Coindb = db.Coins
                    .AsNoTracking()
                    .FirstOrDefault(x => x.ID == input.ID);
                var _Converterdb = db.CoinConverters
                    .AsNoTracking()
                    .Where(x => x.IDCoin == input.ID)
                    .ToList();
                //db.CoinConverters.RemoveRange(_Coin.Converters);
                //db.CoinConverters.AddRange(coin.Converters);

                var converterDelete = _Converterdb.Where(x => input.Converters.Count(z => z.ID == x.ID) == 0).ToList();
                var converterInsert = input.Converters.Where(x => x.ID == 0).ToList();
                var converterUpdate = input.Converters.Where(x => _Converterdb.Count(z => z.ID == x.ID) > 0).ToList();
                db.CoinConverters.RemoveRange(converterDelete);
                db.CoinConverters.AddRange(converterInsert);
                db.CoinConverters.UpdateRange(converterUpdate);
                _Coindb = input;
                _Coindb.Converters.Clear();
                db.Coins.Update(_Coindb);
                db.SaveChanges();
                return input;
            }
            catch (Exception)
            {
                throw;
            }



        }

        public double _ConverterResult(int ID, int IDto, double Amount)
        {
            ValidateCoin(ID, IDto);

            var _Coin = db.Coins.First(x => x.ID == ID);

            var _coinToOption = _Coin.Converters.FirstOrDefault(x => x.IDCoinTo == IDto);

            if (_coinToOption == null)
            {
                throw new Exception("Non existent operation.");
            }

            return (Amount * _coinToOption.Value);
        }

        public string DeleteCoin(int ID)
        {
            try
            {
                var coin = db.Coins.Include(x => x.Converters).FirstOrDefault(x => x.ID == ID);
                db.Coins.Attach(coin);
                db.Coins.Remove(coin);
                db.SaveChanges();
                return string.Format("Coin removed from Database");
            }
            catch (Exception)
            {

                throw new Exception("Coin not found in database.");
            }
        }


        public async Task<Double> UpdateConverterValue(int id, string CodeCoin, string CodeCoinTo)
        {

            try
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.BaseAddress = new System.Uri("https://economia.awesomeapi.com.br");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    CoinContext db = new CoinContext();
                    HttpResponseMessage response = await client.GetAsync(string.Format("/json/daily/{0}-{1}", CodeCoin, CodeCoinTo));
                    var myInstance = JsonConvert.DeserializeObject<List<TesteResponseObj>>(await response.Content.ReadAsStringAsync());

                    if (response.IsSuccessStatusCode)
                    {
                        return Convert.ToDouble(myInstance.FirstOrDefault().high);
                    }
                    return 0;
                }
            }
            catch (Exception ex)
            {

                throw;
            }




        }

        public async Task SyncToApi()
        {
            var allCoins = await db.Coins.Include(x => x.Converters).ToListAsync();
            foreach (var coin in allCoins)
            {
                foreach (var converter in coin.Converters)
                {
                    var coinTo = await db.Coins.Where(x => x.ID == converter.IDCoinTo).FirstOrDefaultAsync();
                    var result = await UpdateConverterValue(coin.ID, coin.Code, coinTo.Code);
                    converter.Value = result;
                    db.CoinConverters.Update(converter);
                    db.SaveChanges();
                }
            }

        }





        [Serializable]
        public class TesteResponseObj
        {
            [JsonProperty(PropertyName = "high")]
            public string high { get; set; }
            public string Value { get { return high; } }
        }
    }
}
