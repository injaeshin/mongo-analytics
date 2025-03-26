using System.Diagnostics;

namespace MongoAnalytics
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // 검색 기준은 시작일보다 크거나 같고, 종료일보다 작다.
            // begin 05/08 ~ 05/09, 05/09 ~ 05/10, 05/10 ~ 05/11, 05/11 ~ 05/12

            var begin = new DateTime(2024, 5, 8, 0, 0, 0, DateTimeKind.Utc);
            var end = new DateTime(2024, 5, 18, 0, 0, 0, DateTimeKind.Utc);

            var log = new LogService(begin, end);
            Console.WriteLine("Begin!!");

            var sw = new Stopwatch();
            sw.Start();

            //Console.WriteLine("Set Account!!");
            //await log.SetAccount();
            //sw.Stop();
            //Console.WriteLine($"Elapsed Time: {sw.ElapsedMilliseconds}ms");

            //sw.Restart();
            //Console.WriteLine("Daily Active User!!");
            //await log.GetDailyActiveUser();
            //sw.Stop();
            //Console.WriteLine($"Elapsed Time: {sw.ElapsedMilliseconds}ms");

            sw.Restart();
            //Console.WriteLine("Daily Active User!!");
            await log.HoldemWithRulesPlayCount(begin, end);
            sw.Stop();
            Console.WriteLine($"Elapsed Time: {sw.ElapsedMilliseconds}ms");

            //sw.Restart();
            //Console.WriteLine("Set Character!!");
            //await log.SetCharacter();
            //sw.Stop();
            //Console.WriteLine($"Elapsed Time: {sw.ElapsedMilliseconds}ms");

            //sw.Restart();
            //Console.WriteLine("Retention!!");
            //await log.Retention();
            //sw.Stop();
            //Console.WriteLine($"Elapsed Time: {sw.ElapsedMilliseconds}ms");

            //sw.Restart();
            //Console.WriteLine("Daily Tier Level!!");
            //await log.GetDailyTierLevel();
            //sw.Stop();
            //Console.WriteLine($"Elapsed Time: {sw.ElapsedMilliseconds}ms");

            //sw.Restart();
            //Console.WriteLine("Daily Shop!!");
            //await log.GetDailyShop();
            //sw.Stop();
            //Console.WriteLine($"Elapsed Time: {sw.ElapsedMilliseconds}ms");

            //sw.Restart();
            //Console.WriteLine("Game Play!!");
            //await log.GamePlay();
            //sw.Stop();
            //Console.WriteLine($"Elapsed Time: {sw.ElapsedMilliseconds}ms");

            //sw.Restart();
            //Console.WriteLine("Content Net Income Top 50!!");
            //DateTime b = new DateTime(2024, 5, 12, 0, 0, 0, DateTimeKind.Utc);
            //DateTime e = new DateTime(2024, 5, 14, 0, 0, 0, DateTimeKind.Utc);
            //await log.GetContentNetIncomeTop50(b, e);
            //sw.Stop();
            //Console.WriteLine($"Elapsed Time: {sw.ElapsedMilliseconds}ms");

            Console.WriteLine("End!!");
        }
    }
}
