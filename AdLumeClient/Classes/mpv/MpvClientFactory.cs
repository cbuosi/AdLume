using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdLumeClient.Classes.mpv;

public static class MpvClientFactory
{
    public static IMpvClient Create()
    {
        return new MpvClient();
    }
}