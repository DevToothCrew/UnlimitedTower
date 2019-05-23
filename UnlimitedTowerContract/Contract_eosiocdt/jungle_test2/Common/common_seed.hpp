#pragma once
#include "../Common/common_header.hpp"

namespace safeseed
{
// uint64_t return_seed_value(uint64_t _owner, uint64_t _user)
// {
//       uint64_t block_num = tapos_block_num();
//       uint64_t block_prefix = tapos_block_prefix();
//       uint64_t current_time = now();

//       uint64_t second_seed;
//       uint64_t result_seed;

//       uint64_t user_seed = _owner ^ _user;
//       if(user_seed <= 0)
//       {
//             user_seed = _user;
//       }
//       second_seed = user_seed / current_time;

//       if (second_seed < block_prefix)
//       {
//             result_seed = second_seed * block_num;
//             if (result_seed / second_seed != block_num)
//             {
//                   return second_seed;
//             }
//       }
//       else
//       {
//             result_seed = second_seed * block_prefix;
//             if (result_seed / second_seed != block_prefix)
//             {
//                   result_seed = second_seed * block_num;
//                   if (result_seed / second_seed != block_num)
//                   {
//                         return second_seed;
//                   }
//                   return result_seed;
//             }
//       }
//       return result_seed;
// }

// uint64_t get_seed(uint64_t _owner, uint64_t _user)
// {
//       uint64_t seed = return_seed_value(_owner, _user);

//       capi_checksum256 check_sum_result;
//       sha256((char *)&seed, sizeof(seed), &check_sum_result);
//       uint64_t *result_seed = reinterpret_cast<uint64_t *>(&check_sum_result.hash);

//       return *result_seed;
// }

uint64_t get_user_seed(uint64_t _owner)
{
    uint64_t seed = _owner;

    capi_checksum256 check_sum_result;
    sha256((char *)&seed, sizeof(seed), &check_sum_result);
    uint64_t *result_seed = reinterpret_cast<uint64_t *>(&check_sum_result.hash);

    return *result_seed;
}


uint64_t get_seed_value(uint64_t _user,uint64_t _seed)
{
      uint64_t result_seed = _user + _seed;
      if(result_seed <= _user)
      {
           result_seed = (_user >> 2) + _seed;
           return get_user_seed(result_seed);
      }
      return get_user_seed(result_seed);
}



template<typename CharT>
static std::string to_hex(const CharT* d, uint32_t s) {
  std::string r;
  const char* to_hex="0123456789abcdef";
  uint8_t* c = (uint8_t*)d;
  for( uint32_t i = 0; i < s; ++i ) {
    (r += to_hex[(c[i] >> 4)]) += to_hex[(c[i] & 0x0f)];
  }
  return r;
}

uint64_t check_seed(std::string _seed, std::string _sha_result)
{
      std::string key = "5JKJTSYDTMLCH1BF9awxLrPAyyAvFyq35rGpJxwAopyH2c9gZ3h";

      std::string client_sha_seed = _sha_result;
      std::string client_seed = _seed;

      std::string seed = key + client_seed;

      capi_checksum256 check_sum_result;
      sha256((char *)seed.c_str(), seed.size() * sizeof(char), &check_sum_result);

      std::string hexstr = to_hex(&check_sum_result, sizeof(check_sum_result));

      eosio_assert(hexstr == client_sha_seed ,"not same key");

      uint64_t result_seed = atoll(client_seed.c_str());

      return result_seed;
}

uint64_t get_random_value(uint64_t _seed, uint32_t _max, uint32_t _min, uint32_t _random_count)
{
      _seed = ((_seed) >> (2 * _random_count));
      uint64_t l_result = _seed % _max;
      if (l_result < _min)
      {
            return l_result = _min;
      }
      return l_result;
}

std::vector<uint64_t>& get_total_rand(std::vector<uint64_t> &_array, uint32_t _total_value, uint64_t _seed)
{
     capi_checksum256 result;
     uint64_t mixedBlock = _seed;
     uint32_t max_num = _total_value - 2;
     uint32_t temp = _total_value - 1;
     const char *mixedChar = reinterpret_cast<const char *>(&mixedBlock);
     sha256( (char *)mixedChar, sizeof(mixedChar), &result);

     const char *p64 = reinterpret_cast<const char *>(&result);
     uint32_t A = (abs((int64_t)p64[0]) % (max_num) )+ 1;
     uint32_t B = (abs((int64_t)p64[1]) % (temp-A)) + 1;
     uint32_t C = _total_value - A - B ;

     _array.push_back(A);
     _array.push_back(B);
     _array.push_back(C);
     return _array;
}


void get_battle_rand_list(std::vector<uint64_t> &_array, uint64_t _seed)
{
      capi_checksum256 result;
      uint64_t mixedBlock = _seed;

      const char *mixedChar = reinterpret_cast<const char *>(&mixedBlock);
      sha256((char *)mixedChar, sizeof(mixedChar), &result);
      const char *p64 = reinterpret_cast<const char *>(&result);
      for (int i = 0; i < 20; i++)
      {
            uint64_t r = (abs((int64_t)p64[i]) % (9999999 + 1 - 1)) + 1;
            _array.push_back(r);
      }
}


uint64_t percent_rand(uint64_t _seed)
{
     capi_checksum256 result;
     uint64_t mixedBlock = _seed;

     const char *mixedChar = reinterpret_cast<const char *>(&mixedBlock);
     sha256((char *)mixedChar, sizeof(mixedChar), &result);
     const char *p64 = reinterpret_cast<const char *>(&result);
     uint64_t r = (abs((int64_t)p64) % (100 + 1 - 1)) + 1;
     return r;
}

std::vector<uint64_t>& get_status_rand(std::vector<uint64_t> &_array, uint32_t _seed)
{

    uint64_t _total_value = _seed;
    uint32_t max_num = _total_value - 2;
    uint32_t temp = _total_value - 1;
   // const char *mixedChar = reinterpret_cast<const char *>(&mixedBlock);
 //   sha256( (char *)mixedChar, sizeof(mixedChar), &result);

    const char *p64 = reinterpret_cast<const char *>(&_total_value);
    uint32_t A = (abs((int64_t)p64[0]) % (max_num) )+ 1;
    uint32_t B = (abs((int64_t)p64[1]) % (temp-A)) + 1;
    uint32_t C = _total_value - A - B ;

    _array.push_back(A);
    _array.push_back(B);
    _array.push_back(C);
    return _array;
}

}; // namespace safemath