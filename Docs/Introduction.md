### About
This API is intended to document the available service endpoints and provide limited example usage to assist developers in building outside programs that leverage the same data sources in an effort to minimize duplicative efforts across the USGS and other agencies.

### Status
Something about the status of project
### Getting Started
The URL and sample of each resource can be obtained by accessing one of the following resources located on the sidebar (or selecting the menu button located on the bottom of the screen, if viewing on a mobile device). The krig services perform multiple procedures which include; database and geospatial queries, and spatial operations to compile and create simple responses with hypermedia enabled links to related resources. The responses are intended to be directly consumed by custom client applications and used to more fully decouple the client-service relationship by providing directional hypermedia links within the resource response objects.

The 2 matrices are multiplied together to obtain an unbiased minimum variance estimate of the correlation between the ungaged point and the referenced sites. 
Each gage is ranked by correlation value.

The Krig Services API performs multiple kriging procedures in order to compile a list of best fit correlated gages that can be consumed by custom client applications. As documented by this page, which can also serve as a simple URL builder, the Krig Services API is built following [RESTful](http://en.wikipedia.org/wiki/Representational_state_transfer) principles to ensure scalability and predictable URLs. JSON is returned by all API responses, including errors and example results and summaries for each resource. This API is intended to provide some guidance in working with the services, however some methods may only provide visual examples due to authentication requirements or limitations of the API documentation application. 

### Getting Started
The URL and sample of each resource can be obtained by accessing one of the following resources located on the sidebar (or selecting the menu button located on the bottom of the screen, if viewing on a mobile device). The NSS services perform multiple procedures which include; database and geospatial queries and spatial operations to compile and create simple responses with hypermedia enabled links to related resources. The responses are intended to be directly consumed by custom client applications and used to more fully decouple the client-service relationship by providing directional hypermedia links within the resource response objects.
### Using the API
The URL and an example response can be obtained by accessing one of the following resources and uri endpoint located on the sidebar (or selecting the menu button located on the bottom of the screen, if viewing on a mobile device). 

<img alt="sidebarImage.jpg" 
src="" />

The sidebar displays the available service resources, accompanying HTTP method and URI endpoints. Selecting a URL endpoint will display additional resource documentation information related to the selected endpoint. Such as;

The description of the resource
The Service URL.
Any URL Parameters.
Parameter Name,
Value Type,
A description of what the parameter represents,
Whether the parameter is required or optional,
A REST Query URL test tool is also available that builds an example url, based on the user provided input parameter values and a simple query tool that provides an example of the requested response.

### Common Status Codes
Common HTTP status codes returned by the services

| Code &nbsp; &nbsp; &nbsp;   |When     
| ------- |---------
| `200`   | Status  `OK`.
| `400`   | Status `Bad Request`. When the request data cannot be read.
| `404`   | Status `Not Found`. When resource is not available.
| `500`   | Status `Internal Server Error`. Please contact the administrator. 

### Example
Web service request can be performed using most HTTP client libraries. The following illustrated a typical http request/response performed by a client application.

```
GET /TravelTimeservices HTTP/1.1
Host: streamstats.usgs.gov
Accept: application/json
```
```
HTTP/1.1 200 OK

  
```
