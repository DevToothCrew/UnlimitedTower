##### addblack : 블랙리스트에 사용자 계정 등록 함수 (Only Master Active)
##### create :  토큰을 만드는 액션 (Only Contract, One Time, Owner)
##### dberase : db 내용 선택 삭제 함수 (Only Master Active)
##### dbinsert : db 내용 선택 추가 함수 (Only Master Active)
##### dbmodify : db 내용 선택 수정 함수 (Only Master Active)
##### deleteblack : 블랙리스트에서 사용자 계정 삭제 함수 (Only Master Active)
##### deleteuser : 유저 정보 삭제 함수(Only Master Active)
##### eostransfer : eosio.token 컨트랙트 트랜스퍼 실행 함수
##### initmaster : Master 초기화 함수 (Only Contract Owner)
##### inittokenlog : PreRegist Log 초기화 함수 (Only Master Active)
##### issue : 토큰 발행 함수 (Only Master Active, One Time, Owner)
##### setmaster : Master 이동 함수 (Only Master Owner)
##### setpause : 컨트랙트 일시정지 (Only Master Active)
##### settokenlog : 프리세일 로그 세팅 함수 (Only Master Active)


jungle_test_net_contract  - unlimittest1

### 1. 테스트 과정
* 디비 데이터는 셋팅이 되어있다는 가정입니다.

1. setmaster(eosio::name _master)
* 액션을 실행해준다 해당 액션은 최초 실행시 마스터 계정은 컨트랙트가 되며 이후 실행시 입력하는 계정명에 따라 마스터 권한을 변경해주는 액션이다.
* 인자값으로는 권한을 이동시켜줄 계정명을 넣어줍니다.

2. settokenlog() 
* 액션을 실행해 줍니다. 프리레지스트 로그를 셋팅해줍니다. 프리레지스트 로그가 셋팅이 되어있다면 에러 입니다.  
* 해당 액션은 마스터의 액티브키만 실행이 가능합니다.

3. create(name _issuer, asset _maximum_supply)
* 액션을 통해 UTG 토큰을 생성해 줍니다. 해당 액션은 컨트랙트의 오너 권한만 가능합니다.
* 인자값은 생성자 계정명, 생성할 토큰의 총량과 심벌을 넣어줍니다.

4. issue(name _to, asset _quantity, string _memo)
* 액션을 통해 UTG 토큰을 발행해 줍니다. 해당 액션은 컨트랙트의 오너 권한만 가능합니다.
* 인자밗은 발행을 받는 사람, 발행받을 양과 토큰의 심벌 , 추가로 적어넣을 메모의 내용을 넣어주시면됩니다.

5. 뽑기시에 토큰을 주기위해선 인라인액션으로 transfer를 실행시켜야하는데 인라인액션을 위한 eosio.code 권한이 필요합니다.

* cleos set account permission <YOUR_ACCOUNT> active '{"threshold": 1,"keys": [{"key": "<YOUR_PUBLIC_KEY>","weight": 1}],"accounts": [{"permission":{"actor":"<CONTRACT_ACCOUNT>","permission":"eosio.code"},"weight":1}]}' owner -p <YOUR_ACCOUNT> 

* 위 명령어를 통해 권한을 셋팅해줍니다.

6. 5번까지 완료하였으면 기능테스트를 위한 기본셋팅이 끝났습니다. 

### 2. 유저용 액션

1. transfer(name _from, name _to, asset _quantity, string _memo)
* UTG 토큰을 이동시키는 액션입니다.
* 인자값으로는 보내는계정명, 받는 계정명, 토큰양과 심볼, 보낼 메모를 넣어줍니다.

2. eostransfer(eosio::name sender, eosio::name receiver)
* eosio.token 컨트랙트를 통해 eos를 받을 경우 실행되는 액션입니다.
* 따로 액션만 실행되지는 않습니다.
* 해당 액션에선 사전뽑기, 사전등록, 등록, 뽑기가 가능합니다.


### 3. 마스터용 액션

1. setpause(uint64_t _state)
* 마스터계정의 상태값을 바꾸는 액션입니다.
* 인자값은 스테이트가 들어갑니다. 0이 normal이고 1이 pause입니다.

2. initmaster()
* 마스터계정을 컨트랙트 계정으로 바꿔주는 액션입니다.

3. inittokenlog()
* prelog를 초기화해주는 액션입니다.

4. deleteuser(eosio::name _user)
* 유저의 기본정보를 지워주는 액션입니다.
* 인자값으로는 지울 계정명을 넣어줍니다.

5. deleteblack(eosio::name _user)
* 블랙리스트에서 해당 유저를 지워주는 액션입니다.
* 인자값으로는 지울 계졍명을 넣어 줍니다.

6. addblack(eosio::name _user);
* 블랙리스트에 해당 유저를 추가해주는 액션입니다.
* 인자값으로는 추가할 계정명을 넣어 줍니다.

7. dbinsert, dbmodify, dberase, dbinit
* 해당 액션은 마스터 계정만 가능하고 마스터계정의 상태가 pause 상태인 1이어야 합니다.
* 관련된 문서가 따로 있습니다.


    


