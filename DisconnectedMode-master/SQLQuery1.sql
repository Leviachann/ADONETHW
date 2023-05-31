CREATE DATABASE Library;
GO
USE [Library];
CREATE TABLE Authors
(
    AuthorID INT IDENTITY(1,1) PRIMARY KEY,
    AuthorName VARCHAR(50) NOT NULL
);