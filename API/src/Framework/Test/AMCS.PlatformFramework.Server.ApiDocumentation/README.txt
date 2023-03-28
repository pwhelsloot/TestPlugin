This project contains the Markdown documentation for the REST API.

This project is a collection of Markdown documents that will be embedded into the
Swagger documentation automatically based on the directrory structure in which the
documents are placed.

Matching is done as follows. Let's say there is an entity mapped to the route
/customer/activeResidentialSalesOrders. To add documentation to this, you'd
create the document Customer\ActiveResidentialOrders.md. The contents of that
document would then be added to all REST endpoints of that entity.

If you only want to add a document for a specific HTTP method, instead of creating
the document above, you'd e.g. create a document named
Customer\ActiveResidentialOrders\POST.md. Entity endpoints will also have a route
named template, e.g. /customer/activeResidentialSalesOrders/template. If a document
named Customer\ActiveResidentialOrders.md exists, that would also be added to the
template route. However, if you want to have a specific document for that instead,
you'd create a document named Customer\ActiveResidentialOrders\Template.md instead.

Basically the algorithm is as follows:

* Take the route;
* Check whether there's a document named route\method.md;
* Check whether there's a document named route.md;
* If none exist, take of the last part of the route and retry.

Note that this project automatically includes all documents available on disk.
The only thing you have to do to add a new document is create the file on disk. You
do not have to manually include the document in the project file.

To author the Markdown documents, try Typora from https://www.typora.io. It's a
pretty decent editor.

Most Markdown features are supported by Swagger. You should just be able to add
images and tables etc. if necessary.