using UnityEngine;
using UnityEngine.SceneManagement;
public class Menu_Script : MonoBehaviour
{
    public void playGame()
    {
        SceneManager.LoadScene("Gameplay");
    }
    public void playAlmanac()
    {
        SceneManager.LoadScene("Almanac");
    }

}