using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EasyFtp
{

    public class FtpService 
    {
        //"ftp://192.168.0.113/PhotoManagerBaloochi"
        //"devuser", "Dinawin@1400"

        public async Task<bool> DirectoryExist(string username, string password, string ftpAddress)
        {
            bool isFolderExist;
            try
            {
                FtpWebRequest folderExistrequest = (FtpWebRequest)WebRequest.Create(ftpAddress);
                folderExistrequest.Credentials = new NetworkCredential(username, password);
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

        public async Task CreateDirectoryWithSubDirectories(string username, string password, string ftpAddress)
        {
            var list = new List<string>();
            var uriBuilder = new UriBuilder(ftpAddress);

            var baseUri = new Uri(uriBuilder.Uri.Scheme + "://" + uriBuilder.Uri.Host);
            list.Add(baseUri.ToString());
            foreach (var item in uriBuilder.Uri.Segments)
            {
                var uri = new Uri(new Uri(list.LastOrDefault()), item);
                list.Add(uri.ToString());
            }

            list = list.Distinct().ToList();

            foreach (var item in list)
            {
                var isDirectoryExist = await DirectoryExist(username, password, item);
                if (!isDirectoryExist)
                    await CreateDirectory(username, password, item);

            }
        }

        public async Task CreateDirectory(string username, string password, string ftpAddress)
        {



            FtpWebRequest folderExistrequest = (FtpWebRequest)WebRequest.Create(ftpAddress);
            folderExistrequest.Credentials = new NetworkCredential(username, password);
            folderExistrequest.Method = WebRequestMethods.Ftp.MakeDirectory;
            FtpWebResponse folderExistresponse = (FtpWebResponse)folderExistrequest.GetResponse();


        }


        private async Task Upload(string username, string password, string ftpAddress, string localFilepath, string fileName = "")
        {
            using var webClient = new WebClient();
            webClient.Credentials = new NetworkCredential(username, password);
            var fileExist = File.Exists(localFilepath);
            if (!fileExist)
                throw new FileNotFoundException(localFilepath);

            if (!string.IsNullOrEmpty(fileName))
            {
                var hasExtention = Path.HasExtension(fileName);
                if (!hasExtention)
                {
                    var fileExtention = Path.GetExtension(localFilepath);
                    fileName = fileName + fileExtention;
                }
            }
            else
            {
                fileName = Path.GetFileName(localFilepath);
            }


            var address = Path.Combine(ftpAddress, fileName).Replace("\\", "/");
            await webClient.UploadFileTaskAsync(address, WebRequestMethods.Ftp.UploadFile, localFilepath);
        }

        public async Task UploadWithCreateDirectory(string username, string password, string ftpAddress, string localFilepath, string fileName)
        {


            bool isFolderExist = await DirectoryExist(username, password, ftpAddress);

            if (!isFolderExist)
                await CreateDirectoryWithSubDirectories(username, password, ftpAddress);

            await Upload(username, password, ftpAddress, localFilepath, fileName);



        }
    }
}