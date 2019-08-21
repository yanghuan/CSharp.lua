local define = System.defStc
local setmetatable = setmetatable

local handle = define("War3ApiCommon.handle", {
})
local agent = define("War3ApiCommon.agent", {
  __inherits__ = { handle }
})
local event = define("War3ApiCommon.event", {
  __inherits__ = { agent }
})
local player = define("War3ApiCommon.player", {
  __inherits__ = { agent }
})
local widget = define("War3ApiCommon.widget", {
  __inherits__ = { agent }
})
local unit = define("War3ApiCommon.unit", {
  __inherits__ = { widget }
})
local destructable = define("War3ApiCommon.destructable", {
  __inherits__ = { widget }
})
local item = define("War3ApiCommon.item", {
  __inherits__ = { widget }
})
local ability = define("War3ApiCommon.ability", {
  __inherits__ = { agent }
})
local buff = define("War3ApiCommon.buff", {
  __inherits__ = { ability }
})
local force = define("War3ApiCommon.force", {
  __inherits__ = { agent }
})
local group = define("War3ApiCommon.group", {
  __inherits__ = { agent }
})
local trigger = define("War3ApiCommon.trigger", {
  __inherits__ = { agent }
})
local triggercondition = define("War3ApiCommon.triggercondition", {
  __inherits__ = { agent }
})
local triggeraction = define("War3ApiCommon.triggeraction", {
  __inherits__ = { handle }
})
local timer = define("War3ApiCommon.timer", {
  __inherits__ = { agent }
})
local location = define("War3ApiCommon.location", {
  __inherits__ = { agent }
})
local region = define("War3ApiCommon.region", {
  __inherits__ = { agent }
})
local rect = define("War3ApiCommon.rect", {
  __inherits__ = { agent }
})
local boolexpr = define("War3ApiCommon.boolexpr", {
  __inherits__ = { agent }
})
local sound = define("War3ApiCommon.sound", {
  __inherits__ = { agent }
})
local conditionfunc = define("War3ApiCommon.conditionfunc", {
  __inherits__ = { boolexpr }
})
local filterfunc = define("War3ApiCommon.filterfunc", {
  __inherits__ = { boolexpr }
})
local unitpool = define("War3ApiCommon.unitpool", {
  __inherits__ = { handle }
})
local itempool = define("War3ApiCommon.itempool", {
  __inherits__ = { handle }
})
local race = define("War3ApiCommon.race", {
  __inherits__ = { handle }
})
local alliancetype = define("War3ApiCommon.alliancetype", {
  __inherits__ = { handle }
})
local racepreference = define("War3ApiCommon.racepreference", {
  __inherits__ = { handle }
})
local gamestate = define("War3ApiCommon.gamestate", {
  __inherits__ = { handle }
})
local igamestate = define("War3ApiCommon.igamestate", {
  __inherits__ = { gamestate }
})
local fgamestate = define("War3ApiCommon.fgamestate", {
  __inherits__ = { gamestate }
})
local playerstate = define("War3ApiCommon.playerstate", {
  __inherits__ = { handle }
})
local playerscore = define("War3ApiCommon.playerscore", {
  __inherits__ = { handle }
})
local playergameresult = define("War3ApiCommon.playergameresult", {
  __inherits__ = { handle }
})
local unitstate = define("War3ApiCommon.unitstate", {
  __inherits__ = { handle }
})
local aidifficulty = define("War3ApiCommon.aidifficulty", {
  __inherits__ = { handle }
})
local eventid = define("War3ApiCommon.eventid", {
  __inherits__ = { handle }
})
local gameevent = define("War3ApiCommon.gameevent", {
  __inherits__ = { eventid }
})
local playerevent = define("War3ApiCommon.playerevent", {
  __inherits__ = { eventid }
})
local playerunitevent = define("War3ApiCommon.playerunitevent", {
  __inherits__ = { eventid }
})
local unitevent = define("War3ApiCommon.unitevent", {
  __inherits__ = { eventid }
})
local limitop = define("War3ApiCommon.limitop", {
  __inherits__ = { eventid }
})
local widgetevent = define("War3ApiCommon.widgetevent", {
  __inherits__ = { eventid }
})
local dialogevent = define("War3ApiCommon.dialogevent", {
  __inherits__ = { eventid }
})
local unittype = define("War3ApiCommon.unittype", {
  __inherits__ = { handle }
})
local gamespeed = define("War3ApiCommon.gamespeed", {
  __inherits__ = { handle }
})
local gamedifficulty = define("War3ApiCommon.gamedifficulty", {
  __inherits__ = { handle }
})
local gametype = define("War3ApiCommon.gametype", {
  __inherits__ = { handle }
})
local mapflag = define("War3ApiCommon.mapflag", {
  __inherits__ = { handle }
})
local mapvisibility = define("War3ApiCommon.mapvisibility", {
  __inherits__ = { handle }
})
local mapsetting = define("War3ApiCommon.mapsetting", {
  __inherits__ = { handle }
})
local mapdensity = define("War3ApiCommon.mapdensity", {
  __inherits__ = { handle }
})
local mapcontrol = define("War3ApiCommon.mapcontrol", {
  __inherits__ = { handle }
})
local playerslotstate = define("War3ApiCommon.playerslotstate", {
  __inherits__ = { handle }
})
local volumegroup = define("War3ApiCommon.volumegroup", {
  __inherits__ = { handle }
})
local camerafield = define("War3ApiCommon.camerafield", {
  __inherits__ = { handle }
})
local camerasetup = define("War3ApiCommon.camerasetup", {
  __inherits__ = { handle }
})
local playercolor = define("War3ApiCommon.playercolor", {
  __inherits__ = { handle }
})
local placement = define("War3ApiCommon.placement", {
  __inherits__ = { handle }
})
local startlocprio = define("War3ApiCommon.startlocprio", {
  __inherits__ = { handle }
})
local raritycontrol = define("War3ApiCommon.raritycontrol", {
  __inherits__ = { handle }
})
local blendmode = define("War3ApiCommon.blendmode", {
  __inherits__ = { handle }
})
local texmapflags = define("War3ApiCommon.texmapflags", {
  __inherits__ = { handle }
})
local effect = define("War3ApiCommon.effect", {
  __inherits__ = { agent }
})
local effecttype = define("War3ApiCommon.effecttype", {
  __inherits__ = { handle }
})
local weathereffect = define("War3ApiCommon.weathereffect", {
  __inherits__ = { handle }
})
local terraindeformation = define("War3ApiCommon.terraindeformation", {
  __inherits__ = { handle }
})
local fogstate = define("War3ApiCommon.fogstate", {
  __inherits__ = { handle }
})
local fogmodifier = define("War3ApiCommon.fogmodifier", {
  __inherits__ = { agent }
})
local dialog = define("War3ApiCommon.dialog", {
  __inherits__ = { agent }
})
local button = define("War3ApiCommon.button", {
  __inherits__ = { agent }
})
local quest = define("War3ApiCommon.quest", {
  __inherits__ = { agent }
})
local questitem = define("War3ApiCommon.questitem", {
  __inherits__ = { agent }
})
local defeatcondition = define("War3ApiCommon.defeatcondition", {
  __inherits__ = { agent }
})
local timerdialog = define("War3ApiCommon.timerdialog", {
  __inherits__ = { agent }
})
local leaderboard = define("War3ApiCommon.leaderboard", {
  __inherits__ = { agent }
})
local multiboard = define("War3ApiCommon.multiboard", {
  __inherits__ = { agent }
})
local multiboarditem = define("War3ApiCommon.multiboarditem", {
  __inherits__ = { agent }
})
local trackable = define("War3ApiCommon.trackable", {
  __inherits__ = { agent }
})
local gamecache = define("War3ApiCommon.gamecache", {
  __inherits__ = { agent }
})
local version = define("War3ApiCommon.version", {
  __inherits__ = { handle }
})
local itemtype = define("War3ApiCommon.itemtype", {
  __inherits__ = { handle }
})
local texttag = define("War3ApiCommon.texttag", {
  __inherits__ = { handle }
})
local attacktype = define("War3ApiCommon.attacktype", {
  __inherits__ = { handle }
})
local damagetype = define("War3ApiCommon.damagetype", {
  __inherits__ = { handle }
})
local weapontype = define("War3ApiCommon.weapontype", {
  __inherits__ = { handle }
})
local soundtype = define("War3ApiCommon.soundtype", {
  __inherits__ = { handle }
})
local lightning = define("War3ApiCommon.lightning", {
  __inherits__ = { handle }
})
local pathingtype = define("War3ApiCommon.pathingtype", {
  __inherits__ = { handle }
})
local mousebuttontype = define("War3ApiCommon.mousebuttontype", {
  __inherits__ = { handle }
})
local animtype = define("War3ApiCommon.animtype", {
  __inherits__ = { handle }
})
local subanimtype = define("War3ApiCommon.subanimtype", {
  __inherits__ = { handle }
})
local image = define("War3ApiCommon.image", {
  __inherits__ = { handle }
})
local ubersplat = define("War3ApiCommon.ubersplat", {
  __inherits__ = { handle }
})
local hashtable = define("War3ApiCommon.hashtable", {
  __inherits__ = { agent }
})
local framehandle = define("War3ApiCommon.framehandle", {
  __inherits__ = { handle }
})
local originframetype = define("War3ApiCommon.originframetype", {
  __inherits__ = { handle }
})
local framepointtype = define("War3ApiCommon.framepointtype", {
  __inherits__ = { handle }
})
local textaligntype = define("War3ApiCommon.textaligntype", {
  __inherits__ = { handle }
})
local frameeventtype = define("War3ApiCommon.frameeventtype", {
  __inherits__ = { handle }
})
local oskeytype = define("War3ApiCommon.oskeytype", {
  __inherits__ = { handle }
})
local abilityintegerfield = define("War3ApiCommon.abilityintegerfield", {
  __inherits__ = { handle }
})
local abilityrealfield = define("War3ApiCommon.abilityrealfield", {
  __inherits__ = { handle }
})
local abilitybooleanfield = define("War3ApiCommon.abilitybooleanfield", {
  __inherits__ = { handle }
})
local abilitystringfield = define("War3ApiCommon.abilitystringfield", {
  __inherits__ = { handle }
})
local abilityintegerlevelfield = define("War3ApiCommon.abilityintegerlevelfield", {
  __inherits__ = { handle }
})
local abilityreallevelfield = define("War3ApiCommon.abilityreallevelfield", {
  __inherits__ = { handle }
})
local abilitybooleanlevelfield = define("War3ApiCommon.abilitybooleanlevelfield", {
  __inherits__ = { handle }
})
local abilitystringlevelfield = define("War3ApiCommon.abilitystringlevelfield", {
  __inherits__ = { handle }
})
local abilityintegerlevelarrayfield = define("War3ApiCommon.abilityintegerlevelarrayfield", {
  __inherits__ = { handle }
})
local abilityreallevelarrayfield = define("War3ApiCommon.abilityreallevelarrayfield", {
  __inherits__ = { handle }
})
local abilitybooleanlevelarrayfield = define("War3ApiCommon.abilitybooleanlevelarrayfield", {
  __inherits__ = { handle }
})
local abilitystringlevelarrayfield = define("War3ApiCommon.abilitystringlevelarrayfield", {
  __inherits__ = { handle }
})
local unitintegerfield = define("War3ApiCommon.unitintegerfield", {
  __inherits__ = { handle }
})
local unitrealfield = define("War3ApiCommon.unitrealfield", {
  __inherits__ = { handle }
})
local unitbooleanfield = define("War3ApiCommon.unitbooleanfield", {
  __inherits__ = { handle }
})
local unitstringfield = define("War3ApiCommon.unitstringfield", {
  __inherits__ = { handle }
})
local unitweaponintegerfield = define("War3ApiCommon.unitweaponintegerfield", {
  __inherits__ = { handle }
})
local unitweaponrealfield = define("War3ApiCommon.unitweaponrealfield", {
  __inherits__ = { handle }
})
local unitweaponbooleanfield = define("War3ApiCommon.unitweaponbooleanfield", {
  __inherits__ = { handle }
})
local unitweaponstringfield = define("War3ApiCommon.unitweaponstringfield", {
  __inherits__ = { handle }
})
local itemintegerfield = define("War3ApiCommon.itemintegerfield", {
  __inherits__ = { handle }
})
local itemrealfield = define("War3ApiCommon.itemrealfield", {
  __inherits__ = { handle }
})
local itembooleanfield = define("War3ApiCommon.itembooleanfield", {
  __inherits__ = { handle }
})
local itemstringfield = define("War3ApiCommon.itemstringfield", {
  __inherits__ = { handle }
})
local movetype = define("War3ApiCommon.movetype", {
  __inherits__ = { handle }
})
local targetflag = define("War3ApiCommon.targetflag", {
  __inherits__ = { handle }
})
local armortype = define("War3ApiCommon.armortype", {
  __inherits__ = { handle }
})
local heroattribute = define("War3ApiCommon.heroattribute", {
  __inherits__ = { handle }
})
local defensetype = define("War3ApiCommon.defensetype", {
  __inherits__ = { handle }
})
local regentype = define("War3ApiCommon.regentype", {
  __inherits__ = { handle }
})
local unitcategory = define("War3ApiCommon.unitcategory", {
  __inherits__ = { handle }
})
local pathingflag = define("War3ApiCommon.pathingflag", {
  __inherits__ = { handle }
})
