# MarshalOperations-and-CustomCollections
If you are using old Unity versions or old Net Version (not recommended) and want to use Span? Here is some functionalities that can help with that

## CustomCollections
A separate version of List. Similar to a list except this version allows you to directly access the _items without 
reflections to get the array--although this one is much more barebones. Goes on par with VCE.MarshalOperations. It is created for performance purposes and will not
 have much flexibility and functionality compared to list. 

## MarshalOperations
This is a workaround for CollectionsMarshal. An unsafe class so just becareful. 

## Usability
These scripts were created when I was trying to find a better way to use SPAN in Unity, but Unity uses a much older framework (or at least the time I created it). I thought it would be a good idea to release this code in its barebone
state so that anyone is able to put in their own functionalities and such. I hope these scripts are able to help assist in your development. 
