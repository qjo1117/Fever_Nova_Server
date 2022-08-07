#pragma once

#pragma warning(disable:4996)
#pragma comment(lib, "libmysql.lib")
#pragma comment(lib, "ws2_32")
#include <winsock2.h>
#include <iostream>
#include <list>
#include <ctime>
#include <tchar.h>
using namespace std;

#define SERVERPORT	9000		
#define BUFSIZE		4096
#define INFOSIZE	128
#define MAXPACKETNUM 210000000

enum class E_OPERATION	// OverlappedEX
{
	ACCEPT,
	SEND,
	RECV,
};

struct OVERLAPPEDEX
{
	OVERLAPPED	overlapped;	// Overlap
	E_OPERATION operation;	// OverlapEx Type
	void* session;	// session pointer
};

//template <typename T>
//BYTE* MemoryCopy(BYTE* _buffer, int& _size, T _source);
template<typename T>
BYTE* MemoryCopy(BYTE* _buffer, int& _size, T _source)
{
	BYTE* l_focusPointer = _buffer;

	memcpy(l_focusPointer, &_source, sizeof(T));
	_size += sizeof(T);
	l_focusPointer += sizeof(T);

	return l_focusPointer;
}

BYTE* MemoryCopy(BYTE* _buffer, int& _size, const TCHAR* _source);


// 7월6일 이현, 종배 작업
/*
1. ConsoleControlHandler 완성
	-> 종료중에  l_overlappedEx 이 null이여서 오류가남, if문으로 예외 처리함
	-> main의 Release와 DestroyInstance는 그대로 두라고 하심(교수님), 이유 : 우리가 콘솔 끄는거 빼고 다른걸로 종료시 대비

2. 패킷 암복호화
	-> 실패! 빠바밤빠바밤

3. 교수님 수업
	-> 상태 패턴 추가 수업
	-> 상태 패턴 추가함
	-> LoginState의 send 부분 만들 예정 확인
4. 패킷 암호화가 진행된것인지 확인하기 위하여 패킷 로그를 찍는 함수 LogManager에 만들기

5. 프로토콜 설계 수업 7월 7일 예정
*/



// 7월7일 준영,찬혁 작업
/*
1. 서버 구조 변경
	-> 클라이언트 역할 축소 : 데이터 수신 후 필요한 작업만 하게 함

2. Packet 클래스에 암호화 추가
	-> PakcetDataAdd에서 암호화
	-> Recvcompcheck에서 복호화
	-> 실제 테스트는 안해봄. 클라이언트에 적용이 안되어있기 때문
	-> 다른 프로젝트로 임시 테스트해본 결과 잘 돌아감
	-> 암호화 로직
		-> 기본적인 Main Key를 이용해 XOR 연산
		-> 연산 후 Sub Key1과 Sub Key2를 이용해 시프트 연산으로 패턴화 제거
		-> 완전히 패턴이 사라진건 아니지만, 최소한의 안정성 확보

		-> 변수명
			-> 연결 시 모든 키는 rand()로 결정됨
			-> 그걸 위해 Main에서 srand(time(NULL)) 실행
			-> 메인 키 : m_cryptionMainKey
			-> 서브 키 : m_cryptionSubKey1, m_cryptionSubKey2

		-> 함수명
			-> 암호화 : Encryption(데이터 크기,데이터)
			-> 복호화 : Decryption(데이터 크기,데이터)
*/

// 7월11일 이현, 종배 작업
/*
1. (이현 작업) 비트연산을 위해 프로토콜과 사이즈 unsigned int로 변경
	- 비트연산 클래스만 만들어고 아직 적용 안함 (수요일에 할 예정)
	- ProtocolExtension 클래스
	- 사용법 ProtocolExtension::SynthesizeProtocol(보낼 프로토콜, 프로토콜 enum, 프로토콜 레벨)
	- 사용법 ProtocolExtension::ExtractionProtocol(받은 프로토콜, 추출한 프로토콜 enum, 프로토콜 레벨)

	- 프로토콜 레벨 같이 토의 필요함

2. (종배 작업) STL 변경 예정 -> 종배 작업중
	- 좋은 리스트 있으면 알리도
	- 큐는 이현이 만들어둔 licked queue 있음
	- https://github.com/RyuYiHyun/AlgorithmStudys/blob/master/LinkedQueue/main.cpp 링크

3. (이현작업) 임계영역 클래스화하기(수업) -> Lock 클래스 , CriticalKey 클래스
 - 보호 받아야하는 스레드에 CriticalKey 만들기
 - 보호 받을 영역에 지역변수로  Lock  = Lock(&Key); 형식으로 만들면 보호
*/

// 7월 12일 준영,찬혁 작업
/*
발견된 버그 목록
	1. 클라에서 빠른 시간동안 메시지를 발생시키면 에러가 발생
		-> 서버랑 클라 둘다 키고 클라에서 빠르게 2엔터 2엔터 2엔터 반복 등 송수신을 빠르게 발생시키면
		-> 서버에서 deque 문제가 터짐. 내용은 큐가 empty인데 front를 가져오려 했다 함
		-> 원인규명 실패
		-> lock제거, 원본으로 테스트(3.6, 3.5, 3.4) 등 이것저것 해봣는데 로직 자체의 문제같음
		-> (찬혁) : 버그 발생 상황 시 recv가 느리게 받아지는 현상 발견
		-> 문의사항 있을시 전화해

요청사항
	1. 버그 테스트 더 빡쎄게 해야될듯
	2. 기능이 미완성 혹은 미적용된 거면 맨위에 써주라

1. (준영 작업)
	Unity와의 연동 준비
	-> char형 자료형 변경
		-> 버퍼용 : BYTE
		-> 문자열 : TCHAR
			이에 따른 길이,비교,복사 작업 일부 변경

	치삼쿤이 알려주신 작업
		-> memcpy 후 사이즈와 포인터 수동 변경 -> 함수화
			-> Global : MemoryCopy
				-> 메모리를 복사해주고, 크기를 증가시키며 증가시킨 만큼 이동된 포인터를 반환
				-> 별거 아닌데 치삼쿤이 이런거 있으면 좋다길래 넣음

// 2. (찬혁 작업)
	Packet클래스의 버퍼 보호 추가
		-> 다른 매니저들간 버퍼를 중복참조하여 데이터가 오염되는것을 방지하기 위하여 추가
			-> Packet에 CriticalKey추가 후
			-> Packet의 Recv,Send 내부 함수에 Lock작업 수행
	
	클래스명 Lock -> LockGuard로 변경
		-> Lock이라는 이름은 함수의 느낌이 강하기떄문에 LockGuard로 변경

	Lock 지역변수 생성 코드 형식 변경 
		-> Lock  = Lock(&Key)형식에서 LockGuard l_lockGuard(&m_criticalKey) 로 변경
		-> Lock객체가 지역변수이므로 '='대입 연산자 안쓰고 생성자로 처리해도됨
*/