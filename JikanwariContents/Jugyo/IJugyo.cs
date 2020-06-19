namespace SnctJikanwari.JikanwariContents.Jugyo
{
    public interface IJugyo
    {
        int Time { get; }
        string Subject { get; }
        string ClassName { get; }
        string Teacher { get; }
        string Other { get; }
    }
}