using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using dataread;
using JBClasses;
public class JBKernel : MonoBehaviour {
	public JBUIsystem UISystem;
	int m_backbuffer_1=0;//0,-1
	int m_backbuffer_2=0;
	int m_backbuffer_3=0;
	float backSpeed=0;
	public GameObject m_Background;
	public GameObject m_BackMossaic1;
	public GameObject m_BackMossaic2;
	public GameObject m_BulletParent;//poison needle
	public GameObject m_BulletPrefab;
	public GameObject m_JB_MainCharacter;
    public static GameObject m_JBBoss;

	public GameObject m_HoneyMessage;
	public GameObject m_Score;
	public GameObject m_Level;
	public GameObject m_Becky;
	public GameObject m_Moth;
    public GameObject m_BG;
	public static int score=0;
	public static JBKernel _instance;
	public static int _killedEnemyCount;

	public enum JB_MainCharacterState
	{
		Flying,
		Shooting,
		Attached,
		Killed,
	}
	public enum GameState
	{
		playing,
		stop,
		end_complete,
        end_complete_kidnap_blossom,
		end_failed,
		end_total
	}
	public enum Enemy_making
	{
		start,
		end,
	}
	public static JB_MainCharacterState MainCharacter_State;
	public static GameState GM_state;
	public Enemy_making Making_State;
	int _enemyKind;
	int[] enemy_count=new int[10];
	public GameObject EnemyHouse;
	//public GameObject m_BossPrefab;
	public GameObject m_GameComplete;
	public GameObject m_BonusPrefab;

	private GameObject[] enemyObj=new GameObject[10];
	private GameObject bossObj;
	private GameObject obsObj;
	private string BonusName;
	public GameObject BonusRoot;
	private int BonusNumber = 5;
	[HideInInspector]
	public float mTotalTick;
	[HideInInspector]
	public float m_SuperEnemyTick;
	[HideInInspector]
	public float m_EnemyTick;
	[HideInInspector]
	public float m_ObsTick;
	float _ObsTime,EnemyTime;
    float EpsTimeEnemy, EpsTimeObs;
    const float _BossPresentTime = 100;
	//const float _BossPresentTime = 1;
	const float _LevelTime = 180;
	private bool BossAlreadyPresent;
	//by ryu for bullet
	static bool shootEnable=true;
	static int shootCount=0;
    int shootFrame = 0;
	int pat_kind,col_cnt;

    int m_levelBombCount = 1;
    bool m_CreateBomb = false;
    float m_CreateBombTick = 0.0f;
    float m_CreateBombTime = 100.0f;

    public static bool m_Bomb = false;
    float m_BombTime = 0.2f;
    float m_BombTick = 0.0f;
    public static Vector3 m_BombPos = new Vector3(0,0,0);
    public static bool m_bAttackDirInvert = false;

    static int m_LastBGIndex = 0;

	// Use this for initialization
	void Start () 
	{
		score = 0;
		GM_state = GameState.playing;
		MainCharacter_State = JB_MainCharacterState.Flying;
		mTotalTick = 0;
		m_SuperEnemyTick = 0;
		m_EnemyTick = 10;
		m_ObsTick = 0;

		_killedEnemyCount = 0;

        m_bAttackDirInvert = false;
		BossAlreadyPresent = false;
		 _enemyKind=JBDB.levelEnemy.enemy_kind;
		enemyObj=new GameObject[_enemyKind];
		for(int i=0;i<10;i++){
			enemy_count[i] = 0;
		}
		backSpeed=3;
		SetBackGround ();

		for(int i=0;i<_enemyKind;i++)
		{
			enemy_count[i]=JBDB.levelEnemy.enemy_count[i];
			string enemyName=JBDB.levelEnemy.enemy_name[i];
			enemyObj[i]=Resources.Load<Object>("Prefabs/"+enemyName) as GameObject;
		}

		string bossName = JBDB.levelBoss.boss_name [0];
		bossObj = Resources.Load<Object> ("Prefabs/" + bossName) as GameObject;
		StartCoroutine ("BonusPresenting");

		if(JBDB.levelint<=5)
		{
			pat_kind=1;
			col_cnt=1;
            m_levelBombCount = 10;
			_ObsTime=9;
			EnemyTime=5;

		}
		else if(JBDB.levelint>5 && JBDB.levelint<=10)
		{
			pat_kind=3;
			col_cnt=3;
            m_levelBombCount = 15;
			_ObsTime=8;
			EnemyTime=5;

		}
		else if(JBDB.levelint>10 && JBDB.levelint<=20)
		{
			pat_kind=4;
			col_cnt=4;
            m_levelBombCount = 30;
			_ObsTime=6;
			EnemyTime=5;

		}
		else if(JBDB.levelint>20 && JBDB.levelint<=30)
		{
			pat_kind=5;
			col_cnt=5;
            m_levelBombCount = 40;
			_ObsTime=8;
			EnemyTime=4;

		}
		else if(JBDB.levelint>30 && JBDB.levelint<=40)
		{
			pat_kind=6;
			col_cnt=4;
            m_levelBombCount = 50;
			_ObsTime=6;
			EnemyTime=4;

		}
		else
		{
			pat_kind=6;
			col_cnt=4;
            m_levelBombCount = 60;
			_ObsTime=5;
			EnemyTime=4;

		}
		//_ObsTime = 8 - JBDB.levelint * 0.05f;
		//EnemyTime = 10 - JBDB.levelint * 0.1f;

        m_CreateBomb = false;
        m_CreateBombTick = 0.0f;
        m_CreateBombTime = _BossPresentTime / (m_levelBombCount + 1);

        if (JBDB.levelint < 4)
        {
            UISystem.HideBeckyDarkIcon();
        }
        else if (JBDB.levelint >=4 && JBDB.levelint <= 40)
        {
            UISystem.ShowBeckyIcon();
        }
        else if (JBDB.levelint > 40)
        {
            UISystem.ShowDarkIcon();
        }

        if (JBDB.characterName.Equals("JungleBee"))
        {
            shootFrame = 20;
        }
        else if (JBDB.characterName.Equals("CarpenterBee"))
        {
            shootFrame = 15;
        }
        else if (JBDB.characterName.Equals("BumbleBee"))
        {
            shootFrame = 10;
        }
        else if (JBDB.characterName.Equals("KillerBee"))
        {
            shootFrame = 5;
        }

	}

	void OnEnable()
	{
		JBDarkSpirit.BeckyPresent+=BeckyAct;
	}
	void OnDisable()
	{
		JBDarkSpirit.BeckyPresent-=BeckyAct;
	}
	// Update is called once per frame
	void Update () 
	{
		if (GM_state == GameState.playing) 
		{
			if(shootEnable==false)
				shootCount++;
			if (shootCount > shootFrame)
				shootEnable = true;

            if (m_Bomb)
            {
                m_BombTick += Time.deltaTime;
                if (m_BombTick > m_BombTime)
                {
                    m_Bomb = false;
                    m_BombTick = 0;
                }
            }

		mTotalTick += Time.deltaTime;
		m_SuperEnemyTick += Time.deltaTime;
		m_EnemyTick += Time.deltaTime;
		m_ObsTick+=Time.deltaTime;
		m_CreateBombTick += Time.deltaTime;

		if(m_SuperEnemyTick>40)
		{
			HiveAttackMessage();
			m_SuperEnemyTick=0;
		}
		if(m_ObsTick>_ObsTime+EpsTimeObs)
		{
			m_ObsTick=0;
			EpsTimeObs=Random.Range(-1,0);

			if(!BossAlreadyPresent)
			{
				ObstaclePresenting();
			}
		}

        if (m_CreateBombTick > m_CreateBombTime)
        {
            if (m_levelBombCount > 0)
            {
                m_CreateBombTick = 0.0f;
                m_levelBombCount--;
                m_CreateBomb = true;
            }
        }

		if(m_EnemyTick>EnemyTime+EpsTimeEnemy)
		{
			m_EnemyTick=0;
			EpsTimeEnemy=Random.Range(-1,0);

			if(!BossAlreadyPresent)
			{
				int n=Random.Range(1,_enemyKind+1);
				for(int i=0;i<n;i++)
				{
					EnemyPresenting(i);
				}
			}
		}
		if (mTotalTick >= _BossPresentTime) 
		{
			if(!BossAlreadyPresent)
			{
				BossPresenting();
			}
		}
		string StringScore = score.ToString ();
		m_Score.GetComponent<Text> ().text = StringScore;
		m_Level.GetComponent<Text> ().text = JBDB.levelint.ToString ();


			///////////
			////////////  -----------------background flow part--------------------------
			if(mTotalTick<_BossPresentTime)
			{
			if (m_Background.transform.GetChild (m_backbuffer_1).localPosition.x >= -2272.0f) 
			{
				background_flow ();
				if (m_Background.transform.GetChild (m_backbuffer_1).localPosition.x <= -2272.0f / 2) 
				{
					m_Background.transform.GetChild (1 - m_backbuffer_1).localPosition = new Vector3 (m_Background.transform.GetChild (m_backbuffer_1).localPosition.x + 4544.0f, 0, 0);
				}
			} 
			else 
			{
				m_backbuffer_1 = 1 - m_backbuffer_1;
			}
			//////////////////Chips flow Part;
			if (m_BackMossaic1.transform.GetChild (m_backbuffer_2).localPosition.x >= -2272.0f) 
			{
				background_mossaic1 ();
				if (m_BackMossaic1.transform.GetChild (m_backbuffer_2).localPosition.x <= -2272.0f / 2) {
					m_BackMossaic1.transform.GetChild (1 - m_backbuffer_2).localPosition = new Vector3 (m_BackMossaic1.transform.GetChild (m_backbuffer_2).localPosition.x + 4544.0f, 0, 0);
				}
			} 
			else 
			{
				m_backbuffer_2 = 1 - m_backbuffer_2;
			}
			if (m_BackMossaic2.transform.GetChild (m_backbuffer_3).localPosition.x >= -2272.0f) 
			{
				background_mossaic2 ();
				if (m_BackMossaic2.transform.GetChild (m_backbuffer_3).localPosition.x <= -2272.0f / 2) {
					m_BackMossaic2.transform.GetChild (1 - m_backbuffer_3).localPosition = new Vector3 (m_BackMossaic2.transform.GetChild (m_backbuffer_3).localPosition.x + 4544.0f, 0, 0);
				}
			} 
			else 
			{
				m_backbuffer_3 = 1 - m_backbuffer_3;
			}
			//-----------------------------
		}
		
		}
		if (GM_state == GameState.end_complete) 
		{
			UISystem.OpenGameCompleteDlg();
		}
        if (GM_state == GameState.end_complete_kidnap_blossom)
        {
            m_BG.SetActive(true);
            StartCoroutine("NextLevel");
        }
		if(GM_state==GameState.end_total)
		{
			UISystem.OpenTotalCompleteDlg();
		}
	}
	void BeckyAct()
	{
		//m_Becky.SetActive (true);
		//m_Becky.GetComponent<TweenPosition> ().enabled = true;
		//m_Becky.GetComponent<TweenPosition> ().PlayForward ();
		UISystem.m_BeckyMessage.SetActive (true);
        StartCoroutine("RescueParticle");
	}
    IEnumerator RescueParticle()
    {
        yield return new WaitForSeconds(0.5f);
        UISystem.m_RescueParticle.SetActive(true);
        //StartCoroutine ("SetState");
    }

    IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(5.0f);
        Application.LoadLevel("Loading_Scene2");
    }

	void RescueTextPresent()
	{
		UISystem.m_DisguiseMessage.SetActive (true);
	}
	public void DisguiseAnimalAct()
	{
		m_Moth.SetActive (true);
	}
	void background_flow()
	{
		m_Background.transform.GetChild(m_backbuffer_1).localPosition = new Vector3 (m_Background.transform.GetChild(m_backbuffer_1).localPosition.x - backSpeed, 0, 0);
		m_Background.transform.GetChild(1-m_backbuffer_1).localPosition = new Vector3 (m_Background.transform.GetChild(1-m_backbuffer_1).localPosition.x - backSpeed, 0, 0); 
	}
	void background_mossaic1()
	{
		m_BackMossaic1.transform.GetChild(m_backbuffer_2).localPosition = new Vector3 (m_BackMossaic1.transform.GetChild(m_backbuffer_2).localPosition.x - 4.0f, 0, 0);
		m_BackMossaic1.transform.GetChild(1-m_backbuffer_2).localPosition = new Vector3 (m_BackMossaic1.transform.GetChild(1-m_backbuffer_2).localPosition.x - 4.0f, 0, 0);
	}
	void background_mossaic2()
	{
		m_BackMossaic2.transform.GetChild(m_backbuffer_3).localPosition = new Vector3 (m_BackMossaic2.transform.GetChild(m_backbuffer_3).localPosition.x - 6.0f, 0, 0);
		m_BackMossaic2.transform.GetChild(1-m_backbuffer_3).localPosition = new Vector3 (m_BackMossaic2.transform.GetChild(1-m_backbuffer_3).localPosition.x - 6.0f, 0, 0);
	}
	public void EnemyPresenting(int number)
	{
		//int number=Random.Range(0,_enemyKind);
		if(enemy_count[number]>0)
		{
			int pat=Random.Range(0,pat_kind);
			int col=Random.Range(1,col_cnt);
			
			if(number==0)
			{
				EnemyTroop(0,pat,col);
			}
			else
			{
				GameObject temp=Instantiate(enemyObj[number]) as GameObject;
				temp.GetComponent<JBEnemy>().m_name=JBDB.levelEnemy.enemy_name[number];
				temp.transform.SetParent(EnemyHouse.transform);
				temp.transform.localPosition=new Vector3(0,0,0);
				temp.transform.localScale=Vector3.one;
			}
			//enemy_count[number]--;
		}
	}


	public void PresentWaspTroop()
	{
		int pat=Random.Range(0,pat_kind);
		int col=Random.Range(1,col_cnt);
		EnemyTroop(0,pat,col);
	}
	void CreateEnemy(int enyKind, float x, float y)
	{
		GameObject temp = null;
        if (enyKind == 0)
        {
            temp = Instantiate(enemyObj[0]) as GameObject;
            temp.GetComponent<JBEnemy>().m_name=JBDB.levelEnemy.enemy_name[enyKind];
        }
        else if (enyKind == 1)
        {
            GameObject temp1 = Resources.Load<Object>("Prefabs/wasp_new") as GameObject;
            temp = Instantiate(temp1) as GameObject;

            temp.GetComponent<JBEnemy>().m_name = "wasp_new";
            m_CreateBomb = false;
        }

		temp.GetComponent<JBEnemy>().setPos(x+600,y);
	}
	void EnemyTroop(int enyKind,int inPat,int length){
		//enyKind = 0;
		int pattern;
		pattern = inPat;
		//pattern = 6;
		float dis_h = 640;
		float eny_h = 80;
		float eny_w = 50;
		//float bwt_h = 20;
		//float bwt_w = 20;
        float bwt_h = 20;
        float bwt_w = 20;

		int maxRN=(int)((dis_h-bwt_h)/(eny_h+bwt_h));
		float x, y;
		switch (pattern) {
		case 0:		//retangular
                {
                    int n = Random.Range(0, length);
                    int z = Random.Range(1, maxRN);
                    for (int i = 0; i < length; i++)
                    {
                        x = (eny_w + bwt_w) * i;
                        for (int j = 1; j <= maxRN; j++)
                        {
                            y = (dis_h - maxRN * eny_h) / (maxRN + 1) + (eny_h - dis_h) / 2 + (j - 1) * (dis_h + eny_h) / (maxRN + 1);
                            if (m_CreateBomb && i == n && j == z)
                                CreateEnemy(1, x + 600, y);
                            else
                                CreateEnemy(0, x + 600, y);
                        }
                    }
                }

			break;
        case 1: //triangle
            {
                int n = Random.Range(1, length);
                int z = Random.Range(1, maxRN);

                for (int i = 0; i < maxRN; i++)
                {
                    x = (eny_w + bwt_w) * i;
                    for (int j = 1; j <= i + 1; j++)
                    {
                        int a = 0;
                        if (m_CreateBomb && j == z) a = 1;
                            
                        y = (dis_h - (i + 1) * eny_h) / (i + 1 + 1) + (eny_h - dis_h) / 2 + (j - 1) * (dis_h + eny_h) / (i + 1 + 1);
                        CreateEnemy(a, x + 600, y);
                    }
                }
            }
			break;
		case 2: 	//arrow-left
            {
                maxRN = (int)(dis_h / 2 / (eny_h / 2 + bwt_h)) - 1;
                int n = Random.Range(0, length);
                int z = Random.Range(1, maxRN);

                for (int j = 0; j < length; j++)
                {
                    CreateEnemy(0, 600 + j * (eny_w + bwt_w * 3), 0);
                    for (int i = 1; i < maxRN + 1; i++)
                    {
                        int a = 0;
                        if (m_CreateBomb && i == z && j == n) a = 1;
                        
                        x = (eny_w + bwt_w) * i;
                        y = (eny_h / 2 + bwt_h) * i;

                        CreateEnemy(a, x + j * (eny_w + bwt_w * 3) + 600, y);
                        CreateEnemy(0, x + j * (eny_w + bwt_w * 3) + 600, -y);

                    }
                }
            }

			break;
        case 3://arrow-right
            {
                maxRN = (int)(dis_h / 2 / (eny_h / 2 + bwt_h)) - 1;
                int n = Random.Range(0, length);
                int z = Random.Range(1, maxRN);
                for (int j = 0; j < length; j++)
                {
                    CreateEnemy(0, 600 - j * (eny_w + bwt_w * 3), 0);
                    for (int i = 1; i < maxRN + 1; i++)
                    {
                        int a = 0;
                        if (m_CreateBomb && i == z && j == n) a = 1;

                        x = (eny_w + bwt_w) * i;
                        y = (eny_h / 2 + bwt_h) * i;
                        CreateEnemy(a, -(x + j * (eny_w + bwt_w * 3)) + 600, y);
                        CreateEnemy(0, -(x + j * (eny_w + bwt_w * 3)) + 600, -y);
                    }
                }
            }
			break;
		case  4://X-type
            {
                float point_x = eny_w * 2 + bwt_w * 2 + 600;
                float point_y = 0;

                int n = Random.Range(0, 4);
                CreateEnemy(0, point_x, point_y);


                for (int i = 0; i < 2; i++)
                {
                    CreateEnemy(0, point_x - (eny_w + bwt_w) * (i + 1), point_y + (eny_h + bwt_h) * (i + 1));
                    CreateEnemy(0, point_x + (eny_w + bwt_w) * (i + 1), point_y + (eny_h + bwt_h) * (i + 1));
                    CreateEnemy(0, point_x - (eny_w + bwt_w) * (i + 1), point_y - (eny_h + bwt_h) * (i + 1));
                    CreateEnemy(0, point_x + (eny_w + bwt_w) * (i + 1), point_y - (eny_h + bwt_h) * (i + 1));
                }
            }
			break;
		case 5://Z-type
			float center_x=eny_w*2+bwt_w*2+600;
			float center_y=0;
			float center_y_upper=center_y+(eny_h+bwt_h)*2;
			float center_y_down=center_y-(eny_h+bwt_h)*2;
			CreateEnemy(0,center_x,center_y);
			CreateEnemy(0,center_x,center_y_upper);
			CreateEnemy(0,center_x,center_y_down);
			for(int i=0;i<2;i++)
			{
				CreateEnemy(0,center_x+(eny_w+bwt_w+10)*(i+1),center_y_down);
				CreateEnemy(0,center_x-(eny_w+bwt_w+10)*(i+1),center_y_down);
				CreateEnemy(0,center_x+(eny_w+bwt_w+10)*(i+1),center_y_upper);
				CreateEnemy(0,center_x-(eny_w+bwt_w+10)*(i+1),center_y_upper);
			}
			CreateEnemy(0,center_x+eny_w+bwt_w,center_y+eny_h+bwt_h);
			CreateEnemy(0,center_x-eny_w-bwt_w,center_y-eny_h-bwt_h);
			break;
		case 6://Circle
			float before_x=600-eny_w-bwt_w-50;
			float after_x=600+eny_w+bwt_w+50;

			CreateEnemy(0,before_x,0);
			CreateEnemy(0,after_x,0);
			for(int i=0;i<2;i++)
			{
				CreateEnemy(0,before_x,eny_h+bwt_h);
				CreateEnemy(0,after_x,eny_h+bwt_h);
				CreateEnemy(0,before_x,-eny_h-bwt_h);
				CreateEnemy(0,after_x,-eny_h-bwt_h);
			}
			CreateEnemy(0,600,(eny_h+bwt_h)*2);
			CreateEnemy(0,600,-(eny_h+bwt_h)*2);
			break;
		}
	}
	void BossPresenting()
	{
		BossAlreadyPresent = true;
		m_JBBoss = Instantiate (bossObj) as GameObject;
		if (m_JBBoss.name == "dark_spirit(Clone)") 
		{
			m_JBBoss.GetComponent<JBDarkSpirit> ().m_BossName = JBDB.levelBoss.boss_name [0];
		}
		else 
		{
			m_JBBoss.GetComponent<JBBoss>().m_BossName=JBDB.levelBoss.boss_name[0];
		}
		m_JBBoss.transform.SetParent (EnemyHouse.transform);
		m_JBBoss.name="Boss";
		m_JBBoss.transform.localPosition = new Vector2 (1.0f,0.0f);
		m_JBBoss.transform.localScale = new Vector2 (1,1);
		Making_State = Enemy_making.end;

        m_JB_MainCharacter.GetComponent<JBMainCharacterObj>().BossPresent = true;
	}
	IEnumerator BonusPresenting()
	{
		while (BonusNumber>0) {
			yield return new WaitForSeconds(45.0f);
		if (GM_state == GameState.playing)
		{
			int number=Random.Range(1,7);
			if(number==1) BonusName="flower";
			
			if(number==2) BonusName="heart";
			
			if(number==3) BonusName="honey";
			
			if(number>=4) BonusName="kiss";

			float ran_x = Random.Range (450.0f, 600.0f);
			float ran_y = Random.Range (-160.0f, 160.0f);

			GameObject item=Instantiate(m_BonusPrefab) as GameObject;
			item.name=BonusName;
			item.GetComponent<JBBonusScript>().m_Itemname=BonusName;
			item.transform.SetParent(BonusRoot.transform);
			item.transform.localPosition=new Vector3(ran_x,ran_y,0);
			item.transform.localScale=new Vector3(1,1,1);
			BonusNumber--;
		}
	}
   }
void ObstaclePresenting()
	{
		int number = Random.Range (0,6);

		ObstacleType type = (ObstacleType)(number);
		string obsName;
		if (number == 0) {
			obsName = "FallingRock";
		} else if (number == 1) {
			obsName = "StaticRock";
		} else if (number == 2) {
			obsName = "RollingRock";
		} else if (number == 3) {
			obsName = "Plant";
		} else if (number == 4) {
			obsName = "poison_gas";
		} else if (number == 5) {
			obsName = "spining_razor";
		} else {
			obsName = "spining_razor";
		}
		obsObj = Resources.Load<Object> ("Prefabs/"+obsName) as GameObject;
		GameObject _obs = Instantiate (obsObj) as GameObject;
		_obs.name = "obstacle";
		_obs.GetComponent<JBObstacle> ().m_type = type;
		_obs.transform.SetParent (EnemyHouse.transform);
		_obs.transform.localPosition = new Vector3 (2,-250,0);
		_obs.transform.localScale = Vector3.one;
	}
   	public void MoveChar(Vector2 delta)
	{
		m_JB_MainCharacter.transform.localPosition = new Vector3 (m_JB_MainCharacter.transform.localPosition.x + /*Time.deltaTime * 12 *  */delta.x,
		                                                        m_JB_MainCharacter.transform.localPosition.y + /*Time.deltaTime * 12 * */delta.y,0);
	}
	public void Shoot()
	{
		if (shootEnable == false)
			   return;

		//m_JB_MainCharacter.GetComponent<JBMainCharacterObj>().ShootingAnim();
        int i = 1;
        if (JBDB.characterName == "BumbleBee" || JBDB.characterName == "KillerBee")
            i = 3;

        for (int j = 0; j < i; j++)
        {
            GameObject Bullet_Child = Instantiate(m_BulletPrefab) as GameObject;
            Bullet_Child.GetComponent<Image>().sprite = Resources.Load<Sprite>("Atlas/" + JBDB.characterName + "/" + JBDB.characterName + "Bullet");
            Bullet_Child.transform.SetParent(m_BulletParent.transform);

            if (m_bAttackDirInvert)
                Bullet_Child.transform.localPosition = m_JB_MainCharacter.transform.localPosition + new Vector3(-50, -10, 0);
            else
                Bullet_Child.transform.localPosition = m_JB_MainCharacter.transform.localPosition + new Vector3(50, -10, 0);

            Bullet_Child.transform.localScale = Vector2.one;
            Bullet_Child.GetComponent<JBBullet>().direction = j;
        }

		shootCount = 0;
		shootEnable = false;
	}
	public void HiveAttackMessage()
	{
		m_HoneyMessage.SetActive (true);
		int n = Random.Range (0,4);
		string enemyMessage,enemyName;
		if (n == 0) 
		{
			enemyMessage="Bear";
			enemyName="bear";
		} 
		else if (n == 1) 
		{
			enemyMessage="BeeEater";
			enemyName="bee_eater";

		}
		else
		{
			enemyMessage="Badger";
			enemyName="honey_badger";
		}
		m_HoneyMessage.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Atlas/"+enemyMessage+"/"+enemyMessage);
		m_HoneyMessage.GetComponent<JBSuperEnemy>().m_name=enemyName;
		m_HoneyMessage.GetComponent<JBSuperEnemy> ().SetEnemyData ();
	}
	public void SetBackGround()
	{
		if (JBDB.levelint == 1) 
		{
			backSpeed=3;
            for (int i = 0; i < 4; i++)
            {
                m_Background.transform.GetChild(0).GetChild(i).GetComponent<Image>().color = new Color32(50, 50, 50, 255);
                m_Background.transform.GetChild(1).GetChild(i).GetComponent<Image>().color = new Color32(50, 50, 50, 255);
            }
			return;
		}
        if (JBDB.levelint == 2)
        {
            backSpeed = 3;
            for (int i = 0; i < 4; i++)
            {
                m_Background.transform.GetChild(0).GetChild(i).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                m_Background.transform.GetChild(1).GetChild(i).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
            return;
        }
		else 
		{
            int rand = Random.Range(1, 10);

			m_BackMossaic1.SetActive(false);
			m_BackMossaic2.SetActive(false);
			backSpeed=5f;
			string pre;
			if(JBDB.levelint%7==0)
			{
				pre="B";
			}
			else if(JBDB.levelint%7==1)
			{
				pre="C";
			}
			else if(JBDB.levelint%7==2)
			{
				pre="D";
			}
			else if(JBDB.levelint%7==3)
			{
				pre="E";
			}
			else if(JBDB.levelint%7==4)
			{
				pre="F";
			}
			else if(JBDB.levelint%7==5)
			{
				pre="G";
			}
            else
            {
                pre = "A";
                m_BackMossaic1.SetActive(true);
                m_BackMossaic2.SetActive(true);

            }

            int nType = 0;
            if (rand <= 6) nType = 1;
            else if (rand <= 8) nType = 2;
            else nType = 3;

            if (m_LastBGIndex == nType)
            {
                nType++;
                if (nType > 3)
                    nType = 1;
            }

            m_LastBGIndex = nType;

			for(int i=0;i<4;i++)
			{
				m_Background.transform.GetChild(0).GetChild(i).GetComponent<Image>().sprite=Resources.Load<Sprite>("Atlas/Back/"+pre+"_0"+(i+1));
				m_Background.transform.GetChild(1).GetChild(i).GetComponent<Image>().sprite=Resources.Load<Sprite>("Atlas/Back/"+pre+"_0"+(i+1));

                if (nType == 1)
                {
                    m_Background.transform.GetChild(0).GetChild(i).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                    m_Background.transform.GetChild(1).GetChild(i).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                }
                else if (nType == 2)
                {
                    m_Background.transform.GetChild(0).GetChild(i).GetComponent<Image>().color = new Color32(40, 40, 255, 255);
                    m_Background.transform.GetChild(1).GetChild(i).GetComponent<Image>().color = new Color32(40, 40, 255, 255);
                }
                else if (nType == 3)
                {
                    m_Background.transform.GetChild(0).GetChild(i).GetComponent<Image>().color = new Color32(250, 100, 13, 255);
                    m_Background.transform.GetChild(1).GetChild(i).GetComponent<Image>().color = new Color32(250, 100, 13, 255);
                }
			}
		}
	}

    public static void ResetBulletTime()
    {
        shootCount = 0;
        shootEnable = true;
    }
}
