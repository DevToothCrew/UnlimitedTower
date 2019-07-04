module.exports = {
   prod : {
        EOS_NETWORK : {
            chainId: 'aca376f206b8fc25a6ed44dbdc66547c36c6c33e3a119ffbeaef943642f0e906', // 32 byte (64 char) hex string
            httpEndpoint: 'https://api-mainnet.starteos.io:443',
            expireInSeconds: 60,
            broadcast: true,
            verbose: false // API activity
        },
        GAME_CONTRACT : "untowermain1",
   },

   dev : {
        EOS_NETWORK : {
            chainId: 'e70aaab8997e1dfce58fbfac80cbbb8fecec7b99cf982a9444273cbc64c41473', // 32 byte (64 char) hex string
            httpEndpoint: 'http://jungle2.cryptolions.io:80',
            expireInSeconds: 60,
            broadcast: true,
            verbose: false // API activity
        },
        GAME_CONTRACT : "dldhtmtlqkf2"
   }

}