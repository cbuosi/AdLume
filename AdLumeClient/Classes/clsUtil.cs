using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdLumeClient.Classes
{
    public static class clsUtil
    {
        public static int HoraParaInt(string? strHora)
        {

            int ret = 0;
            string[] ParteHorario;

            try
            {

                if (strHora == null)
                {
                    return -1;
                }

                if (strHora.Contains(":") == false)
                {
                    return -2;
                }

                ParteHorario = strHora.Split(":");

                if (ParteHorario.Count() != 2)
                {
                    return -3;
                }

                ret = (int.Parse(ParteHorario[0]) * 60) + int.Parse(ParteHorario[1]);

                return ret;

            }
            catch (Exception ex)
            {
                Console.WriteLine("HoraParaInt: " + ex.Message);
                return -1;
            }

        }

    }
}
