const configFile = require('../config');
const config = configFile[process.env.NODE_ENV];

const Eos = require('eosjs');
const eos = Eos(config.EOS_NETWORK);

let resultArr = [];

module.exports = {
    getTuserauth : (init, pkValue, limitNum) => {
        return new Promise((resolve, reject) => {
            eos.getTableRows({
                code: config.GAME_CONTRACT,
                scope: config.GAME_CONTRACT,
                limit: limitNum,
                table: 'tuserauth',
                json: true,
                lower_bound: pkValue
            }).then(docs => {
                resolve(docs);
            }).catch(e => {
                reject(e);
            });
        });
    },

    getTservant : function(init, user, pkValue) {
        return new Promise((resolve, reject) => {
            let lowerBound = typeof pkValue != 'undefined' ? pkValue : null

            eos.getTableRows({
                code: config.GAME_CONTRACT,
                scope: user,
                table: 'tservant',
                limit: 1000,
                json: true,
                lower_bound: lowerBound
            }).then(async (docs) => {
                if(init == true) {
                    resultArr = [];
                }

                if(docs.more == true) {
                    let pkArr = Object.keys(docs.rows[0])
                    let privateKeyVal = pkArr[0];
                    let checkVal = docs.rows[docs.rows.length-1][privateKeyVal];

                    docs.rows.splice(docs.rows.length-1, 1);
                    resultArr = resultArr.concat(docs.rows);

                    await this.getTservant(false, user, checkVal);
                    resolve(resultArr);

                } else if(docs.more == false) {

                    resultArr = resultArr.concat(docs.rows);                    
                    resolve(resultArr);
                }
                    
            }).catch(e => {
                console.log(e)
                reject(e);
            });
        });
    },
    
    getTmonster : function(init, user, pkValue) {
        return new Promise((resolve, reject) => {
            let lowerBound = typeof pkValue != 'undefined' ? pkValue : null

            eos.getTableRows({
                code: config.GAME_CONTRACT,
                scope: user,
                table: 'tmonster',
                limit: 1000,
                json: true,
                lower_bound: lowerBound
            }).then(async (docs) => {
                if(init == true) {
                    resultArr = [];
                } 
                 
                if(docs.more == true) {
                    let pkArr = Object.keys(docs.rows[0])
                    let privateKeyVal = pkArr[0];
                    let checkVal = docs.rows[docs.rows.length-1][privateKeyVal];

                    docs.rows.splice(docs.rows.length-1, 1);
                    resultArr = resultArr.concat(docs.rows);

                    await this.getTmonster(false, user, checkVal);
                    resolve(resultArr);
                } else if(docs.more == false) {
                    resultArr = resultArr.concat(docs.rows);
                    resolve(resultArr);
                }
                    
            }).catch(e => {
                reject(e);
            });
        });
    },
    
    getTequipments : function(init, user, pkValue) {
        return new Promise(async (resolve, reject) => {
            let lowerBound = typeof pkValue != 'undefined' ? pkValue : null

            eos.getTableRows({
                code: config.GAME_CONTRACT,
                scope: user,
                table: 'tequipments',
                limit: 1000,
                json: true,
                lower_bound: lowerBound
            }).then(async (docs) => {
                if(init == true) {
                    resultArr = [];
                }  

                if(docs.more == true) {
                    let pkArr = Object.keys(docs.rows[0])
                    let privateKeyVal = pkArr[0];
                    let checkVal = docs.rows[docs.rows.length-1][privateKeyVal];

                    docs.rows.splice(docs.rows.length-1, 1);
                    resultArr = resultArr.concat(docs.rows);

                    await this.getTequipments(false, user, checkVal);
                    resolve(resultArr);
                } else if(docs.more == false) {
                    resultArr = resultArr.concat(docs.rows);
                    resolve(resultArr);
                }
                
            }).catch(e => {
                reject(e);
            });
        });
    },
    
    getTitem : function(init, user, pkValue) {
        return new Promise((resolve, reject) => {
            let lowerBound = typeof pkValue != 'undefined' ? pkValue : null

            eos.getTableRows({
                code: config.GAME_CONTRACT,
                scope: user,
                table: 'titem',
                limit: 1000,
                json: true,
                lower_bound: lowerBound
            }).then(async (docs) => {
                if(init == true) {
                    resultArr = [];
                }
                
                if(docs.more == true) {
                    let pkArr = Object.keys(docs.rows[0])
                    let privateKeyVal = pkArr[0];
                    let checkVal = docs.rows[docs.rows.length-1][privateKeyVal];

                    docs.rows.splice(docs.rows.length-1, 1);
                    resultArr = resultArr.concat(docs.rows);

                    await this.getTitem(false, user, checkVal);
                    resolve(resultArr);
                } else if(docs.more == false) {
                    resultArr = resultArr.concat(docs.rows);
                    resolve(resultArr);
                }

            }).catch(e => {
                reject(e);
            });
        });
    },
    
    getAccounts : function(init, user, pkValue) {
        return new Promise((resolve, reject) => {
            let lowerBound = typeof pkValue != 'undefined' ? pkValue : null

            eos.getTableRows({
                code: config.GAME_CONTRACT,
                scope: user,
                limit: 1000,
                table: 'accounts',
                json: true,
                lower_bound: lowerBound
            }).then(async (docs) => {
                if(init == true) {
                    resultArr = [];
                }

                if(docs.more == true) {
                    let pkArr = Object.keys(docs.rows[0])
                    let privateKeyVal = pkArr[0];
                    let checkVal = docs.rows[docs.rows.length-1][privateKeyVal];

                    docs.rows.splice(docs.rows.length-1, 1);
                    resultArr = resultArr.concat(docs.rows);

                    await this.getAccounts(false, user, checkVal);
                    resolve(resultArr);
                } else if(docs.more == false) {
                    resultArr = resultArr.concat(docs.rows);
                    resolve(resultArr);
                }

            }).catch(e => {
                reject(e);
            });
        });
    },
    
    getTpartylist : function(init, user, pkValue) {
        return new Promise((resolve, reject) => {
            let lowerBound = typeof pkValue != 'undefined' ? pkValue : null

            eos.getTableRows({
                code: config.GAME_CONTRACT,
                scope: user,
                limit: 1000,
                table: 'tpartylist',
                json: true,
                lower_bound: lowerBound
            }).then(async (docs) => {
                if(init == true) {
                    resultArr = [];
                }
                  
                if(docs.more == true) {
                    let pkArr = Object.keys(docs.rows[0])
                    let privateKeyVal = pkArr[0];
                    let checkVal = docs.rows[docs.rows.length-1][privateKeyVal];

                    docs.rows.splice(docs.rows.length-1, 1);
                    resultArr = resultArr.concat(docs.rows);

                    await this.getTpartylist(false, user, checkVal);
                    resolve(resultArr);
                } else if(docs.more == false) {
                    resultArr = resultArr.concat(docs.rows);
                    resolve(resultArr);
                }

            }).catch(e => {
                reject(e);
            });
        });
    },
    
    getEosioAccounts : function(init, user, pkValue) {
        return new Promise((resolve, reject) => {
            let lowerBound = typeof pkValue != 'undefined' ? pkValue : null

            eos.getTableRows({
                code: 'eosio.token',
                scope: user,
                limit: 1000,
                table: 'accounts',
                json: true,
                lower_bound: lowerBound
            }).then(async (docs) => {
                if(init == true) {
                    resultArr = [];
                }
                   
                if(docs.more == true) {
                    let pkArr = Object.keys(docs.rows[0])
                    let privateKeyVal = pkArr[0];
                    let checkVal = docs.rows[docs.rows.length-1][privateKeyVal];

                    docs.rows.splice(docs.rows.length-1, 1);
                    resultArr = resultArr.concat(docs.rows);

                    await this.getEosioAccounts(false, user, checkVal);
                    resolve(resultArr);
                } else if(docs.more == false) {
                    resultArr = resultArr.concat(docs.rows);
                    resolve(resultArr);
                }

            }).catch(e => {
                reject(e);
            });
        });
    },
    
    getTdaily : function(init, user) {
        return new Promise((resolve, reject) => {
            eos.getTableRows({
                code: config.GAME_CONTRACT,
                scope: config.GAME_CONTRACT,
                lower_bound: user,
                limit: 1,
                table: 'tdaily',
                json: true
            }).then(async (docs) => {
                // init 조건 및 concat 제거 가능하지만 통일성을 위해 남겨놓음
                if(init == true) {
                    resultArr = [];
                }

                resultArr = resultArr.concat(docs.rows);
                resolve(resultArr);
                
            }).catch(e => {
                reject(e);
            });
        });
    }
}