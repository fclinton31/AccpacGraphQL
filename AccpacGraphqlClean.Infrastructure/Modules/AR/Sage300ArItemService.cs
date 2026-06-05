using System.Security.Claims;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;
using Microsoft.Extensions.Configuration;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class Sage300ArItemService : IArItemService
{
    private readonly IConfiguration _configuration;
    private readonly ICompanyConnectionDetailsProvider _companyDetails;

    public Sage300ArItemService(IConfiguration configuration, ICompanyConnectionDetailsProvider companyDetails)
    {
        _configuration = configuration;
        _companyDetails = companyDetails;
    }

    public async Task<(ProcessOut Response, ARItems Item)> CreateOrUpdateAsync(
        ARItems item,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(item.ItemNumber000))
        {
            return (ProcessOut.Fail("9999", "ItemNumber000 is required."), item);
        }

        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        try
        {
            var views = new Sage300ViewSet(session, "AR0009,AR0010,AR0011", compose: true);
            dynamic vDetail = views.ViewById("AR0009");
            dynamic vHeader = views.ViewById("AR0010");

            vHeader.Init();
            vHeader.Fields.FieldByName("IDITEM").Value = item.ItemNumber000;
            var exists = (bool)vHeader.Exists;
            if (exists)
            {
                vHeader.Read();
            }
            else
            {
                vHeader.RecordGenerate(false);
                vHeader.Fields.FieldByName("IDITEM").Value = item.ItemNumber000;
            }

            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "CODECMDY", item.CommodityCode001);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TEXTDESC", item.Description002);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "SWACTV", item.Status003);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "IDDIST", item.DistributionCode006);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "TEXTCMNT", item.Comment007);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "SWDISCABL", item.Discountable008);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "IDACCTREV", item.RevenueAccount009);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "IDACCTINV", item.InventoryAccount010);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(vHeader, "IDACCTCOGS", item.COGSAccount011);

            if (item.ItemPricings is { Count: > 0 })
            {
                foreach (var dtl in item.ItemPricings)
                {
                    vDetail.RecordGenerate(false);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "CODECURN", dtl.CurrencyCode001);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "UNITMEAS", dtl.UnitofMeasure002);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "RATEWGT", dtl.Reserved003);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "AMTCOST", dtl.ItemCost004);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "AMTPRICE", dtl.ItemPrice005);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(vDetail, "AMTBASETAX", dtl.TaxBase006);
                    vDetail.Insert();
                }
            }

            if (exists)
            {
                vHeader.Update();
            }
            else
            {
                vHeader.Insert();
            }

            session.CommitTransaction(tran);

            var response = new ProcessOut(
                "0000",
                $"AR Items Number : {item.ItemNumber000}",
                DocumentNumber: item.ItemNumber000,
                BatchNumber: "",
                ErrorCode: "0000");

            return (response, item);
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

            return (ProcessOut.Fail("9999", ex.Message), item);
        }
    }
}
