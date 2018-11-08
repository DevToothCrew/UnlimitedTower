'use strict';

module.exports = {
    mysql: {
        master: {
            connectionLimit: 800,
            host: "10.140.0.5",
            user: "root",
            password: "wB7fwfajdusk"
        }
    },
    // HTTP Communication Enc/Dec
    encryption: {
        state: true,
        key : "C0LTUK3mgDyjzAB09Ui8BIwTCkkR2sbJ06OnKvJq84M=",
        iv :  "hJrN5XisGO652h2pFNSZ6A=="
    },
    log: {
        console: {
            level: "error"
        },
        file: {
            level: "error"
        }
    },
    // EOS configuration
    eos: {
        chainId: '038f4b0fc8ff18a4f0842a8f0564611f6e96e8535901dd45e43ac8691a1c4dca', // 32 byte (64 char) hex string
        keyProvider: ['5K3wipXP5524WBN15BDjSfNWJ7uzrrBBin1wHF5M42ByVtu2ibq'], // WIF string or array of keys..
        httpEndpoint: 'http://jungle.cryptolions.io:18888',
        expireInSeconds: 60,
        broadcast: true,
        verbose: true, // API activity
        sign: true
    }
};