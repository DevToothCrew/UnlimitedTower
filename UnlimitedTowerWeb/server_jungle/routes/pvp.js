Eos = require('eosjs')
config = require('../config/')

var async = require('async');

var poolCluster = require('../lib/MySQLPoolCluster').PoolCluster;

function Pvp() { }

/**
 * Pvp Action 
 * 
 * @param req
 * @param res
 */
Pvp.pvpAction = function (req, res) {
    var func = 'PvpAction';

    var user = req.body.user;
    eos = Eos(config.eos);

    async.waterfall([
        // Read old table
        function (callback) {
            eos.getTableRows({
                code: config.contract.main,
                scope: config.contract.main,
                table: 'tbattleact',
                lower_bound: user,
                limit: 1,
                json: true
            }, function (err, oldTable) {
                if (err) {
                    console.error("Fail:Get Table:" + func);
                    callback("Fail:Get Table:" + func);
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
                    code: config.contract.main,
                    scope: config.contract.main,
                    table: 'tbattleact',
                    lower_bound: user,
                    limit: 1,
                    json: true
                }, function (err, newTable) {
                    if (err) {
                        clearInterval(timer);
                        console.error("Fail:Get Table:" + func);
                        callback("Fail:Get Table:" + func);
                    }
                    else {
                        if (oldTable.rows[0].turn != newTable.rows[0].turn) {
                            clearInterval(timer);
                            callback(null, newTable.rows[0]);
                        }
                        else{
                            if(count > 10){
                                clearInterval(timer);
                                callback('Fail:Time Out:' + func);
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
            res.status(200).send(err);
        }
        else {
            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            res.status(200).send(result);
        }
    });
}

/**
 * Get Pvp Info
 * 
 * @param req
 * @param res
 */
Pvp.getPvp = function (req, res) {
    var func = 'getPvp';

    var user = req.body.user;
    
    eos = Eos(config.eos);

    eos.getTableRows({
        code: config.contract.main,
        scope: config.contract.main,
        table: 'tstgstate',
        lower_bound: user,
        limit: 1,
        json: true
    }, function (err, stageData) {
        if (err) {
            console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            res.status(200).send("Fail:Get Table:" + func);
        }
        else {
            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            res.status(200).send(stageData.rows[0]);
        }
    });
}

/**
 * Pvp Start
 * 
 * @param req
 * @param res
 */
Pvp.pvpStart = function (req, res) {
    var func = 'pvpStart';

    var user = req.body.user;
    eos = Eos(config.eos);

    eos.getTableRows({
        code: config.contract.main,
        scope: config.contract.main,
        table: 'tstgstates',
        lower_bound: user,
        limit: 1,
        json: true
    }, function (err, newTable) {
        if (err) {
            console.error("Fail:Get Table:" + func);
            res.status(200).send("Fail:Get Table:" + func);
        }
        else {
            if (newTable.rows.length != 0 && newTable.rows[0].user == user) {
                if (newTable.rows[0].turn != 0) {
                    if (count > 10) {
                        console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                        res.status(200).send("Fail:Time Out:" + func);
                    }
                }
                else {
                    console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                    res.status(200).send(newTable.rows[0]);

                    // DB Login Count 추가
                    poolCluster.getConnection(function(err, connection){
                        if(err){
                            console.log("Fail:Connect DB:" + func);
                        }
                        else{
                            var sql = "INSERT INTO unt.log_user (user, pvp) VALUE (?, 1) ON DUPLICATE KEY UPDATE pvp=pvp+1";
                            connection.query(sql, user, function(err){
                                if(err){
                                    console.log(config.color.red, 'user : ', user, ', func : ', func + " User pvp update", ', time : ', new Date(new Date().toUTCString()));
                                }
                                connection.release();
                            });
                        }
                    });
                }
            }
        }
    })
}

/**
 * Pvp Reward
 * 
 * @param req
 * @param res
 */
Pvp.pvpResult = function (req, res) {
    var func = 'pvpResult';

    var user = req.body.user;
    eos = Eos(config.eos);

    eos.getTableRows({
        code: config.contract.main,
        scope: config.contract.main,
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
            if (newTable.rows[0].state == 2) {
                eos.getTableRows({
                    code: config.contract.main,
                    scope: user,
                    table: 'pvplog',
                    limit: 1000,
                    json: true
                }, function (err, reward) {
                    if (err) {
                        console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                        res.status(200).send("Fail:Get Reward Table:" + func);
                    }
                    else {
                        var data = {
                            user_name: reward.rows[newTable.rows.length - 1].user,
                            result: reward.rows[newTable.rows.length - 1].result
                        }
                        console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                        res.status(200).send(data);
                    }
                })
            }
            else {
                console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                res.status(200).send("Fail:Time Out:" + func);
            }
        }
    })
}

/**
 * Pvp Exit
 * 
 * @param req
 * @param res
 */
Pvp.pvpEscape = function(req, res){
    
    var func = "pvpEscape";

    var user = req.body.user;

    var count = 0;

    var timer = setInterval(function () {
        eos.getTableRows({
            code: config.contract.main,
            scope: config.contract.main,
            table: 'tuserauth',
            lower_bound: user,
            limit: 1,
            json: true
        }, function (err, newTable) {
            if (err) {
                clearInterval(timer);
                console.error("Fail:Get Table:" + func);
                res.status(200).send("Fail:Get Table:" + func);
            }
            else {
                if (newTable.rows[0].state == 2) {
                    clearInterval(timer);
                    console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                    var data = {
                        success : true
                    }
                    res.status(200).send(data);
                }
                else {
                    if (count > 10) {
                        clearInterval(timer);
                        console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                        res.status(200).send("Fail:Time Out:" + func);
                    }
                    count++;
                }
            }
        })
    }, 2000);
}

module.exports = Pvp;