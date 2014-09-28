using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MiniCrwaler
{
    interface DataSaver
    {
        StatusCode SaveData(string URI, byte[] content);
    }

    class DiskDataSaver
        : DataSaver
    {
        public StatusCode SaveData(string URI, byte[] content)
        {
            try
            {
                string normailizedPath = URI.Replace("http://","").Replace('/','\\');
                string parentPath = normailizedPath.Substring(0, normailizedPath.LastIndexOf("\\"));
                if (!Directory.Exists(parentPath))
                    Directory.CreateDirectory(parentPath);
                using (FileStream fs = new FileStream(normailizedPath,FileMode.Create,FileAccess.Write))
                {
                    fs.Write(content, 0, content.Length);
                }
                return StatusCode.Success;
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Unable to save {0},{1}",URI,e.ToString()));
                return StatusCode.Error;
            }
        }
    }
}
