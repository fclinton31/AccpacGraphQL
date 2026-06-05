namespace AccpacGraphqlClean.Domain;

public sealed class ARItems
{
    public string? Compid { get; set; }

    public string? ItemNumber000 { get; set; }
    public string? CommodityCode001 { get; set; }
    public string? Description002 { get; set; }
    public string? Status003 { get; set; }
    public string? DistributionCode006 { get; set; }
    public string? Comment007 { get; set; }
    public string? Discountable008 { get; set; }
    public string? RevenueAccount009 { get; set; }
    public string? InventoryAccount010 { get; set; }
    public string? COGSAccount011 { get; set; }

    public List<ARItemPricing> ItemPricings { get; set; } = new();
}

public sealed class ARItemPricing
{
    public string? CurrencyCode001 { get; set; }
    public string? UnitofMeasure002 { get; set; }
    public string? Reserved003 { get; set; }
    public string? ItemCost004 { get; set; }
    public string? ItemPrice005 { get; set; }
    public string? TaxBase006 { get; set; }
}
