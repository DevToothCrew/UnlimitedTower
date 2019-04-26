Eos = require('eosjs')
config = require('../config/')

var async = require('async');

var poolCluster = require('../lib/MySQLPoolCluster').PoolCluster;

function Chat() {}

/**
 * Formation Setting
 * 
 * @param req 
 * @param res 
 */
Chat.chatRegist = function(req, res){
    var func = 'chatRegist';

    var user = req.body.user;

    eos = Eos(config.eos_jungle);

    async.waterfall([
        function(callback){
            eos.getTableRows({
                code : config.contract.dev,
                scope : config.contract.dev,
                table : 'tchat',
                json : true
            }, function(err, old_data){
                if(err){
                    callback("Fail:Get Table:" + func);
                }
                else{
                    callback(null, old_data);
                }
            });
        },
        function(old_data, callback){
            var count = 0;

            var timer = setInterval(function(){
                eos.getTableRows({
                    code: config.contract.dev,
                    scope: config.contract.dev,
                    table: 'tchat',
                    json: true
                }, function(err, new_data){
                    if(err){
                        clearInterval(timer);
                        callback("Fail:Get Table:" + func);
                    }
                    else{
                        var flag = false;

                        if(old_data.rows[0].user != new_data.rows[0].user){
                            flag = true;
                        }

                        if(old_data.rows[0].price != new_data.rows[0].price){
                            flag = true;
                        }

                        if(old_data.rows[0].text != new_data.rows[0].text){
                            flag = true;
                        }

                        if(flag){
                            clearInterval(timer);
                            callback(null, new_data.rows[0]);
                        }
                        else{
                            count += 1;
                            if(count >= 5){
                                clearInterval(timer);
                                console.log("Timer Time Out!");
                                callback("Fail:Time Out:" + func);
                            }
                        }
                    }
                });
            }, 3000);
        }
    ],
    function(err, result){
        if(err){
            console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', Date.now());
            res.status(200).send(err);
        }
        else{
            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', Date.now());
            res.status(200).send(result);

            poolCluster.getConnection(function(err, connection){
                if(err){
                    res.status(200).send("connect fail.");
                }
                else{
                    var token = result.price.split(" ");
                    token = token[0].split(".");
                    var utg = token[0] + token[1];

                    var logData = {
                        user : user,
                        amount : utg,
                        type : "chat",
                        data : result.text
                    };
                    var sql = "INSERT INTO unt.log_utg SET ?";
                    connection.query(sql, logData, function(err){
                        connection.release();
                        if(err){
                            res.status(200).send("Access DB fail.");
                        }
                        else{
                            res.status(200).send("INSERT success.");
                        }
                    });
                }
            });
        }
    });
}

module.exports = Chat;