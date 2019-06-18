Eos = require('eosjs')
config = require('../config/')

var async = require('async');

function Servant() {}

/**
 * Burn 
 * 
 * @param req
 * @param res
 */
Servant.burn = function(req, res){

    var func = "Burn"

    eos = Eos(config.eos);

    var user  = req.body.user;
    var type = req.body.type;
    var burn_idxs = req.body.burn_idxs;

    eos.getTableRows({
        code: config.contract.main,
        scope: user,
        table: 'titem',
        limit: 1000,
        json: true
    }, function (err, itemTable) {
        if (err) {
            console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            res.status(200).send("Fail:Get Item Table:" + func);
        }
        else {
            eos.getTableRows({
                code: config.contract.main,
                scope: user,
                table: 'accounts',
                json: true
            }, function (err, utgTable) {
                if (err) {
                    console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                    res.status(200).send("Fail:Get Account Table:" + func);
                }
                else {
                    var utg = '';

                    if (utgTable.rows.length == 0) {
                        utg = '0';
                    }
                    else {
                        var token = utgTable.rows[0].balance.split(" ");
                        token = token[0].split(".");
                        utg = token[0] + token[1];
                    }
                    var data = {
                    }

                    if(type == 1){
                        data.servantIndexList = burn_idxs;
                        data.itemList = [];

                        for(var i=0; i < itemTable.rows.length; i++){
                            if(itemTable.rows[i].id == 510010 || itemTable.rows[i].id == 510020 || itemTable.rows[i].id == 510030 || itemTable.rows[i].id == 510040){
                                data.itemList.push(itemTable.rows[i]);
                            }
                        }
                    }
                    else if (type == 2) {
                        data.monsterIndexList = burn_idxs;
                        data.itemList = [];
                    }
                    else{
                        data.equipmentIndexList = burn_idxs;
                        data.itemList = [];
                    }
                    
                    data.utg = utg;

                    console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                    res.status(200).send(data);
                }
            });
        }
    });
}

/**
 * Servant Item Equip
 * 
 * @param req 
 * @param res 
 */
Servant.equip = function(req, res){
    var func = 'servantEquip';

    var user = req.body.user;
    var s_idx = req.body.s_idx;
    var type = req.body.type;
    var servant = req.body.servant;

    var count = 0 ;

    eos = Eos(config.eos);

    async.waterfall([
        function (callback) {
            var timer = setInterval(function(){
            eos.getTableRows({
                code: config.contract.main,
                scope: user,
                table: 'tservant',
                lower_bound: s_idx,
                limit: 1,
                json: true
            }, function (err, newTable) {
                if (err) {
                    callback("Fail:Get Table:" + func);
                }
                else {
                    if (newTable.rows[0].servant != servant) {
                        clearInterval(timer);
                        callback(null, newTable.rows[0]);
                    }
                    else
                    {
                        if(count > 10)
                        {
                            clearInterval(timer);
                            callback("error");
                        }
                        count++;
                        console.log(config.color.yellow, "Waiting for sync For ", user, "'s Equip");
                    }
                }
            })
        },500);
        }
    ],
        function (err, result) {
            if (err) {
                console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                res.status(200).send(err);
            }
            else {
                var data = {
                    servant_index: result.index,
                    equipment_slot: type,
                    equipment_index: result.servant.equip_slot[type]
                }
                console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                res.status(200).send(data);
            }
            
        });
}

/**
 *  Servant Burn - Full node
 * 
 * @param req
 * @param res
 */
Servant.mainServantBurn = function(req, res){
    var func = 'mainServantBurn';

    var user = req.body.user;

    const MongoOplog = require('mongo-oplog');
    const oplog = MongoOplog(config.full_node.main_01, { ns: 'EOS.action_traces', reconnectTries: 60, reconnectInterval: 1000 }); 
    
    oplog.tail();

    var count = 0;
    var timer = setInterval(function(){
        if(count == 60){
            clearInterval(timer);
            oplog.stop();
            res.status(200).send("Fail:Time Out:" + func);
        }
        else{
            count++;
        }
    }, 1000);

    oplog.on('insert', doc => {
        const actionInfo = doc.o;
        const action = actionInfo.act;

        if(action.account == 'untowermain1' && action.name == 'servantburn' && action.data._from == 'untowermain1' && action.data._to == user){
            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
  
            var data = {}
            if(action.data._who == user){
                data._type = action.data._type;

                var burn_servant_list = action.data._list.split(',');
                var len = burn_servant_list.length;
                var split_data = "";

                for(var i = 0; i < len ; i++){
                    split_data = burn_servant_list[i].split(":");
                    data._list[i].servant_index = split_data[0];
                    data._list[i].servant_id = split_data[1];
                    data._list[i].item_id = split_data[2];
                    data._list[i].item_type = split_data[3];
                    data._list[i].item_list.item_index = split_data[4];
                    data._list[i].item_list.item_count = split_data[5];
                }
                
                res.status(200).send(data);
                oplog.stop();
                clearInterval(timer);
            }
            else{
                res.status(200).send("Fail:The user does not match:" + func);
                oplog.stop();
                clearInterval(timer);
            }
        }
    });

    oplog.on('error', error => {
        res.status(200).send("Fail:Get Transaction Error:" + func);
        console.log(config.color.red, 'user : ', user, ', func : ', func, ' err : ', error, ' time : ', new Date(new Date().toUTCString()));
    });
}



/**
 * Servant LimitBreak;
 * 
 * @param req
 * @param res
 */
Servant.limitbreak = function (req, res) {

    var func = "limitBreak";

    eos = Eos(config.eos);

    var user = req.body.user;
    var index = req.body.index;
    var item_id = req.body.item_id;

    async.waterfall([
        function (callback) {
            eos.getTableRows({
                code: config.contract.main,
                scope: user,
                table: 'tservant',
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
        function (err, result) {
            if (err) {
                console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                res.status(200).send(err);
            }
            else {
                eos.getTableRows({
                    code: config.contract.main,
                    scope: user,
                    table: 'titem',
                    lower_bound: item_id,
                    iimit: 1,
                    json: true
                }, function (err, itemTable) {
                    if (err) {
                        console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                        res.status(200).send("Fail:Get Account Table:" + func);
                    }
                    else {
                        var item = {}
                        if (itemTable.rows.length == 0) {
                            item.id = item_id;
                            item.type = 0;
                            item.item_list = [];
                        }
                        else {
                            if (itemTable.rows[0].id != item_id) {
                                item.id = item_id;
                                item.type = 0;
                                item.item_list = [];
                            }
                            else {
                                item = itemTable.rows[0];
                            }
                        }
                        var data = {}
                        data.servant = result;
                        data.item = item;

                        console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                        res.status(200).send(data);
                    }
                });
            }
        });
}


module.exports = Servant;