﻿using System;
//using System.Web;
using FTTLib;

//using Microsoft.Win32;


namespace Eu.EDelivery.AS4.Repositories
{
    /// <summary>
    /// Repository with Mime Type specific operations
    /// </summary>
    public class MimeTypeRepository 
    {
        //private const string RegistryPath = @"MIME\Database\Content Type\";

        public static MimeTypeRepository Instance = new MimeTypeRepository();

        private MimeTypeRepository() { }

        /// <summary>
        /// Retrieve the right Extension
        /// from a given MIME Content Type
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        public string GetExtensionFromMimeType(string mimeType)
        {
            if (mimeType == null)
            {
                throw new ArgumentNullException(nameof(mimeType));
            }

            //using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(RegistryPath + mimeType, writable: false))
            //{
            //    if (key == null)
            //    {
            //        return string.Empty;
            //    }

            //    object value = key.GetValue("Extension", defaultValue: string.Empty);
            //    return value.ToString();
            //}

            string[] extensions = FTT.GetMimeTypeFileExtensions(mimeType);
            return (extensions != null && extensions.Length > 0)
                ? ($".{extensions[0]}")
                : string.Empty;
        }

        /// <summary>
        /// Retrieve the right MimeType
        /// from a given Extension
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public string GetMimeTypeFromExtension(string extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException(nameof(extension));
            }

            //return MimeMapping.GetMimeMapping(extension);
            return FTT.GetMimeType($"file{extension}");   // ".txt" -> "file.txt"
        }
    }
}