# Licensebox for C# / .NET

Application licensing backed by the shared C core: native machine identity,
email sign-in, signed offline leases, silent renewal, module access, approval
requests and usage reporting. Linux is the initial runtime target.

Requirements: .NET 10; no managed runtime dependencies; Licensebox C >=0.5.0, OpenSSL 3.5+, libcurl 7.85+ and
json-c. The native dependency is pinned in `native-version.txt` and updated by
the tested contract-sync workflow. No native code is downloaded at runtime.

## Install the native dependency

Clone this repository/tag. Install native development packages, then run:

```sh
sudo apt install build-essential cmake git libssl-dev libcurl4-openssl-dev libjson-c-dev
bash scripts/install-native.sh "$PWD/.native"
export LICENSEBOX_LIB_DIR="$PWD/.native/lib"
export LICENSEBOX_LIBRARY="$PWD/.native/lib/liblicensebox.so"
export LD_LIBRARY_PATH="$PWD/.native/lib${LD_LIBRARY_PATH:+:$LD_LIBRARY_PATH}"
```

Use that absolute native library directory when running the consuming app.
It may instead be installed in the system loader's normal search path.
The helper builds the pinned public C release without changing system folders.

## Authorize protected work

```csharp
var options = new Licensebox.Config("myapp", new Dictionary<string,string> {
    ["myapp-k1"] = "YOUR_PUBLIC_VERIFICATION_KEY"
}) { Version = "1.0" };
using var client = new Licensebox.Client(options);
client.Require("export");
// Protected work starts here.
```

Build a local package with `dotnet pack src/Licensebox/Licensebox.csproj -c Release
-o packages`, then add `Licensebox` 0.3.0 from that package directory to the
consumer. NuGet registry publication is separate. Results are owned
`System.Text.Json.JsonElement` values. Catch `LicenseboxException` and inspect
`Code`. Use `using`/`Dispose`; calls and disposal are synchronized. F# and VB.NET consume the same assembly and each run the shared licensing suite in CI.


Create one client per process. The complete API includes lease, require, local
status, explicit sign-in, logout, module discovery, access requests/listing,
private access links, and fixed command usage records. Rust/Julia use snake_case;
Java uses lowerCamelCase; .NET uses PascalCase. See source API signatures and the
conformance adapter for examples of every operation. Get product/module IDs and
**public verification keys** from the administrator. Never embed admin tokens,
private signing keys or copied installation keys.

Sign-in defaults to instructions on stderr; a callback can present them in your
UI. Cancellation is supported. Callback failures stop the operation and never
unwind into native code. Callbacks must not reenter the client. Clock/native
binding providers are trusted platform/testing seams, never end-user identity
overrides. Config strings and keys are copied by the native client.

## Errors and lifecycle

`machine_changed`, `seat_transferred`, `location_denied` and `location_unknown`
refuse protected work and persist across restart/offline use. Explicit sign-in
can transfer a seat; ordinary renewal cannot take one back. `module_denied`
means missing module access; `offline` means an expired lease could not renew;
`revoked` removes cached access. Always stop protected work on an error.

Each process silently renews on first protected use, bounded to three seconds.
A verified, unexpired cached lease permits offline use during transport/server
outages. Default allowance is 24 hours; there is no background heartbeat. Logout
retains installation identity. Native IDs stay local; only a product-scoped
digest is sent. Cloned/spoofed IDs and modified host software remain limitations:
this is not hardware attestation. Payments and billing are handled separately
by product administrators.

Other operating systems and browser/WASM are not supported by this native C
profile yet. Test/validation status is recorded in GitHub Actions; do not infer
platform support merely because a language runtime exists there.

See [shared protocol and tests](https://github.com/licensebox/protocol),
[the full roadmap](https://github.com/licensebox/protocol/blob/main/ROADMAP.md)
and [integration docs](https://licensebox.dev/docs). Administration remains in
the [Go CLI](https://github.com/licensebox/go) and web console.

MIT. Copyright © 2026 [Vijai Kumar](https://meshing.engineer).

## F# and Visual Basic

The same managed assembly exposes the complete API to C#, F# and VB.NET.
See `examples/Main.fsx` and `examples/Main.vb`. Use F# `use` or VB `Using` to
release the client. The F# and VB adapters are compiled and each runs the shared
69-case Linux suite in CI; no extra native implementation is involved.

## Optional explicit heartbeat

Use `client.Heartbeat("core")` in C#, F# and Visual Basic for an explicit bounded renewal/checkpoint. The mandatory module must be nonempty. The result contains `lease` (the complete verified lease, preserving optional signed `heartbeat.interval_seconds`/`grace_seconds`) and `online` (whether this check renewed successfully online). Every invocation attempts renewal, including after a cached `Require`; it never starts interactive activation or transfers a seat. Eligible outages may return `online=false` only while the last verified signed lease remains unexpired. Refusals, missing modules and expired offline access remain errors.

Default Client/Lease/Require behavior is unchanged. No timer, task, thread or background heartbeat starts automatically. The application owns event-loop scheduling/checkpoints on the language runtime thread, and safe cancellation of protected work on refusal or the last verified expiry deadline. A failed check cannot extend that deadline. This is especially important for MATLAB, Wolfram/JLink and other runtimes whose callbacks must stay on their owning thread. The matching native C heartbeat API/contract is required.

Validate the feature against the candidate native core with `python3 ../protocol/conformance/heartbeat.py --adapter bash ci/adapter.sh` in addition to the existing full shared suite.
