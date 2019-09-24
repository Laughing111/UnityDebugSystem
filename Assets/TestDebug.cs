using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDebug : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("www:你好你好你好");
        StartCoroutine(DebugCoroutine());
    }

    private IEnumerator DebugCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            Debug.Log("HTTP:ansfafahf9uaof;a");
            Debug.Log("WebSocket:sdafagsdhjj");
        }
    }
}
