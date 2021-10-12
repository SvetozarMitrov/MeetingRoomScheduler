using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace MeetingRoomScheduler.Data
{
    public class JsonFileReaderService
    {
        public List<T> LoadRoomData<T>()
        {
            return JsonConvert.DeserializeObject<List<T>>(File.ReadAllText("./Data/RoomData.json"));
        }
    }
}
