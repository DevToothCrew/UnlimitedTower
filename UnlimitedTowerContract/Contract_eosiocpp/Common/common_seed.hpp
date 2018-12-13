#pragma once
#include "../Common/common_header.hpp"

namespace safeseed
{
uint64_t return_seed_value(uint64_t _user)
{
      uint64_t block_num = tapos_block_num();
      uint64_t block_prefix = tapos_block_prefix();
      uint64_t current_time = now();

      uint64_t first_seed = block_num * current_time;
      uint64_t second_seed;

      if(first_seed / block_num != current_time)
      {
            first_seed = block_num + block_prefix + current_time;
            if(first_seed < block_num)
            {
                  return _user + current_time;
            }
            return first_seed;
      }
      else
      {
            second_seed = first_seed * block_prefix;
            if(second_seed / first_seed != block_prefix)
            {
                  if(_user < current_time)
                  {
                        second_seed = current_time - _user;
                  }
                  else
                  {
                        second_seed = _user - current_time;
                  }
                  return second_seed;
            }
            return second_seed;
      }
}

uint64_t get_seed(uint64_t _user)
{
    uint64_t seed = return_seed_value(_user);

    checksum256 check_sum_result;
    sha256((char *)&seed, sizeof(seed), &check_sum_result);
    uint64_t *result_seed = reinterpret_cast<uint64_t *>(&check_sum_result.hash);

    return *result_seed;
}

uint64_t get_random_value(uint64_t _seed, uint32_t _max, uint32_t _min, uint32_t _random_count)
{
      _seed = ((_seed) >> (2 * _random_count));
      uint64_t l_result = _seed % _max;
      if (l_result < _min)
      {
            return l_result += _min;
      }
      return l_result;
}

}; // namespace safemath