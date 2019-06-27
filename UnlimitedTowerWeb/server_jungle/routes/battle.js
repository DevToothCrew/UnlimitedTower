Eos = require('eosjs')
config = require('../config/')

var async = require('async');

var poolCluster = require('../lib/MySQLPoolCluster').PoolCluster;

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
    var turn = req.body.turn;

    eos = Eos(config.eos);

    var count = 0;

    var timer = setInterval(function(){
        eos.getTableRows({
            code: config.contract.main,
            scope: config.contract.main,
            table: 'tbattleact',
            lower_bound: user,
            limit: 1,
            json: true
        }, function (err, battleTable) {
            if (err) {
                console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                res.status(200).send("Fail:Get Table:" + func);
            }
            else {
                if(turn == battleTable.rows[0].turn || battleTable.rows[0].turn == 99999 || battleTable.rows[0].turn == 99998 || battleTable.rows[0].turn == 99997){
                    clearInterval(timer);
                    console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                    res.status(200).send(battleTable.rows[0]);
                }
                else{
                    if(count > 10)
                    {
                        clearInterval(timer);
                        console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                        res.status(200).send("Fail:sync");
                    }
                    count++;
                    console.log(config.color.yellow, "Waiting for sync For ", user, "'s Battle Data");
                }
            }
        })
    }, 1500);
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
    var count = 0;
    
    eos = Eos(config.eos);

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
 * Battle Start
 * 
 * @param req
 * @param res
 */
Battle.battleStart = function (req, res) {
    var func = 'battleStart';

    var stage_type = req.body.stage_type;
    var user = req.body.user;
    var enter_item = req.body.enter_item;
    var key = 0;

    if(enter_item == 4){
        key = 500200;
    }
    else if(enter_item == 3){
        key = 500210;
    }
    else if(enter_item == 2){
        key = 500220;
    }
    else if(enter_item == 1){
        key = 500230;
    }
    else{
        key = 0;
    }
    eos = Eos(config.eos);

    var count = 0;
    
    var timer = setInterval(function(){
        eos.getTableRows({
            code: config.contract.main,
            scope: config.contract.main,
            table: 'tstgstates',
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
                if (newTable.rows.length != 0 && newTable.rows[0].user == user) {
                    if (newTable.rows[0].turn != 0) {
                        if(count >10)
                        {
                            clearInterval(timer);
                            console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                            res.status(200).send("Fail:Turn Out:" + func);
                        }
                        count++;
                        console.log(config.color.yellow, "Waiting for sync For ", user, "'s Battle Start");
                    }
                    else {
                        clearInterval(timer);
                        console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                        var data = {
                            battle_state : newTable.rows[0]
                        }
                        if (stage_type == 1) {
                            eos.getTableRows({
                                code: config.contract.main,
                                scope: user,
                                table: 'titem',
                                lower_bound: key,
                                limit: 1,
                                json: true
                            }, function (err, item_info) {
                                if (err) {
                                    console.error("Fail:Get Table:" + func);
                                    res.status(200).send("Fail:Get Table:" + func);
                                }
                                else {
                                    var enter_item = {}
                                    if (item_info.rows.length == 0) {
                                        enter_item.id = key;
                                        enter_item.type = 3;
                                        enter_item.item_list = [];

                                        data.enter_item = enter_item;
                                    }
                                    else {
                                        if (item_info.rows[0].id != key) {
                                            enter_item.id = key;
                                            enter_item.type = 3;
                                            enter_item.item_list = [];

                                            data.enter_item = enter_item;
                                        }
                                        else {
                                            data.enter_item = item_info.rows[0];
                                        }
                                    }

                                    res.status(200).send(data);

                                    // DB Login Count 추가
                                    poolCluster.getConnection(function (err, connection) {
                                        if (err) {
                                            console.log("Fail:Connect DB:" + func);
                                        }
                                        else {
                                            var sql = "INSERT INTO unt.log_user (user, battle) VALUE (?, 1) ON DUPLICATE KEY UPDATE battle=battle+1";
                                            connection.query(sql, user, function (err) {
                                                if (err) {
                                                    console.log(config.color.red, 'user : ', user, ', func : ', func + " User battle update", ', time : ', new Date(new Date().toUTCString()));
                                                }
                                                connection.release();
                                            });
                                        }
                                    });
                                }
                            })
                        }
                        else
                        {
                            eos.getTableRows({
                                code: config.contract.main,
                                scope: config.contract.main,
                                table: 'tuserauth',
                                lower_bound: user,
                                limit: 1,
                                json: true
                            }, function (err, user_info) {
                                if (err) {
                                    console.error("Fail:Get Table:" + func);
                                    res.status(200).send("Fail:Get Table:" + func);
                                }
                                else {
                                    var day_stage = {}
                                    if (user_info.rows.length == 0) {
                                        count++;
                                        if(count>=10){
                                            clearInterval(timer);
                                            console.error("Fail:Get Table:" + func);
                                            res.status(200).send("Fail:Get Table:" + func);
                                        }
                                    }
                                    else {
                                        if (user_info.rows[0].user == user) {
                                            day_stage.daily_enter_count = user_info.rows[0].daily_enter_count;
                                            day_stage.total_enter_count = user_info.rows[0].total_enter_count;
                                            day_stage.daily_init_time = user_info.rows[0].daily_init_time;
                                            data.day_stage = day_stage;
                                        }
                                        else {
                                            count++;
                                            if(count>=10){
                                                clearInterval(timer);
                                                console.error("Fail:Get Table:" + func);
                                                res.status(200).send("Fail:Get Table:" + func);
                                            }
                                        }
                                    }

                                    res.status(200).send(data);

                                    // DB Login Count 추가
                                    poolCluster.getConnection(function (err, connection) {
                                        if (err) {
                                            console.log("Fail:Connect DB:" + func);
                                        }
                                        else {
                                            var sql = "INSERT INTO unt.log_user (user, battle) VALUE (?, 1) ON DUPLICATE KEY UPDATE battle=battle+1";
                                            connection.query(sql, user, function (err) {
                                                if (err) {
                                                    console.log(config.color.red, 'user : ', user, ', func : ', func + " User battle update", ', time : ', new Date(new Date().toUTCString()));
                                                }
                                                connection.release();
                                            });
                                        }
                                    });
                                }
                            })
                        }
                    }
                }
                else{
                    count++;
                    if(count>=10){
                        clearInterval(timer);
                        console.error("Fail:Get Table:" + func);
                        res.status(200).send("Fail:Get Table:" + func);
                    }
                }
            }
        })
    }, 1000);
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
    eos = Eos(config.eos);
    var count = 0;

    var timer = setInterval(function () {
        eos.getTableRows({
            code: config.contract.main,
            scope: config.contract.main,
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
                if (newTable.rows[0].state == 2 && newTable.rows[0].user === user) {
                    eos.getTableRows({
                        code: config.contract.main,
                        scope: config.contract.main,
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
                            if (reward.rows[0].user == user) {
                                var data = {
                                    reward_money: reward.rows[0].reward_money,
                                    get_rank_exp: reward.rows[0].get_rank_exp,
                                    get_char_exp_list: reward.rows[0].get_char_exp_list,
                                    get_servant_list: reward.rows[0].get_servant_list,
                                    get_monster_list: reward.rows[0].get_monster_list,
                                    get_equipment_list: reward.rows[0].get_equipment_list,
                                    get_item_list: reward.rows[0].get_item_list
                                }
                                clearInterval(timer);
                                console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                                res.status(200).send(data);
                            }
                            else {
                                if (count > 10) {
                                    clearInterval(timer);
                                    console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                                    res.status(200).send("Fail:sync");
                                }
                                count++;
                                console.log(config.color.yellow, "Waiting for sync For ", user, "'s Battle Reward");
                            }
                        }

                    })
                }
                else {
                    // console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                    // res.status(200).send("Fail:Time Out:" + func);

                    if (count > 10) {
                        clearInterval(timer);
                        console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                        res.status(200).send("Fail:sync");
                    }
                    count++;
                    console.log(config.color.yellow, "Waiting for sync For ", user, "'s Battle Reward");
                }
            }
        })
    }, 1500);
}

/**
 * StageEscape
 * 
 * @param req
 * @param res
 */
Battle.stageEscape = function (req, res) {

    var func = "stageEscape";

    var user = req.body.user;
    eos = Eos(config.eos);
    var count = 0;
    var timer = setInterval(function () {
        eos.getTableRows({
            code: config.contract.main,
            scope: config.contract.main,
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
                if (newTable.rows[0].state == 2 &&  newTable.rows[0].user === user) {
                    clearInterval(timer);
                    console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                    var data = {
                        success: true
                    }
                    res.status(200).send(data);                    
                }
                else {
                    if(count > 10)
                    {
                        clearInterval(timer);
                        console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                        res.status(200).send("Fail:sync");
                    }
                    count++;
                    // console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                    // res.status(200).send("Fail:Get Table:" + func);
                }
            }
        })
    }, 1500);
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
                    code: config.contract.main,
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


/**
 * Get Tower Reward Info
 * 
 * @param req
 * @param res
 */
Battle.getTowerReward = function (req, res) {
    var func = 'getTowerReward';

    var user = req.body.user;
    var floor = req.body.floor;
    
    eos = Eos(config.eos);

    async.parallel([
        function (next) {
            eos.getTableRows({
                code: config.contract.main,
                scope: config.contract.main,
                table: 'floorinfo',
                lower_bound : floor, 
                limit: 1,
                json: true
            }, function (err, floor_data) {
                if (err) {
                    next("Fail:Get Floor Table:" + func);
                }
                else {
                    next(null, floor_data);
                }
            });
        },
        function (next) {
            eos.getTableRows({
                code: config.contract.main,
                scope: config.contract.main,
                table: 'towerreward',
                lower_bound : floor, 
                limit: 1,
                json: true
            }, function (err, reward) {
                if (err) {
                    next("Fail:Get Reward Table:" + func);
                }
                else {
                    next(null, reward);
                }
            });
        }
    ],
        function (err, tableData) {
            if (err) {
                console.log(err);
                res.status(200).send(err);
            }
            else {
                if(tableData[0].rows.length != 0)
                {
                    if(tableData[1].rows.length != 0)
                    {
                        var onwer = '';
                        if(tableData[0].rows[0].owner == config.contract.main)
                        {
                            onwer = "NPC Defender";
                        }
                        else
                        {
                            onwer = tableData[0].rows[0].owner;
                        }
                        var user_data = {
                            owner : onwer,
                            utg : tableData[1].rows[0].total_utg,
                            eos : tableData[1].rows[0].total_eos,
                            event_end_time : tableData[0].rows[0].opentime, 
                            tower_end_time : tableData[0].rows[0].endtime
                        };
                        console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                        res.status(200).send(user_data);
                    }
                    else
                    {
                        res.status(200).send("Fail:Get Table Error:" + func);
                        console.log(config.color.red, 'user : ', user, ', func : ', func, ' err : ', error, ' time : ', new Date(new Date().toUTCString()));
                    }
                }
                else
                {
                    res.status(200).send("Fail:Get Table Error:" + func);
                    console.log(config.color.red, 'user : ', user, ', func : ', func, ' err : ', error, ' time : ', new Date(new Date().toUTCString()));
                }
            }
        });
}




/**
 * Tower Start
 * 
 * @param req
 * @param res
 */
Battle.towerStart = function (req, res) {
    var func = 'towerStart';

    var user = req.body.user;
    var floor = req.body.floor;


    eos = Eos(config.eos);

    var count = 0;
    
    var timer = setInterval(function(){
        eos.getTableRows({
            code: config.contract.main,
            scope: config.contract.main,
            table: 'tstgstates',
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
                if (newTable.rows.length != 0 && newTable.rows[0].user == user) {
                    if (newTable.rows[0].turn != 0) {
                        if(count >10)
                        {
                            clearInterval(timer);
                            console.log(config.color.red, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                            res.status(200).send("Fail:Turn Out:" + func);
                        }
                        count++;
                        console.log(config.color.yellow, "Waiting for sync For ", user, "'s Battle Start");
                    }
                    else {
                        clearInterval(timer);
                        console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                        var data = {
                            battle_state: newTable.rows[0]
                        }
                        async.parallel([
                            function (next) {
                                // Get Token Info
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
                                eos.getTableRows({
                                    code: config.contract.main,
                                    scope: config.contract.main,
                                    table: 'floorinfo',
                                    lower_bound : floor, 
                                    limit: 1,
                                    json: true
                                }, function (err, floor_data) {
                                    if (err) {
                                        next("Fail:Get Floor Table:" + func);
                                    }
                                    else {
                                        next(null, floor_data);
                                    }
                                });
                            },
                            function (next) {
                                eos.getTableRows({
                                    code: config.contract.main,
                                    scope: config.contract.main,
                                    table: 'towerreward',
                                    lower_bound : floor, 
                                    limit: 1,
                                    json: true
                                }, function (err, reward) {
                                    if (err) {
                                        next("Fail:Get Reward Table:" + func);
                                    }
                                    else {
                                        next(null, reward);
                                    }
                                });
                            }
                        ],
                            function (err, tableData) {
                                if (err) {
                                    console.log(err);
                                    res.status(200).send(err);
                                }
                                else {
                                    if (tableData[0].rows.length == 0) {
                                        data.utg = '0';
                                    }
                                    else {
                                        var token = tableData[0].rows[0].balance.split(" ");
                                        token = token[0].split(".");
                                        data.utg = token[0] + token[1];
                                    }


                                    if(tableData[1].rows.length != 0)
                                    {
                                        if(tableData[2].rows.length != 0)
                                        {
                                            var onwer = '';
                                            if(tableData[1].rows[0].owner == config.contract.main)
                                            {
                                                onwer = "NPC Defender";
                                            }
                                            else
                                            {
                                                onwer = tableData[1].rows[0].owner;
                                            }
                                            var user_data = {
                                                owner : onwer,
                                                utg : tableData[2].rows[0].total_utg,
                                                eos : tableData[2].rows[0].total_eos,
                                                event_end_time : tableData[1].rows[0].opentime, 
                                                tower_end_time : tableData[1].rows[0].endtime
                                            };
                                            data.tower_reward_info = user_data;
                                            console.log(config.color.green, 'user : ', user, ', func : ', func, ', time : ', new Date(new Date().toUTCString()));
                                            res.status(200).send(data);
                                        }
                                        else
                                        {
                                            res.status(200).send("Fail:Get Table Error:" + func);
                                            console.log(config.color.red, 'user : ', user, ', func : ', func, ' err : ', error, ' time : ', new Date(new Date().toUTCString()));
                                        }
                                    }
                                    else
                                    {
                                        res.status(200).send("Fail:Get Table Error:" + func);
                                        console.log(config.color.red, 'user : ', user, ', func : ', func, ' err : ', error, ' time : ', new Date(new Date().toUTCString()));
                                    }
                                }
                            });

                    }
                }
                else{
                    count++;
                    if(count>=10){
                        clearInterval(timer);
                        console.error("Fail:Get Table:" + func);
                        res.status(200).send("Fail:Get Table:" + func);
                    }
                }
            }
        })
    }, 1000);
}

module.exports = Battle;