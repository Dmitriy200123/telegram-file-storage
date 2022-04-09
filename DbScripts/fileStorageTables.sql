create table "Chats"
(
    "Id"         uuid         not null
        constraint "PK_Chats"
            primary key,
    "TelegramId" bigint       not null
        constraint "AK_Chats_TelegramId"
            unique,
    "Name"       varchar(255) not null,
    "ImageId"    uuid
);

alter table "Chats"
    owner to "FileStorageApp";

create table "FileSenders"
(
    "Id"               uuid         not null
        constraint "PK_FileSenders"
            primary key,
    "TelegramId"       bigint       not null
        constraint "AK_FileSenders_TelegramId"
            unique,
    "TelegramUserName" varchar(255) not null,
    "FullName"         varchar(255) not null
);

alter table "FileSenders"
    owner to "FileStorageApp";

create table "Users"
(
    "Id"           uuid    not null
        constraint "PK_Users"
            primary key,
    "TelegramId"   bigint,
    "GitLabId"     integer not null,
    "Name"         text    not null,
    "Avatar"       text    not null,
    "RefreshToken" text    not null
);

alter table "Users"
    owner to "FileStorageApp";

create table "Files"
(
    "Id"           uuid         not null
        constraint "PK_Files"
            primary key,
    "Name"         varchar(255) not null,
    "Extension"    varchar(255),
    "TypeId"       integer      not null,
    "UploadDate"   timestamp    not null,
    "FileSenderId" uuid         not null
        constraint "FK_Files_FileSenders_FileSenderId"
            references "FileSenders"
            on delete cascade,
    "ChatId"       uuid
        constraint "FK_Files_Chats_ChatId"
            references "Chats"
            on delete restrict
);

alter table "Files"
    owner to "FileStorageApp";

create index "IX_Files_ChatId"
    on "Files" ("ChatId");

create index "IX_Files_FileSenderId"
    on "Files" ("FileSenderId");

create table "SenderAndChat"
(
    "ChatsId"   uuid not null
        constraint "FK_SenderAndChat_Chats_ChatsId"
            references "Chats"
            on delete cascade,
    "SendersId" uuid not null
        constraint "FK_SenderAndChat_FileSenders_SendersId"
            references "FileSenders"
            on delete cascade,
    constraint "PK_SenderAndChat"
        primary key ("ChatsId",
                     "SendersId")
);

alter table "SenderAndChat"
    owner to "FileStorageApp";

create index "IX_SenderAndChat_SendersId"
    on "SenderAndChat" ("SendersId");

create table "Rights"
(
    "Id"     uuid    not null
        constraint "PK_Rights"
            primary key,
    "UserId" uuid    not null
        constraint "FK_Rights_Users_UserId"
            references "Users"
            on delete cascade,
    "Access" integer not null
);

alter table "Rights"
    owner to "FileStorageApp";

create index "IX_Rights_UserId"
    on "Rights" ("UserId");

create table "MarkedTextTags"
(
    "Id"             uuid not null
        constraint "PK_MarkedTextTags"
            primary key,
    "TitleTag"       text not null,
    "DescriptionTag" text not null
);

alter table "MarkedTextTags"
    owner to "FileStorageApp";

create table "CodeForTelegramLoader"
(
    id   serial       not null
        constraint "CodeForTelegramLoader_pkey"
            primary key,
    code varchar(255) not null
);

alter table "CodeForTelegramLoader"
    owner to "FileStorageApp";

create table "DocumentClassifications"
(
    "Id"   uuid not null
        constraint "PK_DocumentClassifications"
            primary key,
    "Name" text not null
);

alter table "DocumentClassifications"
    owner to "FileStorageApp";

create unique index "IX_DocumentClassifications_Name"
    on "DocumentClassifications" ("Name");

create table "DocumentClassificationWords"
(
    "Id"               uuid not null
        constraint "PK_DocumentClassificationWords"
            primary key,
    "Value"            text not null,
    "ClassificationId" uuid not null
        constraint "FK_DocumentClassificationWords_DocumentClassifications_Classif~"
            references "DocumentClassifications"
            on delete cascade,
    constraint "AK_DocumentClassificationWords_ClassificationId_Value"
        unique ("ClassificationId", "Value")
);

alter table "DocumentClassificationWords"
    owner to "FileStorageApp";
