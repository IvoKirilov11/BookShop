﻿namespace BookShop.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportMostCraziestAuthors(BookShopContext context)
        {
            var authors = context.Authors
                .Select(x => new
                {
                    AuthorName = x.FirstName + ' ' + x.LastName,
                    Books = x.AuthorsBooks
                    .OrderByDescending(b => b.Book.Price)
                    .Select(b => new
                    {
                        BookName = b.Book.Name,
                        BookPrice = b.Book.Price.ToString("F2")
                    })

                    .ToArray()
                })
                .ToArray()
                .OrderByDescending(x => x.Books.Length)
                .ThenBy(a => a.AuthorName)
                .ToArray();

                string result = JsonConvert.SerializeObject(authors, Formatting.Indented);

            return result;

        }

        public static string ExportOldestBooks(BookShopContext context, DateTime date)
        {
            var oldBook = context.Books
                .Where(x => x.PublishedOn < date && x.Genre == Genre.Science)
                .ToArray()
               .OrderByDescending(x => x.Pages)
               .ThenByDescending(x => x.PublishedOn)
               .Take(10)
                .Select(x => new OldesBookOutputModel
                {
                    Name = x.Name,
                    Date = x.PublishedOn.ToString("d", CultureInfo.InvariantCulture),
                    Pages = x.Pages

                })
               
               .ToArray();

            var xml = XmlConverter.Serialize(oldBook, "Books");

            return xml;

        }
    }
}