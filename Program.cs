using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;

namespace csv_double_quote_deescaper
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length < 3)
            {
                Console.Error.WriteLine("[ERROR] Expected: [input] [output] [Encoding e.g. 1252]");
                return;
            }

            int encoding = Convert.ToInt32(args[2]);
            DirectoryCopy(args[0], args[1], true);
            DirReplace(args[1], encoding);
            // string _ = Console.ReadLine();
        }

        static void DirReplace(string sDir, int encoding)
        {
            if (!File.GetAttributes(sDir).HasFlag(FileAttributes.Directory)) // checks if its a file
            {
                FileReplace(sDir, encoding);
            }
            else
            {
                foreach (string f in Directory.GetFiles(sDir))
                {
                    FileReplace(f, encoding);
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    DirReplace(d, encoding);
                }
            }
        }

        static void FileReplace(string sDir, int encoding)
        {
            Encoding strEnc = Encoding.GetEncoding(encoding);
            String fullText = File.ReadAllText(sDir, strEnc);
            char[] fullTextArray = fullText.ToCharArray();

            Console.WriteLine("Started: {0}", sDir);
            for (Match cell = Regex.Match(fullText, @"([^;]+|\;);"); cell.Success; cell = cell.NextMatch()) // segments each cell in csv
            {
                if (cell.ToString().Length >= 3)
                {
                    char[] matchStr = cell.ToString().Substring(1, cell.ToString().Length - 3).ToCharArray();

                    for (int i = 0; i < matchStr.Length - 1; i++)
                    {
                        if (matchStr.Length == 0)
                        {
                            break;
                        }

                        if (matchStr[i] == '\"' && matchStr[i + 1] == '\"')
                        {
                            fullTextArray[i + cell.Index + 1] = '\'';
                            fullTextArray[i + cell.Index + 2] = '\'';
                            matchStr[i] = '\'';
                            matchStr[i + 1] = '\'';
                            String cellStr = new String(matchStr);
                            Console.WriteLine($" {cellStr} : {sDir}");
                        }
                    }
                }
            }
            System.IO.File.WriteAllText(sDir, new string(fullTextArray), strEnc);
            // Console.WriteLine("Done:   {0}", sDir);

        }
        static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!File.GetAttributes(sourceDirName).HasFlag(FileAttributes.Directory))
            {
                FileInfo file = new FileInfo(sourceDirName);
                file.CopyTo(destDirName);
                return;
            }

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }


            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

    }
}
