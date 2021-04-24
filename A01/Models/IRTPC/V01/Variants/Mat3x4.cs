namespace A01.Models.IRTPC.V01.Variants
{
    public class Mat3X4 : FloatArrayVariant
    {
        /// <summary>
        /// Empty constructor for XML parsing.
        /// <see cref="Mat3X4"></see>
        /// </summary>
        public Mat3X4()
        {
            
        }
        
        public Mat3X4(Property prop) : base(prop)
        {
            NUM = 12;
            VariantType = EVariantType.Mat3X4;
        }
    }
}