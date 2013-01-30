using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibMemorySearch
{
    public class MemorySearcher
    {
        Process process;
        Dictionary<IntPtr, MemorySegment> memoryCache = new Dictionary<IntPtr, MemorySegment>();
        Func<WinAPI.MEMORY_BASIC_INFORMATION, bool> defaultRegionFilter = r =>  r.RegionSize.ToInt32() < 0x402000 && r.BaseAddress.ToInt32() > 0 && r.RegionSize.ToInt32() > 0 && r.Protect == (uint)WinAPI.AllocationProtect.PAGE_READWRITE && r.Type == (uint)WinAPI.RegionType.MEM_PRIVATE && r.State == (uint)WinAPI.RegionState.MEM_COMMIT;

        public MemorySearcher(Process process)
        {
            this.process = process;
        }

        public void ClearCache()
        {
            memoryCache.Clear();
            GC.Collect();
        }

        public void BuildCache()
        {
            BuildCache(defaultRegionFilter);
        }

        private void BuildCache(Func<WinAPI.MEMORY_BASIC_INFORMATION, bool> filter)
        {
            var memoryRegions = ListMemoryRegions();
            foreach (var region in memoryRegions)
            {
                if (filter(region))
                {
                    if (memoryCache.ContainsKey(region.BaseAddress) == false)
                    {
                        memoryCache.Add(region.BaseAddress, new MemorySegment(process, region));
                    }
                    memoryCache[region.BaseAddress].CopyMemory();
                }
            }
        }

        public string FindString(string needle, Wildcards wildcards)
        {
            foreach (var kvp in memoryCache)
            {
                if (kvp.Value != null && kvp.Value.Data != null)
                {
                    string result = kvp.Value.FindString(needle, wildcards);
                    if (result != null)
                    {
                        return result;
                    }
                    else
                    {
                        kvp.Value.Unload();
                    }
                }
            }
            return null;
        }

        private List<WinAPI.MEMORY_BASIC_INFORMATION> ListMemoryRegions()
        {
            List<WinAPI.MEMORY_BASIC_INFORMATION> regionList = new List<WinAPI.MEMORY_BASIC_INFORMATION>();
            long MaxAddress = 0x7fffffff;
            long address = 0;
            do
            {
                WinAPI.MEMORY_BASIC_INFORMATION m;
                int result = WinAPI.VirtualQueryEx(this.process.Handle, (IntPtr)address, out m, (uint)Marshal.SizeOf(typeof(WinAPI.MEMORY_BASIC_INFORMATION)));
                if (result != 0)
                {
                    regionList.Add(m);
                }
                if (address == (long)m.BaseAddress + (long)m.RegionSize)
                    break;
                address = (long)m.BaseAddress + (long)m.RegionSize;
            } while (address <= MaxAddress);

            return regionList;
        }
    }
}
