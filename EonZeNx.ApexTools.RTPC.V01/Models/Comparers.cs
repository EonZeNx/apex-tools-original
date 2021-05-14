using System;
using System.Collections.Generic;
using EonZeNx.ApexTools.RTPC.V01.Models.Variants;

namespace EonZeNx.ApexTools.RTPC.V01.Models.Comparer
{
    public class PropertyComparer : Comparer<IPropertyVariants>
    {
        public override int Compare(IPropertyVariants x, IPropertyVariants y)
        {
            if (x.Name.Length == 0 && y.Name.Length == 0)
            {
                return string.Compare(x.HexNameHash, y.HexNameHash, StringComparison.Ordinal);
            }
            if (x.Name.Length == 0) return 1;
            if (y.Name.Length == 0) return -1;

            return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
        }
    }
    
    public class ContainerComparer : Comparer<Container>
    {
        public override int Compare(Container x, Container y)
        {
            if (x.Name.Length == 0 && y.Name.Length == 0)
            {
                return string.Compare(x.HexNameHash, y.HexNameHash, StringComparison.Ordinal);
            }
            if (x.Name.Length == 0) return 1;
            if (y.Name.Length == 0) return -1;

            return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
        }
    }
}