using Discord;
using Fortnite.Localization;
using Fortnite.Model.Enums;
using Fortnite.Static;
using fortniteLib.Responses.Pvp;
using FTNPower.Core;
using FTNPower.Data;
using FTNPower.Model.Enums;
using FTNPower.Model.Tables;
using FTNPower.Modules;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FTNPower.Modules.PVP
{
    public abstract class PVPFunctionLayer : FortniteFunctionLayer
    {
        protected PVPFunctionLayer()
            : base()
        {
        }

        public async Task PvpProfile(bool myInfo, string userName = null, Platform platform = Platform.keyboardmouse)
        {
            if (myInfo || !string.IsNullOrWhiteSpace(userName))
            {
                var pstate = await CheckUserPriority(new TimeSpan(0, 0, 7, 0), Context.DiscordUser, Context.Guild.GetUser(Context.User.Id));
                if (pstate.Value == null)
                    return;

                string PlayerName = "";
                if (myInfo)
                {
                    PlayerName = Context.PlayerName;
                }
                else
                {
                    if (pstate.Value.GetPriorityState() == PriorityState.Normal)
                    {
                        await ReplyEmbedErrorAsync(ToTranslate(BotTranslationString.EnablingToBotFeatures));
                        return;
                    }
                    PlayerName = userName;
                }
                KeyValuePair<string, BattleRoyaleStats> profile = await Api.GetPVPProfileByName(PlayerName);
                if (profile.Value != null)
                {
                    var solo_kills = profile.Value.stats.BR_Stats(StatType.kills, MatchType.solo, platform, true);
                    var solo_matches = profile.Value.stats.BR_Stats(StatType.matchesplayed, MatchType.solo, platform, true);
                    var solo_kda = Math.Round(solo_kills / solo_matches, 2);
                    var solo_wins = profile.Value.stats.BR_Stats(StatType.placetop1, MatchType.solo, platform, true);
                    var solo_winPercentage = Math.Round(solo_wins / (solo_matches == 0 ? 1 : solo_matches) * 100, 2);

                    var duo_kills = profile.Value.stats.BR_Stats(StatType.kills, MatchType.duo, platform, true);
                    var duo_matches = profile.Value.stats.BR_Stats(StatType.matchesplayed, MatchType.duo, platform, true);
                    var duo_kda = Math.Round(duo_kills / duo_matches, 2);
                    var duo_wins = profile.Value.stats.BR_Stats(StatType.placetop1, MatchType.duo, platform, true);
                    var duo_winPercentage = Math.Round(duo_wins / (duo_matches == 0 ? 1 : duo_matches) * 100, 2);

                    var squad_kills = profile.Value.stats.BR_Stats(StatType.kills, MatchType.squad, platform, true);
                    var squad_matches = profile.Value.stats.BR_Stats(StatType.matchesplayed, MatchType.squad, platform, true);
                    var squad_kda = Math.Round(squad_kills / squad_matches, 2);
                    var squad_wins = profile.Value.stats.BR_Stats(StatType.placetop1, MatchType.squad, platform, true);
                    var squad_winPercentage = Math.Round(squad_wins / (squad_matches == 0 ? 1 : squad_matches) * 100, 2);

                    var overall_kda = Math.Round(squad_kills / squad_matches, 2);
                    var overall_winPercentage = Math.Round((squad_wins + duo_wins + solo_wins) / (squad_matches + duo_matches + solo_matches) * 100, 2);

                    EmbedBuilder emb = new EmbedBuilder();
                    if (solo_matches > 0)
                    {
                        emb.AddField("Solo Kills", $"**{(int)solo_kills}**", true);
                        emb.AddField("Solo K/D", $"**{solo_kda}**", true);
                        emb.AddField("Wins/Matches", $"**{(int)solo_wins}** / *{(int)solo_matches}* (**{solo_winPercentage}**%)", true);
                    }

                    if (duo_matches > 0)
                    {
                        emb.AddField("Duo Kills", $"**{(int)duo_kills}**", true);
                        emb.AddField("Duo K/D", $"**{duo_kda}**", true);
                        emb.AddField("Wins/Matches", $"**{(int)duo_wins}** / *{(int)duo_matches}* (**{duo_winPercentage}**%)", true);
                    }

                    if (squad_matches > 0)
                    {
                        emb.AddField("Squad Kills", $"**{(int)squad_kills}**", true);
                        emb.AddField("Squad K/D", $"**{squad_kda}**", true);
                        emb.AddField("Wins/Matches", $"**{(int)squad_wins}** / *{(int)squad_matches}* (**{squad_winPercentage}**%)", true);
                    }

                    if (squad_matches > 0 && duo_matches > 0 && solo_matches > 0)
                    {
                        emb.AddField("Overall Kills", $"**{(int)squad_kills + (int)solo_kills + (int)duo_kills}**", true);
                        emb.AddField("Overall K/D", $"**{overall_kda}**", true);
                        emb.AddField("Wins/Matches", $"**{(int)squad_wins + (int)duo_wins + (int)solo_wins}** / *{(int)squad_matches + (int)duo_matches + (int)solo_matches}* (**{overall_winPercentage}**%)", true);
                    }
                    string platformText = "**PC**";
                    if (Platform.gamepad == platform)
                    {
                        platformText = "**CONSOLE**";
                    }
                    await ReplyEmbedAsync($"PvP Stats for **{profile.Key}** [Platform: {platformText}]", "", emb.Fields, emb.Footer, Color.LightOrange, 0, new KeyValuePair<bool, PriorityTable>(false, null));
                }
                else
                {
                    await Context.Message.SetErrorAsync();
                }
            }
        }
    }
}