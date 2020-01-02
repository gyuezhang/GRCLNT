using GRModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRCLNT
{
    public class C_RT
    {
        public static C_User user { get; set; } = new C_User();
        public static List<C_AreaCode> acs { get; set; } = new List<C_AreaCode>();
        public static C_WellParas wp { get; set; } = new C_WellParas();
        public static C_WellParas ewp { get; set; } = new C_WellParas();
    }
}
