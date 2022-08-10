using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    private NetWorkManager m_network = new NetWorkManager();


    private void Awake()
    {
        Screen.SetResolution(900, 600, false);
    }
    public FollowCam followCam;
    public GameObject playerUnit;
    public Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();

    #region �÷��̾� ��Ʈ�� ����
    public Player m_mainPlayer;
    float X;
    float Z;
    bool jumpDown;
    public Vector3 m_moveVector;

    public float m_moveSpeed;
    public float m_rotateSpeed;
    public float m_jumpPower;
    public PacketMoveData m_mainPlayerData;
    #endregion

    #region �÷��̾� ��Ʈ�� �Լ�
    void GetInput()
    {
        X = Input.GetAxis("Horizontal");
        Z = Input.GetAxis("Vertical");
        jumpDown = Input.GetKeyDown(KeyCode.Space);
    }
    void Move()
    {
        m_moveVector = new Vector3(X, 0, Z).normalized;
        m_mainPlayer.transform.position += m_moveVector * m_moveSpeed * Time.deltaTime;
    }
    void Turn()
    {
        m_mainPlayer.transform.LookAt(m_mainPlayer.transform.position + m_moveVector);
    }
    void Jump()
    {
        if (jumpDown && !m_mainPlayer.isJump)
        {
            m_mainPlayer.m_rigidbody.AddForce(Vector3.up * m_jumpPower, ForceMode.Impulse);
            m_mainPlayer.isJump = true;
        }
    }
    #endregion

    void Start()
    {
        m_moveSpeed = 10;
        m_rotateSpeed = 30;
        m_jumpPower = 10;

        m_network.Register(E_PROTOCOL.STC_SPAWN, SpawnProcess);
        m_network.Register(E_PROTOCOL.STC_MOVE, MoveProcess);
        m_network.Register(E_PROTOCOL.STC_OUT, OutProcess);
        m_network.Initialize();
    }
    void Update()
    {
        #region �÷��̾� ��Ʈ�ѷ�
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_network.End();
            //m_network.protocolThread.Join();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit(); // ���ø����̼� ����
#endif
        }
        if (m_mainPlayer != null)
        {
            GetInput();
            Move();
            Turn();
            Jump();
        }
        
        #endregion

       
    }


    const int m_SendTimeCounterF = 3;
    int m_sendTimeCounter = 0;
    private void FixedUpdate()
    {
        if (m_mainPlayer != null)
        {
            if (m_SendTimeCounterF <= m_sendTimeCounter)
            {
                m_mainPlayer.PositionAndRotationWrite(ref m_mainPlayerData);
                m_network.Session.Write((int)E_PROTOCOL.CTS_MOVE, m_mainPlayerData);
                m_sendTimeCounter = 0;
            }
            else
            {
                ++m_sendTimeCounter;
            }
        }
        #region �޼��� ó�� ����
        m_network.UpdateRecvProcess();
        #endregion
    }

    /*=================<��� �Լ�(TEST�� -> Ŭ�� ����ÿ� ���� �ش��ϴ� �޴������� ������ ����� �Լ��� �����ϴ� ���� ����)>=====================*/
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
            if (!players.ContainsKey(liddata.m_list[i]))
            {
                GameObject temp = GameObject.Instantiate(playerUnit);
                temp.GetComponent<Player>().moveData.m_id = liddata.m_list[i];
                players.Add(liddata.m_list[i], temp);
                temp.SetActive(true);
                if (liddata.m_list[i] != m_network.ClientId)
                {
                    temp.GetComponent<Rigidbody>().useGravity = true;
                }
                else
                {
                    temp.GetComponent<Rigidbody>().useGravity = true;
                }
                
            }
        }
        if(m_mainPlayer == null)
        {
            m_mainPlayerData.m_id = m_network.ClientId;

            m_mainPlayerData.m_state = 5;
            m_mainPlayerData.m_move.x = 15;
            m_mainPlayerData.m_move.y = 25;
            m_mainPlayerData.m_animing = 35;

            m_mainPlayer = players[m_network.ClientId].GetComponent<Player>();
            followCam.target = m_mainPlayer.transform;
        }
    }
    void MoveProcess()
    {
        PacketMoveData lData;
        m_network.Session.GetData<PacketMoveData>(out lData);
        if (players.ContainsKey(lData.m_id))
        {
            if (lData.m_id != m_network.ClientId)
            {
                players[lData.m_id].GetComponent<Player>().PositionAndRotationRead(lData);
            }
        }
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
