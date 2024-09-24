namespace RDMSharp.Metadata.JSON.OneOfTypes
{
    public interface IIntegerType
    {
        string Name { get; }
        string DisplayName { get; }
        bool? RestrictToLabeled { get; }
        string Notes { get; }
        EIntegerType Type { get; }
        ERDM_SensorUnit? Units { get; }
        int? PrefixPower { get; } 
        int? PrefixBase { get; }
        double PrefixMultiplyer { get; }
    }
}