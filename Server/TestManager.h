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
		SPAWN,	// 서버 -> 클라, 클라 -> 서버 :  스폰 신호
		MOVE,	// 서버 -> 클라, 클라 -> 서버 :  이동 신호
		EXIT,	// 서버 -> 클라, 클라 -> 서버 :  종료 신호	

		INUSER = 10,
		MOVEUSER = 20,
		OUTUSER = 30,
	};

	void Function(Session* _session);

	void SpawnProcess(Session* _session);
	void PlayProcess(Session* _session);
	void ExitProcess(Session* _session);
	void ForceExitProcess(Session* _session);
#pragma region Packing&Unpacking
	// packing
	int SpawnDataMake(BYTE* _data, int _id);
	int MoveDataMake(BYTE* _data, int _id, float _x, float _y);
	int ExitDataMake(BYTE* _data, int _id);
	// unpacking
	void MoveDataSplit(BYTE* _data, float* _x, float* _y);
#pragma endregion
private:
	CriticalKey m_criticalKey;
	int m_giveIdCounter;
	list<Session*> m_playerList;
};

