#pragma once
#include "Global.h"
#include "Socket.h"
#include "Session.h"
#include "SessionManager.h"

class TestManager
{
#pragma region Singleton
public:
	static bool CreateInstance();
	static void DestroyInstance();
	bool Initialize();
	void Release();
	static TestManager* GetInstance();
private:
	static TestManager* m_instance;

	TestManager();
	virtual ~TestManager();
#pragma endregion

public:
	enum class E_PROTOCOL
	{
		CRYPTOKEY,		// 서버 -> 클라				:	초기 암복호화키 전송 신호

		STC_IDCREATE,
		CTS_IDCREATE,

		STC_SPAWN,
		CTS_SPAWN,

		STC_MOVE,
		CTS_MOVE,

		STC_OUT,
		CTS_OUT,

		STC_EXIT,
		CTS_EXIT,
	};

	struct MoveData
	{
		int m_id;
		float m_positionX;
		float m_positionY;
		float m_positionZ;
		float m_rotationX;
		float m_rotationY;
		float m_rotationZ;
		float m_rotationW;
		float m_moveX;
		float m_moveZ;
		float m_animing;
		int m_state;
	};

	void Function(Session* _session);

	void IdCreateProcess(Session* _session);
	void SpawnProcess(Session* _session);
	void PlayProcess(Session* _session);
	void ExitProcess(Session* _session);
	void ForceExitProcess(Session* _session);
#pragma region Packing&Unpacking
	// packing
	//int SpawnDataMake(BYTE* _data, int _id);
	int IdDataMake(BYTE* _data,int _id);
	int SpawnDataMake(BYTE* _data);
	int MoveDataMake(BYTE* _data, MoveData _moveData);
	int ExitDataMake(BYTE* _data, int _id);
	// unpacking
	void MoveDataSplit(BYTE* _data, MoveData& _moveData);
#pragma endregion
private:
	CriticalKey m_criticalKey;
	int m_giveIdCounter;
	list<Session*> m_playerList;

	
};

