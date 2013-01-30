using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LibMemorySearch
{
    internal class MemorySegment
    {
        WinAPI.MEMORY_BASIC_INFORMATION region;
        Process process;
        byte[] data;
        

        public byte[] Data
        {
            get { return data; }
        }

        public MemorySegment(Process process, WinAPI.MEMORY_BASIC_INFORMATION region)
        {
            this.region = region;
            this.process = process;
        }

        public bool CopyMemory()
        {
            Debug.WriteLine("Loading region. Base: {0}, Size: {1}", region.BaseAddress, region.RegionSize.ToInt32());
            try
            {
                int regionSize = region.RegionSize.ToInt32();
                int bytesRead = 0;
                data = new byte[regionSize];
                if ( WinAPI.ReadProcessMemory(process.Handle, region.BaseAddress, data, regionSize, out bytesRead) == false || bytesRead < regionSize )
                {
                    throw new Exception(String.Format("ReadProcessMemory failed. Base: {0}, Size: {1}, Read: {2}", region.BaseAddress, regionSize, bytesRead));
                }
            }
            catch (Exception ex)
            {
                data = null;
                Debug.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        public void Unload()
        {
            Debug.WriteLine("Unloading region. Base: {0}, Size: {1}", region.BaseAddress, region.RegionSize.ToInt32());
            data = null;
        }

        public string FindString(string needle, Wildcards wildcards)
        {
            if (data == null || data.Length == 0) return null;

            int dataLength = data.Length;
            int needleLength = needle.Length;
            int searchLength = dataLength - needleLength;
            bool found = false;

            for (int d = 0; d < searchLength; d++)
            {
                found = true;
                for (int n = 0; n < needleLength; n++)
                {
                    if (needle[n] != data[d + n] && false == wildcards.IsMatch(needle[n], (char)data[d + n]))
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    return ASCIIEncoding.UTF8.GetString(data, d, needleLength);
                }
            }
            return null;
        }
    }
}
