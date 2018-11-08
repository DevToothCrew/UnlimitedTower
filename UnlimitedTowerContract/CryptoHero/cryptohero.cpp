#include "cryptohero.hpp"

namespace eosio
{
#define MAX_DATA_NUM (32)
#define CHECK_VALID_DATA(data) eosio_assert(data < MAX_DATA_NUM, "invalid data domain")
    
    void cryptohero::createhero(const account_name user)
    {
        print("Start createhero... ID : ", name{user});

        cryptohero::AppearanceComponent appear = {rand32(), rand32(), rand32(), rand32()};
        cryptohero::ParameterComponent param = {rand32(), rand32(), rand32()};
        cryptohero::SkillComponent skills = {rand32(), rand32(), rand32(), rand32()};
        
        cryptohero::t_hero heros(_self,_self);

        auto iter = heros.find(user);
        heros.emplace(_self, [&](auto &newHero) {
            newHero.to = user;
            newHero.exp = 0;
            newHero.job = rand32();
            newHero.appear = appear;
            newHero.param = param;
            newHero.skills = skills;

            print("\n\nComplete createhero... \n", newHero.toString().c_str(), "\n\n");
        });
        // if(iter == heros.end())
        // {
        //     heros.emplace(_self,[&](auto& newHero)
        //     {
        //         newHero.to = user;
        //         newHero.exp = 0;
        //         newHero.job = rand32();
        //         newHero.appear = appear;
        //         newHero.param = param;
        //         newHero.skills = skills;
                
        //         print("\n\nComplete createhero... \n", newHero.toString().c_str(), "\n\n");
        //     });
        // }
        // else
        // {
        //     uint64_t getuser;
        //     getuser = iter->to;
        //     print("\n\nData already exist : ", name{getuser}, "\n\n");
        // }
    }

    void cryptohero::addexp(const account_name user)
    {
        const uint64_t MAX_EXP = 999999;
        const uint32_t MAX_GAIN_EXP = 1000;
        
        uint64_t gainExp = rand() % MAX_GAIN_EXP;

        cryptohero::t_hero heros(_self,_self);
        auto iter = heros.find(user);
        if(iter != heros.end())
        {
            heros.modify(iter, _self, [&](auto& setHero)
            {
                if(setHero.exp == MAX_EXP)
                {
                    print("\n\nInvalid Request, Already Max Exp\n\n");
                }
                else if(setHero.exp + gainExp >= MAX_EXP)
                {
                    setHero.exp = MAX_EXP;
                }
                else
                {
                    setHero.exp += gainExp;
                }

                print("\n\nHeroExp... ", setHero.exp, "\n\n");
            });
        }
    }

    void cryptohero::setjob(const account_name user, const uint32_t _job)
    {
        CHECK_VALID_DATA(_job);
        
        cryptohero::t_hero heros(_self,_self);
        auto iter = heros.find(user);
        if(iter != heros.end())
        {
            heros.modify(iter, _self, [&](auto& setHero)
            {
                setHero.job = _job;

                print("\n\nSetJob... ", setHero.job, "\n\n");
            });
        }
    }
    
    void cryptohero::setappearance(
                        const account_name user,
                        const uint32_t _hair,
                        const uint32_t _face,
                        const uint32_t _upperBody,
                        const uint32_t _underBody)
    {
        CHECK_VALID_DATA(_hair);
        CHECK_VALID_DATA(_face);
        CHECK_VALID_DATA(_upperBody);
        CHECK_VALID_DATA(_underBody);
    
        cryptohero::t_hero heros(_self,_self);
        auto iter = heros.find(user);
        if(iter != heros.end())
        {
            cryptohero::AppearanceComponent appear = {_hair, _face, _upperBody, _underBody};
            heros.modify(iter, _self, [&](auto& setHero)
            {
                setHero.appear = appear;
                
                print("\n\nSetAppearance... ", setHero.appear.toString().c_str(), "\n\n");
            });
        }
    }
    
    void cryptohero::setparam(
                              const account_name user,
                              const uint32_t _str,
                              const uint32_t _dex,
                              const uint32_t _int)
    {
        CHECK_VALID_DATA(_str);
        CHECK_VALID_DATA(_dex);
        CHECK_VALID_DATA(_int);
    
        cryptohero::t_hero heros(_self,_self);
        auto iter = heros.find(user);
        if(iter != heros.end())
        {
            cryptohero::ParameterComponent param = {_str, _dex, _int};
            heros.modify(iter, _self, [&](auto& hero)
            {
                hero.param = param;
                
                print("\n\nSetParam... ", hero.param.toString().c_str(), "\n\n");
            });
        }
    }
    
    void cryptohero::setskill(
                              const account_name user,
                              const uint32_t _skill_1,
                              const uint32_t _skill_2,
                              const uint32_t _skill_3)
    {
        CHECK_VALID_DATA(_skill_1);
        CHECK_VALID_DATA(_skill_1);
        CHECK_VALID_DATA(_skill_1);
        
        cryptohero::t_hero heros(_self,_self);
        auto iter = heros.find(user);
        if(iter != heros.end())
        {
            cryptohero::SkillComponent skills = {_skill_1, _skill_2, _skill_3};
            heros.modify(iter, _self, [&](auto& setHero)
            {
                setHero.skills = skills;
                
                print("\n\nSetSkills... ", setHero.skills.toString().c_str(), "\n\n");
            });
        }
    }
    
EOSIO_ABI( cryptohero::cryptohero, (createhero)(setjob)(setappearance)(setparam)(setskill)(addexp) )
}
