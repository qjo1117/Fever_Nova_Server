#pragma once
#include "Global.h"
#include <mysql.h>
class DataBaseManager
{
#pragma region Singleton
public:
	static bool CreateInstance(); // 생성
	static void DestroyInstance(); // 삭제
	bool Initialize(); // 초기화
	void Release(); // 후처리
	static DataBaseManager* GetInstance();
private:
	static DataBaseManager* m_instance;		// 인스턴스 변수

	DataBaseManager();
	virtual ~DataBaseManager();
#pragma endregion
public:
	bool DataBaseConnect(const TCHAR* _id, const TCHAR* _password, const TCHAR* _dataBaseName, const unsigned int _port);

	void LoginInfoLoad();
	void LoginInfoInsert(const TCHAR* _id, const TCHAR* _password); // 가입    DB에 넣고
	void LoginInfoDelete(const TCHAR* _id); // 탈톼   DB에 삭제

private:
	const TCHAR* HOSTIP = L"127.0.0.1";	// 루프백 주소
	TCHAR* m_id;
	const TCHAR* m_password;
	TCHAR* m_databaseName;

	MYSQL m_mysql;			// 연결 섹션 객체
};


// while(mysql_next_result (&m_mysql))   -> 0 이면 결과가 더있음, -1 이면 없음 , 0보다 크면 오류
// 결과 셋을 다 가져오는것이 나중에 다시 호출을 하였을때 오류 안남
// 

// SetConsoleCtrlHandler

// 
// 
//if(nullptr 정보없다 )
	//{
	//	// 1. 로그인 -> 아이디없음
	//	// 2. 가입 -> 가입가능
	//}
	//if(!= nullptr 정보가 있다)
	//{
	//	// 1. 로그인 -> 로그인 되있냐? 안되있으면 로그인해라, 되있으면 안됨
	//	// 2. 가입 -> 가입안됨
	//}

	//추가	회원가입
	//검색	로그인 등의 가능여부 판단
	//변경	로그인, 로그아웃
	//삭제	회원탈퇴
