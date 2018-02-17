USE [master]
GO

/****** Object:  Database [MultiCurrencyAccountDB]    Script Date: 2/10/2018 10:52:31 PM ******/
CREATE DATABASE [BankDB] 
GO
USE [BankDB]
GO

CREATE TABLE [Account](
[Id] INT NOT NULL IDENTITY(1,1),
[AccountNumber] INT NOT NULL,
[Amount] DECIMAL(18,2) NOT NULL,
[Currency] VARCHAR(3) NOT NULL
)
GO
ALTER TABLE [Account] 
ADD CONSTRAINT PK_Account_Id PRIMARY KEY([Id])
GO
ALTER TABLE [Account] 
ADD CONSTRAINT UK_Account_AccountNumber UNIQUE([AccountNumber])
GO
ALTER TABLE [Account] 
ADD CONSTRAINT UK_Account_Currency UNIQUE([Currency])
GO
INSERT INTO [Account] VALUES(1234,0.00,'THB')

GO
CREATE TABLE [Transaction](
[Id] INT NOT NULL IDENTITY(1,1),
[AccountId] INT NOT NULL,
[Amount] Decimal(18,2) NOT NULL,
[TranasctionType] VARCHAR(3) NOT NULL,
[Currency] VARCHAR(3) NOT NULL,
[Description] VARCHAR(200) NULL
)
GO
ALTER TABLE [Transaction] 
ADD CONSTRAINT PK_Transaction_Id PRIMARY KEY([Id])
GO
ALTER TABLE [Transaction] 
ADD CONSTRAINT FK_Transaction_AccountId_Id FOREIGN KEY([AccountId]) REFERENCES Account([Id])

GO
--balance
CREATE PROCEDURE [dbo].[SP_BALANCE_AMOUNT]
@AccountNumber INT,
@Balance DECIMAL(18,2) OUT
AS
BEGIN
	SELECT @Balance= [Amount] FROM [Account] WHERE [AccountNumber]=@AccountNumber	
END
GO
--deposit
CREATE PROCEDURE [dbo].[SP_DEPOSITE_AMOUNT]
@AccountNumber INT,
@Amount DECIMAL(18,2),
@Balance DECIMAL(18,2) OUT
AS
BEGIN
	
	BEGIN TRY
		SELECT @Balance=(SELECT [Amount] FROM [Account] WHERE [AccountNumber]=@AccountNumber)+ @AMOUNT
		UPDATE [Account] SET [Amount]=@Balance
		WHERE [AccountNumber]=@AccountNumber		
	END TRY
	BEGIN CATCH
		RETURN -1
	END CATCH
END
GO
--Withdraw
CREATE PROCEDURE [dbo].[SP_WITHDRAW_AMOUNT]
@AccountNumber INT,
@Amount DECIMAL(18,2),
@Balance DECIMAL(18,2) OUT
AS
BEGIN
  BEGIN TRY  
	SELECT @Balance= (SELECT [Amount] FROM [Account] WHERE [AccountNumber]=@AccountNumber)-@AMOUNT 
	UPDATE [Account] SET [Amount]=@Balance WHERE [AccountNumber]=@AccountNumber	  	
  END TRY
  BEGIN CATCH
    RETURN -1
  END CATCH
END


GO

CREATE PROCEDURE [SP_GET_ACCOUNT]
@AccountNumber INT
AS
BEGIN
  SELECT [Id],[AccountNumber],[Amount],[Currency] FROM Account
END
GO