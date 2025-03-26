namespace MongoAnalytics.Model
{
    public class AccountModel
    {
        public long Auid { get; set; }
        public int Tier { get; set; }
    }

    public class CharacterModel
    {
        public long Cuid { get; set; }
        public long Auid { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class CharacterAmountModel
    {
        public long Cuid { get; set; }
        public string Name { get; set; } = string.Empty;
        public long Amount { get; set; }
    }

    public class TierModel
    {
        public long Auid { get; set; }
        public int Tier { get; set; }
    }

    public class ShopModel
    {
        public int Id { get; set; }
        public int Qty { get; set; }
        public int TotalAuid { get; set; }
    }

    public class PlayModel
    {
        public long Account { get; set; }
        public long Amount { get; set; }
    }

    public class LoginModel
    {
        public long Auid { get; set; }
        public DateTime Time { get; set; }
        public string CSCode { get; set; } = string.Empty;
    }
}
