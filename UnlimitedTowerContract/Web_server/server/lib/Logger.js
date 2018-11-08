var winston = require('winston');
var moment  = require('moment');

var config  = require('../config/');

module.exports = function() {
    var logger = new winston.Logger({
        transports: [
            new winston.transports.Console({
                level   : config.log.console.level,
                colorize: true
            }),
            new winston.transports.File({
                level   : config.log.file.level,
                filename: __dirname + "/../logs/" + moment(new Date()).format("YYYY-MM-DD") + "_error.log"
            })
        ]
    });

    logger.setLevels(config.logger.levels);
    winston.addColors(config.logger.colors);
    logger.exitOnError = false;

    this.info = function(uidx, file, func, info) {
        var jsonInfo = { user: uidx, file: file, func: func, info: info };
        logger.info(jsonInfo);
    };

    this.debug = function(uidx, file, func, debug) {
        var jsonDebug = { user: uidx, file: file, func: func, debug: debug };
        logger.debug(jsonDebug);
    };

    this.error = function(uidx, file, func, err) {
        var jsonError = { user: uidx, file: file, func: func, error: err };
        logger.error(jsonError);
    };
};