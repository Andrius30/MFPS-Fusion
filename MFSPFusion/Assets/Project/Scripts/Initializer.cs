using UnityEngine;
using UnityEngine.SceneManagement;

public class Initializer : MonoBehaviour
{
    [SerializeField] int sceneIndex;

    void Awake()
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
