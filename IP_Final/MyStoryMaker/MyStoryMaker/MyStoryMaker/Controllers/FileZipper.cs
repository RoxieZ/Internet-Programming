using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;

namespace MyStoryMaker.Controllers
{
    public class FileZipper
    {
        public static void ZipFiles(string[] files, string ZippedFileName)
        {
            foreach (string file in files)
                if (!File.Exists(file))
                    throw new FileNotFoundException("The file " + file + " does not exist.");
            ZipOutputStream z = new ZipOutputStream(File.Create(ZippedFileName));
            z.SetLevel(6);
            ZipFileDirectory(files, z);
            z.Finish();
            z.Close();
        }
        private static void ZipFileDirectory(string[] files, ZipOutputStream z)
        {
            FileStream fs = null;
            Crc32 crc = new Crc32();
            try
            {
                foreach(string file in files)
                {
                    fs = File.OpenRead(file);
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    string fileName = Path.GetFileName(file);
                    
                    ZipEntry entry = new ZipEntry(fileName);
                    entry.DateTime = DateTime.Now;
                    entry.Size = fs.Length;
                    fs.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    z.PutNextEntry(entry);
                    z.Write(buffer, 0, buffer.Length);
                }
            }
            finally
            {
                if(fs!=null)
                {
                    fs.Close();
                    fs = null;
                }
                GC.Collect();
            }
        }
    }
}