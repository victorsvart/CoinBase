namespace CoinBase.Core
{
    public class CoinConverter
    {

        public int ID { get; set; }
        public string ConverterName { get; set; }
        public int IDCoin { get; set; }
        public int IDCoinTo { get; set; }
        public double Value { get; set; }
        public Coin Coin { get; set; }

    }
}
