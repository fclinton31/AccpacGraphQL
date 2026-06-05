using System.Security.Claims;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;
using Microsoft.Extensions.Configuration;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class Sage300ArShipToLocationService : IArShipToLocationService
{
    private readonly IConfiguration _configuration;
    private readonly ICompanyConnectionDetailsProvider _companyDetails;

    public Sage300ArShipToLocationService(IConfiguration configuration, ICompanyConnectionDetailsProvider companyDetails)
    {
        _configuration = configuration;
        _companyDetails = companyDetails;
    }

    public async Task<(ProcessOut Response, ARShipToLocations ShipToLocation)> CreateOrUpdateAsync(
        ARShipToLocations shipToLocation,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(shipToLocation.CustomerNumber000))
        {
            return (ProcessOut.Fail("9999", "CustomerNumber000 is required."), shipToLocation);
        }

        if (string.IsNullOrWhiteSpace(shipToLocation.ShipToLocation001))
        {
            return (ProcessOut.Fail("9999", "ShipToLocation001 is required."), shipToLocation);
        }

        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        try
        {
            var views = new Sage300ViewSet(session, "AR0023,AR0412,AR0024", compose: true);
            dynamic v = views.ViewById("AR0023");
            v.Init();
            v.Fields.FieldByName("IDCUST").Value = shipToLocation.CustomerNumber000;
            v.Fields.FieldByName("IDCUSTSHPT").Value = shipToLocation.ShipToLocation001;

            var exists = (bool)v.Exists;
            if (exists)
            {
                v.Read();
            }
            else
            {
                v.RecordGenerate(false);
                v.Fields.FieldByName("IDCUST").Value = shipToLocation.CustomerNumber000;
                v.Fields.FieldByName("IDCUSTSHPT").Value = shipToLocation.ShipToLocation001;
            }

            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "SWACTV", shipToLocation.Status002);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "NAMELOCN", shipToLocation.Description005);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "TEXTSTRE1", shipToLocation.AddressLine1006);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "TEXTSTRE2", shipToLocation.AddressLine2007);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "TEXTSTRE3", shipToLocation.AddressLine3008);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "TEXTSTRE4", shipToLocation.AddressLine4009);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "NAMECITY", shipToLocation.City010);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "CODESTTE", shipToLocation.StateProv011);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "CODEPSTL", shipToLocation.ZipPostalCode012);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "CODECTRY", shipToLocation.Country013);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "NAMECTAC", shipToLocation.ContactName014);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "TEXTPHON1", shipToLocation.PhoneNumber015);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "TEXTPHON2", shipToLocation.FaxNumber016);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "CODETERR", shipToLocation.TerritoryCode017);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "CODETAXGRP", shipToLocation.TaxGroup018);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "IDTAXREGI1", shipToLocation.TaxRegistrationNo1019);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "IDTAXREGI2", shipToLocation.TaxRegistrationNo2020);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "IDTAXREGI3", shipToLocation.TaxRegistrationNo3021);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "IDTAXREGI4", shipToLocation.TaxRegistrationNo4022);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "IDTAXREGI5", shipToLocation.TaxRegistrationNo5023);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "TAXSTTS1", shipToLocation.TaxClassCode1024);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "TAXSTTS2", shipToLocation.TaxClassCode2025);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "TAXSTTS3", shipToLocation.TaxClassCode3026);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "TAXSTTS4", shipToLocation.TaxClassCode4027);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "TAXSTTS5", shipToLocation.TaxClassCode5028);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "SPCLINST", shipToLocation.SpecialInstructions029);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "CODESLSP1", shipToLocation.Salesperson1030);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "CODESLSP2", shipToLocation.Salesperson2031);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "CODESLSP3", shipToLocation.Salesperson3032);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "CODESLSP4", shipToLocation.Salesperson4033);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "CODESLSP5", shipToLocation.Salesperson5034);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "PCTSASPLT1", shipToLocation.SalesSplitPercentage1035);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "PCTSASPLT2", shipToLocation.SalesSplitPercentage2036);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "PCTSASPLT3", shipToLocation.SalesSplitPercentage3037);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "PCTSASPLT4", shipToLocation.SalesSplitPercentage4038);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "PCTSASPLT5", shipToLocation.SalesSplitPercentage5039);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "PRICLIST", shipToLocation.CustomerPriceList040);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "FOB", shipToLocation.FreeOnBoard041);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "SHPVIACODE", shipToLocation.ShipViaCode042);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "SHPVIADESC", shipToLocation.ShipViaDescription043);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "SWPRIMSHPT", shipToLocation.PrimaryShipToIndicator044);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "EMAIL", shipToLocation.Email045);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "CTACPHONE", shipToLocation.ContactsPhone046);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "CTACFAX", shipToLocation.ContactsFax047);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "CTACEMAIL", shipToLocation.ContactsEmail048);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "LOCATION", shipToLocation.InventoryLocation051);

            if (!string.IsNullOrWhiteSpace(shipToLocation.SuppressIntegration052))
            {
                var truthy = Sage300ApPaymentService.SageViewPut.IsTruthy(shipToLocation.SuppressIntegration052);
                v.Fields.FieldByName("EWSUPPRESS").Value = truthy;
            }

            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "EWARVER", shipToLocation.ARVersion053);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "EWORGID", shipToLocation.Database054);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(v, "EWMODE", shipToLocation.Mode055);

            if (exists)
            {
                v.Update();
            }
            else
            {
                v.Insert();
            }

            session.CommitTransaction(tran);

            var response = new ProcessOut(
                "0000",
                $"Sage 300 ARShipTo Number : {shipToLocation.ShipToLocation001}",
                DocumentNumber: shipToLocation.ShipToLocation001,
                BatchNumber: "",
                ErrorCode: "0000");

            return (response, shipToLocation);
        }
        catch (Exception ex)
        {
            try
            {
                session.RollbackTransaction(tran);
            }
            catch
            {
            }

            return (ProcessOut.Fail("9999", ex.Message), shipToLocation);
        }
    }

    public async Task<(ProcessOut Response, ARShipToLocations ShipToLocation)> ReadAsync(
        string customerNumber,
        string shipToLocation,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        var result = new ARShipToLocations { CustomerNumber000 = customerNumber, ShipToLocation001 = shipToLocation };

        try
        {
            var views = new Sage300ViewSet(session, "AR0023,AR0412,AR0024", compose: true);
            dynamic v = views.ViewById("AR0023");
            v.Init();
            v.Fields.FieldByName("IDCUST").Value = customerNumber;
            v.Fields.FieldByName("IDCUSTSHPT").Value = shipToLocation;

            while (v.Fetch())
            {
                result.Status002 = Convert.ToString(v.Fields.FieldByName("SWACTV").Value);
                result.Description005 = Convert.ToString(v.Fields.FieldByName("NAMELOCN").Value);
                result.AddressLine1006 = Convert.ToString(v.Fields.FieldByName("TEXTSTRE1").Value);
                result.AddressLine2007 = Convert.ToString(v.Fields.FieldByName("TEXTSTRE2").Value);
                result.AddressLine3008 = Convert.ToString(v.Fields.FieldByName("TEXTSTRE3").Value);
                result.AddressLine4009 = Convert.ToString(v.Fields.FieldByName("TEXTSTRE4").Value);
                result.City010 = Convert.ToString(v.Fields.FieldByName("NAMECITY").Value);
                result.StateProv011 = Convert.ToString(v.Fields.FieldByName("CODESTTE").Value);
                result.ZipPostalCode012 = Convert.ToString(v.Fields.FieldByName("CODEPSTL").Value);
                result.Country013 = Convert.ToString(v.Fields.FieldByName("CODECTRY").Value);
                result.ContactName014 = Convert.ToString(v.Fields.FieldByName("NAMECTAC").Value);
                result.PhoneNumber015 = Convert.ToString(v.Fields.FieldByName("TEXTPHON1").Value);
                result.FaxNumber016 = Convert.ToString(v.Fields.FieldByName("TEXTPHON2").Value);
                result.TerritoryCode017 = Convert.ToString(v.Fields.FieldByName("CODETERR").Value);
                result.TaxGroup018 = Convert.ToString(v.Fields.FieldByName("CODETAXGRP").Value);
                result.Email045 = Convert.ToString(v.Fields.FieldByName("EMAIL").Value);
            }

            session.CommitTransaction(tran);

            var response = new ProcessOut(
                "0000",
                $"ARShipTo Location : {shipToLocation}",
                DocumentNumber: shipToLocation,
                BatchNumber: "",
                ErrorCode: "0000");

            return (response, result);
        }
        catch (Exception ex)
        {
            try
            {
                session.RollbackTransaction(tran);
            }
            catch
            {
            }

            return (ProcessOut.Fail("9999", ex.Message), result);
        }
    }

    public async Task<(ProcessOut Response, ARCustomerShipToLocations ShipToLocations)> ReadCustomerShipToLocationsAsync(
        string customerNumber,
        string? shipToId,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        var result = new ARCustomerShipToLocations { CustomerNumber = customerNumber, ShipToID = shipToId };

        try
        {
            var views = new Sage300ViewSet(session, "AR0023,AR0412,AR0024", compose: true);
            dynamic customer = views.ViewById("AR0024");
            customer.Fields.FieldByName("IDCUST").Value = customerNumber;
            if (!(bool)customer.Exists)
            {
                session.CommitTransaction(tran);
                return (ProcessOut.Fail("9999", "Customer does not exist."), result);
            }

            dynamic v = views.ViewById("AR0023");
            var filter = $"IDCUST = \"{customerNumber}\"";
            if (!string.IsNullOrWhiteSpace(shipToId))
            {
                filter += $" AND IDCUSTSHPT = \"{shipToId}\"";
            }

            v.Browse(filter, true);

            var list = new List<ARShipToLocations>();
            while (v.Fetch())
            {
                list.Add(new ARShipToLocations
                {
                    CustomerNumber000 = Convert.ToString(v.Fields.FieldByName("IDCUST").Value),
                    ShipToLocation001 = Convert.ToString(v.Fields.FieldByName("IDCUSTSHPT").Value),
                    Status002 = Convert.ToString(v.Fields.FieldByName("SWACTV").Value),
                    Description005 = Convert.ToString(v.Fields.FieldByName("NAMELOCN").Value),
                    AddressLine1006 = Convert.ToString(v.Fields.FieldByName("TEXTSTRE1").Value),
                    AddressLine2007 = Convert.ToString(v.Fields.FieldByName("TEXTSTRE2").Value),
                    AddressLine3008 = Convert.ToString(v.Fields.FieldByName("TEXTSTRE3").Value),
                    AddressLine4009 = Convert.ToString(v.Fields.FieldByName("TEXTSTRE4").Value),
                    City010 = Convert.ToString(v.Fields.FieldByName("NAMECITY").Value),
                    StateProv011 = Convert.ToString(v.Fields.FieldByName("CODESTTE").Value),
                    ZipPostalCode012 = Convert.ToString(v.Fields.FieldByName("CODEPSTL").Value),
                    Country013 = Convert.ToString(v.Fields.FieldByName("CODECTRY").Value),
                    ContactName014 = Convert.ToString(v.Fields.FieldByName("NAMECTAC").Value),
                    PhoneNumber015 = Convert.ToString(v.Fields.FieldByName("TEXTPHON1").Value),
                    FaxNumber016 = Convert.ToString(v.Fields.FieldByName("TEXTPHON2").Value),
                    TerritoryCode017 = Convert.ToString(v.Fields.FieldByName("CODETERR").Value),
                    TaxGroup018 = Convert.ToString(v.Fields.FieldByName("CODETAXGRP").Value),
                    Email045 = Convert.ToString(v.Fields.FieldByName("EMAIL").Value)
                });
            }

            result.ShipToLocations = list;
            session.CommitTransaction(tran);

            var response = new ProcessOut(
                "0000",
                $"ARShipTo Number : {list.Count}",
                DocumentNumber: list.Count.ToString(),
                BatchNumber: "",
                ErrorCode: "0000");

            return (response, result);
        }
        catch (Exception ex)
        {
            try
            {
                session.RollbackTransaction(tran);
            }
            catch
            {
            }

            return (ProcessOut.Fail("9999", ex.Message), result);
        }
    }

    public async Task<(ProcessOut Response, SyncARShipToLocations Sync)> SyncAsync(
        SyncARShipToLocations request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        var recordLimit = request.RecordLimit > 0 ? request.RecordLimit : 1000;

        try
        {
            var views = new Sage300ViewSet(session, "AR0023,AR0412,AR0024,YH0305", compose: true);
            dynamic v = views.ViewById("AR0023");
            dynamic yhMaster = views.ViewById("YH0305");

            ConfirmPreviousSync(yhMaster, request.Timestamp);
            request.Timestamp = timestamp;

            yhMaster.Browse("MODULE = \"AR\" AND TXNTYPE = \"ST\" AND YHSTATUS = 0", true);

            var items = new List<ARShipToLocations>();
            while (yhMaster.Fetch() && items.Count < recordLimit)
            {
                var masterKey = Convert.ToString(yhMaster.Fields.FieldByName("MASTERKEY").Value);
                if (string.IsNullOrWhiteSpace(masterKey))
                {
                    continue;
                }

                var parts = masterKey.Split('~');
                if (parts.Length < 2)
                {
                    continue;
                }

                v.Fields.FieldByName("IDCUST").PutWithoutVerification(parts[0]);
                v.Fields.FieldByName("IDCUSTSHPT").PutWithoutVerification(parts[1]);
                v.Read();

                items.Add(new ARShipToLocations
                {
                    CustomerNumber000 = Convert.ToString(v.Fields.FieldByName("IDCUST").Value),
                    ShipToLocation001 = Convert.ToString(v.Fields.FieldByName("IDCUSTSHPT").Value),
                    Status002 = Convert.ToString(v.Fields.FieldByName("SWACTV").Value),
                    Description005 = Convert.ToString(v.Fields.FieldByName("NAMELOCN").Value),
                    AddressLine1006 = Convert.ToString(v.Fields.FieldByName("TEXTSTRE1").Value),
                    AddressLine2007 = Convert.ToString(v.Fields.FieldByName("TEXTSTRE2").Value),
                    AddressLine3008 = Convert.ToString(v.Fields.FieldByName("TEXTSTRE3").Value),
                    AddressLine4009 = Convert.ToString(v.Fields.FieldByName("TEXTSTRE4").Value),
                    City010 = Convert.ToString(v.Fields.FieldByName("NAMECITY").Value),
                    StateProv011 = Convert.ToString(v.Fields.FieldByName("CODESTTE").Value),
                    ZipPostalCode012 = Convert.ToString(v.Fields.FieldByName("CODEPSTL").Value),
                    Country013 = Convert.ToString(v.Fields.FieldByName("CODECTRY").Value),
                    ContactName014 = Convert.ToString(v.Fields.FieldByName("NAMECTAC").Value),
                    PhoneNumber015 = Convert.ToString(v.Fields.FieldByName("TEXTPHON1").Value),
                    FaxNumber016 = Convert.ToString(v.Fields.FieldByName("TEXTPHON2").Value),
                    TerritoryCode017 = Convert.ToString(v.Fields.FieldByName("CODETERR").Value),
                    TaxGroup018 = Convert.ToString(v.Fields.FieldByName("CODETAXGRP").Value),
                    Email045 = Convert.ToString(v.Fields.FieldByName("EMAIL").Value),
                    SuppressIntegration052 = Convert.ToString(v.Fields.FieldByName("EWSUPPRESS").Value),
                    ARVersion053 = Convert.ToString(v.Fields.FieldByName("EWARVER").Value),
                    Database054 = Convert.ToString(v.Fields.FieldByName("EWORGID").Value),
                    Mode055 = Convert.ToString(v.Fields.FieldByName("EWMODE").Value)
                });

                yhMaster.Fields.FieldByName("TIMESTAMP").Value = request.Timestamp;
                yhMaster.Update();
            }

            request.ShipToLocations = items;
            session.CommitTransaction(tran);

            var response = new ProcessOut(
                "0000",
                "Sync AR ShipTo Locations completed.",
                DocumentNumber: request.Timestamp,
                BatchNumber: "",
                ErrorCode: "0000");

            return (response, request);
        }
        catch (Exception ex)
        {
            try
            {
                session.RollbackTransaction(tran);
            }
            catch
            {
            }

            return (ProcessOut.Fail("9999", ex.Message), request);
        }
    }

    private static void ConfirmPreviousSync(dynamic yhMaster, string? oldTimestamp)
    {
        if (string.IsNullOrWhiteSpace(oldTimestamp))
        {
            return;
        }

        yhMaster.Browse($"MODULE = \"AR\" AND TXNTYPE = \"ST\" AND TIMESTAMP = \"{oldTimestamp}\"", true);
        while (yhMaster.Fetch())
        {
            yhMaster.Fields.FieldByName("YHSTATUS").Value = 1;
            yhMaster.Update();
        }
    }
}
