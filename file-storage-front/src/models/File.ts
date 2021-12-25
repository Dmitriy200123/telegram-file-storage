export enum Category {
    "документы",
    "аудио",
    "видео",
    "картинки",
}

export enum Rights {
    "Загружать файлы"=1,
    "Переименовывать файлы" = 2,
    "Удалять файлы" = 3,
    "Редактировать права пользователей" = 4,
}

export enum ModalContent {
    Remove,
    Edit
}

export enum MessageTypeEnum {
    Error,
    Message
}

export type FormType<TValue> = { label: string, value: TValue };

export type TypeFile = {
    fileId: string,
    fileName: string,
    fileType: Category,
    sender: {
        "id": string,
        "telegramUserName": string,
        "fullName": string
    },
    uploadDate: string,
    chat: {
        "id": string,
        "name": string,
        "imageId"?: string
    },
    downloadLink?: string
}

export type Chat = {
    id: string,
    name: string,
    imageId: string,
}

export type Sender = {
    id: string,
    fullName: string,
    telegramUserName: string,
}

export type TypePaginator = {
    count: number,
    currentPage: number,
    filesInPage: number
}


export type MessageType = {
    type: MessageTypeEnum,
    value: string
}

export type TokensType = {
    "jwtToken": string,
    "refreshToken": string
}