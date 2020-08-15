/* ***********************************************
 *  © 2020 RazorSoft Media, DBA
 *         Lone Star Logistics & Transport, LLC. All Rights Reserved
 *         David Boarman
 * ***********************************************/


using System;


namespace RazorSoft.Core {

    public struct Uuid {
        private readonly byte[] uuid;


        #region properties
        public DateTime TimeStamp {
            get { return GetTimeStamp(this); }
        }
        #endregion


        #region constructor
        private Uuid(int length) {
            uuid = new byte[length];
        }

        public Uuid(byte[] bytes) {
            uuid = bytes;
        }

        public Uuid(DateTime TimeStamp) {
            var guid = Guid.NewGuid( ).ToByteArray( );
            uuid = new byte[guid.Length];

            BuildUuid(guid, this, TimeStamp.Ticks);
        }
        #endregion


        #region methods
        public static Uuid NewUuid( ) {
            var guid = Guid.NewGuid( ).ToByteArray( );
            var uuid = new Uuid(guid.Length);

            BuildUuid(guid, uuid, DateTime.Now.Ticks);

            return uuid;
        }

        public static implicit operator Guid(Uuid uuid) {
            if (uuid.uuid == null) {
                var guid = Guid.Empty.ToByteArray( );
                uuid = new Uuid(guid.Length);

                BuildUuid(guid, uuid, default(DateTime).Ticks);
            }

            return new Guid(uuid.uuid);
        }

        public static implicit operator Uuid(Guid guid) {
            return new Uuid(guid.ToByteArray( ));
        }

        public static bool operator ==(Uuid uid1, Uuid uid2) {
            return uid1.GetHashCode( ) == uid2.GetHashCode( );
        }

        public static bool operator !=(Uuid uid1, Uuid uid2) {
            return !(uid1 == uid2);
        }

        public override int GetHashCode( ) {
            Guid guid = this;
            return guid.GetHashCode();
        }

        public override bool Equals(object obj) {
            if (!(obj is Uuid)) {
                return false;
            }

            var other = (Uuid)obj;

            return this == other;
        }

        public override string ToString( ) {
            Guid guid = this;
            return guid.ToString( );
        }

        private static void BuildUuid(byte[ ] guid, Uuid uuid, long timestamp) {
            var time = BitConverter.GetBytes(timestamp);
            var start = guid.Length - time.Length;

            Buffer.BlockCopy(time, 0, guid, start, time.Length);
            Buffer.BlockCopy(guid, 0, uuid.uuid, 0, guid.Length);
        }

        private static DateTime GetTimeStamp(Uuid uuid) {
            return new DateTime(BitConverter.ToInt64(uuid.uuid, 8));
        }
        #endregion
    }
}
