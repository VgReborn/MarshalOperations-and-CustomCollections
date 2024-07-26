# MarshalOperations-and-CustomCollections
If you are using old Unity versions or old Net Version (not recommended) and want to use Span? Here is some functionalities that can help with that

##CustomCollections
A separate version of List. Similar to a list respectively, but this version allows you to directly access the _items without 
reflections to get the array. Goes on par with VCE.MarshalOperations. It is created for performance purposes and will not
not have much flexibility compared to a list, but gets rid of certain isssues

##MarshalOperations
This is a workaround for CollectionsMarshal. An unsafe class so it is hidden by default
