using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Fortnite;
using Fortnite.Api;
using Fortnite.External.Api;
using Fortnite.External.Api.Interfaces;
using Fortnite.Localization;
using FTNPower.Core;
using FTNPower.Core.ApplicationService;
using FTNPower.Core.DiscordApi;
using FTNPower.Core.DiscordContext;
using FTNPower.Core.DomainService;
using FTNPower.Core.Interfaces;
using FTNPower.Data.Migrations;
using FTNPower.Model.Enums;
using FTNPower.Model.Interfaces;
using FTNPower.Redis;
using FTNPower.Redis.Messaging.AutoRemove;
using FTNPower.Static.Repositories;
using FTNPower.Static.Services;
using Global;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Threading.Tasks;

namespace FTNPower
{
    public class FTNPowerMain
    {
        public FTNPowerMain()
        {
            var cultureInfo = new CultureInfo("en-GB", true);
            cultureInfo.NumberFormat.CurrencySymbol = "£";
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;
        }

        public DiscordShardedClient Client { get; private set; }
        public CommandService CommandService { get; private set; }
        public IRedisService Redis { get; private set; }

        public bool IsDiscordBotList(IFTNPowerRepository _repo, string discordbotListGuildId)
        {
            return discordbotListGuildId == _repo.Bot.Config.Vars.discordbots_org_Id;
        }

        public async Task StartBot()
        {
            Console.WriteLine("The '{0}' garbage collector is running.", GCSettings.IsServerGC ? "server" : "workstation");

            Global.Log.Information("Bot is using '{DatabaseType}' SQL DATABASE", FTNPower.Data.Utils.USE_LOCAL_DBCONTEXT ? "LOCAL" : "LIVE");
            ConfigureServices();
            Global.Log.Initialize("FTNPowerBot");
            Client = DIManager.Services.GetRequiredService<DiscordShardedClient>();
            CommandService = DIManager.Services.GetRequiredService<CommandService>();
            Redis = DIManager.Services.GetRequiredService<IRedisService>();
            Client.ShardReady += ShardReadyAsync;
            Client.LeftGuild += LeftGuildAsync;
            Client.UserJoined += UserJoinedAsync;
            Client.UserLeft += Client_UserLeft;
            Client.ShardConnected += ShardConnectedAsync;
            Client.MessageReceived += MessageReceivedAsync;
            Client.RoleUpdated += Client_RoleUpdated;
            Client.GuildMemberUpdated += Client_GuildMemberUpdated;
            Client.CurrentUserUpdated += Client_CurrentUserUpdated;
            Client.JoinedGuild += JoinedGuildAsync;
            CommandService.CommandExecuted += CommandService_CommandExecuted;

            CommandService.AddModulesAsync(Assembly.GetEntryAssembly(), DIManager.Services).Wait();
            Global.Log.Information("FortnitePower: Modules are Added");

            using (var repo = DIManager.Services.GetRequiredService<IFTNPowerRepository>())
            {
                await Client.LoginAsync(TokenType.Bot, repo.Bot.Config.Token);
                Global.Log.Information("{lt}: Logged In", "FTNPower");
            };
            await Client.StartAsync();
            Global.Log.Information("{lt}: Started", "FTNPower");

            DIManager.Services.UseFortniteApi().Wait();

            await Task.Delay(-1);
        }

        /// <summary>
        /// Generates an error message based on a command error.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="argPos">
        /// The arg pos.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        internal async Task CmdErrorAsync(Context context, IResult result, int argPos)
        {
            if (result.Error == CommandError.UnknownCommand)
            {
                return;
            }
            else if (result.Error == CommandError.UnmetPrecondition || result.Error == CommandError.BadArgCount)
            {
                string err = "";
                if (result.Error == CommandError.UnmetPrecondition)
                {
                    err = "insufficient permission";
                }
                else if (result.Error == CommandError.BadArgCount)
                {
                    err = "wrong parameter or type";
                }
                await context.Channel.SendMessageAsync(string.Empty, false, new EmbedBuilder
                {
                    Title = $"ERROR [ {err} ]",
                    Description = result.ErrorReason,
                    Color = Color.DarkRed,
                    Author = new EmbedAuthorBuilder()
                    {
                        IconUrl = context.Message.Author.GetAvatarUrl(),
                        Name = $"{context.Message.Author.Username}#{context.Message.Author.Discriminator}"
                    }
                }.Build());
            }

            await LogErrorAsync(result, context);
        }

        /// <summary>
        /// This logs discord messages to our LogHandler
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        internal Task LogAsync(LogMessage message)
        {
            Serilog.Events.LogEventLevel logEventLevel = Serilog.Events.LogEventLevel.Information;
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                    logEventLevel = Serilog.Events.LogEventLevel.Fatal;
                    break;

                case LogSeverity.Error:
                    logEventLevel = Serilog.Events.LogEventLevel.Error;
                    break;

                case LogSeverity.Warning:
                    logEventLevel = Serilog.Events.LogEventLevel.Warning;
                    break;

                case LogSeverity.Info:
                    logEventLevel = Serilog.Events.LogEventLevel.Information;
                    break;

                case LogSeverity.Verbose:
                    logEventLevel = Serilog.Events.LogEventLevel.Verbose;
                    break;

                case LogSeverity.Debug:
                    logEventLevel = Serilog.Events.LogEventLevel.Debug;
                    break;

                default:
                    logEventLevel = Serilog.Events.LogEventLevel.Information;
                    break;
            }
            Global.Log.Write(logEventLevel, message.Message);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Logs specified errors based on type.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        internal Task LogErrorAsync(IResult result, Context context)
        {
            Global.Log.Warning($"{{lt}}: user {{UserId}} in guild {{GuildId}} got this message '{{ErrorReason}}' on channel {{ChannelName}}", "CmdErrorReason", context.User.Id, context.Guild.Id, result.ErrorReason, context.Channel.Name);
            return Task.CompletedTask;
        }

        private Task Client_CurrentUserUpdated(SocketSelfUser arg1, SocketSelfUser arg2)
        {
            return Task.CompletedTask;
        }

        private Task Client_GuildMemberUpdated(SocketGuildUser arg1, SocketGuildUser arg2)
        {
            return Task.CompletedTask;
        }

        private Task Client_RoleUpdated(SocketRole oldRole, SocketRole newRoleGuild)
        {
            using (var Repo = DIManager.Services.GetRequiredService<IFTNPowerRepository>())
            {
                var currBot = newRoleGuild.Guild.CurrentUser;
                if (IsDiscordBotList(Repo, currBot.Guild.Id.ToString()))
                    return Task.CompletedTask;
                if (Repo.Blacklist.BlacklistGuilds.Where(p => p.Id == newRoleGuild.Guild.Id.ToString()).Any())
                {
                    Global.Log.BlackListedGuild(newRoleGuild.Guild.Id.ToString(), newRoleGuild.Name);
                    newRoleGuild.Guild.LeaveAsync().Wait();
                    return Task.CompletedTask;
                }

                var relatedRoleWithBot = currBot.Roles.FirstOrDefault(s => s.Id == newRoleGuild.Id);
                if (relatedRoleWithBot != null)
                {
                    if (!oldRole.Permissions.ManageRoles && newRoleGuild.Permissions.ManageRoles)
                        Repo.Guild.Enable(newRoleGuild.Guild, GuildLanguage.EN);
                }
                currBot = null;
            }
            return Task.CompletedTask;
        }

        private Task Client_UserLeft(SocketGuildUser arg)
        {
            return Task.CompletedTask;
        }

        private Task CommandService_CommandExecuted(Optional<CommandInfo> arg1, ICommandContext context, IResult arg3)
        {
            if (arg3.IsSuccess)
            {
                Global.Log.CommandRequest(context.Guild.Id.ToString(),
                    context.Channel.Name,
                    context.User.Id.ToString(),
                    (context.User.Username + '#' + context.User.Discriminator),
                    context.Message.Content);
            }
            return Task.CompletedTask;
        }

        private void ConfigureServices()
        {
            Global.DIManager.BuildService((services) =>
            {
                var cnf = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
#if RELEASE
                        .AddJsonFile("appsettings.Production.json", optional: true)
#elif DEBUG
                        .AddJsonFile("appsettings.Development.json", optional: true)
#endif
                        .AddEnvironmentVariables()
                        .Build();
                Global.ConfigurationProvider.SetManualConfiguration(cnf);
                services.AddSingleton<IConfiguration>(cnf)
                .AddSingleton(new CommandService(new CommandServiceConfig
                {
                    ThrowOnError = false,
                    IgnoreExtraArgs = false,
                    DefaultRunMode = RunMode.Async,
                    CaseSensitiveCommands = false,
                }))
               .AddSingleton(x =>
               {
                   return new DiscordShardedClient(new DiscordSocketConfig
                   {
                       MessageCacheSize = 0,
                       ConnectionTimeout = 30000,
                       AlwaysDownloadUsers = false,
                       LogLevel = x.GetRequiredService<IFTNPowerRepository>().Bot.Config.Developing ? LogSeverity.Debug : LogSeverity.Warning,
                       // Please change increase this as your server count grows beyond 2000 guilds. ie. < 2000 = 1, 2000 = 2, 4000 = 2 ...
                       TotalShards = x.GetRequiredService<IFTNPowerRepository>().Bot.Config.Shards,
                       ExclusiveBulkDelete = true,
                       DefaultRetryMode = RetryMode.AlwaysRetry,
                       GuildSubscriptions = false,
                   });
               })
               .AddTransient<BotContext>()
               .AddTransient<IUnitOfWork, UnitOfWork>()
               .AddTransient<IFTNPowerRepository, FTNPowerRepository>()
               .AddSingleton<IEpicApi, EpicApi>()
               .AddSingleton<IExternalApi, ExternalApi>()
               .AddSingleton<IFortniteQueueApi>(x=>new FortniteQueueApi(x.FortniteQueueApiConfigs().BasicAuthToken, x.FortniteQueueApiConfigs().Host, x.FortniteQueueApiConfigs().Port))
               .AddSingleton<IJsonStringLocalizer, JsonStringLocalizer>()
               .AddSingleton<IDiscordRestApi>(x => new DiscordRestApi(
                    x.GetRequiredService<IFTNPowerRepository>().Bot.Config.Developing,
                    x.GetRequiredService<IFTNPowerRepository>().Bot.Config.Token))
               .AddSingleton<IRedisService, RedisService>();
            });

            BotContext.Initialize();
        }

        private Task JoinedGuildAsync(SocketGuild guild)
        {
            using (var Repo = DIManager.Services.GetRequiredService<IFTNPowerRepository>())
            {
                if (Repo.Blacklist.BlacklistGuilds.Where(p => p.Id == guild.Id.ToString()).Any())
                {
                    Global.Log.BlackListedGuild(guild.Id.ToString(), guild.Name);
                    guild.LeaveAsync().Wait();
                    return Task.CompletedTask;
                }

                var svr = Repo.Guild.AddOrGetGuild(guild.Id.ToString());
                Global.Log.GuildJoin(guild.Id.ToString(), guild.Name, this.Client.GetShardIdFor(guild).ToString(), guild.CurrentUser.GuildPermissions.ManageRoles);
            }
            return Task.CompletedTask;
        }

        private Task LeftGuildAsync(SocketGuild g)
        {
            Global.Log.GuildLeft(g.Id.ToString(), g.Name);
            return Task.CompletedTask;
        }

        private Task MessageReceivedAsync(SocketMessage socketMessage)
        {
            // Ignore system messages, or messages from other bots
            if (!(socketMessage is SocketUserMessage Message))
                return Task.CompletedTask;

            if (Message.Source != MessageSource.User)
                return Task.CompletedTask;

            SocketGuildChannel guild = (Message.Channel as SocketGuildChannel);
            if (guild == null)
                return Task.CompletedTask;

            var argPos = 0;
            using (IFTNPowerRepository repo = DIManager.Services.GetRequiredService<IFTNPowerRepository>())
            {
                if (!(Message.HasStringPrefix(repo.Bot.Config.Prefix, ref argPos, StringComparison.InvariantCultureIgnoreCase) ||
                        Message.HasMentionPrefix(guild.Guild.CurrentUser, ref argPos)))
                    return Task.CompletedTask;

                if (guild.Guild.Id.ToString() == repo.Bot.Config.Vars.discordbots_org_Id)
                    return Task.CompletedTask;

#if RELEASE
                  if ( repo.Bot.Config.Developing)
#elif DEBUG
                if (FTNPower.Data.Utils.USE_LOCAL_DBCONTEXT)
#endif
                {
                    if (!repo.Bot.Config.Vars.DeveloperDiscordIds.Contains(socketMessage.Author.Id))
                        return Task.CompletedTask;
                }

                if (repo.Blacklist.BlacklistGuilds.Where(p => p.Id == guild.Guild.Id.ToString()).Any()
                     || repo.Blacklist.BlacklistUsers.Where(p => p.Id == Message.Author.Id.ToString()).Any())
                    return Task.CompletedTask;
#if RELEASE
                    if (repo.Bot.Config.Vars.IgnoreRequest &&
                         repo.Bot.Config.Vars.DeveloperDiscordIds.Contains(Message.Author.Id) &&
                        !Message.Content.Contains("f.ignore", StringComparison.InvariantCultureIgnoreCase))
                        return Task.CompletedTask;
#endif

                Context context = new Context(Client, Message, repo, DIManager.Services.GetRequiredService<IDiscordRestApi>());
                try
                {
                    var result = CommandService.ExecuteAsync(context, argPos, DIManager.Services).Result;

                    if (!result.IsSuccess)
                        CmdErrorAsync(context, result, argPos).Wait();

                    if (context.GuildConfig.Owner.AutoRemoveRequest)
                        Redis.ListLeftPush("AutoDiscordMsgRemove", new ReadyToRemove(Message.Channel.Id, Message.Id, 10));
                }
                catch (Exception e)
                {
                    Global.Log.Exception(e);
                }
            }
            GC.Collect(2, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();
            return Task.CompletedTask;
        }

        private Task ShardConnectedAsync(DiscordSocketClient socketClient)
        {
            Global.Log.ShardConnected(socketClient.ShardId.ToString(), socketClient.Guilds.Count.ToString(), socketClient.Guilds.Sum(x => x.MemberCount).ToString());
            return Task.CompletedTask;
        }

        /// <summary>
        /// Triggers when a shard is ready
        /// </summary>
        /// <param name="socketClient">
        /// The socketClient.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task ShardReadyAsync(DiscordSocketClient socketClient)
        {
            //await socketClient.SetGameAsync("UvaBack", "https://www.twitch.tv/selectedStreamerID?utm_source=FortnitePower&utm_medium=DiscordBot&utm_campaign=fortnite-power", ActivityType.Streaming);
            await socketClient.SetActivityAsync(new Game("f.help", ActivityType.Playing));
            if (socketClient.ConnectionState == ConnectionState.Connected)
            {
#if RELEASE
                var repo = DIManager.Services.GetRequiredService<IFTNPowerRepository>();
                if (repo.Bot.Config.Developing)
#elif DEBUG
                if (Data.Utils.USE_LOCAL_DBCONTEXT)
#endif
                {
                    Global.Log.Information("Bot is in Development Mode!");
                }
                Global.Log.ShardReady(socketClient.ShardId.ToString());
#if RELEASE
                repo?.Dispose();
#endif
            }
            else
            {
                Global.Log.ShardError(socketClient.ShardId.ToString(), socketClient.ConnectionState.ToString());
            }
        }

        private Task UserJoinedAsync(SocketGuildUser newUser)
        {
            if (newUser.IsBot || newUser.IsWebhook || newUser.IsServerOwner())
                return Task.CompletedTask;

            using (var repo = DIManager.Services.GetRequiredService<IFTNPowerRepository>())
            {
                if (IsDiscordBotList(repo, newUser.Guild.Id.ToString()))
                    return Task.CompletedTask;

                if (repo.Blacklist.BlacklistGuilds.Any(f => f.Id == newUser.Guild.Id.ToString()))
                    return Task.CompletedTask;

                repo.Guild.AddOrGetGuild(newUser.Guild.Id.ToString());

                repo.User.AddOrGetUserAsync(newUser.Id.ToString(), newUser.Guild.Id.ToString(), GameUserMode.PVE).Wait();

                // MyLogger.Log.UserJoin($"{newUser.Username}#{newUser.Discriminator}", newUser.Guild.Id.ToString(), newUser.Guild.Name, newUser.CreatedAt.ToString());
            }
            return Task.CompletedTask;
        }
    }
}