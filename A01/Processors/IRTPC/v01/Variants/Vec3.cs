using System.Collections.Generic;
using System.IO;

namespace A01.Processors.IRTPC.v01.Variants
{
    public class Vec3 : FloatArrayVariant
    {
        public Vec3(Property prop) : base(prop)
        {
            NUM = 3;
            VariantType = EVariantType.Vec3;
        }
    }
}