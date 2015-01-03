import oauth2 as oauth
import httplib2
import time, os, simplejson
 
# Fill the keys and secrets you retrieved after registering your app
consumer_key      =   'npq2v0qgrl9y'
consumer_secret  =   '50DUgVK6Cl6RwMRy'
user_token           =   '2bec4d34-928c-46b4-99b7-27db00bd2fec'
user_secret          =   '93e725be-186d-4f65-9ae6-b21f62d709a5'
 
# Use your API key and secret to instantiate consumer object
consumer = oauth.Consumer(consumer_key, consumer_secret)
 
# Use the consumer object to initialize the client object
client = oauth.Client(consumer)
 
# Use your developer token and secret to instantiate access token object
access_token = oauth.Token(
            key=user_token,
            secret=user_secret)
 
client = oauth.Client(consumer, access_token)
 
# Make call to LinkedIn to retrieve your own profile
resp,content = client.request("https://api.linkedin.com/v1/people/~", "GET", "")
 
# By default, the LinkedIn API responses are in XML format. If you prefer JSON, simply specify the format in your call
# resp,content = client.request("https://api.linkedin.com/v1/people/~?format=json", "GET", "")
