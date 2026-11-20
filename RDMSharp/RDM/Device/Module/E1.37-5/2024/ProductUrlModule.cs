using System;

namespace RDMSharp.RDM.Device.Module;

/// <summary>
/// PRODUCT_URL (E1.37-5 4.3): URL of the product page on the manufacturer's website.
/// GET only. A URL longer than one packet is sent with ACK_OVERFLOW.
/// </summary>
public sealed class ProductUrlModule : AbstractSimpleStringModule
{
    private const string _moduleName = "ProductUrl";
    private const string _moduleDisplayName = "Product URL";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.PRODUCT_URL;

    public override string DisplayName => _moduleDisplayName;

    public string ProductUrl
    {
        get
        {
            return base.Value;
        }
    }

    public ProductUrlModule(string productUrl) : base(
        validate(productUrl),
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET)
    {
    }
    public ProductUrlModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET)
    {
    }

    private static string validate(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("The URL must not be empty.", nameof(url));
        return url;
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (string.Equals(nameof(Value), propertyName))
            OnPropertyChanged(nameof(ProductUrl));
    }
}
