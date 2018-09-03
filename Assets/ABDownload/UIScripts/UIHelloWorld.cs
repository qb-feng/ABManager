using UnityEngine;
using System.Collections;

public class UIHelloWorld : MonoBehaviour
{
    void Awake() 
    {
        Debug.LogWarning("UIHelloWorld");
    }

    void OnEnable()
    {
        Debug.LogWarning("UIHelloWorld OnEnable !!!!");
    }

    void OnDisable() 
    {
        Debug.LogWarning("UIHelloWorld OnDisable !!!!");
    }
}
