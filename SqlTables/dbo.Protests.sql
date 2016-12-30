CREATE TABLE [dbo].[Protests] (
    [index]           INT        NOT NULL,
    [protestPicture]  VARCHAR(MAX) NULL,
    [name]            NVARCHAR(MAX) NOT NULL,
    [description]     NVARCHAR(MAX) NULL,
    [location]        NVARCHAR(MAX) NOT NULL,
    [date]            VARCHAR(50) NOT NULL,
    [donationsEmail]  NVARCHAR(MAX) NULL,
    [donationCurrent] INT NULL,
    [donationTarget]  INT NULL,
    [likes]           VARCHAR(MAX) NULL,
    [going]           VARCHAR(MAX) NULL,
    [contributions]   VARCHAR(MAX) NULL,
    [chats]           VARCHAR(MAX) NULL,
    [userCreated]     INT NOT NULL,
    PRIMARY KEY CLUSTERED ([index] ASC)
);

