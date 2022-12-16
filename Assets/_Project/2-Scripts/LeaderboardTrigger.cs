using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardTrigger : MonoBehaviour
{
    public void OpenLeaderboard() => Leaderboard.current.ShowLeaderboardCanvas();
}
