using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace VisionDemo
{
    public static class IniFile
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileInt(string lpAppName, string lpKeyName, int nDefault, string lpFileName);
        [DllImport("kernel32.dll")]
         private static extern uint GetPrivateProfileStringA(string section, string key,
            string def, Byte[] retVal, int size, string filePath);
        ///   string lpAppName, // points to section name
        ///   string lpKeyName, // points to key name
        ///   string lpDefault, // points to default string
        ///   byte[] lpReturnedString, // points to destination buffer
        ///   uint nSize, // size of destination buffer
        ///   string lpFileName  // points to initialization filename
        [DllImport("kernel32.dll")]
        public static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, byte[] lpReturnedString, uint nSize, string lpFileName);
          /// <summary>
        /// [扩展]读Int数值
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="name">键</param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static int ReadInt(String inifile,string section, string name, int def)
        {
            return GetPrivateProfileInt(section, name, def, inifile);
        }

        /// <summary>
        /// [扩展]读取string字符串
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="name">键</param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static string ReadString(String inifile, string section, string name, string def)
        {
            StringBuilder vRetSb = new StringBuilder(2048);
            GetPrivateProfileString(section, name, def, vRetSb, 2048, inifile);
            return vRetSb.ToString();
        }

        /// <summary>
        /// [扩展]写入Int数值，如果不存在 节-键，则会自动创建
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="name">键</param>
        /// <param name="Ival">写入值</param>
        public static void WriteInt(String inifile, string section, string name, int Ival)
        {

            WritePrivateProfileString(section, name, Ival.ToString(), inifile);
        }

        /// <summary>
        /// [扩展]写入String字符串，如果不存在 节-键，则会自动创建
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="name">键</param>
        /// <param name="strVal">写入值</param>
        public static void WriteString(String inifile, string section, string name, string strVal)
        {
            WritePrivateProfileString(section, name, strVal, inifile);
        }

        /// <summary>
        /// 删除指定的 节
        /// </summary>
        /// <param name="section"></param>
        public static void DeleteSection(String inifile, string section)
        {
            WritePrivateProfileString(section, null, null, inifile);
        }

        /// <summary>
        /// 删除全部 节
        /// </summary>
        public static void DeleteAllSection(String inifile)
        {
            WritePrivateProfileString(null, null, null, inifile );
        }

        /// <summary>
        /// 读取指定 节-键 的值
        /// </summary>
        /// <param name="section"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string IniReadValue(String inifile, string section, string name)
        {
            StringBuilder strSb = new StringBuilder(256);
            GetPrivateProfileString(section, name, "", strSb, 256, inifile);
            return strSb.ToString();
        }

        /// <summary>
        /// 写入指定值，如果不存在 节-键，则会自动创建
        /// </summary>
        /// <param name="section"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void IniWriteValue(String inifile, string section, string name, string value)
        {
            WritePrivateProfileString(section, name, value, inifile );
        }
        public static List<string> ReadSections(string iniFilename)
        {

            List<string> result = new List<string>();

            byte[] buf = new byte[65536];

            int len = GetPrivateProfileString(null, null, null, buf, (uint)buf.Length, iniFilename);

            int j = 0;

            for (int i = 0; i < len; i++)

                if (buf[i] == 0)
                {

                    result.Add(Encoding.Default.GetString(buf, j, i - j));

                    j = i + 1;

                }

            return result;

        }
        /// <summary>

        /// 读取指定区域Keys列表。

        /// </summary>

        /// <param name="Section"></param>

        /// <param name="Strings"></param>

        /// <returns></returns>

        public static List<string> ReadSingleSection(string Section, string iniFilename)
        {

            List<string> result = new List<string>();

            byte[] buf = new byte[65536];

            int lenf = GetPrivateProfileString(Section, null, null, buf, (uint)buf.Length, iniFilename);

            int j = 0;

            for (int i = 0; i < lenf; i++)

                if (buf[i] == 0)
                {

                    result.Add(Encoding.Default.GetString(buf, j, i - j));

                    j = i + 1;

                }

            return result;

        }
        public static List<string> ReadKeys(string iniFilename,string SectionName)
        {
            List<string> result = new List<string>();
            Byte[] buf = new Byte[65536];
            uint len = GetPrivateProfileStringA(SectionName, null, null, buf, buf.Length, iniFilename);
            int j = 0;
            for (int i = 0; i < len; i++)
                if (buf[i] == 0)
                {
                    result.Add(Encoding.Default.GetString(buf, j, i - j));
                    j = i + 1;
                }
            return result;
        }
    }
}
