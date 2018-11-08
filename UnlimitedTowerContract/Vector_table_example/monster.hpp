//@abi table monster i64
struct monster {
    account_name  name;
    uint64_t job;
    uint64_t str;
    uint64_t dex;
    uint64_t intelligence;
    std::vector<skill> skills;

    account_name primary_key()const {
        return name;
    }
};

typedef eosio::multi_index<N(monster), monster> mon_table;