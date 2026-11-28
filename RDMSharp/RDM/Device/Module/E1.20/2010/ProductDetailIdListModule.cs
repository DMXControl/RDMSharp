using System;
using System.Collections.Generic;
using System.Linq;

namespace RDMSharp.RDM.Device.Module;

/// <summary>
/// PRODUCT_DETAIL_ID_LIST (E1.20 10.5.2): technology details of the device (Table A-6), supplementing the product category.
/// GET only, a packed list of 16-bit Product Detail IDs with at least one entry,
/// a device without details declares <see cref="ERDM_ProductDetail.NONE"/>.
/// </summary>
public sealed class ProductDetailIdListModule : AbstractSimpleParameterIDModule
{
    private const string _moduleName = "ProductDetailIdList";
    private const string _moduleDisplayName = "Product Detail ID List";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.PRODUCT_DETAIL_ID_LIST;

    public sealed override string DisplayName => _moduleDisplayName;

    private readonly ERDM_ProductDetail[] _productDetails;

    public IReadOnlyCollection<ERDM_ProductDetail> ProductDetails
    {
        get
        {
            if (ParentDevice is not null && ParentDevice.GetAllParameterValues().TryGetValue(ModuleParameter, out object res))
            {
                if (res is ERDM_ProductDetail[] array)
                    return array;
                if (res is IEnumerable<ERDM_ProductDetail> enumerable)
                    return enumerable.ToArray();
            }
            return _productDetails;
        }
    }

    public override object CurrentValue => ProductDetails;
    public override string ValueText => ProductDetails is { } details ? string.Join(", ", details) : null;

    public ProductDetailIdListModule(params ERDM_ProductDetail[] productDetails) : base(
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET)
    {
        if (productDetails is null || productDetails.Length == 0)
            _productDetails = new[] { ERDM_ProductDetail.NONE };
        else
            _productDetails = productDetails.ToArray();
    }
    public ProductDetailIdListModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET)
    {
        _productDetails = Array.Empty<ERDM_ProductDetail>();
    }

    protected override void OnParentGeneratedDeviceChanged(AbstractGeneratedRDMDevice device)
    {
        device.setParameterValue(ModuleParameter, _productDetails);
    }
    protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
    {
        if (parameter == ModuleParameter)
            OnPropertyChanged(nameof(ProductDetails));
    }
}
