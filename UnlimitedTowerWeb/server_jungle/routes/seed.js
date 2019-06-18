var crypt = require('../lib/Crypt');
var sha256 = require('sha256');
var crypto = require('crypto');

function Seed() {}

/**
 * Create Seed number
 * 
 * @param req
 * @param res
 */
Seed.createSeed = function(req, res){
    var num = Math.floor(Math.random() * (1000000000000 - 1) + 1);
    var key = "5JKJTSYDTMLCH1BF9awxLrPAyyAvFyq35rGpJxwAopyH2c9gZ3h";
    var data = key+num;

    var seed = sha256(data);

    var data = {
        num: num,
        seed: seed
    }

    res.status(200).send(data);
}

module.exports = Seed;