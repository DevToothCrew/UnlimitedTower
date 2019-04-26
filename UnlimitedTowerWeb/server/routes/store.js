Eos = require('eosjs')
config = require('../config/')

var async = require('async');
var poolCluster = require('../lib/MySQLPoolCluster').PoolCluster;

function Store() {}

/**
 * Get Store List
 * 
 * @param req
 * @param res
 */
Store.get = function(req, res){

    var func = "getStoreList";

    eos = Eos(config.eos_jungle);

    var user  = req.body.user;
    var type = req.body.type;

    eos.getTableRows({
        code : config.contract.dev,
        scope : config.contract.dev,
        table : 'tshoplist',
        json : true
    }, function(err, storeList){
        if(err){
            console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            res.status(200).send("Fail:Get Table:" + func);
        }
        else{
            var data = {
                shop_type : type
            }
            var shop_product_list = [];
            for(var i = 0 ; i < storeList.rows.length ; i++)
            {
                if(storeList.rows[i].type == type){
                    shop_product_list.push({index:storeList.rows[i].id, type:storeList.rows[i].type, id:storeList.rows[i].product_id, limit_count:storeList.rows[i].limit_count});
                }
            }

            data.shop_product_list = shop_product_list;

            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            res.status(200).send(data);
        }
    });
}

/**
 * Buy EOS Item 
 * 
 * @param req
 * @param res
 */
Store.buyEosItem = function(req, res){

    var func = "buyEosItem";

    eos = Eos(config.eos_jungle);

    var user  = req.body.user;

    eos.getTableRows({
        code : config.contract.dev,
        scope : config.contract.dev,
        table : 'tuserauth',
        lower_bound : user,
        limit : 1,
        json : true
    }, function(err, userTable){
        if(err){
            console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            res.status(200).send("Fail:Get Table:" + func);
        }
        else{
            var data = {
                servant_inventory : userTable.rows[0].servant_inventory,
                monster_inventory : userTable.rows[0].monster_inventory,
                equipment_inventory : userTable.rows[0].equipment_inventory,
                item_inventory : userTable.rows[0].item_inventory
            }
            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            res.status(200).send(data);
        }
    });
}

/**
 * Buy UTG Item 
 * 
 * @param req
 * @param res
 */
Store.buyUtgItem = function(req, res){

    var func = "buyUtgItem";

    eos = Eos(config.eos_jungle);

    var user  = req.body.user;

    eos.getTableRows({
        code : config.contract.dev,
        scope : user,
        table : 'tuserauth',
        limit : 1000,
        json : true
    }, function(err, itemTable){
        if(err){
            console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            res.status(200).send("Fail:Get Table:" + func);
        }
        else{
            var data = {
                item_list : itemTable.rows
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
Store.open = function(req, res){

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
                table : "tmaildb",
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
                code : config.contract.dev,
                scope : user,
                table : table,
                limit : 1000,
                json : true
            }, function(err, oldTable){
                if(err){
                    callback("Fail:Get Table:" + func);
                }
                else{
                    callback(null, oldTable, table);
                }
            });
        },
        function(oldTable, table, callback){
            var count = 0;

            var timer = setInterval(function(){
                eos.getTableRows({
                    code : config.contract.dev,
                    scope : user,
                    table : table,
                    limit : 1000,
                    json : true
                }, function(err, newTable){
                    if(err){
                        clearInterval(timer);
                        callback("Fail:Get Table:" + func);
                    }
                    else{
                        var data = {
                            mail_open_index_list : mail_idx
                        }
                        if(oldTable.rows.length != newTable.rows.length){
                            clearInterval(timer);
                            if(table == "tservant"){
                                data.servant_data_list = newTable.rows;
                                data.monster_data_list = null;
                                data.equip_data_list = null;
                                data.item_data_list = null;
                            }
                            if(table == "tmonster"){
                                data.servant_data_list = null;
                                data.monster_data_list = newTable.rows;
                                data.equip_data_list = null;
                                data.item_data_list = null;
                            }
                            if(table == "tequipments"){
                                data.servant_data_list = null;
                                data.monster_data_list = null;
                                data.equip_data_list = newTable.rows;
                                data.item_data_list = null;
                            }
                            else{
                                data.servant_data_list = null;
                                data.monster_data_list = null;
                                data.equip_data_list = null;
                                data.item_data_list = newTable.rows;
                            }
                                
                            callback(null, data);
                        }
                        else{
                            count++;
                            if (count > 10) {
                                clearInterval(timer);
                                callback("Fail:Time Out:" + func);
                            }
                        }
                    }
                });
            }, 1000);
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

module.exports = Store;