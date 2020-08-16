//	* *************************************************************************
//	*  © 2020      RazorSoft Media, DBA                                       *
//	*              Lone Star Logistics & Transport, LLC.                      *
//	*              All Rights Reserved                                        *
//	* *************************************************************************


using System;


namespace RazorSoft.Core {

    /// <summary>
    /// Uuid - Unique Universal Identifier
    /// Integrates a date/time stamp as part of the identifier. Use should be restricted 
    /// to ensure validity of Uuid time stamps.
    /// </summary>
    public struct Uuid {
        private readonly byte[] uuid;


        #region properties
        /// <summary>
        /// Get the time stamp from the identifier
        /// </summary>
        public DateTime TimeStamp {
            get { return GetTimeStamp(this); }
        }
        #endregion


        #region constructors
        /// <summary>
        /// Private ctor
        /// </summary>
        /// <param name="length">buffer length</param>
        private Uuid(int length) {
            uuid = new byte[length];
        }
        /// <summary>
        /// From supplied buffer
        /// </summary>
        /// <param name="bytes">supplied buffer</param>
        public Uuid(byte[] bytes) {
            uuid = bytes;
        }
        /// <summary>
        /// From supplied time stamp
        /// </summary>
        /// <param name="TimeStamp">supplied time stamp</param>
        public Uuid(DateTime TimeStamp) {
            var guid = Guid.NewGuid( ).ToByteArray( );
            uuid = new byte[guid.Length];

            BuildUuid(guid, this, TimeStamp.Ticks);
        }
        #endregion


        #region methods
        /// <summary>
        /// Create a new Uuid
        /// </summary>
        /// <returns>(Uuid) uuid</returns>
        public static Uuid NewUuid( ) {
            var guid = Guid.NewGuid( ).ToByteArray( );
            var uuid = new Uuid(guid.Length);

            BuildUuid(guid, uuid, DateTime.Now.Ticks);

            return uuid;
        }


        /// <summary>
        /// Implicitly casts the supplied Uuid as a Guid
        /// </summary>
        /// <param name="uuid">supplied Uuid</param>
        /// <returns>(Guid) guid</returns>
        public static implicit operator Guid(Uuid uuid) {
            if (uuid.uuid == null) {
                var guid = Guid.Empty.ToByteArray( );
                uuid = new Uuid(guid.Length);

                BuildUuid(guid, uuid, default(DateTime).Ticks);
            }

            return new Guid(uuid.uuid);
        }
        /// <summary>
        /// Implicitly casts the supplied Guid as a Uuid.
        /// This could cause an invalid Uuid. The Uuid must be checked to ensure a valid 
        /// data/time stamp is embedded.
        /// </summary>
        /// <param name="guid">supplied Guid</param>
        /// <returns>(Uuid) uuid</returns>
        public static implicit operator Uuid(Guid guid) {
            return new Uuid(guid.ToByteArray( ));
        }
        /// <summary>
        /// Checks equality of 2 supplied Uuid operands
        /// </summary>
        /// <param name="uid1">op1: supplied Uuid</param>
        /// <param name="uid2">op2: supplied Uuid</param>
        /// <returns>(bool) TRUE if equal; otherwise FALSE</returns>
        public static bool operator ==(Uuid uid1, Uuid uid2) {
            return uid1.GetHashCode( ) == uid2.GetHashCode( );
        }
        /// <summary>
        /// Checks equality of 2 supplied Uuid operands
        /// </summary>
        /// <param name="uid1">op1: supplied Uuid</param>
        /// <param name="uid2">op2: supplied Uuid</param>
        /// <returns>(bool) FALSE if not equal; otherwise TRUE</returns>
        public static bool operator !=(Uuid uid1, Uuid uid2) {
            return !(uid1 == uid2);
        }
        /// <summary>
        /// Returns the hashcode for this instance
        /// </summary>
        /// <returns>(int) the hashcode for this instance</returns>
        public override int GetHashCode( ) {
            Guid guid = this;
            return guid.GetHashCode();
        }
        /// <summary>
        /// Determines equality of this instance against a supplied object
        /// </summary>
        /// <param name="obj">supplied object</param>
        /// <returns>(bool) equality of this instance and another</returns>
        public override bool Equals(object obj) {
            if (!(obj is Uuid)) {
                return false;
            }

            var other = (Uuid)obj;

            return this == other;
        }
        /// <summary>
        /// Returns string representation of this instance.
        /// </summary>
        /// <returns>(string) guid.ToString()</returns>
        public override string ToString( ) {
            Guid guid = this;
            return guid.ToString( );
        }


        /// <summary>
        /// Builds the Uuid from the supplied buffer, Uuid and time stamp
        /// </summary>
        /// <param name="guid">base guid</param>
        /// <param name="uuid">initial uuid</param>
        /// <param name="timestamp">time stamp</param>
        private static void BuildUuid(byte[ ] guid, Uuid uuid, long timestamp) {
            var time = BitConverter.GetBytes(timestamp);
            var start = guid.Length - time.Length;

            Buffer.BlockCopy(time, 0, guid, start, time.Length);
            Buffer.BlockCopy(guid, 0, uuid.uuid, 0, guid.Length);
        }
        /// <summary>
        /// Retrieves the encoded time stamp from the current Uuid instance
        /// </summary>
        /// <param name="uuid">uuid</param>
        /// <returns>(DateTime) time stamp</returns>
        private static DateTime GetTimeStamp(Uuid uuid) {
            return new DateTime(BitConverter.ToInt64(uuid.uuid, 8));
        }
        #endregion
    }
}
