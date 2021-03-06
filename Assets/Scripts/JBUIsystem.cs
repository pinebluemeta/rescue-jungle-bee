using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using dataread;
using JBClasses;

public class JBUIsystem : MonoBehaviour {
	public GameObject JB_UIobj;//including UI_items
	public GameObject JB_MainCharacter;
	//bool joystick_clicked=false;
	//public static bool Shoot_clicked=false;
	public GameObject LifeDlg;
	public GameObject CoinDlg;
	public JBPauseController _pauseDlg;
	public static JBUIsystem _intance;
	public  GameObject m_gameOverDlg;
	public GameObject m_gameCompleteDlg;
	public GameObject m_totalCompleteDlg;
	public Text m_leveltext;
	public GameObject m_BeckyMessage;
	public GameObject m_DisguiseMessage;
    public GameObject m_continuePayDlg;
    public GameObject m_RescueParticle;

    public GameObject m_BeckyIcon;
    public GameObject m_DarkIcon;

	// Use this for initialization
	void Start () 
	{
		_intance = this;
		SetDlgCharAnimIMG ();
	}
	// Update is called once per frame
	void Update () 
	{
		m_leveltext.text = JBDB.Lives.ToString ();
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			StartCoroutine("Pause_Click");
		}
	}
	public void Pause_Click()
	{

		if (JBKernel.GM_state == JBKernel.GameState.playing) {
			JBKernel.GM_state = JBKernel.GameState.stop;
			_pauseDlg.gameObject.SetActive(true);
		} 
		else 
		{
			JBKernel.GM_state = JBKernel.GameState.playing;
			_pauseDlg.gameObject.SetActive(false);
		}
	}
	public void Complete(GameObject obj)
	{

		if (obj.name == "Retry") 
		{
			JBDB.gamestate=false;
			Application.LoadLevel("Loading_Scene2");
		}
		if (obj.name == "Home") 
		{
			Application.LoadLevel ("Start_Scene");
			JBDB.TotalScore+=JBKernel.score;
			JBDB.SaveGameDataText();
		}
		if (obj.name == "Next") 
		{
			JBDB.gamestate=false;
			JBDB.TotalScore+=JBKernel.score;
			JBDB.Coin+=JBKernel.score/100;

            if (JBDB.levelint == 3)
            {
                JBDB.levelint++;
                JBDB.SaveGameDataText();
                JBKernel.GM_state = JBKernel.GameState.end_complete_kidnap_blossom;
            }
            else if (!JBDB.IsPayCompleted && JBDB.levelint == 5)
            {
                OpenContinuePayDlg();
            }
            else
            {
                //if (JBDB.Coin < 2000)
                //{
                //    CoinDlg.SetActive(true);
                //}
                //else
                {
                    //JBDB.Coin -= 2000;
                    JBDB.levelint++;
                    JBDB.SaveGameDataText();
                    Application.LoadLevel("Loading_Scene2");
                }

            }
		}
	}
	public void GameOver(GameObject obj)
	{
		if (obj.name == "Retry")
		{
			JBKernel.GM_state=JBKernel.GameState.playing;
			JBDB.gamestate=false;
			if(JBDB.Lives<=0)
			{
				JBLifeTimer.m_start=true;
				OpenLifeDlg();
			}
			else 
			{
				JBDB.Lives--;
				Application.LoadLevel("Loading_Scene2");
			}
		} 
		else
		{
			if(JBDB.Lives>0)
			{
				JBDB.Lives--;
				Application.LoadLevel("Start_Scene");
			}
			else
			{
				JBLifeTimer.m_start=true;
				OpenLifeDlg();
			}
		}
	}
	public void OpenGameOverDlg()
	{
		m_gameOverDlg.SetActive (true);
	}
	void OpenLifeDlg()
	{
		LifeDlg.SetActive (true);
	}
    void OpenContinuePayDlg()
    {
        m_continuePayDlg.SetActive(true);
    }

	public void OpenGameCompleteDlg()
	{
		m_gameCompleteDlg.SetActive (true);
	}
	public void OpenTotalCompleteDlg()
	{
		m_totalCompleteDlg.SetActive (true);
	}

    public void ShowBeckyIcon()
    {
        m_BeckyIcon.SetActive(true);
        m_DarkIcon.SetActive(false);
    }

    public void ShowDarkIcon()
    {
        m_BeckyIcon.SetActive(false);
        m_DarkIcon.SetActive(true);
    }
    public void HideBeckyDarkIcon()
    {
        m_BeckyIcon.SetActive(false);
        m_DarkIcon.SetActive(false);
    }


	public void DisplayMothText(string str)
	{
		m_DisguiseMessage.SetActive (true);
		m_DisguiseMessage.GetComponent<Text> ().text = str;
	}
	public void DisappearText()
	{
		m_DisguiseMessage.SetActive (false);
	}
	public void SetDlgCharAnimIMG ()
	{
		Transform beeMark_happy = m_gameCompleteDlg.transform.Find ("Bee");
		beeMark_happy.GetComponent<Image>().sprite=JBDB.ImportedCharIMG[4];//4,5
		beeMark_happy.GetComponent<JBSpriteAnim> ().Loop = true;
		beeMark_happy.GetComponent<JBSpriteAnim>()._prefix=JBDB.characterName+"Happy";
		beeMark_happy.GetComponent<JBSpriteAnim> ().frames = 6;
		beeMark_happy.GetComponent<JBSpriteAnim> ()._folderName = "Atlas/"+JBDB.characterName;

		Transform beeMark_failed = m_gameOverDlg.transform.Find ("Bee");
		beeMark_failed.GetComponent<Image>().sprite=JBDB.ImportedCharIMG[0];//0,1
		beeMark_failed.GetComponent<JBSpriteAnim> ().Loop = true;
		beeMark_failed.GetComponent<JBSpriteAnim> ()._prefix = JBDB.characterName + "Failed";
		beeMark_failed.GetComponent<JBSpriteAnim> ().frames = 2;
		beeMark_failed.GetComponent<JBSpriteAnim> ()._folderName = "Atlas/"+JBDB.characterName;

		Transform beeMark_menu = _pauseDlg.transform.Find ("Bee");
		beeMark_menu.GetComponent<Image>().sprite=JBDB.ImportedCharIMG[7];//7,8
		beeMark_menu.GetComponent<JBSpriteAnim> ().Loop = true;
		beeMark_menu.GetComponent<JBSpriteAnim> ()._prefix = JBDB.characterName + "Menu";
		beeMark_menu.GetComponent<JBSpriteAnim> ().frames = 2;
		beeMark_menu.GetComponent<JBSpriteAnim>()._folderName="Atlas/"+JBDB.characterName;
	}
}