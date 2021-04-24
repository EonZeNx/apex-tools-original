namespace A01.Models.IRTPC.V01.Variants
{
    public class Vec4 : FloatArrayVariant
    {
        /// <summary>
        /// Empty constructor for XML parsing.
        /// <see cref="Vec4"></see>
        /// </summary>
        public Vec4()
        {
            
        }
        
        public Vec4(Property prop) : base(prop)
        {
            NUM = 4;
            VariantType = EVariantType.Vec4;
        }
    }
}