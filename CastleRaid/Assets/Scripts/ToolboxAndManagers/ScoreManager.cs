using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Serializable]
    public class UserData
    {
        // Using the open canpus ic card's number
        public string id;
        public string name;
        public int score;

        public UserData(string _id, string _name, int _score)
        {
            id = _id;

            name = _name;
            score = _score;
        }

    }
    List<UserData> ranking = new List<UserData>();

    // Use this for initialization
    void Start()
    {
        ranking = DataStorage.GetList("RANKING", new List<UserData>());

        foreach (UserData r in ranking)
            print(r.name);

        DataStorage.Save();
    }

    void OnEnable()
    {
        LevelInstance_Game li = GameManager.levelInstance as LevelInstance_Game;
        if (li)
            li.levelTimeEnded += OnLevelTimeEnded;
    }
    void OnDisable()
    {
        LevelInstance_Game li = GameManager.levelInstance as LevelInstance_Game;
        if (li)
            li.levelTimeEnded -= OnLevelTimeEnded;
    }
    void ResetRanking()
    {
        DataStorage.Clear();
        for (int i = 0; i < 30; i++)
        {
            ranking.Add(new UserData("000000", "unknown", 0));
        }
        DataStorage.SetList("RANKING", ranking);
        DataStorage.Save();
    }

    // Get any data from ranking.
    public UserData GetUserDataInRanking(int rank)
    {
        CheckRank(rank);
        return ranking[rank];
    }
    public string GetUserIDInRanking(int rank)
    {
        CheckRank(rank);
        return ranking[rank].id;
    }
    public string GetUserNameInRanking(int rank)
    {
        CheckRank(rank);
        return ranking[rank].name;
    }
    public int GetUserScoreInRanking(int rank)
    {
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
        {
            if (r.id == id)
            {
                userData = r;
                return true;
            }
        }
        userData = null;
        return false;
    }
    public bool TryGetUserDataWithName(string name, out UserData userData)
    {
        foreach (var r in ranking)
        {
            if (r.name == name)
            {
                userData = r;
                return true;
            }
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
        {
            throw new ArgumentException("This parameter is outside of ranking!");
        }
    }

    // You should use this function when time up.
    void CharangeRanking(UserData charanger)
    {
        for (int i = 0; i < ranking.Count; i++)
        {
            if (ranking[i].score < charanger.score)
                InsertToRank(i, charanger);
        }
    }
    void OnLevelTimeEnded(object sender, LevelInstance_Game.LevelEventArgs args)
    {
        print("Time up!");
    }

}
