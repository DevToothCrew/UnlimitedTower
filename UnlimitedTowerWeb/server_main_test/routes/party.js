Eos = require('eosjs')
config = require('../config/')

var async = require('async');

function Party() {}

/**
 * Formation Setting
 * 
 * @param req 
 * @param res 
 */
Party.saveParty = function(req, res){
    var func = 'saveParty';

    var user = req.body.user;
    var index = req.body.index;

    eos = Eos(config.eos);

    async.waterfall([
        function(callback){
            eos.getTableRows({
                code: config.contract.main,
                scope: user,
                table: 'tpartylist',
                lower_bound: index,
                limit: 1,
                json: true
            }, function (err, partyTable) {
                if (err) {
                    callback("Fail:Get Table:" + func);
                }
                else {
                    callback(null, partyTable.rows[0]);
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
 *  Set Party - Full node
 * 
 * @param req
 * @param res
 */
Party.mainSetParty = function(req, res){
    var func = 'mainSetParty';

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

        if(action.account == 'untowermain1' && action.name == 'resultparty' && action.data._from == 'untowermain1' && action.data._to == user){
            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            var split_data = action.data._party_info.split(':');

            var data = {}
            if(action.data._who == user){
                data.index = split_data[0];
                data.state = split_data[1];
                data.servant_list = action.data._servant_list;
                data.monster_list = action.data._monster_list;   
                
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

module.exports = Party;