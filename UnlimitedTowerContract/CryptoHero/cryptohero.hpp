#include <eosiolib/eosio.hpp>
#include <eosiolib/print.hpp>
#include <eosiolib/crypto.h>
#include <array>

namespace eosiosystem {
   class system_contract;
}

namespace eosio {

    class cryptohero : public contract
    {
    public:
        cryptohero(account_name self):contract(self), globals(self, self)
        {
            print("self : ", name{self}, " --- ");
        }

        //@abi action
        void createhero(const account_name user);
        //@abi action
        void setjob(const account_name user, const uint32_t _job);
        //@abi action
        void setappearance(
                           const account_name user,
                           const uint32_t _hair,
                           const uint32_t _face,
                           const uint32_t _upperBody,
                           const uint32_t _underBody);
        //@abi action
        void setparam(
                       const account_name user,
                       const uint32_t _str,
                       const uint32_t _dex,
                       const uint32_t _int);
        //@abi action
        void setskill(
                      const account_name user,
                      const uint32_t _skill_1,
                      const uint32_t _skill_2,
                      const uint32_t _skill_3);
        
        //@abi action
        void addexp(const account_name user);
        
        struct AppearanceComponent
        {
            uint32_t _hair;
            uint32_t _face;
            uint32_t _upperBody;
            uint32_t _underBody;

            std::string toString() const
            {
                std::string str;
                str.reserve(50);
                str += "Hair : ";
                str += std::to_string(_hair);
                str += "Face : ";
                str += std::to_string(_face);
                str += "upperBody : ";
                str += std::to_string(_upperBody);
                str += "underBody : ";
                str += std::to_string(_underBody);
                return str;
            }
        };

        struct ParameterComponent
        {
            uint32_t _strength;
            uint32_t _dexterity;
            uint32_t _intelligence;

            std::string toString() const
            {
                std::string str;
                str.reserve(50);
                str += "Str : ";
                str += std::to_string(_strength);
                str += ", Dex : ";
                str += std::to_string(_dexterity);
                str += ", Int : ";
                str += std::to_string(_intelligence);
                return str;
            }
        };

        struct Skill
        {
            uint32_t _skillId;
        };

        struct SkillComponent
        {
            uint32_t _skill_1;
            uint32_t _skill_2;
            uint32_t _skill_3;
            uint32_t _skill_4;

            std::string toString() const
            {
                std::string str;
                str.reserve(50);
                str += "Skill_1 : ";
                str += std::to_string(_skill_1);
                str += ", Skill_2 : ";
                str += std::to_string(_skill_2);
                str += ", Skill_3 : ";
                str += std::to_string(_skill_3);
                str += ", Skill_4 : ";
                str += std::to_string(_skill_4);
                return str;
            }
        };
        
    /*
        class hero
        {
            public:
                hero(
                        const uint32_t job,
                        const AppearanceComponent& appear,
                        const ParameterComponent& param,
                        const SkillComponent& skill)
                : _job(job)
                , _appear(appear)
                , _param(param)
                , _skill(skill)
                {
                }

                std::string toString() const
                {
                    std::string str;
                    str.reserve(200);
                    str += "[HeroInfo - job : ";
                    str += std::to_string(_job);
                    str += " / ";
                    str += _appear.toString();
                    str += " / ";
                    str += _param.toString();
                    str += " / ";
                    str += _skill.toString();
                    str += " ]";
                    return str;
                }

                // todo@cityofstars - change to private
            public:
                uint32_t _job;
                AppearanceComponent _appear;
                ParameterComponent _param;
                SkillComponent _skill;

                // todo@cityofstars - Serialize?
        };
    */
        //@abi table hero i64
        class hero
        {
        public:
            account_name to;
            uint32_t exp;
            uint32_t job;
            AppearanceComponent appear;
            ParameterComponent param;
            SkillComponent skills;
            uint64_t primary_key() const {return to;}

            std::string toString() const
            {
                std::string str;
                str.reserve(200);
                str += "[HeroInfo - job : ";
                str += std::to_string(job);
                str += " / ";
                str += appear.toString();
                str += " / ";
                str += param.toString();
                str += " / ";
                str += skills.toString();
                str += " ]";
                return str;
            }

            EOSLIB_SERIALIZE(hero,(to)(exp)(job)(appear)(param)(skills))
        };

        typedef multi_index<N(hero), hero> t_hero;

        // random impliment
    private:
        struct globalvariable
        {
            int randCount = 0;
            globalvariable() = default;

            uint64_t primary_key()const { return 0; }   // global
        };

        typedef eosio::multi_index<N(globals), globalvariable> global_index;
        global_index     globals;

        int provideNonce()
        {
            auto it = globals.find(0);
            if (it == globals.end())
            {
                it = globals.emplace(_self, [&](auto& e){
                    e.randCount = 0;
                });
            }
            int ret = it->randCount;

            globals.modify(it, 0, [&](auto& e){
                ++e.randCount;
            });
            return ret;
        }

        uint64_t rand()
        {
            uint64_t source = current_time() + provideNonce();
            checksum256 result;
            sha256( (char *)&source, sizeof(source), &result);
            uint64_t* p = reinterpret_cast<uint64_t*>(&result.hash);
            // print(source, ", ", *p);
            return *p;
        }

        // random number 0-to-v
        uint32_t rand32()
        {
            return rand()%32;
        }
    };
}
