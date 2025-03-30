using UnityEngine;
using UnityEngine.SceneManagement;

public class MainAndEndManager : MonoBehaviour
{
    public void OnPlayClick()
    {
        SceneManager.LoadScene("Gameplay");
    }
    public void OnEndCreditClick()
    {
        SceneManager.LoadScene("EndCredit");
    }
    public void OnBackClick()
    {
        SceneManager.LoadScene("Main");
    }
}
