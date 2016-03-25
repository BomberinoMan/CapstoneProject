using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class DBConnection
{
    private static DBConnection _instance;
    private static bool _hasInit = false;
    private string _uri = "https://142.3.21.28/{0}.php?Action={1}";
    private string _superSecretCode = "21548b73-0f5e-42e9-9038-ffb14458119b";

    private DBConnection() { }
    public static DBConnection GetInstance()
    {
        if (_hasInit)
            return _instance;

        ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

        _instance = new DBConnection();
        return _instance;
    }

    public LoginResponse Login(LoginMessage message)
    {
        return Connect<LoginResponse>("User", "Login", new NameValueCollection()
        {
            { "UserName", message.userName },
            { "Password", Hash(message.password) }
        });
    }

    public CreateUserResponse CreateUser(CreateUserMessage message)
    {
        return Connect<CreateUserResponse>("User", "CreateUser", new NameValueCollection()
        {
            { "UserName", message.userName },
            { "Password", Hash(message.password) }
        });
    }

    public ChangePasswordResponse ChangePassword(ChangePasswordMessage message)
    {
        return Connect<ChangePasswordResponse>("User", "ChangePassword", new NameValueCollection()
        {
            { "UserName", message.userName },
            { "OldPassword", Hash(message.oldPassword) },
            { "NewPassword", Hash(message.newPassword) }
        });
    }

    public CreateRoomResponse CreateRooom(CreateRoomMessage message)
    {
        return Connect<CreateRoomResponse>("Matchmaking", "CreateRoom", new NameValueCollection()
        {
            { "UserId", message.userId.ToString() },
            { "Name", message.name },
            { "IP", message.ip }
        });
    }

    public DeleteRoomResponse DeleteRoom(DeleteRoomMessage message)
    {
        return Connect<DeleteRoomResponse>("Matchmaking", "DeleteRoom", new NameValueCollection()
        {
            { "UserId", message.userId.ToString() }
        });
    }

    public ListRoomsResponse ListRooms(ListRoomsMessage message)
    {
        return Connect<ListRoomsResponse>("Matchmaking", "ListRooms", new NameValueCollection()
        {
            { "UserId", message.userId.ToString() }
        });
    }

    private T Connect<T>(string endpoint, string action, NameValueCollection values)
    {
        values.Add(new NameValueCollection() { { "SuperSecretCode", _superSecretCode } });
        using (WebClient client = new WebClient())
        {
            try
            {
                byte[] response = client.UploadValues(string.Format(_uri, endpoint, action), values);
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

