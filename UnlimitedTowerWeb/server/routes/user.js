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
    
    var user = req.body.user;
    eos = Eos(config.eos);

    async.parallel([
        function(next){
            // Get Servant Info
            eos.getTableRows({
                code : config.contract.main,
                scope : config.contract.main,
                table : 'cservant',
                lower_bound : user,
                limit : 1,
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
            // Get Item Info
            eos.getTableRows({
                code : config.contract.main,
                scope : config.contract.main,
                table : 'citem',
                lower_bound : user,
                limit : 1,
                json : true
            }, function(err, item){
                if(err){
                    next(err);
                }
                else{
                    next(null, item);
                }
            });
        },
        function(next){
            // Get Monster Info
            eos.getTableRows({
                code : config.contract.main,
                scope : config.contract.main,
                table : 'cmonster',
                lower_bound : user,
                limit : 1,
                json : true
            }, function(err, monster){
                if(err){
                    next(err);
                }
                else{
                    next(null, monster);
                }
            });
        }
    ],
    function(err, tableData){
        if(err){
            console.log("Get Table Error");
            res.status(200).send("Get Login Data Error.");
        }
        else{
            var user_data = {
                servant: tableData[0],
                item: tableData[1],
                monster: tableData[2]
            }
            res.status(200).send(user_data);
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
            console.log("Get User Info Error.");
            res.status(200).send("Get User Info Error.");
        }
        else{
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
        scope : config.contract.main,
        table : 'cservant',
        lower_bound : user,
        limit : 1,
        json : true
    }, function(err, result){
        if(err){
            console.log("Get Servant Table Error");
            res.status(200).send("Get Servant Table Error.");
        }
        else{
            res.status(200).send(result.rows[0]);
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
        scope : config.contract.main,
        table : 'citem',
        lower_bound : user,
        limit : 1,
        json : true
    }, function(err, result){
        if(err){
            console.log("Get Item Table Error");
            res.status(200).send("Get Item Table Error.");
        }
        else{
            res.status(200).send(result.rows[0]);
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
        scope : config.contract.main,
        table : 'cmonster',
        lower_bound : user,
        limit : 1,
        json : true
    }, function(err, result){
        if(err){
            console.log("Get Monster Table Error");
            res.status(200).send("Get Monster Table Error.");
        }
        else{
            res.status(200).send(result.rows[0]);
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
                            callback(null, newTable);
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

module.exports = User;