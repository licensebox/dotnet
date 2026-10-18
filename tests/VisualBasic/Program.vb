Imports System
Imports System.Collections.Generic
Imports System.Text.Json
Imports System.Text.Json.Nodes
Imports Licensebox
Module Program
    Private r As JsonObject
    Private Function S(name As String) As String
        If r.ContainsKey(name) Then Return r(name).GetValue(Of String)()
        Return ""
    End Function
    Private Function Run() As Object
        Dim product = S("product")
        Dim keys As New Dictionary(Of String, String)
        If r.ContainsKey("keys") Then
            For Each pair In r("keys").AsObject()
                keys.Add(pair.Key,pair.Value.GetValue(Of String)())
            Next
        End If
        Dim op = S("op")
        Select Case op
            Case "derive" : Return Client.DeriveBinding(product,S("source"),S("raw_id"))
            Case "native" : Return Client.NativeBinding(product)
            Case "verify" : Return Client.VerifyLease(S("token"),product,keys,r("now").GetValue(Of Double)())
        End Select
        Dim cfg As New Config(product,keys)
        cfg.Server = S("server")
        cfg.Directory = S("directory")
        cfg.Version = "conformance"
        cfg.Clock = Function() r("now").GetValue(Of Double)()
        cfg.MachineBinding = Function() S("binding")
        cfg.OnSignIn = Sub(url,code)
                       End Sub
        Using c As New Client(cfg)
            Select Case op
                Case "lease" : Return c.Lease()
                Case "status" : Return c.Status()
                Case "sign_in" : Return c.SignIn(3)
                Case "require" : Return c.Require(If(r.ContainsKey("module"),S("module"),"core"))
                Case "repeat" : c.Require("core") : Return c.Require("core")
                Case "heartbeat" : Return c.Heartbeat(If(r.ContainsKey("module"),S("module"),"core"))
                Case "heartbeat-repeat" : c.Heartbeat("core") : Return c.Heartbeat("core")
                Case "heartbeat-after-require" : c.Require("core") : Return c.Heartbeat("core")
                Case "logout" : c.Logout() : Return Nothing
                Case "modules" : Return c.Modules()
                Case "requests" : Return c.Requests()
                Case "request" : Return c.RequestAccess(New String(){"export"},"café – test")
                Case "access-link" : Return c.AccessLink()
                Case "record" : c.Record("export",True,0.125) : c.Record("export",False,0.025) : Return Nothing
                Case Else : Throw New ArgumentException("unknown operation")
            End Select
        End Using
    End Function
    Sub Main()
        r = JsonNode.Parse(Console.In.ReadToEnd()).AsObject()
        Dim answer As New Dictionary(Of String,Object)
        Try
            answer.Add("value",Run())
            answer.Add("ok",True)
        Catch e As LicenseboxException
            answer.Add("ok",False)
            answer.Add("error",e.Code)
        End Try
        Console.WriteLine(JsonSerializer.Serialize(answer))
    End Sub
End Module
