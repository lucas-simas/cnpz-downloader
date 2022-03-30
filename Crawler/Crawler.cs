using HtmlAgilityPack;
using System.ComponentModel;
using System.Net;
using System.Text.RegularExpressions;
using System;
using System.IO;
using cnpz_downloader.FileDownloader;
using cnpz_kafka.GenericKafka;

namespace cnpz_downloader.Crawler
{
    public class Crawler
    {

        public async Task CrawlUrl(HtmlWeb web, string url, string path)
        {

            var doc = web.Load(url);
            var links = doc.DocumentNode.SelectNodes("//a[@href]");
            var regex_download = new Regex(@"(\S*)\.(\S*)\.(D\w*)\.(\S*).((.zip)|(.ZIP))");
            var pasta_criada = false;
            string pasta_nome = "";
            List<Task> tasks = new List<Task>();

            foreach (var link in links) {

                try {

                    if (regex_download.IsMatch(link.Attributes["href"].Value)) {

                        //Verificando expressão regular para validar o nome do arquivo
                        var reg_info = regex_download.Matches(link.Attributes["href"].Value);
						if ( !pasta_criada && reg_info.Count > 0 && reg_info[0].Groups.Count > 3 ) {
                            var grupos = reg_info[0].Groups;
                            var agrupador = reg_info[0].Groups[3].Value;
                            pasta_nome = agrupador;
                            path += agrupador + @"\";

                            if ( !Directory.Exists(path) ) {
                                Directory.CreateDirectory(path);
                            }
                            pasta_criada = true;
                        }

                        Console.WriteLine("Iniciou: " + url + link.Attributes["href"].Value + " - Salvando: " + path);
                        var arquivo_atual = url + link.Attributes["href"].Value;

                        var fd = new FileDownload(arquivo_atual, path + link.Attributes["href"].Value, 20000);
                        tasks.Add(fd.Start());
                    }

                }
                catch (Exception) {

                    Console.WriteLine("Fudeu ruim: " + link.Attributes["href"].Value);

                }
            }

            await Task.WhenAll(tasks);

			Producer.SendKafka("downloader", pasta_nome);
		}

    }

}