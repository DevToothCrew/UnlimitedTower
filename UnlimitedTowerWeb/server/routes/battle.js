Eos = require('eosjs')
config = require('../config/')

var async = require('async');

function Battle() {}

/**
 * Battle Action 
 * 
 * @param req
 * @param res
 */
Battle.battleAction = function(req, res){
    var func = 'battleAction';

    var user = req.body.user;
    eos = Eos(config.eos);

    async.waterfall([
        // Read old table
        function(callback){
            eos.getTableRows({
                code : config.contract.main,
                scope : config.contract.main,
                table : 'cbattle',
                lower_bound : user,
                limit : 1,
                json : true
            }, function(err, oldTable){
                if(err){
                    console.error("Fail to get table.");
                }
                else{
                    console.log('Old Table : ',oldTable);
                    callback(null, oldTable);
                }
            });
        },
        // compare new table with old table after get new table
        function(oldTable, callback){
            var timer = setInterval(function(){
                eos.getTableRows({
                    code : config.contract.main,
                    scope : config.contract.main,
                    table : 'cbattle',
                    lower_bound : user,
                    limit : 1,
                    json : true
                }, function(err, newTable){
                    console.log('New Table : ', newTable);
                    if(err){
                        console.error("Fail to get Table.");
                        callback('err');
                    }
                    else{
                        if(oldTable.rows[0].turn_count != newTable.rows[0].turn_count){
                            clearInterval(timer);
                            callback(null, newTable.rows[0]);
                        }
                    }
            })}, 2000);
        }
    ],
    function(err, result){
        if(err){
            console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', Date.now());
            res.status(200).send("Get Table Test fail.");
        }
        else{
            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', Date.now());
            res.status(200).send(result);
        }
    });
}

/**
 * Get Stage Info
 * 
 * @param req
 * @param res
 */
Battle.getStageInfo = function(req, res){
    var func = 'getStageInfo';

    var stage_num = req.body.stage_num;
    eos = Eos(config.eos);

    eos.getTableRows({
        code : config.contract.main,
        scope : config.contract.main,
        table : 'cstagedata',
        lower_bound : stage_num,
        limit : 1,
        json : true
    }, function(err, stageData){
        if(err){
            console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', Date.now());
            res.status(200).send("Fail to get stage data.");
        }
        else{
            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', Date.now());
            res.status(200).send(stageData.rows[0]);
        }
    });
}

/**
 * Battle Start
 * 
 * @param req
 * @param res
 */
Battle.battleStart = function(req, res){
    var func = 'battleStart';

    var user = req.body.user;
    var stage_num = req.body.stage_num;
    eos = Eos(config.eos);

    async.parallel([
        // Get Stage Info
        function(next){            
            eos.getTableRows({
                code : config.contract.main,
                scope : config.contract.main,
                table : 'cstagedata',
                lower_bound : stage_num,
                limit : 1,
                json : true
            }, function(err, stageInfo){
                if(err){
                    next(err);
                }
                else{
                    next(null, stageInfo);
                }
            });
        },
        // Get Battle Info
        function(next){
            async.waterfall([
                // get Old Table
                function(callback){
                    eos.getTableRows({
                        code : config.contract.main,
                        scope : config.contract.main,
                        table : 'cbattle',
                        lower_bound : user,
                        limit : 1,
                        json : true
                    }, function(err, oldTable){
                        if(err){
                            console.error("Fail to get table.");
                        }
                        else{
                            console.log('Old Table : ', oldTable);
                            callback(null, oldTable);
                        }
                    });
                },
                // compare new table with old table after get new table
                function(oldTable, callback){        
                    var timer = setInterval(function(){
                        eos.getTableRows({
                            code : config.contract.main,
                            scope : config.contract.main,
                            table : 'cbattle',
                            lower_bound : user,
                            limit : 1,
                            json : true
                        }, function(err, newTable){
                            console.log('New Table : ',newTable);
                            if(err){
                                console.error("Fail to get Table.");
                            }
                            else{
                                if(oldTable.rows[0].b_turn_count != newTable.rows[0].b_turn_count){
                                    clearInterval(timer);
                                    callback(null, newTable);
                                }
                            }
                    })}, 1000);
                }
            ],
            function(err, result){
                if(err){
                    console.error("Battle Start Error.");
                    next(err);
                }
                else{
                    next(null, result);
                }
            });
        }
    ],
    function(err, tableData){
        if(err){
            console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', Date.now());
            res.status(200).send("fail ", func);
        }
        else{
            var user_data = {
                stage: tableData[0].rows[0],
                battle: tableData[1].rows[0]
            }
            res.status(200).send(user_data);
            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', Date.now());
        }
    });
}


module.exports = Battle;