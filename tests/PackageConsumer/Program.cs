using Licensebox;
if(!Client.DeriveBinding("demo","linux-machine-id","12345678123456789abc1234567890ab").StartsWith("m1:"))throw new Exception("packaged API failure");
Console.WriteLine("PASS: packaged .NET consumer");
