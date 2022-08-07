#include "TestManager.h"
#include "LogManager.h"
#include "SessionManager.h"

#pragma region Singleton
bool TestManager::CreateInstance()
{
	if (m_instance == nullptr)
	{
		m_instance = new TestManager();
		return true;
	}
	else
	{
		return false;
	}
}
void TestManager::DestroyInstance()
{
	if (m_instance != nullptr)
	{
		delete m_instance;
	}
}
bool TestManager::Initialize() // 초기화
{
	m_giveIdCounter = 0;
	return true;
}
void TestManager::Release() // 후처리
{
}
TestManager* TestManager::GetInstance()
{
	if (m_instance != nullptr)
	{
		return m_instance;
	}
	else
	{
		return nullptr;
	}
}
TestManager::TestManager()// 생성자
{
}
TestManager::~TestManager()// 소멸자
{
}
TestManager* TestManager::m_instance = nullptr;	// Singleton 객체
#pragma endregion


void TestManager::Function(Session* _session)
{
	E_PROTOCOL l_protocol = static_cast<E_PROTOCOL>(_session->GetProtocol());
	switch (l_protocol)
	{
	case TestManager::E_PROTOCOL::SPAWN:
		SpawnProcess(_session);
		break;
	case TestManager::E_PROTOCOL::MOVE:
		PlayProcess(_session);
		break;
	case TestManager::E_PROTOCOL::EXIT:
		ExitProcess(_session);
		break;
	default:
		LogManager::GetInstance()->LogWrite(7777);
		break;
	}
}

void TestManager::SpawnProcess(Session* _session)
{
	BYTE l_data[BUFSIZE];
	ZeroMemory(l_data, BUFSIZE);
	int l_dataSize = -1;

	BYTE l_data2[BUFSIZE];
	ZeroMemory(l_data2, BUFSIZE);
	int l_dataSize2 = -1;

	LockGuard l_lockGuard(&m_criticalKey); // 잠금
	


	for (list<Session*>::iterator iter = m_playerList.begin(); iter != m_playerList.end(); iter++)
	{
		ZeroMemory(l_data, BUFSIZE);
		l_dataSize = SpawnDataMake(l_data, (*iter)->m_id);
		if (!_session->SendPacket(static_cast<int>(E_PROTOCOL::INUSER), l_dataSize, l_data))
		{
			LogManager::GetInstance()->LogWrite(1005);
		}
	}

	m_playerList.push_back(_session);
	l_dataSize2 = SpawnDataMake(l_data2, m_giveIdCounter);
	_session->m_id = m_giveIdCounter;
	m_giveIdCounter++;

	for (list<Session*>::iterator iter = m_playerList.begin(); iter != m_playerList.end(); iter++)
	{
		if (!(*iter)->SendPacket(static_cast<int>(E_PROTOCOL::INUSER), l_dataSize2, l_data2))
		{
			LogManager::GetInstance()->LogWrite(1005);
		}
	}

	
	return;
}

void TestManager::PlayProcess(Session* _session)
{
	BYTE l_data[BUFSIZE];
	ZeroMemory(l_data, BUFSIZE);
	int l_dataSize = -1;
	float l_x;
	float l_y;
	MoveDataSplit(_session->GetDataField(), &l_x, &l_y);

	l_dataSize = MoveDataMake(l_data, _session->m_id, l_x, l_y);

	for (list<Session*>::iterator iter = m_playerList.begin(); iter != m_playerList.end(); iter++)
	{
		if (!(*iter)->SendPacket(static_cast<int>(E_PROTOCOL::MOVEUSER), l_dataSize, l_data))
		{
			LogManager::GetInstance()->LogWrite(1006);
		}
	}
	return;
}

void TestManager::ExitProcess(Session* _session)
{
	BYTE l_data[BUFSIZE];
	ZeroMemory(l_data, BUFSIZE);
	int l_dataSize = -1;

	LockGuard l_lockGuard(&m_criticalKey); // 잠금

	l_dataSize = ExitDataMake(l_data, _session->m_id);

	for (list<Session*>::iterator iter = m_playerList.begin(); iter != m_playerList.end(); iter++)
	{
		if ((*iter) == _session)
		{
			if (!(*iter)->SendPacket(static_cast<int>(E_PROTOCOL::EXIT), l_dataSize, l_data))
			{
				LogManager::GetInstance()->LogWrite(1006);
			}
		}
		if (!(*iter)->SendPacket(static_cast<int>(E_PROTOCOL::OUTUSER), l_dataSize, l_data))
		{
			LogManager::GetInstance()->LogWrite(1006);
		}
	}

	for (list<Session*>::iterator iter = m_playerList.begin(); iter != m_playerList.end(); )
	{
		if ((*iter) == _session)
		{
			m_playerList.erase(iter);
			break;
		}
		else
		{
			iter++;
		}
	}
	return;
}

void TestManager::ForceExitProcess(Session* _session)
{
	BYTE l_data[BUFSIZE];
	ZeroMemory(l_data, BUFSIZE);
	int l_dataSize = -1;

	LockGuard l_lockGuard(&m_criticalKey); // 잠금

	l_dataSize = ExitDataMake(l_data, _session->m_id);

	for (list<Session*>::iterator iter = m_playerList.begin(); iter != m_playerList.end(); iter++)
	{
		if ((*iter) == _session)
		{
			if (!(*iter)->SendPacket(static_cast<int>(E_PROTOCOL::EXIT), l_dataSize, l_data))
			{
				LogManager::GetInstance()->LogWrite(1006);
			}
		}
		if (!(*iter)->SendPacket(static_cast<int>(E_PROTOCOL::OUTUSER), l_dataSize, l_data))
		{
			LogManager::GetInstance()->LogWrite(1006);
		}
	}

	for (list<Session*>::iterator iter = m_playerList.begin(); iter != m_playerList.end(); )
	{
		if ((*iter) == _session)
		{
			m_playerList.erase(iter);
			break;
		}
		else
		{
			iter++;
		}
	}
	return;
}


int TestManager::SpawnDataMake(BYTE* _data, int _id)
{
	int l_packedSize = 0;
	BYTE* l_focusPointer = _data;

	l_focusPointer = MemoryCopy(l_focusPointer, l_packedSize, _id);

	return l_packedSize;
}

int TestManager::MoveDataMake(BYTE* _data, int _id, float _x, float _y)
{
	int l_packedSize = 0;
	BYTE* l_focusPointer = _data;

	l_focusPointer = MemoryCopy(l_focusPointer, l_packedSize, _id);
	l_focusPointer = MemoryCopy(l_focusPointer, l_packedSize, _x);
	l_focusPointer = MemoryCopy(l_focusPointer, l_packedSize, _y);

	return l_packedSize;
}

int TestManager::ExitDataMake(BYTE* _data, int _id)
{
	int l_packedSize = 0;
	BYTE* l_focusPointer = _data;

	l_focusPointer = MemoryCopy(l_focusPointer, l_packedSize, _id);

	return l_packedSize;
}


void TestManager::MoveDataSplit(BYTE* _data, float* _x, float* _y)
{
	BYTE* l_focusPointer = _data;

	memcpy(_x, l_focusPointer, sizeof(float));
	l_focusPointer = l_focusPointer + sizeof(float);

	memcpy(_y, l_focusPointer, sizeof(float));
	l_focusPointer = l_focusPointer + sizeof(float);
}
