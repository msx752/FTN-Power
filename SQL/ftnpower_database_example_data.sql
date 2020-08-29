USE [ftnpowerdb]
GO
---################ fortnite login sample format of the information
INSERT INTO [dbo].[FortniteAuthTokens]
           ([Id]
           ,[access_token]
           ,[expires_in]
           ,[expires_at]
           ,[token_type]
           ,[refresh_token]
           ,[refresh_expires]
           ,[refresh_expires_at]
           ,[account_id]
           ,[client_id]
           ,[internal_client]
           ,[client_service]
           ,[app]
           ,[in_app_id]
           ,[device_id]
           ,[code])
     VALUES
           ('email@outlook.com.tr'
           ,'eg1~eyJrggN27_MbH'
           ,28800
           ,'2020-03-14T10:00:20.101Z'
           ,'bearer'
           ,'eg1~eyJraWSpefZ1'
           ,115200
           ,'2020-08-30 10:00:20.1010000'
           ,'11111111111111111111111111111111'
           ,'22222222222222222222222222222222'
           ,1
           ,'fortnite'
           ,'fortnite'
           ,'11111111111111111111111111111111'
           ,'33333333333333333333333333333333'
           ,'{  "deviceId": "4444444444444444444444",  "accountId": "1111111111111111111",  "secret": "5555555555555555555555",  "userAgent": "Fortnite/++Fortnite+Release-19.55-CL-5511245 Windows/10.0.20000.1.100.64bit",  "created": {    "location": "France",    "ipAddress": "1.2.3.4",    "dateTime": "2020-03-14T10:00:20.101Z"  }}'

		   )
---#############################################################
