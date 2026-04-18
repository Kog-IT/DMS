namespace DMS.Visits;

public static class VisitSettingNames
{
    public const string GeofencingEnabled = "Visit.GeofencingEnabled";
    public const string GeofencingRadiusMeters = "Visit.GeofencingRadiusMeters";
    public const string GpsEnforcement = "Visit.GpsEnforcement";
    public const string DefaultVisitDurationMinutes = "Visit.DefaultVisitDurationMinutes";
    public const string AverageTravelSpeedKmh = "Visit.AverageTravelSpeedKmh";
}

/// <summary>Valid values for the Visit.GpsEnforcement setting.</summary>
public static class GpsEnforcementMode
{
    public const string None = "None";
    public const string Warn = "Warn";
    public const string Block = "Block";
}
