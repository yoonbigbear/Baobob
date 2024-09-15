# BaobabCore
`BaobabCore`는 Baobab 라이브러리에서 제공하는 기본적은 편의 기능들입니다. 예를 들어 UniqueId를 만들어주는 IDGenerator나 Deltatime을 계산하거나 시간 관련 계산을 해주는 GlobalTimer, 그리고 간단한 로그를 사용할 수 있는 Logger클래스가 있습니다.

## GlobalTimer
글로벌 타이머는 delta 타임을 계산할 수 있습니다. 이는 어플리케이션 단계에서 Update 루프가 필요할 때 사용할 수 있습니다.  
간단한 반복적인 작업을 사용하고 싶을 경우 `Repeat()`이 적합합니다. `Repeat()`은 사용자가 취소할 때 까지 반복적으로 작업을 수행합니다.
그 밖에도 RTT를 계산하는 전역 함수를 제공하는데, 현재 BaobabNetwork에서 사용하고 있습니다.

## Logger
콘솔 로그나 파일 로그를 제공합니다.

## IdGenerator
UInt64 값의 GUID나 int32의 EID를 만들어 줍니다.