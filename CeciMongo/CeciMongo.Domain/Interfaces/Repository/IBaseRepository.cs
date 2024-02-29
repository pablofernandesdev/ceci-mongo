using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CeciMongo.Domain.Interfaces.Repository
{
    /// <summary>
    /// Represents a generic repository interface for CRUD operations on documents.
    /// </summary>
    /// <typeparam name="TDocument">The type of document to be stored in the repository, must implement <see cref="IDocument"/>.</typeparam>
    public interface IBaseRepository<TDocument> where TDocument : IDocument
    {
        /// <summary>
        /// Gets an <see cref="IQueryable{T}"/> for querying documents.
        /// </summary>
        /// <returns>An <see cref="IQueryable{T}"/> for querying documents.</returns>
        IQueryable<TDocument> AsQueryable();

        /// <summary>
        /// Filters documents based on the provided filter expression.
        /// </summary>
        /// <param name="filterExpression">The filter expression to apply.</param>
        /// <returns>A collection of documents that match the filter expression.</returns>
        IEnumerable<TDocument> FilterBy(Expression<Func<TDocument, bool>> filterExpression);

        /// <summary>
        /// Asynchronously filters documents based on the provided filter expression.
        /// </summary>
        /// <param name="filterExpression">The filter expression to apply.</param>
        /// <param name="page">The page number for pagination.</param>
        /// <param name="perPage">The number of documents per page for pagination.</param>
        /// <returns>A collection of documents that match the filter expression.</returns>
        Task<IEnumerable<TDocument>> FilterByAsync(Expression<Func<TDocument, bool>> filterExpression, int page, int perPage);

        /// <summary>
        /// Asynchronously filters documents based on the provided filter expression.
        /// </summary>
        /// <param name="filterExpression">The filter expression to apply.</param>
        /// <returns>A collection of documents that match the filter expression.</returns>
        Task<IEnumerable<TDocument>> FilterByAsync(Expression<Func<TDocument, bool>> filterExpression);

        /// <summary>
        /// Filters and projects documents based on the provided filter and projection expressions.
        /// </summary>
        /// <typeparam name="TProjected">The type of the projected result.</typeparam>
        /// <param name="filterExpression">The filter expression to apply.</param>
        /// <param name="projectionExpression">The projection expression to apply.</param>
        /// <returns>A collection of projected results that match the filter expression.</returns>
        IEnumerable<TProjected> FilterBy<TProjected>(Expression<Func<TDocument, bool>> filterExpression, Expression<Func<TDocument, TProjected>> projectionExpression);

        /// <summary>
        /// Finds a single document based on the provided filter expression.
        /// </summary>
        /// <param name="filterExpression">The filter expression to apply.</param>
        /// <returns>The found document or null if not found.</returns>
        TDocument FindOne(Expression<Func<TDocument, bool>> filterExpression);

        /// <summary>
        /// Asynchronously finds a single document based on the provided filter expression.
        /// </summary>
        /// <param name="filterExpression">The filter expression to apply.</param>
        /// <returns>The found document or null if not found.</returns>
        Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression);

        /// <summary>
        /// Finds a document by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the document.</param>
        /// <returns>The found document or null if not found.</returns>
        TDocument FindById(string id);

        /// <summary>
        /// Asynchronously finds a document by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the document.</param>
        /// <returns>The found document or null if not found.</returns>
        Task<TDocument> FindByIdAsync(string id);

        /// <summary>
        /// Counts the number of documents that match the provided filter expression.
        /// </summary>
        /// <param name="filterExpression">The filter expression to apply.</param>
        /// <returns>The number of documents that match the filter expression.</returns>
        Task<long> CountByFilterAsync(Expression<Func<TDocument, bool>> filterExpression);

        /// <summary>
        /// Inserts a single document into the repository.
        /// </summary>
        /// <param name="document">The document to insert.</param>
        void InsertOne(TDocument document);

        /// <summary>
        /// Asynchronously inserts a single document into the repository.
        /// </summary>
        /// <param name="document">The document to insert.</param>
        Task InsertOneAsync(TDocument document);

        /// <summary>
        /// Inserts multiple documents into the repository.
        /// </summary>
        /// <param name="documents">The collection of documents to insert.</param>
        void InsertMany(ICollection<TDocument> documents);

        /// <summary>
        /// Asynchronously inserts multiple documents into the repository.
        /// </summary>
        /// <param name="documents">The collection of documents to insert.</param>
        Task InsertManyAsync(IEnumerable<TDocument> documents);

        /// <summary>
        /// Replaces an existing document with the provided one.
        /// </summary>
        /// <param name="document">The document to replace the existing one.</param>
        void ReplaceOne(TDocument document);

        /// <summary>
        /// Asynchronously replaces an existing document with the provided one.
        /// </summary>
        /// <param name="document">The document to replace the existing one.</param>
        Task ReplaceOneAsync(TDocument document);

        /// <summary>
        /// Deletes a single document based on the provided filter expression.
        /// </summary>
        /// <param name="filterExpression">The filter expression to apply.</param>
        void DeleteOne(Expression<Func<TDocument, bool>> filterExpression);

        /// <summary>
        /// Asynchronously deletes a single document based on the provided filter expression.
        /// </summary>
        /// <param name="filterExpression">The filter expression to apply.</param>
        Task DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression);

        /// <summary>
        /// Deletes a document by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the document to delete.</param>
        void DeleteById(string id);

        /// <summary>
        /// Asynchronously deletes a document by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the document to delete.</param>
        Task DeleteByIdAsync(string id);

        /// <summary>
        /// Deletes multiple documents based on the provided filter expression.
        /// </summary>
        /// <param name="filterExpression">The filter expression to apply.</param>
        void DeleteMany(Expression<Func<TDocument, bool>> filterExpression);

        /// <summary>
        /// Asynchronously deletes multiple documents based on the provided filter expression.
        /// </summary>
        /// <param name="filterExpression">The filter expression to apply.</param>
        Task DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression);
    }
}