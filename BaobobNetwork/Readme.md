# BaobabNetwork
Baobab라이브러리에서 네트워크 통신 기능을 제공합니다. 
BaobabNetwork에는 Tcp/Udp 연결, 패킷 페이로드 생성, 세션 Heartbeat등 다양한 기본 네트워크 기능들이 포함되어 있습니다.

## Tcp 연결
Tcp 세션은 기본적으로 TcpSession 클래스를 기반으로 합니다.  
주 기능은 생성자로 Tcp Socket을 받으면 내부에서 NetworkStream을 생성해 NetworkStream을 이용해 Send/Receive를 처리합니다.  
연결된 두 세션은 Heartbeat를 체크하며 RTT를 계산을 합니다.

## TcpPayload
Tcp 패킷은 ProtocolId, 길이, 암호화여부, 압축 여부, 데이타를 합쳐서 페이로드를 만들어서 전송합니다.  
Byte 직렬화 역직렬화도 제공하기 때문에 사용자는 만들어진 직렬화/역직렬화 함수를 그대로 사용할 수 있습니다.

## ServerBuilder
TcpServer구축에 필요한 ServerBuilder클래스를 제공합니다. 이름은 ServerBuilder이지만 아직 Builder 패턴을 사용하지는 않습니다.  
해당 클래스는 추후 Builder 패턴을 추가해 체이닝 방식으로 암호화나 압축같은 옵션처리도 추가할 예정입니다.