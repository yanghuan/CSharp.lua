#pragma warning disable IDE0052, IDE1006, CS0626
using CSharpLua;

namespace War3Api {
  public static class Common {
    [NativeLuaMemberAttribute]
    public class agent {
      internal agent() {
      }
    }

    [NativeLuaMemberAttribute]
    public class @event : agent {
      internal @event() {
      }
    }

    [NativeLuaMemberAttribute]
    public class player : agent {
      internal player() {
      }
    }

    [NativeLuaMemberAttribute]
    public class widget : agent {
      internal widget() {
      }
    }

    [NativeLuaMemberAttribute]
    public class unit : widget {
      internal unit() {
      }
    }

    [NativeLuaMemberAttribute]
    public class destructable : widget {
      internal destructable() {
      }
    }

    [NativeLuaMemberAttribute]
    public class item : widget {
      internal item() {
      }
    }

    [NativeLuaMemberAttribute]
    public class ability : agent {
      internal ability() {
      }
    }

    [NativeLuaMemberAttribute]
    public class buff : ability {
      internal buff() {
      }
    }

    [NativeLuaMemberAttribute]
    public class force : agent {
      internal force() {
      }
    }

    [NativeLuaMemberAttribute]
    public class group : agent {
      internal group() {
      }
    }

    [NativeLuaMemberAttribute]
    public class trigger : agent {
      internal trigger() {
      }
    }

    [NativeLuaMemberAttribute]
    public class triggercondition : agent {
      internal triggercondition() {
      }
    }

    [NativeLuaMemberAttribute]
    public class triggeraction {
      internal triggeraction() {
      }
    }

    [NativeLuaMemberAttribute]
    public class timer : agent {
      internal timer() {
      }
    }

    [NativeLuaMemberAttribute]
    public class location : agent {
      internal location() {
      }
    }

    [NativeLuaMemberAttribute]
    public class region : agent {
      internal region() {
      }
    }

    [NativeLuaMemberAttribute]
    public class rect : agent {
      internal rect() {
      }
    }

    [NativeLuaMemberAttribute]
    public class boolexpr : agent {
      internal boolexpr() {
      }
    }

    [NativeLuaMemberAttribute]
    public class sound : agent {
      internal sound() {
      }
    }

    [NativeLuaMemberAttribute]
    public class conditionfunc : boolexpr {
      internal conditionfunc() {
      }
    }

    [NativeLuaMemberAttribute]
    public class filterfunc : boolexpr {
      internal filterfunc() {
      }
    }

    [NativeLuaMemberAttribute]
    public class unitpool {
      internal unitpool() {
      }
    }

    [NativeLuaMemberAttribute]
    public class itempool {
      internal itempool() {
      }
    }

    [NativeLuaMemberAttribute]
    public class race {
      internal race() {
      }
    }

    [NativeLuaMemberAttribute]
    public class alliancetype {
      internal alliancetype() {
      }
    }

    [NativeLuaMemberAttribute]
    public class racepreference {
      internal racepreference() {
      }
    }

    [NativeLuaMemberAttribute]
    public class gamestate {
      internal gamestate() {
      }
    }

    [NativeLuaMemberAttribute]
    public class igamestate : gamestate {
      internal igamestate() {
      }
    }

    [NativeLuaMemberAttribute]
    public class fgamestate : gamestate {
      internal fgamestate() {
      }
    }

    [NativeLuaMemberAttribute]
    public class playerstate {
      internal playerstate() {
      }
    }

    [NativeLuaMemberAttribute]
    public class playerscore {
      internal playerscore() {
      }
    }

    [NativeLuaMemberAttribute]
    public class playergameresult {
      internal playergameresult() {
      }
    }

    [NativeLuaMemberAttribute]
    public class unitstate {
      internal unitstate() {
      }
    }

    [NativeLuaMemberAttribute]
    public class aidifficulty {
      internal aidifficulty() {
      }
    }

    [NativeLuaMemberAttribute]
    public class eventid {
      internal eventid() {
      }
    }

    [NativeLuaMemberAttribute]
    public class gameevent : eventid {
      internal gameevent() {
      }
    }

    [NativeLuaMemberAttribute]
    public class playerevent : eventid {
      internal playerevent() {
      }
    }

    [NativeLuaMemberAttribute]
    public class playerunitevent : eventid {
      internal playerunitevent() {
      }
    }

    [NativeLuaMemberAttribute]
    public class unitevent : eventid {
      internal unitevent() {
      }
    }

    [NativeLuaMemberAttribute]
    public class limitop : eventid {
      internal limitop() {
      }
    }

    [NativeLuaMemberAttribute]
    public class widgetevent : eventid {
      internal widgetevent() {
      }
    }

    [NativeLuaMemberAttribute]
    public class dialogevent : eventid {
      internal dialogevent() {
      }
    }

    [NativeLuaMemberAttribute]
    public class unittype {
      internal unittype() {
      }
    }

    [NativeLuaMemberAttribute]
    public class gamespeed {
      internal gamespeed() {
      }
    }

    [NativeLuaMemberAttribute]
    public class gamedifficulty {
      internal gamedifficulty() {
      }
    }

    [NativeLuaMemberAttribute]
    public class gametype {
      internal gametype() {
      }
    }

    [NativeLuaMemberAttribute]
    public class mapflag {
      internal mapflag() {
      }
    }

    [NativeLuaMemberAttribute]
    public class mapvisibility {
      internal mapvisibility() {
      }
    }

    [NativeLuaMemberAttribute]
    public class mapsetting {
      internal mapsetting() {
      }
    }

    [NativeLuaMemberAttribute]
    public class mapdensity {
      internal mapdensity() {
      }
    }

    [NativeLuaMemberAttribute]
    public class mapcontrol {
      internal mapcontrol() {
      }
    }

    [NativeLuaMemberAttribute]
    public class playerslotstate {
      internal playerslotstate() {
      }
    }

    [NativeLuaMemberAttribute]
    public class volumegroup {
      internal volumegroup() {
      }
    }

    [NativeLuaMemberAttribute]
    public class camerafield {
      internal camerafield() {
      }
    }

    [NativeLuaMemberAttribute]
    public class camerasetup {
      internal camerasetup() {
      }
    }

    [NativeLuaMemberAttribute]
    public class playercolor {
      internal playercolor() {
      }
    }

    [NativeLuaMemberAttribute]
    public class placement {
      internal placement() {
      }
    }

    [NativeLuaMemberAttribute]
    public class startlocprio {
      internal startlocprio() {
      }
    }

    [NativeLuaMemberAttribute]
    public class raritycontrol {
      internal raritycontrol() {
      }
    }

    [NativeLuaMemberAttribute]
    public class blendmode {
      internal blendmode() {
      }
    }

    [NativeLuaMemberAttribute]
    public class texmapflags {
      internal texmapflags() {
      }
    }

    [NativeLuaMemberAttribute]
    public class effect : agent {
      internal effect() {
      }
    }

    [NativeLuaMemberAttribute]
    public class effecttype {
      internal effecttype() {
      }
    }

    [NativeLuaMemberAttribute]
    public class weathereffect {
      internal weathereffect() {
      }
    }

    [NativeLuaMemberAttribute]
    public class terraindeformation {
      internal terraindeformation() {
      }
    }

    [NativeLuaMemberAttribute]
    public class fogstate {
      internal fogstate() {
      }
    }

    [NativeLuaMemberAttribute]
    public class fogmodifier : agent {
      internal fogmodifier() {
      }
    }

    [NativeLuaMemberAttribute]
    public class dialog : agent {
      internal dialog() {
      }
    }

    [NativeLuaMemberAttribute]
    public class button : agent {
      internal button() {
      }
    }

    [NativeLuaMemberAttribute]
    public class quest : agent {
      internal quest() {
      }
    }

    [NativeLuaMemberAttribute]
    public class questitem : agent {
      internal questitem() {
      }
    }

    [NativeLuaMemberAttribute]
    public class defeatcondition : agent {
      internal defeatcondition() {
      }
    }

    [NativeLuaMemberAttribute]
    public class timerdialog : agent {
      internal timerdialog() {
      }
    }

    [NativeLuaMemberAttribute]
    public class leaderboard : agent {
      internal leaderboard() {
      }
    }

    [NativeLuaMemberAttribute]
    public class multiboard : agent {
      internal multiboard() {
      }
    }

    [NativeLuaMemberAttribute]
    public class multiboarditem : agent {
      internal multiboarditem() {
      }
    }

    [NativeLuaMemberAttribute]
    public class trackable : agent {
      internal trackable() {
      }
    }

    [NativeLuaMemberAttribute]
    public class gamecache : agent {
      internal gamecache() {
      }
    }

    [NativeLuaMemberAttribute]
    public class version {
      internal version() {
      }
    }

    [NativeLuaMemberAttribute]
    public class itemtype {
      internal itemtype() {
      }
    }

    [NativeLuaMemberAttribute]
    public class texttag {
      internal texttag() {
      }
    }

    [NativeLuaMemberAttribute]
    public class attacktype {
      internal attacktype() {
      }
    }

    [NativeLuaMemberAttribute]
    public class damagetype {
      internal damagetype() {
      }
    }

    [NativeLuaMemberAttribute]
    public class weapontype {
      internal weapontype() {
      }
    }

    [NativeLuaMemberAttribute]
    public class soundtype {
      internal soundtype() {
      }
    }

    [NativeLuaMemberAttribute]
    public class lightning {
      internal lightning() {
      }
    }

    [NativeLuaMemberAttribute]
    public class pathingtype {
      internal pathingtype() {
      }
    }

    [NativeLuaMemberAttribute]
    public class mousebuttontype {
      internal mousebuttontype() {
      }
    }

    [NativeLuaMemberAttribute]
    public class animtype {
      internal animtype() {
      }
    }

    [NativeLuaMemberAttribute]
    public class subanimtype {
      internal subanimtype() {
      }
    }

    [NativeLuaMemberAttribute]
    public class image {
      internal image() {
      }
    }

    [NativeLuaMemberAttribute]
    public class ubersplat {
      internal ubersplat() {
      }
    }

    [NativeLuaMemberAttribute]
    public class hashtable : agent {
      internal hashtable() {
      }
    }

    [NativeLuaMemberAttribute]
    public class framehandle {
      internal framehandle() {
      }
    }

    [NativeLuaMemberAttribute]
    public class originframetype {
      internal originframetype() {
      }
    }

    [NativeLuaMemberAttribute]
    public class framepointtype {
      internal framepointtype() {
      }
    }

    [NativeLuaMemberAttribute]
    public class textaligntype {
      internal textaligntype() {
      }
    }

    [NativeLuaMemberAttribute]
    public class frameeventtype {
      internal frameeventtype() {
      }
    }

    [NativeLuaMemberAttribute]
    public class oskeytype {
      internal oskeytype() {
      }
    }

    [NativeLuaMemberAttribute]
    public class abilityintegerfield {
      internal abilityintegerfield() {
      }
    }

    [NativeLuaMemberAttribute]
    public class abilityrealfield {
      internal abilityrealfield() {
      }
    }

    [NativeLuaMemberAttribute]
    public class abilitybooleanfield {
      internal abilitybooleanfield() {
      }
    }

    [NativeLuaMemberAttribute]
    public class abilitystringfield {
      internal abilitystringfield() {
      }
    }

    [NativeLuaMemberAttribute]
    public class abilityintegerlevelfield {
      internal abilityintegerlevelfield() {
      }
    }

    [NativeLuaMemberAttribute]
    public class abilityreallevelfield {
      internal abilityreallevelfield() {
      }
    }

    [NativeLuaMemberAttribute]
    public class abilitybooleanlevelfield {
      internal abilitybooleanlevelfield() {
      }
    }

    [NativeLuaMemberAttribute]
    public class abilitystringlevelfield {
      internal abilitystringlevelfield() {
      }
    }

    [NativeLuaMemberAttribute]
    public class abilityintegerlevelarrayfield {
      internal abilityintegerlevelarrayfield() {
      }
    }

    [NativeLuaMemberAttribute]
    public class abilityreallevelarrayfield {
      internal abilityreallevelarrayfield() {
      }
    }

    [NativeLuaMemberAttribute]
    public class abilitybooleanlevelarrayfield {
      internal abilitybooleanlevelarrayfield() {
      }
    }

    [NativeLuaMemberAttribute]
    public class abilitystringlevelarrayfield {
      internal abilitystringlevelarrayfield() {
      }
    }

    [NativeLuaMemberAttribute]
    public class unitintegerfield {
      internal unitintegerfield() {
      }
    }

    [NativeLuaMemberAttribute]
    public class unitrealfield {
      internal unitrealfield() {
      }
    }

    [NativeLuaMemberAttribute]
    public class unitbooleanfield {
      internal unitbooleanfield() {
      }
    }

    [NativeLuaMemberAttribute]
    public class unitstringfield {
      internal unitstringfield() {
      }
    }

    [NativeLuaMemberAttribute]
    public class unitweaponintegerfield {
      internal unitweaponintegerfield() {
      }
    }

    [NativeLuaMemberAttribute]
    public class unitweaponrealfield {
      internal unitweaponrealfield() {
      }
    }

    [NativeLuaMemberAttribute]
    public class unitweaponbooleanfield {
      internal unitweaponbooleanfield() {
      }
    }

    [NativeLuaMemberAttribute]
    public class unitweaponstringfield {
      internal unitweaponstringfield() {
      }
    }

    [NativeLuaMemberAttribute]
    public class itemintegerfield {
      internal itemintegerfield() {
      }
    }

    [NativeLuaMemberAttribute]
    public class itemrealfield {
      internal itemrealfield() {
      }
    }

    [NativeLuaMemberAttribute]
    public class itembooleanfield {
      internal itembooleanfield() {
      }
    }

    [NativeLuaMemberAttribute]
    public class itemstringfield {
      internal itemstringfield() {
      }
    }

    [NativeLuaMemberAttribute]
    public class movetype {
      internal movetype() {
      }
    }

    [NativeLuaMemberAttribute]
    public class targetflag {
      internal targetflag() {
      }
    }

    [NativeLuaMemberAttribute]
    public class armortype {
      internal armortype() {
      }
    }

    [NativeLuaMemberAttribute]
    public class heroattribute {
      internal heroattribute() {
      }
    }

    [NativeLuaMemberAttribute]
    public class defensetype {
      internal defensetype() {
      }
    }

    [NativeLuaMemberAttribute]
    public class regentype {
      internal regentype() {
      }
    }

    [NativeLuaMemberAttribute]
    public class unitcategory {
      internal unitcategory() {
      }
    }

    [NativeLuaMemberAttribute]
    public class pathingflag {
      internal pathingflag() {
      }
    }

    [NativeLuaMemberAttribute]
    public static extern race ConvertRace(int i);
    [NativeLuaMemberAttribute]
    public static extern alliancetype ConvertAllianceType(int i);
    [NativeLuaMemberAttribute]
    public static extern racepreference ConvertRacePref(int i);
    [NativeLuaMemberAttribute]
    public static extern igamestate ConvertIGameState(int i);
    [NativeLuaMemberAttribute]
    public static extern fgamestate ConvertFGameState(int i);
    [NativeLuaMemberAttribute]
    public static extern playerstate ConvertPlayerState(int i);
    [NativeLuaMemberAttribute]
    public static extern playerscore ConvertPlayerScore(int i);
    [NativeLuaMemberAttribute]
    public static extern playergameresult ConvertPlayerGameResult(int i);
    [NativeLuaMemberAttribute]
    public static extern unitstate ConvertUnitState(int i);
    [NativeLuaMemberAttribute]
    public static extern aidifficulty ConvertAIDifficulty(int i);
    [NativeLuaMemberAttribute]
    public static extern gameevent ConvertGameEvent(int i);
    [NativeLuaMemberAttribute]
    public static extern playerevent ConvertPlayerEvent(int i);
    [NativeLuaMemberAttribute]
    public static extern playerunitevent ConvertPlayerUnitEvent(int i);
    [NativeLuaMemberAttribute]
    public static extern widgetevent ConvertWidgetEvent(int i);
    [NativeLuaMemberAttribute]
    public static extern dialogevent ConvertDialogEvent(int i);
    [NativeLuaMemberAttribute]
    public static extern unitevent ConvertUnitEvent(int i);
    [NativeLuaMemberAttribute]
    public static extern limitop ConvertLimitOp(int i);
    [NativeLuaMemberAttribute]
    public static extern unittype ConvertUnitType(int i);
    [NativeLuaMemberAttribute]
    public static extern gamespeed ConvertGameSpeed(int i);
    [NativeLuaMemberAttribute]
    public static extern placement ConvertPlacement(int i);
    [NativeLuaMemberAttribute]
    public static extern startlocprio ConvertStartLocPrio(int i);
    [NativeLuaMemberAttribute]
    public static extern gamedifficulty ConvertGameDifficulty(int i);
    [NativeLuaMemberAttribute]
    public static extern gametype ConvertGameType(int i);
    [NativeLuaMemberAttribute]
    public static extern mapflag ConvertMapFlag(int i);
    [NativeLuaMemberAttribute]
    public static extern mapvisibility ConvertMapVisibility(int i);
    [NativeLuaMemberAttribute]
    public static extern mapsetting ConvertMapSetting(int i);
    [NativeLuaMemberAttribute]
    public static extern mapdensity ConvertMapDensity(int i);
    [NativeLuaMemberAttribute]
    public static extern mapcontrol ConvertMapControl(int i);
    [NativeLuaMemberAttribute]
    public static extern playercolor ConvertPlayerColor(int i);
    [NativeLuaMemberAttribute]
    public static extern playerslotstate ConvertPlayerSlotState(int i);
    [NativeLuaMemberAttribute]
    public static extern volumegroup ConvertVolumeGroup(int i);
    [NativeLuaMemberAttribute]
    public static extern camerafield ConvertCameraField(int i);
    [NativeLuaMemberAttribute]
    public static extern blendmode ConvertBlendMode(int i);
    [NativeLuaMemberAttribute]
    public static extern raritycontrol ConvertRarityControl(int i);
    [NativeLuaMemberAttribute]
    public static extern texmapflags ConvertTexMapFlags(int i);
    [NativeLuaMemberAttribute]
    public static extern fogstate ConvertFogState(int i);
    [NativeLuaMemberAttribute]
    public static extern effecttype ConvertEffectType(int i);
    [NativeLuaMemberAttribute]
    public static extern version ConvertVersion(int i);
    [NativeLuaMemberAttribute]
    public static extern itemtype ConvertItemType(int i);
    [NativeLuaMemberAttribute]
    public static extern attacktype ConvertAttackType(int i);
    [NativeLuaMemberAttribute]
    public static extern damagetype ConvertDamageType(int i);
    [NativeLuaMemberAttribute]
    public static extern weapontype ConvertWeaponType(int i);
    [NativeLuaMemberAttribute]
    public static extern soundtype ConvertSoundType(int i);
    [NativeLuaMemberAttribute]
    public static extern pathingtype ConvertPathingType(int i);
    [NativeLuaMemberAttribute]
    public static extern mousebuttontype ConvertMouseButtonType(int i);
    [NativeLuaMemberAttribute]
    public static extern animtype ConvertAnimType(int i);
    [NativeLuaMemberAttribute]
    public static extern subanimtype ConvertSubAnimType(int i);
    [NativeLuaMemberAttribute]
    public static extern originframetype ConvertOriginFrameType(int i);
    [NativeLuaMemberAttribute]
    public static extern framepointtype ConvertFramePointType(int i);
    [NativeLuaMemberAttribute]
    public static extern textaligntype ConvertTextAlignType(int i);
    [NativeLuaMemberAttribute]
    public static extern frameeventtype ConvertFrameEventType(int i);
    [NativeLuaMemberAttribute]
    public static extern oskeytype ConvertOsKeyType(int i);
    [NativeLuaMemberAttribute]
    public static extern abilityintegerfield ConvertAbilityIntegerField(int i);
    [NativeLuaMemberAttribute]
    public static extern abilityrealfield ConvertAbilityRealField(int i);
    [NativeLuaMemberAttribute]
    public static extern abilitybooleanfield ConvertAbilityBooleanField(int i);
    [NativeLuaMemberAttribute]
    public static extern abilitystringfield ConvertAbilityStringField(int i);
    [NativeLuaMemberAttribute]
    public static extern abilityintegerlevelfield ConvertAbilityIntegerLevelField(int i);
    [NativeLuaMemberAttribute]
    public static extern abilityreallevelfield ConvertAbilityRealLevelField(int i);
    [NativeLuaMemberAttribute]
    public static extern abilitybooleanlevelfield ConvertAbilityBooleanLevelField(int i);
    [NativeLuaMemberAttribute]
    public static extern abilitystringlevelfield ConvertAbilityStringLevelField(int i);
    [NativeLuaMemberAttribute]
    public static extern abilityintegerlevelarrayfield ConvertAbilityIntegerLevelArrayField(int i);
    [NativeLuaMemberAttribute]
    public static extern abilityreallevelarrayfield ConvertAbilityRealLevelArrayField(int i);
    [NativeLuaMemberAttribute]
    public static extern abilitybooleanlevelarrayfield ConvertAbilityBooleanLevelArrayField(int i);
    [NativeLuaMemberAttribute]
    public static extern abilitystringlevelarrayfield ConvertAbilityStringLevelArrayField(int i);
    [NativeLuaMemberAttribute]
    public static extern unitintegerfield ConvertUnitIntegerField(int i);
    [NativeLuaMemberAttribute]
    public static extern unitrealfield ConvertUnitRealField(int i);
    [NativeLuaMemberAttribute]
    public static extern unitbooleanfield ConvertUnitBooleanField(int i);
    [NativeLuaMemberAttribute]
    public static extern unitstringfield ConvertUnitStringField(int i);
    [NativeLuaMemberAttribute]
    public static extern unitweaponintegerfield ConvertUnitWeaponIntegerField(int i);
    [NativeLuaMemberAttribute]
    public static extern unitweaponrealfield ConvertUnitWeaponRealField(int i);
    [NativeLuaMemberAttribute]
    public static extern unitweaponbooleanfield ConvertUnitWeaponBooleanField(int i);
    [NativeLuaMemberAttribute]
    public static extern unitweaponstringfield ConvertUnitWeaponStringField(int i);
    [NativeLuaMemberAttribute]
    public static extern itemintegerfield ConvertItemIntegerField(int i);
    [NativeLuaMemberAttribute]
    public static extern itemrealfield ConvertItemRealField(int i);
    [NativeLuaMemberAttribute]
    public static extern itembooleanfield ConvertItemBooleanField(int i);
    [NativeLuaMemberAttribute]
    public static extern itemstringfield ConvertItemStringField(int i);
    [NativeLuaMemberAttribute]
    public static extern movetype ConvertMoveType(int i);
    [NativeLuaMemberAttribute]
    public static extern targetflag ConvertTargetFlag(int i);
    [NativeLuaMemberAttribute]
    public static extern armortype ConvertArmorType(int i);
    [NativeLuaMemberAttribute]
    public static extern heroattribute ConvertHeroAttribute(int i);
    [NativeLuaMemberAttribute]
    public static extern defensetype ConvertDefenseType(int i);
    [NativeLuaMemberAttribute]
    public static extern regentype ConvertRegenType(int i);
    [NativeLuaMemberAttribute]
    public static extern unitcategory ConvertUnitCategory(int i);
    [NativeLuaMemberAttribute]
    public static extern pathingflag ConvertPathingFlag(int i);
    [NativeLuaMemberAttribute]
    public static extern int OrderId(string orderIdString);
    [NativeLuaMemberAttribute]
    public static extern string OrderId2String(int orderId);
    [NativeLuaMemberAttribute]
    public static extern int UnitId(string unitIdString);
    [NativeLuaMemberAttribute]
    public static extern string UnitId2String(int unitId);
    [NativeLuaMemberAttribute]
    public static extern int AbilityId(string abilityIdString);
    [NativeLuaMemberAttribute]
    public static extern string AbilityId2String(int abilityId);
    [NativeLuaMemberAttribute]
    public static extern string GetObjectName(int objectId);
    [NativeLuaMemberAttribute]
    public static extern int GetBJMaxPlayers();
    [NativeLuaMemberAttribute]
    public static extern int GetBJPlayerNeutralVictim();
    [NativeLuaMemberAttribute]
    public static extern int GetBJPlayerNeutralExtra();
    [NativeLuaMemberAttribute]
    public static extern int GetBJMaxPlayerSlots();
    [NativeLuaMemberAttribute]
    public static extern int GetPlayerNeutralPassive();
    [NativeLuaMemberAttribute]
    public static extern int GetPlayerNeutralAggressive();
    [NativeLuaMemberAttribute]
    public const bool FALSE = false;
    [NativeLuaMemberAttribute]
    public const bool TRUE = true;
    [NativeLuaMemberAttribute]
    public const int JASS_MAX_ARRAY_SIZE = 32768;
    [NativeLuaMemberAttribute]
    public static readonly int PLAYER_NEUTRAL_PASSIVE = GetPlayerNeutralPassive();
    [NativeLuaMemberAttribute]
    public static readonly int PLAYER_NEUTRAL_AGGRESSIVE = GetPlayerNeutralAggressive();
    [NativeLuaMemberAttribute]
    public static readonly playercolor PLAYER_COLOR_RED = ConvertPlayerColor(0);
    [NativeLuaMemberAttribute]
    public static readonly playercolor PLAYER_COLOR_BLUE = ConvertPlayerColor(1);
    [NativeLuaMemberAttribute]
    public static readonly playercolor PLAYER_COLOR_CYAN = ConvertPlayerColor(2);
    [NativeLuaMemberAttribute]
    public static readonly playercolor PLAYER_COLOR_PURPLE = ConvertPlayerColor(3);
    [NativeLuaMemberAttribute]
    public static readonly playercolor PLAYER_COLOR_YELLOW = ConvertPlayerColor(4);
    [NativeLuaMemberAttribute]
    public static readonly playercolor PLAYER_COLOR_ORANGE = ConvertPlayerColor(5);
    [NativeLuaMemberAttribute]
    public static readonly playercolor PLAYER_COLOR_GREEN = ConvertPlayerColor(6);
    [NativeLuaMemberAttribute]
    public static readonly playercolor PLAYER_COLOR_PINK = ConvertPlayerColor(7);
    [NativeLuaMemberAttribute]
    public static readonly playercolor PLAYER_COLOR_LIGHT_GRAY = ConvertPlayerColor(8);
    [NativeLuaMemberAttribute]
    public static readonly playercolor PLAYER_COLOR_LIGHT_BLUE = ConvertPlayerColor(9);
    [NativeLuaMemberAttribute]
    public static readonly playercolor PLAYER_COLOR_AQUA = ConvertPlayerColor(10);
    [NativeLuaMemberAttribute]
    public static readonly playercolor PLAYER_COLOR_BROWN = ConvertPlayerColor(11);
    [NativeLuaMemberAttribute]
    public static readonly playercolor PLAYER_COLOR_MAROON = ConvertPlayerColor(12);
    [NativeLuaMemberAttribute]
    public static readonly playercolor PLAYER_COLOR_NAVY = ConvertPlayerColor(13);
    [NativeLuaMemberAttribute]
    public static readonly playercolor PLAYER_COLOR_TURQUOISE = ConvertPlayerColor(14);
    [NativeLuaMemberAttribute]
    public static readonly playercolor PLAYER_COLOR_VIOLET = ConvertPlayerColor(15);
    [NativeLuaMemberAttribute]
    public static readonly playercolor PLAYER_COLOR_WHEAT = ConvertPlayerColor(16);
    [NativeLuaMemberAttribute]
    public static readonly playercolor PLAYER_COLOR_PEACH = ConvertPlayerColor(17);
    [NativeLuaMemberAttribute]
    public static readonly playercolor PLAYER_COLOR_MINT = ConvertPlayerColor(18);
    [NativeLuaMemberAttribute]
    public static readonly playercolor PLAYER_COLOR_LAVENDER = ConvertPlayerColor(19);
    [NativeLuaMemberAttribute]
    public static readonly playercolor PLAYER_COLOR_COAL = ConvertPlayerColor(20);
    [NativeLuaMemberAttribute]
    public static readonly playercolor PLAYER_COLOR_SNOW = ConvertPlayerColor(21);
    [NativeLuaMemberAttribute]
    public static readonly playercolor PLAYER_COLOR_EMERALD = ConvertPlayerColor(22);
    [NativeLuaMemberAttribute]
    public static readonly playercolor PLAYER_COLOR_PEANUT = ConvertPlayerColor(23);
    [NativeLuaMemberAttribute]
    public static readonly race RACE_HUMAN = ConvertRace(1);
    [NativeLuaMemberAttribute]
    public static readonly race RACE_ORC = ConvertRace(2);
    [NativeLuaMemberAttribute]
    public static readonly race RACE_UNDEAD = ConvertRace(3);
    [NativeLuaMemberAttribute]
    public static readonly race RACE_NIGHTELF = ConvertRace(4);
    [NativeLuaMemberAttribute]
    public static readonly race RACE_DEMON = ConvertRace(5);
    [NativeLuaMemberAttribute]
    public static readonly race RACE_OTHER = ConvertRace(7);
    [NativeLuaMemberAttribute]
    public static readonly playergameresult PLAYER_GAME_RESULT_VICTORY = ConvertPlayerGameResult(0);
    [NativeLuaMemberAttribute]
    public static readonly playergameresult PLAYER_GAME_RESULT_DEFEAT = ConvertPlayerGameResult(1);
    [NativeLuaMemberAttribute]
    public static readonly playergameresult PLAYER_GAME_RESULT_TIE = ConvertPlayerGameResult(2);
    [NativeLuaMemberAttribute]
    public static readonly playergameresult PLAYER_GAME_RESULT_NEUTRAL = ConvertPlayerGameResult(3);
    [NativeLuaMemberAttribute]
    public static readonly alliancetype ALLIANCE_PASSIVE = ConvertAllianceType(0);
    [NativeLuaMemberAttribute]
    public static readonly alliancetype ALLIANCE_HELP_REQUEST = ConvertAllianceType(1);
    [NativeLuaMemberAttribute]
    public static readonly alliancetype ALLIANCE_HELP_RESPONSE = ConvertAllianceType(2);
    [NativeLuaMemberAttribute]
    public static readonly alliancetype ALLIANCE_SHARED_XP = ConvertAllianceType(3);
    [NativeLuaMemberAttribute]
    public static readonly alliancetype ALLIANCE_SHARED_SPELLS = ConvertAllianceType(4);
    [NativeLuaMemberAttribute]
    public static readonly alliancetype ALLIANCE_SHARED_VISION = ConvertAllianceType(5);
    [NativeLuaMemberAttribute]
    public static readonly alliancetype ALLIANCE_SHARED_CONTROL = ConvertAllianceType(6);
    [NativeLuaMemberAttribute]
    public static readonly alliancetype ALLIANCE_SHARED_ADVANCED_CONTROL = ConvertAllianceType(7);
    [NativeLuaMemberAttribute]
    public static readonly alliancetype ALLIANCE_RESCUABLE = ConvertAllianceType(8);
    [NativeLuaMemberAttribute]
    public static readonly alliancetype ALLIANCE_SHARED_VISION_FORCED = ConvertAllianceType(9);
    [NativeLuaMemberAttribute]
    public static readonly version VERSION_REIGN_OF_CHAOS = ConvertVersion(0);
    [NativeLuaMemberAttribute]
    public static readonly version VERSION_FROZEN_THRONE = ConvertVersion(1);
    [NativeLuaMemberAttribute]
    public static readonly attacktype ATTACK_TYPE_NORMAL = ConvertAttackType(0);
    [NativeLuaMemberAttribute]
    public static readonly attacktype ATTACK_TYPE_MELEE = ConvertAttackType(1);
    [NativeLuaMemberAttribute]
    public static readonly attacktype ATTACK_TYPE_PIERCE = ConvertAttackType(2);
    [NativeLuaMemberAttribute]
    public static readonly attacktype ATTACK_TYPE_SIEGE = ConvertAttackType(3);
    [NativeLuaMemberAttribute]
    public static readonly attacktype ATTACK_TYPE_MAGIC = ConvertAttackType(4);
    [NativeLuaMemberAttribute]
    public static readonly attacktype ATTACK_TYPE_CHAOS = ConvertAttackType(5);
    [NativeLuaMemberAttribute]
    public static readonly attacktype ATTACK_TYPE_HERO = ConvertAttackType(6);
    [NativeLuaMemberAttribute]
    public static readonly damagetype DAMAGE_TYPE_UNKNOWN = ConvertDamageType(0);
    [NativeLuaMemberAttribute]
    public static readonly damagetype DAMAGE_TYPE_NORMAL = ConvertDamageType(4);
    [NativeLuaMemberAttribute]
    public static readonly damagetype DAMAGE_TYPE_ENHANCED = ConvertDamageType(5);
    [NativeLuaMemberAttribute]
    public static readonly damagetype DAMAGE_TYPE_FIRE = ConvertDamageType(8);
    [NativeLuaMemberAttribute]
    public static readonly damagetype DAMAGE_TYPE_COLD = ConvertDamageType(9);
    [NativeLuaMemberAttribute]
    public static readonly damagetype DAMAGE_TYPE_LIGHTNING = ConvertDamageType(10);
    [NativeLuaMemberAttribute]
    public static readonly damagetype DAMAGE_TYPE_POISON = ConvertDamageType(11);
    [NativeLuaMemberAttribute]
    public static readonly damagetype DAMAGE_TYPE_DISEASE = ConvertDamageType(12);
    [NativeLuaMemberAttribute]
    public static readonly damagetype DAMAGE_TYPE_DIVINE = ConvertDamageType(13);
    [NativeLuaMemberAttribute]
    public static readonly damagetype DAMAGE_TYPE_MAGIC = ConvertDamageType(14);
    [NativeLuaMemberAttribute]
    public static readonly damagetype DAMAGE_TYPE_SONIC = ConvertDamageType(15);
    [NativeLuaMemberAttribute]
    public static readonly damagetype DAMAGE_TYPE_ACID = ConvertDamageType(16);
    [NativeLuaMemberAttribute]
    public static readonly damagetype DAMAGE_TYPE_FORCE = ConvertDamageType(17);
    [NativeLuaMemberAttribute]
    public static readonly damagetype DAMAGE_TYPE_DEATH = ConvertDamageType(18);
    [NativeLuaMemberAttribute]
    public static readonly damagetype DAMAGE_TYPE_MIND = ConvertDamageType(19);
    [NativeLuaMemberAttribute]
    public static readonly damagetype DAMAGE_TYPE_PLANT = ConvertDamageType(20);
    [NativeLuaMemberAttribute]
    public static readonly damagetype DAMAGE_TYPE_DEFENSIVE = ConvertDamageType(21);
    [NativeLuaMemberAttribute]
    public static readonly damagetype DAMAGE_TYPE_DEMOLITION = ConvertDamageType(22);
    [NativeLuaMemberAttribute]
    public static readonly damagetype DAMAGE_TYPE_SLOW_POISON = ConvertDamageType(23);
    [NativeLuaMemberAttribute]
    public static readonly damagetype DAMAGE_TYPE_SPIRIT_LINK = ConvertDamageType(24);
    [NativeLuaMemberAttribute]
    public static readonly damagetype DAMAGE_TYPE_SHADOW_STRIKE = ConvertDamageType(25);
    [NativeLuaMemberAttribute]
    public static readonly damagetype DAMAGE_TYPE_UNIVERSAL = ConvertDamageType(26);
    [NativeLuaMemberAttribute]
    public static readonly weapontype WEAPON_TYPE_WHOKNOWS = ConvertWeaponType(0);
    [NativeLuaMemberAttribute]
    public static readonly weapontype WEAPON_TYPE_METAL_LIGHT_CHOP = ConvertWeaponType(1);
    [NativeLuaMemberAttribute]
    public static readonly weapontype WEAPON_TYPE_METAL_MEDIUM_CHOP = ConvertWeaponType(2);
    [NativeLuaMemberAttribute]
    public static readonly weapontype WEAPON_TYPE_METAL_HEAVY_CHOP = ConvertWeaponType(3);
    [NativeLuaMemberAttribute]
    public static readonly weapontype WEAPON_TYPE_METAL_LIGHT_SLICE = ConvertWeaponType(4);
    [NativeLuaMemberAttribute]
    public static readonly weapontype WEAPON_TYPE_METAL_MEDIUM_SLICE = ConvertWeaponType(5);
    [NativeLuaMemberAttribute]
    public static readonly weapontype WEAPON_TYPE_METAL_HEAVY_SLICE = ConvertWeaponType(6);
    [NativeLuaMemberAttribute]
    public static readonly weapontype WEAPON_TYPE_METAL_MEDIUM_BASH = ConvertWeaponType(7);
    [NativeLuaMemberAttribute]
    public static readonly weapontype WEAPON_TYPE_METAL_HEAVY_BASH = ConvertWeaponType(8);
    [NativeLuaMemberAttribute]
    public static readonly weapontype WEAPON_TYPE_METAL_MEDIUM_STAB = ConvertWeaponType(9);
    [NativeLuaMemberAttribute]
    public static readonly weapontype WEAPON_TYPE_METAL_HEAVY_STAB = ConvertWeaponType(10);
    [NativeLuaMemberAttribute]
    public static readonly weapontype WEAPON_TYPE_WOOD_LIGHT_SLICE = ConvertWeaponType(11);
    [NativeLuaMemberAttribute]
    public static readonly weapontype WEAPON_TYPE_WOOD_MEDIUM_SLICE = ConvertWeaponType(12);
    [NativeLuaMemberAttribute]
    public static readonly weapontype WEAPON_TYPE_WOOD_HEAVY_SLICE = ConvertWeaponType(13);
    [NativeLuaMemberAttribute]
    public static readonly weapontype WEAPON_TYPE_WOOD_LIGHT_BASH = ConvertWeaponType(14);
    [NativeLuaMemberAttribute]
    public static readonly weapontype WEAPON_TYPE_WOOD_MEDIUM_BASH = ConvertWeaponType(15);
    [NativeLuaMemberAttribute]
    public static readonly weapontype WEAPON_TYPE_WOOD_HEAVY_BASH = ConvertWeaponType(16);
    [NativeLuaMemberAttribute]
    public static readonly weapontype WEAPON_TYPE_WOOD_LIGHT_STAB = ConvertWeaponType(17);
    [NativeLuaMemberAttribute]
    public static readonly weapontype WEAPON_TYPE_WOOD_MEDIUM_STAB = ConvertWeaponType(18);
    [NativeLuaMemberAttribute]
    public static readonly weapontype WEAPON_TYPE_CLAW_LIGHT_SLICE = ConvertWeaponType(19);
    [NativeLuaMemberAttribute]
    public static readonly weapontype WEAPON_TYPE_CLAW_MEDIUM_SLICE = ConvertWeaponType(20);
    [NativeLuaMemberAttribute]
    public static readonly weapontype WEAPON_TYPE_CLAW_HEAVY_SLICE = ConvertWeaponType(21);
    [NativeLuaMemberAttribute]
    public static readonly weapontype WEAPON_TYPE_AXE_MEDIUM_CHOP = ConvertWeaponType(22);
    [NativeLuaMemberAttribute]
    public static readonly weapontype WEAPON_TYPE_ROCK_HEAVY_BASH = ConvertWeaponType(23);
    [NativeLuaMemberAttribute]
    public static readonly pathingtype PATHING_TYPE_ANY = ConvertPathingType(0);
    [NativeLuaMemberAttribute]
    public static readonly pathingtype PATHING_TYPE_WALKABILITY = ConvertPathingType(1);
    [NativeLuaMemberAttribute]
    public static readonly pathingtype PATHING_TYPE_FLYABILITY = ConvertPathingType(2);
    [NativeLuaMemberAttribute]
    public static readonly pathingtype PATHING_TYPE_BUILDABILITY = ConvertPathingType(3);
    [NativeLuaMemberAttribute]
    public static readonly pathingtype PATHING_TYPE_PEONHARVESTPATHING = ConvertPathingType(4);
    [NativeLuaMemberAttribute]
    public static readonly pathingtype PATHING_TYPE_BLIGHTPATHING = ConvertPathingType(5);
    [NativeLuaMemberAttribute]
    public static readonly pathingtype PATHING_TYPE_FLOATABILITY = ConvertPathingType(6);
    [NativeLuaMemberAttribute]
    public static readonly pathingtype PATHING_TYPE_AMPHIBIOUSPATHING = ConvertPathingType(7);
    [NativeLuaMemberAttribute]
    public static readonly mousebuttontype MOUSE_BUTTON_TYPE_LEFT = ConvertMouseButtonType(1);
    [NativeLuaMemberAttribute]
    public static readonly mousebuttontype MOUSE_BUTTON_TYPE_MIDDLE = ConvertMouseButtonType(2);
    [NativeLuaMemberAttribute]
    public static readonly mousebuttontype MOUSE_BUTTON_TYPE_RIGHT = ConvertMouseButtonType(3);
    [NativeLuaMemberAttribute]
    public static readonly animtype ANIM_TYPE_BIRTH = ConvertAnimType(0);
    [NativeLuaMemberAttribute]
    public static readonly animtype ANIM_TYPE_DEATH = ConvertAnimType(1);
    [NativeLuaMemberAttribute]
    public static readonly animtype ANIM_TYPE_DECAY = ConvertAnimType(2);
    [NativeLuaMemberAttribute]
    public static readonly animtype ANIM_TYPE_DISSIPATE = ConvertAnimType(3);
    [NativeLuaMemberAttribute]
    public static readonly animtype ANIM_TYPE_STAND = ConvertAnimType(4);
    [NativeLuaMemberAttribute]
    public static readonly animtype ANIM_TYPE_WALK = ConvertAnimType(5);
    [NativeLuaMemberAttribute]
    public static readonly animtype ANIM_TYPE_ATTACK = ConvertAnimType(6);
    [NativeLuaMemberAttribute]
    public static readonly animtype ANIM_TYPE_MORPH = ConvertAnimType(7);
    [NativeLuaMemberAttribute]
    public static readonly animtype ANIM_TYPE_SLEEP = ConvertAnimType(8);
    [NativeLuaMemberAttribute]
    public static readonly animtype ANIM_TYPE_SPELL = ConvertAnimType(9);
    [NativeLuaMemberAttribute]
    public static readonly animtype ANIM_TYPE_PORTRAIT = ConvertAnimType(10);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_ROOTED = ConvertSubAnimType(11);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_ALTERNATE_EX = ConvertSubAnimType(12);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_LOOPING = ConvertSubAnimType(13);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_SLAM = ConvertSubAnimType(14);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_THROW = ConvertSubAnimType(15);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_SPIKED = ConvertSubAnimType(16);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_FAST = ConvertSubAnimType(17);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_SPIN = ConvertSubAnimType(18);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_READY = ConvertSubAnimType(19);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_CHANNEL = ConvertSubAnimType(20);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_DEFEND = ConvertSubAnimType(21);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_VICTORY = ConvertSubAnimType(22);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_TURN = ConvertSubAnimType(23);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_LEFT = ConvertSubAnimType(24);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_RIGHT = ConvertSubAnimType(25);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_FIRE = ConvertSubAnimType(26);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_FLESH = ConvertSubAnimType(27);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_HIT = ConvertSubAnimType(28);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_WOUNDED = ConvertSubAnimType(29);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_LIGHT = ConvertSubAnimType(30);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_MODERATE = ConvertSubAnimType(31);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_SEVERE = ConvertSubAnimType(32);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_CRITICAL = ConvertSubAnimType(33);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_COMPLETE = ConvertSubAnimType(34);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_GOLD = ConvertSubAnimType(35);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_LUMBER = ConvertSubAnimType(36);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_WORK = ConvertSubAnimType(37);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_TALK = ConvertSubAnimType(38);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_FIRST = ConvertSubAnimType(39);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_SECOND = ConvertSubAnimType(40);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_THIRD = ConvertSubAnimType(41);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_FOURTH = ConvertSubAnimType(42);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_FIFTH = ConvertSubAnimType(43);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_ONE = ConvertSubAnimType(44);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_TWO = ConvertSubAnimType(45);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_THREE = ConvertSubAnimType(46);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_FOUR = ConvertSubAnimType(47);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_FIVE = ConvertSubAnimType(48);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_SMALL = ConvertSubAnimType(49);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_MEDIUM = ConvertSubAnimType(50);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_LARGE = ConvertSubAnimType(51);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_UPGRADE = ConvertSubAnimType(52);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_DRAIN = ConvertSubAnimType(53);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_FILL = ConvertSubAnimType(54);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_CHAINLIGHTNING = ConvertSubAnimType(55);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_EATTREE = ConvertSubAnimType(56);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_PUKE = ConvertSubAnimType(57);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_FLAIL = ConvertSubAnimType(58);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_OFF = ConvertSubAnimType(59);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_SWIM = ConvertSubAnimType(60);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_ENTANGLE = ConvertSubAnimType(61);
    [NativeLuaMemberAttribute]
    public static readonly subanimtype SUBANIM_TYPE_BERSERK = ConvertSubAnimType(62);
    [NativeLuaMemberAttribute]
    public static readonly racepreference RACE_PREF_HUMAN = ConvertRacePref(1);
    [NativeLuaMemberAttribute]
    public static readonly racepreference RACE_PREF_ORC = ConvertRacePref(2);
    [NativeLuaMemberAttribute]
    public static readonly racepreference RACE_PREF_NIGHTELF = ConvertRacePref(4);
    [NativeLuaMemberAttribute]
    public static readonly racepreference RACE_PREF_UNDEAD = ConvertRacePref(8);
    [NativeLuaMemberAttribute]
    public static readonly racepreference RACE_PREF_DEMON = ConvertRacePref(16);
    [NativeLuaMemberAttribute]
    public static readonly racepreference RACE_PREF_RANDOM = ConvertRacePref(32);
    [NativeLuaMemberAttribute]
    public static readonly racepreference RACE_PREF_USER_SELECTABLE = ConvertRacePref(64);
    [NativeLuaMemberAttribute]
    public static readonly mapcontrol MAP_CONTROL_USER = ConvertMapControl(0);
    [NativeLuaMemberAttribute]
    public static readonly mapcontrol MAP_CONTROL_COMPUTER = ConvertMapControl(1);
    [NativeLuaMemberAttribute]
    public static readonly mapcontrol MAP_CONTROL_RESCUABLE = ConvertMapControl(2);
    [NativeLuaMemberAttribute]
    public static readonly mapcontrol MAP_CONTROL_NEUTRAL = ConvertMapControl(3);
    [NativeLuaMemberAttribute]
    public static readonly mapcontrol MAP_CONTROL_CREEP = ConvertMapControl(4);
    [NativeLuaMemberAttribute]
    public static readonly mapcontrol MAP_CONTROL_NONE = ConvertMapControl(5);
    [NativeLuaMemberAttribute]
    public static readonly gametype GAME_TYPE_MELEE = ConvertGameType(1);
    [NativeLuaMemberAttribute]
    public static readonly gametype GAME_TYPE_FFA = ConvertGameType(2);
    [NativeLuaMemberAttribute]
    public static readonly gametype GAME_TYPE_USE_MAP_SETTINGS = ConvertGameType(4);
    [NativeLuaMemberAttribute]
    public static readonly gametype GAME_TYPE_BLIZ = ConvertGameType(8);
    [NativeLuaMemberAttribute]
    public static readonly gametype GAME_TYPE_ONE_ON_ONE = ConvertGameType(16);
    [NativeLuaMemberAttribute]
    public static readonly gametype GAME_TYPE_TWO_TEAM_PLAY = ConvertGameType(32);
    [NativeLuaMemberAttribute]
    public static readonly gametype GAME_TYPE_THREE_TEAM_PLAY = ConvertGameType(64);
    [NativeLuaMemberAttribute]
    public static readonly gametype GAME_TYPE_FOUR_TEAM_PLAY = ConvertGameType(128);
    [NativeLuaMemberAttribute]
    public static readonly mapflag MAP_FOG_HIDE_TERRAIN = ConvertMapFlag(1);
    [NativeLuaMemberAttribute]
    public static readonly mapflag MAP_FOG_MAP_EXPLORED = ConvertMapFlag(2);
    [NativeLuaMemberAttribute]
    public static readonly mapflag MAP_FOG_ALWAYS_VISIBLE = ConvertMapFlag(4);
    [NativeLuaMemberAttribute]
    public static readonly mapflag MAP_USE_HANDICAPS = ConvertMapFlag(8);
    [NativeLuaMemberAttribute]
    public static readonly mapflag MAP_OBSERVERS = ConvertMapFlag(16);
    [NativeLuaMemberAttribute]
    public static readonly mapflag MAP_OBSERVERS_ON_DEATH = ConvertMapFlag(32);
    [NativeLuaMemberAttribute]
    public static readonly mapflag MAP_FIXED_COLORS = ConvertMapFlag(128);
    [NativeLuaMemberAttribute]
    public static readonly mapflag MAP_LOCK_RESOURCE_TRADING = ConvertMapFlag(256);
    [NativeLuaMemberAttribute]
    public static readonly mapflag MAP_RESOURCE_TRADING_ALLIES_ONLY = ConvertMapFlag(512);
    [NativeLuaMemberAttribute]
    public static readonly mapflag MAP_LOCK_ALLIANCE_CHANGES = ConvertMapFlag(1024);
    [NativeLuaMemberAttribute]
    public static readonly mapflag MAP_ALLIANCE_CHANGES_HIDDEN = ConvertMapFlag(2048);
    [NativeLuaMemberAttribute]
    public static readonly mapflag MAP_CHEATS = ConvertMapFlag(4096);
    [NativeLuaMemberAttribute]
    public static readonly mapflag MAP_CHEATS_HIDDEN = ConvertMapFlag(8192);
    [NativeLuaMemberAttribute]
    public static readonly mapflag MAP_LOCK_SPEED = ConvertMapFlag(8192 * 2);
    [NativeLuaMemberAttribute]
    public static readonly mapflag MAP_LOCK_RANDOM_SEED = ConvertMapFlag(8192 * 4);
    [NativeLuaMemberAttribute]
    public static readonly mapflag MAP_SHARED_ADVANCED_CONTROL = ConvertMapFlag(8192 * 8);
    [NativeLuaMemberAttribute]
    public static readonly mapflag MAP_RANDOM_HERO = ConvertMapFlag(8192 * 16);
    [NativeLuaMemberAttribute]
    public static readonly mapflag MAP_RANDOM_RACES = ConvertMapFlag(8192 * 32);
    [NativeLuaMemberAttribute]
    public static readonly mapflag MAP_RELOADED = ConvertMapFlag(8192 * 64);
    [NativeLuaMemberAttribute]
    public static readonly placement MAP_PLACEMENT_RANDOM = ConvertPlacement(0);
    [NativeLuaMemberAttribute]
    public static readonly placement MAP_PLACEMENT_FIXED = ConvertPlacement(1);
    [NativeLuaMemberAttribute]
    public static readonly placement MAP_PLACEMENT_USE_MAP_SETTINGS = ConvertPlacement(2);
    [NativeLuaMemberAttribute]
    public static readonly placement MAP_PLACEMENT_TEAMS_TOGETHER = ConvertPlacement(3);
    [NativeLuaMemberAttribute]
    public static readonly startlocprio MAP_LOC_PRIO_LOW = ConvertStartLocPrio(0);
    [NativeLuaMemberAttribute]
    public static readonly startlocprio MAP_LOC_PRIO_HIGH = ConvertStartLocPrio(1);
    [NativeLuaMemberAttribute]
    public static readonly startlocprio MAP_LOC_PRIO_NOT = ConvertStartLocPrio(2);
    [NativeLuaMemberAttribute]
    public static readonly mapdensity MAP_DENSITY_NONE = ConvertMapDensity(0);
    [NativeLuaMemberAttribute]
    public static readonly mapdensity MAP_DENSITY_LIGHT = ConvertMapDensity(1);
    [NativeLuaMemberAttribute]
    public static readonly mapdensity MAP_DENSITY_MEDIUM = ConvertMapDensity(2);
    [NativeLuaMemberAttribute]
    public static readonly mapdensity MAP_DENSITY_HEAVY = ConvertMapDensity(3);
    [NativeLuaMemberAttribute]
    public static readonly gamedifficulty MAP_DIFFICULTY_EASY = ConvertGameDifficulty(0);
    [NativeLuaMemberAttribute]
    public static readonly gamedifficulty MAP_DIFFICULTY_NORMAL = ConvertGameDifficulty(1);
    [NativeLuaMemberAttribute]
    public static readonly gamedifficulty MAP_DIFFICULTY_HARD = ConvertGameDifficulty(2);
    [NativeLuaMemberAttribute]
    public static readonly gamedifficulty MAP_DIFFICULTY_INSANE = ConvertGameDifficulty(3);
    [NativeLuaMemberAttribute]
    public static readonly gamespeed MAP_SPEED_SLOWEST = ConvertGameSpeed(0);
    [NativeLuaMemberAttribute]
    public static readonly gamespeed MAP_SPEED_SLOW = ConvertGameSpeed(1);
    [NativeLuaMemberAttribute]
    public static readonly gamespeed MAP_SPEED_NORMAL = ConvertGameSpeed(2);
    [NativeLuaMemberAttribute]
    public static readonly gamespeed MAP_SPEED_FAST = ConvertGameSpeed(3);
    [NativeLuaMemberAttribute]
    public static readonly gamespeed MAP_SPEED_FASTEST = ConvertGameSpeed(4);
    [NativeLuaMemberAttribute]
    public static readonly playerslotstate PLAYER_SLOT_STATE_EMPTY = ConvertPlayerSlotState(0);
    [NativeLuaMemberAttribute]
    public static readonly playerslotstate PLAYER_SLOT_STATE_PLAYING = ConvertPlayerSlotState(1);
    [NativeLuaMemberAttribute]
    public static readonly playerslotstate PLAYER_SLOT_STATE_LEFT = ConvertPlayerSlotState(2);
    [NativeLuaMemberAttribute]
    public static readonly volumegroup SOUND_VOLUMEGROUP_UNITMOVEMENT = ConvertVolumeGroup(0);
    [NativeLuaMemberAttribute]
    public static readonly volumegroup SOUND_VOLUMEGROUP_UNITSOUNDS = ConvertVolumeGroup(1);
    [NativeLuaMemberAttribute]
    public static readonly volumegroup SOUND_VOLUMEGROUP_COMBAT = ConvertVolumeGroup(2);
    [NativeLuaMemberAttribute]
    public static readonly volumegroup SOUND_VOLUMEGROUP_SPELLS = ConvertVolumeGroup(3);
    [NativeLuaMemberAttribute]
    public static readonly volumegroup SOUND_VOLUMEGROUP_UI = ConvertVolumeGroup(4);
    [NativeLuaMemberAttribute]
    public static readonly volumegroup SOUND_VOLUMEGROUP_MUSIC = ConvertVolumeGroup(5);
    [NativeLuaMemberAttribute]
    public static readonly volumegroup SOUND_VOLUMEGROUP_AMBIENTSOUNDS = ConvertVolumeGroup(6);
    [NativeLuaMemberAttribute]
    public static readonly volumegroup SOUND_VOLUMEGROUP_FIRE = ConvertVolumeGroup(7);
    [NativeLuaMemberAttribute]
    public static readonly igamestate GAME_STATE_DIVINE_INTERVENTION = ConvertIGameState(0);
    [NativeLuaMemberAttribute]
    public static readonly igamestate GAME_STATE_DISCONNECTED = ConvertIGameState(1);
    [NativeLuaMemberAttribute]
    public static readonly fgamestate GAME_STATE_TIME_OF_DAY = ConvertFGameState(2);
    [NativeLuaMemberAttribute]
    public static readonly playerstate PLAYER_STATE_GAME_RESULT = ConvertPlayerState(0);
    [NativeLuaMemberAttribute]
    public static readonly playerstate PLAYER_STATE_RESOURCE_GOLD = ConvertPlayerState(1);
    [NativeLuaMemberAttribute]
    public static readonly playerstate PLAYER_STATE_RESOURCE_LUMBER = ConvertPlayerState(2);
    [NativeLuaMemberAttribute]
    public static readonly playerstate PLAYER_STATE_RESOURCE_HERO_TOKENS = ConvertPlayerState(3);
    [NativeLuaMemberAttribute]
    public static readonly playerstate PLAYER_STATE_RESOURCE_FOOD_CAP = ConvertPlayerState(4);
    [NativeLuaMemberAttribute]
    public static readonly playerstate PLAYER_STATE_RESOURCE_FOOD_USED = ConvertPlayerState(5);
    [NativeLuaMemberAttribute]
    public static readonly playerstate PLAYER_STATE_FOOD_CAP_CEILING = ConvertPlayerState(6);
    [NativeLuaMemberAttribute]
    public static readonly playerstate PLAYER_STATE_GIVES_BOUNTY = ConvertPlayerState(7);
    [NativeLuaMemberAttribute]
    public static readonly playerstate PLAYER_STATE_ALLIED_VICTORY = ConvertPlayerState(8);
    [NativeLuaMemberAttribute]
    public static readonly playerstate PLAYER_STATE_PLACED = ConvertPlayerState(9);
    [NativeLuaMemberAttribute]
    public static readonly playerstate PLAYER_STATE_OBSERVER_ON_DEATH = ConvertPlayerState(10);
    [NativeLuaMemberAttribute]
    public static readonly playerstate PLAYER_STATE_OBSERVER = ConvertPlayerState(11);
    [NativeLuaMemberAttribute]
    public static readonly playerstate PLAYER_STATE_UNFOLLOWABLE = ConvertPlayerState(12);
    [NativeLuaMemberAttribute]
    public static readonly playerstate PLAYER_STATE_GOLD_UPKEEP_RATE = ConvertPlayerState(13);
    [NativeLuaMemberAttribute]
    public static readonly playerstate PLAYER_STATE_LUMBER_UPKEEP_RATE = ConvertPlayerState(14);
    [NativeLuaMemberAttribute]
    public static readonly playerstate PLAYER_STATE_GOLD_GATHERED = ConvertPlayerState(15);
    [NativeLuaMemberAttribute]
    public static readonly playerstate PLAYER_STATE_LUMBER_GATHERED = ConvertPlayerState(16);
    [NativeLuaMemberAttribute]
    public static readonly playerstate PLAYER_STATE_NO_CREEP_SLEEP = ConvertPlayerState(25);
    [NativeLuaMemberAttribute]
    public static readonly unitstate UNIT_STATE_LIFE = ConvertUnitState(0);
    [NativeLuaMemberAttribute]
    public static readonly unitstate UNIT_STATE_MAX_LIFE = ConvertUnitState(1);
    [NativeLuaMemberAttribute]
    public static readonly unitstate UNIT_STATE_MANA = ConvertUnitState(2);
    [NativeLuaMemberAttribute]
    public static readonly unitstate UNIT_STATE_MAX_MANA = ConvertUnitState(3);
    [NativeLuaMemberAttribute]
    public static readonly aidifficulty AI_DIFFICULTY_NEWBIE = ConvertAIDifficulty(0);
    [NativeLuaMemberAttribute]
    public static readonly aidifficulty AI_DIFFICULTY_NORMAL = ConvertAIDifficulty(1);
    [NativeLuaMemberAttribute]
    public static readonly aidifficulty AI_DIFFICULTY_INSANE = ConvertAIDifficulty(2);
    [NativeLuaMemberAttribute]
    public static readonly playerscore PLAYER_SCORE_UNITS_TRAINED = ConvertPlayerScore(0);
    [NativeLuaMemberAttribute]
    public static readonly playerscore PLAYER_SCORE_UNITS_KILLED = ConvertPlayerScore(1);
    [NativeLuaMemberAttribute]
    public static readonly playerscore PLAYER_SCORE_STRUCT_BUILT = ConvertPlayerScore(2);
    [NativeLuaMemberAttribute]
    public static readonly playerscore PLAYER_SCORE_STRUCT_RAZED = ConvertPlayerScore(3);
    [NativeLuaMemberAttribute]
    public static readonly playerscore PLAYER_SCORE_TECH_PERCENT = ConvertPlayerScore(4);
    [NativeLuaMemberAttribute]
    public static readonly playerscore PLAYER_SCORE_FOOD_MAXPROD = ConvertPlayerScore(5);
    [NativeLuaMemberAttribute]
    public static readonly playerscore PLAYER_SCORE_FOOD_MAXUSED = ConvertPlayerScore(6);
    [NativeLuaMemberAttribute]
    public static readonly playerscore PLAYER_SCORE_HEROES_KILLED = ConvertPlayerScore(7);
    [NativeLuaMemberAttribute]
    public static readonly playerscore PLAYER_SCORE_ITEMS_GAINED = ConvertPlayerScore(8);
    [NativeLuaMemberAttribute]
    public static readonly playerscore PLAYER_SCORE_MERCS_HIRED = ConvertPlayerScore(9);
    [NativeLuaMemberAttribute]
    public static readonly playerscore PLAYER_SCORE_GOLD_MINED_TOTAL = ConvertPlayerScore(10);
    [NativeLuaMemberAttribute]
    public static readonly playerscore PLAYER_SCORE_GOLD_MINED_UPKEEP = ConvertPlayerScore(11);
    [NativeLuaMemberAttribute]
    public static readonly playerscore PLAYER_SCORE_GOLD_LOST_UPKEEP = ConvertPlayerScore(12);
    [NativeLuaMemberAttribute]
    public static readonly playerscore PLAYER_SCORE_GOLD_LOST_TAX = ConvertPlayerScore(13);
    [NativeLuaMemberAttribute]
    public static readonly playerscore PLAYER_SCORE_GOLD_GIVEN = ConvertPlayerScore(14);
    [NativeLuaMemberAttribute]
    public static readonly playerscore PLAYER_SCORE_GOLD_RECEIVED = ConvertPlayerScore(15);
    [NativeLuaMemberAttribute]
    public static readonly playerscore PLAYER_SCORE_LUMBER_TOTAL = ConvertPlayerScore(16);
    [NativeLuaMemberAttribute]
    public static readonly playerscore PLAYER_SCORE_LUMBER_LOST_UPKEEP = ConvertPlayerScore(17);
    [NativeLuaMemberAttribute]
    public static readonly playerscore PLAYER_SCORE_LUMBER_LOST_TAX = ConvertPlayerScore(18);
    [NativeLuaMemberAttribute]
    public static readonly playerscore PLAYER_SCORE_LUMBER_GIVEN = ConvertPlayerScore(19);
    [NativeLuaMemberAttribute]
    public static readonly playerscore PLAYER_SCORE_LUMBER_RECEIVED = ConvertPlayerScore(20);
    [NativeLuaMemberAttribute]
    public static readonly playerscore PLAYER_SCORE_UNIT_TOTAL = ConvertPlayerScore(21);
    [NativeLuaMemberAttribute]
    public static readonly playerscore PLAYER_SCORE_HERO_TOTAL = ConvertPlayerScore(22);
    [NativeLuaMemberAttribute]
    public static readonly playerscore PLAYER_SCORE_RESOURCE_TOTAL = ConvertPlayerScore(23);
    [NativeLuaMemberAttribute]
    public static readonly playerscore PLAYER_SCORE_TOTAL = ConvertPlayerScore(24);
    [NativeLuaMemberAttribute]
    public static readonly gameevent EVENT_GAME_VICTORY = ConvertGameEvent(0);
    [NativeLuaMemberAttribute]
    public static readonly gameevent EVENT_GAME_END_LEVEL = ConvertGameEvent(1);
    [NativeLuaMemberAttribute]
    public static readonly gameevent EVENT_GAME_VARIABLE_LIMIT = ConvertGameEvent(2);
    [NativeLuaMemberAttribute]
    public static readonly gameevent EVENT_GAME_STATE_LIMIT = ConvertGameEvent(3);
    [NativeLuaMemberAttribute]
    public static readonly gameevent EVENT_GAME_TIMER_EXPIRED = ConvertGameEvent(4);
    [NativeLuaMemberAttribute]
    public static readonly gameevent EVENT_GAME_ENTER_REGION = ConvertGameEvent(5);
    [NativeLuaMemberAttribute]
    public static readonly gameevent EVENT_GAME_LEAVE_REGION = ConvertGameEvent(6);
    [NativeLuaMemberAttribute]
    public static readonly gameevent EVENT_GAME_TRACKABLE_HIT = ConvertGameEvent(7);
    [NativeLuaMemberAttribute]
    public static readonly gameevent EVENT_GAME_TRACKABLE_TRACK = ConvertGameEvent(8);
    [NativeLuaMemberAttribute]
    public static readonly gameevent EVENT_GAME_SHOW_SKILL = ConvertGameEvent(9);
    [NativeLuaMemberAttribute]
    public static readonly gameevent EVENT_GAME_BUILD_SUBMENU = ConvertGameEvent(10);
    [NativeLuaMemberAttribute]
    public static readonly playerevent EVENT_PLAYER_STATE_LIMIT = ConvertPlayerEvent(11);
    [NativeLuaMemberAttribute]
    public static readonly playerevent EVENT_PLAYER_ALLIANCE_CHANGED = ConvertPlayerEvent(12);
    [NativeLuaMemberAttribute]
    public static readonly playerevent EVENT_PLAYER_DEFEAT = ConvertPlayerEvent(13);
    [NativeLuaMemberAttribute]
    public static readonly playerevent EVENT_PLAYER_VICTORY = ConvertPlayerEvent(14);
    [NativeLuaMemberAttribute]
    public static readonly playerevent EVENT_PLAYER_LEAVE = ConvertPlayerEvent(15);
    [NativeLuaMemberAttribute]
    public static readonly playerevent EVENT_PLAYER_CHAT = ConvertPlayerEvent(16);
    [NativeLuaMemberAttribute]
    public static readonly playerevent EVENT_PLAYER_END_CINEMATIC = ConvertPlayerEvent(17);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_ATTACKED = ConvertPlayerUnitEvent(18);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_RESCUED = ConvertPlayerUnitEvent(19);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_DEATH = ConvertPlayerUnitEvent(20);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_DECAY = ConvertPlayerUnitEvent(21);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_DETECTED = ConvertPlayerUnitEvent(22);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_HIDDEN = ConvertPlayerUnitEvent(23);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_SELECTED = ConvertPlayerUnitEvent(24);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_DESELECTED = ConvertPlayerUnitEvent(25);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_CONSTRUCT_START = ConvertPlayerUnitEvent(26);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_CONSTRUCT_CANCEL = ConvertPlayerUnitEvent(27);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_CONSTRUCT_FINISH = ConvertPlayerUnitEvent(28);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_UPGRADE_START = ConvertPlayerUnitEvent(29);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_UPGRADE_CANCEL = ConvertPlayerUnitEvent(30);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_UPGRADE_FINISH = ConvertPlayerUnitEvent(31);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_TRAIN_START = ConvertPlayerUnitEvent(32);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_TRAIN_CANCEL = ConvertPlayerUnitEvent(33);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_TRAIN_FINISH = ConvertPlayerUnitEvent(34);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_RESEARCH_START = ConvertPlayerUnitEvent(35);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_RESEARCH_CANCEL = ConvertPlayerUnitEvent(36);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_RESEARCH_FINISH = ConvertPlayerUnitEvent(37);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_ISSUED_ORDER = ConvertPlayerUnitEvent(38);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_ISSUED_POINT_ORDER = ConvertPlayerUnitEvent(39);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_ISSUED_TARGET_ORDER = ConvertPlayerUnitEvent(40);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_ISSUED_UNIT_ORDER = ConvertPlayerUnitEvent(40);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_HERO_LEVEL = ConvertPlayerUnitEvent(41);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_HERO_SKILL = ConvertPlayerUnitEvent(42);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_HERO_REVIVABLE = ConvertPlayerUnitEvent(43);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_HERO_REVIVE_START = ConvertPlayerUnitEvent(44);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_HERO_REVIVE_CANCEL = ConvertPlayerUnitEvent(45);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_HERO_REVIVE_FINISH = ConvertPlayerUnitEvent(46);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_SUMMON = ConvertPlayerUnitEvent(47);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_DROP_ITEM = ConvertPlayerUnitEvent(48);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_PICKUP_ITEM = ConvertPlayerUnitEvent(49);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_USE_ITEM = ConvertPlayerUnitEvent(50);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_LOADED = ConvertPlayerUnitEvent(51);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_DAMAGED = ConvertPlayerUnitEvent(308);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_DAMAGING = ConvertPlayerUnitEvent(315);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_DAMAGED = ConvertUnitEvent(52);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_DAMAGING = ConvertUnitEvent(314);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_DEATH = ConvertUnitEvent(53);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_DECAY = ConvertUnitEvent(54);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_DETECTED = ConvertUnitEvent(55);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_HIDDEN = ConvertUnitEvent(56);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_SELECTED = ConvertUnitEvent(57);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_DESELECTED = ConvertUnitEvent(58);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_STATE_LIMIT = ConvertUnitEvent(59);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_ACQUIRED_TARGET = ConvertUnitEvent(60);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_TARGET_IN_RANGE = ConvertUnitEvent(61);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_ATTACKED = ConvertUnitEvent(62);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_RESCUED = ConvertUnitEvent(63);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_CONSTRUCT_CANCEL = ConvertUnitEvent(64);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_CONSTRUCT_FINISH = ConvertUnitEvent(65);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_UPGRADE_START = ConvertUnitEvent(66);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_UPGRADE_CANCEL = ConvertUnitEvent(67);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_UPGRADE_FINISH = ConvertUnitEvent(68);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_TRAIN_START = ConvertUnitEvent(69);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_TRAIN_CANCEL = ConvertUnitEvent(70);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_TRAIN_FINISH = ConvertUnitEvent(71);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_RESEARCH_START = ConvertUnitEvent(72);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_RESEARCH_CANCEL = ConvertUnitEvent(73);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_RESEARCH_FINISH = ConvertUnitEvent(74);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_ISSUED_ORDER = ConvertUnitEvent(75);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_ISSUED_POINT_ORDER = ConvertUnitEvent(76);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_ISSUED_TARGET_ORDER = ConvertUnitEvent(77);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_HERO_LEVEL = ConvertUnitEvent(78);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_HERO_SKILL = ConvertUnitEvent(79);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_HERO_REVIVABLE = ConvertUnitEvent(80);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_HERO_REVIVE_START = ConvertUnitEvent(81);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_HERO_REVIVE_CANCEL = ConvertUnitEvent(82);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_HERO_REVIVE_FINISH = ConvertUnitEvent(83);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_SUMMON = ConvertUnitEvent(84);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_DROP_ITEM = ConvertUnitEvent(85);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_PICKUP_ITEM = ConvertUnitEvent(86);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_USE_ITEM = ConvertUnitEvent(87);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_LOADED = ConvertUnitEvent(88);
    [NativeLuaMemberAttribute]
    public static readonly widgetevent EVENT_WIDGET_DEATH = ConvertWidgetEvent(89);
    [NativeLuaMemberAttribute]
    public static readonly dialogevent EVENT_DIALOG_BUTTON_CLICK = ConvertDialogEvent(90);
    [NativeLuaMemberAttribute]
    public static readonly dialogevent EVENT_DIALOG_CLICK = ConvertDialogEvent(91);
    [NativeLuaMemberAttribute]
    public static readonly gameevent EVENT_GAME_LOADED = ConvertGameEvent(256);
    [NativeLuaMemberAttribute]
    public static readonly gameevent EVENT_GAME_TOURNAMENT_FINISH_SOON = ConvertGameEvent(257);
    [NativeLuaMemberAttribute]
    public static readonly gameevent EVENT_GAME_TOURNAMENT_FINISH_NOW = ConvertGameEvent(258);
    [NativeLuaMemberAttribute]
    public static readonly gameevent EVENT_GAME_SAVE = ConvertGameEvent(259);
    [NativeLuaMemberAttribute]
    public static readonly gameevent EVENT_GAME_CUSTOM_UI_FRAME = ConvertGameEvent(310);
    [NativeLuaMemberAttribute]
    public static readonly playerevent EVENT_PLAYER_ARROW_LEFT_DOWN = ConvertPlayerEvent(261);
    [NativeLuaMemberAttribute]
    public static readonly playerevent EVENT_PLAYER_ARROW_LEFT_UP = ConvertPlayerEvent(262);
    [NativeLuaMemberAttribute]
    public static readonly playerevent EVENT_PLAYER_ARROW_RIGHT_DOWN = ConvertPlayerEvent(263);
    [NativeLuaMemberAttribute]
    public static readonly playerevent EVENT_PLAYER_ARROW_RIGHT_UP = ConvertPlayerEvent(264);
    [NativeLuaMemberAttribute]
    public static readonly playerevent EVENT_PLAYER_ARROW_DOWN_DOWN = ConvertPlayerEvent(265);
    [NativeLuaMemberAttribute]
    public static readonly playerevent EVENT_PLAYER_ARROW_DOWN_UP = ConvertPlayerEvent(266);
    [NativeLuaMemberAttribute]
    public static readonly playerevent EVENT_PLAYER_ARROW_UP_DOWN = ConvertPlayerEvent(267);
    [NativeLuaMemberAttribute]
    public static readonly playerevent EVENT_PLAYER_ARROW_UP_UP = ConvertPlayerEvent(268);
    [NativeLuaMemberAttribute]
    public static readonly playerevent EVENT_PLAYER_MOUSE_DOWN = ConvertPlayerEvent(305);
    [NativeLuaMemberAttribute]
    public static readonly playerevent EVENT_PLAYER_MOUSE_UP = ConvertPlayerEvent(306);
    [NativeLuaMemberAttribute]
    public static readonly playerevent EVENT_PLAYER_MOUSE_MOVE = ConvertPlayerEvent(307);
    [NativeLuaMemberAttribute]
    public static readonly playerevent EVENT_PLAYER_SYNC_DATA = ConvertPlayerEvent(309);
    [NativeLuaMemberAttribute]
    public static readonly playerevent EVENT_PLAYER_KEY = ConvertPlayerEvent(311);
    [NativeLuaMemberAttribute]
    public static readonly playerevent EVENT_PLAYER_KEY_DOWN = ConvertPlayerEvent(312);
    [NativeLuaMemberAttribute]
    public static readonly playerevent EVENT_PLAYER_KEY_UP = ConvertPlayerEvent(313);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_SELL = ConvertPlayerUnitEvent(269);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_CHANGE_OWNER = ConvertPlayerUnitEvent(270);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_SELL_ITEM = ConvertPlayerUnitEvent(271);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_SPELL_CHANNEL = ConvertPlayerUnitEvent(272);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_SPELL_CAST = ConvertPlayerUnitEvent(273);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_SPELL_EFFECT = ConvertPlayerUnitEvent(274);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_SPELL_FINISH = ConvertPlayerUnitEvent(275);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_SPELL_ENDCAST = ConvertPlayerUnitEvent(276);
    [NativeLuaMemberAttribute]
    public static readonly playerunitevent EVENT_PLAYER_UNIT_PAWN_ITEM = ConvertPlayerUnitEvent(277);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_SELL = ConvertUnitEvent(286);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_CHANGE_OWNER = ConvertUnitEvent(287);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_SELL_ITEM = ConvertUnitEvent(288);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_SPELL_CHANNEL = ConvertUnitEvent(289);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_SPELL_CAST = ConvertUnitEvent(290);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_SPELL_EFFECT = ConvertUnitEvent(291);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_SPELL_FINISH = ConvertUnitEvent(292);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_SPELL_ENDCAST = ConvertUnitEvent(293);
    [NativeLuaMemberAttribute]
    public static readonly unitevent EVENT_UNIT_PAWN_ITEM = ConvertUnitEvent(294);
    [NativeLuaMemberAttribute]
    public static readonly limitop LESS_THAN = ConvertLimitOp(0);
    [NativeLuaMemberAttribute]
    public static readonly limitop LESS_THAN_OR_EQUAL = ConvertLimitOp(1);
    [NativeLuaMemberAttribute]
    public static readonly limitop EQUAL = ConvertLimitOp(2);
    [NativeLuaMemberAttribute]
    public static readonly limitop GREATER_THAN_OR_EQUAL = ConvertLimitOp(3);
    [NativeLuaMemberAttribute]
    public static readonly limitop GREATER_THAN = ConvertLimitOp(4);
    [NativeLuaMemberAttribute]
    public static readonly limitop NOT_EQUAL = ConvertLimitOp(5);
    [NativeLuaMemberAttribute]
    public static readonly unittype UNIT_TYPE_HERO = ConvertUnitType(0);
    [NativeLuaMemberAttribute]
    public static readonly unittype UNIT_TYPE_DEAD = ConvertUnitType(1);
    [NativeLuaMemberAttribute]
    public static readonly unittype UNIT_TYPE_STRUCTURE = ConvertUnitType(2);
    [NativeLuaMemberAttribute]
    public static readonly unittype UNIT_TYPE_FLYING = ConvertUnitType(3);
    [NativeLuaMemberAttribute]
    public static readonly unittype UNIT_TYPE_GROUND = ConvertUnitType(4);
    [NativeLuaMemberAttribute]
    public static readonly unittype UNIT_TYPE_ATTACKS_FLYING = ConvertUnitType(5);
    [NativeLuaMemberAttribute]
    public static readonly unittype UNIT_TYPE_ATTACKS_GROUND = ConvertUnitType(6);
    [NativeLuaMemberAttribute]
    public static readonly unittype UNIT_TYPE_MELEE_ATTACKER = ConvertUnitType(7);
    [NativeLuaMemberAttribute]
    public static readonly unittype UNIT_TYPE_RANGED_ATTACKER = ConvertUnitType(8);
    [NativeLuaMemberAttribute]
    public static readonly unittype UNIT_TYPE_GIANT = ConvertUnitType(9);
    [NativeLuaMemberAttribute]
    public static readonly unittype UNIT_TYPE_SUMMONED = ConvertUnitType(10);
    [NativeLuaMemberAttribute]
    public static readonly unittype UNIT_TYPE_STUNNED = ConvertUnitType(11);
    [NativeLuaMemberAttribute]
    public static readonly unittype UNIT_TYPE_PLAGUED = ConvertUnitType(12);
    [NativeLuaMemberAttribute]
    public static readonly unittype UNIT_TYPE_SNARED = ConvertUnitType(13);
    [NativeLuaMemberAttribute]
    public static readonly unittype UNIT_TYPE_UNDEAD = ConvertUnitType(14);
    [NativeLuaMemberAttribute]
    public static readonly unittype UNIT_TYPE_MECHANICAL = ConvertUnitType(15);
    [NativeLuaMemberAttribute]
    public static readonly unittype UNIT_TYPE_PEON = ConvertUnitType(16);
    [NativeLuaMemberAttribute]
    public static readonly unittype UNIT_TYPE_SAPPER = ConvertUnitType(17);
    [NativeLuaMemberAttribute]
    public static readonly unittype UNIT_TYPE_TOWNHALL = ConvertUnitType(18);
    [NativeLuaMemberAttribute]
    public static readonly unittype UNIT_TYPE_ANCIENT = ConvertUnitType(19);
    [NativeLuaMemberAttribute]
    public static readonly unittype UNIT_TYPE_TAUREN = ConvertUnitType(20);
    [NativeLuaMemberAttribute]
    public static readonly unittype UNIT_TYPE_POISONED = ConvertUnitType(21);
    [NativeLuaMemberAttribute]
    public static readonly unittype UNIT_TYPE_POLYMORPHED = ConvertUnitType(22);
    [NativeLuaMemberAttribute]
    public static readonly unittype UNIT_TYPE_SLEEPING = ConvertUnitType(23);
    [NativeLuaMemberAttribute]
    public static readonly unittype UNIT_TYPE_RESISTANT = ConvertUnitType(24);
    [NativeLuaMemberAttribute]
    public static readonly unittype UNIT_TYPE_ETHEREAL = ConvertUnitType(25);
    [NativeLuaMemberAttribute]
    public static readonly unittype UNIT_TYPE_MAGIC_IMMUNE = ConvertUnitType(26);
    [NativeLuaMemberAttribute]
    public static readonly itemtype ITEM_TYPE_PERMANENT = ConvertItemType(0);
    [NativeLuaMemberAttribute]
    public static readonly itemtype ITEM_TYPE_CHARGED = ConvertItemType(1);
    [NativeLuaMemberAttribute]
    public static readonly itemtype ITEM_TYPE_POWERUP = ConvertItemType(2);
    [NativeLuaMemberAttribute]
    public static readonly itemtype ITEM_TYPE_ARTIFACT = ConvertItemType(3);
    [NativeLuaMemberAttribute]
    public static readonly itemtype ITEM_TYPE_PURCHASABLE = ConvertItemType(4);
    [NativeLuaMemberAttribute]
    public static readonly itemtype ITEM_TYPE_CAMPAIGN = ConvertItemType(5);
    [NativeLuaMemberAttribute]
    public static readonly itemtype ITEM_TYPE_MISCELLANEOUS = ConvertItemType(6);
    [NativeLuaMemberAttribute]
    public static readonly itemtype ITEM_TYPE_UNKNOWN = ConvertItemType(7);
    [NativeLuaMemberAttribute]
    public static readonly itemtype ITEM_TYPE_ANY = ConvertItemType(8);
    [NativeLuaMemberAttribute]
    public static readonly itemtype ITEM_TYPE_TOME = ConvertItemType(2);
    [NativeLuaMemberAttribute]
    public static readonly camerafield CAMERA_FIELD_TARGET_DISTANCE = ConvertCameraField(0);
    [NativeLuaMemberAttribute]
    public static readonly camerafield CAMERA_FIELD_FARZ = ConvertCameraField(1);
    [NativeLuaMemberAttribute]
    public static readonly camerafield CAMERA_FIELD_ANGLE_OF_ATTACK = ConvertCameraField(2);
    [NativeLuaMemberAttribute]
    public static readonly camerafield CAMERA_FIELD_FIELD_OF_VIEW = ConvertCameraField(3);
    [NativeLuaMemberAttribute]
    public static readonly camerafield CAMERA_FIELD_ROLL = ConvertCameraField(4);
    [NativeLuaMemberAttribute]
    public static readonly camerafield CAMERA_FIELD_ROTATION = ConvertCameraField(5);
    [NativeLuaMemberAttribute]
    public static readonly camerafield CAMERA_FIELD_ZOFFSET = ConvertCameraField(6);
    [NativeLuaMemberAttribute]
    public static readonly camerafield CAMERA_FIELD_NEARZ = ConvertCameraField(7);
    [NativeLuaMemberAttribute]
    public static readonly camerafield CAMERA_FIELD_LOCAL_PITCH = ConvertCameraField(8);
    [NativeLuaMemberAttribute]
    public static readonly camerafield CAMERA_FIELD_LOCAL_YAW = ConvertCameraField(9);
    [NativeLuaMemberAttribute]
    public static readonly camerafield CAMERA_FIELD_LOCAL_ROLL = ConvertCameraField(10);
    [NativeLuaMemberAttribute]
    public static readonly blendmode BLEND_MODE_NONE = ConvertBlendMode(0);
    [NativeLuaMemberAttribute]
    public static readonly blendmode BLEND_MODE_DONT_CARE = ConvertBlendMode(0);
    [NativeLuaMemberAttribute]
    public static readonly blendmode BLEND_MODE_KEYALPHA = ConvertBlendMode(1);
    [NativeLuaMemberAttribute]
    public static readonly blendmode BLEND_MODE_BLEND = ConvertBlendMode(2);
    [NativeLuaMemberAttribute]
    public static readonly blendmode BLEND_MODE_ADDITIVE = ConvertBlendMode(3);
    [NativeLuaMemberAttribute]
    public static readonly blendmode BLEND_MODE_MODULATE = ConvertBlendMode(4);
    [NativeLuaMemberAttribute]
    public static readonly blendmode BLEND_MODE_MODULATE_2X = ConvertBlendMode(5);
    [NativeLuaMemberAttribute]
    public static readonly raritycontrol RARITY_FREQUENT = ConvertRarityControl(0);
    [NativeLuaMemberAttribute]
    public static readonly raritycontrol RARITY_RARE = ConvertRarityControl(1);
    [NativeLuaMemberAttribute]
    public static readonly texmapflags TEXMAP_FLAG_NONE = ConvertTexMapFlags(0);
    [NativeLuaMemberAttribute]
    public static readonly texmapflags TEXMAP_FLAG_WRAP_U = ConvertTexMapFlags(1);
    [NativeLuaMemberAttribute]
    public static readonly texmapflags TEXMAP_FLAG_WRAP_V = ConvertTexMapFlags(2);
    [NativeLuaMemberAttribute]
    public static readonly texmapflags TEXMAP_FLAG_WRAP_UV = ConvertTexMapFlags(3);
    [NativeLuaMemberAttribute]
    public static readonly fogstate FOG_OF_WAR_MASKED = ConvertFogState(1);
    [NativeLuaMemberAttribute]
    public static readonly fogstate FOG_OF_WAR_FOGGED = ConvertFogState(2);
    [NativeLuaMemberAttribute]
    public static readonly fogstate FOG_OF_WAR_VISIBLE = ConvertFogState(4);
    [NativeLuaMemberAttribute]
    public const int CAMERA_MARGIN_LEFT = 0;
    [NativeLuaMemberAttribute]
    public const int CAMERA_MARGIN_RIGHT = 1;
    [NativeLuaMemberAttribute]
    public const int CAMERA_MARGIN_TOP = 2;
    [NativeLuaMemberAttribute]
    public const int CAMERA_MARGIN_BOTTOM = 3;
    [NativeLuaMemberAttribute]
    public static readonly effecttype EFFECT_TYPE_EFFECT = ConvertEffectType(0);
    [NativeLuaMemberAttribute]
    public static readonly effecttype EFFECT_TYPE_TARGET = ConvertEffectType(1);
    [NativeLuaMemberAttribute]
    public static readonly effecttype EFFECT_TYPE_CASTER = ConvertEffectType(2);
    [NativeLuaMemberAttribute]
    public static readonly effecttype EFFECT_TYPE_SPECIAL = ConvertEffectType(3);
    [NativeLuaMemberAttribute]
    public static readonly effecttype EFFECT_TYPE_AREA_EFFECT = ConvertEffectType(4);
    [NativeLuaMemberAttribute]
    public static readonly effecttype EFFECT_TYPE_MISSILE = ConvertEffectType(5);
    [NativeLuaMemberAttribute]
    public static readonly effecttype EFFECT_TYPE_LIGHTNING = ConvertEffectType(6);
    [NativeLuaMemberAttribute]
    public static readonly soundtype SOUND_TYPE_EFFECT = ConvertSoundType(0);
    [NativeLuaMemberAttribute]
    public static readonly soundtype SOUND_TYPE_EFFECT_LOOPED = ConvertSoundType(1);
    [NativeLuaMemberAttribute]
    public static readonly originframetype ORIGIN_FRAME_GAME_UI = ConvertOriginFrameType(0);
    [NativeLuaMemberAttribute]
    public static readonly originframetype ORIGIN_FRAME_COMMAND_BUTTON = ConvertOriginFrameType(1);
    [NativeLuaMemberAttribute]
    public static readonly originframetype ORIGIN_FRAME_HERO_BAR = ConvertOriginFrameType(2);
    [NativeLuaMemberAttribute]
    public static readonly originframetype ORIGIN_FRAME_HERO_BUTTON = ConvertOriginFrameType(3);
    [NativeLuaMemberAttribute]
    public static readonly originframetype ORIGIN_FRAME_HERO_HP_BAR = ConvertOriginFrameType(4);
    [NativeLuaMemberAttribute]
    public static readonly originframetype ORIGIN_FRAME_HERO_MANA_BAR = ConvertOriginFrameType(5);
    [NativeLuaMemberAttribute]
    public static readonly originframetype ORIGIN_FRAME_HERO_BUTTON_INDICATOR = ConvertOriginFrameType(6);
    [NativeLuaMemberAttribute]
    public static readonly originframetype ORIGIN_FRAME_ITEM_BUTTON = ConvertOriginFrameType(7);
    [NativeLuaMemberAttribute]
    public static readonly originframetype ORIGIN_FRAME_MINIMAP = ConvertOriginFrameType(8);
    [NativeLuaMemberAttribute]
    public static readonly originframetype ORIGIN_FRAME_MINIMAP_BUTTON = ConvertOriginFrameType(9);
    [NativeLuaMemberAttribute]
    public static readonly originframetype ORIGIN_FRAME_SYSTEM_BUTTON = ConvertOriginFrameType(10);
    [NativeLuaMemberAttribute]
    public static readonly originframetype ORIGIN_FRAME_TOOLTIP = ConvertOriginFrameType(11);
    [NativeLuaMemberAttribute]
    public static readonly originframetype ORIGIN_FRAME_UBERTOOLTIP = ConvertOriginFrameType(12);
    [NativeLuaMemberAttribute]
    public static readonly originframetype ORIGIN_FRAME_CHAT_MSG = ConvertOriginFrameType(13);
    [NativeLuaMemberAttribute]
    public static readonly originframetype ORIGIN_FRAME_UNIT_MSG = ConvertOriginFrameType(14);
    [NativeLuaMemberAttribute]
    public static readonly originframetype ORIGIN_FRAME_TOP_MSG = ConvertOriginFrameType(15);
    [NativeLuaMemberAttribute]
    public static readonly originframetype ORIGIN_FRAME_PORTRAIT = ConvertOriginFrameType(16);
    [NativeLuaMemberAttribute]
    public static readonly originframetype ORIGIN_FRAME_WORLD_FRAME = ConvertOriginFrameType(17);
    [NativeLuaMemberAttribute]
    public static readonly framepointtype FRAMEPOINT_TOPLEFT = ConvertFramePointType(0);
    [NativeLuaMemberAttribute]
    public static readonly framepointtype FRAMEPOINT_TOP = ConvertFramePointType(1);
    [NativeLuaMemberAttribute]
    public static readonly framepointtype FRAMEPOINT_TOPRIGHT = ConvertFramePointType(2);
    [NativeLuaMemberAttribute]
    public static readonly framepointtype FRAMEPOINT_LEFT = ConvertFramePointType(3);
    [NativeLuaMemberAttribute]
    public static readonly framepointtype FRAMEPOINT_CENTER = ConvertFramePointType(4);
    [NativeLuaMemberAttribute]
    public static readonly framepointtype FRAMEPOINT_RIGHT = ConvertFramePointType(5);
    [NativeLuaMemberAttribute]
    public static readonly framepointtype FRAMEPOINT_BOTTOMLEFT = ConvertFramePointType(6);
    [NativeLuaMemberAttribute]
    public static readonly framepointtype FRAMEPOINT_BOTTOM = ConvertFramePointType(7);
    [NativeLuaMemberAttribute]
    public static readonly framepointtype FRAMEPOINT_BOTTOMRIGHT = ConvertFramePointType(8);
    [NativeLuaMemberAttribute]
    public static readonly textaligntype TEXT_JUSTIFY_TOP = ConvertTextAlignType(0);
    [NativeLuaMemberAttribute]
    public static readonly textaligntype TEXT_JUSTIFY_MIDDLE = ConvertTextAlignType(1);
    [NativeLuaMemberAttribute]
    public static readonly textaligntype TEXT_JUSTIFY_BOTTOM = ConvertTextAlignType(2);
    [NativeLuaMemberAttribute]
    public static readonly textaligntype TEXT_JUSTIFY_LEFT = ConvertTextAlignType(3);
    [NativeLuaMemberAttribute]
    public static readonly textaligntype TEXT_JUSTIFY_CENTER = ConvertTextAlignType(4);
    [NativeLuaMemberAttribute]
    public static readonly textaligntype TEXT_JUSTIFY_RIGHT = ConvertTextAlignType(5);
    [NativeLuaMemberAttribute]
    public static readonly frameeventtype FRAMEEVENT_CONTROL_CLICK = ConvertFrameEventType(1);
    [NativeLuaMemberAttribute]
    public static readonly frameeventtype FRAMEEVENT_MOUSE_ENTER = ConvertFrameEventType(2);
    [NativeLuaMemberAttribute]
    public static readonly frameeventtype FRAMEEVENT_MOUSE_LEAVE = ConvertFrameEventType(3);
    [NativeLuaMemberAttribute]
    public static readonly frameeventtype FRAMEEVENT_MOUSE_UP = ConvertFrameEventType(4);
    [NativeLuaMemberAttribute]
    public static readonly frameeventtype FRAMEEVENT_MOUSE_DOWN = ConvertFrameEventType(5);
    [NativeLuaMemberAttribute]
    public static readonly frameeventtype FRAMEEVENT_MOUSE_WHEEL = ConvertFrameEventType(6);
    [NativeLuaMemberAttribute]
    public static readonly frameeventtype FRAMEEVENT_CHECKBOX_CHECKED = ConvertFrameEventType(7);
    [NativeLuaMemberAttribute]
    public static readonly frameeventtype FRAMEEVENT_CHECKBOX_UNCHECKED = ConvertFrameEventType(8);
    [NativeLuaMemberAttribute]
    public static readonly frameeventtype FRAMEEVENT_EDITBOX_TEXT_CHANGED = ConvertFrameEventType(9);
    [NativeLuaMemberAttribute]
    public static readonly frameeventtype FRAMEEVENT_POPUPMENU_ITEM_CHANGED = ConvertFrameEventType(10);
    [NativeLuaMemberAttribute]
    public static readonly frameeventtype FRAMEEVENT_MOUSE_DOUBLECLICK = ConvertFrameEventType(11);
    [NativeLuaMemberAttribute]
    public static readonly frameeventtype FRAMEEVENT_SPRITE_ANIM_UPDATE = ConvertFrameEventType(12);
    [NativeLuaMemberAttribute]
    public static readonly frameeventtype FRAMEEVENT_SLIDER_VALUE_CHANGED = ConvertFrameEventType(13);
    [NativeLuaMemberAttribute]
    public static readonly frameeventtype FRAMEEVENT_DIALOG_CANCEL = ConvertFrameEventType(14);
    [NativeLuaMemberAttribute]
    public static readonly frameeventtype FRAMEEVENT_DIALOG_ACCEPT = ConvertFrameEventType(15);
    [NativeLuaMemberAttribute]
    public static readonly frameeventtype FRAMEEVENT_EDITBOX_ENTER = ConvertFrameEventType(16);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_BACKSPACE = ConvertOsKeyType(0x08);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_TAB = ConvertOsKeyType(0x09);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_CLEAR = ConvertOsKeyType(0x0C);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_RETURN = ConvertOsKeyType(0x0D);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_SHIFT = ConvertOsKeyType(0x10);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_CONTROL = ConvertOsKeyType(0x11);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_ALT = ConvertOsKeyType(0x12);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_PAUSE = ConvertOsKeyType(0x13);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_CAPSLOCK = ConvertOsKeyType(0x14);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_KANA = ConvertOsKeyType(0x15);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_HANGUL = ConvertOsKeyType(0x15);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_JUNJA = ConvertOsKeyType(0x17);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_FINAL = ConvertOsKeyType(0x18);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_HANJA = ConvertOsKeyType(0x19);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_KANJI = ConvertOsKeyType(0x19);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_ESCAPE = ConvertOsKeyType(0x1B);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_CONVERT = ConvertOsKeyType(0x1C);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_NONCONVERT = ConvertOsKeyType(0x1D);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_ACCEPT = ConvertOsKeyType(0x1E);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_MODECHANGE = ConvertOsKeyType(0x1F);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_SPACE = ConvertOsKeyType(0x20);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_PAGEUP = ConvertOsKeyType(0x21);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_PAGEDOWN = ConvertOsKeyType(0x22);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_END = ConvertOsKeyType(0x23);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_HOME = ConvertOsKeyType(0x24);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_LEFT = ConvertOsKeyType(0x25);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_UP = ConvertOsKeyType(0x26);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_RIGHT = ConvertOsKeyType(0x27);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_DOWN = ConvertOsKeyType(0x28);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_SELECT = ConvertOsKeyType(0x29);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_PRINT = ConvertOsKeyType(0x2A);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_EXECUTE = ConvertOsKeyType(0x2B);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_PRINTSCREEN = ConvertOsKeyType(0x2C);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_INSERT = ConvertOsKeyType(0x2D);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_DELETE = ConvertOsKeyType(0x2E);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_HELP = ConvertOsKeyType(0x2F);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_0 = ConvertOsKeyType(0x30);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_1 = ConvertOsKeyType(0x31);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_2 = ConvertOsKeyType(0x32);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_3 = ConvertOsKeyType(0x33);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_4 = ConvertOsKeyType(0x34);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_5 = ConvertOsKeyType(0x35);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_6 = ConvertOsKeyType(0x36);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_7 = ConvertOsKeyType(0x37);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_8 = ConvertOsKeyType(0x38);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_9 = ConvertOsKeyType(0x39);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_A = ConvertOsKeyType(0x41);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_B = ConvertOsKeyType(0x42);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_C = ConvertOsKeyType(0x43);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_D = ConvertOsKeyType(0x44);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_E = ConvertOsKeyType(0x45);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_F = ConvertOsKeyType(0x46);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_G = ConvertOsKeyType(0x47);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_H = ConvertOsKeyType(0x48);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_I = ConvertOsKeyType(0x49);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_J = ConvertOsKeyType(0x4A);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_K = ConvertOsKeyType(0x4B);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_L = ConvertOsKeyType(0x4C);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_M = ConvertOsKeyType(0x4D);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_N = ConvertOsKeyType(0x4E);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_O = ConvertOsKeyType(0x4F);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_P = ConvertOsKeyType(0x50);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_Q = ConvertOsKeyType(0x51);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_R = ConvertOsKeyType(0x52);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_S = ConvertOsKeyType(0x53);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_T = ConvertOsKeyType(0x54);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_U = ConvertOsKeyType(0x55);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_V = ConvertOsKeyType(0x56);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_W = ConvertOsKeyType(0x57);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_X = ConvertOsKeyType(0x58);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_Y = ConvertOsKeyType(0x59);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_Z = ConvertOsKeyType(0x5A);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_LMETA = ConvertOsKeyType(0x5B);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_RMETA = ConvertOsKeyType(0x5C);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_APPS = ConvertOsKeyType(0x5D);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_SLEEP = ConvertOsKeyType(0x5F);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_NUMPAD0 = ConvertOsKeyType(0x60);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_NUMPAD1 = ConvertOsKeyType(0x61);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_NUMPAD2 = ConvertOsKeyType(0x62);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_NUMPAD3 = ConvertOsKeyType(0x63);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_NUMPAD4 = ConvertOsKeyType(0x64);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_NUMPAD5 = ConvertOsKeyType(0x65);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_NUMPAD6 = ConvertOsKeyType(0x66);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_NUMPAD7 = ConvertOsKeyType(0x67);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_NUMPAD8 = ConvertOsKeyType(0x68);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_NUMPAD9 = ConvertOsKeyType(0x69);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_MULTIPLY = ConvertOsKeyType(0x6A);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_ADD = ConvertOsKeyType(0x6B);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_SEPARATOR = ConvertOsKeyType(0x6C);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_SUBTRACT = ConvertOsKeyType(0x6D);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_DECIMAL = ConvertOsKeyType(0x6E);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_DIVIDE = ConvertOsKeyType(0x6F);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_F1 = ConvertOsKeyType(0x70);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_F2 = ConvertOsKeyType(0x71);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_F3 = ConvertOsKeyType(0x72);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_F4 = ConvertOsKeyType(0x73);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_F5 = ConvertOsKeyType(0x74);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_F6 = ConvertOsKeyType(0x75);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_F7 = ConvertOsKeyType(0x76);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_F8 = ConvertOsKeyType(0x77);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_F9 = ConvertOsKeyType(0x78);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_F10 = ConvertOsKeyType(0x79);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_F11 = ConvertOsKeyType(0x7A);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_F12 = ConvertOsKeyType(0x7B);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_F13 = ConvertOsKeyType(0x7C);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_F14 = ConvertOsKeyType(0x7D);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_F15 = ConvertOsKeyType(0x7E);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_F16 = ConvertOsKeyType(0x7F);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_F17 = ConvertOsKeyType(0x80);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_F18 = ConvertOsKeyType(0x81);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_F19 = ConvertOsKeyType(0x82);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_F20 = ConvertOsKeyType(0x83);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_F21 = ConvertOsKeyType(0x84);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_F22 = ConvertOsKeyType(0x85);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_F23 = ConvertOsKeyType(0x86);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_F24 = ConvertOsKeyType(0x87);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_NUMLOCK = ConvertOsKeyType(0x90);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_SCROLLLOCK = ConvertOsKeyType(0x91);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_NEC_EQUAL = ConvertOsKeyType(0x92);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_FJ_JISHO = ConvertOsKeyType(0x92);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_FJ_MASSHOU = ConvertOsKeyType(0x93);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_FJ_TOUROKU = ConvertOsKeyType(0x94);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_FJ_LOYA = ConvertOsKeyType(0x95);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_FJ_ROYA = ConvertOsKeyType(0x96);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_LSHIFT = ConvertOsKeyType(0xA0);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_RSHIFT = ConvertOsKeyType(0xA1);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_LCONTROL = ConvertOsKeyType(0xA2);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_RCONTROL = ConvertOsKeyType(0xA3);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_LALT = ConvertOsKeyType(0xA4);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_RALT = ConvertOsKeyType(0xA5);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_BROWSER_BACK = ConvertOsKeyType(0xA6);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_BROWSER_FORWARD = ConvertOsKeyType(0xA7);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_BROWSER_REFRESH = ConvertOsKeyType(0xA8);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_BROWSER_STOP = ConvertOsKeyType(0xA9);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_BROWSER_SEARCH = ConvertOsKeyType(0xAA);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_BROWSER_FAVORITES = ConvertOsKeyType(0xAB);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_BROWSER_HOME = ConvertOsKeyType(0xAC);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_VOLUME_MUTE = ConvertOsKeyType(0xAD);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_VOLUME_DOWN = ConvertOsKeyType(0xAE);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_VOLUME_UP = ConvertOsKeyType(0xAF);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_MEDIA_NEXT_TRACK = ConvertOsKeyType(0xB0);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_MEDIA_PREV_TRACK = ConvertOsKeyType(0xB1);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_MEDIA_STOP = ConvertOsKeyType(0xB2);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_MEDIA_PLAY_PAUSE = ConvertOsKeyType(0xB3);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_LAUNCH_MAIL = ConvertOsKeyType(0xB4);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_LAUNCH_MEDIA_SELECT = ConvertOsKeyType(0xB5);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_LAUNCH_APP1 = ConvertOsKeyType(0xB6);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_LAUNCH_APP2 = ConvertOsKeyType(0xB7);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_1 = ConvertOsKeyType(0xBA);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_PLUS = ConvertOsKeyType(0xBB);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_COMMA = ConvertOsKeyType(0xBC);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_MINUS = ConvertOsKeyType(0xBD);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_PERIOD = ConvertOsKeyType(0xBE);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_2 = ConvertOsKeyType(0xBF);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_3 = ConvertOsKeyType(0xC0);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_4 = ConvertOsKeyType(0xDB);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_5 = ConvertOsKeyType(0xDC);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_6 = ConvertOsKeyType(0xDD);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_7 = ConvertOsKeyType(0xDE);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_8 = ConvertOsKeyType(0xDF);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_AX = ConvertOsKeyType(0xE1);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_102 = ConvertOsKeyType(0xE2);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_ICO_HELP = ConvertOsKeyType(0xE3);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_ICO_00 = ConvertOsKeyType(0xE4);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_PROCESSKEY = ConvertOsKeyType(0xE5);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_ICO_CLEAR = ConvertOsKeyType(0xE6);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_PACKET = ConvertOsKeyType(0xE7);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_RESET = ConvertOsKeyType(0xE9);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_JUMP = ConvertOsKeyType(0xEA);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_PA1 = ConvertOsKeyType(0xEB);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_PA2 = ConvertOsKeyType(0xEC);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_PA3 = ConvertOsKeyType(0xED);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_WSCTRL = ConvertOsKeyType(0xEE);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_CUSEL = ConvertOsKeyType(0xEF);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_ATTN = ConvertOsKeyType(0xF0);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_FINISH = ConvertOsKeyType(0xF1);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_COPY = ConvertOsKeyType(0xF2);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_AUTO = ConvertOsKeyType(0xF3);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_ENLW = ConvertOsKeyType(0xF4);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_BACKTAB = ConvertOsKeyType(0xF5);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_ATTN = ConvertOsKeyType(0xF6);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_CRSEL = ConvertOsKeyType(0xF7);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_EXSEL = ConvertOsKeyType(0xF8);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_EREOF = ConvertOsKeyType(0xF9);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_PLAY = ConvertOsKeyType(0xFA);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_ZOOM = ConvertOsKeyType(0xFB);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_NONAME = ConvertOsKeyType(0xFC);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_PA1 = ConvertOsKeyType(0xFD);
    [NativeLuaMemberAttribute]
    public static readonly oskeytype OSKEY_OEM_CLEAR = ConvertOsKeyType(0xFE);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerfield ABILITY_IF_BUTTON_POSITION_NORMAL_X = ConvertAbilityIntegerField(1633841272);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerfield ABILITY_IF_BUTTON_POSITION_NORMAL_Y = ConvertAbilityIntegerField(1633841273);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerfield ABILITY_IF_BUTTON_POSITION_ACTIVATED_X = ConvertAbilityIntegerField(1635082872);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerfield ABILITY_IF_BUTTON_POSITION_ACTIVATED_Y = ConvertAbilityIntegerField(1635082873);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerfield ABILITY_IF_BUTTON_POSITION_RESEARCH_X = ConvertAbilityIntegerField(1634889848);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerfield ABILITY_IF_BUTTON_POSITION_RESEARCH_Y = ConvertAbilityIntegerField(1634889849);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerfield ABILITY_IF_MISSILE_SPEED = ConvertAbilityIntegerField(1634562928);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerfield ABILITY_IF_TARGET_ATTACHMENTS = ConvertAbilityIntegerField(1635017059);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerfield ABILITY_IF_CASTER_ATTACHMENTS = ConvertAbilityIntegerField(1633902947);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerfield ABILITY_IF_PRIORITY = ConvertAbilityIntegerField(1634759273);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerfield ABILITY_IF_LEVELS = ConvertAbilityIntegerField(1634493814);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerfield ABILITY_IF_REQUIRED_LEVEL = ConvertAbilityIntegerField(1634888822);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerfield ABILITY_IF_LEVEL_SKIP_REQUIREMENT = ConvertAbilityIntegerField(1634497387);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanfield ABILITY_BF_HERO_ABILITY = ConvertAbilityBooleanField(1634231666);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanfield ABILITY_BF_ITEM_ABILITY = ConvertAbilityBooleanField(1634301029);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanfield ABILITY_BF_CHECK_DEPENDENCIES = ConvertAbilityBooleanField(1633904740);
    [NativeLuaMemberAttribute]
    public static readonly abilityrealfield ABILITY_RF_ARF_MISSILE_ARC = ConvertAbilityRealField(1634558307);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringfield ABILITY_SF_NAME = ConvertAbilityStringField(1634623853);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringfield ABILITY_SF_ICON_ACTIVATED = ConvertAbilityStringField(1635082610);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringfield ABILITY_SF_ICON_RESEARCH = ConvertAbilityStringField(1634886002);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringfield ABILITY_SF_EFFECT_SOUND = ConvertAbilityStringField(1634035315);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringfield ABILITY_SF_EFFECT_SOUND_LOOPING = ConvertAbilityStringField(1634035308);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MANA_COST = ConvertAbilityIntegerLevelField(1634558835);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_NUMBER_OF_WAVES = ConvertAbilityIntegerLevelField(1214413361);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_NUMBER_OF_SHARDS = ConvertAbilityIntegerLevelField(1214413363);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_NUMBER_OF_UNITS_TELEPORTED = ConvertAbilityIntegerLevelField(1215132721);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_SUMMONED_UNIT_COUNT_HWE2 = ConvertAbilityIntegerLevelField(1215784242);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_NUMBER_OF_IMAGES = ConvertAbilityIntegerLevelField(1332570417);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_NUMBER_OF_CORPSES_RAISED_UAN1 = ConvertAbilityIntegerLevelField(1432448561);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MORPHING_FLAGS = ConvertAbilityIntegerLevelField(1164797234);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_STRENGTH_BONUS_NRG5 = ConvertAbilityIntegerLevelField(1316120373);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_DEFENSE_BONUS_NRG6 = ConvertAbilityIntegerLevelField(1316120374);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_NUMBER_OF_TARGETS_HIT = ConvertAbilityIntegerLevelField(1331915826);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_DETECTION_TYPE_OFS1 = ConvertAbilityIntegerLevelField(1332114225);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_NUMBER_OF_SUMMONED_UNITS_OSF2 = ConvertAbilityIntegerLevelField(1332962866);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_NUMBER_OF_SUMMONED_UNITS_EFN1 = ConvertAbilityIntegerLevelField(1164340785);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_NUMBER_OF_CORPSES_RAISED_HRE1 = ConvertAbilityIntegerLevelField(1215456561);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_STACK_FLAGS = ConvertAbilityIntegerLevelField(1214472500);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MINIMUM_NUMBER_OF_UNITS = ConvertAbilityIntegerLevelField(1315205170);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MAXIMUM_NUMBER_OF_UNITS_NDP3 = ConvertAbilityIntegerLevelField(1315205171);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_NUMBER_OF_UNITS_CREATED_NRC2 = ConvertAbilityIntegerLevelField(1316119346);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_SHIELD_LIFE = ConvertAbilityIntegerLevelField(1097691955);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MANA_LOSS_AMS4 = ConvertAbilityIntegerLevelField(1097691956);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_GOLD_PER_INTERVAL_BGM1 = ConvertAbilityIntegerLevelField(1114074417);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MAX_NUMBER_OF_MINERS = ConvertAbilityIntegerLevelField(1114074419);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_CARGO_CAPACITY = ConvertAbilityIntegerLevelField(1130459697);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MAXIMUM_CREEP_LEVEL_DEV3 = ConvertAbilityIntegerLevelField(1147500083);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MAX_CREEP_LEVEL_DEV1 = ConvertAbilityIntegerLevelField(1147500081);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_GOLD_PER_INTERVAL_EGM1 = ConvertAbilityIntegerLevelField(1164406065);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_DEFENSE_REDUCTION = ConvertAbilityIntegerLevelField(1180788017);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_DETECTION_TYPE_FLA1 = ConvertAbilityIntegerLevelField(1181507889);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_FLARE_COUNT = ConvertAbilityIntegerLevelField(1181507891);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MAX_GOLD = ConvertAbilityIntegerLevelField(1198285873);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MINING_CAPACITY = ConvertAbilityIntegerLevelField(1198285875);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MAXIMUM_NUMBER_OF_CORPSES_GYD1 = ConvertAbilityIntegerLevelField(1199137841);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_DAMAGE_TO_TREE = ConvertAbilityIntegerLevelField(1214345777);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_LUMBER_CAPACITY = ConvertAbilityIntegerLevelField(1214345778);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_GOLD_CAPACITY = ConvertAbilityIntegerLevelField(1214345779);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_DEFENSE_INCREASE_INF2 = ConvertAbilityIntegerLevelField(1231971890);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_INTERACTION_TYPE = ConvertAbilityIntegerLevelField(1315271986);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_GOLD_COST_NDT1 = ConvertAbilityIntegerLevelField(1315206193);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_LUMBER_COST_NDT2 = ConvertAbilityIntegerLevelField(1315206194);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_DETECTION_TYPE_NDT3 = ConvertAbilityIntegerLevelField(1315206195);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_STACKING_TYPE_POI4 = ConvertAbilityIntegerLevelField(1349478708);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_STACKING_TYPE_POA5 = ConvertAbilityIntegerLevelField(1349476661);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MAXIMUM_CREEP_LEVEL_PLY1 = ConvertAbilityIntegerLevelField(1349286193);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MAXIMUM_CREEP_LEVEL_POS1 = ConvertAbilityIntegerLevelField(1349481265);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MOVEMENT_UPDATE_FREQUENCY_PRG1 = ConvertAbilityIntegerLevelField(1349674801);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_ATTACK_UPDATE_FREQUENCY_PRG2 = ConvertAbilityIntegerLevelField(1349674802);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MANA_LOSS_PRG6 = ConvertAbilityIntegerLevelField(1349674806);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_UNITS_SUMMONED_TYPE_ONE = ConvertAbilityIntegerLevelField(1382115633);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_UNITS_SUMMONED_TYPE_TWO = ConvertAbilityIntegerLevelField(1382115634);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MAX_UNITS_SUMMONED = ConvertAbilityIntegerLevelField(1432576565);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_ALLOW_WHEN_FULL_REJ3 = ConvertAbilityIntegerLevelField(1382378035);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MAXIMUM_UNITS_CHARGED_TO_CASTER = ConvertAbilityIntegerLevelField(1383096885);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MAXIMUM_UNITS_AFFECTED = ConvertAbilityIntegerLevelField(1383096886);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_DEFENSE_INCREASE_ROA2 = ConvertAbilityIntegerLevelField(1383031090);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MAX_UNITS_ROA7 = ConvertAbilityIntegerLevelField(1383031095);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_ROOTED_WEAPONS = ConvertAbilityIntegerLevelField(1383034673);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_UPROOTED_WEAPONS = ConvertAbilityIntegerLevelField(1383034674);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_UPROOTED_DEFENSE_TYPE = ConvertAbilityIntegerLevelField(1383034676);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_ACCUMULATION_STEP = ConvertAbilityIntegerLevelField(1398893618);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_NUMBER_OF_OWLS = ConvertAbilityIntegerLevelField(1165192756);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_STACKING_TYPE_SPO4 = ConvertAbilityIntegerLevelField(1399877428);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_NUMBER_OF_UNITS = ConvertAbilityIntegerLevelField(1399809073);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_SPIDER_CAPACITY = ConvertAbilityIntegerLevelField(1399873841);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_INTERVALS_BEFORE_CHANGING_TREES = ConvertAbilityIntegerLevelField(1466458418);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_AGILITY_BONUS = ConvertAbilityIntegerLevelField(1231120233);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_INTELLIGENCE_BONUS = ConvertAbilityIntegerLevelField(1231646324);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_STRENGTH_BONUS_ISTR = ConvertAbilityIntegerLevelField(1232303218);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_ATTACK_BONUS = ConvertAbilityIntegerLevelField(1231123572);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_DEFENSE_BONUS_IDEF = ConvertAbilityIntegerLevelField(1231316326);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_SUMMON_1_AMOUNT = ConvertAbilityIntegerLevelField(1232301617);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_SUMMON_2_AMOUNT = ConvertAbilityIntegerLevelField(1232301618);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_EXPERIENCE_GAINED = ConvertAbilityIntegerLevelField(1232629863);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_HIT_POINTS_GAINED_IHPG = ConvertAbilityIntegerLevelField(1231581287);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MANA_POINTS_GAINED_IMPG = ConvertAbilityIntegerLevelField(1231908967);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_HIT_POINTS_GAINED_IHP2 = ConvertAbilityIntegerLevelField(1231581234);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MANA_POINTS_GAINED_IMP2 = ConvertAbilityIntegerLevelField(1231908914);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_DAMAGE_BONUS_DICE = ConvertAbilityIntegerLevelField(1231317347);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_ARMOR_PENALTY_IARP = ConvertAbilityIntegerLevelField(1231123056);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_ENABLED_ATTACK_INDEX_IOB5 = ConvertAbilityIntegerLevelField(1232036405);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_LEVELS_GAINED = ConvertAbilityIntegerLevelField(1231840630);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MAX_LIFE_GAINED = ConvertAbilityIntegerLevelField(1231841638);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MAX_MANA_GAINED = ConvertAbilityIntegerLevelField(1231905134);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_GOLD_GIVEN = ConvertAbilityIntegerLevelField(1231515500);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_LUMBER_GIVEN = ConvertAbilityIntegerLevelField(1231844717);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_DETECTION_TYPE_IFA1 = ConvertAbilityIntegerLevelField(1231446321);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MAXIMUM_CREEP_LEVEL_ICRE = ConvertAbilityIntegerLevelField(1231254117);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MOVEMENT_SPEED_BONUS = ConvertAbilityIntegerLevelField(1231910498);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_HIT_POINTS_REGENERATED_PER_SECOND = ConvertAbilityIntegerLevelField(1231581298);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_SIGHT_RANGE_BONUS = ConvertAbilityIntegerLevelField(1232300386);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_DAMAGE_PER_DURATION = ConvertAbilityIntegerLevelField(1231251044);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MANA_USED_PER_SECOND = ConvertAbilityIntegerLevelField(1231251053);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_EXTRA_MANA_REQUIRED = ConvertAbilityIntegerLevelField(1231251064);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_DETECTION_RADIUS_IDET = ConvertAbilityIntegerLevelField(1231316340);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MANA_LOSS_PER_UNIT_IDIM = ConvertAbilityIntegerLevelField(1231317357);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_DAMAGE_TO_SUMMONED_UNITS_IDID = ConvertAbilityIntegerLevelField(1231317348);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MAXIMUM_NUMBER_OF_UNITS_IREC = ConvertAbilityIntegerLevelField(1232233827);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_DELAY_AFTER_DEATH_SECONDS = ConvertAbilityIntegerLevelField(1232233316);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_RESTORED_LIFE = ConvertAbilityIntegerLevelField(1769104178);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_RESTORED_MANA__1_FOR_CURRENT = ConvertAbilityIntegerLevelField(1769104179);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_HIT_POINTS_RESTORED = ConvertAbilityIntegerLevelField(1231581299);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MANA_POINTS_RESTORED = ConvertAbilityIntegerLevelField(1231908979);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MAXIMUM_NUMBER_OF_UNITS_ITPM = ConvertAbilityIntegerLevelField(1232367725);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_NUMBER_OF_CORPSES_RAISED_CAD1 = ConvertAbilityIntegerLevelField(1130456113);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_TERRAIN_DEFORMATION_DURATION_MS = ConvertAbilityIntegerLevelField(1467118387);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MAXIMUM_UNITS = ConvertAbilityIntegerLevelField(1432646449);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_DETECTION_TYPE_DET1 = ConvertAbilityIntegerLevelField(1147499569);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_GOLD_COST_PER_STRUCTURE = ConvertAbilityIntegerLevelField(1316188209);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_LUMBER_COST_PER_USE = ConvertAbilityIntegerLevelField(1316188210);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_DETECTION_TYPE_NSP3 = ConvertAbilityIntegerLevelField(1316188211);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_NUMBER_OF_SWARM_UNITS = ConvertAbilityIntegerLevelField(1433170737);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MAX_SWARM_UNITS_PER_TARGET = ConvertAbilityIntegerLevelField(1433170739);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_NUMBER_OF_SUMMONED_UNITS_NBA2 = ConvertAbilityIntegerLevelField(1315070258);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MAXIMUM_CREEP_LEVEL_NCH1 = ConvertAbilityIntegerLevelField(1315137585);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_ATTACKS_PREVENTED = ConvertAbilityIntegerLevelField(1316186417);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MAXIMUM_NUMBER_OF_TARGETS_EFK3 = ConvertAbilityIntegerLevelField(1164340019);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_NUMBER_OF_SUMMONED_UNITS_ESV1 = ConvertAbilityIntegerLevelField(1165194801);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MAXIMUM_NUMBER_OF_CORPSES_EXH1 = ConvertAbilityIntegerLevelField(1702389809);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_ITEM_CAPACITY = ConvertAbilityIntegerLevelField(1768846897);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MAXIMUM_NUMBER_OF_TARGETS_SPL2 = ConvertAbilityIntegerLevelField(1936747570);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_ALLOW_WHEN_FULL_IRL3 = ConvertAbilityIntegerLevelField(1769106483);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MAXIMUM_DISPELLED_UNITS = ConvertAbilityIntegerLevelField(1768186675);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_NUMBER_OF_LURES = ConvertAbilityIntegerLevelField(1768779569);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_NEW_TIME_OF_DAY_HOUR = ConvertAbilityIntegerLevelField(1768125489);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_NEW_TIME_OF_DAY_MINUTE = ConvertAbilityIntegerLevelField(1768125490);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_NUMBER_OF_UNITS_CREATED_MEC1 = ConvertAbilityIntegerLevelField(1835361073);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MINIMUM_SPELLS = ConvertAbilityIntegerLevelField(1936745011);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MAXIMUM_SPELLS = ConvertAbilityIntegerLevelField(1936745012);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_DISABLED_ATTACK_INDEX = ConvertAbilityIntegerLevelField(1735549235);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_ENABLED_ATTACK_INDEX_GRA4 = ConvertAbilityIntegerLevelField(1735549236);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MAXIMUM_ATTACKS = ConvertAbilityIntegerLevelField(1735549237);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_BUILDING_TYPES_ALLOWED_NPR1 = ConvertAbilityIntegerLevelField(1315992113);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_BUILDING_TYPES_ALLOWED_NSA1 = ConvertAbilityIntegerLevelField(1316184369);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_ATTACK_MODIFICATION = ConvertAbilityIntegerLevelField(1231118641);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_SUMMONED_UNIT_COUNT_NPA5 = ConvertAbilityIntegerLevelField(1315987765);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_UPGRADE_LEVELS = ConvertAbilityIntegerLevelField(1231514673);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_NUMBER_OF_SUMMONED_UNITS_NDO2 = ConvertAbilityIntegerLevelField(1315204914);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_BEASTS_PER_SECOND = ConvertAbilityIntegerLevelField(1316189233);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_TARGET_TYPE = ConvertAbilityIntegerLevelField(1315138610);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_OPTIONS = ConvertAbilityIntegerLevelField(1315138611);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_ARMOR_PENALTY_NAB3 = ConvertAbilityIntegerLevelField(1315004979);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_WAVE_COUNT_NHS6 = ConvertAbilityIntegerLevelField(1315468086);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MAX_CREEP_LEVEL_NTM3 = ConvertAbilityIntegerLevelField(1316252979);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MISSILE_COUNT = ConvertAbilityIntegerLevelField(1315140403);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_SPLIT_ATTACK_COUNT = ConvertAbilityIntegerLevelField(1315728691);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_GENERATION_COUNT = ConvertAbilityIntegerLevelField(1315728694);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_ROCK_RING_COUNT = ConvertAbilityIntegerLevelField(1316381489);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_WAVE_COUNT_NVC2 = ConvertAbilityIntegerLevelField(1316381490);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_PREFER_HOSTILES_TAU1 = ConvertAbilityIntegerLevelField(1415673137);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_PREFER_FRIENDLIES_TAU2 = ConvertAbilityIntegerLevelField(1415673138);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_MAX_UNITS_TAU3 = ConvertAbilityIntegerLevelField(1415673139);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_NUMBER_OF_PULSES = ConvertAbilityIntegerLevelField(1415673140);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_SUMMONED_UNIT_TYPE_HWE1 = ConvertAbilityIntegerLevelField(1215784241);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_SUMMONED_UNIT_UIN4 = ConvertAbilityIntegerLevelField(1432972852);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_SUMMONED_UNIT_OSF1 = ConvertAbilityIntegerLevelField(1332962865);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_SUMMONED_UNIT_TYPE_EFNU = ConvertAbilityIntegerLevelField(1164340853);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_SUMMONED_UNIT_TYPE_NBAU = ConvertAbilityIntegerLevelField(1315070325);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_SUMMONED_UNIT_TYPE_NTOU = ConvertAbilityIntegerLevelField(1316253557);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_SUMMONED_UNIT_TYPE_ESVU = ConvertAbilityIntegerLevelField(1165194869);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_SUMMONED_UNIT_TYPES = ConvertAbilityIntegerLevelField(1315268145);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_SUMMONED_UNIT_TYPE_NDOU = ConvertAbilityIntegerLevelField(1315204981);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_ALTERNATE_FORM_UNIT_EMEU = ConvertAbilityIntegerLevelField(1164797301);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_PLAGUE_WARD_UNIT_TYPE = ConvertAbilityIntegerLevelField(1097886837);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_ALLOWED_UNIT_TYPE_BTL1 = ConvertAbilityIntegerLevelField(1114926129);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_NEW_UNIT_TYPE = ConvertAbilityIntegerLevelField(1130914097);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_RESULTING_UNIT_TYPE_ENT1 = ConvertAbilityIntegerLevelField(1701737521);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_CORPSE_UNIT_TYPE = ConvertAbilityIntegerLevelField(1199137909);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_ALLOWED_UNIT_TYPE_LOA1 = ConvertAbilityIntegerLevelField(1282367793);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_UNIT_TYPE_FOR_LIMIT_CHECK = ConvertAbilityIntegerLevelField(1382115701);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_WARD_UNIT_TYPE_STAU = ConvertAbilityIntegerLevelField(1400136053);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_EFFECT_ABILITY = ConvertAbilityIntegerLevelField(1232036469);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_CONVERSION_UNIT = ConvertAbilityIntegerLevelField(1315201842);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_UNIT_TO_PRESERVE = ConvertAbilityIntegerLevelField(1316187185);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_UNIT_TYPE_ALLOWED = ConvertAbilityIntegerLevelField(1130916913);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_SWARM_UNIT_TYPE = ConvertAbilityIntegerLevelField(1433170805);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_RESULTING_UNIT_TYPE_COAU = ConvertAbilityIntegerLevelField(1668243829);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_UNIT_TYPE_EXHU = ConvertAbilityIntegerLevelField(1702389877);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_WARD_UNIT_TYPE_HWDU = ConvertAbilityIntegerLevelField(1752654965);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_LURE_UNIT_TYPE = ConvertAbilityIntegerLevelField(1768779637);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_UNIT_TYPE_IPMU = ConvertAbilityIntegerLevelField(1768975733);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_FACTORY_UNIT_ID = ConvertAbilityIntegerLevelField(1316190581);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_SPAWN_UNIT_ID_NFYU = ConvertAbilityIntegerLevelField(1315338613);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_DESTRUCTIBLE_ID = ConvertAbilityIntegerLevelField(1316381557);
    [NativeLuaMemberAttribute]
    public static readonly abilityintegerlevelfield ABILITY_ILF_UPGRADE_TYPE = ConvertAbilityIntegerLevelField(1231514741);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_CASTING_TIME = ConvertAbilityRealLevelField(1633902963);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DURATION_NORMAL = ConvertAbilityRealLevelField(1633973618);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DURATION_HERO = ConvertAbilityRealLevelField(1634231413);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_COOLDOWN = ConvertAbilityRealLevelField(1633903726);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_AREA_OF_EFFECT = ConvertAbilityRealLevelField(1633776229);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_CAST_RANGE = ConvertAbilityRealLevelField(1634885998);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_HBZ2 = ConvertAbilityRealLevelField(1214413362);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_BUILDING_REDUCTION_HBZ4 = ConvertAbilityRealLevelField(1214413364);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_PER_SECOND_HBZ5 = ConvertAbilityRealLevelField(1214413365);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MAXIMUM_DAMAGE_PER_WAVE = ConvertAbilityRealLevelField(1214413366);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MANA_REGENERATION_INCREASE = ConvertAbilityRealLevelField(1214341681);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_CASTING_DELAY = ConvertAbilityRealLevelField(1215132722);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_PER_SECOND_OWW1 = ConvertAbilityRealLevelField(1333229361);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MAGIC_DAMAGE_REDUCTION_OWW2 = ConvertAbilityRealLevelField(1333229362);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_CHANCE_TO_CRITICAL_STRIKE = ConvertAbilityRealLevelField(1331917361);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_MULTIPLIER_OCR2 = ConvertAbilityRealLevelField(1331917362);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_BONUS_OCR3 = ConvertAbilityRealLevelField(1331917363);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_CHANCE_TO_EVADE_OCR4 = ConvertAbilityRealLevelField(1331917364);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_DEALT_PERCENT_OMI2 = ConvertAbilityRealLevelField(1332570418);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_TAKEN_PERCENT_OMI3 = ConvertAbilityRealLevelField(1332570419);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ANIMATION_DELAY = ConvertAbilityRealLevelField(1332570420);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_TRANSITION_TIME = ConvertAbilityRealLevelField(1333226289);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MOVEMENT_SPEED_INCREASE_PERCENT_OWK2 = ConvertAbilityRealLevelField(1333226290);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_BACKSTAB_DAMAGE = ConvertAbilityRealLevelField(1333226291);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_AMOUNT_HEALED_DAMAGED_UDC1 = ConvertAbilityRealLevelField(1432642353);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_LIFE_CONVERTED_TO_MANA = ConvertAbilityRealLevelField(1432645681);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_LIFE_CONVERTED_TO_LIFE = ConvertAbilityRealLevelField(1432645682);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MOVEMENT_SPEED_INCREASE_PERCENT_UAU1 = ConvertAbilityRealLevelField(1432450353);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_LIFE_REGENERATION_INCREASE_PERCENT = ConvertAbilityRealLevelField(1432450354);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_CHANCE_TO_EVADE_EEV1 = ConvertAbilityRealLevelField(1164277297);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_PER_INTERVAL = ConvertAbilityRealLevelField(1164537137);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MANA_DRAINED_PER_SECOND_EIM2 = ConvertAbilityRealLevelField(1164537138);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_BUFFER_MANA_REQUIRED = ConvertAbilityRealLevelField(1164537139);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MAX_MANA_DRAINED = ConvertAbilityRealLevelField(1164796465);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_BOLT_DELAY = ConvertAbilityRealLevelField(1164796466);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_BOLT_LIFETIME = ConvertAbilityRealLevelField(1164796467);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ALTITUDE_ADJUSTMENT_DURATION = ConvertAbilityRealLevelField(1164797235);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_LANDING_DELAY_TIME = ConvertAbilityRealLevelField(1164797236);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ALTERNATE_FORM_HIT_POINT_BONUS = ConvertAbilityRealLevelField(1164797237);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MOVE_SPEED_BONUS_INFO_PANEL_ONLY = ConvertAbilityRealLevelField(1315140149);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ATTACK_SPEED_BONUS_INFO_PANEL_ONLY = ConvertAbilityRealLevelField(1315140150);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_LIFE_REGENERATION_RATE_PER_SECOND = ConvertAbilityRealLevelField(1635149109);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_STUN_DURATION_USL1 = ConvertAbilityRealLevelField(1433627697);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ATTACK_DAMAGE_STOLEN_PERCENT = ConvertAbilityRealLevelField(1432450609);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_UCS1 = ConvertAbilityRealLevelField(1432580913);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MAX_DAMAGE_UCS2 = ConvertAbilityRealLevelField(1432580914);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DISTANCE_UCS3 = ConvertAbilityRealLevelField(1432580915);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_FINAL_AREA_UCS4 = ConvertAbilityRealLevelField(1432580916);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_UIN1 = ConvertAbilityRealLevelField(1432972849);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DURATION = ConvertAbilityRealLevelField(1432972850);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_IMPACT_DELAY = ConvertAbilityRealLevelField(1432972851);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_PER_TARGET_OCL1 = ConvertAbilityRealLevelField(1331915825);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_REDUCTION_PER_TARGET = ConvertAbilityRealLevelField(1331915827);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_EFFECT_DELAY_OEQ1 = ConvertAbilityRealLevelField(1332048177);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_PER_SECOND_TO_BUILDINGS = ConvertAbilityRealLevelField(1332048178);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_UNITS_SLOWED_PERCENT = ConvertAbilityRealLevelField(1332048179);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_FINAL_AREA_OEQ4 = ConvertAbilityRealLevelField(1332048180);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_PER_SECOND_EER1 = ConvertAbilityRealLevelField(1164276273);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_DEALT_TO_ATTACKERS = ConvertAbilityRealLevelField(1164011569);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_LIFE_HEALED = ConvertAbilityRealLevelField(1165259057);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_HEAL_INTERVAL = ConvertAbilityRealLevelField(1165259058);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_BUILDING_REDUCTION_ETQ3 = ConvertAbilityRealLevelField(1165259059);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_INITIAL_IMMUNITY_DURATION = ConvertAbilityRealLevelField(1165259060);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MAX_LIFE_DRAINED_PER_SECOND_PERCENT = ConvertAbilityRealLevelField(1432642609);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_BUILDING_REDUCTION_UDD2 = ConvertAbilityRealLevelField(1432642610);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ARMOR_DURATION = ConvertAbilityRealLevelField(1432772913);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ARMOR_BONUS_UFA2 = ConvertAbilityRealLevelField(1432772914);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_AREA_OF_EFFECT_DAMAGE = ConvertAbilityRealLevelField(1432776241);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_SPECIFIC_TARGET_DAMAGE_UFN2 = ConvertAbilityRealLevelField(1432776242);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_BONUS_HFA1 = ConvertAbilityRealLevelField(1214669105);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_DEALT_ESF1 = ConvertAbilityRealLevelField(1165190705);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_INTERVAL_ESF2 = ConvertAbilityRealLevelField(1165190706);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_BUILDING_REDUCTION_ESF3 = ConvertAbilityRealLevelField(1165190707);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_BONUS_PERCENT = ConvertAbilityRealLevelField(1164014129);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DEFENSE_BONUS_HAV1 = ConvertAbilityRealLevelField(1214346801);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_HIT_POINT_BONUS = ConvertAbilityRealLevelField(1214346802);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_BONUS_HAV3 = ConvertAbilityRealLevelField(1214346803);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MAGIC_DAMAGE_REDUCTION_HAV4 = ConvertAbilityRealLevelField(1214346804);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_CHANCE_TO_BASH = ConvertAbilityRealLevelField(1214408753);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_MULTIPLIER_HBH2 = ConvertAbilityRealLevelField(1214408754);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_BONUS_HBH3 = ConvertAbilityRealLevelField(1214408755);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_CHANCE_TO_MISS_HBH4 = ConvertAbilityRealLevelField(1214408756);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_HTB1 = ConvertAbilityRealLevelField(1215586865);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_AOE_DAMAGE = ConvertAbilityRealLevelField(1215587121);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_SPECIFIC_TARGET_DAMAGE_HTC2 = ConvertAbilityRealLevelField(1215587122);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MOVEMENT_SPEED_REDUCTION_PERCENT_HTC3 = ConvertAbilityRealLevelField(1215587123);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ATTACK_SPEED_REDUCTION_PERCENT_HTC4 = ConvertAbilityRealLevelField(1215587124);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ARMOR_BONUS_HAD1 = ConvertAbilityRealLevelField(1214342193);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_AMOUNT_HEALED_DAMAGED_HHB1 = ConvertAbilityRealLevelField(1214800433);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_EXTRA_DAMAGE_HCA1 = ConvertAbilityRealLevelField(1214472497);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MOVEMENT_SPEED_FACTOR_HCA2 = ConvertAbilityRealLevelField(1214472498);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ATTACK_SPEED_FACTOR_HCA3 = ConvertAbilityRealLevelField(1214472499);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MOVEMENT_SPEED_INCREASE_PERCENT_OAE1 = ConvertAbilityRealLevelField(1331782961);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ATTACK_SPEED_INCREASE_PERCENT_OAE2 = ConvertAbilityRealLevelField(1331782962);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_REINCARNATION_DELAY = ConvertAbilityRealLevelField(1332897073);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_OSH1 = ConvertAbilityRealLevelField(1332963377);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MAXIMUM_DAMAGE_OSH2 = ConvertAbilityRealLevelField(1332963378);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DISTANCE_OSH3 = ConvertAbilityRealLevelField(1332963379);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_FINAL_AREA_OSH4 = ConvertAbilityRealLevelField(1332963380);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_GRAPHIC_DELAY_NFD1 = ConvertAbilityRealLevelField(1315333169);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_GRAPHIC_DURATION_NFD2 = ConvertAbilityRealLevelField(1315333170);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_NFD3 = ConvertAbilityRealLevelField(1315333171);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_SUMMONED_UNIT_DAMAGE_AMS1 = ConvertAbilityRealLevelField(1097691953);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MAGIC_DAMAGE_REDUCTION_AMS2 = ConvertAbilityRealLevelField(1097691954);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_AURA_DURATION = ConvertAbilityRealLevelField(1097886769);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_PER_SECOND_APL2 = ConvertAbilityRealLevelField(1097886770);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DURATION_OF_PLAGUE_WARD = ConvertAbilityRealLevelField(1097886771);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_AMOUNT_OF_HIT_POINTS_REGENERATED = ConvertAbilityRealLevelField(1331786289);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ATTACK_DAMAGE_INCREASE_AKB1 = ConvertAbilityRealLevelField(1097556529);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MANA_LOSS_ADM1 = ConvertAbilityRealLevelField(1097100593);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_SUMMONED_UNIT_DAMAGE_ADM2 = ConvertAbilityRealLevelField(1097100594);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_EXPANSION_AMOUNT = ConvertAbilityRealLevelField(1114401073);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_INTERVAL_DURATION_BGM2 = ConvertAbilityRealLevelField(1114074418);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_RADIUS_OF_MINING_RING = ConvertAbilityRealLevelField(1114074420);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ATTACK_SPEED_INCREASE_PERCENT_BLO1 = ConvertAbilityRealLevelField(1114402609);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MOVEMENT_SPEED_INCREASE_PERCENT_BLO2 = ConvertAbilityRealLevelField(1114402610);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_SCALING_FACTOR = ConvertAbilityRealLevelField(1114402611);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_HIT_POINTS_PER_SECOND_CAN1 = ConvertAbilityRealLevelField(1130458673);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MAX_HIT_POINTS = ConvertAbilityRealLevelField(1130458674);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_PER_SECOND_DEV2 = ConvertAbilityRealLevelField(1147500082);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MOVEMENT_UPDATE_FREQUENCY_CHD1 = ConvertAbilityRealLevelField(1130914865);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ATTACK_UPDATE_FREQUENCY_CHD2 = ConvertAbilityRealLevelField(1130914866);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_SUMMONED_UNIT_DAMAGE_CHD3 = ConvertAbilityRealLevelField(1130914867);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MOVEMENT_SPEED_REDUCTION_PERCENT_CRI1 = ConvertAbilityRealLevelField(1131571505);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ATTACK_SPEED_REDUCTION_PERCENT_CRI2 = ConvertAbilityRealLevelField(1131571506);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_REDUCTION_CRI3 = ConvertAbilityRealLevelField(1131571507);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_CHANCE_TO_MISS_CRS = ConvertAbilityRealLevelField(1131574065);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_FULL_DAMAGE_RADIUS_DDA1 = ConvertAbilityRealLevelField(1147429169);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_FULL_DAMAGE_AMOUNT_DDA2 = ConvertAbilityRealLevelField(1147429170);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_PARTIAL_DAMAGE_RADIUS = ConvertAbilityRealLevelField(1147429171);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_PARTIAL_DAMAGE_AMOUNT = ConvertAbilityRealLevelField(1147429172);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_BUILDING_DAMAGE_FACTOR_SDS1 = ConvertAbilityRealLevelField(1399092017);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MAX_DAMAGE_UCO5 = ConvertAbilityRealLevelField(1432579893);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MOVE_SPEED_BONUS_UCO6 = ConvertAbilityRealLevelField(1432579894);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_TAKEN_PERCENT_DEF1 = ConvertAbilityRealLevelField(1147495985);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_DEALT_PERCENT_DEF2 = ConvertAbilityRealLevelField(1147495986);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MOVEMENT_SPEED_FACTOR_DEF3 = ConvertAbilityRealLevelField(1147495987);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ATTACK_SPEED_FACTOR_DEF4 = ConvertAbilityRealLevelField(1147495988);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MAGIC_DAMAGE_REDUCTION_DEF5 = ConvertAbilityRealLevelField(1147495989);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_CHANCE_TO_DEFLECT = ConvertAbilityRealLevelField(1147495990);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DEFLECT_DAMAGE_TAKEN_PIERCING = ConvertAbilityRealLevelField(1147495991);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DEFLECT_DAMAGE_TAKEN_SPELLS = ConvertAbilityRealLevelField(1147495992);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_RIP_DELAY = ConvertAbilityRealLevelField(1164014641);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_EAT_DELAY = ConvertAbilityRealLevelField(1164014642);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_HIT_POINTS_GAINED_EAT3 = ConvertAbilityRealLevelField(1164014643);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_AIR_UNIT_LOWER_DURATION = ConvertAbilityRealLevelField(1164866353);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_AIR_UNIT_HEIGHT = ConvertAbilityRealLevelField(1164866354);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MELEE_ATTACK_RANGE = ConvertAbilityRealLevelField(1164866355);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_INTERVAL_DURATION_EGM2 = ConvertAbilityRealLevelField(1164406066);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_EFFECT_DELAY_FLA2 = ConvertAbilityRealLevelField(1181507890);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MINING_DURATION = ConvertAbilityRealLevelField(1198285874);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_RADIUS_OF_GRAVESTONES = ConvertAbilityRealLevelField(1199137842);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_RADIUS_OF_CORPSES = ConvertAbilityRealLevelField(1199137843);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_HIT_POINTS_GAINED_HEA1 = ConvertAbilityRealLevelField(1214603569);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_INCREASE_PERCENT_INF1 = ConvertAbilityRealLevelField(1231971889);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_AUTOCAST_RANGE = ConvertAbilityRealLevelField(1231971891);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_LIFE_REGEN_RATE = ConvertAbilityRealLevelField(1231971892);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_GRAPHIC_DELAY_LIT1 = ConvertAbilityRealLevelField(1281979441);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_GRAPHIC_DURATION_LIT2 = ConvertAbilityRealLevelField(1281979442);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_PER_SECOND_LSH1 = ConvertAbilityRealLevelField(1282631729);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MANA_GAINED = ConvertAbilityRealLevelField(1298297905);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_HIT_POINTS_GAINED_MBT2 = ConvertAbilityRealLevelField(1298297906);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_AUTOCAST_REQUIREMENT = ConvertAbilityRealLevelField(1298297907);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_WATER_HEIGHT = ConvertAbilityRealLevelField(1298297908);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ACTIVATION_DELAY_MIN1 = ConvertAbilityRealLevelField(1298755121);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_INVISIBILITY_TRANSITION_TIME = ConvertAbilityRealLevelField(1298755122);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ACTIVATION_RADIUS = ConvertAbilityRealLevelField(1315271985);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_AMOUNT_REGENERATED = ConvertAbilityRealLevelField(1098018097);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_PER_SECOND_POI1 = ConvertAbilityRealLevelField(1349478705);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ATTACK_SPEED_FACTOR_POI2 = ConvertAbilityRealLevelField(1349478706);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MOVEMENT_SPEED_FACTOR_POI3 = ConvertAbilityRealLevelField(1349478707);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_EXTRA_DAMAGE_POA1 = ConvertAbilityRealLevelField(1349476657);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_PER_SECOND_POA2 = ConvertAbilityRealLevelField(1349476658);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ATTACK_SPEED_FACTOR_POA3 = ConvertAbilityRealLevelField(1349476659);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MOVEMENT_SPEED_FACTOR_POA4 = ConvertAbilityRealLevelField(1349476660);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_AMPLIFICATION = ConvertAbilityRealLevelField(1349481266);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_CHANCE_TO_STOMP_PERCENT = ConvertAbilityRealLevelField(1466004017);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_DEALT_WAR2 = ConvertAbilityRealLevelField(1466004018);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_FULL_DAMAGE_RADIUS_WAR3 = ConvertAbilityRealLevelField(1466004019);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_HALF_DAMAGE_RADIUS_WAR4 = ConvertAbilityRealLevelField(1466004020);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_SUMMONED_UNIT_DAMAGE_PRG3 = ConvertAbilityRealLevelField(1349674803);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_UNIT_PAUSE_DURATION = ConvertAbilityRealLevelField(1349674804);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_HERO_PAUSE_DURATION = ConvertAbilityRealLevelField(1349674805);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_HIT_POINTS_GAINED_REJ1 = ConvertAbilityRealLevelField(1382378033);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MANA_POINTS_GAINED_REJ2 = ConvertAbilityRealLevelField(1382378034);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MINIMUM_LIFE_REQUIRED = ConvertAbilityRealLevelField(1383096883);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MINIMUM_MANA_REQUIRED = ConvertAbilityRealLevelField(1383096884);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_REPAIR_COST_RATIO = ConvertAbilityRealLevelField(1382379569);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_REPAIR_TIME_RATIO = ConvertAbilityRealLevelField(1382379570);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_POWERBUILD_COST = ConvertAbilityRealLevelField(1382379571);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_POWERBUILD_RATE = ConvertAbilityRealLevelField(1382379572);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_NAVAL_RANGE_BONUS = ConvertAbilityRealLevelField(1382379573);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_INCREASE_PERCENT_ROA1 = ConvertAbilityRealLevelField(1383031089);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_LIFE_REGENERATION_RATE = ConvertAbilityRealLevelField(1383031091);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MANA_REGEN = ConvertAbilityRealLevelField(1383031092);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_INCREASE = ConvertAbilityRealLevelField(1315074609);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_SALVAGE_COST_RATIO = ConvertAbilityRealLevelField(1398893617);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_IN_FLIGHT_SIGHT_RADIUS = ConvertAbilityRealLevelField(1165192753);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_HOVERING_SIGHT_RADIUS = ConvertAbilityRealLevelField(1165192754);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_HOVERING_HEIGHT = ConvertAbilityRealLevelField(1165192755);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DURATION_OF_OWLS = ConvertAbilityRealLevelField(1165192757);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_FADE_DURATION = ConvertAbilityRealLevelField(1399352625);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAY_NIGHT_DURATION = ConvertAbilityRealLevelField(1399352626);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ACTION_DURATION = ConvertAbilityRealLevelField(1399352627);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MOVEMENT_SPEED_FACTOR_SLO1 = ConvertAbilityRealLevelField(1399615281);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ATTACK_SPEED_FACTOR_SLO2 = ConvertAbilityRealLevelField(1399615282);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_PER_SECOND_SPO1 = ConvertAbilityRealLevelField(1399877425);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MOVEMENT_SPEED_FACTOR_SPO2 = ConvertAbilityRealLevelField(1399877426);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ATTACK_SPEED_FACTOR_SPO3 = ConvertAbilityRealLevelField(1399877427);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ACTIVATION_DELAY_STA1 = ConvertAbilityRealLevelField(1400135985);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DETECTION_RADIUS_STA2 = ConvertAbilityRealLevelField(1400135986);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DETONATION_RADIUS = ConvertAbilityRealLevelField(1400135987);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_STUN_DURATION_STA4 = ConvertAbilityRealLevelField(1400135988);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ATTACK_SPEED_BONUS_PERCENT = ConvertAbilityRealLevelField(1432905265);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_PER_SECOND_UHF2 = ConvertAbilityRealLevelField(1432905266);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_LUMBER_PER_INTERVAL = ConvertAbilityRealLevelField(1466458417);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ART_ATTACHMENT_HEIGHT = ConvertAbilityRealLevelField(1466458419);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_TELEPORT_AREA_WIDTH = ConvertAbilityRealLevelField(1467117617);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_TELEPORT_AREA_HEIGHT = ConvertAbilityRealLevelField(1467117618);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_LIFE_STOLEN_PER_ATTACK = ConvertAbilityRealLevelField(1232494957);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_BONUS_IDAM = ConvertAbilityRealLevelField(1231315309);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_CHANCE_TO_HIT_UNITS_PERCENT = ConvertAbilityRealLevelField(1232036402);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_CHANCE_TO_HIT_HEROS_PERCENT = ConvertAbilityRealLevelField(1232036403);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_CHANCE_TO_HIT_SUMMONS_PERCENT = ConvertAbilityRealLevelField(1232036404);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DELAY_FOR_TARGET_EFFECT = ConvertAbilityRealLevelField(1231316332);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_DEALT_PERCENT_OF_NORMAL = ConvertAbilityRealLevelField(1231645796);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_RECEIVED_MULTIPLIER = ConvertAbilityRealLevelField(1231645815);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MANA_REGENERATION_BONUS_AS_FRACTION_OF_NORMAL = ConvertAbilityRealLevelField(1231909488);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MOVEMENT_SPEED_INCREASE_ISPI = ConvertAbilityRealLevelField(1232302185);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_PER_SECOND_IDPS = ConvertAbilityRealLevelField(1231319155);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ATTACK_DAMAGE_INCREASE_CAC1 = ConvertAbilityRealLevelField(1130455857);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_PER_SECOND_COR1 = ConvertAbilityRealLevelField(1131377201);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ATTACK_SPEED_INCREASE_ISX1 = ConvertAbilityRealLevelField(1232304177);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_WRS1 = ConvertAbilityRealLevelField(1467118385);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_TERRAIN_DEFORMATION_AMPLITUDE = ConvertAbilityRealLevelField(1467118386);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_CTC1 = ConvertAbilityRealLevelField(1131701041);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_EXTRA_DAMAGE_TO_TARGET = ConvertAbilityRealLevelField(1131701042);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MOVEMENT_SPEED_REDUCTION_CTC3 = ConvertAbilityRealLevelField(1131701043);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ATTACK_SPEED_REDUCTION_CTC4 = ConvertAbilityRealLevelField(1131701044);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_CTB1 = ConvertAbilityRealLevelField(1131700785);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_CASTING_DELAY_SECONDS = ConvertAbilityRealLevelField(1432646450);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MANA_LOSS_PER_UNIT_DTN1 = ConvertAbilityRealLevelField(1148481073);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_TO_SUMMONED_UNITS_DTN2 = ConvertAbilityRealLevelField(1148481074);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_TRANSITION_TIME_SECONDS = ConvertAbilityRealLevelField(1232499505);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MANA_DRAINED_PER_SECOND_NMR1 = ConvertAbilityRealLevelField(1315795505);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_CHANCE_TO_REDUCE_DAMAGE_PERCENT = ConvertAbilityRealLevelField(1400073009);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MINIMUM_DAMAGE = ConvertAbilityRealLevelField(1400073010);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_IGNORED_DAMAGE = ConvertAbilityRealLevelField(1400073011);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_FULL_DAMAGE_DEALT = ConvertAbilityRealLevelField(1214673713);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_FULL_DAMAGE_INTERVAL = ConvertAbilityRealLevelField(1214673714);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_HALF_DAMAGE_DEALT = ConvertAbilityRealLevelField(1214673715);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_HALF_DAMAGE_INTERVAL = ConvertAbilityRealLevelField(1214673716);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_BUILDING_REDUCTION_HFS5 = ConvertAbilityRealLevelField(1214673717);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MAXIMUM_DAMAGE_HFS6 = ConvertAbilityRealLevelField(1214673718);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MANA_PER_HIT_POINT = ConvertAbilityRealLevelField(1315795761);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_ABSORBED_PERCENT = ConvertAbilityRealLevelField(1315795762);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_WAVE_DISTANCE = ConvertAbilityRealLevelField(1432972593);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_WAVE_TIME_SECONDS = ConvertAbilityRealLevelField(1432972594);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_DEALT_UIM3 = ConvertAbilityRealLevelField(1432972595);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_AIR_TIME_SECONDS_UIM4 = ConvertAbilityRealLevelField(1432972596);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_UNIT_RELEASE_INTERVAL_SECONDS = ConvertAbilityRealLevelField(1433170738);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_RETURN_FACTOR = ConvertAbilityRealLevelField(1433170740);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_RETURN_THRESHOLD = ConvertAbilityRealLevelField(1433170741);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_RETURNED_DAMAGE_FACTOR = ConvertAbilityRealLevelField(1433695025);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_RECEIVED_DAMAGE_FACTOR = ConvertAbilityRealLevelField(1433695026);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DEFENSE_BONUS_UTS3 = ConvertAbilityRealLevelField(1433695027);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_BONUS_NBA1 = ConvertAbilityRealLevelField(1315070257);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_SUMMONED_UNIT_DURATION_SECONDS_NBA3 = ConvertAbilityRealLevelField(1315070259);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MANA_PER_SUMMONED_HITPOINT = ConvertAbilityRealLevelField(1131243314);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_CHARGE_FOR_CURRENT_LIFE = ConvertAbilityRealLevelField(1131243315);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_HIT_POINTS_DRAINED = ConvertAbilityRealLevelField(1315205681);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MANA_POINTS_DRAINED = ConvertAbilityRealLevelField(1315205682);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DRAIN_INTERVAL_SECONDS = ConvertAbilityRealLevelField(1315205683);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_LIFE_TRANSFERRED_PER_SECOND = ConvertAbilityRealLevelField(1315205684);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MANA_TRANSFERRED_PER_SECOND = ConvertAbilityRealLevelField(1315205685);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_BONUS_LIFE_FACTOR = ConvertAbilityRealLevelField(1315205686);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_BONUS_LIFE_DECAY = ConvertAbilityRealLevelField(1315205687);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_BONUS_MANA_FACTOR = ConvertAbilityRealLevelField(1315205688);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_BONUS_MANA_DECAY = ConvertAbilityRealLevelField(1315205689);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_CHANCE_TO_MISS_PERCENT = ConvertAbilityRealLevelField(1316186418);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MOVEMENT_SPEED_MODIFIER = ConvertAbilityRealLevelField(1316186419);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ATTACK_SPEED_MODIFIER = ConvertAbilityRealLevelField(1316186420);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_PER_SECOND_TDG1 = ConvertAbilityRealLevelField(1415866161);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MEDIUM_DAMAGE_RADIUS_TDG2 = ConvertAbilityRealLevelField(1415866162);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MEDIUM_DAMAGE_PER_SECOND = ConvertAbilityRealLevelField(1415866163);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_SMALL_DAMAGE_RADIUS_TDG4 = ConvertAbilityRealLevelField(1415866164);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_SMALL_DAMAGE_PER_SECOND = ConvertAbilityRealLevelField(1415866165);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_AIR_TIME_SECONDS_TSP1 = ConvertAbilityRealLevelField(1416851505);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MINIMUM_HIT_INTERVAL_SECONDS = ConvertAbilityRealLevelField(1416851506);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_PER_SECOND_NBF5 = ConvertAbilityRealLevelField(1315071541);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MAXIMUM_RANGE = ConvertAbilityRealLevelField(1164078129);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MINIMUM_RANGE = ConvertAbilityRealLevelField(1164078130);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_PER_TARGET_EFK1 = ConvertAbilityRealLevelField(1164340017);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MAXIMUM_TOTAL_DAMAGE = ConvertAbilityRealLevelField(1164340018);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MAXIMUM_SPEED_ADJUSTMENT = ConvertAbilityRealLevelField(1164340020);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DECAYING_DAMAGE = ConvertAbilityRealLevelField(1165191217);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MOVEMENT_SPEED_FACTOR_ESH2 = ConvertAbilityRealLevelField(1165191218);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ATTACK_SPEED_FACTOR_ESH3 = ConvertAbilityRealLevelField(1165191219);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DECAY_POWER = ConvertAbilityRealLevelField(1165191220);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_INITIAL_DAMAGE_ESH5 = ConvertAbilityRealLevelField(1165191221);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MAXIMUM_LIFE_ABSORBED = ConvertAbilityRealLevelField(1633841969);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MAXIMUM_MANA_ABSORBED = ConvertAbilityRealLevelField(1633841970);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MOVEMENT_SPEED_INCREASE_BSK1 = ConvertAbilityRealLevelField(1651731249);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ATTACK_SPEED_INCREASE_BSK2 = ConvertAbilityRealLevelField(1651731250);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_TAKEN_INCREASE = ConvertAbilityRealLevelField(1651731251);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_LIFE_PER_UNIT = ConvertAbilityRealLevelField(1685482801);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MANA_PER_UNIT = ConvertAbilityRealLevelField(1685482802);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_LIFE_PER_BUFF = ConvertAbilityRealLevelField(1685482803);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MANA_PER_BUFF = ConvertAbilityRealLevelField(1685482804);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_SUMMONED_UNIT_DAMAGE_DVM5 = ConvertAbilityRealLevelField(1685482805);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_BONUS_FAK1 = ConvertAbilityRealLevelField(1717660465);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MEDIUM_DAMAGE_FACTOR_FAK2 = ConvertAbilityRealLevelField(1717660466);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_SMALL_DAMAGE_FACTOR_FAK3 = ConvertAbilityRealLevelField(1717660467);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_FULL_DAMAGE_RADIUS_FAK4 = ConvertAbilityRealLevelField(1717660468);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_HALF_DAMAGE_RADIUS_FAK5 = ConvertAbilityRealLevelField(1717660469);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_EXTRA_DAMAGE_PER_SECOND = ConvertAbilityRealLevelField(1818849585);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MOVEMENT_SPEED_REDUCTION_LIQ2 = ConvertAbilityRealLevelField(1818849586);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ATTACK_SPEED_REDUCTION_LIQ3 = ConvertAbilityRealLevelField(1818849587);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MAGIC_DAMAGE_FACTOR = ConvertAbilityRealLevelField(1835625777);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_UNIT_DAMAGE_PER_MANA_POINT = ConvertAbilityRealLevelField(1835428913);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_HERO_DAMAGE_PER_MANA_POINT = ConvertAbilityRealLevelField(1835428914);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_UNIT_MAXIMUM_DAMAGE = ConvertAbilityRealLevelField(1835428915);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_HERO_MAXIMUM_DAMAGE = ConvertAbilityRealLevelField(1835428916);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_COOLDOWN = ConvertAbilityRealLevelField(1835428917);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DISTRIBUTED_DAMAGE_FACTOR_SPL1 = ConvertAbilityRealLevelField(1936747569);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_LIFE_REGENERATED = ConvertAbilityRealLevelField(1769106481);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MANA_REGENERATED = ConvertAbilityRealLevelField(1769106482);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MANA_LOSS_PER_UNIT_IDC1 = ConvertAbilityRealLevelField(1768186673);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_SUMMONED_UNIT_DAMAGE_IDC2 = ConvertAbilityRealLevelField(1768186674);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ACTIVATION_DELAY_IMO2 = ConvertAbilityRealLevelField(1768779570);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_LURE_INTERVAL_SECONDS = ConvertAbilityRealLevelField(1768779571);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_BONUS_ISR1 = ConvertAbilityRealLevelField(1769173553);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_REDUCTION_ISR2 = ConvertAbilityRealLevelField(1769173554);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_BONUS_IPV1 = ConvertAbilityRealLevelField(1768977969);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_LIFE_STEAL_AMOUNT = ConvertAbilityRealLevelField(1768977970);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_LIFE_RESTORED_FACTOR = ConvertAbilityRealLevelField(1634956337);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MANA_RESTORED_FACTOR = ConvertAbilityRealLevelField(1634956338);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ATTACH_DELAY = ConvertAbilityRealLevelField(1735549233);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_REMOVE_DELAY = ConvertAbilityRealLevelField(1735549234);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_HERO_REGENERATION_DELAY = ConvertAbilityRealLevelField(1316184370);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_UNIT_REGENERATION_DELAY = ConvertAbilityRealLevelField(1316184371);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MAGIC_DAMAGE_REDUCTION_NSA4 = ConvertAbilityRealLevelField(1316184372);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_HIT_POINTS_PER_SECOND_NSA5 = ConvertAbilityRealLevelField(1316184373);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_TO_SUMMONED_UNITS_IXS1 = ConvertAbilityRealLevelField(1232630577);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MAGIC_DAMAGE_REDUCTION_IXS2 = ConvertAbilityRealLevelField(1232630578);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_SUMMONED_UNIT_DURATION = ConvertAbilityRealLevelField(1315987766);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_SHIELD_COOLDOWN_TIME = ConvertAbilityRealLevelField(1316185393);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_PER_SECOND_NDO1 = ConvertAbilityRealLevelField(1315204913);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_SUMMONED_UNIT_DURATION_SECONDS_NDO3 = ConvertAbilityRealLevelField(1315204915);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MEDIUM_DAMAGE_RADIUS_FLK1 = ConvertAbilityRealLevelField(1718381361);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_SMALL_DAMAGE_RADIUS_FLK2 = ConvertAbilityRealLevelField(1718381362);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_FULL_DAMAGE_AMOUNT_FLK3 = ConvertAbilityRealLevelField(1718381363);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MEDIUM_DAMAGE_AMOUNT = ConvertAbilityRealLevelField(1718381364);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_SMALL_DAMAGE_AMOUNT = ConvertAbilityRealLevelField(1718381365);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MOVEMENT_SPEED_REDUCTION_PERCENT_HBN1 = ConvertAbilityRealLevelField(1214410289);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ATTACK_SPEED_REDUCTION_PERCENT_HBN2 = ConvertAbilityRealLevelField(1214410290);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MAX_MANA_DRAINED_UNITS = ConvertAbilityRealLevelField(1717726001);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_RATIO_UNITS_PERCENT = ConvertAbilityRealLevelField(1717726002);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MAX_MANA_DRAINED_HEROS = ConvertAbilityRealLevelField(1717726003);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_RATIO_HEROS_PERCENT = ConvertAbilityRealLevelField(1717726004);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_SUMMONED_DAMAGE = ConvertAbilityRealLevelField(1717726005);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DISTRIBUTED_DAMAGE_FACTOR_NCA1 = ConvertAbilityRealLevelField(1852006705);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_INITIAL_DAMAGE_PXF1 = ConvertAbilityRealLevelField(1886938673);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_PER_SECOND_PXF2 = ConvertAbilityRealLevelField(1886938674);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_PER_SECOND_MLS1 = ConvertAbilityRealLevelField(1835823921);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_BEAST_COLLISION_RADIUS = ConvertAbilityRealLevelField(1316189234);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_AMOUNT_NST3 = ConvertAbilityRealLevelField(1316189235);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_RADIUS = ConvertAbilityRealLevelField(1316189236);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_DELAY = ConvertAbilityRealLevelField(1316189237);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_FOLLOW_THROUGH_TIME = ConvertAbilityRealLevelField(1315138609);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ART_DURATION = ConvertAbilityRealLevelField(1315138612);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MOVEMENT_SPEED_REDUCTION_PERCENT_NAB1 = ConvertAbilityRealLevelField(1315004977);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ATTACK_SPEED_REDUCTION_PERCENT_NAB2 = ConvertAbilityRealLevelField(1315004978);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_PRIMARY_DAMAGE = ConvertAbilityRealLevelField(1315004980);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_SECONDARY_DAMAGE = ConvertAbilityRealLevelField(1315004981);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_INTERVAL_NAB6 = ConvertAbilityRealLevelField(1315004982);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_GOLD_COST_FACTOR = ConvertAbilityRealLevelField(1316252977);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_LUMBER_COST_FACTOR = ConvertAbilityRealLevelField(1316252978);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MOVE_SPEED_BONUS_NEG1 = ConvertAbilityRealLevelField(1315268401);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_BONUS_NEG2 = ConvertAbilityRealLevelField(1315268402);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_AMOUNT_NCS1 = ConvertAbilityRealLevelField(1315140401);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_INTERVAL_NCS2 = ConvertAbilityRealLevelField(1315140402);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MAX_DAMAGE_NCS4 = ConvertAbilityRealLevelField(1315140404);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_BUILDING_DAMAGE_FACTOR_NCS5 = ConvertAbilityRealLevelField(1315140405);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_EFFECT_DURATION = ConvertAbilityRealLevelField(1315140406);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_SPAWN_INTERVAL_NSY1 = ConvertAbilityRealLevelField(1316190513);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_SPAWN_UNIT_DURATION = ConvertAbilityRealLevelField(1316190515);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_SPAWN_UNIT_OFFSET = ConvertAbilityRealLevelField(1316190516);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_LEASH_RANGE_NSY5 = ConvertAbilityRealLevelField(1316190517);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_SPAWN_INTERVAL_NFY1 = ConvertAbilityRealLevelField(1315338545);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_LEASH_RANGE_NFY2 = ConvertAbilityRealLevelField(1315338546);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_CHANCE_TO_DEMOLISH = ConvertAbilityRealLevelField(1315202353);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_MULTIPLIER_BUILDINGS = ConvertAbilityRealLevelField(1315202354);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_MULTIPLIER_UNITS = ConvertAbilityRealLevelField(1315202355);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_MULTIPLIER_HEROES = ConvertAbilityRealLevelField(1315202356);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_BONUS_DAMAGE_MULTIPLIER = ConvertAbilityRealLevelField(1315529521);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DEATH_DAMAGE_FULL_AMOUNT = ConvertAbilityRealLevelField(1315529522);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DEATH_DAMAGE_FULL_AREA = ConvertAbilityRealLevelField(1315529523);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DEATH_DAMAGE_HALF_AMOUNT = ConvertAbilityRealLevelField(1315529524);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DEATH_DAMAGE_HALF_AREA = ConvertAbilityRealLevelField(1315529525);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DEATH_DAMAGE_DELAY = ConvertAbilityRealLevelField(1315529526);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_AMOUNT_NSO1 = ConvertAbilityRealLevelField(1316187953);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_PERIOD = ConvertAbilityRealLevelField(1316187954);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_DAMAGE_PENALTY = ConvertAbilityRealLevelField(1316187955);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MOVEMENT_SPEED_REDUCTION_PERCENT_NSO4 = ConvertAbilityRealLevelField(1316187956);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_ATTACK_SPEED_REDUCTION_PERCENT_NSO5 = ConvertAbilityRealLevelField(1316187957);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_SPLIT_DELAY = ConvertAbilityRealLevelField(1315728690);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_MAX_HITPOINT_FACTOR = ConvertAbilityRealLevelField(1315728692);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_LIFE_DURATION_SPLIT_BONUS = ConvertAbilityRealLevelField(1315728693);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_WAVE_INTERVAL = ConvertAbilityRealLevelField(1316381491);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_BUILDING_DAMAGE_FACTOR_NVC4 = ConvertAbilityRealLevelField(1316381492);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_FULL_DAMAGE_AMOUNT_NVC5 = ConvertAbilityRealLevelField(1316381493);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_HALF_DAMAGE_FACTOR = ConvertAbilityRealLevelField(1316381494);
    [NativeLuaMemberAttribute]
    public static readonly abilityreallevelfield ABILITY_RLF_INTERVAL_BETWEEN_PULSES = ConvertAbilityRealLevelField(1415673141);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_PERCENT_BONUS_HAB2 = ConvertAbilityBooleanLevelField(1214341682);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_USE_TELEPORT_CLUSTERING_HMT3 = ConvertAbilityBooleanLevelField(1215132723);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_NEVER_MISS_OCR5 = ConvertAbilityBooleanLevelField(1331917365);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_EXCLUDE_ITEM_DAMAGE = ConvertAbilityBooleanLevelField(1331917366);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_BACKSTAB_DAMAGE = ConvertAbilityBooleanLevelField(1333226292);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_INHERIT_UPGRADES_UAN3 = ConvertAbilityBooleanLevelField(1432448563);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_MANA_CONVERSION_AS_PERCENT = ConvertAbilityBooleanLevelField(1432645683);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_LIFE_CONVERSION_AS_PERCENT = ConvertAbilityBooleanLevelField(1432645684);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_LEAVE_TARGET_ALIVE = ConvertAbilityBooleanLevelField(1432645685);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_PERCENT_BONUS_UAU3 = ConvertAbilityBooleanLevelField(1432450355);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_DAMAGE_IS_PERCENT_RECEIVED = ConvertAbilityBooleanLevelField(1164011570);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_MELEE_BONUS = ConvertAbilityBooleanLevelField(1164014130);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_RANGED_BONUS = ConvertAbilityBooleanLevelField(1164014131);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_FLAT_BONUS = ConvertAbilityBooleanLevelField(1164014132);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_NEVER_MISS_HBH5 = ConvertAbilityBooleanLevelField(1214408757);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_PERCENT_BONUS_HAD2 = ConvertAbilityBooleanLevelField(1214342194);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_CAN_DEACTIVATE = ConvertAbilityBooleanLevelField(1214542641);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_RAISED_UNITS_ARE_INVULNERABLE = ConvertAbilityBooleanLevelField(1215456562);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_PERCENTAGE_OAR2 = ConvertAbilityBooleanLevelField(1331786290);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_SUMMON_BUSY_UNITS = ConvertAbilityBooleanLevelField(1114926130);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_CREATES_BLIGHT = ConvertAbilityBooleanLevelField(1114401074);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_EXPLODES_ON_DEATH = ConvertAbilityBooleanLevelField(1399092022);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_ALWAYS_AUTOCAST_FAE2 = ConvertAbilityBooleanLevelField(1180788018);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_REGENERATE_ONLY_AT_NIGHT = ConvertAbilityBooleanLevelField(1298297909);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_SHOW_SELECT_UNIT_BUTTON = ConvertAbilityBooleanLevelField(1315271987);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_SHOW_UNIT_INDICATOR = ConvertAbilityBooleanLevelField(1315271988);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_CHARGE_OWNING_PLAYER = ConvertAbilityBooleanLevelField(1097757494);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_PERCENTAGE_ARM2 = ConvertAbilityBooleanLevelField(1098018098);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_TARGET_IS_INVULNERABLE = ConvertAbilityBooleanLevelField(1349481267);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_TARGET_IS_MAGIC_IMMUNE = ConvertAbilityBooleanLevelField(1349481268);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_KILL_ON_CASTER_DEATH = ConvertAbilityBooleanLevelField(1432576566);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_NO_TARGET_REQUIRED_REJ4 = ConvertAbilityBooleanLevelField(1382378036);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_ACCEPTS_GOLD = ConvertAbilityBooleanLevelField(1383362097);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_ACCEPTS_LUMBER = ConvertAbilityBooleanLevelField(1383362098);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_PREFER_HOSTILES_ROA5 = ConvertAbilityBooleanLevelField(1383031093);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_PREFER_FRIENDLIES_ROA6 = ConvertAbilityBooleanLevelField(1383031094);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_ROOTED_TURNING = ConvertAbilityBooleanLevelField(1383034675);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_ALWAYS_AUTOCAST_SLO3 = ConvertAbilityBooleanLevelField(1399615283);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_HIDE_BUTTON = ConvertAbilityBooleanLevelField(1231579492);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_USE_TELEPORT_CLUSTERING_ITP2 = ConvertAbilityBooleanLevelField(1232367666);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_IMMUNE_TO_MORPH_EFFECTS = ConvertAbilityBooleanLevelField(1165256753);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_DOES_NOT_BLOCK_BUILDINGS = ConvertAbilityBooleanLevelField(1165256754);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_AUTO_ACQUIRE_ATTACK_TARGETS = ConvertAbilityBooleanLevelField(1198026545);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_IMMUNE_TO_MORPH_EFFECTS_GHO2 = ConvertAbilityBooleanLevelField(1198026546);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_DO_NOT_BLOCK_BUILDINGS = ConvertAbilityBooleanLevelField(1198026547);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_INCLUDE_RANGED_DAMAGE = ConvertAbilityBooleanLevelField(1400073012);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_INCLUDE_MELEE_DAMAGE = ConvertAbilityBooleanLevelField(1400073013);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_MOVE_TO_PARTNER = ConvertAbilityBooleanLevelField(1668243762);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_CAN_BE_DISPELLED = ConvertAbilityBooleanLevelField(1668899633);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_IGNORE_FRIENDLY_BUFFS = ConvertAbilityBooleanLevelField(1685482806);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_DROP_ITEMS_ON_DEATH = ConvertAbilityBooleanLevelField(1768846898);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_CAN_USE_ITEMS = ConvertAbilityBooleanLevelField(1768846899);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_CAN_GET_ITEMS = ConvertAbilityBooleanLevelField(1768846900);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_CAN_DROP_ITEMS = ConvertAbilityBooleanLevelField(1768846901);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_REPAIRS_ALLOWED = ConvertAbilityBooleanLevelField(1818849588);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_CASTER_ONLY_SPLASH = ConvertAbilityBooleanLevelField(1835428918);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_NO_TARGET_REQUIRED_IRL4 = ConvertAbilityBooleanLevelField(1769106484);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_DISPEL_ON_ATTACK = ConvertAbilityBooleanLevelField(1769106485);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_AMOUNT_IS_RAW_VALUE = ConvertAbilityBooleanLevelField(1768977971);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_SHARED_SPELL_COOLDOWN = ConvertAbilityBooleanLevelField(1936745010);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_SLEEP_ONCE = ConvertAbilityBooleanLevelField(1936482609);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_ALLOW_ON_ANY_PLAYER_SLOT = ConvertAbilityBooleanLevelField(1936482610);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_DISABLE_OTHER_ABILITIES = ConvertAbilityBooleanLevelField(1315138613);
    [NativeLuaMemberAttribute]
    public static readonly abilitybooleanlevelfield ABILITY_BLF_ALLOW_BOUNTY = ConvertAbilityBooleanLevelField(1316252980);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_ICON_NORMAL = ConvertAbilityStringLevelField(1633776244);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_CASTER = ConvertAbilityStringLevelField(1633902964);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_TARGET = ConvertAbilityStringLevelField(1635017076);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_SPECIAL = ConvertAbilityStringLevelField(1634951540);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_EFFECT = ConvertAbilityStringLevelField(1634034036);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_AREA_EFFECT = ConvertAbilityStringLevelField(1633772897);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_LIGHTNING_EFFECTS = ConvertAbilityStringLevelField(1634494823);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_MISSILE_ART = ConvertAbilityStringLevelField(1634558324);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_TOOLTIP_LEARN = ConvertAbilityStringLevelField(1634887028);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_TOOLTIP_LEARN_EXTENDED = ConvertAbilityStringLevelField(1634891124);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_TOOLTIP_NORMAL = ConvertAbilityStringLevelField(1635020849);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_TOOLTIP_TURN_OFF = ConvertAbilityStringLevelField(1635087409);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_TOOLTIP_NORMAL_EXTENDED = ConvertAbilityStringLevelField(1635082801);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_TOOLTIP_TURN_OFF_EXTENDED = ConvertAbilityStringLevelField(1635087665);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_NORMAL_FORM_UNIT_EME1 = ConvertAbilityStringLevelField(1164797233);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_SPAWNED_UNITS = ConvertAbilityStringLevelField(1315205169);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_ABILITY_FOR_UNIT_CREATION = ConvertAbilityStringLevelField(1316119345);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_NORMAL_FORM_UNIT_MIL1 = ConvertAbilityStringLevelField(1298754609);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_ALTERNATE_FORM_UNIT_MIL2 = ConvertAbilityStringLevelField(1298754610);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_BASE_ORDER_ID_ANS5 = ConvertAbilityStringLevelField(1097757493);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_MORPH_UNITS_GROUND = ConvertAbilityStringLevelField(1349286194);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_MORPH_UNITS_AIR = ConvertAbilityStringLevelField(1349286195);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_MORPH_UNITS_AMPHIBIOUS = ConvertAbilityStringLevelField(1349286196);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_MORPH_UNITS_WATER = ConvertAbilityStringLevelField(1349286197);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_UNIT_TYPE_ONE = ConvertAbilityStringLevelField(1382115635);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_UNIT_TYPE_TWO = ConvertAbilityStringLevelField(1382115636);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_UNIT_TYPE_SOD2 = ConvertAbilityStringLevelField(1399809074);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_SUMMON_1_UNIT_TYPE = ConvertAbilityStringLevelField(1232303153);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_SUMMON_2_UNIT_TYPE = ConvertAbilityStringLevelField(1232303154);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_RACE_TO_CONVERT = ConvertAbilityStringLevelField(1315201841);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_PARTNER_UNIT_TYPE = ConvertAbilityStringLevelField(1668243761);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_PARTNER_UNIT_TYPE_ONE = ConvertAbilityStringLevelField(1684238385);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_PARTNER_UNIT_TYPE_TWO = ConvertAbilityStringLevelField(1684238386);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_REQUIRED_UNIT_TYPE = ConvertAbilityStringLevelField(1953524017);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_CONVERTED_UNIT_TYPE = ConvertAbilityStringLevelField(1953524018);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_SPELL_LIST = ConvertAbilityStringLevelField(1936745009);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_BASE_ORDER_ID_SPB5 = ConvertAbilityStringLevelField(1936745013);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_BASE_ORDER_ID_NCL6 = ConvertAbilityStringLevelField(1315138614);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_ABILITY_UPGRADE_1 = ConvertAbilityStringLevelField(1315268403);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_ABILITY_UPGRADE_2 = ConvertAbilityStringLevelField(1315268404);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_ABILITY_UPGRADE_3 = ConvertAbilityStringLevelField(1315268405);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_ABILITY_UPGRADE_4 = ConvertAbilityStringLevelField(1315268406);
    [NativeLuaMemberAttribute]
    public static readonly abilitystringlevelfield ABILITY_SLF_SPAWN_UNIT_ID_NSY2 = ConvertAbilityStringLevelField(1316190514);
    [NativeLuaMemberAttribute]
    public static readonly itemintegerfield ITEM_IF_LEVEL = ConvertItemIntegerField(1768711542);
    [NativeLuaMemberAttribute]
    public static readonly itemintegerfield ITEM_IF_NUMBER_OF_CHARGES = ConvertItemIntegerField(1769304933);
    [NativeLuaMemberAttribute]
    public static readonly itemintegerfield ITEM_IF_COOLDOWN_GROUP = ConvertItemIntegerField(1768122724);
    [NativeLuaMemberAttribute]
    public static readonly itemintegerfield ITEM_IF_MAX_HIT_POINTS = ConvertItemIntegerField(1768453232);
    [NativeLuaMemberAttribute]
    public static readonly itemintegerfield ITEM_IF_HIT_POINTS = ConvertItemIntegerField(1768452195);
    [NativeLuaMemberAttribute]
    public static readonly itemintegerfield ITEM_IF_PRIORITY = ConvertItemIntegerField(1768977001);
    [NativeLuaMemberAttribute]
    public static readonly itemintegerfield ITEM_IF_ARMOR_TYPE = ConvertItemIntegerField(1767993965);
    [NativeLuaMemberAttribute]
    public static readonly itemintegerfield ITEM_IF_TINTING_COLOR_RED = ConvertItemIntegerField(1768123506);
    [NativeLuaMemberAttribute]
    public static readonly itemintegerfield ITEM_IF_TINTING_COLOR_GREEN = ConvertItemIntegerField(1768123495);
    [NativeLuaMemberAttribute]
    public static readonly itemintegerfield ITEM_IF_TINTING_COLOR_BLUE = ConvertItemIntegerField(1768123490);
    [NativeLuaMemberAttribute]
    public static readonly itemintegerfield ITEM_IF_TINTING_COLOR_ALPHA = ConvertItemIntegerField(1768120684);
    [NativeLuaMemberAttribute]
    public static readonly itemrealfield ITEM_RF_SCALING_VALUE = ConvertItemRealField(1769169761);
    [NativeLuaMemberAttribute]
    public static readonly itembooleanfield ITEM_BF_DROPPED_WHEN_CARRIER_DIES = ConvertItemBooleanField(1768190576);
    [NativeLuaMemberAttribute]
    public static readonly itembooleanfield ITEM_BF_CAN_BE_DROPPED = ConvertItemBooleanField(1768190575);
    [NativeLuaMemberAttribute]
    public static readonly itembooleanfield ITEM_BF_PERISHABLE = ConvertItemBooleanField(1768973682);
    [NativeLuaMemberAttribute]
    public static readonly itembooleanfield ITEM_BF_INCLUDE_AS_RANDOM_CHOICE = ConvertItemBooleanField(1768977006);
    [NativeLuaMemberAttribute]
    public static readonly itembooleanfield ITEM_BF_USE_AUTOMATICALLY_WHEN_ACQUIRED = ConvertItemBooleanField(1768976247);
    [NativeLuaMemberAttribute]
    public static readonly itembooleanfield ITEM_BF_CAN_BE_SOLD_TO_MERCHANTS = ConvertItemBooleanField(1768972663);
    [NativeLuaMemberAttribute]
    public static readonly itembooleanfield ITEM_BF_ACTIVELY_USED = ConvertItemBooleanField(1769304929);
    [NativeLuaMemberAttribute]
    public static readonly itemstringfield ITEM_SF_MODEL_USED = ConvertItemStringField(1768319340);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_DEFENSE_TYPE = ConvertUnitIntegerField(1969517689);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_ARMOR_TYPE = ConvertUnitIntegerField(1969320557);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_LOOPING_FADE_IN_RATE = ConvertUnitIntegerField(1970038377);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_LOOPING_FADE_OUT_RATE = ConvertUnitIntegerField(1970038383);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_AGILITY = ConvertUnitIntegerField(1969317731);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_INTELLIGENCE = ConvertUnitIntegerField(1969843811);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_STRENGTH = ConvertUnitIntegerField(1970500707);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_AGILITY_PERMANENT = ConvertUnitIntegerField(1969317741);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_INTELLIGENCE_PERMANENT = ConvertUnitIntegerField(1969843821);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_STRENGTH_PERMANENT = ConvertUnitIntegerField(1970500717);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_AGILITY_WITH_BONUS = ConvertUnitIntegerField(1969317730);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_INTELLIGENCE_WITH_BONUS = ConvertUnitIntegerField(1969843810);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_STRENGTH_WITH_BONUS = ConvertUnitIntegerField(1970500706);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_GOLD_BOUNTY_AWARDED_NUMBER_OF_DICE = ConvertUnitIntegerField(1969382505);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_GOLD_BOUNTY_AWARDED_BASE = ConvertUnitIntegerField(1969381985);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_GOLD_BOUNTY_AWARDED_SIDES_PER_DIE = ConvertUnitIntegerField(1969386345);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_LUMBER_BOUNTY_AWARDED_NUMBER_OF_DICE = ConvertUnitIntegerField(1970037348);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_LUMBER_BOUNTY_AWARDED_BASE = ConvertUnitIntegerField(1970037345);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_LUMBER_BOUNTY_AWARDED_SIDES_PER_DIE = ConvertUnitIntegerField(1970037363);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_LEVEL = ConvertUnitIntegerField(1970038134);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_FORMATION_RANK = ConvertUnitIntegerField(1969647474);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_ORIENTATION_INTERPOLATION = ConvertUnitIntegerField(1970238057);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_ELEVATION_SAMPLE_POINTS = ConvertUnitIntegerField(1969582196);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_TINTING_COLOR_RED = ConvertUnitIntegerField(1969450098);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_TINTING_COLOR_GREEN = ConvertUnitIntegerField(1969450087);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_TINTING_COLOR_BLUE = ConvertUnitIntegerField(1969450082);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_TINTING_COLOR_ALPHA = ConvertUnitIntegerField(1969447276);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_MOVE_TYPE = ConvertUnitIntegerField(1970108020);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_TARGETED_AS = ConvertUnitIntegerField(1970561394);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_UNIT_CLASSIFICATION = ConvertUnitIntegerField(1970567536);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_HIT_POINTS_REGENERATION_TYPE = ConvertUnitIntegerField(1969779316);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_PLACEMENT_PREVENTED_BY = ConvertUnitIntegerField(1970299250);
    [NativeLuaMemberAttribute]
    public static readonly unitintegerfield UNIT_IF_PRIMARY_ATTRIBUTE = ConvertUnitIntegerField(1970303585);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_STRENGTH_PER_LEVEL = ConvertUnitRealField(1970500720);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_AGILITY_PER_LEVEL = ConvertUnitRealField(1969317744);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_INTELLIGENCE_PER_LEVEL = ConvertUnitRealField(1969843824);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_HIT_POINTS_REGENERATION_RATE = ConvertUnitRealField(1969778802);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_MANA_REGENERATION = ConvertUnitRealField(1970106482);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_DEATH_TIME = ConvertUnitRealField(1969517677);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_FLY_HEIGHT = ConvertUnitRealField(1969650024);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_TURN_RATE = ConvertUnitRealField(1970108018);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_ELEVATION_SAMPLE_RADIUS = ConvertUnitRealField(1969582692);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_FOG_OF_WAR_SAMPLE_RADIUS = ConvertUnitRealField(1969648228);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_MAXIMUM_PITCH_ANGLE_DEGREES = ConvertUnitRealField(1970108528);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_MAXIMUM_ROLL_ANGLE_DEGREES = ConvertUnitRealField(1970108530);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_SCALING_VALUE = ConvertUnitRealField(1970496353);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_ANIMATION_RUN_SPEED = ConvertUnitRealField(1970435438);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_SELECTION_SCALE = ConvertUnitRealField(1970500451);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_SELECTION_CIRCLE_HEIGHT = ConvertUnitRealField(1970498682);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_SHADOW_IMAGE_HEIGHT = ConvertUnitRealField(1970497640);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_SHADOW_IMAGE_WIDTH = ConvertUnitRealField(1970497655);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_SHADOW_IMAGE_CENTER_X = ConvertUnitRealField(1970497656);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_SHADOW_IMAGE_CENTER_Y = ConvertUnitRealField(1970497657);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_ANIMATION_WALK_SPEED = ConvertUnitRealField(1970757996);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_DEFENSE = ConvertUnitRealField(1969514083);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_SIGHT_RADIUS = ConvertUnitRealField(1970497906);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_PRIORITY = ConvertUnitRealField(1970303593);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_SPEED = ConvertUnitRealField(1970108003);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_OCCLUDER_HEIGHT = ConvertUnitRealField(1970234211);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_HP = ConvertUnitRealField(1969778787);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_MANA = ConvertUnitRealField(1970106467);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_ACQUISITION_RANGE = ConvertUnitRealField(1969316721);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_CAST_BACK_SWING = ConvertUnitRealField(1969447539);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_CAST_POINT = ConvertUnitRealField(1969451124);
    [NativeLuaMemberAttribute]
    public static readonly unitrealfield UNIT_RF_MINIMUM_ATTACK_RANGE = ConvertUnitRealField(1969319278);
    [NativeLuaMemberAttribute]
    public static readonly unitbooleanfield UNIT_BF_RAISABLE = ConvertUnitBooleanField(1970430313);
    [NativeLuaMemberAttribute]
    public static readonly unitbooleanfield UNIT_BF_DECAYABLE = ConvertUnitBooleanField(1969513827);
    [NativeLuaMemberAttribute]
    public static readonly unitbooleanfield UNIT_BF_IS_A_BUILDING = ConvertUnitBooleanField(1969382503);
    [NativeLuaMemberAttribute]
    public static readonly unitbooleanfield UNIT_BF_USE_EXTENDED_LINE_OF_SIGHT = ConvertUnitBooleanField(1970040691);
    [NativeLuaMemberAttribute]
    public static readonly unitbooleanfield UNIT_BF_NEUTRAL_BUILDING_SHOWS_MINIMAP_ICON = ConvertUnitBooleanField(1970168429);
    [NativeLuaMemberAttribute]
    public static readonly unitbooleanfield UNIT_BF_HERO_HIDE_HERO_INTERFACE_ICON = ConvertUnitBooleanField(1969776738);
    [NativeLuaMemberAttribute]
    public static readonly unitbooleanfield UNIT_BF_HERO_HIDE_HERO_MINIMAP_DISPLAY = ConvertUnitBooleanField(1969776749);
    [NativeLuaMemberAttribute]
    public static readonly unitbooleanfield UNIT_BF_HERO_HIDE_HERO_DEATH_MESSAGE = ConvertUnitBooleanField(1969776740);
    [NativeLuaMemberAttribute]
    public static readonly unitbooleanfield UNIT_BF_HIDE_MINIMAP_DISPLAY = ConvertUnitBooleanField(1969778541);
    [NativeLuaMemberAttribute]
    public static readonly unitbooleanfield UNIT_BF_SCALE_PROJECTILES = ConvertUnitBooleanField(1970496354);
    [NativeLuaMemberAttribute]
    public static readonly unitbooleanfield UNIT_BF_SELECTION_CIRCLE_ON_WATER = ConvertUnitBooleanField(1970496887);
    [NativeLuaMemberAttribute]
    public static readonly unitbooleanfield UNIT_BF_HAS_WATER_SHADOW = ConvertUnitBooleanField(1970497650);
    [NativeLuaMemberAttribute]
    public static readonly unitstringfield UNIT_SF_NAME = ConvertUnitStringField(1970168173);
    [NativeLuaMemberAttribute]
    public static readonly unitstringfield UNIT_SF_PROPER_NAMES = ConvertUnitStringField(1970303599);
    [NativeLuaMemberAttribute]
    public static readonly unitstringfield UNIT_SF_GROUND_TEXTURE = ConvertUnitStringField(1970627187);
    [NativeLuaMemberAttribute]
    public static readonly unitstringfield UNIT_SF_SHADOW_IMAGE_UNIT = ConvertUnitStringField(1970497653);
    [NativeLuaMemberAttribute]
    public static readonly unitweaponintegerfield UNIT_WEAPON_IF_ATTACK_DAMAGE_NUMBER_OF_DICE = ConvertUnitWeaponIntegerField(1969303908);
    [NativeLuaMemberAttribute]
    public static readonly unitweaponintegerfield UNIT_WEAPON_IF_ATTACK_DAMAGE_BASE = ConvertUnitWeaponIntegerField(1969303906);
    [NativeLuaMemberAttribute]
    public static readonly unitweaponintegerfield UNIT_WEAPON_IF_ATTACK_DAMAGE_SIDES_PER_DIE = ConvertUnitWeaponIntegerField(1969303923);
    [NativeLuaMemberAttribute]
    public static readonly unitweaponintegerfield UNIT_WEAPON_IF_ATTACK_MAXIMUM_NUMBER_OF_TARGETS = ConvertUnitWeaponIntegerField(1970561841);
    [NativeLuaMemberAttribute]
    public static readonly unitweaponintegerfield UNIT_WEAPON_IF_ATTACK_ATTACK_TYPE = ConvertUnitWeaponIntegerField(1969303924);
    [NativeLuaMemberAttribute]
    public static readonly unitweaponintegerfield UNIT_WEAPON_IF_ATTACK_WEAPON_SOUND = ConvertUnitWeaponIntegerField(1969451825);
    [NativeLuaMemberAttribute]
    public static readonly unitweaponintegerfield UNIT_WEAPON_IF_ATTACK_AREA_OF_EFFECT_TARGETS = ConvertUnitWeaponIntegerField(1969303920);
    [NativeLuaMemberAttribute]
    public static readonly unitweaponintegerfield UNIT_WEAPON_IF_ATTACK_TARGETS_ALLOWED = ConvertUnitWeaponIntegerField(1969303911);
    [NativeLuaMemberAttribute]
    public static readonly unitweaponrealfield UNIT_WEAPON_RF_ATTACK_BACKSWING_POINT = ConvertUnitWeaponRealField(1969386289);
    [NativeLuaMemberAttribute]
    public static readonly unitweaponrealfield UNIT_WEAPON_RF_ATTACK_DAMAGE_POINT = ConvertUnitWeaponRealField(1969516593);
    [NativeLuaMemberAttribute]
    public static readonly unitweaponrealfield UNIT_WEAPON_RF_ATTACK_BASE_COOLDOWN = ConvertUnitWeaponRealField(1969303907);
    [NativeLuaMemberAttribute]
    public static readonly unitweaponrealfield UNIT_WEAPON_RF_ATTACK_DAMAGE_LOSS_FACTOR = ConvertUnitWeaponRealField(1969515569);
    [NativeLuaMemberAttribute]
    public static readonly unitweaponrealfield UNIT_WEAPON_RF_ATTACK_DAMAGE_FACTOR_MEDIUM = ConvertUnitWeaponRealField(1969775665);
    [NativeLuaMemberAttribute]
    public static readonly unitweaponrealfield UNIT_WEAPON_RF_ATTACK_DAMAGE_FACTOR_SMALL = ConvertUnitWeaponRealField(1970365489);
    [NativeLuaMemberAttribute]
    public static readonly unitweaponrealfield UNIT_WEAPON_RF_ATTACK_DAMAGE_SPILL_DISTANCE = ConvertUnitWeaponRealField(1970496561);
    [NativeLuaMemberAttribute]
    public static readonly unitweaponrealfield UNIT_WEAPON_RF_ATTACK_DAMAGE_SPILL_RADIUS = ConvertUnitWeaponRealField(1970500145);
    [NativeLuaMemberAttribute]
    public static readonly unitweaponrealfield UNIT_WEAPON_RF_ATTACK_PROJECTILE_SPEED = ConvertUnitWeaponRealField(1969303930);
    [NativeLuaMemberAttribute]
    public static readonly unitweaponrealfield UNIT_WEAPON_RF_ATTACK_PROJECTILE_ARC = ConvertUnitWeaponRealField(1970102577);
    [NativeLuaMemberAttribute]
    public static readonly unitweaponrealfield UNIT_WEAPON_RF_ATTACK_AREA_OF_EFFECT_FULL_DAMAGE = ConvertUnitWeaponRealField(1969303910);
    [NativeLuaMemberAttribute]
    public static readonly unitweaponrealfield UNIT_WEAPON_RF_ATTACK_AREA_OF_EFFECT_MEDIUM_DAMAGE = ConvertUnitWeaponRealField(1969303912);
    [NativeLuaMemberAttribute]
    public static readonly unitweaponrealfield UNIT_WEAPON_RF_ATTACK_AREA_OF_EFFECT_SMALL_DAMAGE = ConvertUnitWeaponRealField(1969303921);
    [NativeLuaMemberAttribute]
    public static readonly unitweaponrealfield UNIT_WEAPON_RF_ATTACK_RANGE = ConvertUnitWeaponRealField(1969303922);
    [NativeLuaMemberAttribute]
    public static readonly unitweaponbooleanfield UNIT_WEAPON_BF_ATTACK_SHOW_UI = ConvertUnitWeaponBooleanField(1970763057);
    [NativeLuaMemberAttribute]
    public static readonly unitweaponbooleanfield UNIT_WEAPON_BF_ATTACKS_ENABLED = ConvertUnitWeaponBooleanField(1969317230);
    [NativeLuaMemberAttribute]
    public static readonly unitweaponbooleanfield UNIT_WEAPON_BF_ATTACK_PROJECTILE_HOMING_ENABLED = ConvertUnitWeaponBooleanField(1970104369);
    [NativeLuaMemberAttribute]
    public static readonly unitweaponstringfield UNIT_WEAPON_SF_ATTACK_PROJECTILE_ART = ConvertUnitWeaponStringField(1969303917);
    [NativeLuaMemberAttribute]
    public static readonly movetype MOVE_TYPE_UNKNOWN = ConvertMoveType(0);
    [NativeLuaMemberAttribute]
    public static readonly movetype MOVE_TYPE_FOOT = ConvertMoveType(1);
    [NativeLuaMemberAttribute]
    public static readonly movetype MOVE_TYPE_FLY = ConvertMoveType(2);
    [NativeLuaMemberAttribute]
    public static readonly movetype MOVE_TYPE_HORSE = ConvertMoveType(4);
    [NativeLuaMemberAttribute]
    public static readonly movetype MOVE_TYPE_HOVER = ConvertMoveType(8);
    [NativeLuaMemberAttribute]
    public static readonly movetype MOVE_TYPE_FLOAT = ConvertMoveType(16);
    [NativeLuaMemberAttribute]
    public static readonly movetype MOVE_TYPE_AMPHIBIOUS = ConvertMoveType(32);
    [NativeLuaMemberAttribute]
    public static readonly movetype MOVE_TYPE_UNBUILDABLE = ConvertMoveType(64);
    [NativeLuaMemberAttribute]
    public static readonly targetflag TARGET_FLAG_NONE = ConvertTargetFlag(1);
    [NativeLuaMemberAttribute]
    public static readonly targetflag TARGET_FLAG_GROUND = ConvertTargetFlag(2);
    [NativeLuaMemberAttribute]
    public static readonly targetflag TARGET_FLAG_AIR = ConvertTargetFlag(4);
    [NativeLuaMemberAttribute]
    public static readonly targetflag TARGET_FLAG_STRUCTURE = ConvertTargetFlag(8);
    [NativeLuaMemberAttribute]
    public static readonly targetflag TARGET_FLAG_WARD = ConvertTargetFlag(16);
    [NativeLuaMemberAttribute]
    public static readonly targetflag TARGET_FLAG_ITEM = ConvertTargetFlag(32);
    [NativeLuaMemberAttribute]
    public static readonly targetflag TARGET_FLAG_TREE = ConvertTargetFlag(64);
    [NativeLuaMemberAttribute]
    public static readonly targetflag TARGET_FLAG_WALL = ConvertTargetFlag(128);
    [NativeLuaMemberAttribute]
    public static readonly targetflag TARGET_FLAG_DEBRIS = ConvertTargetFlag(256);
    [NativeLuaMemberAttribute]
    public static readonly targetflag TARGET_FLAG_DECORATION = ConvertTargetFlag(512);
    [NativeLuaMemberAttribute]
    public static readonly targetflag TARGET_FLAG_BRIDGE = ConvertTargetFlag(1024);
    [NativeLuaMemberAttribute]
    public static readonly defensetype DEFENSE_TYPE_LIGHT = ConvertDefenseType(0);
    [NativeLuaMemberAttribute]
    public static readonly defensetype DEFENSE_TYPE_MEDIUM = ConvertDefenseType(1);
    [NativeLuaMemberAttribute]
    public static readonly defensetype DEFENSE_TYPE_LARGE = ConvertDefenseType(2);
    [NativeLuaMemberAttribute]
    public static readonly defensetype DEFENSE_TYPE_FORT = ConvertDefenseType(3);
    [NativeLuaMemberAttribute]
    public static readonly defensetype DEFENSE_TYPE_NORMAL = ConvertDefenseType(4);
    [NativeLuaMemberAttribute]
    public static readonly defensetype DEFENSE_TYPE_HERO = ConvertDefenseType(5);
    [NativeLuaMemberAttribute]
    public static readonly defensetype DEFENSE_TYPE_DIVINE = ConvertDefenseType(6);
    [NativeLuaMemberAttribute]
    public static readonly defensetype DEFENSE_TYPE_NONE = ConvertDefenseType(7);
    [NativeLuaMemberAttribute]
    public static readonly heroattribute HERO_ATTRIBUTE_STR = ConvertHeroAttribute(1);
    [NativeLuaMemberAttribute]
    public static readonly heroattribute HERO_ATTRIBUTE_INT = ConvertHeroAttribute(2);
    [NativeLuaMemberAttribute]
    public static readonly heroattribute HERO_ATTRIBUTE_AGI = ConvertHeroAttribute(3);
    [NativeLuaMemberAttribute]
    public static readonly armortype ARMOR_TYPE_WHOKNOWS = ConvertArmorType(0);
    [NativeLuaMemberAttribute]
    public static readonly armortype ARMOR_TYPE_FLESH = ConvertArmorType(1);
    [NativeLuaMemberAttribute]
    public static readonly armortype ARMOR_TYPE_METAL = ConvertArmorType(2);
    [NativeLuaMemberAttribute]
    public static readonly armortype ARMOR_TYPE_WOOD = ConvertArmorType(3);
    [NativeLuaMemberAttribute]
    public static readonly armortype ARMOR_TYPE_ETHREAL = ConvertArmorType(4);
    [NativeLuaMemberAttribute]
    public static readonly armortype ARMOR_TYPE_STONE = ConvertArmorType(5);
    [NativeLuaMemberAttribute]
    public static readonly regentype REGENERATION_TYPE_NONE = ConvertRegenType(0);
    [NativeLuaMemberAttribute]
    public static readonly regentype REGENERATION_TYPE_ALWAYS = ConvertRegenType(1);
    [NativeLuaMemberAttribute]
    public static readonly regentype REGENERATION_TYPE_BLIGHT = ConvertRegenType(2);
    [NativeLuaMemberAttribute]
    public static readonly regentype REGENERATION_TYPE_DAY = ConvertRegenType(3);
    [NativeLuaMemberAttribute]
    public static readonly regentype REGENERATION_TYPE_NIGHT = ConvertRegenType(4);
    [NativeLuaMemberAttribute]
    public static readonly unitcategory UNIT_CATEGORY_GIANT = ConvertUnitCategory(1);
    [NativeLuaMemberAttribute]
    public static readonly unitcategory UNIT_CATEGORY_UNDEAD = ConvertUnitCategory(2);
    [NativeLuaMemberAttribute]
    public static readonly unitcategory UNIT_CATEGORY_SUMMONED = ConvertUnitCategory(4);
    [NativeLuaMemberAttribute]
    public static readonly unitcategory UNIT_CATEGORY_MECHANICAL = ConvertUnitCategory(8);
    [NativeLuaMemberAttribute]
    public static readonly unitcategory UNIT_CATEGORY_PEON = ConvertUnitCategory(16);
    [NativeLuaMemberAttribute]
    public static readonly unitcategory UNIT_CATEGORY_SAPPER = ConvertUnitCategory(32);
    [NativeLuaMemberAttribute]
    public static readonly unitcategory UNIT_CATEGORY_TOWNHALL = ConvertUnitCategory(64);
    [NativeLuaMemberAttribute]
    public static readonly unitcategory UNIT_CATEGORY_ANCIENT = ConvertUnitCategory(128);
    [NativeLuaMemberAttribute]
    public static readonly unitcategory UNIT_CATEGORY_NEUTRAL = ConvertUnitCategory(256);
    [NativeLuaMemberAttribute]
    public static readonly unitcategory UNIT_CATEGORY_WARD = ConvertUnitCategory(512);
    [NativeLuaMemberAttribute]
    public static readonly unitcategory UNIT_CATEGORY_STANDON = ConvertUnitCategory(1024);
    [NativeLuaMemberAttribute]
    public static readonly unitcategory UNIT_CATEGORY_TAUREN = ConvertUnitCategory(2048);
    [NativeLuaMemberAttribute]
    public static readonly pathingflag PATHING_FLAG_UNWALKABLE = ConvertPathingFlag(2);
    [NativeLuaMemberAttribute]
    public static readonly pathingflag PATHING_FLAG_UNFLYABLE = ConvertPathingFlag(4);
    [NativeLuaMemberAttribute]
    public static readonly pathingflag PATHING_FLAG_UNBUILDABLE = ConvertPathingFlag(8);
    [NativeLuaMemberAttribute]
    public static readonly pathingflag PATHING_FLAG_UNPEONHARVEST = ConvertPathingFlag(16);
    [NativeLuaMemberAttribute]
    public static readonly pathingflag PATHING_FLAG_BLIGHTED = ConvertPathingFlag(32);
    [NativeLuaMemberAttribute]
    public static readonly pathingflag PATHING_FLAG_UNFLOATABLE = ConvertPathingFlag(64);
    [NativeLuaMemberAttribute]
    public static readonly pathingflag PATHING_FLAG_UNAMPHIBIOUS = ConvertPathingFlag(128);
    [NativeLuaMemberAttribute]
    public static readonly pathingflag PATHING_FLAG_UNITEMPLACABLE = ConvertPathingFlag(256);
    [NativeLuaMemberAttribute]
    public static extern float Deg2Rad(float degrees);
    [NativeLuaMemberAttribute]
    public static extern float Rad2Deg(float radians);
    [NativeLuaMemberAttribute]
    public static extern float Sin(float radians);
    [NativeLuaMemberAttribute]
    public static extern float Cos(float radians);
    [NativeLuaMemberAttribute]
    public static extern float Tan(float radians);
    [NativeLuaMemberAttribute]
    public static extern float Asin(float y);
    [NativeLuaMemberAttribute]
    public static extern float Acos(float x);
    [NativeLuaMemberAttribute]
    public static extern float Atan(float x);
    [NativeLuaMemberAttribute]
    public static extern float Atan2(float y, float x);
    [NativeLuaMemberAttribute]
    public static extern float SquareRoot(float x);
    [NativeLuaMemberAttribute]
    public static extern float Pow(float x, float power);
    [NativeLuaMemberAttribute]
    public static extern float I2R(int i);
    [NativeLuaMemberAttribute]
    public static extern int R2I(float r);
    [NativeLuaMemberAttribute]
    public static extern string I2S(int i);
    [NativeLuaMemberAttribute]
    public static extern string R2S(float r);
    [NativeLuaMemberAttribute]
    public static extern string R2SW(float r, int width, int precision);
    [NativeLuaMemberAttribute]
    public static extern int S2I(string s);
    [NativeLuaMemberAttribute]
    public static extern float S2R(string s);
    [NativeLuaMemberAttribute]
    public static extern int GetHandleId(object h);
    [NativeLuaMemberAttribute]
    public static extern string SubString(string source, int start, int end);
    [NativeLuaMemberAttribute]
    public static extern int StringLength(string s);
    [NativeLuaMemberAttribute]
    public static extern string StringCase(string source, bool upper);
    [NativeLuaMemberAttribute]
    public static extern int StringHash(string s);
    [NativeLuaMemberAttribute]
    public static extern string GetLocalizedString(string source);
    [NativeLuaMemberAttribute]
    public static extern int GetLocalizedHotkey(string source);
    [NativeLuaMemberAttribute]
    public static extern void SetMapName(string name);
    [NativeLuaMemberAttribute]
    public static extern void SetMapDescription(string description);
    [NativeLuaMemberAttribute]
    public static extern void SetTeams(int teamcount);
    [NativeLuaMemberAttribute]
    public static extern void SetPlayers(int playercount);
    [NativeLuaMemberAttribute]
    public static extern void DefineStartLocation(int whichStartLoc, float x, float y);
    [NativeLuaMemberAttribute]
    public static extern void DefineStartLocationLoc(int whichStartLoc, location whichLocation);
    [NativeLuaMemberAttribute]
    public static extern void SetStartLocPrioCount(int whichStartLoc, int prioSlotCount);
    [NativeLuaMemberAttribute]
    public static extern void SetStartLocPrio(int whichStartLoc, int prioSlotIndex, int otherStartLocIndex, startlocprio priority);
    [NativeLuaMemberAttribute]
    public static extern int GetStartLocPrioSlot(int whichStartLoc, int prioSlotIndex);
    [NativeLuaMemberAttribute]
    public static extern startlocprio GetStartLocPrio(int whichStartLoc, int prioSlotIndex);
    [NativeLuaMemberAttribute]
    public static extern void SetGameTypeSupported(gametype whichGameType, bool value);
    [NativeLuaMemberAttribute]
    public static extern void SetMapFlag(mapflag whichMapFlag, bool value);
    [NativeLuaMemberAttribute]
    public static extern void SetGamePlacement(placement whichPlacementType);
    [NativeLuaMemberAttribute]
    public static extern void SetGameSpeed(gamespeed whichspeed);
    [NativeLuaMemberAttribute]
    public static extern void SetGameDifficulty(gamedifficulty whichdifficulty);
    [NativeLuaMemberAttribute]
    public static extern void SetResourceDensity(mapdensity whichdensity);
    [NativeLuaMemberAttribute]
    public static extern void SetCreatureDensity(mapdensity whichdensity);
    [NativeLuaMemberAttribute]
    public static extern int GetTeams();
    [NativeLuaMemberAttribute]
    public static extern int GetPlayers();
    [NativeLuaMemberAttribute]
    public static extern bool IsGameTypeSupported(gametype whichGameType);
    [NativeLuaMemberAttribute]
    public static extern gametype GetGameTypeSelected();
    [NativeLuaMemberAttribute]
    public static extern bool IsMapFlagSet(mapflag whichMapFlag);
    [NativeLuaMemberAttribute]
    public static extern placement GetGamePlacement();
    [NativeLuaMemberAttribute]
    public static extern gamespeed GetGameSpeed();
    [NativeLuaMemberAttribute]
    public static extern gamedifficulty GetGameDifficulty();
    [NativeLuaMemberAttribute]
    public static extern mapdensity GetResourceDensity();
    [NativeLuaMemberAttribute]
    public static extern mapdensity GetCreatureDensity();
    [NativeLuaMemberAttribute]
    public static extern float GetStartLocationX(int whichStartLocation);
    [NativeLuaMemberAttribute]
    public static extern float GetStartLocationY(int whichStartLocation);
    [NativeLuaMemberAttribute]
    public static extern location GetStartLocationLoc(int whichStartLocation);
    [NativeLuaMemberAttribute]
    public static extern void SetPlayerTeam(player whichPlayer, int whichTeam);
    [NativeLuaMemberAttribute]
    public static extern void SetPlayerStartLocation(player whichPlayer, int startLocIndex);
    [NativeLuaMemberAttribute]
    public static extern void ForcePlayerStartLocation(player whichPlayer, int startLocIndex);
    [NativeLuaMemberAttribute]
    public static extern void SetPlayerColor(player whichPlayer, playercolor color);
    [NativeLuaMemberAttribute]
    public static extern void SetPlayerAlliance(player sourcePlayer, player otherPlayer, alliancetype whichAllianceSetting, bool value);
    [NativeLuaMemberAttribute]
    public static extern void SetPlayerTaxRate(player sourcePlayer, player otherPlayer, playerstate whichResource, int rate);
    [NativeLuaMemberAttribute]
    public static extern void SetPlayerRacePreference(player whichPlayer, racepreference whichRacePreference);
    [NativeLuaMemberAttribute]
    public static extern void SetPlayerRaceSelectable(player whichPlayer, bool value);
    [NativeLuaMemberAttribute]
    public static extern void SetPlayerController(player whichPlayer, mapcontrol controlType);
    [NativeLuaMemberAttribute]
    public static extern void SetPlayerName(player whichPlayer, string name);
    [NativeLuaMemberAttribute]
    public static extern void SetPlayerOnScoreScreen(player whichPlayer, bool flag);
    [NativeLuaMemberAttribute]
    public static extern int GetPlayerTeam(player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern int GetPlayerStartLocation(player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern playercolor GetPlayerColor(player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern bool GetPlayerSelectable(player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern mapcontrol GetPlayerController(player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern playerslotstate GetPlayerSlotState(player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern int GetPlayerTaxRate(player sourcePlayer, player otherPlayer, playerstate whichResource);
    [NativeLuaMemberAttribute]
    public static extern bool IsPlayerRacePrefSet(player whichPlayer, racepreference pref);
    [NativeLuaMemberAttribute]
    public static extern string GetPlayerName(player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern timer CreateTimer();
    [NativeLuaMemberAttribute]
    public static extern void DestroyTimer(timer whichTimer);
    [NativeLuaMemberAttribute]
    public static extern void TimerStart(timer whichTimer, float timeout, bool periodic, System.Action handlerFunc);
    [NativeLuaMemberAttribute]
    public static extern float TimerGetElapsed(timer whichTimer);
    [NativeLuaMemberAttribute]
    public static extern float TimerGetRemaining(timer whichTimer);
    [NativeLuaMemberAttribute]
    public static extern float TimerGetTimeout(timer whichTimer);
    [NativeLuaMemberAttribute]
    public static extern void PauseTimer(timer whichTimer);
    [NativeLuaMemberAttribute]
    public static extern void ResumeTimer(timer whichTimer);
    [NativeLuaMemberAttribute]
    public static extern timer GetExpiredTimer();
    [NativeLuaMemberAttribute]
    public static extern group CreateGroup();
    [NativeLuaMemberAttribute]
    public static extern void DestroyGroup(group whichGroup);
    [NativeLuaMemberAttribute]
    public static extern bool GroupAddUnit(group whichGroup, unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern bool GroupRemoveUnit(group whichGroup, unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern int BlzGroupAddGroupFast(group whichGroup, group addGroup);
    [NativeLuaMemberAttribute]
    public static extern int BlzGroupRemoveGroupFast(group whichGroup, group removeGroup);
    [NativeLuaMemberAttribute]
    public static extern void GroupClear(group whichGroup);
    [NativeLuaMemberAttribute]
    public static extern int BlzGroupGetSize(group whichGroup);
    [NativeLuaMemberAttribute]
    public static extern unit BlzGroupUnitAt(group whichGroup, int index);
    [NativeLuaMemberAttribute]
    public static extern void GroupEnumUnitsOfType(group whichGroup, string unitname, boolexpr filter);
    [NativeLuaMemberAttribute]
    public static extern void GroupEnumUnitsOfPlayer(group whichGroup, player whichPlayer, boolexpr filter);
    [NativeLuaMemberAttribute]
    public static extern void GroupEnumUnitsOfTypeCounted(group whichGroup, string unitname, boolexpr filter, int countLimit);
    [NativeLuaMemberAttribute]
    public static extern void GroupEnumUnitsInRect(group whichGroup, rect r, boolexpr filter);
    [NativeLuaMemberAttribute]
    public static extern void GroupEnumUnitsInRectCounted(group whichGroup, rect r, boolexpr filter, int countLimit);
    [NativeLuaMemberAttribute]
    public static extern void GroupEnumUnitsInRange(group whichGroup, float x, float y, float radius, boolexpr filter);
    [NativeLuaMemberAttribute]
    public static extern void GroupEnumUnitsInRangeOfLoc(group whichGroup, location whichLocation, float radius, boolexpr filter);
    [NativeLuaMemberAttribute]
    public static extern void GroupEnumUnitsInRangeCounted(group whichGroup, float x, float y, float radius, boolexpr filter, int countLimit);
    [NativeLuaMemberAttribute]
    public static extern void GroupEnumUnitsInRangeOfLocCounted(group whichGroup, location whichLocation, float radius, boolexpr filter, int countLimit);
    [NativeLuaMemberAttribute]
    public static extern void GroupEnumUnitsSelected(group whichGroup, player whichPlayer, boolexpr filter);
    [NativeLuaMemberAttribute]
    public static extern bool GroupImmediateOrder(group whichGroup, string order);
    [NativeLuaMemberAttribute]
    public static extern bool GroupImmediateOrderById(group whichGroup, int order);
    [NativeLuaMemberAttribute]
    public static extern bool GroupPointOrder(group whichGroup, string order, float x, float y);
    [NativeLuaMemberAttribute]
    public static extern bool GroupPointOrderLoc(group whichGroup, string order, location whichLocation);
    [NativeLuaMemberAttribute]
    public static extern bool GroupPointOrderById(group whichGroup, int order, float x, float y);
    [NativeLuaMemberAttribute]
    public static extern bool GroupPointOrderByIdLoc(group whichGroup, int order, location whichLocation);
    [NativeLuaMemberAttribute]
    public static extern bool GroupTargetOrder(group whichGroup, string order, widget targetWidget);
    [NativeLuaMemberAttribute]
    public static extern bool GroupTargetOrderById(group whichGroup, int order, widget targetWidget);
    [NativeLuaMemberAttribute]
    public static extern void ForGroup(group whichGroup, System.Action callback);
    [NativeLuaMemberAttribute]
    public static extern unit FirstOfGroup(group whichGroup);
    [NativeLuaMemberAttribute]
    public static extern force CreateForce();
    [NativeLuaMemberAttribute]
    public static extern void DestroyForce(force whichForce);
    [NativeLuaMemberAttribute]
    public static extern void ForceAddPlayer(force whichForce, player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern void ForceRemovePlayer(force whichForce, player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern bool BlzForceHasPlayer(force whichForce, player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern void ForceClear(force whichForce);
    [NativeLuaMemberAttribute]
    public static extern void ForceEnumPlayers(force whichForce, boolexpr filter);
    [NativeLuaMemberAttribute]
    public static extern void ForceEnumPlayersCounted(force whichForce, boolexpr filter, int countLimit);
    [NativeLuaMemberAttribute]
    public static extern void ForceEnumAllies(force whichForce, player whichPlayer, boolexpr filter);
    [NativeLuaMemberAttribute]
    public static extern void ForceEnumEnemies(force whichForce, player whichPlayer, boolexpr filter);
    [NativeLuaMemberAttribute]
    public static extern void ForForce(force whichForce, System.Action callback);
    [NativeLuaMemberAttribute]
    public static extern rect Rect(float minx, float miny, float maxx, float maxy);
    [NativeLuaMemberAttribute]
    public static extern rect RectFromLoc(location min, location max);
    [NativeLuaMemberAttribute]
    public static extern void RemoveRect(rect whichRect);
    [NativeLuaMemberAttribute]
    public static extern void SetRect(rect whichRect, float minx, float miny, float maxx, float maxy);
    [NativeLuaMemberAttribute]
    public static extern void SetRectFromLoc(rect whichRect, location min, location max);
    [NativeLuaMemberAttribute]
    public static extern void MoveRectTo(rect whichRect, float newCenterX, float newCenterY);
    [NativeLuaMemberAttribute]
    public static extern void MoveRectToLoc(rect whichRect, location newCenterLoc);
    [NativeLuaMemberAttribute]
    public static extern float GetRectCenterX(rect whichRect);
    [NativeLuaMemberAttribute]
    public static extern float GetRectCenterY(rect whichRect);
    [NativeLuaMemberAttribute]
    public static extern float GetRectMinX(rect whichRect);
    [NativeLuaMemberAttribute]
    public static extern float GetRectMinY(rect whichRect);
    [NativeLuaMemberAttribute]
    public static extern float GetRectMaxX(rect whichRect);
    [NativeLuaMemberAttribute]
    public static extern float GetRectMaxY(rect whichRect);
    [NativeLuaMemberAttribute]
    public static extern region CreateRegion();
    [NativeLuaMemberAttribute]
    public static extern void RemoveRegion(region whichRegion);
    [NativeLuaMemberAttribute]
    public static extern void RegionAddRect(region whichRegion, rect r);
    [NativeLuaMemberAttribute]
    public static extern void RegionClearRect(region whichRegion, rect r);
    [NativeLuaMemberAttribute]
    public static extern void RegionAddCell(region whichRegion, float x, float y);
    [NativeLuaMemberAttribute]
    public static extern void RegionAddCellAtLoc(region whichRegion, location whichLocation);
    [NativeLuaMemberAttribute]
    public static extern void RegionClearCell(region whichRegion, float x, float y);
    [NativeLuaMemberAttribute]
    public static extern void RegionClearCellAtLoc(region whichRegion, location whichLocation);
    [NativeLuaMemberAttribute]
    public static extern location Location(float x, float y);
    [NativeLuaMemberAttribute]
    public static extern void RemoveLocation(location whichLocation);
    [NativeLuaMemberAttribute]
    public static extern void MoveLocation(location whichLocation, float newX, float newY);
    [NativeLuaMemberAttribute]
    public static extern float GetLocationX(location whichLocation);
    [NativeLuaMemberAttribute]
    public static extern float GetLocationY(location whichLocation);
    [NativeLuaMemberAttribute]
    public static extern float GetLocationZ(location whichLocation);
    [NativeLuaMemberAttribute]
    public static extern bool IsUnitInRegion(region whichRegion, unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern bool IsPointInRegion(region whichRegion, float x, float y);
    [NativeLuaMemberAttribute]
    public static extern bool IsLocationInRegion(region whichRegion, location whichLocation);
    [NativeLuaMemberAttribute]
    public static extern rect GetWorldBounds();
    [NativeLuaMemberAttribute]
    public static extern trigger CreateTrigger();
    [NativeLuaMemberAttribute]
    public static extern void DestroyTrigger(trigger whichTrigger);
    [NativeLuaMemberAttribute]
    public static extern void ResetTrigger(trigger whichTrigger);
    [NativeLuaMemberAttribute]
    public static extern void EnableTrigger(trigger whichTrigger);
    [NativeLuaMemberAttribute]
    public static extern void DisableTrigger(trigger whichTrigger);
    [NativeLuaMemberAttribute]
    public static extern bool IsTriggerEnabled(trigger whichTrigger);
    [NativeLuaMemberAttribute]
    public static extern void TriggerWaitOnSleeps(trigger whichTrigger, bool flag);
    [NativeLuaMemberAttribute]
    public static extern bool IsTriggerWaitOnSleeps(trigger whichTrigger);
    [NativeLuaMemberAttribute]
    public static extern unit GetFilterUnit();
    [NativeLuaMemberAttribute]
    public static extern unit GetEnumUnit();
    [NativeLuaMemberAttribute]
    public static extern destructable GetFilterDestructable();
    [NativeLuaMemberAttribute]
    public static extern destructable GetEnumDestructable();
    [NativeLuaMemberAttribute]
    public static extern item GetFilterItem();
    [NativeLuaMemberAttribute]
    public static extern item GetEnumItem();
    [NativeLuaMemberAttribute]
    public static extern player GetFilterPlayer();
    [NativeLuaMemberAttribute]
    public static extern player GetEnumPlayer();
    [NativeLuaMemberAttribute]
    public static extern trigger GetTriggeringTrigger();
    [NativeLuaMemberAttribute]
    public static extern eventid GetTriggerEventId();
    [NativeLuaMemberAttribute]
    public static extern int GetTriggerEvalCount(trigger whichTrigger);
    [NativeLuaMemberAttribute]
    public static extern int GetTriggerExecCount(trigger whichTrigger);
    [NativeLuaMemberAttribute]
    public static extern void ExecuteFunc(string funcName);
    [NativeLuaMemberAttribute]
    public static extern boolexpr And(boolexpr operandA, boolexpr operandB);
    [NativeLuaMemberAttribute]
    public static extern boolexpr Or(boolexpr operandA, boolexpr operandB);
    [NativeLuaMemberAttribute]
    public static extern boolexpr Not(boolexpr operand);
    [NativeLuaMemberAttribute]
    public static extern conditionfunc Condition(System.Func<bool> func);
    [NativeLuaMemberAttribute]
    public static extern void DestroyCondition(conditionfunc c);
    [NativeLuaMemberAttribute]
    public static extern filterfunc Filter(System.Func<bool> func);
    [NativeLuaMemberAttribute]
    public static extern void DestroyFilter(filterfunc f);
    [NativeLuaMemberAttribute]
    public static extern void DestroyBoolExpr(boolexpr e);
    [NativeLuaMemberAttribute]
    public static extern @event TriggerRegisterVariableEvent(trigger whichTrigger, string varName, limitop opcode, float limitval);
    [NativeLuaMemberAttribute]
    public static extern @event TriggerRegisterTimerEvent(trigger whichTrigger, float timeout, bool periodic);
    [NativeLuaMemberAttribute]
    public static extern @event TriggerRegisterTimerExpireEvent(trigger whichTrigger, timer t);
    [NativeLuaMemberAttribute]
    public static extern @event TriggerRegisterGameStateEvent(trigger whichTrigger, gamestate whichState, limitop opcode, float limitval);
    [NativeLuaMemberAttribute]
    public static extern @event TriggerRegisterDialogEvent(trigger whichTrigger, dialog whichDialog);
    [NativeLuaMemberAttribute]
    public static extern @event TriggerRegisterDialogButtonEvent(trigger whichTrigger, button whichButton);
    [NativeLuaMemberAttribute]
    public static extern gamestate GetEventGameState();
    [NativeLuaMemberAttribute]
    public static extern @event TriggerRegisterGameEvent(trigger whichTrigger, gameevent whichGameEvent);
    [NativeLuaMemberAttribute]
    public static extern player GetWinningPlayer();
    [NativeLuaMemberAttribute]
    public static extern @event TriggerRegisterEnterRegion(trigger whichTrigger, region whichRegion, boolexpr filter);
    [NativeLuaMemberAttribute]
    public static extern region GetTriggeringRegion();
    [NativeLuaMemberAttribute]
    public static extern unit GetEnteringUnit();
    [NativeLuaMemberAttribute]
    public static extern @event TriggerRegisterLeaveRegion(trigger whichTrigger, region whichRegion, boolexpr filter);
    [NativeLuaMemberAttribute]
    public static extern unit GetLeavingUnit();
    [NativeLuaMemberAttribute]
    public static extern @event TriggerRegisterTrackableHitEvent(trigger whichTrigger, trackable t);
    [NativeLuaMemberAttribute]
    public static extern @event TriggerRegisterTrackableTrackEvent(trigger whichTrigger, trackable t);
    [NativeLuaMemberAttribute]
    public static extern trackable GetTriggeringTrackable();
    [NativeLuaMemberAttribute]
    public static extern button GetClickedButton();
    [NativeLuaMemberAttribute]
    public static extern dialog GetClickedDialog();
    [NativeLuaMemberAttribute]
    public static extern float GetTournamentFinishSoonTimeRemaining();
    [NativeLuaMemberAttribute]
    public static extern int GetTournamentFinishNowRule();
    [NativeLuaMemberAttribute]
    public static extern player GetTournamentFinishNowPlayer();
    [NativeLuaMemberAttribute]
    public static extern int GetTournamentScore(player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern string GetSaveBasicFilename();
    [NativeLuaMemberAttribute]
    public static extern @event TriggerRegisterPlayerEvent(trigger whichTrigger, player whichPlayer, playerevent whichPlayerEvent);
    [NativeLuaMemberAttribute]
    public static extern player GetTriggerPlayer();
    [NativeLuaMemberAttribute]
    public static extern @event TriggerRegisterPlayerUnitEvent(trigger whichTrigger, player whichPlayer, playerunitevent whichPlayerUnitEvent, boolexpr filter);
    [NativeLuaMemberAttribute]
    public static extern unit GetLevelingUnit();
    [NativeLuaMemberAttribute]
    public static extern unit GetLearningUnit();
    [NativeLuaMemberAttribute]
    public static extern int GetLearnedSkill();
    [NativeLuaMemberAttribute]
    public static extern int GetLearnedSkillLevel();
    [NativeLuaMemberAttribute]
    public static extern unit GetRevivableUnit();
    [NativeLuaMemberAttribute]
    public static extern unit GetRevivingUnit();
    [NativeLuaMemberAttribute]
    public static extern unit GetAttacker();
    [NativeLuaMemberAttribute]
    public static extern unit GetRescuer();
    [NativeLuaMemberAttribute]
    public static extern unit GetDyingUnit();
    [NativeLuaMemberAttribute]
    public static extern unit GetKillingUnit();
    [NativeLuaMemberAttribute]
    public static extern unit GetDecayingUnit();
    [NativeLuaMemberAttribute]
    public static extern unit GetConstructingStructure();
    [NativeLuaMemberAttribute]
    public static extern unit GetCancelledStructure();
    [NativeLuaMemberAttribute]
    public static extern unit GetConstructedStructure();
    [NativeLuaMemberAttribute]
    public static extern unit GetResearchingUnit();
    [NativeLuaMemberAttribute]
    public static extern int GetResearched();
    [NativeLuaMemberAttribute]
    public static extern int GetTrainedUnitType();
    [NativeLuaMemberAttribute]
    public static extern unit GetTrainedUnit();
    [NativeLuaMemberAttribute]
    public static extern unit GetDetectedUnit();
    [NativeLuaMemberAttribute]
    public static extern unit GetSummoningUnit();
    [NativeLuaMemberAttribute]
    public static extern unit GetSummonedUnit();
    [NativeLuaMemberAttribute]
    public static extern unit GetTransportUnit();
    [NativeLuaMemberAttribute]
    public static extern unit GetLoadedUnit();
    [NativeLuaMemberAttribute]
    public static extern unit GetSellingUnit();
    [NativeLuaMemberAttribute]
    public static extern unit GetSoldUnit();
    [NativeLuaMemberAttribute]
    public static extern unit GetBuyingUnit();
    [NativeLuaMemberAttribute]
    public static extern item GetSoldItem();
    [NativeLuaMemberAttribute]
    public static extern unit GetChangingUnit();
    [NativeLuaMemberAttribute]
    public static extern player GetChangingUnitPrevOwner();
    [NativeLuaMemberAttribute]
    public static extern unit GetManipulatingUnit();
    [NativeLuaMemberAttribute]
    public static extern item GetManipulatedItem();
    [NativeLuaMemberAttribute]
    public static extern unit GetOrderedUnit();
    [NativeLuaMemberAttribute]
    public static extern int GetIssuedOrderId();
    [NativeLuaMemberAttribute]
    public static extern float GetOrderPointX();
    [NativeLuaMemberAttribute]
    public static extern float GetOrderPointY();
    [NativeLuaMemberAttribute]
    public static extern location GetOrderPointLoc();
    [NativeLuaMemberAttribute]
    public static extern widget GetOrderTarget();
    [NativeLuaMemberAttribute]
    public static extern destructable GetOrderTargetDestructable();
    [NativeLuaMemberAttribute]
    public static extern item GetOrderTargetItem();
    [NativeLuaMemberAttribute]
    public static extern unit GetOrderTargetUnit();
    [NativeLuaMemberAttribute]
    public static extern unit GetSpellAbilityUnit();
    [NativeLuaMemberAttribute]
    public static extern int GetSpellAbilityId();
    [NativeLuaMemberAttribute]
    public static extern ability GetSpellAbility();
    [NativeLuaMemberAttribute]
    public static extern location GetSpellTargetLoc();
    [NativeLuaMemberAttribute]
    public static extern float GetSpellTargetX();
    [NativeLuaMemberAttribute]
    public static extern float GetSpellTargetY();
    [NativeLuaMemberAttribute]
    public static extern destructable GetSpellTargetDestructable();
    [NativeLuaMemberAttribute]
    public static extern item GetSpellTargetItem();
    [NativeLuaMemberAttribute]
    public static extern unit GetSpellTargetUnit();
    [NativeLuaMemberAttribute]
    public static extern @event TriggerRegisterPlayerAllianceChange(trigger whichTrigger, player whichPlayer, alliancetype whichAlliance);
    [NativeLuaMemberAttribute]
    public static extern @event TriggerRegisterPlayerStateEvent(trigger whichTrigger, player whichPlayer, playerstate whichState, limitop opcode, float limitval);
    [NativeLuaMemberAttribute]
    public static extern playerstate GetEventPlayerState();
    [NativeLuaMemberAttribute]
    public static extern @event TriggerRegisterPlayerChatEvent(trigger whichTrigger, player whichPlayer, string chatMessageToDetect, bool exactMatchOnly);
    [NativeLuaMemberAttribute]
    public static extern string GetEventPlayerChatString();
    [NativeLuaMemberAttribute]
    public static extern string GetEventPlayerChatStringMatched();
    [NativeLuaMemberAttribute]
    public static extern @event TriggerRegisterDeathEvent(trigger whichTrigger, widget whichWidget);
    [NativeLuaMemberAttribute]
    public static extern unit GetTriggerUnit();
    [NativeLuaMemberAttribute]
    public static extern @event TriggerRegisterUnitStateEvent(trigger whichTrigger, unit whichUnit, unitstate whichState, limitop opcode, float limitval);
    [NativeLuaMemberAttribute]
    public static extern unitstate GetEventUnitState();
    [NativeLuaMemberAttribute]
    public static extern @event TriggerRegisterUnitEvent(trigger whichTrigger, unit whichUnit, unitevent whichEvent);
    [NativeLuaMemberAttribute]
    public static extern float GetEventDamage();
    [NativeLuaMemberAttribute]
    public static extern unit GetEventDamageSource();
    [NativeLuaMemberAttribute]
    public static extern player GetEventDetectingPlayer();
    [NativeLuaMemberAttribute]
    public static extern @event TriggerRegisterFilterUnitEvent(trigger whichTrigger, unit whichUnit, unitevent whichEvent, boolexpr filter);
    [NativeLuaMemberAttribute]
    public static extern unit GetEventTargetUnit();
    [NativeLuaMemberAttribute]
    public static extern @event TriggerRegisterUnitInRange(trigger whichTrigger, unit whichUnit, float range, boolexpr filter);
    [NativeLuaMemberAttribute]
    public static extern triggercondition TriggerAddCondition(trigger whichTrigger, boolexpr condition);
    [NativeLuaMemberAttribute]
    public static extern void TriggerRemoveCondition(trigger whichTrigger, triggercondition whichCondition);
    [NativeLuaMemberAttribute]
    public static extern void TriggerClearConditions(trigger whichTrigger);
    [NativeLuaMemberAttribute]
    public static extern triggeraction TriggerAddAction(trigger whichTrigger, System.Action actionFunc);
    [NativeLuaMemberAttribute]
    public static extern void TriggerRemoveAction(trigger whichTrigger, triggeraction whichAction);
    [NativeLuaMemberAttribute]
    public static extern void TriggerClearActions(trigger whichTrigger);
    [NativeLuaMemberAttribute]
    public static extern void TriggerSleepAction(float timeout);
    [NativeLuaMemberAttribute]
    public static extern void TriggerWaitForSound(sound s, float offset);
    [NativeLuaMemberAttribute]
    public static extern bool TriggerEvaluate(trigger whichTrigger);
    [NativeLuaMemberAttribute]
    public static extern void TriggerExecute(trigger whichTrigger);
    [NativeLuaMemberAttribute]
    public static extern void TriggerExecuteWait(trigger whichTrigger);
    [NativeLuaMemberAttribute]
    public static extern void TriggerSyncStart();
    [NativeLuaMemberAttribute]
    public static extern void TriggerSyncReady();
    [NativeLuaMemberAttribute]
    public static extern float GetWidgetLife(widget whichWidget);
    [NativeLuaMemberAttribute]
    public static extern void SetWidgetLife(widget whichWidget, float newLife);
    [NativeLuaMemberAttribute]
    public static extern float GetWidgetX(widget whichWidget);
    [NativeLuaMemberAttribute]
    public static extern float GetWidgetY(widget whichWidget);
    [NativeLuaMemberAttribute]
    public static extern widget GetTriggerWidget();
    [NativeLuaMemberAttribute]
    public static extern destructable CreateDestructable(int objectid, float x, float y, float face, float scale, int variation);
    [NativeLuaMemberAttribute]
    public static extern destructable CreateDestructableZ(int objectid, float x, float y, float z, float face, float scale, int variation);
    [NativeLuaMemberAttribute]
    public static extern destructable CreateDeadDestructable(int objectid, float x, float y, float face, float scale, int variation);
    [NativeLuaMemberAttribute]
    public static extern destructable CreateDeadDestructableZ(int objectid, float x, float y, float z, float face, float scale, int variation);
    [NativeLuaMemberAttribute]
    public static extern void RemoveDestructable(destructable d);
    [NativeLuaMemberAttribute]
    public static extern void KillDestructable(destructable d);
    [NativeLuaMemberAttribute]
    public static extern void SetDestructableInvulnerable(destructable d, bool flag);
    [NativeLuaMemberAttribute]
    public static extern bool IsDestructableInvulnerable(destructable d);
    [NativeLuaMemberAttribute]
    public static extern void EnumDestructablesInRect(rect r, boolexpr filter, System.Action actionFunc);
    [NativeLuaMemberAttribute]
    public static extern int GetDestructableTypeId(destructable d);
    [NativeLuaMemberAttribute]
    public static extern float GetDestructableX(destructable d);
    [NativeLuaMemberAttribute]
    public static extern float GetDestructableY(destructable d);
    [NativeLuaMemberAttribute]
    public static extern void SetDestructableLife(destructable d, float life);
    [NativeLuaMemberAttribute]
    public static extern float GetDestructableLife(destructable d);
    [NativeLuaMemberAttribute]
    public static extern void SetDestructableMaxLife(destructable d, float max);
    [NativeLuaMemberAttribute]
    public static extern float GetDestructableMaxLife(destructable d);
    [NativeLuaMemberAttribute]
    public static extern void DestructableRestoreLife(destructable d, float life, bool birth);
    [NativeLuaMemberAttribute]
    public static extern void QueueDestructableAnimation(destructable d, string whichAnimation);
    [NativeLuaMemberAttribute]
    public static extern void SetDestructableAnimation(destructable d, string whichAnimation);
    [NativeLuaMemberAttribute]
    public static extern void SetDestructableAnimationSpeed(destructable d, float speedFactor);
    [NativeLuaMemberAttribute]
    public static extern void ShowDestructable(destructable d, bool flag);
    [NativeLuaMemberAttribute]
    public static extern float GetDestructableOccluderHeight(destructable d);
    [NativeLuaMemberAttribute]
    public static extern void SetDestructableOccluderHeight(destructable d, float height);
    [NativeLuaMemberAttribute]
    public static extern string GetDestructableName(destructable d);
    [NativeLuaMemberAttribute]
    public static extern destructable GetTriggerDestructable();
    [NativeLuaMemberAttribute]
    public static extern item CreateItem(int itemid, float x, float y);
    [NativeLuaMemberAttribute]
    public static extern void RemoveItem(item whichItem);
    [NativeLuaMemberAttribute]
    public static extern player GetItemPlayer(item whichItem);
    [NativeLuaMemberAttribute]
    public static extern int GetItemTypeId(item i);
    [NativeLuaMemberAttribute]
    public static extern float GetItemX(item i);
    [NativeLuaMemberAttribute]
    public static extern float GetItemY(item i);
    [NativeLuaMemberAttribute]
    public static extern void SetItemPosition(item i, float x, float y);
    [NativeLuaMemberAttribute]
    public static extern void SetItemDropOnDeath(item whichItem, bool flag);
    [NativeLuaMemberAttribute]
    public static extern void SetItemDroppable(item i, bool flag);
    [NativeLuaMemberAttribute]
    public static extern void SetItemPawnable(item i, bool flag);
    [NativeLuaMemberAttribute]
    public static extern void SetItemPlayer(item whichItem, player whichPlayer, bool changeColor);
    [NativeLuaMemberAttribute]
    public static extern void SetItemInvulnerable(item whichItem, bool flag);
    [NativeLuaMemberAttribute]
    public static extern bool IsItemInvulnerable(item whichItem);
    [NativeLuaMemberAttribute]
    public static extern void SetItemVisible(item whichItem, bool show);
    [NativeLuaMemberAttribute]
    public static extern bool IsItemVisible(item whichItem);
    [NativeLuaMemberAttribute]
    public static extern bool IsItemOwned(item whichItem);
    [NativeLuaMemberAttribute]
    public static extern bool IsItemPowerup(item whichItem);
    [NativeLuaMemberAttribute]
    public static extern bool IsItemSellable(item whichItem);
    [NativeLuaMemberAttribute]
    public static extern bool IsItemPawnable(item whichItem);
    [NativeLuaMemberAttribute]
    public static extern bool IsItemIdPowerup(int itemId);
    [NativeLuaMemberAttribute]
    public static extern bool IsItemIdSellable(int itemId);
    [NativeLuaMemberAttribute]
    public static extern bool IsItemIdPawnable(int itemId);
    [NativeLuaMemberAttribute]
    public static extern void EnumItemsInRect(rect r, boolexpr filter, System.Action actionFunc);
    [NativeLuaMemberAttribute]
    public static extern int GetItemLevel(item whichItem);
    [NativeLuaMemberAttribute]
    public static extern itemtype GetItemType(item whichItem);
    [NativeLuaMemberAttribute]
    public static extern void SetItemDropID(item whichItem, int unitId);
    [NativeLuaMemberAttribute]
    public static extern string GetItemName(item whichItem);
    [NativeLuaMemberAttribute]
    public static extern int GetItemCharges(item whichItem);
    [NativeLuaMemberAttribute]
    public static extern void SetItemCharges(item whichItem, int charges);
    [NativeLuaMemberAttribute]
    public static extern int GetItemUserData(item whichItem);
    [NativeLuaMemberAttribute]
    public static extern void SetItemUserData(item whichItem, int data);
    [NativeLuaMemberAttribute]
    public static extern unit CreateUnit(player id, int unitid, float x, float y, float face);
    [NativeLuaMemberAttribute]
    public static extern unit CreateUnitByName(player whichPlayer, string unitname, float x, float y, float face);
    [NativeLuaMemberAttribute]
    public static extern unit CreateUnitAtLoc(player id, int unitid, location whichLocation, float face);
    [NativeLuaMemberAttribute]
    public static extern unit CreateUnitAtLocByName(player id, string unitname, location whichLocation, float face);
    [NativeLuaMemberAttribute]
    public static extern unit CreateCorpse(player whichPlayer, int unitid, float x, float y, float face);
    [NativeLuaMemberAttribute]
    public static extern void KillUnit(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern void RemoveUnit(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern void ShowUnit(unit whichUnit, bool show);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitState(unit whichUnit, unitstate whichUnitState, float newVal);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitX(unit whichUnit, float newX);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitY(unit whichUnit, float newY);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitPosition(unit whichUnit, float newX, float newY);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitPositionLoc(unit whichUnit, location whichLocation);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitFacing(unit whichUnit, float facingAngle);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitFacingTimed(unit whichUnit, float facingAngle, float duration);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitMoveSpeed(unit whichUnit, float newSpeed);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitFlyHeight(unit whichUnit, float newHeight, float rate);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitTurnSpeed(unit whichUnit, float newTurnSpeed);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitPropWindow(unit whichUnit, float newPropWindowAngle);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitAcquireRange(unit whichUnit, float newAcquireRange);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitCreepGuard(unit whichUnit, bool creepGuard);
    [NativeLuaMemberAttribute]
    public static extern float GetUnitAcquireRange(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern float GetUnitTurnSpeed(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern float GetUnitPropWindow(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern float GetUnitFlyHeight(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern float GetUnitDefaultAcquireRange(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern float GetUnitDefaultTurnSpeed(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern float GetUnitDefaultPropWindow(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern float GetUnitDefaultFlyHeight(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitOwner(unit whichUnit, player whichPlayer, bool changeColor);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitColor(unit whichUnit, playercolor whichColor);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitScale(unit whichUnit, float scaleX, float scaleY, float scaleZ);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitTimeScale(unit whichUnit, float timeScale);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitBlendTime(unit whichUnit, float blendTime);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitVertexColor(unit whichUnit, int red, int green, int blue, int alpha);
    [NativeLuaMemberAttribute]
    public static extern void QueueUnitAnimation(unit whichUnit, string whichAnimation);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitAnimation(unit whichUnit, string whichAnimation);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitAnimationByIndex(unit whichUnit, int whichAnimation);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitAnimationWithRarity(unit whichUnit, string whichAnimation, raritycontrol rarity);
    [NativeLuaMemberAttribute]
    public static extern void AddUnitAnimationProperties(unit whichUnit, string animProperties, bool add);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitLookAt(unit whichUnit, string whichBone, unit lookAtTarget, float offsetX, float offsetY, float offsetZ);
    [NativeLuaMemberAttribute]
    public static extern void ResetUnitLookAt(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitRescuable(unit whichUnit, player byWhichPlayer, bool flag);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitRescueRange(unit whichUnit, float range);
    [NativeLuaMemberAttribute]
    public static extern void SetHeroStr(unit whichHero, int newStr, bool permanent);
    [NativeLuaMemberAttribute]
    public static extern void SetHeroAgi(unit whichHero, int newAgi, bool permanent);
    [NativeLuaMemberAttribute]
    public static extern void SetHeroInt(unit whichHero, int newInt, bool permanent);
    [NativeLuaMemberAttribute]
    public static extern int GetHeroStr(unit whichHero, bool includeBonuses);
    [NativeLuaMemberAttribute]
    public static extern int GetHeroAgi(unit whichHero, bool includeBonuses);
    [NativeLuaMemberAttribute]
    public static extern int GetHeroInt(unit whichHero, bool includeBonuses);
    [NativeLuaMemberAttribute]
    public static extern bool UnitStripHeroLevel(unit whichHero, int howManyLevels);
    [NativeLuaMemberAttribute]
    public static extern int GetHeroXP(unit whichHero);
    [NativeLuaMemberAttribute]
    public static extern void SetHeroXP(unit whichHero, int newXpVal, bool showEyeCandy);
    [NativeLuaMemberAttribute]
    public static extern int GetHeroSkillPoints(unit whichHero);
    [NativeLuaMemberAttribute]
    public static extern bool UnitModifySkillPoints(unit whichHero, int skillPointDelta);
    [NativeLuaMemberAttribute]
    public static extern void AddHeroXP(unit whichHero, int xpToAdd, bool showEyeCandy);
    [NativeLuaMemberAttribute]
    public static extern void SetHeroLevel(unit whichHero, int level, bool showEyeCandy);
    [NativeLuaMemberAttribute]
    public static extern int GetHeroLevel(unit whichHero);
    [NativeLuaMemberAttribute]
    public static extern int GetUnitLevel(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern string GetHeroProperName(unit whichHero);
    [NativeLuaMemberAttribute]
    public static extern void SuspendHeroXP(unit whichHero, bool flag);
    [NativeLuaMemberAttribute]
    public static extern bool IsSuspendedXP(unit whichHero);
    [NativeLuaMemberAttribute]
    public static extern void SelectHeroSkill(unit whichHero, int abilcode);
    [NativeLuaMemberAttribute]
    public static extern int GetUnitAbilityLevel(unit whichUnit, int abilcode);
    [NativeLuaMemberAttribute]
    public static extern int DecUnitAbilityLevel(unit whichUnit, int abilcode);
    [NativeLuaMemberAttribute]
    public static extern int IncUnitAbilityLevel(unit whichUnit, int abilcode);
    [NativeLuaMemberAttribute]
    public static extern int SetUnitAbilityLevel(unit whichUnit, int abilcode, int level);
    [NativeLuaMemberAttribute]
    public static extern bool ReviveHero(unit whichHero, float x, float y, bool doEyecandy);
    [NativeLuaMemberAttribute]
    public static extern bool ReviveHeroLoc(unit whichHero, location loc, bool doEyecandy);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitExploded(unit whichUnit, bool exploded);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitInvulnerable(unit whichUnit, bool flag);
    [NativeLuaMemberAttribute]
    public static extern void PauseUnit(unit whichUnit, bool flag);
    [NativeLuaMemberAttribute]
    public static extern bool IsUnitPaused(unit whichHero);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitPathing(unit whichUnit, bool flag);
    [NativeLuaMemberAttribute]
    public static extern void ClearSelection();
    [NativeLuaMemberAttribute]
    public static extern void SelectUnit(unit whichUnit, bool flag);
    [NativeLuaMemberAttribute]
    public static extern int GetUnitPointValue(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern int GetUnitPointValueByType(int unitType);
    [NativeLuaMemberAttribute]
    public static extern bool UnitAddItem(unit whichUnit, item whichItem);
    [NativeLuaMemberAttribute]
    public static extern item UnitAddItemById(unit whichUnit, int itemId);
    [NativeLuaMemberAttribute]
    public static extern bool UnitAddItemToSlotById(unit whichUnit, int itemId, int itemSlot);
    [NativeLuaMemberAttribute]
    public static extern void UnitRemoveItem(unit whichUnit, item whichItem);
    [NativeLuaMemberAttribute]
    public static extern item UnitRemoveItemFromSlot(unit whichUnit, int itemSlot);
    [NativeLuaMemberAttribute]
    public static extern bool UnitHasItem(unit whichUnit, item whichItem);
    [NativeLuaMemberAttribute]
    public static extern item UnitItemInSlot(unit whichUnit, int itemSlot);
    [NativeLuaMemberAttribute]
    public static extern int UnitInventorySize(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern bool UnitDropItemPoint(unit whichUnit, item whichItem, float x, float y);
    [NativeLuaMemberAttribute]
    public static extern bool UnitDropItemSlot(unit whichUnit, item whichItem, int slot);
    [NativeLuaMemberAttribute]
    public static extern bool UnitDropItemTarget(unit whichUnit, item whichItem, widget target);
    [NativeLuaMemberAttribute]
    public static extern bool UnitUseItem(unit whichUnit, item whichItem);
    [NativeLuaMemberAttribute]
    public static extern bool UnitUseItemPoint(unit whichUnit, item whichItem, float x, float y);
    [NativeLuaMemberAttribute]
    public static extern bool UnitUseItemTarget(unit whichUnit, item whichItem, widget target);
    [NativeLuaMemberAttribute]
    public static extern float GetUnitX(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern float GetUnitY(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern location GetUnitLoc(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern float GetUnitFacing(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern float GetUnitMoveSpeed(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern float GetUnitDefaultMoveSpeed(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern float GetUnitState(unit whichUnit, unitstate whichUnitState);
    [NativeLuaMemberAttribute]
    public static extern player GetOwningPlayer(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern int GetUnitTypeId(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern race GetUnitRace(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern string GetUnitName(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern int GetUnitFoodUsed(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern int GetUnitFoodMade(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern int GetFoodMade(int unitId);
    [NativeLuaMemberAttribute]
    public static extern int GetFoodUsed(int unitId);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitUseFood(unit whichUnit, bool useFood);
    [NativeLuaMemberAttribute]
    public static extern location GetUnitRallyPoint(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern unit GetUnitRallyUnit(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern destructable GetUnitRallyDestructable(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern bool IsUnitInGroup(unit whichUnit, group whichGroup);
    [NativeLuaMemberAttribute]
    public static extern bool IsUnitInForce(unit whichUnit, force whichForce);
    [NativeLuaMemberAttribute]
    public static extern bool IsUnitOwnedByPlayer(unit whichUnit, player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern bool IsUnitAlly(unit whichUnit, player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern bool IsUnitEnemy(unit whichUnit, player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern bool IsUnitVisible(unit whichUnit, player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern bool IsUnitDetected(unit whichUnit, player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern bool IsUnitInvisible(unit whichUnit, player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern bool IsUnitFogged(unit whichUnit, player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern bool IsUnitMasked(unit whichUnit, player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern bool IsUnitSelected(unit whichUnit, player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern bool IsUnitRace(unit whichUnit, race whichRace);
    [NativeLuaMemberAttribute]
    public static extern bool IsUnitType(unit whichUnit, unittype whichUnitType);
    [NativeLuaMemberAttribute]
    public static extern bool IsUnit(unit whichUnit, unit whichSpecifiedUnit);
    [NativeLuaMemberAttribute]
    public static extern bool IsUnitInRange(unit whichUnit, unit otherUnit, float distance);
    [NativeLuaMemberAttribute]
    public static extern bool IsUnitInRangeXY(unit whichUnit, float x, float y, float distance);
    [NativeLuaMemberAttribute]
    public static extern bool IsUnitInRangeLoc(unit whichUnit, location whichLocation, float distance);
    [NativeLuaMemberAttribute]
    public static extern bool IsUnitHidden(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern bool IsUnitIllusion(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern bool IsUnitInTransport(unit whichUnit, unit whichTransport);
    [NativeLuaMemberAttribute]
    public static extern bool IsUnitLoaded(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern bool IsHeroUnitId(int unitId);
    [NativeLuaMemberAttribute]
    public static extern bool IsUnitIdType(int unitId, unittype whichUnitType);
    [NativeLuaMemberAttribute]
    public static extern void UnitShareVision(unit whichUnit, player whichPlayer, bool share);
    [NativeLuaMemberAttribute]
    public static extern void UnitSuspendDecay(unit whichUnit, bool suspend);
    [NativeLuaMemberAttribute]
    public static extern bool UnitAddType(unit whichUnit, unittype whichUnitType);
    [NativeLuaMemberAttribute]
    public static extern bool UnitRemoveType(unit whichUnit, unittype whichUnitType);
    [NativeLuaMemberAttribute]
    public static extern bool UnitAddAbility(unit whichUnit, int abilityId);
    [NativeLuaMemberAttribute]
    public static extern bool UnitRemoveAbility(unit whichUnit, int abilityId);
    [NativeLuaMemberAttribute]
    public static extern bool UnitMakeAbilityPermanent(unit whichUnit, bool permanent, int abilityId);
    [NativeLuaMemberAttribute]
    public static extern void UnitRemoveBuffs(unit whichUnit, bool removePositive, bool removeNegative);
    [NativeLuaMemberAttribute]
    public static extern void UnitRemoveBuffsEx(unit whichUnit, bool removePositive, bool removeNegative, bool magic, bool physical, bool timedLife, bool aura, bool autoDispel);
    [NativeLuaMemberAttribute]
    public static extern bool UnitHasBuffsEx(unit whichUnit, bool removePositive, bool removeNegative, bool magic, bool physical, bool timedLife, bool aura, bool autoDispel);
    [NativeLuaMemberAttribute]
    public static extern int UnitCountBuffsEx(unit whichUnit, bool removePositive, bool removeNegative, bool magic, bool physical, bool timedLife, bool aura, bool autoDispel);
    [NativeLuaMemberAttribute]
    public static extern void UnitAddSleep(unit whichUnit, bool add);
    [NativeLuaMemberAttribute]
    public static extern bool UnitCanSleep(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern void UnitAddSleepPerm(unit whichUnit, bool add);
    [NativeLuaMemberAttribute]
    public static extern bool UnitCanSleepPerm(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern bool UnitIsSleeping(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern void UnitWakeUp(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern void UnitApplyTimedLife(unit whichUnit, int buffId, float duration);
    [NativeLuaMemberAttribute]
    public static extern bool UnitIgnoreAlarm(unit whichUnit, bool flag);
    [NativeLuaMemberAttribute]
    public static extern bool UnitIgnoreAlarmToggled(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern void UnitResetCooldown(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern void UnitSetConstructionProgress(unit whichUnit, int constructionPercentage);
    [NativeLuaMemberAttribute]
    public static extern void UnitSetUpgradeProgress(unit whichUnit, int upgradePercentage);
    [NativeLuaMemberAttribute]
    public static extern void UnitPauseTimedLife(unit whichUnit, bool flag);
    [NativeLuaMemberAttribute]
    public static extern void UnitSetUsesAltIcon(unit whichUnit, bool flag);
    [NativeLuaMemberAttribute]
    public static extern bool UnitDamagePoint(unit whichUnit, float delay, float radius, float x, float y, float amount, bool attack, bool ranged, attacktype attackType, damagetype damageType, weapontype weaponType);
    [NativeLuaMemberAttribute]
    public static extern bool UnitDamageTarget(unit whichUnit, widget target, float amount, bool attack, bool ranged, attacktype attackType, damagetype damageType, weapontype weaponType);
    [NativeLuaMemberAttribute]
    public static extern bool IssueImmediateOrder(unit whichUnit, string order);
    [NativeLuaMemberAttribute]
    public static extern bool IssueImmediateOrderById(unit whichUnit, int order);
    [NativeLuaMemberAttribute]
    public static extern bool IssuePointOrder(unit whichUnit, string order, float x, float y);
    [NativeLuaMemberAttribute]
    public static extern bool IssuePointOrderLoc(unit whichUnit, string order, location whichLocation);
    [NativeLuaMemberAttribute]
    public static extern bool IssuePointOrderById(unit whichUnit, int order, float x, float y);
    [NativeLuaMemberAttribute]
    public static extern bool IssuePointOrderByIdLoc(unit whichUnit, int order, location whichLocation);
    [NativeLuaMemberAttribute]
    public static extern bool IssueTargetOrder(unit whichUnit, string order, widget targetWidget);
    [NativeLuaMemberAttribute]
    public static extern bool IssueTargetOrderById(unit whichUnit, int order, widget targetWidget);
    [NativeLuaMemberAttribute]
    public static extern bool IssueInstantPointOrder(unit whichUnit, string order, float x, float y, widget instantTargetWidget);
    [NativeLuaMemberAttribute]
    public static extern bool IssueInstantPointOrderById(unit whichUnit, int order, float x, float y, widget instantTargetWidget);
    [NativeLuaMemberAttribute]
    public static extern bool IssueInstantTargetOrder(unit whichUnit, string order, widget targetWidget, widget instantTargetWidget);
    [NativeLuaMemberAttribute]
    public static extern bool IssueInstantTargetOrderById(unit whichUnit, int order, widget targetWidget, widget instantTargetWidget);
    [NativeLuaMemberAttribute]
    public static extern bool IssueBuildOrder(unit whichPeon, string unitToBuild, float x, float y);
    [NativeLuaMemberAttribute]
    public static extern bool IssueBuildOrderById(unit whichPeon, int unitId, float x, float y);
    [NativeLuaMemberAttribute]
    public static extern bool IssueNeutralImmediateOrder(player forWhichPlayer, unit neutralStructure, string unitToBuild);
    [NativeLuaMemberAttribute]
    public static extern bool IssueNeutralImmediateOrderById(player forWhichPlayer, unit neutralStructure, int unitId);
    [NativeLuaMemberAttribute]
    public static extern bool IssueNeutralPointOrder(player forWhichPlayer, unit neutralStructure, string unitToBuild, float x, float y);
    [NativeLuaMemberAttribute]
    public static extern bool IssueNeutralPointOrderById(player forWhichPlayer, unit neutralStructure, int unitId, float x, float y);
    [NativeLuaMemberAttribute]
    public static extern bool IssueNeutralTargetOrder(player forWhichPlayer, unit neutralStructure, string unitToBuild, widget target);
    [NativeLuaMemberAttribute]
    public static extern bool IssueNeutralTargetOrderById(player forWhichPlayer, unit neutralStructure, int unitId, widget target);
    [NativeLuaMemberAttribute]
    public static extern int GetUnitCurrentOrder(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern void SetResourceAmount(unit whichUnit, int amount);
    [NativeLuaMemberAttribute]
    public static extern void AddResourceAmount(unit whichUnit, int amount);
    [NativeLuaMemberAttribute]
    public static extern int GetResourceAmount(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern float WaygateGetDestinationX(unit waygate);
    [NativeLuaMemberAttribute]
    public static extern float WaygateGetDestinationY(unit waygate);
    [NativeLuaMemberAttribute]
    public static extern void WaygateSetDestination(unit waygate, float x, float y);
    [NativeLuaMemberAttribute]
    public static extern void WaygateActivate(unit waygate, bool activate);
    [NativeLuaMemberAttribute]
    public static extern bool WaygateIsActive(unit waygate);
    [NativeLuaMemberAttribute]
    public static extern void AddItemToAllStock(int itemId, int currentStock, int stockMax);
    [NativeLuaMemberAttribute]
    public static extern void AddItemToStock(unit whichUnit, int itemId, int currentStock, int stockMax);
    [NativeLuaMemberAttribute]
    public static extern void AddUnitToAllStock(int unitId, int currentStock, int stockMax);
    [NativeLuaMemberAttribute]
    public static extern void AddUnitToStock(unit whichUnit, int unitId, int currentStock, int stockMax);
    [NativeLuaMemberAttribute]
    public static extern void RemoveItemFromAllStock(int itemId);
    [NativeLuaMemberAttribute]
    public static extern void RemoveItemFromStock(unit whichUnit, int itemId);
    [NativeLuaMemberAttribute]
    public static extern void RemoveUnitFromAllStock(int unitId);
    [NativeLuaMemberAttribute]
    public static extern void RemoveUnitFromStock(unit whichUnit, int unitId);
    [NativeLuaMemberAttribute]
    public static extern void SetAllItemTypeSlots(int slots);
    [NativeLuaMemberAttribute]
    public static extern void SetAllUnitTypeSlots(int slots);
    [NativeLuaMemberAttribute]
    public static extern void SetItemTypeSlots(unit whichUnit, int slots);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitTypeSlots(unit whichUnit, int slots);
    [NativeLuaMemberAttribute]
    public static extern int GetUnitUserData(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern void SetUnitUserData(unit whichUnit, int data);
    [NativeLuaMemberAttribute]
    public static extern player Player(int number);
    [NativeLuaMemberAttribute]
    public static extern player GetLocalPlayer();
    [NativeLuaMemberAttribute]
    public static extern bool IsPlayerAlly(player whichPlayer, player otherPlayer);
    [NativeLuaMemberAttribute]
    public static extern bool IsPlayerEnemy(player whichPlayer, player otherPlayer);
    [NativeLuaMemberAttribute]
    public static extern bool IsPlayerInForce(player whichPlayer, force whichForce);
    [NativeLuaMemberAttribute]
    public static extern bool IsPlayerObserver(player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern bool IsVisibleToPlayer(float x, float y, player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern bool IsLocationVisibleToPlayer(location whichLocation, player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern bool IsFoggedToPlayer(float x, float y, player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern bool IsLocationFoggedToPlayer(location whichLocation, player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern bool IsMaskedToPlayer(float x, float y, player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern bool IsLocationMaskedToPlayer(location whichLocation, player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern race GetPlayerRace(player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern int GetPlayerId(player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern int GetPlayerUnitCount(player whichPlayer, bool includeIncomplete);
    [NativeLuaMemberAttribute]
    public static extern int GetPlayerTypedUnitCount(player whichPlayer, string unitName, bool includeIncomplete, bool includeUpgrades);
    [NativeLuaMemberAttribute]
    public static extern int GetPlayerStructureCount(player whichPlayer, bool includeIncomplete);
    [NativeLuaMemberAttribute]
    public static extern int GetPlayerState(player whichPlayer, playerstate whichPlayerState);
    [NativeLuaMemberAttribute]
    public static extern int GetPlayerScore(player whichPlayer, playerscore whichPlayerScore);
    [NativeLuaMemberAttribute]
    public static extern bool GetPlayerAlliance(player sourcePlayer, player otherPlayer, alliancetype whichAllianceSetting);
    [NativeLuaMemberAttribute]
    public static extern float GetPlayerHandicap(player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern float GetPlayerHandicapXP(player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern void SetPlayerHandicap(player whichPlayer, float handicap);
    [NativeLuaMemberAttribute]
    public static extern void SetPlayerHandicapXP(player whichPlayer, float handicap);
    [NativeLuaMemberAttribute]
    public static extern void SetPlayerTechMaxAllowed(player whichPlayer, int techid, int maximum);
    [NativeLuaMemberAttribute]
    public static extern int GetPlayerTechMaxAllowed(player whichPlayer, int techid);
    [NativeLuaMemberAttribute]
    public static extern void AddPlayerTechResearched(player whichPlayer, int techid, int levels);
    [NativeLuaMemberAttribute]
    public static extern void SetPlayerTechResearched(player whichPlayer, int techid, int setToLevel);
    [NativeLuaMemberAttribute]
    public static extern bool GetPlayerTechResearched(player whichPlayer, int techid, bool specificonly);
    [NativeLuaMemberAttribute]
    public static extern int GetPlayerTechCount(player whichPlayer, int techid, bool specificonly);
    [NativeLuaMemberAttribute]
    public static extern void SetPlayerUnitsOwner(player whichPlayer, int newOwner);
    [NativeLuaMemberAttribute]
    public static extern void CripplePlayer(player whichPlayer, force toWhichPlayers, bool flag);
    [NativeLuaMemberAttribute]
    public static extern void SetPlayerAbilityAvailable(player whichPlayer, int abilid, bool avail);
    [NativeLuaMemberAttribute]
    public static extern void SetPlayerState(player whichPlayer, playerstate whichPlayerState, int value);
    [NativeLuaMemberAttribute]
    public static extern void RemovePlayer(player whichPlayer, playergameresult gameResult);
    [NativeLuaMemberAttribute]
    public static extern void CachePlayerHeroData(player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern void SetFogStateRect(player forWhichPlayer, fogstate whichState, rect where, bool useSharedVision);
    [NativeLuaMemberAttribute]
    public static extern void SetFogStateRadius(player forWhichPlayer, fogstate whichState, float centerx, float centerY, float radius, bool useSharedVision);
    [NativeLuaMemberAttribute]
    public static extern void SetFogStateRadiusLoc(player forWhichPlayer, fogstate whichState, location center, float radius, bool useSharedVision);
    [NativeLuaMemberAttribute]
    public static extern void FogMaskEnable(bool enable);
    [NativeLuaMemberAttribute]
    public static extern bool IsFogMaskEnabled();
    [NativeLuaMemberAttribute]
    public static extern void FogEnable(bool enable);
    [NativeLuaMemberAttribute]
    public static extern bool IsFogEnabled();
    [NativeLuaMemberAttribute]
    public static extern fogmodifier CreateFogModifierRect(player forWhichPlayer, fogstate whichState, rect where, bool useSharedVision, bool afterUnits);
    [NativeLuaMemberAttribute]
    public static extern fogmodifier CreateFogModifierRadius(player forWhichPlayer, fogstate whichState, float centerx, float centerY, float radius, bool useSharedVision, bool afterUnits);
    [NativeLuaMemberAttribute]
    public static extern fogmodifier CreateFogModifierRadiusLoc(player forWhichPlayer, fogstate whichState, location center, float radius, bool useSharedVision, bool afterUnits);
    [NativeLuaMemberAttribute]
    public static extern void DestroyFogModifier(fogmodifier whichFogModifier);
    [NativeLuaMemberAttribute]
    public static extern void FogModifierStart(fogmodifier whichFogModifier);
    [NativeLuaMemberAttribute]
    public static extern void FogModifierStop(fogmodifier whichFogModifier);
    [NativeLuaMemberAttribute]
    public static extern version VersionGet();
    [NativeLuaMemberAttribute]
    public static extern bool VersionCompatible(version whichVersion);
    [NativeLuaMemberAttribute]
    public static extern bool VersionSupported(version whichVersion);
    [NativeLuaMemberAttribute]
    public static extern void EndGame(bool doScoreScreen);
    [NativeLuaMemberAttribute]
    public static extern void ChangeLevel(string newLevel, bool doScoreScreen);
    [NativeLuaMemberAttribute]
    public static extern void RestartGame(bool doScoreScreen);
    [NativeLuaMemberAttribute]
    public static extern void ReloadGame();
    [NativeLuaMemberAttribute]
    public static extern void SetCampaignMenuRace(race r);
    [NativeLuaMemberAttribute]
    public static extern void SetCampaignMenuRaceEx(int campaignIndex);
    [NativeLuaMemberAttribute]
    public static extern void ForceCampaignSelectScreen();
    [NativeLuaMemberAttribute]
    public static extern void LoadGame(string saveFileName, bool doScoreScreen);
    [NativeLuaMemberAttribute]
    public static extern void SaveGame(string saveFileName);
    [NativeLuaMemberAttribute]
    public static extern bool RenameSaveDirectory(string sourceDirName, string destDirName);
    [NativeLuaMemberAttribute]
    public static extern bool RemoveSaveDirectory(string sourceDirName);
    [NativeLuaMemberAttribute]
    public static extern bool CopySaveGame(string sourceSaveName, string destSaveName);
    [NativeLuaMemberAttribute]
    public static extern bool SaveGameExists(string saveName);
    [NativeLuaMemberAttribute]
    public static extern void SyncSelections();
    [NativeLuaMemberAttribute]
    public static extern void SetFloatGameState(fgamestate whichFloatGameState, float value);
    [NativeLuaMemberAttribute]
    public static extern float GetFloatGameState(fgamestate whichFloatGameState);
    [NativeLuaMemberAttribute]
    public static extern void SetIntegerGameState(igamestate whichIntegerGameState, int value);
    [NativeLuaMemberAttribute]
    public static extern int GetIntegerGameState(igamestate whichIntegerGameState);
    [NativeLuaMemberAttribute]
    public static extern void SetTutorialCleared(bool cleared);
    [NativeLuaMemberAttribute]
    public static extern void SetMissionAvailable(int campaignNumber, int missionNumber, bool available);
    [NativeLuaMemberAttribute]
    public static extern void SetCampaignAvailable(int campaignNumber, bool available);
    [NativeLuaMemberAttribute]
    public static extern void SetOpCinematicAvailable(int campaignNumber, bool available);
    [NativeLuaMemberAttribute]
    public static extern void SetEdCinematicAvailable(int campaignNumber, bool available);
    [NativeLuaMemberAttribute]
    public static extern gamedifficulty GetDefaultDifficulty();
    [NativeLuaMemberAttribute]
    public static extern void SetDefaultDifficulty(gamedifficulty g);
    [NativeLuaMemberAttribute]
    public static extern void SetCustomCampaignButtonVisible(int whichButton, bool visible);
    [NativeLuaMemberAttribute]
    public static extern bool GetCustomCampaignButtonVisible(int whichButton);
    [NativeLuaMemberAttribute]
    public static extern void DoNotSaveReplay();
    [NativeLuaMemberAttribute]
    public static extern dialog DialogCreate();
    [NativeLuaMemberAttribute]
    public static extern void DialogDestroy(dialog whichDialog);
    [NativeLuaMemberAttribute]
    public static extern void DialogClear(dialog whichDialog);
    [NativeLuaMemberAttribute]
    public static extern void DialogSetMessage(dialog whichDialog, string messageText);
    [NativeLuaMemberAttribute]
    public static extern button DialogAddButton(dialog whichDialog, string buttonText, int hotkey);
    [NativeLuaMemberAttribute]
    public static extern button DialogAddQuitButton(dialog whichDialog, bool doScoreScreen, string buttonText, int hotkey);
    [NativeLuaMemberAttribute]
    public static extern void DialogDisplay(player whichPlayer, dialog whichDialog, bool flag);
    [NativeLuaMemberAttribute]
    public static extern bool ReloadGameCachesFromDisk();
    [NativeLuaMemberAttribute]
    public static extern gamecache InitGameCache(string campaignFile);
    [NativeLuaMemberAttribute]
    public static extern bool SaveGameCache(gamecache whichCache);
    [NativeLuaMemberAttribute]
    public static extern void StoreInteger(gamecache cache, string missionKey, string key, int value);
    [NativeLuaMemberAttribute]
    public static extern void StoreReal(gamecache cache, string missionKey, string key, float value);
    [NativeLuaMemberAttribute]
    public static extern void StoreBoolean(gamecache cache, string missionKey, string key, bool value);
    [NativeLuaMemberAttribute]
    public static extern bool StoreUnit(gamecache cache, string missionKey, string key, unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern bool StoreString(gamecache cache, string missionKey, string key, string value);
    [NativeLuaMemberAttribute]
    public static extern void SyncStoredInteger(gamecache cache, string missionKey, string key);
    [NativeLuaMemberAttribute]
    public static extern void SyncStoredReal(gamecache cache, string missionKey, string key);
    [NativeLuaMemberAttribute]
    public static extern void SyncStoredBoolean(gamecache cache, string missionKey, string key);
    [NativeLuaMemberAttribute]
    public static extern void SyncStoredUnit(gamecache cache, string missionKey, string key);
    [NativeLuaMemberAttribute]
    public static extern void SyncStoredString(gamecache cache, string missionKey, string key);
    [NativeLuaMemberAttribute]
    public static extern bool HaveStoredInteger(gamecache cache, string missionKey, string key);
    [NativeLuaMemberAttribute]
    public static extern bool HaveStoredReal(gamecache cache, string missionKey, string key);
    [NativeLuaMemberAttribute]
    public static extern bool HaveStoredBoolean(gamecache cache, string missionKey, string key);
    [NativeLuaMemberAttribute]
    public static extern bool HaveStoredUnit(gamecache cache, string missionKey, string key);
    [NativeLuaMemberAttribute]
    public static extern bool HaveStoredString(gamecache cache, string missionKey, string key);
    [NativeLuaMemberAttribute]
    public static extern void FlushGameCache(gamecache cache);
    [NativeLuaMemberAttribute]
    public static extern void FlushStoredMission(gamecache cache, string missionKey);
    [NativeLuaMemberAttribute]
    public static extern void FlushStoredInteger(gamecache cache, string missionKey, string key);
    [NativeLuaMemberAttribute]
    public static extern void FlushStoredReal(gamecache cache, string missionKey, string key);
    [NativeLuaMemberAttribute]
    public static extern void FlushStoredBoolean(gamecache cache, string missionKey, string key);
    [NativeLuaMemberAttribute]
    public static extern void FlushStoredUnit(gamecache cache, string missionKey, string key);
    [NativeLuaMemberAttribute]
    public static extern void FlushStoredString(gamecache cache, string missionKey, string key);
    [NativeLuaMemberAttribute]
    public static extern int GetStoredInteger(gamecache cache, string missionKey, string key);
    [NativeLuaMemberAttribute]
    public static extern float GetStoredReal(gamecache cache, string missionKey, string key);
    [NativeLuaMemberAttribute]
    public static extern bool GetStoredBoolean(gamecache cache, string missionKey, string key);
    [NativeLuaMemberAttribute]
    public static extern string GetStoredString(gamecache cache, string missionKey, string key);
    [NativeLuaMemberAttribute]
    public static extern unit RestoreUnit(gamecache cache, string missionKey, string key, player forWhichPlayer, float x, float y, float facing);
    [NativeLuaMemberAttribute]
    public static extern hashtable InitHashtable();
    [NativeLuaMemberAttribute]
    public static extern void SaveInteger(hashtable table, int parentKey, int childKey, int value);
    [NativeLuaMemberAttribute]
    public static extern void SaveReal(hashtable table, int parentKey, int childKey, float value);
    [NativeLuaMemberAttribute]
    public static extern void SaveBoolean(hashtable table, int parentKey, int childKey, bool value);
    [NativeLuaMemberAttribute]
    public static extern bool SaveStr(hashtable table, int parentKey, int childKey, string value);
    [NativeLuaMemberAttribute]
    public static extern bool SavePlayerHandle(hashtable table, int parentKey, int childKey, player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern bool SaveWidgetHandle(hashtable table, int parentKey, int childKey, widget whichWidget);
    [NativeLuaMemberAttribute]
    public static extern bool SaveDestructableHandle(hashtable table, int parentKey, int childKey, destructable whichDestructable);
    [NativeLuaMemberAttribute]
    public static extern bool SaveItemHandle(hashtable table, int parentKey, int childKey, item whichItem);
    [NativeLuaMemberAttribute]
    public static extern bool SaveUnitHandle(hashtable table, int parentKey, int childKey, unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern bool SaveAbilityHandle(hashtable table, int parentKey, int childKey, ability whichAbility);
    [NativeLuaMemberAttribute]
    public static extern bool SaveTimerHandle(hashtable table, int parentKey, int childKey, timer whichTimer);
    [NativeLuaMemberAttribute]
    public static extern bool SaveTriggerHandle(hashtable table, int parentKey, int childKey, trigger whichTrigger);
    [NativeLuaMemberAttribute]
    public static extern bool SaveTriggerConditionHandle(hashtable table, int parentKey, int childKey, triggercondition whichTriggercondition);
    [NativeLuaMemberAttribute]
    public static extern bool SaveTriggerActionHandle(hashtable table, int parentKey, int childKey, triggeraction whichTriggeraction);
    [NativeLuaMemberAttribute]
    public static extern bool SaveTriggerEventHandle(hashtable table, int parentKey, int childKey, @event whichEvent);
    [NativeLuaMemberAttribute]
    public static extern bool SaveForceHandle(hashtable table, int parentKey, int childKey, force whichForce);
    [NativeLuaMemberAttribute]
    public static extern bool SaveGroupHandle(hashtable table, int parentKey, int childKey, group whichGroup);
    [NativeLuaMemberAttribute]
    public static extern bool SaveLocationHandle(hashtable table, int parentKey, int childKey, location whichLocation);
    [NativeLuaMemberAttribute]
    public static extern bool SaveRectHandle(hashtable table, int parentKey, int childKey, rect whichRect);
    [NativeLuaMemberAttribute]
    public static extern bool SaveBooleanExprHandle(hashtable table, int parentKey, int childKey, boolexpr whichBoolexpr);
    [NativeLuaMemberAttribute]
    public static extern bool SaveSoundHandle(hashtable table, int parentKey, int childKey, sound whichSound);
    [NativeLuaMemberAttribute]
    public static extern bool SaveEffectHandle(hashtable table, int parentKey, int childKey, effect whichEffect);
    [NativeLuaMemberAttribute]
    public static extern bool SaveUnitPoolHandle(hashtable table, int parentKey, int childKey, unitpool whichUnitpool);
    [NativeLuaMemberAttribute]
    public static extern bool SaveItemPoolHandle(hashtable table, int parentKey, int childKey, itempool whichItempool);
    [NativeLuaMemberAttribute]
    public static extern bool SaveQuestHandle(hashtable table, int parentKey, int childKey, quest whichQuest);
    [NativeLuaMemberAttribute]
    public static extern bool SaveQuestItemHandle(hashtable table, int parentKey, int childKey, questitem whichQuestitem);
    [NativeLuaMemberAttribute]
    public static extern bool SaveDefeatConditionHandle(hashtable table, int parentKey, int childKey, defeatcondition whichDefeatcondition);
    [NativeLuaMemberAttribute]
    public static extern bool SaveTimerDialogHandle(hashtable table, int parentKey, int childKey, timerdialog whichTimerdialog);
    [NativeLuaMemberAttribute]
    public static extern bool SaveLeaderboardHandle(hashtable table, int parentKey, int childKey, leaderboard whichLeaderboard);
    [NativeLuaMemberAttribute]
    public static extern bool SaveMultiboardHandle(hashtable table, int parentKey, int childKey, multiboard whichMultiboard);
    [NativeLuaMemberAttribute]
    public static extern bool SaveMultiboardItemHandle(hashtable table, int parentKey, int childKey, multiboarditem whichMultiboarditem);
    [NativeLuaMemberAttribute]
    public static extern bool SaveTrackableHandle(hashtable table, int parentKey, int childKey, trackable whichTrackable);
    [NativeLuaMemberAttribute]
    public static extern bool SaveDialogHandle(hashtable table, int parentKey, int childKey, dialog whichDialog);
    [NativeLuaMemberAttribute]
    public static extern bool SaveButtonHandle(hashtable table, int parentKey, int childKey, button whichButton);
    [NativeLuaMemberAttribute]
    public static extern bool SaveTextTagHandle(hashtable table, int parentKey, int childKey, texttag whichTexttag);
    [NativeLuaMemberAttribute]
    public static extern bool SaveLightningHandle(hashtable table, int parentKey, int childKey, lightning whichLightning);
    [NativeLuaMemberAttribute]
    public static extern bool SaveImageHandle(hashtable table, int parentKey, int childKey, image whichImage);
    [NativeLuaMemberAttribute]
    public static extern bool SaveUbersplatHandle(hashtable table, int parentKey, int childKey, ubersplat whichUbersplat);
    [NativeLuaMemberAttribute]
    public static extern bool SaveRegionHandle(hashtable table, int parentKey, int childKey, region whichRegion);
    [NativeLuaMemberAttribute]
    public static extern bool SaveFogStateHandle(hashtable table, int parentKey, int childKey, fogstate whichFogState);
    [NativeLuaMemberAttribute]
    public static extern bool SaveFogModifierHandle(hashtable table, int parentKey, int childKey, fogmodifier whichFogModifier);
    [NativeLuaMemberAttribute]
    public static extern bool SaveAgentHandle(hashtable table, int parentKey, int childKey, agent whichAgent);
    [NativeLuaMemberAttribute]
    public static extern bool SaveHashtableHandle(hashtable table, int parentKey, int childKey, hashtable whichHashtable);
    [NativeLuaMemberAttribute]
    public static extern bool SaveFrameHandle(hashtable table, int parentKey, int childKey, framehandle whichFrameHandle);
    [NativeLuaMemberAttribute]
    public static extern int LoadInteger(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern float LoadReal(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern bool LoadBoolean(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern string LoadStr(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern player LoadPlayerHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern widget LoadWidgetHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern destructable LoadDestructableHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern item LoadItemHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern unit LoadUnitHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern ability LoadAbilityHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern timer LoadTimerHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern trigger LoadTriggerHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern triggercondition LoadTriggerConditionHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern triggeraction LoadTriggerActionHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern @event LoadTriggerEventHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern force LoadForceHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern group LoadGroupHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern location LoadLocationHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern rect LoadRectHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern boolexpr LoadBooleanExprHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern sound LoadSoundHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern effect LoadEffectHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern unitpool LoadUnitPoolHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern itempool LoadItemPoolHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern quest LoadQuestHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern questitem LoadQuestItemHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern defeatcondition LoadDefeatConditionHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern timerdialog LoadTimerDialogHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern leaderboard LoadLeaderboardHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern multiboard LoadMultiboardHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern multiboarditem LoadMultiboardItemHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern trackable LoadTrackableHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern dialog LoadDialogHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern button LoadButtonHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern texttag LoadTextTagHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern lightning LoadLightningHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern image LoadImageHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern ubersplat LoadUbersplatHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern region LoadRegionHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern fogstate LoadFogStateHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern fogmodifier LoadFogModifierHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern hashtable LoadHashtableHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern framehandle LoadFrameHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern bool HaveSavedInteger(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern bool HaveSavedReal(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern bool HaveSavedBoolean(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern bool HaveSavedString(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern bool HaveSavedHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern void RemoveSavedInteger(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern void RemoveSavedReal(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern void RemoveSavedBoolean(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern void RemoveSavedString(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern void RemoveSavedHandle(hashtable table, int parentKey, int childKey);
    [NativeLuaMemberAttribute]
    public static extern void FlushParentHashtable(hashtable table);
    [NativeLuaMemberAttribute]
    public static extern void FlushChildHashtable(hashtable table, int parentKey);
    [NativeLuaMemberAttribute]
    public static extern int GetRandomInt(int lowBound, int highBound);
    [NativeLuaMemberAttribute]
    public static extern float GetRandomReal(float lowBound, float highBound);
    [NativeLuaMemberAttribute]
    public static extern unitpool CreateUnitPool();
    [NativeLuaMemberAttribute]
    public static extern void DestroyUnitPool(unitpool whichPool);
    [NativeLuaMemberAttribute]
    public static extern void UnitPoolAddUnitType(unitpool whichPool, int unitId, float weight);
    [NativeLuaMemberAttribute]
    public static extern void UnitPoolRemoveUnitType(unitpool whichPool, int unitId);
    [NativeLuaMemberAttribute]
    public static extern unit PlaceRandomUnit(unitpool whichPool, player forWhichPlayer, float x, float y, float facing);
    [NativeLuaMemberAttribute]
    public static extern itempool CreateItemPool();
    [NativeLuaMemberAttribute]
    public static extern void DestroyItemPool(itempool whichItemPool);
    [NativeLuaMemberAttribute]
    public static extern void ItemPoolAddItemType(itempool whichItemPool, int itemId, float weight);
    [NativeLuaMemberAttribute]
    public static extern void ItemPoolRemoveItemType(itempool whichItemPool, int itemId);
    [NativeLuaMemberAttribute]
    public static extern item PlaceRandomItem(itempool whichItemPool, float x, float y);
    [NativeLuaMemberAttribute]
    public static extern int ChooseRandomCreep(int level);
    [NativeLuaMemberAttribute]
    public static extern int ChooseRandomNPBuilding();
    [NativeLuaMemberAttribute]
    public static extern int ChooseRandomItem(int level);
    [NativeLuaMemberAttribute]
    public static extern int ChooseRandomItemEx(itemtype whichType, int level);
    [NativeLuaMemberAttribute]
    public static extern void SetRandomSeed(int seed);
    [NativeLuaMemberAttribute]
    public static extern void SetTerrainFog(float a, float b, float c, float d, float e);
    [NativeLuaMemberAttribute]
    public static extern void ResetTerrainFog();
    [NativeLuaMemberAttribute]
    public static extern void SetUnitFog(float a, float b, float c, float d, float e);
    [NativeLuaMemberAttribute]
    public static extern void SetTerrainFogEx(int style, float zstart, float zend, float density, float red, float green, float blue);
    [NativeLuaMemberAttribute]
    public static extern void DisplayTextToPlayer(player toPlayer, float x, float y, string message);
    [NativeLuaMemberAttribute]
    public static extern void DisplayTimedTextToPlayer(player toPlayer, float x, float y, float duration, string message);
    [NativeLuaMemberAttribute]
    public static extern void DisplayTimedTextFromPlayer(player toPlayer, float x, float y, float duration, string message);
    [NativeLuaMemberAttribute]
    public static extern void ClearTextMessages();
    [NativeLuaMemberAttribute]
    public static extern void SetDayNightModels(string terrainDNCFile, string unitDNCFile);
    [NativeLuaMemberAttribute]
    public static extern void SetSkyModel(string skyModelFile);
    [NativeLuaMemberAttribute]
    public static extern void EnableUserControl(bool b);
    [NativeLuaMemberAttribute]
    public static extern void EnableUserUI(bool b);
    [NativeLuaMemberAttribute]
    public static extern void SuspendTimeOfDay(bool b);
    [NativeLuaMemberAttribute]
    public static extern void SetTimeOfDayScale(float r);
    [NativeLuaMemberAttribute]
    public static extern float GetTimeOfDayScale();
    [NativeLuaMemberAttribute]
    public static extern void ShowInterface(bool flag, float fadeDuration);
    [NativeLuaMemberAttribute]
    public static extern void PauseGame(bool flag);
    [NativeLuaMemberAttribute]
    public static extern void UnitAddIndicator(unit whichUnit, int red, int green, int blue, int alpha);
    [NativeLuaMemberAttribute]
    public static extern void AddIndicator(widget whichWidget, int red, int green, int blue, int alpha);
    [NativeLuaMemberAttribute]
    public static extern void PingMinimap(float x, float y, float duration);
    [NativeLuaMemberAttribute]
    public static extern void PingMinimapEx(float x, float y, float duration, int red, int green, int blue, bool extraEffects);
    [NativeLuaMemberAttribute]
    public static extern void EnableOcclusion(bool flag);
    [NativeLuaMemberAttribute]
    public static extern void SetIntroShotText(string introText);
    [NativeLuaMemberAttribute]
    public static extern void SetIntroShotModel(string introModelPath);
    [NativeLuaMemberAttribute]
    public static extern void EnableWorldFogBoundary(bool b);
    [NativeLuaMemberAttribute]
    public static extern void PlayModelCinematic(string modelName);
    [NativeLuaMemberAttribute]
    public static extern void PlayCinematic(string movieName);
    [NativeLuaMemberAttribute]
    public static extern void ForceUIKey(string key);
    [NativeLuaMemberAttribute]
    public static extern void ForceUICancel();
    [NativeLuaMemberAttribute]
    public static extern void DisplayLoadDialog();
    [NativeLuaMemberAttribute]
    public static extern void SetAltMinimapIcon(string iconPath);
    [NativeLuaMemberAttribute]
    public static extern void DisableRestartMission(bool flag);
    [NativeLuaMemberAttribute]
    public static extern texttag CreateTextTag();
    [NativeLuaMemberAttribute]
    public static extern void DestroyTextTag(texttag t);
    [NativeLuaMemberAttribute]
    public static extern void SetTextTagText(texttag t, string s, float height);
    [NativeLuaMemberAttribute]
    public static extern void SetTextTagPos(texttag t, float x, float y, float heightOffset);
    [NativeLuaMemberAttribute]
    public static extern void SetTextTagPosUnit(texttag t, unit whichUnit, float heightOffset);
    [NativeLuaMemberAttribute]
    public static extern void SetTextTagColor(texttag t, int red, int green, int blue, int alpha);
    [NativeLuaMemberAttribute]
    public static extern void SetTextTagVelocity(texttag t, float xvel, float yvel);
    [NativeLuaMemberAttribute]
    public static extern void SetTextTagVisibility(texttag t, bool flag);
    [NativeLuaMemberAttribute]
    public static extern void SetTextTagSuspended(texttag t, bool flag);
    [NativeLuaMemberAttribute]
    public static extern void SetTextTagPermanent(texttag t, bool flag);
    [NativeLuaMemberAttribute]
    public static extern void SetTextTagAge(texttag t, float age);
    [NativeLuaMemberAttribute]
    public static extern void SetTextTagLifespan(texttag t, float lifespan);
    [NativeLuaMemberAttribute]
    public static extern void SetTextTagFadepoint(texttag t, float fadepoint);
    [NativeLuaMemberAttribute]
    public static extern void SetReservedLocalHeroButtons(int reserved);
    [NativeLuaMemberAttribute]
    public static extern int GetAllyColorFilterState();
    [NativeLuaMemberAttribute]
    public static extern void SetAllyColorFilterState(int state);
    [NativeLuaMemberAttribute]
    public static extern bool GetCreepCampFilterState();
    [NativeLuaMemberAttribute]
    public static extern void SetCreepCampFilterState(bool state);
    [NativeLuaMemberAttribute]
    public static extern void EnableMinimapFilterButtons(bool enableAlly, bool enableCreep);
    [NativeLuaMemberAttribute]
    public static extern void EnableDragSelect(bool state, bool ui);
    [NativeLuaMemberAttribute]
    public static extern void EnablePreSelect(bool state, bool ui);
    [NativeLuaMemberAttribute]
    public static extern void EnableSelect(bool state, bool ui);
    [NativeLuaMemberAttribute]
    public static extern trackable CreateTrackable(string trackableModelPath, float x, float y, float facing);
    [NativeLuaMemberAttribute]
    public static extern quest CreateQuest();
    [NativeLuaMemberAttribute]
    public static extern void DestroyQuest(quest whichQuest);
    [NativeLuaMemberAttribute]
    public static extern void QuestSetTitle(quest whichQuest, string title);
    [NativeLuaMemberAttribute]
    public static extern void QuestSetDescription(quest whichQuest, string description);
    [NativeLuaMemberAttribute]
    public static extern void QuestSetIconPath(quest whichQuest, string iconPath);
    [NativeLuaMemberAttribute]
    public static extern void QuestSetRequired(quest whichQuest, bool required);
    [NativeLuaMemberAttribute]
    public static extern void QuestSetCompleted(quest whichQuest, bool completed);
    [NativeLuaMemberAttribute]
    public static extern void QuestSetDiscovered(quest whichQuest, bool discovered);
    [NativeLuaMemberAttribute]
    public static extern void QuestSetFailed(quest whichQuest, bool failed);
    [NativeLuaMemberAttribute]
    public static extern void QuestSetEnabled(quest whichQuest, bool enabled);
    [NativeLuaMemberAttribute]
    public static extern bool IsQuestRequired(quest whichQuest);
    [NativeLuaMemberAttribute]
    public static extern bool IsQuestCompleted(quest whichQuest);
    [NativeLuaMemberAttribute]
    public static extern bool IsQuestDiscovered(quest whichQuest);
    [NativeLuaMemberAttribute]
    public static extern bool IsQuestFailed(quest whichQuest);
    [NativeLuaMemberAttribute]
    public static extern bool IsQuestEnabled(quest whichQuest);
    [NativeLuaMemberAttribute]
    public static extern questitem QuestCreateItem(quest whichQuest);
    [NativeLuaMemberAttribute]
    public static extern void QuestItemSetDescription(questitem whichQuestItem, string description);
    [NativeLuaMemberAttribute]
    public static extern void QuestItemSetCompleted(questitem whichQuestItem, bool completed);
    [NativeLuaMemberAttribute]
    public static extern bool IsQuestItemCompleted(questitem whichQuestItem);
    [NativeLuaMemberAttribute]
    public static extern defeatcondition CreateDefeatCondition();
    [NativeLuaMemberAttribute]
    public static extern void DestroyDefeatCondition(defeatcondition whichCondition);
    [NativeLuaMemberAttribute]
    public static extern void DefeatConditionSetDescription(defeatcondition whichCondition, string description);
    [NativeLuaMemberAttribute]
    public static extern void FlashQuestDialogButton();
    [NativeLuaMemberAttribute]
    public static extern void ForceQuestDialogUpdate();
    [NativeLuaMemberAttribute]
    public static extern timerdialog CreateTimerDialog(timer t);
    [NativeLuaMemberAttribute]
    public static extern void DestroyTimerDialog(timerdialog whichDialog);
    [NativeLuaMemberAttribute]
    public static extern void TimerDialogSetTitle(timerdialog whichDialog, string title);
    [NativeLuaMemberAttribute]
    public static extern void TimerDialogSetTitleColor(timerdialog whichDialog, int red, int green, int blue, int alpha);
    [NativeLuaMemberAttribute]
    public static extern void TimerDialogSetTimeColor(timerdialog whichDialog, int red, int green, int blue, int alpha);
    [NativeLuaMemberAttribute]
    public static extern void TimerDialogSetSpeed(timerdialog whichDialog, float speedMultFactor);
    [NativeLuaMemberAttribute]
    public static extern void TimerDialogDisplay(timerdialog whichDialog, bool display);
    [NativeLuaMemberAttribute]
    public static extern bool IsTimerDialogDisplayed(timerdialog whichDialog);
    [NativeLuaMemberAttribute]
    public static extern void TimerDialogSetRealTimeRemaining(timerdialog whichDialog, float timeRemaining);
    [NativeLuaMemberAttribute]
    public static extern leaderboard CreateLeaderboard();
    [NativeLuaMemberAttribute]
    public static extern void DestroyLeaderboard(leaderboard lb);
    [NativeLuaMemberAttribute]
    public static extern void LeaderboardDisplay(leaderboard lb, bool show);
    [NativeLuaMemberAttribute]
    public static extern bool IsLeaderboardDisplayed(leaderboard lb);
    [NativeLuaMemberAttribute]
    public static extern int LeaderboardGetItemCount(leaderboard lb);
    [NativeLuaMemberAttribute]
    public static extern void LeaderboardSetSizeByItemCount(leaderboard lb, int count);
    [NativeLuaMemberAttribute]
    public static extern void LeaderboardAddItem(leaderboard lb, string label, int value, player p);
    [NativeLuaMemberAttribute]
    public static extern void LeaderboardRemoveItem(leaderboard lb, int index);
    [NativeLuaMemberAttribute]
    public static extern void LeaderboardRemovePlayerItem(leaderboard lb, player p);
    [NativeLuaMemberAttribute]
    public static extern void LeaderboardClear(leaderboard lb);
    [NativeLuaMemberAttribute]
    public static extern void LeaderboardSortItemsByValue(leaderboard lb, bool ascending);
    [NativeLuaMemberAttribute]
    public static extern void LeaderboardSortItemsByPlayer(leaderboard lb, bool ascending);
    [NativeLuaMemberAttribute]
    public static extern void LeaderboardSortItemsByLabel(leaderboard lb, bool ascending);
    [NativeLuaMemberAttribute]
    public static extern bool LeaderboardHasPlayerItem(leaderboard lb, player p);
    [NativeLuaMemberAttribute]
    public static extern int LeaderboardGetPlayerIndex(leaderboard lb, player p);
    [NativeLuaMemberAttribute]
    public static extern void LeaderboardSetLabel(leaderboard lb, string label);
    [NativeLuaMemberAttribute]
    public static extern string LeaderboardGetLabelText(leaderboard lb);
    [NativeLuaMemberAttribute]
    public static extern void PlayerSetLeaderboard(player toPlayer, leaderboard lb);
    [NativeLuaMemberAttribute]
    public static extern leaderboard PlayerGetLeaderboard(player toPlayer);
    [NativeLuaMemberAttribute]
    public static extern void LeaderboardSetLabelColor(leaderboard lb, int red, int green, int blue, int alpha);
    [NativeLuaMemberAttribute]
    public static extern void LeaderboardSetValueColor(leaderboard lb, int red, int green, int blue, int alpha);
    [NativeLuaMemberAttribute]
    public static extern void LeaderboardSetStyle(leaderboard lb, bool showLabel, bool showNames, bool showValues, bool showIcons);
    [NativeLuaMemberAttribute]
    public static extern void LeaderboardSetItemValue(leaderboard lb, int whichItem, int val);
    [NativeLuaMemberAttribute]
    public static extern void LeaderboardSetItemLabel(leaderboard lb, int whichItem, string val);
    [NativeLuaMemberAttribute]
    public static extern void LeaderboardSetItemStyle(leaderboard lb, int whichItem, bool showLabel, bool showValue, bool showIcon);
    [NativeLuaMemberAttribute]
    public static extern void LeaderboardSetItemLabelColor(leaderboard lb, int whichItem, int red, int green, int blue, int alpha);
    [NativeLuaMemberAttribute]
    public static extern void LeaderboardSetItemValueColor(leaderboard lb, int whichItem, int red, int green, int blue, int alpha);
    [NativeLuaMemberAttribute]
    public static extern multiboard CreateMultiboard();
    [NativeLuaMemberAttribute]
    public static extern void DestroyMultiboard(multiboard lb);
    [NativeLuaMemberAttribute]
    public static extern void MultiboardDisplay(multiboard lb, bool show);
    [NativeLuaMemberAttribute]
    public static extern bool IsMultiboardDisplayed(multiboard lb);
    [NativeLuaMemberAttribute]
    public static extern void MultiboardMinimize(multiboard lb, bool minimize);
    [NativeLuaMemberAttribute]
    public static extern bool IsMultiboardMinimized(multiboard lb);
    [NativeLuaMemberAttribute]
    public static extern void MultiboardClear(multiboard lb);
    [NativeLuaMemberAttribute]
    public static extern void MultiboardSetTitleText(multiboard lb, string label);
    [NativeLuaMemberAttribute]
    public static extern string MultiboardGetTitleText(multiboard lb);
    [NativeLuaMemberAttribute]
    public static extern void MultiboardSetTitleTextColor(multiboard lb, int red, int green, int blue, int alpha);
    [NativeLuaMemberAttribute]
    public static extern int MultiboardGetRowCount(multiboard lb);
    [NativeLuaMemberAttribute]
    public static extern int MultiboardGetColumnCount(multiboard lb);
    [NativeLuaMemberAttribute]
    public static extern void MultiboardSetColumnCount(multiboard lb, int count);
    [NativeLuaMemberAttribute]
    public static extern void MultiboardSetRowCount(multiboard lb, int count);
    [NativeLuaMemberAttribute]
    public static extern void MultiboardSetItemsStyle(multiboard lb, bool showValues, bool showIcons);
    [NativeLuaMemberAttribute]
    public static extern void MultiboardSetItemsValue(multiboard lb, string value);
    [NativeLuaMemberAttribute]
    public static extern void MultiboardSetItemsValueColor(multiboard lb, int red, int green, int blue, int alpha);
    [NativeLuaMemberAttribute]
    public static extern void MultiboardSetItemsWidth(multiboard lb, float width);
    [NativeLuaMemberAttribute]
    public static extern void MultiboardSetItemsIcon(multiboard lb, string iconPath);
    [NativeLuaMemberAttribute]
    public static extern multiboarditem MultiboardGetItem(multiboard lb, int row, int column);
    [NativeLuaMemberAttribute]
    public static extern void MultiboardReleaseItem(multiboarditem mbi);
    [NativeLuaMemberAttribute]
    public static extern void MultiboardSetItemStyle(multiboarditem mbi, bool showValue, bool showIcon);
    [NativeLuaMemberAttribute]
    public static extern void MultiboardSetItemValue(multiboarditem mbi, string val);
    [NativeLuaMemberAttribute]
    public static extern void MultiboardSetItemValueColor(multiboarditem mbi, int red, int green, int blue, int alpha);
    [NativeLuaMemberAttribute]
    public static extern void MultiboardSetItemWidth(multiboarditem mbi, float width);
    [NativeLuaMemberAttribute]
    public static extern void MultiboardSetItemIcon(multiboarditem mbi, string iconFileName);
    [NativeLuaMemberAttribute]
    public static extern void MultiboardSuppressDisplay(bool flag);
    [NativeLuaMemberAttribute]
    public static extern void SetCameraPosition(float x, float y);
    [NativeLuaMemberAttribute]
    public static extern void SetCameraQuickPosition(float x, float y);
    [NativeLuaMemberAttribute]
    public static extern void SetCameraBounds(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4);
    [NativeLuaMemberAttribute]
    public static extern void StopCamera();
    [NativeLuaMemberAttribute]
    public static extern void ResetToGameCamera(float duration);
    [NativeLuaMemberAttribute]
    public static extern void PanCameraTo(float x, float y);
    [NativeLuaMemberAttribute]
    public static extern void PanCameraToTimed(float x, float y, float duration);
    [NativeLuaMemberAttribute]
    public static extern void PanCameraToWithZ(float x, float y, float zOffsetDest);
    [NativeLuaMemberAttribute]
    public static extern void PanCameraToTimedWithZ(float x, float y, float zOffsetDest, float duration);
    [NativeLuaMemberAttribute]
    public static extern void SetCinematicCamera(string cameraModelFile);
    [NativeLuaMemberAttribute]
    public static extern void SetCameraRotateMode(float x, float y, float radiansToSweep, float duration);
    [NativeLuaMemberAttribute]
    public static extern void SetCameraField(camerafield whichField, float value, float duration);
    [NativeLuaMemberAttribute]
    public static extern void AdjustCameraField(camerafield whichField, float offset, float duration);
    [NativeLuaMemberAttribute]
    public static extern void SetCameraTargetController(unit whichUnit, float xoffset, float yoffset, bool inheritOrientation);
    [NativeLuaMemberAttribute]
    public static extern void SetCameraOrientController(unit whichUnit, float xoffset, float yoffset);
    [NativeLuaMemberAttribute]
    public static extern camerasetup CreateCameraSetup();
    [NativeLuaMemberAttribute]
    public static extern void CameraSetupSetField(camerasetup whichSetup, camerafield whichField, float value, float duration);
    [NativeLuaMemberAttribute]
    public static extern float CameraSetupGetField(camerasetup whichSetup, camerafield whichField);
    [NativeLuaMemberAttribute]
    public static extern void CameraSetupSetDestPosition(camerasetup whichSetup, float x, float y, float duration);
    [NativeLuaMemberAttribute]
    public static extern location CameraSetupGetDestPositionLoc(camerasetup whichSetup);
    [NativeLuaMemberAttribute]
    public static extern float CameraSetupGetDestPositionX(camerasetup whichSetup);
    [NativeLuaMemberAttribute]
    public static extern float CameraSetupGetDestPositionY(camerasetup whichSetup);
    [NativeLuaMemberAttribute]
    public static extern void CameraSetupApply(camerasetup whichSetup, bool doPan, bool panTimed);
    [NativeLuaMemberAttribute]
    public static extern void CameraSetupApplyWithZ(camerasetup whichSetup, float zDestOffset);
    [NativeLuaMemberAttribute]
    public static extern void CameraSetupApplyForceDuration(camerasetup whichSetup, bool doPan, float forceDuration);
    [NativeLuaMemberAttribute]
    public static extern void CameraSetupApplyForceDurationWithZ(camerasetup whichSetup, float zDestOffset, float forceDuration);
    [NativeLuaMemberAttribute]
    public static extern void CameraSetTargetNoise(float mag, float velocity);
    [NativeLuaMemberAttribute]
    public static extern void CameraSetSourceNoise(float mag, float velocity);
    [NativeLuaMemberAttribute]
    public static extern void CameraSetTargetNoiseEx(float mag, float velocity, bool vertOnly);
    [NativeLuaMemberAttribute]
    public static extern void CameraSetSourceNoiseEx(float mag, float velocity, bool vertOnly);
    [NativeLuaMemberAttribute]
    public static extern void CameraSetSmoothingFactor(float factor);
    [NativeLuaMemberAttribute]
    public static extern void SetCineFilterTexture(string filename);
    [NativeLuaMemberAttribute]
    public static extern void SetCineFilterBlendMode(blendmode whichMode);
    [NativeLuaMemberAttribute]
    public static extern void SetCineFilterTexMapFlags(texmapflags whichFlags);
    [NativeLuaMemberAttribute]
    public static extern void SetCineFilterStartUV(float minu, float minv, float maxu, float maxv);
    [NativeLuaMemberAttribute]
    public static extern void SetCineFilterEndUV(float minu, float minv, float maxu, float maxv);
    [NativeLuaMemberAttribute]
    public static extern void SetCineFilterStartColor(int red, int green, int blue, int alpha);
    [NativeLuaMemberAttribute]
    public static extern void SetCineFilterEndColor(int red, int green, int blue, int alpha);
    [NativeLuaMemberAttribute]
    public static extern void SetCineFilterDuration(float duration);
    [NativeLuaMemberAttribute]
    public static extern void DisplayCineFilter(bool flag);
    [NativeLuaMemberAttribute]
    public static extern bool IsCineFilterDisplayed();
    [NativeLuaMemberAttribute]
    public static extern void SetCinematicScene(int portraitUnitId, playercolor color, string speakerTitle, string text, float sceneDuration, float voiceoverDuration);
    [NativeLuaMemberAttribute]
    public static extern void EndCinematicScene();
    [NativeLuaMemberAttribute]
    public static extern void ForceCinematicSubtitles(bool flag);
    [NativeLuaMemberAttribute]
    public static extern float GetCameraMargin(int whichMargin);
    [NativeLuaMemberAttribute]
    public static extern float GetCameraBoundMinX();
    [NativeLuaMemberAttribute]
    public static extern float GetCameraBoundMinY();
    [NativeLuaMemberAttribute]
    public static extern float GetCameraBoundMaxX();
    [NativeLuaMemberAttribute]
    public static extern float GetCameraBoundMaxY();
    [NativeLuaMemberAttribute]
    public static extern float GetCameraField(camerafield whichField);
    [NativeLuaMemberAttribute]
    public static extern float GetCameraTargetPositionX();
    [NativeLuaMemberAttribute]
    public static extern float GetCameraTargetPositionY();
    [NativeLuaMemberAttribute]
    public static extern float GetCameraTargetPositionZ();
    [NativeLuaMemberAttribute]
    public static extern location GetCameraTargetPositionLoc();
    [NativeLuaMemberAttribute]
    public static extern float GetCameraEyePositionX();
    [NativeLuaMemberAttribute]
    public static extern float GetCameraEyePositionY();
    [NativeLuaMemberAttribute]
    public static extern float GetCameraEyePositionZ();
    [NativeLuaMemberAttribute]
    public static extern location GetCameraEyePositionLoc();
    [NativeLuaMemberAttribute]
    public static extern void NewSoundEnvironment(string environmentName);
    [NativeLuaMemberAttribute]
    public static extern sound CreateSound(string fileName, bool looping, bool is3D, bool stopwhenoutofrange, int fadeInRate, int fadeOutRate, string eaxSetting);
    [NativeLuaMemberAttribute]
    public static extern sound CreateSoundFilenameWithLabel(string fileName, bool looping, bool is3D, bool stopwhenoutofrange, int fadeInRate, int fadeOutRate, string SLKEntryName);
    [NativeLuaMemberAttribute]
    public static extern sound CreateSoundFromLabel(string soundLabel, bool looping, bool is3D, bool stopwhenoutofrange, int fadeInRate, int fadeOutRate);
    [NativeLuaMemberAttribute]
    public static extern sound CreateMIDISound(string soundLabel, int fadeInRate, int fadeOutRate);
    [NativeLuaMemberAttribute]
    public static extern void SetSoundParamsFromLabel(sound soundHandle, string soundLabel);
    [NativeLuaMemberAttribute]
    public static extern void SetSoundDistanceCutoff(sound soundHandle, float cutoff);
    [NativeLuaMemberAttribute]
    public static extern void SetSoundChannel(sound soundHandle, int channel);
    [NativeLuaMemberAttribute]
    public static extern void SetSoundVolume(sound soundHandle, int volume);
    [NativeLuaMemberAttribute]
    public static extern void SetSoundPitch(sound soundHandle, float pitch);
    [NativeLuaMemberAttribute]
    public static extern void SetSoundPlayPosition(sound soundHandle, int millisecs);
    [NativeLuaMemberAttribute]
    public static extern void SetSoundDistances(sound soundHandle, float minDist, float maxDist);
    [NativeLuaMemberAttribute]
    public static extern void SetSoundConeAngles(sound soundHandle, float inside, float outside, int outsideVolume);
    [NativeLuaMemberAttribute]
    public static extern void SetSoundConeOrientation(sound soundHandle, float x, float y, float z);
    [NativeLuaMemberAttribute]
    public static extern void SetSoundPosition(sound soundHandle, float x, float y, float z);
    [NativeLuaMemberAttribute]
    public static extern void SetSoundVelocity(sound soundHandle, float x, float y, float z);
    [NativeLuaMemberAttribute]
    public static extern void AttachSoundToUnit(sound soundHandle, unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern void StartSound(sound soundHandle);
    [NativeLuaMemberAttribute]
    public static extern void StopSound(sound soundHandle, bool killWhenDone, bool fadeOut);
    [NativeLuaMemberAttribute]
    public static extern void KillSoundWhenDone(sound soundHandle);
    [NativeLuaMemberAttribute]
    public static extern void SetMapMusic(string musicName, bool random, int index);
    [NativeLuaMemberAttribute]
    public static extern void ClearMapMusic();
    [NativeLuaMemberAttribute]
    public static extern void PlayMusic(string musicName);
    [NativeLuaMemberAttribute]
    public static extern void PlayMusicEx(string musicName, int frommsecs, int fadeinmsecs);
    [NativeLuaMemberAttribute]
    public static extern void StopMusic(bool fadeOut);
    [NativeLuaMemberAttribute]
    public static extern void ResumeMusic();
    [NativeLuaMemberAttribute]
    public static extern void PlayThematicMusic(string musicFileName);
    [NativeLuaMemberAttribute]
    public static extern void PlayThematicMusicEx(string musicFileName, int frommsecs);
    [NativeLuaMemberAttribute]
    public static extern void EndThematicMusic();
    [NativeLuaMemberAttribute]
    public static extern void SetMusicVolume(int volume);
    [NativeLuaMemberAttribute]
    public static extern void SetMusicPlayPosition(int millisecs);
    [NativeLuaMemberAttribute]
    public static extern void SetThematicMusicPlayPosition(int millisecs);
    [NativeLuaMemberAttribute]
    public static extern void SetSoundDuration(sound soundHandle, int duration);
    [NativeLuaMemberAttribute]
    public static extern int GetSoundDuration(sound soundHandle);
    [NativeLuaMemberAttribute]
    public static extern int GetSoundFileDuration(string musicFileName);
    [NativeLuaMemberAttribute]
    public static extern void VolumeGroupSetVolume(volumegroup vgroup, float scale);
    [NativeLuaMemberAttribute]
    public static extern void VolumeGroupReset();
    [NativeLuaMemberAttribute]
    public static extern bool GetSoundIsPlaying(sound soundHandle);
    [NativeLuaMemberAttribute]
    public static extern bool GetSoundIsLoading(sound soundHandle);
    [NativeLuaMemberAttribute]
    public static extern void RegisterStackedSound(sound soundHandle, bool byPosition, float rectwidth, float rectheight);
    [NativeLuaMemberAttribute]
    public static extern void UnregisterStackedSound(sound soundHandle, bool byPosition, float rectwidth, float rectheight);
    [NativeLuaMemberAttribute]
    public static extern weathereffect AddWeatherEffect(rect where, int effectID);
    [NativeLuaMemberAttribute]
    public static extern void RemoveWeatherEffect(weathereffect whichEffect);
    [NativeLuaMemberAttribute]
    public static extern void EnableWeatherEffect(weathereffect whichEffect, bool enable);
    [NativeLuaMemberAttribute]
    public static extern terraindeformation TerrainDeformCrater(float x, float y, float radius, float depth, int duration, bool permanent);
    [NativeLuaMemberAttribute]
    public static extern terraindeformation TerrainDeformRipple(float x, float y, float radius, float depth, int duration, int count, float spaceWaves, float timeWaves, float radiusStartPct, bool limitNeg);
    [NativeLuaMemberAttribute]
    public static extern terraindeformation TerrainDeformWave(float x, float y, float dirX, float dirY, float distance, float speed, float radius, float depth, int trailTime, int count);
    [NativeLuaMemberAttribute]
    public static extern terraindeformation TerrainDeformRandom(float x, float y, float radius, float minDelta, float maxDelta, int duration, int updateInterval);
    [NativeLuaMemberAttribute]
    public static extern void TerrainDeformStop(terraindeformation deformation, int duration);
    [NativeLuaMemberAttribute]
    public static extern void TerrainDeformStopAll();
    [NativeLuaMemberAttribute]
    public static extern effect AddSpecialEffect(string modelName, float x, float y);
    [NativeLuaMemberAttribute]
    public static extern effect AddSpecialEffectLoc(string modelName, location where);
    [NativeLuaMemberAttribute]
    public static extern effect AddSpecialEffectTarget(string modelName, widget targetWidget, string attachPointName);
    [NativeLuaMemberAttribute]
    public static extern void DestroyEffect(effect whichEffect);
    [NativeLuaMemberAttribute]
    public static extern effect AddSpellEffect(string abilityString, effecttype t, float x, float y);
    [NativeLuaMemberAttribute]
    public static extern effect AddSpellEffectLoc(string abilityString, effecttype t, location where);
    [NativeLuaMemberAttribute]
    public static extern effect AddSpellEffectById(int abilityId, effecttype t, float x, float y);
    [NativeLuaMemberAttribute]
    public static extern effect AddSpellEffectByIdLoc(int abilityId, effecttype t, location where);
    [NativeLuaMemberAttribute]
    public static extern effect AddSpellEffectTarget(string modelName, effecttype t, widget targetWidget, string attachPoint);
    [NativeLuaMemberAttribute]
    public static extern effect AddSpellEffectTargetById(int abilityId, effecttype t, widget targetWidget, string attachPoint);
    [NativeLuaMemberAttribute]
    public static extern lightning AddLightning(string codeName, bool checkVisibility, float x1, float y1, float x2, float y2);
    [NativeLuaMemberAttribute]
    public static extern lightning AddLightningEx(string codeName, bool checkVisibility, float x1, float y1, float z1, float x2, float y2, float z2);
    [NativeLuaMemberAttribute]
    public static extern bool DestroyLightning(lightning whichBolt);
    [NativeLuaMemberAttribute]
    public static extern bool MoveLightning(lightning whichBolt, bool checkVisibility, float x1, float y1, float x2, float y2);
    [NativeLuaMemberAttribute]
    public static extern bool MoveLightningEx(lightning whichBolt, bool checkVisibility, float x1, float y1, float z1, float x2, float y2, float z2);
    [NativeLuaMemberAttribute]
    public static extern float GetLightningColorA(lightning whichBolt);
    [NativeLuaMemberAttribute]
    public static extern float GetLightningColorR(lightning whichBolt);
    [NativeLuaMemberAttribute]
    public static extern float GetLightningColorG(lightning whichBolt);
    [NativeLuaMemberAttribute]
    public static extern float GetLightningColorB(lightning whichBolt);
    [NativeLuaMemberAttribute]
    public static extern bool SetLightningColor(lightning whichBolt, float r, float g, float b, float a);
    [NativeLuaMemberAttribute]
    public static extern string GetAbilityEffect(string abilityString, effecttype t, int index);
    [NativeLuaMemberAttribute]
    public static extern string GetAbilityEffectById(int abilityId, effecttype t, int index);
    [NativeLuaMemberAttribute]
    public static extern string GetAbilitySound(string abilityString, soundtype t);
    [NativeLuaMemberAttribute]
    public static extern string GetAbilitySoundById(int abilityId, soundtype t);
    [NativeLuaMemberAttribute]
    public static extern int GetTerrainCliffLevel(float x, float y);
    [NativeLuaMemberAttribute]
    public static extern void SetWaterBaseColor(int red, int green, int blue, int alpha);
    [NativeLuaMemberAttribute]
    public static extern void SetWaterDeforms(bool val);
    [NativeLuaMemberAttribute]
    public static extern int GetTerrainType(float x, float y);
    [NativeLuaMemberAttribute]
    public static extern int GetTerrainVariance(float x, float y);
    [NativeLuaMemberAttribute]
    public static extern void SetTerrainType(float x, float y, int terrainType, int variation, int area, int shape);
    [NativeLuaMemberAttribute]
    public static extern bool IsTerrainPathable(float x, float y, pathingtype t);
    [NativeLuaMemberAttribute]
    public static extern void SetTerrainPathable(float x, float y, pathingtype t, bool flag);
    [NativeLuaMemberAttribute]
    public static extern image CreateImage(string file, float sizeX, float sizeY, float sizeZ, float posX, float posY, float posZ, float originX, float originY, float originZ, int imageType);
    [NativeLuaMemberAttribute]
    public static extern void DestroyImage(image whichImage);
    [NativeLuaMemberAttribute]
    public static extern void ShowImage(image whichImage, bool flag);
    [NativeLuaMemberAttribute]
    public static extern void SetImageConstantHeight(image whichImage, bool flag, float height);
    [NativeLuaMemberAttribute]
    public static extern void SetImagePosition(image whichImage, float x, float y, float z);
    [NativeLuaMemberAttribute]
    public static extern void SetImageColor(image whichImage, int red, int green, int blue, int alpha);
    [NativeLuaMemberAttribute]
    public static extern void SetImageRender(image whichImage, bool flag);
    [NativeLuaMemberAttribute]
    public static extern void SetImageRenderAlways(image whichImage, bool flag);
    [NativeLuaMemberAttribute]
    public static extern void SetImageAboveWater(image whichImage, bool flag, bool useWaterAlpha);
    [NativeLuaMemberAttribute]
    public static extern void SetImageType(image whichImage, int imageType);
    [NativeLuaMemberAttribute]
    public static extern ubersplat CreateUbersplat(float x, float y, string name, int red, int green, int blue, int alpha, bool forcePaused, bool noBirthTime);
    [NativeLuaMemberAttribute]
    public static extern void DestroyUbersplat(ubersplat whichSplat);
    [NativeLuaMemberAttribute]
    public static extern void ResetUbersplat(ubersplat whichSplat);
    [NativeLuaMemberAttribute]
    public static extern void FinishUbersplat(ubersplat whichSplat);
    [NativeLuaMemberAttribute]
    public static extern void ShowUbersplat(ubersplat whichSplat, bool flag);
    [NativeLuaMemberAttribute]
    public static extern void SetUbersplatRender(ubersplat whichSplat, bool flag);
    [NativeLuaMemberAttribute]
    public static extern void SetUbersplatRenderAlways(ubersplat whichSplat, bool flag);
    [NativeLuaMemberAttribute]
    public static extern void SetBlight(player whichPlayer, float x, float y, float radius, bool addBlight);
    [NativeLuaMemberAttribute]
    public static extern void SetBlightRect(player whichPlayer, rect r, bool addBlight);
    [NativeLuaMemberAttribute]
    public static extern void SetBlightPoint(player whichPlayer, float x, float y, bool addBlight);
    [NativeLuaMemberAttribute]
    public static extern void SetBlightLoc(player whichPlayer, location whichLocation, float radius, bool addBlight);
    [NativeLuaMemberAttribute]
    public static extern unit CreateBlightedGoldmine(player id, float x, float y, float face);
    [NativeLuaMemberAttribute]
    public static extern bool IsPointBlighted(float x, float y);
    [NativeLuaMemberAttribute]
    public static extern void SetDoodadAnimation(float x, float y, float radius, int doodadID, bool nearestOnly, string animName, bool animRandom);
    [NativeLuaMemberAttribute]
    public static extern void SetDoodadAnimationRect(rect r, int doodadID, string animName, bool animRandom);
    [NativeLuaMemberAttribute]
    public static extern void StartMeleeAI(player num, string script);
    [NativeLuaMemberAttribute]
    public static extern void StartCampaignAI(player num, string script);
    [NativeLuaMemberAttribute]
    public static extern void CommandAI(player num, int command, int data);
    [NativeLuaMemberAttribute]
    public static extern void PauseCompAI(player p, bool pause);
    [NativeLuaMemberAttribute]
    public static extern aidifficulty GetAIDifficulty(player num);
    [NativeLuaMemberAttribute]
    public static extern void RemoveGuardPosition(unit hUnit);
    [NativeLuaMemberAttribute]
    public static extern void RecycleGuardPosition(unit hUnit);
    [NativeLuaMemberAttribute]
    public static extern void RemoveAllGuardPositions(player num);
    [NativeLuaMemberAttribute]
    public static extern void Cheat(string cheatStr);
    [NativeLuaMemberAttribute]
    public static extern bool IsNoVictoryCheat();
    [NativeLuaMemberAttribute]
    public static extern bool IsNoDefeatCheat();
    [NativeLuaMemberAttribute]
    public static extern void Preload(string filename);
    [NativeLuaMemberAttribute]
    public static extern void PreloadEnd(float timeout);
    [NativeLuaMemberAttribute]
    public static extern void PreloadStart();
    [NativeLuaMemberAttribute]
    public static extern void PreloadRefresh();
    [NativeLuaMemberAttribute]
    public static extern void PreloadEndEx();
    [NativeLuaMemberAttribute]
    public static extern void PreloadGenClear();
    [NativeLuaMemberAttribute]
    public static extern void PreloadGenStart();
    [NativeLuaMemberAttribute]
    public static extern void PreloadGenEnd(string filename);
    [NativeLuaMemberAttribute]
    public static extern void Preloader(string filename);
    [NativeLuaMemberAttribute]
    public static extern void AutomationSetTestType(string testType);
    [NativeLuaMemberAttribute]
    public static extern void AutomationTestStart(string testName);
    [NativeLuaMemberAttribute]
    public static extern void AutomationTestEnd();
    [NativeLuaMemberAttribute]
    public static extern void AutomationTestingFinished();
    [NativeLuaMemberAttribute]
    public static extern float BlzGetTriggerPlayerMouseX();
    [NativeLuaMemberAttribute]
    public static extern float BlzGetTriggerPlayerMouseY();
    [NativeLuaMemberAttribute]
    public static extern location BlzGetTriggerPlayerMousePosition();
    [NativeLuaMemberAttribute]
    public static extern mousebuttontype BlzGetTriggerPlayerMouseButton();
    [NativeLuaMemberAttribute]
    public static extern void BlzSetAbilityTooltip(int abilCode, string tooltip, int level);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetAbilityActivatedTooltip(int abilCode, string tooltip, int level);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetAbilityExtendedTooltip(int abilCode, string extendedTooltip, int level);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetAbilityActivatedExtendedTooltip(int abilCode, string extendedTooltip, int level);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetAbilityResearchTooltip(int abilCode, string researchTooltip, int level);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetAbilityResearchExtendedTooltip(int abilCode, string researchExtendedTooltip, int level);
    [NativeLuaMemberAttribute]
    public static extern string BlzGetAbilityTooltip(int abilCode, int level);
    [NativeLuaMemberAttribute]
    public static extern string BlzGetAbilityActivatedTooltip(int abilCode, int level);
    [NativeLuaMemberAttribute]
    public static extern string BlzGetAbilityExtendedTooltip(int abilCode, int level);
    [NativeLuaMemberAttribute]
    public static extern string BlzGetAbilityActivatedExtendedTooltip(int abilCode, int level);
    [NativeLuaMemberAttribute]
    public static extern string BlzGetAbilityResearchTooltip(int abilCode, int level);
    [NativeLuaMemberAttribute]
    public static extern string BlzGetAbilityResearchExtendedTooltip(int abilCode, int level);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetAbilityIcon(int abilCode, string iconPath);
    [NativeLuaMemberAttribute]
    public static extern string BlzGetAbilityIcon(int abilCode);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetAbilityActivatedIcon(int abilCode, string iconPath);
    [NativeLuaMemberAttribute]
    public static extern string BlzGetAbilityActivatedIcon(int abilCode);
    [NativeLuaMemberAttribute]
    public static extern int BlzGetAbilityPosX(int abilCode);
    [NativeLuaMemberAttribute]
    public static extern int BlzGetAbilityPosY(int abilCode);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetAbilityPosX(int abilCode, int x);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetAbilityPosY(int abilCode, int y);
    [NativeLuaMemberAttribute]
    public static extern int BlzGetAbilityActivatedPosX(int abilCode);
    [NativeLuaMemberAttribute]
    public static extern int BlzGetAbilityActivatedPosY(int abilCode);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetAbilityActivatedPosX(int abilCode, int x);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetAbilityActivatedPosY(int abilCode, int y);
    [NativeLuaMemberAttribute]
    public static extern int BlzGetUnitMaxHP(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetUnitMaxHP(unit whichUnit, int hp);
    [NativeLuaMemberAttribute]
    public static extern int BlzGetUnitMaxMana(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetUnitMaxMana(unit whichUnit, int mana);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetItemName(item whichItem, string name);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetItemDescription(item whichItem, string description);
    [NativeLuaMemberAttribute]
    public static extern string BlzGetItemDescription(item whichItem);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetItemTooltip(item whichItem, string tooltip);
    [NativeLuaMemberAttribute]
    public static extern string BlzGetItemTooltip(item whichItem);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetItemExtendedTooltip(item whichItem, string extendedTooltip);
    [NativeLuaMemberAttribute]
    public static extern string BlzGetItemExtendedTooltip(item whichItem);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetItemIconPath(item whichItem, string iconPath);
    [NativeLuaMemberAttribute]
    public static extern string BlzGetItemIconPath(item whichItem);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetUnitName(unit whichUnit, string name);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetHeroProperName(unit whichUnit, string heroProperName);
    [NativeLuaMemberAttribute]
    public static extern int BlzGetUnitBaseDamage(unit whichUnit, int weaponIndex);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetUnitBaseDamage(unit whichUnit, int baseDamage, int weaponIndex);
    [NativeLuaMemberAttribute]
    public static extern int BlzGetUnitDiceNumber(unit whichUnit, int weaponIndex);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetUnitDiceNumber(unit whichUnit, int diceNumber, int weaponIndex);
    [NativeLuaMemberAttribute]
    public static extern int BlzGetUnitDiceSides(unit whichUnit, int weaponIndex);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetUnitDiceSides(unit whichUnit, int diceSides, int weaponIndex);
    [NativeLuaMemberAttribute]
    public static extern float BlzGetUnitAttackCooldown(unit whichUnit, int weaponIndex);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetUnitAttackCooldown(unit whichUnit, float cooldown, int weaponIndex);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetSpecialEffectColorByPlayer(effect whichEffect, player whichPlayer);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetSpecialEffectColor(effect whichEffect, int r, int g, int b);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetSpecialEffectAlpha(effect whichEffect, int alpha);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetSpecialEffectScale(effect whichEffect, float scale);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetSpecialEffectPosition(effect whichEffect, float x, float y, float z);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetSpecialEffectHeight(effect whichEffect, float height);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetSpecialEffectTimeScale(effect whichEffect, float timeScale);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetSpecialEffectTime(effect whichEffect, float time);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetSpecialEffectOrientation(effect whichEffect, float yaw, float pitch, float roll);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetSpecialEffectYaw(effect whichEffect, float yaw);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetSpecialEffectPitch(effect whichEffect, float pitch);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetSpecialEffectRoll(effect whichEffect, float roll);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetSpecialEffectX(effect whichEffect, float x);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetSpecialEffectY(effect whichEffect, float y);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetSpecialEffectZ(effect whichEffect, float z);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetSpecialEffectPositionLoc(effect whichEffect, location loc);
    [NativeLuaMemberAttribute]
    public static extern float BlzGetLocalSpecialEffectX(effect whichEffect);
    [NativeLuaMemberAttribute]
    public static extern float BlzGetLocalSpecialEffectY(effect whichEffect);
    [NativeLuaMemberAttribute]
    public static extern float BlzGetLocalSpecialEffectZ(effect whichEffect);
    [NativeLuaMemberAttribute]
    public static extern void BlzSpecialEffectClearSubAnimations(effect whichEffect);
    [NativeLuaMemberAttribute]
    public static extern void BlzSpecialEffectRemoveSubAnimation(effect whichEffect, subanimtype whichSubAnim);
    [NativeLuaMemberAttribute]
    public static extern void BlzSpecialEffectAddSubAnimation(effect whichEffect, subanimtype whichSubAnim);
    [NativeLuaMemberAttribute]
    public static extern void BlzPlaySpecialEffect(effect whichEffect, animtype whichAnim);
    [NativeLuaMemberAttribute]
    public static extern void BlzPlaySpecialEffectWithTimeScale(effect whichEffect, animtype whichAnim, float timeScale);
    [NativeLuaMemberAttribute]
    public static extern string BlzGetAnimName(animtype whichAnim);
    [NativeLuaMemberAttribute]
    public static extern float BlzGetUnitArmor(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetUnitArmor(unit whichUnit, float armorAmount);
    [NativeLuaMemberAttribute]
    public static extern void BlzUnitHideAbility(unit whichUnit, int abilId, bool flag);
    [NativeLuaMemberAttribute]
    public static extern void BlzUnitDisableAbility(unit whichUnit, int abilId, bool flag, bool hideUI);
    [NativeLuaMemberAttribute]
    public static extern void BlzUnitCancelTimedLife(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern bool BlzIsUnitSelectable(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern bool BlzIsUnitInvulnerable(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern void BlzUnitInterruptAttack(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern float BlzGetUnitCollisionSize(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern int BlzGetAbilityManaCost(int abilId, int level);
    [NativeLuaMemberAttribute]
    public static extern float BlzGetAbilityCooldown(int abilId, int level);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetUnitAbilityCooldown(unit whichUnit, int abilId, int level, float cooldown);
    [NativeLuaMemberAttribute]
    public static extern float BlzGetUnitAbilityCooldown(unit whichUnit, int abilId, int level);
    [NativeLuaMemberAttribute]
    public static extern float BlzGetUnitAbilityCooldownRemaining(unit whichUnit, int abilId);
    [NativeLuaMemberAttribute]
    public static extern void BlzEndUnitAbilityCooldown(unit whichUnit, int abilCode);
    [NativeLuaMemberAttribute]
    public static extern int BlzGetUnitAbilityManaCost(unit whichUnit, int abilId, int level);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetUnitAbilityManaCost(unit whichUnit, int abilId, int level, int manaCost);
    [NativeLuaMemberAttribute]
    public static extern float BlzGetLocalUnitZ(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern void BlzDecPlayerTechResearched(player whichPlayer, int techid, int levels);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetEventDamage(float damage);
    [NativeLuaMemberAttribute]
    public static extern unit BlzGetEventDamageTarget();
    [NativeLuaMemberAttribute]
    public static extern attacktype BlzGetEventAttackType();
    [NativeLuaMemberAttribute]
    public static extern damagetype BlzGetEventDamageType();
    [NativeLuaMemberAttribute]
    public static extern weapontype BlzGetEventWeaponType();
    [NativeLuaMemberAttribute]
    public static extern bool BlzSetEventAttackType(attacktype attackType);
    [NativeLuaMemberAttribute]
    public static extern bool BlzSetEventDamageType(damagetype damageType);
    [NativeLuaMemberAttribute]
    public static extern bool BlzSetEventWeaponType(weapontype weaponType);
    [NativeLuaMemberAttribute]
    public static extern int RequestExtraIntegerData(int dataType, player whichPlayer, string param1, string param2, bool param3, int param4, int param5, int param6);
    [NativeLuaMemberAttribute]
    public static extern bool RequestExtraBooleanData(int dataType, player whichPlayer, string param1, string param2, bool param3, int param4, int param5, int param6);
    [NativeLuaMemberAttribute]
    public static extern string RequestExtraStringData(int dataType, player whichPlayer, string param1, string param2, bool param3, int param4, int param5, int param6);
    [NativeLuaMemberAttribute]
    public static extern float RequestExtraRealData(int dataType, player whichPlayer, string param1, string param2, bool param3, int param4, int param5, int param6);
    [NativeLuaMemberAttribute]
    public static extern float BlzGetUnitZ(unit whichUnit);
    [NativeLuaMemberAttribute]
    public static extern void BlzEnableSelections(bool enableSelection, bool enableSelectionCircle);
    [NativeLuaMemberAttribute]
    public static extern bool BlzIsSelectionEnabled();
    [NativeLuaMemberAttribute]
    public static extern bool BlzIsSelectionCircleEnabled();
    [NativeLuaMemberAttribute]
    public static extern void BlzCameraSetupApplyForceDurationSmooth(camerasetup whichSetup, bool doPan, float forcedDuration, float easeInDuration, float easeOutDuration, float smoothFactor);
    [NativeLuaMemberAttribute]
    public static extern void BlzEnableTargetIndicator(bool enable);
    [NativeLuaMemberAttribute]
    public static extern bool BlzIsTargetIndicatorEnabled();
    [NativeLuaMemberAttribute]
    public static extern framehandle BlzGetOriginFrame(originframetype frameType, int index);
    [NativeLuaMemberAttribute]
    public static extern void BlzEnableUIAutoPosition(bool enable);
    [NativeLuaMemberAttribute]
    public static extern void BlzHideOriginFrames(bool enable);
    [NativeLuaMemberAttribute]
    public static extern int BlzConvertColor(int a, int r, int g, int b);
    [NativeLuaMemberAttribute]
    public static extern bool BlzLoadTOCFile(string TOCFile);
    [NativeLuaMemberAttribute]
    public static extern framehandle BlzCreateFrame(string name, framehandle owner, int priority, int createContext);
    [NativeLuaMemberAttribute]
    public static extern framehandle BlzCreateSimpleFrame(string name, framehandle owner, int createContext);
    [NativeLuaMemberAttribute]
    public static extern framehandle BlzCreateFrameByType(string typeName, string name, framehandle owner, string inherits, int createContext);
    [NativeLuaMemberAttribute]
    public static extern void BlzDestroyFrame(framehandle frame);
    [NativeLuaMemberAttribute]
    public static extern void BlzFrameSetPoint(framehandle frame, framepointtype point, framehandle relative, framepointtype relativePoint, float x, float y);
    [NativeLuaMemberAttribute]
    public static extern void BlzFrameSetAbsPoint(framehandle frame, framepointtype point, float x, float y);
    [NativeLuaMemberAttribute]
    public static extern void BlzFrameClearAllPoints(framehandle frame);
    [NativeLuaMemberAttribute]
    public static extern void BlzFrameSetAllPoints(framehandle frame, framehandle relative);
    [NativeLuaMemberAttribute]
    public static extern void BlzFrameSetVisible(framehandle frame, bool visible);
    [NativeLuaMemberAttribute]
    public static extern bool BlzFrameIsVisible(framehandle frame);
    [NativeLuaMemberAttribute]
    public static extern framehandle BlzGetFrameByName(string name, int createContext);
    [NativeLuaMemberAttribute]
    public static extern string BlzFrameGetName(framehandle frame);
    [NativeLuaMemberAttribute]
    public static extern void BlzFrameClick(framehandle frame);
    [NativeLuaMemberAttribute]
    public static extern void BlzFrameSetText(framehandle frame, string text);
    [NativeLuaMemberAttribute]
    public static extern string BlzFrameGetText(framehandle frame);
    [NativeLuaMemberAttribute]
    public static extern void BlzFrameAddText(framehandle frame, string text);
    [NativeLuaMemberAttribute]
    public static extern void BlzFrameSetTextSizeLimit(framehandle frame, int size);
    [NativeLuaMemberAttribute]
    public static extern int BlzFrameGetTextSizeLimit(framehandle frame);
    [NativeLuaMemberAttribute]
    public static extern void BlzFrameSetTextColor(framehandle frame, int color);
    [NativeLuaMemberAttribute]
    public static extern void BlzFrameSetFocus(framehandle frame, bool flag);
    [NativeLuaMemberAttribute]
    public static extern void BlzFrameSetModel(framehandle frame, string modelFile, int cameraIndex);
    [NativeLuaMemberAttribute]
    public static extern void BlzFrameSetEnable(framehandle frame, bool enabled);
    [NativeLuaMemberAttribute]
    public static extern bool BlzFrameGetEnable(framehandle frame);
    [NativeLuaMemberAttribute]
    public static extern void BlzFrameSetAlpha(framehandle frame, int alpha);
    [NativeLuaMemberAttribute]
    public static extern int BlzFrameGetAlpha(framehandle frame);
    [NativeLuaMemberAttribute]
    public static extern void BlzFrameSetSpriteAnimate(framehandle frame, int primaryProp, int flags);
    [NativeLuaMemberAttribute]
    public static extern void BlzFrameSetTexture(framehandle frame, string texFile, int flag, bool blend);
    [NativeLuaMemberAttribute]
    public static extern void BlzFrameSetScale(framehandle frame, float scale);
    [NativeLuaMemberAttribute]
    public static extern void BlzFrameSetTooltip(framehandle frame, framehandle tooltip);
    [NativeLuaMemberAttribute]
    public static extern void BlzFrameCageMouse(framehandle frame, bool enable);
    [NativeLuaMemberAttribute]
    public static extern void BlzFrameSetValue(framehandle frame, float value);
    [NativeLuaMemberAttribute]
    public static extern float BlzFrameGetValue(framehandle frame);
    [NativeLuaMemberAttribute]
    public static extern void BlzFrameSetMinMaxValue(framehandle frame, float minValue, float maxValue);
    [NativeLuaMemberAttribute]
    public static extern void BlzFrameSetStepSize(framehandle frame, float stepSize);
    [NativeLuaMemberAttribute]
    public static extern void BlzFrameSetSize(framehandle frame, float width, float height);
    [NativeLuaMemberAttribute]
    public static extern void BlzFrameSetVertexColor(framehandle frame, int color);
    [NativeLuaMemberAttribute]
    public static extern void BlzFrameSetLevel(framehandle frame, int level);
    [NativeLuaMemberAttribute]
    public static extern void BlzFrameSetParent(framehandle frame, framehandle parent);
    [NativeLuaMemberAttribute]
    public static extern framehandle BlzFrameGetParent(framehandle frame);
    [NativeLuaMemberAttribute]
    public static extern float BlzFrameGetHeight(framehandle frame);
    [NativeLuaMemberAttribute]
    public static extern float BlzFrameGetWidth(framehandle frame);
    [NativeLuaMemberAttribute]
    public static extern void BlzFrameSetFont(framehandle frame, string fileName, float height, int flags);
    [NativeLuaMemberAttribute]
    public static extern void BlzFrameSetTextAlignment(framehandle frame, textaligntype vert, textaligntype horz);
    [NativeLuaMemberAttribute]
    public static extern @event BlzTriggerRegisterFrameEvent(trigger whichTrigger, framehandle frame, frameeventtype eventId);
    [NativeLuaMemberAttribute]
    public static extern framehandle BlzGetTriggerFrame();
    [NativeLuaMemberAttribute]
    public static extern frameeventtype BlzGetTriggerFrameEvent();
    [NativeLuaMemberAttribute]
    public static extern float BlzGetTriggerFrameValue();
    [NativeLuaMemberAttribute]
    public static extern string BlzGetTriggerFrameText();
    [NativeLuaMemberAttribute]
    public static extern @event BlzTriggerRegisterPlayerSyncEvent(trigger whichTrigger, player whichPlayer, string prefix, bool fromServer);
    [NativeLuaMemberAttribute]
    public static extern bool BlzSendSyncData(string prefix, string data);
    [NativeLuaMemberAttribute]
    public static extern string BlzGetTriggerSyncPrefix();
    [NativeLuaMemberAttribute]
    public static extern string BlzGetTriggerSyncData();
    [NativeLuaMemberAttribute]
    public static extern @event BlzTriggerRegisterPlayerKeyEvent(trigger whichTrigger, player whichPlayer, oskeytype key, int metaKey, bool keyDown);
    [NativeLuaMemberAttribute]
    public static extern oskeytype BlzGetTriggerPlayerKey();
    [NativeLuaMemberAttribute]
    public static extern int BlzGetTriggerPlayerMetaKey();
    [NativeLuaMemberAttribute]
    public static extern bool BlzGetTriggerPlayerIsKeyDown();
    [NativeLuaMemberAttribute]
    public static extern void BlzEnableCursor(bool enable);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetMousePos(int x, int y);
    [NativeLuaMemberAttribute]
    public static extern int BlzGetLocalClientWidth();
    [NativeLuaMemberAttribute]
    public static extern int BlzGetLocalClientHeight();
    [NativeLuaMemberAttribute]
    public static extern bool BlzIsLocalClientActive();
    [NativeLuaMemberAttribute]
    public static extern unit BlzGetMouseFocusUnit();
    [NativeLuaMemberAttribute]
    public static extern bool BlzChangeMinimapTerrainTex(string texFile);
    [NativeLuaMemberAttribute]
    public static extern string BlzGetLocale();
    [NativeLuaMemberAttribute]
    public static extern float BlzGetSpecialEffectScale(effect whichEffect);
    [NativeLuaMemberAttribute]
    public static extern void BlzSetSpecialEffectMatrixScale(effect whichEffect, float x, float y, float z);
    [NativeLuaMemberAttribute]
    public static extern void BlzResetSpecialEffectMatrix(effect whichEffect);
    [NativeLuaMemberAttribute]
    public static extern ability BlzGetUnitAbility(unit whichUnit, int abilId);
    [NativeLuaMemberAttribute]
    public static extern ability BlzGetUnitAbilityByIndex(unit whichUnit, int index);
    [NativeLuaMemberAttribute]
    public static extern void BlzDisplayChatMessage(player whichPlayer, int recipient, string message);
    [NativeLuaMemberAttribute]
    public static extern void BlzPauseUnitEx(unit whichUnit, bool flag);
    [NativeLuaMemberAttribute]
    public static extern int FourCC(string value);
    [NativeLuaMemberAttribute]
    public static extern int BlzBitOr(int x, int y);
    [NativeLuaMemberAttribute]
    public static extern int BlzBitAnd(int x, int y);
    [NativeLuaMemberAttribute]
    public static extern int BlzBitXor(int x, int y);
    [NativeLuaMemberAttribute]
    public static extern bool BlzGetAbilityBooleanField(ability whichAbility, abilitybooleanfield whichField);
    [NativeLuaMemberAttribute]
    public static extern int BlzGetAbilityIntegerField(ability whichAbility, abilityintegerfield whichField);
    [NativeLuaMemberAttribute]
    public static extern float BlzGetAbilityRealField(ability whichAbility, abilityrealfield whichField);
    [NativeLuaMemberAttribute]
    public static extern string BlzGetAbilityStringField(ability whichAbility, abilitystringfield whichField);
    [NativeLuaMemberAttribute]
    public static extern bool BlzGetAbilityBooleanLevelField(ability whichAbility, abilitybooleanlevelfield whichField, int level);
    [NativeLuaMemberAttribute]
    public static extern int BlzGetAbilityIntegerLevelField(ability whichAbility, abilityintegerlevelfield whichField, int level);
    [NativeLuaMemberAttribute]
    public static extern float BlzGetAbilityRealLevelField(ability whichAbility, abilityreallevelfield whichField, int level);
    [NativeLuaMemberAttribute]
    public static extern string BlzGetAbilityStringLevelField(ability whichAbility, abilitystringlevelfield whichField, int level);
    [NativeLuaMemberAttribute]
    public static extern bool BlzGetAbilityBooleanLevelArrayField(ability whichAbility, abilitybooleanlevelarrayfield whichField, int level, int index);
    [NativeLuaMemberAttribute]
    public static extern int BlzGetAbilityIntegerLevelArrayField(ability whichAbility, abilityintegerlevelarrayfield whichField, int level, int index);
    [NativeLuaMemberAttribute]
    public static extern float BlzGetAbilityRealLevelArrayField(ability whichAbility, abilityreallevelarrayfield whichField, int level, int index);
    [NativeLuaMemberAttribute]
    public static extern string BlzGetAbilityStringLevelArrayField(ability whichAbility, abilitystringlevelarrayfield whichField, int level, int index);
    [NativeLuaMemberAttribute]
    public static extern bool BlzSetAbilityBooleanField(ability whichAbility, abilitybooleanfield whichField, bool value);
    [NativeLuaMemberAttribute]
    public static extern bool BlzSetAbilityIntegerField(ability whichAbility, abilityintegerfield whichField, int value);
    [NativeLuaMemberAttribute]
    public static extern bool BlzSetAbilityRealField(ability whichAbility, abilityrealfield whichField, float value);
    [NativeLuaMemberAttribute]
    public static extern bool BlzSetAbilityStringField(ability whichAbility, abilitystringfield whichField, string value);
    [NativeLuaMemberAttribute]
    public static extern bool BlzSetAbilityBooleanLevelField(ability whichAbility, abilitybooleanlevelfield whichField, int level, bool value);
    [NativeLuaMemberAttribute]
    public static extern bool BlzSetAbilityIntegerLevelField(ability whichAbility, abilityintegerlevelfield whichField, int level, int value);
    [NativeLuaMemberAttribute]
    public static extern bool BlzSetAbilityRealLevelField(ability whichAbility, abilityreallevelfield whichField, int level, float value);
    [NativeLuaMemberAttribute]
    public static extern bool BlzSetAbilityStringLevelField(ability whichAbility, abilitystringlevelfield whichField, int level, string value);
    [NativeLuaMemberAttribute]
    public static extern bool BlzSetAbilityBooleanLevelArrayField(ability whichAbility, abilitybooleanlevelarrayfield whichField, int level, int index, bool value);
    [NativeLuaMemberAttribute]
    public static extern bool BlzSetAbilityIntegerLevelArrayField(ability whichAbility, abilityintegerlevelarrayfield whichField, int level, int index, int value);
    [NativeLuaMemberAttribute]
    public static extern bool BlzSetAbilityRealLevelArrayField(ability whichAbility, abilityreallevelarrayfield whichField, int level, int index, float value);
    [NativeLuaMemberAttribute]
    public static extern bool BlzSetAbilityStringLevelArrayField(ability whichAbility, abilitystringlevelarrayfield whichField, int level, int index, string value);
    [NativeLuaMemberAttribute]
    public static extern bool BlzAddAbilityBooleanLevelArrayField(ability whichAbility, abilitybooleanlevelarrayfield whichField, int level, bool value);
    [NativeLuaMemberAttribute]
    public static extern bool BlzAddAbilityIntegerLevelArrayField(ability whichAbility, abilityintegerlevelarrayfield whichField, int level, int value);
    [NativeLuaMemberAttribute]
    public static extern bool BlzAddAbilityRealLevelArrayField(ability whichAbility, abilityreallevelarrayfield whichField, int level, float value);
    [NativeLuaMemberAttribute]
    public static extern bool BlzAddAbilityStringLevelArrayField(ability whichAbility, abilitystringlevelarrayfield whichField, int level, string value);
    [NativeLuaMemberAttribute]
    public static extern bool BlzRemoveAbilityBooleanLevelArrayField(ability whichAbility, abilitybooleanlevelarrayfield whichField, int level, bool value);
    [NativeLuaMemberAttribute]
    public static extern bool BlzRemoveAbilityIntegerLevelArrayField(ability whichAbility, abilityintegerlevelarrayfield whichField, int level, int value);
    [NativeLuaMemberAttribute]
    public static extern bool BlzRemoveAbilityRealLevelArrayField(ability whichAbility, abilityreallevelarrayfield whichField, int level, float value);
    [NativeLuaMemberAttribute]
    public static extern bool BlzRemoveAbilityStringLevelArrayField(ability whichAbility, abilitystringlevelarrayfield whichField, int level, string value);
    [NativeLuaMemberAttribute]
    public static extern ability BlzGetItemAbilityByIndex(item whichItem, int index);
    [NativeLuaMemberAttribute]
    public static extern ability BlzGetItemAbility(item whichItem, int abilCode);
    [NativeLuaMemberAttribute]
    public static extern bool BlzItemAddAbility(item whichItem, int abilCode);
    [NativeLuaMemberAttribute]
    public static extern bool BlzGetItemBooleanField(item whichItem, itembooleanfield whichField);
    [NativeLuaMemberAttribute]
    public static extern int BlzGetItemIntegerField(item whichItem, itemintegerfield whichField);
    [NativeLuaMemberAttribute]
    public static extern float BlzGetItemRealField(item whichItem, itemrealfield whichField);
    [NativeLuaMemberAttribute]
    public static extern string BlzGetItemStringField(item whichItem, itemstringfield whichField);
    [NativeLuaMemberAttribute]
    public static extern bool BlzSetItemBooleanField(item whichItem, itembooleanfield whichField, bool value);
    [NativeLuaMemberAttribute]
    public static extern bool BlzSetItemIntegerField(item whichItem, itemintegerfield whichField, int value);
    [NativeLuaMemberAttribute]
    public static extern bool BlzSetItemRealField(item whichItem, itemrealfield whichField, float value);
    [NativeLuaMemberAttribute]
    public static extern bool BlzSetItemStringField(item whichItem, itemstringfield whichField, string value);
    [NativeLuaMemberAttribute]
    public static extern bool BlzItemRemoveAbility(item whichItem, int abilCode);
    [NativeLuaMemberAttribute]
    public static extern bool BlzGetUnitBooleanField(unit whichUnit, unitbooleanfield whichField);
    [NativeLuaMemberAttribute]
    public static extern int BlzGetUnitIntegerField(unit whichUnit, unitintegerfield whichField);
    [NativeLuaMemberAttribute]
    public static extern float BlzGetUnitRealField(unit whichUnit, unitrealfield whichField);
    [NativeLuaMemberAttribute]
    public static extern string BlzGetUnitStringField(unit whichUnit, unitstringfield whichField);
    [NativeLuaMemberAttribute]
    public static extern bool BlzSetUnitBooleanField(unit whichUnit, unitbooleanfield whichField, bool value);
    [NativeLuaMemberAttribute]
    public static extern bool BlzSetUnitIntegerField(unit whichUnit, unitintegerfield whichField, int value);
    [NativeLuaMemberAttribute]
    public static extern bool BlzSetUnitRealField(unit whichUnit, unitrealfield whichField, float value);
    [NativeLuaMemberAttribute]
    public static extern bool BlzSetUnitStringField(unit whichUnit, unitstringfield whichField, string value);
    [NativeLuaMemberAttribute]
    public static extern bool BlzGetUnitWeaponBooleanField(unit whichUnit, unitweaponbooleanfield whichField, int index);
    [NativeLuaMemberAttribute]
    public static extern int BlzGetUnitWeaponIntegerField(unit whichUnit, unitweaponintegerfield whichField, int index);
    [NativeLuaMemberAttribute]
    public static extern float BlzGetUnitWeaponRealField(unit whichUnit, unitweaponrealfield whichField, int index);
    [NativeLuaMemberAttribute]
    public static extern string BlzGetUnitWeaponStringField(unit whichUnit, unitweaponstringfield whichField, int index);
    [NativeLuaMemberAttribute]
    public static extern bool BlzSetUnitWeaponBooleanField(unit whichUnit, unitweaponbooleanfield whichField, int index, bool value);
    [NativeLuaMemberAttribute]
    public static extern bool BlzSetUnitWeaponIntegerField(unit whichUnit, unitweaponintegerfield whichField, int index, int value);
    [NativeLuaMemberAttribute]
    public static extern bool BlzSetUnitWeaponRealField(unit whichUnit, unitweaponrealfield whichField, int index, float value);
    [NativeLuaMemberAttribute]
    public static extern bool BlzSetUnitWeaponStringField(unit whichUnit, unitweaponstringfield whichField, int index, string value);
  }
}
