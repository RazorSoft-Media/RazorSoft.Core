//	* *************************************************************************
//	*  © 2020      RazorSoft Media, DBA                                       *
//	*              Lone Star Logistics & Transport, LLC.                      *
//	*              All Rights Reserved                                        *
//	* *************************************************************************


using System.IO;
using System.Text;


namespace RazorSoft.Core.Modules {

    /// <summary>
    /// Abstract stream initializer and handler.
    /// </summary>
    public abstract class Scribe {


        /// <summary>
        /// Get the stream encoding
        /// </summary>
        public Encoding Encoder { get; protected set; } = Encoding.UTF8;


        /// <summary>
        /// Protected ctor
        /// </summary>
        protected Scribe() { }


        /// <summary>
        /// Writes the supplied entry to the underlying stream
        /// </summary>
        /// <param name="entry">supplied entry</param>
        public virtual void Write(string entry) {
            byte[] buffer = Encode(entry);

            using (var stream = RequestStream()) {
                stream.Write(buffer, 0, buffer.Length);
                NotifyWrite(buffer);
            }
        }


        /// <summary>
        /// Protected
        /// Notifies write is complete, supplying the target buffer
        /// </summary>
        /// <param name="buffer">byte[] (stream)</param>
        protected virtual void NotifyWrite(byte[] buffer) { }
        /// <summary>
        /// Protected
        /// Encodes the supplied entry to a byte buffer using the specified encoding
        /// </summary>
        /// <param name="entry">supplied entry</param>
        /// <returns>byte buffer</returns>
        protected virtual byte[] Encode(string entry) {
            return Encoder.GetBytes(entry);
        }
        /// <summary>
        /// Returns a Stream object.
        /// Default MemoryStream unleass inherited class  modifies.
        /// </summary>
        /// <returns>Stream</returns>
        protected virtual Stream RequestStream() {
            return new MemoryStream();
        }
    }
}
