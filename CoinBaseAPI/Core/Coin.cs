namespace CoinBase.Core
{
    public class Coin
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Symbol { get; set; }

        public List<CoinConverter> Converters { get; set; }

        public Coin(int _ID, string _name, string _code, string _symbol)
        {
            Converters = new List<CoinConverter>();
            ID = _ID;
            Name = _name;
            Code = _code;
            Symbol = _symbol;
        }

        public Coin()
        {

        }
    }
}
