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

            if (args.Length != 2)
            {
                Console.Error.WriteLine("[ERROR] Expected: [input] [output]");
                return;
            }

            DirectoryCopy(args[0], args[1], true);
            DirReplace(args[1]);
            string _ = Console.ReadLine();
        }

        static void DirReplace(string sDir)
        {
            if (!File.GetAttributes(sDir).HasFlag(FileAttributes.Directory)) // checks if its a file
            {
                FileReplace(sDir);
            }
            else
            {
                foreach (string f in Directory.GetFiles(sDir))
                {
                    FileReplace(f);
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    DirReplace(d);
                }
            }
        }

        static void FileReplace(string sDir)
        {
            
            String fullText = File.ReadAllText(sDir);
            char[] fullTextArray = fullText.ToCharArray();
            
            for (Match cell = Regex.Match(fullText, @"([^;]+|\;);"); cell.Success; cell = cell.NextMatch()) // segments each cell in csv
            {
                char[] matchStr = cell.ToString().Substring(1, cell.ToString().Length - 3).ToCharArray();
                
                for (int i = 0; i < matchStr.Length - 1; i++)
                {
                    if (matchStr.Length == 0) {
                        break;
                    }

                    if (matchStr[i] == '\"' && matchStr[i+1] == '\"')
                    {
                        fullTextArray[i + cell.Index + 1] = '\'';
                        fullTextArray[i + cell.Index + 2] = '\'';
                        matchStr[i ] = '\'';
                        matchStr[i + 1] = '\'';
                        String cellStr = new String(matchStr);
                        Console.WriteLine($" {cellStr} : {sDir}");
                    }
                }

            }
            System.IO.File.WriteAllText(sDir, new string(fullTextArray), Encoding.UTF8);
            Console.WriteLine("done");
            
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
