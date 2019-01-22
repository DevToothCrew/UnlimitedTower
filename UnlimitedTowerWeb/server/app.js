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
app.post('/getServant', user.getServantTable);
app.post('/getItem', user.getItemTable);
app.post('/getMonster', user.getMonsterTable);
app.post('/prelogin', user.getPreLogindata);
app.post('/presignup', user.preSignUp);

// 전투 관련 API
app.post('/battle', battle.battleAction);
app.post('/getStage', battle.getStageInfo);

// 가챠 API
app.post('/gacha', gacha.gacha);
app.post('/pregacha', gacha.pregacha);

// 파티 관련 API
app.post('/setFormation', party.setFormation);

// 시드 관련 API
app.post('/seed', seed.createSeed);

module.exports = app;