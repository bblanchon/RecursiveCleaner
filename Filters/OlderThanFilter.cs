using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RecursiveCleaner.Filters
{
    class OlderThanFilter : IFilter
    {
        readonly TimeSpan span;

        public OlderThanFilter(int years, int months, int days, int hours, int minutes, int seconds)
        {
            span = new TimeSpan(years * 365 + months * 30 + days, hours, minutes, seconds);
        }

        public bool IsMatch(FileSystemInfo fsi)
        {
            if (fsi is DirectoryInfo)
            {
                var newest = (fsi as DirectoryInfo).EnumerateFiles("*", SearchOption.AllDirectories).Max(x => x.LastWriteTime);

                return newest + span < DateTime.Now;
            }
            else
            {
                return fsi.LastWriteTime + span < DateTime.Now;
            }
        }
    }
}
