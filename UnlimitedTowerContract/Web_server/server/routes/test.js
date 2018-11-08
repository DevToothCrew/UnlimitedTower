Eos = require('eosjs')
config = require('../config/')

eos = Eos(config.eos)

function Test() {}

/**
 * EOS Connect Test
 * 
 * @param req
 * @param res
 */
Test.test = function(req, res){
    var func = "EOS Connect Test";

    eos.transaction({
        actions:[
            {
                account: 'towermasters',
                name: 'signup',
                authorization: [{
                    actor: 'toweraccoun2',
                    permission: 'active'
                }],
                data: {
                    _user : 'toweraccoun2'
                }
            }
        ]
    });

    res.send('test ok');
};

Test.getinfo = function(req, res){
    var func = "EOS Connect Test";

// // Promise
// eos.getAccount('toweraccoun2')
//     .then(result => console.log(result))
//     .catch(error => console.error(error));

// // callback
// eos.getAccount('toweraccoun2', (error, result) => console.log(error, result));

// // Parameters object
// eos.getAccount({account_name: 'toweraccoun2'})
//     .then(result => console.log(result))
//     .catch(error => console.error(error));

eos.getTableRows({
    "json": true,
    "scope": 'towermasters',
    "code": 'towermasters',
    "table": "cuserauth",
    "lower_bound": 'toweraccoun2',
    "limit" : 1,
  }).then(result => console.log(result))
.catch(error => console.error(error));


    res.send('getinfo ok');
};

module.exports = Test;