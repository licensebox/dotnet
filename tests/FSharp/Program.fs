open System
open System.Collections.Generic
open System.Text.Json
open System.Text.Json.Nodes
open Licensebox
let r = JsonNode.Parse(Console.In.ReadToEnd()).AsObject()
let s name = if r.ContainsKey(name) then r[name].GetValue<string>() else ""
let run () : obj =
    let product = s "product"
    let keys = Dictionary<string,string>()
    if r.ContainsKey("keys") then
        for pair in r["keys"].AsObject() do keys.Add(pair.Key,pair.Value.GetValue<string>())
    match s "op" with
    | "derive" -> box (Client.DeriveBinding(product,s "source",s "raw_id"))
    | "native" -> box (Client.NativeBinding(product))
    | "verify" -> box (Client.VerifyLease(s "token",product,keys,r["now"].GetValue<double>()))
    | op ->
        let cfg = Config(product,keys)
        cfg.Server <- s "server"
        cfg.Directory <- s "directory"
        cfg.Version <- "conformance"
        cfg.Clock <- Func<double>(fun () -> r["now"].GetValue<double>())
        cfg.MachineBinding <- Func<string>(fun () -> s "binding")
        cfg.OnSignIn <- Action<string,string>(fun _ _ -> ())
        use c = new Client(cfg)
        match op with
        | "lease" -> box (c.Lease())
        | "status" -> box (c.Status())
        | "sign_in" -> box (c.SignIn(3.0))
        | "require" -> box (c.Require(if r.ContainsKey("module") then s "module" else "core"))
        | "repeat" -> c.Require("core") |> ignore; box (c.Require("core"))
        | "logout" -> c.Logout(); null
        | "modules" -> box (c.Modules())
        | "requests" -> box (c.Requests())
        | "request" -> box (c.RequestAccess(["export"],"café – test"))
        | "access-link" -> box (c.AccessLink())
        | "record" -> c.Record("export",true,0.125); c.Record("export",false,0.025); null
        | _ -> failwith "unknown operation"
let answer = Dictionary<string,obj>()
try
    answer.Add("value",run())
    answer.Add("ok",box true)
with :? LicenseboxException as e ->
    answer.Add("ok",box false)
    answer.Add("error",box e.Code)
Console.WriteLine(JsonSerializer.Serialize(answer))
