/**
 암호화
 */
var crypto = require('crypto');

var config = require('../config/');

function Crypt() {}

/**
 * AES256 인코딩
 *
 * @param text
 * @returns {*}
 * @constructor
 */
Crypt.encode = function(jsonData) {
    var func = 'encode';

    /* cipher */
    var algorithm = 'aes256';

    var key;
    var iv;
    var strText = '';
    var ret = '';

    try {
        strText = JSON.stringify(jsonData);
        if (!config.encryption.state) {
            return strText;
        }
        key = new Buffer(config.encryption.key, 'base64');
        iv = new Buffer(config.encryption.iv, 'base64');
        var cipher = crypto.createCipheriv(algorithm, key, iv);
        ret = cipher.update(strText, 'utf-8', 'base64');
        ret += cipher.final('base64');
    } catch (e) {
        logger.error(strText, __filename, func, e.toLocaleString());
    }

    return  ret;
};

/**
 * AES256 디코딩
 *
 * @param text
 * @returns {*}
 * @constructor
 */
Crypt.decode = function(text) {
    var func = "decode";
    /* cipher */
    var algorithm = 'aes256';

    var key;
    var iv;
    var strText = '';
    var jsonData = {};

    try {
        if (!config.encryption.state) {
            jsonData = JSON.parse(text);
            return jsonData;
        }
        key = new Buffer(config.encryption.key, 'base64');
        iv = new Buffer(config.encryption.iv, 'base64');
        strText = text.toString();
        var byText = new Buffer(strText, 'base64');
        var decipher = crypto.createDecipheriv(algorithm, key, iv);
        var ret = decipher.update(byText, 'binary', 'utf-8');
        ret += decipher.final('utf-8');
        jsonData = JSON.parse(ret);
    } catch (e) {
        logger.error(strText, __filename, func, e.toLocaleString());
    }

    return jsonData;
};

/**
 * CSV 파일 인코딩
 *
 * @param strText
 * @returns {*}
 */
Crypt.csvEncode = function(strText) {
    /* cipher */
    var algorithm = 'aes256';

    var key;
    var iv;
    var ret = '';

    try {
        if (!config.encryption.state) {
            return strText;
        }
        key = new Buffer(config.encryption.key, 'base64');
        iv = new Buffer(config.encryption.iv, 'base64');
        var cipher = crypto.createCipheriv(algorithm, key, iv);
        ret = cipher.update(strText, 'utf-8', 'base64');
        ret += cipher.final('base64');
    } catch (e) {
        logger.error(strText, __filename, func, e.toLocaleString());
    }

    return  ret;
};

/**
 * SH1 인코딩
 *
 * @param text
 * @returns {string}
 * @constructor
 */
Crypt.sha1 = function(text) {
    var ret = "";
    var strText;
    try {
        strText = text.toString();
        if (!strText)
            return ret;
        var shasum = crypto.createHash('sha1');
        shasum.update(text);
        ret = shasum.digest('hex');
        return ret;
    } catch (e) {
        return ret;
    }
};

/**
 * MD5 인코딩
 *
 * @param text
 * @returns {string}
 * @constructor
 */
Crypt.md5 = function(text) {
    var ret = "";
    var strText;
    try {
        strText = text.toString();
        if (!strText)
            return ret;
        var shasum = crypto.createHash('md5');
        shasum.update(text);
        ret = shasum.digest('hex');
    } catch (e) {
        console.log(e.toLocaleString());
    }
    return ret;
};

/**
 * Base64 인코딩
 *
 * @param text
 * @returns {string}
 * @constructor
 */
Crypt.encodeBase64 = function(text) {
    var ret = "";
    try {
        if (!text)
            return ret;
        var b = new Buffer(text);
        ret = b.toString('base64');
    } catch (e) {
        console.log(e.toLocaleString());
    }
    return ret;
};

/**
 * Base64 디코딩
 *
 * @param text
 * @returns {string}
 * @constructor
 */
Crypt.decodeBase64 = function(text) {
    var ret = "";
    try {
        if (!text)
            return ret;
        var b = new Buffer(text, 'base64');
        ret = b.toString();
    } catch (e) {
        ret = "";
    }
    return ret;
};

module.exports = Crypt;