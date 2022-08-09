#pragma once
#include "Global.h"
#include "Socket.h"
#include "Session.h"
#include "SessionManager.h"
#include <map>
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
		CRYPTOKEY,		// ���� -> Ŭ��				:	�ʱ� �Ϻ�ȣȭŰ ���� ��ȣ

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
	struct Vector2
	{
		Vector2()
		{
			X = 0; Y = 0;
		}
		Vector2(float _X, float _Y)
		{
			X = _X; Y = _Y;
		}
		float X;
		float Y;
	};
	struct Vector3
	{
		Vector3()
		{
			X = 0; Y = 0; Z = 0;
		}
		Vector3(float _X, float _Y, float _Z)
		{
			X = _X; Y = _Y; Z = _Z;
		}
		float X;
		float Y;
		float Z;
	};
	struct Quaternion
	{
		Quaternion()
		{
			X = 0; Y = 0; Z = 0; W = 0;
		}
		Quaternion(float _X, float _Y, float _Z, float _W)
		{
			X = _X; Y = _Y; Z = _Z; W = _W;
		}
		float X;
		float Y;
		float Z;
		float W;
	};
	struct MoveData
	{
		MoveData()
		{
			m_id = -1;
			m_animing = 0;
			m_state = 0;
		}
		MoveData(int _id)
		{
			m_id = _id;
			m_animing = 0;
			m_state = 0;
		}
		void CopyData(MoveData _Src)
		{
			m_id = _Src.m_id;
			m_position = _Src.m_position;
			m_rotation = _Src.m_rotation;
			m_move = _Src.m_move;
			m_animing = _Src.m_animing;
			m_state = _Src.m_state;
		}
		
		int m_id;
		Vector3 m_position;
		Quaternion m_rotation;
		Vector2 m_move;
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
	int IdDataMake(BYTE* _data, int _id);
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
	map<Session*, MoveData> m_MoveDataList;
	// �÷��̾� ���� - ����
	// ���� ���� - ����
	// ���� ���� - ����
};

