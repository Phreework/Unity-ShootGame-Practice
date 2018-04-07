using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager   Instance = null;
    public int                  m_score = 0;        //game score
    public static int           m_hiscore = 0;      //high score
    public int                  m_ammo = 100;       //ammo
    Player                      m_player;
    Text                        txt_ammo;
    Text                        txt_hiscore;
    Text                        txt_life;
    Text                        txt_score;
    Button                      button_restart;     //restart button

    void Start() {
        Instance = this;
        //get player
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        //get canva
        GameObject uicanvas = GameObject.Find("Canvas");
        UIInit(uicanvas);
    }

   
    // Update is called once per frame
    void Update() {
        
    }

    #region Extra Method
    private void UIInit(GameObject uicanvas) {
        Transform[] uiChildren = GetAllUICom(uicanvas);
        InitAllUICom(uiChildren);
    }

    private static Transform[] GetAllUICom(GameObject uicanvas) {
        return uicanvas.GetComponentsInChildren<Transform>();
    }

    private void InitAllUICom(Transform[] uiChildren) {
        //foreach
        foreach (Transform t in uiChildren) {
            if (t.name.CompareTo("txt_ammo") == 0) {
                txt_ammo = t.GetComponent<Text>();
            } else if (t.name.CompareTo("txt_hiscore") == 0) {
                txt_hiscore = t.GetComponent<Text>();
                txt_hiscore.text = "High Score" + m_hiscore;
            } else if (t.name.CompareTo("txt_life") == 0) {
                txt_life = t.GetComponent<Text>();
            } else if (t.name.CompareTo("txt_score") == 0) {
                txt_score = t.GetComponent<Text>();
            } else if (t.name.CompareTo("Restart Button") == 0) {
                InitRestartButton(t);
            }
        }
    }

    private void InitRestartButton(Transform t) {
        button_restart = t.GetComponent<Button>();
        button_restart.onClick.AddListener(delegate () {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
        button_restart.gameObject.SetActive(false);
    }

    public void RenewScore(int score) {
        m_score += score;
        if (m_score > m_hiscore)
            m_hiscore = m_score;
        txt_score.text = "Score<color=yellow>" + m_score + "</color>";
        ;
        txt_hiscore.text = "High Score" + m_hiscore;
    }
    public void RenewAmmo(int ammo) {
        m_ammo -= ammo;
        if (m_ammo <= 0)
            m_ammo = 100 - m_ammo;
        txt_ammo.text = m_ammo.ToString() + "/100";
    }
    public void RenewLife(int life) {
        txt_life.text = life.ToString();
        if (life <= 0)
            button_restart.gameObject.SetActive(true);
    }
#endregion
}
