using cnpz_downloader.Crawler;
using HtmlAgilityPack;
using System;
using System.Threading;


class Example {
    static void Main() {
        while (true) {
            CallCrawler();
        }
    }

    static public async void CallCrawler (){
        var crawler = new Crawler();
        await crawler.CrawlUrl(new HtmlWeb(), "http://200.152.38.155/CNPJ/", @"c:\test\");
        Thread.Sleep(600000);
        Console.WriteLine("Sleep for 10 minutes.");
    }
}
