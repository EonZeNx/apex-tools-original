namespace A01.Models.IRTPC.V01.Variants
{
    public class Vec2 : FloatArrayVariant
    {
        /// <summary>
        /// Empty constructor for XML parsing.
        /// <see cref="Vec2"></see>
        /// </summary>
        public Vec2()
        {
            
        }
        
        public Vec2(Property prop) : base(prop)
        {
            NUM = 2;
            VariantType = EVariantType.Vec2;
        }
    }
}