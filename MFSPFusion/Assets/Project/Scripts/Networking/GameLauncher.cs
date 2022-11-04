using UnityEngine;
using UnityEngine.UI;

public class GameLauncher : MonoBehaviour
{
    [SerializeField] Button connectBtn;

    FusionCallbacks fusionCallbacks;

    void Start()
    {
        connectBtn.onClick.AddListener(Connect);
        fusionCallbacks = GetComponent<FusionCallbacks>();
    }

    void Connect()
    {
        fusionCallbacks.Launch();
    }
}
