CREATE TABLE "Files" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Files" PRIMARY KEY AUTOINCREMENT,
    "UniqueId" TEXT NOT NULL,
    "Path" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Size" INTEGER NOT NULL,
    "ContentChecksum" TEXT NOT NULL,
    "DateTimeCreated" TEXT NOT NULL,
    "DateTimeModified" TEXT NOT NULL
);


CREATE TABLE "FileChanges" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_FileChanges" PRIMARY KEY AUTOINCREMENT,
    "TransactionId" TEXT NOT NULL,
    "Created" TEXT NOT NULL,
    "FileId" INTEGER NOT NULL,
    CONSTRAINT "FK_FileChanges_Files_FileId" FOREIGN KEY ("FileId") REFERENCES "Files" ("Id") ON DELETE CASCADE
);


CREATE TABLE "ChangeEvent" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ChangeEvent" PRIMARY KEY AUTOINCREMENT,
    "Type" INTEGER NOT NULL,
    "Complete" INTEGER NOT NULL,
    "Order" INTEGER NOT NULL,
    "TransactionId" INTEGER NOT NULL,
    CONSTRAINT "FK_ChangeEvent_FileChanges_TransactionId" FOREIGN KEY ("TransactionId") REFERENCES "FileChanges" ("Id") ON DELETE CASCADE
);


CREATE INDEX "IX_ChangeEvent_TransactionId" ON "ChangeEvent" ("TransactionId");


CREATE UNIQUE INDEX "IX_FileChanges_FileId" ON "FileChanges" ("FileId");


