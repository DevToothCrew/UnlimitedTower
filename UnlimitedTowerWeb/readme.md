# API
---
## Project Structure
---
Two servers SAPI (/sapi) and CDS (/cds), code in other directories are shared libraries.
* **scatter:** EOS 네트워크와 어플리케이션을 연결하여 사용자를 인증하기 위해 사용.

* **react:** Scatter와 Uniity 를 연동하여 WebGL로 출력하기 위한 Web Client(superagent,react-unity,scatterjs).

* **webserver:** Node.js express를 사용하여 EOS contract의 Table 정보를 가져오기 위해 설계된 서버.

* **unity:** Main client Developement tool.
 
---
## TRX 실행 도식
---
https://drive.google.com/file/d/1G2GUPN6YSV8xWE9B1Q7KUFy6OozC0Lhj/view?usp=sharing
