# UnlimitedTowerContract
EOS Test Version


Server Coding Protocol - By Canie								
					
* 구분선에 제한이 있음(a~z 1~5까지만 사용 가능)

* 행동과 계산하는 함수는 분리 하여야 한다.

* 결과를 Return 받는 함수는 명칭에 get을 붙인다

* 데이터를 settiong 하는 함수는 명칭에 set을 붙인다
 (+ init,rand 등등)

* stdafx.hpp 에 헤더파일은 모아둔다.

* table에 데이터를 검사하기 위해 enum을 통해 정의를 해준다. (임시)

* 명명 방법은 [사용처 + 키워드] Ex) gf = global funtion , gt = global table

* 예외 처리에 신경 써야 한다

키워드		이니셜
Class(클래스)	c
Struct(구조체)	s
Function(함수)	f
Table(테이블)	t
			

사용처		이니셜
Local(로컬)	l
Inlint(인라인)	i
Global(전역)	g
User(유저)	u
Varible(변수)	v