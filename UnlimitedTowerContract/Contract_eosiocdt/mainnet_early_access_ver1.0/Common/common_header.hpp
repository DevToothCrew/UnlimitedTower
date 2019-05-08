#include <vector>
#include <array>
#include <string>
#include <variant>
#include <algorithm>
#include <eosiolib/eosio.hpp>
#include <eosiolib/crypto.h>
#include <eosiolib/asset.hpp>
#include <eosiolib/transaction.hpp>
#include <eosiolib/dispatcher.hpp>

#define DEFAULT_RANDOM_COUNT    0
#define DEFAULT_MIN             0

//============battle & party==============//
#define START_BATTLE            0
#define END_BATTLE              0
#define EMPTY_PARTY             0
#define HERO_LOCATION           0
#define PAIR_SLOT               5

//============Gacha==============//
//최대값의 + 1로 지정해야한다
#define GACHA_MAX_RATE          1000
#define DEFAULT_MIN_DB          1
#define MONSTER_GACHA_ID_START  20000
#define ITEM_GACHA_ID_START     30000
#define SERVANT_GACHA_ID_START  10000
#define SERVANT_JOB_COUNT       4
#define HEAD_COUNT              4
#define HAIR_COUNT              4
#define BODY_COUNT              3
#define GEMDER_COUNT            3

#define JOB_ACTIVE_START_ID             100
#define JOB_PRIVATE_ACTIVE_PER_ID        99

#define GACHA_ID                10000

#define GACHA_SERVANT_COUNT_ID  1
#define GACHA_MONSTER_COUNT_ID  2
#define GACHA_EQUIPMENT_COUNT_ID  3
#define GACHA_PUBLIC_PASSIVE_COUNT_ID  4
#define GACHA_PUBLIC_ACTIVE_COUNT_ID  5


//============Gacha==============//
#define MONSTER_UPGRADE_RATE        10000 
#define ITEM_UPGRADE_RATE     100

using namespace std;
using namespace eosio;


