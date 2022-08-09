using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class NetWorkManager
{
    public Session session;
    public Session Session { get => session; }

    private Dictionary<int, Action> m_NetWorkProcess = new Dictionary<int, Action>();
    public Dictionary<int, Action> NetWorkProcess { get => m_NetWorkProcess; }

    private int m_clientId = -1;
    public int ClientId { get => m_clientId; }
    public void Initialize()
    {
        Register();
        session = new Session();
        session.Initialize();
    }
    public void Register()
    {
        m_NetWorkProcess.Add((int)E_PROTOCOL.CRYPTOKEY, KeyProcess);
        m_NetWorkProcess.Add((int)E_PROTOCOL.STC_IDCREATE, IdProcess);
    }
    public void Register(E_PROTOCOL _protocol, Action _action)
    {
        if (m_NetWorkProcess.ContainsKey((int)_protocol) == false)
        {
            m_NetWorkProcess.Add((int)_protocol, _action);
        }
    }
    private void KeyProcess()
    {
        session.CryptoKeyDataSetting();
        session.Write((int)E_PROTOCOL.CTS_IDCREATE); // 立加
    }
    private void IdProcess()
    {
        int l_id = -1;
        session.GetData(out l_id);
        m_clientId = l_id;
        session.Write((int)E_PROTOCOL.CTS_SPAWN); // 胶迄夸没
    }
    public void End()
    {
        //MoveTest.GetInstance().CloseSocket();
        if (session.CheckConnecting())
        {
            session.Write((int)E_PROTOCOL.CTS_EXIT);// 辆丰
            session.TreadEnd();
            session.CloseSocket();
        }
    }
    public void UpdateRecvProcess()
    {
        if (session.CheckRead())
        {
            if (m_NetWorkProcess.ContainsKey(session.GetProtocol()) == true)
            {
                m_NetWorkProcess[session.GetProtocol()].Invoke();
            }

            //switch (session.GetProtocol())
            //{
            //    case (int)E_PROTOCOL.CRYPTOKEY:
            //        {
            //            session.CryptoKeyDataSetting();
            //            session.Write((int)E_PROTOCOL.CTS_IDCREATE); // 立加
            //        }
            //        break;
            //    case (int)E_PROTOCOL.STC_IDCREATE:
            //        {
            //            IDData liddata;
            //            session.GetData<IDData>(out liddata);
            //            m_moveData.m_id = liddata.m_id;
            //            session.Write((int)E_PROTOCOL.CTS_SPAWN);
            //        }
            //        break;
            //    case (int)E_PROTOCOL.STC_SPAWN:
            //        {
            //            int lid;
            //            ListData liddata;
            //            session.GetData<ListData>(out liddata);

            //            for (int i = 0; i < liddata.m_size; i++)
            //            {
            //                if (liddata.m_list[i] == -1)
            //                {
            //                    continue;
            //                }
            //                bool flag = true;
            //                foreach (GameObject obj in players)
            //                {
            //                    if (obj.GetComponent<Player>().moveData.m_id == liddata.m_list[i])
            //                    {
            //                        flag = false;
            //                    }
            //                }
            //                if (flag)
            //                {
            //                    GameObject temp = GameObject.Instantiate(playerUnit);
            //                    temp.GetComponent<Player>().moveData.m_id = liddata.m_list[i];
            //                    players.Add(temp);
            //                    temp.SetActive(true);
            //                }
            //            }

            //        }
            //        break;
            //    case (int)E_PROTOCOL.STC_MOVE:
            //        {
            //            int lid;
            //            float lx;
            //            float ly;
            //            PacketMoveData lData;
            //            session.GetData<PacketMoveData>(out lData);
            //            foreach (GameObject obj in players)
            //            {
            //                if (obj.GetComponent<Player>().moveData.m_id == lData.m_id)
            //                {
            //                    obj.GetComponent<Player>().moveData = lData;
            //                }
            //            }
            //        }
            //        break;
            //    case (int)E_PROTOCOL.STC_OUT:
            //        {
            //            int lid;
            //            IDData liddata;
            //            session.GetData<IDData>(out liddata);
            //            foreach (GameObject obj in players)
            //            {
            //                if (obj.GetComponent<Player>().moveData.m_id == liddata.m_id)
            //                {
            //                    Destroy(obj);
            //                    players.Remove(obj);
            //                }
            //            }
            //        }
            //        break;
            //}
        }
    }


    void OnApplicationQuit()
    {

    }
}
