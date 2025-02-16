using FoundryBlazor.Extensions;


namespace FoundryBlazor;

public interface IFoCollection
{
    int Count();
    Type TypeSpec { get; set; }
    string GetName();
    string NextItemName();
    List<string> Keys();
    List<U> ValuesOfType<U>();
    bool AddObject(string key, object value);
    bool RemoveObject(string key);
}

[System.Serializable]
public class FoCollection<T>: IFoCollection where T : FoBase
{
    public string Key { get; set; }
    public Type TypeSpec { get; set; } = typeof(T); 

    private readonly Dictionary<string, T> members = new();

    public string GetName()
    {
        return Key;
    }

    public string NextItemName()
    {
        return  $"{Key}-{members.Count}";
    }


    public bool AddObject(string key, object value)
    {

        if (!TryGetValue(key, out T? found) || found == null)
        {
            this.members.Add(key, (T)value);
            return true;
        }
        return false;
    }

    public bool RemoveObject(string key)
    {

        if (!TryGetValue(key, out T? found))
        {
            this.members.Remove(key);
            return true;
        }
        return false;
    }
    public FoCollection()
    {
        Key = typeof(T).Name;
    }
    public int Count()
    {
        return this.members.Keys.Count;
    }
    public List<string> Keys()
    {
        return this.members.Keys.ToList();
    }
    public List<T> Values()
    {
        return this.members.Values.ToList<T>();
    }
    public List<U> ValuesOfType<U>()
    {
        return this.members.Values.Where(item => item is U || item.GetType().IsSubclassOf(typeof(U)) ).Cast<U>().ToList();
    }
    public T? GetValue(string key)
    {
        if (TryGetValue(key, out T? value))
            return value;
        return null;
    }
    public bool TryGetValue(string key, out T? found)
    {
        found = null;
        if (this.members.ContainsKey(key))
            return this.members.TryGetValue(key, out found);
        return false;
    }
    public List<T> AddRange(List<T> list)
    {
        list.ForEach(item => Add(item));
        return list;
    } 
    public T Add(T value)
    {
        if ( string.IsNullOrEmpty(value.Key)) {
            value.Key = NextItemName();
        }
        return this.Add(value.Key, value);
    }
    public T Add(string key, T value)
    {
        if (!TryGetValue(key, out T? found) || found == null)
        {
            this.members.Add(key, value);
        }
        return value;
    }
    public T Remove(T value)
    {
        if ( string.IsNullOrEmpty(value.Key)) {
            value.Key = NextItemName();
        }
        return this.Remove(value.Key, value);
    }
    public bool Remove(string key)
    {
        if (TryGetValue(key, out T? found) && found != null)
        {
            this.members.Remove(key);
            return true;
        }
        return false;
    }
    public T Remove(string key, T value)
    {
        if (TryGetValue(key, out T? found) && found != null)
        {
            this.members.Remove(key);
        }
        return value;
    }
    public List<T> ForEach(Action<T> applyClause)
    {
        var list = Values();
        list.ForEach(applyClause);
        return list;
    }
    public List<T> ExtractWhere(Func<T,bool> whereClause)
    {
        var extraction = FindWhere(whereClause);
        extraction.ForEach(item => this.members.Remove(item.Key));
        return extraction;
    }
    public List<T> FindWhere(Func<T,bool> whereClause)
    {
        var list = Values();
        var extraction = list.Where(item => whereClause(item)).ToList();
        return extraction;
    }
    public void Clear()
    {
        members.Clear();
    }
    public FoCollection<T> Flush()
    {
        Clear();
        return this;
    }



}
