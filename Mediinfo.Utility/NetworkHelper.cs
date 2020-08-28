using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Xml;

namespace Mediinfo.Utility
{
    /// <summary>
    /// 网络工具类
    /// </summary>
    public class NetworkHeler
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        private NetworkHeler()
        {

        }

        /// <summary>
        /// 设置一个静态变量。避免多次获取
        /// </summary>
        private static List<NetworkConfig> NetworkConfigList { get; set; }

        /// <summary>
        /// 获取当前可用的网络配置
        /// </summary>
        /// <returns></returns>
        public static List<NetworkConfig> GetAvailableNetwork()
        {

            if (NetworkConfigList != null)
            {
                return NetworkConfigList;
            }

            string computerName = System.Environment.MachineName;

            List<NetworkConfig> networkList = new List<NetworkConfig>();

            if (!NetworkInterface.GetIsNetworkAvailable())
                return networkList;

            List<NetworkInterface> interfaces = NetworkInterface.GetAllNetworkInterfaces()
                                                .Where(c => (c.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || c.NetworkInterfaceType == NetworkInterfaceType.Ethernet) && c.OperationalStatus == OperationalStatus.Up)
                                                .ToList();
            // 将本地网卡前移，虚拟网卡向后
            for (int i = 0; i < interfaces.Count; i++)
            {
                for (int j = 1; j < interfaces.Count; j++)
                {
                    // 获取注册表信息
                    string fRegistryKey = "SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\" + interfaces[j].Id + "\\Connection";
                    RegistryKey rk = Registry.LocalMachine.OpenSubKey(fRegistryKey, false);
                    if (rk != null)
                    {
                        // 区分 PnpInstanceID   
                        // 如果前面有 PCI 就是本机的真实网卡  
                        string fPnpInstanceID = rk.GetValue("PnPInstanceId", "null").ToString();
                        if (fPnpInstanceID.Length > 3 &&
                            fPnpInstanceID.Substring(0, 3) == "PCI")
                        {
                            NetworkInterface temp = interfaces[j - 1];
                            interfaces[j - 1] = interfaces[j];
                            interfaces[j] = temp;

                        }
                    }
                }
            }

            foreach (NetworkInterface ni in interfaces)
            {
                string ip = String.Empty;

                if (ni.GetIPProperties().UnicastAddresses.Count > 0)
                {
                    foreach (UnicastIPAddressInformation ipadd in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ipadd.Address.AddressFamily == AddressFamily.InterNetwork)
                            ip = ipadd.Address.ToString();
                    }
                }
                else
                {
                    ip = string.Empty;
                }

                string physicalAddress;
                byte[] bytes = ni.GetPhysicalAddress().GetAddressBytes();

                if (bytes.Length > 0)
                    physicalAddress = string.Format("{0:X2}-{1:X2}-{2:X2}-{3:X2}-{4:X2}-{5:X2}", bytes[0], bytes[1], bytes[2], bytes[3], bytes[4], bytes[5]);
                else
                    physicalAddress = string.Empty;

                networkList.Add(new NetworkConfig
                {
                    Id = ni.Id,
                    Name = ni.Name,
                    Description = ni.Description,
                    PhysicalAddress = physicalAddress,
                    NetworkInterfaceType = ni.NetworkInterfaceType,
                    OperationalStatus = ni.OperationalStatus,
                    Ip = ip,
                    IpVersion = ni.Supports(NetworkInterfaceComponent.IPv4) ? 0 : 1,
                    ComputerName = computerName
                });
            }
            #region add by 余佳平 通过比对网关IP来获取最匹配的本机IP

            //比对IP节点各个值来获取匹配度最高的项，并将该项移至首位，因为本机IP为IP列表第一项
            string config = IOHelper.Read(AppDomain.CurrentDomain.BaseDirectory + "WinFormMain.xml");//网关配置文件
            if (config != null)
            {
                XmlDocument xml = new XmlDocument();
                // 打开现有的一个xml文件
                xml.LoadXml(config);
                // 获得xml的根节点
                var configure = xml.DocumentElement;
                string gatewayIP = configure.SelectSingleNode("ServerUrl")?.InnerText;//网关配置
                NetworkConfig dest = null;//匹配度最高的配置项
                foreach (var item in networkList)
                {
                    string[] ip1 = item.Ip.Split('.');//当前ip各节点值
                    string[] ip2 = gatewayIP.Split('.');//网关配置ip各节点值，只取前三个节点
                    if (ip1.Length == 4 && ip2.Length >= 4)
                    {
                        if (ip1[0] == ip2[0] && ip1[1] == ip2[1] && ip1[2] == ip2[2])
                        {
                            //前三个节点值匹配
                            dest = item;
                            break;
                        }
                        else if (ip1[0] == ip2[0] && ip1[1] == ip2[1])
                        {
                            //前两个节点值匹配
                            dest = item;
                            break;
                        }
                        else if (ip1[0] == ip2[0])
                        {
                            //第一个节点值匹配
                            dest = item;
                            break;
                        }
                    }
                }
                if (dest != null)
                {
                    //将匹配项移至首位，因为本机IP为IP列表第一项
                    networkList.Remove(dest);
                    networkList.Insert(0, dest);
                }
            }
            #endregion

            NetworkConfigList = networkList;
            return networkList;
        }
    }

    public class NetworkConfig
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 物理（MAC）地址
        /// </summary>
        public string PhysicalAddress { get; set; }

        /// <summary>
        /// 网络接口类型
        /// </summary>
        public NetworkInterfaceType NetworkInterfaceType { get; set; }

        /// <summary>
        /// 网络接口的操作状态
        /// </summary>
        public OperationalStatus OperationalStatus { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 0:IPV4;1:IPV6
        /// </summary>
        public int IpVersion { get; set; }

        /// <summary>
        /// 计算机名
        /// </summary>
        public string ComputerName { get; set; }
    }
}
