    //ACTION resultgacha(eosio::name _who, std::string _type, std::string _result);
    //ACTION resultparty(eosio::name _who, std::string _party_info, std::string _servant_list, std::string _monster_list);
	//ACTION battlestate(eosio::name _who, std::string _stage_info ,std::vector<std::string> &_my_state_list, std::vector<std::string> &_enemy_state_list);
    //ACTION battleaction(eosio::name _who, std::string _turn, std::vector<std::string> &_action_data);
    //ACTION battleresult(eosio::name _who, std::vector<std::string> &_reward);
    //ACTION contents(eosio::name _who, std::string _type, std::string _result);
    //ACTION contentslist(eosio::name _who, std::string _type, std::string _list);


    void add_party_list(eosio::name _user);

    void deletebattle(eosio::name _user);
    void deleteuser(eosio::name _user);
    // ACTION alluserdel();
    // ACTION allbattle();


    // struct seed_info
    // {
    //     std::string type;
    //     uint64_t seed;
    //     uint64_t result;
    // };
    
// TABLE ttemp
// {
//     eosio::name user;
//     uint64_t primary_key() const { return user.value; }
// };
// typedef eosio::multi_index<"ttemp"_n, ttemp> temp_list;

// TABLE tcount
// {
//     uint64_t count;
//     uint64_t primary_key() const { return count; }
// };
// typedef eosio::multi_index<"tcount"_n, tcount> global_count;

//ACTION deletetemp();
//ACTION recorduser(uint32_t _start_count);
//ACTION itemchange(eosio::name _user);

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
//ACTION accountset(eosio::name _user, uint32_t _ser_lv, uint32_t _mon_lv, uint32_t _equip_up, uint32_t _monster_up);
//ACTION leveltest(eosio::name _user);
//ACTION updatecheack(uint32_t _start_count);


#pragma region new servant monster

// struct skill_info
// {
//     uint32_t id;
//     uint32_t level;
// };

// struct new_servant_info
// {
//     uint32_t state;   //서번트 상태
//     uint32_t exp = 0; //서번트 경험치
//     uint64_t id = 0;
//     uint32_t level = 1;
//     uint32_t grade = 5;
//     status_info status;
//     std::vector<uint32_t> equip_slot; //서번트 장비 리스트
//     std::vector<skill_info> passive_skill;
//     std::vector<skill_info> active_skill;
// };

// TABLE tservants
// {
//     uint64_t index;
//     uint32_t party_number = 0;
//     new_servant_info servant;
//     uint64_t primary_key() const { return index; }
// };
// typedef eosio::multi_index<"tservants"_n, tservants> new_user_servants;
// struct new_monster_info
// {
//     uint64_t id;          //몬스터 id 값
//     uint32_t state;       //몬스터 상태값
//     uint32_t exp = 0;     //경험치
//     uint32_t type = 0;    //속성 타입
//     uint32_t tribe = 0;   //몬스터의 클래스
//     uint32_t grade;       // 등급
//     uint32_t upgrade = 0; //강화수치
//     uint32_t level = 1;
//     status_info status; //기본 힘,민,지 추가 힘,민,지
//     std::vector<skill_info> passive_skill;
//     std::vector<skill_info> active_skill;
// };

// TABLE tmonsters
// {
//     uint64_t index;
//     uint32_t party_number = 0;
//     new_monster_info monster;
//     uint64_t primary_key() const { return index; }
// };
// typedef eosio::multi_index<"tmonsters"_n, tmonsters> new_user_monsters;

//ACTION movedata(uint32_t _start_count);

#pragma endregion

TABLE dbbuff
{
    uint64_t id;
    uint32_t option_check;
    uint32_t buff_debuff_check;
    uint32_t target;
    uint32_t overlapping_check;
    uint32_t effect_type;
    uint32_t effect_stat;
    uint32_t value;
    uint32_t turn_count;
    uint64_t primary_key() const {return id;}
};
typedef eosio::multi_index<"dbbuff"_n, dbbuff> buff_db;