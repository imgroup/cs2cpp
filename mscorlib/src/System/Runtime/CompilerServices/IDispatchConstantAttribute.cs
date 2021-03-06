// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
[Serializable]
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter, Inherited=false)]
[System.Runtime.InteropServices.ComVisible(true)]
    public sealed class IDispatchConstantAttribute : CustomConstantAttribute
    {
        public IDispatchConstantAttribute()
        {
        }

        public override Object Value
        {
            get 
            {
                return new DispatchWrapper(null);
            }
        }
    }
}
