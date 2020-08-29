using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Global;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using FTNPower.Data;
using FTNPower.Model.Tables;
using FTNPower.Model.WebsiteModels;
using FTNPower.Data.Tables;
using Microsoft.Data.SqlClient;

namespace FTNPower.Data.Migrations
{
    public class BotContext : IdentityDbContext
    {
        public DbSet<BotConfig> Configs { get; set; }
        public DbSet<DiscordServer> Servers { get; set; }
        public DbSet<NameState> NameStates { get; set; }
        public DbSet<FortniteUser> FortniteUsers { get; set; }
        public DbSet<BlackListGuild> BlackListGuilds { get; set; }
        public DbSet<BlackListUser> BlackListUsers { get; set; }
        public DbSet<PriorityTable> PriorityTables { get; set; }
        public new DbSet<ApplicationUser> Users { get; set; }
        public DbSet<EpicIdVerifyOrder> VerifyOrders { get; set; }
        public DbSet<GuildConfig> GuildConfigs { get; set; }
        public DbSet<PaypalTxn> PaypalTransactions { get; set; }
        public DbSet<FortniteAuthToken> FortniteAuthTokens { get; set; }
        public DbSet<FortnitePVEProfile> FortnitePVEProfiles { get; set; }
        public DbSet<FortnitePVPProfile> FortnitePVPProfiles { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Global.DIManager.Services.GetDatabaseConfigs().ConnectionString, options => options.EnableRetryOnFailure());
        }

        public BotContext()
               : base()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BotConfig>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Variables).HasConversion(x => JsonConvert.SerializeObject(x), x => JsonConvert.DeserializeObject<FTNPower.Model.WebsiteModels.CustomVariables>(x));
            });

            modelBuilder.Entity<BlackListGuild>().HasKey(s => s.Id);
            modelBuilder.Entity<BlackListUser>().HasKey(s => s.Id);

            modelBuilder.Entity<FortniteUser>().HasKey(s => s.Id);


            modelBuilder.Entity<DiscordServer>().HasKey(s => s.Id);

            modelBuilder.Entity<FortniteAuthToken>().HasKey(s => s.Id);
            // many to many 
            modelBuilder.Entity<NameState>().HasKey(f => new { f.DiscordServerId, f.FortniteUserId });
            modelBuilder.Entity<NameState>()
              .HasOne(s => s.FortniteUser)
              .WithMany(g => g.NameStates)
              .HasForeignKey(s => s.FortniteUserId);
            modelBuilder.Entity<NameState>()
             .HasOne(s => s.DiscordServer)
             .WithMany(g => g.NameStates)
             .HasForeignKey(s => s.DiscordServerId);

            modelBuilder.Entity<GuildConfig>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Owner).HasConversion(x => JsonConvert.SerializeObject(x), x => JsonConvert.DeserializeObject<OwnerConf>(x));
                entity.Property(p => p.Admin).HasConversion(x => JsonConvert.SerializeObject(x), x => JsonConvert.DeserializeObject<AdminConf>(x));
                entity.Property(p => p.Event).HasConversion(x => JsonConvert.SerializeObject(x), x => JsonConvert.DeserializeObject<EventConf>(x));
                entity.Property(p => p.Other).HasConversion(x => JsonConvert.SerializeObject(x), x => JsonConvert.DeserializeObject<OtherConf>(x));
            });

            modelBuilder.Entity<FortnitePVEProfile>().HasKey(s => s.EpicId);
            modelBuilder.Entity<FortnitePVPProfile>().HasKey(s => s.EpicId);
            base.OnModelCreating(modelBuilder);
        }

        public static void Initialize()
        {
            BotContext context = DIManager.Services.GetRequiredService<BotContext>();
            context.Database.EnsureCreated();
            BotConfig config = new BotConfig(DIManager.Services.DiscordBotConfigs().ProjectName, DIManager.Services.DiscordBotConfigs().DiscordAppToken);
            if (context.Configs.Find(config.Id) == null)
            {
                context.Configs.Add(config);
            }

            PriorityTable ptable1 = new PriorityTable();//fortnitepower
            ptable1.Id = $"s{465028350067605504}";
            ptable1.Deadline = DateTimeOffset.UtcNow.AddYears(1);
            if (context.PriorityTables.Find(ptable1.Id) == null)
            {
                context.PriorityTables.Add(ptable1);
            }

            PriorityTable ptable2 = new PriorityTable();//fırtına kalkanı türkiye
            ptable2.Id = $"s{450233662739578880}";
            ptable2.Deadline = DateTimeOffset.UtcNow.AddYears(1);
            if (context.PriorityTables.Find(ptable2.Id) == null)
            {
                context.PriorityTables.Add(ptable2);
            }

            PriorityTable ptable3 = new PriorityTable();//kesintisiz#9182
            ptable3.Id = $"u{193749607107395585}";
            ptable3.Deadline = DateTimeOffset.UtcNow.AddYears(1);
            if (context.PriorityTables.Find(ptable3.Id) == null)
            {
                context.PriorityTables.Add(ptable3);
            }
            var exampleGuildConfig = "{  \n   \"Id\":\"465028350067605504\",  \n   \"Owner\":{  \n      \"DefaultLanguage\":0,  \n      \"AutoRemoveRequest\":false,  \n      \"DefaultGameMode\":1,  \n      \"PVEDecimalState\":false,  \n      \"RestrictedRoleIds\":[  \n           \n      ]  \n   },  \n   \"Admin\":{  \n      \"MissionStates\":{  \n         \"Active\":false,  \n         \"Channels\":[  \n            {  \n               \"MissionType\":0,  \n               \"ChannelId\":\"525161479234781194\",  \n               \"RoleId\":\"\"  \n            }  \n         ]  \n      },  \n      \"LlamaSates\":{  \n         \"Active\":false,  \n         \"ChannelId\":\"568887871910576128\",  \n         \"RoleIdToMention\":\"\"  \n      },  \n      \"BrStoreStates\":{  \n         \"Active\":false,  \n         \"ChannelId\":\"576997364578123787\",  \n         \"RoleIdToMention\":\"\"  \n      },  \n      \"StwStoreStates\":{  \n         \"Active\":false,  \n         \"ChannelId\":\"672260410098843658\",  \n         \"RoleIdToMention\":\"\"  \n      }  \n   },  \n   \"Other\":{  \n        \n   },  \n   \"Event\":{  \n      \"MythicSKStates\":{  \n         \"Active\":true,  \n         \"RoleIdToMythicSK\":\"648882849872871447\"  \n      },  \n      \"EliteFrostnite2019s\":{  \n         \"Active\":true,  \n         \"RoleId\":\"655216469201780751\"  \n      }  \n   }  \n}";
            GuildConfig guildConfig = JsonConvert.DeserializeObject<GuildConfig>(exampleGuildConfig);
            //guildConfig.Id = "";//guild ID
            //guildConfig.Admin.BrStoreStates.ChannelId = "";//webhook channel id
            //guildConfig.Admin.LlamaSates.ChannelId = "";//webhook channel id
            //guildConfig.Admin.MissionStates.Channels.Add(new ChannelItem() { ChannelId = ""/*webhook channel id*/, MissionType=MissionType.All_TheSame_Channel,/* RoleId="" //mentioned role id,not required*/ });
            //guildConfig.Admin.StwStoreStates.ChannelId = "";//webhook channel id
            //context.GuildConfigs.Add(guildConfig);


            //look at in 'SQL' folder search 'ftnpower_database_example_data.sql'

            context.SaveChanges();
        }

        public async Task<List<T>> SqlQuery<T>(string rawSql,
            params SqlParameter[] parameters)
        {
            using (var db = new BotContext())
            {
                var conn = db.Database.GetDbConnection();
                List<T> res = new List<T>();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = rawSql;
                    command.Parameters.Clear();
                    if (parameters != null)
                    {
                        foreach (var item in parameters)
                        {
                            var p = command.CreateParameter();
                            p.ParameterName = item.ParameterName;
                            p.Value = item.Value;
                            p.DbType = item.DbType;
                            command.Parameters.Add(p);
                        }
                    }
                    if (conn.State == System.Data.ConnectionState.Closed)
                    {
                        await conn.OpenAsync();
                    }
                    using (var r = command.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            T t = Activator.CreateInstance<T>();
                            for (int inc = 0; inc < r.FieldCount; inc++)
                            {
                                Type type = t.GetType();
                                string
                                     pname = r.GetName(inc);
                                PropertyInfo
                                    prop = type.GetProperty(pname);

                                prop.SetValue(t, r.GetValue(inc), null);
                            }
                            res.Add(t);
                        }
                    }
                }
                return res;
            }

        }
    }
}