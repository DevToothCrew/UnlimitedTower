Eos = require('eosjs')
config = require('../config/')

var poolCluster = require('../lib/MySQLPoolCluster').PoolCluster;

var async = require('async');

function Gacha() {}

/**
 * TestNet Gacha
 * 
 * @param req 
 * @param res 
 */
Gacha.testGacha = function(req, res){
    var func = 'testGacha';

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
                    callback("Fail:Get Table:" + func);
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
                        clearInterval(timer);
                        callback("Fail:Get Table:" + func);
                    }
                    else{
                        var table = "";
                        var data = {};
                        // 최초 갓챠로 이전 데이터가 비어 있을 경우
                        if(old_data.rows.length == 0){
                            // 새로운 데이터의 길이가 0이 아니면, 데이터가 추가 된 것이므로 처리해준다.
                            if(new_data.rows.length != 0){
                                if(new_data.rows[0].result.type == 1){
                                    table = "tservant";
                                    data.result_type = 1;
                                }
                                if(new_data.rows[0].result.type == 2){
                                    table = "tmonster";
                                    data.result_type = 2;
                                }
                                if(new_data.rows[0].result.type == 3){
                                    table = "tequipments";
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
                                        callback("Fail:Success gacha but get data error:" + func);
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
                                if(count >= 20){
                                    clearInterval(timer);
                                    console.log("Fail:Time Out:" + func);
                                    callback("Fail:Time Out:" + func);
                                }
                            }
                        }
                        // 이전에 갓챠를 했던 기록이 었어서 이전 데이터가 존재 할 경우
                        else{
                            if(new_data.rows[0].result.type == 1){
                                table = "tservant";
                                data.result_type = 1;
                            }
                            if(new_data.rows[0].result.type == 2){
                                table = "tmonster";
                                data.result_type = 2;
                            }
                            if(new_data.rows[0].result.type == 3){
                                table = "tequipments";
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
                                        callback("Fail:Success gacha but get data error:" + func);
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
                                            callback("Fail:Success gacha but get data error:" + func);
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
                                    if(count >= 20){
                                        clearInterval(timer);
                                        console.log("Fail:Time Out:" + func);
                                        callback("Fail:Time Out:" + func);
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
            res.status(200).send(err);
        }
        else{
            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            res.status(200).send(result);

            poolCluster.getConnection(function(err, connection){
                if(err){
                    console.log("Fail:Connect DB:" + func);
                }
                else{
                    var type = "";
                    var type_index = 0;
                    var grade = 0;
                    if(result.result_type == 1){
                        type = "servant";
                        type_index = result.data.servant.id;
                    }
                    if(result.result_type == 2){
                        type = "monster";
                        grade = result.data.monster.grade;
                        type_index = result.data.monster.id;
                    }
                    if(result.result_type == 3){
                        type = "equipment";
                        grade = result.data.equipment.grade;
                        type_index = result.data.equipment.id;
                    }
                    var gachaLogData = {
                        user : user,
                        type : type,
                        grade : grade,
                        type_index : type_index
                    };
                    
                    var sql = "INSERT INTO unt.log_gacha SET ?";
                    connection.query(sql, gachaLogData, function(err){
                        if(err){
                            connection.release();
                            console.log(config.color.red, 'user : ', user, ', func : ', func + " GachaLog DB Insert", ', time : ', new Date(new Date().toUTCString()));
                        }
                    });

                    var eosLogData = {
                        user : user,
                        amount : 10000,
                        type : "gacha"
                    }
                    sql = "INSERT INTO unt.log_eos SET ?";
                    connection.query(sql, eosLogData, function(err){
                        if(err){
                            connection.release();
                            console.log(config.color.red, 'user : ', user, ', func : ', func + " EosLog DB Insert", ', time : ', new Date(new Date().toUTCString()));
                        }
                    });
                    sql = "INSERT INTO unt.log_user (user, login) VALUE (?, 1) ON DUPLICATE KEY UPDATE gacha=gacha+1";
                    connection.query(sql, user, function (err) {
                        if (err) {
                            console.log(config.color.red, 'user : ', user, ', func : ', func + " User gacha update", ', time : ', new Date(new Date().toUTCString()));
                        }
                        connection.release();
                    });
                }
            });
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
    const oplog = MongoOplog(config.full_node.main_01, { ns: 'EOS.action_traces', reconnectTries: 60, reconnectInterval: 1000 });
    
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

/**
 *  Main Gacha - Full node
 * 
 * @param req
 * @param res
 */
Gacha.mainGacha = function(req, res){
    var func = 'mainGacha';

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

        if(action.account == 'untowermain1' && action.name == 'resultgacha' && action.data._from == 'untowermain1' && action.data._to == user){
            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
            var split_data = action.data._result.split(':');

            var data = {}
            if(action.data._who == user){
                if(action.data._type == 'servant'){
                    data.result_type = 1;
                    data.index = split_data[0];
                    data.party_number = split_data[1];
                    data.servant.state = split_data[2];
                    data.servant.exp = split_data[3];
                    data.servant.id = split_data[4];
                    data.servant.level = split_data[5];
                    data.servant.grade = split_data[6];
                    data.servant.status.basic_str = split_data[7];
                    data.servant.status.basic_dex = split_data[8];
                    data.servant.status.basic_int = split_data[9];
                    data.servant.equip_slot = split_data[10];
                    data.servant.passive_skill = split_data[11];
                    data.servant.active_skill = split_data[12];
                }
    
                else if(action.data._type == 'monster'){
                    data.result_type = 2;
                    data.index = split_data[0];
                    data.party_number = split_data[1];
                    data.monster.id = split_data[2];
                    data.monster.state = split_data[3];
                    data.monster.exp = split_data[4];
                    data.monster.type = split_data[5];
                    data.monster.monster_class = split_data[6];
                    data.monster.grade = split_data[7];
                    data.monster.upgrade = split_data[8];
                    data.monster.level = split_data[9];
                    data.monster.status.basic_str = split_data[10];
                    data.monster.status.basic_dex = split_data[11];
                    data.monster.status.basic_int = split_data[12];
                    data.monster.passive_skill = split_data[13];
                    data.monster.active_skill = split_data[14];

                }
    
                else{
                    data.result_type = 3;
                    data.index = split_data[0];
                    data.equipment.id = split_data[1];
                    data.equipment.state = split_data[2];
                    data.equipment.type = split_data[3];
                    data.equipment.tier = split_data[4];
                    data.equipment.job = split_data[5];
                    data.equipment.grade = split_data[6];
                    data.equipment.upgrade = split_data[7];
                    data.equipment.value = split_data[8];
                    data.equipment.equipservantindex = equipservantindex[9];
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
 * Main Gacha - New
 * 
 * @param req 
 * @param res 
 */
Gacha.gacha = function(req, res){
    var func = 'gacha';

    var user = req.body.user;
    var type = req.body.type;
    var gacha_type = req.body.gacha_type;
    var table = "";

    if(type == "eos"){
        table = "tgacharesult";
    }
    else{
        table = "tgoldresult";
    }
    eos = Eos(config.eos);
    
    var count = 0;
    var timer = setInterval(function () {
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
                // Get EOS Info : 1
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
            function(next){
                eos.getTableRows({
                    code: config.contract.main,
                    scope: config.contract.main,
                    table: table,
                    lower_bound: user,
                    limit: 1,
                    json: true
                }, function(err, gacha){
                    if (err) {
                        next("Fail:Get Token Table:" + func);
                    }
                    else {
                        next(null, gacha);
                    }
                })
            }
        ], function(err, result_data){
            if(err){
                res.status(200).send("Fail:Get Table:" + func);
            }
            else{
                var table = "";
                var data = {};
                    
                // EOS Setting
                var uEos = result_data[1].rows[0].balance.split(" ");
                uEos = uEos[0].split(".");
                if (uEos[0] == '0' && uEos[1] == '0000') {
                    data.eos = '0';
                }
                else {
                    data.eos = uEos[0] + uEos[1];
                }

                // Token Setting
                if (result_data[0].rows.length == 0) {
                    data.utg = '0';
                }
                else {
                    var token = result_data[0].rows[0].balance.split(" ");
                    token = token[0].split(".");
                    data.utg = token[0] + token[1];
                }
                
                if (result_data[2].rows[0].result.type == 1) {
                    table = "tservant";
                }
                if (result_data[2].rows[0].result.type == 2) {
                    table = "tmonster";
                }
                if (result_data[2].rows[0].result.type == 3) {
                    table = "tequipments";
                }
                if(result_data[2].rows[0].result.type == 4){
                    table = "titem";
                }

                eos.getTableRows({
                    code: config.contract.main,
                    scope: user,
                    table: table,
                    lower_bound: result_data[2].rows[0].result.index,
                    limit: 1,
                    json: true
                }, function (err, table_result){
                    if (err) {
                        clearInterval(timer);
                        console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                        res.status(200).send("Fail:Success gacha but get data error:" + func);
                    }
                    else{
                        if(table_result.rows.length != 0){
                            clearInterval(timer);
                            var temp_list = [];
                            temp_list.push(table_result.rows[0]);
                            if (result_data[2].rows[0].result.type == 1) {
                                data.get_servant_list = temp_list;
                                data.get_monster_list = [];
                                data.get_equipment_list = [];
                                data.get_item_list = [];
                            }
                            if (result_data[2].rows[0].result.type == 2) {
                                data.get_servant_list = [];
                                data.get_monster_list = temp_list;;
                                data.get_equipment_list = [];
                                data.get_item_list = [];
                            }
                            if (result_data[2].rows[0].result.type == 3) {
                                data.get_servant_list = [];
                                data.get_monster_list = [];
                                data.get_equipment_list = temp_list;;
                                data.get_item_list = [];
                            }
                            if(result_data[2].rows[0].result.type == 4){
                                data.get_servant_list = [];
                                data.get_monster_list = [];
                                data.get_equipment_list = [];
                                data.get_item_list = temp_list;;
                            }
                            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                            res.status(200).send(data);

                            poolCluster.getConnection(function(err, connection){
                                if(err){
                                    console.log("Fail:Connect DB:" + func);
                                }
                                else{
                                    var gachaLogData = {
                                        user : user
                                    }
                                    
                                    if(result_data[2].rows[0].result.type == 1){
                                        gachaLogData.type = "servant";
                                        gachaLogData.type_index = table_result.rows[0].servant.id;
                                        gachaLogData.grade = table_result.rows[0].servant.grade;
                                    }
                                    if(result_data[2].rows[0].result.type == 2){
                                        gachaLogData.type = "monster";
                                        gachaLogData.type_index = table_result.rows[0].monster.id;
                                        gachaLogData.grade = table_result.rows[0].monster.grade;
                                    }
                                    if(result_data[2].rows[0].result.type == 3){
                                        gachaLogData.type = "equipment";
                                        gachaLogData.type_index = table_result.rows[0].equipment.id;
                                        gachaLogData.grade = table_result.rows[0].equipment.grade;
                                    }
                                    if(result_data[2].rows[0].result.type == 4){
                                        gachaLogData.type = "item";
                                        gachaLogData.type_index = table_result.rows[0].id;
                                        gachaLogData.grade = 0;
                                    }
                                    
                                    var sql = "INSERT INTO unt.log_gacha SET ?";
                                    connection.query(sql, gachaLogData, function(err){
                                        if(err){
                                            connection.release();
                                            console.log(config.color.red, 'user : ', user, ', func : ', func + " GachaLog DB Insert", ', time : ', new Date(new Date().toUTCString()));
                                        }
                                        else{
                                            var eosLogData = {
                                                user : user,
                                                amount : 10000,
                                                type : "gacha"
                                            }
                                            sql = "INSERT INTO unt.log_eos SET ?";
                                            connection.query(sql, eosLogData, function(err){
                                                if(err){
                                                    connection.release();
                                                    console.log(config.color.red, 'user : ', user, ', func : ', func + " EosLog DB Insert", ', time : ', new Date(new Date().toUTCString()));
                                                }
                                                else{
                                                    sql = "INSERT INTO unt.log_user (user, gacha) VALUE (?, 1) ON DUPLICATE KEY UPDATE gacha=gacha+1";
                                                    connection.query(sql, user, function (err) {
                                                        if (err) {
                                                            console.log(config.color.red, 'user : ', user, ', func : ', func + " User gacha update", ', time : ', new Date(new Date().toUTCString()));
                                                        }
                                                        connection.release();
                                                    });
                                                }
                                            });
                                        }
                                    });
                                }
                            });
                        }
                        else{
                            count++;
                            console.log("::::::::::::::::::Error");
                            console.log(new_data.rows[0]);
                            if(count > 5){
                                clearInterval(timer);
                                res.status(200).send("Fail:Time Out:",func);
                            }
                        }
                    }
                })
            }
        })
    }, 200);;
}


/**
 * Main 10 Gacha - New
 * 
 * @param req 
 * @param res 
 */
Gacha.tenGacha = function(req, res){
    var func = 'tenGacha';

    var user = req.body.user;
    var count = req.body.count;
    eos = Eos(config.eos);
    
    var count = 0;
    var timer = setInterval(function () {
        eos.getTableRows({
            code: config.contract.main,
            scope: config.contract.main,
            table: 'ttotalgacha',
            lower_bound: user,
            limit: 1,
            json: true
        }, function (err, new_data) {
            if (err) {
                res.status(200).send("Fail:Get Table:" + func);
            }
            else {
                var len = new_data.rows[0].result_list.length;
                if(len == (count + 10)){
                    var table = "";
                    var servant_list = [];
                    var monster_list = [];
                    var equip_list = [];

                    for(var i = 0; i > 9; i++){
                        if(new_data.rows[0].result_list[len-1-i].type == 1){
                            table = "tservant";
                        }
                        if(new_data.rows[0].result_list[len-1-i].type == 2){
                            table = "tmonster";
                        }
                        if(new_data.rows[0].result_list[len-1-i].type == 3){
                            table = "tequipments"
                        }
                        eos.getTableRows({
                            code : config.contract.main,
                            scope : user,
                            table : table,
                            lower_bound : new_data.rows[0].result_list[len-1-i].index,
                            limit : 1,
                            json : true
                        }, function(err, result){
                            if(err){
                                clearInterval(timer);
                                console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                                res.status(200).send("Fail:Success gacha but get data error:" + func);
                            }
                            else{
                                if(new_data.rows[0].result_list[len-1-i].type == 1){
                                    servant_list.push(result.rows[0]);
                                }
                                if(new_data.rows[0].result_list[len-1-i].type == 2){
                                    monster_list.push(result.rows[0]);
                                }
                                if(new_data.rows[0].result_list[len-1-i].type == 3){
                                    equip_list.push(result.rows[0]);
                                }

                                poolCluster.getConnection(function(err, connection){
                                    if(err){
                                        console.log("Fail:Connect DB:" + func);
                                    }
                                    else{
                                        var type = "";
                                        var type_index = 0;
                                        var grade = 0;
                                        
                                        if(new_data.rows[0].result_list[len-1-i].type == 1){
                                            type = "servant";
                                            type_index = result.rows[0].servant.id;
                                            grade = result.rows[0].servant.grade;
                                        }
                                        if(new_data.rows[0].result_list[len-1-i].type == 2){
                                            type = "monster";
                                            type_index = result.rows[0].monster.id;
                                            grade = result.rows[0].monster.grade;
                                        }
                                        if(new_data.rows[0].result_list[len-1-i].type == 3){
                                            type = "equipment";
                                            type_index = result.rows[0].equipment.id;
                                            grade = result.rows[0].equipment.grade;
                                        }
                                        var gachaLogData = {
                                            user : user,
                                            type : type,
                                            grade : grade,
                                            type_index : type_index
                                        };
                                        
                                        var sql = "INSERT INTO unt.log_gacha SET ?";
                                        connection.query(sql, gachaLogData, function(err){
                                            if(err){
                                                connection.release();
                                                console.log(config.color.red, 'user : ', user, ', func : ', func + " GachaLog DB Insert", ', time : ', new Date(new Date().toUTCString()));
                                            }
                                            else{
                                                var eosLogData = {
                                                    user : user,
                                                    amount : 10000,
                                                    type : "gacha"
                                                }
                                                sql = "INSERT INTO unt.log_eos SET ?";
                                                connection.query(sql, eosLogData, function(err){
                                                    if(err){
                                                        connection.release();
                                                        console.log(config.color.red, 'user : ', user, ', func : ', func + " EosLog DB Insert", ', time : ', new Date(new Date().toUTCString()));
                                                    }
                                                    else{
                                                        sql = "INSERT INTO unt.log_user (user, gacha) VALUE (?, 1) ON DUPLICATE KEY UPDATE gacha=gacha+1";
                                                        connection.query(sql, user, function (err) {
                                                            if (err) {
                                                                console.log(config.color.red, 'user : ', user, ', func : ', func + " User gacha update", ', time : ', new Date(new Date().toUTCString()));
                                                            }
                                                            connection.release();
                                                        });
                                                    }
                                                });
                                            }
                                        });
                                    }
                                });
                            }
                        })
                    }

                    var data ={
                        servant : servant_list,
                        monster : monster_list,
                        equipment : equip_list
                    }

                    clearInterval(timer);
                    console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                    res.status(200).send(data);

                }
                else{
                    count++;
                    console.log("::::::::::::::::::Error");
                    console.log(new_data.rows[0]);
                    if (count > 5) {
                        clearInterval(timer);
                        res.status(200).send("Fail:Time Out:", func);
                    }
                }
            }
        }) 
    }, 200);;
}



/**
 * Limit Gacha - New
 * 
 * @param req 
 * @param res 
 */
Gacha.limitgacha = function(req, res){
    var func = 'limitgacha';

    var user = req.body.user;
    var limit_total_count = req.body.total_count;
    

    eos = Eos(config.eos);
    
    var count = 0;
    var timer = setInterval(function () {
        async.parallel([
            function (next) {
                // Get EOS Info : 1
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
            function(next){
                eos.getTableRows({
                    code: config.contract.main,
                    scope: config.contract.main,
                    table: 'tgacharesult',
                    lower_bound: user,
                    limit: 1,
                    json: true
                }, function(err, gacha){
                    if (err) {
                        next("Fail:Get Token Table:" + func);
                    }
                    else {
                        next(null, gacha);
                    }
                })
            },
            function (next) {
                eos.getTableRows({
                    code: config.contract.main,
                    scope: config.contract.main,
                    table: 'tlimit',
                    lower_bound: config.contract.main,
                    limit: 1,
                    json: true
                }, function (err, gacha) {
                    if (err) {
                        next("Fail:Get Token Table:" + func);
                    }
                    else {
                        next(null, gacha);
                    }
                })
            }
        ], function(err, result_data){
            if(err){
                res.status(200).send("Fail:Get Table:" + func);
            }
            else{
                var table = "";
                var data = {};

                // EOS Setting
                var uEos = result_data[0].rows[0].balance.split(" ");
                uEos = uEos[0].split(".");
                if (uEos[0] == '0' && uEos[1] == '0000') {
                    data.eos = '0';
                }
                else {
                    data.eos = uEos[0] + uEos[1];
                }

                if (result_data[1].rows[0].result.type == 1) {
                    table = "tservant";
                }
                if (result_data[1].rows[0].result.type == 2) {
                    table = "tmonster";
                }
                if (result_data[1].rows[0].result.type == 3) {
                    table = "tequipments";
                }
                if(result_data[1].rows[0].result.type == 4){
                    table = "titem";
                }

                eos.getTableRows({
                    code: config.contract.main,
                    scope: user,
                    table: table,
                    lower_bound: result_data[1].rows[0].result.index,
                    limit: 1,
                    json: true
                }, function (err, table_result){
                    if (err) {
                        clearInterval(timer);
                        console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                        res.status(200).send("Fail:Success gacha but get data error:" + func);
                    }
                    else{
                        if(table_result.rows.length != 0){
                            clearInterval(timer);
                            var temp_list = [];                        
                            temp_list.push(table_result.rows[0]);
                            if (result_data[1].rows[0].result.type == 1) {
                                data.get_servant_list = temp_list;
                                data.get_monster_list = [];
                                data.get_equipment_list = [];
                                data.get_item_list = [];
                                data.get_total_count = limit_total_count; 
                            }
                            if (result_data[1].rows[0].result.type == 2) {
                                data.get_servant_list = [];
                                data.get_monster_list = temp_list;;
                                data.get_equipment_list = [];
                                data.get_item_list = [];
                                data.get_total_count = limit_total_count; 
                            }
                            if (result_data[1].rows[0].result.type == 3) {
                                data.get_servant_list = [];
                                data.get_monster_list = [];
                                data.get_equipment_list = temp_list;;
                                data.get_item_list = [];
                                data.get_total_count = limit_total_count; 
                            }
                            if(result_data[1].rows[0].result.type == 4){
                                data.get_servant_list = [];
                                data.get_monster_list = [];
                                data.get_equipment_list = [];
                                data.get_item_list = temp_list;
                                data.get_total_count = limit_total_count; 
                            }
                            
                            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                            res.status(200).send(data);

                            poolCluster.getConnection(function(err, connection){
                                if(err){
                                    console.log("Fail:Connect DB:" + func);
                                }
                                else{
                                    var gachaLogData = {
                                        user : user
                                    }
                                    
                                    if(result_data[1].rows[0].result.type == 1){
                                        gachaLogData.type = "servant";
                                        gachaLogData.type_index = table_result.rows[0].servant.id;
                                        gachaLogData.grade = table_result.rows[0].servant.grade;
                                    }
                                    if(result_data[1].rows[0].result.type == 2){
                                        gachaLogData.type = "monster";
                                        gachaLogData.type_index = table_result.rows[0].monster.id;
                                        gachaLogData.grade = table_result.rows[0].monster.grade;
                                    }
                                    if(result_data[1].rows[0].result.type == 3){
                                        gachaLogData.type = "equipment";
                                        gachaLogData.type_index = table_result.rows[0].equipment.id;
                                        gachaLogData.grade = table_result.rows[0].equipment.grade;
                                    }
                                    if(result_data[1].rows[0].result.type == 4){
                                        gachaLogData.type = "item";
                                        gachaLogData.type_index = table_result.rows[0].id;
                                        gachaLogData.grade = 0;
                                    }
                                    
                                    var sql = "INSERT INTO unt.log_gacha SET ?";
                                    connection.query(sql, gachaLogData, function(err){
                                        if(err){
                                            connection.release();
                                            console.log(config.color.red, 'user : ', user, ', func : ', func + " GachaLog DB Insert", ', time : ', new Date(new Date().toUTCString()));
                                        }
                                        else{
                                            var eosLogData = {
                                                user : user,
                                                amount : 10000,
                                                type : "gacha"
                                            }
                                            sql = "INSERT INTO unt.log_eos SET ?";
                                            connection.query(sql, eosLogData, function(err){
                                                if(err){
                                                    connection.release();
                                                    console.log(config.color.red, 'user : ', user, ', func : ', func + " EosLog DB Insert", ', time : ', new Date(new Date().toUTCString()));
                                                }
                                                else{
                                                    sql = "INSERT INTO unt.log_user (user, gacha) VALUE (?, 1) ON DUPLICATE KEY UPDATE gacha=gacha+1";
                                                    connection.query(sql, user, function (err) {
                                                        if (err) {
                                                            console.log(config.color.red, 'user : ', user, ', func : ', func + " User gacha update", ', time : ', new Date(new Date().toUTCString()));
                                                        }
                                                        connection.release();
                                                    });
                                                }
                                            });
                                        }
                                    });
                                }
                            });
                        }
                        else{
                            count++;
                            console.log("::::::::::::::::::Error");
                            console.log(new_data.rows[0]);
                            if(count > 5){
                                clearInterval(timer);
                                res.status(200).send("Fail:Time Out:",func);
                            }
                        }
                    }
                })
            }
        })
    }, 200);;
}


module.exports = Gacha;