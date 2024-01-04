# Operation ZERO

[[English Readme]](https://github.com/lunetis/AceCombatZERO/blob/main/README.md)

에이스 컴뱃 제로의 마지막 미션 "ZERO"를 유니티 엔진으로 구현하는 프로젝트입니다.

- __Editor version: 2020.2.3f1__
- __프로젝트를 확인하기 위해서는 [Blender](https://www.blender.org/download/)를 설치해주세요.__

> 참고: 만약 비행 슈팅 게임을 만들기 위해 프로젝트를 참고하려 한다면, 좀 더 개선된 코드를 사용하고 있는 [Operation Maverick](https://github.com/lunetis/OperationMaverick)을 살펴보시는 것을 추천드립니다.

<br>

![](https://github.com/lunetis/AceCombatZERO/blob/main/itchio.PNG)

### itch.io에서 게임을 플레이할 수 있습니다.

####  (웹에서 실행 가능, Windows 스탠드얼론 빌드 지원)

> https://lunetis.itch.io/operation-zero

<br>

* 개인 프로젝트~~이며 현재 개발중~~입니다.
* 듀얼쇼크 4 컨트롤러 (플레이스테이션 4 전용 컨트롤러)를 사용해서 프로젝트를 진행하고 있기 때문에, XBOX 컨트롤러를 포함한 다른 컨트롤러가 제대로 지원되는지는 아직 확인하지 못했습니다.
* 에셋의 라이센스 문제로 인해, 때때로 커밋 내역이 초기화될 수 있습니다.
* 이 프로젝트에 사용된 에셋은 [LICENSE.md](https://github.com/lunetis/AceCombatZERO/blob/main/LICENSE.md) 파일을 확인해주세요.

### TODO List:
- [x] 입력
- [x] 비행기 조종
- [x] 카메라 조종
- [x] 미사일
- [x] 기총
- [x] UI
- [x] 미사일/카메라 락온
- [x] 비행기 조종 구현 심화 (오토파일럿, 실속 등)
- [x] 데미지 시스템
- [x] 적 AI
  - [x] 자동비행
  - [x] 미사일 회피
  - [x] 플레이어 공격
- [x] 게임 오버 연출
- [x] 사운드
- [x] 페이즈 시스템/보스 특수무기
- [x] 일시정지/게임 오버 UI
- [x] 대사 및 자막
- [x] 미션 컷씬
  - [x] 페이즈 3 컷씬
  - [x] 엔딩 컷씬
- [x] 결과 화면, 메인 화면
- [x] 다국어 지원 (한국어, 영어)
- [x] 빌드 테스트

<br>

*이 리스트는 개발자 마음대로 바뀔 수 있습니다.*

Always TODO:
- 애니메이션
- 그래픽 및 연출 강화
- 최적화

<br>

[![](https://img.youtube.com/vi/gNUb0X9rimQ/0.jpg)](https://www.youtube.com/watch?v=gNUb0X9rimQ)

(YouTube 링크 이미지)

<br>

![](https://github.com/lunetis/AceCombatZERO/blob/main/0810.gif)

[구현 예시]

devlog : https://velog.io/@lunetis/series/Ace-Combat-Zero


---

### 참고: Missing prefab 에러 해결 방법

![](https://github.com/lunetis/AceCombatZERO/blob/main/missingerror.PNG)

Blender가 설치되어 있지 않은 상태에서 프로젝트를 열 때 위와 같은 에러가 발생할 수 있습니다.

[Blender](https://www.blender.org/download/)를 설치한 후, Unity 에디터의 상단 메뉴에서 Assets - Reimport All 을 실행해주세요.

(한 번 에러가 발생한 상태에서는 단순 설치만으로 에러가 해결되지 않으며, 에셋들을 다시 임포트해야 합니다.)

<br>

프로젝트를 최초로 열 때 Blender가 이미 설치되어 있는 상태라면 위 에러가 발생하지 않습니다.
