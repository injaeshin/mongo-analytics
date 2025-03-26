using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.Serialization;
using MongoDB.Bson.Serialization;

namespace MongoAnalytics.ModelLog
{
    public static class LogTLMapper
    {
        public static void Init()
        {
            BsonClassMap.RegisterClassMap<LogTLModel>(classMap =>
            {
                classMap.AutoMap();
                classMap.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<LogTLDataModel>(classMap =>
            {
                classMap.AutoMap();
                classMap.SetIgnoreExtraElements(true);
            });
        }
    }

    [BsonNoId, DataContract]
    public class LogTLModel
    {
        [BsonElement("filter")]
        public int Filter { get; set; }

        [BsonElement("user")]
        public LogUserDataModel? User { get; set; }

        [BsonElement("tl")]
        public LogTLDataModel? TLPlay { get; set; }

        [BsonElement("time")]
        public DateTime Time { get; set; }
    }

    [BsonNoId, DataContract]
    public class LogTLDataModel
    {
        [BsonElement("pattern")]
        public List<int>? Pattern { get; set; }
    }

}
