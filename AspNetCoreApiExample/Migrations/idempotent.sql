CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(95) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
);


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20191201000001_InitialCreate') THEN

    CREATE TABLE `Users` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `UserName` varchar(191) CHARACTER SET utf8mb4 NOT NULL,
        `Password` varchar(191) CHARACTER SET utf8mb4 NULL,
        `LastLogin` datetime(6) NULL,
        `CreatedAt` datetime(6) NULL,
        `UpdatedAt` datetime(6) NULL,
        CONSTRAINT `PK_Users` PRIMARY KEY (`Id`)
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20191201000001_InitialCreate') THEN

    CREATE TABLE `Blogs` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Name` varchar(191) CHARACTER SET utf8mb4 NOT NULL,
        `UserId` int NOT NULL,
        `CreatedAt` datetime(6) NULL,
        `UpdatedAt` datetime(6) NULL,
        CONSTRAINT `PK_Blogs` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Blogs_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20191201000001_InitialCreate') THEN

    CREATE TABLE `Articles` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `BlogId` int NOT NULL,
        `Subject` varchar(191) CHARACTER SET utf8mb4 NOT NULL,
        `Body` longtext CHARACTER SET utf8mb4 NOT NULL,
        `CreatedAt` datetime(6) NULL,
        `UpdatedAt` datetime(6) NULL,
        CONSTRAINT `PK_Articles` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Articles_Blogs_BlogId` FOREIGN KEY (`BlogId`) REFERENCES `Blogs` (`Id`) ON DELETE CASCADE
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20191201000001_InitialCreate') THEN

    CREATE TABLE `Tags` (
        `ArticleId` int NOT NULL,
        `Name` varchar(191) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_Tags` PRIMARY KEY (`ArticleId`, `Name`),
        CONSTRAINT `FK_Tags_Articles_ArticleId` FOREIGN KEY (`ArticleId`) REFERENCES `Articles` (`Id`) ON DELETE CASCADE
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20191201000001_InitialCreate') THEN

    CREATE INDEX `IX_Articles_BlogId` ON `Articles` (`BlogId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20191201000001_InitialCreate') THEN

    CREATE INDEX `IX_Articles_CreatedAt` ON `Articles` (`CreatedAt`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20191201000001_InitialCreate') THEN

    CREATE INDEX `IX_Articles_Subject` ON `Articles` (`Subject`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20191201000001_InitialCreate') THEN

    CREATE INDEX `IX_Blogs_CreatedAt` ON `Blogs` (`CreatedAt`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20191201000001_InitialCreate') THEN

    CREATE INDEX `IX_Blogs_Name` ON `Blogs` (`Name`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20191201000001_InitialCreate') THEN

    CREATE INDEX `IX_Blogs_UserId` ON `Blogs` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20191201000001_InitialCreate') THEN

    CREATE INDEX `IX_Tags_Name_ArticleId` ON `Tags` (`Name`, `ArticleId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20191201000001_InitialCreate') THEN

    CREATE INDEX `IX_Users_CreatedAt` ON `Users` (`CreatedAt`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20191201000001_InitialCreate') THEN

    CREATE INDEX `IX_Users_LastLogin` ON `Users` (`LastLogin`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20191201000001_InitialCreate') THEN

    CREATE UNIQUE INDEX `IX_Users_UserName` ON `Users` (`UserName`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20191201000001_InitialCreate') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20191201000001_InitialCreate', '3.1.7');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;
