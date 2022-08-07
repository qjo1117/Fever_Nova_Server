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


// 7��6�� ����, ���� �۾�
/*
1. ConsoleControlHandler �ϼ�
	-> �����߿�  l_overlappedEx �� null�̿��� ��������, if������ ���� ó����
	-> main�� Release�� DestroyInstance�� �״�� �ζ�� �Ͻ�(������), ���� : �츮�� �ܼ� ���°� ���� �ٸ��ɷ� ����� ���

2. ��Ŷ �Ϻ�ȣȭ
	-> ����! ���ٹ���ٹ�

3. ������ ����
	-> ���� ���� �߰� ����
	-> ���� ���� �߰���
	-> LoginState�� send �κ� ���� ���� Ȯ��
4. ��Ŷ ��ȣȭ�� ����Ȱ����� Ȯ���ϱ� ���Ͽ� ��Ŷ �α׸� ��� �Լ� LogManager�� �����

5. �������� ���� ���� 7�� 7�� ����
*/



// 7��7�� �ؿ�,���� �۾�
/*
1. ���� ���� ����
	-> Ŭ���̾�Ʈ ���� ��� : ������ ���� �� �ʿ��� �۾��� �ϰ� ��

2. Packet Ŭ������ ��ȣȭ �߰�
	-> PakcetDataAdd���� ��ȣȭ
	-> Recvcompcheck���� ��ȣȭ
	-> ���� �׽�Ʈ�� ���غ�. Ŭ���̾�Ʈ�� ������ �ȵǾ��ֱ� ����
	-> �ٸ� ������Ʈ�� �ӽ� �׽�Ʈ�غ� ��� �� ���ư�
	-> ��ȣȭ ����
		-> �⺻���� Main Key�� �̿��� XOR ����
		-> ���� �� Sub Key1�� Sub Key2�� �̿��� ����Ʈ �������� ����ȭ ����
		-> ������ ������ ������� �ƴ�����, �ּ����� ������ Ȯ��

		-> ������
			-> ���� �� ��� Ű�� rand()�� ������
			-> �װ� ���� Main���� srand(time(NULL)) ����
			-> ���� Ű : m_cryptionMainKey
			-> ���� Ű : m_cryptionSubKey1, m_cryptionSubKey2

		-> �Լ���
			-> ��ȣȭ : Encryption(������ ũ��,������)
			-> ��ȣȭ : Decryption(������ ũ��,������)
*/

// 7��11�� ����, ���� �۾�
/*
1. (���� �۾�) ��Ʈ������ ���� �������ݰ� ������ unsigned int�� ����
	- ��Ʈ���� Ŭ������ ������ ���� ���� ���� (�����Ͽ� �� ����)
	- ProtocolExtension Ŭ����
	- ���� ProtocolExtension::SynthesizeProtocol(���� ��������, �������� enum, �������� ����)
	- ���� ProtocolExtension::ExtractionProtocol(���� ��������, ������ �������� enum, �������� ����)

	- �������� ���� ���� ���� �ʿ���

2. (���� �۾�) STL ���� ���� -> ���� �۾���
	- ���� ����Ʈ ������ �˸���
	- ť�� ������ ������ licked queue ����
	- https://github.com/RyuYiHyun/AlgorithmStudys/blob/master/LinkedQueue/main.cpp ��ũ

3. (�����۾�) �Ӱ迵�� Ŭ����ȭ�ϱ�(����) -> Lock Ŭ���� , CriticalKey Ŭ����
 - ��ȣ �޾ƾ��ϴ� �����忡 CriticalKey �����
 - ��ȣ ���� ������ ����������  Lock  = Lock(&Key); �������� ����� ��ȣ
*/

// 7�� 12�� �ؿ�,���� �۾�
/*
�߰ߵ� ���� ���
	1. Ŭ�󿡼� ���� �ð����� �޽����� �߻���Ű�� ������ �߻�
		-> ������ Ŭ�� �Ѵ� Ű�� Ŭ�󿡼� ������ 2���� 2���� 2���� �ݺ� �� �ۼ����� ������ �߻���Ű��
		-> �������� deque ������ ����. ������ ť�� empty�ε� front�� �������� �ߴ� ��
		-> ���αԸ� ����
		-> lock����, �������� �׽�Ʈ(3.6, 3.5, 3.4) �� �̰����� �ؔf�µ� ���� ��ü�� ��������
		-> (����) : ���� �߻� ��Ȳ �� recv�� ������ �޾����� ���� �߰�
		-> ���ǻ��� ������ ��ȭ��

��û����
	1. ���� �׽�Ʈ �� ����� �ؾߵɵ�
	2. ����� �̿ϼ� Ȥ�� ������� �Ÿ� ������ ���ֶ�

1. (�ؿ� �۾�)
	Unity���� ���� �غ�
	-> char�� �ڷ��� ����
		-> ���ۿ� : BYTE
		-> ���ڿ� : TCHAR
			�̿� ���� ����,��,���� �۾� �Ϻ� ����

	ġ������ �˷��ֽ� �۾�
		-> memcpy �� ������� ������ ���� ���� -> �Լ�ȭ
			-> Global : MemoryCopy
				-> �޸𸮸� �������ְ�, ũ�⸦ ������Ű�� ������Ų ��ŭ �̵��� �����͸� ��ȯ
				-> ���� �ƴѵ� ġ������ �̷��� ������ ���ٱ淡 ����

// 2. (���� �۾�)
	PacketŬ������ ���� ��ȣ �߰�
		-> �ٸ� �Ŵ����鰣 ���۸� �ߺ������Ͽ� �����Ͱ� �����Ǵ°��� �����ϱ� ���Ͽ� �߰�
			-> Packet�� CriticalKey�߰� ��
			-> Packet�� Recv,Send ���� �Լ��� Lock�۾� ����
	
	Ŭ������ Lock -> LockGuard�� ����
		-> Lock�̶�� �̸��� �Լ��� ������ ���ϱ⋚���� LockGuard�� ����

	Lock �������� ���� �ڵ� ���� ���� 
		-> Lock  = Lock(&Key)���Ŀ��� LockGuard l_lockGuard(&m_criticalKey) �� ����
		-> Lock��ü�� ���������̹Ƿ� '='���� ������ �Ⱦ��� �����ڷ� ó���ص���
*/