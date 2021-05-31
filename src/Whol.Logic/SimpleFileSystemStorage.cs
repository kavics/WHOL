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

        private string _eventsFilePath;
        private string _holidaysFilePath;
        private string EnsureEventsFile()
        {
            if (_eventsFilePath != null)
                return _eventsFilePath;

            var dirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            _eventsFilePath = Path.Combine(dirPath, "current.log");

            if (File.Exists(_eventsFilePath))
                return _eventsFilePath;

            if (!_disk.IsDirectoryExists(dirPath))
                _disk.CreateDirectory(dirPath);

            // ReSharper disable once EmptyEmbeddedStatement
            using (var _ = _disk.GetStreamForWrite(_eventsFilePath)) ;

            return _eventsFilePath;
        }
        private string EnsureHolidaysFile()
        {
            if (_holidaysFilePath != null)
                return _holidaysFilePath;

            var dirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            _holidaysFilePath = Path.Combine(dirPath, "holidays.txt");

            if (File.Exists(_holidaysFilePath))
                return _holidaysFilePath;

            if (!_disk.IsDirectoryExists(dirPath))
                _disk.CreateDirectory(dirPath);

            // ReSharper disable once EmptyEmbeddedStatement
            using (var _ = _disk.GetStreamForWrite(_holidaysFilePath)) ;

            return _holidaysFilePath;
        }

        public IEnumerable<Event> LoadEvents()
        {
            var events = new List<Event>();
            string line = null;

            var path = EnsureEventsFile();

            using (var stream = _disk.GetStreamForRead(path))
            using (var reader = new StreamReader(stream))
                while (null != (line = reader.ReadLine()))
                    events.Add(_eventSerializer.Deserialize(line));

            return events;
        }

        public IEnumerable<Holiday> LoadHolidays()
        {
            var holidays = new List<Holiday>();
            string line = null;

            var path = EnsureHolidaysFile();

            using (var stream = _disk.GetStreamForRead(path))
            using (var reader = new StreamReader(stream))
                while (null != (line = reader.ReadLine()))
                    holidays.Add(CreateHoliday(line));

            return holidays;
        }
        private Holiday CreateHoliday(string src)
        {
            var fields = src.Split('\t');
            return new Holiday
            {
                Day = DateTime.Parse(fields[0]),
                Description = fields[1],
            };
        }

        public void SaveEvents(IEnumerable<Event> events)
        {
            var path = EnsureEventsFile();
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
