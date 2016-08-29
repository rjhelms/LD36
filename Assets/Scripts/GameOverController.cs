using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverController : MonoBehaviour {

    public GameObject[] GameOverTexts;
    public float tickSpeed = 0.5f;

    private float nextTick;
    private int state;

    private AudioSource tickSoundSource;

    // Use this for initialization

	void Start () {
        GameOverTexts[2].GetComponent<Text>().text = string.Format("{0}", ScoreManager.Instance.Level - 1);
        GameOverTexts[5].GetComponent<Text>().text = string.Format("{0}", ScoreManager.Instance.Score);
        GameOverTexts[0].SetActive(true);
        state = 0;
        nextTick = Time.time + tickSpeed;
        tickSoundSource = this.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        if (state <= 5)
        {
            if (Time.time > nextTick)
            {
                nextTick += tickSpeed;
                state++;
                GameOverTexts[state].SetActive(true);
                tickSoundSource.Play();
            }
        } else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
            if (Input.anyKeyDown)
            {
                ScoreManager.Instance.Score = 0;
                ScoreManager.Instance.Level = 1;
                ScoreManager.Instance.Lives = 3;
                ScoreManager.Instance.HitPoints = 3;
                SceneManager.LoadScene(0);
            }
        }

	}
}
