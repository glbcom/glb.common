using System;
using System.Collections.Generic;
using Glb.Common.Enums;
namespace Glb.Common.Settings;
public class SMS_Settings
{
    public  List<SMS_ServiceProvider> ServiceProviders{get;init;}=new();
}
public class SMS_ServiceProvider{
    public ServiceProvider ServiceProvider{get;init;}
    public required Uri BaseAddress {get;init;}
    public required string Username {get;init;}
    public required string Password {get;init;}
    public string? SenderID {get;init;}
}