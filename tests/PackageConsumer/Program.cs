using Licensebox;
if(!Client.DeriveBinding("demo","linux-machine-id","12345678123456789abc1234567890ab").StartsWith("m1:"))throw new Exception("packaged API failure");
var directory=Directory.CreateTempSubdirectory("licensebox-consumer-");
try {
    using var client=new Client(new Config("demo",new Dictionary<string,string>{{"test-key","A6EHv_POEL4dcN0Y50vAmWfk1jCbpQ1fHdyGZBJVMbg"}}){Directory=directory.FullName,Server="http://127.0.0.1:1"});
    try {client.Heartbeat("core");throw new Exception("heartbeat accepted missing lease");}
    catch(LicenseboxException expected) when(expected.Code=="not_signed_in") {}
} finally {directory.Delete(true);}
Console.WriteLine("PASS: packaged .NET consumer and explicit heartbeat refusal");
