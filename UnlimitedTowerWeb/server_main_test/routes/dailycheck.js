Eos = require('eosjs')
config = require('../config')

var async = require('async');

function Dailycheck() {}

/**
 * Dailycheck
 * 
 * @param req
 * @param res
 */
Dailycheck.check = function(req, res){

    var func = "dailyCheck";

    eos = Eos(config.eos);
    var count = 0;

    var user = req.body.user;
    var day_count= req.body.day; 
   // var index = req.body.index;
   var timer = setInterval(function(){
    async.waterfall([
        function(callback){
            eos.getTableRows({
                code: config.contract.main,
                scope: config.contract.main,
                table: 'tdaily',
                lower_bound: user, 
                limit: 1,
                json: true
            }, function (err, newTable) {
                if (err) {
                    callback("Fail:Get Table:" + func);
                }
                else {
                    if (newTable.rows[0].length != 0) {
                        if (newTable.rows[0].user == user) {
                            callback(null, newTable.rows[0]);
                        }
                    }
                    else {
                        callback("Fail:Get Table:" + func);
                    }
                }
            });
        }
    ],
    function(err, result){
        if(err){
            clearInterval(timer);
            console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            res.status(200).send(err);
        }
        else {
            if (day_count == result.total_day) {
                if(count > 3)
                {
                    clearInterval(timer);
                    console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                    res.status(200).send("Fail:sync");
                }
                count++;
                console.log(config.color.yellow, "Waiting for sync For ", user, "'s Daily check");
            }
            else {
                var data = {
                    check_day: result.total_day,
                    check_time: result.check_time
                }
                clearInterval(timer);
                console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                res.status(200).send(data);
            }
        }
    })
    }, 1000);
}

/**
 *  DailyCheck - Full node
 * 
 * @param req
 * @param res
 */
Dailycheck.mainDailyCheck = function(req, res){
    var func = 'mainDailyCheck';

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

        if(action.account == 'untowermain1' && action.name == 'dailycheck' && action.data._from == 'untowermain1' && action.data._to == user){
            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
  
            var data = {}
            if(action.data._who == user){
                data._type = action.data._type;

                var daily_check_list = action.data._list.split(',');
                var len = daily_check_list.length;
                var split_data = "";

                for(var i = 0; i < len ; i++){
                    split_data = daily_check_list[i].split(":");
                    data._list[i].total_day = split_data[0];
                    data._list[i].check_time = split_data[1];
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

module.exports = Dailycheck;