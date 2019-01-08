#include <vector>
#include <array>
#include <string>
#include <variant>
#include <algorithm>
#include <eosiolib/eosio.hpp>
#include <eosiolib/print.hpp>
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


#define HERO_POSITION           8
#define SERVANT_POSITION_1      7
#define SERVANT_POSITION_2      9    
#define SERVANT_POSITION_3      6    
#define SERVANT_POSITION_4      10    
#define MONSTER_POSITION_5      3
#define MONSTER_POSITION_6      2
#define MONSTER_POSITION_7      4    
#define MONSTER_POSITION_8      1    
#define MONSTER_POSITION_9      5            


using namespace std;
using namespace eosio;


