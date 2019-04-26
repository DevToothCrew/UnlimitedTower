Eos = require('eosjs')
config = require('../config/')

var async = require('async');

function Tower() { }

/**
 * Battle Reward
 * 
 * @param req
 * @param res
 */
Tower.battleReward = function (req, res) {
    var func = 'battleReward';

    var user = req.body.user;
    eos = Eos(config.eos_jungle);

    var count = 0;

    var timer = setInterval(function () {
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
                    clearInterval(timer);

                    eos.getTableRows({
                        code: config.contract.dev,
                        scope: config.contract.dev,
                        table: 'tclearreward',
                        lower_bound: user,
                        limit: 1,
                        json: true
                    }, function(err, reward){
                        if(err){
                            console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                            res.status(200).send("Fail:Get Reward Table:" + func);
                        }
                        else{
                            var data = {
                                reward_money : reward.rows[0].reward_money,
                                get_rank_exp : reward.rows[0].get_rank_exp,
                                get_char_exp_list : reward.rows[0].get_char_exp_list,
                                get_servant_list : reward.rows[0].get_servant_list,
                                get_monster_list : reward.rows[0].get_monster_list,
                                get_equipment_list : reward.rows[0].get_equipment_list,
                                get_item_list : reward.rows[0].get_item_list
                            }
                            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                            res.status(200).send(data);
                        }
                    })
                }
                else {
                    if (count > 10) {
                        clearInterval(timer);
                        console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                        res.status(200).send("Fail:Time Out:" + func);
                    }
                    count++;
                }
            }
        })
    }, 2000);
}

module.exports = Tower;