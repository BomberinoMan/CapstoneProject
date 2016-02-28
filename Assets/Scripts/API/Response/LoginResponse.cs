using System;

[Serializable]
public class LoginResponse
{
    public string userId;
    public bool isSuccessful;
    public string errorMessage;
}
