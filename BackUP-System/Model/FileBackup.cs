using System;
using System.IO;
using System.IO.Compression;

namespace BackUP_System.Model
{
    public class FileBackup
    {
        public string fromDirectory { get; set; }
        private string baseToDirectory; 

        public FileBackup(string fromDirectory, string baseToDirectory)
        {
            this.fromDirectory = fromDirectory;
            this.baseToDirectory = baseToDirectory;
        }

        public string PerformBackup(bool shouldCompress)
        {
            try
            {
                if (!Directory.Exists(fromDirectory))
                {
                    return "Source directory does not exist.";
                }

                string toDirectory = baseToDirectory;
                
                if (!Directory.Exists(toDirectory))
                {
                    Directory.CreateDirectory(toDirectory);
                }

                CopyFilesRecursively(fromDirectory, toDirectory);

                if (shouldCompress)
                {
                    string zipFilePath = toDirectory + ".zip";
                    CompressToZip(toDirectory, zipFilePath);

                    // Delete the original uncompressed backup after creating the zip file
                    DeleteDirectory(toDirectory);

                    return "Backup and compression completed successfully. ZIP file created at: " + zipFilePath;
                }

                return "Backup completed successfully.";
            }
            catch (Exception ex)
            {
                return $"An error occurred: {ex.Message}";
            }
        }

        private void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            foreach (string filePath in Directory.GetFiles(sourcePath))
            {
                string fileName = Path.GetFileName(filePath);
                string destFile = Path.Combine(targetPath, fileName);
                File.Copy(filePath, destFile, true);
            }

            foreach (string directoryPath in Directory.GetDirectories(sourcePath))
            {
                string directoryName = Path.GetFileName(directoryPath);
                string destDirectory = Path.Combine(targetPath, directoryName);
                if (!Directory.Exists(destDirectory))
                {
                    Directory.CreateDirectory(destDirectory);
                }
                CopyFilesRecursively(directoryPath, destDirectory);
            }
        }

        private void CompressToZip(string sourceDirectory, string zipFilePath)
        {
            if (File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }
            ZipFile.CreateFromDirectory(sourceDirectory, zipFilePath);
        }
        

        private void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true); 
            }
        }
    }
}
