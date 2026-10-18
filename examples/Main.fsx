// Reference the installed Licensebox assembly before loading this script.
open System.Collections.Generic
open Licensebox
let keys = dict [ "myapp-k1", "YOUR_PUBLIC_VERIFICATION_KEY" ]
let cfg = Config("myapp",keys)
use client = new Client(cfg)
let authorized = client.Require("export")
// Process user files only after Require succeeds.
