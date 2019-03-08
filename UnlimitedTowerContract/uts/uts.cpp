#include "uts.hpp"

namespace eosio {
    using std::string;
    using eosio::asset;

    void uts::create( account_name issuer, string sym ) {
	    require_auth( _self );

	    // Check if issuer account exists
	    eosio_assert( is_account( issuer ), "issuer account does not exist");

        // Valid symbol
        asset supply(0, string_to_symbol(0, sym.c_str()));

        auto symbol = supply.symbol;
        eosio_assert( symbol.is_valid(), "invalid symbol name" );
        eosio_assert( supply.is_valid(), "invalid supply");

        // Check if currency with symbol already exists
        currency_index currency_table( _self, symbol.name() );
        auto existing_currency = currency_table.find( symbol.name() );
        eosio_assert( existing_currency == currency_table.end(), "token with symbol already exists" );

        // Create new currency
        currency_table.emplace( _self, [&]( auto& currency ) {
           currency.supply = supply;
           currency.issuer = issuer;
        });
    }

    void uts::issue(account_name to, asset quantity, uint64_t index)
    {
	    eosio_assert( is_account( to ), "to account does not exist");

        // e,g, Get EOS from 3 EOS
        symbol_type symbol = quantity.symbol;
        eosio_assert( symbol.is_valid(), "invalid symbol name" );
        eosio_assert( symbol.precision() == 0, "quantity must be a whole number" );

        // Ensure currency has been created
        auto symbol_name = symbol.name();
        currency_index currency_table( _self, symbol_name );
        auto existing_currency = currency_table.find( symbol_name );
        eosio_assert( existing_currency != currency_table.end(), "token with symbol does not exist. create token before issue" );
        const auto& st = *existing_currency;

        // Ensure have issuer authorization and valid quantity
        require_auth( st.issuer );
        eosio_assert( quantity.is_valid(), "invalid quantity" );
        eosio_assert( quantity.amount > 0, "must issue positive quantity of uts" );
        eosio_assert( symbol == st.supply.symbol, "symbol precision mismatch" );

        // Get Servant info
        servant_table servant(N(unlimittest1), to);
        auto servant_iter = servant.get(index, "Not exist servant");

        // Add token with creator paying for RAM
        tokens.emplace( to, [&]( auto& token ) {
            token.idx = tokens.available_primary_key();
            token.t_idx = index;
            token.s_idx = servant_iter.id;
            token.state = 0;
            token.exp = 0;
            token.stat_point = 0;
            token.status = servant_iter.status;

            token.owner = to;
            token.value = asset{1, symbol};
        });

        // Increase supply
	    add_supply( quantity );

        // Add balance to account
        add_balance( to, quantity, st.issuer );
    }

    void uts::transferid( account_name from, account_name to, id_type id)
    {
        // Ensure authorized to send from account
        eosio_assert( from != to, "cannot transfer to self" );
        require_auth( from );

        // Ensure 'to' account exists
        eosio_assert( is_account( to ), "to account does not exist");

        // Ensure token ID exists
        auto sender_token = tokens.find( id );
        eosio_assert( sender_token != tokens.end(), "token with specified ID does not exist" );

	    // Ensure owner owns token
        eosio_assert( sender_token->owner == from, "sender does not own token with specified ID");

	    const auto& st = *sender_token;

	    // Notify both recipients
        require_recipient( from );
        require_recipient( to );

        // Transfer uts from sender to receiver
        tokens.modify( st, from, [&]( auto& token ) {
	        token.owner = to;
        });

        // Change balance of both accounts
        sub_balance( from, st.value );
        add_balance( to, st.value, from );
    }

    void uts::sub_balance( account_name owner, asset value ) {

	account_index from_acnts( _self, owner );
        const auto& from = from_acnts.get( value.symbol.name(), "no balance object found" );
        eosio_assert( from.balance.amount >= value.amount, "overdrawn balance" );


        if( from.balance.amount == value.amount ) {
            from_acnts.erase( from );
        } else {
            from_acnts.modify( from, owner, [&]( auto& a ) {
                a.balance -= value;
            });
        }
    }

    void uts::add_balance( account_name owner, asset value, account_name ram_payer )
    {
        account_index to_accounts( _self, owner );
        auto to = to_accounts.find( value.symbol.name() );
        if( to == to_accounts.end() ) {
            to_accounts.emplace( ram_payer, [&]( auto& a ){
                a.balance = value;
            });
        } else {
            to_accounts.modify( to, 0, [&]( auto& a ) {
                a.balance += value;
            });
        }
    }

    void uts::sub_supply( asset quantity ) {
        auto symbol_name = quantity.symbol.name();
        currency_index currency_table( _self, symbol_name );
        auto current_currency = currency_table.find( symbol_name );

        currency_table.modify( current_currency, 0, [&]( auto& currency ) {
            currency.supply -= quantity;
        });
    }

    void uts::add_supply( asset quantity )
    {
        auto symbol_name = quantity.symbol.name();
        currency_index currency_table( _self, symbol_name );
        auto current_currency = currency_table.find( symbol_name );

        currency_table.modify( current_currency, 0, [&]( auto& currency ) {
            currency.supply += quantity;
        });
    }

EOSIO_ABI( uts, (create)(issue)(transferid) )

} /// namespace eosio
