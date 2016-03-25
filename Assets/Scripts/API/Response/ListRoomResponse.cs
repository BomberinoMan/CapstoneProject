using System;

[Serializable]
public class ListRoomsResponse : GeneralResponse
{
    public Room[] rooms;
}

[Serializable]
public class Room
{
    public string name;
    public string ip;
}