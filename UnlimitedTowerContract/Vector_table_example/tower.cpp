#include <vector>
#include <string>
#include <eosiolib/eosio.hpp>
#include <eosiolib/asset.hpp>
#include <eosiolib/transaction.hpp>
#include <eosiolib/crypto.h>
#include <eosiolib/print.hpp>

#include <character.hpp>

using namespace eosio;

class tower : public eosio::contract {
    private:
        char_table character;

    public:
        tower(account_name self) : contract(self), character(_self, _self){

        }

        //@abi action
        void insert(std::string skill_name, uint64_t skill_dmg)
        {
            std::vector<skill> new_skills;
            skill new_skill;
            new_skill.name = skill_name;
            new_skill.level = 1;
            new_skill.dmg = skill_dmg;
            new_skills.push_back(new_skill);

            
            auto iter = character.find(_self);
            if(iter == character.end()){
                character.emplace(_self, [&](auto &target){
                    target.name = _self;
                    target.job = 189;
                    target.str = 100;
                    target.dex = 80;
                    target.intelligence = 90;
                    target.skills.insert(target.skills.end(), new_skills.begin(),new_skills.end());
                });
            }
            else{
                character.modify(iter, _self, [&](auto &target){
                    target.skills.insert(target.skills.end(), new_skills.begin(),new_skills.end());
                });
            }
        }

        //@abi action
        void output()
        {
            const auto& iter = character.get(_self);
            print("str : ", iter.str);
            print("\ndex : ", iter.dex);
            print("\nint : ", iter.intelligence);
            print("\nskills : ", iter.skills[0].name);
        }
};

EOSIO_ABI(tower, (insert)(output))