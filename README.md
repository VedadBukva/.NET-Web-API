# Simple blogging platform - Web API
The goal of this project is to implement a backend solution for a simple blogging platform. It uses a custom REST API for all requests. Any client should be able to use this API.<br>
The API is written in ASP.NET Core using the MVC architecture 

## General functionality
- CRUD Blog posts
- Filter blog posts by tag
- List of all tags in the system

## Database setup
For purpose of this project was used SQLite database which is integrated in project<br>
Database is filled with initial data

## How to run? 
ALERT: If you don't have installed .NET Core 3.1.0 SDK, please download it: https://dotnet.microsoft.com/download/dotnet/3.1
Load project folder in Visual Studio Code (or just open terminal with git functionalities in root of project folder) and run the ASP.NET Core application with the following line of code in terminal:
```
dotnet run
```
After successful start of app in the terminal will be visible URL of localhost where we can test functionalities of this API<br>
Link of started app will be http://localhost:5000 or http://localhost:5001

## Endpoints
### Get Blog Post
```
GET /api/posts/:slug
```
Will return single blog post

### List Blog Posts
```
GET /api/posts
```
Returns most recent blog posts by default, optionally provide tag query parameter to filter results.<br>
Filter by tag: `?tag=AngularJS`

### Create Blog Post
```
POST /api/posts
```
Example request body:
```
{
  "blogPost": {
    "title": "Internet Trends 2018",
    "description": "Ever wonder how?",
    "body": "An opinionated commentary, of the most important presentation of the year",
    "tagList": ["trends", "innovation", "2018"]
  }
}
```
Will return a blog post.<br>
Required fields: title, description, body.<br>
Optional fields: tagList as an array of strings.<br>

### Update Blog Post
```
PUT /api/posts/:slug
```
Example request body:
```
{
  "blogPost": {
    "title": "React Why and How?"
  }
}
```
Returns the updated blog post.<br>
Optional fields: title, description, body<br>
The slug also gets updated when the title is changed.<br>

### Delete Blog Post
```
DELETE /api/posts/:slug
```

### Get Tags
```
GET /api/tags
```
Returns a list of all tags in db.

## Testing
Testing was worked in app Postman
