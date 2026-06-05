using System.Security.Claims;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;
using Microsoft.Extensions.Configuration;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class Sage300ArTermsCodesService : IArTermsCodesService
{
    private readonly IConfiguration _configuration;
    private readonly ICompanyConnectionDetailsProvider _companyDetails;

    public Sage300ArTermsCodesService(IConfiguration configuration, ICompanyConnectionDetailsProvider companyDetails)
    {
        _configuration = configuration;
        _companyDetails = companyDetails;
    }

    public async Task<(ProcessOut Response, ARTermsCodes TermsCodes)> CreateOrUpdateAsync(
        ARTermsCodes termsCodes,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(termsCodes.TermsCode000))
        {
            return (ProcessOut.Fail("9999", "TermsCode000 is required."), termsCodes);
        }

        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var tran = session.BeginTransaction();

        try
        {
            var views = new Sage300ViewSet(session, "AR0016,AR0017", compose: true);
            dynamic header = views.ViewById("AR0016");
            dynamic schedule = views.ViewById("AR0017");

            header.Init();
            header.Fields.FieldByName("CODETERM").Value = termsCodes.TermsCode000;
            var exists = (bool)header.Exists;
            if (exists)
            {
                header.Read();
            }
            else
            {
                header.RecordGenerate(false);
                header.Fields.FieldByName("CODETERM").Value = termsCodes.TermsCode000;
            }

            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "TEXTDESC", termsCodes.Description001);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "ACTIVESW", termsCodes.Status002);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "MULTIPAYM", termsCodes.UsePaymentSchedule005);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "VATCODEM", termsCodes.CalcBaseforDiscountwithTax006);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DISCTYPE", termsCodes.DiscountType007);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DDAYSTRT1", termsCodes.DiscountTableStartingDay1008);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DDAYSTRT2", termsCodes.DiscountTableStartingDay2009);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DDAYSTRT3", termsCodes.DiscountTableStartingDay3010);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DDAYSTRT4", termsCodes.DiscountTableStartingDay4011);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DDAYEND1", termsCodes.DiscountTableEndingDay1012);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DDAYEND2", termsCodes.DiscountTableEndingDay2013);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DDAYEND3", termsCodes.DiscountTableEndingDay3014);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DDAYEND4", termsCodes.DiscountTableEndingDay4015);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DMNTHADD1", termsCodes.DiscountTableMonthsAdded1016);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DMNTHADD2", termsCodes.DiscountTableMonthsAdded2017);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DMNTHADD3", termsCodes.DiscountTableMonthsAdded3018);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DMNTHADD4", termsCodes.DiscountTableMonthsAdded4019);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DDAYUSE1", termsCodes.DiscountTableDayofMonth1020);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DDAYUSE2", termsCodes.DiscountTableDayofMonth2021);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DDAYUSE3", termsCodes.DiscountTableDayofMonth3022);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DDAYUSE4", termsCodes.DiscountTableDayofMonth4023);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DUETYPE", termsCodes.DueDateType024);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DUDAYST1", termsCodes.DueDateTableStartingDay1025);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DUDAYST2", termsCodes.DueDateTableStartingDay2026);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DUDAYST3", termsCodes.DueDateTableStartingDay3027);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DUDAYST4", termsCodes.DueDateTableStartingDay4028);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DUDAYEND1", termsCodes.DueDateTableEndingDay1029);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DUDAYEND2", termsCodes.DueDateTableEndingDay2030);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DUDAYEND3", termsCodes.DueDateTableEndingDay3031);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DUDAYEND4", termsCodes.DueDateTableEndingDay4032);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DUMNTHAD1", termsCodes.DueDateTableMonthsAdded1033);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DUMNTHAD2", termsCodes.DueDateTableMonthsAdded2034);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DUMNTHAD3", termsCodes.DueDateTableMonthsAdded3035);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DUMNTHAD4", termsCodes.DueDateTableMonthsAdded4036);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DUDAYUSE1", termsCodes.DueDateTableDayofMonth1037);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DUDAYUSE2", termsCodes.DueDateTableDayofMonth2038);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DUDAYUSE3", termsCodes.DueDateTableDayofMonth3039);
            Sage300ApPaymentService.SageViewPut.PutIfNotNull(header, "DUDAYUSE4", termsCodes.DueDateTableDayofMonth4040);

            if (termsCodes.TermsSchedules is { Count: > 0 })
            {
                foreach (var dtl in termsCodes.TermsSchedules)
                {
                    schedule.RecordGenerate(false);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(schedule, "CNTPAYM", dtl.PaymentNumber001);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(schedule, "PCTDUE", dtl.PercentDue003);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(schedule, "DISCTYPE", dtl.Reserved004);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(schedule, "PCTDISC", dtl.DiscountPercent005);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(schedule, "NUMDAYS", dtl.DiscountNumberofDays006);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(schedule, "DISCDAY", dtl.DiscountDayofMonth007);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(schedule, "DUETYPE", dtl.Reserved008);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(schedule, "DUEDAYS", dtl.DueNumberofDays009);
                    Sage300ApPaymentService.SageViewPut.PutIfNotNull(schedule, "DUEDAY", dtl.DueDayofMonth010);
                    schedule.Insert();
                }
            }

            if (exists)
            {
                header.Update();
            }
            else
            {
                header.Insert();
            }

            session.CommitTransaction(tran);

            var response = new ProcessOut(
                "0000",
                $"AR Terms Codes Number : {termsCodes.TermsCode000}",
                DocumentNumber: termsCodes.TermsCode000,
                BatchNumber: "",
                ErrorCode: "0000");

            return (response, termsCodes);
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

            return (ProcessOut.Fail("9999", ex.Message), termsCodes);
        }
    }
}
