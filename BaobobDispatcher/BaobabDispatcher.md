# BaobabDispatcher
BaobabDispatcher는 패킷 프로토콜을 간편하게 만들 수 있도록 도와주는 패킷 자동 핸들러입니다.  
기본적인 사용법은 
  
# 동작 방식
BaobabDispatcher는 리플렉션에 기반해서 작동합니다. 리플렉션을 통해 `HandlerDispatcher`클래스에 [BaobabDispatcher] 어트리뷰트가 붙은 메서드들을 찾아 MessageHandler에 등록합니다.  
사용 가능한 타입은 두 가지인데, 첫 번째는 `IMessage`인터페이스를 상속받는 메시지 스키마입니다. [DispatcherHandlerIMessage]파일에서 그 예시를 찾을 수 있습니다.  
두 번째는 `Google.Flatbuffers`를 사용하는 예시입니다. 말 그대로 flatbuffer로 만들어진 통신 프로토콜을 사용할 수 있습니다.  

# 내부 구현
핸들러는 동기, 비동기를 구분해서 등록됩니다. 비동기일 경우 비동기호 호출하고 동기일 경우 동기로 호출합니다.  
메서드는 동적 바인딩이 아닌 리플렉션을 통해 메서드를 Action으로 만들어 등록하는 방식입니다.
