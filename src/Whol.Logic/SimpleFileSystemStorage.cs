using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Whol.Logic
{
    public class SimpleFileSystemStorage : IStorage
    {
        private readonly IDisk _disk;
        private readonly IEventSerializer _eventSerializer;

        public SimpleFileSystemStorage(IDisk disk, IEventSerializer eventSerializer)
        {
            _disk = disk;
            _eventSerializer = eventSerializer;
        }

        private string _filePath;
        private string EnsureFile()
        {
            if (_filePath != null)
                return _filePath;

            var dirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            _filePath = Path.Combine(dirPath, "current.log");

            if (File.Exists(_filePath))
                return _filePath;

            if (!_disk.IsDirectoryExists(dirPath))
                _disk.CreateDirectory(dirPath);

            // ReSharper disable once EmptyEmbeddedStatement
            using (var _ = _disk.GetStreamForWrite(_filePath)) ;

            return _filePath;
        }

        public IEnumerable<Event> LoadEvents()
        {
            var events = new List<Event>();
            string line = null;

            var path = EnsureFile();

            using (var stream = _disk.GetStreamForRead(path))
            using (var reader = new StreamReader(stream))
                while (null != (line = reader.ReadLine()))
                    events.Add(_eventSerializer.Deserialize(line));

            return events;
        }

        public IEnumerable<Holiday> LoadHolidays()
        {
            throw new NotImplementedException(); //UNDONE: SimpleFileSystemStorage.LoadEvents is not implemented
        }
        public void SaveEvents(IEnumerable<Event> events)
        {
            var path = EnsureFile();
            using (var stream = _disk.GetStreamForWrite(path))
            using (var writer = new StreamWriter(stream))
                foreach (var @event in events)
                    writer.WriteLine(_eventSerializer.Serialize(@event));
        }
        public void SaveHolidays(IEnumerable<Holiday> holidays)
        {
            throw new NotImplementedException(); //UNDONE: SimpleFileSystemStorage.LoadEvents is not implemented
        }
    }
}
