using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    private NetWorkManager m_network = new NetWorkManager();
    private void Awake()
    {
        Screen.SetResolution(600, 900, false);
    }
    public GameObject playerUnit;
    public Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
    public float m_moveSpeed;
    public float m_rotateSpeed;
    private Player m_mainPlayer;
    public PacketMoveData m_mainPlayerData;
    // Start is called before the first frame update
    void Start()
    {
        m_moveSpeed = 2;
        m_rotateSpeed = 30;

        m_network.Register(E_PROTOCOL.STC_SPAWN, SpawnProcess);
        m_network.Register(E_PROTOCOL.STC_MOVE, MoveProcess);
        m_network.Register(E_PROTOCOL.STC_OUT, OutProcess);
        m_network.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        bool l_isMove = false;
        
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            m_mainPlayerData.m_position.x -= Time.deltaTime * m_moveSpeed;
            l_isMove = true;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            m_mainPlayerData.m_position.x += Time.deltaTime * m_moveSpeed;
            l_isMove = true;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            m_mainPlayerData.m_position.y -= Time.deltaTime * m_moveSpeed;
            l_isMove = true;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            m_mainPlayerData.m_position.y += Time.deltaTime * m_moveSpeed;
            l_isMove = true;
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //m_mainPlayer.GetComponent<R>()
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(Time.deltaTime * new Vector3(0, -m_rotateSpeed, 0));

            m_mainPlayerData.m_rotation.x = transform.rotation.x;
            m_mainPlayerData.m_rotation.y = transform.rotation.y;
            m_mainPlayerData.m_rotation.z = transform.rotation.z;
            m_mainPlayerData.m_rotation.w = transform.rotation.w;
            l_isMove = true;
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(Time.deltaTime * new Vector3(0, m_rotateSpeed, 0));

            m_mainPlayerData.m_rotation.x = transform.rotation.x;
            m_mainPlayerData.m_rotation.y = transform.rotation.y;
            m_mainPlayerData.m_rotation.z = transform.rotation.z;
            m_mainPlayerData.m_rotation.w = transform.rotation.w;
            l_isMove = true;
        }

        if (l_isMove)
        {
            m_network.Session.Write((int)E_PROTOCOL.CTS_MOVE, m_mainPlayerData);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_network.End();
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit(); // 어플리케이션 종료
            #endif
        }

        m_network.UpdateRecvProcess();
    }



    /*=================<기능 함수(TEST용 -> 클라에 적용시에 각자 해당하는 메니저에서 유사한 기능의 함수를 제작하는 것이 좋음)>=====================*/
    void SpawnProcess()
    {
        ListData liddata;
        m_network.Session.GetData<ListData>(out liddata);

        for (int i = 0; i < liddata.m_size; i++)
        {
            if (liddata.m_list[i] == -1)
            {
                continue;
            }
            bool flag = true;
            foreach (int id in players.Keys)
            {
                if (id == liddata.m_list[i])
                {
                    flag = false;
                }
            }
            if (flag)
            {
                GameObject temp = GameObject.Instantiate(playerUnit);
                temp.GetComponent<Player>().moveData.m_id = liddata.m_list[i];
                players.Add(liddata.m_list[i], temp);
                temp.SetActive(true);
            }
        }

        m_mainPlayer = players[m_network.ClientId].GetComponent<Player>();
        m_mainPlayerData = players[m_network.ClientId].GetComponent<Player>().moveData;
    }
    void MoveProcess()
    {
        PacketMoveData lData;
        m_network.Session.GetData<PacketMoveData>(out lData);

        players[lData.m_id].GetComponent<Player>().moveData = lData;
    }
    void OutProcess()
    {
        IDData liddata;
        m_network.Session.GetData<IDData>(out liddata);
        Destroy(players[liddata.m_id]);
        players.Remove(liddata.m_id);
    }
    /*======================================*/
}
