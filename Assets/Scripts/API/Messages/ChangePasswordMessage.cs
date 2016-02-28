using System;

[Serializable]
public class ChangePasswordMessage
{
    public string userName;
    public string oldPassword;
    public string newPassword;
}
