Eos = require('eosjs')
config = require('../config/')

var async = require('async');

function Test() {}

Test.get_table = function(req, res){
    var func = "Get EOS Table";
    
    //var user = req.body.user;

    eos = Eos(config.eos);

    async.parallel([
        function(next){
            eos.getTableRows({
                code : 'canietest111',
                scope : 'canietest111',
                table : 'cservant',
                json : true
            }, function(err, servant){
                if(err){
                    next(err);
                }
                else{
                    next(null, servant);
                }
            });
        },
        function(next){
            eos.getTableRows({
                code : 'canietest111',
                scope : 'canietest111',
                table : 'citem',
                json : true
            }, function(err, item){
                if(err){
                    next(err);
                }
                else{
                    next(null, item);
                }
            });
        },
        function(next){
            eos.getTableRows({
                code : 'canietest111',
                scope : 'canietest111',
                table : 'cmonster',
                json : true
            }, function(err, monster){
                if(err){
                    next(err);
                }
                else{
                    next(null, monster);
                }
            });
        }
    ],
    function(err, tableData){
        if(err){
            console.log("Get Table Error");
        }
        else{
            var user_data = {
                servant: tableData[0],
                item: tableData[1],
                monster: tableData[2]
            }
            res.status(200).send(user_data);
        }
    });
}

Test.table_update = function(req, res){
    var func = "Table Update Test";

    var user = req.body.user;
    eos = Eos(config.eos);

    async.waterfall([
        // 테이블 정보 먼저 불러옴
        function(callback){
            eos.getTableRows({
                code : 'doraemonydwy',
                scope : user,
                table : 'scores',
                json : true
            }, function(err, oldTable){
                if(err){
                    console.error("Fail to get table.");
                }
                else{
                    console.log('Old Table : ',oldTable);
                    callback(null, oldTable);
                }
            });
        },
        // 이전 테이블 정보과 현재 테이블 정보가 차이가 나면 될때까지 2초마다 새로운 테이블 불러옴
        function(oldTable, callback){
            /*
            setTimeout(function(){
                clearInterval(timer);
                callback('error');
            }, 10000);
            */

            var timer = setInterval(function(){
                eos.getTableRows({
                    code : 'doraemonydwy',
                    scope : user,
                    table : 'scores',
                    json : true
                }, function(err, newTable){
                    console.log('New Table : ',newTable);
                    if(err){
                        console.error("Fail to get Table.");
                    }
                    else{
                        if(oldTable.rows[0].num != newTable.rows[0].num){
                            clearInterval(timer);
                            callback(null, newTable);
                        }
                    }
            })}, 1000);
        }
    ],
    function(err, result){
        if(err){
            res.status(200).send("Get Table Test fail.");
        }
        else{
            res.status(200).send(result);
        }
    });
}

Test.gacha = function(req, res){
    var func = "gacha";

    var user = req.body.user;
    eos = Eos(config.eos);

    async.waterfall([
        function(callback){
            async.parallel([
                function(next){
                    eos.getTableRows({
                        code : 'canietest444',
                        scope : 'canietest444',
                        table : 'cservant',
                        lower_bound : user,
                        limit : 1,
                        json : true
                    }, function(err, servant){
                        if(err){
                            next(err);
                        }
                        else{
                            next(null, servant);
                        }
                    });
                },
                function(next){
                    eos.getTableRows({
                        code : 'canietest444',
                        scope : 'canietest444',
                        table : 'citem',
                        lower_bound : user,
                        limit : 1,
                        json : true
                    }, function(err, item){
                        if(err){
                            next(err);
                        }
                        else{
                            next(null, item);
                        }
                    });
                },
                function(next){
                    eos.getTableRows({
                        code : 'canietest444',
                        scope : 'canietest444',
                        table : 'cmonster',
                        lower_bound : user,
                        limit : 1,
                        json : true
                    }, function(err, monster){
                        if(err){
                            next(err);
                        }
                        else{
                            next(null, monster);
                        }
                    });
                }
            ],
            function(err, tableData){
                if(err){
                    console.log("Get Table Error");
                }
                else{
                    var old_data = {
                        servant: tableData[0],
                        item: tableData[1],
                        monster: tableData[2]
                    }
                    callback(null, old_data);
                }
            });
        },
        // 이전 테이블 정보과 현재 테이블 정보가 차이가 나면 될때까지 2초마다 새로운 테이블 불러옴
        function(old_data, callback){
            var count = 0;
        
            var timer = setInterval(function(){
                async.parallel([
                    function(next){
                        eos.getTableRows({
                            code : 'canietest444',
                            scope : 'canietest444',
                            table : 'cservant',
                            lower_bound : user,
                            limit : 1,
                            json : true
                        }, function(err, servant){
                            if(err){
                                next(err);
                            }
                            else{
                                next(null, servant);
                            }
                        });
                    },
                    function(next){
                        eos.getTableRows({
                            code : 'canietest444',
                            scope : 'canietest444',
                            table : 'citem',
                            lower_bound : user,
                            limit : 1,
                            json : true
                        }, function(err, item){
                            if(err){
                                next(err);
                            }
                            else{
                                next(null, item);
                            }
                        });
                    },
                    function(next){
                        eos.getTableRows({
                            code : 'canietest444',
                            scope : 'canietest444',
                            table : 'cmonster',
                            lower_bound : user,
                            limit : 1,
                            json : true
                        }, function(err, monster){
                            if(err){
                                next(err);
                            }
                            else{
                                next(null, monster);
                            }
                        });
                    }
                ],
                function(err, tableData){
                    count += 1;
                    if(err){
                        console.log("Get Table Error");
                    }
                    else{
                        var new_data = {
                            servant: tableData[0],
                            item: tableData[1],
                            monster: tableData[2]
                        }
                        if(old_data.servant.rows[0].servant_list.length != new_data.servant.rows[0].servant_list.length){
                            clearInterval(timer);
                            new_data.servant.rows[0].servant_list[new_data.servant.rows[0].servant_list.length-1].result = "s";
                            callback(null, new_data.servant.rows[0].servant_list[new_data.servant.rows[0].servant_list.length-1]);
                        }
                        if(old_data.item.rows[0].item_list.length != new_data.item.rows[0].item_list.length){
                            clearInterval(timer);
                            new_data.item.rows[0].item_list[new_data.item.rows[0].item_list.length-1].result = "i";
                            callback(null, new_data.item.rows[0].item_list[new_data.item.rows[0].item_list.length-1]);
                        }
                        if(old_data.monster.rows[0].monster_list.length != new_data.monster.rows[0].monster_list.length){
                            clearInterval(timer);
                            new_data.monster.rows[0].monster_list[new_data.monster.rows[0].monster_list.length-1].result = "m";
                            callback(null, new_data.monster.rows[0].monster_list[new_data.monster.rows[0].monster_list.length-1]);
                        }
                        if(count >= 5){
                            clearInterval(timer);
                            callback('error');
                        }
                    }
                })}, 2000);
        }
    ],
    function(err, result){
        if(err){
            res.status(200).send("Fail to get gacha data");
        }
        else{
            res.status(200).send(result);
        }
    });
}

module.exports = Test;