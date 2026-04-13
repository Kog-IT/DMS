using DMS.Debugging;

namespace DMS;

public class DMSConsts
{
    public const string LocalizationSourceName = "DMS";

    public const string ConnectionStringName = "Default";

    public const bool MultiTenancyEnabled = true;


    /// <summary>
    /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
    /// </summary>
    public static readonly string DefaultPassPhrase =
        DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "05ee5b6c4af34151b29124597792859e";
}
