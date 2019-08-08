local define = System.defStc
local setmetatable = setmetatable

local handle = define("War3ApiJassCommonJ.handle", {
})
local agent = define("War3ApiJassCommonJ.agent", {
  __inherits__ = { handle }
})
local event = define("War3ApiJassCommonJ.event", {
  __inherits__ = { agent }
})
local player = define("War3ApiJassCommonJ.player", {
  __inherits__ = { agent }
})
local widget = define("War3ApiJassCommonJ.widget", {
  __inherits__ = { agent }
})
local unit = define("War3ApiJassCommonJ.unit", {
  __inherits__ = { widget }
})
local destructable = define("War3ApiJassCommonJ.destructable", {
  __inherits__ = { widget }
})
local item = define("War3ApiJassCommonJ.item", {
  __inherits__ = { widget }
})
local ability = define("War3ApiJassCommonJ.ability", {
  __inherits__ = { agent }
})
local buff = define("War3ApiJassCommonJ.buff", {
  __inherits__ = { ability }
})
local force = define("War3ApiJassCommonJ.force", {
  __inherits__ = { agent }
})
local group = define("War3ApiJassCommonJ.group", {
  __inherits__ = { agent }
})
local trigger = define("War3ApiJassCommonJ.trigger", {
  __inherits__ = { agent }
})
local triggercondition = define("War3ApiJassCommonJ.triggercondition", {
  __inherits__ = { agent }
})
local triggeraction = define("War3ApiJassCommonJ.triggeraction", {
  __inherits__ = { handle }
})
local timer = define("War3ApiJassCommonJ.timer", {
  __inherits__ = { agent }
})
local location = define("War3ApiJassCommonJ.location", {
  __inherits__ = { agent }
})
local region = define("War3ApiJassCommonJ.region", {
  __inherits__ = { agent }
})
local rect = define("War3ApiJassCommonJ.rect", {
  __inherits__ = { agent }
})
local boolexpr = define("War3ApiJassCommonJ.boolexpr", {
  __inherits__ = { agent }
})
local sound = define("War3ApiJassCommonJ.sound", {
  __inherits__ = { agent }
})
local conditionfunc = define("War3ApiJassCommonJ.conditionfunc", {
  __inherits__ = { boolexpr }
})
local filterfunc = define("War3ApiJassCommonJ.filterfunc", {
  __inherits__ = { boolexpr }
})
local unitpool = define("War3ApiJassCommonJ.unitpool", {
  __inherits__ = { handle }
})
local itempool = define("War3ApiJassCommonJ.itempool", {
  __inherits__ = { handle }
})
local race = define("War3ApiJassCommonJ.race", {
  __inherits__ = { handle }
})
local alliancetype = define("War3ApiJassCommonJ.alliancetype", {
  __inherits__ = { handle }
})
local racepreference = define("War3ApiJassCommonJ.racepreference", {
  __inherits__ = { handle }
})
local gamestate = define("War3ApiJassCommonJ.gamestate", {
  __inherits__ = { handle }
})
local igamestate = define("War3ApiJassCommonJ.igamestate", {
  __inherits__ = { gamestate }
})
local fgamestate = define("War3ApiJassCommonJ.fgamestate", {
  __inherits__ = { gamestate }
})
local playerstate = define("War3ApiJassCommonJ.playerstate", {
  __inherits__ = { handle }
})
local playerscore = define("War3ApiJassCommonJ.playerscore", {
  __inherits__ = { handle }
})
local playergameresult = define("War3ApiJassCommonJ.playergameresult", {
  __inherits__ = { handle }
})
local unitstate = define("War3ApiJassCommonJ.unitstate", {
  __inherits__ = { handle }
})
local aidifficulty = define("War3ApiJassCommonJ.aidifficulty", {
  __inherits__ = { handle }
})
local eventid = define("War3ApiJassCommonJ.eventid", {
  __inherits__ = { handle }
})
local gameevent = define("War3ApiJassCommonJ.gameevent", {
  __inherits__ = { eventid }
})
local playerevent = define("War3ApiJassCommonJ.playerevent", {
  __inherits__ = { eventid }
})
local playerunitevent = define("War3ApiJassCommonJ.playerunitevent", {
  __inherits__ = { eventid }
})
local unitevent = define("War3ApiJassCommonJ.unitevent", {
  __inherits__ = { eventid }
})
local limitop = define("War3ApiJassCommonJ.limitop", {
  __inherits__ = { eventid }
})
local widgetevent = define("War3ApiJassCommonJ.widgetevent", {
  __inherits__ = { eventid }
})
local dialogevent = define("War3ApiJassCommonJ.dialogevent", {
  __inherits__ = { eventid }
})
local unittype = define("War3ApiJassCommonJ.unittype", {
  __inherits__ = { handle }
})
local gamespeed = define("War3ApiJassCommonJ.gamespeed", {
  __inherits__ = { handle }
})
local gamedifficulty = define("War3ApiJassCommonJ.gamedifficulty", {
  __inherits__ = { handle }
})
local gametype = define("War3ApiJassCommonJ.gametype", {
  __inherits__ = { handle }
})
local mapflag = define("War3ApiJassCommonJ.mapflag", {
  __inherits__ = { handle }
})
local mapvisibility = define("War3ApiJassCommonJ.mapvisibility", {
  __inherits__ = { handle }
})
local mapsetting = define("War3ApiJassCommonJ.mapsetting", {
  __inherits__ = { handle }
})
local mapdensity = define("War3ApiJassCommonJ.mapdensity", {
  __inherits__ = { handle }
})
local mapcontrol = define("War3ApiJassCommonJ.mapcontrol", {
  __inherits__ = { handle }
})
local playerslotstate = define("War3ApiJassCommonJ.playerslotstate", {
  __inherits__ = { handle }
})
local volumegroup = define("War3ApiJassCommonJ.volumegroup", {
  __inherits__ = { handle }
})
local camerafield = define("War3ApiJassCommonJ.camerafield", {
  __inherits__ = { handle }
})
local camerasetup = define("War3ApiJassCommonJ.camerasetup", {
  __inherits__ = { handle }
})
local playercolor = define("War3ApiJassCommonJ.playercolor", {
  __inherits__ = { handle }
})
local placement = define("War3ApiJassCommonJ.placement", {
  __inherits__ = { handle }
})
local startlocprio = define("War3ApiJassCommonJ.startlocprio", {
  __inherits__ = { handle }
})
local raritycontrol = define("War3ApiJassCommonJ.raritycontrol", {
  __inherits__ = { handle }
})
local blendmode = define("War3ApiJassCommonJ.blendmode", {
  __inherits__ = { handle }
})
local texmapflags = define("War3ApiJassCommonJ.texmapflags", {
  __inherits__ = { handle }
})
local effect = define("War3ApiJassCommonJ.effect", {
  __inherits__ = { agent }
})
local effecttype = define("War3ApiJassCommonJ.effecttype", {
  __inherits__ = { handle }
})
local weathereffect = define("War3ApiJassCommonJ.weathereffect", {
  __inherits__ = { handle }
})
local terraindeformation = define("War3ApiJassCommonJ.terraindeformation", {
  __inherits__ = { handle }
})
local fogstate = define("War3ApiJassCommonJ.fogstate", {
  __inherits__ = { handle }
})
local fogmodifier = define("War3ApiJassCommonJ.fogmodifier", {
  __inherits__ = { agent }
})
local dialog = define("War3ApiJassCommonJ.dialog", {
  __inherits__ = { agent }
})
local button = define("War3ApiJassCommonJ.button", {
  __inherits__ = { agent }
})
local quest = define("War3ApiJassCommonJ.quest", {
  __inherits__ = { agent }
})
local questitem = define("War3ApiJassCommonJ.questitem", {
  __inherits__ = { agent }
})
local defeatcondition = define("War3ApiJassCommonJ.defeatcondition", {
  __inherits__ = { agent }
})
local timerdialog = define("War3ApiJassCommonJ.timerdialog", {
  __inherits__ = { agent }
})
local leaderboard = define("War3ApiJassCommonJ.leaderboard", {
  __inherits__ = { agent }
})
local multiboard = define("War3ApiJassCommonJ.multiboard", {
  __inherits__ = { agent }
})
local multiboarditem = define("War3ApiJassCommonJ.multiboarditem", {
  __inherits__ = { agent }
})
local trackable = define("War3ApiJassCommonJ.trackable", {
  __inherits__ = { agent }
})
local gamecache = define("War3ApiJassCommonJ.gamecache", {
  __inherits__ = { agent }
})
local version = define("War3ApiJassCommonJ.version", {
  __inherits__ = { handle }
})
local itemtype = define("War3ApiJassCommonJ.itemtype", {
  __inherits__ = { handle }
})
local texttag = define("War3ApiJassCommonJ.texttag", {
  __inherits__ = { handle }
})
local attacktype = define("War3ApiJassCommonJ.attacktype", {
  __inherits__ = { handle }
})
local damagetype = define("War3ApiJassCommonJ.damagetype", {
  __inherits__ = { handle }
})
local weapontype = define("War3ApiJassCommonJ.weapontype", {
  __inherits__ = { handle }
})
local soundtype = define("War3ApiJassCommonJ.soundtype", {
  __inherits__ = { handle }
})
local lightning = define("War3ApiJassCommonJ.lightning", {
  __inherits__ = { handle }
})
local pathingtype = define("War3ApiJassCommonJ.pathingtype", {
  __inherits__ = { handle }
})
local mousebuttontype = define("War3ApiJassCommonJ.mousebuttontype", {
  __inherits__ = { handle }
})
local animtype = define("War3ApiJassCommonJ.animtype", {
  __inherits__ = { handle }
})
local subanimtype = define("War3ApiJassCommonJ.subanimtype", {
  __inherits__ = { handle }
})
local image = define("War3ApiJassCommonJ.image", {
  __inherits__ = { handle }
})
local ubersplat = define("War3ApiJassCommonJ.ubersplat", {
  __inherits__ = { handle }
})
local hashtable = define("War3ApiJassCommonJ.hashtable", {
  __inherits__ = { agent }
})
local framehandle = define("War3ApiJassCommonJ.framehandle", {
  __inherits__ = { handle }
})
local originframetype = define("War3ApiJassCommonJ.originframetype", {
  __inherits__ = { handle }
})
local framepointtype = define("War3ApiJassCommonJ.framepointtype", {
  __inherits__ = { handle }
})
local textaligntype = define("War3ApiJassCommonJ.textaligntype", {
  __inherits__ = { handle }
})
local frameeventtype = define("War3ApiJassCommonJ.frameeventtype", {
  __inherits__ = { handle }
})
local oskeytype = define("War3ApiJassCommonJ.oskeytype", {
  __inherits__ = { handle }
})
local abilityintegerfield = define("War3ApiJassCommonJ.abilityintegerfield", {
  __inherits__ = { handle }
})
local abilityrealfield = define("War3ApiJassCommonJ.abilityrealfield", {
  __inherits__ = { handle }
})
local abilitybooleanfield = define("War3ApiJassCommonJ.abilitybooleanfield", {
  __inherits__ = { handle }
})
local abilitystringfield = define("War3ApiJassCommonJ.abilitystringfield", {
  __inherits__ = { handle }
})
local abilityintegerlevelfield = define("War3ApiJassCommonJ.abilityintegerlevelfield", {
  __inherits__ = { handle }
})
local abilityreallevelfield = define("War3ApiJassCommonJ.abilityreallevelfield", {
  __inherits__ = { handle }
})
local abilitybooleanlevelfield = define("War3ApiJassCommonJ.abilitybooleanlevelfield", {
  __inherits__ = { handle }
})
local abilitystringlevelfield = define("War3ApiJassCommonJ.abilitystringlevelfield", {
  __inherits__ = { handle }
})
local abilityintegerlevelarrayfield = define("War3ApiJassCommonJ.abilityintegerlevelarrayfield", {
  __inherits__ = { handle }
})
local abilityreallevelarrayfield = define("War3ApiJassCommonJ.abilityreallevelarrayfield", {
  __inherits__ = { handle }
})
local abilitybooleanlevelarrayfield = define("War3ApiJassCommonJ.abilitybooleanlevelarrayfield", {
  __inherits__ = { handle }
})
local abilitystringlevelarrayfield = define("War3ApiJassCommonJ.abilitystringlevelarrayfield", {
  __inherits__ = { handle }
})
local unitintegerfield = define("War3ApiJassCommonJ.unitintegerfield", {
  __inherits__ = { handle }
})
local unitrealfield = define("War3ApiJassCommonJ.unitrealfield", {
  __inherits__ = { handle }
})
local unitbooleanfield = define("War3ApiJassCommonJ.unitbooleanfield", {
  __inherits__ = { handle }
})
local unitstringfield = define("War3ApiJassCommonJ.unitstringfield", {
  __inherits__ = { handle }
})
local unitweaponintegerfield = define("War3ApiJassCommonJ.unitweaponintegerfield", {
  __inherits__ = { handle }
})
local unitweaponrealfield = define("War3ApiJassCommonJ.unitweaponrealfield", {
  __inherits__ = { handle }
})
local unitweaponbooleanfield = define("War3ApiJassCommonJ.unitweaponbooleanfield", {
  __inherits__ = { handle }
})
local unitweaponstringfield = define("War3ApiJassCommonJ.unitweaponstringfield", {
  __inherits__ = { handle }
})
local itemintegerfield = define("War3ApiJassCommonJ.itemintegerfield", {
  __inherits__ = { handle }
})
local itemrealfield = define("War3ApiJassCommonJ.itemrealfield", {
  __inherits__ = { handle }
})
local itembooleanfield = define("War3ApiJassCommonJ.itembooleanfield", {
  __inherits__ = { handle }
})
local itemstringfield = define("War3ApiJassCommonJ.itemstringfield", {
  __inherits__ = { handle }
})
local movetype = define("War3ApiJassCommonJ.movetype", {
  __inherits__ = { handle }
})
local targetflag = define("War3ApiJassCommonJ.targetflag", {
  __inherits__ = { handle }
})
local armortype = define("War3ApiJassCommonJ.armortype", {
  __inherits__ = { handle }
})
local heroattribute = define("War3ApiJassCommonJ.heroattribute", {
  __inherits__ = { handle }
})
local defensetype = define("War3ApiJassCommonJ.defensetype", {
  __inherits__ = { handle }
})
local regentype = define("War3ApiJassCommonJ.regentype", {
  __inherits__ = { handle }
})
local unitcategory = define("War3ApiJassCommonJ.unitcategory", {
  __inherits__ = { handle }
})
local pathingflag = define("War3ApiJassCommonJ.pathingflag", {
  __inherits__ = { handle }
})
