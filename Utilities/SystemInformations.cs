using mFFramework.Types;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace mFFramework.Utilities
{

    /// <summary>
    /// 
    /// </summary>
    public class Information
    {
        /// <summary>
        /// 
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UserDomainName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Network { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public CLR Clr { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public OS Os { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DNS Dns{get;set;}
       

        /// <summary>
        /// Fornisce le informazioni di versione del mFFramework
        /// </summary>
        public static mFVersion GetmFFrameworkVersion()
        {


            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            FileInfo fi = new FileInfo(assembly.CodeBase.Replace("file:///", string.Empty));

            mFVersion version = new mFVersion
            {


                FullName = assembly.FullName
                ,
                Description = fvi.Comments
                ,
                NumberVersion = fvi.FileVersion
                                    .Replace(".*", string.Empty)
                                    .Replace(".", "r")
                                    .Insert(0, "v")
                ,
                Company = fvi.CompanyName
                ,
                CopyRight = fvi.LegalCopyright
                ,
                DateCreation = fi.LastWriteTime

            };



            return version;

           

        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class CLR
    {
    
        /// <summary>
        /// 
        /// </summary>
        public int Major {get;set;}
        /// <summary>
        /// 
        /// </summary>
        public int Minor {get;set;}
        /// <summary>
        /// 
        /// </summary>
        public int Build {get;set;}
        /// <summary>
        /// 
        /// </summary>
        public int Revision {get;set;}
    
    
    }

    /// <summary>
    /// 
    /// </summary>
    public class OS
    {
        /// <summary>
        /// 
        /// </summary>
        public int Major { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Minor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Build { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Revision { get; set; }


    }

    /// <summary>
    /// 
    /// </summary>
    public class DNS
    { 
    
        /// <summary>
        /// 
        /// </summary>
        public string HostName {get;set;}
            //iphe = Dns.Resolve(info[14]);
            //ipa = iphe.AddressList[0];
            //ipa2 = iphe.AddressList;
            //info[15] = (ipa.Address).ToString();
            //ipendpoint = new IPEndPoint(ipa, 100);
            //info[16] = (ipendpoint.Address).ToString();
            //info[22] = IPEndPoint.MaxPort.ToString();
            //info[21] = IPEndPoint.MinPort.ToString();

        /// <summary>
        /// 
        /// </summary>
        public IPAddress iPADDRESS{get;set;}

    
    }

    /// <summary>
    /// 
    /// </summary>
    public class IPAddress
    {
    
        /// <summary>
        /// 
        /// </summary>
        public string Broadcast { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Loopback { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Any { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string None { get; set; }
    
    }

    /// <summary>
    /// 
    /// </summary>
    public class IPEndPoint
    {
    
        /// <summary>
        /// 
        /// </summary>
        public string Broadcast { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Loopback { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Any { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string None { get; set; }
    
    }

    /// <summary>
    /// 
    /// </summary>
    public static class SystemInformations
    {

        /// <summary>
        /// 
        /// </summary>
        public static void Get()
        {


            Information info = new Information();


            //  System Information
            info.UserName = SystemInformation.UserName;
            info.UserDomainName = SystemInformation.UserDomainName;
            info.Network = (SystemInformation.Network).ToString();

            //  Environment Version CLR
            info.Clr = new CLR();
            info.Clr.Major = Environment.Version.Major;
            info.Clr.Minor = Environment.Version.Minor;
            info.Clr.Build = Environment.Version.Build;
            info.Clr.Revision = Environment.Version.Revision;


            //  Environment Version OS
            info.Os = new OS
            {
                Build = Environment.OSVersion.Version.Build
                ,
                Major = Environment.OSVersion.Version.Major
                ,
                Minor = Environment.OSVersion.Version.Minor
                ,
                Revision = Environment.OSVersion.Version.Revision
            };



          

            //info[11] = Environment.UserName;
            //info[12] = Environment.UserDomainName;
            //info[13] = Environment.MachineName;

 

            //info[14] = Dns.GetHostName();
            //iphe = Dns.Resolve(info[14]);
            //ipa = iphe.AddressList[0];
            //ipa2 = iphe.AddressList;
            //info[15] = (ipa.Address).ToString();

            //ipendpoint = new IPEndPoint(ipa, 100);
            //info[16] = (ipendpoint.Address).ToString();
            //info[22] = IPEndPoint.MaxPort.ToString();
            //info[21] = IPEndPoint.MinPort.ToString();






            //info.Dns = new DNS{
            // iPADDRESS = new IPAddress
            // {
            //  Broadcast = IPAddress.Broadcast.ToString()
            //  , Any = IPAddress.Any.ToString()
            //  , Loopback = IPAddress.Loopback.ToString()
            //  , None = IPAddress.None.ToString()

             
            // }
            
            //};

        }


    }



}
