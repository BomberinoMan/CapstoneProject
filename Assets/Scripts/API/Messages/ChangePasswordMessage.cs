using System;

[Serializable]
public class ChangePasswordMessage
{
    public string UserName;
    public string OldPassword;
    public string NewPassword;
}
