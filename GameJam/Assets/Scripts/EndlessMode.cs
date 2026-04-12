using UnityEngine;

public class EndlessMode : MonoBehaviour
{

    public static EndlessMode Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

}
