using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Biegusowo.Tests.Common;

internal class VerifyConfiguration
{
    [ModuleInitializer]
    public static void Init()
    {
        Verifier.UseProjectRelativeDirectory("Snapshots");
    }
}
