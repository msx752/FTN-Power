## Linking console account to epicgames account to get an EPIC NAME

#### __note that the shown name while playing on console is not your epic name__
1. open "https://www.epicgames.com/id/login"
2. login with a **console** account (`PSN` or `XBOX`)
3. in general settings find `Account Info`
4. find `Display Name` under `ID` label
5. `edit/define` `Display Name` value with a new epic name **don't worry, it doesn't change the console name, it will only show playing on the PC**
6. then save the changes
7. after awhile use `new Epic Name` on **FTN Power** like `f.name <epic name>`


## Initial settings of FTN Power discordbot

1. invite FTN Power bot to the discord using `https://ftnpower.com/Home/Invite`
2. (`OPTIONAL`, `DEFAULT:PVE`)prefer a default game-mode of discord using `f.discord.mode <pve/pvp>` which effects the results of `f.up` and `f.name` commads
3. adding **S.T.W.** map-zones by using `f.pve.maproles.add` or you can create a NEW discord by using the template which includes map-zones `https://discord.new/hQD3DxWpSB7b` (**you can remove any role if you don't need**)
4. after adding map-zones you can change the response language of **FTN Power** discordbot by using `f.lang <TR/EN/RU/ES/DE/PT/FR/NL>`, it will also change the name of discord-role to specified language.



## Meaning of 'no bot can manage a discord owner'
- this means **any discordbot CANNOT** `add`/`remove`/`update` information of `DiscordGuild Owner`, this special condition for specific person to prevent stealing or kicking the owner of the discord from his own discord, [related with discord policy](https://stackoverflow.com/questions/45251598/can-i-change-discord-servers-owner-nickname-with-bot).(**still bot will work at background but can not show on discord such as role or changing  the nickname**)

## The 'f.name' and 'f.up' commands aren't work on discord staf who have Administrator role
- In this situation you must move `FTN Power` discord role to above of `Administrator` discord role, because **FTN Power** bot doesn't have permission to manage user

## Basic Commands
  full command list on the [ftnpower.com](https://ftnpower.com/Home/Commands)

  **allowed to checking only-self information**
- f.name / f.link
- f.unlink
- f.up / f.update
- f.name.tag
- f.pve
- f.pr / f.pve.resources
- f.mode
- f.verify
- f.top.global
- f.pve.maproles.add
- f.pvp
- f.help
- f.info / f.donate
- f.vote
- f.patch
- f.discord.info
- f.lang
- f.user.update / f.user.up
- f.user.name
- f.user.mode
- f.user.info


## Important informations

1. using **FTN Power** requires a valid epic name
2. if you need to change response language of **FTN Power**, firstly use `f.lang en` then create map-zones or be sure you already have an english map-zones (**REQUIRES Guild Owner permission to use f.lang command**)

