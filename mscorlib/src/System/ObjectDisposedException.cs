// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System {
    using System;
    using System.Runtime.Serialization;
    using System.Globalization;
    using System.Security.Permissions;

    /// <devdoc>
    ///    <para> The exception that is thrown when accessing an object that was
    ///       disposed.</para>
    /// </devdoc>
[System.Runtime.InteropServices.ComVisible(true)]
    [Serializable]
    public class ObjectDisposedException : InvalidOperationException {
        private String objectName;

        // This constructor should only be called by the EE (COMPlusThrow)
        private ObjectDisposedException() : 
            this(null ,Environment.GetResourceString("ObjectDisposed_Generic")) {
        }

        public ObjectDisposedException(String objectName) : 
            this(objectName, Environment.GetResourceString("ObjectDisposed_Generic")) {
        }

        public ObjectDisposedException(String objectName, String message) : base(message) {
            SetErrorCode(__HResults.COR_E_OBJECTDISPOSED);
            this.objectName = objectName;
        }

        public ObjectDisposedException(String message, Exception innerException) 
            : base(message, innerException) {
            SetErrorCode(__HResults.COR_E_OBJECTDISPOSED);
        }

        /// <devdoc>
        ///    <para>Gets the text for the message for this exception.</para>
        /// </devdoc>
        public override String Message {
            get {
                String name = ObjectName;
                if (name == null || name.Length == 0)
                    return base.Message;

                String objectDisposed = Environment.GetResourceString("ObjectDisposed_ObjectName_Name", name);
                return base.Message + Environment.NewLine + objectDisposed;
            }
        }

        public String ObjectName {
            get {
                if ((objectName == null) && !CompatibilitySwitches.IsAppEarlierThanWindowsPhone8)
                {
                    return String.Empty;
                }
                return objectName; 
            }
        }

        protected ObjectDisposedException(SerializationInfo info, StreamingContext context) : base(info, context) {
            objectName = info.GetString("ObjectName");
        }

        [System.Security.SecurityCritical]  // auto-generated_required
        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
            info.AddValue("ObjectName",ObjectName,typeof(String));
        }

    }
}
