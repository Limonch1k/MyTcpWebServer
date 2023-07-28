using Model.JsonModel;
using SMBLibrary.Client;
using SMBLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Новая_папка__2_.Protocls
{
    public class SamboService
    {
        public ISMBClient GetSmbClient(ConfigModel model)
        {
            ISMBClient client = null;
            if (model.protocol.Contains("smb1"))
            {
                client = new SMB1Client();
            }
            else if (model.protocol.Contains("smb2"))
            {
                client = new SMB2Client();
            }

            bool isConnected = client.Connect(IPAddress.Parse(model.host), SMBTransportType.DirectTCPTransport);
            if (isConnected)
            {
                NTStatus status = client.Login(string.Empty, model.user, model.password);
                if (status == NTStatus.STATUS_SUCCESS)
                {
                    return client;
                }
                return null;
            }
            else
            {
                throw new Exception("cannot connect to server");
            }
            return client;
        }


        public void ReadFile(ISMBClient client, ConfigModel model)
        {
            NTStatus status;
            ISMBFileStore fileStore = client.TreeConnect(model.share_folder_name, out status);
            if (fileStore is null)
            {
                throw new Exception("cannot get fileStore + status" + status);
            }
            object fileHandle;
            FileStatus fileStatus;
            string filePath = model.path;

            if (fileStore is SMB1FileStore)
            {
                filePath = @"\\" + filePath;
            }

            status = fileStore.CreateFile(out fileHandle, out fileStatus, filePath, AccessMask.GENERIC_READ | AccessMask.SYNCHRONIZE, SMBLibrary.FileAttributes.Normal,
                ShareAccess.Read, CreateDisposition.FILE_OPEN, CreateOptions.FILE_NON_DIRECTORY_FILE | CreateOptions.FILE_SYNCHRONOUS_IO_ALERT, null);

            if (status == NTStatus.STATUS_SUCCESS)
            {
                byte[] data;
                long bytesRead = 0;
                while (true)
                {
                    status = fileStore.ReadFile(out data, fileHandle, bytesRead, (int)client.MaxReadSize);
                    if (status != NTStatus.STATUS_SUCCESS && status != NTStatus.STATUS_END_OF_FILE)
                    {
                        throw new Exception("Failed to read from file");
                    }

                    if (status == NTStatus.STATUS_END_OF_FILE || data.Length == 0)
                    {
                        break;
                    }
                    bytesRead += data.Length;
                    using (FileStream fs = new FileStream(@"" + model.new_file_name, FileMode.Create))
                    {
                        fs.Write(data, 0, data.Length);
                    }
                }
            }
            status = fileStore.CloseFile(fileHandle);
            status = fileStore.Disconnect();

        }
    }
}
