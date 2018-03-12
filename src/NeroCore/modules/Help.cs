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

public class Help : ModuleBase {
    private readonly CommandService _commands;
    private readonly IServiceProvider _map;

    public Help(IServiceProvider map, CommandService commands) {
        _commands = commands;
        _map = map;
    }

    [Command("help")]
    [Alias("h")]
    [Summary("Lists Commands & related info.")]
    public async Task HelpCommand(string module = "", string command = "") {
        // Todo: Replace path variable with module and command strings
        EmbedBuilder output = new EmbedBuilder();
        if (module == ""){
            output.Title = "Cid - Help";

            foreach (var mod in _commands.Modules.Where(m => m.Parent == null)) {
                AddHelp(mod, ref output);
            }

            output.Footer = new EmbedFooterBuilder {
                Text = "Use '!n Help <module> <command>' to get help with a command."
            };
        } else {
            var mod = _commands.Modules.FirstOrDefault(m => m.Name.Replace("Module", "").ToLower() == module.ToLower());
            if (mod == null) { await ReplyAsync("No module could be found with that name."); return; }
            
            output.Title = $"__{mod.Name}__";
            output.Description = $"{mod.Summary}\n" +
            (!string.IsNullOrEmpty(mod.Remarks) ? $"({mod.Remarks})\n" : "") +
            (mod.Aliases.Any(a => a != "") ? $"*Prefix(es)*: {string.Join(",", mod.Aliases)}\n" : "");

            if (command == "") {
                
                output.Description += 
                    (mod.Submodules.Any(s => s != null) ? $"Submodules: {mod.Submodules.Select(m => m.Name)}\n" : "") + " ";
                    AddCommands(mod, ref output);
            } else {
                List<CommandInfo> cmds = new List<CommandInfo>();                                   // Counts how many overloaded commands there are

                foreach (var comm in mod.Commands) {
                    if (comm.Name.ToLower().Contains(command.ToLower())) {
                        if (comm.Name.ToLower() != "worldadd")
                            cmds.Add(comm);
                    }
                }
                foreach( var cmd in cmds) {
                    AddCommand(cmd, ref output);
                }
            }

            
        }

        await ReplyAsync("", embed: output.Build());
    }

    public void AddHelp(ModuleInfo module, ref EmbedBuilder builder)
    {
        foreach (var sub in module.Submodules) AddHelp(sub, ref builder);
        builder.AddField(f =>
        {
            f.Name = $"**__{module.Name}__**";
            f.Value = (module.Submodules.Any(s => s != null) ? $"Submodules: {module.Submodules.Select(m => m.Name)}\n" : "") +
            $"\n" +
            $"Commands: {string.Join(", ", module.Commands.Where(x => x.Name.ToLower() != "worldadd").Select(x => $"`{x.Name}`"))}" +
            $"\n \u2063";
            });
    }

    public void AddCommands(ModuleInfo module, ref EmbedBuilder builder)
    {
        foreach (var command in module.Commands)
        {
            if (command.Name.ToLower() != "worldadd") {
                command.CheckPreconditionsAsync(Context, _map).GetAwaiter().GetResult();
                AddCommand(command, ref builder);
            }
        }

    }

    public void AddCommand(CommandInfo command, ref EmbedBuilder builder)
    {
        builder.AddField(f =>
        {
            f.Name = $"**{command.Name}**";
            f.Value = $"{command.Summary}\n" +
            (!string.IsNullOrEmpty(command.Remarks) ? $"({command.Remarks})\n" : "") +
            (command.Aliases.Any() ? $"Aliases:  {string.Join(", ", command.Aliases.Select(x => $"`{x}`"))}\n" : "") +
            $"Usage:  `{GetPrefix(command)} {GetAliases(command)}`" +
            $"\n \u2063";
            });
    }

     public string GetAliases(CommandInfo command)
    {
        StringBuilder output = new StringBuilder();
        if (!command.Parameters.Any()) return output.ToString();
        foreach (var param in command.Parameters)
        {
            if (command.Name == "help") {
                output.Append($"<{param}> ");
            } else {
                if (param.IsOptional)
                    output.Append($"[{param.Name} = {param.DefaultValue}] ");
                else if (param.IsMultiple)
                    output.Append($"|{param.Name}| ");
                else if (param.IsRemainder)
                    output.Append($"{param.Name} ");
                else
                    output.Append($"<{param.Name}> ");
            }
            
        }
        return output.ToString();
    }
    public string GetPrefix(CommandInfo command)
    {
        var output = GetPrefix(command.Module);
        output += $"{command.Aliases.FirstOrDefault()} ";
        return output;
    }
    public string GetPrefix(ModuleInfo module)
    {
        string output = "";
        if (module.Parent != null) output = $"{GetPrefix(module.Parent)}{output}";
        if (module.Aliases.Any())
        output += string.Concat(module.Aliases.FirstOrDefault(), " ");
        return output;
    }

}