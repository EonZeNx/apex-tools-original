using System.Collections.Generic;
using System.IO;

namespace A01.Processors.IRTPC.v01.Variants
{
    public class Mat3X4 : FloatArrayVariant
    {
        public Mat3X4(Property prop) : base(prop)
        {
            NUM = 12;
            VariantType = EVariantType.Mat3X4;
        }
    }
}