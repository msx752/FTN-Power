using Discord;
using Discord.Commands;
using FTNPower.Core.DomainService;
using FTNPower.Model.Tables;
using Global;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FTNPower.Core.DiscordContext.Preconditions
{
    public class FTNPowerOwnerAttribute : PreconditionAttribute
    {
        /// <inheritdoc />
        public override string ErrorMessage { get; set; }

        /// <inheritdoc />
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.Channel is IDMChannel)
                return await Task.FromResult(PreconditionResult.FromError("User is not in a guild"));

            switch (context.Client.TokenType)
            {
                case TokenType.Bot:
                    using (var repo = DIManager.Services.GetRequiredService<IFTNPowerRepository>())
                    {
                        if (!repo.Bot.Config.Vars.DeveloperDiscordIds.Contains(context.User.Id))
                            return PreconditionResult.FromError(ErrorMessage ?? "The command can only be run by **the Owner of the FTNPower bot!**");
                    }
                    return PreconditionResult.FromSuccess();
                default:
                    return PreconditionResult.FromError($"{nameof(RequireOwnerAttribute)} is not supported by this {nameof(TokenType)}.");
            }
        }
    }
}
