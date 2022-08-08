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
        m_moveData.m_positionX = 0;
        m_moveData.m_positionY = 0;
        m_moveData.m_positionZ = 0;

        m_moveData.m_rotationX = 0;
        m_moveData.m_rotationY = 0;
        m_moveData.m_rotationZ = 0;
        m_moveData.m_rotationW = 0;

        m_moveData.m_moveX = 10;
        m_moveData.m_moveZ = 10;

        m_moveData.m_animing = 5;
        
        session = new Session();
        session.Initialize();
    }
    public GameObject playerUnit;
    public List<GameObject> players = new List<GameObject>();

    public PacketMoveData m_moveData;

    private void Update()
    {
        bool l_isMove = false;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            m_moveData.m_positionX -= Time.deltaTime * 1;
            l_isMove = true;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            m_moveData.m_positionX += Time.deltaTime * 1;
            l_isMove = true;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            m_moveData.m_positionY -= Time.deltaTime * 1;
            l_isMove = true;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            m_moveData.m_positionY += Time.deltaTime * 1;
            l_isMove = true;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(Time.deltaTime *  new Vector3(0, -30, 0));
            m_moveData.m_rotationX = transform.rotation.x;
            m_moveData.m_rotationY = transform.rotation.y;
            m_moveData.m_rotationZ = transform.rotation.z;
            m_moveData.m_rotationW = transform.rotation.w;
            l_isMove = true;
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(Time.deltaTime * new Vector3(0, 30, 0));
            m_moveData.m_rotationX = transform.rotation.x;
            m_moveData.m_rotationY = transform.rotation.y;
            m_moveData.m_rotationZ = transform.rotation.z;
            m_moveData.m_rotationW = transform.rotation.w;
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
                        m_moveData.m_id = liddata.m_id;
                        session.Write((int)E_PROTOCOL.CTS_SPAWN);
                    }
                    break;
                case (int)E_PROTOCOL.STC_SPAWN:
                    {
                        int lid;
                        ListData liddata;
                        session.GetData<ListData>(out liddata);

                        for (int i = 0; i < liddata.m_size; i++)
                        {
                            if (liddata.m_list[i] == -1)
                            {
                                continue;
                            }
                            bool flag = true;
                            foreach (GameObject obj in players)
                            {
                                if (obj.GetComponent<Player>().moveData.m_id == liddata.m_list[i])
                                {
                                    flag = false;
                                }
                            }
                            if (flag)
                            {
                                GameObject temp = GameObject.Instantiate(playerUnit);
                                temp.GetComponent<Player>().moveData.m_id = liddata.m_list[i];
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
                        PacketMoveData lData;
                        session.GetData<PacketMoveData>(out lData);
                        foreach (GameObject obj in players)
                        {
                            if (obj.GetComponent<Player>().moveData.m_id == lData.m_id)
                            {
                                obj.GetComponent<Player>().moveData.m_positionX = lData.m_positionX;
                                obj.GetComponent<Player>().moveData.m_positionY = lData.m_positionY;
                                obj.GetComponent<Player>().moveData.m_positionZ = lData.m_positionZ;

                                obj.GetComponent<Player>().moveData.m_rotationX = lData.m_rotationX;
                                obj.GetComponent<Player>().moveData.m_rotationY = lData.m_rotationY;
                                obj.GetComponent<Player>().moveData.m_rotationZ = lData.m_rotationZ;
                                obj.GetComponent<Player>().moveData.m_rotationW = lData.m_rotationW;

                                obj.GetComponent<Player>().moveData.m_moveX = lData.m_moveX;
                                obj.GetComponent<Player>().moveData.m_moveZ = lData.m_moveZ;
                                obj.GetComponent<Player>().moveData.m_animing = lData.m_animing;
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
                            if (obj.GetComponent<Player>().moveData.m_id == liddata.m_id)
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
