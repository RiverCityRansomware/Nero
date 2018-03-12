using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NeroLib.dbsql {
    public class DBAccess {

        public void AddParseToDB(NeroLib.Parse parse) {
            using (var db = new UsersContext()) {
                db.Parses.Add(parse);
                db.SaveChanges();
            }
        }

        public void AddServerToDB(NeroLib.Server server) {
            using (var db = new UsersContext()) {
                db.Servers.Add(server);
                db.SaveChanges();
            }
        }

        public void AddUserToDB(NeroLib.User user) {
            using (var db = new UsersContext()) {
                db.Users.Add(user);
                db.SaveChanges();
            }
        }

       public void AddWorldToDB(NeroLib.World worl) {
            using (var db = new UsersContext()) {
                db.Worlds.Add(worl);
                db.SaveChanges();
            }
        }

        public void UpdateParse(NeroLib.Parse parse) {
            using (var db = new UsersContext()) {
                db.Parses.Remove(parse);
                db.SaveChanges();
                AddParseToDB(parse);
            }
        }

        public void UpdateUser(NeroLib.User user) {
            using (var db = new UsersContext()) {  
                db.Users.Remove(user);
                db.SaveChanges();
                AddUserToDB(user);
            }
        }

        public bool CheckIfParseExists(NeroLib.Parse parse) {
            using (var db = new UsersContext()) {
                if (db.Parses.Any(p => p.ParseId == parse.ParseId)) {
                    return true;
                } else {
                    return false;
                }
            }
        }
        
        public bool CheckIfServerExists(NeroLib.Server server) {
            using (var db = new UsersContext()) {
                if (db.Servers.Any(s => s.ServerId == server.ServerId)) {
                    return true;
                } else {
                    return false;
                }
            }
        }

        public bool CheckIfUserExists(NeroLib.User user) {
            using (var db = new UsersContext()) {
                if (db.Users.Any(u => u.UserId == user.UserId)) {
                    return true;
                } else {
                    return false;
                }
            }
        }

        public void CountWorldPopulation(NeroLib.User user) {
            using (var db = new UsersContext()) {
                var worlds = db.Worlds
                                //.Where(w => w.WorldId == user.WorldId)
                                .Include(w => w.Users);
                                //.FirstOrDefault();

                foreach (var world in worlds) {
                    world.Population = 0;
                    world.Population += (uint)world.Users.Count();
                    db.Worlds.Update(world);
                }
                
                db.SaveChanges();
            }
        }
        
        public NeroLib.User ReturnUser(ulong ID) {
            using (var db = new UsersContext()) {
                var searchResult = db.Users
                                        .Where(u => u.UserId == ID)
                                        .Include(u => u.World)
                                        .Include(u => u.Parses)
                                        .FirstOrDefault();  

                if (searchResult != null) {
                    return searchResult;
                } else {
                    return null;
                }
            }
        }

        public NeroLib.World ReturnWorld(string name) {
            using (var db = new UsersContext()) {
                var worldSearchResult = from w in db.Worlds
                    where w.WorldName.ToLower() == name.ToLower()
                    select w;

                var worldResult = worldSearchResult.First();

                if (worldResult != null) {
                    return worldResult;
                } else {
                    return null;
                }
            }
        }

        public NeroLib.World ReturnWorld(int id) {
            using (var db = new UsersContext()) {
                var worldSearchResult = from w in db.Worlds
                    where w.WorldId == id
                    select w;

                var worldResult = worldSearchResult.First();

                if (worldResult != null) {
                    return worldResult;
                } else {
                    return null;
                }
            }
        }


        public string ReturnAbbreviation(string job) {
            switch(job.ToLower()) {
                case "whitemage": 
                    return "whm";
                case "redmage":
                    return "rdm";
                case "blackmage":
                    return "blm";
                case "samurai":
                    return "sam";
                case "bard":
                    return "brd";
                case "warrior":
                    return "war";
                case "paladin":
                    return "pld";
                case "dragoon":
                    return "drg";
                case "scholar":
                    return "sch";
                case "summoner":
                    return" smn";
                case "machinist":
                    return "mch";
                case "monk":
                    return "mnk";
                case "darkknight":
                    return "drk";
                case "ninja":
                    return "nin";
                case "astrologian":
                    return "ast";
                default:
                    return null;
            }
        }

    }
}
