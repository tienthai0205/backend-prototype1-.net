# Backend Prototype 1

## Reservation Api

*Note: Run project in Vscode instead of Visual Studio.

To start up the server:

```csharp
dotnet watch run
```

The launchUrl is set to `[localhost/api/users/seed](http://localhost/api/users/seed)` : for testing purpose, I have setup a few seed data ( can be found in `Controllers/UsersController, Seed`). I have tried to seed data with DbContext using HasData() but through some searching, this only seems to work with real db and not InMemory ones.

The seed url then right away redirect to `/api/users`

This route need proper authorization to get access to. You should be receiving `401` after being redirected here without login

### Authentication and authorization flow

1. Login 
    - With login, please use the seed credentials provided in the UserController. They are

        ```csharp
        {
        	"Email":"tien@saxion.nl",
        	"Password": "Tien12345"
        }
        or
        {
        	"Email": "max@saxion.nl",
        	"Password": "Max12345"
        }
        ```

        *Login url*: POST `[https://localhost:5001/api/users/login](https://localhost:5001/api/users/login)` 

        *Response body:* 

        ```csharp
        200 OK 
        {
        	"access_token": <jwt_token_here>,
        	"email": <logged_in_email>
        }
        ```

2. Register

    *Register url:* POST `[https://localhost:5001/api/users](https://localhost:5001/api/users/login)` 

    Sample request body:

    ```csharp
    {
    	"Email": "gerralt@saxion.nl",
    	"Password": "VerySecuredPassword"
    }
    ```

    The Request body takes in a JSon object of type User, so `Email` and `Password` properties are string data type. No need to specify `Id` as this is auto incremented by the server. Provided `Id` property in request body will be ignored

### Update and Delete user account

- In Postman, choose Authorization type as Oauth 2.0 if using Authorization tab. If using Headers, specify Key="Authorization" and Value=Bearer <your_access_token> (**space** between `Bearer` and `access_token`)
- update user account:
    - url: PUT `https://localhost:5001/api/users/<your_user_id>`
    - Response body:
        - 200 OK: User successfully updated

            ```csharp
            {
            	"Id": "<your_current_id>",
            	"Email": "<updated_email_here>",
            	"Password": "<updated_password_here>"
            }
            ```

        - 401 Unauthorized: When user tries to update other user information

            ```csharp
            { "error": "You are not authorized to update other user accounts!"}
            ```

        - 400 BadRequest: id provided in url and request body doesn't match

            ```csharp
            {"error": "The provided ids don't match"}
            ```

    - Delete user account
        - url: DELETE `https://localhost:5001/api/users/<your_user_id>`
        - Response body:
            - 200 OK: User successfully deleted

                ```csharp
                {
                	"message" : "Your account has been removed!"
                }
                ```

            - 401 Unauthorized: When user tries to delete other user information

                ```csharp
                { "error": "You are not authorized to delete other user accounts!"}
                ```

            - 404 User not found: id provided in url doesn't match any ids

                ```csharp
                {"error": "User not found!"}
                ```

### Rooms

- Get list of free rooms on date (without authorization needed):
    - url: GET  `https://localhost:5001/api/rooms/filter?date=<date>`
    - example: `https://localhost:5001/api/rooms/filter?date=02-05-2021`
- Get list of all rooms with detailed reservation (bearer with access_token needed)
    - url: GET `https://localhost:5001/api/rooms`

### Reservation (bearer with access_token needed) - authorized users only

- Get list of current user's reservations :
    - url: GET `https://localhost:5001/api/reservations`
- Update a reservation:
    - url: PUT `https://localhost:5001/api/reservations/<id>`
- Create a reservation:
    - url: POST `https://localhost:5001/api/reservations`
- Delete a reservation:
    - url: DELETE `https://localhost:5001/api/reservations/<id>`