using System;

[Serializable]
public class CreateRoomMessage : ListRoomsMessage
{
    public string name;
    public string ip;
}
