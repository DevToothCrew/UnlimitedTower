<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"%>
<!DOCTYPE html>
<html lang="en">
<head>
   <meta charset="UTF-8">
   <title>eosTock Test</title>
</head>
<body>
<span id="identity"></span>
<button onclick="login()">로그인</button>
<button onclick="logout()">로그아웃</button>
<button onclick="transaction()">transcrion</button>
<script src="https://cdn.jsdelivr.net/npm/eosjs@16.0.8/lib/eos.min.js"
       integrity="sha512-zhPSKFEBlDVvUzjl9aBS66cI8tDYoLetynuKvIekHT8NZZ12oxwcZ//M/eT/2Rb/pR/cjFvLD8104Cy//sdEnA=="
       crossorigin="anonymous"></script>
<script>
   let eos = null;
   let account = null;
   let eosTock = null;
   document.addEventListener('EOSTOCK_CONNECTED', () => {
       eosTock = window.eosTock;
       window.eosTock = null;
       console.log(111);
   });
   function login() {
       eosTock.login([{
           // httpEndPoint: 'https://kylin.eoscanada.com',
           // chainId: '5fff1dae8dc8e2fc4d5b23b2c7665c97f9e9d8edf2b6485a86ba311c25639191',
           httpEndPoint: 'https://proxy.eosnode.tools:443',
           chainId: 'aca376f206b8fc25a6ed44dbdc66547c36c6c33e3a119ffbeaef943642f0e906',
       }]).then((ssoToken) => {
           document.getElementById('identity').innerText = ssoToken.account;
           account = ssoToken.account;
           const config = {
               // httpEndpoint: 'https://kylin.eoscanada.com',
               // chainId: '5fff1dae8dc8e2fc4d5b23b2c7665c97f9e9d8edf2b6485a86ba311c25639191',
               httpEndPoint: 'https://proxy.eosnode.tools:443',
               chainId: 'aca376f206b8fc25a6ed44dbdc66547c36c6c33e3a119ffbeaef943642f0e906',
           };
           eos = eosTock.eos(config, Eos);
       });
   }
   function logout() {
       eosTock.logout().then(() => {
           document.getElementById('identity').innerText = '';
       })
   }
   function transaction() {
       eos.transfer(
           account,
           'itamnetwork2',
           '0.1000 EOS',
           'gagsgdsgds').then(console.log).catch(console.error);
   }
</script>
</body>
</html>