# Backend Prototype 1

- To try out the setup with swagger API, run:
    
```shell
dotnet run watch
```

Oauth implementation:
- Access `https://localhost:5001/home/secret`. You should receive a 401 unauthenticated error
- Access `https://localhost:5001/home/authenticate`
- Grabd the token from the access token json
- Access `https://localhost:5001/home/secret` from postman, add Authorization Bearer with the token value. You now should be able to access to see contents of this website