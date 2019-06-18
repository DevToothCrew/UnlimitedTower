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

    eos = Eos(config.eos);

    var user = req.body.user;
    var index = req.body.index;
    var add_item_list = req.body.add_item_list;
    var pre_info = req.body.pre_info;
    var item_info = req.body.item_info;

    var old_item = JSON.parse(item_info);
    var old_data = JSON.parse(pre_info);
    var count =0 ;
    var old_item_count =JSON.stringify(old_item.item_list);

    var timer = setInterval(function(){
    async.waterfall([
        function(callback){
            eos.getTableRows({
                code: config.contract.main,
                scope: user,
                table: 'titem',
                lower_bound: add_item_list[0],
                limit: 1,
                json: true
            }, function(err, item_list){
                if(err){
                    callback("Fail:Get Table:" + func);
                }
                else{
                    var send_data = {
                        id:add_item_list[0]
                    }
                    if(item_list.rows.length == 0){
                        send_data.item_list = [];
                    }
                    else if(item_list.rows[0].id != add_item_list[0]){
                        send_data.item_list = [];
                    }
                    else {
                        var new_item_count = JSON.stringify(item_list.rows[0].item_list);
                        if (old_item_count != new_item_count) {
                            clearInterval(timer);
                            send_data.item_list = item_list.rows[0].item_list;
                        }
                        else {
                            //타이머
                            if (count > 10)
                            {
                                clearInterval(timer);
                                res.status(200).send("Fail:sync");
                            }
                            count++;
                        }
                    }
                    var send_add_item_list = [];
                    send_add_item_list.push(send_data);
                    callback(null, send_add_item_list);
                }
            })
        },
        function(send_add_item_list, callback){
            eos.getTableRows({
                code: config.contract.main,
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
                            main_equipment_data:old_data,
                            del_equipment_index: index,
                            add_item_list: send_add_item_list
                        }
                        callback(null, data);
                    }
                    // 아이템이 꺠진 경우
                    else if (newTable.rows[0].index != index) {
                        var data = {
                            is_success: false,
                            main_equipment_data:old_data,
                            del_equipment_index: index,
                            add_item_list: send_add_item_list
                        }
                        callback(null, data);
                    }
                    // 아이템이 +1 강화된 경우
                        else {
                            //강화 수치에 대한 변화 비교
                            //강화 수치가 변화가 없을경우 타이머 돌게
                            if (old_data.equipment.upgrade != newTable.rows[0].equipment.upgrade) {
                                var data = {
                                    is_success: true,
                                    main_equipment_data: newTable.rows[0],
                                    del_equipment_index: 0,
                                    add_item_list: send_add_item_list
                                }
                                clearInterval(timer);
                                callback(null, data);
                            }
                            else {
                                if (count > 10) {
                                    clearInterval(timer);
                                    res.status(200).send("Fail:sync");
                                }
                                count++;
                            }
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
                code: config.contract.main,
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
    })
    }, 1000);
}

/**
 * Consumable Item Buy
 * 
 * @param req 
 * @param res 
 */
Item.buy = function(req, res){

    var func = "itemBuy";

    eos = Eos(config.eos);

    var user = req.body.user;
    var index = req.body.index;
    
    async.waterfall([
        function (callback) {
            eos.getTableRows({
                code: config.contract.main,
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

module.exports = Item;