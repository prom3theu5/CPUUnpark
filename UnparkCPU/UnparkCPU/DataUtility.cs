using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogQuery = Interop.MSUtil.LogQueryClass;
using RegistryInputFormat = Interop.MSUtil.COMRegistryInputContextClass;
using RegRecordSet = Interop.MSUtil.ILogRecordset;
using System.Diagnostics;
using Microsoft.Win32;

namespace UnparkCPU
{
    public class DataUtility
    {

        public bool RegisterParserDLL()
        {
            try
            {
                //'/s' : indicates regsvr32.exe to run silently.
                string name = "LogParser.dll";
                string fileinfo = "/s" + " " + "\"" + name + "\"";

                Process reg = new Process();
                reg.StartInfo.FileName = "regsvr32.exe";
                reg.StartInfo.Arguments = fileinfo;
                reg.StartInfo.UseShellExecute = false;
                reg.StartInfo.CreateNoWindow = true;
                reg.StartInfo.RedirectStandardOutput = true;
                reg.Start();
                reg.WaitForExit();
                reg.Close();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<RegData> SelectRegistryData()
        {
            RegRecordSet rs = null;
            List<RegData> regDataCollection = new List<RegData>();

            try
            {
                string query = @"Select Path, ValueName, Value from HKEY_LOCAL_MACHINE where Path like '%0cc5b647-c1df-4637-891a-dec35c318583' and ValueName='ValueMax'";
                
                LogQuery qry = new LogQuery();
                RegistryInputFormat registryFormat = new RegistryInputFormat();
                
                rs = qry.Execute(query, registryFormat);
                int index = 0;

                for (; !rs.atEnd(); rs.moveNext())
                {
                    RegData regData = null;

                    if ((regData = GetRegData(rs.getRecord().toNativeString(","),index)) != null)
                    {
                        regDataCollection.Add(regData);
                    }

                    index++;
                }

                return regDataCollection;

            }
            finally
            {
                rs.close();
            }
        }

        public bool Write(string path,string KeyName, object Value)
        {
            if (path != null)
            {
                path = path.Replace(@"HKEY_LOCAL_MACHINE\", String.Empty);
                try
                {
                    RegistryKey registry = Registry.LocalMachine;
                    RegistryKey sk1 = registry.CreateSubKey(path.ToUpper());
                    sk1.SetValue(KeyName, Value);

                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            return false;
        }

        private  RegData GetRegData(string data, int index)
        {
            if (data != null)
            {
                char separator = ',';
                string[] splitted = data.Split(separator);

                if (splitted != null && splitted.Length > 0)
                {
                    RegData regData =
                        new RegData(index,
                                    splitted[0] ?? String.Empty,
                                    splitted[1] ?? String.Empty,
                                    int.Parse(splitted[2]??"0"));

                    return regData;
                }
            }

            return null;

        }


    }
}
