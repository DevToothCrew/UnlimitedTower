const configFile = require('../config');
const config = configFile[process.env.NODE_ENV];

const Eos = require('eosjs');
const eos = Eos(config.EOS_NETWORK);

module.exports = {
    getBlockNum : () => {
        return new Promise((resolve, reject) => {
            eos.getInfo((err, docs) => {
                if(err) {
                    reject(err);
                } else {
                    resolve(docs.head_block_num);
                }
            })
        });
    },

    getBlock : (blockNum) => {
        return new Promise((resolve, reject) => {
            eos.getBlock(blockNum).then(docs => {
                resolve(docs);
            }).catch(e => {
                reject(e);
            });
        });
    }
}