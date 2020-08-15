//	* ********************************************************************
//	*  © 2020 RazorSoft Media, DBA                                       *
//	*         Lone Star Logistics & Transport, LLC. All Rights Reserved  *
//	*         David Boarman                                              *
//	* ********************************************************************


using System;
using System.IO;
using System.Text;


namespace RazorSoft.Core.Modules {

    public abstract class Scribe {

        public Encoding Encoder { get; protected set; } = Encoding.UTF8;

        protected Scribe() { }

        public virtual void Write(string entry) {
            byte[] buffer = GetBuffer(entry);

            using (var stream = RequestStream()) {
                stream.Write(buffer, 0, buffer.Length);
                NotifyWrite(buffer);
            }
        }
        protected virtual void NotifyWrite(byte[] buffer) { }
        protected virtual byte[] GetBuffer(string entry) {
            return Encoder.GetBytes(entry);
        }
        protected virtual Stream RequestStream() {
            return new MemoryStream();
        }
    }
}
