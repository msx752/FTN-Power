USE [ftnpowerdb]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 29.08.2020 12:00:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoleClaims]    Script Date: 29.08.2020 12:00:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 29.08.2020 12:00:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 29.08.2020 12:00:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 29.08.2020 12:00:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](450) NOT NULL,
	[ProviderKey] [nvarchar](450) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 29.08.2020 12:00:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](450) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 29.08.2020 12:00:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](450) NOT NULL,
	[UserName] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[Email] [nvarchar](256) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[Discriminator] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 29.08.2020 12:00:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserTokens](
	[UserId] [nvarchar](450) NOT NULL,
	[LoginProvider] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BlackListGuilds]    Script Date: 29.08.2020 12:00:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BlackListGuilds](
	[Id] [nvarchar](20) NOT NULL,
	[Until] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_dbo.BlackListGuilds] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BlackListUsers]    Script Date: 29.08.2020 12:00:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BlackListUsers](
	[Id] [nvarchar](20) NOT NULL,
	[Until] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_dbo.BlackListUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BotConfigs]    Script Date: 29.08.2020 12:00:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BotConfigs](
	[Id] [nvarchar](20) NOT NULL,
	[Shards] [int] NOT NULL,
	[Prefix] [nvarchar](max) NULL,
	[Token] [nvarchar](max) NULL,
	[LogUserMessages] [bit] NOT NULL,
	[LogCommandUsages] [bit] NOT NULL,
	[Developing] [bit] NOT NULL,
	[Variables] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.BotConfigs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DiscordServers]    Script Date: 29.08.2020 12:00:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DiscordServers](
	[Id] [nvarchar](20) NOT NULL,
	[Language] [int] NOT NULL,
	[IsInitialized] [bit] NOT NULL,
	[CustomPrefix] [nvarchar](1) NULL,
	[LastMassUpdate] [datetimeoffset](7) NOT NULL,
	[RestrictedRoleIds] [nvarchar](max) NULL,
	[AutoRemoveRequest] [bit] NOT NULL,
	[DefaultGameMode] [tinyint] NOT NULL,
	[PVEDecimals] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.DiscordServers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FortniteAuthTokens]    Script Date: 29.08.2020 12:00:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FortniteAuthTokens](
	[Id] [nvarchar](450) NOT NULL,
	[access_token] [nvarchar](max) NULL,
	[expires_in] [int] NOT NULL,
	[expires_at] [datetime2](7) NOT NULL,
	[token_type] [nvarchar](max) NULL,
	[refresh_token] [nvarchar](max) NULL,
	[refresh_expires] [int] NOT NULL,
	[refresh_expires_at] [datetime2](7) NOT NULL,
	[account_id] [nvarchar](max) NULL,
	[client_id] [nvarchar](max) NULL,
	[internal_client] [bit] NOT NULL,
	[client_service] [nvarchar](max) NULL,
	[app] [nvarchar](max) NULL,
	[in_app_id] [nvarchar](max) NULL,
	[device_id] [nvarchar](max) NULL,
	[code] [nvarchar](max) NULL,
 CONSTRAINT [PK_FortniteAuthTokens] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FortnitePVEProfiles]    Script Date: 29.08.2020 12:00:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FortnitePVEProfiles](
	[EpicId] [nvarchar](50) NOT NULL,
	[PlayerName] [nvarchar](50) NULL,
	[AccountPowerLevel] [float] NOT NULL,
	[Map] [int] NOT NULL,
	[CommanderLevel] [int] NOT NULL,
	[CollectionBookLevel] [int] NOT NULL,
	[NumMythicSchematics] [int] NOT NULL,
	[EliteFortnite2019] [bit] NOT NULL,
 CONSTRAINT [PK_FortnitePVEProfiles] PRIMARY KEY CLUSTERED 
(
	[EpicId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FortnitePVPProfiles]    Script Date: 29.08.2020 12:00:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FortnitePVPProfiles](
	[EpicId] [nvarchar](50) NOT NULL,
	[PlayerName] [nvarchar](50) NULL,
	[PvpWinSolo] [int] NOT NULL,
	[PvpWinDuo] [int] NOT NULL,
	[PvpWinSquad] [int] NOT NULL,
 CONSTRAINT [PK_FortnitePVPProfiles] PRIMARY KEY CLUSTERED 
(
	[EpicId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FortniteUsers]    Script Date: 29.08.2020 12:00:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FortniteUsers](
	[Id] [nvarchar](20) NOT NULL,
	[IsValidName] [bit] NOT NULL,
	[LastUpDateTime] [datetimeoffset](7) NOT NULL,
	[EpicId] [nvarchar](max) NULL,
	[GameUserMode] [tinyint] NOT NULL,
	[NameTag] [bit] NOT NULL,
	[VerifiedProfile] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.FortniteUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GuildConfigs]    Script Date: 29.08.2020 12:00:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GuildConfigs](
	[Id] [nvarchar](20) NOT NULL,
	[Owner] [nvarchar](max) NULL,
	[Admin] [nvarchar](max) NULL,
	[Other] [nvarchar](max) NULL,
	[Event] [nvarchar](max) NULL,
 CONSTRAINT [PK_GuildConfigs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NameStates]    Script Date: 29.08.2020 12:00:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NameStates](
	[DiscordServerId] [nvarchar](20) NOT NULL,
	[FortniteUserId] [nvarchar](20) NOT NULL,
	[InQueue] [bit] NOT NULL,
	[LockName] [bit] NOT NULL,
	[Priority] [tinyint] NOT NULL,
 CONSTRAINT [PK_NameStates] PRIMARY KEY CLUSTERED 
(
	[DiscordServerId] ASC,
	[FortniteUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PaypalTransactions]    Script Date: 29.08.2020 12:00:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaypalTransactions](
	[TxnId] [nvarchar](30) NOT NULL,
	[DiscordUserId] [nvarchar](max) NULL,
 CONSTRAINT [PK_PaypalTransactions] PRIMARY KEY CLUSTERED 
(
	[TxnId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PriorityTables]    Script Date: 29.08.2020 12:00:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PriorityTables](
	[Id] [nvarchar](21) NOT NULL,
	[Deadline] [datetimeoffset](7) NOT NULL,
	[AdvertCustomText] [nvarchar](max) NULL,
	[AdvertOn] [bit] NOT NULL,
	[Notified] [bit] NOT NULL,
	[PromoteCreatorCode] [nvarchar](50) NULL,
 CONSTRAINT [PK_dbo.PriorityTables] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VerifyOrders]    Script Date: 29.08.2020 12:00:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VerifyOrders](
	[Id] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_VerifyOrders] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AspNetUsers] ADD  DEFAULT (N'') FOR [Discriminator]
GO
ALTER TABLE [dbo].[DiscordServers] ADD  DEFAULT ((0)) FOR [Language]
GO
ALTER TABLE [dbo].[DiscordServers] ADD  DEFAULT ((0)) FOR [IsInitialized]
GO
ALTER TABLE [dbo].[DiscordServers] ADD  DEFAULT ('0001-01-01T00:00:00.000+00:00') FOR [LastMassUpdate]
GO
ALTER TABLE [dbo].[DiscordServers] ADD  DEFAULT ((0)) FOR [AutoRemoveRequest]
GO
ALTER TABLE [dbo].[DiscordServers] ADD  DEFAULT (CONVERT([tinyint],(0))) FOR [DefaultGameMode]
GO
ALTER TABLE [dbo].[DiscordServers] ADD  DEFAULT (CONVERT([bit],(1))) FOR [PVEDecimals]
GO
ALTER TABLE [dbo].[FortniteUsers] ADD  DEFAULT ((0)) FOR [GameUserMode]
GO
ALTER TABLE [dbo].[FortniteUsers] ADD  DEFAULT ((0)) FOR [NameTag]
GO
ALTER TABLE [dbo].[FortniteUsers] ADD  DEFAULT (CONVERT([bit],(0))) FOR [VerifiedProfile]
GO
ALTER TABLE [dbo].[AspNetRoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetRoleClaims] CHECK CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserTokens]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserTokens] CHECK CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[NameStates]  WITH CHECK ADD  CONSTRAINT [FK_NameState_DiscordServers_DiscordServerId] FOREIGN KEY([DiscordServerId])
REFERENCES [dbo].[DiscordServers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[NameStates] CHECK CONSTRAINT [FK_NameState_DiscordServers_DiscordServerId]
GO
ALTER TABLE [dbo].[NameStates]  WITH CHECK ADD  CONSTRAINT [FK_NameStates_DiscordServers_DiscordServerId] FOREIGN KEY([DiscordServerId])
REFERENCES [dbo].[DiscordServers] ([Id])
GO
ALTER TABLE [dbo].[NameStates] CHECK CONSTRAINT [FK_NameStates_DiscordServers_DiscordServerId]
GO
