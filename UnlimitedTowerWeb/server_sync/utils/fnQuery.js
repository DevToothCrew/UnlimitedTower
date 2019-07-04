module.exports = {
    dbInsert : (collectionModel, data) => {
        return new Promise((resolve, reject) => {
            collectionModel.collection.insert(data).then(docs => {
                resolve(true);
            }).catch(e => {
                reject(e);
            });
        });
    },

    dbInsertMany : (collectionModel, dataArr) => {
        return new Promise((resolve, reject) => {
            collectionModel.collection.insertMany(dataArr).then(docs => {
                resolve(true);
            }).catch(e => {
                reject(e);
            });
        });
    },

    dbUpdae : (collectionModel, query) => {
        return new Promise((resolve, reject) => {
            collectionModel.update(query).then(docs => {
                resolve(true);
            }).catch(e => {
                reject(e);
            });
        })
    },

    dbReplace : (collectionModel, query) => {
        return new Promise((resolve, reject) => {
            collectionModel.replaceOne(query).then(docs => {
                resolve(true);
            }).catch(e => {
                reject(e);
            });
        });
    },

    dbSave : (collectionModel, query) => {
        return new Promise((resolve, reject) => {
            collectionModel.find({userName : query.userName}).then(docs => {
                if(docs.length != 0) {
                    query._id = docs[0]._id;
                } 

                collectionModel.collection.save(query).then(docs => {
                    resolve(true);
                }).catch(e1 => {
                    reject(e1);
                });
            }).catch(e2 => {
                reject(e2);
            })
        });
    }
}