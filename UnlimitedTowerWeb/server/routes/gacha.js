Eos = require('eosjs')
config = require('../config/')

var async = require('async');

function Gacha() {}

/**
 * Gacha
 * 
 * @param req 
 * @param res 
 */
Gacha.gacha = function(req, res){
    var func = 'gacha';

    var user = req.body.user;
    eos = Eos(config.eos);

    async.waterfall([
        function(callback){
            eos.getTableRows({
                code : config.contract.main,
                scope : config.contract.main,
                table : 'cgacharesult',
                lower_bound : user,
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
                    scope : config.contract.main,
                    table : 'cgacharesult',
                    lower_bound : user,
                    limit : 1,
                    json : true
                }, function(err, new_data){
                    if(err){
                        callback(err);
                    }
                    else{
                        var table = "";
                        var data = {};
                        // 최초 갓챠로 이전 데이터가 비어 있을 경우
                        if(old_data.rows.length == 0){
                            // 새로운 데이터의 길이가 0이 아니면, 데이터가 추가 된 것이므로 처리해준다.
                            if(new_data.rows.length != 0){
                                if(new_data.rows[0].result.type == 1){
                                    table = "cservant";
                                    data.result_type = 1;
                                }
                                if(new_data.rows[0].result.type == 2){
                                    table = "cmonster";
                                    data.result_type = 2;
                                }
                                if(new_data.rows[0].result.type == 3){
                                    table = "citem";
                                    data.result_type = 3;
                                }
                                eos.getTableRows({
                                    code : config.contract.main,
                                    scope : user,
                                    table : table,
                                    lower_bound : new_data.rows[0].result.index,
                                    limit : 1,
                                    json : true
                                }, function(err, result){
                                    if(err){
                                        clearInterval(timer);
                                        callback("Gacha is success. But get data Error.");
                                    }
                                    else{
                                        clearInterval(timer);
                                        data.data = result.rows[0];
                                        callback(null, data);
                                    }
                                });
                            }
                            // 새로운 데이터의 길이가 0이면 다시 타이머를 돌려준다.
                            else{
                                count += 1;
                                if(count >= 5){
                                    clearInterval(timer);
                                    console.log("Timer Time Out!");
                                    callback('error');
                                }
                            }
                        }
                        // 이전에 갓챠를 했던 기록이 었어서 이전 데이터가 존재 할 경우
                        else{
                            if(new_data.rows[0].result.type == 1){
                                table = "cservant";
                                data.result_type = 1;
                            }
                            if(new_data.rows[0].result.type == 2){
                                table = "cmonster";
                                data.result_type = 2;
                            }
                            if(new_data.rows[0].result.type == 3){
                                table = "citem";
                                data.result_type = 3;
                            }
                            //타입이 다르면 새로운 결과가 적용 된 것이므로 데이터 검색해서 전송
                            if(old_data.rows[0].result.type != new_data.rows[0].result.type){
                                eos.getTableRows({
                                    code : config.contract.main,
                                    scope : user,
                                    table : table,
                                    lower_bound : new_data.rows[0].result.index,
                                    limit : 1,
                                    json : true
                                }, function(err, result){
                                    if(err){
                                        clearInterval(timer);
                                        callback("Gacha is success. But get data Error.");
                                    }
                                    else{
                                        clearInterval(timer);
                                        data.data = result.rows[0];
                                        callback(null, data);
                                    }
                                });
                            }
                            // 타입이 같으면 다시 한번 인덱스로 데이터 변경 확인
                            else{
                                // 타입이 같은데, 인덱스가 다르면 새로운 데이터가 적용 된 것이므로 갓챠 결과 검색해서 전송
                                if(old_data.rows[0].result.index != new_data.rows[0].result.index){
                                    eos.getTableRows({
                                        code : config.contract.main,
                                        scope : user,
                                        table : table,
                                        lower_bound : new_data.rows[0].result.index,
                                        limit : 1,
                                        json : true
                                    }, function(err, result){
                                        if(err){
                                            clearInterval(timer);
                                            callback("Gacha is success. But get data Error.");
                                        }
                                        else{
                                            clearInterval(timer);
                                            data.data = result.rows[0];
                                            callback(null, data);
                                        }
                                    });
                                }
                                // 타입이 같고, 인덱스도 같으면 적용되지 않은 것이므로 타이머를 다시 시작
                                else{
                                    count += 1;
                                    if(count >= 10){
                                        clearInterval(timer);
                                        console.log("Timer Time Out!");
                                        callback('error');
                                    }
                                }
                            }
                        }
                    }
                });
            }, 1000);
        }
    ],
    function(err, result){
        if(err){
            console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            res.status(200).send("Fail to get gacha data");
        }
        else{
            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            res.status(200).send(result);
        }
    });
}

/**
 * Pre Sale Gacha
 * 
 * @param req 
 * @param res 
 */
Gacha.pregacha = function(req, res){
    var func = 'pregacha';

    var user = req.body.user;
    eos = Eos(config.eos);

    async.waterfall([
        function(callback){
            eos.getTableRows({
                code : config.contract.main,
                scope : config.contract.main,
                table : 'tgacharesult',
                lower_bound : user,
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
                    scope : config.contract.main,
                    table : 'tgacharesult',
                    lower_bound : user,
                    limit : 1,
                    json : true
                }, function(err, new_data){
                    if(err){
                        callback(err);
                    }
                    else{
                        var table = "";
                        var data = {};
                        // 최초 갓챠로 이전 데이터가 비어 있을 경우
                        if(old_data.rows.length == 0){
                            // 새로운 데이터의 길이가 0이 아니면, 데이터가 추가 된 것이므로 처리해준다.
                            if(new_data.rows.length != 0){
                                if(new_data.rows[0].result.type == 1){
                                    table = "preservant";
                                    data.result_type = 1;
                                }
                                if(new_data.rows[0].result.type == 2){
                                    table = "premonster";
                                    data.result_type = 2;
                                }
                                if(new_data.rows[0].result.type == 3){
                                    table = "preitem";
                                    data.result_type = 3;
                                }
                                eos.getTableRows({
                                    code : config.contract.main,
                                    scope : user,
                                    table : table,
                                    lower_bound : new_data.rows[0].result.index,
                                    limit : 1,
                                    json : true
                                }, function(err, result){
                                    if(err){
                                        clearInterval(timer);
                                        callback("Gacha is success. But get data Error.");
                                    }
                                    else{
                                        clearInterval(timer);
                                        data.data = result.rows[0];
                                        callback(null, data);
                                    }
                                });
                            }
                            // 새로운 데이터의 길이가 0이면 다시 타이머를 돌려준다.
                            else{
                                count += 1;
                                if(count >= 5){
                                    clearInterval(timer);
                                    console.log("Timer Time Out!");
                                    callback('error');
                                }
                            }
                        }
                        // 이전에 갓챠를 했던 기록이 었어서 이전 데이터가 존재 할 경우
                        else{
                            if(new_data.rows[0].result.type == 1){
                                table = "preservant";
                                data.result_type = 1;
                            }
                            if(new_data.rows[0].result.type == 2){
                                table = "premonster";
                                data.result_type = 2;
                            }
                            if(new_data.rows[0].result.type == 3){
                                table = "preitem";
                                data.result_type = 3;
                            }
                            //타입이 다르면 새로운 결과가 적용 된 것이므로 데이터 검색해서 전송
                            if(old_data.rows[0].result.type != new_data.rows[0].result.type){
                                eos.getTableRows({
                                    code : config.contract.main,
                                    scope : user,
                                    table : table,
                                    lower_bound : new_data.rows[0].result.index,
                                    limit : 1,
                                    json : true
                                }, function(err, result){
                                    if(err){
                                        clearInterval(timer);
                                        callback("Gacha is success. But get data Error.");
                                    }
                                    else{
                                        clearInterval(timer);
                                        data.data = result.rows[0];
                                        callback(null, data);
                                    }
                                });
                            }
                            // 타입이 같으면 다시 한번 인덱스로 데이터 변경 확인
                            else{
                                // 타입이 같은데, 인덱스가 다르면 새로운 데이터가 적용 된 것이므로 갓챠 결과 검색해서 전송
                                if(old_data.rows[0].result.index != new_data.rows[0].result.index){
                                    eos.getTableRows({
                                        code : config.contract.main,
                                        scope : user,
                                        table : table,
                                        lower_bound : new_data.rows[0].result.index,
                                        limit : 1,
                                        json : true
                                    }, function(err, result){
                                        if(err){
                                            clearInterval(timer);
                                            callback("Gacha is success. But get data Error.");
                                        }
                                        else{
                                            clearInterval(timer);
                                            data.data = result.rows[0];
                                            callback(null, data);
                                        }
                                    });
                                }
                                // 타입이 같고, 인덱스도 같으면 적용되지 않은 것이므로 타이머를 다시 시작
                                else{
                                    count += 1;
                                    if(count >= 60){
                                        clearInterval(timer);
                                        console.log("Timer Time Out!");
                                        callback('error');
                                    }
                                }
                            }
                        }
                    }
                });
            }, 1000);
        }
    ],
    function(err, result){
        if(err){
            console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            res.status(200).send("Fail to get gacha data");
        }
        else{
            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            res.status(200).send(result);
        }
    });
}

/**
 *  Pre Gacha
 * 
 * @param req
 * @param res
 */
Gacha.mainPreGacha = function(req, res){
    var func = 'mainPreGacha';

    var user = req.body.user;

    const MongoOplog = require('mongo-oplog');
    const oplog = MongoOplog('mongodb://devTooth:U0S91ydQ0uYPSaOqHz3T@eos-sg-00-00-mongo-01.itam.games:27017/local?replicaSet=eos-01&authSource=admin', 
        { ns: 'EOS.action_traces', reconnectTries: 60, reconnectInterval: 1000 });
    
    oplog.tail();

    var count = 0;
    var timer = setInterval(function(){
        if(count == 60){
            clearInterval(timer);
            oplog.stop();
            res.status(200).send("ERR:Time Out");
        }
        else{
            count++;
        }
    }, 1000);

    oplog.on('insert', doc => {
        const actionInfo = doc.o;
        const action = actionInfo.act;

        if(action.account == 'untowermain1' && action.name == 'resultgacha' && action.data._from == 'untowermain1' && action.data._to == user){
            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            var temp = action.data._result.split(':');
            var data = { result : temp[0] }
            if(data.result == 'ser'){
                data.index = temp[1];
                data.id = temp[2];
                data.b_str = temp[3];
                data.b_dex = temp[4];
                data.b_int = temp[5];
                data.token = temp[6];

            }
            else if(data.result == 'mon'){
                data.index = temp[1];
                data.id = temp[2];
                data.grade = temp[3];
                data.b_str = temp[4];
                data.b_dex = temp[5];
                data.b_int = temp[6];
                data.token = temp[7];

            }
            else{
                data.index = temp[1];
                data.id = temp[2];
                data.type = temp[3];
                data.tier = temp[4];
                data.job = temp[5];
                data.grade = temp[6];
                data.main_status = temp[7];
                data.token = temp[8];
            }
            res.status(200).send(data);
            oplog.stop();
            clearInterval(timer);
        }
    });

    oplog.on('error', error => {
        console.log(config.color.red, 'user : ', user, ', func : ', func, ' err : ', error, ' time : ', new Date(new Date().toUTCString()));
    });
}
module.exports = Gacha;