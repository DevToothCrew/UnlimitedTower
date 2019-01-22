import React from 'react';
import ScatterJS from 'scatterjs-core';
import ScatterEOS from 'scatterjs-plugin-eosjs';
import Eos from 'eosjs';
import './App.css';
import Unity, { UnityContent } from "react-unity-webgl";

// 스캐터 사용하기 위해 객체 생성 및 옵션 설정
ScatterJS.plugins( new ScatterEOS() );
const network = {
    blockchain:'eos',
    protocol:'https',
    host:'jungle2.cryptolions.io',
    port:443,
    chainId:'e70aaab8997e1dfce58fbfac80cbbb8fecec7b99cf982a9444273cbc64c41473'
}

// 유니티 객체를 전역으로 설정
let unityContent = new UnityContent(
    "http://192.168.219.116:3000/Build/BUILD_KIBAEK1.json",
    "http://192.168.219.116:3000/Build/UnityLoader.js",
    {
        adjustOnWindowResize: true
    }
  ); 
  
class App extends React.Component {
    
    constructor(props){
        super(props);

        // 유니티에서 실행 된 함수를 unityContent 객체로 받아서 실행
        unityContent.on("SignUp", function(){
            ScatterJS.scatter.connect('My-App').then(connected => {
                if(!connected) return false;
                const scatter = ScatterJS.scatter;
                const requiredFields = { accounts:[network] };
                scatter.getIdentity(requiredFields).then(() => {
                    const account = scatter.identity.accounts.find(x => x.blockchain === 'eos');
                    const eosOptions = { expireInSeconds:60 };
                    const eos = scatter.eos(network, Eos, eosOptions);
                    /*
                    eos.transaction({
                        actions:[
                            {
                                account : 'canieparty22',
                                name : 'signup',
                                authorization:[{
                                    actor : account.name,
                                    permission : account.authority
                                }],
                                data : {
                                    _user : account.name
                                }
                            }                  
                        ]
                    });  */


                    eos.transfer(account.name, 'unlimittest1', '1.0000 EOS', 'signup').then(trx => {
                        console.log(`Transaction ID: ${trx.transaction_id}`);
                    }).catch(error => {
                        console.error(error);
                    });
                    

                    const request = require('superagent');
                    const url='https://dcugl.com:5000/signup';
                    request.post(url)
                        .set('Content-Type', 'application/json')
                        .send({user : account.name})
                        .then(result=>{
                        const data = JSON.stringify(result.body);
                        console.log(result.body);
                        unityContent.send("PacketManager", "ResponseLogin", data);
                        
                    }).catch(error=>{
                        console.error(error);
                    });
                });
            });
        });

        unityContent.on("Gacha", function(){
            ScatterJS.scatter.connect('My-App').then(connected => {
                if(!connected) return false;
                const scatter = ScatterJS.scatter;
                const requiredFields = { accounts:[network] };
                scatter.getIdentity(requiredFields).then(() => {
                    const account = scatter.identity.accounts.find(x => x.blockchain === 'eos');
                    const eosOptions = { expireInSeconds:60 };
                    const eos = scatter.eos(network, Eos, eosOptions);
                    
                    const transactionOptions = { authorization:[`${account.name}@${account.authority}`] };

                    // POST Request
                    const request = require('superagent');
                    const url='https://dcugl.com:5000/seed';
                    request.post(url)
                        .set('Content-Type', 'application/json')
                        .send({user : account.name})
                        .then(result=>{
                            console.log(result.body.seed);
                            console.log(result.body.num);
                            eos.transfer(account.name, 'unlimittest1', '1.0000 EOS', 'gacha:'+result.body.num+':'+result.body.seed, transactionOptions).then(trx => {
                                console.log(`Transaction ID: ${trx.transaction_id}`);
                            }).catch(error => {
                                console.error(error);
                            });
                    }).catch(error=>{
                        console.error(error);
                    });
                    //
                    // POST Request
                    const gachaurl='https://dcugl.com:5000/gacha';
                    request.post(gachaurl)
                        .set('Content-Type', 'application/json')
                        .send({user : account.name})
                        .then(result=>{
                            const data = JSON.stringify(result.body);
                            unityContent.send("PacketManager", "ResponseGacha", data);
                            console.log(data);
                    }).catch(error=>{
                        console.error(error);
                    });
                    // 
                }).catch(error => {
                    console.error(error);
                });   
            });
        });

        unityContent.on("Login", function(){
            ScatterJS.scatter.connect('My-App').then(connected => {
                if(!connected) return false;
                const scatter = ScatterJS.scatter;
                const requiredFields = { accounts:[network] };
                scatter.getIdentity(requiredFields).then(() => {
                    const account = scatter.identity.accounts.find(x => x.blockchain === 'eos');
                    const eosOptions = { expireInSeconds:60 };
                    const eos = scatter.eos(network, Eos, eosOptions);

                    // POST Request
                    const request = require('superagent');
                    const url='https://dcugl.com:5000/login';
                    request.post(url)
                        .set('Content-Type', 'application/json')
                        .send({user : account.name})
                        .then(result=>{
                        const data = JSON.stringify(result.body);
                        console.log(result.body);
                        if(result.signup == null)
                        {
                            eos.transfer(account.name, 'unlimittest1', '1.0000 EOS', 'signup').then(trx => {
                                console.log(`Transaction ID: ${trx.transaction_id}`);
                            }).catch(error => {
                                console.error(error);
                            });
                            const url='https://dcugl.com:5000/signup';
                            request.post(url)
                                 .set('Content-Type', 'application/json')
                                 .send({user : account.name})
                                 .then(result=>{
                                 const data = JSON.stringify(result.body);
                                 console.log(result.body);
                                 unityContent.send("PacketManager", "ResponseLogin", data);
                        
                    }).catch(error=>{
                        console.error(error);
                    });
                        }
                        else
                        unityContent.send("PacketManager", "ResponseLogin", data);
                        
                    }).catch(error=>{
                        console.error(error);
                    });
                    // 
                }).catch(error => {
                    console.error(error);
                });   
            });
        });

        unityContent.on("SetFormation", data => {
            ScatterJS.scatter.connect('My-App').then(connected => {
                if(!connected) return false;
                const scatter = ScatterJS.scatter;
                const requiredFields = { accounts:[network] };
                scatter.getIdentity(requiredFields).then(() => {
                    const account = scatter.identity.accounts.find(x => x.blockchain === 'eos');
                    const eosOptions = { expireInSeconds:60 };
                    const eos = scatter.eos(network, Eos, eosOptions);
                  
                    const value = JSON.parse(data);
                    eos.transaction({
                        actions:[
                            {
                                account : 'canieparty22',
                                name : 'setparty',
                                authorization:[{
                                    actor : account.name,
                                    permission : account.authority
                                }],
                                data : {
                                    _user : account.name,
                                    _party_number : value.partyNum,
                                    _party_list : value.partyList
                                }
                            }
                        ]
                    });  
                    // POST Request
                    const request = require('superagent');
                    const url='https://dcugl.com:5000/setFormation';
                    request.post(url)
                        .set('Content-Type', 'application/json')
                        .send({
                            user : account.name,
                            index: value.partyNum,
                            party: value.partyList
                        })
                        .then(result=>{
                            const data = JSON.stringify(result.body);
                            console.log(data);
                            unityContent.send("PacketManager", "ResponseGetParty", data);
                        
                    }).catch(error=>{
                        console.error(error);
                    });
                    // 
                }).catch(error => {
                    console.error(error);
                });   
            });


        });
        


        unityContent.on("GetServant", () => {
            ScatterJS.scatter.connect('My-App').then(connected => {
                if(!connected) return false;
                const scatter = ScatterJS.scatter;
                const requiredFields = { accounts:[network] };
                scatter.getIdentity(requiredFields).then(() => {
                    const account = scatter.identity.accounts.find(x => x.blockchain === 'eos');
                   
                    // POST Request
                    const request = require('superagent');
                    const url='https://dcugl.com:5000/getServant';
                    request.post(url)
                        .set('Content-Type', 'application/json')
                        .send({user : account.name})
                        .then(result=>{
                        const data = JSON.stringify(result.body);
                        console.log(result);
                        unityContent.send("PacketManager", "ResponseGetAllServant", data);

                    }).catch(error=>{
                        console.error(error);
                    });
                    // 
                }).catch(error => {
                    console.error(error);
                });   
            });
        });

        unityContent.on("GetItem", () => {
            ScatterJS.scatter.connect('My-App').then(connected => {
                if(!connected) return false;
                const scatter = ScatterJS.scatter;
                const requiredFields = { accounts:[network] };
                scatter.getIdentity(requiredFields).then(() => {
                    const account = scatter.identity.accounts.find(x => x.blockchain === 'eos');
                   
                    // POST Request
                    const request = require('superagent');
                    const url='https://dcugl.com:5000/getItem';
                    request.post(url)
                        .set('Content-Type', 'application/json')
                        .send({user : account.name})
                        .then(result=>{
                        const data = JSON.stringify(result.body);
                        console.log(result);
                        unityContent.send("Packet", "TestReceive", data);

                    }).catch(error=>{
                        console.error(error);
                    });
                    // 
                }).catch(error => {
                    console.error(error);
                });   
            });
        });

        unityContent.on("GetMonster", () => {
            ScatterJS.scatter.connect('My-App').then(connected => {
                if(!connected) return false;
                const scatter = ScatterJS.scatter;
                const requiredFields = { accounts:[network] };
                scatter.getIdentity(requiredFields).then(() => {
                    const account = scatter.identity.accounts.find(x => x.blockchain === 'eos');
                   
                    // POST Request
                    const request = require('superagent');
                    const url='https://dcugl.com:5000/getMonster';
                    request.post(url)
                        .set('Content-Type', 'application/json')
                        .send({user : account.name})
                        .then(result=>{
                        const data = JSON.stringify(result.body);
                        console.log(result);
                        unityContent.send("PacketManager", "ResponseGetAllMonster", data);

                    }).catch(error=>{
                        console.error(error);
                    });
                    // 
                }).catch(error => {
                    console.error(error);
                });   
            });
        });

        unityContent.on("Logout", () => {
            ScatterJS.scatter.connect('My-App').then(connected => {
                if(!connected) return false;
                const scatter = ScatterJS.scatter;
               scatter.forgetIdentity();
               unityContent.send("PacketManager", "ResponseGetAllMonster", false);
            });
        });
    }

    stressTest = () => {
        ScatterJS.scatter.connect('My-App').then(connected => {
            if(!connected) return false;
            const scatter = ScatterJS.scatter;
            const requiredFields = { accounts:[network] };
            scatter.getIdentity(requiredFields).then(() => {
                const account = scatter.identity.accounts.find(x => x.blockchain === 'eos');
                const eosOptions = { expireInSeconds:60 };
                const eos = scatter.eos(network, Eos, eosOptions);

                // POST Request
                const request = require('superagent');
                const url='https://dcugl.com:5000/login';
                request.post(url)
                    .set('Content-Type', 'application/json')
                    .send({user : account.name})
                    .then(result=>{
                    const data = JSON.stringify(result.body);
                    console.log(result.body);
                    if(result.signup == null)
                    {
                        eos.transfer(account.name, 'unlimittest1', '1.0000 EOS', 'signup').then(trx => {
                            console.log(`Transaction ID: ${trx.transaction_id}`);
                        }).catch(error => {
                            console.error(error);
                        });
                        const url='https://dcugl.com:5000/signup';
                        request.post(url)
                             .set('Content-Type', 'application/json')
                             .send({user : account.name})
                             .then(result=>{
                             const data = JSON.stringify(result.body);
                             console.log(result.body);
                             unityContent.send("PacketManager", "ResponseLogin", data);
                    
                }).catch(error=>{
                    console.error(error);
                });
                    }
                    else
                    unityContent.send("PacketManager", "ResponseLogin", data);
                    
                }).catch(error=>{
                    console.error(error);
                });
                // 
            }).catch(error => {
                console.error(error);
            });   
        });
    }

    stressGachaTest = () =>
    {
       
            ScatterJS.scatter.connect('My-App').then(connected => {
                if(!connected) return false;
                const scatter = ScatterJS.scatter;
                const requiredFields = { accounts:[network] };
                scatter.getIdentity(requiredFields).then(() => {
                    const account = scatter.identity.accounts.find(x => x.blockchain === 'eos');
                    const eosOptions = { expireInSeconds:60 };
                    const eos = scatter.eos(network, Eos, eosOptions);
                    
                    const transactionOptions = { authorization:[`${account.name}@${account.authority}`] };

                    // POST Request
                    const request = require('superagent');
                    const url='https://dcugl.com:5000/seed';
                    request.post(url)
                        .set('Content-Type', 'application/json')
                        .send({user : account.name})
                        .then(result=>{
                            console.log(result.body.seed);
                            console.log(result.body.num);
                            eos.transfer(account.name, 'canieparty22', '0.0001 EOS', 'gacha:'+result.body.num+':'+result.body.seed, transactionOptions).then(trx => {
                                console.log(`Transaction ID: ${trx.transaction_id}`);
                            }).catch(error => {
                                console.error(error);
                            });
                    }).catch(error=>{
                        console.error(error);
                    });
                    //
                    // POST Request
                    const gachaurl='https://dcugl.com:5000/gacha';
                    request.post(gachaurl)
                        .set('Content-Type', 'application/json')
                        .send({user : account.name})
                        .then(result=>{
                            const data = JSON.stringify(result.body);
                            console.log(data);
                    }).catch(error=>{
                        console.error(error);
                    });
                    // 
                }).catch(error => {
                    console.error(error);
                });   
            });
        
    }
    testbotton=()=>
    {
        ScatterJS.scatter.connect('My-App').then(connected => {
            if(!connected) return false;
            const scatter = ScatterJS.scatter;
            const requiredFields = { accounts:[network] };
            scatter.getIdentity(requiredFields).then(() => {
                const account = scatter.identity.accounts.find(x => x.blockchain === 'eos');
                const eosOptions = { expireInSeconds:60 };
                const eos = scatter.eos(network, Eos, eosOptions);
                eos.transfer(account.name, 'unlimittest1', '1.0000 EOS', 'signup').then(trx => {
                    console.log(`Transaction ID: ${trx.transaction_id}`);
                }).catch(error => {
                    console.error(error);
                });
                

                const request = require('superagent');
                const url='https://dcugl.com:5000/signup';
                request.post(url)
                    .set('Content-Type', 'application/json')
                    .send({user : account.name})
                    .then(result=>{
                    const data = JSON.stringify(result.body);
                    console.log(result.body);
                    unityContent.send("PacketManager", "ResponseLogin", data);
                    
                }).catch(error=>{
                    console.error(error);
                });
            });
        });

    }

    render() {
        return (
            <div>
            <Unity unityContent={unityContent} className = "unityapp"   />
            <button onClick={this.stressTest}>click yoondaewon</button>
            <button onClick={this.stressGachaTest}>click gacha yoon! 노예로만들어라 윤대원을!</button>
            <button onClick={this.testbotton}>ooooooooooooooooooooooooo</button>
            </div>
        );
    }
}

export default App;