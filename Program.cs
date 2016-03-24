using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Libg.soPatch
{
  class Patcher
  {
    static void Main(string[] args)
    {
      string fileName = args[0].ToString();
      byte[] fileBytes = File.ReadAllBytes(fileName);
      byte[] searchPattern = new byte[] { 0x15, 0x0c, 0x52, 0xdb, 0x12, 0xba, 
                                          0x1c, 0x9d, 0xd8, 0x09, 0xb8, 0x93, 
                                          0x4a, 0x53, 0x5f, 0x42, 0x8a, 0x91, 
                                          0xb7, 0xb6, 0x1e, 0x15, 0xab, 0x46,
                                          0x9e, 0x42, 0xb9, 0x61, 0x4c, 0x76, 
                                          0xa3, 0x25};
      byte[] replacePattern = new byte[] { 0x72, 0xF1, 0xA4, 0xA4, 0xC4, 0x8E, 0x44, 0xDA,
                                           0x0C, 0x42, 0x31, 0x0F, 0x80, 0x0E, 0x96, 0x62,
                                           0x4E, 0x6D, 0xC6, 0xA6, 0x41, 0xA9, 0xD4, 0x1C,
                                           0x3B, 0x50, 0x39, 0xD8, 0xDF, 0xAD, 0xC2, 0x7E};

      //Search
      IEnumerable<int> positions = FindPattern(fileBytes, searchPattern);
      if (positions.Count() == 0)
      {
        Console.WriteLine("Pattern not found.");
        Console.Read();
        return;
      }

      //Backup
      string backupFileName = fileName + ".bak";
      File.Copy(fileName, backupFileName);
      Console.WriteLine("Backup file: {0} -> {1}", fileName, backupFileName);

      foreach (int pos in positions)
      {
        //Replace
        Console.WriteLine("Key offset: 0x{0}", pos.ToString("X8"));
        using (BinaryWriter bw = new BinaryWriter(File.Open(fileName, FileMode.Open, FileAccess.Write)))
        {
          bw.BaseStream.Seek(pos, SeekOrigin.Begin);
          bw.Write(replacePattern);
        }
        Console.WriteLine("File: {0} patched", fileName);
      }
      Console.Read();
    }

    public static IEnumerable<int> FindPattern(byte[] fileBytes, byte[] searchPattern)
    {
      if ((searchPattern != null) && (fileBytes.Length >= searchPattern.Length))
        for (int i = 0; i < fileBytes.Length - searchPattern.Length + 1; i++)
          if (!searchPattern.Where((data, index) => !fileBytes[i + index].Equals(data)).Any())
            yield return i;
    }
  }
}
