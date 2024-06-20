# EasyBook

## Theme of the project
Web API for an online bookstore where users can browse, search and
shop for books. The store will contain a catalogue of currently available books.
User can browse or search them by title/genre/etc. The user can then
can place an order. There will also be reviews for each product and
ratings for product comparison.

## Description of the implementation approach
ASP.Net Core is used to implement this API. Lightweight SQLite is used as database. JWT usage is implemented for the authentication method and
authorization of API users.

The API will have the following features:
- Item Book Management (CRUD)
- Order item management (CRUD + status tracking)
- Manage User items ([CRUD for administrators and regular users] + registration /
authorization))
- Ability to create a review for an item
- Authorization, supporting the following &quot;JWT handshake&quot;
- Search for items by name/author/genre
