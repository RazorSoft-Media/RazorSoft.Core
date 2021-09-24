// Copyright: ©2021 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;
using System.IO;
using System.Linq;
using System.Text;


namespace RazorSoft.Core.IO {

    /// <summary>
    /// 
    /// </summary>
    public class BinaryStream : Stream {
        #region		fields
        private Stream memory;
        #endregion	fields


        #region		properties
        /// <summary>
        /// 
        /// </summary>
        public Encoding Encoder => Encoding.UTF8;
        /// <summary>
        /// 
        /// </summary>
        public override bool CanRead { get; } = false;
        /// <summary>
        /// 
        /// </summary>
        public override bool CanSeek => true;
        /// <summary>
        /// 
        /// </summary>
        public override bool CanWrite { get; } = false;
        /// <summary>
        /// 
        /// </summary>
        public override long Length { get; }
        /// <summary>
        /// 
        /// </summary>
        public override long Position {
            get => memory.Position;
            set => memory.Seek(value, SeekOrigin.Begin);
        }

        #endregion	properties


        #region		constructors & destructors
        /// <summary>
        /// 
        /// </summary>
        public BinaryStream(AccessMode mode) : this(new MemoryStream(), mode) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="mode"></param>
        public BinaryStream(Stream stream, AccessMode mode) : base() {
            CanRead = (mode & AccessMode.Read) == AccessMode.Read;
            CanWrite = (mode & AccessMode.Write) == AccessMode.Write;
            memory = stream;
        }

        #endregion	constructors & destructors


        #region		public methods & functions
        /// <summary>
        /// 
        /// </summary>
        public override void Flush() {
            memory.Flush();
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count) {
            return memory.Read(buffer, offset, count);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public byte[] Read(int count) {
            var buffer = new byte[count];
            Read(buffer, 0, count);

            return buffer;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public BinaryReader Read() {
            return new(memory, Encoder, true);
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin) {
            return memory.Seek(offset, origin);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        public override void SetLength(long count) {
            memory.SetLength(count);
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count) {
            memory.Write(buffer, offset, count);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public BinaryWriter Write() {
            return new(memory, Encoder, true);
        }
        #endregion	public methods & functions


        #region		non-public methods & functions
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                memory.Close();
                memory.Dispose();
            }

            base.Dispose(disposing);
        }
        #endregion	non-public methods & functions
    }

    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum AccessMode {
        /// <summary>
        /// 
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// 
        /// </summary>
        Read = 1,
        /// <summary>
        /// 
        /// </summary>
        Write = 2
    }
}
