using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.Serialization;

namespace MongoAnalytics.ModelLog
{
    public static class LogHoldemMapper
    {
        public static void Init()
        {
            BsonClassMap.RegisterClassMap<LogGameHoldemRecordModel>(classMap =>
            {
                classMap.AutoMap();
                classMap.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<LogGameHoldemDataModel>(classMap =>
            {
                classMap.AutoMap();
                classMap.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<LogGameHandCardDataModel>(classMap =>
            {
                classMap.AutoMap();
                classMap.SetIgnoreExtraElements(true);
            });
        }
    }

    [BsonNoId, DataContract]
    public class LogGameHoldemRecordModel
    {
        [BsonElement("filter")]
        public int Filter { get; set; }

        [BsonElement("reason")]
        public int Reason { get; set; }

        [BsonElement("holdem_id")]
        public int HoldemId { get; set; }

        [BsonElement("user")]
        public LogUserDataModel? User { get; set; }

        [BsonElement("holdem")]
        public LogGameHoldemDataModel? Holdem { get; set; }

        [BsonElement("match_uid")]
        public long MatchUid { get; set; }

        [BsonElement("room_uid")]
        public long RoomUid { get; set; }

        [BsonElement("time")]
        public DateTime Time { get; set; }
    }

    [BsonNoId, DataContract]
    public class LogGameHoldemDataModel
    {
        [BsonElement("hand_card")]
        public List<List<LogGameHandCardDataModel>> HandCard { get; set; } = new();
    }


    [BsonNoId, DataContract]
    public class LogGameHandCardDataModel
    {
        [BsonElement("cuid")]
        public long Cuid { get; set; }

        [BsonElement("num")]
        public int Num { get; set; }

        [BsonElement("suit")]
        public string Suit { get; set; } = string.Empty;
    }


}
