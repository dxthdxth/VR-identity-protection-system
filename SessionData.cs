using UnityEngine;

public static class SessionData
{
    public static string username = "";
    public static string token = "";
    public static string meetingId = "";
    public static string role = "";

    public static void Clear()
    {
        username = "";
        token = "";
        meetingId = "";
        role = "";
    }

    public static bool IsLoggedIn()
    {
        return !string.IsNullOrEmpty(username) &&
               !string.IsNullOrEmpty(token);
    }

    public static void Print()
    {
        Debug.Log($"USER={username}, TOKEN={token}, MEETING={meetingId}, ROLE={role}");
    }
}