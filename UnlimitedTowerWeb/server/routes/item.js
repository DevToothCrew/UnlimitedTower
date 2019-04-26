Eos = require('eosjs')
config = require('../config/')

var async = require('async');

function Item() {}
 
/**
 * Equipment Item Upgrade
 * 
 * @param req 
 * @param res 
 */
Item.upgrade = function(req, res){

    var func = "equipUpgrade";

    eos = Eos(config.eos_jungle);

    var user = req.body.user;
    var index = req.body.index;
    var need_idxs = req.body.need_idxs;

    async.waterfall([
        function(callback){
            eos.getTableRows({
                code: config.contract.dev,
                scope: user,
                table: 'tequipments',
                lower_bound: index,
                limit: 1,
                json: true
            }, function (err, newTable) {
                if (err) {
                    callback("Fail:Get Table:" + func);
                }
                else {
                    // 맨 마지막 위치에서 아이템이 깨진 경우
                    if (newTable.rows.length == 0) {
                        var data = {
                            is_success: false,
                            del_equipment_index: index,
                            add_item_list: need_idxs
                        }
                        callback(null, data);
                    }
                    // 아이템이 꺠진 경우
                    else if (newTable.rows[0].index != index) {
                        var data = {
                            is_success: false,
                            del_equipment_index: index,
                            add_item_list: need_idxs
                        }
                        callback(null, data);
                    }
                    // 아이템이 +1 강화된 경우
                    else {
                        var data = {
                            is_success: true,
                            main_equipment_data: newTable.rows[0],
                            del_equipment_index: 0,
                            add_item_list: need_idxs
                        }
                        callback(null, data);
                    }
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
            eos.getTableRows({
                code: config.contract.dev,
                scope: user,
                table: 'accounts',
                json: true
            }, function(err, utgTable){
                if(err){
                    console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                    res.status(200).send("Fail:Get Account Table:" + func);
                }
                else{
                    var utg = '';
                    
                    if (utgTable.rows.length == 0) {
                        utg = '0';
                    }
                    else {
                        var token = utgTable.rows[0].balance.split(" ");
                        token = token[0].split(".");
                        utg = token[0] + token[1];
                    }

                    result.utg = utg;

                    console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                    res.status(200).send(result);
                }
            });
        }
    });
}

/**
 * Consumable Item Buy
 * 
 * @param req 
 * @param res 
 */
Item.buy = function(req, res){

    var func = "itemBuy";

    eos = Eos(config.eos_jungle);

    var user = req.body.user;
    var index = req.body.index;
    
    async.waterfall([
        function (callback) {
            eos.getTableRows({
                code: config.contract.dev,
                scope: user,
                table: 'titem',
                lower_bound: index,
                limit: 1,
                json: true
            }, function (err, newTable) {
                if (err) {
                    callback("Fail:Get Table:" + func);
                }
                else {
                    callback(null, newTable.rows[0]);
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

Item.buyInventory = function(req, res){
    var func = "buyInventory";

    var user = req.body.user;
    var type = req.body.type;

    eos = Eos(config.eor_jungle);

    async.waterfall([
        function(callback){
            eos.getTableRows({
                code : config.contract.dev,
                scope : config.contract.dev,
                table : 'tuserauth',
                lower_bound : user,
                limit : 1,
                json : true
            }, function(err, oldTable){
                if(err){
                    callback("Fail:Get Table:" + func);
                }
                else{
                    callback(null, oldTable);
                }
            });
        },
        function(oldTable, callback){
            var count = 0;

            var timer = setInterval(function(){
                eos.getTableRows({
                    code : config.contract.dev,
                    scope : config.contract.dev,
                    table : 'tuserauth',
                    lower_bound : user,
                    limit : 1,
                    json : true
                }, function(err, newTable){
                    if(err){
                        clearInterval(timer);
                        callback("Fail:Get Table:" + func);
                    }
                    else{
                        if(type == 1){
                            if(oldTable.rows[0].servant_inventory != newTable.rows[0].servant_inventory){
                                clearInterval(timer);
                                callback(null, newTable.rows[0]);
                            }
                        }
                        if(type == 2){
                            if(oldTable.rows[0].monster_inventory != newTable.rows[0].monster_inventory){
                                clearInterval(timer);
                                callback(null, newTable.rows[0]);
                            }
                        }
                        if(type == 3){
                            if(oldTable.rows[0].equipitem_inventory != newTable.rows[0].equipitem_inventory){
                                clearInterval(timer);
                                callback(null, newTable.rows[0]);
                            }
                        }
                        if(type == 4){
                            if(oldTable.rows[0].item_inventory != newTable.rows[0].item_inventory){
                                clearInterval(timer);
                                callback(null, newTable.rows[0]);
                            }
                        }
                        if(count > 10){
                            clearInterval(timer);
                            callback("Fail:Time Out:" + func);
                        }
                        count++;
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
module.exports = Item;