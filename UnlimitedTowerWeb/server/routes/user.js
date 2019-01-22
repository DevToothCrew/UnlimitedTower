Eos = require('eosjs')
config = require('../config/')

var async = require('async');

function User() {}

/**
 * Get Login Data
 * 
 * @param req
 * @param res
 */
User.getLoginInfo = function(req, res){
    var func = 'getLoginInfo';
    
    var user = req.body.user;
    eos = Eos(config.eos);

    async.parallel([
        function(next){
            // Get User Info
            eos.getTableRows({
                code : config.contract.main,
                scope : config.contract.main,
                table : 'tuserauth',
                lower_bound : user,
                limit : 1,
                json : true
            }, function(err, userInfo){
                if(err){
                    next(err);
                }
                else{
                    next(null, userInfo);
                }
            });
        },
        function(next){
            // Get Servant Info
            eos.getTableRows({
                code : config.contract.main,
                scope : user,
                table : 'tservant',
                json : true
            }, function(err, servant){
                if(err){
                    next(err);
                }
                else{
                    next(null, servant);
                }
            });
        },
        function(next){
            // Get Monster Info
            eos.getTableRows({
                code : config.contract.main,
                scope : user,
                table : 'tmonster',
                json : true
            }, function(err, monster){
                if(err){
                    next(err);
                }
                else{
                    next(null, monster);
                }
            });
        },
        function(next){
            // Get Item Info
            eos.getTableRows({
                code : config.contract.main,
                scope : user,
                table : 'titem',
                json : true
            }, function(err, item){
                if(err){
                    next(err);
                }
                else{
                    next(null, item);
                }
            });
        }
        /*
        function(next){
            // Get Party Info
            eos.getTableRows({
                code : config.contract.main,
                scope : user,
                table : 'cparty',
                json : true
            }, function(err, party){
                if(err){
                    next(err);
                }
                else{
                    next(null, party);
                }
            });
        }
        */
    ],
    function(err, tableData){
        if(err){
            console.log("Get Table Error");
            res.status(200).send("Get Login Data Error.");
        }
        else{
            var user_data = {};
            if(tableData[0].rows.length == 0)
            {
                user_data.signup = null;
                console.log(config.color.yellow, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                res.status(200).send(user_data);
            }
            else{
                if(tableData[0].rows[0].user != user){
                    user_data.signup = null;
                    console.log(config.color.yellow, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                    res.status(200).send(user_data);
                }
                else{
                    user_data.userinfo = tableData[0].rows[0];
                    user_data.servant_list = tableData[1].rows;
                    user_data.monster_list = tableData[2].rows;
                    user_data.item_list = tableData[3].rows;
                    //user_data.party_list = tableData[4].rows;

                    console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                    res.status(200).send(user_data);
                }
            }
        }
    });
}

/**
 * Get User Info
 * 
 * @param req
 * @param res
 */
User.getUserInfo = function(req, res){
    var func = 'getUserInfo';

    var user = req.body.user;
    eos = Eos(config.eos);

    eos.getTableRows({
        code : config.contract.main,
        scope : config.contract.main,
        table : 'cuserauth',
        lower_bound : user,
        limit : 1,
        json : true
    }, function(err, result){
        if(err){
            console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', Date.now());
            res.status(200).send("Get User Info Error.");
        }
        else{
            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', Date.now());
            res.status(200).send(result.rows[0]);
        }
    });
}

/**
 * Get Servant Table
 * 
 * @param req
 * @param res
 */
User.getServantTable = function(req, res){

    var user = req.body.user;
    eos = Eos(config.eos);

    eos.getTableRows({
        code : config.contract.main,
        scope : user,
        table : 'cservant',
        json : true
    }, function(err, result){
        if(err){
            console.log("Get Servant Table Error");
            res.status(200).send("Get Servant Table Error.");
        }
        else{
            res.status(200).send(result.rows);
        }
    });
}

/**
 * Get Item Table
 * 
 * @param req
 * @param res
 */
User.getItemTable = function(req, res){

    var user = req.body.user;
    eos = Eos(config.eos);

    eos.getTableRows({
        code : config.contract.main,
        scope : user,
        table : 'citem',
        json : true
    }, function(err, result){
        if(err){
            console.log("Get Item Table Error");
            res.status(200).send("Get Item Table Error.");
        }
        else{
            res.status(200).send(result.rows);
        }
    });
}

/**
 * Get Monster Table
 * 
 * @param req
 * @param res
 */
User.getMonsterTable = function(req, res){

    var user = req.body.user;
    eos = Eos(config.eos);

    eos.getTableRows({
        code : config.contract.main,
        scope : user,
        table : 'cmonster',
        json : true
    }, function(err, result){
        if(err){
            console.log("Get Monster Table Error");
            res.status(200).send("Get Monster Table Error.");
        }
        else{
            res.status(200).send(result.rows);
        }
    });
}

/**
 * Update Servant Table
 * 
 * @param req
 * @param res
 */
User.updateServantTable = function(req, res){

    var user = req.body.user;
    eos = Eos(config.eos);

    async.waterfall([
        // 변경 되기 이전 정보 가져옴
        function(callback){
            eos.getTableRows({
                code : config.contract.main,
                scope : config.contract.main,
                table : 'cservant',
                lower_bound : user,
                limit : 1,
                json : true
            }, function(err, oldTable){
                if(err){
                    console.error("Fail to get table.");
                }
                else{
                    callback(null, oldTable);
                }
            });
        },
        // 이전 테이블 정보과 현재 테이블 정보가 차이가 생길때까지 1초마다 새로운 테이블 불러옴
        function(oldTable, callback){
            var timer = setInterval(function(){
                eos.getTableRows({
                    code : config.contract.main,
                    scope : config.contract.main,
                    table : 'cservant',
                    lower_bound : user,
                    limit : 1,
                    json : true
                }, function(err, newTable){
                    console.log('New Table : ', newTable);
                    if(err){
                        console.error("Fail to get Table.");
                    }
                    else{
                        if(oldTable.rows[0].num != newTable.rows[0].num){
                            clearInterval(timer);
                            callback(null, newTable.rows[0]);
                        }
                    }
            })}, 1000);
        }
    ],
    function(err, result){
        if(err){
            res.status(200).send("Get Table Test fail.");
        }
        else{
            res.status(200).send(result);
        }
    });
}

/**
 * Sign Up 
 * 
 * @param req
 * @param res
 */
User.signUp = function(req, res){
    var func = 'signUp';

    var user = req.body.user;
    eos = Eos(config.eos);

    var count = 0;
    var timer = setInterval(function(){
        eos.getTableRows({
            code : config.contract.main,
            scope : config.contract.main,
            table : 'tuserauth',
            lower_bound : user,
            limit : 1,
            json : true
        }, function(err, newTable){
            if(err){
                console.error("Fail to get Table.");
            }
            else{
                if(newTable.rows.length != 0){
                    if(newTable.rows[0].user == user){
                        async.parallel([
                            function(next){
                                // Get Servant Info
                                eos.getTableRows({
                                    code : config.contract.main,
                                    scope : user,
                                    table : 'tservant',
                                    json : true
                                }, function(err, servant){
                                    if(err){
                                        next(err);
                                    }
                                    else{
                                        next(null, servant);
                                    }
                                });
                            },
                            function(next){
                                // Get Monster Info
                                eos.getTableRows({
                                    code : config.contract.main,
                                    scope : user,
                                    table : 'tmonster',
                                    json : true
                                }, function(err, monster){
                                    if(err){
                                        next(err);
                                    }
                                    else{
                                        next(null, monster);
                                    }
                                });
                            },
                            function(next){
                                // Get Item Info
                                eos.getTableRows({
                                    code : config.contract.main,
                                    scope : user,
                                    table : 'titem',
                                    json : true
                                }, function(err, item){
                                    if(err){
                                        next(err);
                                    }
                                    else{
                                        next(null, item);
                                    }
                                });
                            }
                            /*
                            function(next){
                                // Get Party Info
                                eos.getTableRows({
                                    code : config.contract.main,
                                    scope : user,
                                    table : 'cparty',
                                    json : true
                                }, function(err, party){
                                    if(err){
                                        next(err);
                                    }
                                    else{
                                        next(null, party);
                                    }
                                });
                            }
                            */
                        ],
                        function(err, tableData){
                            if(err){
                                console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                                res.status(200).send("Get Login Data Error.");
                            }
                            else{
                                var user_data = {
                                    userinfo : newTable.rows[0],
                                    servant_list : tableData[0].rows,
                                    monster_list : tableData[1].rows,
                                    item_list : tableData[2].rows,
                                    //party_list : tableData[3].rows
                                }
                                clearInterval(timer);
                                console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                                res.status(200).send(user_data);
                            }
                        });
                    }
                    else{
                        count += 1;
                        if(count >= 5){
                            clearInterval(timer);
                            console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                            res.status(200).send("Get Login Table Error.");
                        }
                    }
                }
                else{
                    count += 1;
                    if(count >= 5){
                        clearInterval(timer);
                        console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                        res.status(200).send("Get Login Table Error.");
                    }
                }
            }
    })}, 2000);
}

/**
 * Get Pre-Sale Login Data
 * 
 * @param req
 * @param res
 */
User.getPreLogindata = function(req, res){
    var func = 'getPreLogindata';
    
    var user = req.body.user;
    eos = Eos(config.eos);

    async.parallel([
        function(next){
            // Get Servant Info
            eos.getTableRows({
                code : config.contract.main,
                scope : user,
                table : 'preservant',
                json : true
            }, function(err, servant){
                if(err){
                    next(err);
                }
                else{
                    next(null, servant);
                }
            });
        },
        function(next){
            // Get Monster Info
            eos.getTableRows({
                code : config.contract.main,
                scope : user,
                table : 'premonster',
                json : true
            }, function(err, monster){
                if(err){
                    next(err);
                }
                else{
                    next(null, monster);
                }
            });
        },
        function(next){
            // Get Item Info
            eos.getTableRows({
                code : config.contract.main,
                scope : user,
                table : 'preitem',
                json : true
            }, function(err, item){
                if(err){
                    next(err);
                }
                else{
                    next(null, item);
                }
            });
        }
    ],
    function(err, tableData){
        if(err){
            console.log("Get Table Error");
            res.status(200).send("Get Pre Login Data Error.");
        }
        else{
            var user_data = {};
            if(tableData[1].rows.length == 0)
            {
                user_data.signup = null;
                console.log(config.color.yellow, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                res.status(200).send(user_data);
            }
            else{
                user_data.servant_list = tableData[0].rows;
                user_data.monster_list = tableData[1].rows;
                user_data.item_list = tableData[2].rows;
                user_data.signup = true;

                console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                res.status(200).send(user_data);
            }
        }
    });
}

/**
 * PreSale Sign Up 
 * 
 * @param req
 * @param res
 */
User.preSignUp = function(req, res){
    var func = 'preSignUp';

    var user = req.body.user;
    eos = Eos(config.eos);

    var count = 0;
    var timer = setInterval(function(){
        eos.getTableRows({
            code : config.contract.main,
            scope : user,
            table : 'premonster',
            json : true
        }, function(err, newTable){
            if(err){
                console.error("Fail to get Table.");
            }
            else{
                if(newTable.rows.length != 0){
                    async.parallel([
                        function(next){
                            // Get Servant Info
                            eos.getTableRows({
                                code : config.contract.main,
                                scope : user,
                                table : 'preservant',
                                json : true
                            }, function(err, servant){
                                if(err){
                                    next(err);
                                }
                                else{
                                    next(null, servant);
                                }
                            });                            
                        },
                        function(next){
                            // Get Monster Info
                            eos.getTableRows({
                                code : config.contract.main,
                                scope : user,
                                table : 'premonster',
                                json : true
                            }, function(err, monster){
                                if(err){
                                    next(err);
                                }
                                else{
                                    next(null, monster);
                                }
                            });
                        },
                        function(next){
                            // Get Item Info
                            eos.getTableRows({
                                code : config.contract.main,
                                scope : user,
                                table : 'preitem',
                                json : true
                            }, function(err, item){
                                if(err){
                                    next(err);
                                }
                                else{
                                    next(null, item);
                                }
                            });                            
                        }
                    ],
                    function(err, tableData){
                        if(err){
                            console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                            res.status(200).send("Get Pre Sign up Data Error.");
                        }
                        else{
                            var user_data = {
                                servant_list : tableData[0].rows,
                                monster_list : tableData[1].rows,
                                item_list : tableData[2].rows,
                            }
                            clearInterval(timer);
                            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                            res.status(200).send(user_data);
                        }
                    });
                }
                else{
                    count += 1;
                    if(count >= 5){
                        clearInterval(timer);
                        console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                        res.status(200).send("Pre signup time out");
                    }
                }
            }
    })}, 2000);
}

module.exports = User;