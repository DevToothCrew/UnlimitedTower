var mysql               = require('mysql');
var config              = require('../config/');

var clusterConfig = {
    removeNodeErrorCount: 1,
    defaultSelector: "ORDER"
};

var poolCluster = mysql.createPoolCluster(clusterConfig);

poolCluster.add("MASTER", config.mysql.master);

exports.PoolCluster = poolCluster;