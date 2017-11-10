using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    public readonly string RANKING_KEY = "RANKING";
    public readonly int maxRanking = 30;
    private bool isRunningAutoSave = false;

    List<UserData> ranking = new List<UserData>();

    // Use this for initialization
    void Start()
    {
        FetchRankingFromJson();
    }

    void Example()
    {
        CharangeRanking(new UserData("212121", "Ryohei", 1900));
        foreach (UserData r in ranking)
            print(r.ToString());

        //print(GetUserDataInRanking(1).ToString());
        //print(GetUserDataInRanking(3).ToString());

        DataStorage.Save();
    }
    void OnEnable()
    {
        var levelInstance = GameManager.levelInstance as LevelInstance_Game;
        if (levelInstance)
            levelInstance.levelTimeEnded += OnLevelTimeEnded;
    }
    void OnDisable()
    {
        var levelInstance = GameManager.levelInstance as LevelInstance_Game;
        if (levelInstance)
            levelInstance.levelTimeEnded -= OnLevelTimeEnded;
    }
    public void ResetAllRanking()
    {
        DataStorage.Clear();
        FillInUnknown();
        DataStorage.SetList(RANKING_KEY, ranking);
        DataStorage.Save();
    }
    public void FillInUnknown()
    {
        for (int i = 0; i < maxRanking; i++)
            if (ranking.Count <= i)
                ranking.Add(UserData.Empty);
    }
    public void RemoveRanking(UserData userData)
    {
        if (ranking.Remove(userData))
            Debug.Log("Remove completed!");
        Debug.LogError("Did NOT complete!");
    }
    public void RemoveRankingWithID(string id)
    {
        if (ranking.Remove(GetUserDataWithID(id)))
            Debug.Log("Remove completed!");
        Debug.LogError("Did NOT complete!");
    }
    public void RemoveRankingWithName(string name)
    {
        if (ranking.Remove(GetUserDataWithName(name)))
            Debug.Log("Remove completed!");
        Debug.LogError("Did NOT complete!");
    }
    public void RemoveRankingWithRank(int rank)
    {
        if (ranking.Remove(GetUserDataInRanking(rank)))
            Debug.Log("Remove completed!");
        Debug.LogError("Did NOT complete!");
    }
    public void AutoSave(float interval)
    {
        StartCoroutine(AutoSaveCoroutine(interval));
    }
    IEnumerator AutoSaveCoroutine(float interval)
    {
        if (isRunningAutoSave)
            yield break;
        isRunningAutoSave = true;

        yield return new WaitForSeconds(interval);
        DataStorage.Save();
    }
    public void FetchRankingFromJson()
    {
        ranking = DataStorage.GetList(RANKING_KEY, new List<UserData>());
        if (ranking.Count <= 0)
            ResetAllRanking();
    }

    public void Sort()
    {
        ranking.Sort((a, b) => b.score - a.score);
    }

    /// <summary>
    /// Get any data from ranking.
    /// </summary>
    /// <param name="rank">1~maxRanking</param>
    /// <returns></returns>
    public UserData GetUserDataInRanking(int rank)
    {
        rank--;
        CheckRank(rank);
        return ranking[rank];
    }
    public string GetUserIDInRanking(int rank)
    {
        rank--;
        CheckRank(rank);
        return ranking[rank].id;
    }
    public string GetUserNameInRanking(int rank)
    {
        rank--;
        CheckRank(rank);
        return ranking[rank].name;
    }
    public int GetUserScoreInRanking(int rank)
    {
        rank--;
        CheckRank(rank);
        return ranking[rank].score;
    }

    // Get user data.
    public UserData GetUserDataWithID(string id)
    {
        UserData userData;
        TryGetUserDataWithID(id, out userData);
        return userData;
    }
    public UserData GetUserDataWithName(string name)
    {
        UserData userData;
        TryGetUserDataWithName(name, out userData);
        return userData;
    }

    // Get user's ID.
    public string GetUserIDWithName(string name)
    {
        string userID;
        TryGetUserIDWithName(name, out userID);
        return userID;
    }

    // Get user's name.
    public string GetUserNameWithID(string id)
    {
        string userName;
        TryGetUserNameWithID(id, out userName);
        return userName;
    }

    // Get user's score.
    public int GetUserScoreWithID(string id)
    {
        UserData userData;
        if (TryGetUserDataWithID(id, out userData))
            return userData.score;
        return 0;
    }
    public int GetUserScoreWithName(string name)
    {
        int score;
        TryGetUserScoreWithName(name, out score);
        return score;
    }

    // Try to get user data.
    public bool TryGetUserDataWithID(string id, out UserData userData)
    {
        foreach (var r in ranking)
            if (r.id == id)
            {
                userData = r;
                return true;
            }
        userData = null;
        return false;
    }
    public bool TryGetUserDataWithName(string name, out UserData userData)
    {
        foreach (var r in ranking)
            if (r.name == name)
            {
                userData = r;
                return true;
            }
        userData = null;
        return false;
    }

    // Try to get user's score.
    public bool TryGetUserScoreWithID(string id, out int score)
    {
        UserData userData;
        if (TryGetUserDataWithID(id, out userData))
        {
            score = userData.score;
            return true;
        }
        score = 0;
        return false;
    }
    public bool TryGetUserScoreWithName(string name, out int score)
    {
        UserData userData;
        if (TryGetUserDataWithName(name, out userData))
        {
            score = userData.score;
            return true;
        }
        score = 0;
        return false;
    }

    // Try to get user's id.
    public bool TryGetUserIDWithName(string name, out string id)
    {
        UserData userData;
        if (TryGetUserDataWithName(name, out userData))
        {
            id = userData.id;
            return true;
        }
        id = string.Empty;
        return false;
    }

    // Try to get user's name.
    public bool TryGetUserNameWithID(string id, out string name)
    {
        UserData userData;
        if (TryGetUserDataWithID(id, out userData))
        {
            name = userData.name;
            return true;
        }
        name = string.Empty;
        return false;
    }
    private void InsertToRank(int rank, UserData data)
    {
        CheckRank(rank);
        ranking.Insert(rank, data);
    }
    private void CheckRank(int rank)
    {
        if (rank < 0 || rank > ranking.Count)
            throw new ArgumentException("This parameter is outside of ranking!");
    }

    // You should use this function when time up.
    public void CharangeRanking(UserData charanger)
    {
        // When the first charange.
        for (int i = 0; i < maxRanking; i++)
            if (ranking[i].score < charanger.score)
            {
                InsertToRank(i, charanger);
                // Delete ranking[maxRanking]. So delete last data.
                ranking.RemoveAt(maxRanking);
                return;
            }
    }
    void OnLevelTimeEnded(object sender, LevelInstance_Game.LevelEventArgs args)
    {
        print("Time up!");
    }

}
