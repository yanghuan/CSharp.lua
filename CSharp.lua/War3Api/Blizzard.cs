#pragma warning disable IDE0052, IDE1006, CS0626
using CSharpLua;
using static War3Api.Common;

namespace War3Api {
  public static class Blizzard {
    [NativeLuaMemberAttribute]
    public const float bj_PI = 3.14159f;
    [NativeLuaMemberAttribute]
    public const float bj_E = 2.71828f;
    [NativeLuaMemberAttribute]
    public const float bj_CELLWIDTH = 128.0f;
    [NativeLuaMemberAttribute]
    public const float bj_CLIFFHEIGHT = 128.0f;
    [NativeLuaMemberAttribute]
    public const float bj_UNIT_FACING = 270.0f;
    [NativeLuaMemberAttribute]
    public const float bj_RADTODEG = 180.0f / bj_PI;
    [NativeLuaMemberAttribute]
    public const float bj_DEGTORAD = bj_PI / 180.0f;
    [NativeLuaMemberAttribute]
    public const float bj_TEXT_DELAY_QUEST = 20.00f;
    [NativeLuaMemberAttribute]
    public const float bj_TEXT_DELAY_QUESTUPDATE = 20.00f;
    [NativeLuaMemberAttribute]
    public const float bj_TEXT_DELAY_QUESTDONE = 20.00f;
    [NativeLuaMemberAttribute]
    public const float bj_TEXT_DELAY_QUESTFAILED = 20.00f;
    [NativeLuaMemberAttribute]
    public const float bj_TEXT_DELAY_QUESTREQUIREMENT = 20.00f;
    [NativeLuaMemberAttribute]
    public const float bj_TEXT_DELAY_MISSIONFAILED = 20.00f;
    [NativeLuaMemberAttribute]
    public const float bj_TEXT_DELAY_ALWAYSHINT = 12.00f;
    [NativeLuaMemberAttribute]
    public const float bj_TEXT_DELAY_HINT = 12.00f;
    [NativeLuaMemberAttribute]
    public const float bj_TEXT_DELAY_SECRET = 10.00f;
    [NativeLuaMemberAttribute]
    public const float bj_TEXT_DELAY_UNITACQUIRED = 15.00f;
    [NativeLuaMemberAttribute]
    public const float bj_TEXT_DELAY_UNITAVAILABLE = 10.00f;
    [NativeLuaMemberAttribute]
    public const float bj_TEXT_DELAY_ITEMACQUIRED = 10.00f;
    [NativeLuaMemberAttribute]
    public const float bj_TEXT_DELAY_WARNING = 12.00f;
    [NativeLuaMemberAttribute]
    public const float bj_QUEUE_DELAY_QUEST = 5.00f;
    [NativeLuaMemberAttribute]
    public const float bj_QUEUE_DELAY_HINT = 5.00f;
    [NativeLuaMemberAttribute]
    public const float bj_QUEUE_DELAY_SECRET = 3.00f;
    [NativeLuaMemberAttribute]
    public const float bj_HANDICAP_EASY = 60.00f;
    [NativeLuaMemberAttribute]
    public const float bj_GAME_STARTED_THRESHOLD = 0.01f;
    [NativeLuaMemberAttribute]
    public const float bj_WAIT_FOR_COND_MIN_INTERVAL = 0.10f;
    [NativeLuaMemberAttribute]
    public const float bj_POLLED_WAIT_INTERVAL = 0.10f;
    [NativeLuaMemberAttribute]
    public const float bj_POLLED_WAIT_SKIP_THRESHOLD = 2.00f;
    [NativeLuaMemberAttribute]
    public const int bj_MAX_INVENTORY = 6;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MAX_PLAYERS = GetBJMaxPlayers();
    [NativeLuaMemberAttribute]
    public static readonly int bj_PLAYER_NEUTRAL_VICTIM = GetBJPlayerNeutralVictim();
    [NativeLuaMemberAttribute]
    public static readonly int bj_PLAYER_NEUTRAL_EXTRA = GetBJPlayerNeutralExtra();
    [NativeLuaMemberAttribute]
    public static readonly int bj_MAX_PLAYER_SLOTS = GetBJMaxPlayerSlots();
    [NativeLuaMemberAttribute]
    public const int bj_MAX_SKELETONS = 25;
    [NativeLuaMemberAttribute]
    public const int bj_MAX_STOCK_ITEM_SLOTS = 11;
    [NativeLuaMemberAttribute]
    public const int bj_MAX_STOCK_UNIT_SLOTS = 11;
    [NativeLuaMemberAttribute]
    public const int bj_MAX_ITEM_LEVEL = 10;
    [NativeLuaMemberAttribute]
    public const float bj_TOD_DAWN = 6.00f;
    [NativeLuaMemberAttribute]
    public const float bj_TOD_DUSK = 18.00f;
    [NativeLuaMemberAttribute]
    public const float bj_MELEE_STARTING_TOD = 8.00f;
    [NativeLuaMemberAttribute]
    public const int bj_MELEE_STARTING_GOLD_V0 = 750;
    [NativeLuaMemberAttribute]
    public const int bj_MELEE_STARTING_GOLD_V1 = 500;
    [NativeLuaMemberAttribute]
    public const int bj_MELEE_STARTING_LUMBER_V0 = 200;
    [NativeLuaMemberAttribute]
    public const int bj_MELEE_STARTING_LUMBER_V1 = 150;
    [NativeLuaMemberAttribute]
    public const int bj_MELEE_STARTING_HERO_TOKENS = 1;
    [NativeLuaMemberAttribute]
    public const int bj_MELEE_HERO_LIMIT = 3;
    [NativeLuaMemberAttribute]
    public const int bj_MELEE_HERO_TYPE_LIMIT = 1;
    [NativeLuaMemberAttribute]
    public const float bj_MELEE_MINE_SEARCH_RADIUS = 2000;
    [NativeLuaMemberAttribute]
    public const float bj_MELEE_CLEAR_UNITS_RADIUS = 1500;
    [NativeLuaMemberAttribute]
    public const float bj_MELEE_CRIPPLE_TIMEOUT = 120.00f;
    [NativeLuaMemberAttribute]
    public const float bj_MELEE_CRIPPLE_MSG_DURATION = 20.00f;
    [NativeLuaMemberAttribute]
    public const int bj_MELEE_MAX_TWINKED_HEROES_V0 = 3;
    [NativeLuaMemberAttribute]
    public const int bj_MELEE_MAX_TWINKED_HEROES_V1 = 1;
    [NativeLuaMemberAttribute]
    public const float bj_CREEP_ITEM_DELAY = 0.50f;
    [NativeLuaMemberAttribute]
    public const float bj_STOCK_RESTOCK_INITIAL_DELAY = 120;
    [NativeLuaMemberAttribute]
    public const float bj_STOCK_RESTOCK_INTERVAL = 30;
    [NativeLuaMemberAttribute]
    public const int bj_STOCK_MAX_ITERATIONS = 20;
    [NativeLuaMemberAttribute]
    public const int bj_MAX_DEST_IN_REGION_EVENTS = 64;
    [NativeLuaMemberAttribute]
    public const int bj_CAMERA_MIN_FARZ = 100;
    [NativeLuaMemberAttribute]
    public const int bj_CAMERA_DEFAULT_DISTANCE = 1650;
    [NativeLuaMemberAttribute]
    public const int bj_CAMERA_DEFAULT_FARZ = 5000;
    [NativeLuaMemberAttribute]
    public const int bj_CAMERA_DEFAULT_AOA = 304;
    [NativeLuaMemberAttribute]
    public const int bj_CAMERA_DEFAULT_FOV = 70;
    [NativeLuaMemberAttribute]
    public const int bj_CAMERA_DEFAULT_ROLL = 0;
    [NativeLuaMemberAttribute]
    public const int bj_CAMERA_DEFAULT_ROTATION = 90;
    [NativeLuaMemberAttribute]
    public const float bj_RESCUE_PING_TIME = 2.00f;
    [NativeLuaMemberAttribute]
    public const float bj_NOTHING_SOUND_DURATION = 5.00f;
    [NativeLuaMemberAttribute]
    public const float bj_TRANSMISSION_PING_TIME = 1.00f;
    [NativeLuaMemberAttribute]
    public const int bj_TRANSMISSION_IND_RED = 255;
    [NativeLuaMemberAttribute]
    public const int bj_TRANSMISSION_IND_BLUE = 255;
    [NativeLuaMemberAttribute]
    public const int bj_TRANSMISSION_IND_GREEN = 255;
    [NativeLuaMemberAttribute]
    public const int bj_TRANSMISSION_IND_ALPHA = 255;
    [NativeLuaMemberAttribute]
    public const float bj_TRANSMISSION_PORT_HANGTIME = 1.50f;
    [NativeLuaMemberAttribute]
    public const float bj_CINEMODE_INTERFACEFADE = 0.50f;
    [NativeLuaMemberAttribute]
    public static readonly gamespeed bj_CINEMODE_GAMESPEED = MAP_SPEED_NORMAL;
    [NativeLuaMemberAttribute]
    public const float bj_CINEMODE_VOLUME_UNITMOVEMENT = 0.40f;
    [NativeLuaMemberAttribute]
    public const float bj_CINEMODE_VOLUME_UNITSOUNDS = 0.00f;
    [NativeLuaMemberAttribute]
    public const float bj_CINEMODE_VOLUME_COMBAT = 0.40f;
    [NativeLuaMemberAttribute]
    public const float bj_CINEMODE_VOLUME_SPELLS = 0.40f;
    [NativeLuaMemberAttribute]
    public const float bj_CINEMODE_VOLUME_UI = 0.00f;
    [NativeLuaMemberAttribute]
    public const float bj_CINEMODE_VOLUME_MUSIC = 0.55f;
    [NativeLuaMemberAttribute]
    public const float bj_CINEMODE_VOLUME_AMBIENTSOUNDS = 1.00f;
    [NativeLuaMemberAttribute]
    public const float bj_CINEMODE_VOLUME_FIRE = 0.60f;
    [NativeLuaMemberAttribute]
    public const float bj_SPEECH_VOLUME_UNITMOVEMENT = 0.25f;
    [NativeLuaMemberAttribute]
    public const float bj_SPEECH_VOLUME_UNITSOUNDS = 0.00f;
    [NativeLuaMemberAttribute]
    public const float bj_SPEECH_VOLUME_COMBAT = 0.25f;
    [NativeLuaMemberAttribute]
    public const float bj_SPEECH_VOLUME_SPELLS = 0.25f;
    [NativeLuaMemberAttribute]
    public const float bj_SPEECH_VOLUME_UI = 0.00f;
    [NativeLuaMemberAttribute]
    public const float bj_SPEECH_VOLUME_MUSIC = 0.55f;
    [NativeLuaMemberAttribute]
    public const float bj_SPEECH_VOLUME_AMBIENTSOUNDS = 1.00f;
    [NativeLuaMemberAttribute]
    public const float bj_SPEECH_VOLUME_FIRE = 0.60f;
    [NativeLuaMemberAttribute]
    public const float bj_SMARTPAN_TRESHOLD_PAN = 500;
    [NativeLuaMemberAttribute]
    public const float bj_SMARTPAN_TRESHOLD_SNAP = 3500;
    [NativeLuaMemberAttribute]
    public const int bj_MAX_QUEUED_TRIGGERS = 100;
    [NativeLuaMemberAttribute]
    public const float bj_QUEUED_TRIGGER_TIMEOUT = 180.00f;
    [NativeLuaMemberAttribute]
    public const int bj_CAMPAIGN_INDEX_T = 0;
    [NativeLuaMemberAttribute]
    public const int bj_CAMPAIGN_INDEX_H = 1;
    [NativeLuaMemberAttribute]
    public const int bj_CAMPAIGN_INDEX_U = 2;
    [NativeLuaMemberAttribute]
    public const int bj_CAMPAIGN_INDEX_O = 3;
    [NativeLuaMemberAttribute]
    public const int bj_CAMPAIGN_INDEX_N = 4;
    [NativeLuaMemberAttribute]
    public const int bj_CAMPAIGN_INDEX_XN = 5;
    [NativeLuaMemberAttribute]
    public const int bj_CAMPAIGN_INDEX_XH = 6;
    [NativeLuaMemberAttribute]
    public const int bj_CAMPAIGN_INDEX_XU = 7;
    [NativeLuaMemberAttribute]
    public const int bj_CAMPAIGN_INDEX_XO = 8;
    [NativeLuaMemberAttribute]
    public const int bj_CAMPAIGN_OFFSET_T = 0;
    [NativeLuaMemberAttribute]
    public const int bj_CAMPAIGN_OFFSET_H = 1;
    [NativeLuaMemberAttribute]
    public const int bj_CAMPAIGN_OFFSET_U = 2;
    [NativeLuaMemberAttribute]
    public const int bj_CAMPAIGN_OFFSET_O = 3;
    [NativeLuaMemberAttribute]
    public const int bj_CAMPAIGN_OFFSET_N = 4;
    [NativeLuaMemberAttribute]
    public const int bj_CAMPAIGN_OFFSET_XN = 0;
    [NativeLuaMemberAttribute]
    public const int bj_CAMPAIGN_OFFSET_XH = 1;
    [NativeLuaMemberAttribute]
    public const int bj_CAMPAIGN_OFFSET_XU = 2;
    [NativeLuaMemberAttribute]
    public const int bj_CAMPAIGN_OFFSET_XO = 3;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_T00 = bj_CAMPAIGN_OFFSET_T * 1000 + 0;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_T01 = bj_CAMPAIGN_OFFSET_T * 1000 + 1;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_H00 = bj_CAMPAIGN_OFFSET_H * 1000 + 0;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_H01 = bj_CAMPAIGN_OFFSET_H * 1000 + 1;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_H02 = bj_CAMPAIGN_OFFSET_H * 1000 + 2;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_H03 = bj_CAMPAIGN_OFFSET_H * 1000 + 3;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_H04 = bj_CAMPAIGN_OFFSET_H * 1000 + 4;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_H05 = bj_CAMPAIGN_OFFSET_H * 1000 + 5;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_H06 = bj_CAMPAIGN_OFFSET_H * 1000 + 6;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_H07 = bj_CAMPAIGN_OFFSET_H * 1000 + 7;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_H08 = bj_CAMPAIGN_OFFSET_H * 1000 + 8;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_H09 = bj_CAMPAIGN_OFFSET_H * 1000 + 9;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_H10 = bj_CAMPAIGN_OFFSET_H * 1000 + 10;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_H11 = bj_CAMPAIGN_OFFSET_H * 1000 + 11;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_U00 = bj_CAMPAIGN_OFFSET_U * 1000 + 0;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_U01 = bj_CAMPAIGN_OFFSET_U * 1000 + 1;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_U02 = bj_CAMPAIGN_OFFSET_U * 1000 + 2;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_U03 = bj_CAMPAIGN_OFFSET_U * 1000 + 3;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_U05 = bj_CAMPAIGN_OFFSET_U * 1000 + 4;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_U07 = bj_CAMPAIGN_OFFSET_U * 1000 + 5;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_U08 = bj_CAMPAIGN_OFFSET_U * 1000 + 6;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_U09 = bj_CAMPAIGN_OFFSET_U * 1000 + 7;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_U10 = bj_CAMPAIGN_OFFSET_U * 1000 + 8;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_U11 = bj_CAMPAIGN_OFFSET_U * 1000 + 9;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_O00 = bj_CAMPAIGN_OFFSET_O * 1000 + 0;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_O01 = bj_CAMPAIGN_OFFSET_O * 1000 + 1;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_O02 = bj_CAMPAIGN_OFFSET_O * 1000 + 2;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_O03 = bj_CAMPAIGN_OFFSET_O * 1000 + 3;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_O04 = bj_CAMPAIGN_OFFSET_O * 1000 + 4;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_O05 = bj_CAMPAIGN_OFFSET_O * 1000 + 5;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_O06 = bj_CAMPAIGN_OFFSET_O * 1000 + 6;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_O07 = bj_CAMPAIGN_OFFSET_O * 1000 + 7;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_O08 = bj_CAMPAIGN_OFFSET_O * 1000 + 8;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_O09 = bj_CAMPAIGN_OFFSET_O * 1000 + 9;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_O10 = bj_CAMPAIGN_OFFSET_O * 1000 + 10;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_N00 = bj_CAMPAIGN_OFFSET_N * 1000 + 0;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_N01 = bj_CAMPAIGN_OFFSET_N * 1000 + 1;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_N02 = bj_CAMPAIGN_OFFSET_N * 1000 + 2;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_N03 = bj_CAMPAIGN_OFFSET_N * 1000 + 3;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_N04 = bj_CAMPAIGN_OFFSET_N * 1000 + 4;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_N05 = bj_CAMPAIGN_OFFSET_N * 1000 + 5;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_N06 = bj_CAMPAIGN_OFFSET_N * 1000 + 6;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_N07 = bj_CAMPAIGN_OFFSET_N * 1000 + 7;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_N08 = bj_CAMPAIGN_OFFSET_N * 1000 + 8;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_N09 = bj_CAMPAIGN_OFFSET_N * 1000 + 9;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XN00 = bj_CAMPAIGN_OFFSET_XN * 1000 + 0;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XN01 = bj_CAMPAIGN_OFFSET_XN * 1000 + 1;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XN02 = bj_CAMPAIGN_OFFSET_XN * 1000 + 2;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XN03 = bj_CAMPAIGN_OFFSET_XN * 1000 + 3;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XN04 = bj_CAMPAIGN_OFFSET_XN * 1000 + 4;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XN05 = bj_CAMPAIGN_OFFSET_XN * 1000 + 5;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XN06 = bj_CAMPAIGN_OFFSET_XN * 1000 + 6;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XN07 = bj_CAMPAIGN_OFFSET_XN * 1000 + 7;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XN08 = bj_CAMPAIGN_OFFSET_XN * 1000 + 8;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XN09 = bj_CAMPAIGN_OFFSET_XN * 1000 + 9;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XN10 = bj_CAMPAIGN_OFFSET_XN * 1000 + 10;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XH00 = bj_CAMPAIGN_OFFSET_XH * 1000 + 0;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XH01 = bj_CAMPAIGN_OFFSET_XH * 1000 + 1;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XH02 = bj_CAMPAIGN_OFFSET_XH * 1000 + 2;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XH03 = bj_CAMPAIGN_OFFSET_XH * 1000 + 3;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XH04 = bj_CAMPAIGN_OFFSET_XH * 1000 + 4;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XH05 = bj_CAMPAIGN_OFFSET_XH * 1000 + 5;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XH06 = bj_CAMPAIGN_OFFSET_XH * 1000 + 6;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XH07 = bj_CAMPAIGN_OFFSET_XH * 1000 + 7;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XH08 = bj_CAMPAIGN_OFFSET_XH * 1000 + 8;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XH09 = bj_CAMPAIGN_OFFSET_XH * 1000 + 9;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XU00 = bj_CAMPAIGN_OFFSET_XU * 1000 + 0;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XU01 = bj_CAMPAIGN_OFFSET_XU * 1000 + 1;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XU02 = bj_CAMPAIGN_OFFSET_XU * 1000 + 2;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XU03 = bj_CAMPAIGN_OFFSET_XU * 1000 + 3;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XU04 = bj_CAMPAIGN_OFFSET_XU * 1000 + 4;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XU05 = bj_CAMPAIGN_OFFSET_XU * 1000 + 5;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XU06 = bj_CAMPAIGN_OFFSET_XU * 1000 + 6;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XU07 = bj_CAMPAIGN_OFFSET_XU * 1000 + 7;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XU08 = bj_CAMPAIGN_OFFSET_XU * 1000 + 8;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XU09 = bj_CAMPAIGN_OFFSET_XU * 1000 + 9;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XU10 = bj_CAMPAIGN_OFFSET_XU * 1000 + 10;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XU11 = bj_CAMPAIGN_OFFSET_XU * 1000 + 11;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XU12 = bj_CAMPAIGN_OFFSET_XU * 1000 + 12;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XU13 = bj_CAMPAIGN_OFFSET_XU * 1000 + 13;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XO00 = bj_CAMPAIGN_OFFSET_XO * 1000 + 0;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XO01 = bj_CAMPAIGN_OFFSET_XO * 1000 + 1;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XO02 = bj_CAMPAIGN_OFFSET_XO * 1000 + 2;
    [NativeLuaMemberAttribute]
    public static readonly int bj_MISSION_INDEX_XO03 = bj_CAMPAIGN_OFFSET_XO * 1000 + 3;
    [NativeLuaMemberAttribute]
    public const int bj_CINEMATICINDEX_TOP = 0;
    [NativeLuaMemberAttribute]
    public const int bj_CINEMATICINDEX_HOP = 1;
    [NativeLuaMemberAttribute]
    public const int bj_CINEMATICINDEX_HED = 2;
    [NativeLuaMemberAttribute]
    public const int bj_CINEMATICINDEX_OOP = 3;
    [NativeLuaMemberAttribute]
    public const int bj_CINEMATICINDEX_OED = 4;
    [NativeLuaMemberAttribute]
    public const int bj_CINEMATICINDEX_UOP = 5;
    [NativeLuaMemberAttribute]
    public const int bj_CINEMATICINDEX_UED = 6;
    [NativeLuaMemberAttribute]
    public const int bj_CINEMATICINDEX_NOP = 7;
    [NativeLuaMemberAttribute]
    public const int bj_CINEMATICINDEX_NED = 8;
    [NativeLuaMemberAttribute]
    public const int bj_CINEMATICINDEX_XOP = 9;
    [NativeLuaMemberAttribute]
    public const int bj_CINEMATICINDEX_XED = 10;
    [NativeLuaMemberAttribute]
    public const int bj_ALLIANCE_UNALLIED = 0;
    [NativeLuaMemberAttribute]
    public const int bj_ALLIANCE_UNALLIED_VISION = 1;
    [NativeLuaMemberAttribute]
    public const int bj_ALLIANCE_ALLIED = 2;
    [NativeLuaMemberAttribute]
    public const int bj_ALLIANCE_ALLIED_VISION = 3;
    [NativeLuaMemberAttribute]
    public const int bj_ALLIANCE_ALLIED_UNITS = 4;
    [NativeLuaMemberAttribute]
    public const int bj_ALLIANCE_ALLIED_ADVUNITS = 5;
    [NativeLuaMemberAttribute]
    public const int bj_ALLIANCE_NEUTRAL = 6;
    [NativeLuaMemberAttribute]
    public const int bj_ALLIANCE_NEUTRAL_VISION = 7;
    [NativeLuaMemberAttribute]
    public const int bj_KEYEVENTTYPE_DEPRESS = 0;
    [NativeLuaMemberAttribute]
    public const int bj_KEYEVENTTYPE_RELEASE = 1;
    [NativeLuaMemberAttribute]
    public const int bj_KEYEVENTKEY_LEFT = 0;
    [NativeLuaMemberAttribute]
    public const int bj_KEYEVENTKEY_RIGHT = 1;
    [NativeLuaMemberAttribute]
    public const int bj_KEYEVENTKEY_DOWN = 2;
    [NativeLuaMemberAttribute]
    public const int bj_KEYEVENTKEY_UP = 3;
    [NativeLuaMemberAttribute]
    public const int bj_MOUSEEVENTTYPE_DOWN = 0;
    [NativeLuaMemberAttribute]
    public const int bj_MOUSEEVENTTYPE_UP = 1;
    [NativeLuaMemberAttribute]
    public const int bj_MOUSEEVENTTYPE_MOVE = 2;
    [NativeLuaMemberAttribute]
    public const int bj_TIMETYPE_ADD = 0;
    [NativeLuaMemberAttribute]
    public const int bj_TIMETYPE_SET = 1;
    [NativeLuaMemberAttribute]
    public const int bj_TIMETYPE_SUB = 2;
    [NativeLuaMemberAttribute]
    public const int bj_CAMERABOUNDS_ADJUST_ADD = 0;
    [NativeLuaMemberAttribute]
    public const int bj_CAMERABOUNDS_ADJUST_SUB = 1;
    [NativeLuaMemberAttribute]
    public const int bj_QUESTTYPE_REQ_DISCOVERED = 0;
    [NativeLuaMemberAttribute]
    public const int bj_QUESTTYPE_REQ_UNDISCOVERED = 1;
    [NativeLuaMemberAttribute]
    public const int bj_QUESTTYPE_OPT_DISCOVERED = 2;
    [NativeLuaMemberAttribute]
    public const int bj_QUESTTYPE_OPT_UNDISCOVERED = 3;
    [NativeLuaMemberAttribute]
    public const int bj_QUESTMESSAGE_DISCOVERED = 0;
    [NativeLuaMemberAttribute]
    public const int bj_QUESTMESSAGE_UPDATED = 1;
    [NativeLuaMemberAttribute]
    public const int bj_QUESTMESSAGE_COMPLETED = 2;
    [NativeLuaMemberAttribute]
    public const int bj_QUESTMESSAGE_FAILED = 3;
    [NativeLuaMemberAttribute]
    public const int bj_QUESTMESSAGE_REQUIREMENT = 4;
    [NativeLuaMemberAttribute]
    public const int bj_QUESTMESSAGE_MISSIONFAILED = 5;
    [NativeLuaMemberAttribute]
    public const int bj_QUESTMESSAGE_ALWAYSHINT = 6;
    [NativeLuaMemberAttribute]
    public const int bj_QUESTMESSAGE_HINT = 7;
    [NativeLuaMemberAttribute]
    public const int bj_QUESTMESSAGE_SECRET = 8;
    [NativeLuaMemberAttribute]
    public const int bj_QUESTMESSAGE_UNITACQUIRED = 9;
    [NativeLuaMemberAttribute]
    public const int bj_QUESTMESSAGE_UNITAVAILABLE = 10;
    [NativeLuaMemberAttribute]
    public const int bj_QUESTMESSAGE_ITEMACQUIRED = 11;
    [NativeLuaMemberAttribute]
    public const int bj_QUESTMESSAGE_WARNING = 12;
    [NativeLuaMemberAttribute]
    public const int bj_SORTTYPE_SORTBYVALUE = 0;
    [NativeLuaMemberAttribute]
    public const int bj_SORTTYPE_SORTBYPLAYER = 1;
    [NativeLuaMemberAttribute]
    public const int bj_SORTTYPE_SORTBYLABEL = 2;
    [NativeLuaMemberAttribute]
    public const int bj_CINEFADETYPE_FADEIN = 0;
    [NativeLuaMemberAttribute]
    public const int bj_CINEFADETYPE_FADEOUT = 1;
    [NativeLuaMemberAttribute]
    public const int bj_CINEFADETYPE_FADEOUTIN = 2;
    [NativeLuaMemberAttribute]
    public const int bj_REMOVEBUFFS_POSITIVE = 0;
    [NativeLuaMemberAttribute]
    public const int bj_REMOVEBUFFS_NEGATIVE = 1;
    [NativeLuaMemberAttribute]
    public const int bj_REMOVEBUFFS_ALL = 2;
    [NativeLuaMemberAttribute]
    public const int bj_REMOVEBUFFS_NONTLIFE = 3;
    [NativeLuaMemberAttribute]
    public const int bj_BUFF_POLARITY_POSITIVE = 0;
    [NativeLuaMemberAttribute]
    public const int bj_BUFF_POLARITY_NEGATIVE = 1;
    [NativeLuaMemberAttribute]
    public const int bj_BUFF_POLARITY_EITHER = 2;
    [NativeLuaMemberAttribute]
    public const int bj_BUFF_RESIST_MAGIC = 0;
    [NativeLuaMemberAttribute]
    public const int bj_BUFF_RESIST_PHYSICAL = 1;
    [NativeLuaMemberAttribute]
    public const int bj_BUFF_RESIST_EITHER = 2;
    [NativeLuaMemberAttribute]
    public const int bj_BUFF_RESIST_BOTH = 3;
    [NativeLuaMemberAttribute]
    public const int bj_HEROSTAT_STR = 0;
    [NativeLuaMemberAttribute]
    public const int bj_HEROSTAT_AGI = 1;
    [NativeLuaMemberAttribute]
    public const int bj_HEROSTAT_INT = 2;
    [NativeLuaMemberAttribute]
    public const int bj_MODIFYMETHOD_ADD = 0;
    [NativeLuaMemberAttribute]
    public const int bj_MODIFYMETHOD_SUB = 1;
    [NativeLuaMemberAttribute]
    public const int bj_MODIFYMETHOD_SET = 2;
    [NativeLuaMemberAttribute]
    public const int bj_UNIT_STATE_METHOD_ABSOLUTE = 0;
    [NativeLuaMemberAttribute]
    public const int bj_UNIT_STATE_METHOD_RELATIVE = 1;
    [NativeLuaMemberAttribute]
    public const int bj_UNIT_STATE_METHOD_DEFAULTS = 2;
    [NativeLuaMemberAttribute]
    public const int bj_UNIT_STATE_METHOD_MAXIMUM = 3;
    [NativeLuaMemberAttribute]
    public const int bj_GATEOPERATION_CLOSE = 0;
    [NativeLuaMemberAttribute]
    public const int bj_GATEOPERATION_OPEN = 1;
    [NativeLuaMemberAttribute]
    public const int bj_GATEOPERATION_DESTROY = 2;
    [NativeLuaMemberAttribute]
    public const int bj_GAMECACHE_BOOLEAN = 0;
    [NativeLuaMemberAttribute]
    public const int bj_GAMECACHE_INTEGER = 1;
    [NativeLuaMemberAttribute]
    public const int bj_GAMECACHE_REAL = 2;
    [NativeLuaMemberAttribute]
    public const int bj_GAMECACHE_UNIT = 3;
    [NativeLuaMemberAttribute]
    public const int bj_GAMECACHE_STRING = 4;
    [NativeLuaMemberAttribute]
    public const int bj_HASHTABLE_BOOLEAN = 0;
    [NativeLuaMemberAttribute]
    public const int bj_HASHTABLE_INTEGER = 1;
    [NativeLuaMemberAttribute]
    public const int bj_HASHTABLE_REAL = 2;
    [NativeLuaMemberAttribute]
    public const int bj_HASHTABLE_STRING = 3;
    [NativeLuaMemberAttribute]
    public const int bj_HASHTABLE_HANDLE = 4;
    [NativeLuaMemberAttribute]
    public const int bj_ITEM_STATUS_HIDDEN = 0;
    [NativeLuaMemberAttribute]
    public const int bj_ITEM_STATUS_OWNED = 1;
    [NativeLuaMemberAttribute]
    public const int bj_ITEM_STATUS_INVULNERABLE = 2;
    [NativeLuaMemberAttribute]
    public const int bj_ITEM_STATUS_POWERUP = 3;
    [NativeLuaMemberAttribute]
    public const int bj_ITEM_STATUS_SELLABLE = 4;
    [NativeLuaMemberAttribute]
    public const int bj_ITEM_STATUS_PAWNABLE = 5;
    [NativeLuaMemberAttribute]
    public const int bj_ITEMCODE_STATUS_POWERUP = 0;
    [NativeLuaMemberAttribute]
    public const int bj_ITEMCODE_STATUS_SELLABLE = 1;
    [NativeLuaMemberAttribute]
    public const int bj_ITEMCODE_STATUS_PAWNABLE = 2;
    [NativeLuaMemberAttribute]
    public const int bj_MINIMAPPINGSTYLE_SIMPLE = 0;
    [NativeLuaMemberAttribute]
    public const int bj_MINIMAPPINGSTYLE_FLASHY = 1;
    [NativeLuaMemberAttribute]
    public const int bj_MINIMAPPINGSTYLE_ATTACK = 2;
    [NativeLuaMemberAttribute]
    public const float bj_CORPSE_MAX_DEATH_TIME = 8.00f;
    [NativeLuaMemberAttribute]
    public const int bj_CORPSETYPE_FLESH = 0;
    [NativeLuaMemberAttribute]
    public const int bj_CORPSETYPE_BONE = 1;
    [NativeLuaMemberAttribute]
    public const int bj_ELEVATOR_BLOCKER_CODE = 1146381680;
    [NativeLuaMemberAttribute]
    public const int bj_ELEVATOR_CODE01 = 1146384998;
    [NativeLuaMemberAttribute]
    public const int bj_ELEVATOR_CODE02 = 1146385016;
    [NativeLuaMemberAttribute]
    public const int bj_ELEVATOR_WALL_TYPE_ALL = 0;
    [NativeLuaMemberAttribute]
    public const int bj_ELEVATOR_WALL_TYPE_EAST = 1;
    [NativeLuaMemberAttribute]
    public const int bj_ELEVATOR_WALL_TYPE_NORTH = 2;
    [NativeLuaMemberAttribute]
    public const int bj_ELEVATOR_WALL_TYPE_SOUTH = 3;
    [NativeLuaMemberAttribute]
    public const int bj_ELEVATOR_WALL_TYPE_WEST = 4;
    [NativeLuaMemberAttribute]
    public static force bj_FORCE_ALL_PLAYERS = null;
    [NativeLuaMemberAttribute]
    public static force[] bj_FORCE_PLAYER = new force[JASS_MAX_ARRAY_SIZE];
    [NativeLuaMemberAttribute]
    public static int bj_MELEE_MAX_TWINKED_HEROES = 0;
    [NativeLuaMemberAttribute]
    public static rect bj_mapInitialPlayableArea = null;
    [NativeLuaMemberAttribute]
    public static rect bj_mapInitialCameraBounds = null;
    [NativeLuaMemberAttribute]
    public static int bj_forLoopAIndex = 0;
    [NativeLuaMemberAttribute]
    public static int bj_forLoopBIndex = 0;
    [NativeLuaMemberAttribute]
    public static int bj_forLoopAIndexEnd = 0;
    [NativeLuaMemberAttribute]
    public static int bj_forLoopBIndexEnd = 0;
    [NativeLuaMemberAttribute]
    public static bool bj_slotControlReady = false;
    [NativeLuaMemberAttribute]
    public static bool[] bj_slotControlUsed = new bool[JASS_MAX_ARRAY_SIZE];
    [NativeLuaMemberAttribute]
    public static mapcontrol[] bj_slotControl = new mapcontrol[JASS_MAX_ARRAY_SIZE];
    [NativeLuaMemberAttribute]
    public static timer bj_gameStartedTimer = null;
    [NativeLuaMemberAttribute]
    public static bool bj_gameStarted = false;
    [NativeLuaMemberAttribute]
    public static timer bj_volumeGroupsTimer = CreateTimer();
    [NativeLuaMemberAttribute]
    public static bool bj_isSinglePlayer = false;
    [NativeLuaMemberAttribute]
    public static trigger bj_dncSoundsDay = null;
    [NativeLuaMemberAttribute]
    public static trigger bj_dncSoundsNight = null;
    [NativeLuaMemberAttribute]
    public static sound bj_dayAmbientSound = null;
    [NativeLuaMemberAttribute]
    public static sound bj_nightAmbientSound = null;
    [NativeLuaMemberAttribute]
    public static trigger bj_dncSoundsDawn = null;
    [NativeLuaMemberAttribute]
    public static trigger bj_dncSoundsDusk = null;
    [NativeLuaMemberAttribute]
    public static sound bj_dawnSound = null;
    [NativeLuaMemberAttribute]
    public static sound bj_duskSound = null;
    [NativeLuaMemberAttribute]
    public static bool bj_useDawnDuskSounds = true;
    [NativeLuaMemberAttribute]
    public static bool bj_dncIsDaytime = false;
    [NativeLuaMemberAttribute]
    public static sound bj_rescueSound = null;
    [NativeLuaMemberAttribute]
    public static sound bj_questDiscoveredSound = null;
    [NativeLuaMemberAttribute]
    public static sound bj_questUpdatedSound = null;
    [NativeLuaMemberAttribute]
    public static sound bj_questCompletedSound = null;
    [NativeLuaMemberAttribute]
    public static sound bj_questFailedSound = null;
    [NativeLuaMemberAttribute]
    public static sound bj_questHintSound = null;
    [NativeLuaMemberAttribute]
    public static sound bj_questSecretSound = null;
    [NativeLuaMemberAttribute]
    public static sound bj_questItemAcquiredSound = null;
    [NativeLuaMemberAttribute]
    public static sound bj_questWarningSound = null;
    [NativeLuaMemberAttribute]
    public static sound bj_victoryDialogSound = null;
    [NativeLuaMemberAttribute]
    public static sound bj_defeatDialogSound = null;
    [NativeLuaMemberAttribute]
    public static trigger bj_stockItemPurchased = null;
    [NativeLuaMemberAttribute]
    public static timer bj_stockUpdateTimer = null;
    [NativeLuaMemberAttribute]
    public static bool[] bj_stockAllowedPermanent = new bool[JASS_MAX_ARRAY_SIZE];
    [NativeLuaMemberAttribute]
    public static bool[] bj_stockAllowedCharged = new bool[JASS_MAX_ARRAY_SIZE];
    [NativeLuaMemberAttribute]
    public static bool[] bj_stockAllowedArtifact = new bool[JASS_MAX_ARRAY_SIZE];
    [NativeLuaMemberAttribute]
    public static int bj_stockPickedItemLevel = 0;
    [NativeLuaMemberAttribute]
    public static itemtype bj_stockPickedItemType = default;
    [NativeLuaMemberAttribute]
    public static trigger bj_meleeVisibilityTrained = null;
    [NativeLuaMemberAttribute]
    public static bool bj_meleeVisibilityIsDay = true;
    [NativeLuaMemberAttribute]
    public static bool bj_meleeGrantHeroItems = false;
    [NativeLuaMemberAttribute]
    public static location bj_meleeNearestMineToLoc = null;
    [NativeLuaMemberAttribute]
    public static unit bj_meleeNearestMine = null;
    [NativeLuaMemberAttribute]
    public static float bj_meleeNearestMineDist = 0.00f;
    [NativeLuaMemberAttribute]
    public static bool bj_meleeGameOver = false;
    [NativeLuaMemberAttribute]
    public static bool[] bj_meleeDefeated = new bool[JASS_MAX_ARRAY_SIZE];
    [NativeLuaMemberAttribute]
    public static bool[] bj_meleeVictoried = new bool[JASS_MAX_ARRAY_SIZE];
    [NativeLuaMemberAttribute]
    public static unit[] bj_ghoul = new unit[JASS_MAX_ARRAY_SIZE];
    [NativeLuaMemberAttribute]
    public static timer[] bj_crippledTimer = new timer[JASS_MAX_ARRAY_SIZE];
    [NativeLuaMemberAttribute]
    public static timerdialog[] bj_crippledTimerWindows = new timerdialog[JASS_MAX_ARRAY_SIZE];
    [NativeLuaMemberAttribute]
    public static bool[] bj_playerIsCrippled = new bool[JASS_MAX_ARRAY_SIZE];
    [NativeLuaMemberAttribute]
    public static bool[] bj_playerIsExposed = new bool[JASS_MAX_ARRAY_SIZE];
    [NativeLuaMemberAttribute]
    public static bool bj_finishSoonAllExposed = false;
    [NativeLuaMemberAttribute]
    public static timerdialog bj_finishSoonTimerDialog = null;
    [NativeLuaMemberAttribute]
    public static int[] bj_meleeTwinkedHeroes = new int[JASS_MAX_ARRAY_SIZE];
    [NativeLuaMemberAttribute]
    public static trigger bj_rescueUnitBehavior = null;
    [NativeLuaMemberAttribute]
    public static bool bj_rescueChangeColorUnit = true;
    [NativeLuaMemberAttribute]
    public static bool bj_rescueChangeColorBldg = true;
    [NativeLuaMemberAttribute]
    public static timer bj_cineSceneEndingTimer = null;
    [NativeLuaMemberAttribute]
    public static sound bj_cineSceneLastSound = null;
    [NativeLuaMemberAttribute]
    public static trigger bj_cineSceneBeingSkipped = null;
    [NativeLuaMemberAttribute]
    public static gamespeed bj_cineModePriorSpeed = MAP_SPEED_NORMAL;
    [NativeLuaMemberAttribute]
    public static bool bj_cineModePriorFogSetting = false;
    [NativeLuaMemberAttribute]
    public static bool bj_cineModePriorMaskSetting = false;
    [NativeLuaMemberAttribute]
    public static bool bj_cineModeAlreadyIn = false;
    [NativeLuaMemberAttribute]
    public static bool bj_cineModePriorDawnDusk = false;
    [NativeLuaMemberAttribute]
    public static int bj_cineModeSavedSeed = 0;
    [NativeLuaMemberAttribute]
    public static timer bj_cineFadeFinishTimer = null;
    [NativeLuaMemberAttribute]
    public static timer bj_cineFadeContinueTimer = null;
    [NativeLuaMemberAttribute]
    public static float bj_cineFadeContinueRed = 0;
    [NativeLuaMemberAttribute]
    public static float bj_cineFadeContinueGreen = 0;
    [NativeLuaMemberAttribute]
    public static float bj_cineFadeContinueBlue = 0;
    [NativeLuaMemberAttribute]
    public static float bj_cineFadeContinueTrans = 0;
    [NativeLuaMemberAttribute]
    public static float bj_cineFadeContinueDuration = 0;
    [NativeLuaMemberAttribute]
    public static string bj_cineFadeContinueTex = string.Empty;
    [NativeLuaMemberAttribute]
    public static int bj_queuedExecTotal = 0;
    [NativeLuaMemberAttribute]
    public static trigger[] bj_queuedExecTriggers = new trigger[JASS_MAX_ARRAY_SIZE];
    [NativeLuaMemberAttribute]
    public static bool[] bj_queuedExecUseConds = new bool[JASS_MAX_ARRAY_SIZE];
    [NativeLuaMemberAttribute]
    public static timer bj_queuedExecTimeoutTimer = CreateTimer();
    [NativeLuaMemberAttribute]
    public static trigger bj_queuedExecTimeout = null;
    [NativeLuaMemberAttribute]
    public static int bj_destInRegionDiesCount = 0;
    [NativeLuaMemberAttribute]
    public static trigger bj_destInRegionDiesTrig = null;
    [NativeLuaMemberAttribute]
    public static int bj_groupCountUnits = 0;
    [NativeLuaMemberAttribute]
    public static int bj_forceCountPlayers = 0;
    [NativeLuaMemberAttribute]
    public static int bj_groupEnumTypeId = 0;
    [NativeLuaMemberAttribute]
    public static player bj_groupEnumOwningPlayer = null;
    [NativeLuaMemberAttribute]
    public static group bj_groupAddGroupDest = null;
    [NativeLuaMemberAttribute]
    public static group bj_groupRemoveGroupDest = null;
    [NativeLuaMemberAttribute]
    public static int bj_groupRandomConsidered = 0;
    [NativeLuaMemberAttribute]
    public static unit bj_groupRandomCurrentPick = null;
    [NativeLuaMemberAttribute]
    public static group bj_groupLastCreatedDest = null;
    [NativeLuaMemberAttribute]
    public static group bj_randomSubGroupGroup = null;
    [NativeLuaMemberAttribute]
    public static int bj_randomSubGroupWant = 0;
    [NativeLuaMemberAttribute]
    public static int bj_randomSubGroupTotal = 0;
    [NativeLuaMemberAttribute]
    public static float bj_randomSubGroupChance = 0;
    [NativeLuaMemberAttribute]
    public static int bj_destRandomConsidered = 0;
    [NativeLuaMemberAttribute]
    public static destructable bj_destRandomCurrentPick = null;
    [NativeLuaMemberAttribute]
    public static destructable bj_elevatorWallBlocker = null;
    [NativeLuaMemberAttribute]
    public static destructable bj_elevatorNeighbor = null;
    [NativeLuaMemberAttribute]
    public static int bj_itemRandomConsidered = 0;
    [NativeLuaMemberAttribute]
    public static item bj_itemRandomCurrentPick = null;
    [NativeLuaMemberAttribute]
    public static int bj_forceRandomConsidered = 0;
    [NativeLuaMemberAttribute]
    public static player bj_forceRandomCurrentPick = null;
    [NativeLuaMemberAttribute]
    public static unit bj_makeUnitRescuableUnit = null;
    [NativeLuaMemberAttribute]
    public static bool bj_makeUnitRescuableFlag = true;
    [NativeLuaMemberAttribute]
    public static bool bj_pauseAllUnitsFlag = true;
    [NativeLuaMemberAttribute]
    public static location bj_enumDestructableCenter = null;
    [NativeLuaMemberAttribute]
    public static float bj_enumDestructableRadius = 0;
    [NativeLuaMemberAttribute]
    public static playercolor bj_setPlayerTargetColor = null;
    [NativeLuaMemberAttribute]
    public static bool bj_isUnitGroupDeadResult = true;
    [NativeLuaMemberAttribute]
    public static bool bj_isUnitGroupEmptyResult = true;
    [NativeLuaMemberAttribute]
    public static bool bj_isUnitGroupInRectResult = true;
    [NativeLuaMemberAttribute]
    public static rect bj_isUnitGroupInRectRect = null;
    [NativeLuaMemberAttribute]
    public static bool bj_changeLevelShowScores = false;
    [NativeLuaMemberAttribute]
    public static string bj_changeLevelMapName = null;
    [NativeLuaMemberAttribute]
    public static group bj_suspendDecayFleshGroup = CreateGroup();
    [NativeLuaMemberAttribute]
    public static group bj_suspendDecayBoneGroup = CreateGroup();
    [NativeLuaMemberAttribute]
    public static timer bj_delayedSuspendDecayTimer = CreateTimer();
    [NativeLuaMemberAttribute]
    public static trigger bj_delayedSuspendDecayTrig = null;
    [NativeLuaMemberAttribute]
    public static int bj_livingPlayerUnitsTypeId = 0;
    [NativeLuaMemberAttribute]
    public static widget bj_lastDyingWidget = null;
    [NativeLuaMemberAttribute]
    public static int bj_randDistCount = 0;
    [NativeLuaMemberAttribute]
    public static int[] bj_randDistID = new int[JASS_MAX_ARRAY_SIZE];
    [NativeLuaMemberAttribute]
    public static int[] bj_randDistChance = new int[JASS_MAX_ARRAY_SIZE];
    [NativeLuaMemberAttribute]
    public static unit bj_lastCreatedUnit = null;
    [NativeLuaMemberAttribute]
    public static item bj_lastCreatedItem = null;
    [NativeLuaMemberAttribute]
    public static item bj_lastRemovedItem = null;
    [NativeLuaMemberAttribute]
    public static unit bj_lastHauntedGoldMine = null;
    [NativeLuaMemberAttribute]
    public static destructable bj_lastCreatedDestructable = null;
    [NativeLuaMemberAttribute]
    public static group bj_lastCreatedGroup = CreateGroup();
    [NativeLuaMemberAttribute]
    public static fogmodifier bj_lastCreatedFogModifier = null;
    [NativeLuaMemberAttribute]
    public static effect bj_lastCreatedEffect = null;
    [NativeLuaMemberAttribute]
    public static weathereffect bj_lastCreatedWeatherEffect = null;
    [NativeLuaMemberAttribute]
    public static terraindeformation bj_lastCreatedTerrainDeformation = null;
    [NativeLuaMemberAttribute]
    public static quest bj_lastCreatedQuest = null;
    [NativeLuaMemberAttribute]
    public static questitem bj_lastCreatedQuestItem = null;
    [NativeLuaMemberAttribute]
    public static defeatcondition bj_lastCreatedDefeatCondition = null;
    [NativeLuaMemberAttribute]
    public static timer bj_lastStartedTimer = CreateTimer();
    [NativeLuaMemberAttribute]
    public static timerdialog bj_lastCreatedTimerDialog = null;
    [NativeLuaMemberAttribute]
    public static leaderboard bj_lastCreatedLeaderboard = null;
    [NativeLuaMemberAttribute]
    public static multiboard bj_lastCreatedMultiboard = null;
    [NativeLuaMemberAttribute]
    public static sound bj_lastPlayedSound = null;
    [NativeLuaMemberAttribute]
    public static string bj_lastPlayedMusic = string.Empty;
    [NativeLuaMemberAttribute]
    public static float bj_lastTransmissionDuration = 0;
    [NativeLuaMemberAttribute]
    public static gamecache bj_lastCreatedGameCache = null;
    [NativeLuaMemberAttribute]
    public static hashtable bj_lastCreatedHashtable = null;
    [NativeLuaMemberAttribute]
    public static unit bj_lastLoadedUnit = null;
    [NativeLuaMemberAttribute]
    public static button bj_lastCreatedButton = null;
    [NativeLuaMemberAttribute]
    public static unit bj_lastReplacedUnit = null;
    [NativeLuaMemberAttribute]
    public static texttag bj_lastCreatedTextTag = null;
    [NativeLuaMemberAttribute]
    public static lightning bj_lastCreatedLightning = null;
    [NativeLuaMemberAttribute]
    public static image bj_lastCreatedImage = null;
    [NativeLuaMemberAttribute]
    public static ubersplat bj_lastCreatedUbersplat = null;
    [NativeLuaMemberAttribute]
    public static boolexpr filterIssueHauntOrderAtLocBJ = null;
    [NativeLuaMemberAttribute]
    public static boolexpr filterEnumDestructablesInCircleBJ = null;
    [NativeLuaMemberAttribute]
    public static boolexpr filterGetUnitsInRectOfPlayer = null;
    [NativeLuaMemberAttribute]
    public static boolexpr filterGetUnitsOfTypeIdAll = null;
    [NativeLuaMemberAttribute]
    public static boolexpr filterGetUnitsOfPlayerAndTypeId = null;
    [NativeLuaMemberAttribute]
    public static boolexpr filterMeleeTrainedUnitIsHeroBJ = null;
    [NativeLuaMemberAttribute]
    public static boolexpr filterLivingPlayerUnitsOfTypeId = null;
    [NativeLuaMemberAttribute]
    public static bool bj_wantDestroyGroup = false;
    [NativeLuaMemberAttribute]
    public static bool bj_lastInstObjFuncSuccessful = true;
    [NativeLuaMemberAttribute]
    public static void BJDebugMsg(string msg) {
      int i = 0;
      while (true) {
        DisplayTimedTextToPlayer(Player(i), 0, 0, 60, msg);
        i = i + 1;
        if (i == bj_MAX_PLAYERS)
          break;
      }
    }

    [NativeLuaMemberAttribute]
    public static float RMinBJ(float a, float b) {
      if ((a < b)) {
        return a;
      } else {
        return b;
      }
    }

    [NativeLuaMemberAttribute]
    public static float RMaxBJ(float a, float b) {
      if ((a < b)) {
        return b;
      } else {
        return a;
      }
    }

    [NativeLuaMemberAttribute]
    public static float RAbsBJ(float a) {
      if ((a >= 0)) {
        return a;
      } else {
        return -a;
      }
    }

    [NativeLuaMemberAttribute]
    public static float RSignBJ(float a) {
      if ((a >= 0.0f)) {
        return 1.0f;
      } else {
        return -1.0f;
      }
    }

    [NativeLuaMemberAttribute]
    public static int IMinBJ(int a, int b) {
      if ((a < b)) {
        return a;
      } else {
        return b;
      }
    }

    [NativeLuaMemberAttribute]
    public static int IMaxBJ(int a, int b) {
      if ((a < b)) {
        return b;
      } else {
        return a;
      }
    }

    [NativeLuaMemberAttribute]
    public static int IAbsBJ(int a) {
      if ((a >= 0)) {
        return a;
      } else {
        return -a;
      }
    }

    [NativeLuaMemberAttribute]
    public static int ISignBJ(int a) {
      if ((a >= 0)) {
        return 1;
      } else {
        return -1;
      }
    }

    [NativeLuaMemberAttribute]
    public static float SinBJ(float degrees) {
      return Sin(degrees * bj_DEGTORAD);
    }

    [NativeLuaMemberAttribute]
    public static float CosBJ(float degrees) {
      return Cos(degrees * bj_DEGTORAD);
    }

    [NativeLuaMemberAttribute]
    public static float TanBJ(float degrees) {
      return Tan(degrees * bj_DEGTORAD);
    }

    [NativeLuaMemberAttribute]
    public static float AsinBJ(float degrees) {
      return Asin(degrees) * bj_RADTODEG;
    }

    [NativeLuaMemberAttribute]
    public static float AcosBJ(float degrees) {
      return Acos(degrees) * bj_RADTODEG;
    }

    [NativeLuaMemberAttribute]
    public static float AtanBJ(float degrees) {
      return Atan(degrees) * bj_RADTODEG;
    }

    [NativeLuaMemberAttribute]
    public static float Atan2BJ(float y, float x) {
      return Atan2(y, x) * bj_RADTODEG;
    }

    [NativeLuaMemberAttribute]
    public static float AngleBetweenPoints(location locA, location locB) {
      return bj_RADTODEG * Atan2(GetLocationY(locB) - GetLocationY(locA), GetLocationX(locB) - GetLocationX(locA));
    }

    [NativeLuaMemberAttribute]
    public static float DistanceBetweenPoints(location locA, location locB) {
      float dx = GetLocationX(locB) - GetLocationX(locA);
      float dy = GetLocationY(locB) - GetLocationY(locA);
      return SquareRoot(dx * dx + dy * dy);
    }

    [NativeLuaMemberAttribute]
    public static location PolarProjectionBJ(location source, float dist, float angle) {
      float x = GetLocationX(source) + dist * Cos(angle * bj_DEGTORAD);
      float y = GetLocationY(source) + dist * Sin(angle * bj_DEGTORAD);
      return Location(x, y);
    }

    [NativeLuaMemberAttribute]
    public static float GetRandomDirectionDeg() {
      return GetRandomReal(0, 360);
    }

    [NativeLuaMemberAttribute]
    public static float GetRandomPercentageBJ() {
      return GetRandomReal(0, 100);
    }

    [NativeLuaMemberAttribute]
    public static location GetRandomLocInRect(rect whichRect) {
      return Location(GetRandomReal(GetRectMinX(whichRect), GetRectMaxX(whichRect)), GetRandomReal(GetRectMinY(whichRect), GetRectMaxY(whichRect)));
    }

    [NativeLuaMemberAttribute]
    public static int ModuloInteger(int dividend, int divisor) {
      int modulus = dividend - (dividend / divisor) * divisor;
      if ((modulus < 0)) {
        modulus = modulus + divisor;
      }

      return modulus;
    }

    [NativeLuaMemberAttribute]
    public static float ModuloReal(float dividend, float divisor) {
      float modulus = dividend - I2R(R2I(dividend / divisor)) * divisor;
      if ((modulus < 0)) {
        modulus = modulus + divisor;
      }

      return modulus;
    }

    [NativeLuaMemberAttribute]
    public static location OffsetLocation(location loc, float dx, float dy) {
      return Location(GetLocationX(loc) + dx, GetLocationY(loc) + dy);
    }

    [NativeLuaMemberAttribute]
    public static rect OffsetRectBJ(rect r, float dx, float dy) {
      return Rect(GetRectMinX(r) + dx, GetRectMinY(r) + dy, GetRectMaxX(r) + dx, GetRectMaxY(r) + dy);
    }

    [NativeLuaMemberAttribute]
    public static rect RectFromCenterSizeBJ(location center, float width, float height) {
      float x = GetLocationX(center);
      float y = GetLocationY(center);
      return Rect(x - width * 0.5f, y - height * 0.5f, x + width * 0.5f, y + height * 0.5f);
    }

    [NativeLuaMemberAttribute]
    public static bool RectContainsCoords(rect r, float x, float y) {
      return (GetRectMinX(r) <= x) && (x <= GetRectMaxX(r)) && (GetRectMinY(r) <= y) && (y <= GetRectMaxY(r));
    }

    [NativeLuaMemberAttribute]
    public static bool RectContainsLoc(rect r, location loc) {
      return RectContainsCoords(r, GetLocationX(loc), GetLocationY(loc));
    }

    [NativeLuaMemberAttribute]
    public static bool RectContainsUnit(rect r, unit whichUnit) {
      return RectContainsCoords(r, GetUnitX(whichUnit), GetUnitY(whichUnit));
    }

    [NativeLuaMemberAttribute]
    public static bool RectContainsItem(item whichItem, rect r) {
      if ((whichItem == null)) {
        return false;
      }

      if ((IsItemOwned(whichItem))) {
        return false;
      }

      return RectContainsCoords(r, GetItemX(whichItem), GetItemY(whichItem));
    }

    [NativeLuaMemberAttribute]
    public static void ConditionalTriggerExecute(trigger trig) {
      if (TriggerEvaluate(trig)) {
        TriggerExecute(trig);
      }
    }

    [NativeLuaMemberAttribute]
    public static bool TriggerExecuteBJ(trigger trig, bool checkConditions) {
      if (checkConditions) {
        if (!(TriggerEvaluate(trig))) {
          return false;
        }
      }

      TriggerExecute(trig);
      return true;
    }

    [NativeLuaMemberAttribute]
    public static bool PostTriggerExecuteBJ(trigger trig, bool checkConditions) {
      if (checkConditions) {
        if (!(TriggerEvaluate(trig))) {
          return false;
        }
      }

      TriggerRegisterTimerEvent(trig, 0, false);
      return true;
    }

    [NativeLuaMemberAttribute]
    public static void QueuedTriggerCheck() {
      string s = "TrigQueue Check ";
      int i = default;
      i = 0;
      while (true) {
        if (i >= bj_queuedExecTotal)
          break;
        s = s + "q[" + I2S(i) + "]=";
        if ((bj_queuedExecTriggers[i] == null)) {
          s = s + "null ";
        } else {
          s = s + "x ";
        }

        i = i + 1;
      }

      s = s + "(" + I2S(bj_queuedExecTotal) + " total)";
      DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, 600, s);
    }

    [NativeLuaMemberAttribute]
    public static int QueuedTriggerGetIndex(trigger trig) {
      int index = 0;
      while (true) {
        if (index >= bj_queuedExecTotal)
          break;
        if ((bj_queuedExecTriggers[index] == trig)) {
          return index;
        }

        index = index + 1;
      }

      return -1;
    }

    [NativeLuaMemberAttribute]
    public static bool QueuedTriggerRemoveByIndex(int trigIndex) {
      int index = default;
      if ((trigIndex >= bj_queuedExecTotal)) {
        return false;
      }

      bj_queuedExecTotal = bj_queuedExecTotal - 1;
      index = trigIndex;
      while (true) {
        if (index >= bj_queuedExecTotal)
          break;
        bj_queuedExecTriggers[index] = bj_queuedExecTriggers[index + 1];
        bj_queuedExecUseConds[index] = bj_queuedExecUseConds[index + 1];
        index = index + 1;
      }

      return true;
    }

    [NativeLuaMemberAttribute]
    public static bool QueuedTriggerAttemptExec() {
      while (true) {
        if (bj_queuedExecTotal == 0)
          break;
        if (TriggerExecuteBJ(bj_queuedExecTriggers[0], bj_queuedExecUseConds[0])) {
          TimerStart(bj_queuedExecTimeoutTimer, bj_QUEUED_TRIGGER_TIMEOUT, false, null);
          return true;
        }

        QueuedTriggerRemoveByIndex(0);
      }

      return false;
    }

    [NativeLuaMemberAttribute]
    public static bool QueuedTriggerAddBJ(trigger trig, bool checkConditions) {
      if ((bj_queuedExecTotal >= bj_MAX_QUEUED_TRIGGERS)) {
        return false;
      }

      bj_queuedExecTriggers[bj_queuedExecTotal] = trig;
      bj_queuedExecUseConds[bj_queuedExecTotal] = checkConditions;
      bj_queuedExecTotal = bj_queuedExecTotal + 1;
      if ((bj_queuedExecTotal == 1)) {
        QueuedTriggerAttemptExec();
      }

      return true;
    }

    [NativeLuaMemberAttribute]
    public static void QueuedTriggerRemoveBJ(trigger trig) {
      int index = default;
      int trigIndex = default;
      bool trigExecuted = default;
      trigIndex = QueuedTriggerGetIndex(trig);
      if ((trigIndex == -1)) {
        return;
      }

      QueuedTriggerRemoveByIndex(trigIndex);
      if ((trigIndex == 0)) {
        PauseTimer(bj_queuedExecTimeoutTimer);
        QueuedTriggerAttemptExec();
      }
    }

    [NativeLuaMemberAttribute]
    public static void QueuedTriggerDoneBJ() {
      int index = default;
      if ((bj_queuedExecTotal <= 0)) {
        return;
      }

      QueuedTriggerRemoveByIndex(0);
      PauseTimer(bj_queuedExecTimeoutTimer);
      QueuedTriggerAttemptExec();
    }

    [NativeLuaMemberAttribute]
    public static void QueuedTriggerClearBJ() {
      PauseTimer(bj_queuedExecTimeoutTimer);
      bj_queuedExecTotal = 0;
    }

    [NativeLuaMemberAttribute]
    public static void QueuedTriggerClearInactiveBJ() {
      bj_queuedExecTotal = IMinBJ(bj_queuedExecTotal, 1);
    }

    [NativeLuaMemberAttribute]
    public static int QueuedTriggerCountBJ() {
      return bj_queuedExecTotal;
    }

    [NativeLuaMemberAttribute]
    public static bool IsTriggerQueueEmptyBJ() {
      return bj_queuedExecTotal <= 0;
    }

    [NativeLuaMemberAttribute]
    public static bool IsTriggerQueuedBJ(trigger trig) {
      return QueuedTriggerGetIndex(trig) != -1;
    }

    [NativeLuaMemberAttribute]
    public static int GetForLoopIndexA() {
      return bj_forLoopAIndex;
    }

    [NativeLuaMemberAttribute]
    public static void SetForLoopIndexA(int newIndex) {
      bj_forLoopAIndex = newIndex;
    }

    [NativeLuaMemberAttribute]
    public static int GetForLoopIndexB() {
      return bj_forLoopBIndex;
    }

    [NativeLuaMemberAttribute]
    public static void SetForLoopIndexB(int newIndex) {
      bj_forLoopBIndex = newIndex;
    }

    [NativeLuaMemberAttribute]
    public static void PolledWait(float duration) {
      timer t = default;
      float timeRemaining = default;
      if ((duration > 0)) {
        t = CreateTimer();
        TimerStart(t, duration, false, null);
        while (true) {
          timeRemaining = TimerGetRemaining(t);
          if (timeRemaining <= 0)
            break;
          if ((timeRemaining > bj_POLLED_WAIT_SKIP_THRESHOLD)) {
            TriggerSleepAction(0.1f * timeRemaining);
          } else {
            TriggerSleepAction(bj_POLLED_WAIT_INTERVAL);
          }
        }

        DestroyTimer(t);
      }
    }

    [NativeLuaMemberAttribute]
    public static int IntegerTertiaryOp(bool flag, int valueA, int valueB) {
      if (flag) {
        return valueA;
      } else {
        return valueB;
      }
    }

    [NativeLuaMemberAttribute]
    public static void DoNothing() {
    }

    [NativeLuaMemberAttribute]
    public static void CommentString(string commentString) {
    }

    [NativeLuaMemberAttribute]
    public static string StringIdentity(string theString) {
      return GetLocalizedString(theString);
    }

    [NativeLuaMemberAttribute]
    public static bool GetBooleanAnd(bool valueA, bool valueB) {
      return valueA && valueB;
    }

    [NativeLuaMemberAttribute]
    public static bool GetBooleanOr(bool valueA, bool valueB) {
      return valueA || valueB;
    }

    [NativeLuaMemberAttribute]
    public static int PercentToInt(float percentage, int max) {
      int result = R2I(percentage * I2R(max) * 0.01f);
      if ((result < 0)) {
        result = 0;
      } else if ((result > max)) {
        result = max;
      }

      return result;
    }

    [NativeLuaMemberAttribute]
    public static int PercentTo255(float percentage) {
      return PercentToInt(percentage, 255);
    }

    [NativeLuaMemberAttribute]
    public static float GetTimeOfDay() {
      return GetFloatGameState(GAME_STATE_TIME_OF_DAY);
    }

    [NativeLuaMemberAttribute]
    public static void SetTimeOfDay(float whatTime) {
      SetFloatGameState(GAME_STATE_TIME_OF_DAY, whatTime);
    }

    [NativeLuaMemberAttribute]
    public static void SetTimeOfDayScalePercentBJ(float scalePercent) {
      SetTimeOfDayScale(scalePercent * 0.01f);
    }

    [NativeLuaMemberAttribute]
    public static float GetTimeOfDayScalePercentBJ() {
      return GetTimeOfDayScale() * 100;
    }

    [NativeLuaMemberAttribute]
    public static void PlaySound(string soundName) {
      sound soundHandle = CreateSound(soundName, false, false, true, 12700, 12700, string.Empty);
      StartSound(soundHandle);
      KillSoundWhenDone(soundHandle);
    }

    [NativeLuaMemberAttribute]
    public static bool CompareLocationsBJ(location A, location B) {
      return GetLocationX(A) == GetLocationX(B) && GetLocationY(A) == GetLocationY(B);
    }

    [NativeLuaMemberAttribute]
    public static bool CompareRectsBJ(rect A, rect B) {
      return GetRectMinX(A) == GetRectMinX(B) && GetRectMinY(A) == GetRectMinY(B) && GetRectMaxX(A) == GetRectMaxX(B) && GetRectMaxY(A) == GetRectMaxY(B);
    }

    [NativeLuaMemberAttribute]
    public static rect GetRectFromCircleBJ(location center, float radius) {
      float centerX = GetLocationX(center);
      float centerY = GetLocationY(center);
      return Rect(centerX - radius, centerY - radius, centerX + radius, centerY + radius);
    }

    [NativeLuaMemberAttribute]
    public static camerasetup GetCurrentCameraSetup() {
      camerasetup theCam = CreateCameraSetup();
      float duration = 0;
      CameraSetupSetField(theCam, CAMERA_FIELD_TARGET_DISTANCE, GetCameraField(CAMERA_FIELD_TARGET_DISTANCE), duration);
      CameraSetupSetField(theCam, CAMERA_FIELD_FARZ, GetCameraField(CAMERA_FIELD_FARZ), duration);
      CameraSetupSetField(theCam, CAMERA_FIELD_ZOFFSET, GetCameraField(CAMERA_FIELD_ZOFFSET), duration);
      CameraSetupSetField(theCam, CAMERA_FIELD_ANGLE_OF_ATTACK, bj_RADTODEG * GetCameraField(CAMERA_FIELD_ANGLE_OF_ATTACK), duration);
      CameraSetupSetField(theCam, CAMERA_FIELD_FIELD_OF_VIEW, bj_RADTODEG * GetCameraField(CAMERA_FIELD_FIELD_OF_VIEW), duration);
      CameraSetupSetField(theCam, CAMERA_FIELD_ROLL, bj_RADTODEG * GetCameraField(CAMERA_FIELD_ROLL), duration);
      CameraSetupSetField(theCam, CAMERA_FIELD_ROTATION, bj_RADTODEG * GetCameraField(CAMERA_FIELD_ROTATION), duration);
      CameraSetupSetField(theCam, CAMERA_FIELD_LOCAL_PITCH, bj_RADTODEG * GetCameraField(CAMERA_FIELD_LOCAL_PITCH), duration);
      CameraSetupSetField(theCam, CAMERA_FIELD_LOCAL_YAW, bj_RADTODEG * GetCameraField(CAMERA_FIELD_LOCAL_YAW), duration);
      CameraSetupSetField(theCam, CAMERA_FIELD_LOCAL_ROLL, bj_RADTODEG * GetCameraField(CAMERA_FIELD_LOCAL_ROLL), duration);
      CameraSetupSetDestPosition(theCam, GetCameraTargetPositionX(), GetCameraTargetPositionY(), duration);
      return theCam;
    }

    [NativeLuaMemberAttribute]
    public static void CameraSetupApplyForPlayer(bool doPan, camerasetup whichSetup, player whichPlayer, float duration) {
      if ((GetLocalPlayer() == whichPlayer)) {
        CameraSetupApplyForceDuration(whichSetup, doPan, duration);
      }
    }

    [NativeLuaMemberAttribute]
    public static float CameraSetupGetFieldSwap(camerafield whichField, camerasetup whichSetup) {
      return CameraSetupGetField(whichSetup, whichField);
    }

    [NativeLuaMemberAttribute]
    public static void SetCameraFieldForPlayer(player whichPlayer, camerafield whichField, float value, float duration) {
      if ((GetLocalPlayer() == whichPlayer)) {
        SetCameraField(whichField, value, duration);
      }
    }

    [NativeLuaMemberAttribute]
    public static void SetCameraTargetControllerNoZForPlayer(player whichPlayer, unit whichUnit, float xoffset, float yoffset, bool inheritOrientation) {
      if ((GetLocalPlayer() == whichPlayer)) {
        SetCameraTargetController(whichUnit, xoffset, yoffset, inheritOrientation);
      }
    }

    [NativeLuaMemberAttribute]
    public static void SetCameraPositionForPlayer(player whichPlayer, float x, float y) {
      if ((GetLocalPlayer() == whichPlayer)) {
        SetCameraPosition(x, y);
      }
    }

    [NativeLuaMemberAttribute]
    public static void SetCameraPositionLocForPlayer(player whichPlayer, location loc) {
      if ((GetLocalPlayer() == whichPlayer)) {
        SetCameraPosition(GetLocationX(loc), GetLocationY(loc));
      }
    }

    [NativeLuaMemberAttribute]
    public static void RotateCameraAroundLocBJ(float degrees, location loc, player whichPlayer, float duration) {
      if ((GetLocalPlayer() == whichPlayer)) {
        SetCameraRotateMode(GetLocationX(loc), GetLocationY(loc), bj_DEGTORAD * degrees, duration);
      }
    }

    [NativeLuaMemberAttribute]
    public static void PanCameraToForPlayer(player whichPlayer, float x, float y) {
      if ((GetLocalPlayer() == whichPlayer)) {
        PanCameraTo(x, y);
      }
    }

    [NativeLuaMemberAttribute]
    public static void PanCameraToLocForPlayer(player whichPlayer, location loc) {
      if ((GetLocalPlayer() == whichPlayer)) {
        PanCameraTo(GetLocationX(loc), GetLocationY(loc));
      }
    }

    [NativeLuaMemberAttribute]
    public static void PanCameraToTimedForPlayer(player whichPlayer, float x, float y, float duration) {
      if ((GetLocalPlayer() == whichPlayer)) {
        PanCameraToTimed(x, y, duration);
      }
    }

    [NativeLuaMemberAttribute]
    public static void PanCameraToTimedLocForPlayer(player whichPlayer, location loc, float duration) {
      if ((GetLocalPlayer() == whichPlayer)) {
        PanCameraToTimed(GetLocationX(loc), GetLocationY(loc), duration);
      }
    }

    [NativeLuaMemberAttribute]
    public static void PanCameraToTimedLocWithZForPlayer(player whichPlayer, location loc, float zOffset, float duration) {
      if ((GetLocalPlayer() == whichPlayer)) {
        PanCameraToTimedWithZ(GetLocationX(loc), GetLocationY(loc), zOffset, duration);
      }
    }

    [NativeLuaMemberAttribute]
    public static void SmartCameraPanBJ(player whichPlayer, location loc, float duration) {
      float dist = default;
      location cameraLoc = GetCameraTargetPositionLoc();
      if ((GetLocalPlayer() == whichPlayer)) {
        dist = DistanceBetweenPoints(loc, cameraLoc);
        if ((dist >= bj_SMARTPAN_TRESHOLD_SNAP)) {
          PanCameraToTimed(GetLocationX(loc), GetLocationY(loc), 0);
        } else if ((dist >= bj_SMARTPAN_TRESHOLD_PAN)) {
          PanCameraToTimed(GetLocationX(loc), GetLocationY(loc), duration);
        } else {
        }
      }

      RemoveLocation(cameraLoc);
    }

    [NativeLuaMemberAttribute]
    public static void SetCinematicCameraForPlayer(player whichPlayer, string cameraModelFile) {
      if ((GetLocalPlayer() == whichPlayer)) {
        SetCinematicCamera(cameraModelFile);
      }
    }

    [NativeLuaMemberAttribute]
    public static void ResetToGameCameraForPlayer(player whichPlayer, float duration) {
      if ((GetLocalPlayer() == whichPlayer)) {
        ResetToGameCamera(duration);
      }
    }

    [NativeLuaMemberAttribute]
    public static void CameraSetSourceNoiseForPlayer(player whichPlayer, float magnitude, float velocity) {
      if ((GetLocalPlayer() == whichPlayer)) {
        CameraSetSourceNoise(magnitude, velocity);
      }
    }

    [NativeLuaMemberAttribute]
    public static void CameraSetTargetNoiseForPlayer(player whichPlayer, float magnitude, float velocity) {
      if ((GetLocalPlayer() == whichPlayer)) {
        CameraSetTargetNoise(magnitude, velocity);
      }
    }

    [NativeLuaMemberAttribute]
    public static void CameraSetEQNoiseForPlayer(player whichPlayer, float magnitude) {
      float richter = magnitude;
      if ((richter > 5.0f)) {
        richter = 5.0f;
      }

      if ((richter < 2.0f)) {
        richter = 2.0f;
      }

      if ((GetLocalPlayer() == whichPlayer)) {
        CameraSetTargetNoiseEx(magnitude * 2.0f, magnitude * Pow(10, richter), true);
        CameraSetSourceNoiseEx(magnitude * 2.0f, magnitude * Pow(10, richter), true);
      }
    }

    [NativeLuaMemberAttribute]
    public static void CameraClearNoiseForPlayer(player whichPlayer) {
      if ((GetLocalPlayer() == whichPlayer)) {
        CameraSetSourceNoise(0, 0);
        CameraSetTargetNoise(0, 0);
      }
    }

    [NativeLuaMemberAttribute]
    public static rect GetCurrentCameraBoundsMapRectBJ() {
      return Rect(GetCameraBoundMinX(), GetCameraBoundMinY(), GetCameraBoundMaxX(), GetCameraBoundMaxY());
    }

    [NativeLuaMemberAttribute]
    public static rect GetCameraBoundsMapRect() {
      return bj_mapInitialCameraBounds;
    }

    [NativeLuaMemberAttribute]
    public static rect GetPlayableMapRect() {
      return bj_mapInitialPlayableArea;
    }

    [NativeLuaMemberAttribute]
    public static rect GetEntireMapRect() {
      return GetWorldBounds();
    }

    [NativeLuaMemberAttribute]
    public static void SetCameraBoundsToRect(rect r) {
      float minX = GetRectMinX(r);
      float minY = GetRectMinY(r);
      float maxX = GetRectMaxX(r);
      float maxY = GetRectMaxY(r);
      SetCameraBounds(minX, minY, minX, maxY, maxX, maxY, maxX, minY);
    }

    [NativeLuaMemberAttribute]
    public static void SetCameraBoundsToRectForPlayerBJ(player whichPlayer, rect r) {
      if ((GetLocalPlayer() == whichPlayer)) {
        SetCameraBoundsToRect(r);
      }
    }

    [NativeLuaMemberAttribute]
    public static void AdjustCameraBoundsBJ(int adjustMethod, float dxWest, float dxEast, float dyNorth, float dySouth) {
      float minX = 0;
      float minY = 0;
      float maxX = 0;
      float maxY = 0;
      float scale = 0;
      if ((adjustMethod == bj_CAMERABOUNDS_ADJUST_ADD)) {
        scale = 1;
      } else if ((adjustMethod == bj_CAMERABOUNDS_ADJUST_SUB)) {
        scale = -1;
      } else {
        return;
      }

      minX = GetCameraBoundMinX() - scale * dxWest;
      maxX = GetCameraBoundMaxX() + scale * dxEast;
      minY = GetCameraBoundMinY() - scale * dySouth;
      maxY = GetCameraBoundMaxY() + scale * dyNorth;
      if ((maxX < minX)) {
        minX = (minX + maxX) * 0.5f;
        maxX = minX;
      }

      if ((maxY < minY)) {
        minY = (minY + maxY) * 0.5f;
        maxY = minY;
      }

      SetCameraBounds(minX, minY, minX, maxY, maxX, maxY, maxX, minY);
    }

    [NativeLuaMemberAttribute]
    public static void AdjustCameraBoundsForPlayerBJ(int adjustMethod, player whichPlayer, float dxWest, float dxEast, float dyNorth, float dySouth) {
      if ((GetLocalPlayer() == whichPlayer)) {
        AdjustCameraBoundsBJ(adjustMethod, dxWest, dxEast, dyNorth, dySouth);
      }
    }

    [NativeLuaMemberAttribute]
    public static void SetCameraQuickPositionForPlayer(player whichPlayer, float x, float y) {
      if ((GetLocalPlayer() == whichPlayer)) {
        SetCameraQuickPosition(x, y);
      }
    }

    [NativeLuaMemberAttribute]
    public static void SetCameraQuickPositionLocForPlayer(player whichPlayer, location loc) {
      if ((GetLocalPlayer() == whichPlayer)) {
        SetCameraQuickPosition(GetLocationX(loc), GetLocationY(loc));
      }
    }

    [NativeLuaMemberAttribute]
    public static void SetCameraQuickPositionLoc(location loc) {
      SetCameraQuickPosition(GetLocationX(loc), GetLocationY(loc));
    }

    [NativeLuaMemberAttribute]
    public static void StopCameraForPlayerBJ(player whichPlayer) {
      if ((GetLocalPlayer() == whichPlayer)) {
        StopCamera();
      }
    }

    [NativeLuaMemberAttribute]
    public static void SetCameraOrientControllerForPlayerBJ(player whichPlayer, unit whichUnit, float xoffset, float yoffset) {
      if ((GetLocalPlayer() == whichPlayer)) {
        SetCameraOrientController(whichUnit, xoffset, yoffset);
      }
    }

    [NativeLuaMemberAttribute]
    public static void CameraSetSmoothingFactorBJ(float factor) {
      CameraSetSmoothingFactor(factor);
    }

    [NativeLuaMemberAttribute]
    public static void CameraResetSmoothingFactorBJ() {
      CameraSetSmoothingFactor(0);
    }

    [NativeLuaMemberAttribute]
    public static void DisplayTextToForce(force toForce, string message) {
      if ((IsPlayerInForce(GetLocalPlayer(), toForce))) {
        DisplayTextToPlayer(GetLocalPlayer(), 0, 0, message);
      }
    }

    [NativeLuaMemberAttribute]
    public static void DisplayTimedTextToForce(force toForce, float duration, string message) {
      if ((IsPlayerInForce(GetLocalPlayer(), toForce))) {
        DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, duration, message);
      }
    }

    [NativeLuaMemberAttribute]
    public static void ClearTextMessagesBJ(force toForce) {
      if ((IsPlayerInForce(GetLocalPlayer(), toForce))) {
        ClearTextMessages();
      }
    }

    [NativeLuaMemberAttribute]
    public static string SubStringBJ(string source, int start, int end) {
      return SubString(source, start - 1, end);
    }

    [NativeLuaMemberAttribute]
    public static int GetHandleIdBJ(object h) {
      return GetHandleId(h);
    }

    [NativeLuaMemberAttribute]
    public static int StringHashBJ(string s) {
      return StringHash(s);
    }

    [NativeLuaMemberAttribute]
    public static @event TriggerRegisterTimerEventPeriodic(trigger trig, float timeout) {
      return TriggerRegisterTimerEvent(trig, timeout, true);
    }

    [NativeLuaMemberAttribute]
    public static @event TriggerRegisterTimerEventSingle(trigger trig, float timeout) {
      return TriggerRegisterTimerEvent(trig, timeout, false);
    }

    [NativeLuaMemberAttribute]
    public static @event TriggerRegisterTimerExpireEventBJ(trigger trig, timer t) {
      return TriggerRegisterTimerExpireEvent(trig, t);
    }

    [NativeLuaMemberAttribute]
    public static @event TriggerRegisterPlayerUnitEventSimple(trigger trig, player whichPlayer, playerunitevent whichEvent) {
      return TriggerRegisterPlayerUnitEvent(trig, whichPlayer, whichEvent, null);
    }

    [NativeLuaMemberAttribute]
    public static void TriggerRegisterAnyUnitEventBJ(trigger trig, playerunitevent whichEvent) {
      int index = default;
      index = 0;
      while (true) {
        TriggerRegisterPlayerUnitEvent(trig, Player(index), whichEvent, null);
        index = index + 1;
        if (index == bj_MAX_PLAYER_SLOTS)
          break;
      }
    }

    [NativeLuaMemberAttribute]
    public static @event TriggerRegisterPlayerSelectionEventBJ(trigger trig, player whichPlayer, bool selected) {
      if (selected) {
        return TriggerRegisterPlayerUnitEvent(trig, whichPlayer, EVENT_PLAYER_UNIT_SELECTED, null);
      } else {
        return TriggerRegisterPlayerUnitEvent(trig, whichPlayer, EVENT_PLAYER_UNIT_DESELECTED, null);
      }
    }

    [NativeLuaMemberAttribute]
    public static @event TriggerRegisterPlayerKeyEventBJ(trigger trig, player whichPlayer, int keType, int keKey) {
      if ((keType == bj_KEYEVENTTYPE_DEPRESS)) {
        if ((keKey == bj_KEYEVENTKEY_LEFT)) {
          return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_ARROW_LEFT_DOWN);
        } else if ((keKey == bj_KEYEVENTKEY_RIGHT)) {
          return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_ARROW_RIGHT_DOWN);
        } else if ((keKey == bj_KEYEVENTKEY_DOWN)) {
          return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_ARROW_DOWN_DOWN);
        } else if ((keKey == bj_KEYEVENTKEY_UP)) {
          return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_ARROW_UP_DOWN);
        } else {
          return null;
        }
      } else if ((keType == bj_KEYEVENTTYPE_RELEASE)) {
        if ((keKey == bj_KEYEVENTKEY_LEFT)) {
          return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_ARROW_LEFT_UP);
        } else if ((keKey == bj_KEYEVENTKEY_RIGHT)) {
          return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_ARROW_RIGHT_UP);
        } else if ((keKey == bj_KEYEVENTKEY_DOWN)) {
          return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_ARROW_DOWN_UP);
        } else if ((keKey == bj_KEYEVENTKEY_UP)) {
          return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_ARROW_UP_UP);
        } else {
          return null;
        }
      } else {
        return null;
      }
    }

    [NativeLuaMemberAttribute]
    public static @event TriggerRegisterPlayerMouseEventBJ(trigger trig, player whichPlayer, int meType) {
      if ((meType == bj_MOUSEEVENTTYPE_DOWN)) {
        return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_MOUSE_DOWN);
      } else if ((meType == bj_MOUSEEVENTTYPE_UP)) {
        return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_MOUSE_UP);
      } else if ((meType == bj_MOUSEEVENTTYPE_MOVE)) {
        return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_MOUSE_MOVE);
      } else {
        return null;
      }
    }

    [NativeLuaMemberAttribute]
    public static @event TriggerRegisterPlayerEventVictory(trigger trig, player whichPlayer) {
      return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_VICTORY);
    }

    [NativeLuaMemberAttribute]
    public static @event TriggerRegisterPlayerEventDefeat(trigger trig, player whichPlayer) {
      return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_DEFEAT);
    }

    [NativeLuaMemberAttribute]
    public static @event TriggerRegisterPlayerEventLeave(trigger trig, player whichPlayer) {
      return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_LEAVE);
    }

    [NativeLuaMemberAttribute]
    public static @event TriggerRegisterPlayerEventAllianceChanged(trigger trig, player whichPlayer) {
      return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_ALLIANCE_CHANGED);
    }

    [NativeLuaMemberAttribute]
    public static @event TriggerRegisterPlayerEventEndCinematic(trigger trig, player whichPlayer) {
      return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_END_CINEMATIC);
    }

    [NativeLuaMemberAttribute]
    public static @event TriggerRegisterGameStateEventTimeOfDay(trigger trig, limitop opcode, float limitval) {
      return TriggerRegisterGameStateEvent(trig, GAME_STATE_TIME_OF_DAY, opcode, limitval);
    }

    [NativeLuaMemberAttribute]
    public static @event TriggerRegisterEnterRegionSimple(trigger trig, region whichRegion) {
      return TriggerRegisterEnterRegion(trig, whichRegion, null);
    }

    [NativeLuaMemberAttribute]
    public static @event TriggerRegisterLeaveRegionSimple(trigger trig, region whichRegion) {
      return TriggerRegisterLeaveRegion(trig, whichRegion, null);
    }

    [NativeLuaMemberAttribute]
    public static @event TriggerRegisterEnterRectSimple(trigger trig, rect r) {
      region rectRegion = CreateRegion();
      RegionAddRect(rectRegion, r);
      return TriggerRegisterEnterRegion(trig, rectRegion, null);
    }

    [NativeLuaMemberAttribute]
    public static @event TriggerRegisterLeaveRectSimple(trigger trig, rect r) {
      region rectRegion = CreateRegion();
      RegionAddRect(rectRegion, r);
      return TriggerRegisterLeaveRegion(trig, rectRegion, null);
    }

    [NativeLuaMemberAttribute]
    public static @event TriggerRegisterDistanceBetweenUnits(trigger trig, unit whichUnit, boolexpr condition, float range) {
      return TriggerRegisterUnitInRange(trig, whichUnit, range, condition);
    }

    [NativeLuaMemberAttribute]
    public static @event TriggerRegisterUnitInRangeSimple(trigger trig, float range, unit whichUnit) {
      return TriggerRegisterUnitInRange(trig, whichUnit, range, null);
    }

    [NativeLuaMemberAttribute]
    public static @event TriggerRegisterUnitLifeEvent(trigger trig, unit whichUnit, limitop opcode, float limitval) {
      return TriggerRegisterUnitStateEvent(trig, whichUnit, UNIT_STATE_LIFE, opcode, limitval);
    }

    [NativeLuaMemberAttribute]
    public static @event TriggerRegisterUnitManaEvent(trigger trig, unit whichUnit, limitop opcode, float limitval) {
      return TriggerRegisterUnitStateEvent(trig, whichUnit, UNIT_STATE_MANA, opcode, limitval);
    }

    [NativeLuaMemberAttribute]
    public static @event TriggerRegisterDialogEventBJ(trigger trig, dialog whichDialog) {
      return TriggerRegisterDialogEvent(trig, whichDialog);
    }

    [NativeLuaMemberAttribute]
    public static @event TriggerRegisterShowSkillEventBJ(trigger trig) {
      return TriggerRegisterGameEvent(trig, EVENT_GAME_SHOW_SKILL);
    }

    [NativeLuaMemberAttribute]
    public static @event TriggerRegisterBuildSubmenuEventBJ(trigger trig) {
      return TriggerRegisterGameEvent(trig, EVENT_GAME_BUILD_SUBMENU);
    }

    [NativeLuaMemberAttribute]
    public static @event TriggerRegisterGameLoadedEventBJ(trigger trig) {
      return TriggerRegisterGameEvent(trig, EVENT_GAME_LOADED);
    }

    [NativeLuaMemberAttribute]
    public static @event TriggerRegisterGameSavedEventBJ(trigger trig) {
      return TriggerRegisterGameEvent(trig, EVENT_GAME_SAVE);
    }

    [NativeLuaMemberAttribute]
    public static void RegisterDestDeathInRegionEnum() {
      bj_destInRegionDiesCount = bj_destInRegionDiesCount + 1;
      if ((bj_destInRegionDiesCount <= bj_MAX_DEST_IN_REGION_EVENTS)) {
        TriggerRegisterDeathEvent(bj_destInRegionDiesTrig, GetEnumDestructable());
      }
    }

    [NativeLuaMemberAttribute]
    public static void TriggerRegisterDestDeathInRegionEvent(trigger trig, rect r) {
      bj_destInRegionDiesTrig = trig;
      bj_destInRegionDiesCount = 0;
      EnumDestructablesInRect(r, null, RegisterDestDeathInRegionEnum);
    }

    [NativeLuaMemberAttribute]
    public static weathereffect AddWeatherEffectSaveLast(rect where, int effectID) {
      bj_lastCreatedWeatherEffect = AddWeatherEffect(where, effectID);
      return bj_lastCreatedWeatherEffect;
    }

    [NativeLuaMemberAttribute]
    public static weathereffect GetLastCreatedWeatherEffect() {
      return bj_lastCreatedWeatherEffect;
    }

    [NativeLuaMemberAttribute]
    public static void RemoveWeatherEffectBJ(weathereffect whichWeatherEffect) {
      RemoveWeatherEffect(whichWeatherEffect);
    }

    [NativeLuaMemberAttribute]
    public static terraindeformation TerrainDeformationCraterBJ(float duration, bool permanent, location where, float radius, float depth) {
      bj_lastCreatedTerrainDeformation = TerrainDeformCrater(GetLocationX(where), GetLocationY(where), radius, depth, R2I(duration * 1000), permanent);
      return bj_lastCreatedTerrainDeformation;
    }

    [NativeLuaMemberAttribute]
    public static terraindeformation TerrainDeformationRippleBJ(float duration, bool limitNeg, location where, float startRadius, float endRadius, float depth, float wavePeriod, float waveWidth) {
      float spaceWave = default;
      float timeWave = default;
      float radiusRatio = default;
      if ((endRadius <= 0 || waveWidth <= 0 || wavePeriod <= 0)) {
        return null;
      }

      timeWave = 2.0f * duration / wavePeriod;
      spaceWave = 2.0f * endRadius / waveWidth;
      radiusRatio = startRadius / endRadius;
      bj_lastCreatedTerrainDeformation = TerrainDeformRipple(GetLocationX(where), GetLocationY(where), endRadius, depth, R2I(duration * 1000), 1, spaceWave, timeWave, radiusRatio, limitNeg);
      return bj_lastCreatedTerrainDeformation;
    }

    [NativeLuaMemberAttribute]
    public static terraindeformation TerrainDeformationWaveBJ(float duration, location source, location target, float radius, float depth, float trailDelay) {
      float distance = default;
      float dirX = default;
      float dirY = default;
      float speed = default;
      distance = DistanceBetweenPoints(source, target);
      if ((distance == 0 || duration <= 0)) {
        return null;
      }

      dirX = (GetLocationX(target) - GetLocationX(source)) / distance;
      dirY = (GetLocationY(target) - GetLocationY(source)) / distance;
      speed = distance / duration;
      bj_lastCreatedTerrainDeformation = TerrainDeformWave(GetLocationX(source), GetLocationY(source), dirX, dirY, distance, speed, radius, depth, R2I(trailDelay * 1000), 1);
      return bj_lastCreatedTerrainDeformation;
    }

    [NativeLuaMemberAttribute]
    public static terraindeformation TerrainDeformationRandomBJ(float duration, location where, float radius, float minDelta, float maxDelta, float updateInterval) {
      bj_lastCreatedTerrainDeformation = TerrainDeformRandom(GetLocationX(where), GetLocationY(where), radius, minDelta, maxDelta, R2I(duration * 1000), R2I(updateInterval * 1000));
      return bj_lastCreatedTerrainDeformation;
    }

    [NativeLuaMemberAttribute]
    public static void TerrainDeformationStopBJ(terraindeformation deformation, float duration) {
      TerrainDeformStop(deformation, R2I(duration * 1000));
    }

    [NativeLuaMemberAttribute]
    public static terraindeformation GetLastCreatedTerrainDeformation() {
      return bj_lastCreatedTerrainDeformation;
    }

    [NativeLuaMemberAttribute]
    public static lightning AddLightningLoc(string codeName, location where1, location where2) {
      bj_lastCreatedLightning = AddLightningEx(codeName, true, GetLocationX(where1), GetLocationY(where1), GetLocationZ(where1), GetLocationX(where2), GetLocationY(where2), GetLocationZ(where2));
      return bj_lastCreatedLightning;
    }

    [NativeLuaMemberAttribute]
    public static bool DestroyLightningBJ(lightning whichBolt) {
      return DestroyLightning(whichBolt);
    }

    [NativeLuaMemberAttribute]
    public static bool MoveLightningLoc(lightning whichBolt, location where1, location where2) {
      return MoveLightningEx(whichBolt, true, GetLocationX(where1), GetLocationY(where1), GetLocationZ(where1), GetLocationX(where2), GetLocationY(where2), GetLocationZ(where2));
    }

    [NativeLuaMemberAttribute]
    public static float GetLightningColorABJ(lightning whichBolt) {
      return GetLightningColorA(whichBolt);
    }

    [NativeLuaMemberAttribute]
    public static float GetLightningColorRBJ(lightning whichBolt) {
      return GetLightningColorR(whichBolt);
    }

    [NativeLuaMemberAttribute]
    public static float GetLightningColorGBJ(lightning whichBolt) {
      return GetLightningColorG(whichBolt);
    }

    [NativeLuaMemberAttribute]
    public static float GetLightningColorBBJ(lightning whichBolt) {
      return GetLightningColorB(whichBolt);
    }

    [NativeLuaMemberAttribute]
    public static bool SetLightningColorBJ(lightning whichBolt, float r, float g, float b, float a) {
      return SetLightningColor(whichBolt, r, g, b, a);
    }

    [NativeLuaMemberAttribute]
    public static lightning GetLastCreatedLightningBJ() {
      return bj_lastCreatedLightning;
    }

    [NativeLuaMemberAttribute]
    public static string GetAbilityEffectBJ(int abilcode, effecttype t, int index) {
      return GetAbilityEffectById(abilcode, t, index);
    }

    [NativeLuaMemberAttribute]
    public static string GetAbilitySoundBJ(int abilcode, soundtype t) {
      return GetAbilitySoundById(abilcode, t);
    }

    [NativeLuaMemberAttribute]
    public static int GetTerrainCliffLevelBJ(location where) {
      return GetTerrainCliffLevel(GetLocationX(where), GetLocationY(where));
    }

    [NativeLuaMemberAttribute]
    public static int GetTerrainTypeBJ(location where) {
      return GetTerrainType(GetLocationX(where), GetLocationY(where));
    }

    [NativeLuaMemberAttribute]
    public static int GetTerrainVarianceBJ(location where) {
      return GetTerrainVariance(GetLocationX(where), GetLocationY(where));
    }

    [NativeLuaMemberAttribute]
    public static void SetTerrainTypeBJ(location where, int terrainType, int variation, int area, int shape) {
      SetTerrainType(GetLocationX(where), GetLocationY(where), terrainType, variation, area, shape);
    }

    [NativeLuaMemberAttribute]
    public static bool IsTerrainPathableBJ(location where, pathingtype t) {
      return IsTerrainPathable(GetLocationX(where), GetLocationY(where), t);
    }

    [NativeLuaMemberAttribute]
    public static void SetTerrainPathableBJ(location where, pathingtype t, bool flag) {
      SetTerrainPathable(GetLocationX(where), GetLocationY(where), t, flag);
    }

    [NativeLuaMemberAttribute]
    public static void SetWaterBaseColorBJ(float red, float green, float blue, float transparency) {
      SetWaterBaseColor(PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0f - transparency));
    }

    [NativeLuaMemberAttribute]
    public static fogmodifier CreateFogModifierRectSimple(player whichPlayer, fogstate whichFogState, rect r, bool afterUnits) {
      bj_lastCreatedFogModifier = CreateFogModifierRect(whichPlayer, whichFogState, r, true, afterUnits);
      return bj_lastCreatedFogModifier;
    }

    [NativeLuaMemberAttribute]
    public static fogmodifier CreateFogModifierRadiusLocSimple(player whichPlayer, fogstate whichFogState, location center, float radius, bool afterUnits) {
      bj_lastCreatedFogModifier = CreateFogModifierRadiusLoc(whichPlayer, whichFogState, center, radius, true, afterUnits);
      return bj_lastCreatedFogModifier;
    }

    [NativeLuaMemberAttribute]
    public static fogmodifier CreateFogModifierRectBJ(bool enabled, player whichPlayer, fogstate whichFogState, rect r) {
      bj_lastCreatedFogModifier = CreateFogModifierRect(whichPlayer, whichFogState, r, true, false);
      if (enabled) {
        FogModifierStart(bj_lastCreatedFogModifier);
      }

      return bj_lastCreatedFogModifier;
    }

    [NativeLuaMemberAttribute]
    public static fogmodifier CreateFogModifierRadiusLocBJ(bool enabled, player whichPlayer, fogstate whichFogState, location center, float radius) {
      bj_lastCreatedFogModifier = CreateFogModifierRadiusLoc(whichPlayer, whichFogState, center, radius, true, false);
      if (enabled) {
        FogModifierStart(bj_lastCreatedFogModifier);
      }

      return bj_lastCreatedFogModifier;
    }

    [NativeLuaMemberAttribute]
    public static fogmodifier GetLastCreatedFogModifier() {
      return bj_lastCreatedFogModifier;
    }

    [NativeLuaMemberAttribute]
    public static void FogEnableOn() {
      FogEnable(true);
    }

    [NativeLuaMemberAttribute]
    public static void FogEnableOff() {
      FogEnable(false);
    }

    [NativeLuaMemberAttribute]
    public static void FogMaskEnableOn() {
      FogMaskEnable(true);
    }

    [NativeLuaMemberAttribute]
    public static void FogMaskEnableOff() {
      FogMaskEnable(false);
    }

    [NativeLuaMemberAttribute]
    public static void UseTimeOfDayBJ(bool flag) {
      SuspendTimeOfDay(!flag);
    }

    [NativeLuaMemberAttribute]
    public static void SetTerrainFogExBJ(int style, float zstart, float zend, float density, float red, float green, float blue) {
      SetTerrainFogEx(style, zstart, zend, density, red * 0.01f, green * 0.01f, blue * 0.01f);
    }

    [NativeLuaMemberAttribute]
    public static void ResetTerrainFogBJ() {
      ResetTerrainFog();
    }

    [NativeLuaMemberAttribute]
    public static void SetDoodadAnimationBJ(string animName, int doodadID, float radius, location center) {
      SetDoodadAnimation(GetLocationX(center), GetLocationY(center), radius, doodadID, false, animName, false);
    }

    [NativeLuaMemberAttribute]
    public static void SetDoodadAnimationRectBJ(string animName, int doodadID, rect r) {
      SetDoodadAnimationRect(r, doodadID, animName, false);
    }

    [NativeLuaMemberAttribute]
    public static void AddUnitAnimationPropertiesBJ(bool add, string animProperties, unit whichUnit) {
      AddUnitAnimationProperties(whichUnit, animProperties, add);
    }

    [NativeLuaMemberAttribute]
    public static image CreateImageBJ(string file, float size, location where, float zOffset, int imageType) {
      bj_lastCreatedImage = CreateImage(file, size, size, size, GetLocationX(where), GetLocationY(where), zOffset, 0, 0, 0, imageType);
      return bj_lastCreatedImage;
    }

    [NativeLuaMemberAttribute]
    public static void ShowImageBJ(bool flag, image whichImage) {
      ShowImage(whichImage, flag);
    }

    [NativeLuaMemberAttribute]
    public static void SetImagePositionBJ(image whichImage, location where, float zOffset) {
      SetImagePosition(whichImage, GetLocationX(where), GetLocationY(where), zOffset);
    }

    [NativeLuaMemberAttribute]
    public static void SetImageColorBJ(image whichImage, float red, float green, float blue, float alpha) {
      SetImageColor(whichImage, PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0f - alpha));
    }

    [NativeLuaMemberAttribute]
    public static image GetLastCreatedImage() {
      return bj_lastCreatedImage;
    }

    [NativeLuaMemberAttribute]
    public static ubersplat CreateUbersplatBJ(location where, string name, float red, float green, float blue, float alpha, bool forcePaused, bool noBirthTime) {
      bj_lastCreatedUbersplat = CreateUbersplat(GetLocationX(where), GetLocationY(where), name, PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0f - alpha), forcePaused, noBirthTime);
      return bj_lastCreatedUbersplat;
    }

    [NativeLuaMemberAttribute]
    public static void ShowUbersplatBJ(bool flag, ubersplat whichSplat) {
      ShowUbersplat(whichSplat, flag);
    }

    [NativeLuaMemberAttribute]
    public static ubersplat GetLastCreatedUbersplat() {
      return bj_lastCreatedUbersplat;
    }

    [NativeLuaMemberAttribute]
    public static void PlaySoundBJ(sound soundHandle) {
      bj_lastPlayedSound = soundHandle;
      if ((soundHandle != null)) {
        StartSound(soundHandle);
      }
    }

    [NativeLuaMemberAttribute]
    public static void StopSoundBJ(sound soundHandle, bool fadeOut) {
      StopSound(soundHandle, false, fadeOut);
    }

    [NativeLuaMemberAttribute]
    public static void SetSoundVolumeBJ(sound soundHandle, float volumePercent) {
      SetSoundVolume(soundHandle, PercentToInt(volumePercent, 127));
    }

    [NativeLuaMemberAttribute]
    public static void SetSoundOffsetBJ(float newOffset, sound soundHandle) {
      SetSoundPlayPosition(soundHandle, R2I(newOffset * 1000));
    }

    [NativeLuaMemberAttribute]
    public static void SetSoundDistanceCutoffBJ(sound soundHandle, float cutoff) {
      SetSoundDistanceCutoff(soundHandle, cutoff);
    }

    [NativeLuaMemberAttribute]
    public static void SetSoundPitchBJ(sound soundHandle, float pitch) {
      SetSoundPitch(soundHandle, pitch);
    }

    [NativeLuaMemberAttribute]
    public static void SetSoundPositionLocBJ(sound soundHandle, location loc, float z) {
      SetSoundPosition(soundHandle, GetLocationX(loc), GetLocationY(loc), z);
    }

    [NativeLuaMemberAttribute]
    public static void AttachSoundToUnitBJ(sound soundHandle, unit whichUnit) {
      AttachSoundToUnit(soundHandle, whichUnit);
    }

    [NativeLuaMemberAttribute]
    public static void SetSoundConeAnglesBJ(sound soundHandle, float inside, float outside, float outsideVolumePercent) {
      SetSoundConeAngles(soundHandle, inside, outside, PercentToInt(outsideVolumePercent, 127));
    }

    [NativeLuaMemberAttribute]
    public static void KillSoundWhenDoneBJ(sound soundHandle) {
      KillSoundWhenDone(soundHandle);
    }

    [NativeLuaMemberAttribute]
    public static void PlaySoundAtPointBJ(sound soundHandle, float volumePercent, location loc, float z) {
      SetSoundPositionLocBJ(soundHandle, loc, z);
      SetSoundVolumeBJ(soundHandle, volumePercent);
      PlaySoundBJ(soundHandle);
    }

    [NativeLuaMemberAttribute]
    public static void PlaySoundOnUnitBJ(sound soundHandle, float volumePercent, unit whichUnit) {
      AttachSoundToUnitBJ(soundHandle, whichUnit);
      SetSoundVolumeBJ(soundHandle, volumePercent);
      PlaySoundBJ(soundHandle);
    }

    [NativeLuaMemberAttribute]
    public static void PlaySoundFromOffsetBJ(sound soundHandle, float volumePercent, float startingOffset) {
      SetSoundVolumeBJ(soundHandle, volumePercent);
      PlaySoundBJ(soundHandle);
      SetSoundOffsetBJ(startingOffset, soundHandle);
    }

    [NativeLuaMemberAttribute]
    public static void PlayMusicBJ(string musicFileName) {
      bj_lastPlayedMusic = musicFileName;
      PlayMusic(musicFileName);
    }

    [NativeLuaMemberAttribute]
    public static void PlayMusicExBJ(string musicFileName, float startingOffset, float fadeInTime) {
      bj_lastPlayedMusic = musicFileName;
      PlayMusicEx(musicFileName, R2I(startingOffset * 1000), R2I(fadeInTime * 1000));
    }

    [NativeLuaMemberAttribute]
    public static void SetMusicOffsetBJ(float newOffset) {
      SetMusicPlayPosition(R2I(newOffset * 1000));
    }

    [NativeLuaMemberAttribute]
    public static void PlayThematicMusicBJ(string musicName) {
      PlayThematicMusic(musicName);
    }

    [NativeLuaMemberAttribute]
    public static void PlayThematicMusicExBJ(string musicName, float startingOffset) {
      PlayThematicMusicEx(musicName, R2I(startingOffset * 1000));
    }

    [NativeLuaMemberAttribute]
    public static void SetThematicMusicOffsetBJ(float newOffset) {
      SetThematicMusicPlayPosition(R2I(newOffset * 1000));
    }

    [NativeLuaMemberAttribute]
    public static void EndThematicMusicBJ() {
      EndThematicMusic();
    }

    [NativeLuaMemberAttribute]
    public static void StopMusicBJ(bool fadeOut) {
      StopMusic(fadeOut);
    }

    [NativeLuaMemberAttribute]
    public static void ResumeMusicBJ() {
      ResumeMusic();
    }

    [NativeLuaMemberAttribute]
    public static void SetMusicVolumeBJ(float volumePercent) {
      SetMusicVolume(PercentToInt(volumePercent, 127));
    }

    [NativeLuaMemberAttribute]
    public static float GetSoundDurationBJ(sound soundHandle) {
      if ((soundHandle == null)) {
        return bj_NOTHING_SOUND_DURATION;
      } else {
        return I2R(GetSoundDuration(soundHandle)) * 0.001f;
      }
    }

    [NativeLuaMemberAttribute]
    public static float GetSoundFileDurationBJ(string musicFileName) {
      return I2R(GetSoundFileDuration(musicFileName)) * 0.001f;
    }

    [NativeLuaMemberAttribute]
    public static sound GetLastPlayedSound() {
      return bj_lastPlayedSound;
    }

    [NativeLuaMemberAttribute]
    public static string GetLastPlayedMusic() {
      return bj_lastPlayedMusic;
    }

    [NativeLuaMemberAttribute]
    public static void VolumeGroupSetVolumeBJ(volumegroup vgroup, float percent) {
      VolumeGroupSetVolume(vgroup, percent * 0.01f);
    }

    [NativeLuaMemberAttribute]
    public static void SetCineModeVolumeGroupsImmediateBJ() {
      VolumeGroupSetVolume(SOUND_VOLUMEGROUP_UNITMOVEMENT, bj_CINEMODE_VOLUME_UNITMOVEMENT);
      VolumeGroupSetVolume(SOUND_VOLUMEGROUP_UNITSOUNDS, bj_CINEMODE_VOLUME_UNITSOUNDS);
      VolumeGroupSetVolume(SOUND_VOLUMEGROUP_COMBAT, bj_CINEMODE_VOLUME_COMBAT);
      VolumeGroupSetVolume(SOUND_VOLUMEGROUP_SPELLS, bj_CINEMODE_VOLUME_SPELLS);
      VolumeGroupSetVolume(SOUND_VOLUMEGROUP_UI, bj_CINEMODE_VOLUME_UI);
      VolumeGroupSetVolume(SOUND_VOLUMEGROUP_MUSIC, bj_CINEMODE_VOLUME_MUSIC);
      VolumeGroupSetVolume(SOUND_VOLUMEGROUP_AMBIENTSOUNDS, bj_CINEMODE_VOLUME_AMBIENTSOUNDS);
      VolumeGroupSetVolume(SOUND_VOLUMEGROUP_FIRE, bj_CINEMODE_VOLUME_FIRE);
    }

    [NativeLuaMemberAttribute]
    public static void SetCineModeVolumeGroupsBJ() {
      if (bj_gameStarted) {
        SetCineModeVolumeGroupsImmediateBJ();
      } else {
        TimerStart(bj_volumeGroupsTimer, bj_GAME_STARTED_THRESHOLD, false, SetCineModeVolumeGroupsImmediateBJ);
      }
    }

    [NativeLuaMemberAttribute]
    public static void SetSpeechVolumeGroupsImmediateBJ() {
      VolumeGroupSetVolume(SOUND_VOLUMEGROUP_UNITMOVEMENT, bj_SPEECH_VOLUME_UNITMOVEMENT);
      VolumeGroupSetVolume(SOUND_VOLUMEGROUP_UNITSOUNDS, bj_SPEECH_VOLUME_UNITSOUNDS);
      VolumeGroupSetVolume(SOUND_VOLUMEGROUP_COMBAT, bj_SPEECH_VOLUME_COMBAT);
      VolumeGroupSetVolume(SOUND_VOLUMEGROUP_SPELLS, bj_SPEECH_VOLUME_SPELLS);
      VolumeGroupSetVolume(SOUND_VOLUMEGROUP_UI, bj_SPEECH_VOLUME_UI);
      VolumeGroupSetVolume(SOUND_VOLUMEGROUP_MUSIC, bj_SPEECH_VOLUME_MUSIC);
      VolumeGroupSetVolume(SOUND_VOLUMEGROUP_AMBIENTSOUNDS, bj_SPEECH_VOLUME_AMBIENTSOUNDS);
      VolumeGroupSetVolume(SOUND_VOLUMEGROUP_FIRE, bj_SPEECH_VOLUME_FIRE);
    }

    [NativeLuaMemberAttribute]
    public static void SetSpeechVolumeGroupsBJ() {
      if (bj_gameStarted) {
        SetSpeechVolumeGroupsImmediateBJ();
      } else {
        TimerStart(bj_volumeGroupsTimer, bj_GAME_STARTED_THRESHOLD, false, SetSpeechVolumeGroupsImmediateBJ);
      }
    }

    [NativeLuaMemberAttribute]
    public static void VolumeGroupResetImmediateBJ() {
      VolumeGroupReset();
    }

    [NativeLuaMemberAttribute]
    public static void VolumeGroupResetBJ() {
      if (bj_gameStarted) {
        VolumeGroupResetImmediateBJ();
      } else {
        TimerStart(bj_volumeGroupsTimer, bj_GAME_STARTED_THRESHOLD, false, VolumeGroupResetImmediateBJ);
      }
    }

    [NativeLuaMemberAttribute]
    public static bool GetSoundIsPlayingBJ(sound soundHandle) {
      return GetSoundIsLoading(soundHandle) || GetSoundIsPlaying(soundHandle);
    }

    [NativeLuaMemberAttribute]
    public static void WaitForSoundBJ(sound soundHandle, float offset) {
      TriggerWaitForSound(soundHandle, offset);
    }

    [NativeLuaMemberAttribute]
    public static void SetMapMusicIndexedBJ(string musicName, int index) {
      SetMapMusic(musicName, false, index);
    }

    [NativeLuaMemberAttribute]
    public static void SetMapMusicRandomBJ(string musicName) {
      SetMapMusic(musicName, true, 0);
    }

    [NativeLuaMemberAttribute]
    public static void ClearMapMusicBJ() {
      ClearMapMusic();
    }

    [NativeLuaMemberAttribute]
    public static void SetStackedSoundBJ(bool add, sound soundHandle, rect r) {
      float width = GetRectMaxX(r) - GetRectMinX(r);
      float height = GetRectMaxY(r) - GetRectMinY(r);
      SetSoundPosition(soundHandle, GetRectCenterX(r), GetRectCenterY(r), 0);
      if (add) {
        RegisterStackedSound(soundHandle, true, width, height);
      } else {
        UnregisterStackedSound(soundHandle, true, width, height);
      }
    }

    [NativeLuaMemberAttribute]
    public static void StartSoundForPlayerBJ(player whichPlayer, sound soundHandle) {
      if ((whichPlayer == GetLocalPlayer())) {
        StartSound(soundHandle);
      }
    }

    [NativeLuaMemberAttribute]
    public static void VolumeGroupSetVolumeForPlayerBJ(player whichPlayer, volumegroup vgroup, float scale) {
      if ((GetLocalPlayer() == whichPlayer)) {
        VolumeGroupSetVolume(vgroup, scale);
      }
    }

    [NativeLuaMemberAttribute]
    public static void EnableDawnDusk(bool flag) {
      bj_useDawnDuskSounds = flag;
    }

    [NativeLuaMemberAttribute]
    public static bool IsDawnDuskEnabled() {
      return bj_useDawnDuskSounds;
    }

    [NativeLuaMemberAttribute]
    public static void SetAmbientDaySound(string inLabel) {
      float ToD = default;
      if ((bj_dayAmbientSound != null)) {
        StopSound(bj_dayAmbientSound, true, true);
      }

      bj_dayAmbientSound = CreateMIDISound(inLabel, 20, 20);
      ToD = GetTimeOfDay();
      if ((ToD >= bj_TOD_DAWN && ToD < bj_TOD_DUSK)) {
        StartSound(bj_dayAmbientSound);
      }
    }

    [NativeLuaMemberAttribute]
    public static void SetAmbientNightSound(string inLabel) {
      float ToD = default;
      if ((bj_nightAmbientSound != null)) {
        StopSound(bj_nightAmbientSound, true, true);
      }

      bj_nightAmbientSound = CreateMIDISound(inLabel, 20, 20);
      ToD = GetTimeOfDay();
      if ((ToD < bj_TOD_DAWN || ToD >= bj_TOD_DUSK)) {
        StartSound(bj_nightAmbientSound);
      }
    }

    [NativeLuaMemberAttribute]
    public static effect AddSpecialEffectLocBJ(location where, string modelName) {
      bj_lastCreatedEffect = AddSpecialEffectLoc(modelName, where);
      return bj_lastCreatedEffect;
    }

    [NativeLuaMemberAttribute]
    public static effect AddSpecialEffectTargetUnitBJ(string attachPointName, widget targetWidget, string modelName) {
      bj_lastCreatedEffect = AddSpecialEffectTarget(modelName, targetWidget, attachPointName);
      return bj_lastCreatedEffect;
    }

    [NativeLuaMemberAttribute]
    public static void DestroyEffectBJ(effect whichEffect) {
      DestroyEffect(whichEffect);
    }

    [NativeLuaMemberAttribute]
    public static effect GetLastCreatedEffectBJ() {
      return bj_lastCreatedEffect;
    }

    [NativeLuaMemberAttribute]
    public static location GetItemLoc(item whichItem) {
      return Location(GetItemX(whichItem), GetItemY(whichItem));
    }

    [NativeLuaMemberAttribute]
    public static float GetItemLifeBJ(widget whichWidget) {
      return GetWidgetLife(whichWidget);
    }

    [NativeLuaMemberAttribute]
    public static void SetItemLifeBJ(widget whichWidget, float life) {
      SetWidgetLife(whichWidget, life);
    }

    [NativeLuaMemberAttribute]
    public static void AddHeroXPSwapped(int xpToAdd, unit whichHero, bool showEyeCandy) {
      AddHeroXP(whichHero, xpToAdd, showEyeCandy);
    }

    [NativeLuaMemberAttribute]
    public static void SetHeroLevelBJ(unit whichHero, int newLevel, bool showEyeCandy) {
      int oldLevel = GetHeroLevel(whichHero);
      if ((newLevel > oldLevel)) {
        SetHeroLevel(whichHero, newLevel, showEyeCandy);
      } else if ((newLevel < oldLevel)) {
        UnitStripHeroLevel(whichHero, oldLevel - newLevel);
      } else {
      }
    }

    [NativeLuaMemberAttribute]
    public static int DecUnitAbilityLevelSwapped(int abilcode, unit whichUnit) {
      return DecUnitAbilityLevel(whichUnit, abilcode);
    }

    [NativeLuaMemberAttribute]
    public static int IncUnitAbilityLevelSwapped(int abilcode, unit whichUnit) {
      return IncUnitAbilityLevel(whichUnit, abilcode);
    }

    [NativeLuaMemberAttribute]
    public static int SetUnitAbilityLevelSwapped(int abilcode, unit whichUnit, int level) {
      return SetUnitAbilityLevel(whichUnit, abilcode, level);
    }

    [NativeLuaMemberAttribute]
    public static int GetUnitAbilityLevelSwapped(int abilcode, unit whichUnit) {
      return GetUnitAbilityLevel(whichUnit, abilcode);
    }

    [NativeLuaMemberAttribute]
    public static bool UnitHasBuffBJ(unit whichUnit, int buffcode) {
      return (GetUnitAbilityLevel(whichUnit, buffcode) > 0);
    }

    [NativeLuaMemberAttribute]
    public static bool UnitRemoveBuffBJ(int buffcode, unit whichUnit) {
      return UnitRemoveAbility(whichUnit, buffcode);
    }

    [NativeLuaMemberAttribute]
    public static bool UnitAddItemSwapped(item whichItem, unit whichHero) {
      return UnitAddItem(whichHero, whichItem);
    }

    [NativeLuaMemberAttribute]
    public static item UnitAddItemByIdSwapped(int itemId, unit whichHero) {
      bj_lastCreatedItem = CreateItem(itemId, GetUnitX(whichHero), GetUnitY(whichHero));
      UnitAddItem(whichHero, bj_lastCreatedItem);
      return bj_lastCreatedItem;
    }

    [NativeLuaMemberAttribute]
    public static void UnitRemoveItemSwapped(item whichItem, unit whichHero) {
      bj_lastRemovedItem = whichItem;
      UnitRemoveItem(whichHero, whichItem);
    }

    [NativeLuaMemberAttribute]
    public static item UnitRemoveItemFromSlotSwapped(int itemSlot, unit whichHero) {
      bj_lastRemovedItem = UnitRemoveItemFromSlot(whichHero, itemSlot - 1);
      return bj_lastRemovedItem;
    }

    [NativeLuaMemberAttribute]
    public static item CreateItemLoc(int itemId, location loc) {
      bj_lastCreatedItem = CreateItem(itemId, GetLocationX(loc), GetLocationY(loc));
      return bj_lastCreatedItem;
    }

    [NativeLuaMemberAttribute]
    public static item GetLastCreatedItem() {
      return bj_lastCreatedItem;
    }

    [NativeLuaMemberAttribute]
    public static item GetLastRemovedItem() {
      return bj_lastRemovedItem;
    }

    [NativeLuaMemberAttribute]
    public static void SetItemPositionLoc(item whichItem, location loc) {
      SetItemPosition(whichItem, GetLocationX(loc), GetLocationY(loc));
    }

    [NativeLuaMemberAttribute]
    public static int GetLearnedSkillBJ() {
      return GetLearnedSkill();
    }

    [NativeLuaMemberAttribute]
    public static void SuspendHeroXPBJ(bool flag, unit whichHero) {
      SuspendHeroXP(whichHero, !flag);
    }

    [NativeLuaMemberAttribute]
    public static void SetPlayerHandicapXPBJ(player whichPlayer, float handicapPercent) {
      SetPlayerHandicapXP(whichPlayer, handicapPercent * 0.01f);
    }

    [NativeLuaMemberAttribute]
    public static float GetPlayerHandicapXPBJ(player whichPlayer) {
      return GetPlayerHandicapXP(whichPlayer) * 100;
    }

    [NativeLuaMemberAttribute]
    public static void SetPlayerHandicapBJ(player whichPlayer, float handicapPercent) {
      SetPlayerHandicap(whichPlayer, handicapPercent * 0.01f);
    }

    [NativeLuaMemberAttribute]
    public static float GetPlayerHandicapBJ(player whichPlayer) {
      return GetPlayerHandicap(whichPlayer) * 100;
    }

    [NativeLuaMemberAttribute]
    public static int GetHeroStatBJ(int whichStat, unit whichHero, bool includeBonuses) {
      if ((whichStat == bj_HEROSTAT_STR)) {
        return GetHeroStr(whichHero, includeBonuses);
      } else if ((whichStat == bj_HEROSTAT_AGI)) {
        return GetHeroAgi(whichHero, includeBonuses);
      } else if ((whichStat == bj_HEROSTAT_INT)) {
        return GetHeroInt(whichHero, includeBonuses);
      } else {
        return 0;
      }
    }

    [NativeLuaMemberAttribute]
    public static void SetHeroStat(unit whichHero, int whichStat, int value) {
      if ((value <= 0)) {
        return;
      }

      if ((whichStat == bj_HEROSTAT_STR)) {
        SetHeroStr(whichHero, value, true);
      } else if ((whichStat == bj_HEROSTAT_AGI)) {
        SetHeroAgi(whichHero, value, true);
      } else if ((whichStat == bj_HEROSTAT_INT)) {
        SetHeroInt(whichHero, value, true);
      } else {
      }
    }

    [NativeLuaMemberAttribute]
    public static void ModifyHeroStat(int whichStat, unit whichHero, int modifyMethod, int value) {
      if ((modifyMethod == bj_MODIFYMETHOD_ADD)) {
        SetHeroStat(whichHero, whichStat, GetHeroStatBJ(whichStat, whichHero, false) + value);
      } else if ((modifyMethod == bj_MODIFYMETHOD_SUB)) {
        SetHeroStat(whichHero, whichStat, GetHeroStatBJ(whichStat, whichHero, false) - value);
      } else if ((modifyMethod == bj_MODIFYMETHOD_SET)) {
        SetHeroStat(whichHero, whichStat, value);
      } else {
      }
    }

    [NativeLuaMemberAttribute]
    public static bool ModifyHeroSkillPoints(unit whichHero, int modifyMethod, int value) {
      if ((modifyMethod == bj_MODIFYMETHOD_ADD)) {
        return UnitModifySkillPoints(whichHero, value);
      } else if ((modifyMethod == bj_MODIFYMETHOD_SUB)) {
        return UnitModifySkillPoints(whichHero, -value);
      } else if ((modifyMethod == bj_MODIFYMETHOD_SET)) {
        return UnitModifySkillPoints(whichHero, value - GetHeroSkillPoints(whichHero));
      } else {
        return false;
      }
    }

    [NativeLuaMemberAttribute]
    public static bool UnitDropItemPointBJ(unit whichUnit, item whichItem, float x, float y) {
      return UnitDropItemPoint(whichUnit, whichItem, x, y);
    }

    [NativeLuaMemberAttribute]
    public static bool UnitDropItemPointLoc(unit whichUnit, item whichItem, location loc) {
      return UnitDropItemPoint(whichUnit, whichItem, GetLocationX(loc), GetLocationY(loc));
    }

    [NativeLuaMemberAttribute]
    public static bool UnitDropItemSlotBJ(unit whichUnit, item whichItem, int slot) {
      return UnitDropItemSlot(whichUnit, whichItem, slot - 1);
    }

    [NativeLuaMemberAttribute]
    public static bool UnitDropItemTargetBJ(unit whichUnit, item whichItem, widget target) {
      return UnitDropItemTarget(whichUnit, whichItem, target);
    }

    [NativeLuaMemberAttribute]
    public static bool UnitUseItemDestructable(unit whichUnit, item whichItem, widget target) {
      return UnitUseItemTarget(whichUnit, whichItem, target);
    }

    [NativeLuaMemberAttribute]
    public static bool UnitUseItemPointLoc(unit whichUnit, item whichItem, location loc) {
      return UnitUseItemPoint(whichUnit, whichItem, GetLocationX(loc), GetLocationY(loc));
    }

    [NativeLuaMemberAttribute]
    public static item UnitItemInSlotBJ(unit whichUnit, int itemSlot) {
      return UnitItemInSlot(whichUnit, itemSlot - 1);
    }

    [NativeLuaMemberAttribute]
    public static int GetInventoryIndexOfItemTypeBJ(unit whichUnit, int itemId) {
      int index = default;
      item indexItem = default;
      index = 0;
      while (true) {
        indexItem = UnitItemInSlot(whichUnit, index);
        if ((indexItem != null) && (GetItemTypeId(indexItem) == itemId)) {
          return index + 1;
        }

        index = index + 1;
        if (index >= bj_MAX_INVENTORY)
          break;
      }

      return 0;
    }

    [NativeLuaMemberAttribute]
    public static item GetItemOfTypeFromUnitBJ(unit whichUnit, int itemId) {
      int index = GetInventoryIndexOfItemTypeBJ(whichUnit, itemId);
      if ((index == 0)) {
        return null;
      } else {
        return UnitItemInSlot(whichUnit, index - 1);
      }
    }

    [NativeLuaMemberAttribute]
    public static bool UnitHasItemOfTypeBJ(unit whichUnit, int itemId) {
      return GetInventoryIndexOfItemTypeBJ(whichUnit, itemId) > 0;
    }

    [NativeLuaMemberAttribute]
    public static int UnitInventoryCount(unit whichUnit) {
      int index = 0;
      int count = 0;
      while (true) {
        if ((UnitItemInSlot(whichUnit, index) != null)) {
          count = count + 1;
        }

        index = index + 1;
        if (index >= bj_MAX_INVENTORY)
          break;
      }

      return count;
    }

    [NativeLuaMemberAttribute]
    public static int UnitInventorySizeBJ(unit whichUnit) {
      return UnitInventorySize(whichUnit);
    }

    [NativeLuaMemberAttribute]
    public static void SetItemInvulnerableBJ(item whichItem, bool flag) {
      SetItemInvulnerable(whichItem, flag);
    }

    [NativeLuaMemberAttribute]
    public static void SetItemDropOnDeathBJ(item whichItem, bool flag) {
      SetItemDropOnDeath(whichItem, flag);
    }

    [NativeLuaMemberAttribute]
    public static void SetItemDroppableBJ(item whichItem, bool flag) {
      SetItemDroppable(whichItem, flag);
    }

    [NativeLuaMemberAttribute]
    public static void SetItemPlayerBJ(item whichItem, player whichPlayer, bool changeColor) {
      SetItemPlayer(whichItem, whichPlayer, changeColor);
    }

    [NativeLuaMemberAttribute]
    public static void SetItemVisibleBJ(bool show, item whichItem) {
      SetItemVisible(whichItem, show);
    }

    [NativeLuaMemberAttribute]
    public static bool IsItemHiddenBJ(item whichItem) {
      return !IsItemVisible(whichItem);
    }

    [NativeLuaMemberAttribute]
    public static int ChooseRandomItemBJ(int level) {
      return ChooseRandomItem(level);
    }

    [NativeLuaMemberAttribute]
    public static int ChooseRandomItemExBJ(int level, itemtype whichType) {
      return ChooseRandomItemEx(whichType, level);
    }

    [NativeLuaMemberAttribute]
    public static int ChooseRandomNPBuildingBJ() {
      return ChooseRandomNPBuilding();
    }

    [NativeLuaMemberAttribute]
    public static int ChooseRandomCreepBJ(int level) {
      return ChooseRandomCreep(level);
    }

    [NativeLuaMemberAttribute]
    public static void EnumItemsInRectBJ(rect r, System.Action actionFunc) {
      EnumItemsInRect(r, null, actionFunc);
    }

    [NativeLuaMemberAttribute]
    public static void RandomItemInRectBJEnum() {
      bj_itemRandomConsidered = bj_itemRandomConsidered + 1;
      if ((GetRandomInt(1, bj_itemRandomConsidered) == 1)) {
        bj_itemRandomCurrentPick = GetEnumItem();
      }
    }

    [NativeLuaMemberAttribute]
    public static item RandomItemInRectBJ(rect r, boolexpr filter) {
      bj_itemRandomConsidered = 0;
      bj_itemRandomCurrentPick = null;
      EnumItemsInRect(r, filter, RandomItemInRectBJEnum);
      DestroyBoolExpr(filter);
      return bj_itemRandomCurrentPick;
    }

    [NativeLuaMemberAttribute]
    public static item RandomItemInRectSimpleBJ(rect r) {
      return RandomItemInRectBJ(r, null);
    }

    [NativeLuaMemberAttribute]
    public static bool CheckItemStatus(item whichItem, int status) {
      if ((status == bj_ITEM_STATUS_HIDDEN)) {
        return !IsItemVisible(whichItem);
      } else if ((status == bj_ITEM_STATUS_OWNED)) {
        return IsItemOwned(whichItem);
      } else if ((status == bj_ITEM_STATUS_INVULNERABLE)) {
        return IsItemInvulnerable(whichItem);
      } else if ((status == bj_ITEM_STATUS_POWERUP)) {
        return IsItemPowerup(whichItem);
      } else if ((status == bj_ITEM_STATUS_SELLABLE)) {
        return IsItemSellable(whichItem);
      } else if ((status == bj_ITEM_STATUS_PAWNABLE)) {
        return IsItemPawnable(whichItem);
      } else {
        return false;
      }
    }

    [NativeLuaMemberAttribute]
    public static bool CheckItemcodeStatus(int itemId, int status) {
      if ((status == bj_ITEMCODE_STATUS_POWERUP)) {
        return IsItemIdPowerup(itemId);
      } else if ((status == bj_ITEMCODE_STATUS_SELLABLE)) {
        return IsItemIdSellable(itemId);
      } else if ((status == bj_ITEMCODE_STATUS_PAWNABLE)) {
        return IsItemIdPawnable(itemId);
      } else {
        return false;
      }
    }

    [NativeLuaMemberAttribute]
    public static int UnitId2OrderIdBJ(int unitId) {
      return unitId;
    }

    [NativeLuaMemberAttribute]
    public static int String2UnitIdBJ(string unitIdString) {
      return UnitId(unitIdString);
    }

    [NativeLuaMemberAttribute]
    public static string UnitId2StringBJ(int unitId) {
      string unitString = UnitId2String(unitId);
      if ((unitString != null)) {
        return unitString;
      }

      return string.Empty;
    }

    [NativeLuaMemberAttribute]
    public static int String2OrderIdBJ(string orderIdString) {
      int orderId = default;
      orderId = OrderId(orderIdString);
      if ((orderId != 0)) {
        return orderId;
      }

      orderId = UnitId(orderIdString);
      if ((orderId != 0)) {
        return orderId;
      }

      return 0;
    }

    [NativeLuaMemberAttribute]
    public static string OrderId2StringBJ(int orderId) {
      string orderString = default;
      orderString = OrderId2String(orderId);
      if ((orderString != null)) {
        return orderString;
      }

      orderString = UnitId2String(orderId);
      if ((orderString != null)) {
        return orderString;
      }

      return string.Empty;
    }

    [NativeLuaMemberAttribute]
    public static int GetIssuedOrderIdBJ() {
      return GetIssuedOrderId();
    }

    [NativeLuaMemberAttribute]
    public static unit GetKillingUnitBJ() {
      return GetKillingUnit();
    }

    [NativeLuaMemberAttribute]
    public static unit CreateUnitAtLocSaveLast(player id, int unitid, location loc, float face) {
      if ((unitid == 1969713004)) {
        bj_lastCreatedUnit = CreateBlightedGoldmine(id, GetLocationX(loc), GetLocationY(loc), face);
      } else {
        bj_lastCreatedUnit = CreateUnitAtLoc(id, unitid, loc, face);
      }

      return bj_lastCreatedUnit;
    }

    [NativeLuaMemberAttribute]
    public static unit GetLastCreatedUnit() {
      return bj_lastCreatedUnit;
    }

    [NativeLuaMemberAttribute]
    public static group CreateNUnitsAtLoc(int count, int unitId, player whichPlayer, location loc, float face) {
      GroupClear(bj_lastCreatedGroup);
      while (true) {
        count = count - 1;
        if (count < 0)
          break;
        CreateUnitAtLocSaveLast(whichPlayer, unitId, loc, face);
        GroupAddUnit(bj_lastCreatedGroup, bj_lastCreatedUnit);
      }

      return bj_lastCreatedGroup;
    }

    [NativeLuaMemberAttribute]
    public static group CreateNUnitsAtLocFacingLocBJ(int count, int unitId, player whichPlayer, location loc, location lookAt) {
      return CreateNUnitsAtLoc(count, unitId, whichPlayer, loc, AngleBetweenPoints(loc, lookAt));
    }

    [NativeLuaMemberAttribute]
    public static void GetLastCreatedGroupEnum() {
      GroupAddUnit(bj_groupLastCreatedDest, GetEnumUnit());
    }

    [NativeLuaMemberAttribute]
    public static group GetLastCreatedGroup() {
      bj_groupLastCreatedDest = CreateGroup();
      ForGroup(bj_lastCreatedGroup, GetLastCreatedGroupEnum);
      return bj_groupLastCreatedDest;
    }

    [NativeLuaMemberAttribute]
    public static unit CreateCorpseLocBJ(int unitid, player whichPlayer, location loc) {
      bj_lastCreatedUnit = CreateCorpse(whichPlayer, unitid, GetLocationX(loc), GetLocationY(loc), GetRandomReal(0, 360));
      return bj_lastCreatedUnit;
    }

    [NativeLuaMemberAttribute]
    public static void UnitSuspendDecayBJ(bool suspend, unit whichUnit) {
      UnitSuspendDecay(whichUnit, suspend);
    }

    [NativeLuaMemberAttribute]
    public static void DelayedSuspendDecayStopAnimEnum() {
      unit enumUnit = GetEnumUnit();
      if ((GetUnitState(enumUnit, UNIT_STATE_LIFE) <= 0)) {
        SetUnitTimeScale(enumUnit, 0.0001f);
      }
    }

    [NativeLuaMemberAttribute]
    public static void DelayedSuspendDecayBoneEnum() {
      unit enumUnit = GetEnumUnit();
      if ((GetUnitState(enumUnit, UNIT_STATE_LIFE) <= 0)) {
        UnitSuspendDecay(enumUnit, true);
        SetUnitTimeScale(enumUnit, 0.0001f);
      }
    }

    [NativeLuaMemberAttribute]
    public static void DelayedSuspendDecayFleshEnum() {
      unit enumUnit = GetEnumUnit();
      if ((GetUnitState(enumUnit, UNIT_STATE_LIFE) <= 0)) {
        UnitSuspendDecay(enumUnit, true);
        SetUnitTimeScale(enumUnit, 10.0f);
        SetUnitAnimation(enumUnit, "decay flesh");
      }
    }

    [NativeLuaMemberAttribute]
    public static void DelayedSuspendDecay() {
      group boneGroup = default;
      group fleshGroup = default;
      boneGroup = bj_suspendDecayBoneGroup;
      fleshGroup = bj_suspendDecayFleshGroup;
      bj_suspendDecayBoneGroup = CreateGroup();
      bj_suspendDecayFleshGroup = CreateGroup();
      ForGroup(fleshGroup, DelayedSuspendDecayStopAnimEnum);
      ForGroup(boneGroup, DelayedSuspendDecayStopAnimEnum);
      TriggerSleepAction(bj_CORPSE_MAX_DEATH_TIME);
      ForGroup(fleshGroup, DelayedSuspendDecayFleshEnum);
      ForGroup(boneGroup, DelayedSuspendDecayBoneEnum);
      TriggerSleepAction(0.05f);
      ForGroup(fleshGroup, DelayedSuspendDecayStopAnimEnum);
      DestroyGroup(boneGroup);
      DestroyGroup(fleshGroup);
    }

    [NativeLuaMemberAttribute]
    public static void DelayedSuspendDecayCreate() {
      bj_delayedSuspendDecayTrig = CreateTrigger();
      TriggerRegisterTimerExpireEvent(bj_delayedSuspendDecayTrig, bj_delayedSuspendDecayTimer);
      TriggerAddAction(bj_delayedSuspendDecayTrig, DelayedSuspendDecay);
    }

    [NativeLuaMemberAttribute]
    public static unit CreatePermanentCorpseLocBJ(int style, int unitid, player whichPlayer, location loc, float facing) {
      bj_lastCreatedUnit = CreateCorpse(whichPlayer, unitid, GetLocationX(loc), GetLocationY(loc), facing);
      SetUnitBlendTime(bj_lastCreatedUnit, 0);
      if ((style == bj_CORPSETYPE_FLESH)) {
        SetUnitAnimation(bj_lastCreatedUnit, "decay flesh");
        GroupAddUnit(bj_suspendDecayFleshGroup, bj_lastCreatedUnit);
      } else if ((style == bj_CORPSETYPE_BONE)) {
        SetUnitAnimation(bj_lastCreatedUnit, "decay bone");
        GroupAddUnit(bj_suspendDecayBoneGroup, bj_lastCreatedUnit);
      } else {
        SetUnitAnimation(bj_lastCreatedUnit, "decay bone");
        GroupAddUnit(bj_suspendDecayBoneGroup, bj_lastCreatedUnit);
      }

      TimerStart(bj_delayedSuspendDecayTimer, 0.05f, false, null);
      return bj_lastCreatedUnit;
    }

    [NativeLuaMemberAttribute]
    public static float GetUnitStateSwap(unitstate whichState, unit whichUnit) {
      return GetUnitState(whichUnit, whichState);
    }

    [NativeLuaMemberAttribute]
    public static float GetUnitStatePercent(unit whichUnit, unitstate whichState, unitstate whichMaxState) {
      float value = GetUnitState(whichUnit, whichState);
      float maxValue = GetUnitState(whichUnit, whichMaxState);
      if ((whichUnit == null) || (maxValue == 0)) {
        return 0.0f;
      }

      return value / maxValue * 100.0f;
    }

    [NativeLuaMemberAttribute]
    public static float GetUnitLifePercent(unit whichUnit) {
      return GetUnitStatePercent(whichUnit, UNIT_STATE_LIFE, UNIT_STATE_MAX_LIFE);
    }

    [NativeLuaMemberAttribute]
    public static float GetUnitManaPercent(unit whichUnit) {
      return GetUnitStatePercent(whichUnit, UNIT_STATE_MANA, UNIT_STATE_MAX_MANA);
    }

    [NativeLuaMemberAttribute]
    public static void SelectUnitSingle(unit whichUnit) {
      ClearSelection();
      SelectUnit(whichUnit, true);
    }

    [NativeLuaMemberAttribute]
    public static void SelectGroupBJEnum() {
      SelectUnit(GetEnumUnit(), true);
    }

    [NativeLuaMemberAttribute]
    public static void SelectGroupBJ(group g) {
      ClearSelection();
      ForGroup(g, SelectGroupBJEnum);
    }

    [NativeLuaMemberAttribute]
    public static void SelectUnitAdd(unit whichUnit) {
      SelectUnit(whichUnit, true);
    }

    [NativeLuaMemberAttribute]
    public static void SelectUnitRemove(unit whichUnit) {
      SelectUnit(whichUnit, false);
    }

    [NativeLuaMemberAttribute]
    public static void ClearSelectionForPlayer(player whichPlayer) {
      if ((GetLocalPlayer() == whichPlayer)) {
        ClearSelection();
      }
    }

    [NativeLuaMemberAttribute]
    public static void SelectUnitForPlayerSingle(unit whichUnit, player whichPlayer) {
      if ((GetLocalPlayer() == whichPlayer)) {
        ClearSelection();
        SelectUnit(whichUnit, true);
      }
    }

    [NativeLuaMemberAttribute]
    public static void SelectGroupForPlayerBJ(group g, player whichPlayer) {
      if ((GetLocalPlayer() == whichPlayer)) {
        ClearSelection();
        ForGroup(g, SelectGroupBJEnum);
      }
    }

    [NativeLuaMemberAttribute]
    public static void SelectUnitAddForPlayer(unit whichUnit, player whichPlayer) {
      if ((GetLocalPlayer() == whichPlayer)) {
        SelectUnit(whichUnit, true);
      }
    }

    [NativeLuaMemberAttribute]
    public static void SelectUnitRemoveForPlayer(unit whichUnit, player whichPlayer) {
      if ((GetLocalPlayer() == whichPlayer)) {
        SelectUnit(whichUnit, false);
      }
    }

    [NativeLuaMemberAttribute]
    public static void SetUnitLifeBJ(unit whichUnit, float newValue) {
      SetUnitState(whichUnit, UNIT_STATE_LIFE, RMaxBJ(0, newValue));
    }

    [NativeLuaMemberAttribute]
    public static void SetUnitManaBJ(unit whichUnit, float newValue) {
      SetUnitState(whichUnit, UNIT_STATE_MANA, RMaxBJ(0, newValue));
    }

    [NativeLuaMemberAttribute]
    public static void SetUnitLifePercentBJ(unit whichUnit, float percent) {
      SetUnitState(whichUnit, UNIT_STATE_LIFE, GetUnitState(whichUnit, UNIT_STATE_MAX_LIFE) * RMaxBJ(0, percent) * 0.01f);
    }

    [NativeLuaMemberAttribute]
    public static void SetUnitManaPercentBJ(unit whichUnit, float percent) {
      SetUnitState(whichUnit, UNIT_STATE_MANA, GetUnitState(whichUnit, UNIT_STATE_MAX_MANA) * RMaxBJ(0, percent) * 0.01f);
    }

    [NativeLuaMemberAttribute]
    public static bool IsUnitDeadBJ(unit whichUnit) {
      return GetUnitState(whichUnit, UNIT_STATE_LIFE) <= 0;
    }

    [NativeLuaMemberAttribute]
    public static bool IsUnitAliveBJ(unit whichUnit) {
      return !IsUnitDeadBJ(whichUnit);
    }

    [NativeLuaMemberAttribute]
    public static void IsUnitGroupDeadBJEnum() {
      if (!IsUnitDeadBJ(GetEnumUnit())) {
        bj_isUnitGroupDeadResult = false;
      }
    }

    [NativeLuaMemberAttribute]
    public static bool IsUnitGroupDeadBJ(group g) {
      bool wantDestroy = bj_wantDestroyGroup;
      bj_wantDestroyGroup = false;
      bj_isUnitGroupDeadResult = true;
      ForGroup(g, IsUnitGroupDeadBJEnum);
      if ((wantDestroy)) {
        DestroyGroup(g);
      }

      return bj_isUnitGroupDeadResult;
    }

    [NativeLuaMemberAttribute]
    public static void IsUnitGroupEmptyBJEnum() {
      bj_isUnitGroupEmptyResult = false;
    }

    [NativeLuaMemberAttribute]
    public static bool IsUnitGroupEmptyBJ(group g) {
      bool wantDestroy = bj_wantDestroyGroup;
      bj_wantDestroyGroup = false;
      bj_isUnitGroupEmptyResult = true;
      ForGroup(g, IsUnitGroupEmptyBJEnum);
      if ((wantDestroy)) {
        DestroyGroup(g);
      }

      return bj_isUnitGroupEmptyResult;
    }

    [NativeLuaMemberAttribute]
    public static void IsUnitGroupInRectBJEnum() {
      if (!RectContainsUnit(bj_isUnitGroupInRectRect, GetEnumUnit())) {
        bj_isUnitGroupInRectResult = false;
      }
    }

    [NativeLuaMemberAttribute]
    public static bool IsUnitGroupInRectBJ(group g, rect r) {
      bj_isUnitGroupInRectResult = true;
      bj_isUnitGroupInRectRect = r;
      ForGroup(g, IsUnitGroupInRectBJEnum);
      return bj_isUnitGroupInRectResult;
    }

    [NativeLuaMemberAttribute]
    public static bool IsUnitHiddenBJ(unit whichUnit) {
      return IsUnitHidden(whichUnit);
    }

    [NativeLuaMemberAttribute]
    public static void ShowUnitHide(unit whichUnit) {
      ShowUnit(whichUnit, false);
    }

    [NativeLuaMemberAttribute]
    public static void ShowUnitShow(unit whichUnit) {
      if ((IsUnitType(whichUnit, UNIT_TYPE_HERO) && IsUnitDeadBJ(whichUnit))) {
        return;
      }

      ShowUnit(whichUnit, true);
    }

    [NativeLuaMemberAttribute]
    public static bool IssueHauntOrderAtLocBJFilter() {
      return GetUnitTypeId(GetFilterUnit()) == 1852272492;
    }

    [NativeLuaMemberAttribute]
    public static bool IssueHauntOrderAtLocBJ(unit whichPeon, location loc) {
      group g = null;
      unit goldMine = null;
      g = CreateGroup();
      GroupEnumUnitsInRangeOfLoc(g, loc, 2 * bj_CELLWIDTH, filterIssueHauntOrderAtLocBJ);
      goldMine = FirstOfGroup(g);
      DestroyGroup(g);
      if ((goldMine == null)) {
        return false;
      }

      return IssueTargetOrderById(whichPeon, 1969713004, goldMine);
    }

    [NativeLuaMemberAttribute]
    public static bool IssueBuildOrderByIdLocBJ(unit whichPeon, int unitId, location loc) {
      if ((unitId == 1969713004)) {
        return IssueHauntOrderAtLocBJ(whichPeon, loc);
      } else {
        return IssueBuildOrderById(whichPeon, unitId, GetLocationX(loc), GetLocationY(loc));
      }
    }

    [NativeLuaMemberAttribute]
    public static bool IssueTrainOrderByIdBJ(unit whichUnit, int unitId) {
      return IssueImmediateOrderById(whichUnit, unitId);
    }

    [NativeLuaMemberAttribute]
    public static bool GroupTrainOrderByIdBJ(group g, int unitId) {
      return GroupImmediateOrderById(g, unitId);
    }

    [NativeLuaMemberAttribute]
    public static bool IssueUpgradeOrderByIdBJ(unit whichUnit, int techId) {
      return IssueImmediateOrderById(whichUnit, techId);
    }

    [NativeLuaMemberAttribute]
    public static unit GetAttackedUnitBJ() {
      return GetTriggerUnit();
    }

    [NativeLuaMemberAttribute]
    public static void SetUnitFlyHeightBJ(unit whichUnit, float newHeight, float rate) {
      SetUnitFlyHeight(whichUnit, newHeight, rate);
    }

    [NativeLuaMemberAttribute]
    public static void SetUnitTurnSpeedBJ(unit whichUnit, float turnSpeed) {
      SetUnitTurnSpeed(whichUnit, turnSpeed);
    }

    [NativeLuaMemberAttribute]
    public static void SetUnitPropWindowBJ(unit whichUnit, float propWindow) {
      float angle = propWindow;
      if ((angle <= 0)) {
        angle = 1;
      } else if ((angle >= 360)) {
        angle = 359;
      }

      angle = angle * bj_DEGTORAD;
      SetUnitPropWindow(whichUnit, angle);
    }

    [NativeLuaMemberAttribute]
    public static float GetUnitPropWindowBJ(unit whichUnit) {
      return GetUnitPropWindow(whichUnit) * bj_RADTODEG;
    }

    [NativeLuaMemberAttribute]
    public static float GetUnitDefaultPropWindowBJ(unit whichUnit) {
      return GetUnitDefaultPropWindow(whichUnit);
    }

    [NativeLuaMemberAttribute]
    public static void SetUnitBlendTimeBJ(unit whichUnit, float blendTime) {
      SetUnitBlendTime(whichUnit, blendTime);
    }

    [NativeLuaMemberAttribute]
    public static void SetUnitAcquireRangeBJ(unit whichUnit, float acquireRange) {
      SetUnitAcquireRange(whichUnit, acquireRange);
    }

    [NativeLuaMemberAttribute]
    public static void UnitSetCanSleepBJ(unit whichUnit, bool canSleep) {
      UnitAddSleep(whichUnit, canSleep);
    }

    [NativeLuaMemberAttribute]
    public static bool UnitCanSleepBJ(unit whichUnit) {
      return UnitCanSleep(whichUnit);
    }

    [NativeLuaMemberAttribute]
    public static void UnitWakeUpBJ(unit whichUnit) {
      UnitWakeUp(whichUnit);
    }

    [NativeLuaMemberAttribute]
    public static bool UnitIsSleepingBJ(unit whichUnit) {
      return UnitIsSleeping(whichUnit);
    }

    [NativeLuaMemberAttribute]
    public static void WakePlayerUnitsEnum() {
      UnitWakeUp(GetEnumUnit());
    }

    [NativeLuaMemberAttribute]
    public static void WakePlayerUnits(player whichPlayer) {
      group g = CreateGroup();
      GroupEnumUnitsOfPlayer(g, whichPlayer, null);
      ForGroup(g, WakePlayerUnitsEnum);
      DestroyGroup(g);
    }

    [NativeLuaMemberAttribute]
    public static void EnableCreepSleepBJ(bool enable) {
      SetPlayerState(Player(PLAYER_NEUTRAL_AGGRESSIVE), PLAYER_STATE_NO_CREEP_SLEEP, IntegerTertiaryOp(enable, 0, 1));
      if ((!enable)) {
        WakePlayerUnits(Player(PLAYER_NEUTRAL_AGGRESSIVE));
      }
    }

    [NativeLuaMemberAttribute]
    public static bool UnitGenerateAlarms(unit whichUnit, bool generate) {
      return UnitIgnoreAlarm(whichUnit, !generate);
    }

    [NativeLuaMemberAttribute]
    public static bool DoesUnitGenerateAlarms(unit whichUnit) {
      return !UnitIgnoreAlarmToggled(whichUnit);
    }

    [NativeLuaMemberAttribute]
    public static void PauseAllUnitsBJEnum() {
      PauseUnit(GetEnumUnit(), bj_pauseAllUnitsFlag);
    }

    [NativeLuaMemberAttribute]
    public static void PauseAllUnitsBJ(bool pause) {
      int index = default;
      player indexPlayer = default;
      group g = default;
      bj_pauseAllUnitsFlag = pause;
      g = CreateGroup();
      index = 0;
      while (true) {
        indexPlayer = Player(index);
        if ((GetPlayerController(indexPlayer) == MAP_CONTROL_COMPUTER)) {
          PauseCompAI(indexPlayer, pause);
        }

        GroupEnumUnitsOfPlayer(g, indexPlayer, null);
        ForGroup(g, PauseAllUnitsBJEnum);
        GroupClear(g);
        index = index + 1;
        if (index == bj_MAX_PLAYER_SLOTS)
          break;
      }

      DestroyGroup(g);
    }

    [NativeLuaMemberAttribute]
    public static void PauseUnitBJ(bool pause, unit whichUnit) {
      PauseUnit(whichUnit, pause);
    }

    [NativeLuaMemberAttribute]
    public static bool IsUnitPausedBJ(unit whichUnit) {
      return IsUnitPaused(whichUnit);
    }

    [NativeLuaMemberAttribute]
    public static void UnitPauseTimedLifeBJ(bool flag, unit whichUnit) {
      UnitPauseTimedLife(whichUnit, flag);
    }

    [NativeLuaMemberAttribute]
    public static void UnitApplyTimedLifeBJ(float duration, int buffId, unit whichUnit) {
      UnitApplyTimedLife(whichUnit, buffId, duration);
    }

    [NativeLuaMemberAttribute]
    public static void UnitShareVisionBJ(bool share, unit whichUnit, player whichPlayer) {
      UnitShareVision(whichUnit, whichPlayer, share);
    }

    [NativeLuaMemberAttribute]
    public static void UnitRemoveBuffsBJ(int buffType, unit whichUnit) {
      if ((buffType == bj_REMOVEBUFFS_POSITIVE)) {
        UnitRemoveBuffs(whichUnit, true, false);
      } else if ((buffType == bj_REMOVEBUFFS_NEGATIVE)) {
        UnitRemoveBuffs(whichUnit, false, true);
      } else if ((buffType == bj_REMOVEBUFFS_ALL)) {
        UnitRemoveBuffs(whichUnit, true, true);
      } else if ((buffType == bj_REMOVEBUFFS_NONTLIFE)) {
        UnitRemoveBuffsEx(whichUnit, true, true, false, false, false, true, false);
      } else {
      }
    }

    [NativeLuaMemberAttribute]
    public static void UnitRemoveBuffsExBJ(int polarity, int resist, unit whichUnit, bool bTLife, bool bAura) {
      bool bPos = (polarity == bj_BUFF_POLARITY_EITHER) || (polarity == bj_BUFF_POLARITY_POSITIVE);
      bool bNeg = (polarity == bj_BUFF_POLARITY_EITHER) || (polarity == bj_BUFF_POLARITY_NEGATIVE);
      bool bMagic = (resist == bj_BUFF_RESIST_BOTH) || (resist == bj_BUFF_RESIST_MAGIC);
      bool bPhys = (resist == bj_BUFF_RESIST_BOTH) || (resist == bj_BUFF_RESIST_PHYSICAL);
      UnitRemoveBuffsEx(whichUnit, bPos, bNeg, bMagic, bPhys, bTLife, bAura, false);
    }

    [NativeLuaMemberAttribute]
    public static int UnitCountBuffsExBJ(int polarity, int resist, unit whichUnit, bool bTLife, bool bAura) {
      bool bPos = (polarity == bj_BUFF_POLARITY_EITHER) || (polarity == bj_BUFF_POLARITY_POSITIVE);
      bool bNeg = (polarity == bj_BUFF_POLARITY_EITHER) || (polarity == bj_BUFF_POLARITY_NEGATIVE);
      bool bMagic = (resist == bj_BUFF_RESIST_BOTH) || (resist == bj_BUFF_RESIST_MAGIC);
      bool bPhys = (resist == bj_BUFF_RESIST_BOTH) || (resist == bj_BUFF_RESIST_PHYSICAL);
      return UnitCountBuffsEx(whichUnit, bPos, bNeg, bMagic, bPhys, bTLife, bAura, false);
    }

    [NativeLuaMemberAttribute]
    public static bool UnitRemoveAbilityBJ(int abilityId, unit whichUnit) {
      return UnitRemoveAbility(whichUnit, abilityId);
    }

    [NativeLuaMemberAttribute]
    public static bool UnitAddAbilityBJ(int abilityId, unit whichUnit) {
      return UnitAddAbility(whichUnit, abilityId);
    }

    [NativeLuaMemberAttribute]
    public static bool UnitRemoveTypeBJ(unittype whichType, unit whichUnit) {
      return UnitRemoveType(whichUnit, whichType);
    }

    [NativeLuaMemberAttribute]
    public static bool UnitAddTypeBJ(unittype whichType, unit whichUnit) {
      return UnitAddType(whichUnit, whichType);
    }

    [NativeLuaMemberAttribute]
    public static bool UnitMakeAbilityPermanentBJ(bool permanent, int abilityId, unit whichUnit) {
      return UnitMakeAbilityPermanent(whichUnit, permanent, abilityId);
    }

    [NativeLuaMemberAttribute]
    public static void SetUnitExplodedBJ(unit whichUnit, bool exploded) {
      SetUnitExploded(whichUnit, exploded);
    }

    [NativeLuaMemberAttribute]
    public static void ExplodeUnitBJ(unit whichUnit) {
      SetUnitExploded(whichUnit, true);
      KillUnit(whichUnit);
    }

    [NativeLuaMemberAttribute]
    public static unit GetTransportUnitBJ() {
      return GetTransportUnit();
    }

    [NativeLuaMemberAttribute]
    public static unit GetLoadedUnitBJ() {
      return GetLoadedUnit();
    }

    [NativeLuaMemberAttribute]
    public static bool IsUnitInTransportBJ(unit whichUnit, unit whichTransport) {
      return IsUnitInTransport(whichUnit, whichTransport);
    }

    [NativeLuaMemberAttribute]
    public static bool IsUnitLoadedBJ(unit whichUnit) {
      return IsUnitLoaded(whichUnit);
    }

    [NativeLuaMemberAttribute]
    public static bool IsUnitIllusionBJ(unit whichUnit) {
      return IsUnitIllusion(whichUnit);
    }

    [NativeLuaMemberAttribute]
    public static unit ReplaceUnitBJ(unit whichUnit, int newUnitId, int unitStateMethod) {
      unit oldUnit = whichUnit;
      unit newUnit = default;
      bool wasHidden = default;
      int index = default;
      item indexItem = default;
      float oldRatio = default;
      if ((oldUnit == null)) {
        bj_lastReplacedUnit = oldUnit;
        return oldUnit;
      }

      wasHidden = IsUnitHidden(oldUnit);
      ShowUnit(oldUnit, false);
      if ((newUnitId == 1969713004)) {
        newUnit = CreateBlightedGoldmine(GetOwningPlayer(oldUnit), GetUnitX(oldUnit), GetUnitY(oldUnit), GetUnitFacing(oldUnit));
      } else {
        newUnit = CreateUnit(GetOwningPlayer(oldUnit), newUnitId, GetUnitX(oldUnit), GetUnitY(oldUnit), GetUnitFacing(oldUnit));
      }

      if ((unitStateMethod == bj_UNIT_STATE_METHOD_RELATIVE)) {
        if ((GetUnitState(oldUnit, UNIT_STATE_MAX_LIFE) > 0)) {
          oldRatio = GetUnitState(oldUnit, UNIT_STATE_LIFE) / GetUnitState(oldUnit, UNIT_STATE_MAX_LIFE);
          SetUnitState(newUnit, UNIT_STATE_LIFE, oldRatio * GetUnitState(newUnit, UNIT_STATE_MAX_LIFE));
        }

        if ((GetUnitState(oldUnit, UNIT_STATE_MAX_MANA) > 0) && (GetUnitState(newUnit, UNIT_STATE_MAX_MANA) > 0)) {
          oldRatio = GetUnitState(oldUnit, UNIT_STATE_MANA) / GetUnitState(oldUnit, UNIT_STATE_MAX_MANA);
          SetUnitState(newUnit, UNIT_STATE_MANA, oldRatio * GetUnitState(newUnit, UNIT_STATE_MAX_MANA));
        }
      } else if ((unitStateMethod == bj_UNIT_STATE_METHOD_ABSOLUTE)) {
        SetUnitState(newUnit, UNIT_STATE_LIFE, GetUnitState(oldUnit, UNIT_STATE_LIFE));
        if ((GetUnitState(newUnit, UNIT_STATE_MAX_MANA) > 0)) {
          SetUnitState(newUnit, UNIT_STATE_MANA, GetUnitState(oldUnit, UNIT_STATE_MANA));
        }
      } else if ((unitStateMethod == bj_UNIT_STATE_METHOD_DEFAULTS)) {
      } else if ((unitStateMethod == bj_UNIT_STATE_METHOD_MAXIMUM)) {
        SetUnitState(newUnit, UNIT_STATE_LIFE, GetUnitState(newUnit, UNIT_STATE_MAX_LIFE));
        SetUnitState(newUnit, UNIT_STATE_MANA, GetUnitState(newUnit, UNIT_STATE_MAX_MANA));
      } else {
      }

      SetResourceAmount(newUnit, GetResourceAmount(oldUnit));
      if ((IsUnitType(oldUnit, UNIT_TYPE_HERO) && IsUnitType(newUnit, UNIT_TYPE_HERO))) {
        SetHeroXP(newUnit, GetHeroXP(oldUnit), false);
        index = 0;
        while (true) {
          indexItem = UnitItemInSlot(oldUnit, index);
          if ((indexItem != null)) {
            UnitRemoveItem(oldUnit, indexItem);
            UnitAddItem(newUnit, indexItem);
          }

          index = index + 1;
          if (index >= bj_MAX_INVENTORY)
            break;
        }
      }

      if (wasHidden) {
        KillUnit(oldUnit);
        RemoveUnit(oldUnit);
      } else {
        RemoveUnit(oldUnit);
      }

      bj_lastReplacedUnit = newUnit;
      return newUnit;
    }

    [NativeLuaMemberAttribute]
    public static unit GetLastReplacedUnitBJ() {
      return bj_lastReplacedUnit;
    }

    [NativeLuaMemberAttribute]
    public static void SetUnitPositionLocFacingBJ(unit whichUnit, location loc, float facing) {
      SetUnitPositionLoc(whichUnit, loc);
      SetUnitFacing(whichUnit, facing);
    }

    [NativeLuaMemberAttribute]
    public static void SetUnitPositionLocFacingLocBJ(unit whichUnit, location loc, location lookAt) {
      SetUnitPositionLoc(whichUnit, loc);
      SetUnitFacing(whichUnit, AngleBetweenPoints(loc, lookAt));
    }

    [NativeLuaMemberAttribute]
    public static void AddItemToStockBJ(int itemId, unit whichUnit, int currentStock, int stockMax) {
      AddItemToStock(whichUnit, itemId, currentStock, stockMax);
    }

    [NativeLuaMemberAttribute]
    public static void AddUnitToStockBJ(int unitId, unit whichUnit, int currentStock, int stockMax) {
      AddUnitToStock(whichUnit, unitId, currentStock, stockMax);
    }

    [NativeLuaMemberAttribute]
    public static void RemoveItemFromStockBJ(int itemId, unit whichUnit) {
      RemoveItemFromStock(whichUnit, itemId);
    }

    [NativeLuaMemberAttribute]
    public static void RemoveUnitFromStockBJ(int unitId, unit whichUnit) {
      RemoveUnitFromStock(whichUnit, unitId);
    }

    [NativeLuaMemberAttribute]
    public static void SetUnitUseFoodBJ(bool enable, unit whichUnit) {
      SetUnitUseFood(whichUnit, enable);
    }

    [NativeLuaMemberAttribute]
    public static bool UnitDamagePointLoc(unit whichUnit, float delay, float radius, location loc, float amount, attacktype whichAttack, damagetype whichDamage) {
      return UnitDamagePoint(whichUnit, delay, radius, GetLocationX(loc), GetLocationY(loc), amount, true, false, whichAttack, whichDamage, WEAPON_TYPE_WHOKNOWS);
    }

    [NativeLuaMemberAttribute]
    public static bool UnitDamageTargetBJ(unit whichUnit, unit target, float amount, attacktype whichAttack, damagetype whichDamage) {
      return UnitDamageTarget(whichUnit, target, amount, true, false, whichAttack, whichDamage, WEAPON_TYPE_WHOKNOWS);
    }

    [NativeLuaMemberAttribute]
    public static destructable CreateDestructableLoc(int objectid, location loc, float facing, float scale, int variation) {
      bj_lastCreatedDestructable = CreateDestructable(objectid, GetLocationX(loc), GetLocationY(loc), facing, scale, variation);
      return bj_lastCreatedDestructable;
    }

    [NativeLuaMemberAttribute]
    public static destructable CreateDeadDestructableLocBJ(int objectid, location loc, float facing, float scale, int variation) {
      bj_lastCreatedDestructable = CreateDeadDestructable(objectid, GetLocationX(loc), GetLocationY(loc), facing, scale, variation);
      return bj_lastCreatedDestructable;
    }

    [NativeLuaMemberAttribute]
    public static destructable GetLastCreatedDestructable() {
      return bj_lastCreatedDestructable;
    }

    [NativeLuaMemberAttribute]
    public static void ShowDestructableBJ(bool flag, destructable d) {
      ShowDestructable(d, flag);
    }

    [NativeLuaMemberAttribute]
    public static void SetDestructableInvulnerableBJ(destructable d, bool flag) {
      SetDestructableInvulnerable(d, flag);
    }

    [NativeLuaMemberAttribute]
    public static bool IsDestructableInvulnerableBJ(destructable d) {
      return IsDestructableInvulnerable(d);
    }

    [NativeLuaMemberAttribute]
    public static location GetDestructableLoc(destructable whichDestructable) {
      return Location(GetDestructableX(whichDestructable), GetDestructableY(whichDestructable));
    }

    [NativeLuaMemberAttribute]
    public static void EnumDestructablesInRectAll(rect r, System.Action actionFunc) {
      EnumDestructablesInRect(r, null, actionFunc);
    }

    [NativeLuaMemberAttribute]
    public static bool EnumDestructablesInCircleBJFilter() {
      location destLoc = GetDestructableLoc(GetFilterDestructable());
      bool result = default;
      result = DistanceBetweenPoints(destLoc, bj_enumDestructableCenter) <= bj_enumDestructableRadius;
      RemoveLocation(destLoc);
      return result;
    }

    [NativeLuaMemberAttribute]
    public static bool IsDestructableDeadBJ(destructable d) {
      return GetDestructableLife(d) <= 0;
    }

    [NativeLuaMemberAttribute]
    public static bool IsDestructableAliveBJ(destructable d) {
      return !IsDestructableDeadBJ(d);
    }

    [NativeLuaMemberAttribute]
    public static void RandomDestructableInRectBJEnum() {
      bj_destRandomConsidered = bj_destRandomConsidered + 1;
      if ((GetRandomInt(1, bj_destRandomConsidered) == 1)) {
        bj_destRandomCurrentPick = GetEnumDestructable();
      }
    }

    [NativeLuaMemberAttribute]
    public static destructable RandomDestructableInRectBJ(rect r, boolexpr filter) {
      bj_destRandomConsidered = 0;
      bj_destRandomCurrentPick = null;
      EnumDestructablesInRect(r, filter, RandomDestructableInRectBJEnum);
      DestroyBoolExpr(filter);
      return bj_destRandomCurrentPick;
    }

    [NativeLuaMemberAttribute]
    public static destructable RandomDestructableInRectSimpleBJ(rect r) {
      return RandomDestructableInRectBJ(r, null);
    }

    [NativeLuaMemberAttribute]
    public static void EnumDestructablesInCircleBJ(float radius, location loc, System.Action actionFunc) {
      rect r = default;
      if ((radius >= 0)) {
        bj_enumDestructableCenter = loc;
        bj_enumDestructableRadius = radius;
        r = GetRectFromCircleBJ(loc, radius);
        EnumDestructablesInRect(r, filterEnumDestructablesInCircleBJ, actionFunc);
        RemoveRect(r);
      }
    }

    [NativeLuaMemberAttribute]
    public static void SetDestructableLifePercentBJ(destructable d, float percent) {
      SetDestructableLife(d, GetDestructableMaxLife(d) * percent * 0.01f);
    }

    [NativeLuaMemberAttribute]
    public static void SetDestructableMaxLifeBJ(destructable d, float max) {
      SetDestructableMaxLife(d, max);
    }

    [NativeLuaMemberAttribute]
    public static void ModifyGateBJ(int gateOperation, destructable d) {
      if ((gateOperation == bj_GATEOPERATION_CLOSE)) {
        if ((GetDestructableLife(d) <= 0)) {
          DestructableRestoreLife(d, GetDestructableMaxLife(d), true);
        }

        SetDestructableAnimation(d, "stand");
      } else if ((gateOperation == bj_GATEOPERATION_OPEN)) {
        if ((GetDestructableLife(d) > 0)) {
          KillDestructable(d);
        }

        SetDestructableAnimation(d, "death alternate");
      } else if ((gateOperation == bj_GATEOPERATION_DESTROY)) {
        if ((GetDestructableLife(d) > 0)) {
          KillDestructable(d);
        }

        SetDestructableAnimation(d, "death");
      } else {
      }
    }

    [NativeLuaMemberAttribute]
    public static int GetElevatorHeight(destructable d) {
      int height = default;
      height = 1 + R2I(GetDestructableOccluderHeight(d) / bj_CLIFFHEIGHT);
      if ((height < 1) || (height > 3)) {
        height = 1;
      }

      return height;
    }

    [NativeLuaMemberAttribute]
    public static void ChangeElevatorHeight(destructable d, int newHeight) {
      int oldHeight = default;
      newHeight = IMaxBJ(1, newHeight);
      newHeight = IMinBJ(3, newHeight);
      oldHeight = GetElevatorHeight(d);
      SetDestructableOccluderHeight(d, bj_CLIFFHEIGHT * (newHeight - 1));
      if ((newHeight == 1)) {
        if ((oldHeight == 2)) {
          SetDestructableAnimation(d, "birth");
          QueueDestructableAnimation(d, "stand");
        } else if ((oldHeight == 3)) {
          SetDestructableAnimation(d, "birth third");
          QueueDestructableAnimation(d, "stand");
        } else {
          SetDestructableAnimation(d, "stand");
        }
      } else if ((newHeight == 2)) {
        if ((oldHeight == 1)) {
          SetDestructableAnimation(d, "death");
          QueueDestructableAnimation(d, "stand second");
        } else if ((oldHeight == 3)) {
          SetDestructableAnimation(d, "birth second");
          QueueDestructableAnimation(d, "stand second");
        } else {
          SetDestructableAnimation(d, "stand second");
        }
      } else if ((newHeight == 3)) {
        if ((oldHeight == 1)) {
          SetDestructableAnimation(d, "death third");
          QueueDestructableAnimation(d, "stand third");
        } else if ((oldHeight == 2)) {
          SetDestructableAnimation(d, "death second");
          QueueDestructableAnimation(d, "stand third");
        } else {
          SetDestructableAnimation(d, "stand third");
        }
      } else {
      }
    }

    [NativeLuaMemberAttribute]
    public static void NudgeUnitsInRectEnum() {
      unit nudgee = GetEnumUnit();
      SetUnitPosition(nudgee, GetUnitX(nudgee), GetUnitY(nudgee));
    }

    [NativeLuaMemberAttribute]
    public static void NudgeItemsInRectEnum() {
      item nudgee = GetEnumItem();
      SetItemPosition(nudgee, GetItemX(nudgee), GetItemY(nudgee));
    }

    [NativeLuaMemberAttribute]
    public static void NudgeObjectsInRect(rect nudgeArea) {
      group g = default;
      g = CreateGroup();
      GroupEnumUnitsInRect(g, nudgeArea, null);
      ForGroup(g, NudgeUnitsInRectEnum);
      DestroyGroup(g);
      EnumItemsInRect(nudgeArea, null, NudgeItemsInRectEnum);
    }

    [NativeLuaMemberAttribute]
    public static void NearbyElevatorExistsEnum() {
      destructable d = GetEnumDestructable();
      int dType = GetDestructableTypeId(d);
      if ((dType == bj_ELEVATOR_CODE01) || (dType == bj_ELEVATOR_CODE02)) {
        bj_elevatorNeighbor = d;
      }
    }

    [NativeLuaMemberAttribute]
    public static bool NearbyElevatorExists(float x, float y) {
      float findThreshold = 32;
      rect r = default;
      r = Rect(x - findThreshold, y - findThreshold, x + findThreshold, y + findThreshold);
      bj_elevatorNeighbor = null;
      EnumDestructablesInRect(r, null, NearbyElevatorExistsEnum);
      RemoveRect(r);
      return bj_elevatorNeighbor != null;
    }

    [NativeLuaMemberAttribute]
    public static void FindElevatorWallBlockerEnum() {
      bj_elevatorWallBlocker = GetEnumDestructable();
    }

    [NativeLuaMemberAttribute]
    public static void ChangeElevatorWallBlocker(float x, float y, float facing, bool open) {
      destructable blocker = null;
      float findThreshold = 32;
      float nudgeLength = 4.25f * bj_CELLWIDTH;
      float nudgeWidth = 1.25f * bj_CELLWIDTH;
      rect r = default;
      r = Rect(x - findThreshold, y - findThreshold, x + findThreshold, y + findThreshold);
      bj_elevatorWallBlocker = null;
      EnumDestructablesInRect(r, null, FindElevatorWallBlockerEnum);
      RemoveRect(r);
      blocker = bj_elevatorWallBlocker;
      if ((blocker == null)) {
        blocker = CreateDeadDestructable(bj_ELEVATOR_BLOCKER_CODE, x, y, facing, 1, 0);
      } else if ((GetDestructableTypeId(blocker) != bj_ELEVATOR_BLOCKER_CODE)) {
        return;
      }

      if ((open)) {
        if ((GetDestructableLife(blocker) > 0)) {
          KillDestructable(blocker);
        }
      } else {
        if ((GetDestructableLife(blocker) <= 0)) {
          DestructableRestoreLife(blocker, GetDestructableMaxLife(blocker), false);
        }

        if ((facing == 0)) {
          r = Rect(x - nudgeWidth / 2, y - nudgeLength / 2, x + nudgeWidth / 2, y + nudgeLength / 2);
          NudgeObjectsInRect(r);
          RemoveRect(r);
        } else if ((facing == 90)) {
          r = Rect(x - nudgeLength / 2, y - nudgeWidth / 2, x + nudgeLength / 2, y + nudgeWidth / 2);
          NudgeObjectsInRect(r);
          RemoveRect(r);
        } else {
        }
      }
    }

    [NativeLuaMemberAttribute]
    public static void ChangeElevatorWalls(bool open, int walls, destructable d) {
      float x = GetDestructableX(d);
      float y = GetDestructableY(d);
      float distToBlocker = 192;
      float distToNeighbor = 256;
      if ((walls == bj_ELEVATOR_WALL_TYPE_ALL) || (walls == bj_ELEVATOR_WALL_TYPE_EAST)) {
        if ((!NearbyElevatorExists(x + distToNeighbor, y))) {
          ChangeElevatorWallBlocker(x + distToBlocker, y, 0, open);
        }
      }

      if ((walls == bj_ELEVATOR_WALL_TYPE_ALL) || (walls == bj_ELEVATOR_WALL_TYPE_NORTH)) {
        if ((!NearbyElevatorExists(x, y + distToNeighbor))) {
          ChangeElevatorWallBlocker(x, y + distToBlocker, 90, open);
        }
      }

      if ((walls == bj_ELEVATOR_WALL_TYPE_ALL) || (walls == bj_ELEVATOR_WALL_TYPE_SOUTH)) {
        if ((!NearbyElevatorExists(x, y - distToNeighbor))) {
          ChangeElevatorWallBlocker(x, y - distToBlocker, 90, open);
        }
      }

      if ((walls == bj_ELEVATOR_WALL_TYPE_ALL) || (walls == bj_ELEVATOR_WALL_TYPE_WEST)) {
        if ((!NearbyElevatorExists(x - distToNeighbor, y))) {
          ChangeElevatorWallBlocker(x - distToBlocker, y, 0, open);
        }
      }
    }

    [NativeLuaMemberAttribute]
    public static void WaygateActivateBJ(bool activate, unit waygate) {
      WaygateActivate(waygate, activate);
    }

    [NativeLuaMemberAttribute]
    public static bool WaygateIsActiveBJ(unit waygate) {
      return WaygateIsActive(waygate);
    }

    [NativeLuaMemberAttribute]
    public static void WaygateSetDestinationLocBJ(unit waygate, location loc) {
      WaygateSetDestination(waygate, GetLocationX(loc), GetLocationY(loc));
    }

    [NativeLuaMemberAttribute]
    public static location WaygateGetDestinationLocBJ(unit waygate) {
      return Location(WaygateGetDestinationX(waygate), WaygateGetDestinationY(waygate));
    }

    [NativeLuaMemberAttribute]
    public static void UnitSetUsesAltIconBJ(bool flag, unit whichUnit) {
      UnitSetUsesAltIcon(whichUnit, flag);
    }

    [NativeLuaMemberAttribute]
    public static void ForceUIKeyBJ(player whichPlayer, string key) {
      if ((GetLocalPlayer() == whichPlayer)) {
        ForceUIKey(key);
      }
    }

    [NativeLuaMemberAttribute]
    public static void ForceUICancelBJ(player whichPlayer) {
      if ((GetLocalPlayer() == whichPlayer)) {
        ForceUICancel();
      }
    }

    [NativeLuaMemberAttribute]
    public static void ForGroupBJ(group whichGroup, System.Action callback) {
      bool wantDestroy = bj_wantDestroyGroup;
      bj_wantDestroyGroup = false;
      ForGroup(whichGroup, callback);
      if ((wantDestroy)) {
        DestroyGroup(whichGroup);
      }
    }

    [NativeLuaMemberAttribute]
    public static void GroupAddUnitSimple(unit whichUnit, group whichGroup) {
      GroupAddUnit(whichGroup, whichUnit);
    }

    [NativeLuaMemberAttribute]
    public static void GroupRemoveUnitSimple(unit whichUnit, group whichGroup) {
      GroupRemoveUnit(whichGroup, whichUnit);
    }

    [NativeLuaMemberAttribute]
    public static void GroupAddGroupEnum() {
      GroupAddUnit(bj_groupAddGroupDest, GetEnumUnit());
    }

    [NativeLuaMemberAttribute]
    public static void GroupAddGroup(group sourceGroup, group destGroup) {
      bool wantDestroy = bj_wantDestroyGroup;
      bj_wantDestroyGroup = false;
      bj_groupAddGroupDest = destGroup;
      ForGroup(sourceGroup, GroupAddGroupEnum);
      if ((wantDestroy)) {
        DestroyGroup(sourceGroup);
      }
    }

    [NativeLuaMemberAttribute]
    public static void GroupRemoveGroupEnum() {
      GroupRemoveUnit(bj_groupRemoveGroupDest, GetEnumUnit());
    }

    [NativeLuaMemberAttribute]
    public static void GroupRemoveGroup(group sourceGroup, group destGroup) {
      bool wantDestroy = bj_wantDestroyGroup;
      bj_wantDestroyGroup = false;
      bj_groupRemoveGroupDest = destGroup;
      ForGroup(sourceGroup, GroupRemoveGroupEnum);
      if ((wantDestroy)) {
        DestroyGroup(sourceGroup);
      }
    }

    [NativeLuaMemberAttribute]
    public static void ForceAddPlayerSimple(player whichPlayer, force whichForce) {
      ForceAddPlayer(whichForce, whichPlayer);
    }

    [NativeLuaMemberAttribute]
    public static void ForceRemovePlayerSimple(player whichPlayer, force whichForce) {
      ForceRemovePlayer(whichForce, whichPlayer);
    }

    [NativeLuaMemberAttribute]
    public static void GroupPickRandomUnitEnum() {
      bj_groupRandomConsidered = bj_groupRandomConsidered + 1;
      if ((GetRandomInt(1, bj_groupRandomConsidered) == 1)) {
        bj_groupRandomCurrentPick = GetEnumUnit();
      }
    }

    [NativeLuaMemberAttribute]
    public static unit GroupPickRandomUnit(group whichGroup) {
      bool wantDestroy = bj_wantDestroyGroup;
      bj_wantDestroyGroup = false;
      bj_groupRandomConsidered = 0;
      bj_groupRandomCurrentPick = null;
      ForGroup(whichGroup, GroupPickRandomUnitEnum);
      if ((wantDestroy)) {
        DestroyGroup(whichGroup);
      }

      return bj_groupRandomCurrentPick;
    }

    [NativeLuaMemberAttribute]
    public static void ForcePickRandomPlayerEnum() {
      bj_forceRandomConsidered = bj_forceRandomConsidered + 1;
      if ((GetRandomInt(1, bj_forceRandomConsidered) == 1)) {
        bj_forceRandomCurrentPick = GetEnumPlayer();
      }
    }

    [NativeLuaMemberAttribute]
    public static player ForcePickRandomPlayer(force whichForce) {
      bj_forceRandomConsidered = 0;
      bj_forceRandomCurrentPick = null;
      ForForce(whichForce, ForcePickRandomPlayerEnum);
      return bj_forceRandomCurrentPick;
    }

    [NativeLuaMemberAttribute]
    public static void EnumUnitsSelected(player whichPlayer, boolexpr enumFilter, System.Action enumAction) {
      group g = CreateGroup();
      SyncSelections();
      GroupEnumUnitsSelected(g, whichPlayer, enumFilter);
      DestroyBoolExpr(enumFilter);
      ForGroup(g, enumAction);
      DestroyGroup(g);
    }

    [NativeLuaMemberAttribute]
    public static group GetUnitsInRectMatching(rect r, boolexpr filter) {
      group g = CreateGroup();
      GroupEnumUnitsInRect(g, r, filter);
      DestroyBoolExpr(filter);
      return g;
    }

    [NativeLuaMemberAttribute]
    public static group GetUnitsInRectAll(rect r) {
      return GetUnitsInRectMatching(r, null);
    }

    [NativeLuaMemberAttribute]
    public static bool GetUnitsInRectOfPlayerFilter() {
      return GetOwningPlayer(GetFilterUnit()) == bj_groupEnumOwningPlayer;
    }

    [NativeLuaMemberAttribute]
    public static group GetUnitsInRectOfPlayer(rect r, player whichPlayer) {
      group g = CreateGroup();
      bj_groupEnumOwningPlayer = whichPlayer;
      GroupEnumUnitsInRect(g, r, filterGetUnitsInRectOfPlayer);
      return g;
    }

    [NativeLuaMemberAttribute]
    public static group GetUnitsInRangeOfLocMatching(float radius, location whichLocation, boolexpr filter) {
      group g = CreateGroup();
      GroupEnumUnitsInRangeOfLoc(g, whichLocation, radius, filter);
      DestroyBoolExpr(filter);
      return g;
    }

    [NativeLuaMemberAttribute]
    public static group GetUnitsInRangeOfLocAll(float radius, location whichLocation) {
      return GetUnitsInRangeOfLocMatching(radius, whichLocation, null);
    }

    [NativeLuaMemberAttribute]
    public static bool GetUnitsOfTypeIdAllFilter() {
      return GetUnitTypeId(GetFilterUnit()) == bj_groupEnumTypeId;
    }

    [NativeLuaMemberAttribute]
    public static group GetUnitsOfTypeIdAll(int unitid) {
      group result = CreateGroup();
      group g = CreateGroup();
      int index = default;
      index = 0;
      while (true) {
        bj_groupEnumTypeId = unitid;
        GroupClear(g);
        GroupEnumUnitsOfPlayer(g, Player(index), filterGetUnitsOfTypeIdAll);
        GroupAddGroup(g, result);
        index = index + 1;
        if (index == bj_MAX_PLAYER_SLOTS)
          break;
      }

      DestroyGroup(g);
      return result;
    }

    [NativeLuaMemberAttribute]
    public static group GetUnitsOfPlayerMatching(player whichPlayer, boolexpr filter) {
      group g = CreateGroup();
      GroupEnumUnitsOfPlayer(g, whichPlayer, filter);
      DestroyBoolExpr(filter);
      return g;
    }

    [NativeLuaMemberAttribute]
    public static group GetUnitsOfPlayerAll(player whichPlayer) {
      return GetUnitsOfPlayerMatching(whichPlayer, null);
    }

    [NativeLuaMemberAttribute]
    public static bool GetUnitsOfPlayerAndTypeIdFilter() {
      return GetUnitTypeId(GetFilterUnit()) == bj_groupEnumTypeId;
    }

    [NativeLuaMemberAttribute]
    public static group GetUnitsOfPlayerAndTypeId(player whichPlayer, int unitid) {
      group g = CreateGroup();
      bj_groupEnumTypeId = unitid;
      GroupEnumUnitsOfPlayer(g, whichPlayer, filterGetUnitsOfPlayerAndTypeId);
      return g;
    }

    [NativeLuaMemberAttribute]
    public static group GetUnitsSelectedAll(player whichPlayer) {
      group g = CreateGroup();
      SyncSelections();
      GroupEnumUnitsSelected(g, whichPlayer, null);
      return g;
    }

    [NativeLuaMemberAttribute]
    public static force GetForceOfPlayer(player whichPlayer) {
      force f = CreateForce();
      ForceAddPlayer(f, whichPlayer);
      return f;
    }

    [NativeLuaMemberAttribute]
    public static force GetPlayersAll() {
      return bj_FORCE_ALL_PLAYERS;
    }

    [NativeLuaMemberAttribute]
    public static force GetPlayersByMapControl(mapcontrol whichControl) {
      force f = CreateForce();
      int playerIndex = default;
      player indexPlayer = default;
      playerIndex = 0;
      while (true) {
        indexPlayer = Player(playerIndex);
        if (GetPlayerController(indexPlayer) == whichControl) {
          ForceAddPlayer(f, indexPlayer);
        }

        playerIndex = playerIndex + 1;
        if (playerIndex == bj_MAX_PLAYER_SLOTS)
          break;
      }

      return f;
    }

    [NativeLuaMemberAttribute]
    public static force GetPlayersAllies(player whichPlayer) {
      force f = CreateForce();
      ForceEnumAllies(f, whichPlayer, null);
      return f;
    }

    [NativeLuaMemberAttribute]
    public static force GetPlayersEnemies(player whichPlayer) {
      force f = CreateForce();
      ForceEnumEnemies(f, whichPlayer, null);
      return f;
    }

    [NativeLuaMemberAttribute]
    public static force GetPlayersMatching(boolexpr filter) {
      force f = CreateForce();
      ForceEnumPlayers(f, filter);
      DestroyBoolExpr(filter);
      return f;
    }

    [NativeLuaMemberAttribute]
    public static void CountUnitsInGroupEnum() {
      bj_groupCountUnits = bj_groupCountUnits + 1;
    }

    [NativeLuaMemberAttribute]
    public static int CountUnitsInGroup(group g) {
      bool wantDestroy = bj_wantDestroyGroup;
      bj_wantDestroyGroup = false;
      bj_groupCountUnits = 0;
      ForGroup(g, CountUnitsInGroupEnum);
      if ((wantDestroy)) {
        DestroyGroup(g);
      }

      return bj_groupCountUnits;
    }

    [NativeLuaMemberAttribute]
    public static void CountPlayersInForceEnum() {
      bj_forceCountPlayers = bj_forceCountPlayers + 1;
    }

    [NativeLuaMemberAttribute]
    public static int CountPlayersInForceBJ(force f) {
      bj_forceCountPlayers = 0;
      ForForce(f, CountPlayersInForceEnum);
      return bj_forceCountPlayers;
    }

    [NativeLuaMemberAttribute]
    public static void GetRandomSubGroupEnum() {
      if ((bj_randomSubGroupWant > 0)) {
        if ((bj_randomSubGroupWant >= bj_randomSubGroupTotal) || (GetRandomReal(0, 1) < bj_randomSubGroupChance)) {
          GroupAddUnit(bj_randomSubGroupGroup, GetEnumUnit());
          bj_randomSubGroupWant = bj_randomSubGroupWant - 1;
        }
      }

      bj_randomSubGroupTotal = bj_randomSubGroupTotal - 1;
    }

    [NativeLuaMemberAttribute]
    public static group GetRandomSubGroup(int count, group sourceGroup) {
      group g = CreateGroup();
      bj_randomSubGroupGroup = g;
      bj_randomSubGroupWant = count;
      bj_randomSubGroupTotal = CountUnitsInGroup(sourceGroup);
      if ((bj_randomSubGroupWant <= 0 || bj_randomSubGroupTotal <= 0)) {
        return g;
      }

      bj_randomSubGroupChance = I2R(bj_randomSubGroupWant) / I2R(bj_randomSubGroupTotal);
      ForGroup(sourceGroup, GetRandomSubGroupEnum);
      return g;
    }

    [NativeLuaMemberAttribute]
    public static bool LivingPlayerUnitsOfTypeIdFilter() {
      unit filterUnit = GetFilterUnit();
      return IsUnitAliveBJ(filterUnit) && GetUnitTypeId(filterUnit) == bj_livingPlayerUnitsTypeId;
    }

    [NativeLuaMemberAttribute]
    public static int CountLivingPlayerUnitsOfTypeId(int unitId, player whichPlayer) {
      group g = default;
      int matchedCount = default;
      g = CreateGroup();
      bj_livingPlayerUnitsTypeId = unitId;
      GroupEnumUnitsOfPlayer(g, whichPlayer, filterLivingPlayerUnitsOfTypeId);
      matchedCount = CountUnitsInGroup(g);
      DestroyGroup(g);
      return matchedCount;
    }

    [NativeLuaMemberAttribute]
    public static void ResetUnitAnimation(unit whichUnit) {
      SetUnitAnimation(whichUnit, "stand");
    }

    [NativeLuaMemberAttribute]
    public static void SetUnitTimeScalePercent(unit whichUnit, float percentScale) {
      SetUnitTimeScale(whichUnit, percentScale * 0.01f);
    }

    [NativeLuaMemberAttribute]
    public static void SetUnitScalePercent(unit whichUnit, float percentScaleX, float percentScaleY, float percentScaleZ) {
      SetUnitScale(whichUnit, percentScaleX * 0.01f, percentScaleY * 0.01f, percentScaleZ * 0.01f);
    }

    [NativeLuaMemberAttribute]
    public static void SetUnitVertexColorBJ(unit whichUnit, float red, float green, float blue, float transparency) {
      SetUnitVertexColor(whichUnit, PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0f - transparency));
    }

    [NativeLuaMemberAttribute]
    public static void UnitAddIndicatorBJ(unit whichUnit, float red, float green, float blue, float transparency) {
      AddIndicator(whichUnit, PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0f - transparency));
    }

    [NativeLuaMemberAttribute]
    public static void DestructableAddIndicatorBJ(destructable whichDestructable, float red, float green, float blue, float transparency) {
      AddIndicator(whichDestructable, PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0f - transparency));
    }

    [NativeLuaMemberAttribute]
    public static void ItemAddIndicatorBJ(item whichItem, float red, float green, float blue, float transparency) {
      AddIndicator(whichItem, PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0f - transparency));
    }

    [NativeLuaMemberAttribute]
    public static void SetUnitFacingToFaceLocTimed(unit whichUnit, location target, float duration) {
      location unitLoc = GetUnitLoc(whichUnit);
      SetUnitFacingTimed(whichUnit, AngleBetweenPoints(unitLoc, target), duration);
      RemoveLocation(unitLoc);
    }

    [NativeLuaMemberAttribute]
    public static void SetUnitFacingToFaceUnitTimed(unit whichUnit, unit target, float duration) {
      location unitLoc = GetUnitLoc(target);
      SetUnitFacingToFaceLocTimed(whichUnit, unitLoc, duration);
      RemoveLocation(unitLoc);
    }

    [NativeLuaMemberAttribute]
    public static void QueueUnitAnimationBJ(unit whichUnit, string whichAnimation) {
      QueueUnitAnimation(whichUnit, whichAnimation);
    }

    [NativeLuaMemberAttribute]
    public static void SetDestructableAnimationBJ(destructable d, string whichAnimation) {
      SetDestructableAnimation(d, whichAnimation);
    }

    [NativeLuaMemberAttribute]
    public static void QueueDestructableAnimationBJ(destructable d, string whichAnimation) {
      QueueDestructableAnimation(d, whichAnimation);
    }

    [NativeLuaMemberAttribute]
    public static void SetDestAnimationSpeedPercent(destructable d, float percentScale) {
      SetDestructableAnimationSpeed(d, percentScale * 0.01f);
    }

    [NativeLuaMemberAttribute]
    public static void DialogDisplayBJ(bool flag, dialog whichDialog, player whichPlayer) {
      DialogDisplay(whichPlayer, whichDialog, flag);
    }

    [NativeLuaMemberAttribute]
    public static void DialogSetMessageBJ(dialog whichDialog, string message) {
      DialogSetMessage(whichDialog, message);
    }

    [NativeLuaMemberAttribute]
    public static button DialogAddButtonBJ(dialog whichDialog, string buttonText) {
      bj_lastCreatedButton = DialogAddButton(whichDialog, buttonText, 0);
      return bj_lastCreatedButton;
    }

    [NativeLuaMemberAttribute]
    public static button DialogAddButtonWithHotkeyBJ(dialog whichDialog, string buttonText, int hotkey) {
      bj_lastCreatedButton = DialogAddButton(whichDialog, buttonText, hotkey);
      return bj_lastCreatedButton;
    }

    [NativeLuaMemberAttribute]
    public static void DialogClearBJ(dialog whichDialog) {
      DialogClear(whichDialog);
    }

    [NativeLuaMemberAttribute]
    public static button GetLastCreatedButtonBJ() {
      return bj_lastCreatedButton;
    }

    [NativeLuaMemberAttribute]
    public static button GetClickedButtonBJ() {
      return GetClickedButton();
    }

    [NativeLuaMemberAttribute]
    public static dialog GetClickedDialogBJ() {
      return GetClickedDialog();
    }

    [NativeLuaMemberAttribute]
    public static void SetPlayerAllianceBJ(player sourcePlayer, alliancetype whichAllianceSetting, bool value, player otherPlayer) {
      if ((sourcePlayer == otherPlayer)) {
        return;
      }

      SetPlayerAlliance(sourcePlayer, otherPlayer, whichAllianceSetting, value);
    }

    [NativeLuaMemberAttribute]
    public static void SetPlayerAllianceStateAllyBJ(player sourcePlayer, player otherPlayer, bool flag) {
      SetPlayerAlliance(sourcePlayer, otherPlayer, ALLIANCE_PASSIVE, flag);
      SetPlayerAlliance(sourcePlayer, otherPlayer, ALLIANCE_HELP_REQUEST, flag);
      SetPlayerAlliance(sourcePlayer, otherPlayer, ALLIANCE_HELP_RESPONSE, flag);
      SetPlayerAlliance(sourcePlayer, otherPlayer, ALLIANCE_SHARED_XP, flag);
      SetPlayerAlliance(sourcePlayer, otherPlayer, ALLIANCE_SHARED_SPELLS, flag);
    }

    [NativeLuaMemberAttribute]
    public static void SetPlayerAllianceStateVisionBJ(player sourcePlayer, player otherPlayer, bool flag) {
      SetPlayerAlliance(sourcePlayer, otherPlayer, ALLIANCE_SHARED_VISION, flag);
    }

    [NativeLuaMemberAttribute]
    public static void SetPlayerAllianceStateControlBJ(player sourcePlayer, player otherPlayer, bool flag) {
      SetPlayerAlliance(sourcePlayer, otherPlayer, ALLIANCE_SHARED_CONTROL, flag);
    }

    [NativeLuaMemberAttribute]
    public static void SetPlayerAllianceStateFullControlBJ(player sourcePlayer, player otherPlayer, bool flag) {
      SetPlayerAlliance(sourcePlayer, otherPlayer, ALLIANCE_SHARED_ADVANCED_CONTROL, flag);
    }

    [NativeLuaMemberAttribute]
    public static void SetPlayerAllianceStateBJ(player sourcePlayer, player otherPlayer, int allianceState) {
      if ((sourcePlayer == otherPlayer)) {
        return;
      }

      if (allianceState == bj_ALLIANCE_UNALLIED) {
        SetPlayerAllianceStateAllyBJ(sourcePlayer, otherPlayer, false);
        SetPlayerAllianceStateVisionBJ(sourcePlayer, otherPlayer, false);
        SetPlayerAllianceStateControlBJ(sourcePlayer, otherPlayer, false);
        SetPlayerAllianceStateFullControlBJ(sourcePlayer, otherPlayer, false);
      } else if (allianceState == bj_ALLIANCE_UNALLIED_VISION) {
        SetPlayerAllianceStateAllyBJ(sourcePlayer, otherPlayer, false);
        SetPlayerAllianceStateVisionBJ(sourcePlayer, otherPlayer, true);
        SetPlayerAllianceStateControlBJ(sourcePlayer, otherPlayer, false);
        SetPlayerAllianceStateFullControlBJ(sourcePlayer, otherPlayer, false);
      } else if (allianceState == bj_ALLIANCE_ALLIED) {
        SetPlayerAllianceStateAllyBJ(sourcePlayer, otherPlayer, true);
        SetPlayerAllianceStateVisionBJ(sourcePlayer, otherPlayer, false);
        SetPlayerAllianceStateControlBJ(sourcePlayer, otherPlayer, false);
        SetPlayerAllianceStateFullControlBJ(sourcePlayer, otherPlayer, false);
      } else if (allianceState == bj_ALLIANCE_ALLIED_VISION) {
        SetPlayerAllianceStateAllyBJ(sourcePlayer, otherPlayer, true);
        SetPlayerAllianceStateVisionBJ(sourcePlayer, otherPlayer, true);
        SetPlayerAllianceStateControlBJ(sourcePlayer, otherPlayer, false);
        SetPlayerAllianceStateFullControlBJ(sourcePlayer, otherPlayer, false);
      } else if (allianceState == bj_ALLIANCE_ALLIED_UNITS) {
        SetPlayerAllianceStateAllyBJ(sourcePlayer, otherPlayer, true);
        SetPlayerAllianceStateVisionBJ(sourcePlayer, otherPlayer, true);
        SetPlayerAllianceStateControlBJ(sourcePlayer, otherPlayer, true);
        SetPlayerAllianceStateFullControlBJ(sourcePlayer, otherPlayer, false);
      } else if (allianceState == bj_ALLIANCE_ALLIED_ADVUNITS) {
        SetPlayerAllianceStateAllyBJ(sourcePlayer, otherPlayer, true);
        SetPlayerAllianceStateVisionBJ(sourcePlayer, otherPlayer, true);
        SetPlayerAllianceStateControlBJ(sourcePlayer, otherPlayer, true);
        SetPlayerAllianceStateFullControlBJ(sourcePlayer, otherPlayer, true);
      } else if (allianceState == bj_ALLIANCE_NEUTRAL) {
        SetPlayerAllianceStateAllyBJ(sourcePlayer, otherPlayer, false);
        SetPlayerAllianceStateVisionBJ(sourcePlayer, otherPlayer, false);
        SetPlayerAllianceStateControlBJ(sourcePlayer, otherPlayer, false);
        SetPlayerAllianceStateFullControlBJ(sourcePlayer, otherPlayer, false);
        SetPlayerAlliance(sourcePlayer, otherPlayer, ALLIANCE_PASSIVE, true);
      } else if (allianceState == bj_ALLIANCE_NEUTRAL_VISION) {
        SetPlayerAllianceStateAllyBJ(sourcePlayer, otherPlayer, false);
        SetPlayerAllianceStateVisionBJ(sourcePlayer, otherPlayer, true);
        SetPlayerAllianceStateControlBJ(sourcePlayer, otherPlayer, false);
        SetPlayerAllianceStateFullControlBJ(sourcePlayer, otherPlayer, false);
        SetPlayerAlliance(sourcePlayer, otherPlayer, ALLIANCE_PASSIVE, true);
      } else {
      }
    }

    [NativeLuaMemberAttribute]
    public static void SetForceAllianceStateBJ(force sourceForce, force targetForce, int allianceState) {
      int sourceIndex = default;
      int targetIndex = default;
      sourceIndex = 0;
      while (true) {
        if ((sourceForce == bj_FORCE_ALL_PLAYERS || IsPlayerInForce(Player(sourceIndex), sourceForce))) {
          targetIndex = 0;
          while (true) {
            if ((targetForce == bj_FORCE_ALL_PLAYERS || IsPlayerInForce(Player(targetIndex), targetForce))) {
              SetPlayerAllianceStateBJ(Player(sourceIndex), Player(targetIndex), allianceState);
            }

            targetIndex = targetIndex + 1;
            if (targetIndex == bj_MAX_PLAYER_SLOTS)
              break;
          }
        }

        sourceIndex = sourceIndex + 1;
        if (sourceIndex == bj_MAX_PLAYER_SLOTS)
          break;
      }
    }

    [NativeLuaMemberAttribute]
    public static bool PlayersAreCoAllied(player playerA, player playerB) {
      if ((playerA == playerB)) {
        return true;
      }

      if (GetPlayerAlliance(playerA, playerB, ALLIANCE_PASSIVE)) {
        if (GetPlayerAlliance(playerB, playerA, ALLIANCE_PASSIVE)) {
          return true;
        }
      }

      return false;
    }

    [NativeLuaMemberAttribute]
    public static void ShareEverythingWithTeamAI(player whichPlayer) {
      int playerIndex = default;
      player indexPlayer = default;
      playerIndex = 0;
      while (true) {
        indexPlayer = Player(playerIndex);
        if ((PlayersAreCoAllied(whichPlayer, indexPlayer) && whichPlayer != indexPlayer)) {
          if ((GetPlayerController(indexPlayer) == MAP_CONTROL_COMPUTER)) {
            SetPlayerAlliance(whichPlayer, indexPlayer, ALLIANCE_SHARED_VISION, true);
            SetPlayerAlliance(whichPlayer, indexPlayer, ALLIANCE_SHARED_CONTROL, true);
            SetPlayerAlliance(whichPlayer, indexPlayer, ALLIANCE_SHARED_ADVANCED_CONTROL, true);
          }
        }

        playerIndex = playerIndex + 1;
        if (playerIndex == bj_MAX_PLAYERS)
          break;
      }
    }

    [NativeLuaMemberAttribute]
    public static void ShareEverythingWithTeam(player whichPlayer) {
      int playerIndex = default;
      player indexPlayer = default;
      playerIndex = 0;
      while (true) {
        indexPlayer = Player(playerIndex);
        if ((PlayersAreCoAllied(whichPlayer, indexPlayer) && whichPlayer != indexPlayer)) {
          SetPlayerAlliance(whichPlayer, indexPlayer, ALLIANCE_SHARED_VISION, true);
          SetPlayerAlliance(whichPlayer, indexPlayer, ALLIANCE_SHARED_CONTROL, true);
          SetPlayerAlliance(indexPlayer, whichPlayer, ALLIANCE_SHARED_CONTROL, true);
          SetPlayerAlliance(whichPlayer, indexPlayer, ALLIANCE_SHARED_ADVANCED_CONTROL, true);
        }

        playerIndex = playerIndex + 1;
        if (playerIndex == bj_MAX_PLAYERS)
          break;
      }
    }

    [NativeLuaMemberAttribute]
    public static void ConfigureNeutralVictim() {
      int index = default;
      player indexPlayer = default;
      player neutralVictim = Player(bj_PLAYER_NEUTRAL_VICTIM);
      index = 0;
      while (true) {
        indexPlayer = Player(index);
        SetPlayerAlliance(neutralVictim, indexPlayer, ALLIANCE_PASSIVE, true);
        SetPlayerAlliance(indexPlayer, neutralVictim, ALLIANCE_PASSIVE, false);
        index = index + 1;
        if (index == bj_MAX_PLAYERS)
          break;
      }

      indexPlayer = Player(PLAYER_NEUTRAL_AGGRESSIVE);
      SetPlayerAlliance(neutralVictim, indexPlayer, ALLIANCE_PASSIVE, true);
      SetPlayerAlliance(indexPlayer, neutralVictim, ALLIANCE_PASSIVE, true);
      SetPlayerState(neutralVictim, PLAYER_STATE_GIVES_BOUNTY, 0);
    }

    [NativeLuaMemberAttribute]
    public static void MakeUnitsPassiveForPlayerEnum() {
      SetUnitOwner(GetEnumUnit(), Player(bj_PLAYER_NEUTRAL_VICTIM), false);
    }

    [NativeLuaMemberAttribute]
    public static void MakeUnitsPassiveForPlayer(player whichPlayer) {
      group playerUnits = CreateGroup();
      CachePlayerHeroData(whichPlayer);
      GroupEnumUnitsOfPlayer(playerUnits, whichPlayer, null);
      ForGroup(playerUnits, MakeUnitsPassiveForPlayerEnum);
      DestroyGroup(playerUnits);
    }

    [NativeLuaMemberAttribute]
    public static void MakeUnitsPassiveForTeam(player whichPlayer) {
      int playerIndex = default;
      player indexPlayer = default;
      playerIndex = 0;
      while (true) {
        indexPlayer = Player(playerIndex);
        if (PlayersAreCoAllied(whichPlayer, indexPlayer)) {
          MakeUnitsPassiveForPlayer(indexPlayer);
        }

        playerIndex = playerIndex + 1;
        if (playerIndex == bj_MAX_PLAYERS)
          break;
      }
    }

    [NativeLuaMemberAttribute]
    public static bool AllowVictoryDefeat(playergameresult gameResult) {
      if ((gameResult == PLAYER_GAME_RESULT_VICTORY)) {
        return !IsNoVictoryCheat();
      }

      if ((gameResult == PLAYER_GAME_RESULT_DEFEAT)) {
        return !IsNoDefeatCheat();
      }

      if ((gameResult == PLAYER_GAME_RESULT_NEUTRAL)) {
        return (!IsNoVictoryCheat()) && (!IsNoDefeatCheat());
      }

      return true;
    }

    [NativeLuaMemberAttribute]
    public static void EndGameBJ() {
      EndGame(true);
    }

    [NativeLuaMemberAttribute]
    public static void MeleeVictoryDialogBJ(player whichPlayer, bool leftGame) {
      trigger t = CreateTrigger();
      dialog d = DialogCreate();
      string formatString = default;
      if ((leftGame)) {
        formatString = GetLocalizedString("PLAYER_LEFT_GAME");
      } else {
        formatString = GetLocalizedString("PLAYER_VICTORIOUS");
      }

      DisplayTimedTextFromPlayer(whichPlayer, 0, 0, 60, formatString);
      DialogSetMessage(d, GetLocalizedString("GAMEOVER_VICTORY_MSG"));
      DialogAddButton(d, GetLocalizedString("GAMEOVER_CONTINUE_GAME"), GetLocalizedHotkey("GAMEOVER_CONTINUE_GAME"));
      t = CreateTrigger();
      TriggerRegisterDialogButtonEvent(t, DialogAddQuitButton(d, true, GetLocalizedString("GAMEOVER_QUIT_GAME"), GetLocalizedHotkey("GAMEOVER_QUIT_GAME")));
      DialogDisplay(whichPlayer, d, true);
      StartSoundForPlayerBJ(whichPlayer, bj_victoryDialogSound);
    }

    [NativeLuaMemberAttribute]
    public static void MeleeDefeatDialogBJ(player whichPlayer, bool leftGame) {
      trigger t = CreateTrigger();
      dialog d = DialogCreate();
      string formatString = default;
      if ((leftGame)) {
        formatString = GetLocalizedString("PLAYER_LEFT_GAME");
      } else {
        formatString = GetLocalizedString("PLAYER_DEFEATED");
      }

      DisplayTimedTextFromPlayer(whichPlayer, 0, 0, 60, formatString);
      DialogSetMessage(d, GetLocalizedString("GAMEOVER_DEFEAT_MSG"));
      if ((!bj_meleeGameOver && IsMapFlagSet(MAP_OBSERVERS_ON_DEATH))) {
        DialogAddButton(d, GetLocalizedString("GAMEOVER_CONTINUE_OBSERVING"), GetLocalizedHotkey("GAMEOVER_CONTINUE_OBSERVING"));
      }

      t = CreateTrigger();
      TriggerRegisterDialogButtonEvent(t, DialogAddQuitButton(d, true, GetLocalizedString("GAMEOVER_QUIT_GAME"), GetLocalizedHotkey("GAMEOVER_QUIT_GAME")));
      DialogDisplay(whichPlayer, d, true);
      StartSoundForPlayerBJ(whichPlayer, bj_defeatDialogSound);
    }

    [NativeLuaMemberAttribute]
    public static void GameOverDialogBJ(player whichPlayer, bool leftGame) {
      trigger t = CreateTrigger();
      dialog d = DialogCreate();
      string s = default;
      DisplayTimedTextFromPlayer(whichPlayer, 0, 0, 60, GetLocalizedString("PLAYER_LEFT_GAME"));
      if ((GetIntegerGameState(GAME_STATE_DISCONNECTED) != 0)) {
        s = GetLocalizedString("GAMEOVER_DISCONNECTED");
      } else {
        s = GetLocalizedString("GAMEOVER_GAME_OVER");
      }

      DialogSetMessage(d, s);
      t = CreateTrigger();
      TriggerRegisterDialogButtonEvent(t, DialogAddQuitButton(d, true, GetLocalizedString("GAMEOVER_OK"), GetLocalizedHotkey("GAMEOVER_OK")));
      DialogDisplay(whichPlayer, d, true);
      StartSoundForPlayerBJ(whichPlayer, bj_defeatDialogSound);
    }

    [NativeLuaMemberAttribute]
    public static void RemovePlayerPreserveUnitsBJ(player whichPlayer, playergameresult gameResult, bool leftGame) {
      if (AllowVictoryDefeat(gameResult)) {
        RemovePlayer(whichPlayer, gameResult);
        if ((gameResult == PLAYER_GAME_RESULT_VICTORY)) {
          MeleeVictoryDialogBJ(whichPlayer, leftGame);
          return;
        } else if ((gameResult == PLAYER_GAME_RESULT_DEFEAT)) {
          MeleeDefeatDialogBJ(whichPlayer, leftGame);
        } else {
          GameOverDialogBJ(whichPlayer, leftGame);
        }
      }
    }

    [NativeLuaMemberAttribute]
    public static void CustomVictoryOkBJ() {
      if (bj_isSinglePlayer) {
        PauseGame(false);
        SetGameDifficulty(GetDefaultDifficulty());
      }

      if ((bj_changeLevelMapName == null)) {
        EndGame(bj_changeLevelShowScores);
      } else {
        ChangeLevel(bj_changeLevelMapName, bj_changeLevelShowScores);
      }
    }

    [NativeLuaMemberAttribute]
    public static void CustomVictoryQuitBJ() {
      if (bj_isSinglePlayer) {
        PauseGame(false);
        SetGameDifficulty(GetDefaultDifficulty());
      }

      EndGame(bj_changeLevelShowScores);
    }

    [NativeLuaMemberAttribute]
    public static void CustomVictoryDialogBJ(player whichPlayer) {
      trigger t = CreateTrigger();
      dialog d = DialogCreate();
      DialogSetMessage(d, GetLocalizedString("GAMEOVER_VICTORY_MSG"));
      t = CreateTrigger();
      TriggerRegisterDialogButtonEvent(t, DialogAddButton(d, GetLocalizedString("GAMEOVER_CONTINUE"), GetLocalizedHotkey("GAMEOVER_CONTINUE")));
      TriggerAddAction(t, CustomVictoryOkBJ);
      t = CreateTrigger();
      TriggerRegisterDialogButtonEvent(t, DialogAddButton(d, GetLocalizedString("GAMEOVER_QUIT_MISSION"), GetLocalizedHotkey("GAMEOVER_QUIT_MISSION")));
      TriggerAddAction(t, CustomVictoryQuitBJ);
      if ((GetLocalPlayer() == whichPlayer)) {
        EnableUserControl(true);
        if (bj_isSinglePlayer) {
          PauseGame(true);
        }

        EnableUserUI(false);
      }

      DialogDisplay(whichPlayer, d, true);
      VolumeGroupSetVolumeForPlayerBJ(whichPlayer, SOUND_VOLUMEGROUP_UI, 1.0f);
      StartSoundForPlayerBJ(whichPlayer, bj_victoryDialogSound);
    }

    [NativeLuaMemberAttribute]
    public static void CustomVictorySkipBJ(player whichPlayer) {
      if ((GetLocalPlayer() == whichPlayer)) {
        if (bj_isSinglePlayer) {
          SetGameDifficulty(GetDefaultDifficulty());
        }

        if ((bj_changeLevelMapName == null)) {
          EndGame(bj_changeLevelShowScores);
        } else {
          ChangeLevel(bj_changeLevelMapName, bj_changeLevelShowScores);
        }
      }
    }

    [NativeLuaMemberAttribute]
    public static void CustomVictoryBJ(player whichPlayer, bool showDialog, bool showScores) {
      if (AllowVictoryDefeat(PLAYER_GAME_RESULT_VICTORY)) {
        RemovePlayer(whichPlayer, PLAYER_GAME_RESULT_VICTORY);
        if (!bj_isSinglePlayer) {
          DisplayTimedTextFromPlayer(whichPlayer, 0, 0, 60, GetLocalizedString("PLAYER_VICTORIOUS"));
        }

        if ((GetPlayerController(whichPlayer) == MAP_CONTROL_USER)) {
          bj_changeLevelShowScores = showScores;
          if (showDialog) {
            CustomVictoryDialogBJ(whichPlayer);
          } else {
            CustomVictorySkipBJ(whichPlayer);
          }
        }
      }
    }

    [NativeLuaMemberAttribute]
    public static void CustomDefeatRestartBJ() {
      PauseGame(false);
      RestartGame(true);
    }

    [NativeLuaMemberAttribute]
    public static void CustomDefeatReduceDifficultyBJ() {
      gamedifficulty diff = GetGameDifficulty();
      PauseGame(false);
      if ((diff == MAP_DIFFICULTY_EASY)) {
      } else if ((diff == MAP_DIFFICULTY_NORMAL)) {
        SetGameDifficulty(MAP_DIFFICULTY_EASY);
      } else if ((diff == MAP_DIFFICULTY_HARD)) {
        SetGameDifficulty(MAP_DIFFICULTY_NORMAL);
      } else {
      }

      RestartGame(true);
    }

    [NativeLuaMemberAttribute]
    public static void CustomDefeatLoadBJ() {
      PauseGame(false);
      DisplayLoadDialog();
    }

    [NativeLuaMemberAttribute]
    public static void CustomDefeatQuitBJ() {
      if (bj_isSinglePlayer) {
        PauseGame(false);
      }

      SetGameDifficulty(GetDefaultDifficulty());
      EndGame(true);
    }

    [NativeLuaMemberAttribute]
    public static void CustomDefeatDialogBJ(player whichPlayer, string message) {
      trigger t = CreateTrigger();
      dialog d = DialogCreate();
      DialogSetMessage(d, message);
      if (bj_isSinglePlayer) {
        t = CreateTrigger();
        TriggerRegisterDialogButtonEvent(t, DialogAddButton(d, GetLocalizedString("GAMEOVER_RESTART"), GetLocalizedHotkey("GAMEOVER_RESTART")));
        TriggerAddAction(t, CustomDefeatRestartBJ);
        if ((GetGameDifficulty() != MAP_DIFFICULTY_EASY)) {
          t = CreateTrigger();
          TriggerRegisterDialogButtonEvent(t, DialogAddButton(d, GetLocalizedString("GAMEOVER_REDUCE_DIFFICULTY"), GetLocalizedHotkey("GAMEOVER_REDUCE_DIFFICULTY")));
          TriggerAddAction(t, CustomDefeatReduceDifficultyBJ);
        }

        t = CreateTrigger();
        TriggerRegisterDialogButtonEvent(t, DialogAddButton(d, GetLocalizedString("GAMEOVER_LOAD"), GetLocalizedHotkey("GAMEOVER_LOAD")));
        TriggerAddAction(t, CustomDefeatLoadBJ);
      }

      t = CreateTrigger();
      TriggerRegisterDialogButtonEvent(t, DialogAddButton(d, GetLocalizedString("GAMEOVER_QUIT_MISSION"), GetLocalizedHotkey("GAMEOVER_QUIT_MISSION")));
      TriggerAddAction(t, CustomDefeatQuitBJ);
      if ((GetLocalPlayer() == whichPlayer)) {
        EnableUserControl(true);
        if (bj_isSinglePlayer) {
          PauseGame(true);
        }

        EnableUserUI(false);
      }

      DialogDisplay(whichPlayer, d, true);
      VolumeGroupSetVolumeForPlayerBJ(whichPlayer, SOUND_VOLUMEGROUP_UI, 1.0f);
      StartSoundForPlayerBJ(whichPlayer, bj_defeatDialogSound);
    }

    [NativeLuaMemberAttribute]
    public static void CustomDefeatBJ(player whichPlayer, string message) {
      if (AllowVictoryDefeat(PLAYER_GAME_RESULT_DEFEAT)) {
        RemovePlayer(whichPlayer, PLAYER_GAME_RESULT_DEFEAT);
        if (!bj_isSinglePlayer) {
          DisplayTimedTextFromPlayer(whichPlayer, 0, 0, 60, GetLocalizedString("PLAYER_DEFEATED"));
        }

        if ((GetPlayerController(whichPlayer) == MAP_CONTROL_USER)) {
          CustomDefeatDialogBJ(whichPlayer, message);
        }
      }
    }

    [NativeLuaMemberAttribute]
    public static void SetNextLevelBJ(string nextLevel) {
      if ((nextLevel == string.Empty)) {
        bj_changeLevelMapName = null;
      } else {
        bj_changeLevelMapName = nextLevel;
      }
    }

    [NativeLuaMemberAttribute]
    public static void SetPlayerOnScoreScreenBJ(bool flag, player whichPlayer) {
      SetPlayerOnScoreScreen(whichPlayer, flag);
    }

    [NativeLuaMemberAttribute]
    public static quest CreateQuestBJ(int questType, string title, string description, string iconPath) {
      bool required = (questType == bj_QUESTTYPE_REQ_DISCOVERED) || (questType == bj_QUESTTYPE_REQ_UNDISCOVERED);
      bool discovered = (questType == bj_QUESTTYPE_REQ_DISCOVERED) || (questType == bj_QUESTTYPE_OPT_DISCOVERED);
      bj_lastCreatedQuest = CreateQuest();
      QuestSetTitle(bj_lastCreatedQuest, title);
      QuestSetDescription(bj_lastCreatedQuest, description);
      QuestSetIconPath(bj_lastCreatedQuest, iconPath);
      QuestSetRequired(bj_lastCreatedQuest, required);
      QuestSetDiscovered(bj_lastCreatedQuest, discovered);
      QuestSetCompleted(bj_lastCreatedQuest, false);
      return bj_lastCreatedQuest;
    }

    [NativeLuaMemberAttribute]
    public static void DestroyQuestBJ(quest whichQuest) {
      DestroyQuest(whichQuest);
    }

    [NativeLuaMemberAttribute]
    public static void QuestSetEnabledBJ(bool enabled, quest whichQuest) {
      QuestSetEnabled(whichQuest, enabled);
    }

    [NativeLuaMemberAttribute]
    public static void QuestSetTitleBJ(quest whichQuest, string title) {
      QuestSetTitle(whichQuest, title);
    }

    [NativeLuaMemberAttribute]
    public static void QuestSetDescriptionBJ(quest whichQuest, string description) {
      QuestSetDescription(whichQuest, description);
    }

    [NativeLuaMemberAttribute]
    public static void QuestSetCompletedBJ(quest whichQuest, bool completed) {
      QuestSetCompleted(whichQuest, completed);
    }

    [NativeLuaMemberAttribute]
    public static void QuestSetFailedBJ(quest whichQuest, bool failed) {
      QuestSetFailed(whichQuest, failed);
    }

    [NativeLuaMemberAttribute]
    public static void QuestSetDiscoveredBJ(quest whichQuest, bool discovered) {
      QuestSetDiscovered(whichQuest, discovered);
    }

    [NativeLuaMemberAttribute]
    public static quest GetLastCreatedQuestBJ() {
      return bj_lastCreatedQuest;
    }

    [NativeLuaMemberAttribute]
    public static questitem CreateQuestItemBJ(quest whichQuest, string description) {
      bj_lastCreatedQuestItem = QuestCreateItem(whichQuest);
      QuestItemSetDescription(bj_lastCreatedQuestItem, description);
      QuestItemSetCompleted(bj_lastCreatedQuestItem, false);
      return bj_lastCreatedQuestItem;
    }

    [NativeLuaMemberAttribute]
    public static void QuestItemSetDescriptionBJ(questitem whichQuestItem, string description) {
      QuestItemSetDescription(whichQuestItem, description);
    }

    [NativeLuaMemberAttribute]
    public static void QuestItemSetCompletedBJ(questitem whichQuestItem, bool completed) {
      QuestItemSetCompleted(whichQuestItem, completed);
    }

    [NativeLuaMemberAttribute]
    public static questitem GetLastCreatedQuestItemBJ() {
      return bj_lastCreatedQuestItem;
    }

    [NativeLuaMemberAttribute]
    public static defeatcondition CreateDefeatConditionBJ(string description) {
      bj_lastCreatedDefeatCondition = CreateDefeatCondition();
      DefeatConditionSetDescription(bj_lastCreatedDefeatCondition, description);
      return bj_lastCreatedDefeatCondition;
    }

    [NativeLuaMemberAttribute]
    public static void DestroyDefeatConditionBJ(defeatcondition whichCondition) {
      DestroyDefeatCondition(whichCondition);
    }

    [NativeLuaMemberAttribute]
    public static void DefeatConditionSetDescriptionBJ(defeatcondition whichCondition, string description) {
      DefeatConditionSetDescription(whichCondition, description);
    }

    [NativeLuaMemberAttribute]
    public static defeatcondition GetLastCreatedDefeatConditionBJ() {
      return bj_lastCreatedDefeatCondition;
    }

    [NativeLuaMemberAttribute]
    public static void FlashQuestDialogButtonBJ() {
      FlashQuestDialogButton();
    }

    [NativeLuaMemberAttribute]
    public static void QuestMessageBJ(force f, int messageType, string message) {
      if ((IsPlayerInForce(GetLocalPlayer(), f))) {
        if ((messageType == bj_QUESTMESSAGE_DISCOVERED)) {
          DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_QUEST, " ");
          DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_QUEST, message);
          StartSound(bj_questDiscoveredSound);
          FlashQuestDialogButton();
        } else if ((messageType == bj_QUESTMESSAGE_UPDATED)) {
          DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_QUESTUPDATE, " ");
          DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_QUESTUPDATE, message);
          StartSound(bj_questUpdatedSound);
          FlashQuestDialogButton();
        } else if ((messageType == bj_QUESTMESSAGE_COMPLETED)) {
          DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_QUESTDONE, " ");
          DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_QUESTDONE, message);
          StartSound(bj_questCompletedSound);
          FlashQuestDialogButton();
        } else if ((messageType == bj_QUESTMESSAGE_FAILED)) {
          DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_QUESTFAILED, " ");
          DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_QUESTFAILED, message);
          StartSound(bj_questFailedSound);
          FlashQuestDialogButton();
        } else if ((messageType == bj_QUESTMESSAGE_REQUIREMENT)) {
          DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_QUESTREQUIREMENT, message);
        } else if ((messageType == bj_QUESTMESSAGE_MISSIONFAILED)) {
          DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_MISSIONFAILED, " ");
          DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_MISSIONFAILED, message);
          StartSound(bj_questFailedSound);
        } else if ((messageType == bj_QUESTMESSAGE_HINT)) {
          DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_HINT, " ");
          DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_HINT, message);
          StartSound(bj_questHintSound);
        } else if ((messageType == bj_QUESTMESSAGE_ALWAYSHINT)) {
          DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_ALWAYSHINT, " ");
          DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_ALWAYSHINT, message);
          StartSound(bj_questHintSound);
        } else if ((messageType == bj_QUESTMESSAGE_SECRET)) {
          DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_SECRET, " ");
          DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_SECRET, message);
          StartSound(bj_questSecretSound);
        } else if ((messageType == bj_QUESTMESSAGE_UNITACQUIRED)) {
          DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_UNITACQUIRED, " ");
          DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_UNITACQUIRED, message);
          StartSound(bj_questHintSound);
        } else if ((messageType == bj_QUESTMESSAGE_UNITAVAILABLE)) {
          DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_UNITAVAILABLE, " ");
          DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_UNITAVAILABLE, message);
          StartSound(bj_questHintSound);
        } else if ((messageType == bj_QUESTMESSAGE_ITEMACQUIRED)) {
          DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_ITEMACQUIRED, " ");
          DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_ITEMACQUIRED, message);
          StartSound(bj_questItemAcquiredSound);
        } else if ((messageType == bj_QUESTMESSAGE_WARNING)) {
          DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_WARNING, " ");
          DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_WARNING, message);
          StartSound(bj_questWarningSound);
        } else {
        }
      }
    }

    [NativeLuaMemberAttribute]
    public static timer StartTimerBJ(timer t, bool periodic, float timeout) {
      bj_lastStartedTimer = t;
      TimerStart(t, timeout, periodic, null);
      return bj_lastStartedTimer;
    }

    [NativeLuaMemberAttribute]
    public static timer CreateTimerBJ(bool periodic, float timeout) {
      bj_lastStartedTimer = CreateTimer();
      TimerStart(bj_lastStartedTimer, timeout, periodic, null);
      return bj_lastStartedTimer;
    }

    [NativeLuaMemberAttribute]
    public static void DestroyTimerBJ(timer whichTimer) {
      DestroyTimer(whichTimer);
    }

    [NativeLuaMemberAttribute]
    public static void PauseTimerBJ(bool pause, timer whichTimer) {
      if (pause) {
        PauseTimer(whichTimer);
      } else {
        ResumeTimer(whichTimer);
      }
    }

    [NativeLuaMemberAttribute]
    public static timer GetLastCreatedTimerBJ() {
      return bj_lastStartedTimer;
    }

    [NativeLuaMemberAttribute]
    public static timerdialog CreateTimerDialogBJ(timer t, string title) {
      bj_lastCreatedTimerDialog = CreateTimerDialog(t);
      TimerDialogSetTitle(bj_lastCreatedTimerDialog, title);
      TimerDialogDisplay(bj_lastCreatedTimerDialog, true);
      return bj_lastCreatedTimerDialog;
    }

    [NativeLuaMemberAttribute]
    public static void DestroyTimerDialogBJ(timerdialog td) {
      DestroyTimerDialog(td);
    }

    [NativeLuaMemberAttribute]
    public static void TimerDialogSetTitleBJ(timerdialog td, string title) {
      TimerDialogSetTitle(td, title);
    }

    [NativeLuaMemberAttribute]
    public static void TimerDialogSetTitleColorBJ(timerdialog td, float red, float green, float blue, float transparency) {
      TimerDialogSetTitleColor(td, PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0f - transparency));
    }

    [NativeLuaMemberAttribute]
    public static void TimerDialogSetTimeColorBJ(timerdialog td, float red, float green, float blue, float transparency) {
      TimerDialogSetTimeColor(td, PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0f - transparency));
    }

    [NativeLuaMemberAttribute]
    public static void TimerDialogSetSpeedBJ(timerdialog td, float speedMultFactor) {
      TimerDialogSetSpeed(td, speedMultFactor);
    }

    [NativeLuaMemberAttribute]
    public static void TimerDialogDisplayForPlayerBJ(bool show, timerdialog td, player whichPlayer) {
      if ((GetLocalPlayer() == whichPlayer)) {
        TimerDialogDisplay(td, show);
      }
    }

    [NativeLuaMemberAttribute]
    public static void TimerDialogDisplayBJ(bool show, timerdialog td) {
      TimerDialogDisplay(td, show);
    }

    [NativeLuaMemberAttribute]
    public static timerdialog GetLastCreatedTimerDialogBJ() {
      return bj_lastCreatedTimerDialog;
    }

    [NativeLuaMemberAttribute]
    public static void LeaderboardResizeBJ(leaderboard lb) {
      int size = LeaderboardGetItemCount(lb);
      if ((LeaderboardGetLabelText(lb) == string.Empty)) {
        size = size - 1;
      }

      LeaderboardSetSizeByItemCount(lb, size);
    }

    [NativeLuaMemberAttribute]
    public static void LeaderboardSetPlayerItemValueBJ(player whichPlayer, leaderboard lb, int val) {
      LeaderboardSetItemValue(lb, LeaderboardGetPlayerIndex(lb, whichPlayer), val);
    }

    [NativeLuaMemberAttribute]
    public static void LeaderboardSetPlayerItemLabelBJ(player whichPlayer, leaderboard lb, string val) {
      LeaderboardSetItemLabel(lb, LeaderboardGetPlayerIndex(lb, whichPlayer), val);
    }

    [NativeLuaMemberAttribute]
    public static void LeaderboardSetPlayerItemStyleBJ(player whichPlayer, leaderboard lb, bool showLabel, bool showValue, bool showIcon) {
      LeaderboardSetItemStyle(lb, LeaderboardGetPlayerIndex(lb, whichPlayer), showLabel, showValue, showIcon);
    }

    [NativeLuaMemberAttribute]
    public static void LeaderboardSetPlayerItemLabelColorBJ(player whichPlayer, leaderboard lb, float red, float green, float blue, float transparency) {
      LeaderboardSetItemLabelColor(lb, LeaderboardGetPlayerIndex(lb, whichPlayer), PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0f - transparency));
    }

    [NativeLuaMemberAttribute]
    public static void LeaderboardSetPlayerItemValueColorBJ(player whichPlayer, leaderboard lb, float red, float green, float blue, float transparency) {
      LeaderboardSetItemValueColor(lb, LeaderboardGetPlayerIndex(lb, whichPlayer), PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0f - transparency));
    }

    [NativeLuaMemberAttribute]
    public static void LeaderboardSetLabelColorBJ(leaderboard lb, float red, float green, float blue, float transparency) {
      LeaderboardSetLabelColor(lb, PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0f - transparency));
    }

    [NativeLuaMemberAttribute]
    public static void LeaderboardSetValueColorBJ(leaderboard lb, float red, float green, float blue, float transparency) {
      LeaderboardSetValueColor(lb, PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0f - transparency));
    }

    [NativeLuaMemberAttribute]
    public static void LeaderboardSetLabelBJ(leaderboard lb, string label) {
      LeaderboardSetLabel(lb, label);
      LeaderboardResizeBJ(lb);
    }

    [NativeLuaMemberAttribute]
    public static void LeaderboardSetStyleBJ(leaderboard lb, bool showLabel, bool showNames, bool showValues, bool showIcons) {
      LeaderboardSetStyle(lb, showLabel, showNames, showValues, showIcons);
    }

    [NativeLuaMemberAttribute]
    public static int LeaderboardGetItemCountBJ(leaderboard lb) {
      return LeaderboardGetItemCount(lb);
    }

    [NativeLuaMemberAttribute]
    public static bool LeaderboardHasPlayerItemBJ(leaderboard lb, player whichPlayer) {
      return LeaderboardHasPlayerItem(lb, whichPlayer);
    }

    [NativeLuaMemberAttribute]
    public static void ForceSetLeaderboardBJ(leaderboard lb, force toForce) {
      int index = default;
      player indexPlayer = default;
      index = 0;
      while (true) {
        indexPlayer = Player(index);
        if (IsPlayerInForce(indexPlayer, toForce)) {
          PlayerSetLeaderboard(indexPlayer, lb);
        }

        index = index + 1;
        if (index == bj_MAX_PLAYERS)
          break;
      }
    }

    [NativeLuaMemberAttribute]
    public static leaderboard CreateLeaderboardBJ(force toForce, string label) {
      bj_lastCreatedLeaderboard = CreateLeaderboard();
      LeaderboardSetLabel(bj_lastCreatedLeaderboard, label);
      ForceSetLeaderboardBJ(bj_lastCreatedLeaderboard, toForce);
      LeaderboardDisplay(bj_lastCreatedLeaderboard, true);
      return bj_lastCreatedLeaderboard;
    }

    [NativeLuaMemberAttribute]
    public static void DestroyLeaderboardBJ(leaderboard lb) {
      DestroyLeaderboard(lb);
    }

    [NativeLuaMemberAttribute]
    public static void LeaderboardDisplayBJ(bool show, leaderboard lb) {
      LeaderboardDisplay(lb, show);
    }

    [NativeLuaMemberAttribute]
    public static void LeaderboardAddItemBJ(player whichPlayer, leaderboard lb, string label, int value) {
      if ((LeaderboardHasPlayerItem(lb, whichPlayer))) {
        LeaderboardRemovePlayerItem(lb, whichPlayer);
      }

      LeaderboardAddItem(lb, label, value, whichPlayer);
      LeaderboardResizeBJ(lb);
    }

    [NativeLuaMemberAttribute]
    public static void LeaderboardRemovePlayerItemBJ(player whichPlayer, leaderboard lb) {
      LeaderboardRemovePlayerItem(lb, whichPlayer);
      LeaderboardResizeBJ(lb);
    }

    [NativeLuaMemberAttribute]
    public static void LeaderboardSortItemsBJ(leaderboard lb, int sortType, bool ascending) {
      if ((sortType == bj_SORTTYPE_SORTBYVALUE)) {
        LeaderboardSortItemsByValue(lb, ascending);
      } else if ((sortType == bj_SORTTYPE_SORTBYPLAYER)) {
        LeaderboardSortItemsByPlayer(lb, ascending);
      } else if ((sortType == bj_SORTTYPE_SORTBYLABEL)) {
        LeaderboardSortItemsByLabel(lb, ascending);
      } else {
      }
    }

    [NativeLuaMemberAttribute]
    public static void LeaderboardSortItemsByPlayerBJ(leaderboard lb, bool ascending) {
      LeaderboardSortItemsByPlayer(lb, ascending);
    }

    [NativeLuaMemberAttribute]
    public static void LeaderboardSortItemsByLabelBJ(leaderboard lb, bool ascending) {
      LeaderboardSortItemsByLabel(lb, ascending);
    }

    [NativeLuaMemberAttribute]
    public static int LeaderboardGetPlayerIndexBJ(player whichPlayer, leaderboard lb) {
      return LeaderboardGetPlayerIndex(lb, whichPlayer) + 1;
    }

    [NativeLuaMemberAttribute]
    public static player LeaderboardGetIndexedPlayerBJ(int position, leaderboard lb) {
      int index = default;
      player indexPlayer = default;
      index = 0;
      while (true) {
        indexPlayer = Player(index);
        if ((LeaderboardGetPlayerIndex(lb, indexPlayer) == position - 1)) {
          return indexPlayer;
        }

        index = index + 1;
        if (index == bj_MAX_PLAYERS)
          break;
      }

      return Player(PLAYER_NEUTRAL_PASSIVE);
    }

    [NativeLuaMemberAttribute]
    public static leaderboard PlayerGetLeaderboardBJ(player whichPlayer) {
      return PlayerGetLeaderboard(whichPlayer);
    }

    [NativeLuaMemberAttribute]
    public static leaderboard GetLastCreatedLeaderboard() {
      return bj_lastCreatedLeaderboard;
    }

    [NativeLuaMemberAttribute]
    public static multiboard CreateMultiboardBJ(int cols, int rows, string title) {
      bj_lastCreatedMultiboard = CreateMultiboard();
      MultiboardSetRowCount(bj_lastCreatedMultiboard, rows);
      MultiboardSetColumnCount(bj_lastCreatedMultiboard, cols);
      MultiboardSetTitleText(bj_lastCreatedMultiboard, title);
      MultiboardDisplay(bj_lastCreatedMultiboard, true);
      return bj_lastCreatedMultiboard;
    }

    [NativeLuaMemberAttribute]
    public static void DestroyMultiboardBJ(multiboard mb) {
      DestroyMultiboard(mb);
    }

    [NativeLuaMemberAttribute]
    public static multiboard GetLastCreatedMultiboard() {
      return bj_lastCreatedMultiboard;
    }

    [NativeLuaMemberAttribute]
    public static void MultiboardDisplayBJ(bool show, multiboard mb) {
      MultiboardDisplay(mb, show);
    }

    [NativeLuaMemberAttribute]
    public static void MultiboardMinimizeBJ(bool minimize, multiboard mb) {
      MultiboardMinimize(mb, minimize);
    }

    [NativeLuaMemberAttribute]
    public static void MultiboardSetTitleTextColorBJ(multiboard mb, float red, float green, float blue, float transparency) {
      MultiboardSetTitleTextColor(mb, PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0f - transparency));
    }

    [NativeLuaMemberAttribute]
    public static void MultiboardAllowDisplayBJ(bool flag) {
      MultiboardSuppressDisplay(!flag);
    }

    [NativeLuaMemberAttribute]
    public static void MultiboardSetItemStyleBJ(multiboard mb, int col, int row, bool showValue, bool showIcon) {
      int curRow = 0;
      int curCol = 0;
      int numRows = MultiboardGetRowCount(mb);
      int numCols = MultiboardGetColumnCount(mb);
      multiboarditem mbitem = null;
      while (true) {
        curRow = curRow + 1;
        if (curRow > numRows)
          break;
        if ((row == 0 || row == curRow)) {
          curCol = 0;
          while (true) {
            curCol = curCol + 1;
            if (curCol > numCols)
              break;
            if ((col == 0 || col == curCol)) {
              mbitem = MultiboardGetItem(mb, curRow - 1, curCol - 1);
              MultiboardSetItemStyle(mbitem, showValue, showIcon);
              MultiboardReleaseItem(mbitem);
            }
          }
        }
      }
    }

    [NativeLuaMemberAttribute]
    public static void MultiboardSetItemValueBJ(multiboard mb, int col, int row, string val) {
      int curRow = 0;
      int curCol = 0;
      int numRows = MultiboardGetRowCount(mb);
      int numCols = MultiboardGetColumnCount(mb);
      multiboarditem mbitem = null;
      while (true) {
        curRow = curRow + 1;
        if (curRow > numRows)
          break;
        if ((row == 0 || row == curRow)) {
          curCol = 0;
          while (true) {
            curCol = curCol + 1;
            if (curCol > numCols)
              break;
            if ((col == 0 || col == curCol)) {
              mbitem = MultiboardGetItem(mb, curRow - 1, curCol - 1);
              MultiboardSetItemValue(mbitem, val);
              MultiboardReleaseItem(mbitem);
            }
          }
        }
      }
    }

    [NativeLuaMemberAttribute]
    public static void MultiboardSetItemColorBJ(multiboard mb, int col, int row, float red, float green, float blue, float transparency) {
      int curRow = 0;
      int curCol = 0;
      int numRows = MultiboardGetRowCount(mb);
      int numCols = MultiboardGetColumnCount(mb);
      multiboarditem mbitem = null;
      while (true) {
        curRow = curRow + 1;
        if (curRow > numRows)
          break;
        if ((row == 0 || row == curRow)) {
          curCol = 0;
          while (true) {
            curCol = curCol + 1;
            if (curCol > numCols)
              break;
            if ((col == 0 || col == curCol)) {
              mbitem = MultiboardGetItem(mb, curRow - 1, curCol - 1);
              MultiboardSetItemValueColor(mbitem, PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0f - transparency));
              MultiboardReleaseItem(mbitem);
            }
          }
        }
      }
    }

    [NativeLuaMemberAttribute]
    public static void MultiboardSetItemWidthBJ(multiboard mb, int col, int row, float width) {
      int curRow = 0;
      int curCol = 0;
      int numRows = MultiboardGetRowCount(mb);
      int numCols = MultiboardGetColumnCount(mb);
      multiboarditem mbitem = null;
      while (true) {
        curRow = curRow + 1;
        if (curRow > numRows)
          break;
        if ((row == 0 || row == curRow)) {
          curCol = 0;
          while (true) {
            curCol = curCol + 1;
            if (curCol > numCols)
              break;
            if ((col == 0 || col == curCol)) {
              mbitem = MultiboardGetItem(mb, curRow - 1, curCol - 1);
              MultiboardSetItemWidth(mbitem, width / 100.0f);
              MultiboardReleaseItem(mbitem);
            }
          }
        }
      }
    }

    [NativeLuaMemberAttribute]
    public static void MultiboardSetItemIconBJ(multiboard mb, int col, int row, string iconFileName) {
      int curRow = 0;
      int curCol = 0;
      int numRows = MultiboardGetRowCount(mb);
      int numCols = MultiboardGetColumnCount(mb);
      multiboarditem mbitem = null;
      while (true) {
        curRow = curRow + 1;
        if (curRow > numRows)
          break;
        if ((row == 0 || row == curRow)) {
          curCol = 0;
          while (true) {
            curCol = curCol + 1;
            if (curCol > numCols)
              break;
            if ((col == 0 || col == curCol)) {
              mbitem = MultiboardGetItem(mb, curRow - 1, curCol - 1);
              MultiboardSetItemIcon(mbitem, iconFileName);
              MultiboardReleaseItem(mbitem);
            }
          }
        }
      }
    }

    [NativeLuaMemberAttribute]
    public static float TextTagSize2Height(float size) {
      return size * 0.023f / 10;
    }

    [NativeLuaMemberAttribute]
    public static float TextTagSpeed2Velocity(float speed) {
      return speed * 0.071f / 128;
    }

    [NativeLuaMemberAttribute]
    public static void SetTextTagColorBJ(texttag tt, float red, float green, float blue, float transparency) {
      SetTextTagColor(tt, PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0f - transparency));
    }

    [NativeLuaMemberAttribute]
    public static void SetTextTagVelocityBJ(texttag tt, float speed, float angle) {
      float vel = TextTagSpeed2Velocity(speed);
      float xvel = vel * Cos(angle * bj_DEGTORAD);
      float yvel = vel * Sin(angle * bj_DEGTORAD);
      SetTextTagVelocity(tt, xvel, yvel);
    }

    [NativeLuaMemberAttribute]
    public static void SetTextTagTextBJ(texttag tt, string s, float size) {
      float textHeight = TextTagSize2Height(size);
      SetTextTagText(tt, s, textHeight);
    }

    [NativeLuaMemberAttribute]
    public static void SetTextTagPosBJ(texttag tt, location loc, float zOffset) {
      SetTextTagPos(tt, GetLocationX(loc), GetLocationY(loc), zOffset);
    }

    [NativeLuaMemberAttribute]
    public static void SetTextTagPosUnitBJ(texttag tt, unit whichUnit, float zOffset) {
      SetTextTagPosUnit(tt, whichUnit, zOffset);
    }

    [NativeLuaMemberAttribute]
    public static void SetTextTagSuspendedBJ(texttag tt, bool flag) {
      SetTextTagSuspended(tt, flag);
    }

    [NativeLuaMemberAttribute]
    public static void SetTextTagPermanentBJ(texttag tt, bool flag) {
      SetTextTagPermanent(tt, flag);
    }

    [NativeLuaMemberAttribute]
    public static void SetTextTagAgeBJ(texttag tt, float age) {
      SetTextTagAge(tt, age);
    }

    [NativeLuaMemberAttribute]
    public static void SetTextTagLifespanBJ(texttag tt, float lifespan) {
      SetTextTagLifespan(tt, lifespan);
    }

    [NativeLuaMemberAttribute]
    public static void SetTextTagFadepointBJ(texttag tt, float fadepoint) {
      SetTextTagFadepoint(tt, fadepoint);
    }

    [NativeLuaMemberAttribute]
    public static texttag CreateTextTagLocBJ(string s, location loc, float zOffset, float size, float red, float green, float blue, float transparency) {
      bj_lastCreatedTextTag = CreateTextTag();
      SetTextTagTextBJ(bj_lastCreatedTextTag, s, size);
      SetTextTagPosBJ(bj_lastCreatedTextTag, loc, zOffset);
      SetTextTagColorBJ(bj_lastCreatedTextTag, red, green, blue, transparency);
      return bj_lastCreatedTextTag;
    }

    [NativeLuaMemberAttribute]
    public static texttag CreateTextTagUnitBJ(string s, unit whichUnit, float zOffset, float size, float red, float green, float blue, float transparency) {
      bj_lastCreatedTextTag = CreateTextTag();
      SetTextTagTextBJ(bj_lastCreatedTextTag, s, size);
      SetTextTagPosUnitBJ(bj_lastCreatedTextTag, whichUnit, zOffset);
      SetTextTagColorBJ(bj_lastCreatedTextTag, red, green, blue, transparency);
      return bj_lastCreatedTextTag;
    }

    [NativeLuaMemberAttribute]
    public static void DestroyTextTagBJ(texttag tt) {
      DestroyTextTag(tt);
    }

    [NativeLuaMemberAttribute]
    public static void ShowTextTagForceBJ(bool show, texttag tt, force whichForce) {
      if ((IsPlayerInForce(GetLocalPlayer(), whichForce))) {
        SetTextTagVisibility(tt, show);
      }
    }

    [NativeLuaMemberAttribute]
    public static texttag GetLastCreatedTextTag() {
      return bj_lastCreatedTextTag;
    }

    [NativeLuaMemberAttribute]
    public static void PauseGameOn() {
      PauseGame(true);
    }

    [NativeLuaMemberAttribute]
    public static void PauseGameOff() {
      PauseGame(false);
    }

    [NativeLuaMemberAttribute]
    public static void SetUserControlForceOn(force whichForce) {
      if ((IsPlayerInForce(GetLocalPlayer(), whichForce))) {
        EnableUserControl(true);
      }
    }

    [NativeLuaMemberAttribute]
    public static void SetUserControlForceOff(force whichForce) {
      if ((IsPlayerInForce(GetLocalPlayer(), whichForce))) {
        EnableUserControl(false);
      }
    }

    [NativeLuaMemberAttribute]
    public static void ShowInterfaceForceOn(force whichForce, float fadeDuration) {
      if ((IsPlayerInForce(GetLocalPlayer(), whichForce))) {
        ShowInterface(true, fadeDuration);
      }
    }

    [NativeLuaMemberAttribute]
    public static void ShowInterfaceForceOff(force whichForce, float fadeDuration) {
      if ((IsPlayerInForce(GetLocalPlayer(), whichForce))) {
        ShowInterface(false, fadeDuration);
      }
    }

    [NativeLuaMemberAttribute]
    public static void PingMinimapForForce(force whichForce, float x, float y, float duration) {
      if ((IsPlayerInForce(GetLocalPlayer(), whichForce))) {
        PingMinimap(x, y, duration);
      }
    }

    [NativeLuaMemberAttribute]
    public static void PingMinimapLocForForce(force whichForce, location loc, float duration) {
      PingMinimapForForce(whichForce, GetLocationX(loc), GetLocationY(loc), duration);
    }

    [NativeLuaMemberAttribute]
    public static void PingMinimapForPlayer(player whichPlayer, float x, float y, float duration) {
      if ((GetLocalPlayer() == whichPlayer)) {
        PingMinimap(x, y, duration);
      }
    }

    [NativeLuaMemberAttribute]
    public static void PingMinimapLocForPlayer(player whichPlayer, location loc, float duration) {
      PingMinimapForPlayer(whichPlayer, GetLocationX(loc), GetLocationY(loc), duration);
    }

    [NativeLuaMemberAttribute]
    public static void PingMinimapForForceEx(force whichForce, float x, float y, float duration, int style, float red, float green, float blue) {
      int red255 = PercentTo255(red);
      int green255 = PercentTo255(green);
      int blue255 = PercentTo255(blue);
      if ((IsPlayerInForce(GetLocalPlayer(), whichForce))) {
        if ((red255 == 255) && (green255 == 0) && (blue255 == 0)) {
          red255 = 254;
        }

        if ((style == bj_MINIMAPPINGSTYLE_SIMPLE)) {
          PingMinimapEx(x, y, duration, red255, green255, blue255, false);
        } else if ((style == bj_MINIMAPPINGSTYLE_FLASHY)) {
          PingMinimapEx(x, y, duration, red255, green255, blue255, true);
        } else if ((style == bj_MINIMAPPINGSTYLE_ATTACK)) {
          PingMinimapEx(x, y, duration, 255, 0, 0, false);
        } else {
        }
      }
    }

    [NativeLuaMemberAttribute]
    public static void PingMinimapLocForForceEx(force whichForce, location loc, float duration, int style, float red, float green, float blue) {
      PingMinimapForForceEx(whichForce, GetLocationX(loc), GetLocationY(loc), duration, style, red, green, blue);
    }

    [NativeLuaMemberAttribute]
    public static void EnableWorldFogBoundaryBJ(bool enable, force f) {
      if ((IsPlayerInForce(GetLocalPlayer(), f))) {
        EnableWorldFogBoundary(enable);
      }
    }

    [NativeLuaMemberAttribute]
    public static void EnableOcclusionBJ(bool enable, force f) {
      if ((IsPlayerInForce(GetLocalPlayer(), f))) {
        EnableOcclusion(enable);
      }
    }

    [NativeLuaMemberAttribute]
    public static void CancelCineSceneBJ() {
      StopSoundBJ(bj_cineSceneLastSound, true);
      EndCinematicScene();
    }

    [NativeLuaMemberAttribute]
    public static void TryInitCinematicBehaviorBJ() {
      int index = default;
      if ((bj_cineSceneBeingSkipped == null)) {
        bj_cineSceneBeingSkipped = CreateTrigger();
        index = 0;
        while (true) {
          TriggerRegisterPlayerEvent(bj_cineSceneBeingSkipped, Player(index), EVENT_PLAYER_END_CINEMATIC);
          index = index + 1;
          if (index == bj_MAX_PLAYERS)
            break;
        }

        TriggerAddAction(bj_cineSceneBeingSkipped, CancelCineSceneBJ);
      }
    }

    [NativeLuaMemberAttribute]
    public static void SetCinematicSceneBJ(sound soundHandle, int portraitUnitId, playercolor color, string speakerTitle, string text, float sceneDuration, float voiceoverDuration) {
      bj_cineSceneLastSound = soundHandle;
      PlaySoundBJ(soundHandle);
      SetCinematicScene(portraitUnitId, color, speakerTitle, text, sceneDuration, voiceoverDuration);
    }

    [NativeLuaMemberAttribute]
    public static float GetTransmissionDuration(sound soundHandle, int timeType, float timeVal) {
      float duration = default;
      if ((timeType == bj_TIMETYPE_ADD)) {
        duration = GetSoundDurationBJ(soundHandle) + timeVal;
      } else if ((timeType == bj_TIMETYPE_SET)) {
        duration = timeVal;
      } else if ((timeType == bj_TIMETYPE_SUB)) {
        duration = GetSoundDurationBJ(soundHandle) - timeVal;
      } else {
        duration = GetSoundDurationBJ(soundHandle);
      }

      if ((duration < 0)) {
        duration = 0;
      }

      return duration;
    }

    [NativeLuaMemberAttribute]
    public static void WaitTransmissionDuration(sound soundHandle, int timeType, float timeVal) {
      if ((timeType == bj_TIMETYPE_SET)) {
        TriggerSleepAction(timeVal);
      } else if ((soundHandle == null)) {
        TriggerSleepAction(bj_NOTHING_SOUND_DURATION);
      } else if ((timeType == bj_TIMETYPE_SUB)) {
        WaitForSoundBJ(soundHandle, timeVal);
      } else if ((timeType == bj_TIMETYPE_ADD)) {
        WaitForSoundBJ(soundHandle, 0);
        TriggerSleepAction(timeVal);
      } else {
      }
    }

    [NativeLuaMemberAttribute]
    public static void DoTransmissionBasicsXYBJ(int unitId, playercolor color, float x, float y, sound soundHandle, string unitName, string message, float duration) {
      SetCinematicSceneBJ(soundHandle, unitId, color, unitName, message, duration + bj_TRANSMISSION_PORT_HANGTIME, duration);
      if ((unitId != 0)) {
        PingMinimap(x, y, bj_TRANSMISSION_PING_TIME);
      }
    }

    [NativeLuaMemberAttribute]
    public static void TransmissionFromUnitWithNameBJ(force toForce, unit whichUnit, string unitName, sound soundHandle, string message, int timeType, float timeVal, bool wait) {
      TryInitCinematicBehaviorBJ();
      timeVal = RMaxBJ(timeVal, 0);
      bj_lastTransmissionDuration = GetTransmissionDuration(soundHandle, timeType, timeVal);
      bj_lastPlayedSound = soundHandle;
      if ((IsPlayerInForce(GetLocalPlayer(), toForce))) {
        if ((whichUnit == null)) {
          DoTransmissionBasicsXYBJ(0, PLAYER_COLOR_RED, 0, 0, soundHandle, unitName, message, bj_lastTransmissionDuration);
        } else {
          DoTransmissionBasicsXYBJ(GetUnitTypeId(whichUnit), GetPlayerColor(GetOwningPlayer(whichUnit)), GetUnitX(whichUnit), GetUnitY(whichUnit), soundHandle, unitName, message, bj_lastTransmissionDuration);
          if ((!IsUnitHidden(whichUnit))) {
            UnitAddIndicator(whichUnit, bj_TRANSMISSION_IND_RED, bj_TRANSMISSION_IND_BLUE, bj_TRANSMISSION_IND_GREEN, bj_TRANSMISSION_IND_ALPHA);
          }
        }
      }

      if (wait && (bj_lastTransmissionDuration > 0)) {
        WaitTransmissionDuration(soundHandle, timeType, timeVal);
      }
    }

    [NativeLuaMemberAttribute]
    public static void TransmissionFromUnitTypeWithNameBJ(force toForce, player fromPlayer, int unitId, string unitName, location loc, sound soundHandle, string message, int timeType, float timeVal, bool wait) {
      TryInitCinematicBehaviorBJ();
      timeVal = RMaxBJ(timeVal, 0);
      bj_lastTransmissionDuration = GetTransmissionDuration(soundHandle, timeType, timeVal);
      bj_lastPlayedSound = soundHandle;
      if ((IsPlayerInForce(GetLocalPlayer(), toForce))) {
        DoTransmissionBasicsXYBJ(unitId, GetPlayerColor(fromPlayer), GetLocationX(loc), GetLocationY(loc), soundHandle, unitName, message, bj_lastTransmissionDuration);
      }

      if (wait && (bj_lastTransmissionDuration > 0)) {
        WaitTransmissionDuration(soundHandle, timeType, timeVal);
      }
    }

    [NativeLuaMemberAttribute]
    public static float GetLastTransmissionDurationBJ() {
      return bj_lastTransmissionDuration;
    }

    [NativeLuaMemberAttribute]
    public static void ForceCinematicSubtitlesBJ(bool flag) {
      ForceCinematicSubtitles(flag);
    }

    [NativeLuaMemberAttribute]
    public static void CinematicModeExBJ(bool cineMode, force forForce, float interfaceFadeTime) {
      if ((!bj_gameStarted)) {
        interfaceFadeTime = 0;
      }

      if ((cineMode)) {
        if ((!bj_cineModeAlreadyIn)) {
          bj_cineModeAlreadyIn = true;
          bj_cineModePriorSpeed = GetGameSpeed();
          bj_cineModePriorFogSetting = IsFogEnabled();
          bj_cineModePriorMaskSetting = IsFogMaskEnabled();
          bj_cineModePriorDawnDusk = IsDawnDuskEnabled();
          bj_cineModeSavedSeed = GetRandomInt(0, 1000000);
        }

        if ((IsPlayerInForce(GetLocalPlayer(), forForce))) {
          ClearTextMessages();
          ShowInterface(false, interfaceFadeTime);
          EnableUserControl(false);
          EnableOcclusion(false);
          SetCineModeVolumeGroupsBJ();
        }

        SetGameSpeed(bj_CINEMODE_GAMESPEED);
        SetMapFlag(MAP_LOCK_SPEED, true);
        FogMaskEnable(false);
        FogEnable(false);
        EnableWorldFogBoundary(false);
        EnableDawnDusk(false);
        SetRandomSeed(0);
      } else {
        bj_cineModeAlreadyIn = false;
        if ((IsPlayerInForce(GetLocalPlayer(), forForce))) {
          ShowInterface(true, interfaceFadeTime);
          EnableUserControl(true);
          EnableOcclusion(true);
          VolumeGroupReset();
          EndThematicMusic();
          CameraResetSmoothingFactorBJ();
        }

        SetMapFlag(MAP_LOCK_SPEED, false);
        SetGameSpeed(bj_cineModePriorSpeed);
        FogMaskEnable(bj_cineModePriorMaskSetting);
        FogEnable(bj_cineModePriorFogSetting);
        EnableWorldFogBoundary(true);
        EnableDawnDusk(bj_cineModePriorDawnDusk);
        SetRandomSeed(bj_cineModeSavedSeed);
      }
    }

    [NativeLuaMemberAttribute]
    public static void CinematicModeBJ(bool cineMode, force forForce) {
      CinematicModeExBJ(cineMode, forForce, bj_CINEMODE_INTERFACEFADE);
    }

    [NativeLuaMemberAttribute]
    public static void DisplayCineFilterBJ(bool flag) {
      DisplayCineFilter(flag);
    }

    [NativeLuaMemberAttribute]
    public static void CinematicFadeCommonBJ(float red, float green, float blue, float duration, string tex, float startTrans, float endTrans) {
      if ((duration == 0)) {
        startTrans = endTrans;
      }

      EnableUserUI(false);
      SetCineFilterTexture(tex);
      SetCineFilterBlendMode(BLEND_MODE_BLEND);
      SetCineFilterTexMapFlags(TEXMAP_FLAG_NONE);
      SetCineFilterStartUV(0, 0, 1, 1);
      SetCineFilterEndUV(0, 0, 1, 1);
      SetCineFilterStartColor(PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100 - startTrans));
      SetCineFilterEndColor(PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100 - endTrans));
      SetCineFilterDuration(duration);
      DisplayCineFilter(true);
    }

    [NativeLuaMemberAttribute]
    public static void FinishCinematicFadeBJ() {
      DestroyTimer(bj_cineFadeFinishTimer);
      bj_cineFadeFinishTimer = null;
      DisplayCineFilter(false);
      EnableUserUI(true);
    }

    [NativeLuaMemberAttribute]
    public static void FinishCinematicFadeAfterBJ(float duration) {
      bj_cineFadeFinishTimer = CreateTimer();
      TimerStart(bj_cineFadeFinishTimer, duration, false, FinishCinematicFadeBJ);
    }

    [NativeLuaMemberAttribute]
    public static void ContinueCinematicFadeBJ() {
      DestroyTimer(bj_cineFadeContinueTimer);
      bj_cineFadeContinueTimer = null;
      CinematicFadeCommonBJ(bj_cineFadeContinueRed, bj_cineFadeContinueGreen, bj_cineFadeContinueBlue, bj_cineFadeContinueDuration, bj_cineFadeContinueTex, bj_cineFadeContinueTrans, 100);
    }

    [NativeLuaMemberAttribute]
    public static void ContinueCinematicFadeAfterBJ(float duration, float red, float green, float blue, float trans, string tex) {
      bj_cineFadeContinueRed = red;
      bj_cineFadeContinueGreen = green;
      bj_cineFadeContinueBlue = blue;
      bj_cineFadeContinueTrans = trans;
      bj_cineFadeContinueDuration = duration;
      bj_cineFadeContinueTex = tex;
      bj_cineFadeContinueTimer = CreateTimer();
      TimerStart(bj_cineFadeContinueTimer, duration, false, ContinueCinematicFadeBJ);
    }

    [NativeLuaMemberAttribute]
    public static void AbortCinematicFadeBJ() {
      if ((bj_cineFadeContinueTimer != null)) {
        DestroyTimer(bj_cineFadeContinueTimer);
      }

      if ((bj_cineFadeFinishTimer != null)) {
        DestroyTimer(bj_cineFadeFinishTimer);
      }
    }

    [NativeLuaMemberAttribute]
    public static void CinematicFadeBJ(int fadetype, float duration, string tex, float red, float green, float blue, float trans) {
      if ((fadetype == bj_CINEFADETYPE_FADEOUT)) {
        AbortCinematicFadeBJ();
        CinematicFadeCommonBJ(red, green, blue, duration, tex, 100, trans);
      } else if ((fadetype == bj_CINEFADETYPE_FADEIN)) {
        AbortCinematicFadeBJ();
        CinematicFadeCommonBJ(red, green, blue, duration, tex, trans, 100);
        FinishCinematicFadeAfterBJ(duration);
      } else if ((fadetype == bj_CINEFADETYPE_FADEOUTIN)) {
        if ((duration > 0)) {
          AbortCinematicFadeBJ();
          CinematicFadeCommonBJ(red, green, blue, duration * 0.5f, tex, 100, trans);
          ContinueCinematicFadeAfterBJ(duration * 0.5f, red, green, blue, trans, tex);
          FinishCinematicFadeAfterBJ(duration);
        }
      } else {
      }
    }

    [NativeLuaMemberAttribute]
    public static void CinematicFilterGenericBJ(float duration, blendmode bmode, string tex, float red0, float green0, float blue0, float trans0, float red1, float green1, float blue1, float trans1) {
      AbortCinematicFadeBJ();
      SetCineFilterTexture(tex);
      SetCineFilterBlendMode(bmode);
      SetCineFilterTexMapFlags(TEXMAP_FLAG_NONE);
      SetCineFilterStartUV(0, 0, 1, 1);
      SetCineFilterEndUV(0, 0, 1, 1);
      SetCineFilterStartColor(PercentTo255(red0), PercentTo255(green0), PercentTo255(blue0), PercentTo255(100 - trans0));
      SetCineFilterEndColor(PercentTo255(red1), PercentTo255(green1), PercentTo255(blue1), PercentTo255(100 - trans1));
      SetCineFilterDuration(duration);
      DisplayCineFilter(true);
    }

    [NativeLuaMemberAttribute]
    public static void RescueUnitBJ(unit whichUnit, player rescuer, bool changeColor) {
      if (IsUnitDeadBJ(whichUnit) || (GetOwningPlayer(whichUnit) == rescuer)) {
        return;
      }

      StartSound(bj_rescueSound);
      SetUnitOwner(whichUnit, rescuer, changeColor);
      UnitAddIndicator(whichUnit, 0, 255, 0, 255);
      PingMinimapForPlayer(rescuer, GetUnitX(whichUnit), GetUnitY(whichUnit), bj_RESCUE_PING_TIME);
    }

    [NativeLuaMemberAttribute]
    public static void TriggerActionUnitRescuedBJ() {
      unit theUnit = GetTriggerUnit();
      if (IsUnitType(theUnit, UNIT_TYPE_STRUCTURE)) {
        RescueUnitBJ(theUnit, GetOwningPlayer(GetRescuer()), bj_rescueChangeColorBldg);
      } else {
        RescueUnitBJ(theUnit, GetOwningPlayer(GetRescuer()), bj_rescueChangeColorUnit);
      }
    }

    [NativeLuaMemberAttribute]
    public static void TryInitRescuableTriggersBJ() {
      int index = default;
      if ((bj_rescueUnitBehavior == null)) {
        bj_rescueUnitBehavior = CreateTrigger();
        index = 0;
        while (true) {
          TriggerRegisterPlayerUnitEvent(bj_rescueUnitBehavior, Player(index), EVENT_PLAYER_UNIT_RESCUED, null);
          index = index + 1;
          if (index == bj_MAX_PLAYER_SLOTS)
            break;
        }

        TriggerAddAction(bj_rescueUnitBehavior, TriggerActionUnitRescuedBJ);
      }
    }

    [NativeLuaMemberAttribute]
    public static void SetRescueUnitColorChangeBJ(bool changeColor) {
      bj_rescueChangeColorUnit = changeColor;
    }

    [NativeLuaMemberAttribute]
    public static void SetRescueBuildingColorChangeBJ(bool changeColor) {
      bj_rescueChangeColorBldg = changeColor;
    }

    [NativeLuaMemberAttribute]
    public static void MakeUnitRescuableToForceBJEnum() {
      TryInitRescuableTriggersBJ();
      SetUnitRescuable(bj_makeUnitRescuableUnit, GetEnumPlayer(), bj_makeUnitRescuableFlag);
    }

    [NativeLuaMemberAttribute]
    public static void MakeUnitRescuableToForceBJ(unit whichUnit, bool isRescuable, force whichForce) {
      bj_makeUnitRescuableUnit = whichUnit;
      bj_makeUnitRescuableFlag = isRescuable;
      ForForce(whichForce, MakeUnitRescuableToForceBJEnum);
    }

    [NativeLuaMemberAttribute]
    public static void InitRescuableBehaviorBJ() {
      int index = default;
      index = 0;
      while (true) {
        if ((GetPlayerController(Player(index)) == MAP_CONTROL_RESCUABLE)) {
          TryInitRescuableTriggersBJ();
          return;
        }

        index = index + 1;
        if (index == bj_MAX_PLAYERS)
          break;
      }
    }

    [NativeLuaMemberAttribute]
    public static void SetPlayerTechResearchedSwap(int techid, int levels, player whichPlayer) {
      SetPlayerTechResearched(whichPlayer, techid, levels);
    }

    [NativeLuaMemberAttribute]
    public static void SetPlayerTechMaxAllowedSwap(int techid, int maximum, player whichPlayer) {
      SetPlayerTechMaxAllowed(whichPlayer, techid, maximum);
    }

    [NativeLuaMemberAttribute]
    public static void SetPlayerMaxHeroesAllowed(int maximum, player whichPlayer) {
      SetPlayerTechMaxAllowed(whichPlayer, 1212502607, maximum);
    }

    [NativeLuaMemberAttribute]
    public static int GetPlayerTechCountSimple(int techid, player whichPlayer) {
      return GetPlayerTechCount(whichPlayer, techid, true);
    }

    [NativeLuaMemberAttribute]
    public static int GetPlayerTechMaxAllowedSwap(int techid, player whichPlayer) {
      return GetPlayerTechMaxAllowed(whichPlayer, techid);
    }

    [NativeLuaMemberAttribute]
    public static void SetPlayerAbilityAvailableBJ(bool avail, int abilid, player whichPlayer) {
      SetPlayerAbilityAvailable(whichPlayer, abilid, avail);
    }

    [NativeLuaMemberAttribute]
    public static void SetCampaignMenuRaceBJ(int campaignNumber) {
      if ((campaignNumber == bj_CAMPAIGN_INDEX_T)) {
        SetCampaignMenuRace(RACE_OTHER);
      } else if ((campaignNumber == bj_CAMPAIGN_INDEX_H)) {
        SetCampaignMenuRace(RACE_HUMAN);
      } else if ((campaignNumber == bj_CAMPAIGN_INDEX_U)) {
        SetCampaignMenuRace(RACE_UNDEAD);
      } else if ((campaignNumber == bj_CAMPAIGN_INDEX_O)) {
        SetCampaignMenuRace(RACE_ORC);
      } else if ((campaignNumber == bj_CAMPAIGN_INDEX_N)) {
        SetCampaignMenuRace(RACE_NIGHTELF);
      } else if ((campaignNumber == bj_CAMPAIGN_INDEX_XN)) {
        SetCampaignMenuRaceEx(bj_CAMPAIGN_OFFSET_XN);
      } else if ((campaignNumber == bj_CAMPAIGN_INDEX_XH)) {
        SetCampaignMenuRaceEx(bj_CAMPAIGN_OFFSET_XH);
      } else if ((campaignNumber == bj_CAMPAIGN_INDEX_XU)) {
        SetCampaignMenuRaceEx(bj_CAMPAIGN_OFFSET_XU);
      } else if ((campaignNumber == bj_CAMPAIGN_INDEX_XO)) {
        SetCampaignMenuRaceEx(bj_CAMPAIGN_OFFSET_XO);
      } else {
      }
    }

    [NativeLuaMemberAttribute]
    public static void SetMissionAvailableBJ(bool available, int missionIndex) {
      int campaignNumber = missionIndex / 1000;
      int missionNumber = missionIndex - campaignNumber * 1000;
      SetMissionAvailable(campaignNumber, missionNumber, available);
    }

    [NativeLuaMemberAttribute]
    public static void SetCampaignAvailableBJ(bool available, int campaignNumber) {
      int campaignOffset = default;
      if ((campaignNumber == bj_CAMPAIGN_INDEX_H)) {
        SetTutorialCleared(true);
      }

      if ((campaignNumber == bj_CAMPAIGN_INDEX_XN)) {
        campaignOffset = bj_CAMPAIGN_OFFSET_XN;
      } else if ((campaignNumber == bj_CAMPAIGN_INDEX_XH)) {
        campaignOffset = bj_CAMPAIGN_OFFSET_XH;
      } else if ((campaignNumber == bj_CAMPAIGN_INDEX_XU)) {
        campaignOffset = bj_CAMPAIGN_OFFSET_XU;
      } else if ((campaignNumber == bj_CAMPAIGN_INDEX_XO)) {
        campaignOffset = bj_CAMPAIGN_OFFSET_XO;
      } else {
        campaignOffset = campaignNumber;
      }

      SetCampaignAvailable(campaignOffset, available);
      SetCampaignMenuRaceBJ(campaignNumber);
      ForceCampaignSelectScreen();
    }

    [NativeLuaMemberAttribute]
    public static void SetCinematicAvailableBJ(bool available, int cinematicIndex) {
      if ((cinematicIndex == bj_CINEMATICINDEX_TOP)) {
        SetOpCinematicAvailable(bj_CAMPAIGN_INDEX_T, available);
        PlayCinematic("TutorialOp");
      } else if ((cinematicIndex == bj_CINEMATICINDEX_HOP)) {
        SetOpCinematicAvailable(bj_CAMPAIGN_INDEX_H, available);
        PlayCinematic("HumanOp");
      } else if ((cinematicIndex == bj_CINEMATICINDEX_HED)) {
        SetEdCinematicAvailable(bj_CAMPAIGN_INDEX_H, available);
        PlayCinematic("HumanEd");
      } else if ((cinematicIndex == bj_CINEMATICINDEX_OOP)) {
        SetOpCinematicAvailable(bj_CAMPAIGN_INDEX_O, available);
        PlayCinematic("OrcOp");
      } else if ((cinematicIndex == bj_CINEMATICINDEX_OED)) {
        SetEdCinematicAvailable(bj_CAMPAIGN_INDEX_O, available);
        PlayCinematic("OrcEd");
      } else if ((cinematicIndex == bj_CINEMATICINDEX_UOP)) {
        SetEdCinematicAvailable(bj_CAMPAIGN_INDEX_U, available);
        PlayCinematic("UndeadOp");
      } else if ((cinematicIndex == bj_CINEMATICINDEX_UED)) {
        SetEdCinematicAvailable(bj_CAMPAIGN_INDEX_U, available);
        PlayCinematic("UndeadEd");
      } else if ((cinematicIndex == bj_CINEMATICINDEX_NOP)) {
        SetEdCinematicAvailable(bj_CAMPAIGN_INDEX_N, available);
        PlayCinematic("NightElfOp");
      } else if ((cinematicIndex == bj_CINEMATICINDEX_NED)) {
        SetEdCinematicAvailable(bj_CAMPAIGN_INDEX_N, available);
        PlayCinematic("NightElfEd");
      } else if ((cinematicIndex == bj_CINEMATICINDEX_XOP)) {
        SetOpCinematicAvailable(bj_CAMPAIGN_OFFSET_XN, available);
        PlayCinematic("IntroX");
      } else if ((cinematicIndex == bj_CINEMATICINDEX_XED)) {
        SetEdCinematicAvailable(bj_CAMPAIGN_OFFSET_XU, available);
        PlayCinematic("OutroX");
      } else {
      }
    }

    [NativeLuaMemberAttribute]
    public static gamecache InitGameCacheBJ(string campaignFile) {
      bj_lastCreatedGameCache = InitGameCache(campaignFile);
      return bj_lastCreatedGameCache;
    }

    [NativeLuaMemberAttribute]
    public static bool SaveGameCacheBJ(gamecache cache) {
      return SaveGameCache(cache);
    }

    [NativeLuaMemberAttribute]
    public static gamecache GetLastCreatedGameCacheBJ() {
      return bj_lastCreatedGameCache;
    }

    [NativeLuaMemberAttribute]
    public static hashtable InitHashtableBJ() {
      bj_lastCreatedHashtable = InitHashtable();
      return bj_lastCreatedHashtable;
    }

    [NativeLuaMemberAttribute]
    public static hashtable GetLastCreatedHashtableBJ() {
      return bj_lastCreatedHashtable;
    }

    [NativeLuaMemberAttribute]
    public static void StoreRealBJ(float value, string key, string missionKey, gamecache cache) {
      StoreReal(cache, missionKey, key, value);
    }

    [NativeLuaMemberAttribute]
    public static void StoreIntegerBJ(int value, string key, string missionKey, gamecache cache) {
      StoreInteger(cache, missionKey, key, value);
    }

    [NativeLuaMemberAttribute]
    public static void StoreBooleanBJ(bool value, string key, string missionKey, gamecache cache) {
      StoreBoolean(cache, missionKey, key, value);
    }

    [NativeLuaMemberAttribute]
    public static bool StoreStringBJ(string value, string key, string missionKey, gamecache cache) {
      return StoreString(cache, missionKey, key, value);
    }

    [NativeLuaMemberAttribute]
    public static bool StoreUnitBJ(unit whichUnit, string key, string missionKey, gamecache cache) {
      return StoreUnit(cache, missionKey, key, whichUnit);
    }

    [NativeLuaMemberAttribute]
    public static void SaveRealBJ(float value, int key, int missionKey, hashtable table) {
      SaveReal(table, missionKey, key, value);
    }

    [NativeLuaMemberAttribute]
    public static void SaveIntegerBJ(int value, int key, int missionKey, hashtable table) {
      SaveInteger(table, missionKey, key, value);
    }

    [NativeLuaMemberAttribute]
    public static void SaveBooleanBJ(bool value, int key, int missionKey, hashtable table) {
      SaveBoolean(table, missionKey, key, value);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveStringBJ(string value, int key, int missionKey, hashtable table) {
      return SaveStr(table, missionKey, key, value);
    }

    [NativeLuaMemberAttribute]
    public static bool SavePlayerHandleBJ(player whichPlayer, int key, int missionKey, hashtable table) {
      return SavePlayerHandle(table, missionKey, key, whichPlayer);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveWidgetHandleBJ(widget whichWidget, int key, int missionKey, hashtable table) {
      return SaveWidgetHandle(table, missionKey, key, whichWidget);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveDestructableHandleBJ(destructable whichDestructable, int key, int missionKey, hashtable table) {
      return SaveDestructableHandle(table, missionKey, key, whichDestructable);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveItemHandleBJ(item whichItem, int key, int missionKey, hashtable table) {
      return SaveItemHandle(table, missionKey, key, whichItem);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveUnitHandleBJ(unit whichUnit, int key, int missionKey, hashtable table) {
      return SaveUnitHandle(table, missionKey, key, whichUnit);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveAbilityHandleBJ(ability whichAbility, int key, int missionKey, hashtable table) {
      return SaveAbilityHandle(table, missionKey, key, whichAbility);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveTimerHandleBJ(timer whichTimer, int key, int missionKey, hashtable table) {
      return SaveTimerHandle(table, missionKey, key, whichTimer);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveTriggerHandleBJ(trigger whichTrigger, int key, int missionKey, hashtable table) {
      return SaveTriggerHandle(table, missionKey, key, whichTrigger);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveTriggerConditionHandleBJ(triggercondition whichTriggercondition, int key, int missionKey, hashtable table) {
      return SaveTriggerConditionHandle(table, missionKey, key, whichTriggercondition);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveTriggerActionHandleBJ(triggeraction whichTriggeraction, int key, int missionKey, hashtable table) {
      return SaveTriggerActionHandle(table, missionKey, key, whichTriggeraction);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveTriggerEventHandleBJ(@event whichEvent, int key, int missionKey, hashtable table) {
      return SaveTriggerEventHandle(table, missionKey, key, whichEvent);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveForceHandleBJ(force whichForce, int key, int missionKey, hashtable table) {
      return SaveForceHandle(table, missionKey, key, whichForce);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveGroupHandleBJ(group whichGroup, int key, int missionKey, hashtable table) {
      return SaveGroupHandle(table, missionKey, key, whichGroup);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveLocationHandleBJ(location whichLocation, int key, int missionKey, hashtable table) {
      return SaveLocationHandle(table, missionKey, key, whichLocation);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveRectHandleBJ(rect whichRect, int key, int missionKey, hashtable table) {
      return SaveRectHandle(table, missionKey, key, whichRect);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveBooleanExprHandleBJ(boolexpr whichBoolexpr, int key, int missionKey, hashtable table) {
      return SaveBooleanExprHandle(table, missionKey, key, whichBoolexpr);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveSoundHandleBJ(sound whichSound, int key, int missionKey, hashtable table) {
      return SaveSoundHandle(table, missionKey, key, whichSound);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveEffectHandleBJ(effect whichEffect, int key, int missionKey, hashtable table) {
      return SaveEffectHandle(table, missionKey, key, whichEffect);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveUnitPoolHandleBJ(unitpool whichUnitpool, int key, int missionKey, hashtable table) {
      return SaveUnitPoolHandle(table, missionKey, key, whichUnitpool);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveItemPoolHandleBJ(itempool whichItempool, int key, int missionKey, hashtable table) {
      return SaveItemPoolHandle(table, missionKey, key, whichItempool);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveQuestHandleBJ(quest whichQuest, int key, int missionKey, hashtable table) {
      return SaveQuestHandle(table, missionKey, key, whichQuest);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveQuestItemHandleBJ(questitem whichQuestitem, int key, int missionKey, hashtable table) {
      return SaveQuestItemHandle(table, missionKey, key, whichQuestitem);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveDefeatConditionHandleBJ(defeatcondition whichDefeatcondition, int key, int missionKey, hashtable table) {
      return SaveDefeatConditionHandle(table, missionKey, key, whichDefeatcondition);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveTimerDialogHandleBJ(timerdialog whichTimerdialog, int key, int missionKey, hashtable table) {
      return SaveTimerDialogHandle(table, missionKey, key, whichTimerdialog);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveLeaderboardHandleBJ(leaderboard whichLeaderboard, int key, int missionKey, hashtable table) {
      return SaveLeaderboardHandle(table, missionKey, key, whichLeaderboard);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveMultiboardHandleBJ(multiboard whichMultiboard, int key, int missionKey, hashtable table) {
      return SaveMultiboardHandle(table, missionKey, key, whichMultiboard);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveMultiboardItemHandleBJ(multiboarditem whichMultiboarditem, int key, int missionKey, hashtable table) {
      return SaveMultiboardItemHandle(table, missionKey, key, whichMultiboarditem);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveTrackableHandleBJ(trackable whichTrackable, int key, int missionKey, hashtable table) {
      return SaveTrackableHandle(table, missionKey, key, whichTrackable);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveDialogHandleBJ(dialog whichDialog, int key, int missionKey, hashtable table) {
      return SaveDialogHandle(table, missionKey, key, whichDialog);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveButtonHandleBJ(button whichButton, int key, int missionKey, hashtable table) {
      return SaveButtonHandle(table, missionKey, key, whichButton);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveTextTagHandleBJ(texttag whichTexttag, int key, int missionKey, hashtable table) {
      return SaveTextTagHandle(table, missionKey, key, whichTexttag);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveLightningHandleBJ(lightning whichLightning, int key, int missionKey, hashtable table) {
      return SaveLightningHandle(table, missionKey, key, whichLightning);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveImageHandleBJ(image whichImage, int key, int missionKey, hashtable table) {
      return SaveImageHandle(table, missionKey, key, whichImage);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveUbersplatHandleBJ(ubersplat whichUbersplat, int key, int missionKey, hashtable table) {
      return SaveUbersplatHandle(table, missionKey, key, whichUbersplat);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveRegionHandleBJ(region whichRegion, int key, int missionKey, hashtable table) {
      return SaveRegionHandle(table, missionKey, key, whichRegion);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveFogStateHandleBJ(fogstate whichFogState, int key, int missionKey, hashtable table) {
      return SaveFogStateHandle(table, missionKey, key, whichFogState);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveFogModifierHandleBJ(fogmodifier whichFogModifier, int key, int missionKey, hashtable table) {
      return SaveFogModifierHandle(table, missionKey, key, whichFogModifier);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveAgentHandleBJ(agent whichAgent, int key, int missionKey, hashtable table) {
      return SaveAgentHandle(table, missionKey, key, whichAgent);
    }

    [NativeLuaMemberAttribute]
    public static bool SaveHashtableHandleBJ(hashtable whichHashtable, int key, int missionKey, hashtable table) {
      return SaveHashtableHandle(table, missionKey, key, whichHashtable);
    }

    [NativeLuaMemberAttribute]
    public static float GetStoredRealBJ(string key, string missionKey, gamecache cache) {
      return GetStoredReal(cache, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static int GetStoredIntegerBJ(string key, string missionKey, gamecache cache) {
      return GetStoredInteger(cache, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static bool GetStoredBooleanBJ(string key, string missionKey, gamecache cache) {
      return GetStoredBoolean(cache, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static string GetStoredStringBJ(string key, string missionKey, gamecache cache) {
      string s = default;
      s = GetStoredString(cache, missionKey, key);
      if ((s == null)) {
        return string.Empty;
      } else {
        return s;
      }
    }

    [NativeLuaMemberAttribute]
    public static float LoadRealBJ(int key, int missionKey, hashtable table) {
      return LoadReal(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static int LoadIntegerBJ(int key, int missionKey, hashtable table) {
      return LoadInteger(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static bool LoadBooleanBJ(int key, int missionKey, hashtable table) {
      return LoadBoolean(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static string LoadStringBJ(int key, int missionKey, hashtable table) {
      string s = default;
      s = LoadStr(table, missionKey, key);
      if ((s == null)) {
        return string.Empty;
      } else {
        return s;
      }
    }

    [NativeLuaMemberAttribute]
    public static player LoadPlayerHandleBJ(int key, int missionKey, hashtable table) {
      return LoadPlayerHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static widget LoadWidgetHandleBJ(int key, int missionKey, hashtable table) {
      return LoadWidgetHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static destructable LoadDestructableHandleBJ(int key, int missionKey, hashtable table) {
      return LoadDestructableHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static item LoadItemHandleBJ(int key, int missionKey, hashtable table) {
      return LoadItemHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static unit LoadUnitHandleBJ(int key, int missionKey, hashtable table) {
      return LoadUnitHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static ability LoadAbilityHandleBJ(int key, int missionKey, hashtable table) {
      return LoadAbilityHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static timer LoadTimerHandleBJ(int key, int missionKey, hashtable table) {
      return LoadTimerHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static trigger LoadTriggerHandleBJ(int key, int missionKey, hashtable table) {
      return LoadTriggerHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static triggercondition LoadTriggerConditionHandleBJ(int key, int missionKey, hashtable table) {
      return LoadTriggerConditionHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static triggeraction LoadTriggerActionHandleBJ(int key, int missionKey, hashtable table) {
      return LoadTriggerActionHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static @event LoadTriggerEventHandleBJ(int key, int missionKey, hashtable table) {
      return LoadTriggerEventHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static force LoadForceHandleBJ(int key, int missionKey, hashtable table) {
      return LoadForceHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static group LoadGroupHandleBJ(int key, int missionKey, hashtable table) {
      return LoadGroupHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static location LoadLocationHandleBJ(int key, int missionKey, hashtable table) {
      return LoadLocationHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static rect LoadRectHandleBJ(int key, int missionKey, hashtable table) {
      return LoadRectHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static boolexpr LoadBooleanExprHandleBJ(int key, int missionKey, hashtable table) {
      return LoadBooleanExprHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static sound LoadSoundHandleBJ(int key, int missionKey, hashtable table) {
      return LoadSoundHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static effect LoadEffectHandleBJ(int key, int missionKey, hashtable table) {
      return LoadEffectHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static unitpool LoadUnitPoolHandleBJ(int key, int missionKey, hashtable table) {
      return LoadUnitPoolHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static itempool LoadItemPoolHandleBJ(int key, int missionKey, hashtable table) {
      return LoadItemPoolHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static quest LoadQuestHandleBJ(int key, int missionKey, hashtable table) {
      return LoadQuestHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static questitem LoadQuestItemHandleBJ(int key, int missionKey, hashtable table) {
      return LoadQuestItemHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static defeatcondition LoadDefeatConditionHandleBJ(int key, int missionKey, hashtable table) {
      return LoadDefeatConditionHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static timerdialog LoadTimerDialogHandleBJ(int key, int missionKey, hashtable table) {
      return LoadTimerDialogHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static leaderboard LoadLeaderboardHandleBJ(int key, int missionKey, hashtable table) {
      return LoadLeaderboardHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static multiboard LoadMultiboardHandleBJ(int key, int missionKey, hashtable table) {
      return LoadMultiboardHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static multiboarditem LoadMultiboardItemHandleBJ(int key, int missionKey, hashtable table) {
      return LoadMultiboardItemHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static trackable LoadTrackableHandleBJ(int key, int missionKey, hashtable table) {
      return LoadTrackableHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static dialog LoadDialogHandleBJ(int key, int missionKey, hashtable table) {
      return LoadDialogHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static button LoadButtonHandleBJ(int key, int missionKey, hashtable table) {
      return LoadButtonHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static texttag LoadTextTagHandleBJ(int key, int missionKey, hashtable table) {
      return LoadTextTagHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static lightning LoadLightningHandleBJ(int key, int missionKey, hashtable table) {
      return LoadLightningHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static image LoadImageHandleBJ(int key, int missionKey, hashtable table) {
      return LoadImageHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static ubersplat LoadUbersplatHandleBJ(int key, int missionKey, hashtable table) {
      return LoadUbersplatHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static region LoadRegionHandleBJ(int key, int missionKey, hashtable table) {
      return LoadRegionHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static fogstate LoadFogStateHandleBJ(int key, int missionKey, hashtable table) {
      return LoadFogStateHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static fogmodifier LoadFogModifierHandleBJ(int key, int missionKey, hashtable table) {
      return LoadFogModifierHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static hashtable LoadHashtableHandleBJ(int key, int missionKey, hashtable table) {
      return LoadHashtableHandle(table, missionKey, key);
    }

    [NativeLuaMemberAttribute]
    public static unit RestoreUnitLocFacingAngleBJ(string key, string missionKey, gamecache cache, player forWhichPlayer, location loc, float facing) {
      bj_lastLoadedUnit = RestoreUnit(cache, missionKey, key, forWhichPlayer, GetLocationX(loc), GetLocationY(loc), facing);
      return bj_lastLoadedUnit;
    }

    [NativeLuaMemberAttribute]
    public static unit RestoreUnitLocFacingPointBJ(string key, string missionKey, gamecache cache, player forWhichPlayer, location loc, location lookAt) {
      return RestoreUnitLocFacingAngleBJ(key, missionKey, cache, forWhichPlayer, loc, AngleBetweenPoints(loc, lookAt));
    }

    [NativeLuaMemberAttribute]
    public static unit GetLastRestoredUnitBJ() {
      return bj_lastLoadedUnit;
    }

    [NativeLuaMemberAttribute]
    public static void FlushGameCacheBJ(gamecache cache) {
      FlushGameCache(cache);
    }

    [NativeLuaMemberAttribute]
    public static void FlushStoredMissionBJ(string missionKey, gamecache cache) {
      FlushStoredMission(cache, missionKey);
    }

    [NativeLuaMemberAttribute]
    public static void FlushParentHashtableBJ(hashtable table) {
      FlushParentHashtable(table);
    }

    [NativeLuaMemberAttribute]
    public static void FlushChildHashtableBJ(int missionKey, hashtable table) {
      FlushChildHashtable(table, missionKey);
    }

    [NativeLuaMemberAttribute]
    public static bool HaveStoredValue(string key, int valueType, string missionKey, gamecache cache) {
      if ((valueType == bj_GAMECACHE_BOOLEAN)) {
        return HaveStoredBoolean(cache, missionKey, key);
      } else if ((valueType == bj_GAMECACHE_INTEGER)) {
        return HaveStoredInteger(cache, missionKey, key);
      } else if ((valueType == bj_GAMECACHE_REAL)) {
        return HaveStoredReal(cache, missionKey, key);
      } else if ((valueType == bj_GAMECACHE_UNIT)) {
        return HaveStoredUnit(cache, missionKey, key);
      } else if ((valueType == bj_GAMECACHE_STRING)) {
        return HaveStoredString(cache, missionKey, key);
      } else {
        return false;
      }
    }

    [NativeLuaMemberAttribute]
    public static bool HaveSavedValue(int key, int valueType, int missionKey, hashtable table) {
      if ((valueType == bj_HASHTABLE_BOOLEAN)) {
        return HaveSavedBoolean(table, missionKey, key);
      } else if ((valueType == bj_HASHTABLE_INTEGER)) {
        return HaveSavedInteger(table, missionKey, key);
      } else if ((valueType == bj_HASHTABLE_REAL)) {
        return HaveSavedReal(table, missionKey, key);
      } else if ((valueType == bj_HASHTABLE_STRING)) {
        return HaveSavedString(table, missionKey, key);
      } else if ((valueType == bj_HASHTABLE_HANDLE)) {
        return HaveSavedHandle(table, missionKey, key);
      } else {
        return false;
      }
    }

    [NativeLuaMemberAttribute]
    public static void ShowCustomCampaignButton(bool show, int whichButton) {
      SetCustomCampaignButtonVisible(whichButton - 1, show);
    }

    [NativeLuaMemberAttribute]
    public static bool IsCustomCampaignButtonVisibile(int whichButton) {
      return GetCustomCampaignButtonVisible(whichButton - 1);
    }

    [NativeLuaMemberAttribute]
    public static void LoadGameBJ(string loadFileName, bool doScoreScreen) {
      LoadGame(loadFileName, doScoreScreen);
    }

    [NativeLuaMemberAttribute]
    public static void SaveAndChangeLevelBJ(string saveFileName, string newLevel, bool doScoreScreen) {
      SaveGame(saveFileName);
      ChangeLevel(newLevel, doScoreScreen);
    }

    [NativeLuaMemberAttribute]
    public static void SaveAndLoadGameBJ(string saveFileName, string loadFileName, bool doScoreScreen) {
      SaveGame(saveFileName);
      LoadGame(loadFileName, doScoreScreen);
    }

    [NativeLuaMemberAttribute]
    public static bool RenameSaveDirectoryBJ(string sourceDirName, string destDirName) {
      return RenameSaveDirectory(sourceDirName, destDirName);
    }

    [NativeLuaMemberAttribute]
    public static bool RemoveSaveDirectoryBJ(string sourceDirName) {
      return RemoveSaveDirectory(sourceDirName);
    }

    [NativeLuaMemberAttribute]
    public static bool CopySaveGameBJ(string sourceSaveName, string destSaveName) {
      return CopySaveGame(sourceSaveName, destSaveName);
    }

    [NativeLuaMemberAttribute]
    public static float GetPlayerStartLocationX(player whichPlayer) {
      return GetStartLocationX(GetPlayerStartLocation(whichPlayer));
    }

    [NativeLuaMemberAttribute]
    public static float GetPlayerStartLocationY(player whichPlayer) {
      return GetStartLocationY(GetPlayerStartLocation(whichPlayer));
    }

    [NativeLuaMemberAttribute]
    public static location GetPlayerStartLocationLoc(player whichPlayer) {
      return GetStartLocationLoc(GetPlayerStartLocation(whichPlayer));
    }

    [NativeLuaMemberAttribute]
    public static location GetRectCenter(rect whichRect) {
      return Location(GetRectCenterX(whichRect), GetRectCenterY(whichRect));
    }

    [NativeLuaMemberAttribute]
    public static bool IsPlayerSlotState(player whichPlayer, playerslotstate whichState) {
      return GetPlayerSlotState(whichPlayer) == whichState;
    }

    [NativeLuaMemberAttribute]
    public static int GetFadeFromSeconds(float seconds) {
      if ((seconds != 0)) {
        return 128 / R2I(seconds);
      }

      return 10000;
    }

    [NativeLuaMemberAttribute]
    public static float GetFadeFromSecondsAsReal(float seconds) {
      if ((seconds != 0)) {
        return 128.00f / seconds;
      }

      return 10000.00f;
    }

    [NativeLuaMemberAttribute]
    public static void AdjustPlayerStateSimpleBJ(player whichPlayer, playerstate whichPlayerState, int delta) {
      SetPlayerState(whichPlayer, whichPlayerState, GetPlayerState(whichPlayer, whichPlayerState) + delta);
    }

    [NativeLuaMemberAttribute]
    public static void AdjustPlayerStateBJ(int delta, player whichPlayer, playerstate whichPlayerState) {
      if ((delta > 0)) {
        if ((whichPlayerState == PLAYER_STATE_RESOURCE_GOLD)) {
          AdjustPlayerStateSimpleBJ(whichPlayer, PLAYER_STATE_GOLD_GATHERED, delta);
        } else if ((whichPlayerState == PLAYER_STATE_RESOURCE_LUMBER)) {
          AdjustPlayerStateSimpleBJ(whichPlayer, PLAYER_STATE_LUMBER_GATHERED, delta);
        }
      }

      AdjustPlayerStateSimpleBJ(whichPlayer, whichPlayerState, delta);
    }

    [NativeLuaMemberAttribute]
    public static void SetPlayerStateBJ(player whichPlayer, playerstate whichPlayerState, int value) {
      int oldValue = GetPlayerState(whichPlayer, whichPlayerState);
      AdjustPlayerStateBJ(value - oldValue, whichPlayer, whichPlayerState);
    }

    [NativeLuaMemberAttribute]
    public static void SetPlayerFlagBJ(playerstate whichPlayerFlag, bool flag, player whichPlayer) {
      SetPlayerState(whichPlayer, whichPlayerFlag, IntegerTertiaryOp(flag, 1, 0));
    }

    [NativeLuaMemberAttribute]
    public static void SetPlayerTaxRateBJ(int rate, playerstate whichResource, player sourcePlayer, player otherPlayer) {
      SetPlayerTaxRate(sourcePlayer, otherPlayer, whichResource, rate);
    }

    [NativeLuaMemberAttribute]
    public static int GetPlayerTaxRateBJ(playerstate whichResource, player sourcePlayer, player otherPlayer) {
      return GetPlayerTaxRate(sourcePlayer, otherPlayer, whichResource);
    }

    [NativeLuaMemberAttribute]
    public static bool IsPlayerFlagSetBJ(playerstate whichPlayerFlag, player whichPlayer) {
      return GetPlayerState(whichPlayer, whichPlayerFlag) == 1;
    }

    [NativeLuaMemberAttribute]
    public static void AddResourceAmountBJ(int delta, unit whichUnit) {
      AddResourceAmount(whichUnit, delta);
    }

    [NativeLuaMemberAttribute]
    public static int GetConvertedPlayerId(player whichPlayer) {
      return GetPlayerId(whichPlayer) + 1;
    }

    [NativeLuaMemberAttribute]
    public static player ConvertedPlayer(int convertedPlayerId) {
      return Player(convertedPlayerId - 1);
    }

    [NativeLuaMemberAttribute]
    public static float GetRectWidthBJ(rect r) {
      return GetRectMaxX(r) - GetRectMinX(r);
    }

    [NativeLuaMemberAttribute]
    public static float GetRectHeightBJ(rect r) {
      return GetRectMaxY(r) - GetRectMinY(r);
    }

    [NativeLuaMemberAttribute]
    public static unit BlightGoldMineForPlayerBJ(unit goldMine, player whichPlayer) {
      float mineX = default;
      float mineY = default;
      int mineGold = default;
      unit newMine = default;
      if (GetUnitTypeId(goldMine) != 1852272492) {
        return null;
      }

      mineX = GetUnitX(goldMine);
      mineY = GetUnitY(goldMine);
      mineGold = GetResourceAmount(goldMine);
      RemoveUnit(goldMine);
      newMine = CreateBlightedGoldmine(whichPlayer, mineX, mineY, bj_UNIT_FACING);
      SetResourceAmount(newMine, mineGold);
      return newMine;
    }

    [NativeLuaMemberAttribute]
    public static unit BlightGoldMineForPlayer(unit goldMine, player whichPlayer) {
      bj_lastHauntedGoldMine = BlightGoldMineForPlayerBJ(goldMine, whichPlayer);
      return bj_lastHauntedGoldMine;
    }

    [NativeLuaMemberAttribute]
    public static unit GetLastHauntedGoldMine() {
      return bj_lastHauntedGoldMine;
    }

    [NativeLuaMemberAttribute]
    public static bool IsPointBlightedBJ(location where) {
      return IsPointBlighted(GetLocationX(where), GetLocationY(where));
    }

    [NativeLuaMemberAttribute]
    public static void SetPlayerColorBJEnum() {
      SetUnitColor(GetEnumUnit(), bj_setPlayerTargetColor);
    }

    [NativeLuaMemberAttribute]
    public static void SetPlayerColorBJ(player whichPlayer, playercolor color, bool changeExisting) {
      group g = default;
      SetPlayerColor(whichPlayer, color);
      if (changeExisting) {
        bj_setPlayerTargetColor = color;
        g = CreateGroup();
        GroupEnumUnitsOfPlayer(g, whichPlayer, null);
        ForGroup(g, SetPlayerColorBJEnum);
        DestroyGroup(g);
      }
    }

    [NativeLuaMemberAttribute]
    public static void SetPlayerUnitAvailableBJ(int unitId, bool allowed, player whichPlayer) {
      if (allowed) {
        SetPlayerTechMaxAllowed(whichPlayer, unitId, -1);
      } else {
        SetPlayerTechMaxAllowed(whichPlayer, unitId, 0);
      }
    }

    [NativeLuaMemberAttribute]
    public static void LockGameSpeedBJ() {
      SetMapFlag(MAP_LOCK_SPEED, true);
    }

    [NativeLuaMemberAttribute]
    public static void UnlockGameSpeedBJ() {
      SetMapFlag(MAP_LOCK_SPEED, false);
    }

    [NativeLuaMemberAttribute]
    public static bool IssueTargetOrderBJ(unit whichUnit, string order, widget targetWidget) {
      return IssueTargetOrder(whichUnit, order, targetWidget);
    }

    [NativeLuaMemberAttribute]
    public static bool IssuePointOrderLocBJ(unit whichUnit, string order, location whichLocation) {
      return IssuePointOrderLoc(whichUnit, order, whichLocation);
    }

    [NativeLuaMemberAttribute]
    public static bool IssueTargetDestructableOrder(unit whichUnit, string order, widget targetWidget) {
      return IssueTargetOrder(whichUnit, order, targetWidget);
    }

    [NativeLuaMemberAttribute]
    public static bool IssueTargetItemOrder(unit whichUnit, string order, widget targetWidget) {
      return IssueTargetOrder(whichUnit, order, targetWidget);
    }

    [NativeLuaMemberAttribute]
    public static bool IssueImmediateOrderBJ(unit whichUnit, string order) {
      return IssueImmediateOrder(whichUnit, order);
    }

    [NativeLuaMemberAttribute]
    public static bool GroupTargetOrderBJ(group whichGroup, string order, widget targetWidget) {
      return GroupTargetOrder(whichGroup, order, targetWidget);
    }

    [NativeLuaMemberAttribute]
    public static bool GroupPointOrderLocBJ(group whichGroup, string order, location whichLocation) {
      return GroupPointOrderLoc(whichGroup, order, whichLocation);
    }

    [NativeLuaMemberAttribute]
    public static bool GroupImmediateOrderBJ(group whichGroup, string order) {
      return GroupImmediateOrder(whichGroup, order);
    }

    [NativeLuaMemberAttribute]
    public static bool GroupTargetDestructableOrder(group whichGroup, string order, widget targetWidget) {
      return GroupTargetOrder(whichGroup, order, targetWidget);
    }

    [NativeLuaMemberAttribute]
    public static bool GroupTargetItemOrder(group whichGroup, string order, widget targetWidget) {
      return GroupTargetOrder(whichGroup, order, targetWidget);
    }

    [NativeLuaMemberAttribute]
    public static destructable GetDyingDestructable() {
      return GetTriggerDestructable();
    }

    [NativeLuaMemberAttribute]
    public static void SetUnitRallyPoint(unit whichUnit, location targPos) {
      IssuePointOrderLocBJ(whichUnit, "setrally", targPos);
    }

    [NativeLuaMemberAttribute]
    public static void SetUnitRallyUnit(unit whichUnit, unit targUnit) {
      IssueTargetOrder(whichUnit, "setrally", targUnit);
    }

    [NativeLuaMemberAttribute]
    public static void SetUnitRallyDestructable(unit whichUnit, destructable targDest) {
      IssueTargetOrder(whichUnit, "setrally", targDest);
    }

    [NativeLuaMemberAttribute]
    public static void SaveDyingWidget() {
      bj_lastDyingWidget = GetTriggerWidget();
    }

    [NativeLuaMemberAttribute]
    public static void SetBlightRectBJ(bool addBlight, player whichPlayer, rect r) {
      SetBlightRect(whichPlayer, r, addBlight);
    }

    [NativeLuaMemberAttribute]
    public static void SetBlightRadiusLocBJ(bool addBlight, player whichPlayer, location loc, float radius) {
      SetBlightLoc(whichPlayer, loc, radius, addBlight);
    }

    [NativeLuaMemberAttribute]
    public static string GetAbilityName(int abilcode) {
      return GetObjectName(abilcode);
    }

    [NativeLuaMemberAttribute]
    public static void MeleeStartingVisibility() {
      SetFloatGameState(GAME_STATE_TIME_OF_DAY, bj_MELEE_STARTING_TOD);
    }

    [NativeLuaMemberAttribute]
    public static void MeleeStartingResources() {
      int index = default;
      player indexPlayer = default;
      version v = default;
      int startingGold = default;
      int startingLumber = default;
      v = VersionGet();
      if ((v == VERSION_REIGN_OF_CHAOS)) {
        startingGold = bj_MELEE_STARTING_GOLD_V0;
        startingLumber = bj_MELEE_STARTING_LUMBER_V0;
      } else {
        startingGold = bj_MELEE_STARTING_GOLD_V1;
        startingLumber = bj_MELEE_STARTING_LUMBER_V1;
      }

      index = 0;
      while (true) {
        indexPlayer = Player(index);
        if ((GetPlayerSlotState(indexPlayer) == PLAYER_SLOT_STATE_PLAYING)) {
          SetPlayerState(indexPlayer, PLAYER_STATE_RESOURCE_GOLD, startingGold);
          SetPlayerState(indexPlayer, PLAYER_STATE_RESOURCE_LUMBER, startingLumber);
        }

        index = index + 1;
        if (index == bj_MAX_PLAYERS)
          break;
      }
    }

    [NativeLuaMemberAttribute]
    public static void ReducePlayerTechMaxAllowed(player whichPlayer, int techId, int limit) {
      int oldMax = GetPlayerTechMaxAllowed(whichPlayer, techId);
      if ((oldMax < 0 || oldMax > limit)) {
        SetPlayerTechMaxAllowed(whichPlayer, techId, limit);
      }
    }

    [NativeLuaMemberAttribute]
    public static void MeleeStartingHeroLimit() {
      int index = default;
      index = 0;
      while (true) {
        SetPlayerMaxHeroesAllowed(bj_MELEE_HERO_LIMIT, Player(index));
        ReducePlayerTechMaxAllowed(Player(index), 1214344551, bj_MELEE_HERO_TYPE_LIMIT);
        ReducePlayerTechMaxAllowed(Player(index), 1215130471, bj_MELEE_HERO_TYPE_LIMIT);
        ReducePlayerTechMaxAllowed(Player(index), 1215324524, bj_MELEE_HERO_TYPE_LIMIT);
        ReducePlayerTechMaxAllowed(Player(index), 1214409837, bj_MELEE_HERO_TYPE_LIMIT);
        ReducePlayerTechMaxAllowed(Player(index), 1331850337, bj_MELEE_HERO_TYPE_LIMIT);
        ReducePlayerTechMaxAllowed(Player(index), 1332109682, bj_MELEE_HERO_TYPE_LIMIT);
        ReducePlayerTechMaxAllowed(Player(index), 1333027688, bj_MELEE_HERO_TYPE_LIMIT);
        ReducePlayerTechMaxAllowed(Player(index), 1332963428, bj_MELEE_HERO_TYPE_LIMIT);
        ReducePlayerTechMaxAllowed(Player(index), 1164207469, bj_MELEE_HERO_TYPE_LIMIT);
        ReducePlayerTechMaxAllowed(Player(index), 1164666213, bj_MELEE_HERO_TYPE_LIMIT);
        ReducePlayerTechMaxAllowed(Player(index), 1164799855, bj_MELEE_HERO_TYPE_LIMIT);
        ReducePlayerTechMaxAllowed(Player(index), 1165451634, bj_MELEE_HERO_TYPE_LIMIT);
        ReducePlayerTechMaxAllowed(Player(index), 1432642913, bj_MELEE_HERO_TYPE_LIMIT);
        ReducePlayerTechMaxAllowed(Player(index), 1432646245, bj_MELEE_HERO_TYPE_LIMIT);
        ReducePlayerTechMaxAllowed(Player(index), 1433168227, bj_MELEE_HERO_TYPE_LIMIT);
        ReducePlayerTechMaxAllowed(Player(index), 1432580716, bj_MELEE_HERO_TYPE_LIMIT);
        ReducePlayerTechMaxAllowed(Player(index), 1315988077, bj_MELEE_HERO_TYPE_LIMIT);
        ReducePlayerTechMaxAllowed(Player(index), 1315074670, bj_MELEE_HERO_TYPE_LIMIT);
        ReducePlayerTechMaxAllowed(Player(index), 1315858291, bj_MELEE_HERO_TYPE_LIMIT);
        ReducePlayerTechMaxAllowed(Player(index), 1315990632, bj_MELEE_HERO_TYPE_LIMIT);
        ReducePlayerTechMaxAllowed(Player(index), 1315074932, bj_MELEE_HERO_TYPE_LIMIT);
        ReducePlayerTechMaxAllowed(Player(index), 1315007587, bj_MELEE_HERO_TYPE_LIMIT);
        ReducePlayerTechMaxAllowed(Player(index), 1316252014, bj_MELEE_HERO_TYPE_LIMIT);
        ReducePlayerTechMaxAllowed(Player(index), 1315334514, bj_MELEE_HERO_TYPE_LIMIT);
        index = index + 1;
        if (index == bj_MAX_PLAYERS)
          break;
      }
    }

    [NativeLuaMemberAttribute]
    public static bool MeleeTrainedUnitIsHeroBJFilter() {
      return IsUnitType(GetFilterUnit(), UNIT_TYPE_HERO);
    }

    [NativeLuaMemberAttribute]
    public static void MeleeGrantItemsToHero(unit whichUnit) {
      int owner = GetPlayerId(GetOwningPlayer(whichUnit));
      if ((bj_meleeTwinkedHeroes[owner] < bj_MELEE_MAX_TWINKED_HEROES)) {
        UnitAddItemById(whichUnit, 1937012592);
        bj_meleeTwinkedHeroes[owner] = bj_meleeTwinkedHeroes[owner] + 1;
      }
    }

    [NativeLuaMemberAttribute]
    public static void MeleeGrantItemsToTrainedHero() {
      MeleeGrantItemsToHero(GetTrainedUnit());
    }

    [NativeLuaMemberAttribute]
    public static void MeleeGrantItemsToHiredHero() {
      MeleeGrantItemsToHero(GetSoldUnit());
    }

    [NativeLuaMemberAttribute]
    public static void MeleeGrantHeroItems() {
      int index = default;
      trigger trig = default;
      index = 0;
      while (true) {
        bj_meleeTwinkedHeroes[index] = 0;
        index = index + 1;
        if (index == bj_MAX_PLAYER_SLOTS)
          break;
      }

      index = 0;
      while (true) {
        trig = CreateTrigger();
        TriggerRegisterPlayerUnitEvent(trig, Player(index), EVENT_PLAYER_UNIT_TRAIN_FINISH, filterMeleeTrainedUnitIsHeroBJ);
        TriggerAddAction(trig, MeleeGrantItemsToTrainedHero);
        index = index + 1;
        if (index == bj_MAX_PLAYERS)
          break;
      }

      trig = CreateTrigger();
      TriggerRegisterPlayerUnitEvent(trig, Player(PLAYER_NEUTRAL_PASSIVE), EVENT_PLAYER_UNIT_SELL, filterMeleeTrainedUnitIsHeroBJ);
      TriggerAddAction(trig, MeleeGrantItemsToHiredHero);
      bj_meleeGrantHeroItems = true;
    }

    [NativeLuaMemberAttribute]
    public static void MeleeClearExcessUnit() {
      unit theUnit = GetEnumUnit();
      int owner = GetPlayerId(GetOwningPlayer(theUnit));
      if ((owner == PLAYER_NEUTRAL_AGGRESSIVE)) {
        RemoveUnit(GetEnumUnit());
      } else if ((owner == PLAYER_NEUTRAL_PASSIVE)) {
        if (!IsUnitType(theUnit, UNIT_TYPE_STRUCTURE)) {
          RemoveUnit(GetEnumUnit());
        }
      }
    }

    [NativeLuaMemberAttribute]
    public static void MeleeClearNearbyUnits(float x, float y, float range) {
      group nearbyUnits = default;
      nearbyUnits = CreateGroup();
      GroupEnumUnitsInRange(nearbyUnits, x, y, range, null);
      ForGroup(nearbyUnits, MeleeClearExcessUnit);
      DestroyGroup(nearbyUnits);
    }

    [NativeLuaMemberAttribute]
    public static void MeleeClearExcessUnits() {
      int index = default;
      float locX = default;
      float locY = default;
      player indexPlayer = default;
      index = 0;
      while (true) {
        indexPlayer = Player(index);
        if ((GetPlayerSlotState(indexPlayer) == PLAYER_SLOT_STATE_PLAYING)) {
          locX = GetStartLocationX(GetPlayerStartLocation(indexPlayer));
          locY = GetStartLocationY(GetPlayerStartLocation(indexPlayer));
          MeleeClearNearbyUnits(locX, locY, bj_MELEE_CLEAR_UNITS_RADIUS);
        }

        index = index + 1;
        if (index == bj_MAX_PLAYERS)
          break;
      }
    }

    [NativeLuaMemberAttribute]
    public static void MeleeEnumFindNearestMine() {
      unit enumUnit = GetEnumUnit();
      float dist = default;
      location unitLoc = default;
      if ((GetUnitTypeId(enumUnit) == 1852272492)) {
        unitLoc = GetUnitLoc(enumUnit);
        dist = DistanceBetweenPoints(unitLoc, bj_meleeNearestMineToLoc);
        RemoveLocation(unitLoc);
        if ((bj_meleeNearestMineDist < 0) || (dist < bj_meleeNearestMineDist)) {
          bj_meleeNearestMine = enumUnit;
          bj_meleeNearestMineDist = dist;
        }
      }
    }

    [NativeLuaMemberAttribute]
    public static unit MeleeFindNearestMine(location src, float range) {
      group nearbyMines = default;
      bj_meleeNearestMine = null;
      bj_meleeNearestMineDist = -1;
      bj_meleeNearestMineToLoc = src;
      nearbyMines = CreateGroup();
      GroupEnumUnitsInRangeOfLoc(nearbyMines, src, range, null);
      ForGroup(nearbyMines, MeleeEnumFindNearestMine);
      DestroyGroup(nearbyMines);
      return bj_meleeNearestMine;
    }

    [NativeLuaMemberAttribute]
    public static unit MeleeRandomHeroLoc(player p, int id1, int id2, int id3, int id4, location loc) {
      unit hero = null;
      int roll = default;
      int pick = default;
      version v = default;
      v = VersionGet();
      if ((v == VERSION_REIGN_OF_CHAOS)) {
        roll = GetRandomInt(1, 3);
      } else {
        roll = GetRandomInt(1, 4);
      }

      if (roll == 1) {
        pick = id1;
      } else if (roll == 2) {
        pick = id2;
      } else if (roll == 3) {
        pick = id3;
      } else if (roll == 4) {
        pick = id4;
      } else {
        pick = id1;
      }

      hero = CreateUnitAtLoc(p, pick, loc, bj_UNIT_FACING);
      if (bj_meleeGrantHeroItems) {
        MeleeGrantItemsToHero(hero);
      }

      return hero;
    }

    [NativeLuaMemberAttribute]
    public static location MeleeGetProjectedLoc(location src, location targ, float distance, float deltaAngle) {
      float srcX = GetLocationX(src);
      float srcY = GetLocationY(src);
      float direction = Atan2(GetLocationY(targ) - srcY, GetLocationX(targ) - srcX) + deltaAngle;
      return Location(srcX + distance * Cos(direction), srcY + distance * Sin(direction));
    }

    [NativeLuaMemberAttribute]
    public static float MeleeGetNearestValueWithin(float val, float minVal, float maxVal) {
      if ((val < minVal)) {
        return minVal;
      } else if ((val > maxVal)) {
        return maxVal;
      } else {
        return val;
      }
    }

    [NativeLuaMemberAttribute]
    public static location MeleeGetLocWithinRect(location src, rect r) {
      float withinX = MeleeGetNearestValueWithin(GetLocationX(src), GetRectMinX(r), GetRectMaxX(r));
      float withinY = MeleeGetNearestValueWithin(GetLocationY(src), GetRectMinY(r), GetRectMaxY(r));
      return Location(withinX, withinY);
    }

    [NativeLuaMemberAttribute]
    public static void MeleeStartingUnitsHuman(player whichPlayer, location startLoc, bool doHeroes, bool doCamera, bool doPreload) {
      bool useRandomHero = IsMapFlagSet(MAP_RANDOM_HERO);
      float unitSpacing = 64.00f;
      unit nearestMine = default;
      location nearMineLoc = default;
      location heroLoc = default;
      float peonX = default;
      float peonY = default;
      unit townHall = null;
      if ((doPreload)) {
        Preloader("scripts\\HumanMelee.pld");
      }

      nearestMine = MeleeFindNearestMine(startLoc, bj_MELEE_MINE_SEARCH_RADIUS);
      if ((nearestMine != null)) {
        townHall = CreateUnitAtLoc(whichPlayer, 1752461175, startLoc, bj_UNIT_FACING);
        nearMineLoc = MeleeGetProjectedLoc(GetUnitLoc(nearestMine), startLoc, 320, 0);
        peonX = GetLocationX(nearMineLoc);
        peonY = GetLocationY(nearMineLoc);
        CreateUnit(whichPlayer, 1752196449, peonX + 0.00f * unitSpacing, peonY + 1.00f * unitSpacing, bj_UNIT_FACING);
        CreateUnit(whichPlayer, 1752196449, peonX + 1.00f * unitSpacing, peonY + 0.15f * unitSpacing, bj_UNIT_FACING);
        CreateUnit(whichPlayer, 1752196449, peonX - 1.00f * unitSpacing, peonY + 0.15f * unitSpacing, bj_UNIT_FACING);
        CreateUnit(whichPlayer, 1752196449, peonX + 0.60f * unitSpacing, peonY - 1.00f * unitSpacing, bj_UNIT_FACING);
        CreateUnit(whichPlayer, 1752196449, peonX - 0.60f * unitSpacing, peonY - 1.00f * unitSpacing, bj_UNIT_FACING);
        heroLoc = MeleeGetProjectedLoc(GetUnitLoc(nearestMine), startLoc, 384, 45);
      } else {
        townHall = CreateUnitAtLoc(whichPlayer, 1752461175, startLoc, bj_UNIT_FACING);
        peonX = GetLocationX(startLoc);
        peonY = GetLocationY(startLoc) - 224.00f;
        CreateUnit(whichPlayer, 1752196449, peonX + 2.00f * unitSpacing, peonY + 0.00f * unitSpacing, bj_UNIT_FACING);
        CreateUnit(whichPlayer, 1752196449, peonX + 1.00f * unitSpacing, peonY + 0.00f * unitSpacing, bj_UNIT_FACING);
        CreateUnit(whichPlayer, 1752196449, peonX + 0.00f * unitSpacing, peonY + 0.00f * unitSpacing, bj_UNIT_FACING);
        CreateUnit(whichPlayer, 1752196449, peonX - 1.00f * unitSpacing, peonY + 0.00f * unitSpacing, bj_UNIT_FACING);
        CreateUnit(whichPlayer, 1752196449, peonX - 2.00f * unitSpacing, peonY + 0.00f * unitSpacing, bj_UNIT_FACING);
        heroLoc = Location(peonX, peonY - 2.00f * unitSpacing);
      }

      if ((townHall != null)) {
        UnitAddAbilityBJ(1097689443, townHall);
        UnitMakeAbilityPermanentBJ(true, 1097689443, townHall);
      }

      if ((doHeroes)) {
        if (useRandomHero) {
          MeleeRandomHeroLoc(whichPlayer, 1214344551, 1215130471, 1215324524, 1214409837, heroLoc);
        } else {
          SetPlayerState(whichPlayer, PLAYER_STATE_RESOURCE_HERO_TOKENS, bj_MELEE_STARTING_HERO_TOKENS);
        }
      }

      if ((doCamera)) {
        SetCameraPositionForPlayer(whichPlayer, peonX, peonY);
        SetCameraQuickPositionForPlayer(whichPlayer, peonX, peonY);
      }
    }

    [NativeLuaMemberAttribute]
    public static void MeleeStartingUnitsOrc(player whichPlayer, location startLoc, bool doHeroes, bool doCamera, bool doPreload) {
      bool useRandomHero = IsMapFlagSet(MAP_RANDOM_HERO);
      float unitSpacing = 64.00f;
      unit nearestMine = default;
      location nearMineLoc = default;
      location heroLoc = default;
      float peonX = default;
      float peonY = default;
      if ((doPreload)) {
        Preloader("scripts\\OrcMelee.pld");
      }

      nearestMine = MeleeFindNearestMine(startLoc, bj_MELEE_MINE_SEARCH_RADIUS);
      if ((nearestMine != null)) {
        CreateUnitAtLoc(whichPlayer, 1869050469, startLoc, bj_UNIT_FACING);
        nearMineLoc = MeleeGetProjectedLoc(GetUnitLoc(nearestMine), startLoc, 320, 0);
        peonX = GetLocationX(nearMineLoc);
        peonY = GetLocationY(nearMineLoc);
        CreateUnit(whichPlayer, 1869636975, peonX + 0.00f * unitSpacing, peonY + 1.00f * unitSpacing, bj_UNIT_FACING);
        CreateUnit(whichPlayer, 1869636975, peonX + 1.00f * unitSpacing, peonY + 0.15f * unitSpacing, bj_UNIT_FACING);
        CreateUnit(whichPlayer, 1869636975, peonX - 1.00f * unitSpacing, peonY + 0.15f * unitSpacing, bj_UNIT_FACING);
        CreateUnit(whichPlayer, 1869636975, peonX + 0.60f * unitSpacing, peonY - 1.00f * unitSpacing, bj_UNIT_FACING);
        CreateUnit(whichPlayer, 1869636975, peonX - 0.60f * unitSpacing, peonY - 1.00f * unitSpacing, bj_UNIT_FACING);
        heroLoc = MeleeGetProjectedLoc(GetUnitLoc(nearestMine), startLoc, 384, 45);
      } else {
        CreateUnitAtLoc(whichPlayer, 1869050469, startLoc, bj_UNIT_FACING);
        peonX = GetLocationX(startLoc);
        peonY = GetLocationY(startLoc) - 224.00f;
        CreateUnit(whichPlayer, 1869636975, peonX + 2.00f * unitSpacing, peonY + 0.00f * unitSpacing, bj_UNIT_FACING);
        CreateUnit(whichPlayer, 1869636975, peonX + 1.00f * unitSpacing, peonY + 0.00f * unitSpacing, bj_UNIT_FACING);
        CreateUnit(whichPlayer, 1869636975, peonX + 0.00f * unitSpacing, peonY + 0.00f * unitSpacing, bj_UNIT_FACING);
        CreateUnit(whichPlayer, 1869636975, peonX - 1.00f * unitSpacing, peonY + 0.00f * unitSpacing, bj_UNIT_FACING);
        CreateUnit(whichPlayer, 1869636975, peonX - 2.00f * unitSpacing, peonY + 0.00f * unitSpacing, bj_UNIT_FACING);
        heroLoc = Location(peonX, peonY - 2.00f * unitSpacing);
      }

      if ((doHeroes)) {
        if (useRandomHero) {
          MeleeRandomHeroLoc(whichPlayer, 1331850337, 1332109682, 1333027688, 1332963428, heroLoc);
        } else {
          SetPlayerState(whichPlayer, PLAYER_STATE_RESOURCE_HERO_TOKENS, bj_MELEE_STARTING_HERO_TOKENS);
        }
      }

      if ((doCamera)) {
        SetCameraPositionForPlayer(whichPlayer, peonX, peonY);
        SetCameraQuickPositionForPlayer(whichPlayer, peonX, peonY);
      }
    }

    [NativeLuaMemberAttribute]
    public static void MeleeStartingUnitsUndead(player whichPlayer, location startLoc, bool doHeroes, bool doCamera, bool doPreload) {
      bool useRandomHero = IsMapFlagSet(MAP_RANDOM_HERO);
      float unitSpacing = 64.00f;
      unit nearestMine = default;
      location nearMineLoc = default;
      location nearTownLoc = default;
      location heroLoc = default;
      float peonX = default;
      float peonY = default;
      float ghoulX = default;
      float ghoulY = default;
      if ((doPreload)) {
        Preloader("scripts\\UndeadMelee.pld");
      }

      nearestMine = MeleeFindNearestMine(startLoc, bj_MELEE_MINE_SEARCH_RADIUS);
      if ((nearestMine != null)) {
        CreateUnitAtLoc(whichPlayer, 1970172012, startLoc, bj_UNIT_FACING);
        nearestMine = BlightGoldMineForPlayerBJ(nearestMine, whichPlayer);
        nearTownLoc = MeleeGetProjectedLoc(startLoc, GetUnitLoc(nearestMine), 288, 0);
        ghoulX = GetLocationX(nearTownLoc);
        ghoulY = GetLocationY(nearTownLoc);
        bj_ghoul[GetPlayerId(whichPlayer)] = CreateUnit(whichPlayer, 1969711215, ghoulX + 0.00f * unitSpacing, ghoulY + 0.00f * unitSpacing, bj_UNIT_FACING);
        nearMineLoc = MeleeGetProjectedLoc(GetUnitLoc(nearestMine), startLoc, 320, 0);
        peonX = GetLocationX(nearMineLoc);
        peonY = GetLocationY(nearMineLoc);
        CreateUnit(whichPlayer, 1969316719, peonX + 0.00f * unitSpacing, peonY + 0.50f * unitSpacing, bj_UNIT_FACING);
        CreateUnit(whichPlayer, 1969316719, peonX + 0.65f * unitSpacing, peonY - 0.50f * unitSpacing, bj_UNIT_FACING);
        CreateUnit(whichPlayer, 1969316719, peonX - 0.65f * unitSpacing, peonY - 0.50f * unitSpacing, bj_UNIT_FACING);
        SetBlightLoc(whichPlayer, nearMineLoc, 768, true);
        heroLoc = MeleeGetProjectedLoc(GetUnitLoc(nearestMine), startLoc, 384, 45);
      } else {
        CreateUnitAtLoc(whichPlayer, 1970172012, startLoc, bj_UNIT_FACING);
        peonX = GetLocationX(startLoc);
        peonY = GetLocationY(startLoc) - 224.00f;
        CreateUnit(whichPlayer, 1969316719, peonX - 1.50f * unitSpacing, peonY + 0.00f * unitSpacing, bj_UNIT_FACING);
        CreateUnit(whichPlayer, 1969316719, peonX - 0.50f * unitSpacing, peonY + 0.00f * unitSpacing, bj_UNIT_FACING);
        CreateUnit(whichPlayer, 1969316719, peonX + 0.50f * unitSpacing, peonY + 0.00f * unitSpacing, bj_UNIT_FACING);
        CreateUnit(whichPlayer, 1969711215, peonX + 1.50f * unitSpacing, peonY + 0.00f * unitSpacing, bj_UNIT_FACING);
        SetBlightLoc(whichPlayer, startLoc, 768, true);
        heroLoc = Location(peonX, peonY - 2.00f * unitSpacing);
      }

      if ((doHeroes)) {
        if (useRandomHero) {
          MeleeRandomHeroLoc(whichPlayer, 1432642913, 1432646245, 1433168227, 1432580716, heroLoc);
        } else {
          SetPlayerState(whichPlayer, PLAYER_STATE_RESOURCE_HERO_TOKENS, bj_MELEE_STARTING_HERO_TOKENS);
        }
      }

      if ((doCamera)) {
        SetCameraPositionForPlayer(whichPlayer, peonX, peonY);
        SetCameraQuickPositionForPlayer(whichPlayer, peonX, peonY);
      }
    }

    [NativeLuaMemberAttribute]
    public static void MeleeStartingUnitsNightElf(player whichPlayer, location startLoc, bool doHeroes, bool doCamera, bool doPreload) {
      bool useRandomHero = IsMapFlagSet(MAP_RANDOM_HERO);
      float unitSpacing = 64.00f;
      float minTreeDist = 3.50f * bj_CELLWIDTH;
      float minWispDist = 1.75f * bj_CELLWIDTH;
      unit nearestMine = default;
      location nearMineLoc = default;
      location wispLoc = default;
      location heroLoc = default;
      float peonX = default;
      float peonY = default;
      unit tree = default;
      if ((doPreload)) {
        Preloader("scripts\\NightElfMelee.pld");
      }

      nearestMine = MeleeFindNearestMine(startLoc, bj_MELEE_MINE_SEARCH_RADIUS);
      if ((nearestMine != null)) {
        nearMineLoc = MeleeGetProjectedLoc(GetUnitLoc(nearestMine), startLoc, 650, 0);
        nearMineLoc = MeleeGetLocWithinRect(nearMineLoc, GetRectFromCircleBJ(GetUnitLoc(nearestMine), minTreeDist));
        tree = CreateUnitAtLoc(whichPlayer, 1702129516, nearMineLoc, bj_UNIT_FACING);
        IssueTargetOrder(tree, "entangleinstant", nearestMine);
        wispLoc = MeleeGetProjectedLoc(GetUnitLoc(nearestMine), startLoc, 320, 0);
        wispLoc = MeleeGetLocWithinRect(wispLoc, GetRectFromCircleBJ(GetUnitLoc(nearestMine), minWispDist));
        peonX = GetLocationX(wispLoc);
        peonY = GetLocationY(wispLoc);
        CreateUnit(whichPlayer, 1702327152, peonX + 0.00f * unitSpacing, peonY + 1.00f * unitSpacing, bj_UNIT_FACING);
        CreateUnit(whichPlayer, 1702327152, peonX + 1.00f * unitSpacing, peonY + 0.15f * unitSpacing, bj_UNIT_FACING);
        CreateUnit(whichPlayer, 1702327152, peonX - 1.00f * unitSpacing, peonY + 0.15f * unitSpacing, bj_UNIT_FACING);
        CreateUnit(whichPlayer, 1702327152, peonX + 0.58f * unitSpacing, peonY - 1.00f * unitSpacing, bj_UNIT_FACING);
        CreateUnit(whichPlayer, 1702327152, peonX - 0.58f * unitSpacing, peonY - 1.00f * unitSpacing, bj_UNIT_FACING);
        heroLoc = MeleeGetProjectedLoc(GetUnitLoc(nearestMine), startLoc, 384, 45);
      } else {
        CreateUnitAtLoc(whichPlayer, 1702129516, startLoc, bj_UNIT_FACING);
        peonX = GetLocationX(startLoc);
        peonY = GetLocationY(startLoc) - 224.00f;
        CreateUnit(whichPlayer, 1702327152, peonX - 2.00f * unitSpacing, peonY + 0.00f * unitSpacing, bj_UNIT_FACING);
        CreateUnit(whichPlayer, 1702327152, peonX - 1.00f * unitSpacing, peonY + 0.00f * unitSpacing, bj_UNIT_FACING);
        CreateUnit(whichPlayer, 1702327152, peonX + 0.00f * unitSpacing, peonY + 0.00f * unitSpacing, bj_UNIT_FACING);
        CreateUnit(whichPlayer, 1702327152, peonX + 1.00f * unitSpacing, peonY + 0.00f * unitSpacing, bj_UNIT_FACING);
        CreateUnit(whichPlayer, 1702327152, peonX + 2.00f * unitSpacing, peonY + 0.00f * unitSpacing, bj_UNIT_FACING);
        heroLoc = Location(peonX, peonY - 2.00f * unitSpacing);
      }

      if ((doHeroes)) {
        if (useRandomHero) {
          MeleeRandomHeroLoc(whichPlayer, 1164207469, 1164666213, 1164799855, 1165451634, heroLoc);
        } else {
          SetPlayerState(whichPlayer, PLAYER_STATE_RESOURCE_HERO_TOKENS, bj_MELEE_STARTING_HERO_TOKENS);
        }
      }

      if ((doCamera)) {
        SetCameraPositionForPlayer(whichPlayer, peonX, peonY);
        SetCameraQuickPositionForPlayer(whichPlayer, peonX, peonY);
      }
    }

    [NativeLuaMemberAttribute]
    public static void MeleeStartingUnitsUnknownRace(player whichPlayer, location startLoc, bool doHeroes, bool doCamera, bool doPreload) {
      int index = default;
      if ((doPreload)) {
      }

      index = 0;
      while (true) {
        CreateUnit(whichPlayer, 1853057125, GetLocationX(startLoc) + GetRandomReal(-256, 256), GetLocationY(startLoc) + GetRandomReal(-256, 256), GetRandomReal(0, 360));
        index = index + 1;
        if (index == 12)
          break;
      }

      if ((doHeroes)) {
        SetPlayerState(whichPlayer, PLAYER_STATE_RESOURCE_HERO_TOKENS, bj_MELEE_STARTING_HERO_TOKENS);
      }

      if ((doCamera)) {
        SetCameraPositionLocForPlayer(whichPlayer, startLoc);
        SetCameraQuickPositionLocForPlayer(whichPlayer, startLoc);
      }
    }

    [NativeLuaMemberAttribute]
    public static void MeleeStartingUnits() {
      int index = default;
      player indexPlayer = default;
      location indexStartLoc = default;
      race indexRace = default;
      Preloader("scripts\\SharedMelee.pld");
      index = 0;
      while (true) {
        indexPlayer = Player(index);
        if ((GetPlayerSlotState(indexPlayer) == PLAYER_SLOT_STATE_PLAYING)) {
          indexStartLoc = GetStartLocationLoc(GetPlayerStartLocation(indexPlayer));
          indexRace = GetPlayerRace(indexPlayer);
          if ((indexRace == RACE_HUMAN)) {
            MeleeStartingUnitsHuman(indexPlayer, indexStartLoc, true, true, true);
          } else if ((indexRace == RACE_ORC)) {
            MeleeStartingUnitsOrc(indexPlayer, indexStartLoc, true, true, true);
          } else if ((indexRace == RACE_UNDEAD)) {
            MeleeStartingUnitsUndead(indexPlayer, indexStartLoc, true, true, true);
          } else if ((indexRace == RACE_NIGHTELF)) {
            MeleeStartingUnitsNightElf(indexPlayer, indexStartLoc, true, true, true);
          } else {
            MeleeStartingUnitsUnknownRace(indexPlayer, indexStartLoc, true, true, true);
          }
        }

        index = index + 1;
        if (index == bj_MAX_PLAYERS)
          break;
      }
    }

    [NativeLuaMemberAttribute]
    public static void MeleeStartingUnitsForPlayer(race whichRace, player whichPlayer, location loc, bool doHeroes) {
      if ((whichRace == RACE_HUMAN)) {
        MeleeStartingUnitsHuman(whichPlayer, loc, doHeroes, false, false);
      } else if ((whichRace == RACE_ORC)) {
        MeleeStartingUnitsOrc(whichPlayer, loc, doHeroes, false, false);
      } else if ((whichRace == RACE_UNDEAD)) {
        MeleeStartingUnitsUndead(whichPlayer, loc, doHeroes, false, false);
      } else if ((whichRace == RACE_NIGHTELF)) {
        MeleeStartingUnitsNightElf(whichPlayer, loc, doHeroes, false, false);
      } else {
      }
    }

    [NativeLuaMemberAttribute]
    public static void PickMeleeAI(player num, string s1, string s2, string s3) {
      int pick = default;
      if (GetAIDifficulty(num) == AI_DIFFICULTY_NEWBIE) {
        StartMeleeAI(num, s1);
        return;
      }

      if (s2 == null) {
        pick = 1;
      } else if (s3 == null) {
        pick = GetRandomInt(1, 2);
      } else {
        pick = GetRandomInt(1, 3);
      }

      if (pick == 1) {
        StartMeleeAI(num, s1);
      } else if (pick == 2) {
        StartMeleeAI(num, s2);
      } else {
        StartMeleeAI(num, s3);
      }
    }

    [NativeLuaMemberAttribute]
    public static void MeleeStartingAI() {
      int index = default;
      player indexPlayer = default;
      race indexRace = default;
      index = 0;
      while (true) {
        indexPlayer = Player(index);
        if ((GetPlayerSlotState(indexPlayer) == PLAYER_SLOT_STATE_PLAYING)) {
          indexRace = GetPlayerRace(indexPlayer);
          if ((GetPlayerController(indexPlayer) == MAP_CONTROL_COMPUTER)) {
            if ((indexRace == RACE_HUMAN)) {
              PickMeleeAI(indexPlayer, "human.ai", null, null);
            } else if ((indexRace == RACE_ORC)) {
              PickMeleeAI(indexPlayer, "orc.ai", null, null);
            } else if ((indexRace == RACE_UNDEAD)) {
              PickMeleeAI(indexPlayer, "undead.ai", null, null);
              RecycleGuardPosition(bj_ghoul[index]);
            } else if ((indexRace == RACE_NIGHTELF)) {
              PickMeleeAI(indexPlayer, "elf.ai", null, null);
            } else {
            }

            ShareEverythingWithTeamAI(indexPlayer);
          }
        }

        index = index + 1;
        if (index == bj_MAX_PLAYERS)
          break;
      }
    }

    [NativeLuaMemberAttribute]
    public static void LockGuardPosition(unit targ) {
      SetUnitCreepGuard(targ, true);
    }

    [NativeLuaMemberAttribute]
    public static bool MeleePlayerIsOpponent(int playerIndex, int opponentIndex) {
      player thePlayer = Player(playerIndex);
      player theOpponent = Player(opponentIndex);
      if ((playerIndex == opponentIndex)) {
        return false;
      }

      if ((GetPlayerSlotState(theOpponent) != PLAYER_SLOT_STATE_PLAYING)) {
        return false;
      }

      if ((bj_meleeDefeated[opponentIndex])) {
        return false;
      }

      if (GetPlayerAlliance(thePlayer, theOpponent, ALLIANCE_PASSIVE)) {
        if (GetPlayerAlliance(theOpponent, thePlayer, ALLIANCE_PASSIVE)) {
          if ((GetPlayerState(thePlayer, PLAYER_STATE_ALLIED_VICTORY) == 1)) {
            if ((GetPlayerState(theOpponent, PLAYER_STATE_ALLIED_VICTORY) == 1)) {
              return false;
            }
          }
        }
      }

      return true;
    }

    [NativeLuaMemberAttribute]
    public static int MeleeGetAllyStructureCount(player whichPlayer) {
      int playerIndex = default;
      int buildingCount = default;
      player indexPlayer = default;
      buildingCount = 0;
      playerIndex = 0;
      while (true) {
        indexPlayer = Player(playerIndex);
        if ((PlayersAreCoAllied(whichPlayer, indexPlayer))) {
          buildingCount = buildingCount + GetPlayerStructureCount(indexPlayer, true);
        }

        playerIndex = playerIndex + 1;
        if (playerIndex == bj_MAX_PLAYERS)
          break;
      }

      return buildingCount;
    }

    [NativeLuaMemberAttribute]
    public static int MeleeGetAllyCount(player whichPlayer) {
      int playerIndex = default;
      int playerCount = default;
      player indexPlayer = default;
      playerCount = 0;
      playerIndex = 0;
      while (true) {
        indexPlayer = Player(playerIndex);
        if (PlayersAreCoAllied(whichPlayer, indexPlayer) && !bj_meleeDefeated[playerIndex] && (whichPlayer != indexPlayer)) {
          playerCount = playerCount + 1;
        }

        playerIndex = playerIndex + 1;
        if (playerIndex == bj_MAX_PLAYERS)
          break;
      }

      return playerCount;
    }

    [NativeLuaMemberAttribute]
    public static int MeleeGetAllyKeyStructureCount(player whichPlayer) {
      int playerIndex = default;
      player indexPlayer = default;
      int keyStructs = default;
      keyStructs = 0;
      playerIndex = 0;
      while (true) {
        indexPlayer = Player(playerIndex);
        if ((PlayersAreCoAllied(whichPlayer, indexPlayer))) {
          keyStructs = keyStructs + GetPlayerTypedUnitCount(indexPlayer, "townhall", true, true);
          keyStructs = keyStructs + GetPlayerTypedUnitCount(indexPlayer, "greathall", true, true);
          keyStructs = keyStructs + GetPlayerTypedUnitCount(indexPlayer, "treeoflife", true, true);
          keyStructs = keyStructs + GetPlayerTypedUnitCount(indexPlayer, "necropolis", true, true);
        }

        playerIndex = playerIndex + 1;
        if (playerIndex == bj_MAX_PLAYERS)
          break;
      }

      return keyStructs;
    }

    [NativeLuaMemberAttribute]
    public static void MeleeDoDrawEnum() {
      player thePlayer = GetEnumPlayer();
      CachePlayerHeroData(thePlayer);
      RemovePlayerPreserveUnitsBJ(thePlayer, PLAYER_GAME_RESULT_TIE, false);
    }

    [NativeLuaMemberAttribute]
    public static void MeleeDoVictoryEnum() {
      player thePlayer = GetEnumPlayer();
      int playerIndex = GetPlayerId(thePlayer);
      if ((!bj_meleeVictoried[playerIndex])) {
        bj_meleeVictoried[playerIndex] = true;
        CachePlayerHeroData(thePlayer);
        RemovePlayerPreserveUnitsBJ(thePlayer, PLAYER_GAME_RESULT_VICTORY, false);
      }
    }

    [NativeLuaMemberAttribute]
    public static void MeleeDoDefeat(player whichPlayer) {
      bj_meleeDefeated[GetPlayerId(whichPlayer)] = true;
      RemovePlayerPreserveUnitsBJ(whichPlayer, PLAYER_GAME_RESULT_DEFEAT, false);
    }

    [NativeLuaMemberAttribute]
    public static void MeleeDoDefeatEnum() {
      player thePlayer = GetEnumPlayer();
      CachePlayerHeroData(thePlayer);
      MakeUnitsPassiveForTeam(thePlayer);
      MeleeDoDefeat(thePlayer);
    }

    [NativeLuaMemberAttribute]
    public static void MeleeDoLeave(player whichPlayer) {
      if ((GetIntegerGameState(GAME_STATE_DISCONNECTED) != 0)) {
        GameOverDialogBJ(whichPlayer, true);
      } else {
        bj_meleeDefeated[GetPlayerId(whichPlayer)] = true;
        RemovePlayerPreserveUnitsBJ(whichPlayer, PLAYER_GAME_RESULT_DEFEAT, true);
      }
    }

    [NativeLuaMemberAttribute]
    public static void MeleeRemoveObservers() {
      int playerIndex = default;
      player indexPlayer = default;
      playerIndex = 0;
      while (true) {
        indexPlayer = Player(playerIndex);
        if ((IsPlayerObserver(indexPlayer))) {
          RemovePlayerPreserveUnitsBJ(indexPlayer, PLAYER_GAME_RESULT_NEUTRAL, false);
        }

        playerIndex = playerIndex + 1;
        if (playerIndex == bj_MAX_PLAYERS)
          break;
      }
    }

    [NativeLuaMemberAttribute]
    public static force MeleeCheckForVictors() {
      int playerIndex = default;
      int opponentIndex = default;
      force opponentlessPlayers = CreateForce();
      bool gameOver = false;
      playerIndex = 0;
      while (true) {
        if ((!bj_meleeDefeated[playerIndex])) {
          opponentIndex = 0;
          while (true) {
            if (MeleePlayerIsOpponent(playerIndex, opponentIndex)) {
              return CreateForce();
            }

            opponentIndex = opponentIndex + 1;
            if (opponentIndex == bj_MAX_PLAYERS)
              break;
          }

          ForceAddPlayer(opponentlessPlayers, Player(playerIndex));
          gameOver = true;
        }

        playerIndex = playerIndex + 1;
        if (playerIndex == bj_MAX_PLAYERS)
          break;
      }

      bj_meleeGameOver = gameOver;
      return opponentlessPlayers;
    }

    [NativeLuaMemberAttribute]
    public static void MeleeCheckForLosersAndVictors() {
      int playerIndex = default;
      player indexPlayer = default;
      force defeatedPlayers = CreateForce();
      force victoriousPlayers = default;
      bool gameOver = false;
      if ((bj_meleeGameOver)) {
        return;
      }

      if ((GetIntegerGameState(GAME_STATE_DISCONNECTED) != 0)) {
        bj_meleeGameOver = true;
        return;
      }

      playerIndex = 0;
      while (true) {
        indexPlayer = Player(playerIndex);
        if ((!bj_meleeDefeated[playerIndex] && !bj_meleeVictoried[playerIndex])) {
          if ((MeleeGetAllyStructureCount(indexPlayer) <= 0)) {
            ForceAddPlayer(defeatedPlayers, Player(playerIndex));
            bj_meleeDefeated[playerIndex] = true;
          }
        }

        playerIndex = playerIndex + 1;
        if (playerIndex == bj_MAX_PLAYERS)
          break;
      }

      victoriousPlayers = MeleeCheckForVictors();
      ForForce(defeatedPlayers, MeleeDoDefeatEnum);
      ForForce(victoriousPlayers, MeleeDoVictoryEnum);
      if ((bj_meleeGameOver)) {
        MeleeRemoveObservers();
      }
    }

    [NativeLuaMemberAttribute]
    public static string MeleeGetCrippledWarningMessage(player whichPlayer) {
      race r = GetPlayerRace(whichPlayer);
      if ((r == RACE_HUMAN)) {
        return GetLocalizedString("CRIPPLE_WARNING_HUMAN");
      } else if ((r == RACE_ORC)) {
        return GetLocalizedString("CRIPPLE_WARNING_ORC");
      } else if ((r == RACE_NIGHTELF)) {
        return GetLocalizedString("CRIPPLE_WARNING_NIGHTELF");
      } else if ((r == RACE_UNDEAD)) {
        return GetLocalizedString("CRIPPLE_WARNING_UNDEAD");
      } else {
        return string.Empty;
      }
    }

    [NativeLuaMemberAttribute]
    public static string MeleeGetCrippledTimerMessage(player whichPlayer) {
      race r = GetPlayerRace(whichPlayer);
      if ((r == RACE_HUMAN)) {
        return GetLocalizedString("CRIPPLE_TIMER_HUMAN");
      } else if ((r == RACE_ORC)) {
        return GetLocalizedString("CRIPPLE_TIMER_ORC");
      } else if ((r == RACE_NIGHTELF)) {
        return GetLocalizedString("CRIPPLE_TIMER_NIGHTELF");
      } else if ((r == RACE_UNDEAD)) {
        return GetLocalizedString("CRIPPLE_TIMER_UNDEAD");
      } else {
        return string.Empty;
      }
    }

    [NativeLuaMemberAttribute]
    public static string MeleeGetCrippledRevealedMessage(player whichPlayer) {
      return GetLocalizedString("CRIPPLE_REVEALING_PREFIX") + GetPlayerName(whichPlayer) + GetLocalizedString("CRIPPLE_REVEALING_POSTFIX");
    }

    [NativeLuaMemberAttribute]
    public static void MeleeExposePlayer(player whichPlayer, bool expose) {
      int playerIndex = default;
      player indexPlayer = default;
      force toExposeTo = CreateForce();
      CripplePlayer(whichPlayer, toExposeTo, false);
      bj_playerIsExposed[GetPlayerId(whichPlayer)] = expose;
      playerIndex = 0;
      while (true) {
        indexPlayer = Player(playerIndex);
        if ((!PlayersAreCoAllied(whichPlayer, indexPlayer))) {
          ForceAddPlayer(toExposeTo, indexPlayer);
        }

        playerIndex = playerIndex + 1;
        if (playerIndex == bj_MAX_PLAYERS)
          break;
      }

      CripplePlayer(whichPlayer, toExposeTo, expose);
      DestroyForce(toExposeTo);
    }

    [NativeLuaMemberAttribute]
    public static void MeleeExposeAllPlayers() {
      int playerIndex = default;
      player indexPlayer = default;
      int playerIndex2 = default;
      player indexPlayer2 = default;
      force toExposeTo = CreateForce();
      playerIndex = 0;
      while (true) {
        indexPlayer = Player(playerIndex);
        ForceClear(toExposeTo);
        CripplePlayer(indexPlayer, toExposeTo, false);
        playerIndex2 = 0;
        while (true) {
          indexPlayer2 = Player(playerIndex2);
          if (playerIndex != playerIndex2) {
            if ((!PlayersAreCoAllied(indexPlayer, indexPlayer2))) {
              ForceAddPlayer(toExposeTo, indexPlayer2);
            }
          }

          playerIndex2 = playerIndex2 + 1;
          if (playerIndex2 == bj_MAX_PLAYERS)
            break;
        }

        CripplePlayer(indexPlayer, toExposeTo, true);
        playerIndex = playerIndex + 1;
        if (playerIndex == bj_MAX_PLAYERS)
          break;
      }

      DestroyForce(toExposeTo);
    }

    [NativeLuaMemberAttribute]
    public static void MeleeCrippledPlayerTimeout() {
      timer expiredTimer = GetExpiredTimer();
      int playerIndex = default;
      player exposedPlayer = default;
      playerIndex = 0;
      while (true) {
        if ((bj_crippledTimer[playerIndex] == expiredTimer)) {
          if (true)
            break;
        }

        playerIndex = playerIndex + 1;
        if (playerIndex == bj_MAX_PLAYERS)
          break;
      }

      if ((playerIndex == bj_MAX_PLAYERS)) {
        return;
      }

      exposedPlayer = Player(playerIndex);
      if ((GetLocalPlayer() == exposedPlayer)) {
        TimerDialogDisplay(bj_crippledTimerWindows[playerIndex], false);
      }

      DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_MELEE_CRIPPLE_MSG_DURATION, MeleeGetCrippledRevealedMessage(exposedPlayer));
      MeleeExposePlayer(exposedPlayer, true);
    }

    [NativeLuaMemberAttribute]
    public static bool MeleePlayerIsCrippled(player whichPlayer) {
      int allyStructures = MeleeGetAllyStructureCount(whichPlayer);
      int allyKeyStructures = MeleeGetAllyKeyStructureCount(whichPlayer);
      return (allyStructures > 0) && (allyKeyStructures <= 0);
    }

    [NativeLuaMemberAttribute]
    public static void MeleeCheckForCrippledPlayers() {
      int playerIndex = default;
      player indexPlayer = default;
      force crippledPlayers = CreateForce();
      bool isNowCrippled = default;
      race indexRace = default;
      if (bj_finishSoonAllExposed) {
        return;
      }

      playerIndex = 0;
      while (true) {
        indexPlayer = Player(playerIndex);
        isNowCrippled = MeleePlayerIsCrippled(indexPlayer);
        if ((!bj_playerIsCrippled[playerIndex] && isNowCrippled)) {
          bj_playerIsCrippled[playerIndex] = true;
          TimerStart(bj_crippledTimer[playerIndex], bj_MELEE_CRIPPLE_TIMEOUT, false, MeleeCrippledPlayerTimeout);
          if ((GetLocalPlayer() == indexPlayer)) {
            TimerDialogDisplay(bj_crippledTimerWindows[playerIndex], true);
            DisplayTimedTextToPlayer(indexPlayer, 0, 0, bj_MELEE_CRIPPLE_MSG_DURATION, MeleeGetCrippledWarningMessage(indexPlayer));
          }
        } else if ((bj_playerIsCrippled[playerIndex] && !isNowCrippled)) {
          bj_playerIsCrippled[playerIndex] = false;
          PauseTimer(bj_crippledTimer[playerIndex]);
          if ((GetLocalPlayer() == indexPlayer)) {
            TimerDialogDisplay(bj_crippledTimerWindows[playerIndex], false);
            if ((MeleeGetAllyStructureCount(indexPlayer) > 0)) {
              if ((bj_playerIsExposed[playerIndex])) {
                DisplayTimedTextToPlayer(indexPlayer, 0, 0, bj_MELEE_CRIPPLE_MSG_DURATION, GetLocalizedString("CRIPPLE_UNREVEALED"));
              } else {
                DisplayTimedTextToPlayer(indexPlayer, 0, 0, bj_MELEE_CRIPPLE_MSG_DURATION, GetLocalizedString("CRIPPLE_UNCRIPPLED"));
              }
            }
          }

          MeleeExposePlayer(indexPlayer, false);
        }

        playerIndex = playerIndex + 1;
        if (playerIndex == bj_MAX_PLAYERS)
          break;
      }
    }

    [NativeLuaMemberAttribute]
    public static void MeleeCheckLostUnit(unit lostUnit) {
      player lostUnitOwner = GetOwningPlayer(lostUnit);
      if ((GetPlayerStructureCount(lostUnitOwner, true) <= 0)) {
        MeleeCheckForLosersAndVictors();
      }

      MeleeCheckForCrippledPlayers();
    }

    [NativeLuaMemberAttribute]
    public static void MeleeCheckAddedUnit(unit addedUnit) {
      player addedUnitOwner = GetOwningPlayer(addedUnit);
      if ((bj_playerIsCrippled[GetPlayerId(addedUnitOwner)])) {
        MeleeCheckForCrippledPlayers();
      }
    }

    [NativeLuaMemberAttribute]
    public static void MeleeTriggerActionConstructCancel() {
      MeleeCheckLostUnit(GetCancelledStructure());
    }

    [NativeLuaMemberAttribute]
    public static void MeleeTriggerActionUnitDeath() {
      if ((IsUnitType(GetDyingUnit(), UNIT_TYPE_STRUCTURE))) {
        MeleeCheckLostUnit(GetDyingUnit());
      }
    }

    [NativeLuaMemberAttribute]
    public static void MeleeTriggerActionUnitConstructionStart() {
      MeleeCheckAddedUnit(GetConstructingStructure());
    }

    [NativeLuaMemberAttribute]
    public static void MeleeTriggerActionPlayerDefeated() {
      player thePlayer = GetTriggerPlayer();
      CachePlayerHeroData(thePlayer);
      if ((MeleeGetAllyCount(thePlayer) > 0)) {
        ShareEverythingWithTeam(thePlayer);
        if ((!bj_meleeDefeated[GetPlayerId(thePlayer)])) {
          MeleeDoDefeat(thePlayer);
        }
      } else {
        MakeUnitsPassiveForTeam(thePlayer);
        if ((!bj_meleeDefeated[GetPlayerId(thePlayer)])) {
          MeleeDoDefeat(thePlayer);
        }
      }

      MeleeCheckForLosersAndVictors();
    }

    [NativeLuaMemberAttribute]
    public static void MeleeTriggerActionPlayerLeft() {
      player thePlayer = GetTriggerPlayer();
      if ((IsPlayerObserver(thePlayer))) {
        RemovePlayerPreserveUnitsBJ(thePlayer, PLAYER_GAME_RESULT_NEUTRAL, false);
        return;
      }

      CachePlayerHeroData(thePlayer);
      if ((MeleeGetAllyCount(thePlayer) > 0)) {
        ShareEverythingWithTeam(thePlayer);
        MeleeDoLeave(thePlayer);
      } else {
        MakeUnitsPassiveForTeam(thePlayer);
        MeleeDoLeave(thePlayer);
      }

      MeleeCheckForLosersAndVictors();
    }

    [NativeLuaMemberAttribute]
    public static void MeleeTriggerActionAllianceChange() {
      MeleeCheckForLosersAndVictors();
      MeleeCheckForCrippledPlayers();
    }

    [NativeLuaMemberAttribute]
    public static void MeleeTriggerTournamentFinishSoon() {
      int playerIndex = default;
      player indexPlayer = default;
      float timeRemaining = GetTournamentFinishSoonTimeRemaining();
      if (!bj_finishSoonAllExposed) {
        bj_finishSoonAllExposed = true;
        playerIndex = 0;
        while (true) {
          indexPlayer = Player(playerIndex);
          if (bj_playerIsCrippled[playerIndex]) {
            bj_playerIsCrippled[playerIndex] = false;
            PauseTimer(bj_crippledTimer[playerIndex]);
            if ((GetLocalPlayer() == indexPlayer)) {
              TimerDialogDisplay(bj_crippledTimerWindows[playerIndex], false);
            }
          }

          playerIndex = playerIndex + 1;
          if (playerIndex == bj_MAX_PLAYERS)
            break;
        }

        MeleeExposeAllPlayers();
      }

      TimerDialogDisplay(bj_finishSoonTimerDialog, true);
      TimerDialogSetRealTimeRemaining(bj_finishSoonTimerDialog, timeRemaining);
    }

    [NativeLuaMemberAttribute]
    public static bool MeleeWasUserPlayer(player whichPlayer) {
      playerslotstate slotState = default;
      if ((GetPlayerController(whichPlayer) != MAP_CONTROL_USER)) {
        return false;
      }

      slotState = GetPlayerSlotState(whichPlayer);
      return (slotState == PLAYER_SLOT_STATE_PLAYING || slotState == PLAYER_SLOT_STATE_LEFT);
    }

    [NativeLuaMemberAttribute]
    public static void MeleeTournamentFinishNowRuleA(int multiplier) {
      int[] playerScore = new int[JASS_MAX_ARRAY_SIZE];
      int[] teamScore = new int[JASS_MAX_ARRAY_SIZE];
      force[] teamForce = new force[JASS_MAX_ARRAY_SIZE];
      int teamCount = default;
      int index = default;
      player indexPlayer = default;
      int index2 = default;
      player indexPlayer2 = default;
      int bestTeam = default;
      int bestScore = default;
      bool draw = default;
      index = 0;
      while (true) {
        indexPlayer = Player(index);
        if (MeleeWasUserPlayer(indexPlayer)) {
          playerScore[index] = GetTournamentScore(indexPlayer);
          if (playerScore[index] <= 0) {
            playerScore[index] = 1;
          }
        } else {
          playerScore[index] = 0;
        }

        index = index + 1;
        if (index == bj_MAX_PLAYERS)
          break;
      }

      teamCount = 0;
      index = 0;
      while (true) {
        if (playerScore[index] != 0) {
          indexPlayer = Player(index);
          teamScore[teamCount] = 0;
          teamForce[teamCount] = CreateForce();
          index2 = index;
          while (true) {
            if (playerScore[index2] != 0) {
              indexPlayer2 = Player(index2);
              if (PlayersAreCoAllied(indexPlayer, indexPlayer2)) {
                teamScore[teamCount] = teamScore[teamCount] + playerScore[index2];
                ForceAddPlayer(teamForce[teamCount], indexPlayer2);
                playerScore[index2] = 0;
              }
            }

            index2 = index2 + 1;
            if (index2 == bj_MAX_PLAYERS)
              break;
          }

          teamCount = teamCount + 1;
        }

        index = index + 1;
        if (index == bj_MAX_PLAYERS)
          break;
      }

      bj_meleeGameOver = true;
      if (teamCount != 0) {
        bestTeam = -1;
        bestScore = -1;
        index = 0;
        while (true) {
          if (teamScore[index] > bestScore) {
            bestTeam = index;
            bestScore = teamScore[index];
          }

          index = index + 1;
          if (index == teamCount)
            break;
        }

        draw = false;
        index = 0;
        while (true) {
          if (index != bestTeam) {
            if (bestScore < (multiplier * teamScore[index])) {
              draw = true;
            }
          }

          index = index + 1;
          if (index == teamCount)
            break;
        }

        if (draw) {
          index = 0;
          while (true) {
            ForForce(teamForce[index], MeleeDoDrawEnum);
            index = index + 1;
            if (index == teamCount)
              break;
          }
        } else {
          index = 0;
          while (true) {
            if (index != bestTeam) {
              ForForce(teamForce[index], MeleeDoDefeatEnum);
            }

            index = index + 1;
            if (index == teamCount)
              break;
          }

          ForForce(teamForce[bestTeam], MeleeDoVictoryEnum);
        }
      }
    }

    [NativeLuaMemberAttribute]
    public static void MeleeTriggerTournamentFinishNow() {
      int rule = GetTournamentFinishNowRule();
      if (bj_meleeGameOver) {
        return;
      }

      if ((rule == 1)) {
        MeleeTournamentFinishNowRuleA(1);
      } else {
        MeleeTournamentFinishNowRuleA(3);
      }

      MeleeRemoveObservers();
    }

    [NativeLuaMemberAttribute]
    public static void MeleeInitVictoryDefeat() {
      trigger trig = default;
      int index = default;
      player indexPlayer = default;
      bj_finishSoonTimerDialog = CreateTimerDialog(null);
      trig = CreateTrigger();
      TriggerRegisterGameEvent(trig, EVENT_GAME_TOURNAMENT_FINISH_SOON);
      TriggerAddAction(trig, MeleeTriggerTournamentFinishSoon);
      trig = CreateTrigger();
      TriggerRegisterGameEvent(trig, EVENT_GAME_TOURNAMENT_FINISH_NOW);
      TriggerAddAction(trig, MeleeTriggerTournamentFinishNow);
      index = 0;
      while (true) {
        indexPlayer = Player(index);
        if ((GetPlayerSlotState(indexPlayer) == PLAYER_SLOT_STATE_PLAYING)) {
          bj_meleeDefeated[index] = false;
          bj_meleeVictoried[index] = false;
          bj_playerIsCrippled[index] = false;
          bj_playerIsExposed[index] = false;
          bj_crippledTimer[index] = CreateTimer();
          bj_crippledTimerWindows[index] = CreateTimerDialog(bj_crippledTimer[index]);
          TimerDialogSetTitle(bj_crippledTimerWindows[index], MeleeGetCrippledTimerMessage(indexPlayer));
          trig = CreateTrigger();
          TriggerRegisterPlayerUnitEvent(trig, indexPlayer, EVENT_PLAYER_UNIT_CONSTRUCT_CANCEL, null);
          TriggerAddAction(trig, MeleeTriggerActionConstructCancel);
          trig = CreateTrigger();
          TriggerRegisterPlayerUnitEvent(trig, indexPlayer, EVENT_PLAYER_UNIT_DEATH, null);
          TriggerAddAction(trig, MeleeTriggerActionUnitDeath);
          trig = CreateTrigger();
          TriggerRegisterPlayerUnitEvent(trig, indexPlayer, EVENT_PLAYER_UNIT_CONSTRUCT_START, null);
          TriggerAddAction(trig, MeleeTriggerActionUnitConstructionStart);
          trig = CreateTrigger();
          TriggerRegisterPlayerEvent(trig, indexPlayer, EVENT_PLAYER_DEFEAT);
          TriggerAddAction(trig, MeleeTriggerActionPlayerDefeated);
          trig = CreateTrigger();
          TriggerRegisterPlayerEvent(trig, indexPlayer, EVENT_PLAYER_LEAVE);
          TriggerAddAction(trig, MeleeTriggerActionPlayerLeft);
          trig = CreateTrigger();
          TriggerRegisterPlayerAllianceChange(trig, indexPlayer, ALLIANCE_PASSIVE);
          TriggerRegisterPlayerStateEvent(trig, indexPlayer, PLAYER_STATE_ALLIED_VICTORY, EQUAL, 1);
          TriggerAddAction(trig, MeleeTriggerActionAllianceChange);
        } else {
          bj_meleeDefeated[index] = true;
          bj_meleeVictoried[index] = false;
          if ((IsPlayerObserver(indexPlayer))) {
            trig = CreateTrigger();
            TriggerRegisterPlayerEvent(trig, indexPlayer, EVENT_PLAYER_LEAVE);
            TriggerAddAction(trig, MeleeTriggerActionPlayerLeft);
          }
        }

        index = index + 1;
        if (index == bj_MAX_PLAYERS)
          break;
      }

      TimerStart(CreateTimer(), 2.0f, false, MeleeTriggerActionAllianceChange);
    }

    [NativeLuaMemberAttribute]
    public static void CheckInitPlayerSlotAvailability() {
      int index = default;
      if ((!bj_slotControlReady)) {
        index = 0;
        while (true) {
          bj_slotControlUsed[index] = false;
          bj_slotControl[index] = MAP_CONTROL_USER;
          index = index + 1;
          if (index == bj_MAX_PLAYERS)
            break;
        }

        bj_slotControlReady = true;
      }
    }

    [NativeLuaMemberAttribute]
    public static void SetPlayerSlotAvailable(player whichPlayer, mapcontrol control) {
      int playerIndex = GetPlayerId(whichPlayer);
      CheckInitPlayerSlotAvailability();
      bj_slotControlUsed[playerIndex] = true;
      bj_slotControl[playerIndex] = control;
    }

    [NativeLuaMemberAttribute]
    public static void TeamInitPlayerSlots(int teamCount) {
      int index = default;
      player indexPlayer = default;
      int team = default;
      SetTeams(teamCount);
      CheckInitPlayerSlotAvailability();
      index = 0;
      team = 0;
      while (true) {
        if ((bj_slotControlUsed[index])) {
          indexPlayer = Player(index);
          SetPlayerTeam(indexPlayer, team);
          team = team + 1;
          if ((team >= teamCount)) {
            team = 0;
          }
        }

        index = index + 1;
        if (index == bj_MAX_PLAYERS)
          break;
      }
    }

    [NativeLuaMemberAttribute]
    public static void MeleeInitPlayerSlots() {
      TeamInitPlayerSlots(bj_MAX_PLAYERS);
    }

    [NativeLuaMemberAttribute]
    public static void FFAInitPlayerSlots() {
      TeamInitPlayerSlots(bj_MAX_PLAYERS);
    }

    [NativeLuaMemberAttribute]
    public static void OneOnOneInitPlayerSlots() {
      SetTeams(2);
      SetPlayers(2);
      TeamInitPlayerSlots(2);
    }

    [NativeLuaMemberAttribute]
    public static void InitGenericPlayerSlots() {
      gametype gType = GetGameTypeSelected();
      if ((gType == GAME_TYPE_MELEE)) {
        MeleeInitPlayerSlots();
      } else if ((gType == GAME_TYPE_FFA)) {
        FFAInitPlayerSlots();
      } else if ((gType == GAME_TYPE_USE_MAP_SETTINGS)) {
      } else if ((gType == GAME_TYPE_ONE_ON_ONE)) {
        OneOnOneInitPlayerSlots();
      } else if ((gType == GAME_TYPE_TWO_TEAM_PLAY)) {
        TeamInitPlayerSlots(2);
      } else if ((gType == GAME_TYPE_THREE_TEAM_PLAY)) {
        TeamInitPlayerSlots(3);
      } else if ((gType == GAME_TYPE_FOUR_TEAM_PLAY)) {
        TeamInitPlayerSlots(4);
      } else {
      }
    }

    [NativeLuaMemberAttribute]
    public static void SetDNCSoundsDawn() {
      if (bj_useDawnDuskSounds) {
        StartSound(bj_dawnSound);
      }
    }

    [NativeLuaMemberAttribute]
    public static void SetDNCSoundsDusk() {
      if (bj_useDawnDuskSounds) {
        StartSound(bj_duskSound);
      }
    }

    [NativeLuaMemberAttribute]
    public static void SetDNCSoundsDay() {
      float ToD = GetTimeOfDay();
      if ((ToD >= bj_TOD_DAWN && ToD < bj_TOD_DUSK) && !bj_dncIsDaytime) {
        bj_dncIsDaytime = true;
        StopSound(bj_nightAmbientSound, false, true);
        StartSound(bj_dayAmbientSound);
      }
    }

    [NativeLuaMemberAttribute]
    public static void SetDNCSoundsNight() {
      float ToD = GetTimeOfDay();
      if ((ToD < bj_TOD_DAWN || ToD >= bj_TOD_DUSK) && bj_dncIsDaytime) {
        bj_dncIsDaytime = false;
        StopSound(bj_dayAmbientSound, false, true);
        StartSound(bj_nightAmbientSound);
      }
    }

    [NativeLuaMemberAttribute]
    public static void InitDNCSounds() {
      bj_dawnSound = CreateSoundFromLabel("RoosterSound", false, false, false, 10000, 10000);
      bj_duskSound = CreateSoundFromLabel("WolfSound", false, false, false, 10000, 10000);
      bj_dncSoundsDawn = CreateTrigger();
      TriggerRegisterGameStateEvent(bj_dncSoundsDawn, GAME_STATE_TIME_OF_DAY, EQUAL, bj_TOD_DAWN);
      TriggerAddAction(bj_dncSoundsDawn, SetDNCSoundsDawn);
      bj_dncSoundsDusk = CreateTrigger();
      TriggerRegisterGameStateEvent(bj_dncSoundsDusk, GAME_STATE_TIME_OF_DAY, EQUAL, bj_TOD_DUSK);
      TriggerAddAction(bj_dncSoundsDusk, SetDNCSoundsDusk);
      bj_dncSoundsDay = CreateTrigger();
      TriggerRegisterGameStateEvent(bj_dncSoundsDay, GAME_STATE_TIME_OF_DAY, GREATER_THAN_OR_EQUAL, bj_TOD_DAWN);
      TriggerRegisterGameStateEvent(bj_dncSoundsDay, GAME_STATE_TIME_OF_DAY, LESS_THAN, bj_TOD_DUSK);
      TriggerAddAction(bj_dncSoundsDay, SetDNCSoundsDay);
      bj_dncSoundsNight = CreateTrigger();
      TriggerRegisterGameStateEvent(bj_dncSoundsNight, GAME_STATE_TIME_OF_DAY, LESS_THAN, bj_TOD_DAWN);
      TriggerRegisterGameStateEvent(bj_dncSoundsNight, GAME_STATE_TIME_OF_DAY, GREATER_THAN_OR_EQUAL, bj_TOD_DUSK);
      TriggerAddAction(bj_dncSoundsNight, SetDNCSoundsNight);
    }

    [NativeLuaMemberAttribute]
    public static void InitBlizzardGlobals() {
      int index = default;
      int userControlledPlayers = default;
      version v = default;
      filterIssueHauntOrderAtLocBJ = Filter(IssueHauntOrderAtLocBJFilter);
      filterEnumDestructablesInCircleBJ = Filter(EnumDestructablesInCircleBJFilter);
      filterGetUnitsInRectOfPlayer = Filter(GetUnitsInRectOfPlayerFilter);
      filterGetUnitsOfTypeIdAll = Filter(GetUnitsOfTypeIdAllFilter);
      filterGetUnitsOfPlayerAndTypeId = Filter(GetUnitsOfPlayerAndTypeIdFilter);
      filterMeleeTrainedUnitIsHeroBJ = Filter(MeleeTrainedUnitIsHeroBJFilter);
      filterLivingPlayerUnitsOfTypeId = Filter(LivingPlayerUnitsOfTypeIdFilter);
      index = 0;
      while (true) {
        if (index == bj_MAX_PLAYER_SLOTS)
          break;
        bj_FORCE_PLAYER[index] = CreateForce();
        ForceAddPlayer(bj_FORCE_PLAYER[index], Player(index));
        index = index + 1;
      }

      bj_FORCE_ALL_PLAYERS = CreateForce();
      ForceEnumPlayers(bj_FORCE_ALL_PLAYERS, null);
      bj_cineModePriorSpeed = GetGameSpeed();
      bj_cineModePriorFogSetting = IsFogEnabled();
      bj_cineModePriorMaskSetting = IsFogMaskEnabled();
      index = 0;
      while (true) {
        if (index >= bj_MAX_QUEUED_TRIGGERS)
          break;
        bj_queuedExecTriggers[index] = null;
        bj_queuedExecUseConds[index] = false;
        index = index + 1;
      }

      bj_isSinglePlayer = false;
      userControlledPlayers = 0;
      index = 0;
      while (true) {
        if (index >= bj_MAX_PLAYERS)
          break;
        if ((GetPlayerController(Player(index)) == MAP_CONTROL_USER && GetPlayerSlotState(Player(index)) == PLAYER_SLOT_STATE_PLAYING)) {
          userControlledPlayers = userControlledPlayers + 1;
        }

        index = index + 1;
      }

      bj_isSinglePlayer = (userControlledPlayers == 1);
      bj_rescueSound = CreateSoundFromLabel("Rescue", false, false, false, 10000, 10000);
      bj_questDiscoveredSound = CreateSoundFromLabel("QuestNew", false, false, false, 10000, 10000);
      bj_questUpdatedSound = CreateSoundFromLabel("QuestUpdate", false, false, false, 10000, 10000);
      bj_questCompletedSound = CreateSoundFromLabel("QuestCompleted", false, false, false, 10000, 10000);
      bj_questFailedSound = CreateSoundFromLabel("QuestFailed", false, false, false, 10000, 10000);
      bj_questHintSound = CreateSoundFromLabel("Hint", false, false, false, 10000, 10000);
      bj_questSecretSound = CreateSoundFromLabel("SecretFound", false, false, false, 10000, 10000);
      bj_questItemAcquiredSound = CreateSoundFromLabel("ItemReward", false, false, false, 10000, 10000);
      bj_questWarningSound = CreateSoundFromLabel("Warning", false, false, false, 10000, 10000);
      bj_victoryDialogSound = CreateSoundFromLabel("QuestCompleted", false, false, false, 10000, 10000);
      bj_defeatDialogSound = CreateSoundFromLabel("QuestFailed", false, false, false, 10000, 10000);
      DelayedSuspendDecayCreate();
      v = VersionGet();
      if ((v == VERSION_REIGN_OF_CHAOS)) {
        bj_MELEE_MAX_TWINKED_HEROES = bj_MELEE_MAX_TWINKED_HEROES_V0;
      } else {
        bj_MELEE_MAX_TWINKED_HEROES = bj_MELEE_MAX_TWINKED_HEROES_V1;
      }
    }

    [NativeLuaMemberAttribute]
    public static void InitQueuedTriggers() {
      bj_queuedExecTimeout = CreateTrigger();
      TriggerRegisterTimerExpireEvent(bj_queuedExecTimeout, bj_queuedExecTimeoutTimer);
      TriggerAddAction(bj_queuedExecTimeout, QueuedTriggerDoneBJ);
    }

    [NativeLuaMemberAttribute]
    public static void InitMapRects() {
      bj_mapInitialPlayableArea = Rect(GetCameraBoundMinX() - GetCameraMargin(CAMERA_MARGIN_LEFT), GetCameraBoundMinY() - GetCameraMargin(CAMERA_MARGIN_BOTTOM), GetCameraBoundMaxX() + GetCameraMargin(CAMERA_MARGIN_RIGHT), GetCameraBoundMaxY() + GetCameraMargin(CAMERA_MARGIN_TOP));
      bj_mapInitialCameraBounds = GetCurrentCameraBoundsMapRectBJ();
    }

    [NativeLuaMemberAttribute]
    public static void InitSummonableCaps() {
      int index = default;
      index = 0;
      while (true) {
        if ((!GetPlayerTechResearched(Player(index), 1382576756, true))) {
          SetPlayerTechMaxAllowed(Player(index), 1752331380, 0);
        }

        if ((!GetPlayerTechResearched(Player(index), 1383031403, true))) {
          SetPlayerTechMaxAllowed(Player(index), 1869898347, 0);
        }

        SetPlayerTechMaxAllowed(Player(index), 1970498405, bj_MAX_SKELETONS);
        index = index + 1;
        if (index == bj_MAX_PLAYERS)
          break;
      }
    }

    [NativeLuaMemberAttribute]
    public static void UpdateStockAvailability(item whichItem) {
      itemtype iType = GetItemType(whichItem);
      int iLevel = GetItemLevel(whichItem);
      if ((iType == ITEM_TYPE_PERMANENT)) {
        bj_stockAllowedPermanent[iLevel] = true;
      } else if ((iType == ITEM_TYPE_CHARGED)) {
        bj_stockAllowedCharged[iLevel] = true;
      } else if ((iType == ITEM_TYPE_ARTIFACT)) {
        bj_stockAllowedArtifact[iLevel] = true;
      } else {
      }
    }

    [NativeLuaMemberAttribute]
    public static void UpdateEachStockBuildingEnum() {
      int iteration = 0;
      int pickedItemId = default;
      while (true) {
        pickedItemId = ChooseRandomItemEx(bj_stockPickedItemType, bj_stockPickedItemLevel);
        if (IsItemIdSellable(pickedItemId))
          break;
        iteration = iteration + 1;
        if ((iteration > bj_STOCK_MAX_ITERATIONS)) {
          return;
        }
      }

      AddItemToStock(GetEnumUnit(), pickedItemId, 1, 1);
    }

    [NativeLuaMemberAttribute]
    public static void UpdateEachStockBuilding(itemtype iType, int iLevel) {
      group g = default;
      bj_stockPickedItemType = iType;
      bj_stockPickedItemLevel = iLevel;
      g = CreateGroup();
      GroupEnumUnitsOfType(g, "marketplace", null);
      ForGroup(g, UpdateEachStockBuildingEnum);
      DestroyGroup(g);
    }

    [NativeLuaMemberAttribute]
    public static void PerformStockUpdates() {
      int pickedItemId = default;
      itemtype pickedItemType = default;
      int pickedItemLevel = 0;
      int allowedCombinations = 0;
      int iLevel = default;
      iLevel = 1;
      while (true) {
        if ((bj_stockAllowedPermanent[iLevel])) {
          allowedCombinations = allowedCombinations + 1;
          if ((GetRandomInt(1, allowedCombinations) == 1)) {
            pickedItemType = ITEM_TYPE_PERMANENT;
            pickedItemLevel = iLevel;
          }
        }

        if ((bj_stockAllowedCharged[iLevel])) {
          allowedCombinations = allowedCombinations + 1;
          if ((GetRandomInt(1, allowedCombinations) == 1)) {
            pickedItemType = ITEM_TYPE_CHARGED;
            pickedItemLevel = iLevel;
          }
        }

        if ((bj_stockAllowedArtifact[iLevel])) {
          allowedCombinations = allowedCombinations + 1;
          if ((GetRandomInt(1, allowedCombinations) == 1)) {
            pickedItemType = ITEM_TYPE_ARTIFACT;
            pickedItemLevel = iLevel;
          }
        }

        iLevel = iLevel + 1;
        if (iLevel > bj_MAX_ITEM_LEVEL)
          break;
      }

      if ((allowedCombinations == 0)) {
        return;
      }

      UpdateEachStockBuilding(pickedItemType, pickedItemLevel);
    }

    [NativeLuaMemberAttribute]
    public static void StartStockUpdates() {
      PerformStockUpdates();
      TimerStart(bj_stockUpdateTimer, bj_STOCK_RESTOCK_INTERVAL, true, PerformStockUpdates);
    }

    [NativeLuaMemberAttribute]
    public static void RemovePurchasedItem() {
      RemoveItemFromStock(GetSellingUnit(), GetItemTypeId(GetSoldItem()));
    }

    [NativeLuaMemberAttribute]
    public static void InitNeutralBuildings() {
      int iLevel = default;
      iLevel = 0;
      while (true) {
        bj_stockAllowedPermanent[iLevel] = false;
        bj_stockAllowedCharged[iLevel] = false;
        bj_stockAllowedArtifact[iLevel] = false;
        iLevel = iLevel + 1;
        if (iLevel > bj_MAX_ITEM_LEVEL)
          break;
      }

      SetAllItemTypeSlots(bj_MAX_STOCK_ITEM_SLOTS);
      SetAllUnitTypeSlots(bj_MAX_STOCK_UNIT_SLOTS);
      bj_stockUpdateTimer = CreateTimer();
      TimerStart(bj_stockUpdateTimer, bj_STOCK_RESTOCK_INITIAL_DELAY, false, StartStockUpdates);
      bj_stockItemPurchased = CreateTrigger();
      TriggerRegisterPlayerUnitEvent(bj_stockItemPurchased, Player(PLAYER_NEUTRAL_PASSIVE), EVENT_PLAYER_UNIT_SELL_ITEM, null);
      TriggerAddAction(bj_stockItemPurchased, RemovePurchasedItem);
    }

    [NativeLuaMemberAttribute]
    public static void MarkGameStarted() {
      bj_gameStarted = true;
      DestroyTimer(bj_gameStartedTimer);
    }

    [NativeLuaMemberAttribute]
    public static void DetectGameStarted() {
      bj_gameStartedTimer = CreateTimer();
      TimerStart(bj_gameStartedTimer, bj_GAME_STARTED_THRESHOLD, false, MarkGameStarted);
    }

    [NativeLuaMemberAttribute]
    public static void InitBlizzard() {
      ConfigureNeutralVictim();
      InitBlizzardGlobals();
      InitQueuedTriggers();
      InitRescuableBehaviorBJ();
      InitDNCSounds();
      InitMapRects();
      InitSummonableCaps();
      InitNeutralBuildings();
      DetectGameStarted();
    }

    [NativeLuaMemberAttribute]
    public static void RandomDistReset() {
      bj_randDistCount = 0;
    }

    [NativeLuaMemberAttribute]
    public static void RandomDistAddItem(int inID, int inChance) {
      bj_randDistID[bj_randDistCount] = inID;
      bj_randDistChance[bj_randDistCount] = inChance;
      bj_randDistCount = bj_randDistCount + 1;
    }

    [NativeLuaMemberAttribute]
    public static int RandomDistChoose() {
      int sum = 0;
      int chance = 0;
      int index = default;
      int foundID = -1;
      bool done = default;
      if ((bj_randDistCount == 0)) {
        return -1;
      }

      index = 0;
      while (true) {
        sum = sum + bj_randDistChance[index];
        index = index + 1;
        if (index == bj_randDistCount)
          break;
      }

      chance = GetRandomInt(1, sum);
      index = 0;
      sum = 0;
      done = false;
      while (true) {
        sum = sum + bj_randDistChance[index];
        if ((chance <= sum)) {
          foundID = bj_randDistID[index];
          done = true;
        }

        index = index + 1;
        if ((index == bj_randDistCount)) {
          done = true;
        }

        if (done == true)
          break;
      }

      return foundID;
    }

    [NativeLuaMemberAttribute]
    public static item UnitDropItem(unit inUnit, int inItemID) {
      float x = default;
      float y = default;
      float radius = 32;
      float unitX = default;
      float unitY = default;
      item droppedItem = default;
      if ((inItemID == -1)) {
        return null;
      }

      unitX = GetUnitX(inUnit);
      unitY = GetUnitY(inUnit);
      x = GetRandomReal(unitX - radius, unitX + radius);
      y = GetRandomReal(unitY - radius, unitY + radius);
      droppedItem = CreateItem(inItemID, x, y);
      SetItemDropID(droppedItem, GetUnitTypeId(inUnit));
      UpdateStockAvailability(droppedItem);
      return droppedItem;
    }

    [NativeLuaMemberAttribute]
    public static item WidgetDropItem(widget inWidget, int inItemID) {
      float x = default;
      float y = default;
      float radius = 32;
      float widgetX = default;
      float widgetY = default;
      if ((inItemID == -1)) {
        return null;
      }

      widgetX = GetWidgetX(inWidget);
      widgetY = GetWidgetY(inWidget);
      x = GetRandomReal(widgetX - radius, widgetX + radius);
      y = GetRandomReal(widgetY - radius, widgetY + radius);
      return CreateItem(inItemID, x, y);
    }

    [NativeLuaMemberAttribute]
    public static bool BlzIsLastInstanceObjectFunctionSuccessful() {
      return bj_lastInstObjFuncSuccessful;
    }

    [NativeLuaMemberAttribute]
    public static void BlzSetAbilityBooleanFieldBJ(ability whichAbility, abilitybooleanfield whichField, bool value) {
      bj_lastInstObjFuncSuccessful = BlzSetAbilityBooleanField(whichAbility, whichField, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzSetAbilityIntegerFieldBJ(ability whichAbility, abilityintegerfield whichField, int value) {
      bj_lastInstObjFuncSuccessful = BlzSetAbilityIntegerField(whichAbility, whichField, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzSetAbilityRealFieldBJ(ability whichAbility, abilityrealfield whichField, float value) {
      bj_lastInstObjFuncSuccessful = BlzSetAbilityRealField(whichAbility, whichField, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzSetAbilityStringFieldBJ(ability whichAbility, abilitystringfield whichField, string value) {
      bj_lastInstObjFuncSuccessful = BlzSetAbilityStringField(whichAbility, whichField, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzSetAbilityBooleanLevelFieldBJ(ability whichAbility, abilitybooleanlevelfield whichField, int level, bool value) {
      bj_lastInstObjFuncSuccessful = BlzSetAbilityBooleanLevelField(whichAbility, whichField, level, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzSetAbilityIntegerLevelFieldBJ(ability whichAbility, abilityintegerlevelfield whichField, int level, int value) {
      bj_lastInstObjFuncSuccessful = BlzSetAbilityIntegerLevelField(whichAbility, whichField, level, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzSetAbilityRealLevelFieldBJ(ability whichAbility, abilityreallevelfield whichField, int level, float value) {
      bj_lastInstObjFuncSuccessful = BlzSetAbilityRealLevelField(whichAbility, whichField, level, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzSetAbilityStringLevelFieldBJ(ability whichAbility, abilitystringlevelfield whichField, int level, string value) {
      bj_lastInstObjFuncSuccessful = BlzSetAbilityStringLevelField(whichAbility, whichField, level, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzSetAbilityBooleanLevelArrayFieldBJ(ability whichAbility, abilitybooleanlevelarrayfield whichField, int level, int index, bool value) {
      bj_lastInstObjFuncSuccessful = BlzSetAbilityBooleanLevelArrayField(whichAbility, whichField, level, index, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzSetAbilityIntegerLevelArrayFieldBJ(ability whichAbility, abilityintegerlevelarrayfield whichField, int level, int index, int value) {
      bj_lastInstObjFuncSuccessful = BlzSetAbilityIntegerLevelArrayField(whichAbility, whichField, level, index, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzSetAbilityRealLevelArrayFieldBJ(ability whichAbility, abilityreallevelarrayfield whichField, int level, int index, float value) {
      bj_lastInstObjFuncSuccessful = BlzSetAbilityRealLevelArrayField(whichAbility, whichField, level, index, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzSetAbilityStringLevelArrayFieldBJ(ability whichAbility, abilitystringlevelarrayfield whichField, int level, int index, string value) {
      bj_lastInstObjFuncSuccessful = BlzSetAbilityStringLevelArrayField(whichAbility, whichField, level, index, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzAddAbilityBooleanLevelArrayFieldBJ(ability whichAbility, abilitybooleanlevelarrayfield whichField, int level, bool value) {
      bj_lastInstObjFuncSuccessful = BlzAddAbilityBooleanLevelArrayField(whichAbility, whichField, level, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzAddAbilityIntegerLevelArrayFieldBJ(ability whichAbility, abilityintegerlevelarrayfield whichField, int level, int value) {
      bj_lastInstObjFuncSuccessful = BlzAddAbilityIntegerLevelArrayField(whichAbility, whichField, level, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzAddAbilityRealLevelArrayFieldBJ(ability whichAbility, abilityreallevelarrayfield whichField, int level, float value) {
      bj_lastInstObjFuncSuccessful = BlzAddAbilityRealLevelArrayField(whichAbility, whichField, level, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzAddAbilityStringLevelArrayFieldBJ(ability whichAbility, abilitystringlevelarrayfield whichField, int level, string value) {
      bj_lastInstObjFuncSuccessful = BlzAddAbilityStringLevelArrayField(whichAbility, whichField, level, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzRemoveAbilityBooleanLevelArrayFieldBJ(ability whichAbility, abilitybooleanlevelarrayfield whichField, int level, bool value) {
      bj_lastInstObjFuncSuccessful = BlzRemoveAbilityBooleanLevelArrayField(whichAbility, whichField, level, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzRemoveAbilityIntegerLevelArrayFieldBJ(ability whichAbility, abilityintegerlevelarrayfield whichField, int level, int value) {
      bj_lastInstObjFuncSuccessful = BlzRemoveAbilityIntegerLevelArrayField(whichAbility, whichField, level, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzRemoveAbilityRealLevelArrayFieldBJ(ability whichAbility, abilityreallevelarrayfield whichField, int level, float value) {
      bj_lastInstObjFuncSuccessful = BlzRemoveAbilityRealLevelArrayField(whichAbility, whichField, level, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzRemoveAbilityStringLevelArrayFieldBJ(ability whichAbility, abilitystringlevelarrayfield whichField, int level, string value) {
      bj_lastInstObjFuncSuccessful = BlzRemoveAbilityStringLevelArrayField(whichAbility, whichField, level, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzItemAddAbilityBJ(item whichItem, int abilCode) {
      bj_lastInstObjFuncSuccessful = BlzItemAddAbility(whichItem, abilCode);
    }

    [NativeLuaMemberAttribute]
    public static void BlzItemRemoveAbilityBJ(item whichItem, int abilCode) {
      bj_lastInstObjFuncSuccessful = BlzItemRemoveAbility(whichItem, abilCode);
    }

    [NativeLuaMemberAttribute]
    public static void BlzSetItemBooleanFieldBJ(item whichItem, itembooleanfield whichField, bool value) {
      bj_lastInstObjFuncSuccessful = BlzSetItemBooleanField(whichItem, whichField, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzSetItemIntegerFieldBJ(item whichItem, itemintegerfield whichField, int value) {
      bj_lastInstObjFuncSuccessful = BlzSetItemIntegerField(whichItem, whichField, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzSetItemRealFieldBJ(item whichItem, itemrealfield whichField, float value) {
      bj_lastInstObjFuncSuccessful = BlzSetItemRealField(whichItem, whichField, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzSetItemStringFieldBJ(item whichItem, itemstringfield whichField, string value) {
      bj_lastInstObjFuncSuccessful = BlzSetItemStringField(whichItem, whichField, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzSetUnitBooleanFieldBJ(unit whichUnit, unitbooleanfield whichField, bool value) {
      bj_lastInstObjFuncSuccessful = BlzSetUnitBooleanField(whichUnit, whichField, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzSetUnitIntegerFieldBJ(unit whichUnit, unitintegerfield whichField, int value) {
      bj_lastInstObjFuncSuccessful = BlzSetUnitIntegerField(whichUnit, whichField, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzSetUnitRealFieldBJ(unit whichUnit, unitrealfield whichField, float value) {
      bj_lastInstObjFuncSuccessful = BlzSetUnitRealField(whichUnit, whichField, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzSetUnitStringFieldBJ(unit whichUnit, unitstringfield whichField, string value) {
      bj_lastInstObjFuncSuccessful = BlzSetUnitStringField(whichUnit, whichField, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzSetUnitWeaponBooleanFieldBJ(unit whichUnit, unitweaponbooleanfield whichField, int index, bool value) {
      bj_lastInstObjFuncSuccessful = BlzSetUnitWeaponBooleanField(whichUnit, whichField, index, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzSetUnitWeaponIntegerFieldBJ(unit whichUnit, unitweaponintegerfield whichField, int index, int value) {
      bj_lastInstObjFuncSuccessful = BlzSetUnitWeaponIntegerField(whichUnit, whichField, index, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzSetUnitWeaponRealFieldBJ(unit whichUnit, unitweaponrealfield whichField, int index, float value) {
      bj_lastInstObjFuncSuccessful = BlzSetUnitWeaponRealField(whichUnit, whichField, index, value);
    }

    [NativeLuaMemberAttribute]
    public static void BlzSetUnitWeaponStringFieldBJ(unit whichUnit, unitweaponstringfield whichField, int index, string value) {
      bj_lastInstObjFuncSuccessful = BlzSetUnitWeaponStringField(whichUnit, whichField, index, value);
    }
  }
}
