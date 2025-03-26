using System.Collections;
using MongoDB.Driver;

using MongoDB.Driver.Linq;
using System.Globalization;
using MongoAnalytics.Model;
using MongoAnalytics.ModelLog;

namespace MongoAnalytics
{
    public class MongoDB
    {
        private const string ConnectionString = "mongodb://127.0.0.1:27017";
        private IMongoClient? _client = null;

        private readonly DateTime _open = DateTime.Parse("2024-05-07T00:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

        private bool IsOpenDay(DateTime date) =>
            date == DateTime.Parse("2024-05-08T00:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

        public void Open()
        {
            var settings = MongoClientSettings.FromConnectionString(ConnectionString);
            settings.LinqProvider = LinqProvider.V3;
            settings.UseTls = false;
            
            MongoIdentity identity = new MongoInternalIdentity("web", "user");
            MongoIdentityEvidence evidence = new PasswordEvidence("0iGtDYEBLIgoUehuppQz");
            settings.Credential = new MongoCredential("SCRAM-SHA-1", identity, evidence);
            
            _client = new MongoClient(settings);
        }

        public async Task<IDictionary<long, AccountModel>> GetAccount(DateTime date)
        {
            var begin = date;
            var end = begin.AddDays(1);

            var db = _client!.GetDatabase("web");
            var collection = db.GetCollection<LogWebRegAccountModel>("login");

            if (IsOpenDay(date))
            {
                return await GetAccountWithOpenDay(collection, begin, end);
            }

            var ret = await collection.AsQueryable()
                .Where(x => x.Filter == 1)
                .Where(x => x.Time >= begin && x.Time < end).ToListAsync();
            
            return ret.Select(x => new AccountModel() { Auid = x.Auid, Tier = 1 }).ToDictionary(x => x.Auid, x => x);
        }

        public async Task<Dictionary<long, CharacterModel>> GetCharacter(DateTime begin, DateTime end)
        {
            if (IsOpenDay(begin))
                begin = _open;

            var db = _client!.GetDatabase("user");
            var collection = db.GetCollection<LogCharacterModel>("character");

            var query = await collection.AsQueryable()
                .Where(x => x.Filter == 1501)
                .Where(x => x.Time >= begin && x.Time < end)
                .GroupBy(x => x.User!.Cuid).Select(g => g.First())
                .ToListAsync();

            return query.Select(x => new CharacterModel() { Cuid = x.User.Cuid, Auid = x.User.Auid, Name = x.User.Name }).ToDictionary(x => x.Cuid, x => x);
        }

        private async Task<IDictionary<long, AccountModel>> GetAccountWithOpenDay(IMongoCollection<LogWebRegAccountModel> collection, DateTime begin, DateTime end)
        {
            var query = await collection.AsQueryable()
                .Where(x => x.Filter == 1 || x.Filter == 2)
                .Where(x => x.Time >= _open && x.Time < end)
                .ToListAsync();

            var ret = query.GroupBy(x => x.Auid).Select(g => g.First());
            return ret.Select(x => new AccountModel() { Auid = x.Auid, Tier = 1 }).ToDictionary(x => x.Auid, x => x);
        }

        public async Task<IDictionary<long, TierModel>> GetTier(DateTime date)
        {
            var begin = date.Date;
            var end = begin.AddDays(1);

            var db = _client!.GetDatabase("user");
            var collection = db.GetCollection<LogTierModel>("tier");

            var ret = await collection.AsQueryable()
                .Where(x => x.Filter == 1001)
                .Where(x => x.Time >= begin && x.Time < end)
                .GroupBy(x => new { x.User!.Auid })
                .Select(g => new { g.Key.Auid, MaxTier = g.Max(x => x.Tier!.RemainGrade) }) // 각 그룹에서 최대 TIER 값을 찾습니다.
                .ToListAsync();

            return ret.ToDictionary(x => x.Auid, x => new TierModel() { Auid = x.Auid, Tier = x.MaxTier });
        }

        public async Task<Dictionary<int, ShopModel>> GetShopGoods(DateTime date)
        {
            var begin = date.Date;
            var end = begin.AddDays(1);

            var db = _client!.GetDatabase("user");
            var collection = db.GetCollection<LogShopModel>("shop");

            var query = await collection.AsQueryable()
                .Where(x => x.Filter == 3501)
                .Where(x => x.Time >= begin && x.Time < end)
                .Where(x => x.Goods.Id == 40101001 || x.Goods.Id == 40101002 || x.Goods.Id == 40101003 || x.Goods.Id == 40101004 || x.Goods.Id == 40101005)
                .ToListAsync();

            var ret = query
                .GroupBy(x => x.Goods.Id)
                .Select(g => new
                {
                    Id = g.Key,
                    TotalAuid = g.Select(x => x.User.Auid).Distinct().Count(), // Auid의 전체 수를 계산합니다.
                    TotalAmount = g.Sum(x => x.Goods.Amount) // Goods.Amount의 총 합을 계산합니다.
                });

            return ret.ToDictionary(x => x.Id, x => new ShopModel() { Id = x.Id, Qty = x.TotalAmount, TotalAuid = x.TotalAuid });
        }

        public async Task<List<long>> GetDailyLogin(DateTime begin, DateTime end)
        {
            var db = _client!.GetDatabase("user");
            var collection = db.GetCollection<LogUserAccountModel>("account");

            var query = await collection.AsQueryable()
                .Where(x => x.Filter == 503)
                .Where(x => x.Time >= begin && x.Time < end)
                .GroupBy(x => x.User!.Auid)
                .ToListAsync();

            return query.Select(x => x.Key).ToList();
        }

        public async Task<Dictionary<long, PlayModel>> GetTreasureLotteryPlay(DateTime date)
        {
            var begin = date.Date;
            var end = begin.AddDays(1);

            var db = _client!.GetDatabase("user");
            var collection = db.GetCollection<LogTLModel>("treasure_lottery");

            var ret = await collection.AsQueryable()
                .Where(x => x.Filter == 20401)
                .Where(x => x.Time >= begin && x.Time < end)
                .GroupBy(x => new { x.User.Auid })
                .Select(g => new { g.Key.Auid, Count = g.Select(x => x.TLPlay.Pattern).Sum(x => x!.Count) })
                .ToListAsync();

            return ret.ToDictionary(x => x.Auid, x => new PlayModel() { Account = x.Auid, Amount = x.Count * 2000 });
        }

        public async Task<Dictionary<long, PlayModel>> GetHeadAndTailPlay(DateTime date)
        {
            var begin = date.Date;
            var end = begin.AddDays(1);

            var db = _client!.GetDatabase("user");
            var collection = db.GetCollection<LogCurrencyModel>("currency");

            var query = await collection.AsQueryable()
                .Where(x => x.Filter == 2503) // CurrencyDecrease
                .Where(x => x.Time >= begin && x.Time < end)
                .Where(x => x.Reason == 93) // HeadsAndTailsCost
                .Where(x => x.Currency!.Any(i => i.Type == 10))
                .GroupBy(x => new { x.User!.Auid })
                .ToListAsync();

            var ret = query
                .Select(g => new { Auid = g.Key.Auid, Amount = g.Sum(x => x.Currency!.Where(i => i.Type == 10).Sum(i => i.Amount)) });

            return ret.ToDictionary(x => x.Auid, x => new PlayModel() { Account = x.Auid, Amount = x.Amount });
        }


        public async Task<List<CharacterAmountModel>> GetCurrencyIncrease(DateTime begin, DateTime end)
        {
            var db = _client!.GetDatabase("user");
            var collection = db.GetCollection<LogCurrencyModel>("currency");

            var query = collection.AsQueryable()
                .Where(x => x.Filter == 2501)
                .Where(x => x.Time >= begin && x.Time < end)
                .Where(x => x.Reason == 100 || x.Reason == 101 || x.Reason == 102 || x.Reason == 88 || x.Reason == 92)
                .Where(x => x.Currency!.Any(i => i.Type == 10))
                .GroupBy(x => new { x.User.Cuid, x.User.Name })
                .Select(g => new { Cuid = g.Key.Cuid, Name = g.Key.Name, Amount = g.Sum(x => x.Currency!.Where(i => i.Type == 10).Sum(i => i.Amount)) });

            var ret = await query.ToListAsync();

            return ret.Select(x => new CharacterAmountModel() { Cuid = x.Cuid, Name = x.Name, Amount = x.Amount }).ToList();
        }

        public async Task<List<CharacterAmountModel>> GetCurrencyDecrease(DateTime begin, DateTime end)
        {
            var db = _client!.GetDatabase("user");
            var collection = db.GetCollection<LogCurrencyModel>("currency");

            var query = collection.AsQueryable()
                .Where(x => x.Filter == 2503)
                .Where(x => x.Time >= begin && x.Time < end)
                .Where(x => x.Reason == 104 || x.Reason == 105 || x.Reason == 87 || x.Reason == 93)
                .Where(x => x.Currency!.Any(i => i.Type == 10))
                .GroupBy(x => new { x.User.Cuid, x.User.Name })
                .Select(g => new { Cuid = g.Key.Cuid, Name = g.Key.Name, Amount = g.Sum(x => x.Currency!.Where(i => i.Type == 10).Sum(i => i.Amount)) });

            var ret = await query.ToListAsync();

            return ret.Select(x => new CharacterAmountModel() { Cuid = x.Cuid, Name = x.Name, Amount = x.Amount }).ToList();
        }
        
        public async Task<List<long>> GetHoldemPlayCharacter(DateTime date)
        {
            var db = _client.GetDatabase("user");
            var collection = db.GetCollection<LogGameHoldemRecordModel>("holdem_record");

            var query = await collection.AsQueryable()
                .Where(x => x.Filter == 20609 && x.Reason == 105)
                .Where(x => x.Time >= date && x.Time < date.AddDays(1))
                .ToListAsync();

            var ret = query
                    .SelectMany(x => x.Holdem!.HandCard!.SelectMany(i => i))
                    .GroupBy(i => i.Cuid);

            return ret.Select(x => x.Key).ToList();
        }

        public async Task<long> GetHoldemPlayCount(DateTime date)
        {
            var db = _client!.GetDatabase("user");
            var collection = db.GetCollection<LogGameHoldemRecordModel>("holdem_record");

            var query = await collection.AsQueryable()
                .Where(x => x.Filter == 20607)
                .Where(x => x.Time >= date && x.Time < date.AddDays(1))
                .GroupBy(x => x.MatchUid)
                .ToListAsync();

            return query.Count;
        }

        public async Task<long> GetHoldemPlayAmount(DateTime date)
        {
            var db = _client!.GetDatabase("user");
            var collection = db.GetCollection<LogCurrencyModel>("currency");

            var query = await collection.AsQueryable()
                .Where(x => x.Filter == 2503) // CurrencyDecrease
                .Where(x => x.Time >= date && x.Time < date.AddDays(1))
                .Where(x => x.Reason == 104 || x.Reason == 105)// HoldemAnte, HoldemBetting
                .GroupBy(x => x.User!.Auid)
                .Select(g => new
                {
                    Amount = g.Sum(x => x.Currency!.Where(i => i.Type == 10).Sum(i => i.Amount))
                })
                .ToListAsync();
            
            return query.Sum(x => x.Amount);
        }

        public class HoldemWithRuleModel
        {
            public int HoldemId { get; set; }
            public long RoomUid { get; set; }
        }

        public async Task<IEnumerable<HoldemWithRuleModel>> GetHoldemOpenWithRule(DateTime begin, DateTime end)
        {
            var db = _client!.GetDatabase("user");
            var collection = db.GetCollection<LogGameHoldemRecordModel>("holdem_record");

            var query = await collection.AsQueryable()
                .Where(x => x.Filter == 20601)  // HoldemCreateRoom
                .Where(x => x.Time >= begin && x.Time < end)
                .ToListAsync();

            return query.Select(x => new HoldemWithRuleModel() { HoldemId = x.HoldemId, RoomUid = x.RoomUid });
        }

        public async Task<Dictionary<long, int>> GetHoldemPlayCountWithRule(DateTime begin, DateTime end)
        {
            var db = _client!.GetDatabase("user");
            var collection = db.GetCollection<LogGameHoldemRecordModel>("holdem_record");

            var query = await collection.AsQueryable()
                .Where(x => x.Filter == 20607) // 홀덤 종료
                .Where(x => x.Time >= begin && x.Time < end)
                //.Where(x => x.RoomUid == roomUid)
                .ToListAsync();

            var ret = query.GroupBy(x => x.MatchUid);
            return ret.ToDictionary(x => x.Key, x => x.Count());
        }

        public class HoldemDataModel
        {
            public long MatchUid { get; set; }
            public long RoomUid { get; set; }
        }

        public async Task<Dictionary<long, int>> GetHoldemPlayBegin(DateTime begin, DateTime end)
        {
            var db = _client!.GetDatabase("user");
            var collection = db.GetCollection<LogGameHoldemRecordModel>("holdem_record");

            var query = await collection.AsQueryable()
                .Where(x => x.Filter == 20609) // 프리플랍
                .Where(x => x.Reason == 98)    // 카드 배분
                .Where(x => x.Time >= begin && x.Time < end)
                //.Where(x => x.RoomUid == roomUid)
                .ToListAsync();

            var ret = query.GroupBy(x => x.RoomUid);       // MatchUid로 그룹화
            return ret.ToDictionary(x => x.Key, x => x.Count());
        }

        public async Task<Dictionary<long, int>> GetHoldemPlayEnd(DateTime begin, DateTime end)
        {
            var db = _client!.GetDatabase("user");
            var collection = db.GetCollection<LogGameHoldemRecordModel>("holdem_record");

            var query = await collection.AsQueryable()
                .Where(x => x.Filter == 20607)
                .Where(x => x.Time >= begin && x.Time < end)
                //.Where(x => x.RoomUid == roomUid)
                .ToListAsync();

            var ret = query.GroupBy(x => x.RoomUid);
            return ret.ToDictionary(x => x.Key, x => x.Count());
        }

        public async Task<long> GetDailyActiveAccount(DateTime date)
        {
            var begin = date;
            var end = begin.AddDays(1);

            var db = _client!.GetDatabase("web");
            var collection = db.GetCollection<LogWebRegAccountModel>("login");

            if (IsOpenDay(date))
            {
                return (await GetAccountWithOpenDay(collection, begin, end)).Count;
            }

            var ret = await collection.AsQueryable()
                .Where(x => x.Filter == 2)
                .Where(x => x.Time >= begin && x.Time < end)
                .GroupBy(x => x.Auid)
                .ToListAsync();

            return ret.Count;
        }
    }
}
