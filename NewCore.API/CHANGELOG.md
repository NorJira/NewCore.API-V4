
# NewCore.API V4 - Change Log

History of changes to newcore.api, newcore.dtos, newcore.services, newcore.data


## V4.01 (26/10/2021)

- Add SeriLog to each controller/action to keep track of user activities


## V4.00 (25/10/2021)

- Add JWTIdentity ClassLib to V3.0
- Add ModelState checking to every controller/actions to validate input parameters from the front-end app and send any error back for better problem tracking
- Update all controller/action to use ActionResult<> to return the response value and response status code to the front-end app

