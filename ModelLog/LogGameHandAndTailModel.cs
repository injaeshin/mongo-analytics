using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.Serialization;
using MongoDB.Bson.Serialization;

namespace MongoAnalytics.ModelLog
{
    public static class LogHatMapper
    {
        public static void Init()
        {
            BsonClassMap.RegisterClassMap<LogHatModel>(classMap =>
            {
                classMap.AutoMap();
                classMap.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<LogHatDataModel>(classMap =>
            {
                classMap.AutoMap();
                classMap.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<LogHatInfoDataModel>(classMap =>
            {
                classMap.AutoMap();
                classMap.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<LogHatResultDataModel>(classMap =>
            {
                classMap.AutoMap();
                classMap.SetIgnoreExtraElements(true);
            });
        }
    }


    [BsonNoId, DataContract]
    public class LogHatModel
    {
        [BsonElement("filter")]
        public int Filter { get; set; }

        [BsonElement("reason")]
        public int Reason { get; set; }

        [BsonElement("user")]
        public LogUserDataModel? User { get; set; }

        [BsonElement("hat")]
        public LogHatDataModel? LogHatDataModel { get; set; }

        [BsonElement("time")]
        public DateTime Time { get; set; }
    }

    [BsonNoId, DataContract]
    public class LogHatDataModel
    {
        [BsonElement("info")]
        public LogHatInfoDataModel? Info { get; set; }

        [BsonElement("result")]
        public LogHatResultDataModel? Result { get; set; }
    }

    [BsonNoId, DataContract]
    public class LogHatInfoDataModel
    {
        [BsonElement("cost")]
        public int Cost { get; set; }

        [BsonElement("Pick")]
        public int Pick { get; set; }
    }

    [BsonNoId, DataContract]
    public class LogHatResultDataModel
    {
        [BsonElement("pick_result")]
        public int PickResult { get; set; }

        [BsonElement("reward")]
        public int Reward { get; set; }
    }
}
