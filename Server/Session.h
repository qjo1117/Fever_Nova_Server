#pragma once
#include "Packet.h"
#include "State.h"
#include "LoginState.h"
#include "TestState.h"
class Session : public Packet
{
public:
	Session();
	Session(const SOCKET& _sock);
	virtual ~Session();

	bool Initialize();
	void Release();
	//void* GetLoginInfo();

	State* GetState();

	//void SetLoginInfo(void* _loginInfo);

	bool SendPacket(unsigned int _protocol, unsigned int _datasize, BYTE* _data);
	bool RecvPacket();


	int m_id;
	float m_x;
	float m_y;
private:
	//void* m_loginInfo;

	


	State* m_state;

	/* Session이 가지는 State를 추가 */
	//LoginState* m_loginState;
	TestState* m_testState;
	/*============================= */
};

