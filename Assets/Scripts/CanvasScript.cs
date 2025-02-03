using UnityEngine;
using UnityEngine.UI;

public class CanvasScript : MonoBehaviour
{

    public GameObject PauseMenu;
    public bool IsPaused;
    public GameObject WinMenu;
    public GameObject LoseMenu;
    public int score;
    public Text ScoreText1, ScoreText2;
    public PlaySoundOnStart sound;

    public float level;
    public float health;
    public GameObject[] heart;

    public AudioSource audioSource;  // AudioSource dari GameObject
    public AudioClip Gayyyyyy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PauseMenu.SetActive(false);
        WinMenu.SetActive(false);
        LoseMenu.SetActive(false);
        resume();
        sound = GameObject.FindGameObjectWithTag("Player").GetComponent<PlaySoundOnStart>();

        score = PlayerPrefs.GetInt("Score", 0);
    }

    public void StartHeart()
    {
        for (int i = 0; i < heart.Length; i++)
        {
            if (heart[i] != null) // Cek jika tidak null untuk menghindari error
            {
                heart[i].SetActive(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
            {
                resumeGame();
            }
            else
            {
                pauseGame();
            }
        }

        if(health < 1 && !LoseMenu.activeSelf)
        {
            LoseMenu.SetActive(true);
            audioSource.PlayOneShot(Gayyyyyy);

            Time.timeScale = 0f;
            IsPaused = true;

            ScoreText2.text = "Score: " + score + "/1000";
        }

        switch (health)
        { 
            case 9:
                heart[9].SetActive(false);
                break;
            case 8:
                heart[8].SetActive(false);
                break;
            case 7:
                heart[7].SetActive(false);
                break;
            case 6:
                heart[6].SetActive(false);
                break;
            case 5:
                heart[5].SetActive(false);
                break;
            case 4:
                heart[4].SetActive(false);
                break;
            case 3:
                heart[3].SetActive(false);
                break;
            case 2:
                heart[2].SetActive(false);
                break;
            case 1:
                heart[1].SetActive(false);
                break;
            case 0:
                heart[0].SetActive(false);
                break;

        }
    }

    public void resume()
    {
        resumeGame();
    }

    public void Finish()
    {
        switch (level)
        {
            case 1:
                score += 5;
                break;
            case 2:
                score += 10;
                break;
            case 3:
                score += 100;
                break;
            default:
                Debug.Log("bro yang level antara 1-3");
                break;
        }

        WinMenu.SetActive(true);

        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.Save();

        ScoreText1.text = "Score: " + score + "/1000";
        Time.timeScale = 0f;

        //PlayerPrefs.DeleteAll(); // JANGAN LUPA DIHAPUS SEBELUM NGUMPULKAN
    }


    public void pauseGame()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0f;
        IsPaused = true;
    }


    public void resumeGame()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
    }


    public void minOneHealth()
    {
        health--;
    }
}
