using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class DBConnection {
    private static DBConnection _instance;
    private static bool _hasInit = false;
    private string uri = "https://142.3.21.28/{0}.php?Action={1}";

    private DBConnection() { }
    public static DBConnection Instance()
    {
        if(_hasInit)
            return _instance;

        ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

        _instance = new DBConnection();
        return _instance;
    }

    public LoginResponse Login(LoginMessage message)
    {
        return Connect<LoginResponse>("User", "Login", new NameValueCollection()
        {
            { "UserName", message.UserName },
            { "Password", Hash(message.Password) }
        });
    }

    public CreateUserResponse CreateUser(CreateUserMessage message)
    {
        return Connect<CreateUserResponse>("User", "CreateUser", new NameValueCollection()
        {
            { "UserName", message.UserName },
            { "Password", Hash(message.Password) }
        });
    }

    public ChangePasswordResponse ChangePassword(ChangePasswordMessage message)
    {
        return Connect<ChangePasswordResponse>("User", "ChangePassword", new NameValueCollection()
        {
            { "UserName", message.UserName },
            { "OldPassword", Hash(message.OldPassword) },
            { "NewPassword", Hash(message.NewPassword) }
        });
    }

    private T Connect<T>(string endpoint, string action, NameValueCollection values)
    {
        using (WebClient client = new WebClient())
        {
            try
            {
                byte[] response = client.UploadValues(string.Format(uri, endpoint, action), values);
                return JsonUtility.FromJson<T>(System.Text.Encoding.UTF8.GetString(response));
            }
            catch (WebException e)  // TODO better handling of error message from server, do we want to throw an exception if the sever is down?
            {
                using (var errorResponse = (HttpWebResponse)e.Response)
                {
                    using (var errorResponseReader = new StreamReader(errorResponse.GetResponseStream()))
                    {
                        return JsonUtility.FromJson<T>(errorResponseReader.ReadToEnd());
                    }
                }
            }
        }
    }

    private static bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }

    private static string Hash(string input)
    {
        using (SHA1Managed sha1 = new SHA1Managed())
        {
            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder(hash.Length * 2);

            foreach (byte b in hash)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }

}

