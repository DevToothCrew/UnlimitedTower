struct skill {
    std::string name;
    uint64_t level;
    uint64_t dmg;
};

//@abi table character i64
struct character {
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

typedef eosio::multi_index<N(character), character> char_table;