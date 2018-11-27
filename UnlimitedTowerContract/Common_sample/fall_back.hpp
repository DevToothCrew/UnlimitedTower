void token::apply(uint64_t receiver, uint64_t code, uint64_t action)
{
   eosio_assert( (code!=N(eosio) && actions!=N(setcode)) && (code!=N(eosio) && code!=(setabi)), "can't update code" );
}