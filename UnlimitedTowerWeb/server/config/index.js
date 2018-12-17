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
    // EOS configuration
    eos: {
        chainId: 'e70aaab8997e1dfce58fbfac80cbbb8fecec7b99cf982a9444273cbc64c41473', // 32 byte (64 char) hex string
        httpEndpoint: 'https://jungle2.cryptolions.io:443',
        expireInSeconds: 60,
        broadcast: true,
        verbose: true // API activity
    },
    // Contract list
    contract : {
        main : 'canietest444'
    }
};