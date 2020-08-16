//	* *************************************************************************
//	*  © 2020      RazorSoft Media, DBA                                       *
//	*              Lone Star Logistics & Transport, LLC.                      *
//	*              All Rights Reserved                                        *
//	* *************************************************************************


using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;


namespace RazorSoft.Core.Extensions {

    public static class Extensions {
        /// <summary>
        /// Determines if a given DateTime is valid: ie, greater than the
        /// minimum data value
        /// </summary>
        /// <param name="dateTime">supplied DateTime value</param>
        /// <returns>(bool) TRUE if given DateTime is greater than the DateTime.MinValue constant.</returns>
        public static bool IsValid(this DateTime dateTime) {
            return dateTime > DateTime.MinValue;
        }
        /// <summary>
        /// Combines directory paths using DirectoryInfo object as the target
        /// </summary>
        /// <param name="root">The root directory</param>
        /// <param name="path">The first sub-directory</param>
        /// <param name="paths">Additional sub-directories</param>
        /// <returns>(DirectoryInfo) directory object with complete path structure</returns>
        public static DirectoryInfo CombinePaths(this DirectoryInfo root, string path, params string[] paths) {
            var subDirs = new string[paths.Length + 2];
            subDirs[0] = root.FullName;
            subDirs[1] = path;
            Array.Copy(paths, 0, subDirs, 2, paths.Length);

            return new DirectoryInfo(Path.Combine(subDirs));
        }
        /// <summary>
        /// Get the size of a file from the specified file info object.
        /// </summary>
        /// <param name="fileInfo">The given file info object.</param>
        /// <returns>(long) Size of the file; -1 if the file is not found.</returns>
        public static long Size(this FileInfo fileInfo) {
            var size = -1L;

            if (fileInfo.Exists) {
                using (var file = fileInfo.OpenRead()) {
                    file.Seek(0, SeekOrigin.Begin);
                    size = file.Length;
                }
            }

            return size;
        }
        /// <summary>
        /// Creates entire path structure from given DirectoryInfo
        /// </summary>
        /// <param name="directory">supplied DirectoryInfo</param>
        /// <returns>(string) Full string path structure if success; otherwise, empty string</returns>
        public static string CreatePath(this DirectoryInfo directory) {
            var stack = new Stack<string>();
            var parent = directory;

            //  stack directories that don't exist
            while (!parent.Exists) {
                stack.Push(parent.Name);

                parent = parent.Parent;
            }

            directory = parent;

            //  now create path structure
            while (stack.Count > 0) {
                directory = directory.CombinePaths(stack.Pop());
                directory.Create();
            }

            if (directory.Exists) {
                return directory.FullName;
            } else {
                return string.Empty;
            }
        }
        /// <summary>
        /// Formats given DateTime: MMM-dd-yyyy HH:mm:ss
        /// </summary>
        /// <param name="time">supplied DateTime value</param>
        /// <returns>(string) "MMM-dd-yyyy HH:mm:ss"</returns>
        public static string AsStandard(this DateTime time) {
            return $"{time:MMM-dd-yyyy HH:mm:ss}";
        }
        /// <summary>
        /// Formats given DateTime: yyyy MMMM dd
        /// </summary>
        /// <param name="time">supplied DateTime value</param>
        /// <returns>(string) "yyyy MMMM dd"</returns>
        public static string AsStandardDate(this DateTime time) {
            return $"{time:yyyy MMMM dd}";
        }
        /// <summary>
        /// Formats given DateTime: MMyyddHHmmss
        /// </summary>
        /// <param name="time">supplied DateTime value</param>
        /// <param name="ext">file name extension</param>
        /// <returns>(string) "MMyyddHHmmss.{ext}"</returns>
        public static string AsFileName(this DateTime time, string ext) {
            return $"{time:MMyyddHHmmss}.{ext}";
        }
        /// <summary>
        /// Formats given DateTime: Log Header: yyyy, MMMM dd
        /// </summary>
        /// <param name="time">supplied DateTime value</param>
        /// <returns>(string) "Log Header: yyyy, MMMM dd"</returns>
        public static string AsLogHeader(this DateTime time) {
            return time.AsLogEntry($"Log Header: {time.AsStandardDate()}");
        }
        /// <summary>
        /// Formats given DateTime: HH:mm:ssss:    {entry}
        /// </summary>
        /// <param name="time">supplied DateTime value</param>
        /// <returns>(string) "HH:mm:ssss:    {entry}"</returns>
        public static string AsLogEntry(this DateTime time, string entry) {
            return $"{time:HH:mm:sss}:    {entry}";
        }
        /// <summary>
        /// Encodes target value to buffer
        /// </summary>
        /// <typeparam name="TValue">The specified type parameter</typeparam>
        /// <param name="value">target value</param>
        /// <returns>(byte[]) buffer</returns>
        public static byte[] Encode<TValue>(this TValue value) {
            var typeCode = Type.GetTypeCode(typeof(TValue));
            var v = (object)value;
            var buffer = default(byte[]);

            switch (typeCode) {
                case TypeCode.Boolean:
                    buffer = BitConverter.GetBytes((bool)v);
                    break;
                case TypeCode.Char:
                    buffer = BitConverter.GetBytes((char)v);
                    break;
                case TypeCode.SByte:
                    buffer = BitConverter.GetBytes((sbyte)v);
                    break;
                case TypeCode.Byte:
                    buffer = new byte[] { (byte)v };
                    break;
                case TypeCode.Int16:
                    buffer = BitConverter.GetBytes((short)v);
                    break;
                case TypeCode.UInt16:
                    buffer = BitConverter.GetBytes((ushort)v);
                    break;
                case TypeCode.Int32:
                    buffer = BitConverter.GetBytes((int)v);
                    break;
                case TypeCode.UInt32:
                    buffer = BitConverter.GetBytes((uint)v);
                    break;
                case TypeCode.Int64:
                    buffer = BitConverter.GetBytes((long)v);
                    break;
                case TypeCode.UInt64:
                    buffer = BitConverter.GetBytes((ulong)v);
                    break;
                case TypeCode.Single:
                    buffer = BitConverter.GetBytes((float)v);
                    break;
                case TypeCode.Double:
                    buffer = BitConverter.GetBytes((double)v);
                    break;
                case TypeCode.Decimal:
                    var ints = decimal.GetBits((decimal)v);
                    buffer = ints
                        .Select(i => (byte)i)
                        .ToArray();
                    break;
                case TypeCode.DateTime:
                    buffer = BitConverter.GetBytes(((DateTime)v).Ticks);
                    break;
                case TypeCode.String:
                    buffer = Encoding.UTF8.GetBytes((string)v);
                    break;
                case TypeCode.Empty:
                case TypeCode.Object:
                case TypeCode.DBNull:
                default:
                    throw new InvalidOperationException("Not Supported");
            }

            return buffer;
        }
        /// <summary>
        /// Decodes target buffer to specified type
        /// </summary>
        /// <typeparam name="TValue">The specified type parameter</typeparam>
        /// <param name="buffer">target byte[] buffer</param>
        /// <returns>(TValue) value</returns>
        public static TValue DecodeAs<TValue>(this byte[] buffer) {
            var typeCode = Type.GetTypeCode(typeof(TValue));
            var value = default(object);

            switch (typeCode) {
                case TypeCode.Boolean:
                    value = BitConverter.ToBoolean(buffer);
                    break;
                case TypeCode.Char:
                    value = BitConverter.ToChar(buffer);
                    break;
                case TypeCode.SByte:
                    value = (sbyte)buffer[0];
                    break;
                case TypeCode.Byte:
                    value = buffer[0];
                    break;
                case TypeCode.Int16:
                    value = BitConverter.ToInt16(buffer);
                    break;
                case TypeCode.UInt16:
                    value = BitConverter.ToUInt16(buffer);
                    break;
                case TypeCode.Int32:
                    value = BitConverter.ToInt32(buffer);
                    break;
                case TypeCode.UInt32:
                    value = BitConverter.ToUInt32(buffer);
                    break;
                case TypeCode.Int64:
                    value = BitConverter.ToInt64(buffer);
                    break;
                case TypeCode.UInt64:
                    value = BitConverter.ToUInt64(buffer);
                    break;
                case TypeCode.Single:
                    value = BitConverter.ToSingle(buffer);
                    break;
                case TypeCode.Decimal:
                case TypeCode.Double:
                    value = BitConverter.ToDouble(buffer);
                    break;
                case TypeCode.DateTime:
                    var ticks = buffer.DecodeAs<long>();
                    value = DateTime.FromBinary(ticks);
                    break;
                case TypeCode.String:
                    value = Encoding.UTF8.GetString(buffer);
                    break;
                case TypeCode.Empty:
                case TypeCode.Object:
                case TypeCode.DBNull:
                default:
                    break;
            }

            return (TValue)value;
        }
    }
}
