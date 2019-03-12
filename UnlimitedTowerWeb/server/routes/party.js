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
Party.setFormation = function(req, res){
    var func = 'setFormation';

    var user = req.body.user;
    var index = req.body.index;
    var party = req.body.party;

    eos = Eos(config.eos);

    async.waterfall([
        function(callback){
            eos.getTableRows({
                code : config.contract.main,
                scope : user,
                table : 'cparty',
                lower_bound : index,
                limit : 1,
                json : true
            }, function(err, old_data){
                if(err){
                    callback(err);
                }
                else{
                    callback(null, old_data);
                }
            });
        },
        function(old_data, callback){
            var count = 0;

            var timer = setInterval(function(){
                eos.getTableRows({
                    code : config.contract.main,
                    scope : user,
                    table : 'cparty',
                    lower_bound : index,
                    limit : 1,
                    json : true
                }, function(err, new_data){
                    if(err){
                        callback(err);
                    }
                    else{
                        var flag = false;
                        for(var i = 0; i < 10; i++){
                            if(old_data.rows[0].party[i] != new_data.rows[0].party[i]){
                                flag = true;
                                break;
                            }
                        }

                        if(flag){
                            clearInterval(timer);
                            callback(null, new_data.rows[0]);
                        }
                        else{
                            count += 1;
                            if(count >= 5){
                                clearInterval(timer);
                                console.log("Timer Time Out!");
                                callback('error');
                            }
                        }
                    }
                });
            }, 3000);
        }
    ],
    function(err, result){
        if(err){
            console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', Date.now());
            res.status(200).send("Fail to set formation.");
        }
        else{
            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', Date.now());
            res.status(200).send(result);
        }
    });
}

module.exports = Party;