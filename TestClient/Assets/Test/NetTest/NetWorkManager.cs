using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class NetWorkManager : MonoBehaviour
{
    Session session;

    private void Awake()
    {
        Screen.SetResolution(600, 900, false);
    }

    void Start()
    {
        m_moveData.m_x = 0;
        m_moveData.m_y = 0;
        session = new Session();
        session.Initialize();
    }
    public GameObject playerUnit;
    public List<GameObject> players = new List<GameObject>();

    public int m_id;
    public MoveData m_moveData;

    private void Update()
    {
        bool l_isMove = false;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            m_moveData.m_x -= Time.deltaTime * 1;
            l_isMove = true;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            m_moveData.m_x += Time.deltaTime * 1;
            l_isMove = true;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            m_moveData.m_y -= Time.deltaTime * 1;
            l_isMove = true;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            m_moveData.m_y += Time.deltaTime * 1;
            l_isMove = true;
        }

        if (l_isMove)
        {
            session.Write((int)E_PROTOCOL.CTS_MOVE, m_moveData);
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //MoveTest.GetInstance().CloseSocket();
            if (session.CheckConnecting())
            {
                session.Write((int)E_PROTOCOL.CTS_EXIT);// 종료
                session.TreadEnd();
                session.CloseSocket();
            }
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit(); // 어플리케이션 종료
#endif
        }


        if (session.CheckRead())
        {
            switch (session.GetProtocol())
            {
                case (int)E_PROTOCOL.CRYPTOKEY:
                    {
                        session.CryptoKeyDataSetting();
                        session.Write((int)E_PROTOCOL.CTS_IDCREATE); // 접속
                    }
                    break;
                case (int)E_PROTOCOL.STC_IDCREATE:
                    {
                        IDData liddata;
                        session.GetData<IDData>(out liddata);
                        m_id = liddata.m_id;
                        session.Write((int)E_PROTOCOL.CTS_SPAWN);
                    }
                    break;
                case (int)E_PROTOCOL.STC_SPAWN:
                    {
                        int lid;
                        ListData liddata;
                        session.GetData<ListData>(out liddata);

                        for(int i=0; i< liddata.m_size; i++)
                        {
                            if(liddata.m_list[i] == -1)
                            {
                                continue;
                            }
                            bool flag = true;
                            foreach (GameObject obj in players)
                            {
                                if (obj.GetComponent<Player>().id == liddata.m_list[i])
                                {
                                    flag = false;
                                }
                            }
                            if (flag)
                            {
                                GameObject temp = GameObject.Instantiate(playerUnit);
                                temp.GetComponent<Player>().id = liddata.m_list[i];
                                players.Add(temp);
                                temp.SetActive(true);
                            }
                        }
                        
                    }
                    break;
                case (int)E_PROTOCOL.STC_MOVE:
                    {
                        int lid;
                        float lx;
                        float ly;
                        RecvMoveData lData;
                        session.GetData<RecvMoveData>(out lData);
                        foreach (GameObject obj in players)
                        {
                            if (obj.GetComponent<Player>().id == lData.m_id)
                            {
                                obj.GetComponent<Player>().x = lData.m_x;
                                obj.GetComponent<Player>().y = lData.m_y;
                            }
                        }
                    }
                    break;
                case (int)E_PROTOCOL.STC_OUT:
                    {
                        int lid;
                        IDData liddata;
                        session.GetData<IDData>(out liddata);
                        foreach (GameObject obj in players)
                        {
                            if (obj.GetComponent<Player>().id == liddata.m_id)
                            {
                                Destroy(obj);
                                players.Remove(obj);
                            }
                        }
                    }
                    break;
            }
        }
    }


    void OnApplicationQuit()
    {

    }
}
