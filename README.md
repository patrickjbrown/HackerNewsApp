# Hacker News Viewer

This project was written as part of an interview coding challenge. It provides an Angular-based client and a .NET Core based server to display stories using the [Hacker News API](https://github.com/HackerNews/API).

## Task

The requirements of the coding challenge were as follows (descriptions modified to prevent cheating/Google-ability):

For the UI:
* Create a solution that allows for viewing of the newest stories from the HN feed using Angular.
* The UI must display a paginated list of the newest stories.
* Stories must include a title and link to the article.
* A search function must be implemented.
* Unit test must be included.

For the back-end:
* Create a RESTful C# and .NET Core based backend to obtain stories from the feed.
* Cache stories to cut down on service calls.
* Make use of dependency injection.
* Include automated tests.

## Skills / Concepts / Technologies

This project showcases the following skills, concepts, and technologies:

* C#
* .NET Core 9.0
* .NET Core MVC
* Angular 19
* Swagger
* REST APIs
* Unit testing / NSubstitute
* JavaScript / TypeScript
* HTML / CSS / JSON

## Resources

The following non-Microsoft NuGet packages were included in this solution:

1. Refit: Used to reduce the overhead of making REST API calls.
2. SwaggerUI: UI for quick manual testing of REST services.
3. NSubstitute: Unit test mocking framework.

The project "skeleton" was generated using Visual Studio's built-in "Angular and ASP.NET Core" project template. No AI was used in the completion of this coding challenge.