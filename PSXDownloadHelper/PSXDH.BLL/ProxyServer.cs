using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PSXDH.BLL
{
    public class ProxyServer
    {
        /// <summary>
        /// 获取本机IP
        /// </summary>
        /// <returns></returns>
        public static IPAddress[] GetHostIp()
        {
            var ip = Dns.GetHostEntry(Dns.GetHostName());
            var iplist = ip.AddressList.Where(p => p.AddressFamily == AddressFamily.InterNetwork).ToArray();
            return iplist;
        }
    }
}
