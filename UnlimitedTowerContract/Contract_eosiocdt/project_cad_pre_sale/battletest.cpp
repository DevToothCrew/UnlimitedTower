#include "Common/common_header.hpp"
#include "Common/common_seed.hpp"
#include "Common/common_math.hpp"
#include "battletest.hpp"

// eosio.token recipient
// memo description spec
//-------------------------------------------------------------------------
void battletest::substr_value(std::string _value, std::vector<std::string> &_value_list, std::vector<size_t> &_size_list, uint32_t _size)
{
    _size_list.resize(_size);
    for (uint8_t i = 0; i < _size_list.size(); ++i)
    {
        if (i == 0)
        {
            _size_list[i] = _value.find(':');
            _value_list.push_back(_value.substr(0, _size_list[i]));
        }
        else
        {
            if (_value.find(':', _size_list[i - 1] + 1) == std::string::npos)
            {
                _size_list[i] = _value.length() - (_size_list[i - 1]);
                _value_list.push_back(_value.substr(_size_list[i - 1] + 1, _size_list[i]));
                break;
            }
            else
            {
                _size_list[i] = _value.find(':', _size_list[i - 1] + 1);
                _value_list.push_back(_value.substr(_size_list[i - 1] + 1, (_size_list[i] - 1) - _size_list[i - 1]));
            }
        }
    }
}

ACTION battletest::setpause(uint64_t _state)
{
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();

    permission_level master_auth;
    master_auth.actor = system_master_iter->master;
    master_auth.permission = "active"_n;
    require_auth(master_auth);

    system_master_table.modify(system_master_iter, _self, [&](auto &set_owner_account) {
        set_owner_account.state = _state;
    });
}

ACTION battletest::setmaster(eosio::name _master, uint8_t _type)
{
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();
    if (_type == 1)
    {
        if (system_master_iter == system_master_table.end())
        {
            require_auth(owner_auth);

            system_master_table.emplace(_self, [&](auto &set_master) {
                set_master.master = _self;
                set_master.state = system_state::pause;
            });
        }
        else
        {
            permission_level master_auth;
            master_auth.actor = system_master_iter->master;
            master_auth.permission = "owner"_n;
            require_auth(master_auth);

            system_master_table.emplace(_self, [&](auto &move_master) {
                move_master.master = _master;
                move_master.state = system_state::pause;
            });
            system_master_table.erase(system_master_iter);
        }
    }
    else
    {
        require_auth(owner_auth);

        system_master_table.erase(system_master_iter);

        system_master_table.emplace(_self, [&](auto &owner_master) {
            owner_master.master = _self;
            owner_master.state = system_state::pause;
        });
    }
}

void battletest::master_active_check()
{
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();

    permission_level master_auth;
    master_auth.actor = system_master_iter->master;
    master_auth.permission = "active"_n;
    require_auth(master_auth);
}

void battletest::system_check(eosio::name _user)
{
    require_auth(_user);
    system_master system_master_table(_self, _self.value);
    auto system_master_iter = system_master_table.begin();
    eosio_assert(system_master_iter != system_master_table.end(), "System Check : Need Master");
    if (system_master_iter->state == system_state::pause)
    {
        whitelist whitelist_table(_self, _self.value);
        auto whitelist_iter = whitelist_table.find(_user.value);
        eosio_assert(whitelist_iter != whitelist_table.end(), "System Check : Server Pause");
    }

    blacklist blacklist_table(_self, _self.value);
    auto blacklist_iter = blacklist_table.find(_user.value);
    eosio_assert(blacklist_iter == blacklist_table.end(), "System Check : BlackList User");
}

void battletest::white(eosio::name _user, std::string _type)
{
    whitelist referlist_tabe(_self, _self.value);
    auto referlist_iter = referlist_tabe.find(_user.value);
    if(_type == "add")
    {
        eosio_assert(referlist_iter == referlist_tabe.end(), "Add White : Already Exist");
        referlist_tabe.emplace(_self, [&](auto &new_refer) {
            new_refer.user = _user;
        });
    }
    if(_type == "delete")
    {
        eosio_assert(referlist_iter != referlist_tabe.end(), "Delete White : Not Exist");
        referlist_tabe.erase(referlist_iter);
    }
}

void battletest::black(eosio::name _user, std::string _type)
{
    blacklist blacklist_table(_self, _self.value);
    auto blacklist_iter = blacklist_table.find(_user.value);
    if (_type == "add")
    {
        eosio_assert(blacklist_iter == blacklist_table.end(), "Black List User 4");
        blacklist_table.emplace(_self, [&](auto &new_black_user) {
            new_black_user.user = _user;
        });
    }
    if (_type == "delete")
    {
        eosio_assert(blacklist_iter != blacklist_table.end(), "User Not Black List");
        blacklist_table.erase(blacklist_iter);
    }
}

ACTION battletest::systemact(std::string _function, eosio::name _user, std::string _type)
{
    master_active_check();
    if(_function == "white")
    {
        white(_user, _type);
    }
    if(_function == "black")
    {
        black(_user, _type);
    }
}

void battletest::get_pre_item(eosio::name _user, uint64_t _seed)
{
    user_item user_item_table(_self, _self.value);
    auto iter = user_item_table.find(_user.value);

    std::string data += to_string(_seed % 3);
    data += to_string(_seed % 10000);
    data += to_string(_seed % 10);
    if (iter != user_item_table.end())
    {
        user_item_table.emplace(_self, [&](auto &new_data) {
            new_data.user = _user;
            new_data.item_list.push_back(data);
        });
    }
    else
    {
    }
}


ACTION battletest::eostransfer(eosio::name sender, eosio::name receiver)
{
    eosiotoken_transfer(sender, receiver, [&](const auto &ad) {
        eosio_assert(ad.action.size() != 0, "Eos Transfer : Wrong Action");
        if (ad.action == "gacha")
        {
            get_pre_item(ad.user, ad.seed);
        }
    });
}

template <typename T>
void battletest::eosiotoken_transfer(eosio::name sender, eosio::name receiver, T func)
{
    require_auth(sender);
    auto transfer_data = eosio::unpack_action_data<st_transfer>();
    require_auth(transfer_data.from);
    eosio_assert(transfer_data.to == receiver, "Eos Transfer : Reciver Miss Match");
    eosio_assert(transfer_data.from == sender, "Eos Transfer : Sender Miss Match");
    eosio_assert(transfer_data.quantity.symbol == symbol("EOS", 4), "Eos Transfer : Only Accepts EOS");
    eosio_assert(transfer_data.quantity.is_valid(), "Eos Transfer : Invalid Token Transfer");
    eosio_assert(transfer_data.quantity.amount > 0, "Eos Transfer : Quantity Must Be Positive");

    std::vector<size_t> size_list;
    std::vector<std::string> value_list;

    transfer_action res;
    size_t l_center = transfer_data.memo.find(':');

    res.action = transfer_data.memo.substr(0, l_center);
    if (transfer_data.from == _self)
    {
        bool flag = false;
        if(transfer_data.to == "eosio.ram"_n)
        {
            flag = true;
        }
        if(transfer_data.to == "eosio.ramfee"_n)
        {
            flag = true;
        }
        if(transfer_data.to == "eosio.stake"_n)
        {
            flag = true;
        }

        if (flag == false)
        {
            system_master system_master_table(_self, _self.value);
            auto system_master_iter = system_master_table.find(transfer_data.to.value);
            eosio_assert(system_master_iter != system_master_table.end(), "Eos Transfer : Impossible Send EOS");
        }
    }
    else
    {
        system_master system_master_table(_self, _self.value);
        auto system_master_iter = system_master_table.find(transfer_data.from.value);
        if (system_master_iter == system_master_table.end())
        {
            system_check(transfer_data.from);
            if (res.action == "gacha")
            {
                substr_value(transfer_data.memo, value_list, size_list, 3);
                res.user = transfer_data.from;
                res.seed = safeseed::check_seed(value_list[1], value_list[2]);     
                eosio_assert(transfer_data.memo.find(':') != std::string::npos, "Eos Transfer Gacha : Seed Memo [:] Error");
                eosio_assert(transfer_data.memo.find(':', l_center + 1) != std::string::npos, "Eos Transfer Gacha : Seed Memo [:] Error");
                eosio_assert(transfer_data.quantity.amount == GACHA_PRICE, "Eos Transfer Gacha : Gacha need 0.0001 EOS"); //가격 필히 수정해야함 10000
            }
            else
            {
                eosio_assert(1 == 0, "Wrong Action 4");
            }
        }
    }

    func(res);
}




#undef EOSIO_DISPATCH

#define EOSIO_DISPATCH(TYPE, MEMBERS)                                                          \
    extern "C"                                                                                 \
    {                                                                                          \
        void apply(uint64_t receiver, uint64_t code, uint64_t action)                          \
        {                                                                                      \
            if (code == receiver)                                                              \
            {                                                                                  \
                eosio_assert(action != name("eostransfer").value, "Impossible This Action 1"); \
                switch (action)                                                                \
                {                                                                              \
                    EOSIO_DISPATCH_HELPER(TYPE, MEMBERS)                                       \
                }                                                                              \
                /* does not allow destructor of thiscontract to run: eosio_exit(0); */         \
            }                                                                                  \
            else if (code == name("eosio.token").value && action == name("transfer").value)    \
            {                                                                                  \
                eosio_assert(code == name("eosio.token").value, "Must transfer EOS");          \
                execute_action(name(receiver), name(code), &battletest::eostransfer);          \
            }                                                                                  \
        }                                                                                      \
    }

//(dbinsert)(dberase)(dblistinsert)(insertequipr)(dbinsert)(setdata)

EOSIO_DISPATCH(battletest,
              //admin
                (setmaster)(systemact)(setpause)(eostransfer)
			  )