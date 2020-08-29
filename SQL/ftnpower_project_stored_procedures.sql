USE [ftnpowerdb]
GO
/****** Object:  StoredProcedure [dbo].[SP_GlobalTop20Users]    Script Date: 29.08.2020 11:59:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_GlobalTop20Users]
AS
BEGIN
select top 20 a.AccountPowerLevel, a.PlayerName, '' as Id, a.EpicId, a.CommanderLevel, a.CollectionBookLevel 
from(SELECT TOP 30 CAST(AccountPowerLevel AS float) as AccountPowerLevel, PlayerName, EpicId, CommanderLevel, CollectionBookLevel,ROW_NUMBER() OVER (PARTITION BY PlayerName ORDER BY AccountPowerLevel DESC) AS RN 
	from dbo.FortnitePVEProfiles order by AccountPowerLevel desc, CommanderLevel desc, CollectionBookLevel desc) as a where RN = 1
END
GO
/****** Object:  StoredProcedure [dbo].[SP_IJTABLE_UserNotValid]    Script Date: 29.08.2020 11:59:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_IJTABLE_UserNotValid]
    @GuildId [nvarchar](max),
    @UserId [nvarchar](max)
AS
BEGIN
    exec dbo.SP_User_ValidState @UserId, 0
    exec SP_NameState_InQueue @GuildId, @UserId, 0
END
GO
/****** Object:  StoredProcedure [dbo].[SP_ListOfReadyToUpdate]    Script Date: 29.08.2020 11:59:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_ListOfReadyToUpdate]
AS
BEGIN
	select distinct DiscordServerId, FortniteUserId, EpicId, NameTag, GameUserMode, PVEDecimals
    from dbo.NameStates as a
    INNER JOIN dbo.FortniteUsers as b ON b.Id = a.FortniteUserId
	INNER JOIN dbo.DiscordServers c ON c.Id = a.DiscordServerId
    where
    EpicId IS NOT NULL and
    a.LockName = 0 and 
    b.IsValidName = 1 and
    a.DiscordServerId in (select SUBSTRING(Id, 2,25 ) from PriorityTables where DATEDIFF(MINUTE, PriorityTables.Deadline, SYSDATETIMEOFFSET()) < 1 and Id like 's%')
    order by a.FortniteUserId
END
GO
/****** Object:  StoredProcedure [dbo].[SP_LocalTop20Users]    Script Date: 29.08.2020 11:59:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[SP_LocalTop20Users]
    @GuildId [nvarchar](max)
AS
BEGIN
  select top 20 a.AccountPowerLevel, a.PlayerName, a.Id, a.EpicId, a.CommanderLevel, a.CollectionBookLevel from(select TOP 30 CAST(c.AccountPowerLevel AS float) as AccountPowerLevel, c.PlayerName, FortniteUserId as Id, b.EpicId, c.CommanderLevel, c.CollectionBookLevel,ROW_NUMBER() OVER (PARTITION BY PlayerName ORDER BY AccountPowerLevel DESC) AS RN 
		from dbo.NameStates 
		INNER JOIN dbo.FortniteUsers as b ON b.Id = dbo.NameStates.FortniteUserId
		INNER JOIN dbo.FortnitePVEProfiles as c ON c.EpicId collate Turkish_CI_AS = b.EpicId
		where DiscordServerId = @GuildId and b.IsValidName=1 and NOT EXISTS (SELECT Id from dbo.BlackListUsers where b.Id = dbo.BlackListUsers.Id)
    order by AccountPowerLevel desc, CommanderLevel desc,CollectionBookLevel desc) as a  where RN = 1
END
GO
/****** Object:  StoredProcedure [dbo].[SP_NameState_ClearQueueByGuildId]    Script Date: 29.08.2020 11:59:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_NameState_ClearQueueByGuildId]
    @GuildId [nvarchar](max)
AS
BEGIN
    update dbo.NameStates
    set InQueue = 0
    WHERE DiscordServerId = @GuildId and InQueue != 0
END
GO
/****** Object:  StoredProcedure [dbo].[SP_NameState_InQueue]    Script Date: 29.08.2020 11:59:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_NameState_InQueue]
    @GuildId [nvarchar](max),
    @UserId [nvarchar](max),
    @InQueue [bit]
AS
BEGIN
	
    update dbo.NameStates
                        set InQueue = @InQueue
                        WHERE FortniteUserId = @UserId and DiscordServerId = @GuildId and InQueue != @InQueue
	
END
GO
/****** Object:  StoredProcedure [dbo].[SP_RemoveNameStateForDiscord]    Script Date: 29.08.2020 11:59:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_RemoveNameStateForDiscord]
    @GuildId [nvarchar](max),
    @UserId [nvarchar](max)
AS
BEGIN
    DELETE FROM NameStates WHERE DiscordServerId = @GuildId and FortniteUserId = @UserId
END
GO
/****** Object:  StoredProcedure [dbo].[SP_TABLE_FortnitePVEProfile_Update]    Script Date: 29.08.2020 11:59:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[SP_TABLE_FortnitePVEProfile_Update]
    @EpicId [nvarchar](50),
    @PlayerName [nvarchar](50),
    @AccountPowerLevel [float],
    @Map [int],
    @CommanderLevel [int],
    @CollectionBookLevel [int],
    @NumMythicSchematics [int],
    @EliteFortnite2019 [bit]
AS
BEGIN
    IF EXISTS (SELECT 1 FROM dbo.FortnitePVEProfiles where EpicId = @EpicId)
    BEGIN
        UPDATE dbo.FortnitePVEProfiles 
		set PlayerName = @PlayerName,
		AccountPowerLevel = @AccountPowerLevel,
		Map = @Map,
		CommanderLevel = @CommanderLevel,
		CollectionBookLevel = @CollectionBookLevel,
		NumMythicSchematics = @NumMythicSchematics,
		EliteFortnite2019 = @EliteFortnite2019
		where EpicId = @EpicId
    END
ELSE
    BEGIN
        insert into dbo.FortnitePVEProfiles (EpicId,PlayerName,AccountPowerLevel, Map, CommanderLevel, CollectionBookLevel, NumMythicSchematics, EliteFortnite2019)
		values (@EpicId,@PlayerName,@AccountPowerLevel,@Map,@CommanderLevel,@CollectionBookLevel,@NumMythicSchematics, @EliteFortnite2019)
    END

END
GO
/****** Object:  StoredProcedure [dbo].[SP_TABLE_FortnitePVPProfile_Update]    Script Date: 29.08.2020 11:59:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[SP_TABLE_FortnitePVPProfile_Update]
    @EpicId [nvarchar](50),
    @PlayerName [nvarchar](50),
    @PvpWinSolo [int],
    @PvpWinDuo [int],
    @PvpWinSquad [int]
AS
BEGIN
    IF EXISTS (SELECT 1 FROM dbo.FortnitePVPProfiles where EpicId = @EpicId)
    BEGIN
        UPDATE dbo.FortnitePVPProfiles 
		set PlayerName = @PlayerName,
			PvpWinSolo = @PvpWinSolo,
			PvpWinDuo = @PvpWinDuo,
		    PvpWinSquad = @PvpWinSquad
		where EpicId = @EpicId
    END
ELSE
    BEGIN
        insert into dbo.FortnitePVPProfiles (EpicId,PlayerName,PvpWinSolo,PvpWinDuo,PvpWinSquad)
		values (@EpicId,@PlayerName,@PvpWinSolo,@PvpWinDuo,@PvpWinSquad)
    END
END
GO
/****** Object:  StoredProcedure [dbo].[SP_TABLE_FortniteUser_Update]    Script Date: 29.08.2020 11:59:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_TABLE_FortniteUser_Update]
    @Id [nvarchar](20),
    @EpicId [nvarchar](50)= NULL,
    @NameTag [bit],
    @IsValidName [bit],
    @GameUserMode [int]
AS
BEGIN
    IF EXISTS (SELECT 1 FROM dbo.FortniteUsers where Id = @Id)
    BEGIN
        UPDATE dbo.FortniteUsers 
		set  EpicId = @EpicId, NameTag = @NameTag, IsValidName = @IsValidName, GameUserMode = @GameUserMode, LastUpDateTime = SYSDATETIMEOFFSET()
		where Id = @Id and DATEDIFF(SECOND, LastUpDateTime, SYSDATETIMEOFFSET()) > 5
    END
ELSE
    BEGIN
        insert into dbo.FortniteUsers (Id, EpicId, NameTag, IsValidName, GameUserMode, LastUpDateTime)
		values (@Id, @EpicId, @NameTag, @IsValidName, @GameUserMode, SYSDATETIMEOFFSET())
    END
END
GO
/****** Object:  StoredProcedure [dbo].[SP_User_LastUpdateTime]    Script Date: 29.08.2020 11:59:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_User_LastUpdateTime]
    @UserId [nvarchar](max)
AS
BEGIN

    update dbo.FortniteUsers
                        set LastUpDateTime = SYSDATETIMEOFFSET()
                        WHERE Id = @UserId and DATEDIFF(SECOND, LastUpDateTime, SYSDATETIMEOFFSET()) > 1
END
GO
/****** Object:  StoredProcedure [dbo].[SP_User_ValidState]    Script Date: 29.08.2020 11:59:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_User_ValidState]
    @UserId [nvarchar](max),
    @ValidState [bit]
AS
BEGIN
    update dbo.FortniteUsers
    set IsValidName = @ValidState, 
    LastUpDateTime = SYSDATETIMEOFFSET()
    WHERE Id = @UserId and IsValidName != @ValidState and DATEDIFF(SECOND, LastUpDateTime, SYSDATETIMEOFFSET()) > 5
END
GO
/****** Object:  StoredProcedure [dbo].[SP_User_VerifiedProfile]    Script Date: 29.08.2020 11:59:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[SP_User_VerifiedProfile]
    @Id [nvarchar](20),
    @VerifiedProfile [bit]
AS
BEGIN
    update dbo.FortniteUsers 
	set VerifiedProfile = @VerifiedProfile 
	WHERE Id = @Id
END
GO
