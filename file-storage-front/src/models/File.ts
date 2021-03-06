import {ClassificationType} from "./Classification";

export type Nullable<T> = T | null;

export enum Rights {
    "Загружать файлы" = 1,
    "Переименовывать файлы" = 2,
    "Удалять файлы" = 3,
    "Редактировать права пользователей" = 4,
    "Поиск классификаций" = 6,
    "Добавление классификаций" = 7,
    "Редактирование классификаций" = 8,
    "Удаление классификаций" = 9,
    "Присвоение классификаций" = 10,
    "Отзыв классификаций"= 11,
}

export interface ExpandingObject<T> {
    [key: string]: T
}

export enum ModalContent {
    Remove,
    Edit,
    AddClass,
}

export enum MessageTypeEnum {
    Error,
    Message
}

export type FormType<TValue> = { label: string, value: TValue };

export type TypeFile = {
    fileId: string,
    fileName: string,
    fileType: string,
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
    url?: string,
    classification?: ClassificationType |  null
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

export type MainPageFilters = {}