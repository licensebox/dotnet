Imports Licensebox
Imports System.Collections.Generic
Module Example
    Sub Main()
        Dim cfg As New Config("myapp", New Dictionary(Of String,String) From {{"myapp-k1","YOUR_PUBLIC_VERIFICATION_KEY"}})
        Using client As New Client(cfg)
            Dim authorized = client.Require("export")
            ' Process user files only after Require succeeds.
        End Using
    End Sub
End Module
