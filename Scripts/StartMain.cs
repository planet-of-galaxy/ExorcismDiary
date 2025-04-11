using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMain : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.ShowPanel<StartPanel>();
    }
}
