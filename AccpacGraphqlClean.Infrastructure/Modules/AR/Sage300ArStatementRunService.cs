using System.Globalization;
using System.Security.Claims;
using AccpacGraphqlClean.Application;
using AccpacGraphqlClean.Domain;
using Microsoft.Extensions.Configuration;

namespace AccpacGraphqlClean.Infrastructure;

public sealed class Sage300ArStatementRunService : IArStatementRunService
{
    private readonly IConfiguration _configuration;
    private readonly ICompanyConnectionDetailsProvider _companyDetails;

    public Sage300ArStatementRunService(IConfiguration configuration, ICompanyConnectionDetailsProvider companyDetails)
    {
        _configuration = configuration;
        _companyDetails = companyDetails;
    }

    public async Task<(ProcessOut Response, ARStatementRun StatementRun)> ReadAsync(
        ARStatementRun request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        if (request.StatementRunDate001 is not { } stmtDate || stmtDate == default)
        {
            return (ProcessOut.Fail("9999", "StatementRunDate001 is required."), request);
        }

        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var views = new Sage300ViewSet(session, "AR0110,AR0111,AR0112,AR0113,AR0114", compose: true);

        dynamic vRun = views.ViewById("AR0110");
        dynamic vCustomers = views.ViewById("AR0111");
        dynamic vReceipts = views.ViewById("AR0112");

        vRun.Init();
        vRun.Order = 1;
        vRun.Fields.FieldByName("STMTDATE").Value = stmtDate;
        if (!(bool)vRun.Exists)
        {
            return (ProcessOut.Fail("9999", "Statement run not found."), request);
        }

        vRun.Read();

        request.StatementSEQ = GetString(vRun, "STMTSEQ");
        request.StatementRunCompletedFlag002 = GetString(vRun, "SWFINISH");
        request.CutoffDate003 = GetDate(vRun, "DATECUTOFF");
        request.DueDateInvoiceDate004 = GetString(vRun, "SWINVCDATE");
        request.IncludeDebitBalances005 = GetString(vRun, "SWDEBIT");
        request.IncludeCreditBalances006 = GetString(vRun, "SWCREDIT");
        request.IncludeZeroBalances007 = GetString(vRun, "SWZEROBAL");
        request.IncludeFullyPaidTransactions008 = GetString(vRun, "SWINCLPAID");
        request.IncludeDetails009 = GetString(vRun, "SWDETAIL");
        request.RunType010 = GetString(vRun, "SWTYPERUN");
        request.DetailSort011 = GetString(vRun, "SWDTLSRTBY");
        request.DunningMessageCode012 = GetString(vRun, "IDDUNNING");
        request.Range1From013 = GetString(vRun, "IDFROM1");
        request.Range1To014 = GetString(vRun, "IDTO1");
        request.Range1Type015 = GetString(vRun, "INDEX1");
        request.Range2From016 = GetString(vRun, "IDFROM2");
        request.Range2To017 = GetString(vRun, "IDTO2");
        request.Range2Type018 = GetString(vRun, "INDEX2");
        request.Range3From019 = GetString(vRun, "IDFROM3");
        request.Range3To020 = GetString(vRun, "IDTO3");
        request.Range3Type021 = GetString(vRun, "INDEX3");
        request.Range4From022 = GetString(vRun, "IDFROM4");
        request.Range4To023 = GetString(vRun, "IDTO4");
        request.Range4Type024 = GetString(vRun, "INDEX4");
        request.ReportName025 = GetString(vRun, "RPTNAME");
        request.DeliveryMethod026 = GetString(vRun, "DELMETHOD");
        request.Sortfield1027 = GetString(vRun, "SORTINDEX1");
        request.Sortfield2028 = GetString(vRun, "SORTINDEX2");
        request.Sortfield3029 = GetString(vRun, "SORTINDEX3");
        request.Sortfield4030 = GetString(vRun, "SORTINDEX4");
        request.Current031 = GetString(vRun, "AGEPERIOD1");
        request.FirstPeriod032 = GetString(vRun, "AGEPERIOD2");
        request.SecondPeriod033 = GetString(vRun, "AGEPERIOD3");
        request.ThirdPeriod034 = GetString(vRun, "AGEPERIOD4");
        request.SelectCustomersBasedOnOverdu035 = GetString(vRun, "SWOVERDUE");
        request.OpenItemStatementType037 = GetString(vRun, "STMTTYPE");

        request.StatementCustomers = new List<ARStatementCustomer>();
        vCustomers.Init();
        while (vCustomers.Fetch())
        {
            request.StatementCustomers.Add(MapCustomer(vCustomers));
        }

        request.StatementReceipts = new List<ARStatementReceipt>();
        vReceipts.Init();
        while (vReceipts.Fetch())
        {
            request.StatementReceipts.Add(new ARStatementReceipt
            {
                CustomerNumber001 = GetString(vReceipts, "IDCUST"),
                DocumentNumber002 = GetString(vReceipts, "IDINVC"),
                StatementRunDate004 = GetDate(vReceipts, "STMTDATE"),
                CheckReceiptNo005 = GetString(vReceipts, "IDRMIT")
            });
        }

        request.StatementDocuments = new List<ARStatementDocument>();

        var seq = request.StatementSEQ ?? "";
        return (ProcessOut.Ok($"Sage 300 Statement Sequence Number : {seq}", seq), request);
    }

    public async Task<(ProcessOut Response, SyncARStatement Sync)> ReadAllAsync(
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var details = await _companyDetails.GetAsync(user, cancellationToken);
        using var session = Sage300Session.Open(_configuration, details);
        var views = new Sage300ViewSet(session, "AR0110,AR0111,AR0112,AR0113,AR0114", compose: true);

        dynamic vRun = views.ViewById("AR0110");
        dynamic vCustomers = views.ViewById("AR0111");
        dynamic vReceipts = views.ViewById("AR0113");
        dynamic vDocuments = views.ViewById("AR0112");

        dynamic csqry = session.OpenView("CS0120");
        try
        {
            csqry.Cancel();
        }
        catch
        {
        }

        csqry.Browse("SELECT * FROM ARSTRUN", true);
        try
        {
            csqry.InternalSet(256);
        }
        catch
        {
        }

        var keys = new List<string>();
        while (csqry.Fetch())
        {
            var masterKey = GetString(csqry, "STMTSEQ");
            if (!string.IsNullOrWhiteSpace(masterKey))
            {
                keys.Add(masterKey);
            }
        }

        var sync = new SyncARStatement { ARStatements = new List<ARStatementRun>() };

        var keyFields = GetKeyFields(vRun);

        foreach (var masterKey in keys)
        {
            vRun.Init();
            var parts = masterKey.Split('~');
            for (var i = 0; i < keyFields.Count; i++)
            {
                var part = parts.Length > i ? parts[i] : parts[0];
                vRun.Fields.FieldByName(keyFields[i]).PutWithoutVerification(part);
            }

            vRun.Read();

            var statement = new ARStatementRun
            {
                StatementSEQ = GetString(vRun, "STMTSEQ"),
                StatementRunDate001 = GetDate(vRun, "STMTDATE"),
                StatementRunCompletedFlag002 = GetString(vRun, "SWFINISH"),
                CutoffDate003 = GetDate(vRun, "DATECUTOFF"),
                DueDateInvoiceDate004 = GetString(vRun, "SWINVCDATE"),
                IncludeDebitBalances005 = GetString(vRun, "SWDEBIT"),
                IncludeCreditBalances006 = GetString(vRun, "SWCREDIT"),
                IncludeZeroBalances007 = GetString(vRun, "SWZEROBAL"),
                IncludeFullyPaidTransactions008 = GetString(vRun, "SWINCLPAID"),
                IncludeDetails009 = GetString(vRun, "SWDETAIL"),
                RunType010 = GetString(vRun, "SWTYPERUN"),
                DetailSort011 = GetString(vRun, "SWDTLSRTBY"),
                DunningMessageCode012 = GetString(vRun, "IDDUNNING"),
                Range1From013 = GetString(vRun, "IDFROM1"),
                Range1To014 = GetString(vRun, "IDTO1"),
                Range1Type015 = GetString(vRun, "INDEX1"),
                Range2From016 = GetString(vRun, "IDFROM2"),
                Range2To017 = GetString(vRun, "IDTO2"),
                Range2Type018 = GetString(vRun, "INDEX2"),
                Range3From019 = GetString(vRun, "IDFROM3"),
                Range3To020 = GetString(vRun, "IDTO3"),
                Range3Type021 = GetString(vRun, "INDEX3"),
                Range4From022 = GetString(vRun, "IDFROM4"),
                Range4To023 = GetString(vRun, "IDTO4"),
                Range4Type024 = GetString(vRun, "INDEX4"),
                ReportName025 = GetString(vRun, "RPTNAME"),
                DeliveryMethod026 = GetString(vRun, "DELMETHOD"),
                Sortfield1027 = GetString(vRun, "SORTINDEX1"),
                Sortfield2028 = GetString(vRun, "SORTINDEX2"),
                Sortfield3029 = GetString(vRun, "SORTINDEX3"),
                Sortfield4030 = GetString(vRun, "SORTINDEX4"),
                Current031 = GetString(vRun, "AGEPERIOD1"),
                FirstPeriod032 = GetString(vRun, "AGEPERIOD2"),
                SecondPeriod033 = GetString(vRun, "AGEPERIOD3"),
                ThirdPeriod034 = GetString(vRun, "AGEPERIOD4"),
                SelectCustomersBasedOnOverdu035 = GetString(vRun, "SWOVERDUE"),
                OpenItemStatementType037 = GetString(vRun, "STMTTYPE"),
                StatementCustomers = new List<ARStatementCustomer>(),
                StatementReceipts = new List<ARStatementReceipt>(),
                StatementDocuments = new List<ARStatementDocument>()
            };

            var stmtSeq = statement.StatementSEQ;
            if (string.IsNullOrWhiteSpace(stmtSeq))
            {
                continue;
            }

            vCustomers.Init();
            vCustomers.Fields.FieldByName("STMTSEQ").PutWithoutVerification(stmtSeq);
            vCustomers.Browse($"STMTSEQ = {stmtSeq}", true);
            while (vCustomers.Fetch())
            {
                statement.StatementCustomers.Add(MapCustomer(vCustomers));
            }

            vReceipts.Init();
            vReceipts.Fields.FieldByName("STMTSEQ").PutWithoutVerification(stmtSeq);
            vReceipts.Browse($"STMTSEQ = {stmtSeq}", true);
            while (vReceipts.Fetch())
            {
                statement.StatementReceipts.Add(new ARStatementReceipt
                {
                    StatementSEQ = GetString(vReceipts, "STMTSEQ"),
                    CustomerNumber001 = GetString(vReceipts, "IDCUST"),
                    DocumentNumber002 = GetString(vReceipts, "IDINVC"),
                    StatementRunDate004 = GetDate(vReceipts, "STMTDATE"),
                    CheckReceiptNo005 = GetString(vReceipts, "IDRMIT"),
                    PostingDate006 = GetDate(vReceipts, "DATEBUS"),
                    DocumentType007 = GetString(vReceipts, "TRANSTYPE"),
                    CustReceiptAmount008 = GetString(vReceipts, "AMTPAYMTC"),
                    TransactionType009 = GetString(vReceipts, "TRXTYPE"),
                    ReferenceDocumentNumber010 = GetString(vReceipts, "IDMEMOXREF")
                });
            }

            vDocuments.Init();
            vDocuments.Fields.FieldByName("STMTSEQ").PutWithoutVerification(stmtSeq);
            vDocuments.Browse($"STMTSEQ = {stmtSeq}", true);
            while (vDocuments.Fetch())
            {
                statement.StatementDocuments.Add(new ARStatementDocument
                {
                    StatementSEQ = GetString(vDocuments, "STMTSEQ"),
                    CustomerNumber001 = GetString(vDocuments, "IDCUST"),
                    DocumentNumber002 = GetString(vDocuments, "IDINVC"),
                    StatementRunDate003 = GetDate(vDocuments, "STMTDATE"),
                    Code004 = GetString(vDocuments, "CODE"),
                    RecordType005 = GetString(vDocuments, "RECTYPE"),
                    CheckReceiptNumber006 = GetString(vDocuments, "IDRMIT"),
                    PONumber007 = GetString(vDocuments, "IDCUSTPO"),
                    OrderNumber008 = GetString(vDocuments, "IDORDERNBR"),
                    DocumentDate009 = GetDate(vDocuments, "DATEINVC"),
                    DueDate010 = GetDate(vDocuments, "DATEDUE"),
                    DocumentDescription011 = GetString(vDocuments, "DESCINVC"),
                    DocumentType012 = GetString(vDocuments, "TRXTYPETXT"),
                    TransactionType013 = GetString(vDocuments, "TRXTYPEID"),
                    AmountDue014 = GetString(vDocuments, "AMTDUE"),
                    DiscountAmount015 = GetString(vDocuments, "AMTDISC"),
                    ShipToLocation016 = GetString(vDocuments, "IDCUSTSHPT"),
                    Terms017 = GetString(vDocuments, "CODETERM"),
                    LastStatementDate018 = GetDate(vDocuments, "DATELASTST"),
                    InvoiceAmount020 = GetString(vDocuments, "AMTINVC")
                });
            }

            sync.ARStatements.Add(statement);
        }

        return (ProcessOut.Ok($"AR Statement Runs : {sync.ARStatements.Count}"), sync);
    }

    private static ARStatementCustomer MapCustomer(dynamic vCustomers)
        => new()
        {
            StatementSEQ = GetString(vCustomers, "STMTSEQ"),
            CustomerNumber001 = GetString(vCustomers, "IDCUST"),
            StatementPrintedFlag002 = GetString(vCustomers, "SWPRINTED"),
            NATStatementSwitch003 = GetString(vCustomers, "SWNATSTMT"),
            NationalAccountNumber004 = GetString(vCustomers, "IDNATACCT"),
            StatementRunDate005 = GetDate(vCustomers, "STMTDATE"),
            ShortName006 = GetString(vCustomers, "TEXTSNAM"),
            GroupCode007 = GetString(vCustomers, "IDGRP"),
            Status008 = GetString(vCustomers, "SWACTV"),
            InactiveDate009 = GetDate(vCustomers, "DATEINAC"),
            DateLastMaintained010 = GetDate(vCustomers, "DATELASTMN"),
            OnHold011 = GetString(vCustomers, "SWHOLD"),
            StartDate012 = GetDate(vCustomers, "DATESTART"),
            Reserved013 = GetString(vCustomers, "IDPPNT"),
            CreditBureauNumber014 = GetString(vCustomers, "CODEDAB"),
            CreditBureauRating015 = GetString(vCustomers, "CODEDABRTG"),
            CreditBureauDate016 = GetDate(vCustomers, "DATEDAB"),
            CustomerName017 = GetString(vCustomers, "NAMECUST"),
            AddressLine1018 = GetString(vCustomers, "TEXTSTRE1"),
            AddressLine2019 = GetString(vCustomers, "TEXTSTRE2"),
            AddressLine3020 = GetString(vCustomers, "TEXTSTRE3"),
            AddressLine4021 = GetString(vCustomers, "TEXTSTRE4"),
            City022 = GetString(vCustomers, "NAMECITY"),
            StateProv023 = GetString(vCustomers, "CODESTTE"),
            ZipPostalCode024 = GetString(vCustomers, "CODEPSTL"),
            Country025 = GetString(vCustomers, "CODECTRY"),
            ContactName026 = GetString(vCustomers, "NAMECTAC"),
            PhoneNumber027 = GetString(vCustomers, "TEXTPHON1"),
            FaxNumber028 = GetString(vCustomers, "TEXTPHON2"),
            TerritoryCode029 = GetString(vCustomers, "CODETERR"),
            AccountSet030 = GetString(vCustomers, "IDACCTSET"),
            AutocashProfile031 = GetString(vCustomers, "IDAUTOCASH"),
            BillingCycle032 = GetString(vCustomers, "IDBILLCYCL"),
            InterestProfile033 = GetString(vCustomers, "IDSVCCHRG"),
            Reserved034 = GetString(vCustomers, "IDDLNQ"),
            CurrencyCode035 = GetString(vCustomers, "CODECURN"),
            PrintStatements036 = GetString(vCustomers, "SWPRTSTMT"),
            Reserved037 = GetString(vCustomers, "SWPRTDLNQ"),
            AccountType038 = GetString(vCustomers, "SWBALFWD"),
            Terms039 = GetString(vCustomers, "CODETERM"),
            RateType040 = GetString(vCustomers, "IDRATETYPE"),
            TaxGroup041 = GetString(vCustomers, "CODETAXGRP"),
            TaxRegistrationNo1042 = GetString(vCustomers, "IDTAXREGI1"),
            TaxRegistrationNo2043 = GetString(vCustomers, "IDTAXREGI2"),
            TaxRegistrationNo3044 = GetString(vCustomers, "IDTAXREGI3"),
            TaxRegistrationNo4045 = GetString(vCustomers, "IDTAXREGI4"),
            TaxRegistrationNo5046 = GetString(vCustomers, "IDTAXREGI5"),
            TaxClassCode1047 = GetString(vCustomers, "TAXSTTS1"),
            TaxClassCode2048 = GetString(vCustomers, "TAXSTTS2"),
            TaxClassCode3049 = GetString(vCustomers, "TAXSTTS3"),
            TaxClassCode4050 = GetString(vCustomers, "TAXSTTS4"),
            TaxClassCode5051 = GetString(vCustomers, "TAXSTTS5"),
            CreditLimitCustCurr052 = GetString(vCustomers, "AMTCRLIMT"),
            BalanceDueinCustCurr053 = GetString(vCustomers, "AMTBALDUET"),
            BalanceDueinFuncCurr054 = GetString(vCustomers, "AMTBALDUEH"),
            DateofLastStatement055 = GetDate(vCustomers, "DATELASTST"),
            LastStatementTotalCustCurr056 = GetString(vCustomers, "AMTLASTSTT"),
            Reserved057 = GetString(vCustomers, "AMTLASTSTH"),
            DateofLastBalFwdStatement058 = GetDate(vCustomers, "DTBEGBALFW"),
            BeginningBalonLastStatement059 = GetString(vCustomers, "AMTBALFWDT"),
            Reserved060 = GetString(vCustomers, "AMTBALFWDH"),
            DateofLastRevaluation061 = GetDate(vCustomers, "DTLASTRVAL"),
            LastRevaluationBalance062 = GetString(vCustomers, "AMTBALLARV"),
            NumberofOpenDocuments063 = GetString(vCustomers, "CNTOPENINV"),
            NumberofPaidInvoices064 = GetString(vCustomers, "CNTINVPAID"),
            NumberofDaystoPay065 = GetString(vCustomers, "DAYSTOPAY"),
            DateofLargestInvoice066 = GetDate(vCustomers, "DATEINVCHI"),
            DateofHighestBalance067 = GetDate(vCustomers, "DATEBALHI"),
            DateofLargestInvoiceLastYr068 = GetDate(vCustomers, "DATEINVHIL"),
            DateofHighestBalanceLastYr069 = GetDate(vCustomers, "DATEBALHIL"),
            DateofLastActivity070 = GetDate(vCustomers, "DATELASTAC"),
            DateofLastInvoice071 = GetDate(vCustomers, "DATELASTIV"),
            DateofLastCreditNote072 = GetDate(vCustomers, "DATELASTCR"),
            DateofLastDebitNote073 = GetDate(vCustomers, "DATELASTDR"),
            DateofLastReceipt074 = GetDate(vCustomers, "DATELASTPA"),
            DateofLastDiscount075 = GetDate(vCustomers, "DATELASTDI"),
            DateofLastAdjustment076 = GetDate(vCustomers, "DATELASTAD"),
            DateofLastWriteOff077 = GetDate(vCustomers, "DATELASTWR"),
            DateofLastReturnedCheck078 = GetDate(vCustomers, "DATELASTRI"),
            DateofLastInterestCharge079 = GetDate(vCustomers, "DATELASTIN"),
            Reserved080 = GetDate(vCustomers, "DATELASTDQ"),
            LargestInvoiceNumber081 = GetString(vCustomers, "IDINVCHI"),
            LargestInvoiceNumberLastYr082 = GetString(vCustomers, "IDINVCHILY"),
            LargestInvoiceCustCurr083 = GetString(vCustomers, "AMTINVHIT"),
            HighestBalanceCustCurr084 = GetString(vCustomers, "AMTBALHIT"),
            LgstInvLastYrCustCurr085 = GetString(vCustomers, "AMTINVHILT"),
            HighBalLastYrCustCurr086 = GetString(vCustomers, "AMTBALHILT"),
            LastInvoiceAmtCustCurr087 = GetString(vCustomers, "AMTLASTIVT"),
            LastCrNoteAmtCustCurr088 = GetString(vCustomers, "AMTLASTCRT"),
            LastDrNoteAmtCustCurr089 = GetString(vCustomers, "AMTLASTDRT"),
            LastReceiptCustCurr090 = GetString(vCustomers, "AMTLASTPYT"),
            LastDiscountAmtCustCurr091 = GetString(vCustomers, "AMTLASTDIT"),
            LastAdjAmtCustCurr092 = GetString(vCustomers, "AMTLASTADT"),
            LastWriteOffAmtCustCurr093 = GetString(vCustomers, "AMTLASTWRT"),
            LastRetdChkAmtCustCurr094 = GetString(vCustomers, "AMTLASTRIT"),
            LastIntChargeCustCurr095 = GetString(vCustomers, "AMTLASTINT"),
            LargestInvoiceFuncCurr096 = GetString(vCustomers, "AMTINVHIH"),
            HighestBalanceFuncCurr097 = GetString(vCustomers, "AMTBALHIH"),
            LgstInvLastYrFuncCurr098 = GetString(vCustomers, "AMTINVHILH"),
            HighBalLastYrFuncCurr099 = GetString(vCustomers, "AMTBALHILH"),
            LastInvoiceAmtFuncCurr100 = GetString(vCustomers, "AMTLASTIVH"),
            LastCrNoteAmtFuncCurr101 = GetString(vCustomers, "AMTLASTCRH"),
            LastDrNoteAmtFuncCurr102 = GetString(vCustomers, "AMTLASTDRH"),
            LastReceiptFuncCurr103 = GetString(vCustomers, "AMTLASTPYH"),
            LastDiscountAmtFuncCurr104 = GetString(vCustomers, "AMTLASTDIH"),
            LastAdjAmtFuncCurr105 = GetString(vCustomers, "AMTLASTADH"),
            LastWriteOffAmtFuncCurr106 = GetString(vCustomers, "AMTLASTWRH"),
            LastRetdChkAmtFuncCurr107 = GetString(vCustomers, "AMTLASTRIH"),
            LastIntChargeFuncCurr108 = GetString(vCustomers, "AMTLASTINH"),
            Salesperson1109 = GetString(vCustomers, "CODESLSP1"),
            Salesperson2110 = GetString(vCustomers, "CODESLSP2"),
            Salesperson3111 = GetString(vCustomers, "CODESLSP3"),
            Salesperson4112 = GetString(vCustomers, "CODESLSP4"),
            Salesperson5113 = GetString(vCustomers, "CODESLSP5"),
            SalesSplitPercentage1114 = GetString(vCustomers, "PCTSASPLT1"),
            SalesSplitPercentage2115 = GetString(vCustomers, "PCTSASPLT2"),
            SalesSplitPercentage3116 = GetString(vCustomers, "PCTSASPLT3"),
            SalesSplitPercentage4117 = GetString(vCustomers, "PCTSASPLT4"),
            SalesSplitPercentage5118 = GetString(vCustomers, "PCTSASPLT5"),
            CustomerPriceList119 = GetString(vCustomers, "PRICLIST"),
            CustomerDiscountType120 = GetString(vCustomers, "CUSTTYPE"),
            AmountPastDue121 = GetString(vCustomers, "AMTPDUE"),
            DunningMessage122 = GetString(vCustomers, "TEXTSTMT"),
            ContactsEmail123 = GetString(vCustomers, "EMAIL1"),
            CustomersEmail124 = GetString(vCustomers, "EMAIL2"),
            WebSite125 = GetString(vCustomers, "WEBSITE"),
            DeliveryMethod126 = GetString(vCustomers, "DELMETHOD"),
            ContactsPhone127 = GetString(vCustomers, "CTACPHONE"),
            ContactsFax128 = GetString(vCustomers, "CTACFAX"),
            AllowPartialShipments129 = GetString(vCustomers, "SWPARTSHIP"),
            HDRAmountBeginningBalanceFor130 = GetString(vCustomers, "HAMTBGNBLF"),
            HDRAmountEndingBalanceForwar131 = GetString(vCustomers, "HAMTEBALFD"),
            HDRAmountStatementBalance132 = GetString(vCustomers, "HAMTSTMTBL"),
            HDRAmountDueCurrentPeriod133 = GetString(vCustomers, "HAMTDUECUR"),
            HDRAmountDue1stPeriod134 = GetString(vCustomers, "HAMTDUEAG1"),
            HDRAmountDue2ndPeriod135 = GetString(vCustomers, "HAMTDUEAG2"),
            HDRAmountDue3rdPeriod136 = GetString(vCustomers, "HAMTDUEAG3"),
            HDRAmountDue4thPeriod137 = GetString(vCustomers, "HAMTDUEAG4"),
            HDRAmountDueForwardBalance138 = GetString(vCustomers, "HAMTDUEFWD"),
            RemitToName139 = GetString(vCustomers, "RBCNAME"),
            RemitToAddress1140 = GetString(vCustomers, "RBCSTREET1"),
            RemitToAddress2141 = GetString(vCustomers, "RBCSTREET2"),
            RemitToAddress3142 = GetString(vCustomers, "RBCSTREET3"),
            RemitToAddress4143 = GetString(vCustomers, "RBCSTREET4"),
            RemitToCity144 = GetString(vCustomers, "RBCCITY"),
            RemitToStateProv145 = GetString(vCustomers, "RBCSTATE"),
            RemitToZipPostalCode146 = GetString(vCustomers, "RBCPSTCDE"),
            RemitToCountry147 = GetString(vCustomers, "RBCCNTYCDE"),
            CustomerCurrencyDecimal148 = GetString(vCustomers, "CUSDECIMAL"),
            CustomerCurrencySymbol149 = GetString(vCustomers, "CURSYMBOL")
        };

    private static List<string> GetKeyFields(dynamic view)
    {
        try
        {
            dynamic key = view.Keys.Item(0);
            var count = (int)key.FieldCount;
            var names = new List<string>(count);
            for (var i = 0; i < count; i++)
            {
                names.Add(Convert.ToString(key.Field(i).Name, CultureInfo.InvariantCulture) ?? "");
            }

            names.RemoveAll(string.IsNullOrWhiteSpace);
            return names;
        }
        catch
        {
            return new List<string> { "STMTSEQ" };
        }
    }

    private static string? GetString(dynamic view, string fieldName)
    {
        try
        {
            return Convert.ToString(view.Fields.FieldByName(fieldName).Value, CultureInfo.InvariantCulture);
        }
        catch
        {
            return null;
        }
    }

    private static DateTime? GetDate(dynamic view, string fieldName)
    {
        try
        {
            var value = view.Fields.FieldByName(fieldName).Value;
            if (value is DateTime dt && dt != default)
            {
                return dt;
            }

            if (value is null)
            {
                return null;
            }

            var s = Convert.ToString(value, CultureInfo.InvariantCulture);
            if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime parsed))
            {
                return parsed;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}
