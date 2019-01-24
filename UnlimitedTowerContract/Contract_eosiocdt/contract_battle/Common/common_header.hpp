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

#define DEFAULE_RANDOM_COUNT    0
#define DEFAULT_MIN             0
#define DEFAULT_MAX             10
#define START_BATTLE            1
#define END_BATTLE              0
#define NOT_APPEAR              0
#define EMPTY_PARTY             0
#define DEFAULT_MIN_DB          1
         

#define  MONSTER_ID_START       20000
#define  ITEM_ID_START          30000


#define SERVANT_JOB_COUNT       4
#define MONSTER_ID_COUNT        51
#define ITEM_ID_COUNT           33
#define HEAD_COUNT              4
#define HAIR_COUNT              4
#define BODY_COUNT              3
#define GEMDER_COUNT            3


using namespace std;
using namespace eosio;


