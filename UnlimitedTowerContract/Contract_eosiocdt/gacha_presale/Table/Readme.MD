1. servant_table

 struct servant_info                        //서번트 정보 구조체

   uint32_t state;                          //서번트 상태
   uint32_t exp = 0;                        //서번트 경험치
   uint32_t job;                            //서번트 직업 
   uint32_t stat_point = 0;                 //서번트 추가 스텟  포인트 
   appear_info appear;                      //서번트 외형 정보
   status_info status;                      //기본 힘,민,지 추가 힘,민,지
   std::vector<uint32_t> equip_slot;        //서번트 장비 리스트


class cservant 

    uint64_t index;                         //서번트 클래스 인덱스 
    uint32_t party_number;                  //파티 번호 
    servant_info servant;                   //서번트 구조체 

    cservant()                              //클래스 cservant 생성자, 빈 파티
    uint64_t primary_key()                  //기본키 = 서번트 클래스 인덱스 


2. monster_table 

 struct monster_info                        //몬스터 정보 구조체 

   uint32_t state;                          //몬스터 상태값
   uint32_t exp = 0;                        //경험치
   uint32_t type = 0;                       //속성 타입
   uint32_t look;
   uint32_t grade;                          //등급
   uint32_t upgrade = 0;                    //강화수치
   status_info status;                      //기본 힘,민,지 추가 힘,민,지


class cmonster

    uint64_t index;                         //몬스터 클래스 인덱스
    uint32_t party_number;                  //파티 번호
    monster_info monster;                   //몬스터 구조체

    cmonster()                              //클래스 cmonster 생성자, 빈파티 
    uint64_t primary_key()                  //기본키 = 몬스터 클래스 인덱스



3. party_table              

enum party_state                            //파티 종류

    on_wait = 1,                            //대기 
    on_tower_defense,                       //타워 디펜스


class cparty                                //파티 클래스 
     
    uint32_t index;                         //유저 클래스 인덱스  
    uint32_t state;                         //파티 스테이터스
    std::vector<uint64_t> party;            //파티를 벡터로 관리 

    cparty()                                //클래서 cparty 생성자, 상태 = 대기 
    uint64_t primary_key()                  //기본키 = 유저 클래스 인덱스
