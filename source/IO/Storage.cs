// Copyright: ©2021 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System;
using System.IO;


namespace RazorSoft.Core.IO {

    /// <summary>
    /// 
    /// </summary>
    public class Storage {
        #region		fields
        private readonly DirectoryInfo directory;
        #endregion	fields


        #region		properties
        /// <summary>
        /// Get the storage file's full path
        /// </summary>
        public string FullPath { get; init; }

        #endregion	properties


        #region		constructors & destructors
        /// <summary>
        /// 
        /// </summary>
        private Storage(string filePath) {
            directory = new DirectoryInfo(filePath).Parent;
            FullPath = Path.GetFullPath(filePath);
        }
        #endregion	constructors & destructors


        #region		public methods & functions
        /// <summary>
        /// 
        /// </summary>
        /// <param name="access"></param>
        /// <returns></returns>
        public BinaryStream OpenBinary(AccessMode access) {
            BinaryStream binStream;

            switch (access) {
                case AccessMode.Unknown:
                    throw new StorageOperationException(FullPath);
                case AccessMode.Read:
                    binStream = new(File.Open(FullPath, FileMode.Open, FileAccess.Read), access);

                    break;
                case AccessMode.Write:
                    binStream = new(File.Open(FullPath, FileMode.Open, FileAccess.Write), access);

                    break;
                case AccessMode.Read | AccessMode.Write:
                default:
                    binStream = new(File.Open(FullPath, FileMode.Open, FileAccess.ReadWrite), access);

                    break;
            }

            return binStream;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="access"></param>
        /// <returns></returns>
        public FileStream OpenFile(AccessMode access) {
            FileStream fileStream;

            switch (access) {
                case AccessMode.Unknown:
                    throw new StorageOperationException(FullPath);
                case AccessMode.Read:
                    fileStream = File.Open(FullPath, FileMode.Open, FileAccess.Read);

                    break;
                case AccessMode.Write:
                    fileStream = File.Open(FullPath, FileMode.Open, FileAccess.Write);

                    break;
                case AccessMode.Read | AccessMode.Write:
                default:
                    fileStream = File.Open(FullPath, FileMode.Open, FileAccess.ReadWrite);

                    break;
            }

            return fileStream;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        public static Storage FromFileInfo(FileInfo fileInfo, bool create = false) {
            return FromPath(fileInfo.FullName, create);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        public static Storage FromPath(string filePath, bool create = false) {
            if (!IsFile(filePath, out bool exists)) {
                throw new StorageOperationException(filePath);
            }

            if (!create && !exists) {
                throw new StorageOperationException(filePath);
            }
            if (!exists && create) {
                Create(filePath);
            }

            return new Storage(filePath);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public static string Create(string filePath) {
            using (var stream = File.Create(filePath)) { }

            return filePath;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="exists"></param>
        /// <returns></returns>
        public static bool IsFile(string path, out bool exists) {
            var fileExists = File.Exists(path);
            var dirExists = Directory.Exists(path);

            //  make our best guess given the path endpoint does not exist
            if (!(exists = fileExists || dirExists)) {
                return !string.IsNullOrEmpty(Path.GetFileName(path));
            }

            // get the file attributes for file or directory
            FileAttributes pathAttribs = File.GetAttributes(path);

            //detect whether its a directory or file
            if ((pathAttribs & FileAttributes.Directory) == FileAttributes.Directory) {
                return false;
            }

            return true;
        }
        #endregion	public methods & functions


        #region		non-public methods & functions

        #endregion	non-public methods & functions
    }
}
