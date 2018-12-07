'use strict';

var express             = require('express');
var path                = require('path');
var morgan              = require('morgan');
var bodyParser          = require('body-parser');
require('env2')('config.env');

var app = express();

var test = require('./routes/test');

var Logger = require('./lib/Logger');
global.logger = new Logger();

// engine setup
app.use(morgan('dev'));
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({extended: false}));
app.use(express.static(path.join(__dirname, 'public')));

//API

app.get('/', function(req, res) {
    res.send('Devtooth Server is running');
});

app.post('/eos', test.action_start);
app.post('/get', test.get_table);
app.post('/post', test.post);
app.post('/updateTable', test.table_update);
app.post('/gacha', test.gacha);

module.exports = app;