using System;

public static class LoginInformation
{
    public static string username = "";
    public static Guid guid = Guid.Empty;
    public static bool loggedIn = false;
}

/*using System;
using UnityEngine;

public class LoginInformation : MonoBehaviour
{
    public static LoginInformation instance;
    public string userName = "";
    public Guid guid = Guid.Empty;
    public bool loggedIn = false;
    
    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(transform.gameObject);
    }
}
*/
