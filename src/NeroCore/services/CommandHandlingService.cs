using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace NeroCore.Services {
    public class CommandHandlingService {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private IServiceProvider _provider;

        public CommandHandlingService(IServiceProvider provider, DiscordSocketClient discord, CommandService commands) {
            _commands = commands;
            _discord = discord;
            _provider = provider;

            _discord.MessageReceived += MessageReceived;
        }

        public async Task InitializeAsync(IServiceProvider provider) {
            _provider = provider;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task MessageReceived(SocketMessage rawMessage) {
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            int argPos = 0;
            if (!(message.HasMentionPrefix(_discord.CurrentUser, ref argPos) || message.HasStringPrefix(Configuration.Load().Prefix, ref argPos))) return;

            var context = new SocketCommandContext(_discord, message);

            //  check to see if its the test server
            if (context.Guild.Id != 286211518691934210 &&
            context.Guild.Id != 141199412423557120) return;

            var result = await _commands.ExecuteAsync(context, argPos, _provider);

            if (result.Error.HasValue && result.Error.Value != CommandError.UnknownCommand) {
                await context.Channel.SendMessageAsync(result.ToString());
            } else if (result.Error.HasValue && result.Error.Value == CommandError.UnknownCommand) {
                
                await context.Channel.SendMessageAsync("Not a valid Command");
            }
        }

        
    }
}