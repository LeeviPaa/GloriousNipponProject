using System;

[Serializable]
public class UserData
{
    // Using the open canpus IC card's number
    public string id;
    public string name;
    public int score;

    public UserData(string _id, string _name, int _score)
    {
        id = _id;

        name = _name;
        score = _score;
    }

    public override string ToString()
    {
        return "ID:" + id + ", NAME:" + name + ", SCORE:" + score.ToString();
    }
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        UserData temp = (UserData)obj;
        return (id == temp.id);
    }
    public override int GetHashCode()
    {
        return int.Parse("1" + id);
    }

    public static bool operator ==(UserData a, UserData b)
    {
        return (a.id == b.id);
    }
    public static bool operator !=(UserData a, UserData b)
    {
        return (a.id != b.id);
    }
    public static UserData Empty = new UserData("000000", "unknown", 0);

}