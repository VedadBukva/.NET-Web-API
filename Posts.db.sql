BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
	"MigrationId"	TEXT NOT NULL,
	"ProductVersion"	TEXT NOT NULL,
	CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY("MigrationId")
);
CREATE TABLE IF NOT EXISTS "Posts" (
	"Slug"	TEXT NOT NULL,
	"Title"	TEXT NOT NULL,
	"Description"	TEXT NOT NULL,
	"Body"	TEXT NOT NULL,
	"TagList"	TEXT,
	"CreatedAt"	TEXT NOT NULL,
	"UpdatedAt"	TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS "Tags" (
	"Slug"	TEXT NOT NULL,
	"TagDescription"	TEXT NOT NULL
);
INSERT INTO "__EFMigrationsHistory" VALUES ('20210422144456_InitialMigration','5.0.5');
INSERT INTO "Posts" VALUES ('example','exampleTitle','exampleDescription','exampleBody','','created','updated');
INSERT INTO "Posts" VALUES ('example1','exampleTitle1','description','body','','created','updated');
INSERT INTO "Tags" VALUES ('example','postTag');
COMMIT;
