Eos = require('eosjs')
config = require('../config/')

function Test() {}

/**
 * Create Seed number
 * 
 * @param req
 * @param res
 */
Test.test = function(req, res){
    const MongoOplog = require('mongo-oplog');
    const oplog = MongoOplog('mongodb://devTooth:U0S91ydQ0uYPSaOqHz3T@eos-sg-00-00-mongo-00.itam.games:27017/local?replicaSet=eos-00&authSource=admin', 
        { ns: 'EOS.action_traces', reconnectTries: 60, reconnectInterval: 1000 });
    // 아래의 명령어를 통해 oplog 트리거 시작
    oplog.tail();

    oplog.on('insert', doc => {
        const actionInfo = doc.o;
        const action = actionInfo.act;
        if(action.account == 'untowermain1'){
            console.log(action.data._result);
            res.status(200).send(action.data._result);
            oplog.stop();
        }
    });

    oplog.on('error', error => {
        console.log(error);
    });
}

Test.string = function(req, res){
    var text = 'mon:2:20024:3:4:3:3';
    let array = text.split(':');
    var data = {
        type : array[0],
        index : array[1],
        num : array[2],
        grade : array[3],
        str : array[4],
        dex : array[5],
        int : array[6]
    }

    res.status(200).send(data);
}

Test.timer = function(req, res){
    const MongoOplog = require('mongo-oplog');
    const oplog = MongoOplog('mongodb://devTooth:U0S91ydQ0uYPSaOqHz3T@eos-sg-00-00-mongo-00.itam.games:27017/local?replicaSet=eos-00&authSource=admin', 
        { ns: 'EOS.action_traces', reconnectTries: 60, reconnectInterval: 1000 });
    // 아래의 명령어를 통해 oplog 트리거 시작
    oplog.tail();

    oplog.on('insert', doc => {
        const actionInfo = doc.o;
        const action = actionInfo.act;
        console.log(action);
    });

    var count = 0;
    var timer = setInterval(function(){
        if(count == 3){
            clearInterval(timer);
            oplog.stop();
            res.status(200).send("gooog");
        }
        else{
            count++;
        }
    }, 1000);

    oplog.on('error', error => {
        console.log(error);
    });
}


Test.getEos = function(req, res){

    eos = Eos(config.eos);

    var user  = req.body.user;

    eos.getTableRows({
        code : 'eosio.token',
        scope : user,
        table : 'accounts',
        limit : 100,
        json : true
    }, function(err, result){
        if(err){
            res.status(200).send('Error');
        }
        else{
            res.status(200).send(result);
        }
    });
}

Test.table1 = function(req, res){

    eos = Eos(config.eos_jungle);

    var user  = req.body.user;

    eos.getTableRows({
        code : config.contract.dev,
        scope : config.contract.dev,
        table : 'tclearreward',
        lower_bound : user,
        limit : 1,
        json : true
    }, function(err, result){
        if(err){
            res.status(200).send('Error');
        }
        else{
            res.status(200).send(result.rows[0]);
        }
    });
}

Test.table2 = function(req, res){

    eos = Eos(config.eos_jungle);

    var user  = req.body.user;

    eos.getTableRows({
        code : config.contract.dev,
        scope : config.contract.dev,
        table : 'tbattlestate',
        lower_bound : user,
        limit : 1,
        json : true
    }, function(err, result){
        if(err){
            res.status(200).send('Error');
        }
        else{
            res.status(200).send(result.rows[0]);
        }
    });
}

Test.table3 = function(req, res){

    eos = Eos(config.eos_jungle);

    var user  = req.body.user;

    eos.getAccount(user, function(err, result){
        if(err){
            res.status(200).send('Error');
        }
        else{
            var data = {
                ram_quota : result.ram_quota,
                net_weight : result.net_weight,
                cpu_weight : result.cpu_weight,
                net_limit : result.net_limit,
                cpu_limit : result.cpu_limit,
                ram_usage : result.ram_usage 
            }
            res.status(200).send(data);
        }
    });
}

module.exports = Test;