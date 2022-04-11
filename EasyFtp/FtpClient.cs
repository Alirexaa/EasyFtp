using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EasyFtp
{
    public class FtpClient
    {
        public FtpClient(string basrUri, string username, string password,bool validateLogin = false)
        {
            BaseUri = new Uri(basrUri);
            UserName = username;
            Password = password;
            Credential = new NetworkCredential(UserName, password);
            if (!validateLogin)
            {
                return;
            }
            if (!CanLogin())
                throw new UnauthorizedAccessException();

        }

        public FtpClient(Uri baseUri, string username, string password, bool validateLogin = false)
        {
            BaseUri = baseUri;
            UserName = username;
            Password = password;
            Credential = new NetworkCredential(UserName, password);
            if (!validateLogin)
            {
                return;
            }
            if (!CanLogin())
                throw new UnauthorizedAccessException();


        }

        public Uri BaseUri { get; }
        public string UserName { get; }
        public string Password { get; }
        public NetworkCredential Credential { get; }

        public bool DirectoryExist(string ftpAddress)
        {
            bool isFolderExist;
            try
            {
                FtpWebRequest folderExistrequest = (FtpWebRequest)WebRequest.Create(ftpAddress);
                folderExistrequest.Credentials = Credential;
                folderExistrequest.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse folderExistresponse = (FtpWebResponse)folderExistrequest.GetResponse();

                isFolderExist = true;
            }
            catch (Exception)
            {

                isFolderExist = false;
            }

            return isFolderExist;
        }

        public async Task<bool> DirectoryExistAsync(string ftpAddress)
        {
            bool isFolderExist;
            try
            {
                FtpWebRequest folderExistrequest = (FtpWebRequest)WebRequest.Create(ftpAddress);
                folderExistrequest.Credentials = new NetworkCredential(UserName, Password);
                folderExistrequest.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse folderExistresponse = (FtpWebResponse)(await folderExistrequest.GetResponseAsync());

                isFolderExist = true;
            }
            catch (Exception)
            {

                isFolderExist = false;
            }

            return isFolderExist;
        }

        public bool CanLogin()
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(BaseUri);
                request.Credentials = Credential;
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();


            }
            catch (WebException ex)
            {

                return false;
            }

            return true;

        }
    }
}
