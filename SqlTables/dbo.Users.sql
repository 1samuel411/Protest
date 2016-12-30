CREATE TABLE [dbo].[Table]
(
	[index] INT NOT NULL PRIMARY KEY, 
    [sessionToken] VARCHAR(MAX) NOT NULL, 
    [facebookUserToken] VARCHAR(MAX) NULL, 
    [googleUserToken] VARCHAR(MAX) NULL, 
    [profilePicture] VARCHAR(MAX) NOT NULL, 
    [email] NVARCHAR(MAX) NOT NULL, 
    [name] NVARCHAR(50) NOT NULL, 
    [bio] NVARCHAR(MAX) NOT NULL, 
    [protestsAttended] NVARCHAR(MAX) NULL, 
    [protestsCreated] NVARCHAR(MAX) NULL, 
    [followers] NVARCHAR(MAX) NULL, 
    [following] NVARCHAR(MAX) NULL, 
    [snapchatUser] NVARCHAR(50) NULL, 
    [facebookUser] NVARCHAR(50) NULL, 
    [instagramUser] NVARCHAR(50) NULL, 
    [twitterUser] NVARCHAR(50) NULL, 
    [notifyLikesComments] BIT NOT NULL, 
    [notifyFollowers] BIT NOT NULL, 
    [notifyFollowing] BIT NOT NULL
)
