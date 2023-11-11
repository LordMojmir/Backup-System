using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using EmailBackup;
using Org.BouncyCastle.Pqc.Crypto.Falcon;

namespace BackUP_System.Model
{
    public class BackupModel : INotifyPropertyChanged
    {
        private string sourceDirectory;
        private string destinationDirectory;
        private bool secureSave;
        private string _mail;
        private string _passwordMail;

        public string SourceDirectory
        {
            get => sourceDirectory;
            set
            {
                sourceDirectory = value;
                OnPropertyChanged();
            }
        }

        public string DestinationDirectory
        {
            get => destinationDirectory;
            set
            {
                destinationDirectory = value;
                OnPropertyChanged();
            }
        }

        public bool SecureSave
        {
            get => secureSave;
            set
            {
                secureSave = value;
                OnPropertyChanged();
            }
        }

        public string Username
        {
            get => _mail;
            set
            {
                try
                {
                    var mailAddress = new System.Net.Mail.MailAddress(value);
                    _mail = value; // If no exception is thrown, the email is valid.
                }
                catch (FormatException)
                {
                    // Optionally handle the error, e.g., by logging or setting a flag.
                    Console.WriteLine("Invalid email address format.");
                }
                // if (value.Contains("@"))
                //     _mail = value;
            }
        }

        public string Password { get => _passwordMail;
            set
            {
                _passwordMail = value ?? _passwordMail;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool makeBackup()
        {

            
            string dateFolder = DateTime.Now.ToString("yyMMdd") + "_BackUP";
            string destinationPath = DestinationDirectory;
            string toDirectory = Path.Combine(destinationPath, dateFolder);
        
            string mailBackupDirectory = Path.Combine(toDirectory, "MailBackups");
            string fileBackupDirectory = Path.Combine(toDirectory, "FileBackups");
              
            Directory.CreateDirectory(mailBackupDirectory);
            Directory.CreateDirectory(fileBackupDirectory);
            
            #region Backup Files
            FileBackup fb = new(SourceDirectory, fileBackupDirectory);

            if (secureSave)
            {
                Console.WriteLine(fb.PerformBackup(false));
                compress_directory(toDirectory);
            }
            
            #endregion

            #region Backup Mail

            ImapEmailBackup eb = new ImapEmailBackup();
            Console.WriteLine("Starting to download and save Mails...");

            try
            {
                eb.DownloadEmails("imap.m1.websupport.sk", 993, true, _mail, _passwordMail, mailBackupDirectory);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Console.WriteLine("Mail saved");
            return true;
            
            #endregion
        }

        public bool makeBackup(TextBlock errorMessageBlock)
        {
            string dateFolder = DateTime.Now.ToString("yyMMdd") + "_BackUP";
            string destinationPath = DestinationDirectory;
            string toDirectory = Path.Combine(destinationPath, dateFolder);
        
            string mailBackupDirectory = Path.Combine(toDirectory, "MailBackups");
            string fileBackupDirectory = Path.Combine(toDirectory, "FileBackups");
              
            Directory.CreateDirectory(mailBackupDirectory);
            Directory.CreateDirectory(fileBackupDirectory);
            
            #region Backup Files
            FileBackup fb = new(SourceDirectory, fileBackupDirectory);
            Console.WriteLine(fb.PerformBackup(secureSave));
            #endregion

            #region Backup Mail

            ImapEmailBackup eb = new ImapEmailBackup();
            Console.WriteLine("Starting to download and save Mails...");

            try
            {
                eb.DownloadEmails("imap.m1.websupport.sk", 993, true, _mail, _passwordMail, mailBackupDirectory);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            Console.WriteLine("Mail saved");
            return true;
            
            #endregion
        }

        public string compress_directory(string path)
        {
            string zipFilePath = path + ".zip";
            CompressToZip(path, zipFilePath);

            return "Backup and compression completed successfully. ZIP file created at: " + zipFilePath;
            
        }
        
        private void CompressToZip(string sourceDirectory, string zipFilePath)
        {
            if (File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath); // Overwrite existing ZIP file
            }
            ZipFile.CreateFromDirectory(sourceDirectory, zipFilePath);
        }
        
    }
}