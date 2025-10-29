// PlayerStats.cs
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public static class PlayerStats
{
    public const string KEY_POINTS = "Points";
    public const string KEY_RANK = "Rank";

    // Call this once, per client, after joining a room.
    public static void InitializeLocalPlayer()
    {
        Player local = PhotonNetwork.LocalPlayer;
        if (!local.CustomProperties.ContainsKey(KEY_POINTS))
        {
            var props = new Hashtable
            {
                { KEY_POINTS, 0 },
                { KEY_RANK,   DetermineRank(0) }
            };
            local.SetCustomProperties(props);
        }
    }

    // MasterClient (or whoever is authoritative) calls this to add/subtract points.
    public static (int updatedXP, string newRank) AddPointsToPlayer(Player player, int delta)
    {
        int current = 0;
        if (player.CustomProperties.ContainsKey(KEY_POINTS))
            current = (int)player.CustomProperties[KEY_POINTS];

        int updated = Mathf.Clamp(current + delta, 0, 500);
        string newRank = DetermineRank(updated);

        var newProps = new Hashtable
    {
        { KEY_POINTS, updated },
        { KEY_RANK, newRank }
    };
        player.SetCustomProperties(newProps);

        return (updated, newRank);
    }


    // Maps 0–500 → rank string
    public static string DetermineRank(int points)
    {
        if (points <= 100) return "Iron";
        if (points <= 200) return "Iron2";
        if (points <= 300) return "Silver";
        if (points <= 400) return "Gold";
        return "Master";
    }
}
