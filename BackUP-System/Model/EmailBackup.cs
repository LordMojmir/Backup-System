using System;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit;
using MimeKit;
using System.IO;
using MailKit.Security;

namespace EmailBackup
{
    public class ImapEmailBackup
    {
        /// <summary>
        /// Downloads emails from the specified IMAP server.
        /// </summary>
        /// <param name="host">The IMAP server host.</param>
        /// <param name="port">The port to connect to.</param>
        /// <param name="useSsl">Whether to use SSL for the connection.</param>
        /// <param name="username">The username for authentication.</param>
        /// <param name="password">The password for authentication.</param>
        /// <param name="backupFolderPath">The path to the folder where emails will be saved.</param>
        /// <exception cref="MailKit.Security.AuthenticationException">Thrown when authentication fails.</exception>
        public void DownloadEmails(string host, int port, bool useSsl, string username, string password, string backupFolderPath)
        {
            ClearFolderContents(backupFolderPath);
    
            using (var client = new ImapClient())
            {
                try
                {
                    client.Connect(host, port, useSsl);
                    client.Authenticate(username, password);

                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadOnly);

                    var uids = inbox.Search(SearchQuery.All);
                    foreach (var uid in uids)
                    {
                        var message = inbox.GetMessage(uid);
                        var subject = CleanSubject(message.Subject);
                        string emailFilePath = Path.Combine(backupFolderPath, subject + ".eml");

                        emailFilePath = GetUniqueFilePath(emailFilePath);

                        using (var fileStream = File.Create(emailFilePath))
                        {
                            message.WriteTo(fileStream);
                        }
                    }

                    client.Disconnect(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while downloading emails.");
                    throw new InvalidOperationException("Failed to download emails. Please verify your email credentials and network connection, then try again.", ex);
                }
            }
        }

        
        private void ClearFolderContents(string folderPath)
        {
            DirectoryInfo di = new DirectoryInfo(folderPath);

            // Check if the directory exists
            if (di.Exists)
            {
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete(); // Delete each file
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true); // Delete each subdirectory and its content
                }
            }
            else
            {
                // If the directory does not exist, create it
                Directory.CreateDirectory(folderPath);
            }
        }

        
        private string CleanSubject(string subject)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();

            // Replace invalid characters with a safe character like an underscore.
            foreach (char c in invalidChars)
            {
                subject = subject.Replace(c, '_');
            }

            int maxLength = 200; 
            if (subject.Length > maxLength)
            {
                subject = subject.Substring(0, maxLength);
            }


            return subject;
        }

        
        private string GetUniqueFilePath(string filePath)
        {
            int count = 1;
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string fileExtension = Path.GetExtension(filePath);
            string directoryPath = Path.GetDirectoryName(filePath);
            string newFilePath = filePath;

            while (File.Exists(newFilePath))
            {
                string tempFileName = $"{fileName}({count++}){fileExtension}";
                newFilePath = Path.Combine(directoryPath, tempFileName);
            }

            return newFilePath;
        }
    }
}
