using cnpz_downloader.Crawler;
using HtmlAgilityPack;

var crawler = new Crawler();
await crawler.CrawlUrl(new HtmlWeb(), "http://200.152.38.155/CNPJ/", @"c:\test\");