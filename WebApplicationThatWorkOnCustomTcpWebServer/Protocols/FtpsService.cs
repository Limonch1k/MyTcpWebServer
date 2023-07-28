using Model.JsonModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Новая_папка__2_.Protocls
{
    public class FtpsService
    {
        private FtpWebRequest request { get; set; }


        public void CreateDownloadRequest(ConfigModel model)
        {
            string url = model.host + model.path;
            request = (FtpWebRequest)WebRequest.Create(url);
            request.Credentials = new NetworkCredential(model.user, model.password);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            //request.EnableSsl = true;
        }

        public void GetResponse(ConfigModel model)
        {
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        using (StreamWriter writter = new StreamWriter(model.new_file_name, false))
                        {
                            writter.Write(reader.ReadToEnd());
                        }
                    }
                }
            }
        }
    }
}
