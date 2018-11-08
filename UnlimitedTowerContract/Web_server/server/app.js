'use strict';

var express             = require('express');
var path                = require('path');
var morgan              = require('morgan');
var moment              = require('moment');
var bodyParser          = require('body-parser');

require('env2')('config.env');

var test = require('./routes/test');

var app = express();

app.get('/', function(req, res) {
    res.send('ok');
});

app.get('/test', test.test);
app.get('/get_table_info',test.getinfo);


module.exports = app;