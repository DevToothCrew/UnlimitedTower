Eos = require('eosjs')
config = require('../config/')

var async = require('async');

function User() { }

/**
 * Get Login Data
 * 
 * @param req
 * @param res
 */
User.getLoginInfo = function (req, res) {
    var func = 'getLoginInfo';

    var user = req.body.user;
    eos = Eos(config.eos_jungle);

    async.parallel([
        function (next) {
            // Get Servant Info
            eos.getTableRows({
                code: config.contract.dev,
                scope: user,
                table: 'tservant',
                limit: 1000,
                json: true
            }, function (err, servant) {
                if (err) {
                    next("Fail:Get Servant Table:" + func);
                }
                else {
                    next(null, servant);
                }
            });
        },
        function (next) {
            // Get Monster Info
            eos.getTableRows({
                code: config.contract.dev,
                scope: user,
                table: 'tmonster',
                limit: 1000,
                json: true
            }, function (err, monster) {
                if (err) {
                    next("Fail:Get Monster Table:" + func);
                }
                else {
                    next(null, monster);
                }
            });
        },
        function (next) {
            // Get Equip Info
            eos.getTableRows({
                code: config.contract.dev,
                scope: user,
                table: 'tequipments',
                limit: 1000,
                json: true
            }, function (err, equip) {
                if (err) {
                    next("Fail:Get Equipment Table:" + func);
                }
                else {
                    next(null, equip);
                }
            });
        },
        function (next) {
            // Get Item Info
            eos.getTableRows({
                code: config.contract.dev,
                scope: user,
                table: 'titem',
                limit: 1000,
                json: true
            }, function (err, item) {
                if (err) {
                    next("Fail:Get Item Table:" + func);
                }
                else {
                    next(null, item);
                }
            });
        },
        function (next) {
            // Get Token Info
            eos.getTableRows({
                code: config.contract.dev,
                scope: user,
                table: 'accounts',
                json: true
            }, function (err, token) {
                if (err) {
                    next("Fail:Get Account Table:" + func);
                }
                else {
                    next(null, token);
                }
            });
        },
        function (next) {
            // Get Party Info
            eos.getTableRows({
                code: config.contract.dev,
                scope: user,
                table: 'tpartylist',
                json: true
            }, function (err, party) {
                if (err) {
                    next("Fail:Get Party Table:" + func);
                }
                else {
                    next(null, party);
                }
            });
        },
        function (next) {
            // Get User Info
            eos.getTableRows({
                code: config.contract.dev,
                scope: config.contract.dev,
                lower_bound: user,
                limit: 1,
                table: 'tuserauth',
                json: true
            }, function (err, user) {
                if (err) {
                    next("Fail:Get Auth Table:" + func);
                }
                else {
                    next(null, user);
                }
            });
        },
        function (next) {
            // Get EOS Info
            eos.getTableRows({
                code: 'eosio.token',
                scope: user,
                limit: 1,
                table: 'accounts',
                json: true
            }, function (err, uEos) {
                if (err) {
                    next("Fail:Get Token Table:" + func);
                }
                else {
                    next(null, uEos);
                }
            });
        }
    ],
        function (err, tableData) {
            if (err) {
                console.log(err);
                res.status(200).send(err);
            }
            else {
                var user_data = {};
                if (tableData[6].rows.length == 0) {
                    user_data.signup = null;
                    console.log(config.color.yellow, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                    res.status(200).send(user_data);
                }
                else {
                    if (tableData[6].rows[0].user != user) {
                        user_data.signup = null;
                        console.log(config.color.yellow, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                        res.status(200).send(user_data);
                    }
                    else {
                        user_data.servant_list = tableData[0].rows;
                        user_data.monster_list = tableData[1].rows;
                        user_data.equipment_list = tableData[2].rows;
                        user_data.item_list = tableData[3].rows;
                        user_data.party_info = tableData[5].rows[0];

                        // 유저 정보 데이터 가공
                        var inventory_info = {
                            servant_inventory : tableData[6].rows[0].servant_inventory,
                            monster_inventory : tableData[6].rows[0].monster_inventory,
                            equipment_inventory : tableData[6].rows[0].equipment_inventory,
                            item_inventory : tableData[6].rows[0].item_inventory
                        };

                        user_data.user_data = {
                            user : tableData[6].rows[0].user,
                            state : tableData[6].rows[0].state,
                            exp : tableData[6].rows[0].exp,
                            rank : tableData[6].rows[0].rank,
                            inventory_info : inventory_info
                        };

                        var uEos = tableData[7].rows[0].balance.split(" ");
                        uEos = uEos[0].split(".");
                        if (uEos[0] == '0' && uEos[1] == '0000') {
                            user_data.eos = '0';
                        }
                        else {
                            user_data.eos = uEos[0] + uEos[1];
                        }

                        if (tableData[4].rows.length == 0) {
                            user_data.utg = '0';
                        }
                        else {
                            var token = tableData[4].rows[0].balance.split(" ");
                            token = token[0].split(".");
                            user_data.utg = token[0] + token[1];
                        }
                        
                        user_data.signup = true;

                        console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                        res.status(200).send(user_data);
                    }
                }
            }
        });
}

/**
 * Get Pre-Sale Login Data
 * 
 * @param req
 * @param res
 */
User.getPreLogindata = function (req, res) {
    var func = 'getPreLogindata';

    var user = req.body.user;
    eos = Eos(config.eos);

    async.parallel([
        function (next) {
            // Get Servant Info
            eos.getTableRows({
                code: config.contract.main,
                scope: user,
                table: 'preservant',
                limit: 100,
                json: true
            }, function (err, servant) {
                if (err) {
                    next(err);
                }
                else {
                    next(null, servant);
                }
            });
        },
        function (next) {
            // Get Monster Info
            eos.getTableRows({
                code: config.contract.main,
                scope: user,
                table: 'premonster',
                limit: 100,
                json: true
            }, function (err, monster) {
                if (err) {
                    next(err);
                }
                else {
                    next(null, monster);
                }
            });
        },
        function (next) {
            // Get Item Info
            eos.getTableRows({
                code: config.contract.main,
                scope: user,
                table: 'preitem',
                limit: 100,
                json: true
            }, function (err, item) {
                if (err) {
                    next(err);
                }
                else {
                    next(null, item);
                }
            });
        },
        function (next) {
            // Get Token Info
            eos.getTableRows({
                code: config.contract.main,
                scope: user,
                table: 'accounts',
                json: true
            }, function (err, token) {
                if (err) {
                    next(err);
                }
                else {
                    next(null, token);
                }
            });
        }
    ],
        function (err, tableData) {
            if (err) {
                console.log("Get Table Error");
                res.status(200).send("Get Pre Login Data Error.");
            }
            else {
                var user_data = {};
                if (tableData[1].rows.length == 0) {
                    user_data.signup = null;
                    console.log(config.color.yellow, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                    res.status(200).send(user_data);
                }
                else {
                    user_data.servant_list = tableData[0].rows;
                    user_data.monster_list = tableData[1].rows;
                    user_data.item_list = tableData[2].rows;
                    user_data.token = tableData[3].rows;
                    user_data.signup = true;

                    console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                    res.status(200).send(user_data);
                }
            }
        });
}

// MainNet FullNode 적용

/**
 * Pre Sign up
 * 
 * @param req
 * @param res
 */
User.mainPreSignup = function (req, res) {
    var func = 'mainPreSignup';

    var user = req.body.user;

    const MongoOplog = require('mongo-oplog');
    const oplog = MongoOplog('mongodb://devTooth:U0S91ydQ0uYPSaOqHz3T@eos-sg-00-00-mongo-01.itam.games:27017/local?replicaSet=eos-01&authSource=admin',
        { ns: 'EOS.action_traces', reconnectTries: 60, reconnectInterval: 1000 });

    oplog.tail();

    var count = 0;
    var timer = setInterval(function () {
        if (count == 60) {
            clearInterval(timer);
            oplog.stop();
            res.status(200).send("ERR:Time Out");
        }
        else {
            count++;
        }
    }, 1000);

    oplog.on('insert', doc => {
        const actionInfo = doc.o;
        const action = actionInfo.act;

        if (action.account == 'untowermain1' && action.name == 'resultpre' && action.data._from == 'untowermain1' && action.data._to == user) {
            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            var temp = action.data._result.split(':');
            var data = {
                result: temp[0],
                index: temp[1],
                id: temp[2],
                grade: temp[3],
                b_str: temp[4],
                b_dex: temp[5],
                b_int: temp[6],
                token: temp[7]
            }
            res.status(200).send(data);
            oplog.stop();
            clearInterval(timer);
        }
    });



    oplog.on('error', error => {
        console.log(config.color.red, 'user : ', user, ', func : ', func, ' err : ', error, ' time : ', new Date(new Date().toUTCString()));
    });
}

/**
 * Sign Up 
 * 
 * @param req
 * @param res
 */
User.signUp = function (req, res) {
    var func = 'signUp';

    var user = req.body.user;
    eos = Eos(config.eos_jungle);

    var count = 0;
    var timer = setInterval(function () {
        eos.getTableRows({
            code: config.contract.dev,
            scope: config.contract.dev,
            table: 'tuserauth',
            lower_bound: user,
            limit: 1,
            json: true
        }, function (err, newTable) {
            if (err) {
                console.error("Fail:Get Table:" + func);
                res.status(200).send("Fail:Get Table:" + func);
            }
            else {
                if (newTable.rows.length != 0) {
                    if (newTable.rows[0].user == user) {
                        clearInterval(timer);
                        async.parallel([
                            function (next) {
                                // Get Servant Info
                                eos.getTableRows({
                                    code: config.contract.dev,
                                    scope: user,
                                    table: 'tservant',
                                    limit: 1000,
                                    json: true
                                }, function (err, servant) {
                                    if (err) {
                                        next("Fail:Get Servant Table:" + func);
                                    }
                                    else {
                                        next(null, servant);
                                    }
                                });
                            },
                            function (next) {
                                // Get Monster Info
                                eos.getTableRows({
                                    code: config.contract.dev,
                                    scope: user,
                                    table: 'tmonster',
                                    limit: 1000,
                                    json: true
                                }, function (err, monster) {
                                    if (err) {
                                        next("Fail:Get Monster Table:" + func);
                                    }
                                    else {
                                        next(null, monster);
                                    }
                                });
                            },
                            function (next) {
                                // Get Equip Info
                                eos.getTableRows({
                                    code: config.contract.dev,
                                    scope: user,
                                    table: 'tequipments',
                                    limit: 1000,
                                    json: true
                                }, function (err, equip) {
                                    if (err) {
                                        next("Fail:Get Equipment Table:" + func);
                                    }
                                    else {
                                        next(null, equip);
                                    }
                                });
                            },
                            function (next) {
                                // Get Item Info
                                eos.getTableRows({
                                    code: config.contract.dev,
                                    scope: user,
                                    table: 'titem',
                                    limit: 1000,
                                    json: true
                                }, function (err, item) {
                                    if (err) {
                                        next("Fail:Get Item Table:" + func);
                                    }
                                    else {
                                        next(null, item);
                                    }
                                });
                            },
                            function (next) {
                                // Get Token Info
                                eos.getTableRows({
                                    code: config.contract.dev,
                                    scope: user,
                                    table: 'accounts',
                                    json: true
                                }, function (err, token) {
                                    if (err) {
                                        next("Fail:Get Account Table:" + func);
                                    }
                                    else {
                                        next(null, token);
                                    }
                                });
                            },
                            function (next) {
                                // Get Party Info
                                eos.getTableRows({
                                    code: config.contract.dev,
                                    scope: user,
                                    table: 'tpartylist',
                                    json: true
                                }, function (err, party) {
                                    if (err) {
                                        next("Fail:Get Party Table:" + func);
                                    }
                                    else {
                                        next(null, party);
                                    }
                                });
                            },
                            function (next) {
                                // Get User Info
                                eos.getTableRows({
                                    code: config.contract.dev,
                                    scope: config.contract.dev,
                                    lower_bound: user,
                                    limit: 1,
                                    table: 'tuserauth',
                                    json: true
                                }, function (err, user) {
                                    if (err) {
                                        next("Fail:Get Auth Table:" + func);
                                    }
                                    else {
                                        next(null, user);
                                    }
                                });
                            },
                            function (next) {
                                // Get EOS Info
                                eos.getTableRows({
                                    code: 'eosio.token',
                                    scope: user,
                                    limit: 1,
                                    table: 'accounts',
                                    json: true
                                }, function (err, uEos) {
                                    if (err) {
                                        next("Fail:Get Token Table:" + func);
                                    }
                                    else {
                                        next(null, uEos);
                                    }
                                });
                            }
                        ],
                        function (err, tableData) {
                            if (err) {
                                clearInterval(timer);
                                console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                                res.status(200).send(err);                                
                            }
                            else {
                                var user_data = {}
                                user_data.servant_list = tableData[0].rows;
                                user_data.monster_list = tableData[1].rows;
                                user_data.equipment_list = tableData[2].rows;
                                user_data.item_list = tableData[3].rows;
                                user_data.party_info = tableData[5].rows[0];
        
                                // 유저 정보 데이터 가공
                                var inventory_info = {
                                    servant_inventory : tableData[6].rows[0].servant_inventory,
                                    monster_inventory : tableData[6].rows[0].monster_inventory,
                                    equipment_inventory : tableData[6].rows[0].equipment_inventory,
                                    item_inventory : tableData[6].rows[0].item_inventory
                                };
        
                                user_data.user_data = {
                                    user : tableData[6].rows[0].user,
                                    state : tableData[6].rows[0].state,
                                    exp : tableData[6].rows[0].exp,
                                    rank : tableData[6].rows[0].rank,
                                    inventory_info : inventory_info
                                };
        
                                var uEos = tableData[7].rows[0].balance.split(" ");
                                uEos = uEos[0].split(".");
                                if (uEos[0] == '0' && uEos[1] == '0000') {
                                    user_data.eos = '0';
                                }
                                else {
                                    user_data.eos = uEos[0] + uEos[1];
                                }
                                
                                if (tableData[4].rows.length == 0) {
                                    user_data.utg = '0';
                                }
                                else {
                                    var token = tableData[4].rows[0].balance.split(" ");
                                    token = token[0].split(".");
                                    user_data.utg = token[0] + token[1];
                                }

                                user_data.signup = true;
        
                                console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                                res.status(200).send(user_data);
                            }
                        });
                    }
                    else {
                        count += 1;
                        if (count >= 10) {
                            clearInterval(timer);
                            console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                            res.status(200).send("Fail:Time Out:" + func);
                        }
                    }
                }
                else {
                    count += 1;
                    if (count >= 10) {
                        clearInterval(timer);
                        console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                        res.status(200).send("Fail:Time Out:" + func);
                    }
                }
            }
        })
    }, 1000);
}

/**
 * Get User Data
 * 
 * @param req
 * @param res
 */
User.getUserData = function (req, res) {
    var func = 'getUserData';

    var user = req.body.user;

    eos = Eos(config.eos_jungle);

    async.parallel([
        function (next) {
            // Get Chat Info
            eos.getTableRows({
                code: config.contract.dev,
                scope: config.contract.dev,
                table: 'tchat',
                json: true
            }, function (err, chat) {
                if (err) {
                    console.log("Fail:Get Chat Table:" + func);
                    next("Fail:Get Chat Table:" + func);
                }
                else {
                    next(null, chat);
                }
            });
        },
        function (next) {
            // Get Mail Info
            eos.getTableRows({
                code: config.contract.dev,
                scope: user,
                table: 'tmaildb',
                json: true
            }, function (err, mail) {
                if (err) {
                    console.log("Fail:Get Mail Table:" + func);
                    next("Fail:Get Mail Table:" + func);
                }
                else {
                    next(null, mail);
                }
            });
        },
        function (next) {
            // Get Resource Info
            eos.getAccount(user, function (err, resource) {
                if (err) {
                    console.log("Fail:Get REsource Table:" + func);
                    next("Fail:Get Resource Table:" + func);
                }
                else {
                    next(null, resource);
                }
            });
        },
        function (next) {
            // Get EOS Info
            eos.getTableRows({
                code: 'eosio.token',
                scope: user,
                limit: 1,
                table: 'accounts',
                json: true
            }, function (err, uEos) {
                if (err) {
                    console.log("Fail:Get EOS Table:" + func);
                    next("Fail:Get EOS Table:" + func);
                }
                else {
                    next(null, uEos);
                }
            });
        },
        function (next) {
            // Get Token Info
            eos.getTableRows({
                code: config.contract.dev,
                scope: user,
                table: 'accounts',
                json: true
            }, function (err, token) {
                if (err) {
                    console.log("Fail:Get Account Table:" + func);
                    next("Fail:Get Account Table:" + func);
                }
                else {
                    next(null, token);
                }
            });
        }
    ],
        function (err, tableData) {
            if (err) {
                console.log(err);
                res.status(200).send(err);
            }
            else {
                var resource = {
                    ram_quota: tableData[2].ram_quota,
                    net_weight: tableData[2].net_weight,
                    cpu_weight: tableData[2].cpu_weight,
                    net_limit: tableData[2].net_limit,
                    cpu_limit: tableData[2].cpu_limit,
                    ram_usage: tableData[2].ram_usage 
                }
                var chat_string = "";

                if(tableData[0].rows.length == 0){
                    chat_string = "";
                }
                else{
                    chat_string = tableData[0].rows[0].text;
                }

                var user_data = {
                    chat_string : chat_string,
                    mail_count : tableData[1].rows.length,
                    resource_data : resource
                };

                var uEos = tableData[3].rows[0].balance.split(" ");
                        uEos = uEos[0].split(".");
                        if (uEos[0] == '0' && uEos[1] == '0000') {
                            user_data.eos = '0';
                        }
                        else {
                            user_data.eos = uEos[0] + uEos[1];
                        }

                if (tableData[4].rows.length == 0) {
                    user_data.utg = '0';
                }
                else {
                    var token = tableData[4].rows[0].balance.split(" ");
                    token = token[0].split(".");
                    user_data.utg = token[0] + token[1];
                }
                
                console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                res.status(200).send(user_data);
            }
        }
    );
}

module.exports = User;