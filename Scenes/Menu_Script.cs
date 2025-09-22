using UnityEngine;
using UnityEngine.SceneManagement;
public class Menu_Script : MonoBehaviour
{
    public void playGame()
    {
        SceneManager.LoadScene("Game");
    }

}