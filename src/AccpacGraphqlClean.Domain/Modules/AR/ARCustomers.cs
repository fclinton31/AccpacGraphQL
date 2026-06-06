namespace AccpacGraphqlClean.Domain;

public sealed class ARCustomers
{
    public string? Compid { get; set; }

    [SageField("IDCUST")]
    public string? CustomerNumber000 { get; set; }

    [SageField("TEXTSNAM")]
    public string? ShortName001 { get; set; }

    [SageField("IDGRP")]
    public string? GroupCode002 { get; set; }

    [SageField("IDNATACCT")]
    public string? NationalAccount003 { get; set; }

    [SageField("SWACTV")]
    public string? Status004 { get; set; }

    [SageField("SWHOLD")]
    public string? OnHold007 { get; set; }

    [SageField("DATESTART")]
    public DateTime? StartDate008 { get; set; }

    [SageField("CODEDAB")]
    public string? CreditBureauNumber010 { get; set; }

    [SageField("CODEDABRTG")]
    public string? CreditBureauRating011 { get; set; }

    [SageField("DATEDAB")]
    public DateTime? CreditBureauDate012 { get; set; }

    [SageField("NAMECUST")]
    public string? CustomerName013 { get; set; }

    [SageField("TEXTSTRE1")]
    public string? AddressLine1014 { get; set; }

    [SageField("TEXTSTRE2")]
    public string? AddressLine2015 { get; set; }

    [SageField("TEXTSTRE3")]
    public string? AddressLine3016 { get; set; }

    [SageField("TEXTSTRE4")]
    public string? AddressLine4017 { get; set; }

    [SageField("NAMECITY")]
    public string? City018 { get; set; }

    [SageField("CODESTTE")]
    public string? StateProv019 { get; set; }

    [SageField("CODEPSTL")]
    public string? ZipPostalCode020 { get; set; }

    [SageField("CODECTRY")]
    public string? Country021 { get; set; }

    [SageField("NAMECTAC")]
    public string? ContactName022 { get; set; }

    [SageField("TEXTPHON1")]
    public string? PhoneNumber023 { get; set; }

    [SageField("TEXTPHON2")]
    public string? FaxNumber024 { get; set; }

    [SageField("CODETERR")]
    public string? TerritoryCode025 { get; set; }

    [SageField("IDACCTSET")]
    public string? AccountSet026 { get; set; }

    [SageField("IDAUTOCASH")]
    public string? AutocashProfile027 { get; set; }

    [SageField("IDBILLCYCL")]
    public string? BillingCycle028 { get; set; }

    [SageField("IDSVCCHRG")]
    public string? InterestProfile029 { get; set; }

    [SageField("CODECURN")]
    public string? CurrencyCode031 { get; set; }

    [SageField("SWPRTSTMT")]
    public string? PrintStatements032 { get; set; }

    [SageField("SWBALFWD")]
    public string? AccountType034 { get; set; }

    [SageField("CODETERM")]
    public string? Terms035 { get; set; }

    [SageField("IDRATETYPE")]
    public string? RateType036 { get; set; }

    [SageField("CODETAXGRP")]
    public string? TaxGroup037 { get; set; }

    [SageField("IDTAXREGI1")]
    public string? TaxRegistrationNo1038 { get; set; }

    [SageField("IDTAXREGI2")]
    public string? TaxRegistrationNo2039 { get; set; }

    [SageField("IDTAXREGI3")]
    public string? TaxRegistrationNo3040 { get; set; }

    [SageField("IDTAXREGI4")]
    public string? TaxRegistrationNo4041 { get; set; }

    [SageField("IDTAXREGI5")]
    public string? TaxRegistrationNo5042 { get; set; }

    [SageField("TAXSTTS1")]
    public string? TaxClassCode1043 { get; set; }

    [SageField("TAXSTTS2")]
    public string? TaxClassCode2044 { get; set; }

    [SageField("TAXSTTS3")]
    public string? TaxClassCode3045 { get; set; }

    [SageField("TAXSTTS4")]
    public string? TaxClassCode4046 { get; set; }

    [SageField("TAXSTTS5")]
    public string? TaxClassCode5047 { get; set; }

    [SageField("AMTCRLIMT")]
    public string? CreditLimitCustCurr048 { get; set; }

    [SageField("AMTBALDUET")]
    public string? BalanceDueInCustCurr049 { get; set; }

    [SageField("AMTBALDUEH")]
    public string? BalanceDueinFuncCurr050 { get; set; }

    [SageField("DTLASTRVAL")]
    public DateTime? DateofLastRevaluation057 { get; set; }

    [SageField("AMTBALLARV")]
    public string? LastRevaluationBalance058 { get; set; }

    [SageField("CNTINVPAID")]
    public string? NumberofPaidInvoices060 { get; set; }

    [SageField("DAYSTOPAY")]
    public string? NumberofDaystoPay061 { get; set; }

    [SageField("DATEINVCHI")]
    public DateTime? DateofLargestInvoice062 { get; set; }

    [SageField("DATEBALHI")]
    public DateTime? DateofHighestBalance063 { get; set; }

    [SageField("DATEINVHIL")]
    public DateTime? DateofLargestInvoiceLastYr064 { get; set; }

    [SageField("DATEBALHIL")]
    public DateTime? DateofHighestBalanceLastYr065 { get; set; }

    [SageField("DATELASTAC")]
    public DateTime? DateofLastActivity066 { get; set; }

    [SageField("DATELASTIV")]
    public DateTime? DateofLastInvoice067 { get; set; }

    [SageField("DATELASTCR")]
    public DateTime? DateofLastCreditNote068 { get; set; }

    [SageField("DATELASTDR")]
    public DateTime? DateofLastDebitNote069 { get; set; }

    [SageField("DATELASTPA")]
    public DateTime? DateofLastReceipt070 { get; set; }

    [SageField("DATELASTDI")]
    public DateTime? DateofLastDiscount071 { get; set; }

    [SageField("DATELASTAD")]
    public DateTime? DateofLastAdjustment072 { get; set; }

    [SageField("DATELASTWR")]
    public DateTime? DateofLastWriteOff073 { get; set; }

    [SageField("DATELASTRI")]
    public DateTime? DateofLastReturnedCheck074 { get; set; }

    [SageField("DATELASTIN")]
    public DateTime? DateofLastInterestCharge075 { get; set; }

    [SageField("AMTINVHIT")]
    public string? LargestInvoiceCustCurr079 { get; set; }

    [SageField("AMTBALHIT")]
    public string? HighestBalanceCustCurr080 { get; set; }

    [SageField("AMTINVHILT")]
    public string? LgstInvLastYrCustCurr081 { get; set; }

    [SageField("AMTBALHILT")]
    public string? HighBalLastYrCustCurr082 { get; set; }

    [SageField("AMTLASTIVT")]
    public string? LastInvoiceAmtCustCurr083 { get; set; }

    [SageField("AMTLASTCRT")]
    public string? LastCrNoteAmtCustCurr084 { get; set; }

    [SageField("AMTLASTDRT")]
    public string? LastDrNoteAmtCustCurr085 { get; set; }

    [SageField("AMTLASTPYT")]
    public string? LastReceiptCustCurr086 { get; set; }

    [SageField("AMTLASTDIT")]
    public string? LastDiscountAmtCustCurr087 { get; set; }

    [SageField("AMTLASTADT")]
    public string? LastAdjAmtCustCurr088 { get; set; }

    [SageField("AMTLASTWRT")]
    public string? LastWriteOffAmtCustCurr089 { get; set; }

    [SageField("AMTLASTRIT")]
    public string? LastRetdChkAmtCustCurr090 { get; set; }

    [SageField("AMTLASTINT")]
    public string? LastIntChargeCustCurr091 { get; set; }

    [SageField("AMTINVHIH")]
    public string? LargestInvoiceFuncCurr092 { get; set; }

    [SageField("AMTBALHIH")]
    public string? HighestBalanceFuncCurr093 { get; set; }

    [SageField("AMTINVHILH")]
    public string? LgstInvLastYrFuncCurr094 { get; set; }

    [SageField("AMTBALHILH")]
    public string? HighBalLastYrFuncCurr095 { get; set; }

    [SageField("AMTLASTIVH")]
    public string? LastInvoiceAmtFuncCurr096 { get; set; }

    [SageField("AMTLASTCRH")]
    public string? LastCrNoteAmtFuncCurr097 { get; set; }

    [SageField("AMTLASTDRH")]
    public string? LastDrNoteAmtFuncCurr098 { get; set; }

    [SageField("AMTLASTPYH")]
    public string? LastReceiptFuncCurr099 { get; set; }

    [SageField("AMTLASTDIH")]
    public string? LastDiscountAmtFuncCurr100 { get; set; }

    [SageField("AMTLASTADH")]
    public string? LastAdjAmtFuncCurr101 { get; set; }

    [SageField("AMTLASTWRH")]
    public string? LastWriteOffAmtFuncCurr102 { get; set; }

    [SageField("AMTLASTRIH")]
    public string? LastRetdChkAmtFuncCurr103 { get; set; }

    [SageField("AMTLASTINH")]
    public string? LastIntChargeFuncCurr104 { get; set; }

    [SageField("CODESLSP1")]
    public string? Salesperson1105 { get; set; }

    [SageField("CODESLSP2")]
    public string? Salesperson2106 { get; set; }

    [SageField("CODESLSP3")]
    public string? Salesperson3107 { get; set; }

    [SageField("CODESLSP4")]
    public string? Salesperson4108 { get; set; }

    [SageField("CODESLSP5")]
    public string? Salesperson5109 { get; set; }

    [SageField("PCTSASPLT1")]
    public string? SalesSplitPercentage1110 { get; set; }

    [SageField("PCTSASPLT2")]
    public string? SalesSplitPercentage2111 { get; set; }

    [SageField("PCTSASPLT3")]
    public string? SalesSplitPercentage3112 { get; set; }

    [SageField("PCTSASPLT4")]
    public string? SalesSplitPercentage4113 { get; set; }

    [SageField("PCTSASPLT5")]
    public string? SalesSplitPercentage5114 { get; set; }

    [SageField("PRICLIST")]
    public string? CustomerPriceList116 { get; set; }

    [SageField("CUSTTYPE")]
    public string? CustomerDiscountType117 { get; set; }

    [SageField("EMAIL1")]
    public string? ContactsEmail119 { get; set; }

    [SageField("EMAIL2")]
    public string? Email120 { get; set; }

    [SageField("WEBSITE")]
    public string? WebSite121 { get; set; }

    [SageField("BILLMETHOD")]
    public string? BillingMethod122 { get; set; }

    [SageField("PAYMCODE")]
    public string? PaymentCode123 { get; set; }

    [SageField("FOB")]
    public string? FreeOnBoard124 { get; set; }

    [SageField("SHPVIACODE")]
    public string? ShipViaCode125 { get; set; }

    [SageField("SHPVIADESC")]
    public string? ShipViaDescription126 { get; set; }

    [SageField("DELMETHOD")]
    public string? DeliveryMethod127 { get; set; }

    [SageField("CTACPHONE")]
    public string? ContactsPhone129 { get; set; }

    [SageField("CTACFAX")]
    public string? ContactsFax130 { get; set; }

    [SageField("SWPARTSHIP")]
    public string? AllowPartialShipments131 { get; set; }

    [SageField("SWWEBSHOP")]
    public string? AllowWebStoreShopping132 { get; set; }

    [SageField("RTGPERCENT")]
    public string? PercentRetained133 { get; set; }

    [SageField("RTGDAYS")]
    public string? DaysRetained134 { get; set; }

    [SageField("RTGTERMS")]
    public string? RetainageTermsCode135 { get; set; }

    [SageField("DATELASTRF")]
    public DateTime? DateofLastRefund143 { get; set; }

    [SageField("AMTLASTRFT")]
    public string? LastRefundAmtCustCurr144 { get; set; }

    [SageField("AMTLASTRFH")]
    public string? LastRefundAmtFuncCurr145 { get; set; }

    [SageField("CODECHECK")]
    public string? CheckLanguage146 { get; set; }

    [SageField("LOCATION")]
    public string? InventoryLocation148 { get; set; }

    [SageField("SWCHKLIMIT")]
    public string? CheckCreditLimit149 { get; set; }

    [SageField("SWCHKOVER")]
    public string? CheckOverdueAmounts150 { get; set; }

    [SageField("OVERDAYS")]
    public string? DaysOverdue151 { get; set; }

    [SageField("OVERAMT")]
    public string? AmountOverdue152 { get; set; }

    [SageField("SWBACKORDR")]
    public string? AllowBackorderQuantities153 { get; set; }

    [SageField("SWCHKDUPPO")]
    public string? CheckforDuplicatePOs154 { get; set; }

    [SageField("EWSUPPRESS")]
    public string? SuppressIntegration155 { get; set; }

    [SageField("EWARVER")]
    public string? ARVersion156 { get; set; }

    [SageField("EWORGID")]
    public string? Database157 { get; set; }

    [SageField("EWMODE")]
    public string? Mode158 { get; set; }

    public string? BalanceDueFuncCurr { get; set; }

    public string? BalanceDueFuncCurr159 { get; set; }

    public string? SageBillingAndPaymentCustome160 { get; set; }

    [SageField("BRN")]
    public string? BusinessRegistrationNumber161 { get; set; }

    public OptionalField[]? OptField { get; set; }
}
