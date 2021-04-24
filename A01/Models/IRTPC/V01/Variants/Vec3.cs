namespace A01.Models.IRTPC.V01.Variants
{
    public class Vec3 : FloatArrayVariant
    {
        /// <summary>
        /// Empty constructor for XML parsing.
        /// <see cref="Vec3"></see>
        /// </summary>
        public Vec3()
        {
            
        }
        
        public Vec3(Property prop) : base(prop)
        {
            NUM = 3;
            VariantType = EVariantType.Vec3;
        }
    }
}