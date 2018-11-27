#include <vector>
#include <array>
#include <string>
#include <eosiolib/eosio.hpp>
#include <eosiolib/print.hpp>
#include <eosiolib/crypto.h>
#include <eosiolib/asset.hpp>

uint64_t mul(uint64_t a,uint64_t b)
{
    if(a==0 || b==0)
    {
        return 0;
    }
    uint64_t c = a * b;
    eosio_assert(c/a == b,"mul error");
    return c;
}
uint64_t div(uint64_t a,uint64_t b)
{
    eosio_assert(b != 0,"impossible div");
    if(a==0)
    {
        return 0;
    }
    uint64_t c = a / b;
    eosio_assert(a == b * c + a % b,"div error");
    return c;
}
uint64_t add(uint64_t a,uint64_t b)
{
    uint64_t c = a + b;
    eosio_assert(c >= a,"add error");
    return c;
}
uint64_t sub(uint64_t a,uint64_t b)
{
    eosio_assert(b<=a ,"impossible sub");
    return a-b;
}

//   function mul(uint256 a, uint256 b) internal pure returns (uint256 c) {
//     if (a == 0) {
//       return 0;
//     }
//     c = a * b;
//     assert(c / a == b);
//     return c;
//   }

//   /**
//   * @dev Integer division of two numbers, truncating the quotient.
//   */
//   function div(uint256 a, uint256 b) internal pure returns (uint256) {
//     // assert(b > 0); // Solidity automatically throws when dividing by 0
//     // uint256 c = a / b;
//     // assert(a == b * c + a % b); // There is no case in which this doesn't hold
//     return a / b;
//   }

//   /**
//   * @dev Subtracts two numbers, throws on overflow (i.e. if subtrahend is greater than minuend).
//   */
//   function sub(uint256 a, uint256 b) internal pure returns (uint256) {
//     assert(b <= a);
//     return a - b;
//   }

//   /**
//   * @dev Adds two numbers, throws on overflow.
//   */
//   function add(uint256 a, uint256 b) internal pure returns (uint256 c) {
//     c = a + b;
//     assert(c >= a);
//     return c;
//   }