using Microsoft.AspNetCore.Mvc;
using CoinBase.Core;
using CoinBase.Base;
using CoinBase.Services;
using System.Net.Http.Headers;

namespace CoinBase.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoinController : ControllerCoinBase
    {
        private CoinService _coinService;

        private InsertOUpdateResult _insertResult;

        static HttpClient client = new HttpClient();

        private readonly ILogger<CoinController> _logger;

        public CoinController(ILogger<CoinController> logger)
        {

            _insertResult = new InsertOUpdateResult();
            _coinService = new CoinService();
            _logger = logger;
        }

        [HttpPost]
        [Route("InsertCoins")]
        public InsertOUpdateResult InsertCoins([FromBody] List<CoinInput> coins)
        {
            try
            {
                var Coins = mapper.Map<List<Coin>>(coins);
                _coinService.Insert(Coins);
                _insertResult.IsSuccess = true;
                _insertResult.Message = "Coins successfully added.";
                return _insertResult;
            }
            catch (Exception)
            {

                throw new NotImplementedException();
            }

        }


        [HttpPost]
        [Route("InsertCoin")]
        public InsertOUpdateResult InsertCoin(CoinInput coin)
        {
            try
            {
                var Coin = mapper.Map<Coin>(coin);
                _coinService.InsertSingular(Coin);
                _insertResult.IsSuccess = true;
                _insertResult.Message = "Coin succesfully added.";
                return _insertResult;
            }
            catch (Exception)
            {

                throw new NotImplementedException();
            }
        }



        [HttpPost]
        [Route("UpdateCoinDb")]
        public InsertOUpdateResult UpdateCoinDb(CoinInput coin)
        {
            try
            {
                var Coin = mapper.Map<Coin>(coin);
                _coinService.UpdateDatabase(Coin);
                _insertResult.IsSuccess = true;
                _insertResult.Message = "Database updated.";
            }
            catch (Exception ex)
            {
                _insertResult.IsSuccess = false;
                _insertResult.Message = ex.Message;
            }
            return (_insertResult);


        }

        [HttpGet]
        [Route("CoinList")]
        public List<CoinInput> CoinList()
        {
            var listReturn = _coinService.CoinsList();
            return mapper.Map<List<CoinInput>>(listReturn);
        }

        [HttpPost]
        [Route("GetCoin")]
        public CoinInput GetCoin(int ID)
        {
            var coinReturn = _coinService.GetCoin(ID);
            return mapper.Map<CoinInput>(coinReturn);
        }

        [HttpGet]
        [Route("CoinCalc")]
        public CalcResult CoinCalc(int ID, int IDto, double Amount)
        {
            var _calcResult = new CalcResult();
            _calcResult.Amount = Amount;
            try
            {
                _calcResult.CoinName = _coinService.GetCoin(ID).Name;
                _calcResult.CointoName = _coinService.GetCoin(IDto).Name;
                _calcResult.ConvertResult = _coinService._ConverterResult(ID, IDto, Amount);
                _calcResult.IsSuccess = true;
                _calcResult.Message = "Sucessfully converted!";
            }
            catch (Exception ex)
            {
                _calcResult.IsSuccess = false;
                _calcResult.Message = ex.Message;
            }

            return _calcResult;
        }

    

        [HttpPost]
        [Route("SyncWithApi")]
        public InsertOUpdateResult SyncWithApi()
        {
            try
            {
                var result =_coinService.SyncToApi();
                var results = new InsertOUpdateResult();
                results.Message = "Synced to API";
                results.IsSuccess = true;
                return results;

            }
            catch (Exception)
            {

                var results = new InsertOUpdateResult();
                results.Message = "Error in sync";
                results.IsSuccess = false;
                return results;
            }
            
        }

        [HttpDelete]
        [Route("DeleteCoin")]
        public string DeleteCoin(int ID)
        {
            return _coinService.DeleteCoin(ID);
        }




        public class CalcResult
        {
            public string CoinName { get; set; }
            public string CointoName { get; set; }

            public double ConvertResult { get; set; }

            public double Amount { get; set; }
            public string Message { get; set; }
            public bool IsSuccess { get; set; }
        }

        public class InsertOUpdateResult
        {
            public string Message { get; set; }
            public bool IsSuccess { get; set; }
        }

        public class CoinInput
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
            public string Symbol { get; set; }
            public List<CoinConverterInput> Converters { get; set; }
        }

        public class CoinConverterInput
        {
            public int ID { get; set; }
            public string ConverterName { get; set; }
            public int IDCoin { get; set; }
            public int IDCoinTo { get; set; }

            public double Value { get; set; }
        }

    }
}