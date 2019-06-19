Eos = require('eosjs')
config = require('../config/')

var async = require('async');

function Monster() {}

/**
 * Monster Upgrade
 * 
 * @param req
 * @param res
 */
Monster.upgrade = function(req, res){

    var func = "monsterUpgrade";

    eos = Eos(config.eos);

    var user = req.body.user;
    var index = req.body.index;
    var sub_index = req.body.sub_index;
    var need_item = req.body.need_item;
    var upgrade = req.body.upgrade;

    var count = 0;

    var timer = setInterval(function(){
    async.waterfall([        
        function(callback){
            eos.getTableRows({
                code: config.contract.main,
                scope: user,
                table: 'tmonster',
                lower_bound: index,
                limit: 1,
                json: true
            }, function (err, newTable) {
                if (err) {
                    callback("Fail:Get Table:" + func);
                }
                else {
                    if (newTable.rows[0].length != 0) {
                        if (newTable.rows[0].index == index) {
                            if(upgrade == newTable.rows[0].monster.upgrade)
                            {
                                //한번만 더 읽고 보내준다
                                if(count >5)
                                {
                                    var success;
                                    clearInterval(timer);
                                    if(upgrade <= newTable.rows[0].monster.upgrade)
                                    {
                                        success = false;
                                    }
                                    else
                                    {
                                        success = true;
                                    }
                                    callback(null, success, newTable.rows[0]);
                                }
                                count++;
                                
                            }
                            else if(upgrade < newTable.rows[0].monster.upgrade)
                            {
                                var success = true;
                                callback(null, success,newTable.rows[0]);
                            }
                            else if(upgrade >  newTable.rows[0].monster.upgrade)
                            {
                                var success = false;
                                callback(null, success,newTable.rows[0]);
                            }
                        }
                        else {
                            callback("Fail:Get Table:" + func);
                        }
                    }
                    else {
                        callback("Fail:Get Table:" + func);
                    }
                }
            });
        }
    ],
    function(err, success, result){
        if(err){
            clearInterval(timer);
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
                    clearInterval(timer);
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
                    
                    var data = {}
                    // 강화 성공
                    // if((upgrade + 1) == result.monster.upgrade){
                    //     data.is_success = true;
                    // }
                    // // 강화 실패
                    // else{
                    //     data.is_success = false;
                    // }
                    data.is_success = success;
                    data.main_monster_data = result;
                    data.sub_monster_index = sub_index;
                    data.need_item_list = need_item;
                    data.utg = utg;
                    clearInterval(timer);
                    console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                    res.status(200).send(data);

                }
            });
        }
    })
    },1000);
}

/**
 *  Monster Burn - Full node
 * 
 * @param req
 * @param res
 */
Monster.mainMonsterBurn = function(req, res){
    var func = 'mainMonsterBurn';

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

        if(action.account == 'untowermain1' && action.name == 'monsterburn' && action.data._from == 'untowermain1' && action.data._to == user){
            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
  
            var data = {}
            if(action.data._who == user){
                data._type = action.data._type;

                var burn_monster_list = action.data._list.split(',');
                var len = burn_monster_list.length;
                var split_data = "";

                for(var i = 0; i < len ; i++){
                    split_data = burn_monster_list[i].split(":");
                    data._list[i].monster_index = split_data[0];
                    data._list[i].monster_id = split_data[1];
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

module.exports = Monster;