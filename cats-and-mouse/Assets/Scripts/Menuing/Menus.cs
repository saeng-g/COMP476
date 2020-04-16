using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menus : MonoBehaviour
{
    readonly string titleSceneName = "TitleScreen";
    readonly string mainLevelName  = "ActiveLevel3";

    readonly string gameOverHeaderOnLoss = "Game over!";
    readonly string gameOverHeaderOnWin  = "You win!";

    readonly string gameOverTextOnLoss = "You were caught by one of the cats.";
    readonly string gameOverTextOnWin = "You successfully got all the cheese.";

    //holds an instance of the ending screen to create upon the game finishing (for either a loss or win)
    public GameObject endingScreen, canvas;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    #region TitleScreen
    //methods to call for buttons on the title screen
    public void StartGame() {
        SceneManager.LoadScene(mainLevelName);
    }
    public void QuitGame() {
        Application.Quit();
    }
    #endregion

    #region GameOver
    //methods to call for buttons on the game over screen
    public void PlayAgain() {
        SceneManager.LoadScene(mainLevelName); //reset the level scene in case of trying again
    }
    public void BackToTitle() {
        SceneManager.LoadScene(titleSceneName);
    }
    #endregion

    //functions to call in game (particlarly for when game ends)
    public void GameOverScreen(bool gameWon) {
        GameObject screen = Instantiate(endingScreen);
        screen.transform.parent = canvas.transform;
        screen.transform.localPosition = Vector3.zero;
        screen.transform.localScale = Vector3.one;

        Text header  = GameObject.Find("Header").GetComponent<Text>();
        Text message = GameObject.Find("Message").GetComponent<Text>();

        Button playAgain = GameObject.Find("PlayAgainButton").GetComponent<Button>();
        Button title     = GameObject.Find("TitleButton").GetComponent<Button>();
        //add methods to trigger on each of these button clicks
        playAgain.onClick.AddListener(() => PlayAgain());
        title.onClick.AddListener(() => BackToTitle());

        if (gameWon) {
            header.text = gameOverHeaderOnWin;
            message.text = gameOverTextOnWin;
        }
        else {
            header.text = gameOverHeaderOnLoss;
            message.text = gameOverTextOnLoss;
        }
    }
}
