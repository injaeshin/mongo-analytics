using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.Serialization;

namespace MongoAnalytics.ModelLog
{
    public static class LogMapper
    {
        public static void Init()
        {
            BsonClassMap.RegisterClassMap<LogWebRegAccountModel>(classMap =>
            {
                classMap.AutoMap();
                classMap.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<LogUserAccountModel>(classMap =>
            {
                classMap.AutoMap();
                classMap.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<LogTierModel>(classMap =>
            {
                classMap.AutoMap();
                classMap.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<LogUserDataModel>(classMap =>
            {
                classMap.AutoMap();
                classMap.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<LogTierDataModel>(classMap =>
            {
                classMap.AutoMap();
                classMap.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<LogShopModel>(classMap =>
            {
                classMap.AutoMap();
                classMap.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<LogGoodsModel>(classMap =>
            {
                classMap.AutoMap();
                classMap.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<LogCharacterModel>(classMap =>
            {
                classMap.AutoMap();
                classMap.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<LogCurrencyModel>(classMap =>
            {
                classMap.AutoMap();
                classMap.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<LogCurrencyDataModel>(classMap =>
            {
                classMap.AutoMap();
                classMap.SetIgnoreExtraElements(true);
            });
        }
    }

    [BsonNoId, DataContract]
    public class LogWebRegAccountModel
    {
        [BsonElement("filter")]
        public long Filter { get; set; }

        [BsonElement("time")]
        public DateTime Time { get; set; }

        [BsonElement("cs_code")]
        public string CSCode { get; set; } = string.Empty;

        [BsonElement("auid")]
        public long Auid { get; set; }
    }

    [BsonNoId, DataContract]
    public class LogUserAccountModel
    {
        [BsonElement("filter")]
        public long Filter { get; set; }

        [BsonElement("time")]
        public DateTime Time { get; set; }

        [BsonElement("user")]
        public LogUserDataModel? User { get; set; }
    }

    public class LogTierModel
    {
        [BsonElement("filter")]
        public long Filter { get; set; }

        [BsonElement("user")]
        public LogUserDataModel? User { get; set; }

        [BsonElement("tier")]
        public LogTierDataModel? Tier { get; set; }

        [BsonElement("time")]
        public DateTime Time { get; set; }
    }

    [BsonNoId, DataContract]
    public class LogTierDataModel
    {
        [BsonElement("remain_grade")]
        public int RemainGrade { get; set; }
    }

    [BsonNoId, DataContract]
    public class LogUserDataModel
    {
        [BsonElement("auth_key")]
        public string Key { get; set; } = string.Empty;

        [BsonElement("auid")]
        public long Auid { get; set; }

        [BsonElement("cuid")]
        public long Cuid { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("tier")]
        public int Tier { get; set; }
    }

    [BsonNoId, DataContract]
    public class LogShopModel
    {
        [BsonElement("filter")]
        public long Filter { get; set; }

        [BsonElement("user")]
        public LogUserDataModel? User { get; set; }

        [BsonElement("goods")]
        public LogGoodsModel? Goods { get; set; }

        [BsonElement("time")]
        public DateTime Time { get; set; }
    }

    [BsonNoId, DataContract]
    public class LogGoodsModel
    {
        [BsonElement("amount")]
        public int Amount { get; set; }

        [BsonElement("cost")]
        public int Cost { get; set; }

        [BsonElement("id")]
        public int Id { get; set; }
    }

    [BsonNoId, DataContract]
    public class LogCurrencyModel
    {
        [BsonElement("filter")]
        public int Filter { get; set; }

        [BsonElement("user")]
        public LogUserDataModel? User { get; set; }

        [BsonElement("currency")]
        public List<LogCurrencyDataModel>? Currency { get; set; }

        [BsonElement("reason")]
        public int Reason { get; set; }

        [BsonElement("time")]
        public DateTime Time { get; set; }
    }

    [BsonNoId, DataContract]
    public class LogCurrencyDataModel
    {
        [BsonElement("amount")]
        public long Amount { get; set; }

        [BsonElement("type")]
        public byte Type { get; set; }
    }

    [BsonNoId, DataContract]
    public class LogCharacterModel
    {
        [BsonElement("filter")]
        public int Filter { get; set; }

        [BsonElement("user")]
        public LogUserDataModel? User { get; set; }

        [BsonElement("time")]
        public DateTime Time { get; set; }
    }
}
