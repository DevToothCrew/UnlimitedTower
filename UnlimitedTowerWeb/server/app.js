'use strict';

var express             = require('express');
var path                = require('path');
var morgan              = require('morgan');
var bodyParser          = require('body-parser');
var cors                = require('cors');
require('env2')('config.env');

var app = express();

var test = require('./routes/test');
var battle = require('./routes/battle');
var user = require('./routes/user');

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

app.post('/login', user.getUserInfo);
app.post('/getServant', user.getServantTable);
app.post('/getItem', user.getItemTable);
app.post('/getMonster', user.getMonsterTable);
app.post('/battle', battle.battleAction);
app.post('/getStage', battle.getStageInfo);
app.post('/gacha', test.gacha);

module.exports = app;