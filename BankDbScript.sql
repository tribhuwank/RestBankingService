USE [master]
GO

/****** Object:  Database [MultiCurrencyAccountDB]    Script Date: 2/10/2018 10:52:31 PM ******/
DROP DATABASE [BankDB]
GO

/****** Object:  Database [MultiCurrencyAccountDB]    Script Date: 2/10/2018 10:52:31 PM ******/
CREATE DATABASE [BankDB] 
GO
USE [BankDB]
GO
CREATE TABLE [Exchange_Rate](
[CurrencyCode] VARCHAR(3) NOT NULL,
[Date] [datetime] NOT NULL,
[Unit] INT NOT NULL,
[Rate] Decimal(18,2) NOT NULL
)
GO
ALTER TABLE [Exchange_Rate] 
ADD CONSTRAINT PK_Exchange_Rate_CurrencyCode_Date PRIMARY KEY([CurrencyCode],[Date])

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
@AccountNumber INT

AS
BEGIN
	SELECT Amount FROM Account WHERE AccountNumber=@AccountNumber	
END
GO
--deposit
CREATE PROCEDURE [dbo].[SP_DEPOSITE_AMOUNT]
@AccountNumber INT,
@Currency VARCHAR(3),
@Amount DECIMAL(18,2),
@Balance DECIMAL(18,2) OUT
AS
BEGIN
	
	BEGIN TRY
		SELECT @Balance=(SELECT Amount FROM Account WHERE AccountNumber=@AccountNumber)+(@AMOUNT*(SELECT (Unit*Rate)/Unit FROM Exchange_Rate WHERE CurrencyCode=@Currency AND  [Date] = @@DATEFIRST))
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
@Currency VARCHAR(3),
@Amount DECIMAL(18,2),
@Balance DECIMAL(18,2) OUT
AS
BEGIN
  BEGIN TRY
	SELECT @Balance= (SELECT Amount FROM Account WHERE AccountNumber=@AccountNumber)-(@AMOUNT*(SELECT (Unit*Rate)/Unit FROM Exchange_Rate WHERE CurrencyCode=@Currency AND  [Date] = @@DATEFIRST) )
	UPDATE [Account] SET [Amount]=@Balance WHERE [AccountNumber]=@AccountNumber	  	
  END TRY
  BEGIN CATCH
    RETURN -1
  END CATCH
END


GO

