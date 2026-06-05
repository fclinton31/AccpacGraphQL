namespace AccpacGraphqlClean.Domain;

public sealed class ARStatementRun
{
    public string? Compid { get; set; }

    public string? StatementSEQ { get; set; }
    public DateTime? StatementRunDate001 { get; set; }
    public string? StatementRunCompletedFlag002 { get; set; }
    public DateTime? CutoffDate003 { get; set; }
    public string? DueDateInvoiceDate004 { get; set; }
    public string? IncludeDebitBalances005 { get; set; }
    public string? IncludeCreditBalances006 { get; set; }
    public string? IncludeZeroBalances007 { get; set; }
    public string? IncludeFullyPaidTransactions008 { get; set; }
    public string? IncludeDetails009 { get; set; }
    public string? RunType010 { get; set; }
    public string? DetailSort011 { get; set; }
    public string? DunningMessageCode012 { get; set; }
    public string? Range1From013 { get; set; }
    public string? Range1To014 { get; set; }
    public string? Range1Type015 { get; set; }
    public string? Range2From016 { get; set; }
    public string? Range2To017 { get; set; }
    public string? Range2Type018 { get; set; }
    public string? Range3From019 { get; set; }
    public string? Range3To020 { get; set; }
    public string? Range3Type021 { get; set; }
    public string? Range4From022 { get; set; }
    public string? Range4To023 { get; set; }
    public string? Range4Type024 { get; set; }
    public string? ReportName025 { get; set; }
    public string? DeliveryMethod026 { get; set; }
    public string? Sortfield1027 { get; set; }
    public string? Sortfield2028 { get; set; }
    public string? Sortfield3029 { get; set; }
    public string? Sortfield4030 { get; set; }
    public string? Current031 { get; set; }
    public string? FirstPeriod032 { get; set; }
    public string? SecondPeriod033 { get; set; }
    public string? ThirdPeriod034 { get; set; }
    public string? SelectCustomersBasedOnOverdu035 { get; set; }
    public string? OpenItemStatementType037 { get; set; }

    public List<ARStatementCustomer> StatementCustomers { get; set; } = new();
    public List<ARStatementReceipt> StatementReceipts { get; set; } = new();
    public List<ARStatementDocument> StatementDocuments { get; set; } = new();
}

public sealed class ARStatementCustomer
{
    public string? StatementSEQ { get; set; }
    public string? CustomerNumber001 { get; set; }
    public string? StatementPrintedFlag002 { get; set; }
    public string? NATStatementSwitch003 { get; set; }
    public string? NationalAccountNumber004 { get; set; }
    public DateTime? StatementRunDate005 { get; set; }
    public string? ShortName006 { get; set; }
    public string? GroupCode007 { get; set; }
    public string? Status008 { get; set; }
    public DateTime? InactiveDate009 { get; set; }
    public DateTime? DateLastMaintained010 { get; set; }
    public string? OnHold011 { get; set; }
    public DateTime? StartDate012 { get; set; }
    public string? Reserved013 { get; set; }
    public string? CreditBureauNumber014 { get; set; }
    public string? CreditBureauRating015 { get; set; }
    public DateTime? CreditBureauDate016 { get; set; }
    public string? CustomerName017 { get; set; }
    public string? AddressLine1018 { get; set; }
    public string? AddressLine2019 { get; set; }
    public string? AddressLine3020 { get; set; }
    public string? AddressLine4021 { get; set; }
    public string? City022 { get; set; }
    public string? StateProv023 { get; set; }
    public string? ZipPostalCode024 { get; set; }
    public string? Country025 { get; set; }
    public string? ContactName026 { get; set; }
    public string? PhoneNumber027 { get; set; }
    public string? FaxNumber028 { get; set; }
    public string? TerritoryCode029 { get; set; }
    public string? AccountSet030 { get; set; }
    public string? AutocashProfile031 { get; set; }
    public string? BillingCycle032 { get; set; }
    public string? InterestProfile033 { get; set; }
    public string? Reserved034 { get; set; }
    public string? CurrencyCode035 { get; set; }
    public string? PrintStatements036 { get; set; }
    public string? Reserved037 { get; set; }
    public string? AccountType038 { get; set; }
    public string? Terms039 { get; set; }
    public string? RateType040 { get; set; }
    public string? TaxGroup041 { get; set; }
    public string? TaxRegistrationNo1042 { get; set; }
    public string? TaxRegistrationNo2043 { get; set; }
    public string? TaxRegistrationNo3044 { get; set; }
    public string? TaxRegistrationNo4045 { get; set; }
    public string? TaxRegistrationNo5046 { get; set; }
    public string? TaxClassCode1047 { get; set; }
    public string? TaxClassCode2048 { get; set; }
    public string? TaxClassCode3049 { get; set; }
    public string? TaxClassCode4050 { get; set; }
    public string? TaxClassCode5051 { get; set; }
    public string? CreditLimitCustCurr052 { get; set; }
    public string? BalanceDueinCustCurr053 { get; set; }
    public string? BalanceDueinFuncCurr054 { get; set; }
    public DateTime? DateofLastStatement055 { get; set; }
    public string? LastStatementTotalCustCurr056 { get; set; }
    public string? Reserved057 { get; set; }
    public DateTime? DateofLastBalFwdStatement058 { get; set; }
    public string? BeginningBalonLastStatement059 { get; set; }
    public string? Reserved060 { get; set; }
    public DateTime? DateofLastRevaluation061 { get; set; }
    public string? LastRevaluationBalance062 { get; set; }
    public string? NumberofOpenDocuments063 { get; set; }
    public string? NumberofPaidInvoices064 { get; set; }
    public string? NumberofDaystoPay065 { get; set; }
    public DateTime? DateofLargestInvoice066 { get; set; }
    public DateTime? DateofHighestBalance067 { get; set; }
    public DateTime? DateofLargestInvoiceLastYr068 { get; set; }
    public DateTime? DateofHighestBalanceLastYr069 { get; set; }
    public DateTime? DateofLastActivity070 { get; set; }
    public DateTime? DateofLastInvoice071 { get; set; }
    public DateTime? DateofLastCreditNote072 { get; set; }
    public DateTime? DateofLastDebitNote073 { get; set; }
    public DateTime? DateofLastReceipt074 { get; set; }
    public DateTime? DateofLastDiscount075 { get; set; }
    public DateTime? DateofLastAdjustment076 { get; set; }
    public DateTime? DateofLastWriteOff077 { get; set; }
    public DateTime? DateofLastReturnedCheck078 { get; set; }
    public DateTime? DateofLastInterestCharge079 { get; set; }
    public DateTime? Reserved080 { get; set; }
    public string? LargestInvoiceNumber081 { get; set; }
    public string? LargestInvoiceNumberLastYr082 { get; set; }
    public string? LargestInvoiceCustCurr083 { get; set; }
    public string? HighestBalanceCustCurr084 { get; set; }
    public string? LgstInvLastYrCustCurr085 { get; set; }
    public string? HighBalLastYrCustCurr086 { get; set; }
    public string? LastInvoiceAmtCustCurr087 { get; set; }
    public string? LastCrNoteAmtCustCurr088 { get; set; }
    public string? LastDrNoteAmtCustCurr089 { get; set; }
    public string? LastReceiptCustCurr090 { get; set; }
    public string? LastDiscountAmtCustCurr091 { get; set; }
    public string? LastAdjAmtCustCurr092 { get; set; }
    public string? LastWriteOffAmtCustCurr093 { get; set; }
    public string? LastRetdChkAmtCustCurr094 { get; set; }
    public string? LastIntChargeCustCurr095 { get; set; }
    public string? LargestInvoiceFuncCurr096 { get; set; }
    public string? HighestBalanceFuncCurr097 { get; set; }
    public string? LgstInvLastYrFuncCurr098 { get; set; }
    public string? HighBalLastYrFuncCurr099 { get; set; }
    public string? LastInvoiceAmtFuncCurr100 { get; set; }
    public string? LastCrNoteAmtFuncCurr101 { get; set; }
    public string? LastDrNoteAmtFuncCurr102 { get; set; }
    public string? LastReceiptFuncCurr103 { get; set; }
    public string? LastDiscountAmtFuncCurr104 { get; set; }
    public string? LastAdjAmtFuncCurr105 { get; set; }
    public string? LastWriteOffAmtFuncCurr106 { get; set; }
    public string? LastRetdChkAmtFuncCurr107 { get; set; }
    public string? LastIntChargeFuncCurr108 { get; set; }
    public string? Salesperson1109 { get; set; }
    public string? Salesperson2110 { get; set; }
    public string? Salesperson3111 { get; set; }
    public string? Salesperson4112 { get; set; }
    public string? Salesperson5113 { get; set; }
    public string? SalesSplitPercentage1114 { get; set; }
    public string? SalesSplitPercentage2115 { get; set; }
    public string? SalesSplitPercentage3116 { get; set; }
    public string? SalesSplitPercentage4117 { get; set; }
    public string? SalesSplitPercentage5118 { get; set; }
    public string? CustomerPriceList119 { get; set; }
    public string? CustomerDiscountType120 { get; set; }
    public string? AmountPastDue121 { get; set; }
    public string? DunningMessage122 { get; set; }
    public string? ContactsEmail123 { get; set; }
    public string? CustomersEmail124 { get; set; }
    public string? WebSite125 { get; set; }
    public string? DeliveryMethod126 { get; set; }
    public string? ContactsPhone127 { get; set; }
    public string? ContactsFax128 { get; set; }
    public string? AllowPartialShipments129 { get; set; }
    public string? HDRAmountBeginningBalanceFor130 { get; set; }
    public string? HDRAmountEndingBalanceForwar131 { get; set; }
    public string? HDRAmountStatementBalance132 { get; set; }
    public string? HDRAmountDueCurrentPeriod133 { get; set; }
    public string? HDRAmountDue1stPeriod134 { get; set; }
    public string? HDRAmountDue2ndPeriod135 { get; set; }
    public string? HDRAmountDue3rdPeriod136 { get; set; }
    public string? HDRAmountDue4thPeriod137 { get; set; }
    public string? HDRAmountDueForwardBalance138 { get; set; }
    public string? RemitToName139 { get; set; }
    public string? RemitToAddress1140 { get; set; }
    public string? RemitToAddress2141 { get; set; }
    public string? RemitToAddress3142 { get; set; }
    public string? RemitToAddress4143 { get; set; }
    public string? RemitToCity144 { get; set; }
    public string? RemitToStateProv145 { get; set; }
    public string? RemitToZipPostalCode146 { get; set; }
    public string? RemitToCountry147 { get; set; }
    public string? CustomerCurrencyDecimal148 { get; set; }
    public string? CustomerCurrencySymbol149 { get; set; }
}

public sealed class ARStatementReceipt
{
    public string? StatementSEQ { get; set; }
    public string? CustomerNumber001 { get; set; }
    public string? DocumentNumber002 { get; set; }
    public DateTime? StatementRunDate004 { get; set; }
    public string? CheckReceiptNo005 { get; set; }
    public DateTime? PostingDate006 { get; set; }
    public string? DocumentType007 { get; set; }
    public string? CustReceiptAmount008 { get; set; }
    public string? TransactionType009 { get; set; }
    public string? ReferenceDocumentNumber010 { get; set; }
}

public sealed class ARStatementDocument
{
    public string? StatementSEQ { get; set; }
    public string? CustomerNumber001 { get; set; }
    public string? DocumentNumber002 { get; set; }
    public DateTime? StatementRunDate003 { get; set; }
    public string? Code004 { get; set; }
    public string? RecordType005 { get; set; }
    public string? CheckReceiptNumber006 { get; set; }
    public string? PONumber007 { get; set; }
    public string? OrderNumber008 { get; set; }
    public DateTime? DocumentDate009 { get; set; }
    public DateTime? DueDate010 { get; set; }
    public string? DocumentDescription011 { get; set; }
    public string? DocumentType012 { get; set; }
    public string? TransactionType013 { get; set; }
    public string? AmountDue014 { get; set; }
    public string? DiscountAmount015 { get; set; }
    public string? ShipToLocation016 { get; set; }
    public string? Terms017 { get; set; }
    public DateTime? LastStatementDate018 { get; set; }
    public string? InvoiceAmount020 { get; set; }
}

public sealed class SyncARStatement
{
    public string? Compid { get; set; }
    public List<ARStatementRun> ARStatements { get; set; } = new();
}
