using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.JsonModel
{
    public class ConfigModel
    {
        public string host { get; set; }

        public string path { get; set; }

        public string user { get; set; }
        public string password { get; set; }

        public string share_folder_name { get; set; }

        public string protocol { get; set; }

        public string new_file_name { get; set; }
    }
}
