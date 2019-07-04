const fnGetTable = require('./fnGetEosTable');
const mongoQuery = require('./fnQuery');

const mongoose = require('mongoose');

const CUserAuth = mongoose.model('UserAuth');
const CUserServant = mongoose.model('UserServant');
const CUserMonster = mongoose.model('UserMonster');
const CUserEquipments = mongoose.model('UserEquipments');
const CUserItem = mongoose.model('UserItem');
const CUserAccounts = mongoose.model('UserAccounts');
const CUserPartylist = mongoose.model('UserPartylist');
const CUserEosioAccounts = mongoose.model('UserEosioAccounts');
const CUserDaily = mongoose.model('UserDaily');

module.exports = {
    /**
     * @method activeturn
     * @param {eosio::name _user, uint32_t _turn, std::string _seed} data
     */
    async activeturn(data) {
        return new Promise((resolve, reject) => {
            Promise.all([fnGetTable.getTuserauth(true, data._user, 1), fnGetTable.getTservant(true, data._user), fnGetTable.getTmonster(true, data._user), 
                fnGetTable.getTequipments(true, data._user), fnGetTable.getTitem(true, data._user), fnGetTable.getAccounts(true, data._user)])
            .then(async docs => {

                let userAuthQuery = { 'userName' : data._user, 'tuserauth' : docs[0].rows };
                let servantQuery = { 'userName' : data._user, 'tservant' : docs[1] };
                let monsterQuery = { 'userName' : data._user, 'tmonster' : docs[2] };
                let equipmentsQuery = { 'userName' : data._user, 'tequipments' : docs[3] };
                let itemQuery = { 'userName' : data._user, 'titem' : docs[4] };
                let accountsQuery = { 'userName' : data._user, 'accounts' : docs[5] };

                Promise.all([mongoQuery.dbSave(CUserAuth, userAuthQuery), mongoQuery.dbSave(CUserServant, servantQuery), mongoQuery.dbSave(CUserMonster, monsterQuery),
                    mongoQuery.dbSave(CUserEquipments, equipmentsQuery), mongoQuery.dbSave(CUserItem, itemQuery), mongoQuery.dbSave(CUserAccounts, accountsQuery)])
                .then(_docs => {
                    resolve(true);
                }).catch(e1 => {
                    reject(e1);
                });

            }).catch(e2 => {
                reject(e2);
            });
        });
    },

    /**
     * @method burn
     * @param {eosio::name _user, uint64_t _type, std::vector<uint64_t> &_list} data 
     */
    burn(data) {
        return new Promise((resolve, reject) => {
            Promise.all([fnGetTable.getTuserauth(true, data._user, 1), fnGetTable.getTservant(true, data._user), fnGetTable.getTmonster(true, data._user),
                fnGetTable.getTequipments(true, data._user), fnGetTable.getTitem(true, data._user), fnGetTable.getAccounts(true, data._user)])
            .then(docs => {

                let userAuthQuery = { 'userName' : data._user, 'tuserauth' : docs[0].rows };
                let servantQuery = { 'userName' : data._user, 'tservant' : docs[1] };
                let monsterQuery = { 'userName' : data._user, 'tmonster' : docs[2] };
                let equipmentsQuery = { 'userName' : data._user, 'tequipments' : docs[3] };
                let itemQuery = { 'userName' : data._user, 'titem' : docs[4] };
                let accountsQuery = { 'userName' : data._user, 'accounts' : docs[5] };

                Promise.all([mongoQuery.dbSave(CUserAuth, userAuthQuery), mongoQuery.dbSave(CUserServant, servantQuery), mongoQuery.dbSave(CUserMonster, monsterQuery),
                    mongoQuery.dbSave(CUserEquipments, equipmentsQuery), mongoQuery.dbSave(CUserItem, itemQuery), mongoQuery.dbSave(CUserAccounts, accountsQuery)])
                .then(_docs => {
                    resolve(true);
                }).catch(e1 => {
                    reject(e1);
                });

            }).catch(e => {
                reject(e);
            });
        });
    },

    /**
     * @method changetoken
     * @param {eosio::name _user, std::string _type, uint64_t _index} data 
     */
    changetoken(data) {
        return new Promise((resolve, reject) => {
            Promise.all([fnGetTable.getTuserauth(true, data._user, 1), fnGetTable.getTservant(true, data._user), fnGetTable.getTmonster(true, data._user), 
                fnGetTable.getTequipments(true, data._user), fnGetTable.getEosioAccounts(true, userAuthArr[i].user)])
            .then(docs => {

                let userAuthQuery = { 'userName' : data._user, 'tuserauth' : docs[0].rows };
                let servantQuery = { 'userName' : data._user, 'tservant' : docs[1] };
                let monsterQuery = { 'userName' : data._user, 'tmonster' : docs[2] };
                let equipmentsQuery = { 'userName' : data._user, 'tequipments' : docs[3] };
                let eosioAccountQuery = { 'userName' : data._user, 'eosioaccounts' : docs[4] };

                Promise.all([mongoQuery.dbSave(CUserAuth, userAuthQuery), mongoQuery.dbSave(CUserServant, servantQuery), mongoQuery.dbSave(CUserMonster, monsterQuery),
                    mongoQuery.dbSave(CUserEquipments, equipmentsQuery), mongoQuery.dbSave(CUserEosioAccounts, eosioAccountQuery)])
                .then(_docs => {
                    resolve(true);
                }).catch(e1 => {
                    reject(e1);
                });

            }).catch(e => {
                reject(e);
            });
        });
    },

    /**
     * @method create
     * @param {eosio::name issuer, asset maximum_supply} data 
     */
    create(data) {
        return new Promise((resolve, reject) => {
            // Promise.all([]).then(docs => {

            // }).catch(e => {
            //     reject(e);
            // });
            resolve(true);
        });
    },

    /**
     * @method dailycheck
     * @param {name _user, string _seed} data 
     */
    dailycheck(data) {
        return new Promise((resolve, reject) => {
            Promise.all([fnGetTable.getTdaily(true, data._user)]).then(docs => {

                let userDailyQuery = { 'userName' : data._user, 'tdaily' : docs[0] };

                Promise.all([mongoQuery.dbSave(CUserDaily, userDailyQuery)]).then(_docs => {
                    resolve(true);
                }).catch(e1 => {
                    reject(e1);
                });

            }).catch(e => {
                reject(e);
            });
        });
    },

    /**
     * @method eostransfer
     * @param {eosio::name sender, eosio::name receiver} data 
     */
    eostransfer(data) {
        return new Promise((resolve, reject) => {
            Promise.all([fnGetTable.getTuserauth(true, data._user, 1), fnGetTable.getTservant(true, data._user), fnGetTable.getTmonster(true, data._user), 
                fnGetTable.getTequipments(true, data._user), fnGetTable.getTitem(true, data._user), fnGetTable.getAccounts(true, data._user),
                fnGetTable.getTpartylist(true, data._user), fnGetTable.getEosioAccounts(true, data._user), fnGetTable.getTdaily(true, data._user)])
            .then(async docs => {

                let userAuthQuery = { 'userName' : data._user, 'tuserauth' : docs[0].rows };
                let servantQuery = { 'userName' : data._user, 'tservant' : docs[1] };
                let monsterQuery = { 'userName' : data._user, 'tmonster' : docs[2] };
                let equipmentsQuery = { 'userName' : data._user, 'tequipments' : docs[3] };
                let itemQuery = { 'userName' : data._user, 'titem' : docs[4] };
                let accountsQuery = { 'userName' : data._user, 'accounts' : docs[5] };
                let partyQuery = { 'userName' : data._user, 'tpartylist' : docs[6] };
                let eosioAccountQuery = { 'userName' : data._user, 'eosioaccounts' : docs[7] };
                let userDailyQuery = { 'userName' : data._user, 'tdaily' : docs[8] };

                Promise.all([mongoQuery.dbSave(CUserAuth, userAuthQuery), mongoQuery.dbSave(CUserServant, servantQuery), mongoQuery.dbSave(CUserMonster, monsterQuery),
                    mongoQuery.dbSave(CUserEquipments, equipmentsQuery), mongoQuery.dbSave(CUserItem, itemQuery), mongoQuery.dbSave(CUserAccounts, accountsQuery),
                    mongoQuery.dbSave(CUserPartylist, partyQuery), mongoQuery.dbSave(CUserEosioAccounts, eosioAccountQuery), mongoQuery.dbSave(CUserDaily, userDailyQuery)])
                .then(_docs => {
                    resolve(true);
                }).catch(e1 => {
                    reject(e1);
                });

            }).catch(e2 => {
                reject(e2);
            });
        });
    },

    /**
     * @method equip
     * @param {eosio::name _user, uint32_t _servant_index, uint32_t _item_index} data 
     */
    equip(data) {
        return new Promise((resolve, reject) => {
            Promise.all([fnGetTable.getTuserauth(true, data._user, 1), fnGetTable.getTservant(true, data._user), fnGetTable.getTequipments(true, data._user)]).then(docs => {

                let userAuthQuery = { 'userName' : data._user, 'tuserauth' : docs[0].rows };
                let servantQuery = { 'userName' : data._user, 'tservant' : docs[1] };
                let equipmentsQuery = { 'userName' : data._user, 'tequipments' : docs[2] };

                Promise.all([mongoQuery.dbSave(CUserAuth, userAuthQuery), mongoQuery.dbSave(CUserServant, servantQuery), 
                    mongoQuery.dbSave(CUserEquipments, equipmentsQuery)])
                .then(_docs => {
                    resolve(true);
                }).catch(e1 => {
                    reject(e1);
                });

            }).catch(e => {
                reject(e);
            });
        });
    },

    /** 
     * @method equipmentup
     * @param {eosio::name _user, uint32_t _equipment, const std::vector<uint64_t> &_get_item_list} data 
     */
    equipmentup(data) {
        return new Promise((resolve, reject) => {
            Promise.all([fnGetTable.getTuserauth(true, data._user, 1), fnGetTable.getTequipments(true, data._user), fnGetTable.getTitem(true, data._user), 
                fnGetTable.getAccounts(true, data._user)])
            .then(docs => {

                let userAuthQuery = { 'userName' : data._user, 'tuserauth' : docs[0].rows };
                let equipmentsQuery = { 'userName' : data._user, 'tequipments' : docs[1] };
                let itemQuery = { 'userName' : data._user, 'titem' : docs[2] };
                let accountsQuery = { 'userName' : data._user, 'accounts' : docs[3] };

                Promise.all([mongoQuery.dbSave(CUserAuth, userAuthQuery), mongoQuery.dbSave(CUserEquipments, equipmentsQuery), mongoQuery.dbSave(CUserItem, itemQuery), 
                    mongoQuery.dbSave(CUserAccounts, accountsQuery) ])
                .then(_docs => {
                    resolve(true);
                }).catch(e1 => {
                    reject(e1);
                });

            }).catch(e => {
                reject(e);
            });
        });
    },

    /**
     * @method goldgacha
     * @param {eosio::name _user, string _memo} data 
     */
    goldgacha(data) {
        return new Promise((resolve, reject) => {
            Promise.all([fnGetTable.getTuserauth(true, data._user, 1), fnGetTable.getTmonster(true, data._user), fnGetTable.getTequipments(true, data._user), 
                fnGetTable.getTitem(true, data._user), fnGetTable.getAccounts(true, data._user)])
            .then(docs => {

                let userAuthQuery = { 'userName' : data._user, 'tuserauth' : docs[0].rows };
                let monsterQuery = { 'userName' : data._user, 'tmonster' : docs[1] };
                let equipmentsQuery = { 'userName' : data._user, 'tequipments' : docs[2] };
                let itemQuery = { 'userName' : data._user, 'titem' : docs[3] };
                let accountsQuery = { 'userName' : data._user, 'accounts' : docs[4] };

                Promise.all([mongoQuery.dbSave(CUserAuth, userAuthQuery), mongoQuery.dbSave(CUserMonster, monsterQuery), mongoQuery.dbSave(CUserEquipments, equipmentsQuery),
                    mongoQuery.dbSave(CUserItem, itemQuery), mongoQuery.dbSave(CUserAccounts, accountsQuery) ])
                .then(_docs => {
                    resolve(true);
                }).catch(e1 => {
                    reject(e1);
                });

            }).catch(e => {
                reject(e);
            });
        });
    },

    /**
     * @method issue
     * @param {eosio::name to, asset quantity, string memo} data 
     */
    issue(data) {
        return new Promise((resolve, reject) => {
            // Promise.all([]).then(docs => {

            // }).catch(e => {
            //     reject(e);
            // });
            resolve(true);
        });
    },

    /**
     * @method itembuy
     * @param {eosio::name _user, uint32_t _item_id, uint32_t _count} data 
     */
    itembuy(data) {
        return new Promise((resolve, reject) => {
            Promise.all([fnGetTable.getTuserauth(true, data._user, 1), fnGetTable.getTitem(true, data._user), fnGetTable.getAccounts(true, data._user)]).then(docs => {

                let userAuthQuery = { 'userName' : data._user, 'tuserauth' : docs[0].rows };
                let itemQuery = { 'userName' : data._user, 'titem' : docs[1] };
                let accountsQuery = { 'userName' : data._user, 'accounts' : docs[2] };

                Promise.all([mongoQuery.dbSave(CUserAuth, userAuthQuery), mongoQuery.dbSave(CUserItem, itemQuery), mongoQuery.dbSave(CUserAccounts, accountsQuery) ])
                .then(_docs => {
                    resolve(true);
                }).catch(e1 => {
                    reject(e1);
                });

            }).catch(e => {
                reject(e);
            });
        });
    },

    /**
     * @method limitbreak
     * @param {eosio::name _user, uint32_t _object_type, uint32_t _index, uint32_t _item_id, uint32_t _break_count} data 
     */
    limitbreak(data) {
        return new Promise((resolve, reject) => {
            Promise.all([fnGetTable.getTuserauth(true, data._user, 1), fnGetTable.getTmonster(true, data._user), fnGetTable.getTitem(true, data._user), 
                fnGetTable.getAccounts(true, data._user)])
            .then(docs => {

                let userAuthQuery = { 'userName' : data._user, 'tuserauth' : docs[0].rows };
                let monsterQuery = { 'userName' : data._user, 'tmonster' : docs[1] };
                let itemQuery = { 'userName' : data._user, 'titem' : docs[2] };
                let accountsQuery = { 'userName' : data._user, 'accounts' : docs[3] };

                Promise.all([mongoQuery.dbSave(CUserAuth, userAuthQuery), mongoQuery.dbSave(CUserMonster, monsterQuery), 
                    mongoQuery.dbSave(CUserItem, itemQuery), mongoQuery.dbSave(CUserAccounts, accountsQuery)])
                .then(_docs => {
                    resolve(true);
                }).catch(e1 => {
                    reject(e1);
                });

            }).catch(e2 => {
                reject(e2);
            });
        });
    },

    /**
     * @method mailopen
     * @param {eosio::name _user, const std::vector<uint64_t> &_mail_index} data 
     */
    mailopen(data) {
        return new Promise((resolve, reject) => {
            Promise.all([fnGetTable.getTuserauth(true, data._user, 1), fnGetTable.getTservant(true, data._user), fnGetTable.getTmonster(true, data._user), 
                fnGetTable.getTequipments(true, data._user), fnGetTable.getTitem(true, data._user), fnGetTable.getAccounts(true, data._user)])
            .then(docs => {

                let userAuthQuery = { 'userName' : data._user, 'tuserauth' : docs[0].rows };
                let servantQuery = { 'userName' : data._user, 'tservant' : docs[1] };
                let monsterQuery = { 'userName' : data._user, 'tmonster' : docs[2] };
                let equipmentsQuery = { 'userName' : data._user, 'tequipments' : docs[3] };
                let itemQuery = { 'userName' : data._user, 'titem' : docs[4] };
                let accountsQuery = { 'userName' : data._user, 'accounts' : docs[5] };

                Promise.all([mongoQuery.dbSave(CUserAuth, userAuthQuery), mongoQuery.dbSave(CUserServant, servantQuery), mongoQuery.dbSave(CUserMonster, monsterQuery),
                    mongoQuery.dbSave(CUserEquipments, equipmentsQuery), mongoQuery.dbSave(CUserItem, itemQuery), mongoQuery.dbSave(CUserAccounts, accountsQuery)])
                .then(_docs => {
                    resolve(true);
                }).catch(e1 => {
                    reject(e1);
                });

            }).catch(e2 => {
                reject(e2);
            });
        });
    },

    /**
     * @method monsterup
     * @param {eosio::name _user, uint32_t _monster, uint32_t _monster2} data 
     */
    monsterup(data) {
        return new Promise((resolve, reject) => {
            Promise.all([fnGetTable.getTuserauth(true, data._user, 1), fnGetTable.getTmonster(true, data._user), fnGetTable.getAccounts(true, data._user)]).then(docs => {

                let userAuthQuery = { 'userName' : data._user, 'tuserauth' : docs[0].rows };
                let monsterQuery = { 'userName' : data._user, 'tmonster' : docs[1] };
                let accountsQuery = { 'userName' : data._user, 'accounts' : docs[2] };

                Promise.all([mongoQuery.dbSave(CUserAuth, userAuthQuery), mongoQuery.dbSave(CUserMonster, monsterQuery),
                    mongoQuery.dbSave(CUserAccounts, accountsQuery)])
                .then(_docs => {
                    resolve(true);
                }).catch(e1 => {
                    reject(e1);
                });

            }).catch(e2 => {
                reject(e2);
            });
        });
    },

    /**
     * @method nftmail
     * @param {eosio::name _user, std::string _type, uint64_t _token_index, uint64_t _icon_id} data 
     */
    nftmail(data) {
        return new Promise((resolve, reject) => {
            // Promise.all([]).then(docs => {

            // }).catch(e => {
            //     reject(e);
            // });
            resolve(true);
        });
    },

    /**
     * @method pvpstart
     * @param {eosio::name _from, eosio::name _to} data 
     */
    pvpstart(data) {
        return new Promise((resolve, reject) => {
            // Promise.all([]).then(docs => {

            // }).catch(e => {
            //     reject(e);
            // });
            resolve(true);
        });
    },

    /**
     * @method saveparty
     * @param {eosio::name _user, uint32_t _party_number, const std::vector<uint64_t> &_servant_list, const std::vector<uint64_t> &_monster_list} data 
     */
    saveparty(data) {
        return new Promise((resolve, reject) => {
            Promise.all([fnGetTable.getTuserauth(true, data._user, 1), fnGetTable.getTservant(true, data._user), fnGetTable.getTmonster(true, data._user), 
                fnGetTable.getTpartylist(true, data._user)])
            .then(docs => {

                let userAuthQuery = { 'userName' : data._user, 'tuserauth' : docs[0].rows };
                let servantQuery = { 'userName' : data._user, 'tservant' : docs[1] };
                let monsterQuery = { 'userName' : data._user, 'tmonster' : docs[2] };
                let partyQuery = { 'userName' : data._user, 'tpartylist' : docs[3] };

                Promise.all([mongoQuery.dbSave(CUserAuth, userAuthQuery), mongoQuery.dbSave(CUserServant, servantQuery), mongoQuery.dbSave(CUserMonster, monsterQuery),
                    mongoQuery.dbSave(CUserPartylist, partyQuery)])
                .then(_docs => {
                    resolve(true);
                }).catch(e1 => {
                    reject(e1);
                });

            }).catch(e2 => {
                reject(e2);
            });
        });
    },

    /**
     * @method setmaster
     * @param {eosio::name _master, uint32_t _type} data 
     */
    setmaster(data) {
        return new Promise((resolve, reject) => {
            // Promise.all([]).then(docs => {

            // }).catch(e => {
            //     reject(e);
            // });
            resolve(true);
        });
    },

    /**
     * @method setpause
     * @param {uint64_t _state} data 
     */
    setpause(data) {
        return new Promise((resolve, reject) => {
            // Promise.all([]).then(docs => {

            // }).catch(e => {
            //     reject(e);
            // });
            resolve(true);
        });
    },

    /**
     * @method stageexit
     * @param {eosio::name _user} data 
     */
    stageexit(data) {
        return new Promise((resolve, reject) => {
            Promise.all([fnGetTable.getTuserauth(true, data._user, 1), fnGetTable.getAccounts(true, data._user)]).then(docs => {

                let userAuthQuery = { 'userName' : data._user, 'tuserauth' : docs[0].rows };
                let accountsQuery = { 'userName' : data._user, 'accounts' : docs[1] };

                Promise.all([mongoQuery.dbSave(CUserAuth, userAuthQuery), mongoQuery.dbSave(CUserAccounts, accountsQuery)])
                .then(_docs => {
                    resolve(true);
                }).catch(e1 => {
                    reject(e1);
                });

            }).catch(e2 => {
                reject(e2);
            });
        });
    },

    /**
     * @method stagestart
     * @param {eosio::name _user, uint32_t _party_number, uint32_t _floor, uint32_t _type, uint32_t _difficult} data 
     */
    stagestart(data) {
        return new Promise((resolve, reject) => {
            // Promise.all([]).then(docs => {

            // }).catch(e => {
            //     reject(e);
            // });

            // userauth, titem

            resolve(true);
        });
    },

    /**
     * @method systemact
     * @param {std::string _function, eosio::name _user, std::string _type} data 
     */
    systemact(data) {
        return new Promise((resolve, reject) => {
            // Promise.all([]).then(docs => {

            // }).catch(e => {
            //     reject(e);
            // });
            resolve(true);
        });
    },

    /**
     * @method transfer
     * @param {name from, name to, asset quantity, string memo} data 
     */
    transfer(data) {
        return new Promise((resolve, reject) => {
            Promise.all([fnGetTable.getAccounts(true, data._user)]).then(docs => {
                let userAuthQuery = { 'userName' : data._user, 'tuserauth' : docs[0].rows };

                Promise.all([mongoQuery.dbSave(CUserAuth, userAuthQuery)]).then(_docs => {
                    resolve(true);
                }).catch(e1 => {
                    reject(e1);
                });

            }).catch(e => {
                reject(e);
            });
        });
    },

    /**
     * eosio transfer 발생에 대한 처리
     * 
     */
    eosioTransfer(user) {
        return new Promise((resolve, reject) => {

        });
    }
}