using System.Net;

//from https://stackoverflow.com/questions/15995705/adding-pause-and-continue-ability-in-my-downloader

namespace cnpz_downloader.FileDownloader
{
    public class FileDownload
    {
        private volatile bool _allowedToRun;
        private readonly string _sourceUrl;
        private readonly string _destination;
        private readonly int _chunkSize;
        private readonly IProgress<double> _progress;
        private readonly Lazy<long> _contentLength;

        public long BytesWritten { get; private set; }
        public long ContentLength => _contentLength.Value;

        public bool Done => ContentLength <= BytesWritten;

        public FileDownload(string source, string destination, int chunkSizeInBytes = 10000 /*Default to 0.01 mb*/, IProgress<double> progress = null)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source is empty");
            if (string.IsNullOrEmpty(destination))
                throw new ArgumentNullException("destination is empty");

            _allowedToRun = true;
            _sourceUrl = source;
            _destination = destination;
            _chunkSize = chunkSizeInBytes;
            _contentLength = new Lazy<long>(GetContentLength);
            _progress = progress;

            if (!File.Exists(destination))
                BytesWritten = 0;
            else
            {
                try
                {
                    BytesWritten = new FileInfo(destination).Length;
                }
                catch
                {
                    BytesWritten = 0;
                }
            }
        }

        private long GetContentLength()
        {
            var request = (HttpWebRequest)WebRequest.Create(_sourceUrl);
            request.Method = "HEAD";

            using (var response = request.GetResponse())
                return response.ContentLength;
        }

        private async Task Start(long range)
        {
            if (!_allowedToRun)
                throw new InvalidOperationException();

            if (Done)
                //file has been found in folder destination and is already fully downloaded 
                return;

            if (BytesWritten >= ContentLength)
                //file has been found in folder destination and is already fully downloaded 
                return;

            Console.WriteLine(BytesWritten);
            Console.WriteLine(ContentLength);
            Console.WriteLine(range);

            var request = (HttpWebRequest)WebRequest.Create(_sourceUrl);
            request.Method = "GET";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
            request.AddRange(range);

            using (var response = await request.GetResponseAsync())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    using (var fs = new FileStream(_destination, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    {
                        while (_allowedToRun)
                        {
                            var chunkFaltante = ContentLength - BytesWritten;
							var buffer = new byte[chunkFaltante < _chunkSize ? chunkFaltante : _chunkSize];
							var bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

                            if (bytesRead == 0) break;

                            await fs.WriteAsync(buffer, 0, bytesRead);
                            BytesWritten += bytesRead;
                            _progress?.Report((double)BytesWritten / ContentLength);

                            
                        }

                        await fs.FlushAsync();
                    }
                }
            }
        }

        public Task Start()
        {
            _allowedToRun = true;
            return Start(BytesWritten);
        }

        public void Pause()
        {
            _allowedToRun = false;
        }
    }
}
