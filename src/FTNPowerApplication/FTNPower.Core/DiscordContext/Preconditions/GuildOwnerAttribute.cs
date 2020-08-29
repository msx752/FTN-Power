using FTNPower.Core.DomainService;
using FTNPower.Model.Tables;
using global::Discord;
using global::Discord.Commands;
using Global;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace FTNPower.Core.DiscordContext.Preconditions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class GuildOwnerAttribute : PreconditionAttribute
    {
        /// <summary>
        ///     This will check whether or not a user has permissions to use a command/module
        /// </summary>
        /// <param name="context">The Command Context</param>
        /// <param name="command">The command being invoked</param>
        /// <param name="services">The service provider</param>
        /// ///
        /// <returns>Success if the user is the owner of the current guild</returns>
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.Channel is IDMChannel)
                return Task.FromResult(PreconditionResult.FromError("User is not in a guild"));

            switch (context.Client.TokenType)
            {
                case TokenType.Bot:
                    using (var repo = DIManager.Services.GetRequiredService<IFTNPowerRepository>())
                    {
                        if (repo.Bot.Config.Vars.DeveloperDiscordIds.Contains(context.User.Id))
                            return Task.FromResult(PreconditionResult.FromSuccess());
                    }
                    return Task.FromResult(context.Guild.OwnerId == context.User.Id ? PreconditionResult.FromSuccess() : PreconditionResult.FromError("User is not the Guild Owner!"));
                default:
                    return Task.FromResult(PreconditionResult.FromError($"{nameof(RequireOwnerAttribute)} is not supported by this {nameof(TokenType)}."));
            }
        }
    }
}
