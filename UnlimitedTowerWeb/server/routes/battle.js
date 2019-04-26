Eos = require('eosjs')
config = require('../config/')

var async = require('async');

function Battle() { }

/**
 * Battle Action 
 * 
 * @param req
 * @param res
 */
Battle.battleAction = function (req, res) {
    var func = 'battleAction';

    var user = req.body.user;
    eos = Eos(config.eos_jungle);

    async.waterfall([
        function (callback) {
            eos.getTableRows({
                code: config.contract.dev,
                scope: config.contract.dev,
                table: 'tbattleact',
                lower_bound: user,
                limit: 1,
                json: true
            }, function (err, newTable) {
                if (err) {
                    console.error("Fail:Get Table:" + func);
                    callback("Fail:Get Table:" + func);
                }
                else {
                    callback(null, newTable.rows[0]);
                }
            })
        }
    ],
    function (err, result) {
        if (err) {
            console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            res.status(200).send(err);
        }
        else {
            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            res.status(200).send(result);
        }
    });
}

/**
 * Get Battle Info
 * 
 * @param req
 * @param res
 */
Battle.getBattle = function (req, res) {
    var func = 'getBattle';

    var user = req.body.user;
    
    eos = Eos(config.eos_jungle);

    eos.getTableRows({
        code: config.contract.dev,
        scope: config.contract.dev,
        table: 'tstgstate',
        lower_bound: user,
        limit: 1,
        json: true
    }, function (err, stageData) {
        if (err) {
            console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            res.status(200).send("Fail:Get Table:" + func);
        }
        else {
            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            res.status(200).send(stageData.rows[0]);
        }
    });
}

/**
 * Battle Start
 * 
 * @param req
 * @param res
 */
Battle.battleStart = function (req, res) {
    var func = 'battleStart';

    var user = req.body.user;
    eos = Eos(config.eos_jungle);

    eos.getTableRows({
        code: config.contract.dev,
        scope: config.contract.dev,
        table: 'tstgstate',
        lower_bound: user,
        limit: 1,
        json: true
    }, function (err, newTable) {
        if (err) {
            console.error("Fail:Get Table:" + func);
            res.status(200).send("Fail:Get Table:" + func);
        }
        else {
            if (newTable.rows.length != 0 && newTable.rows[0].user == user) {
                if (newTable.rows[0].turn != 0) {
                    console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                    res.status(200).send("Fail:Turn Out:" + func);
                }
                else {
                    console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                    res.status(200).send(newTable.rows[0]);
                }
            }
        }
    })
}

/**
 * Battle Reward
 * 
 * @param req
 * @param res
 */
Battle.battleReward = function (req, res) {
    var func = 'battleReward';

    var user = req.body.user;
    eos = Eos(config.eos_jungle);

    eos.getTableRows({
        code: config.contract.dev,
        scope: config.contract.dev,
        table: 'tuserauth',
        lower_bound: user,
        limit: 1,
        json: true
    }, function (err, newTable) {
        if (err) {
            console.error("Fail:Get Table:" + func);
            res.status(200).send("Fail:Get Table:" + func);
        }
        else {
            if (newTable.rows[0].state == 2) {
                eos.getTableRows({
                    code: config.contract.dev,
                    scope: config.contract.dev,
                    table: 'tclearreward',
                    lower_bound: user,
                    limit: 1,
                    json: true
                }, function (err, reward) {
                    if (err) {
                        console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                        res.status(200).send("Fail:Get Reward Table:" + func);
                    }
                    else {
                        var data = {
                            reward_money: reward.rows[0].reward_money,
                            get_rank_exp: reward.rows[0].get_rank_exp,
                            get_char_exp_list: reward.rows[0].get_char_exp_list,
                            get_servant_list: reward.rows[0].get_servant_list,
                            get_monster_list: reward.rows[0].get_monster_list,
                            get_equipment_list: reward.rows[0].get_equipment_list,
                            get_item_list: reward.rows[0].get_item_list
                        }
                        console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                        res.status(200).send(data);
                    }
                })
            }
            else {
                console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                res.status(200).send("Fail:Time Out:" + func);
            }
        }
    });
}

Battle.stageEscape = function(req, res){
    
    var func = "stageEscape";

    var user = req.body.user;

    eos.getTableRows({
        code: config.contract.dev,
        scope: config.contract.dev,
        table: 'tuserauth',
        lower_bound: user,
        limit: 1,
        json: true
    }, function (err, newTable) {
        if (err) {
            clearInterval(timer);
            console.error("Fail:Get Table:" + func);
            res.status(200).send("Fail:Get Table:" + func);
        }
        else {
            if (newTable.rows[0].state == 2) {
                console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                var data = {
                    success: true
                }
                res.status(200).send(data);
            }
            else {
                console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                res.status(200).send("Fail:Time Out:" + func);
            }
        }
    })
}

/**
 *  Battle State - Full node
 * 
 * @param req
 * @param res
 */
Battle.mainBattleState = function(req, res){
    var func = 'mainBattleState';

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

        if(action.account == 'untowermain1' && action.name == 'battlestate' && action.data._from == 'untowermain1' && action.data._to == user){
            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));

            if(action.data._who == user){
                eos.getTableRows({
                    code: config.contract.dev,
                    scope: user,
                    table: 'tstgstate',
                    limit: 1,
                    json: true
                }, function (err, stageState) {
                    if (err) {
                        oplog.stop();
                        clearInterval(timer);
                        console.error("Fail:Get Table:" + func);
                        res.status(200).send("Fail:Get Table:" + func);
                    }
                    else {
                        res.status(200).send(stageState.rows[0]);
                        oplog.stop();
                        clearInterval(timer);
                    }
                });
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
 *  Battle Action - Full node
 * 
 * @param req
 * @param res
 */
Battle.mainBattleAction = function(req, res){
    var func = 'mainBattleAction';

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

        if(action.account == 'untowermain1' && action.name == 'battleaction' && action.data._from == 'untowermain1' && action.data._to == user){
            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
  
            var data = {}
            if(action.data._who == user){
                data.user = action.data._who;
                data.turn = action.data._turn;

                var len = action.data._action_data.length;
                var split_data = "";

                for(var i = 0; i < len ; i++){
                    split_data = action.data._action_data[i].split(":");
                    data.character_action_data[i].my_position = split_data[0];
                    data.character_action_data[i].action_type = split_data[1];
                    data.character_action_data[i].action_info_list.target_position = split_data[2];
                    data.character_action_data[i].action_info_list.avoid = split_data[3];
                    data.character_action_data[i].action_info_list.critical = split_data[4];
                    data.character_action_data[i].action_info_list.damage = split_data[5];
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


module.exports = Battle;