using System.Collections.Generic;
using System.IO;

namespace A01.Processors.IRTPC.v01.Variants
{
    public class Vec2 : FloatArrayVariant
    {
        public Vec2(Property prop) : base(prop)
        {
            NUM = 2;
            VariantType = EVariantType.Vec2;
        }
    }
}