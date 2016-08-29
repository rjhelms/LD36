using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleScreenController : MonoBehaviour {

    public GameObject MusicPlayerPrefab;

    // Use this for initialization
    void Start () {
        GameObject[] musicPlayers = GameObject.FindGameObjectsWithTag("Music");
        if (musicPlayers.Length == 0)
        {
            GameObject musicPlayer = Instantiate(MusicPlayerPrefab);
            DontDestroyOnLoad(musicPlayer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            SceneManager.LoadScene("Main");
        }
    }
}
