using System;
using System.Runtime.InteropServices;


[InterfaceType(ComInterfaceType.InterfaceIsDual)]
[Guid("73F49300-A12D-4FA6-B6C0-1597F03C2FC3")]
public interface _ApiTokenBase
{
    string Test();
    string GetAccessToken();
    string GetTokenType();
}

[ClassInterface(ClassInterfaceType.None)]
[Guid("B61537E2-62A6-4A58-B5E0-01F21FE2D04C")]
[ProgId("ApiToken")]
public class ApiToken : _ApiTokenBase
{

    public string Test()
    {
        // Tests whether the class works properly
        return "Success.";
    }
    public string access_token { get; set; }
    public string token_type { get; set; }

    public string GetAccessToken()
    {
        return access_token;
    }
    public string GetTokenType()
    {
        return token_type;
    }
}

