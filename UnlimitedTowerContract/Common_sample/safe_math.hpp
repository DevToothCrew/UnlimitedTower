
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
