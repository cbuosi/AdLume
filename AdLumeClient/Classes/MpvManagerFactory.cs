using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace AdLumeClient.Classes;

public static class MpvManagerFactory
{
    public static IMpvManager Create()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return new MpvManager_Windows();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return new MpvManager_Linux();

        throw new NotSupportedException();
    }
}
