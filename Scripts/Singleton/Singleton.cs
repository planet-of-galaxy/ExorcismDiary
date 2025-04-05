public abstract class Singleton<T> where T : class, new()
{
    private static T instance;

    protected readonly static object lockObj = new object();
    public static T Instance {
        get
        {
            lock (lockObj) {
                if (instance == null)
                    instance = new T();
            }
            return instance;
        }
    }
}
