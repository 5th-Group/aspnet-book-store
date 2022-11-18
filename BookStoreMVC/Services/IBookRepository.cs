using BookStoreMVC.Models;
using BookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreMVC.Services;

public interface IBookRepository
{
    IEnumerable<Book> GetAll(string filter);

    Book GetById(string bookId);

    // IActionResult Add(Book book);
    
    Task AddAsync(Book book);

    // IActionResult Update(Book book);

    Task UpdateAsync(Book book);

    // IActionResult Delete(string bookId);

    Task DeleteAsync(string bookId);
    
}