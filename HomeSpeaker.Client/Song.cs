using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeSpeaker.Client
{
    public record Song(int Id, string Name, string Album, string Artist, string Path);
}
