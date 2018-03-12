using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NeroLib
{
    public class fflogs
    {
        NeroLib.dbsql.DBAccess dba = new NeroLib.dbsql.DBAccess();

        public NeroLib.User LoadUser(NeroLib.User user) {
            if (dba.CheckIfUserExists(user)) {
                return dba.ReturnUser(user.UserId);
            } else {
                return null;
            }
        }

        public async Task GetParse(NeroLib.User argUser)
        {
            NeroLib.User user = LoadUser(argUser);
            NeroLib.World requestedWorld = dba.ReturnWorld(user.WorldId);


            Console.WriteLine($"Username: {user.Username}\nDiscordID: {user.UserId}\nCharacter Name: {user.Name}\nCharacter World: {user.World}");

            var SavageUrl = new Uri($"https://www.fflogs.com/v1/parses/character/{user.Name}/{requestedWorld.WorldName}/{requestedWorld.Region}/?api_key={LibConfiguration.Load().FFLogsKey}");

            var TrialUrl = new Uri($"https://www.fflogs.com/v1/parses/character/{user.Name}/{requestedWorld.WorldName}/{requestedWorld.Region}/?api_key={LibConfiguration.Load().FFLogsKey}&zone=15");

            var UltimateUrl = new Uri($"https://www.fflogs.com/v1/parses/character/{user.Name}/{requestedWorld.WorldName}/{requestedWorld.Region}/?api_key={LibConfiguration.Load().FFLogsKey}&zone=19");

            Console.WriteLine($"FFLogs API URL: {SavageUrl}");
            await LogParse(user, SavageUrl);
            await LogParse(user, TrialUrl);
            await LogParse(user, UltimateUrl);
        }

        public string GetListParse(NeroLib.User argUser)
        {   
            //await GetParse(argUser);
            var user = argUser;
            user = LoadUser(user);

            var reply = $"";

            var pCollection = user.Parses.OrderBy(p => p.Percent).Reverse().Take(10);

            foreach (var p in pCollection) {
                reply += $"\n**{p.JobAbrv.ToUpper()} | {p.Name}**: {p.Percent.ToString().Substring(0,5)}% - {p.PerSecondAmount}";
            }

            return reply;
        }

        private async Task LogParse(User user, Uri url)
        {
            var savageBody = await returnResponseBlob(url);

            JArray ffSavageResponse = JArray.Parse(savageBody);

            if (ffSavageResponse == null || ffSavageResponse.ToString() == "[]")
            {
                Console.WriteLine("Response Empty.\n");
                Console.WriteLine("[GetParse] No Response");
            }

            IterateThroughFFlogsJSON(user, ffSavageResponse);
        }

        private void IterateThroughFFlogsJSON(User requestedUser, JArray ffResponse)
        {
            var fightsearch =
                from f in ffResponse.Children()
                select f;

            foreach (var fight in fightsearch)
            {
                var jobSearch =
                from s in fight["specs"]
                select s;

                foreach (var job in jobSearch)
                {
                    Console.WriteLine($"spec: {job["spec"]}");

                    var encounters =
                        from e in job["data"].Children()
                        select e;

                    foreach (var encounter in encounters)
                    {
                        var perSeconAmount = Double.Parse(encounter["persecondamount"].ToString());
                        var percent = Double.Parse(encounter["percent"].ToString());
                        var historicCount = Double.Parse(encounter["historical_count"].ToString());
                        var historicPercent = Double.Parse(encounter["historical_percent"].ToString());
                        Parse parse = CreateParse(requestedUser, fight, job, encounter, perSeconAmount, percent, historicCount, historicPercent);
                        if (dba.CheckIfParseExists(parse) == true) {
                            dba.UpdateParse(parse);
                        } else {
                            dba.AddParseToDB(parse); 
                        }
                    }
                }
            }
        }

        private Parse CreateParse(User requestedUser, JToken fight, JToken job, JToken encounter, double perSeconAmount, double percent, double historicCount, double historicPercent)
        {
            return new NeroLib.Parse
            {
                ParseId = encounter["ranking_id"].ToString(),
                UserId = requestedUser.UserId,
                Difficulty = fight["difficulty"].ToString(),
                JobName = job["spec"].ToString().ToLower(),
                JobAbrv = dba.ReturnAbbreviation(job["spec"].ToString().ToLower()),
                Size = fight["size"].ToString(),
                Kill = fight["kill"].ToString(),
                Name = fight["name"].ToString(),
                PerSecondAmount = perSeconAmount,
                Percent = percent,
                HistoricalCount = historicCount,
                HistoricalPercent = historicPercent,
                ServerId = requestedUser.ServerId
            };
        }

        public async Task<string> returnResponseBlob(Uri url) {
            HttpClient client = HTTPHelpers.NewClient();
            string responseBody = await client.GetStringAsync(url);
            return responseBody;
        }

        public NeroLib.World returnWorldEnt(string servname, string dc, string reg) {
            var worldToAdd = new NeroLib.World {
                WorldName = servname,
                DataCenter = dc,
                Region = reg,
                Population = 0
            };
            return worldToAdd;
        }

        public List<NeroLib.World> GetWorlds() {
            var worlds = new List<NeroLib.World>();
            
            // -------------
            // | Primal 11 |
            // -------------
            worlds.Add(returnWorldEnt("Behemoth", "Primal", "NA"));
            worlds.Add(returnWorldEnt("Brynhildr", "Primal", "NA"));
            worlds.Add(returnWorldEnt("Diabolos", "Primal", "NA"));
            worlds.Add(returnWorldEnt("Excalibur", "Primal", "NA"));
            worlds.Add(returnWorldEnt("Exodus", "Primal", "NA"));
            worlds.Add(returnWorldEnt("Famfrit", "Primal", "NA"));
            worlds.Add(returnWorldEnt("Hyperion", "Primal", "NA"));
            worlds.Add(returnWorldEnt("Lamia", "Primal", "NA"));
            worlds.Add(returnWorldEnt("Leviathan", "Primal", "NA"));
            worlds.Add(returnWorldEnt("Malboro", "Primal", "NA"));
            worlds.Add(returnWorldEnt("Ultros", "Primal", "NA"));

            // ----------------
            // | Elemental 10 | 21
            // ----------------
            worlds.Add(returnWorldEnt("Aegis", "Elemental", "JP"));
            worlds.Add(returnWorldEnt("Atomos", "Elemental", "JP"));
            worlds.Add(returnWorldEnt("Carbuncle", "Elemental", "JP"));
            worlds.Add(returnWorldEnt("Garuda", "Elemental", "JP"));
            worlds.Add(returnWorldEnt("Gungnir", "Elemental", "JP"));
            worlds.Add(returnWorldEnt("Kujata", "Elemental", "JP"));
            worlds.Add(returnWorldEnt("Ramuh", "Elemental", "JP"));
            worlds.Add(returnWorldEnt("Tonberry", "Elemental", "JP"));
            worlds.Add(returnWorldEnt("Typhon", "Elemental", "JP"));
            worlds.Add(returnWorldEnt("Unicorn", "Elemental", "JP"));

            // ------------
            // | Chaos 10 | 31
            // ------------
            worlds.Add(returnWorldEnt("Cerberus", "Chaos", "EU"));
            worlds.Add(returnWorldEnt("Lich", "Chaos", "EU"));
            worlds.Add(returnWorldEnt("Louisoix", "Chaos", "EU"));
            worlds.Add(returnWorldEnt("Moogle", "Chaos", "EU"));
            worlds.Add(returnWorldEnt("Odin", "Chaos", "EU"));
            worlds.Add(returnWorldEnt("Omega", "Chaos", "EU"));
            worlds.Add(returnWorldEnt("Phoenix", "Chaos", "EU"));
            worlds.Add(returnWorldEnt("Ragnarok", "Chaos", "EU"));
            worlds.Add(returnWorldEnt("Shiva", "Chaos", "EU"));
            worlds.Add(returnWorldEnt("Zodiark", "Chaos", "EU"));

            // -----------
            // | Gaia 11 | 42
            // -----------
            worlds.Add(returnWorldEnt("Alexander", "Gaia", "JP"));
            worlds.Add(returnWorldEnt("Bahamut", "Gaia", "JP"));
            worlds.Add(returnWorldEnt("Durandal", "Gaia", "JP"));
            worlds.Add(returnWorldEnt("Fenrir", "Gaia", "JP"));
            worlds.Add(returnWorldEnt("Ifrit", "Gaia", "JP"));
            worlds.Add(returnWorldEnt("Ridill", "Gaia", "JP"));
            worlds.Add(returnWorldEnt("Tiamat", "Gaia", "JP"));
            worlds.Add(returnWorldEnt("Ultima", "Gaia", "JP"));
            worlds.Add(returnWorldEnt("Valefor", "Gaia", "JP"));
            worlds.Add(returnWorldEnt("Yojimbo", "Gaia", "JP"));
            worlds.Add(returnWorldEnt("Zeromus", "Gaia", "JP"));

            // -----------
            // | Mana 11 | 53
            // -----------
            worlds.Add(returnWorldEnt("Anima", "Mana", "JP"));
            worlds.Add(returnWorldEnt("Asura", "Mana", "JP"));
            worlds.Add(returnWorldEnt("Belias", "Mana", "JP"));
            worlds.Add(returnWorldEnt("Chocobo", "Mana", "JP"));
            worlds.Add(returnWorldEnt("Hades", "Mana", "JP"));
            worlds.Add(returnWorldEnt("Ixion", "Mana", "JP"));
            worlds.Add(returnWorldEnt("Mandragora", "Mana", "JP"));
            worlds.Add(returnWorldEnt("Masamune", "Mana", "JP"));
            worlds.Add(returnWorldEnt("Pandaemonium", "Mana", "JP"));
            worlds.Add(returnWorldEnt("Shinryu", "Mana", "JP"));
            worlds.Add(returnWorldEnt("Titan", "Mana", "JP"));

            // -------------
            // | Aether 13 | 66
            // -------------
            worlds.Add(returnWorldEnt("Adamantoise", "Aether", "NA"));
            worlds.Add(returnWorldEnt("Balmung", "Aether", "NA"));
            worlds.Add(returnWorldEnt("Cactuar", "Aether", "NA"));
            worlds.Add(returnWorldEnt("Coeurl", "Aether", "NA"));
            worlds.Add(returnWorldEnt("Faerie", "Aether", "NA"));
            worlds.Add(returnWorldEnt("Gilgamesh", "Aether", "NA"));
            worlds.Add(returnWorldEnt("Goblin", "Aether", "NA"));
            worlds.Add(returnWorldEnt("Jenova", "Aether", "NA"));
            worlds.Add(returnWorldEnt("Mateus", "Aether", "NA"));
            worlds.Add(returnWorldEnt("Midgardsormr", "Aether", "NA"));
            worlds.Add(returnWorldEnt("Sargatanas", "Aether", "NA"));
            worlds.Add(returnWorldEnt("Siren", "Aether", "NA"));
            worlds.Add(returnWorldEnt("Zalera", "Aether", "NA"));

            return worlds;
        }
    }

}
