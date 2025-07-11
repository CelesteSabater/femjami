using System.IO;

namespace femjami.Utils.FileManagement
{
    public static class FileLoader
    {

        public static string[] GetFilesInDirectory(string dir)
        {
            string[] audioFiles = new string[0];

            if (dir == null)
                return audioFiles;

            audioFiles = Directory.GetFiles(dir);

            return audioFiles;
        }
    }
}