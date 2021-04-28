namespace mölndal_chan_v2
{
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("motivation")]
        public async Task MotivationAsync()
        {
            IChannel server_channel = Context.Channel;
            var users = server_channel.GetUsersAsync().FlattenAsync();
            var user_list = users.Result.ToArray();
            var rnd = new Random();

            var rendom = rnd.Next(user_list.Length);
            await ReplyAsync(user_list[rendom].Mention + Program.motivation_mental_text[rnd.Next(Program.motivation_mental_text.Length)]);

            await Context.Channel.DeleteMessageAsync(Context.Message);
            debug.ConsolePrint.Print(Context.Guild, "motivation");
        }
        [Command("addrole")]
        [RequireUserPermission(Discord.ChannelPermission.ManageRoles)]
        public async Task AddRoleJoinAsync()
        {
            await FixRoleAsync(Context, true);
        }
        [Command("removerole")]
        [RequireUserPermission(Discord.ChannelPermission.ManageRoles)]
        public async Task RemoveRoleJoinAsync()
        {
            await FixRoleAsync(Context, false);
        }

        [Command("prefix")]
        [RequireUserPermission(ChannelPermission.ManageMessages)]
        public async Task SetPrefixAsync(string prefix)
        {
            var guildID = Context.Guild.Id;
            PublicConfig.ConfigData("prefix", prefix, guildID);
            await ReplyAsync("prefix is now set to " + prefix);
            debug.ConsolePrint.Print(Context.Guild, $"Prefix is now set to {prefix}", Context.User);
        }
        [Command("status")]
        public async Task statusAsync()
        {
            var ts = Program.stop_watch.Elapsed;
            var elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00} ";

            await ReplyAsync("```" +
                "Status: Online " +
                "\nRunTime: " + elapsedTime + "```");
        }

         [Command("addBotChannel")]
         [RequireUserPermission(ChannelPermission.ManageChannels)]
         public async Task AddBotChannelAsync(IChannel channel)
         {
            var guildID = Context.Guild.Id;
            PublicConfig.ConfigData("botChannel.add", channel.Id, guildID);
            await ReplyAsync(channel.Name + " Is now set to Server Bot Channel");
            debug.ConsolePrint.Print(Context.Guild, $"{channel.Name} is not set to Server Bot Channel", Context.User);
        }

        [Command("removeBotChannel")]
        [RequireUserPermission(ChannelPermission.ManageChannels)]
        public async Task RemoveBotChannelAsync()
        {
            var guildID = Context.Guild.Id;
            PublicConfig.ConfigData("botChannel.remove", null, guildID);
            await ReplyAsync("Bot Channle is now set to none");
            debug.ConsolePrint.Print(Context.Guild, $"bot Channle is now set to null", Context.User);
        }

        [Command("clearBotChannel")]
        [RequireUserPermission(ChannelPermission.ManageChannels)]
        public async Task ClearBotChannelAsync(bool mode)
        {
            var guildID = Context.Guild.Id;
            PublicConfig.ConfigData("clearBotChannle", mode, guildID);
            await ReplyAsync( mode?" I will now remove most stuff from the bot channel in intervalls":" I wont remove your commands");
            debug.ConsolePrint.Print(Context.Guild, $"Clear Bot Channel is set to {mode}" , Context.User);
        }

        private async Task FixRoleAsync(SocketCommandContext Context,bool addRemove)
        {
            var roles = Context.Message.MentionedRoles.ToArray();
            var guildID = Context.Guild.Id;
            var removedAffected = 0;
            if (roles.Length <= 0)
            {
                await ReplyAsync("wooks wike someone didn't wenter a role");
                debug.ConsolePrint.Print(Context.Guild, "add role to list Error no roles was taged");
                return;
            }
            else
            {
                var data = PublicConfig.GetSettings();
                var server = data.Servers.First(s => s.ServerID == guildID);
                foreach (var role in roles)
                {
                    if(server.RolesID == null && addRemove)
                    {
                        PublicConfig.ConfigData("role.add", role.Id, guildID);
                        removedAffected++;
                    }

                    if (addRemove && !server.RolesID.Contains(role.Id))
                    {
                            PublicConfig.ConfigData("role.add", role.Id, guildID);
                            removedAffected++;
                    }
                    else if (!addRemove && server.RolesID.Contains(role.Id))
                    {
                        PublicConfig.ConfigData("role.remove", role.Id, guildID);
                        removedAffected++;
                    }
                }
                if (removedAffected > 0)
                {
                    if (addRemove)
                    {
                        await ReplyAsync("```" + removedAffected.ToString() + " roles have been added to role list!```");
                    }
                    else
                    {
                        await ReplyAsync("```" + removedAffected.ToString() + " roles have been removed frome role list!```");
                    }
                }
                else
                {
                    await ReplyAsync("Looks like at the roles you mention is already fixed");
                }
            }
        }
    }
}
