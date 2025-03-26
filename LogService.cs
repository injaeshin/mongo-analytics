using MongoAnalytics.Model;
using MongoAnalytics.ModelLog;

namespace MongoAnalytics
{
    public class LogService
    {
        private Dictionary<DateTime, Dictionary<long, AccountModel>> _accountDaily = new();
        private Dictionary<long, CharacterModel> _character = new();
        private MongoDB _db = new();
        
        private readonly DateTime _start; 
        private readonly DateTime _end;

        public LogService(DateTime begin, DateTime end)
        {
            _db.Open();
            LogMapper.Init();
            LogTLMapper.Init();
            LogHatMapper.Init();
            LogHoldemMapper.Init();

            _start = begin;
            _end = end;
        }

        // 전체 날짜별 계정 정보를 획득
        public async Task SetAccount()
        {
            var end = _end;

            for (DateTime date = _start; date < end; date = date.AddDays(1))
            {
                var account = await _db.GetAccount(date);
                
                account.Remove(0);
                if (account.Count == 0)
                {
                    throw new Exception($"Account not found on {date:yyyy-MM-dd}");
                }

                _accountDaily.Add(date, account.ToDictionary());
            }

            Console.WriteLine($"Account - {_accountDaily.Values.Select(i => i.Values.Count).Sum()}");
        }

        public async Task SetCharacter()
        {
            var end = _end;

            _character = await _db.GetCharacter(_start, end);
            if (_character.Count == 0)
            {
                throw new Exception("Character not found");
            }

            Console.WriteLine($"Character - {_character.Count}");
        }

        // 해당 날짜별 Account 정보를 획득
        private Dictionary<long, AccountModel> GetDailyAccount(DateTime begin, DateTime end)
        {
            var account = new Dictionary<long, AccountModel>();
            for (DateTime date = begin; date < end; date = date.AddDays(1))
            {
                if (!_accountDaily.ContainsKey(date))
                {
                    throw new Exception($"Account not found on {date:yyyy-MM-dd}");
                }

                account = account.Concat(_accountDaily[date]).ToDictionary(x => x.Key, x => x.Value);
            }

            return account;
        }

        public async Task GetDailyTierLevel()
        {
            var end = _end;

            if (_accountDaily.Count == 0)
            {
                Console.WriteLine("No account found");
                return;
            }

            for (DateTime date = _start; date < end; date = date.AddDays(1))
            {
                var nextDay = date.AddDays(1);
                var account = GetDailyAccount(_start, nextDay);
                await SetTierLevel(account, _start, nextDay);

                Output(account, date);
            }
        }

        private async Task SetTierLevel(Dictionary<long, AccountModel> account, DateTime begin, DateTime end)
        {
            for (DateTime date = begin; date < end; date = date.AddDays(1))
            {
                var tier = await _db.GetTier(date);
                foreach (var tm in tier)
                {
                    if (!account.ContainsKey(tm.Key))
                    {
                        throw new Exception($"Account {tm.Key} not found");
                    }

                    account[tm.Key].Tier = tm.Value.Tier;
                }
            }
        }

        private void Output(Dictionary<long, AccountModel> account, DateTime start)
        {
            var tierLevel = account.GroupBy(x => x.Value.Tier).ToDictionary(x => x.Key, x => x.Count());
            foreach (var kv in tierLevel)
            {
                Console.WriteLine($"{start:yyyy-MM-dd} Tier {kv.Key} : {kv.Value}");
            }
        }

        public async Task GetDailyShop()
        {
            var start = _start;
            var end = _end;

            List<int> goods = [40101001, 40101002, 40101003, 40101004, 40101005];

            for (var date = start; date < end; date = date.AddDays(1))
            {
                var s = await _db.GetShopGoods(date);

                foreach (var g in goods)
                {
                    if (!s.ContainsKey(g))
                    {
                        Console.WriteLine($"{date:yyyy-MM-dd} Goods id {g} : Amount {0} / total auid {0}");
                        continue;
                    }

                    var sm = s[g];
                    Console.WriteLine($"{date:yyyy-MM-dd} Goods id {sm.Id} : Amount {sm.Qty} / total auid {sm.TotalAuid}");
                }

                Console.WriteLine("------------");
            }
        }

        public async Task GamePlay()
        {
            var start = _start;
            var end = _end;
            
            for (var date = start; date < end; date = date.AddDays(1))
            {
                var h = await _db.GetHoldemPlayCharacter(date);
                foreach (var c in h)
                {
                    if (!_character.ContainsKey(c))
                    {
                        Console.WriteLine($"Character {c} not found");
                    }
                }

                var playAccount = h.Where(i => _character.ContainsKey(i)).Select(x => _character[x].Auid).ToList();
                var playAmount = await _db.GetHoldemPlayAmount(date);
                var playCount = await _db.GetHoldemPlayCount(date);
                Console.WriteLine($"{date:yyyy-MM-dd} HoldemPlay - Account {playAccount.Count}, Amount {playAmount}, Count {playCount}");

                var f = await _db.GetTreasureLotteryPlay(date);
                Console.WriteLine($"{date:yyyy-MM-dd} TLPlay - Account {f.Count} : {f.Values.Select(x => x.Amount).Sum()}");

                var headAndTailPlay = await _db.GetHeadAndTailPlay(date);
                Console.WriteLine($"{date:yyyy-MM-dd} HatPlay - Account {headAndTailPlay.Count} : {headAndTailPlay.Values.Select(x => x.Amount).Sum()}");

                Console.WriteLine("------------");
            }
        }

        public async Task Retention()
        {
            var start = _start;
            var end = _end;

            for (var date = start; date < end; date = date.AddDays(1))
            {
                var retention = await GetDailyLogin(date, end);

                Console.WriteLine($"Retention : {date:d} {retention.Item1} - {string.Join(", ", retention.Item2)}");
            }
        }

        private async Task<Tuple<int, List<int>>> GetDailyLogin(DateTime begin, DateTime end)
        {
            var account = GetDailyAccount(begin, begin.AddDays(1));

            List<int> retention = [ 100 ];

            for (DateTime date = begin.AddDays(1); date < end; date = date.AddDays(1))
            {
                var dailyLoginAccount = await _db.GetDailyLogin(date, date.AddDays(1));
                var cnt = dailyLoginAccount.Intersect(account.Keys).Count();
                retention.Add(cnt * 100 / account.Count);
            }

            return new Tuple<int, List<int>>(account.Count, retention);
        }

        public async Task GetContentNetIncomeTop50(DateTime begin, DateTime end)
        {
            var inc = await _db.GetCurrencyIncrease(begin, end);
            var incDict = inc.ToDictionary(x => x.Cuid, x => x);
            
            var dec = await _db.GetCurrencyDecrease(begin, end);
            var decDict = dec.ToDictionary(x => x.Cuid, x => x);

            List<CharacterAmountModel> diff = new();
            
            foreach (var i in incDict)
            {
                if (!decDict.ContainsKey(i.Key))
                {
                    Console.WriteLine($"{i.Value.Cuid}|{i.Value.Name}|{i.Value.Amount}");
                    continue;
                }

                diff.Add(new()
                {
                    Cuid = i.Value.Cuid,
                    Name = i.Value.Name,
                    Amount = i.Value.Amount - decDict[i.Key].Amount
                });
            }

            diff = diff.OrderByDescending(x => x.Amount).Take(50).ToList();
            foreach (var d in diff)
            {
                Console.WriteLine($"{d.Cuid}|{d.Name}|{d.Amount}");
            }
        }

        public async Task HoldemWithRulesPlayCount(DateTime begin, DateTime end)
        {
            for (DateTime date = begin; date < end; date = date.AddDays(1))
            {
                // 210101	211001	211002	211003	211004	211011	211012
                var playCount = new Dictionary<int, long>()
                {
                    { 210101, 0 },
                    { 211001, 0 },
                    { 211002, 0 },
                    { 211003, 0 },
                    { 211004, 0 },
                    { 211011, 0 },
                    { 211012, 0 }
                };

                var holdemPlay = await _db.GetHoldemOpenWithRule(date, date.AddDays(1));
                var hcnt = holdemPlay.ToList().Count;
                Console.WriteLine($"{date:yyyy-MM-dd} HoldemOpen - Holdem {hcnt}");


                var endGame = await _db.GetHoldemPlayEnd(date, date.AddDays(1));
                if (endGame.Count == 0)
                {
                    continue;
                }

                foreach (var h in holdemPlay)
                {
                    if (h.RoomUid == 0)
                    {
                        throw new Exception($"Holdem {h.HoldemId} not found");
                    }

                    if (!endGame.ContainsKey(h.RoomUid))
                    {
                        continue;
                    }

                    playCount[h.HoldemId] += endGame[h.RoomUid];
                }

                Console.WriteLine($"{date:yyyy-MM-dd}");
                foreach (var t in playCount)
                {
                    Console.Write($"{t.Key} : {t.Value} / ");
                }
                Console.WriteLine();
            }

            Console.WriteLine("------------");
        }
    }
}
