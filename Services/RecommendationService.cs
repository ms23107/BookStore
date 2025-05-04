using System;
using System.Collections.Generic;
using System.Linq;
using BookStore.Data;
using BookStore.Models;

namespace BookStore.Services
{
    public class RecommendationService
    {
        private readonly BookStoreContext _context;
        public RecommendationService(BookStoreContext context)
        {
            _context = context;
        }
        public List<Book> GetRecommendedBooks(int bookId, int topN = 5)
        {
            var selectedBook = _context.Books.Find(bookId);
            if (selectedBook == null || string.IsNullOrEmpty(selectedBook.Description))
            {
                return new List<Book>();
            }
            var allBooks = _context.Books
                .Where(b => b.Id != bookId && !string.IsNullOrEmpty(b.Description))
                .ToList();
            var tfidfVectors = allBooks
                .Select(b => new
                {
                    Book = b,
                    Vector = CalculateTfIdfVector(b.Description, allBooks)
                })
                .ToList();
            var selectedBookVector = CalculateTfIdfVector(selectedBook.Description, allBooks);
            var similarities = tfidfVectors
                .Select(b => new
                {
                    Book = b.Book,
                    Similarity = CosineSimilarity(selectedBookVector, b.Vector)
                })
                .OrderByDescending(b => b.Similarity)
                .Take(topN)
                .Select(b => b.Book)
                .ToList();
            return similarities;
        }
        private Dictionary<string, double> CalculateTfIdfVector(string description, List<Book> allBooks)
        {
            var words = description.Split(new[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
            var tf = words.GroupBy(w => w).ToDictionary(g => g.Key, g => (double)g.Count() / words.Length);
            var idf = new Dictionary<string, double>();
            foreach (var word in tf.Keys)
            {
                int docsContainingWord = allBooks.Count(b => b.Description != null && b.Description.Contains(word));
                idf[word] = Math.Log((double)allBooks.Count / (1 + docsContainingWord));
            }
            var tfidf = tf.ToDictionary(t => t.Key, t => t.Value * idf[t.Key]);
            return tfidf;
        }
        private double CosineSimilarity(Dictionary<string, double> vectorA, Dictionary<string, double> vectorB)
        {
            var intersection = vectorA.Keys.Intersect(vectorB.Keys);
            if (!intersection.Any())
            {
                return 0;
            }
            double dotProduct = intersection.Sum(k => vectorA[k] * vectorB[k]);
            double magnitudeA = Math.Sqrt(vectorA.Values.Sum(v => Math.Pow(v, 2)));
            double magnitudeB = Math.Sqrt(vectorB.Values.Sum(v => Math.Pow(v, 2)));
            return dotProduct / (magnitudeA * magnitudeB);
        }
    }
}