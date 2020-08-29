using System.Collections.Generic;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using FTNPower.Core.DomainService;
using FTNPower.Core;
using FTNPower.Model.Tables;
using FTNPower.Core.Interfaces;
using Global;
using Fortnite.Localization;
using System.IO;

namespace FTNPower.Core.DiscordContext
{

    /// <summary>
    /// The module base.
    /// </summary>
    public abstract class BaseModule : ModuleBase<Context>, IDisposable
    {
        private bool disposedValue;

        public BaseModule()
        {

        }


        public IJsonStringLocalizer Translate
        {
            get
            {
                return DIManager.Services.GetRequiredService<IJsonStringLocalizer>();
            }
        }
        public GuildLanguage GetLanguage(ulong? serverId = null)
        {
            try
            {
                if (serverId.HasValue && serverId != Context.Guild.Id)
                {
                    return Context.Repo.Guild.Language(serverId.Value.ToString());
                }
                else
                {
                    return Context.GuildConfig.Owner.DefaultLanguage;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Reply in the server and then delete after the provided delay.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<IUserMessage> ReplyAndDeleteAsync(string message, TimeSpan? timeout = null)
        {
            timeout = timeout ?? TimeSpan.FromSeconds(5);
            var msg = await Context.Channel.SendMessageAsync(message).ConfigureAwait(false);
            _ = Task.Delay(timeout.Value).ContinueWith(_ => msg.DeleteAsync().ConfigureAwait(false)).ConfigureAwait(false);
            return msg;
        }

        /// <summary>
        /// Reply in the server. Shorthand for Context.Channel.SendMessageAsync()
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="embed">
        /// The embed.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<IUserMessage> ReplyAsync(string message, Embed embed = null)
        {
            return await ReplyAsync(message, false, embed);
        }
        public async Task<IUserMessage> ReplyImageAsync(Stream streamImage, string message = null, Embed embd = null)
        {
            return await Context.Channel.SendFileAsync(streamImage, "ftnpower.png", text: message, embed: embd, options: Utils.RequestOption)
               .ContinueWith(o =>
           {
               streamImage.Close();
               return o.Result;
           });
        }
        /// <summary>
        /// Shorthand for  replying with just an embed
        /// </summary>
        /// <param name="embed">
        /// The embed.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task<IUserMessage> ReplyAsync(Embed embed)
        {
            return ReplyAsync(string.Empty, false, embed);
        }

        /// <summary>
        /// Shorthand for  replying with just an embed
        /// </summary>
        /// <param name="embed">
        /// The embed.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task<IUserMessage> ReplyEmbedAsync(EmbedBuilder embed, KeyValuePair<bool, PriorityTable>? ptable = null)
        {
            if (!ptable.HasValue)
                ptable = new KeyValuePair<bool, PriorityTable>(false, null);
            if (!embed.Color.HasValue)
            {
                embed.Color = Color.Orange;
            }
            return ReplyEmbedAsync(embed.Description, embed.Title, embed.Fields, embed.Footer, embed.Color.Value, 0, ptable.Value);
        }

        public Task<IUserMessage> ReplyEmbedAsync(
              string desc)
        {
            return ReplyEmbedAsync(desc, null);
        }

        public Task<IUserMessage> ReplyEmbedAsync(
            string desc,
            string title)
        {
            return ReplyEmbedAsync(desc, title, new List<EmbedFieldBuilder>());
        }

        public Task<IUserMessage> ReplyEmbedAsync(
            string desc,
            string title,
            List<EmbedFieldBuilder> fields)
        {
            return ReplyEmbedAsync(desc, title, fields, null);
        }

        public Task<IUserMessage> ReplyEmbedAsync(
            string desc,
            string title,
            List<EmbedFieldBuilder> fields,
            EmbedFooterBuilder footer)
        {
            return ReplyEmbedAsync(desc, title, fields, footer, Color.Orange);
        }

        public Task<IUserMessage> ReplyEmbedAsync(
            string desc,
            string title,
            List<EmbedFieldBuilder> fields,
            EmbedFooterBuilder footer,
            Color color)
        {
            return ReplyEmbedAsync(desc, title, fields, footer, color, 0);
        }

        public Task<IUserMessage> ReplyEmbedAsync(
            string desc,
            string title,
            List<EmbedFieldBuilder> fields,
            EmbedFooterBuilder footer,
            Color color,
            int timeoutSecond)
        {
            return ReplyEmbedAsync(desc, title, fields, footer, color, timeoutSecond, new KeyValuePair<bool, PriorityTable>(false, null));
        }

        public string GetVerificationState()
        {
            //https://urlzs.com/HDvyW
            //https://cdn.discordapp.com/emojis/572433323419500553.png //old
            //https://cdn.discordapp.com/emojis/614075539896008725.png //new

            return Context.DiscordUser.VerifiedProfile ? "https://cdn.discordapp.com/emojis/614075539896008725.png" : "";
        }

        public string GetVerificationName()
        {
            return Context.DiscordUser.VerifiedProfile ? Context.PlayerName : $"{Context.Message.Author.Username.Normalize()}#{Context.Message.Author.Discriminator}";
        }

        public EmbedAuthorBuilder GetVerificationAuthor()
        {
            return new EmbedAuthorBuilder()
            {
                IconUrl = GetVerificationState(),
                Name = GetVerificationName(),
            };
        }

        public string GenerateFooterText(string guildId)
        {
            var footerText = "";
            var state = Context.Repo.Priority.IsPartnerGuild(guildId);
            if (state.Key && guildId != "465028350067605504" && guildId != "450233662739578880")
            {
                if (!string.IsNullOrWhiteSpace(state.Value.PromoteCreatorCode))
                {
                    footerText = Translate.GetBotTranslation(BotTranslationString.CreatorCode, GetLanguage(), state.Value.PromoteCreatorCode);
                }
                else if (state.Value.AdvertOn && !string.IsNullOrWhiteSpace(state.Value.AdvertCustomText))
                {
                    footerText = Translate.GetBotTranslation(BotTranslationString.SupportedBy, GetLanguage(), state.Value.AdvertCustomText);
                }
                else
                {
                    var name = "Kesintisiz";
                    footerText = Translate.GetBotTranslation(BotTranslationString.SupportedBy, GetLanguage(), name);
                }
            }
            else
            {
                var partner = Context.Repo.Priority.GetRandomAdvertGuildPartners();
                if (partner != null)
                {
                    if (!string.IsNullOrWhiteSpace(partner.AdvertCustomText))
                    {
                        footerText = Translate.GetBotTranslation(BotTranslationString.SupportedBy, GetLanguage(), partner.AdvertCustomText);
                    }
                    else
                    {
                        var name = "Kesintisiz";
                        footerText = Translate.GetBotTranslation(BotTranslationString.SupportedBy, GetLanguage(), name);
                    }
                }
            }
            return footerText;
        }

        public async Task<IUserMessage> ReplyEmbedAsync(string desc, string title, List<EmbedFieldBuilder> fields, EmbedFooterBuilder footer, Color color, int timeoutSecond, KeyValuePair<bool, PriorityTable> state)
        {
            try
            {
                var embed = new EmbedBuilder
                {
                    Title = title,
                    Description = desc,
                    Color = color,
                    Author = GetVerificationAuthor()
                };

                if (fields == null)
                    fields = new List<EmbedFieldBuilder>();

                foreach (var field in fields)
                    embed.AddField(field);

                embed.Footer = new EmbedFooterBuilder()
                {
                    Text = GenerateFooterText(Context.Guild.Id.ToString())
                };

                if (timeoutSecond < 1)
                {
                    return await ReplyAsync(string.Empty, false, embed.Build(), Utils.RequestOption);
                }
                else
                {
                    var msg = await Context.Channel.SendMessageAsync(null, false, embed.Build(), Utils.RequestOption);
                    return msg;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Task<IUserMessage> ReplyEmbedErrorAsync(
            string desc)
        {
            return ReplyEmbedAsync(desc, null, null, null, Color.Red);
        }

        public Task<IUserMessage> ReplyEmbedPriorityAsync(
            string desc, KeyValuePair<bool, PriorityTable> state)
        {
            return ReplyEmbedAsync(desc, null, null, null, Color.Orange, 0, state);
        }

        /// <summary>
        ///     Rather than just replying, we can spice things up a bit and embed them in a small message
        /// </summary>
        /// <param name="message">The text that will be contained in the embed</param>
        /// <returns>The message that was sent</returns>
        public Task<IUserMessage> SimpleEmbedAsync(string message)
        {
            var embed = new EmbedBuilder
            {
                Description = message,
                Color = Color.DarkOrange
            };
            return ReplyAsync(string.Empty, false, embed.Build());
        }

        public string ToTranslate(BotTranslationString str, GuildLanguage lang, params object[] list)
        {
            try
            {
                return Translate.GetBotTranslation(str, lang, list);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string ToTranslate(BotTranslationString str, params object[] list)
        {
            try
            {
                var lang = GetLanguage();

                return Translate.GetBotTranslation(str, lang, list);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        ///     This is just a shorthand conversion from out custom context to a socket context, for use in things like Interactive
        /// </summary>
        /// <returns>A new SocketCommandContext</returns>
        private SocketCommandContext SocketContext()
        {
            return new SocketCommandContext(Context.Client.GetShardFor(Context.Guild), Context.Message);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                   
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}