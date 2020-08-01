using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillingModule.Primitives
{
    public class Obstruction
    {
        public float Xc { get; set; }
        public float Yc { get; set; }
        public float R { get; set; }

        public bool IsEmpty()
        {
            if (Xc == 0 && Yc == 0 && R == 0)
                return true;
            else
                return false;
        }
    }
}
