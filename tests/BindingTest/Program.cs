using Licensebox;
var cfg=new Config("demo",new Dictionary<string,string>{{"test","A6EHv_POEL4dcN0Y50vAmWfk1jCbpQ1fHdyGZBJVMbg"}}){Server="http://127.0.0.1:1",MachineBinding=()=>throw new InvalidOperationException("synthetic provider failure")};
var c=new Client(cfg);GC.Collect();GC.WaitForPendingFinalizers();
try{c.Require("core");throw new Exception("callback failure allowed protected work");}catch(LicenseboxException e)when(e.Code=="callback_error"){}finally{c.Dispose();}
c.Dispose();try{c.Status();throw new Exception("closed client used native handle");}catch(ObjectDisposedException){}
Console.WriteLine("PASS: callback errors, garbage collection and explicit disposal");
