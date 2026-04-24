using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdLumeClient.Classes;

public static class MpvClientFactory
{
    public static IMpvClient Create()
    {
        return new MpvClient();
    }
}