Eos = require('eosjs')
config = require('../config/')

var async = require('async');

function Battle() { }

/**
 * Battle Action 
 * 
 * @param req
 * @param res
 */
Battle.battleAction = function (req, res) {
    var func = 'battleAction';

    var user = req.body.user;
    eos = Eos(config.eos_jungle);

    async.waterfall([
        // Read old table
        function (callback) {
            eos.getTableRows({
                code: config.contract.dev,
                scope: config.contract.dev,
                table: 'tbattleact',
                lower_bound: user,
                limit: 1,
                json: true
            }, function (err, oldTable) {
                if (err) {
                    console.error("Fail to get table.");
                }
                else {
                    callback(null, oldTable);
                }
            });
        },
        // compare new table with old table after get new table
        function (oldTable, callback) {
            var count = 0;
            var timer = setInterval(function () {
                eos.getTableRows({
                    code: config.contract.dev,
                    scope: config.contract.dev,
                    table: 'tbattleact',
                    lower_bound: user,
                    limit: 1,
                    json: true
                }, function (err, newTable) {
                    if (err) {
                        console.error("Fail to get Table.");
                        callback('err');
                    }
                    else {
                        if (oldTable.rows[0].turn != newTable.rows[0].turn) {
                            clearInterval(timer);
                            eos.getTableRows({
                                code: config.contract.dev,
                                scope: config.contract.dev,
                                table: 'tstgstate',
                                lower_bound: user,
                                limit: 1,
                                json: true
                            }, function (err, result) {
                                if (err) {
                                    console.error("Fail to get Battle state Table.");
                                    callback('err');
                                }
                                else {
                                    var data = {
                                        action : newTable.rows[0],
                                        state : result.rows[0].state
                                    }

                                    if(result.rows[0].state != 1){
                                        eos.getTableRows({
                                            code: config.contract.dev,
                                            scope: config.contract.dev,
                                            table: 'tclearreward',
                                            lower_bound: user,
                                            limit: 1,
                                            json: true
                                        }, function(err, reward){
                                            if(err){
                                                console.err("Fail to get reward Table.");
                                                callback('err');
                                            }
                                            else{
                                                data.reward = reward.rows[0];
                                                callback(null, data);
                                            }
                                        });
                                    }
                                    else{
                                        callback(null, data);
                                    }
                                }
                            });
                        }
                        else{
                            if(count > 10){
                                clearInterval(timer);
                                callback('error');
                            }
                            count++;
                        }
                    }
                })
            }, 2000);
        }
    ],
    function (err, result) {
        if (err) {
            console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            res.status(200).send("Get Table Test fail.");
        }
        else {
            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
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
Battle.getStageInfo = function (req, res) {
    var func = 'getStageInfo';

    var stage_num = req.body.stage_num;
    eos = Eos(config.eos_jungle);

    eos.getTableRows({
        code: config.contract.main,
        scope: config.contract.main,
        table: 'cstagedata',
        lower_bound: stage_num,
        limit: 1,
        json: true
    }, function (err, stageData) {
        if (err) {
            console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            res.status(200).send("Fail to get stage data.");
        }
        else {
            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
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
Battle.battleStart = function (req, res) {
    var func = 'battleStart';

    var user = req.body.user;
    eos = Eos(config.eos_jungle);

    var count = 0;

    var timer = setInterval(function () {
        eos.getTableRows({
            code: config.contract.dev,
            scope: config.contract.dev,
            table: 'tstgstate',
            lower_bound: user,
            limit: 1,
            json: true
        }, function (err, newTable) {
            if (err) {
                console.error("Fail to get Table.");
            }
            else {
                if (newTable.rows.length != 0) {
                    if(newTable.rows[0].state != 1){
                        if (count > 10) {
                            clearInterval(timer);
                            console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                            res.status(200).send("fail");
                        }
                        count++;
                    }
                    else{
                        clearInterval(timer);
                        console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                        res.status(200).send(newTable.rows[0]);
                    }
                    
                }
                else {
                    if (count > 10) {
                        clearInterval(timer);
                        console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                        res.status(200).send("fail");
                    }
                    count++;
                }
            }
        })
    }, 2000);
}

/**
 * Battle Reward
 * 
 * @param req
 * @param res
 */
Battle.battleReward = function (req, res) {
    var func = 'battleReward';

    var user = req.body.user;
    eos = Eos(config.eos_jungle);

    var count = 0;

    var timer = setInterval(function () {
        eos.getTableRows({
            code: config.contract.dev,
            scope: config.contract.dev,
            table: 'tclearreward',
            lower_bound: user,
            limit: 1,
            json: true
        }, function (err, newTable) {
            if (err) {
                console.error("Fail to get Table.");
            }
            else {
                if (newTable.rows.length != 0) {
                    clearInterval(timer);
                    console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                    res.status(200).send(newTable.rows[0]);
                }
                else {
                    if (count > 10) {
                        clearInterval(timer);
                        console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                        res.status(200).send("fail ", func);
                    }
                    count++;
                }
            }
        })
    }, 2000);
}
module.exports = Battle;