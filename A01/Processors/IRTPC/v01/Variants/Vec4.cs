using System.Collections.Generic;
using System.IO;

namespace A01.Processors.IRTPC.v01.Variants
{
    public class Vec4 : FloatArrayVariant
    {
        public Vec4(Property prop) : base(prop)
        {
            NUM = 4;
            VariantType = EVariantType.Vec4;
        }
    }
}