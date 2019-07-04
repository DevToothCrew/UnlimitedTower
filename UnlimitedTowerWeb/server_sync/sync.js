/**
 * todo
 * 로그 모듈 사용 추가
 */
require('./db.js');
const fnGetEos = require('./utils/fnGetEos');
const fnGetTable = require('./utils/fnGetEosTable');
const fnAction = require('./utils/fnActions');
const fnQuery = require('./utils/fnQuery');
const configFile = require('./config');
const config = configFile[process.env.NODE_ENV];

const mongoose = require('mongoose');

const CUserAuth = mongoose.model('UserAuth');
const CUserServant = mongoose.model('UserServant');
const CUserMonster = mongoose.model('UserMonster');
const CUserEquipments = mongoose.model('UserEquipments');
const CUserItem = mongoose.model('UserItem');
const CUserAccounts = mongoose.model('UserAccounts');
const CUserPartylist = mongoose.model('UserPartylist');
const CUserEosioAccounts = mongoose.model('UserEosioAccounts');
const CUserDaily = mongoose.model('UserDaily');

let checkedBlock = 0;

const initSync = () => {
    console.log("\n================\t initSync \t================");
    return new Promise(async (resolve, reject) => {

        // db 값 초기화
        await CUserAuth.remove();
        await CUserServant.remove();
        await CUserMonster.remove();
        await CUserEquipments.remove();
        await CUserItem.remove();
        await CUserAccounts.remove();
        await CUserPartylist.remove();
        await CUserEosioAccounts.remove();
        await CUserDaily.remove();
        console.log("================\t mongoDB DataBase untowerUserInfo initialization finished \t================");

        await fnGetEos.getBlockNum().then(blockNumber => {
            tempBlockNum = blockNumber;
            return true;
        });

        let userAuthArr = [];
        let getTableState = true;
        let tableMoreState = true;

        while(getTableState) {
            if(tableMoreState == false) {
                getTableState = false;
                break;
            } else {
                let lastTuserauthValue = userAuthArr.length == 0 ? null : userAuthArr[userAuthArr.length-1].user;

                await fnGetTable.getTuserauth(true, lastTuserauthValue, 1000).then(async docs => {
                    getTableState = true;
                    tableMoreState = docs.more;

                    for(let i=0; i< docs.rows.length; i++) {
                        if(userAuthArr.length != 0 && userAuthArr[userAuthArr.length-1].user == docs.rows[i].user) {
                            continue;
                        }
                        await userAuthArr.push(docs.rows[i]);
                    };

                    return true
                }).catch(e => {
                    reject(e);
                });
            }
        }

        let authObjArr = [];
        let servantObjArr = [];
        let monsterObjArr = [];
        let equipObjArr = [];
        let itemObjArr = [];
        let accObjArr = [];
        let partyObjArr = [];
        let eosioAccObjArr = [];
        let dailyObjArr = [];

        // remove db and insert latest db info
        for(let i=0; i<userAuthArr.length; i++) {
            console.log("User checking == " + userAuthArr.length + " : " + (i+1));
            await Promise.all([fnGetTable.getTservant(true, userAuthArr[i].user), fnGetTable.getTmonster(true, userAuthArr[i].user), fnGetTable.getTequipments(true, userAuthArr[i].user),
                fnGetTable.getTitem(true, userAuthArr[i].user), fnGetTable.getAccounts(true, userAuthArr[i].user), fnGetTable.getTpartylist(true, userAuthArr[i].user), 
                fnGetTable.getEosioAccounts(true, userAuthArr[i].user), fnGetTable.getTdaily(true, userAuthArr[i].user)])
            .then(async (eosTableData) => {
                /**
                 * db insert model
                 * 분리가능하면 분리
                 */
                let authObj = {
                    'userName' : userAuthArr[i].user,
                    'tuserauth' : userAuthArr[i]
                }
                let servantObj = {
                    'userName' : userAuthArr[i].user,
                    'tservant' : eosTableData[0]
                }
                let monsterObj = {
                    'userName' : userAuthArr[i].user,
                    'tmonster' : eosTableData[1]
                }
                let equipObj = {
                    'userName' : userAuthArr[i].user,
                    'tequipments' : eosTableData[2]
                }
                let itemObj = {
                    'userName' : userAuthArr[i].user,
                    'titem' : eosTableData[3]
                }
                let accObj = {
                    'userName' : userAuthArr[i].user,
                    'accounts' : eosTableData[4]
                }
                let partyObj = {
                    'userName' : userAuthArr[i].user,
                    'tpartylist' : eosTableData[5]
                }
                let eosioAccObj = {
                    'userName' : userAuthArr[i].user,
                    'eosioaccounts' : eosTableData[6]
                }
                let dailyObj = {
                    'userName' : userAuthArr[i].user,
                    'tdaily' : eosTableData[7]
                }

                authObjArr.push(authObj);
                servantObjArr.push(servantObj);
                monsterObjArr.push(monsterObj);
                equipObjArr.push(equipObj);
                itemObjArr.push(itemObj);
                accObjArr.push(accObj);
                partyObjArr.push(partyObj);
                eosioAccObjArr.push(eosioAccObj);
                dailyObjArr.push(dailyObj);

            }).catch(e2 => {
                console.log(e2);
                reject(e2);
            });
        }
        //db insert
        await Promise.all([fnQuery.dbInsertMany(CUserAuth, authObjArr), fnQuery.dbInsertMany(CUserServant, servantObjArr), fnQuery.dbInsertMany(CUserMonster, monsterObjArr),
            fnQuery.dbInsertMany(CUserEquipments, equipObjArr), fnQuery.dbInsertMany(CUserItem, itemObjArr), fnQuery.dbInsertMany(CUserAccounts, accObjArr),
            fnQuery.dbInsertMany(CUserPartylist, partyObjArr), fnQuery.dbInsertMany(CUserEosioAccounts, eosioAccObjArr), fnQuery.dbInsertMany(CUserDaily, dailyObjArr)])
        .then(dbEndData => {
            console.log("================\t initSync and db insert finish \t================");
            return true;
        }).catch(e => {
            reject(e);
        });

        resolve(true);
    });
}


const startSync = (checkBlockNum) => {
    // console.log("================\t block sync start \t================");
    return new Promise(async (resolve, reject) => {
        let blockNum = 0;
        
        currentBlock = await fnGetEos.getBlockNum();

        if(currentBlock <= (checkedBlock) && checkedBlock != 0) {
            resolve(true);
            return true;
        } else {
            blockNum = currentBlock;
            checkedBlock = currentBlock;
            // console.log("================\t find new Block And Block Number :: " + blockNum + " \t================");
    
            if(typeof checkBlockNum != 'undefined') {
                blockNum = checkBlockNum;
            }
    
            await fnGetEos.getBlock(blockNum).then(async docs => {
                for(let i=0; i<docs.transactions.length; i++) {
                    if(docs.transactions[i].trx.transaction == undefined) {
                        continue;
                    } else {
                        for(let k=0; k<docs.transactions[i].trx.transaction.actions.length; k++) {
                            
                            if(docs.transactions[i].trx.transaction.actions[k].account != config.GAME_CONTRACT) {
                                continue;
                            } else if(docs.transactions[i].trx.transaction.actions[0].account == config.GAME_CONTRACT) {
    
                                let account = docs.transactions[i].trx.transaction.actions[k].account;
                                let actionName = docs.transactions[i].trx.transaction.actions[k].name;
                                let actionData = docs.transactions[i].trx.transaction.actions[k].data;
                                let transferFrom = docs.transactions[i].trx.transaction.actions[k].data.from != undefined ? docs.transactions[i].trx.transaction.actions[k].data.from : null;
                                let transferTo = docs.transactions[i].trx.transaction.actions[k].data.to != undefined ? docs.transactions[i].trx.transaction.actions[k].data.to : null;
        
                                if(account == 'eosio.token' && actionName == 'transfer' && (transferFrom == config.GAME_CONTRACT || transferTo == config.GAME_CONTRACT)) {
                                    console.log("================ eosioTransfer ================");
                                    await Action["eosioTransfer"](transferFrom).then(actionResult => {
                                        console.log("================ User : " + transferFrom + "  eosioTransfer db save finish ================");
                                        return true;
                                    }).catch(e => {
                                        console.log(e);
                                        throw e;
                                    })
                
                                } else if(account == config.GAME_CONTRACT) {
    
                                    // action 함수에 대한 존재여부 validation check
                                    if(typeof fnAction[actionName] != 'function') {
                                        console.log("!!!!!!!!!!!!! there is no function on same action name !!!!!!!!!!!!!");
                                        continue;
                                    } else if(typeof fnAction[actionName] == 'function') {
    
                                        await fnAction[actionName](actionData).then(actionResult => {
                                            console.log("======================= " + actionName  + " action db save finish =======================");
                                            return true;
                                        }).catch(e => {
                                            console.log(e);
                                            throw e;
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
                console.log("*********************** blockNumber : " + blockNum + " sync end ***********************\n");
                
                resolve(true);
            }).catch(async (err) => {
                console.log(err);
                await startSync(blockNum);
                resolve(true);
            });
        }
        

    });
}

initSync().then(docs => {
    if(docs != true) {
        console.log("!!!!!!!!!!!!!!!!!!\t unknown error on initSync \t!!!!!!!!!!!!!!!!!!");
        initSync();
    } else {
        setInterval(startSync, 100);
    }
}).catch(error => {
    console.log(error);
    initSync();
});
