'use strict';

var express             = require('express');
var path                = require('path');
var morgan              = require('morgan');
var bodyParser          = require('body-parser');
var cors                = require('cors');
require('env2')('config.env');

var app = express();

var gacha = require('./routes/gacha');
var battle = require('./routes/battle');
var user = require('./routes/user');
var party = require('./routes/party.js');
var seed = require('./routes/seed');
var test =  require('./routes/test');
var log = require('./routes/log.js');
var servant = require('./routes/servant');
var monster = require('./routes/monster');
var item = require('./routes/item');
var mail = require('./routes/mail');
var chat = require('./routes/chat');
var pvp = require('./routes/pvp');
var tower = require('./routes/tower');
var store = require('./routes/store');
var dailyCheck = require('./routes/dailycheck');

var Logger = require('./lib/Logger');
global.logger = new Logger();

// engine setup
app.use(cors());
app.use(morgan('dev'));
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({extended: false}));
app.use(express.static(path.join(__dirname, 'public')));

//API

app.get('/', function(req, res) {
    res.send('Devtooth Server is running');
});

// 유저 정보 관련 API
app.post('/login', user.getLoginInfo);
app.post('/signup', user.signUp);
app.post('/userInfo', user.getUserData);
app.post('/newSignUp', user.newSignup);

// 전투 관련 API
app.post('/battle', battle.battleAction);
app.post('/battlestart', battle.battleStart);
app.post('/battlereward', battle.battleReward);
app.post('/getBattle', battle.getBattle);
app.post('/stageEscape', battle.stageEscape);

// PVP 관련 API
app.post('/pvpAction', pvp.pvpAction);
app.post('/pvpStart', pvp.pvpStart);
app.post('/pvpResult', pvp.pvpResult);
app.post('/getPvp', pvp.getPvp);
app.post('/pvpEscape', pvp.pvpEscape);

// 가챠 API
app.post('/gacha', gacha.gacha);
app.post('/testgacha', gacha.testGacha);
app.post('/tengacha', gacha.tenGacha);

// 파티 관련 API
app.post('/saveParty', party.saveParty);

// 시드 관련 API
app.post('/seed', seed.createSeed);

// Burn 관련 API
app.post('/burn', servant.burn);

// Servant 관련 API
app.post('/servantEquip', servant.equip);
app.post('/servantLimitBreak',servant.limitbreak);
// Monster 관련 API
app.post('/monsterUpgrade', monster.upgrade);
app.post('/monsterLimitBreak',monster.limitbreak);

// Item 관련 API
app.post('/equipUpgrade', item.upgrade);
app.post('/itemBuy', item.buy);

// 우편 관련 API
app.post('/getMail', mail.get);
app.post('/mailOpen', mail.open);
app.post('/getMailPage', mail.getFromPage);
app.post('/mailOpenAll', mail.openAllMailInPage);

// 상점 관련 API
app.post('/getStoreList', store.get);
app.post('/buyItem', store.buyItem);

// 메인넷 관련 API
app.post('/mainpresignup', user.mainPreSignup);
app.post('/mainpregacha', gacha.mainPreGacha);

// 채팅 관련 API
app.post('/chatRegist', chat.chatRegist);

// 출석체크 관련 API
app.post('/dailyCheck', dailyCheck.check);

// 로그 관련 API
app.post('/getLog', log.showLog);
app.post('/setLog', log.setLog);

// 테스트
//app.post('/insert', test.adddb);

module.exports = app;