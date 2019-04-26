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

    eos = Eos(config.eos_jungle);

    var user  = req.body.user;
    var type = req.body.type;
    var burn_idxs = req.body.burn_idxs;

    eos.getTableRows({
        code: config.contract.dev,
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
                code: config.contract.dev,
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
                         type : type
                    }

                    if(type = 1){
                        data.servantIndexList = burn_idxs;
                    }
                    if(type = 2){
                        data.monsterIndexList = burn_idxs;
                    }
                    if(type = 3){
                        data.equipmentIndexList = burn_idxs;
                    }

                    data.itemList = itemTable.rows;
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
    var i_idx = req.body.i_idx;
    var type = req.body.type;

    eos = Eos(config.eos_jungle);

    async.waterfall([
        function (callback) {
            eos.getTableRows({
                code: config.contract.dev,
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
            eos.getTableRows({
                    code : config.contract.dev,
                    scope : user,
                    table : 'tequipments',
                    lower_bound : i_idx,
                    limit : 1,
                    json : true
            }, function(err, itemTable){
                if(err){
                    console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                    res.status(200).send("Fail:Get Equipment Table:" + func);
                }
                else{
                    if(itemTable.rows[0].equipment.equipservantindex == result.index)
                    {
                        var data = {
                            servant_index : result.index,
                            equipment_slot : type,
                            equipment_index : i_idx
                        }
                        console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                        res.status(200).send(data);
                    }
                    else{
                        console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                        res.status(200).send("Fail:Index do not match:" + func);
                    }
                }
            });
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

module.exports = Servant;