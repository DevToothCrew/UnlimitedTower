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

    eos = Eos(config.eos);

    var user  = req.body.user;

    eos.getTableRows({
        code : config.contract.main,
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

    eos = Eos(config.eos);

    var user  = req.body.user;
    var mail_idx = req.body.mail_idx;

    var table = "";

    async.waterfall([
        function(callback){
            eos.getTableRows({
                code : config.contract.main,
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
                    if(mailTable.rows[0].mail_type == 1 || mailTable.rows[0].mail_type == 5 || mailTable.rows[0].mail_type == 8){
                        table = "tservant";
                    }   
                    else if(mailTable.rows[0].mail_type == 2 || mailTable.rows[0].mail_type == 6 || mailTable.rows[0].mail_type == 9){
                        table = "tmonster";
                    }
                    else if(mailTable.rows[0].mail_type == 3 || mailTable.rows[0].mail_type == 7 || mailTable.rows[0].mail_type == 10){
                        table = "tequipments";
                    }
                    else if(mailTable.rows[0].mail_type == 11){
                        table = "titem";
                    }
                    else{
                        table = "account";
                    }
                    callback(null, table);
                }
            });
        },
        function(table, callback){
            eos.getTableRows({
                code: config.contract.main,
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
                    if (table == "titem") {
                        data.servant_data_list = null;
                        data.monster_data_list = null;
                        data.equip_data_list = null;
                        data.item_data_list = newTable.rows;

                        console.log(data);
                    }
                    else {
                        data.servant_data_list = null;
                        data.monster_data_list = null;
                        data.equip_data_list = null;
                        data.item_data_list = null;

                        if (newTable.rows.length == 0) {
                            data.utg = '0';
                        }
                        else {
                            var token = newTable.rows[0].balance.split(" ");
                            token = token[0].split(".");
                            data.utg = token[0] + token[1];
                        }
                        
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

/**
 * Get Mail - Paging
 * 
 * @param req
 * @param res
 */
Mail.getFromPage = function(req, res){

    var func = "getMailFromPage";

    eos = Eos(config.eos);

    var paging_count = 6;
    var user  = req.body.user;
    var page = (req.body.page - 1) * paging_count + 1;

    eos.getTableRows({
        code : config.contract.main,
        scope : user,
        table : 'tmail',
        limit : page,
        json : true
    }, function(err, mailTable){
        if(err){
            console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            res.status(200).send("Fail:Get Table1:" + func);
        }
        else{
            if(mailTable.rows.length < page){
                console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                res.status(200).send("Fail:There is no more mail.:" + func);
            }
            else{
                eos.getTableRows({
                    code: config.contract.main,
                    scope: user,
                    table: 'tmail',
                    lower_bound : mailTable.rows[mailTable.rows.length - 1].mail_index,
                    limit: paging_count,
                    json: true
                }, function(err, mail_page){
                    if(err){
                        console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                        res.status(200).send("Fail:Get Table2:" + func);
                    }
                    else {
                        var data = {}
                        if (mailTable.rows.length == 0) {
                            data.mail_data_list = null;
                        }
                        else {
                            data.mail_data_list = mail_page.rows;
                        }
                        console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                        res.status(200).send(data);
                    }
                })
            }
        }
    });
}


/**
 * Mail All Open - Paging
 * 
 * @param req
 * @param res
 */
Mail.openAllMailInPage = function(req, res){

    var func = "openAllMailInPage";

    eos = Eos(config.eos);

    var user  = req.body.user;
    var mail_idxs = req.body.mail_idxs;
    var servant_count = req.body.servant_count;
    var monster_count = req.body.monster_count;
    var equip_count = req.body.equip_count;
    var item_count = req.body.item_count;
    var utg_count = req.body.utg_count;

    async.parallel([
        function(next){
            if(servant_count != 0)
            {
                eos.getTableRows({
                    code : config.contract.main,
                    scope : user,
                    table : "tservant",
                    limit : 1000,
                    json : true
                }, function(err, servant){
                    if(err){
                        next("Fail:Get Table:" + func);
                    }
                    else{
                        eos.getTableRows({
                            code: config.contract.main,
                            scope: user,
                            table: "tservant",
                            lower_bound : servant.rows[servant.rows.length - servant_count].index,
                            limit: servant_count,
                            json: true
                        }, function(err, new_servant){
                            if(err){
                                next("Fail:Get Table:" + func);
                            }
                            else{
                                next(null, new_servant);
                            }
                        })
                    }
                });
            }
            else{
                next(null, "null");
            }
            
        },
        function(next){
            if(monster_count != 0){
                eos.getTableRows({
                    code : config.contract.main,
                    scope : user,
                    table : "tmonster",
                    limit : 1000,
                    json : true
                }, function(err, monster){
                    if(err){
                        next("Fail:Get Table:" + func);
                    }
                    else{
                        eos.getTableRows({
                            code: config.contract.main,
                            scope: user,
                            table: "tmonster",
                            lower_bound : monster.rows[monster.rows.length - monster_count].index,
                            limit: monster_count,
                            json: true
                        }, function(err, new_monster){
                            if(err){
                                next("Fail:Get Table:" + func);
                            }
                            else{
                                next(null, new_monster);
                            }
                        })
                    }
                });
            }
            else{
                next(null, "null");
            }
        },
        function(next){
            if(equip_count != 0){
                eos.getTableRows({
                    code : config.contract.main,
                    scope : user,
                    table : "tequipments",
                    limit : 1000,
                    json : true
                }, function(err, equipment){
                    if(err){
                        next("Fail:Get Table:" + func);
                    }
                    else{
                        eos.getTableRows({
                            code: config.contract.main,
                            scope: user,
                            table: "tequipments",
                            lower_bound : equipment.rows[equipment.rows.length - equip_count].index,
                            limit: equip_count,
                            json: true
                        }, function(err, new_equip){
                            if(err){
                                next("Fail:Get Table:" + func);
                            }
                            else{
                                next(null, new_equip);
                            }
                        })
                    }
                });
            }
            else{
                next(null, "null");
            }
        },
        function(next){
            if(item_count != 0)
            {
                eos.getTableRows({
                    code : config.contract.main,
                    scope : user,
                    table : "titem",
                    limit : 1000,
                    json : true
                }, function(err, item){
                    if(err){
                        next("Fail:Get Table:" + func);
                    }
                    else{
                        next(null, item);
                    }
                });
            }
            else{
                next(null, "null");
            }            
        },function(next){
            eos.getTableRows({
                code: config.contract.main,
                scope: user,
                table: "accounts",
                limit: 1000,
                json: true
            }, function (err, utg) {
                if (err) {
                    next("Fail:Get Table:" + func);
                }
                else {
                    next(null, utg);
                }
            });
        },
    ],
    function(err, result){
        if(err){
            console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            res.status(200).send(err);
        }
        else{
            var data = {
                mail_open_index_list: mail_idxs
            }

            if(result[0] == "null"){
                data.servant_data_list = null;
            }
            else{
                data.servant_data_list = result[0].rows;
            }
            if(result[1] == "null"){
                data.monster_data_list = null;
            }
            else{
                data.monster_data_list = result[1].rows;
            }
            if(result[2] == "null"){
                data.equip_data_list = null;
            }
            else{
                data.equip_data_list = result[2].rows;
            }
            if(result[3] == "null"){
                data.item_data_list = null;
            }
            else{
                data.item_data_list = result[3].rows;
            }
            if (result[4].rows.length == 0) {
                data.utg = '0';
            }
            else {
                var token = result[4].rows[0].balance.split(" ");
                token = token[0].split(".");
                data.utg = token[0] + token[1];
            }

            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            res.status(200).send(data);
        }
    });
}

module.exports = Mail;