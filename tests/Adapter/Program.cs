using Licensebox;
using System.Text.Json;
using System.Text.Json.Nodes;
static object? Run(JsonObject r)
{
    string S(string name)=>r[name]?.GetValue<string>()??"";
    string op=S("op"),product=S("product");
    var keys=r["keys"]?.Deserialize<Dictionary<string,string>>()??new();
    if(op=="derive")return Client.DeriveBinding(product,S("source"),S("raw_id"));
    if(op=="native")return Client.NativeBinding(product);
    if(op=="verify")return Client.VerifyLease(S("token"),product,keys,r["now"]!.GetValue<double>());
    using var c=new Client(new Config(product,keys){Server=S("server"),Directory=S("directory"),Version="conformance",Clock=()=>r["now"]!.GetValue<double>(),MachineBinding=()=>S("binding"),OnSignIn=(_,_)=>{}});
    switch(op)
    {
        case "lease":return c.Lease();case "status":return c.Status();case "sign_in":return c.SignIn(3);
        case "require":return c.Require(r.ContainsKey("module")?S("module"):"core");
        case "heartbeat":return c.Heartbeat(r.ContainsKey("module")?S("module"):"core");
        case "heartbeat-repeat":c.Heartbeat("core");return c.Heartbeat("core");
        case "heartbeat-after-require":c.Require("core");return c.Heartbeat("core");
        case "repeat":c.Require("core");return c.Require("core");case "logout":c.Logout();return null;
        case "modules":return c.Modules();case "requests":return c.Requests();case "request":return c.RequestAccess(["export"],"café – test");
        case "access-link":return c.AccessLink();case "record":c.Record("export",true,.125);c.Record("export",false,.025);return null;
        default:throw new ArgumentException("unknown operation");
    }
}
var r=JsonNode.Parse(Console.In.ReadToEnd())!.AsObject();object answer;
try{answer=new{ok=true,value=Run(r)};}catch(LicenseboxException e){answer=new{ok=false,error=e.Code};}
Console.WriteLine(JsonSerializer.Serialize(answer));
