namespace Glb.Common.Enums;
using System.ComponentModel;

public enum Gender { Unkown, Male, Female }
public enum MourselSMS_Type
{
    Latin = 1,
    Unicode = 6
}
public enum ServiceProvider
{
    Moursel = 0
}
public enum ProductType
{
    All = 0,
    [Description("DSL")]
    DSL = 1,
    [Description("IPTV")]
    IPTV = 6,
    [Description("DualPlay")]
    DualPlay = 7,
    [Description("FTTH")]
    Fiber = 4,
    [Description("Additional Service DSL")]
    AdditionalServiceDSL = 8,
    [Description("Additional Service FTTH")]
    AdditionalServiceFTTH = 13,
    [Description("Additional Quota")]
    AdditionalQuota = 9,
    [Description("3G Residential")]
    IIIG = 10,
    [Description("4G Residential")]
    IVG = 3,
    [Description("One Time Event")]
    OTE = 11,
    [Description("IFly")]
    IFly = 12,
    [Description("DialUp")]
    DialUp = 5,
    [Description("Cable Vission Bundle")]
    TVBunble = 14
}