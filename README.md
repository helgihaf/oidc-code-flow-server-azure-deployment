# OpenID Connect Code Flow Server Deployment
[![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://azuredeploy.net/)   
## What is this repository?
There are two types of Open ID connect flows, implicit and code flow. I am not going to go over in detail what the difference between these flows are, you can read upon it here [Connect2id article](https://connect2id.com/learn/openid-connect).
This repo is a one click deployment of the server part of the openid connect code flow. For now I only support deployments to Azure but that might change. This deployment uses the nuget package [Nuget CodeFlowServer](https://www.nuget.org/packages/Landsbanki.Authentication.OidcCodeFlowServer/) and is open source, the source is here [GitHub CodeFlowServer](https://github.com/landsbankinn/oidc-code-flow-server).   
This server also handles refresh token logins.

## Code Endpoint
The code endpoint ( accepts GET requests ) takes two query parameters. They are   
- `code` The code from the authorization server
- `redirect_url` The redirect url to you application where the access, id and refresh token should be sent

You define what the relative url to it is in the deployment parameters, see parameters [here](#Parameters)

## Refresh Endpoint
The refresh endpoint ( accepts POST requests ) accepts a body that has two fields. They are   
- `refesh_token` The refresh token to authenticate with
- `scope` Space delimitered list of scopes to ask for in the authentication

## Refresh Endpoint Authentication
You can optionally have a basic auth on the refresh token endpoint. You enable it in the deployment parameters, see parameters [here](#Parameters)   
You have to set UseAuthorizationOnRefreshEndpoint to true and provide a Username and Password parameters in the deployment parameters.

## Validation
You can optionally validate both the access token and ID token with this server. You specify it in the parameters in the deployment, see parameters [here](#Parameters).   
The validation is done by using the JWKS endpoint on the authentication server.   
To validate the Access token you will have to set ValidateAccessToken to true, and provide the Issuer, audience and AccessTokenJwksEndpoint
To validate the Id token you will have to set ValidateIdToken to true, and provide the Issuer, audience and IdTokenJwksEndpoint

## Parameters
__There is a gotcha that you need to be aware of. ARM templates do not let you pass a empty string, so when ever a value is not needed, just use \*__
There are alot of parameters to be set in this deployment. The underlying nuget package is a generic OpenID connect code flow server so you will have to point it to the Authorization server and provide the Issuer etc.   
This deployment most of the parameters are aleary set for you and point to the sandbox authentication and apis.   
Lets go over what each of these values are

| Parameter      | Description |
|---------------|:-------------:|
| RelativeCallbackEndpoint  | This will be the relative path to your callback endpoint. So the url will be some thing like this https://<SITENAME>.azurewebsites.net/<RelativeCallbackEndpoint> |
| RelativeRefreshEndpoint | This will be the relative path to your refresh authorization endpoint. So the url will be some thing like this https://<SITENAME>.azurewebsites.net/<RelativeRefreshEndpoint> |
| TokenEndpoint | The token endpoint of your Authentication server |
| ClientId | The client id of the for the server |
| ClientSecret | The client secret of the for the server |
| ValidateAccessToken | Sets whether or not to validate access token |
| ValidateIdToken | Sets whether or not to validate id token |
| UseAuthorizationOnRefreshEndpoint | Set whether or not to use basic auth on the refresh authentication endpoint |
| Issuer | The issuer of the token, your Authentication server. Used in the validation of the Access and ID token |
| Audience | The audience of the token. This is set by you Authentication server. Used in the validation of the Access and ID token |
| AccessTokenJwksEndpoint | The Access token JWKS endpoint. The server will pick up the tokens public cert and verify the signature of the token |
| IdTokenJwksEndpoint | The ID token JWKS endpoint. The server will pick up the webkey and verify the signature of the ID token from here. |
| Username | The username used in the basic auth on the refresh authentication endpoint |
| Password | The password used in the basic auth on the refresh authentication endpoint |
| AllowedCORSHeaders | Allowed CORS Headers on the server. Wildcard \* are allowed |
| AllowedCORSOrigins | Allowed CORS Origins on the server. Wildcard \* are allowed |

The only parameters that you will have to provide your self are, `ClientId`, `ClientSecret` and `AllowedCORSOrigins`. Although wildcard is allowed on AllowedCORSOrigins parameters, we highly recomend that you use your origin here.

## Production
In most cases you would want to write your own server for production to gain more control of the process. You could use the underlying nuget package for that implementation if you so choose. However, there is nothing stoping you from using this deployment for production if that fits your need. There are only a few changes that you will have to change. They are listed here below.   
`TokenEndpoint`: `https://auth.landsbankinn.is/as/token.oauth2`   
`Issuer`: `https://auth.landsbankinn.is`   
`Audience`: `https://api.landsbankinn.is`   
`AccessTokenJwksEndpoint`: `https://auth.landsbankinn.is/ext/oauth/jwks`   
`IdTokenJwksEndpoint`: `https://auth.landsbankinn.is/pf/JWKS`   
And you will have to provide a production `ClientId` and `ClientSecret`