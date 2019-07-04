#include "Common/common_header.hpp"

CONTRACT battletest : public contract
{
  public:
    eosio::name owner;
    permission_level owner_auth;

    battletest(eosio::name _self, eosio::name _code, datastream<const char *> ds) : contract(_self, _code, ds)
    {
        owner = _self;
        owner_auth.actor = _self;
        owner_auth.permission = "owner"_n;
    }

    struct st_transfer
    {
        eosio::name from;
        eosio::name to;
        asset quantity;
        string memo;
    };

    struct transfer_action
    {
        name user;
        std::string action;
        uint64_t seed;
    };

    void substr_value(std::string _value, std::vector<std::string> & _value_list, std::vector<size_t> & _size_list, uint32_t _size);

    enum system_state
    {
        normal = 0,
        pause,
    };

    TABLE systemmaster
    {
        eosio::name master;
        uint8_t state;
        uint64_t primary_key() const { return master.value; }
    };
    typedef eosio::multi_index<"systemmaster"_n, systemmaster> system_master;
    ACTION setmaster(eosio::name _master, uint8_t _type);
    ACTION setpause(uint64_t _state);
    void master_active_check();
    void system_check(eosio::name _user);

    TABLE user
    {
        eosio::name user;
        uint64_t primary_key() const { return user.value; }
    };
    typedef eosio::multi_index<"whitelist"_n, user> whitelist;
    typedef eosio::multi_index<"blacklist"_n, user> blacklist;
    void white(eosio::name _user, std::string _type);
    void black(eosio::name _user, std::string _type);
    ACTION systemact(std::string _function, eosio::name _user, std::string _type);

    void get_pre_item(eosio::name _user, uint64_t _seed);

    ACTION eostransfer(eosio::name sender, eosio::name receiver);
    template <typename T>
    void eosiotoken_transfer(eosio::name sender, eosio::name receiver, T func);


    enum item_state
    {
        lock = 0,
        open = 1,
    };

    TABLE iteminfo
    {
        eosio::name user;
        std::vector<std::string> item_list;
        uint64_t primary_key() const { return user.value; }
    };
    typedef eosio::multi_index<"item"_n, iteminfo> user_item;

};