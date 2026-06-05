namespace AccpacGraphqlClean.Domain;

public sealed class ARShipToLocations
{
    public string? Compid { get; set; }

    public string? CustomerNumber000 { get; set; }
    public string? ShipToLocation001 { get; set; }
    public string? Status002 { get; set; }
    public string? Description005 { get; set; }
    public string? AddressLine1006 { get; set; }
    public string? AddressLine2007 { get; set; }
    public string? AddressLine3008 { get; set; }
    public string? AddressLine4009 { get; set; }
    public string? City010 { get; set; }
    public string? StateProv011 { get; set; }
    public string? ZipPostalCode012 { get; set; }
    public string? Country013 { get; set; }
    public string? ContactName014 { get; set; }
    public string? PhoneNumber015 { get; set; }
    public string? FaxNumber016 { get; set; }
    public string? TerritoryCode017 { get; set; }
    public string? TaxGroup018 { get; set; }
    public string? TaxRegistrationNo1019 { get; set; }
    public string? TaxRegistrationNo2020 { get; set; }
    public string? TaxRegistrationNo3021 { get; set; }
    public string? TaxRegistrationNo4022 { get; set; }
    public string? TaxRegistrationNo5023 { get; set; }
    public string? TaxClassCode1024 { get; set; }
    public string? TaxClassCode2025 { get; set; }
    public string? TaxClassCode3026 { get; set; }
    public string? TaxClassCode4027 { get; set; }
    public string? TaxClassCode5028 { get; set; }
    public string? SpecialInstructions029 { get; set; }
    public string? Salesperson1030 { get; set; }
    public string? Salesperson2031 { get; set; }
    public string? Salesperson3032 { get; set; }
    public string? Salesperson4033 { get; set; }
    public string? Salesperson5034 { get; set; }
    public string? SalesSplitPercentage1035 { get; set; }
    public string? SalesSplitPercentage2036 { get; set; }
    public string? SalesSplitPercentage3037 { get; set; }
    public string? SalesSplitPercentage4038 { get; set; }
    public string? SalesSplitPercentage5039 { get; set; }
    public string? CustomerPriceList040 { get; set; }
    public string? FreeOnBoard041 { get; set; }
    public string? ShipViaCode042 { get; set; }
    public string? ShipViaDescription043 { get; set; }
    public string? PrimaryShipToIndicator044 { get; set; }
    public string? Email045 { get; set; }
    public string? ContactsPhone046 { get; set; }
    public string? ContactsFax047 { get; set; }
    public string? ContactsEmail048 { get; set; }
    public string? InventoryLocation051 { get; set; }
    public string? SuppressIntegration052 { get; set; }
    public string? ARVersion053 { get; set; }
    public string? Database054 { get; set; }
    public string? Mode055 { get; set; }
}

public sealed class ARCustomerShipToLocations
{
    public string? Compid { get; set; }
    public string? CustomerNumber { get; set; }
    public string? ShipToID { get; set; }
    public IReadOnlyList<ARShipToLocations> ShipToLocations { get; set; } = Array.Empty<ARShipToLocations>();
}

public sealed class SyncARShipToLocations
{
    public string CallMethod { get; set; } = "SYNC";
    public string Systemid { get; set; } = "";
    public int RecordLimit { get; set; } = 1000;
    public string Timestamp { get; set; } = "";
    public IReadOnlyList<ARShipToLocations> ShipToLocations { get; set; } = Array.Empty<ARShipToLocations>();
}
