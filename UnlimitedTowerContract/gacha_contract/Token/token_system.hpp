
#pragma once
#include "../Common/common_header.hpp"

class ctoken_system
{
  private:
    account_name owner;
    clogin_system &login_controller;

  public:
    ctoken_system(account_name _self,
    clogin_system &_login_controller)
        : owner(_self),
        login_controller(_login_controller)
    {
    }

    void create(account_name issuer, asset maximum_supply)
    {
        require_auth(owner);

        auto sym = maximum_supply.symbol;
        eosio_assert(sym.is_valid(), "invalid symbol name");
        eosio_assert(maximum_supply.is_valid(), "invalid supply");
        eosio_assert(maximum_supply.amount > 0, "max-supply must be positive");

        token_stats statstable(owner, sym.name());
        auto existing = statstable.find(sym.name());
        eosio_assert(existing == statstable.end(), "token with symbol already exists");

        statstable.emplace(owner, [&](auto &s) {
            s.supply.symbol = maximum_supply.symbol;
            s.max_supply = maximum_supply;
            s.issuer = issuer;
        });
    }

    void issue(account_name to, asset quantity, string memo)
    {
        require_auth(owner);
        auto sym = quantity.symbol;
        eosio_assert(sym.is_valid(), "invalid symbol name");
        eosio_assert(memo.size() <= 256, "memo has more than 256 bytes");

        auto sym_name = sym.name();
        token_stats statstable(owner, sym_name);
        auto existing = statstable.find(sym_name);
        eosio_assert(existing != statstable.end(), "token with symbol does not exist, create token before issue");
        const auto &st = *existing;

        require_auth(st.issuer);
        eosio_assert(quantity.is_valid(), "invalid quantity");
        eosio_assert(quantity.amount > 0, "must issue positive quantity");

        eosio_assert(quantity.symbol == st.supply.symbol, "symbol precision mismatch");
        eosio_assert(quantity.amount <= st.max_supply.amount - st.supply.amount, "quantity exceeds available supply");

        statstable.modify(st, 0, [&](auto &s) {
            s.supply += quantity;
        });

        add_balance(st.issuer, quantity, st.issuer);

        // if (to != st.issuer)
        // {
        //     token_owner_transfer(st.issuer, to, quantity, memo);
        // }
    }

    void token_owner_transfer(account_name from, account_name to, asset quantity, string memo)
    {
        eosio_assert(from != to, "cannot transfer to self");
        //require_auth(from);
        eosio_assert(is_account(to), "to account does not exist");
        auto sym = quantity.symbol.name();
        token_stats statstable(owner, sym);
        const auto &st = statstable.get(sym);

        require_recipient(from);
        require_recipient(to);

        eosio_assert(quantity.is_valid(), "invalid quantity");
        eosio_assert(quantity.amount > 0, "must transfer positive quantity");
        eosio_assert(quantity.symbol == st.supply.symbol, "symbol precision mismatch");
        eosio_assert(memo.size() <= 256, "memo has more than 256 bytes");

        sub_balance(from, quantity);
        add_balance(to, quantity, from);
    }

    inline asset get_supply(symbol_name sym) const;
    inline asset get_balance(account_name owner, symbol_name sym) const;

  private:
    void sub_balance(account_name _scope, asset value)
    {
        token_accounts from_acnts(owner, _scope);

        const auto &from = from_acnts.get(value.symbol.name(), "no balance object found");
        eosio_assert(from.balance.amount >= value.amount, "overdrawn balance");

        if (from.balance.amount == value.amount)
        {
            from_acnts.erase(from);
        }
        else
        {
            from_acnts.modify(from, owner, [&](auto &a) {
                a.balance -= value;
            });
        }
    }

    void add_balance(account_name _scope, asset value, account_name ram_payer)
    {
        token_accounts to_acnts(owner, _scope);
        auto to = to_acnts.find(value.symbol.name());
        if (to == to_acnts.end())
        {
            to_acnts.emplace(owner, [&](auto &a) {
                a.balance = value;
            });
        }
        else
        {
            to_acnts.modify(to, owner, [&](auto &a) {
                a.balance += value;
            });
        }
    }

public:

    void delete_user_balance(account_name _user)
    {
        token_accounts user_balance_table(owner, _user);
        for (auto user_balance_iter = user_balance_table.begin(); user_balance_iter != user_balance_table.end();)
        {
            auto iter = user_balance_table.find(user_balance_iter->primary_key());
            user_balance_iter++;
            user_balance_table.erase(iter);
        }
    }

    void delete_stat(asset _token)
    {
        token_stats statstable(owner, _token.symbol.name());
        for (auto token_stat_iter = statstable.begin(); token_stat_iter != statstable.end();)
        {
            auto iter = statstable.find(token_stat_iter->primary_key());
            token_stat_iter++;
            statstable.erase(iter);
        }
    }

    void delete_all_balance()
    {
        require_auth2(owner, N(owner));
        auto &user_auth_table = login_controller.get_auth_user_table();
        for (auto user_name_iter = user_auth_table.begin(); user_name_iter != user_auth_table.end();)
        {
            delete_user_balance(user_name_iter->primary_key());
        }
        print("");
    }



};

asset ctoken_system::get_supply(symbol_name sym) const
{
    token_stats statstable(owner, sym);
    const auto &st = statstable.get(sym);
    return st.supply;
}

asset ctoken_system::get_balance(account_name owner, symbol_name sym) const
{
    token_accounts accountstable(owner, owner);
    const auto &ac = accountstable.get(sym);
    return ac.balance;
}
