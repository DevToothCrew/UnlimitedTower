Eos = require('eosjs')
config = require('../config/')

var async = require('async');

function Mail() {}

/**
 * Get Mail
 * 
 * @param req
 * @param res
 */
Mail.get = function(req, res){

    var func = "getMail";

    eos = Eos(config.eos_jungle);

    var user  = req.body.user;

    eos.getTableRows({
        code : config.contract.dev,
        scope : user,
        table : 'tmail',
        limit : 1000,
        json : true
    }, function(err, mailTable){
        if(err){
            console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            res.status(200).send("Fail:Get Table:" + func);
        }
        else{
            var data = {}
            if(mailTable.rows.length == 0)
            {
                data.mail_data_list = null;
            }
            else{
                data.mail_data_list = mailTable.rows;
            }
            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            res.status(200).send(data);
        }
    });
}

/**
 * Mail Open
 * 
 * @param req
 * @param res
 */
Mail.open = function(req, res){

    var func = "mailOpen";

    eos = Eos(config.eos_jungle);

    var user  = req.body.user;
    var mail_idx = req.body.mail_idx;

    var table = "";

    async.waterfall([
        function(callback){
            eos.getTableRows({
                code : config.contract.dev,
                scope : user,
                table : "tmail",
                lower_bound : mail_idx[0],
                limit : 1,
                json : true
            }, function(err, mailTable){
                if(err){
                    callback("Fail:Get Table:" + func);
                }
                else{
                    if(mailTable.rows[0].mail_type == 1 || mailTable.rows[0].mail_type == 5){
                        table = "tservant";
                    }   
                    else if(mailTable.rows[0].mail_type == 2 || mailTable.rows[0].mail_type == 6){
                        table = "tmonster";
                    }
                    else{
                        table = "tequipments";
                    }
                    callback(null, table);
                }
            });
        },
        function(table, callback){
            eos.getTableRows({
                code: config.contract.dev,
                scope: user,
                table: table,
                limit: 1000,
                json: true
            }, function (err, newTable) {
                if (err) {
                    callback("Fail:Get Table:" + func);
                }
                else {
                    var data = {
                        mail_open_index_list: mail_idx
                    }
                    if (table == "tservant") {
                        data.servant_data_list = newTable.rows;
                        data.monster_data_list = null;
                        data.equip_data_list = null;
                        data.item_data_list = null;
                    }
                    if (table == "tmonster") {
                        data.servant_data_list = null;
                        data.monster_data_list = newTable.rows;
                        data.equip_data_list = null;
                        data.item_data_list = null;
                    }
                    if (table == "tequipments") {
                        data.servant_data_list = null;
                        data.monster_data_list = null;
                        data.equip_data_list = newTable.rows;
                        data.item_data_list = null;
                    }
                    else {
                        data.servant_data_list = null;
                        data.monster_data_list = null;
                        data.equip_data_list = null;
                        data.item_data_list = newTable.rows;
                    }
                    callback(null, data);
                }
            });
        }
    ],
    function(err, result){
        if(err){
            console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            res.status(200).send(err);
        }
        else{
            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            res.status(200).send(result);
        }
    });
}

module.exports = Mail;