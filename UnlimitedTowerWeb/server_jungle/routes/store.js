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

    eos = Eos(config.eos);

    var user  = req.body.user;
    var type = req.body.type;

    eos.getTableRows({
        code : config.contract.main,
        scope : config.contract.main,
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
 * Buy Item 
 * 
 * @param req
 * @param res
 */
Store.buyItem = function(req, res){

    var func = "buyItem";

    eos = Eos(config.eos);

    var user  = req.body.user;
    var type = req.body.type;
    var count = req.body.count;
    var consumeEos = req.body.consumeeos;
    var index = req.body.index;
    var item_idx = 0;

    // 무기 강화 스크롤 구매
    if(index == 5){
        item_idx = 500100;
    }
    // 방어구 강화 스크롤 구매
    else if(index == 6){
        item_idx = 500110;
    }
    // 악세서리 강화 스크롤 구매
    else if(index == 7){
        item_idx = 500120;
    }
    // 스테이지 입장권 : 보통 구매
    else if(index == 8){
        item_idx = 500200;
    }
     // 스테이지 입장권 : 어려움 구매
     else if(index == 9){
        item_idx = 500210;
    }
     // 스테이지 입장권 : 악몽 구매
     else if(index == 10){
        item_idx = 500220;
    }
     // 스테이지 입장권 : 지옥 구매
     else if(index == 11){
        item_idx = 500230;
    }
    // 서번트 소환권1
    else if(index == 12){
        item_idx = 500200;
    }
    // 서번트 소환권2
    else if(index == 13){
        item_idx = 500210;
    }
    // 서번트 소환권3
    else if(index == 14){
        item_idx = 500220;
    }
    // 서번트 소환권4
    else{
        item_idx = 500230;
    }

    async.parallel([
        function (next) {
            // Get Token Info : 0
            eos.getTableRows({
                code: config.contract.main,
                scope: user,
                table: 'accounts',
                json: true
            }, function (err, token) {
                if (err) {
                    next("Fail:Get Account Table:" + func);
                }
                else {
                    next(null, token);
                }
            });
        },
        function (next) {
            // Get User Info : 1
            eos.getTableRows({
                code: config.contract.main,
                scope: config.contract.main,
                lower_bound: user,
                limit: 1,
                table: 'tuserauth',
                json: true
            }, function (err, user) {
                if (err) {
                    next("Fail:Get Auth Table:" + func);
                }
                else {
                    next(null, user);
                }
            });
        },
        function (next) {
            // Get EOS Info : 2
            eos.getTableRows({
                code: 'eosio.token',
                scope: user,
                limit: 1,
                table: 'accounts',
                json: true
            }, function (err, uEos) {
                if (err) {
                    next("Fail:Get Token Table:" + func);
                }
                else {
                    next(null, uEos);
                }
            });
        },
        function (next) {
            // Get Item Info : 3
            eos.getTableRows({
                code: config.contract.main,
                scope: user,
                table: 'titem',
                limit: 1,
                lower_bound : item_idx,
                json: true
            }, function (err, item) {
                if (err) {
                    next("Fail:Get Item Table:" + func);
                }
                else {
                    next(null, item);
                }
            });
        },
        function (next) {
            // Get Servant Info : 4
            eos.getTableRows({
                code: config.contract.main,
                scope: user,
                table: 'tservant',
                limit: 900,
                json: true
            }, function (err, servant) {
                if (err) {
                    next("Fail:Get Item Table:" + func);
                }
                else {
                    next(null, servant);
                }
            });
        },
        function (next) {
            // Get Monster Info : 5
            eos.getTableRows({
                code: config.contract.main,
                scope: user,
                table: 'tmonster',
                limit: 900,
                json: true
            }, function (err, monster) {
                if (err) {
                    next("Fail:Get Item Table:" + func);
                }
                else {
                    next(null, monster);
                }
            });
        },
        function (next) {
            // Get Equipment Info : 6
            eos.getTableRows({
                code: config.contract.main,
                scope: user,
                table: 'tequipments',
                limit: 900,
                json: true
            }, function (err, euqip) {
                if (err) {
                    next("Fail:Get Item Table:" + func);
                }
                else {
                    next(null, euqip);
                }
            });
        }
    ],
        function (err, tableData) {
            if (err) {
                console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                res.status(200).send(err);
            }
            else {
                var user_data = {
                }

                // EOS Setting
                var uEos = tableData[2].rows[0].balance.split(" ");
                uEos = uEos[0].split(".");
                if (uEos[0] == '0' && uEos[1] == '0000') {
                    user_data.eos = '0';
                }
                else {
                    user_data.eos = uEos[0] + uEos[1];
                }

                // Token Setting
                if (tableData[0].rows.length == 0) {
                    user_data.utg = '0';
                }
                else {
                    var token = tableData[0].rows[0].balance.split(" ");
                    token = token[0].split(".");
                    user_data.utg = token[0] + token[1];
                }
                // 인벤토리 구매
                if(index < 5){
                    user_data.get_servant_list = [];
                    user_data.get_monster_list = [];
                    user_data.get_equipment_list = [];
                    user_data.get_item_list = [];

                    var inventory_info = {
                        servant_inventory : tableData[1].rows[0].servant_inventory,
                        monster_inventory : tableData[1].rows[0].monster_inventory,
                        equipment_inventory : tableData[1].rows[0].equipment_inventory,
                        item_inventory : tableData[1].rows[0].item_inventory
                    };
    
                    user_data.inventory_info = inventory_info;
    
                    console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                    res.status(200).send(user_data);
                }
                // 스크롤 구매
                else if(index >= 5 && index <= 11){
                    var temp_list = [];
                    temp_list.push(tableData[3].rows[0]);
                    user_data.get_servant_list = [];
                    user_data.get_monster_list = [];
                    user_data.get_equipment_list = [];
                    user_data.get_item_list = temp_list;

                    var inventory_info = {
                        servant_inventory : tableData[1].rows[0].servant_inventory,
                        monster_inventory : tableData[1].rows[0].monster_inventory,
                        equipment_inventory : tableData[1].rows[0].equipment_inventory,
                        item_inventory : tableData[1].rows[0].item_inventory
                    };
    
                    user_data.inventory_info = inventory_info;
    
                    console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                    res.status(200).send(user_data);
                }
                // 서번트 소환권 구매
                else if(index >= 12 && index <= 15){
                    user_data.get_servant_list = [];
                    //user_data.get_servant_list.push(tableData[4].rows[tableData[4].rows.length - 1]);
                    user_data.get_monster_list = [];
                    user_data.get_equipment_list = [];

                    if(tableData[3].rows.length == 0 || tableData[3].rows[0].id != item_idx){
                        var temp_item_data = {}
                        temp_item_data.id = item_idx;
                        temp_item_data.type = 3;
                        temp_item_data.item_list = [];

                        user_data.get_item_list = temp_item_data;
                    }
                    else{
                        user_data.get_item_list = tableData[3].rows[0];
                        }
                    
                    
                    var inventory_info = {
                        servant_inventory : tableData[1].rows[0].servant_inventory,
                        monster_inventory : tableData[1].rows[0].monster_inventory,
                        equipment_inventory : tableData[1].rows[0].equipment_inventory,
                        item_inventory : tableData[1].rows[0].item_inventory
                    };
    
                    user_data.inventory_info = inventory_info;

                    // console.log(user_data);
    
                    console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                    res.status(200).send(user_data);
                }
                // 스타트 패키지
                else if(index == 16){
                    user_data.get_servant_list = [];
                    user_data.get_monster_list = [];
                    user_data.get_equipment_list = [];
                    user_data.get_item_list = [];

                    var inventory_info = {
                        servant_inventory : tableData[1].rows[0].servant_inventory,
                        monster_inventory : tableData[1].rows[0].monster_inventory,
                        equipment_inventory : tableData[1].rows[0].equipment_inventory,
                        item_inventory : tableData[1].rows[0].item_inventory
                    };
    
                    user_data.inventory_info = inventory_info;
    
                    console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                    res.status(200).send(user_data);
                }
                else if(index == 17){   // 입장권 패키지
                    user_data.get_servant_list = [];
                    user_data.get_monster_list = [];
                    user_data.get_equipment_list = [];
                    user_data.get_item_list = [];

                    var inventory_info = {
                        servant_inventory : tableData[1].rows[0].servant_inventory,
                        monster_inventory : tableData[1].rows[0].monster_inventory,
                        equipment_inventory : tableData[1].rows[0].equipment_inventory,
                        item_inventory : tableData[1].rows[0].item_inventory
                    };
    
                    user_data.inventory_info = inventory_info;
    
                    console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                    res.status(200).send(user_data);
                }
                // // 강화 패키지 
                // else if(index == 17){
                //     async.parallel([
                //         function(next){
                //             eos.getTableRows({
                //                 code: config.contract.main,
                //                 scope: user,
                //                 table: 'titem',
                //                 limit: 1,
                //                 lower_bound: 500100,
                //                 json: true
                //             }, function(err, item_1){
                //                 if(err){
                //                     next("Fail:Get Item Table:" + func);
                //                 }
                //                 else{
                //                     next(null, item_1);
                //                 }
                //             })
                //         },
                //         function(next){
                //             eos.getTableRows({
                //                 code: config.contract.main,
                //                 scope: user,
                //                 table: 'titem',
                //                 limit: 1,
                //                 lower_bound: 500110,
                //                 json: true
                //             }, function(err, item_2){
                //                 if(err){
                //                     next("Fail:Get Item Table:" + func);
                //                 }
                //                 else{
                //                     next(null, item_2);
                //                 }
                //             })
                //         },
                //         function(next){
                //             eos.getTableRows({
                //                 code: config.contract.main,
                //                 scope: user,
                //                 table: 'titem',
                //                 limit: 1,
                //                 lower_bound: 500120,
                //                 json: true
                //             }, function(err, item_3){
                //                 if(err){
                //                     next("Fail:Get Item Table:" + func);
                //                 }
                //                 else{
                //                     next(null, item_3);
                //                 }
                //             })
                //         },

                //     ], function(err, result){
                //         if(err){
                //             console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                //             res.status(200).send(err);
                //         }
                //         else{
                //             var temp_list = [];
                //             temp_list.push(result[0].rows[0]);
                //             temp_list.push(result[1].rows[0]);
                //             temp_list.push(result[2].rows[0]);

                //             user_data.get_servant_list = [];
                //             user_data.get_monster_list = [];
                //             user_data.get_equipment_list = [];
                //             user_data.get_item_list = temp_list;

                //             var inventory_info = {
                //                 servant_inventory : tableData[1].rows[0].servant_inventory,
                //                 monster_inventory : tableData[1].rows[0].monster_inventory,
                //                 equipment_inventory : tableData[1].rows[0].equipment_inventory,
                //                 item_inventory : tableData[1].rows[0].item_inventory
                //             };
            
                //             user_data.inventory_info = inventory_info;
            
                //             console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                //             res.status(200).send(user_data);
                //         }
                //     })
                // }
                else if(index == 18){   //10연뽑 
                    user_data.get_servant_list = [];
                    user_data.get_monster_list = [];
                    user_data.get_equipment_list = [];
                    user_data.get_item_list = [];

                    var inventory_info = {
                        servant_inventory : tableData[1].rows[0].servant_inventory,
                        monster_inventory : tableData[1].rows[0].monster_inventory,
                        equipment_inventory : tableData[1].rows[0].equipment_inventory,
                        item_inventory : tableData[1].rows[0].item_inventory
                    };
    
                    user_data.inventory_info = inventory_info;
    
                    console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                    res.status(200).send(user_data);
                }
                // // 입장권 패키지
                // else if(index == 19){
                //     async.parallel([
                //         function(next){
                //             eos.getTableRows({
                //                 code: config.contract.main,
                //                 scope: user,
                //                 table: 'titem',
                //                 limit: 1,
                //                 lower_bound: 500200,
                //                 json: true
                //             }, function(err, item_1){
                //                 if(err){
                //                     next("Fail:Get Item Table:" + func);
                //                 }
                //                 else{
                //                     next(null, item_1);
                //                 }
                //             })
                //         },
                //         function(next){
                //             eos.getTableRows({
                //                 code: config.contract.main,
                //                 scope: user,
                //                 table: 'titem',
                //                 limit: 1,
                //                 lower_bound: 500210,
                //                 json: true
                //             }, function(err, item_2){
                //                 if(err){
                //                     next("Fail:Get Item Table:" + func);
                //                 }
                //                 else{
                //                     next(null, item_2);
                //                 }
                //             })
                //         },
                //         function(next){
                //             eos.getTableRows({
                //                 code: config.contract.main,
                //                 scope: user,
                //                 table: 'titem',
                //                 limit: 1,
                //                 lower_bound: 500220,
                //                 json: true
                //             }, function(err, item_3){
                //                 if(err){
                //                     next("Fail:Get Item Table:" + func);
                //                 }
                //                 else{
                //                     next(null, item_3);
                //                 }
                //             })
                //         },
                //         function(next){
                //             eos.getTableRows({
                //                 code: config.contract.main,
                //                 scope: user,
                //                 table: 'titem',
                //                 limit: 1,
                //                 lower_bound: 500230,
                //                 json: true
                //             }, function(err, item_4){
                //                 if(err){
                //                     next("Fail:Get Item Table:" + func);
                //                 }
                //                 else{
                //                     next(null, item_4);
                //                 }
                //             })
                //         },

                //     ], function(err, result){
                //         if(err){
                //             console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                //             res.status(200).send(err);
                //         }
                //         else{
                //             var temp_list = [];
                //             temp_list.push(result[0].rows[0]);
                //             temp_list.push(result[1].rows[0]);
                //             temp_list.push(result[2].rows[0]);
                //             temp_list.push(result[3].rows[0]);

                //             user_data.get_servant_list = [];
                //             user_data.get_monster_list = [];
                //             user_data.get_equipment_list = [];
                //             user_data.get_item_list = temp_list;

                //             var inventory_info = {
                //                 servant_inventory : tableData[1].rows[0].servant_inventory,
                //                 monster_inventory : tableData[1].rows[0].monster_inventory,
                //                 equipment_inventory : tableData[1].rows[0].equipment_inventory,
                //                 item_inventory : tableData[1].rows[0].item_inventory
                //             };
            
                //             user_data.inventory_info = inventory_info;
            
                //             console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                //             res.status(200).send(user_data);
                //         }
                //     })
                // }
                // // 언커먼 패키지
                // else if(index == 20){
                //     eos.getTableRows({
                //         code: config.contract.main,
                //         scope: user,
                //         table: 'titem',
                //         limit: 1,
                //         lower_bound: 500200,
                //         json: true
                //     }, function(err, result){
                //         if(err){
                //             console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                //             res.status(200).send(err);
                //         }
                //         else{
                //             user_data.get_servant_list = [];
                //             user_data.get_monster_list = [];
                //             user_data.get_monster_list.push(tableData[5].rows[tableData[5].rows.length - 1]);
                //             user_data.get_equipment_list = [];
                //             user_data.get_equipment_list.push(tableData[6].rows[tableData[6].rows.length - 1]);
                //             user_data.get_item_list = [];
                //             user_data.get_item_list.push(result.rows[0]);

                //             var inventory_info = {
                //                 servant_inventory : tableData[1].rows[0].servant_inventory,
                //                 monster_inventory : tableData[1].rows[0].monster_inventory,
                //                 equipment_inventory : tableData[1].rows[0].equipment_inventory,
                //                 item_inventory : tableData[1].rows[0].item_inventory
                //             };
            
                //             user_data.inventory_info = inventory_info;
            
                //             console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                //             res.status(200).send(user_data);
                //         }
                //     })
                // }
                // // 레어 패키지
                // else if(index == 21){
                //     async.parallel([
                //         function(next){
                //             eos.getTableRows({
                //                 code: config.contract.main,
                //                 scope: user,
                //                 table: 'titem',
                //                 limit: 1,
                //                 lower_bound: 500200,
                //                 json: true
                //             }, function(err, item_1){
                //                 if(err){
                //                     next("Fail:Get Item Table:" + func);
                //                 }
                //                 else{
                //                     next(null, item_1);
                //                 }
                //             })
                //         },
                //         function(next){
                //             eos.getTableRows({
                //                 code: config.contract.main,
                //                 scope: user,
                //                 table: 'titem',
                //                 limit: 1,
                //                 lower_bound: 500210,
                //                 json: true
                //             }, function(err, item_2){
                //                 if(err){
                //                     next("Fail:Get Item Table:" + func);
                //                 }
                //                 else{
                //                     next(null, item_2);
                //                 }
                //             })
                //         },
                //     ], function(err, result){
                //         if(err){
                //             console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                //             res.status(200).send(err);
                //         }
                //         else{
                //             var temp_list = [];
                //             temp_list.push(result[0].rows[0]);
                //             temp_list.push(result[1].rows[0]);

                //             user_data.get_servant_list = [];
                //             user_data.get_monster_list = [];
                //             user_data.get_monster_list.push(tableData[5].rows[tableData[5].rows.length - 1]);
                //             user_data.get_equipment_list = [];
                //             user_data.get_equipment_list.push(tableData[6].rows[tableData[6].rows.length - 1]);
                //             user_data.get_item_list = temp_list;

                //             var inventory_info = {
                //                 servant_inventory : tableData[1].rows[0].servant_inventory,
                //                 monster_inventory : tableData[1].rows[0].monster_inventory,
                //                 equipment_inventory : tableData[1].rows[0].equipment_inventory,
                //                 item_inventory : tableData[1].rows[0].item_inventory
                //             };
            
                //             user_data.inventory_info = inventory_info;
            
                //             console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                //             res.status(200).send(user_data);
                //         }
                //     })
                // }
                // // 유니크 패키지
                // else if(index == 22){
                //     async.parallel([
                //         function(next){
                //             eos.getTableRows({
                //                 code: config.contract.main,
                //                 scope: user,
                //                 table: 'titem',
                //                 limit: 1,
                //                 lower_bound: 500200,
                //                 json: true
                //             }, function(err, item_1){
                //                 if(err){
                //                     next("Fail:Get Item Table:" + func);
                //                 }
                //                 else{
                //                     next(null, item_1);
                //                 }
                //             })
                //         },
                //         function(next){
                //             eos.getTableRows({
                //                 code: config.contract.main,
                //                 scope: user,
                //                 table: 'titem',
                //                 limit: 1,
                //                 lower_bound: 500210,
                //                 json: true
                //             }, function(err, item_2){
                //                 if(err){
                //                     next("Fail:Get Item Table:" + func);
                //                 }
                //                 else{
                //                     next(null, item_2);
                //                 }
                //             })
                //         },
                //         function(next){
                //             eos.getTableRows({
                //                 code: config.contract.main,
                //                 scope: user,
                //                 table: 'titem',
                //                 limit: 1,
                //                 lower_bound: 500220,
                //                 json: true
                //             }, function(err, item_3){
                //                 if(err){
                //                     next("Fail:Get Item Table:" + func);
                //                 }
                //                 else{
                //                     next(null, item_3);
                //                 }
                //             })
                //         },
                //     ], function(err, result){
                //         if(err){
                //             console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                //             res.status(200).send(err);
                //         }
                //         else{
                //             var temp_list = [];
                //             temp_list.push(result[0].rows[0]);
                //             temp_list.push(result[1].rows[0]);
                //             temp_list.push(result[2].rows[0]);

                //             user_data.get_servant_list = [];
                //             user_data.get_monster_list = [];
                //             user_data.get_monster_list.push(tableData[5].rows[tableData[5].rows.length - 1]);
                //             user_data.get_equipment_list = [];
                //             user_data.get_equipment_list.push(tableData[6].rows[tableData[6].rows.length - 1]);
                //             user_data.get_item_list = temp_list;

                //             var inventory_info = {
                //                 servant_inventory : tableData[1].rows[0].servant_inventory,
                //                 monster_inventory : tableData[1].rows[0].monster_inventory,
                //                 equipment_inventory : tableData[1].rows[0].equipment_inventory,
                //                 item_inventory : tableData[1].rows[0].item_inventory
                //             };
            
                //             user_data.inventory_info = inventory_info;
            
                //             console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                //             res.status(200).send(user_data);
                //         }
                //     })
                // }
                // // 레전드 패키지
                // else if(index == 23){
                //     async.parallel([
                //         function(next){
                //             eos.getTableRows({
                //                 code: config.contract.main,
                //                 scope: user,
                //                 table: 'titem',
                //                 limit: 1,
                //                 lower_bound: 500200,
                //                 json: true
                //             }, function(err, item_1){
                //                 if(err){
                //                     next("Fail:Get Item Table:" + func);
                //                 }
                //                 else{
                //                     next(null, item_1);
                //                 }
                //             })
                //         },
                //         function(next){
                //             eos.getTableRows({
                //                 code: config.contract.main,
                //                 scope: user,
                //                 table: 'titem',
                //                 limit: 1,
                //                 lower_bound: 500210,
                //                 json: true
                //             }, function(err, item_2){
                //                 if(err){
                //                     next("Fail:Get Item Table:" + func);
                //                 }
                //                 else{
                //                     next(null, item_2);
                //                 }
                //             })
                //         },
                //         function(next){
                //             eos.getTableRows({
                //                 code: config.contract.main,
                //                 scope: user,
                //                 table: 'titem',
                //                 limit: 1,
                //                 lower_bound: 500220,
                //                 json: true
                //             }, function(err, item_3){
                //                 if(err){
                //                     next("Fail:Get Item Table:" + func);
                //                 }
                //                 else{
                //                     next(null, item_3);
                //                 }
                //             })
                //         },
                //         function(next){
                //             eos.getTableRows({
                //                 code: config.contract.main,
                //                 scope: user,
                //                 table: 'titem',
                //                 limit: 1,
                //                 lower_bound: 500230,
                //                 json: true
                //             }, function(err, item_4){
                //                 if(err){
                //                     next("Fail:Get Item Table:" + func);
                //                 }
                //                 else{
                //                     next(null, item_4);
                //                 }
                //             })
                //         },
                //     ], function(err, result){
                //         if(err){
                //             console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                //             res.status(200).send(err);
                //         }
                //         else{
                //             var temp_list = [];
                //             temp_list.push(result[0].rows[0]);
                //             temp_list.push(result[1].rows[0]);
                //             temp_list.push(result[2].rows[0]);
                //             temp_list.push(result[3].rows[0]);

                //             user_data.get_servant_list = [];
                //             user_data.get_monster_list = [];
                //             user_data.get_monster_list.push(tableData[5].rows[tableData[5].rows.length - 1]);
                //             user_data.get_equipment_list = [];
                //             user_data.get_equipment_list.push(tableData[6].rows[tableData[6].rows.length - 1]);
                //             user_data.get_item_list = temp_list;

                //             var inventory_info = {
                //                 servant_inventory : tableData[1].rows[0].servant_inventory,
                //                 monster_inventory : tableData[1].rows[0].monster_inventory,
                //                 equipment_inventory : tableData[1].rows[0].equipment_inventory,
                //                 item_inventory : tableData[1].rows[0].item_inventory
                //             };
            
                //             user_data.inventory_info = inventory_info;
            
                //             console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                //             res.status(200).send(user_data);
                //         }
                //     })
                // }
                else{
                    res.status(200).send("Fail:Store Buy Error.:Store Buy Error.");
                }

                // DB Login Count 추가
                poolCluster.getConnection(function(err, connection){
                    if(err){
                        console.log("Fail:Connect DB:" + func);
                    }
                    else{
                        var insert_data = {
                            user : user
                        }
                        var sql = "";
                        if(type == 2){
                            insert_data.amount = 500*count;
                            insert_data.type = "buyScroll";
                            insert_data.data = "Store";
                            sql = "INSERT INTO unt.log_utg SET ?";
                        }
                        else{
                            insert_data.amount = consumeEos;

                            if(index >= 8 || index <= 11){
                                insert_data.type = "ticket";
                            }
                            else if(index < 5)
                            {
                                insert_data.type = "buyInventory";
                            }
                            else{
                                insert_data.type = "package";
                            }

                            sql = "INSERT INTO unt.log_eos SET ?";
                        }
                        connection.query(sql, insert_data, function(err){
                            if(err){
                                console.log(config.color.red, 'user : ', user, ', func : ', func + "Currency Log update", ', time : ', new Date(new Date().toUTCString()));
                            }
                            connection.release();
                        });
                    }
                });
            }
        });
}

module.exports = Store;