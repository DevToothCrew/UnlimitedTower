#pragma once
#include "../Common/common_header.hpp"

namespace safemath
{
uint32_t check_under_minus_flow(uint32_t _a, uint32_t _b)
{
    int result = _a - _b;
    if (result <= 0)
    {
        return 0;
    }
    return uint32_t(result);
}

uint32_t check_over_plus_flow(uint32_t _a, uint32_t _b)
{
    uint32_t result = _a + _b;
    if (result < _a || result < _b)
    {
        return 0;
    }
    return result;
}

uint32_t check_over_mult_flow(uint32_t _a, uint32_t _b)
{
    uint32_t result = _a * _b;
    if ((_a != 0) && ((result / _a) != _b))
    {
        return 0;
    }
    return result;
}
uint32_t check_under_divide_flow(uint32_t _a, uint32_t _b)
{
    uint32_t result = _a / _b;
    if (_b == 0)
    {
        return 0;
    }
    return result;
}
};