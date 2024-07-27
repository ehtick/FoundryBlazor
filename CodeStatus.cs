
namespace FoundryBlazor;

public class CodeStatus
{
    public string Version()
    {
        var version = GetType().Assembly.GetName().Version ?? new Version(0, 0, 0, 0);
        var ver = $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        return ver;
    }   
}