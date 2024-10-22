﻿using EonZeNx.ApexTools.Core.Models;

namespace EonZeNx.ApexTools.Core.Interfaces.Serializable
{
    public interface IBinaryClassIO : IBinarySerializable, IBinaryConvertedSerializable
    {
        MetaInfo GetMetaInfo();
    }
}