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
    logger: {
        levels: {
            info: 1,
            error: 2,
            debug: 3
        },
        colors: {
            info: "green",
            debug: "blue",
            error: "red"
        }
    },
    color: {
        green : '\x1b[32m',
        red : '\x1b[31m',
        yellow : '\x1b[33m',
        cyan : '\x1b[36m',
        white : '\x1b[37m',
        magenta : '\x1b[35m'
    },
    // EOS configuration
    eos: {
        chainId: 'aca376f206b8fc25a6ed44dbdc66547c36c6c33e3a119ffbeaef943642f0e906', // 32 byte (64 char) hex string
        httpEndpoint: 'https://proxy.eosnode.tools:443',
        expireInSeconds: 60,
        broadcast: true,
        verbose: true // API activity
    },

     // EOS Junglenet configuration
     eos_jungle: {
        chainId: 'e70aaab8997e1dfce58fbfac80cbbb8fecec7b99cf982a9444273cbc64c41473', // 32 byte (64 char) hex string
        httpEndpoint: 'http://jungle2.cryptolions.io:80',
        expireInSeconds: 60,
        broadcast: true,
        verbose: true // API activity
    },
    // Contract list
    contract : {
        main : 'untowermain1',
        dev : 'unlimitedmas'
    }
};