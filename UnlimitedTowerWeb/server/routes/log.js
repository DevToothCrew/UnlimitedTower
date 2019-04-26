Eos = require('eosjs')
config = require('../config/')

var poolCluster = require('../lib/MySQLPoolCluster').PoolCluster;

function Log() {}

/**
 * Get Table Data
 * 
 * @param req
 * @param res
 */
Log.showLog = function(req, res){

    var user = req.body.user;
    var table = req.body.table;

    var sql_user = "";
    var sql_table = "";

    if(user.length != 0){
        sql_user = " WHERE user='" + user + "'";
    }

    if(table == 'eos'){
        sql_table = "unt.log_eos";
    }
    else if(table == 'gacha'){
        sql_table = "unt.log_gacha";
    }
    else{
        sql_table = "unt.log_utg";
    }

    poolCluster.getConnection(function(err, connection){
        if(err){
            res.status(200).send("Fail:DB Connection Err:" + func);
        }
        else{
            var sql = "SELECT * FROM " + sql_table + sql_user;
            connection.query(sql, function(err, result){
                connection.release();
                if(err){
                    res.status(200).send("Fail:DB Access Err:" + func);
                }
                else{
                    res.status(200).send(result);
                }
            });
        }
    });
}

/**
 * Insert Log Data
 * 
 * @param req
 * @param res
 */
Log.setLog = function(req, res){

    var user = req.body.user;
    var amount = req.body.amount;
    var type = req.body.type;
    var data = req.body.data;
    var table = req.body.table;

    var sql_table = "";

    if(table == 'eos'){
        sql_table = "unt.log_eos";
    }
    else{
        sql_table = "unt.log_utg";
    }

    poolCluster.getConnection(function(err, connection){
        if(err){
            res.status(200).send("Fail:DB Connection Err:" + func);
        }
        else{
            var logData = {
                user : user,
                amount : amount,
                type : type,
                data : data
            };
            var sql = "INSERT INTO "  + sql_table + " SET ?";
            connection.query(sql, logData, function(err){
                connection.release();
                if(err){
                    res.status(200).send("Fail:DB Access Err:" + func);
                }
                else{
                    res.status(200).send("OK:DB Insert:" + func);
                }
            });
        }
    });
}

module.exports = Log;