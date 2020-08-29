using Serilog;
using Serilog.Events;
using System;
using System.Linq;

namespace Global
{
    public static class Log
    {
        public static void Initialize(string projectName, bool activateRemoteLog = true)
        {
            var lc = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Information()
                .Enrich.WithProperty("Application", projectName);
            if (activateRemoteLog)
            {
                lc = lc.WriteTo.Seq($"{DIManager.Services.SeqConfigs().Schema}://{DIManager.Services.SeqConfigs().EndPoint}:{DIManager.Services.SeqConfigs().Port}");
            }
#if DEBUG
            lc = lc.Enrich.WithProperty("Environment", "Debug");
#else
            lc = lc.Enrich.WithProperty("Environment", "Release");
#endif
            Serilog.Log.Logger = lc.CreateLogger();
        }
        public static void BlackListedGuild(string guildId, string guildName)
        {
            var format = "{lt}: Jailed Guild {GuildId} name is {GuildName}";
            Warning(format,"DB", guildId, guildName);
        }

        public static void BlackListedUser(string userId, string userName)
        {
            var format = "{lt}: Jailed User {UserId} name is {UserName}";
            Warning(format, "DB", userId, userName);
        }

        public static void CommandRequest(string guildId, string channelName, string discordUserId, string discordUserName, string commandRequest)
        {
            var format = "G: {GuildId} | C: {ChannelName} | U: {UserId} | U: {UserName} | RCMD: {CommandRequest}";
            Information(format, guildId, channelName, discordUserId, discordUserName, commandRequest);
        }

        public static void Exception(string exceptionMessage, Serilog.Events.LogEventLevel logEventLevel = LogEventLevel.Error)
        {
            var format = exceptionMessage;
            Write(logEventLevel, format);
        }
        public static void Exception(Exception exception, Serilog.Events.LogEventLevel logEventLevel = LogEventLevel.Error, string exceptionNote = null)
        {
            string LogType = "SeqException";
            string Message = null;
            string StackTrace = null;
            string Source = null;
            string InnerMessage = null;
            string InnerStackTrace = null;
            string InnerSource = null;
            string format = "{lt}: {Message} {StackTrace} {Source} {InnerMessage} {InnerStackTrace} {InnerSource} {ExceptionNote}";
            Message = exception.Message;
            StackTrace = exception?.StackTrace ?? ".";
            Source = exception?.Source ?? ".";
            InnerMessage = (exception?.InnerException?.Message) ?? ".";
            InnerStackTrace = (exception?.InnerException?.StackTrace) ?? ".";
            InnerSource = (exception?.InnerException?.Source) ?? ".";
            Write(logEventLevel, format, LogType, Message, StackTrace, Source, InnerMessage, InnerStackTrace, InnerSource, exceptionNote);
        }
        public static void GuildJoin(string guildId, string guildName, string shardId, bool hasManageRolePermission)
        {
            var format = "{lt}: Joined {ManageRolePermission} {GuildId} name is '{GuildName}' to shard {ShardId}";
            Information(format, "DB", (hasManageRolePermission == true ? "p+" : "p-"), guildId, guildName, shardId);
        }
        public static void UserJoin(string userName, string guildId, string guildName, string userAccountAge)
        {
            var format = "{lt}: Joined User {UserName} to ({GuildId}){GuildName}, CreatedAt:{UserAccountAge}";
            Information(format, "DB", userName, guildId, guildName, userAccountAge);
        }

        public static void GuildLeft(string guildId, string guildName)
        {
            var format = "{lt}: Left {GuildId} name is '{GuildName}'";
            Information(format, "DB", guildId, guildName);
        }

        public static void ShardConnected(string shardId, string guildCount, string userCount)
        {
            var format = "{lt}: Shard {ShardId} Connected with {GuildCount} Guilds and {UserCount} Users";
            Information(format, "FTNPower", shardId, guildCount, userCount);
        }

        public static void ShardReady(string shardId)
        {
            var format = "{lt}: Shard {ShardId} Ready";
            Information(format,"FTNPower", shardId);
        }
        public static void ShardError(string shardId,string connectionState)
        {
            var format = "{lt}: Shard {ShardId} is not connected, current state: {ShardConnectionState}";
            Error(format, "FTNPower", shardId, connectionState);
        }

        public static void Write(LogEventLevel level, string messageTemplate, params object[] propertyValues)
        {
            Serilog.Log.Write(level, messageTemplate, propertyValues);
        }
        public static void Information(string messageTemplate, params object[] propertyValues)
        {
            Serilog.Log.Information(messageTemplate, propertyValues);
        }
        public static void Warning(string messageTemplate, params object[] propertyValues)
        {
            Serilog.Log.Warning(messageTemplate, propertyValues);
        }
        public static void Fatal(string messageTemplate, params object[] propertyValues)
        {
            Serilog.Log.Fatal(messageTemplate, propertyValues);
        }
        public static void Error(string messageTemplate, params object[] propertyValues)
        {
            Serilog.Log.Error(messageTemplate, propertyValues);
        }
    }
}