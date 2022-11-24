using AutoMapper;
using CoinBase.Core;
using Microsoft.AspNetCore.Mvc;
using static CoinBase.Controllers.CoinController;

namespace CoinBase.Base
{
    public class ControllerCoinBase : ControllerBase
    {
        public Mapper mapper;
        public ControllerCoinBase()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Coin, CoinInput>().ReverseMap();
                cfg.CreateMap<CoinConverter, CoinConverterInput>().ReverseMap();
            });

            mapper = new Mapper(config);
        }
    }
}
