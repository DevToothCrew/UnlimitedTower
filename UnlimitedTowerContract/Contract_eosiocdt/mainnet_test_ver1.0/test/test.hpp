    //void get_mail(eosio::name _user, uint32_t _type_index);

        //ACTION resultgacha(eosio::name _who, std::string _type, std::string _result);
    //ACTION resultparty(eosio::name _who, std::string _party_info, std::string _servant_list, std::string _monster_list);
	//ACTION battlestate(eosio::name _who, std::string _stage_info ,std::vector<std::string> &_my_state_list, std::vector<std::string> &_enemy_state_list);
    //ACTION battleaction(eosio::name _who, std::string _turn, std::vector<std::string> &_action_data);
    //ACTION battleresult(eosio::name _who, std::vector<std::string> &_reward);
    //ACTION contents(eosio::name _who, std::string _type, std::string _result);
    //ACTION contentslist(eosio::name _who, std::string _type, std::string _list);

     //ACTION itemburn(eosio::name _user, const std::vector<uint64_t> &_item_list, const std::vector<uint64_t> &_count_list);

         void deletebattle(eosio::name _user);
    void deleteuser(eosio::name _user);
    //ACTION alluserdel();
    //ACTION allbattle();

    #pragma region chat
    // ACTION chat(name _user, asset _price, string _text);
    // TABLE tchat
    // {
    //     name owner;
    //     asset price;
    //     name user;
    //     string text;
    //     uint64_t start_time;
    //     uint64_t primary_key() const { return owner.value; }
    // };
    // typedef eosio::multi_index<"tchat"_n, tchat> chat_index;
#pragma endregion

    // struct seed_info
    // {
    //     std::string type;
    //     uint64_t seed;
    //     uint64_t result;
    // };
    
    // TABLE tcheck
    // {
    //     uint64_t index;
    //     seed_info value;
    //     uint64_t primary_key() const { return index; }
    // };
    // typedef eosio::multi_index<"tcheck"_n, tcheck> seed_log;

    // TABLE ttemp
    // {
    //     uint64_t count;
    //     uint64_t primary_key() const {return count;}
    // };
    // typedef eosio::multi_index<"ttemp"_n, ttemp> global_count;

    //void set_seed(std::string _type, uint64_t _seed, uint64_t _result);
    //ACTION partycheat(eosio::name _user);
    //ACTION deleteuser2(eosio::name _user);
    //ACTION simulate(eosio::name _user, std::string _type, std::string _value);
    //ACTION lvcheat(eosio::name _user);

    //ACTION deletelog();
    //ACTION anothercheck(uint32_t _start_count);
    //ACTION anothercheck2(uint32_t _start_count);

    //치트키
//ACTION monstercheat(eosio::name _user, uint32_t _grade, uint32_t _id, uint32_t _count);
//ACTION balancetest(eosio::name _user, std::string _type, std::string _value);
//ACTION accountset(eosio::name _user);
//ACTION leveltest(eosio::name _user);
//ACTION updatecheack(uint32_t _start_count);
