const mongoose = require( 'mongoose' );
const Schema   = mongoose.Schema;

const UserAuth = new Schema({
    "userName" : {type: String, index: {unique:true}},
    "tuserauth" : [Object]
});

const UserServant = new Schema({
    "userName" : {type: String, index: {unique:true}},
    "tservant" : [Object]
});

const UserMonster = new Schema({
    "userName" : {type: String, index: {unique:true}},
    "tmonster" : [Object]
});

const UserEquipments = new Schema({
    "userName" : {type: String, index: {unique:true}},
    "tequipments" : [Object]
});

const UserItem = new Schema({
    "userName" : {type: String, index: {unique:true}},
    "titem" : [Object]
});

const UserAccounts = new Schema({
    "userName" : {type: String, index: {unique:true}},
    "accounts" : [Object]
});

const UserPartylist = new Schema({
    "userName" : {type: String, index: {unique:true}},
    "tpartylist" : [Object]
});

const UserEosioAccounts = new Schema({
    "userName" : {type: String, index: {unique:true}},
    "eosioaccounts" : [Object]
});

const UserDaily = new Schema({
    "userName" : {type: String, index: {unique:true}},
    "tdaily" : [Object]
});

// create indices
UserAuth.index({userName:-1});
UserServant.index({userName:-1});
UserMonster.index({userName:-1});
UserEquipments.index({userName:-1});
UserItem.index({userName:-1});
UserAccounts.index({userName:-1});
UserPartylist.index({userName:-1});
UserEosioAccounts.index({userName:-1});
UserDaily.index({userName:-1});

//create model
mongoose.model('UserAuth', UserAuth);
mongoose.model('UserServant', UserServant);
mongoose.model('UserMonster', UserMonster);
mongoose.model('UserEquipments', UserEquipments);
mongoose.model('UserItem', UserItem);
mongoose.model('UserAccounts', UserAccounts);
mongoose.model('UserPartylist', UserPartylist);
mongoose.model('UserEosioAccounts', UserEosioAccounts);
mongoose.model('UserDaily', UserDaily);

//module exports
module.exports.UserAuth = mongoose.model('UserAuth');
module.exports.UserServant = mongoose.model('UserServant');
module.exports.UserMonster = mongoose.model('UserMonster');
module.exports.UserEquipments = mongoose.model('UserEquipments');
module.exports.UserItem = mongoose.model('UserItem');
module.exports.UserAccounts = mongoose.model('UserAccounts');
module.exports.UserPartylist = mongoose.model('UserPartylist');
module.exports.UserEosioAccounts = mongoose.model('UserEosioAccounts');
module.exports.UserDaily = mongoose.model('UserDaily');

//db connection
mongoose.connect(process.env.MONGO_URI || 'mongodb://localhost/untowerUserInfo');
