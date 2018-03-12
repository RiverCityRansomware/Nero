using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NeroLib;
using NeroLib.dbsql;


namespace NeroCore {

    public class FFlogs : ModuleBase {

        NeroLib.dbsql.DBAccess dba = new NeroLib.dbsql.DBAccess();
        NeroLib.fflogs ff = new NeroLib.fflogs();

        [Command("add")]
        [Alias("a")]
        [Summary("Adds the specified character to the system.")]
        public async Task AddProfile(string server, [Remainder] string characterName) {
            NeroLib.World worldTemp = dba.ReturnWorld(server);
            if (worldTemp == null) {
                await ReplyAsync("World not found. Did you input the correct one?");
                return;
            }

            var g = Context.Guild as SocketGuild;

            var userServer = new NeroLib.Server {
                ServerId = Context.Guild.Id,
                Name = Context.Guild.Name,
                Population = g.MemberCount
            };

            if (!dba.CheckIfServerExists(userServer)) {
                dba.AddServerToDB(userServer);
            }

            var userToAdd = new NeroLib.User { 
                UserId = Context.User.Id, 
                Username = Context.User.Username.ToLower(),
                Name = characterName.ToLower(),
                WorldId = worldTemp.WorldId,
                ServerId = userServer.ServerId
            };

            if (dba.CheckIfUserExists(userToAdd) == true) {
                dba.UpdateUser(userToAdd);
                await ReplyAsync("Profile Updated");
            } else {
                dba.AddUserToDB(userToAdd);
                await ReplyAsync("User Added.");
            }
            dba.CountWorldPopulation(userToAdd);
            await ff.GetParse(ff.LoadUser(userToAdd));
        }

        [Command("worldadd")]
        [Alias("w")]
        [Summary("Add worlds to db")]
        public async Task AddWorlds() {
            List<NeroLib.World> worlds = ff.GetWorlds();

            foreach (var wor in worlds) {
                dba.AddWorldToDB(wor);
            }

            await ReplyAsync("Worlds Added.");
        }

        [Command("list")]
        [Alias("l")]
        [Summary("Lists your profile's top 10 clears")]
        public async Task ListClears() {
            var user = dba.ReturnUser(Context.User.Id);
            if (user == null) {
                await ReplyAsync("You have not added yourself to the system yet.\nPlease add yourself with the command `!n a <server> <character name>`");
                return;
            }

            var clears = ff.GetListParse(user);

            var embed = new EmbedBuilder();
            embed
            .WithTitle($"{user.Name.ToUpperInvariant()} - {user.World.WorldName.ToUpperInvariant()}").WithAuthor("<Job> | <Fight>: <Percentage compared to others> - <average dps>")
            
            .WithDescription(clears);
            await ReplyAsync("", embed: embed.Build());
        }

        [Command("list")]
        [Alias("l")]
        [Summary("Lists mentioned user's top 10 clears")]
        public async Task ListOtherClears(IUser serverUser) {
            var user = dba.ReturnUser(serverUser.Id);
            if (user == null) {
                await ReplyAsync("You have not added yourself to the system yet.\nPlease add yourself with the command `!n a <server> <character name>`");
                return;
            }
            
            var parses = ff.GetListParse(user);
            
            var embed = new EmbedBuilder();
            embed
            .WithTitle($"{user.Name.ToUpperInvariant()} - {user.World.WorldName.ToUpperInvariant()}")
            .WithDescription(parses);

            await ReplyAsync("", embed: embed.Build());
        }


        

    }
}